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
using MPfm.Sound.AudioFiles;
using MPfm.Sound.PeakFiles;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Objects;
using MPfm.iOS.Helpers;
using MPfm.Player.Objects;
using MPfm.GenericControls.Managers;
using MPfm.GenericControls.Managers.Events;
using MPfm.GenericControls.Basics;
using MPfm.iOS.Managers;

namespace MPfm.iOS.Classes.Controls
{
    [Register("MPfmWaveFormView")]
    public class MPfmWaveFormView : UIView
    {
		private Guid _activeMarkerId = Guid.Empty;
        private List<Marker> _markers = new List<Marker>();
        private string _status = "";
        private bool _isLoading = false;
        private bool _isGeneratingImageCache = false;
        private UIImage _imageCache = null;
        private float _cursorX;
        private float _secondaryCursorX;
        private CGColor _colorGradient1 = GlobalTheme.BackgroundColor.CGColor;
        private CGColor _colorGradient2 = GlobalTheme.BackgroundColor.CGColor;

        public WaveFormDisplayType DisplayType { get; set; }

		private WaveFormCacheManagerLegacy _waveFormCacheManager;
		public WaveFormCacheManagerLegacy WaveFormCacheManager
        {
            get
            {
                return _waveFormCacheManager;
            }
        }

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
                
