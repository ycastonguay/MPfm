// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of MPfm.
//
// MPfm is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// MPfm is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with MPfm. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MPfm.Core;
using MPfm.GenericControls.Basics;
using MPfm.GenericControls.Graphics;
using MPfm.GenericControls.Managers.Events;
using MPfm.Sound.AudioFiles;
using MPfm.Sound.PeakFiles;

namespace MPfm.GenericControls.Managers
{
    /// <summary>
    /// Manager for caching wave form bitmaps.
    /// </summary>
    public class WaveFormCacheManager
    {
        private readonly IPeakFileService _peakFileService;
        private readonly IMemoryGraphicsContextFactory _memoryGraphicsContextFactory;
        private Dictionary<string, List<WaveDataMinMax>> _waveDataCache = new Dictionary<string, List<WaveDataMinMax>>();
        private Dictionary<Tuple<string, WaveFormDisplayType, float>, object> _bitmapCache = new Dictionary<Tuple<string, WaveFormDisplayType, float>, object>();
        private BasicColor _colorBackground = new BasicColor(255, 0, 0);
        private BasicColor _colorWaveForm = new BasicColor(255, 0, 255);
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

        public WaveFormCacheManager(IPeakFileService peakFileService, IMemoryGraphicsContextFactory memoryGraphicsContextFactory)
        {
            _peakFileService = peakFileService;
            _memoryGraphicsContextFactory = memoryGraphicsContextFactory;
            _peakFileService.OnProcessStarted += HandleOnPeakFileProcessStarted;
            _peakFileService.OnProcessData += HandleOnPeakFileProcessData;
            _peakFileService.OnProcessDone += HandleOnPeakFileProcessDone;
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
            //Console.WriteLine("WaveFormCacheManager - HandleOnPeakFileProcessStarted");
            OnGeneratePeakFileBegun(new GeneratePeakFileEventArgs());
        }

        void HandleOnPeakFileProcessData(PeakFileProgressData data)
        {
            //Console.WriteLine("WaveFormCacheManager - HandleOnPeakFileProcessData");
            OnGeneratePeakFileProgress(new GeneratePeakFileEventArgs()
            {
                AudioFilePath = data.AudioFilePath,
                PercentageDone = data.PercentageDone
            });
        }

        void HandleOnPeakFileProcessDone(PeakFileDoneData data)
        {
            //Console.WriteLine("WaveFormCacheManager - HandleOnPeakFileProcessDone - Cancelled: " + data.Cancelled.ToString());
            OnGeneratePeakFileEnded(new GeneratePeakFileEventArgs()
            {
                AudioFilePath = data.AudioFilePath,
                PercentageDone = 100,
                Cancelled = data.Cancelled
            });
        }

        public void FlushCache()
        {
            //Console.WriteLine("WaveFormCacheManager - FlushCache");
            _waveDataCache = null;
            _waveDataCache = new Dictionary<string, List<WaveDataMinMax>>();
            _bitmapCache = null;
            _bitmapCache = new Dictionary<Tuple<string, WaveFormDisplayType, float>, object>(); ;
        }

