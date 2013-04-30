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
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MPfm.Core;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.Sound;
using MPfm.Sound.AudioFiles;
using MPfm.Sound.PeakFiles;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Objects;
using MPfm.iOS.Helpers;
using MPfm.iOS.Managers;
using MPfm.iOS.Managers.Events;

namespace MPfm.iOS.Classes.Controls
{
    [Register("MPfmWaveFormView")]
    public class MPfmWaveFormView : UIView
    {
        private WaveFormCacheManager _waveFormCacheManager;
        private string _status = "";
        private bool _isLoading = false;
        private bool _isGeneratingImageCache = false;
        private UIImage _imageCache = null;
        private float _cursorX;
        private float _secondaryCursorX;
        private CGColor _colorGradient1 = GlobalTheme.BackgroundColor.CGColor;
        private CGColor _colorGradient2 = GlobalTheme.BackgroundColor.CGColor;

        public WaveFormDisplayType DisplayType { get; set; }

        private AudioFile _audioFile = null;
        public AudioFile AudioFile
        {
            get
            {
                return _audioFile;
            }
        }

        private long _position;
        public long Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;

                // Don't bother if a peak file is loading
                if(_isLoading || _imageCache == null)
                    return;

                // Invalidate cursor
                RectangleF rectCursor = new RectangleF(_cursorX - 5, 0, 10, Frame.Height);
                SetNeedsDisplayInRect(rectCursor);
            }
        }

        private long _secondaryPosition;
        public long SecondaryPosition
        {
            get
            {
                return _secondaryPosition;
            }
            set
            {
                _secondaryPosition = value;
                
                // Don't bother if a peak file is loading
                if(_isLoading)
                    return;
                
                // Invalidate cursor
                RectangleF rectCursor = new RectangleF(_secondaryCursorX - 15, 0, 0, Frame.Height);
                SetNeedsDisplayInRect(rectCursor);
            }
        }

        private long _length;
        public long Length
        {
            get
            {
                return _length;
            }
            set
            {
                _length = value;
            }
        }

        private float _scrollX;
        public float ScrollX
        {
            get
            {
                return _scrollX;
            }
            set
            {
                _scrollX = value;
            }
        }

        private float _zoom = 1.0f;
        public float Zoom
        {
            get
            {
                return _zoom;
            }
            set
            {
                _zoom = value;
            }
        }

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
            DisplayType = WaveFormDisplayType.Stereo;
            _waveFormCacheManager = Bootstrapper.GetContainer().Resolve<WaveFormCacheManager>();
            _waveFormCacheManager.GeneratePeakFileBegunEvent += HandleGeneratePeakFileBegunEvent;
            _waveFormCacheManager.GeneratePeakFileProgressEvent += HandleGeneratePeakFileProgressEvent;
            _waveFormCacheManager.GeneratePeakFileEndedEvent += HandleGeneratePeakFileEndedEvent;
            _waveFormCacheManager.LoadedPeakFileSuccessfullyEvent += HandleLoadedPeakFileSuccessfullyEvent;
            _waveFormCacheManager.GenerateWaveFormBitmapBegunEvent += HandleGenerateWaveFormBegunEvent;
            _waveFormCacheManager.GenerateWaveFormBitmapEndedEvent += HandleGenerateWaveFormEndedEvent;
        }

        private void HandleGeneratePeakFileBegunEvent(object sender, GeneratePeakFileEventArgs e)
        {
            InvokeOnMainThread(() => {
                //Console.WriteLine("MPfmWaveFormView - HandleGeneratePeakFileBegunEvent");
                RefreshStatus("Generating wave form (0% done)");
            });
        }

        private void HandleGeneratePeakFileProgressEvent(object sender, GeneratePeakFileEventArgs e)
        {
            InvokeOnMainThread(() => {
                //Console.WriteLine("MPfmWaveFormView - HandleGeneratePeakFileProgressEvent  (" + e.PercentageDone.ToString("0") + "% done)");
                RefreshStatus("Generating wave form (" + e.PercentageDone.ToString("0") + "% done)");
            });            
        }

        private void HandleGeneratePeakFileEndedEvent(object sender, GeneratePeakFileEventArgs e)
        {
            InvokeOnMainThread(() => {
                // TODO: Check if cancelled? This will not fire another LoadPeakFile if the peak file gen was cancelled.
                Console.WriteLine("MPfmWaveFormView - HandleGeneratePeakFileEndedEvent - LoadPeakFile Cancelled: " + e.Cancelled.ToString() + " FilePath: " + e.AudioFilePath);
                if(!e.Cancelled)
                    _waveFormCacheManager.LoadPeakFile(new AudioFile(e.AudioFilePath));
            });
        }

        private void HandleLoadedPeakFileSuccessfullyEvent(object sender, LoadPeakFileEventArgs e)
        {
            InvokeOnMainThread(() => {
                //Console.WriteLine("MPfmWaveFormView - HandleLoadedPeakFileSuccessfullyEvent");
                GenerateWaveFormBitmap(e.AudioFile.FilePath, Bounds);
            });
        }

        private void HandleGenerateWaveFormBegunEvent(object sender, GenerateWaveFormEventArgs e)
        {
            
        }

        private void HandleGenerateWaveFormEndedEvent(object sender, GenerateWaveFormEventArgs e)
        {
            InvokeOnMainThread(() => {
                _isLoading = false;
                _imageCache = e.Image;
                SetNeedsDisplay();
            });            
        }

        void HandleOnPeakFileProcessStarted(PeakFileStartedData data)
        {
        }

        void HandleOnPeakFileProcessData(PeakFileProgressData data)
        {

        }

        void HandleOnPeakFileProcessDone(PeakFileDoneData data)
        {
        }

        public void FlushCache()
        {
            _waveFormCacheManager.FlushCache();

            if(_imageCache != null)
            {
                _imageCache.Dispose();
                _imageCache = null;
            }
        }

        public void LoadPeakFile(AudioFile audioFile)
        {
            Console.WriteLine("WaveFormView - LoadPeakFile " + audioFile.FilePath);
            _audioFile = audioFile;
            RefreshStatus("Loading peak file...");
            _waveFormCacheManager.LoadPeakFile(audioFile);
        }

        private void RefreshStatus(string status)
        {
            _isLoading = true;
            _status = status;
            SetNeedsDisplay();
        }

        private void DrawStatus(CGContext context, string status)
        {
            CoreGraphicsHelper.FillGradient(context, Bounds, _colorGradient2, _colorGradient1);

            NSString str = new NSString(status);
            context.SetFillColor(UIColor.White.CGColor);
            float y = (Bounds.Height - 30) / 2;

            UIGraphics.PushContext(context);
            str.DrawString(new RectangleF(0, y, Bounds.Width, 30), UIFont.FromName("HelveticaNeue", 12), UILineBreakMode.TailTruncation, UITextAlignment.Center);
            UIGraphics.PopContext();
        }

        private void DrawWaveFormBitmap(CGContext context)
        {
            _isLoading = false;
            int heightAvailable = (int)Frame.Height;

            // Draw bitmap cache
            context.DrawImage(Bounds, _imageCache.CGImage);
            
            // Calculate position
            float positionPercentage = (float)Position / (float)Length;
            _cursorX = (positionPercentage * Bounds.Width) - ScrollX;
            
            // Draw cursor line
            context.SetStrokeColor(new CGColor(0, 0.5f, 1, 1));
            context.SetLineWidth(1.0f);
            context.StrokeLineSegments(new PointF[2] { new PointF(_cursorX, 0), new PointF(_cursorX, heightAvailable) });
            
            // Check if a secondary cursor must be drawn
            if(_secondaryPosition > 0)
            {
                // Calculate position
                float secondaryPositionPercentage = (float)SecondaryPosition / (float)Length;
                _secondaryCursorX = (secondaryPositionPercentage * Bounds.Width) - ScrollX;
                
                // Draw cursor line
                context.SetStrokeColor(new CGColor(1, 1, 1, 1));
                context.SetLineWidth(1.0f);
                context.StrokeLineSegments(new PointF[2] { new PointF(_secondaryCursorX, 0), new PointF(_secondaryCursorX, heightAvailable) });
            }
        }

        public void RefreshWaveFormBitmap()
        {
            GenerateWaveFormBitmap(_audioFile.FilePath, Frame);
        }

        private void GenerateWaveFormBitmap(string audioFilePath, RectangleF frame)
        {
            var context = UIGraphics.GetCurrentContext();
            if(!_isGeneratingImageCache)
            {
                _isGeneratingImageCache = true;
                Console.WriteLine("MPfmWaveFormView - GenerateWaveFormBitmap audioFilePath: " + audioFilePath.ToString());
                _waveFormCacheManager.RequestBitmap(audioFilePath, WaveFormDisplayType.Stereo, frame, 1);
                _isGeneratingImageCache = false;
            }
        }

        public void SetFrame(RectangleF frame)
        {
            Frame = frame;
            InvokeOnMainThread(() => {
                GenerateWaveFormBitmap(_audioFile.FilePath, frame);
            });
        }

        public override void Draw(RectangleF rect)
        {
            // Leave empty! Actual drawing is in DrawLayer
        }

        [Export ("drawLayer:inContext:")]
        public void DrawLayer(CALayer layer, CGContext context)
        {
            if(_isLoading)
            {
                DrawStatus(context, _status);
            }
            else if (_imageCache != null)
            {
                DrawWaveFormBitmap(context);
            }
            else
            {
                CoreGraphicsHelper.FillRect(context, Bounds, _colorGradient1);
            }
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
