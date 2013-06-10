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
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Helpers;
using MPfm.iOS.Managers;
using MPfm.iOS.Managers.Events;
using MPfm.iOS.Classes.Objects;

namespace MPfm.iOS.Classes.Controls
{
    [Register("MPfmWaveFormScrollView")]
    public class MPfmWaveFormScrollView : UIScrollView
    {
        private float _zoomScale;
        private float _offsetRatio;
        private UILabel _lblZoom;
        private UILabel _lblScrollStartPosition;
        private UILabel _lblScrollEndPosition;
        private UIView _viewScrollStartPosition;
        private UIView _viewScrollStartPositionLine;
        private UIView _viewScrollEndPosition;
        private UIView _viewScrollEndPositionLine;

        // TODO: Make this entirely private and add methods to set wave forms
        public MPfmWaveFormView WaveFormView { get; private set; }

        public MPfmWaveFormScrollView(IntPtr handle) 
            : base (handle)
        {
            Initialize();
        }
        
        public MPfmWaveFormScrollView(RectangleF frame) 
            : base(frame)
        {
            Initialize();
        }

        private void Initialize()
        {
            //WeakDelegate = this;
            MinimumZoomScale = 1.0f;
            MaximumZoomScale = 16.0f;
            ShowsHorizontalScrollIndicator = true;
            ShowsVerticalScrollIndicator = false;
            AlwaysBounceHorizontal = false;
            BouncesZoom = true;
            BackgroundColor = GlobalTheme.BackgroundColor;

            UITapGestureRecognizer doubleTap = new UITapGestureRecognizer((recognizer) => {
                _zoomScale = 1.0f;
                _offsetRatio = 0;
                UpdateZoomScale(0);
                WaveFormView.RefreshWaveFormBitmap();                
                
                _lblZoom.Text = "100.0%";
                _viewScrollStartPosition.Alpha = 0;
                _viewScrollEndPosition.Alpha = 0;
                UIView.Animate(0.15, () => {
                    _lblZoom.Alpha = 0.9f;
                }, () => {
                    UIView.Animate(1, () => {
                        _lblZoom.Alpha = 0.9f;
                    }, () => {
                        UIView.Animate(0.15, () => {
                            _lblZoom.Alpha = 0;
                        });
                    });
                });
            });
            doubleTap.DelaysTouchesBegan = true;
            doubleTap.NumberOfTapsRequired = 2;
            AddGestureRecognizer(doubleTap);

            WaveFormView = new MPfmWaveFormView(Bounds);
            AddSubview(WaveFormView);

            _lblZoom = new UILabel(new RectangleF(0, 0, 60, 20));
            _lblZoom.BackgroundColor = GlobalTheme.BackgroundColor;
            _lblZoom.TextColor = UIColor.White;
            _lblZoom.Font = UIFont.FromName("HelveticaNeue", 12.0f);
            _lblZoom.TextAlignment = UITextAlignment.Center;
            _lblZoom.Text = "100.0%";
            _lblZoom.Alpha = 0;
            AddSubview(_lblZoom);

            _viewScrollStartPosition = new UIView(new RectangleF(0, 0, 58, 20));
            _lblScrollStartPosition = new UILabel(new RectangleF(5, 0, 50, 20));
            _lblScrollStartPosition.BackgroundColor = UIColor.Clear;
            _lblScrollStartPosition.TextColor = UIColor.White;
            _lblScrollStartPosition.Font = UIFont.FromName("HelveticaNeue", 12.0f);
            _lblScrollStartPosition.Text = "0:00.000";
            _viewScrollStartPosition.BackgroundColor = GlobalTheme.MainLightColor;
            _viewScrollStartPosition.Alpha = 0;
            _viewScrollStartPosition.AddSubview(_lblScrollStartPosition);
            AddSubview(_viewScrollStartPosition);

            _viewScrollStartPositionLine = new UIView(new RectangleF(0, 0, 1, Bounds.Height));
            _viewScrollStartPositionLine.BackgroundColor = GlobalTheme.MainLightColor;
            _viewScrollStartPositionLine.Alpha = 0;
            AddSubview(_viewScrollStartPositionLine);

            _viewScrollEndPosition = new UIView(new RectangleF(Bounds.Width - 58, 0, 58, 20));
            _lblScrollEndPosition = new UILabel(new RectangleF(6, 0, 50, 20));
            _lblScrollEndPosition.BackgroundColor = UIColor.Clear;
            _lblScrollEndPosition.TextColor = UIColor.White;
            _lblScrollEndPosition.Font = UIFont.FromName("HelveticaNeue", 12.0f);
            _lblScrollEndPosition.Text = "0:00.000";
            _viewScrollEndPosition.BackgroundColor = GlobalTheme.MainLightColor;
            _viewScrollEndPosition.Alpha = 0;
            _viewScrollEndPosition.AddSubview(_lblScrollEndPosition);
            AddSubview(_viewScrollEndPosition);

            _viewScrollEndPositionLine = new UIView(new RectangleF(Bounds.Width - 1, 0, 1, Bounds.Height));
            _viewScrollEndPositionLine.BackgroundColor = GlobalTheme.MainLightColor;
            _viewScrollEndPositionLine.Alpha = 0;
            AddSubview(_viewScrollEndPositionLine);

            ContentSize = WaveFormView.Bounds.Size;

            this.ViewForZoomingInScrollView = delegate {
                _offsetRatio = (ContentOffset.X / ContentSize.Width);
                return WaveFormView;
            };

            this.ZoomingStarted += delegate {
                ShowViewportPosition();
                UIView.Animate(0.15, () => {
                    _lblZoom.Alpha = 0.9f;
                });
            };

            this.ZoomingEnded += delegate(object sender, ZoomingEndedEventArgs e) {
                WaveFormView.RefreshWaveFormBitmap();
                UIView.Animate(0.25, () => {
                    _lblZoom.Alpha = 0;
                });
            };

            this.DidZoom += delegate(object sender, EventArgs e) {
                UpdateZoomScale(_offsetRatio);
            };

            this.Scrolled += delegate(object sender, EventArgs e) {
                long startPositionBytes = (long)((ContentOffset.X / ContentSize.Width) * WaveFormView.Length);
                startPositionBytes = (startPositionBytes < 0) ? 0 : startPositionBytes;                
                long endPositionBytes = (long)(((ContentOffset.X + Bounds.Width) / ContentSize.Width) * WaveFormView.Length);
                endPositionBytes = (endPositionBytes > WaveFormView.Length) ? WaveFormView.Length : endPositionBytes;    

                string startPosition = ConvertAudio.ToTimeString(startPositionBytes, (uint)WaveFormView.AudioFile.BitsPerSample, WaveFormView.AudioFile.AudioChannels, (uint)WaveFormView.AudioFile.SampleRate);
                string endPosition = ConvertAudio.ToTimeString(endPositionBytes, (uint)WaveFormView.AudioFile.BitsPerSample, WaveFormView.AudioFile.AudioChannels, (uint)WaveFormView.AudioFile.SampleRate);

//                long positionSamples = ConvertAudio.ToPCM(positionBytes, (uint)WaveFormView.AudioFile.BitsPerSample, 2);
//                long positionMS = (int)ConvertAudio.ToMS(positionSamples, (uint)WaveFormView.AudioFile.SampleRate);
//                string position = Conversion.MillisecondsToTimeString((ulong)positionMS);

                _lblScrollStartPosition.Text = startPosition;
                _lblScrollEndPosition.Text = endPosition;
            };

            this.DraggingStarted += delegate(object sender, EventArgs e) {
                ShowViewportPosition();
            };

            this.DraggingEnded += delegate(object sender, DraggingEventArgs e) {
                if(e.Decelerate)
                    return;

                HideViewportPosition();
            };

            this.DecelerationEnded += delegate(object sender, EventArgs e) {
                HideViewportPosition();
            };
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            // Determine if the scroll position is before the start of the wave form 
            float startOffsetX = (ContentOffset.X < 0) ? 0 : ContentOffset.X;
            _viewScrollStartPosition.Frame = new RectangleF(startOffsetX, 0, 58, 20); 
            _viewScrollStartPositionLine.Frame = new RectangleF(startOffsetX, 0, 1, Bounds.Height); 

            // Determine if the scroll position is past the end of the wave form
            float endOffsetX = (ContentOffset.X + Bounds.Width > ContentSize.Width) ? 
                (ContentOffset.X + Bounds.Width) - ((ContentOffset.X + Bounds.Width) - ContentSize.Width) : ContentOffset.X + Bounds.Width;
            _viewScrollEndPosition.Frame = new RectangleF(endOffsetX - 58, 0, 58, 20);
            _viewScrollEndPositionLine.Frame = new RectangleF(endOffsetX - 1, 0, 1, Bounds.Height); 

            _lblZoom.Frame = new RectangleF(ContentOffset.X + ((Bounds.Width - 54) / 2), (Bounds.Height - 20) / 2, 54, 20);
        }

