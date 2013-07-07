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
using MPfm.Player.Objects;

namespace MPfm.iOS.Classes.Controls
{
    [Register("MPfmWaveFormScrollView")]
    public class MPfmWaveFormScrollView : UIScrollView
    {
        private float _zoomScale;
        private float _offsetRatio;
        private UILabel _lblZoom;
        private UIView _viewCenterLine;
        private float _scaleHeight = 22f;

        private WaveFormScrollViewMode _scrollViewMode = WaveFormScrollViewMode.SelectPosition;
        public WaveFormScrollViewMode ScrollViewMode
        {
            get
            {
                return _scrollViewMode;
            }
            set
            {
                _scrollViewMode = value;

                if (_scrollViewMode == WaveFormScrollViewMode.Standard)
                {
                    _viewCenterLine.Alpha = 0;
                }
                else if (_scrollViewMode == WaveFormScrollViewMode.SelectPosition)
                {
                    _viewCenterLine.Alpha = 1;
                }
            }
        }

        // TODO: Make this entirely private and add methods to set wave forms
        public MPfmWaveFormView WaveFormView { get; private set; }
        public MPfmWaveFormScaleView WaveFormScaleView { get; private set; }

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

            //WaveFormView = new MPfmWaveFormView(Bounds);
            WaveFormView = new MPfmWaveFormView(new RectangleF(0, _scaleHeight, Bounds.Width, Bounds.Height - _scaleHeight));
            AddSubview(WaveFormView);

            WaveFormScaleView = new MPfmWaveFormScaleView(new RectangleF(0, 0, Bounds.Width, _scaleHeight));
            AddSubview(WaveFormScaleView);

            _lblZoom = new UILabel(new RectangleF(0, 0, 60, 20));
            _lblZoom.BackgroundColor = GlobalTheme.BackgroundColor;
            _lblZoom.TextColor = UIColor.White;
            _lblZoom.Font = UIFont.FromName("HelveticaNeue", 12.0f);
            _lblZoom.TextAlignment = UITextAlignment.Center;
            _lblZoom.Text = "100.0%";
            _lblZoom.Alpha = 0;
            AddSubview(_lblZoom);

            _viewCenterLine = new UIView(new RectangleF(Bounds.Width / 2, 0, 1, Bounds.Height));
            _viewCenterLine.BackgroundColor = GlobalTheme.LightColor;
            _viewCenterLine.Alpha = 1;
            AddSubview(_viewCenterLine);

            ContentSize = WaveFormView.Bounds.Size;

            this.ViewForZoomingInScrollView = delegate {
                _offsetRatio = (ContentOffset.X / ContentSize.Width);
                return WaveFormView;
            };

            this.ZoomingStarted += delegate {
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

//            this.Scrolled += delegate(object sender, EventArgs e) {
//            };
//
//            this.DraggingStarted += delegate(object sender, EventArgs e) {
//            };
//
//            this.DraggingEnded += delegate(object sender, DraggingEventArgs e) {
//                if(e.Decelerate)
//                    return;
//            };
//
//            this.DecelerationEnded += delegate(object sender, EventArgs e) {
//            };
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            _lblZoom.Frame = new RectangleF(ContentOffset.X + ((Bounds.Width - 54) / 2), (Bounds.Height - 20) / 2, 54, 20);
            _viewCenterLine.Frame = new RectangleF(ContentOffset.X + (Bounds.Width / 2), 0, 1, Bounds.Height);
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
            
            if(ScrollViewMode == WaveFormScrollViewMode.Standard)
            {
                //WaveFormView.Frame = new RectangleF(WaveFormView.Frame.X, WaveFormView.Frame.Y, UIScreen.MainScreen.Bounds.Width * _zoomScale, WaveFormView.Frame.Height);
                WaveFormView.Frame = new RectangleF(WaveFormView.Frame.X, WaveFormView.Frame.Y, UIScreen.MainScreen.Bounds.Width * _zoomScale, WaveFormView.Frame.Height);
                WaveFormScaleView.Frame = new RectangleF(WaveFormScaleView.Frame.X, WaveFormScaleView.Frame.Y, UIScreen.MainScreen.Bounds.Width * _zoomScale, _scaleHeight);
                ContentSize = new SizeF(WaveFormView.Frame.Width, Bounds.Height);
                ContentOffset = new PointF(WaveFormView.Frame.Width * offsetRatio, 0);
            }
            else if(ScrollViewMode == WaveFormScrollViewMode.SelectPosition)
            {
                //WaveFormView.Frame = new RectangleF(WaveFormView.Frame.X, WaveFormView.Frame.Y, UIScreen.MainScreen.Bounds.Width * _zoomScale, WaveFormView.Frame.Height);
                WaveFormView.Frame = new RectangleF(WaveFormView.Frame.X, WaveFormView.Frame.Y, UIScreen.MainScreen.Bounds.Width * _zoomScale, WaveFormView.Frame.Height);
                WaveFormScaleView.Frame = new RectangleF(WaveFormScaleView.Frame.X, WaveFormScaleView.Frame.Y, UIScreen.MainScreen.Bounds.Width * _zoomScale, _scaleHeight);
                ContentSize = new SizeF(WaveFormView.Frame.Width + Bounds.Width, Bounds.Height);
                ContentOffset = new PointF(WaveFormView.Frame.Width * offsetRatio, 0);
            }
        }

        public void LoadPeakFile(AudioFile audioFile)
        {
            if(ScrollViewMode == WaveFormScrollViewMode.Standard)
            {
                WaveFormView.Frame = new RectangleF(0, _scaleHeight, Bounds.Width, Bounds.Height - _scaleHeight);
                WaveFormScaleView.Frame = new RectangleF(0, 0, Bounds.Width, _scaleHeight);
                //WaveFormScaleView.Hidden = true;
                ContentSize = Bounds.Size;
                ContentOffset = new PointF(0, 0);
            }
            else if(ScrollViewMode == WaveFormScrollViewMode.SelectPosition)
            {
                WaveFormView.Frame = new RectangleF(Bounds.Width / 2, _scaleHeight, Bounds.Width, Bounds.Height - _scaleHeight);
                WaveFormScaleView.Frame = new RectangleF(Bounds.Width / 2, 0, Bounds.Width, _scaleHeight);
                //WaveFormScaleView.Hidden = true;
                ContentSize = new SizeF(Bounds.Width * 2, Bounds.Height);
                ContentOffset = new PointF(0, 0);
            }
            WaveFormView.LoadPeakFile(audioFile);
        }

        public void SetWaveFormLength(long lengthBytes)
        {
            WaveFormView.Length = lengthBytes;
            WaveFormScaleView.Length = lengthBytes;
        }

        public void SetMarkers(IEnumerable<Marker> markers)
        {
            WaveFormView.SetMarkers(markers);
        }

        public override void Draw(RectangleF rect)
        {
            var context = UIGraphics.GetCurrentContext();
            CoreGraphicsHelper.FillRect(context, new RectangleF(0, 22, Bounds.Width, Bounds.Height), new CGColor(0, 0, 0, 0.25f));
        }
    }

    public enum WaveFormScrollViewMode
    {
        Standard = 0,
        SelectPosition = 1
    }
}
