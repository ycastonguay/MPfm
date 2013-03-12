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

namespace MPfm.iOS.Classes.Controls
{
    [Register("MPfmWaveFormView")]
    public class MPfmWaveFormView : UIView
    {
        private PeakFileGenerator _peakFileGenerator;
        private string _status = "Initial status";
        private bool _isLoading = true;

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
            this.BackgroundColor = UIColor.DarkGray;
            _peakFileGenerator = new PeakFileGenerator();
            _peakFileGenerator.OnProcessStarted += HandleOnPeakFileProcessStarted;
            _peakFileGenerator.OnProcessData += HandleOnPeakFileProcessData;
            _peakFileGenerator.OnProcessDone += HandleOnPeakFileProcessDone;
        }

        void HandleOnPeakFileProcessStarted(PeakFileStartedData data)
        {
            InvokeOnMainThread(() => {
                _isLoading = true;
                _status = "Peak file started";
                SetNeedsDisplay();
            });
        }

        void HandleOnPeakFileProcessData(PeakFileProgressData data)
        {
            InvokeOnMainThread(() => {
                _status = "Loading (" + data.PercentageDone.ToString("0") + "% done)";
                SetNeedsDisplay();
            });
        }

        void HandleOnPeakFileProcessDone(PeakFileDoneData data)
        {
            InvokeOnMainThread(() => {
                _status = string.Empty;
                _isLoading = false;
                SetNeedsDisplay();
            });
        }

        public void LoadPeakFile(AudioFile audioFile)
        {
            // Check if another peak file is already loading
            if (_peakFileGenerator.IsLoading)
                _peakFileGenerator.Cancel();

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
                    Console.WriteLine("PeakFile - Failed to create folder!");
                    return;
                }
            }

            // Generate peak file path
            string peakFilePath = Path.Combine(peakFileFolder, Normalizer.NormalizeStringForUrl(audioFile.ArtistName + "_" + audioFile.AlbumTitle + "_" + audioFile.Title + "_" + audioFile.FileType.ToString()) + ".peak");
            
            // Start generating peak file in background
            Console.WriteLine("PeakFile - Generating " + peakFilePath + "...");
            _peakFileGenerator.GeneratePeakFile(audioFile.FilePath, peakFilePath);
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

            // Load bitmap cache (iOS 4.0+ / OSX 10.6+ does not require to have a byte array buffer)
//            int bitsPerComponent = 8;
//            int bytesPerPixel = 4;
//            int bytesPerRow = (Bounds.Width * bitsPerComponent * bytesPerPixel + 7) / 8;
//            int dataSize = bytesPerRow * Bounds.Height;
//            CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB();
//            context = new CGBitmapContext(null, Bounds.Width, Bounds.Height, bitsPerComponent, bytesPerRow, colorSpace, CGImageAlphaInfo.PremultipliedLast | CGBitmapFlags.ByteOrder32Big);
            UIGraphics.BeginImageContext(Bounds.Size);
            context = UIGraphics.GetCurrentContext();
            if (context == null)
            {
                // Error
                Console.WriteLine("Error initializing bitmap cache!");
                return;
            }

            context.SetStrokeColor(new CGColor(1, 1, 1, 1));
            context.SetLineWidth(10);
            context.StrokeLineSegments(new PointF[2] { new PointF(10, 0), new PointF(10, this.Bounds.Height) });

            UIImage image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            // Draw bitmap cache
            context = UIGraphics.GetCurrentContext();
            context.DrawImage(Bounds, image.CGImage);

        }
    }
}
