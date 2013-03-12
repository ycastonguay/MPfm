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
using System.Drawing;
using System.Linq;
using MPfm.Core;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.Sound;
using MPfm.Sound.AudioFiles;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.IO;
using System.Threading.Tasks;

namespace MPfm.iOS.Classes.Controls
{
    [Register("MPfmWaveFormView")]
    public class MPfmWaveFormView : UIView
    {
        private PeakFileGenerator _peakFileGenerator;
        private string _status = "Initial status";
        private bool _isLoading = false;
        private UIImage _imageCache = null;
        private List<WaveDataMinMax> WaveDataHistory { get; set; }

        public WaveFormDisplayType DisplayType { get; set; }

        public MPfmWaveFormView(IntPtr handle) 
            : base (handle)
        {
            Initialize();
        }

        public MPfmWaveFormView(RectangleF frame) 
            : base(frame)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.BackgroundColor = UIColor.Black;
            WaveDataHistory = new List<WaveDataMinMax>();
            DisplayType = WaveFormDisplayType.Stereo;

            _peakFileGenerator = new PeakFileGenerator();
            _peakFileGenerator.OnProcessStarted += HandleOnPeakFileProcessStarted;
            _peakFileGenerator.OnProcessData += HandleOnPeakFileProcessData;
            _peakFileGenerator.OnProcessDone += HandleOnPeakFileProcessDone;
        }

        void HandleOnPeakFileProcessStarted(PeakFileStartedData data)
        {
            InvokeOnMainThread(() => {
                WaveDataHistory = new List<WaveDataMinMax>((int)data.TotalBlocks);
                if(_imageCache != null)
                {
                    _imageCache.Dispose();
                    _imageCache = null;
                }
                _isLoading = true;
                _status = "Loading (0% done)";
                SetNeedsDisplay();
            });
        }

        void HandleOnPeakFileProcessData(PeakFileProgressData data)
        {
            InvokeOnMainThread(() => {

                // Add wave data to history. Use a while a loop to modify the collection while looping.
                List<WaveDataMinMax> minMaxs = data.MinMax;
                while (true)
                {
                    if (minMaxs.Count == 0)
                    {
                        break;
                    }
                    
                    WaveDataHistory.Add(minMaxs[0]);
                    minMaxs.RemoveAt(0);
                }

                _status = "Loading (" + data.PercentageDone.ToString("0") + "% done)";
                SetNeedsDisplay();
            });
        }

        void HandleOnPeakFileProcessDone(PeakFileDoneData data)
        {
            InvokeOnMainThread(() => {
                if(data.Cancelled)
                {
                    if(_imageCache != null)
                    {
                        _imageCache.Dispose();
                        _imageCache = null;
                    }
                    WaveDataHistory = new List<WaveDataMinMax>();
                }

                _status = string.Empty;
                _isLoading = false;
                SetNeedsDisplay();
            });
        }