        public void LoadPeakFile(AudioFile audioFile)
        {
            // Check if another peak file is already loading
            //Console.WriteLine("WaveFormCacheManager - LoadPeakFile audioFile: " + audioFile.FilePath);
            if (_peakFileService.IsLoading)
            {
                //Console.WriteLine("WaveFormCacheManager - Cancelling current peak file generation...");
                _peakFileService.Cancel();
            }

            // Check if the peak file subfolder exists
            string peakFileFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PeakFiles");
            if (!Directory.Exists(peakFileFolder))
            {
                try
                {
                    //Console.WriteLine("WaveFormCacheManager - Creating folder " + peakFileFolder + "...");
                    Directory.CreateDirectory(peakFileFolder);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("WaveFormCacheManager - Failed to create folder: " + ex.Message);
                    return;
                }
            }

            // Generate peak file path
            string peakFilePath = Path.Combine(peakFileFolder, Normalizer.NormalizeStringForUrl(audioFile.ArtistName + "_" + audioFile.AlbumTitle + "_" + audioFile.Title + "_" + audioFile.FileType.ToString()) + ".peak");

            // Check if peak file exists
            if (File.Exists(peakFilePath))
            {
                Task<List<WaveDataMinMax>>.Factory.StartNew(() =>
                {
                    List<WaveDataMinMax> data = null;

                    // TODO: Flush cache less often. For now, flush cache every time we load a new peak file to save memory.
                    FlushCache();

                    try
                    {
                        //Console.WriteLine("WaveFormCacheManager - Reading peak file: " + peakFilePath);
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
                        //Console.WriteLine("WaveFormCacheManager - Peak file could not be loaded - Generating " + peakFilePath + "...");
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
                    //Console.WriteLine("WaveFormCacheManager - Read peak file over.");
                    List<WaveDataMinMax> data = (List<WaveDataMinMax>)t.Result;
                    if (data == null)
                    {
                        //Console.WriteLine("WaveFormCacheManager - Could not load a peak file from disk (i.e. generating a new peak file).");
                        return;
                    }

                    //Console.WriteLine("WaveFormCacheManager - Adding wave data to cache...");
                    if (!_waveDataCache.ContainsKey(audioFile.FilePath))
                        _waveDataCache.Add(audioFile.FilePath, data);

                    OnLoadedPeakFileSuccessfully(new LoadPeakFileEventArgs()
                    {
                        AudioFile = audioFile
                    });

                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                // Start generating peak file in background
                Console.WriteLine("Peak file doesn't exist - Generating " + peakFilePath + "...");
                _peakFileService.GeneratePeakFile(audioFile.FilePath, peakFilePath);
            }
        }

        public void RequestBitmap(AudioFile audioFile, WaveFormDisplayType displayType, BasicRectangle bounds, float zoom, long audioFileLength)
        {
            object imageCache;
            var boundsWaveForm = new BasicRectangle();

            // key = FilePath + DisplayType + Zoom

            // Calculate available size
            int widthAvailable = (int)bounds.Width;
            int heightAvailable = (int)bounds.Height;
            if (zoom > 1)
            {
                widthAvailable = (int)(bounds.Width * zoom);
            }
            boundsWaveForm = new BasicRectangle(0, 0, widthAvailable - (_padding * 2), heightAvailable - (_padding * 2));

            Task<object>.Factory.StartNew(() =>
            {
                IMemoryGraphicsContext context;
                try
                {
                    Console.WriteLine("WaveFormCacheManager - Creating image cache...");
                    context = _memoryGraphicsContextFactory.CreateMemoryGraphicsContext(bounds.Width, bounds.Height);
                    if (context == null)
                    {
                        Console.WriteLine("Error initializing image cache context!");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while creating image cache context: " + ex.Message);
                    return null;
                }

                try
                {
                    // Draw gradient background
                    //CoreGraphicsHelper.FillGradient(context, new RectangleF(0, 0, bounds.Width + 2, bounds.Height), _colorGradient1, _colorGradient2);
                    context.DrawRectangle(new BasicRectangle(0, 0, bounds.Width + 2, bounds.Height), new BasicBrush(_colorBackground), new BasicPen());

                    // Declare variables
                    float x1 = 0;
                    float x2 = 0;
                    float leftMin = 0;
                    float leftMax = 0;
                    float rightMin = 0;
                    float rightMax = 0;
                    float mixMin = 0;
                    float mixMax = 0;
                    float leftMaxHeight = 0;
                    float leftMinHeight = 0;
                    float rightMaxHeight = 0;
                    float rightMinHeight = 0;
                    float mixMaxHeight = 0;
                    float mixMinHeight = 0;
                    int historyIndex = 0;
                    int historyCount = 0;
                    float lineWidth = 0;
                    float lineWidthPerHistoryItem = 0;
                    int nHistoryItemsPerLine = 0;
                    float desiredLineWidth = 0.5f;
                    WaveDataMinMax[] subset = null;

                    historyCount = _waveDataCache[audioFile.FilePath].Count;

                    // Find out how many samples are represented by each line of the wave form, depending on its width.
                    // For example, if the history has 45000 items, and the control has a width of 1000px, 45 items will need to be averaged by line.
                    lineWidthPerHistoryItem = boundsWaveForm.Width / (float)historyCount;

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
                    //Console.WriteLine("WaveFormView - historyItemsPerLine: " + nHistoryItemsPerLine.ToString());

                    float heightToRenderLine = 0;
                    if (displayType == WaveFormDisplayType.Stereo)
                        heightToRenderLine = (boundsWaveForm.Height / 4);
                    else
                        heightToRenderLine = (boundsWaveForm.Height / 2);

                    var pen = new BasicPen(new BasicBrush(_colorWaveForm), lineWidth);
                    //context.SetStrokeColor(GlobalTheme.WaveFormColor.CGColor);
                    //context.SetLineWidth(0.2f);
                    //context.SetLineWidth(lineWidth);

                    List<float> roundValues = new List<float>();
                    for (float i = 0; i < boundsWaveForm.Width; i += lineWidth)
                    {
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
                        x1 = i;
                        x2 = i;

                        if (nHistoryItemsPerLine > 1)
                        {
                            if (historyIndex + nHistoryItemsPerLine > historyCount)
                            {
                                // Create subset with remaining data
                                subset = new WaveDataMinMax[historyCount - historyIndex];
                                _waveDataCache[audioFile.FilePath].CopyTo(historyIndex, subset, 0, historyCount - historyIndex);
                            }
                            else
                            {
                                subset = new WaveDataMinMax[nHistoryItemsPerLine];
                                _waveDataCache[audioFile.FilePath].CopyTo(historyIndex, subset, 0, nHistoryItemsPerLine);
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
                            leftMin = _waveDataCache[audioFile.FilePath][historyIndex].leftMin;
                            leftMax = _waveDataCache[audioFile.FilePath][historyIndex].leftMax;
                            rightMin = _waveDataCache[audioFile.FilePath][historyIndex].rightMin;
                            rightMax = _waveDataCache[audioFile.FilePath][historyIndex].rightMax;
                            mixMin = _waveDataCache[audioFile.FilePath][historyIndex].mixMin;
                            mixMax = _waveDataCache[audioFile.FilePath][historyIndex].mixMax;
                        }

                        leftMaxHeight = leftMax * heightToRenderLine;
                        leftMinHeight = leftMin * heightToRenderLine;
                        rightMaxHeight = rightMax * heightToRenderLine;
                        rightMinHeight = rightMin * heightToRenderLine;
                        mixMaxHeight = mixMax * heightToRenderLine;
                        mixMinHeight = mixMin * heightToRenderLine;

                        if (displayType == WaveFormDisplayType.LeftChannel ||
                            displayType == WaveFormDisplayType.RightChannel ||
                            displayType == WaveFormDisplayType.Mix)
                        {
                            // Calculate min/max line height
                            float minLineHeight = 0;
                            float maxLineHeight = 0;

                            // Set mib/max
                            if (displayType == WaveFormDisplayType.LeftChannel)
                            {
                                minLineHeight = leftMinHeight;
                                maxLineHeight = leftMaxHeight;
                            }
                            else if (displayType == WaveFormDisplayType.RightChannel)
                            {
                                minLineHeight = rightMinHeight;
                                maxLineHeight = rightMaxHeight;
                            }
                            else if (displayType == WaveFormDisplayType.Mix)
                            {
                                minLineHeight = mixMinHeight;
                                maxLineHeight = mixMaxHeight;
                            }

                            // ------------------------
                            // Positive Max Value                   

                            // Draw positive value (y: middle to top)                   

                            //context.StrokeLineSegments(new PointF[2] {
                            //    new PointF(x1, heightToRenderLine), new PointF(x2, heightToRenderLine - maxLineHeight)                        
                            //});
                            context.DrawLine(new BasicPoint(x1, heightToRenderLine), new BasicPoint(x2, heightToRenderLine - maxLineHeight), pen);

                            // ------------------------
                            // Negative Max Value

                            // Draw negative value (y: middle to height)
                            //context.StrokeLineSegments(new PointF[2] {
                            //    new PointF(x1, heightToRenderLine), new PointF(x2, heightToRenderLine + (-minLineHeight))
                            //});
                            context.DrawLine(new BasicPoint(x1, heightToRenderLine), new BasicPoint(x2, heightToRenderLine + (-minLineHeight)), pen);
                        }
                        else if (displayType == WaveFormDisplayType.Stereo)
                        {
                            // -----------------------------------------
                            // LEFT Channel - Positive Max Value

                            // Draw positive value (y: middle to top)
                            //context.StrokeLineSegments(new PointF[2] {
                            //    new PointF(x1, heightToRenderLine), new PointF(x2, heightToRenderLine - leftMaxHeight)
                            //});
                            context.DrawLine(new BasicPoint(x1, heightToRenderLine), new BasicPoint(x2, heightToRenderLine - leftMaxHeight), pen);

                            // -----------------------------------------
                            // LEFT Channel - Negative Max Value

                            // Draw negative value (y: middle to height)
                            //context.StrokeLineSegments(new PointF[2] {
                            //    new PointF(x1, heightToRenderLine), new PointF(x2, heightToRenderLine + (-leftMinHeight))
                            //});
                            context.DrawLine(new BasicPoint(x1, heightToRenderLine), new BasicPoint(x2, heightToRenderLine + (-leftMinHeight)), pen);

                            // -----------------------------------------
                            // RIGHT Channel - Positive Max Value

                            // Multiply by 3 to get the new center line for right channel
                            // Draw positive value (y: middle to top)
                            //context.StrokeLineSegments(new PointF[2] {
                            //    new PointF(x1, (heightToRenderLine * 3)), new PointF(x2, (heightToRenderLine * 3) - rightMaxHeight)
                            //});
                            context.DrawLine(new BasicPoint(x1, (heightToRenderLine*3)), new BasicPoint(x2, (heightToRenderLine*3) - rightMaxHeight), pen);

                            // -----------------------------------------
                            // RIGHT Channel - Negative Max Value

                            // Draw negative value (y: middle to height)
                            //context.StrokeLineSegments(new PointF[2] {
                            //    new PointF(x1, (heightToRenderLine * 3)), new PointF(x2, (heightToRenderLine * 3) + (-rightMinHeight))
                            //});
                            context.DrawLine(new BasicPoint(x1, (heightToRenderLine*3)), new BasicPoint(x2, (heightToRenderLine*3) + (-rightMinHeight)), pen);
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
                    imageCache = context.RenderToImageInMemory();
                }
                return imageCache;
            }, TaskCreationOptions.LongRunning).ContinueWith(t =>
            {
                //Console.WriteLine("WaveFormCacheManager - Created image successfully.");
                OnGenerateWaveFormBitmapEnded(new GenerateWaveFormEventArgs()
                {
                    AudioFilePath = audioFile.FilePath,
                    Zoom = zoom,
                    DisplayType = displayType,
                    Image = t.Result
                });
            }, TaskScheduler.FromCurrentSynchronizationContext());

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