        private void UpdateZoomScale(float offsetRatio)
        {
            var originalZoomScale = ZoomScale;
            _zoomScale *= ZoomScale;
            _zoomScale = (_zoomScale < MinimumZoomScale) ? MinimumZoomScale : _zoomScale;
            _zoomScale = (_zoomScale > MaximumZoomScale) ? MaximumZoomScale : _zoomScale;
            ZoomScale = 1.0f;
            _lblZoom.Text = (_zoomScale * 100).ToString("0.0") + "%";
            //Console.WriteLine("MPfmWaveFormScrollView - DidZoom ZoomScale: " + originalZoomScale.ToString() + " _zoomScale: " + _zoomScale.ToString());
            
            WaveFormView.Frame = new RectangleF(WaveFormView.Frame.X, WaveFormView.Frame.Y, UIScreen.MainScreen.Bounds.Width * _zoomScale, WaveFormView.Frame.Height);
            ContentSize = new SizeF(WaveFormView.Frame.Width, Bounds.Height);
            ContentOffset = new PointF(WaveFormView.Frame.Width * offsetRatio, 0);
        }

        private void ShowViewportPosition()
        {
            UIView.Animate(0.25, () => {
                _viewScrollStartPosition.Alpha = 0.8f;
                _viewScrollStartPositionLine.Alpha = 0.8f;
                _viewScrollEndPosition.Alpha = 0.8f;
                _viewScrollEndPositionLine.Alpha = 0.8f;
            });
        }

        private void HideViewportPosition()
        {
            UIView.Animate(0.35, () => {
                _viewScrollStartPosition.Alpha = 0;
                _viewScrollStartPositionLine.Alpha = 0;
                _viewScrollEndPosition.Alpha = 0;
                _viewScrollEndPositionLine.Alpha = 0;
            });
        }

        public void LoadPeakFile(AudioFile audioFile)
        {
            WaveFormView.Frame = new RectangleF(0, 0, Bounds.Width, Bounds.Height);
            ContentSize = Bounds.Size;
            ContentOffset = new PointF(0, 0);
            WaveFormView.LoadPeakFile(audioFile);
        }

        public void SetWaveFormLength(long lengthBytes)
        {
            WaveFormView.Length = lengthBytes;
        }
    }
}