        public void LoadPeakFile(AudioFile audioFile)
        {
            // Check if another peak file is already loading
            Console.WriteLine("WaveFormView - LoadPeakFile audioFile: " + audioFile.FilePath);
            if (_peakFileGenerator.IsLoading)
                _peakFileGenerator.Cancel();

            //

            // Check if the peak file subfolder exists
            string peakFileFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PeakFiles");
            if (!Directory.Exists(peakFileFolder))
            {
                try
                {
                    Console.WriteLine("PeakFile - Creating folder " + peakFileFolder + "...");
                    DirectoryInfo directoryInfo = Directory.CreateDirectory(peakFileFolder);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("PeakFile - Failed to create folder: " + ex.Message);
                    return;
                }
            }

            // Generate peak file path
            string peakFilePath = Path.Combine(peakFileFolder, Normalizer.NormalizeStringForUrl(audioFile.ArtistName + "_" + audioFile.AlbumTitle + "_" + audioFile.Title + "_" + audioFile.FileType.ToString()) + ".peak");

            // Check if peak file exists
            bool peakFileLoadedSuccessfully = false;
            if (File.Exists(peakFilePath))
            {
                Task<List<WaveDataMinMax>>.Factory.StartNew(() => {
                    List<WaveDataMinMax> data = null;
                    try
                    {
                        data = _peakFileGenerator.ReadPeakFile(peakFilePath);
                        if(data != null)
                            return data;
                    } 
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error reading peak file: " + ex.Message);
                    }

                    try
                    {
                        Console.WriteLine("Peak file could not be loaded - Generating " + peakFilePath + "...");
                        _peakFileGenerator.GeneratePeakFile(audioFile.FilePath, peakFilePath);
                    } 
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error generating peak file: " + ex.Message);
                    }
                    return null;
                }, TaskCreationOptions.LongRunning).ContinueWith(t => {
                    List<WaveDataMinMax> data = (List<WaveDataMinMax>)t.Result;

                    if (data == null)
                    {
                        // The peak file has been generated on another thread
                        return;
                    }
                    else
                    {
                        InvokeOnMainThread(() => {
                            // Refresh image cache
                            WaveDataHistory = data;
                            if(_imageCache != null)
                            {
                                _imageCache.Dispose();
                                _imageCache = null;
                            }
                            SetNeedsDisplay();
                        });
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            } 
            else
            {
                // Start generating peak file in background
                Console.WriteLine("Peak file doesn't exist - Generating " + peakFilePath + "...");
                _peakFileGenerator.GeneratePeakFile(audioFile.FilePath, peakFilePath);
            }
        }

        public void CancelPeakFileGeneration()
        {
            _peakFileGenerator.Cancel();
        }

        public override void Draw(RectangleF rect)
        {
            base.Draw(rect);

            CGContext context;
            if (_isLoading)
            {
                context = UIGraphics.GetCurrentContext();
                NSString str = new NSString(_status);
                context.SetFillColor(new CGColor(1, 1, 1, 1));
                str.DrawString(new RectangleF(0, 30, Bounds.Width, 30), UIFont.FromName("OstrichSans-Black", 16), UILineBreakMode.TailTruncation, UITextAlignment.Center);
                return;
            }

            // If there is no wave data and nothing is loading, just display nothing
            if (WaveDataHistory.Count == 0)
            {
                return;
            }

            // Check if bitmap cache should be reloaded
            if (_imageCache == null)
            {
                //UIGraphics.BeginImageContext(Bounds.Size);
                UIGraphics.BeginImageContextWithOptions(Bounds.Size, false, 0);
                context = UIGraphics.GetCurrentContext();
                if (context == null)
                {
                    // Error
                    Console.WriteLine("Error initializing bitmap cache!");
                    return;
                }

                // Declare variables
                int widthAvailable = (int)Bounds.Width;
                int heightAvailable = (int)Bounds.Height;
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
                float desiredLineWidth = 1.0f;
                WaveDataMinMax[] subset = null;      

                historyCount = WaveDataHistory.Count;

                // Find out how many samples are represented by each line of the wave form, depending on its width.
                // For example, if the history has 45000 items, and the control has a width of 1000px, 45 items will need to be averaged by line.
                lineWidthPerHistoryItem = (float)widthAvailable / (float)historyCount;
                //float historyItemsPerLine = (float)WaveDataHistory.Count / (float)Bounds.Width;

                // Check if the line width is below the desired line width
                if (lineWidthPerHistoryItem < desiredLineWidth)
                {
                    // Try to get a line width around 0.5f so the precision is good enough and no artifacts will be shown.
                    while (lineWidth < desiredLineWidth)
                    {
                        // Increment the number of history items per line
                        Console.WriteLine("Determining line width (lineWidth: " + lineWidth.ToString() + " desiredLineWidth: " + desiredLineWidth.ToString() + " nHistoryItemsPerLine: " + nHistoryItemsPerLine.ToString() + " lineWidthPerHistoryItem: " + lineWidthPerHistoryItem.ToString());
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

                Console.WriteLine("WaveFormView - historyItemsPerLine: " + nHistoryItemsPerLine.ToString());

                context.SetStrokeColor(new CGColor(1, 1, 0.3f, 1));
                context.SetLineWidth(lineWidth);
                //context.SetLineWidth(0.5f);

                for (float i = 0; i < widthAvailable; i += lineWidth)
                {
                    // Determine the maximum height of a line (+/-)
                    //Console.WriteLine("WaveForm - Rendering " + i.ToString() + " on " + widthAvailable.ToString());
                    float heightToRenderLine = 0;
                    if (DisplayType == WaveFormDisplayType.Stereo)
                    {
                        heightToRenderLine = (float)heightAvailable / 4;
                    }
                    else
                    {
                        heightToRenderLine = (float)heightAvailable / 2;
                    }
                    
                    // Determine x position
                    x1 = i;
                    x2 = i + lineWidth;
                    
                    try
                    {
                        // Check if there are multiple history items per line
                        if (nHistoryItemsPerLine > 1)
                        {
                            if (historyIndex + nHistoryItemsPerLine > historyCount)
                            {
                                // Create subset with remaining data
                                subset = new WaveDataMinMax[historyCount - historyIndex];
                                WaveDataHistory.CopyTo(historyIndex, subset, 0, historyCount - historyIndex);
                            }
                            else
                            {
                                subset = new WaveDataMinMax[nHistoryItemsPerLine];
                                WaveDataHistory.CopyTo(historyIndex, subset, 0, nHistoryItemsPerLine);
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
                            leftMin = WaveDataHistory[historyIndex].leftMin;
                            leftMax = WaveDataHistory[historyIndex].leftMax;
                            rightMin = WaveDataHistory[historyIndex].rightMin;
                            rightMax = WaveDataHistory[historyIndex].rightMax;
                            mixMin = WaveDataHistory[historyIndex].mixMin;
                            mixMax = WaveDataHistory[historyIndex].mixMax;
                        }
                        
                        // Increment history count
                        //historyCount += historyItemsPerLine;
                        
                        leftMaxHeight = leftMax * heightToRenderLine;
                        leftMinHeight = leftMin * heightToRenderLine;
                        rightMaxHeight = rightMax * heightToRenderLine;
                        rightMinHeight = rightMin * heightToRenderLine;
                        mixMaxHeight = mixMax * heightToRenderLine;
                        mixMinHeight = mixMin * heightToRenderLine;
                    }
                    catch
                    {
                        throw;
                    }

                    // Determine display type
                    if (DisplayType == WaveFormDisplayType.LeftChannel ||
                        DisplayType == WaveFormDisplayType.RightChannel ||
                        DisplayType == WaveFormDisplayType.Mix)
                    {
                        // Calculate min/max line height
                        float minLineHeight = 0;
                        float maxLineHeight = 0;
                        
                        // Set mib/max
                        if (DisplayType == WaveFormDisplayType.LeftChannel)
                        {
                            minLineHeight = leftMinHeight;
                            maxLineHeight = leftMaxHeight;
                        }
                        else if (DisplayType == WaveFormDisplayType.RightChannel)
                        {
                            minLineHeight = rightMinHeight;
                            maxLineHeight = rightMaxHeight;
                        }
                        else if (DisplayType == WaveFormDisplayType.Mix)
                        {
                            minLineHeight = mixMinHeight;
                            maxLineHeight = mixMaxHeight;
                        }
                        
                        // ------------------------
                        // Positive Max Value                   
                        
                        // Draw positive value (y: middle to top)                   

                        context.StrokeLineSegments(new PointF[2] {
                            new PointF(x1, heightToRenderLine), new PointF(x2, heightToRenderLine - maxLineHeight)                        
                        });
                        
                        // ------------------------
                        // Negative Max Value
                        
                        // Draw negative value (y: middle to height)
                        context.StrokeLineSegments(new PointF[2] {
                            new PointF(x1, heightToRenderLine), new PointF(x2, heightToRenderLine + (-minLineHeight))
                        });
                    }
                    else if (DisplayType == WaveFormDisplayType.Stereo)
                    {
                        // -----------------------------------------
                        // LEFT Channel - Positive Max Value
                        
                        // Draw positive value (y: middle to top)
                        context.StrokeLineSegments(new PointF[2] {
                            new PointF(x1, heightToRenderLine), new PointF(x2, heightToRenderLine - leftMaxHeight)
                        });
                        
                        // -----------------------------------------
                        // LEFT Channel - Negative Max Value
                        
                        // Draw negative value (y: middle to height)
                        context.StrokeLineSegments(new PointF[2] {
                            new PointF(x1, heightToRenderLine), new PointF(x2, heightToRenderLine + (-leftMinHeight))
                        });
                        
                        // -----------------------------------------
                        // RIGHT Channel - Positive Max Value
                        
                        // Multiply by 3 to get the new center line for right channel
                        // Draw positive value (y: middle to top)
                        context.StrokeLineSegments(new PointF[2] {
                            new PointF(x1, (heightToRenderLine * 3)), new PointF(x2, (heightToRenderLine * 3) - rightMaxHeight)
                        });
                        
                        // -----------------------------------------
                        // RIGHT Channel - Negative Max Value
                        
                        // Draw negative value (y: middle to height)
                        context.StrokeLineSegments(new PointF[2] {
                            new PointF(x1, (heightToRenderLine * 3)), new PointF(x2, (heightToRenderLine * 3) + (-rightMinHeight))
                        });
                    }

                    // Increment the history index; pad the last values if the count is about to exceed
                    if (historyIndex < historyCount - 1)
                    {
                        // Increment by the number of history items per line
                        historyIndex += nHistoryItemsPerLine;
                    }
                }

                // Get image from context
                _imageCache = UIGraphics.GetImageFromCurrentImageContext();
                UIGraphics.EndImageContext();
            }

            // Draw bitmap cache
            context = UIGraphics.GetCurrentContext();
            context.DrawImage(Bounds, _imageCache.CGImage);
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