                // Invalidate cursor. TODO: When the cursor is moving quickly, it dispappears because of the invalidation.
                //                          Maybe the cursor shouldn't be rendered, but instead be a simple rect over this control?
                RectangleF rectCursor = new RectangleF(_secondaryCursorX - 25, 0, 50, Frame.Height);
                SetNeedsDisplayInRect(rectCursor);
            }
        }

        private bool _showSecondaryPosition = false;
        public bool ShowSecondaryPosition
        {
            get
            {
                return _showSecondaryPosition;
            }
            set
            {
                _showSecondaryPosition = value;
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
			//_waveFormCacheManager = Bootstrapper.GetContainer().Resolve<WaveFormCacheManager>();
			_waveFormCacheManager = Bootstrapper.GetContainer().Resolve<WaveFormCacheManagerLegacy>();
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
				//Console.WriteLine("MPfmWaveFormView - HandleGeneratePeakFileEndedEvent - LoadPeakFile Cancelled: " + e.Cancelled.ToString() + " FilePath: " + e.AudioFilePath);
                if(!e.Cancelled)
                    _waveFormCacheManager.LoadPeakFile(new AudioFile(e.AudioFilePath));
            });
        }

        private void HandleLoadedPeakFileSuccessfullyEvent(object sender, LoadPeakFileEventArgs e)
        {
            InvokeOnMainThread(() => {
                //Console.WriteLine("MPfmWaveFormView - HandleLoadedPeakFileSuccessfullyEvent");
                GenerateWaveFormBitmap(e.AudioFile, Bounds);
            });
        }

        private void HandleGenerateWaveFormBegunEvent(object sender, GenerateWaveFormEventArgs e)
        {
            
        }

        private void HandleGenerateWaveFormEndedEvent(object sender, GenerateWaveFormEventArgs e)
        {
            InvokeOnMainThread(() => {
                _isLoading = false;
				_imageCache = (UIImage)e.Image;
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

		public void SetActiveMarker(Guid markerId)
		{
			if(markerId != _activeMarkerId)
				InvalidateMarker(_activeMarkerId);

			_activeMarkerId = markerId;
			InvalidateMarker(_activeMarkerId);
		}

		private void InvalidateMarker(Guid markerId)
		{
			try
			{
				var marker = _markers.FirstOrDefault(x => x.MarkerId == markerId);
				if (marker == null)
					return;

				float xPct = (float)marker.PositionBytes / (float)Length;
				float xMarker = xPct * Bounds.Width;
				RectangleF rectCursor = new RectangleF(xMarker - 5, 0, 25, Frame.Height);
				SetNeedsDisplayInRect(rectCursor);
			}
			catch(Exception ex)
			{
				Console.WriteLine("WaveFormView - InvalidateMarker - ex: {0}", ex);
			}
		}

		public void SetMarkers(IEnumerable<Marker> markers)
        {
            _markers = markers.ToList();
            SetNeedsDisplay();
        }

		public void SetMarkerPosition(Marker marker)
		{
			var localMarker = _markers.FirstOrDefault(x => x.MarkerId == marker.MarkerId);
			if (localMarker == null)
				return;
		
			localMarker.Position = marker.Position;
			localMarker.PositionBytes = marker.PositionBytes;
			localMarker.PositionPercentage = marker.PositionPercentage;
			localMarker.PositionSamples = marker.PositionSamples;

			// TODO: Only refresh the old/new marker positions
			SetNeedsDisplay();
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
			//Console.WriteLine("WaveFormView - LoadPeakFile " + audioFile.FilePath);
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
            var screenSize = UIKitHelper.GetDeviceSize();
            CoreGraphicsHelper.FillGradient(context, Bounds, _colorGradient2, _colorGradient1);

            NSString str = new NSString(status);
            context.SetFillColor(UIColor.White.CGColor);
            float y = (Bounds.Height - 30) / 2;

            UIGraphics.PushContext(context);
            str.DrawString(new RectangleF(0, y, screenSize.Width, 30), UIFont.FromName("HelveticaNeue-Light", 12), UILineBreakMode.TailTruncation, UITextAlignment.Center);
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
            _cursorX = positionPercentage * Bounds.Width;
            
            // Draw markers
            for(int a = 0; a < _markers.Count; a++)
            {
                float xPct = (float)_markers[a].PositionBytes / (float)Length;
                float x = xPct * Bounds.Width;

                // Draw cursor line
				var color = _markers[a].MarkerId == _activeMarkerId ? GlobalTheme.SecondaryLightColor : new UIColor(1, 0, 0, 1);
				context.SetStrokeColor(color.CGColor);
                context.SetLineWidth(1.0f);
                context.StrokeLineSegments(new PointF[2] { new PointF(x, 0), new PointF(x, heightAvailable) });

                // Draw text
                var rectText = new RectangleF(x, 0, 12, 12);
				CoreGraphicsHelper.FillRect(context, rectText, color.ColorWithAlpha(0.7f).CGColor);
                string letter = Conversion.IndexToLetter(a).ToString();
                UIGraphics.PushContext(context);
                CoreGraphicsHelper.DrawTextAtPoint(context, new PointF(x+2, 0), letter, "HelveticaNeue", 10, new CGColor(1, 1, 1, 1));
                UIGraphics.PopContext();
            }

            // Draw cursor line
            context.SetStrokeColor(new CGColor(0, 0.5f, 1, 1));
            context.SetLineWidth(1.0f);
            context.StrokeLineSegments(new PointF[2] { new PointF(_cursorX, 0), new PointF(_cursorX, heightAvailable) });

            // Check if a secondary cursor must be drawn (i.e. when changing position)
            if(_showSecondaryPosition)
            {
                float secondaryPositionPercentage = (float)SecondaryPosition / (float)Length;
                _secondaryCursorX = secondaryPositionPercentage * Bounds.Width;
                _secondaryCursorX = (float)Math.Round(_secondaryCursorX * 2) / 2; // Round to 0.5

                // Draw cursor line
                context.SetStrokeColor(new CGColor(1, 1, 1, 1));
                context.SetLineWidth(1.0f);
                context.StrokeLineSegments(new PointF[2] { new PointF(_secondaryCursorX, 0), new PointF(_secondaryCursorX, heightAvailable) });
            }
        }

        public void RefreshWaveFormBitmap()
        {
            RefreshWaveFormBitmap(Frame.Width);
        }

        public void RefreshWaveFormBitmap(float width)
        {
            if (_audioFile == null)
                return;

            //RefreshStatus("Generating new bitmap...");
            GenerateWaveFormBitmap(_audioFile, new RectangleF(Frame.X, Frame.Y, width, Frame.Height));
        }

        private void GenerateWaveFormBitmap(AudioFile audioFile, RectangleF frame)
        {
            if(!_isGeneratingImageCache)
            {
                _isGeneratingImageCache = true;
				//Console.WriteLine("MPfmWaveFormView - GenerateWaveFormBitmap audioFilePath: {0}", audioFile.FilePath);
				_waveFormCacheManager.RequestBitmap(audioFile, WaveFormDisplayType.Stereo, frame, 1, _length);
				//_waveFormCacheManager.RequestBitmap(audioFile, WaveFormDisplayType.Stereo, new BasicRectangle(frame.X, frame.Y, frame.Width, frame.Height), 1, _length);
                _isGeneratingImageCache = false;
            }
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
}
