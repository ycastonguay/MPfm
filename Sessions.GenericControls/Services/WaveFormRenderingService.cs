// Copyright � 2011-2013 Yanick Castonguay
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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Graphics;
using Sessions.GenericControls.Services.Events;
using Sessions.GenericControls.Services.Interfaces;
using Sessions.GenericControls.Services.Objects;
using System.Threading;
using Sessions.Sound.AudioFiles;
using Sessions.Sound.PeakFiles;
using Sessions.Sound.PeakFiles.Interfaces;

namespace Sessions.GenericControls.Services
{
    public class WaveFormRenderingService : IWaveFormRenderingService
    {
        private readonly object _locker = new object();
        private readonly IPeakFileService _peakFileService;
        private readonly IPeakFileQueueService _peakFileQueueService;
        private readonly IMemoryGraphicsContextFactory _memoryGraphicsContextFactory;
        private AudioFile _audioFile;
        private BasicBrush _brushBackground;
        private BasicPen _penTransparent;
        private List<WaveDataMinMax> _waveDataCache = new List<WaveDataMinMax>();
		private BasicColor _colorBackground = new BasicColor(32, 40, 46);        
		private BasicColor _colorWaveForm = new BasicColor(255, 255, 64);

        public delegate void LoadPeakFileEventHandler(object sender, LoadPeakFileEventArgs e);
        public delegate void GeneratePeakFileEventHandler(object sender, GeneratePeakFileEventArgs e);
        public delegate void GenerateWaveFormEventHandler(object sender, GenerateWaveFormEventArgs e);

        public bool IsGeneratingPeakFile { get { return _peakFileQueueService.IsLoading; } }

        public event GeneratePeakFileEventHandler GeneratePeakFileBegunEvent;
        public event GeneratePeakFileEventHandler GeneratePeakFileProgressEvent;
        public event GeneratePeakFileEventHandler GeneratePeakFileEndedEvent;
        public event LoadPeakFileEventHandler LoadedPeakFileSuccessfullyEvent;
        public event GenerateWaveFormEventHandler GenerateWaveFormBitmapBegunEvent;
        public event GenerateWaveFormEventHandler GenerateWaveFormBitmapEndedEvent;

        public WaveFormRenderingService(IPeakFileService peakFileService, IPeakFileQueueService peakFileQueueService, IMemoryGraphicsContextFactory memoryGraphicsContextFactory)
        {
            _peakFileService = peakFileService;
            _peakFileQueueService = peakFileQueueService;
            _peakFileQueueService.OnGenerationStarted += HandleOnPeakFileProcessStarted;
            _peakFileQueueService.OnGenerationProgress += HandleOnPeakFileProcessData;
            _peakFileQueueService.OnGenerationFinished += HandleOnPeakFileProcessDone;

            _memoryGraphicsContextFactory = memoryGraphicsContextFactory;

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

        void HandleOnPeakFileProcessStarted(PeakFileGenerationStartedData data)
        {
            //Console.WriteLine("WaveFormRenderingService - HandleOnPeakFileProcessStarted");
            OnGeneratePeakFileBegun(new GeneratePeakFileEventArgs());
        }

        void HandleOnPeakFileProcessData(PeakFileGenerationProgressData data)
        {
            //Console.WriteLine("WaveFormRenderingService - HandleOnPeakFileProcessData");
            OnGeneratePeakFileProgress(new GeneratePeakFileEventArgs()
            {
                AudioFilePath = data.AudioFilePath,
                PercentageDone = data.PercentageDone
            });
        }

        void HandleOnPeakFileProcessDone(PeakFileGenerationFinishedData data)
        {
//            Console.WriteLine("WaveFormRenderingService - HandleOnPeakFileProcessDone - Cancelled: " + data.Cancelled.ToString());
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
//            Console.WriteLine("WaveFormRenderingService - FlushCache");
            lock (_locker)
            {
                _waveDataCache = null;
                _waveDataCache = new List<WaveDataMinMax>();
            }
        }

        public void LoadPeakFile(AudioFile audioFile)
        {
            Console.WriteLine("==========> [WaveFormRenderingService] - LoadPeakFile audioFile: " + audioFile.FilePath);
            _audioFile = audioFile;

            string peakFilePath = PeakFileService.GetPeakFilePathForAudioFileAndCreatePeakFileDirectory(audioFile);

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
                        Console.WriteLine("==========> [WaveFormRenderingService] - Reading peak file: " + peakFilePath);
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
                        Console.WriteLine("==========> [WaveFormRenderingService] - Peak file could not be loaded - Generating " + peakFilePath + "...");
                        OnGeneratePeakFileBegun(new GeneratePeakFileEventArgs());
                        _peakFileQueueService.GeneratePeakFile(audioFile.FilePath, peakFilePath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error generating peak file: " + ex.Message);
                    }
                    return null;
                }, TaskCreationOptions.LongRunning).ContinueWith(t =>
                {
                    Console.WriteLine("==========> [WaveFormRenderingService] - Read peak file over.");
                    var data = (List<WaveDataMinMax>)t.Result;
                    if (data == null)
                    {
                        Console.WriteLine("==========> [WaveFormRenderingService] - Could not load a peak file from disk (i.e. generating a new peak file).");
                        return;
                    }

                    Console.WriteLine("==========> [WaveFormRenderingService] - Adding wave data to cache...");
                    _waveDataCache = data;

					OnLoadedPeakFileSuccessfully(new LoadPeakFileEventArgs() { AudioFile = audioFile });
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                // Start generating peak file in background
                Console.WriteLine("==========> [WaveFormRenderingService] - Peak file doesn't exist - Generating " + peakFilePath + "...");
                _peakFileQueueService.GeneratePeakFile(audioFile.FilePath, peakFilePath);
            }
        }

