// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Graphics;
using Sessions.GenericControls.Services.Events;
using Sessions.GenericControls.Services.Interfaces;
using Sessions.GenericControls.Services.Objects;
using System.Threading;
using Sessions.Core;
using Sessions.Sound.AudioFiles;
using Sessions.Sound.PeakFiles;

namespace Sessions.GenericControls.Services
{
    public class WaveFormRenderingService : IWaveFormRenderingService
    {
        private readonly object _locker = new object();
        private readonly IPeakFileService _peakFileService;
        private readonly IMemoryGraphicsContextFactory _memoryGraphicsContextFactory;
        private AudioFile _audioFile;
        private BasicBrush _brushBackground;
        private BasicPen _penTransparent;
        private List<WaveDataMinMax> _waveDataCache = new List<WaveDataMinMax>();
		private BasicColor _colorBackground = new BasicColor(32, 40, 46);        
		private BasicColor _colorWaveForm = new BasicColor(255, 255, 64);
        private float _padding = 0;

        public delegate void LoadPeakFileEventHandler(object sender, LoadPeakFileEventArgs e);
        public delegate void GeneratePeakFileEventHandler(object sender, GeneratePeakFileEventArgs e);
        public delegate void GenerateWaveFormEventHandler(object sender, GenerateWaveFormEventArgs e);

        public event GeneratePeakFileEventHandler GeneratePeakFileBegunEvent;
        public event GeneratePeakFileEventHandler GeneratePeakFileProgressEvent;
        public event GeneratePeakFileEventHandler GeneratePeakFileEndedEvent;
        public event LoadPeakFileEventHandler LoadedPeakFileSuccessfullyEvent;
        public event GenerateWaveFormEventHandler GenerateWaveFormBitmapBegunEvent;
        public event GenerateWaveFormEventHandler GenerateWaveFormBitmapEndedEvent;

        public WaveFormRenderingService(IPeakFileService peakFileService, IMemoryGraphicsContextFactory memoryGraphicsContextFactory)
        {
            _peakFileService = peakFileService;
            _memoryGraphicsContextFactory = memoryGraphicsContextFactory;
            _peakFileService.OnProcessStarted += HandleOnPeakFileProcessStarted;
            _peakFileService.OnProcessData += HandleOnPeakFileProcessData;
            _peakFileService.OnProcessDone += HandleOnPeakFileProcessDone;

            CreateDrawingResources();
        }

        private void CreateDrawingResources()
        {
            _penTransparent = new BasicPen();
            _brushBackground = new BasicBrush(_colorBackground);
        }

        protected virtual void OnGeneratePeakFileBegun(GeneratePeakFileEventArgs e)
        {
            if (GeneratePeakFileBegunEvent != null)
                GeneratePeakFileBegunEvent(this, e);
        }

        protected virtual void OnGeneratePeakFileProgress(GeneratePeakFileEventArgs e)
        {
            if (GeneratePeakFileProgressEvent != null)
                GeneratePeakFileProgressEvent(this, e);
        }

        protected virtual void OnGeneratePeakFileEnded(GeneratePeakFileEventArgs e)
        {
            if (GeneratePeakFileEndedEvent != null)
                GeneratePeakFileEndedEvent(this, e);
        }

        protected virtual void OnLoadedPeakFileSuccessfully(LoadPeakFileEventArgs e)
        {
            if (LoadedPeakFileSuccessfullyEvent != null)
                LoadedPeakFileSuccessfullyEvent(this, e);
        }

        protected virtual void OnGenerateWaveFormBitmapBegun(GenerateWaveFormEventArgs e)
        {
            if (GenerateWaveFormBitmapBegunEvent != null)
                GenerateWaveFormBitmapBegunEvent(this, e);
        }

        protected virtual void OnGenerateWaveFormBitmapEnded(GenerateWaveFormEventArgs e)
        {
            if (GenerateWaveFormBitmapEndedEvent != null)
                GenerateWaveFormBitmapEndedEvent(this, e);
        }

        void HandleOnPeakFileProcessStarted(PeakFileStartedData data)
        {
            //Console.WriteLine("WaveFormRenderingService - HandleOnPeakFileProcessStarted");
            OnGeneratePeakFileBegun(new GeneratePeakFileEventArgs());
        }

        void HandleOnPeakFileProcessData(PeakFileProgressData data)
        {
            //Console.WriteLine("WaveFormRenderingService - HandleOnPeakFileProcessData");
            OnGeneratePeakFileProgress(new GeneratePeakFileEventArgs()
            {
                AudioFilePath = data.AudioFilePath,
                PercentageDone = data.PercentageDone
            });
        }