        public void CancelPeakFile()
        {
            _peakFileQueueService.CancelAll();
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
//                    Console.WriteLine("Error initializing image cache context!");
					return;
                }
            }
            catch (Exception ex)
            {
//                Console.WriteLine("Error while creating image cache context: " + ex.Message);
                return;
            }

			IBasicImage imageCache;
			try
			{
			    float x = 0;
                float leftMin = 0;
                float leftMax = 0;
                float rightMin = 0;
                float rightMax = 0;
                float mixMin = 0;
                float mixMax = 0;
                int startHistoryLine = 0;
                int endHistoryLine = 0;
                int currentHistoryCount = 0;
                const float lineWidth = 0.25f;
                WaveDataMinMax[] subset = null;

                float lineWidthPerHistoryItem = request.BoundsWaveForm.Width / (float)_waveDataCache.Count;
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

                //context.DrawRectangle(new BasicRectangle(0, 0, request.BoundsBitmap.Width, request.BoundsBitmap.Height), new BasicBrush(new BasicColor(0, 0, 0)), new BasicPen(new BasicBrush(new BasicColor(255, 0, 0)), 2));
                //context.DrawText(string.Format("{0:0.0}", request.BoundsBitmap.X), new BasicPoint(1, request.BoundsBitmap.Height - 22), new BasicColor(255, 255, 255), "Roboto", 6 * context.Density);
                //context.DrawText(string.Format("{0:0.0}", request.Zoom), new BasicPoint(1, request.BoundsBitmap.Height - 10), new BasicColor(255, 255, 255), "Roboto", 6 * context.Density);

                //context.DrawText(string.Format("{0:0.0}", startLine), new BasicPoint(1, request.BoundsBitmap.Height - 10), new BasicColor(255, 255, 255), "Roboto Bold", 10 * context.Density);
                //context.DrawText(string.Format("{0:0.0}", lastLine), new BasicPoint(1, request.BoundsBitmap.Height - 22), new BasicColor(255, 255, 255), "Roboto Bold", 10 * context.Density);
                //context.DrawText(string.Format("{0:0.0}", request.BoundsWaveForm.Width), new BasicPoint(1, request.BoundsBitmap.Height - 46), new BasicColor(255, 255, 255), "Roboto Bold", 10 * context.Density);
                //context.DrawText(string.Format("{0}", request.BoundsBitmap.X), new BasicPoint(1, request.BoundsBitmap.Height - 20), new BasicColor(255, 255, 255), "Roboto Bold", 10 * context.Density);

                for (float plotX = request.BoundsBitmap.X; plotX < request.BoundsBitmap.X + request.BoundsBitmap.Width; plotX += lineWidth)
                {
                    startHistoryLine = (int)Math.Floor(plotX / lineWidthPerHistoryItem);
                    endHistoryLine = (int)Math.Ceiling((plotX + lineWidth) / lineWidthPerHistoryItem);
                    x = plotX - request.BoundsBitmap.X;

                    #if MACOSX
                    // On Mac, the pen needs to be set every time we draw or the color might change to black randomly (weird?)
                    context.SetPen(penWaveForm);
                    #endif

                    currentHistoryCount = endHistoryLine - startHistoryLine;
                    if(currentHistoryCount > 1)
                    {
                        subset = new WaveDataMinMax[currentHistoryCount];
                        _waveDataCache.CopyTo(startHistoryLine, subset, 0, currentHistoryCount);

                        leftMin = AudioTools.GetMinPeakFromWaveDataMaxHistory(subset.ToList(), currentHistoryCount, ChannelType.Left);
                        leftMax = AudioTools.GetMaxPeakFromWaveDataMaxHistory(subset.ToList(), currentHistoryCount, ChannelType.Left);
                        rightMin = AudioTools.GetMinPeakFromWaveDataMaxHistory(subset.ToList(), currentHistoryCount, ChannelType.Right);
                        rightMax = AudioTools.GetMaxPeakFromWaveDataMaxHistory(subset.ToList(), currentHistoryCount, ChannelType.Right);
                        mixMin = AudioTools.GetMinPeakFromWaveDataMaxHistory(subset.ToList(), currentHistoryCount, ChannelType.Mix);
                        mixMax = AudioTools.GetMaxPeakFromWaveDataMaxHistory(subset.ToList(), currentHistoryCount, ChannelType.Mix);
                    }
                    else
                    {
                        leftMin = _waveDataCache[startHistoryLine].leftMin;
                        leftMax = _waveDataCache[startHistoryLine].leftMax;
                        rightMin = _waveDataCache[startHistoryLine].rightMin;
                        rightMax = _waveDataCache[startHistoryLine].rightMax;
                        mixMin = _waveDataCache[startHistoryLine].mixMin;
                        mixMax = _waveDataCache[startHistoryLine].mixMax;
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
						context.StrokeLine(new BasicPoint(x, heightToRenderLine), new BasicPoint(x, heightToRenderLine - maxLineHeight));
                        // Negative Max Value - Draw negative value (y: middle to height)
						context.StrokeLine(new BasicPoint(x, heightToRenderLine), new BasicPoint(x, heightToRenderLine + (-minLineHeight)));
                    }
                    else if (request.DisplayType == WaveFormDisplayType.Stereo)
                    {
                        // LEFT Channel - Positive Max Value - Draw positive value (y: middle to top)
                        context.StrokeLine(new BasicPoint(x, heightToRenderLine), new BasicPoint(x, heightToRenderLine - leftMaxHeight));
                        // LEFT Channel - Negative Max Value - Draw negative value (y: middle to height)
                        context.StrokeLine(new BasicPoint(x, heightToRenderLine), new BasicPoint(x, heightToRenderLine + (-leftMinHeight)));
                        // RIGHT Channel - Positive Max Value (Multiply by 3 to get the new center line for right channel) - Draw positive value (y: middle to top)
                        context.StrokeLine(new BasicPoint(x, (heightToRenderLine * 3)), new BasicPoint(x, (heightToRenderLine * 3) - rightMaxHeight));
                        // RIGHT Channel - Negative Max Value - Draw negative value (y: middle to height)
                        context.StrokeLine(new BasicPoint(x, (heightToRenderLine * 3)), new BasicPoint(x, (heightToRenderLine * 3) + (-rightMinHeight)));
                    }
                }
            }
            catch (Exception ex)
            {
//                Console.WriteLine("RequestBitmapInternal - Error while creating image cache: " + ex.Message);
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