        void HandleOnPeakFileProcessDone(PeakFileDoneData data)
        {
            Console.WriteLine("WaveFormRenderingService - HandleOnPeakFileProcessDone - Cancelled: " + data.Cancelled.ToString());
            if (!data.Cancelled)
            {
                // Load the new peak file from disk
                FlushCache();
                LoadPeakFile(_audioFile);
            }

            OnGeneratePeakFileEnded(new GeneratePeakFileEventArgs()
            {
                AudioFilePath = data.AudioFilePath,
                PercentageDone = 100,
                Cancelled = data.Cancelled
            });
        }

        public void FlushCache()
        {
            Console.WriteLine("WaveFormRenderingService - FlushCache");
            lock (_locker)
            {
                _waveDataCache = null;
                _waveDataCache = new List<WaveDataMinMax>();
            }
        }

        public void LoadPeakFile(AudioFile audioFile)
        {
            // Check if another peak file is already loading
            Console.WriteLine("WaveFormRenderingService - LoadPeakFile audioFile: " + audioFile.FilePath);
            _audioFile = audioFile;
            if (_peakFileService.IsLoading)
            {
                //Console.WriteLine("WaveFormRenderingService - Cancelling current peak file generation...");
                _peakFileService.Cancel();
            }

            // Check if the peak file subfolder exists
            string peakFileFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PeakFiles");
            if (!Directory.Exists(peakFileFolder))
            {
                try
                {
                    //Console.WriteLine("WaveFormRenderingService - Creating folder " + peakFileFolder + "...");
                    Directory.CreateDirectory(peakFileFolder);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("WaveFormRenderingService - Failed to create folder: " + ex.Message);
                    return;
                }
            }

            // Generate peak file path
            string peakFilePath = Path.Combine(peakFileFolder, Normalizer.NormalizeStringForUrl(audioFile.ArtistName + "_" + audioFile.AlbumTitle + "_" + audioFile.Title + "_" + audioFile.FileType.ToString()) + ".peak");

            // Check if peak file exists
            if (File.Exists(peakFilePath))
            {
                // TODO: This needs to be done in another thread with new Thread(), long running thread sucks
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
                Task<List<WaveDataMinMax>>.Factory.StartNew(() =>
                {
                    List<WaveDataMinMax> data = null;
                    FlushCache();
                    try
                    {
                        Console.WriteLine("WaveFormRenderingService - Reading peak file: " + peakFilePath);
                        data = _peakFileService.ReadPeakFile(peakFilePath);
                        if (data != null)
                            return data;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error reading peak file: " + ex.Message);
                    }

                    try
                    {
                        Console.WriteLine("WaveFormRenderingService - Peak file could not be loaded - Generating " + peakFilePath + "...");
                        OnGeneratePeakFileBegun(new GeneratePeakFileEventArgs());
                        _peakFileService.GeneratePeakFile(audioFile.FilePath, peakFilePath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error generating peak file: " + ex.Message);
                    }
                    return null;
                }, TaskCreationOptions.LongRunning).ContinueWith(t =>
                {
                    Console.WriteLine("WaveFormRenderingService - Read peak file over.");
                    var data = (List<WaveDataMinMax>)t.Result;
                    if (data == null)
                    {
                        //Console.WriteLine("WaveFormRenderingService - Could not load a peak file from disk (i.e. generating a new peak file).");
                        return;
                    }

                    Console.WriteLine("WaveFormRenderingService - Adding wave data to cache...");
                    _waveDataCache = data;

					OnLoadedPeakFileSuccessfully(new LoadPeakFileEventArgs() { AudioFile = audioFile });
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                // Start generating peak file in background
                Console.WriteLine("Peak file doesn't exist - Generating " + peakFilePath + "...");
                _peakFileService.GeneratePeakFile(audioFile.FilePath, peakFilePath);
            }
        }

        /// <summary>
        /// Requests a wave form bitmap to be generated. Once the bitmap is generated, the OnGenerateWaveFormBitmapEnded event is fired.
        /// </summary>
        public void RequestBitmap(WaveFormBitmapRequest request)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(RequestBitmapInternal), request);
        }

        private void RequestBitmapInternal(Object stateInfo)
        {
            // Use this instead of a task, this guarantees to execute in another thread
            //Console.WriteLine("WaveFormRenderingService - RequestBitmap - boundsBitmap: {0} boundsWaveForm: {1} zoom: {2}", boundsBitmap, boundsWaveForm, zoom);
            var request = stateInfo as WaveFormBitmapRequest;
            IMemoryGraphicsContext context;
            try
            {
                //Console.WriteLine("WaveFormRenderingService - Creating image cache...");
                context = _memoryGraphicsContextFactory.CreateMemoryGraphicsContext(request.BoundsBitmap.Width, request.BoundsBitmap.Height);
                if (context == null)
                {
                    Console.WriteLine("Error initializing image cache context!");
					return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while creating image cache context: " + ex.Message);
                return;
            }

			IBasicImage imageCache;
			try
			{
			    float x1 = 0;
                float x2 = 0;
                float leftMin = 0;
                float leftMax = 0;
                float rightMin = 0;
                float rightMax = 0;
                float mixMin = 0;
                float mixMax = 0;
                int historyIndex = 0;
                int historyCount = _waveDataCache.Count;
                float lineWidth = 0;
                int nHistoryItemsPerLine = 0;
                const float desiredLineWidth = 0.5f;
                WaveDataMinMax[] subset = null;

                // Find out how many samples are represented by each line of the wave form, depending on its width.
                // For example, if the history has 45000 items, and the control has a width of 1000px, 45 items will need to be averaged by line.
                float lineWidthPerHistoryItem = request.BoundsWaveForm.Width / (float)historyCount;

                // Check if the line width is below the desired line width
                if (lineWidthPerHistoryItem < desiredLineWidth)
                {
                    // Try to get a line width around 0.5f so the precision is good enough and no artifacts will be shown.
                    while (lineWidth < desiredLineWidth)
                    {
                        // Increment the number of history items per line
                        //Console.WriteLine("Determining line width (lineWidth: " + lineWidth.ToString() + " desiredLineWidth: " + desiredLineWidth.ToString() + " nHistoryItemsPerLine: " + nHistoryItemsPerLine.ToString() + " lineWidthPerHistoryItem: " + lineWidthPerHistoryItem.ToString());
                        nHistoryItemsPerLine++;
                        lineWidth += lineWidthPerHistoryItem;
                    }
                    nHistoryItemsPerLine--;
                    lineWidth -= lineWidthPerHistoryItem;
                }
                else
                {
                    // The lines are larger than 0.5 pixels.
                    lineWidth = lineWidthPerHistoryItem;
                    nHistoryItemsPerLine = 1;
                }

                float heightToRenderLine = 0;
                if (request.DisplayType == WaveFormDisplayType.Stereo)
                    heightToRenderLine = (request.BoundsWaveForm.Height / 4);
                else
                    heightToRenderLine = (request.BoundsWaveForm.Height / 2);

                context.DrawRectangle(new BasicRectangle(0, 0, request.BoundsBitmap.Width + 2, request.BoundsBitmap.Height), _brushBackground, _penTransparent);

                // The pen cannot be cached between refreshes because the line width changes every time the width changes
                //context.SetLineWidth(0.2f);
                var penWaveForm = new BasicPen(new BasicBrush(_colorWaveForm), lineWidth);
                context.SetPen(penWaveForm);

                float startLine = ((int)Math.Floor(request.BoundsBitmap.X / lineWidth)) * lineWidth;
                historyIndex = (int) ((startLine / lineWidth) * nHistoryItemsPerLine);

                //Console.WriteLine("WaveFormRenderingService - startLine: {0} boundsWaveForm.Width: {1} nHistoryItemsPerLine: {2} historyIndex: {3}", startLine, boundsWaveForm.Width, nHistoryItemsPerLine, historyIndex);
                //List<float> roundValues = new List<float>();
                float lastLine = startLine + request.BoundsBitmap.Width;
                if (request.BoundsWaveForm.Width - request.BoundsBitmap.X < request.BoundsBitmap.Width)
                    lastLine = request.BoundsWaveForm.Width;

                // Make sure we don't end up in a value with decimals
                startLine = (float)Math.Ceiling(startLine); // should it be Floor?
                lastLine = (float)Math.Ceiling(lastLine);

//                context.DrawRectangle(new BasicRectangle(0, 0, request.BoundsBitmap.Width, request.BoundsBitmap.Height), new BasicBrush(new BasicColor(0, 0, 0)), new BasicPen(new BasicBrush(new BasicColor(255, 0, 0)), 2));
//                context.DrawText(string.Format("{0:0.0}", request.BoundsBitmap.X), new BasicPoint(1, request.BoundsBitmap.Height - 22), new BasicColor(255, 255, 255), "Roboto", 11 * context.Density);
//                context.DrawText(string.Format("{0:0.0}", request.Zoom), new BasicPoint(1, request.BoundsBitmap.Height - 10), new BasicColor(255, 255, 255), "Roboto", 11 * context.Density);

                //context.DrawText(string.Format("{0:0.0}", startLine), new BasicPoint(1, request.BoundsBitmap.Height - 10), new BasicColor(255, 255, 255), "Roboto Bold", 10 * context.Density);
                //context.DrawText(string.Format("{0:0.0}", lastLine), new BasicPoint(1, request.BoundsBitmap.Height - 22), new BasicColor(255, 255, 255), "Roboto Bold", 10 * context.Density);
                //context.DrawText(string.Format("{0:0.0}", request.BoundsWaveForm.Width), new BasicPoint(1, request.BoundsBitmap.Height - 46), new BasicColor(255, 255, 255), "Roboto Bold", 10 * context.Density);
                //context.DrawText(string.Format("{0}", request.BoundsBitmap.X), new BasicPoint(1, request.BoundsBitmap.Height - 20), new BasicColor(255, 255, 255), "Roboto Bold", 10 * context.Density);

                for (float i = startLine; i < lastLine; i += lineWidth)
                {
                    #if MACOSX
                    // On Mac, the pen needs to be set every time we draw or the color might change to black randomly (weird?)
                    context.SetPen(penWaveForm);
                    #endif

                    // COMMENTED possible solution for varying line widths rendering problem
                    // Round to 0.5
                    //i = (float)Math.Round(i * 2) / 2;
                    //float iRound = (float)Math.Round(i);
                    //float iRound = (float)Math.Round(i * 2) / 2;
                    //float iRound = (float)Math.Round(i * 4) / 4;

                    //                        // If this value has already been drawn, skip it (this happens because of the rounding, and this fixes a visual bug)
                    //                        if(roundValues.Contains(iRound))
                    //                        {
                    //                            // Increment the history index; pad the last values if the count is about to exceed
                    //                            if (historyIndex < historyCount - 1)
                    //                                historyIndex += nHistoryItemsPerLine;                         
                    //                            continue;
                    //                        }
                    //                        else
                    //                        {
                    //                            roundValues.Add(iRound);
                    //                        }

                    // Determine the maximum height of a line (+/-)
                    //Console.WriteLine("WaveForm - Rendering " + i.ToString() + " (rnd=" + iRound.ToString() + ") on " + widthAvailable.ToString());

                    // Determine x position
                    //                        x1 = iRound; //i;
                    //                        x2 = iRound; //i;
                    x1 = i - startLine;
                    x2 = i - startLine;
                    if (nHistoryItemsPerLine > 1)
                    {
                        if (historyIndex + nHistoryItemsPerLine > historyCount)
                        {
                            // Create subset with remaining data
                            subset = new WaveDataMinMax[historyCount - historyIndex];
                            _waveDataCache.CopyTo(historyIndex, subset, 0, historyCount - historyIndex);
                        }
                        else
                        {
                            subset = new WaveDataMinMax[nHistoryItemsPerLine];
                            _waveDataCache.CopyTo(historyIndex, subset, 0, nHistoryItemsPerLine);
                        }

                        leftMin = AudioTools.GetMinPeakFromWaveDataMaxHistory(subset.ToList(), nHistoryItemsPerLine, ChannelType.Left);
                        leftMax = AudioTools.GetMaxPeakFromWaveDataMaxHistory(subset.ToList(), nHistoryItemsPerLine, ChannelType.Left);
                        rightMin = AudioTools.GetMinPeakFromWaveDataMaxHistory(subset.ToList(), nHistoryItemsPerLine, ChannelType.Right);
                        rightMax = AudioTools.GetMaxPeakFromWaveDataMaxHistory(subset.ToList(), nHistoryItemsPerLine, ChannelType.Right);
                        mixMin = AudioTools.GetMinPeakFromWaveDataMaxHistory(subset.ToList(), nHistoryItemsPerLine, ChannelType.Mix);
                        mixMax = AudioTools.GetMaxPeakFromWaveDataMaxHistory(subset.ToList(), nHistoryItemsPerLine, ChannelType.Mix);
                    }
                    else
                    {
                        leftMin = _waveDataCache[historyIndex].leftMin;
                        leftMax = _waveDataCache[historyIndex].leftMax;
                        rightMin = _waveDataCache[historyIndex].rightMin;
                        rightMax = _waveDataCache[historyIndex].rightMax;
                        mixMin = _waveDataCache[historyIndex].mixMin;
                        mixMax = _waveDataCache[historyIndex].mixMax;
                    }

                    float leftMaxHeight = leftMax * heightToRenderLine;
                    float leftMinHeight = leftMin * heightToRenderLine;
                    float rightMaxHeight = rightMax * heightToRenderLine;
                    float rightMinHeight = rightMin * heightToRenderLine;
                    float mixMaxHeight = mixMax * heightToRenderLine;
                    float mixMinHeight = mixMin * heightToRenderLine;

                    //Console.WriteLine("WaveFormRenderingService - line: {0} x1: {1} x2: {2} historyIndex: {3} historyCount: {4} width: {5}", i, x1, x2, historyIndex, historyCount, boundsWaveForm.Width);
                    if (request.DisplayType == WaveFormDisplayType.LeftChannel ||
                        request.DisplayType == WaveFormDisplayType.RightChannel ||
                        request.DisplayType == WaveFormDisplayType.Mix)
                    {
                        // Calculate min/max line height
                        float minLineHeight = 0;
                        float maxLineHeight = 0;

                        // Set mib/max
                        if (request.DisplayType == WaveFormDisplayType.LeftChannel)
                        {
                            minLineHeight = leftMinHeight;
                            maxLineHeight = leftMaxHeight;
                        }
                        else if (request.DisplayType == WaveFormDisplayType.RightChannel)
                        {
                            minLineHeight = rightMinHeight;
                            maxLineHeight = rightMaxHeight;
                        }
                        else if (request.DisplayType == WaveFormDisplayType.Mix)
                        {
                            minLineHeight = mixMinHeight;
                            maxLineHeight = mixMaxHeight;
                        }

                        // Positive Max Value - Draw positive value (y: middle to top)                   
						context.StrokeLine(new BasicPoint(x1, heightToRenderLine), new BasicPoint(x2, heightToRenderLine - maxLineHeight));
                        // Negative Max Value - Draw negative value (y: middle to height)
						context.StrokeLine(new BasicPoint(x1, heightToRenderLine), new BasicPoint(x2, heightToRenderLine + (-minLineHeight)));
                    }
                    else if (request.DisplayType == WaveFormDisplayType.Stereo)
                    {
                        // LEFT Channel - Positive Max Value - Draw positive value (y: middle to top)
                        context.StrokeLine(new BasicPoint(x1, heightToRenderLine), new BasicPoint(x2, heightToRenderLine - leftMaxHeight));
                        // LEFT Channel - Negative Max Value - Draw negative value (y: middle to height)
                        context.StrokeLine(new BasicPoint(x1, heightToRenderLine), new BasicPoint(x2, heightToRenderLine + (-leftMinHeight)));
                        // RIGHT Channel - Positive Max Value (Multiply by 3 to get the new center line for right channel) - Draw positive value (y: middle to top)
                        context.StrokeLine(new BasicPoint(x1, (heightToRenderLine * 3)), new BasicPoint(x2, (heightToRenderLine * 3) - rightMaxHeight));
                        // RIGHT Channel - Negative Max Value - Draw negative value (y: middle to height)
                        context.StrokeLine(new BasicPoint(x1, (heightToRenderLine * 3)), new BasicPoint(x2, (heightToRenderLine * 3) + (-rightMinHeight)));
                    }

                    // Increment the history index; pad the last values if the count is about to exceed
                    if (historyIndex < historyCount - 1)
                        historyIndex += nHistoryItemsPerLine;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while creating image cache: " + ex.Message);
            }
            finally
            {
                // Get image from context (at this point, we are sure the image context has been initialized properly)
				//Console.WriteLine("WaveFormRenderingService - Rendering image to memory...");
                context.Close();
                imageCache = context.RenderToImageInMemory();
            }

			//Console.WriteLine("WaveFormRenderingService - Created image successfully.");
            //stopwatch.Stop();
            //Console.WriteLine("WaveFormRenderingService - Created image successfully in {0} ms.", stopwatch.ElapsedMilliseconds);
			OnGenerateWaveFormBitmapEnded(new GenerateWaveFormEventArgs()
			{
				//AudioFilePath = audioFile.FilePath,
                OffsetX = request.BoundsBitmap.X,
				Zoom = request.Zoom,
                Width = context.BoundsWidth,
				DisplayType = request.DisplayType,
				Image = imageCache,
                IsScrollBar = request.IsScrollBar
			});
        }
    }

    /// <summary>
    /// Defines the wave form display type.
    /// </summary>
    public enum WaveFormDisplayType
    {
        /// <summary>
        /// Left channel.
        /// </summary>
        LeftChannel = 0,
        /// <summary>
        /// Right channel.
        /// </summary>
        RightChannel = 1,
        /// <summary>
        /// Stereo (left and right channels).
        /// </summary>
        Stereo = 2,
        /// <summary>
        /// Mix (mix of left/right channels).
        /// </summary>
        Mix = 3
    }
}
