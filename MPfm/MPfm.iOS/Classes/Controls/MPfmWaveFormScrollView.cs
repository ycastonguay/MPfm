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
using MPfm.Sound.AudioFiles;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Helpers;
using MPfm.iOS.Classes.Objects;
using MPfm.Player.Objects;

namespace MPfm.iOS.Classes.Controls
{
    [Register("MPfmWaveFormScrollView")]
    public class MPfmWaveFormScrollView : UIView
    {
        //private float _zoomScale;
        //private float _offsetRatio;
        private UITapGestureRecognizer _doubleTapGesture;
        private UIPinchGestureRecognizer _pinchGesture;
		private UIPanGestureRecognizer _panGesture;
        private UILabel _lblZoom;
        private UIView _viewCenterLine;
        private float _scaleHeight = 22f;
		private PointF _initialPanPoint = new PointF();

        private WaveFormScrollViewMode _scrollViewMode = WaveFormScrollViewMode.Standard;
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
                    _viewCenterLine.Alpha = 0;
                else if (_scrollViewMode == WaveFormScrollViewMode.SelectPosition)
                    _viewCenterLine.Alpha = 1;
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
                WaveFormScaleView.Zoom = _zoom;
                WaveFormView.Zoom = _zoom;
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
//            MinimumZoomScale = 1.0f;
//            MaximumZoomScale = 16.0f;
//            ShowsHorizontalScrollIndicator = true;
//            ShowsVerticalScrollIndicator = false;
//            AlwaysBounceHorizontal = false;
//            BouncesZoom = true;
			BackgroundColor = GlobalTheme.BackgroundColor;
			UserInteractionEnabled = true;
			MultipleTouchEnabled = true;

			_doubleTapGesture = new UITapGestureRecognizer(HandleDoubleTapGestureRecognizer);
            _doubleTapGesture.DelaysTouchesBegan = true;
            _doubleTapGesture.NumberOfTapsRequired = 2;
			AddGestureRecognizer(_doubleTapGesture);
            
			_pinchGesture = new UIPinchGestureRecognizer(HandlePinchGestureRecognizer);
            AddGestureRecognizer(_pinchGesture);

			_panGesture = new UIPanGestureRecognizer(HandlePanGestureRecognizer);
			_panGesture.MinimumNumberOfTouches = 1;
			_panGesture.MaximumNumberOfTouches = 1;
			AddGestureRecognizer(_panGesture);

            WaveFormView = new MPfmWaveFormView(new RectangleF(0, _scaleHeight, Bounds.Width, Bounds.Height - _scaleHeight));
            AddSubview(WaveFormView);

            WaveFormScaleView = new MPfmWaveFormScaleView(new RectangleF(0, 0, Bounds.Width, _scaleHeight));
            AddSubview(WaveFormScaleView);

			WaveFormView.AddGestureRecognizer(_doubleTapGesture);

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
            _viewCenterLine.Alpha = 0;
            AddSubview(_viewCenterLine);

//            this.ViewForZoomingInScrollView = delegate {
//                _offsetRatio = (ContentOffset.X / ContentSize.Width);
//				//Tracing.Log("WaveFormScrollView - ViewForZoomingInScrollView - offsetRatio: {0} contentOffset: {1} contentSize: {2}", _offsetRatio, ContentOffset, ContentSize);
//				return WaveFormView;
//            };
//
//            this.ZoomingStarted += delegate {
//                UIView.Animate(0.15, () => {
//                    _lblZoom.Alpha = 0.9f;
//                });
//            };
//
//            this.ZoomingEnded += delegate(object sender, ZoomingEndedEventArgs e) {
//                //WaveFormView.RefreshWaveFormBitmap();
//                WaveFormScaleView.SetNeedsDisplay();
//                UIView.Animate(0.25, () => {
//                    _lblZoom.Alpha = 0;
//                });
//            };
//
//            this.DidZoom += delegate(object sender, EventArgs e) {
//                UpdateZoomScale(Bounds.Width);
//                WaveFormScaleView.SetNeedsDisplay();
//            };

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

		private void HandleDoubleTapGestureRecognizer(UITapGestureRecognizer sender)
		{
			Zoom = 1.0f;
			Console.WriteLine("HandleDoubleTapGestureRecognizer");

			//_offsetRatio = 0;
			//UpdateZoomScale(Bounds.Width);
			//WaveFormView.RefreshWaveFormBitmap();
			//WaveFormScaleView.SetNeedsDisplay();

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
		}

		private void HandlePinchGestureRecognizer(UIPinchGestureRecognizer sender)
		{
			float scale = _pinchGesture.Scale;
			Console.WriteLine("HandlePinchGestureRecognizer - scale: {0}", scale);
			Zoom = scale;
		}

		private void HandlePanGestureRecognizer(UIPanGestureRecognizer sender)
		{
			var ptPan = sender.TranslationInView(this);

			if(sender.State == UIGestureRecognizerState.Began)
				_initialPanPoint = sender.View.Center;

			var ptTranslated = new PointF(_initialPanPoint.X + ptPan.X, _initialPanPoint.Y);

			Console.WriteLine("HandlePanGestureRecognizer - state: {0} initialPanPoint: {1} ptPan: {2} ptTranslated: {3}", _panGesture.State, _initialPanPoint, ptPan, ptTranslated);
		}

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

			WaveFormView.Frame = new RectangleF(0, _scaleHeight, Bounds.Width, Bounds.Height - _scaleHeight);
			WaveFormScaleView.Frame = new RectangleF(0, 0, Bounds.Width, _scaleHeight);
            _lblZoom.Frame = new RectangleF(((Bounds.Width - 70) / 2), (Bounds.Height - 20) / 2, 54, 20);
            _viewCenterLine.Frame = new RectangleF((Bounds.Width / 2), 0, 1, Bounds.Height);
        }

//        private void UpdateZoomScale(float width)
//        {
//            _zoomScale *= ZoomScale;
//            _zoomScale = (_zoomScale < MinimumZoomScale) ? MinimumZoomScale : _zoomScale;
//            _zoomScale = (_zoomScale > MaximumZoomScale) ? MaximumZoomScale : _zoomScale;
//            ZoomScale = 1.0f;
//            _lblZoom.Text = (_zoomScale * 100).ToString("0") + "%";
//			//Tracing.Log("MPfmWaveFormScrollView - UpdateZoomScale - zoomScale: {0} offsetRatio: {1} width: {2}", _zoomScale, _offsetRatio, width);
//			UpdateContentSizeAndOffset(width);
//        }

//		private void UpdateContentSizeAndOffset(float width)
//		{
//			//Tracing.Log("MPfmWaveFormScrollView - UpdateContentSizeAndOffset - zoomScale: {0} offsetRatio: {1} width: {2}", _zoomScale, _offsetRatio, width);
//			if(ScrollViewMode == WaveFormScrollViewMode.Standard)
//			{
//				WaveFormView.Frame = new RectangleF(WaveFormView.Frame.X, WaveFormView.Frame.Y, width * _zoomScale, WaveFormView.Frame.Height);
//				WaveFormScaleView.Frame = new RectangleF(WaveFormScaleView.Frame.X, WaveFormScaleView.Frame.Y, width * _zoomScale, _scaleHeight);
//				ContentSize = new SizeF(WaveFormView.Frame.Width, Bounds.Height);
//				ContentOffset = new PointF(WaveFormView.Frame.Width * _offsetRatio, 0);
//			}
//			else if(ScrollViewMode == WaveFormScrollViewMode.SelectPosition)
//			{
//				WaveFormView.Frame = new RectangleF(WaveFormView.Frame.X, WaveFormView.Frame.Y, width * _zoomScale, WaveFormView.Frame.Height);
//				WaveFormScaleView.Frame = new RectangleF(WaveFormScaleView.Frame.X, WaveFormScaleView.Frame.Y, width * _zoomScale, _scaleHeight);
//				ContentSize = new SizeF(WaveFormView.Frame.Width + width, Bounds.Height);
//				ContentOffset = new PointF(WaveFormView.Frame.Width * _offsetRatio, 0);
//			}
//		}

        public void LoadPeakFile(AudioFile audioFile)
        {
			//WaveFormScaleView.Hidden = true;
			//UserInteractionEnabled = false;
            if(ScrollViewMode == WaveFormScrollViewMode.Standard)
            {
                WaveFormView.Frame = new RectangleF(0, _scaleHeight, Bounds.Width, Bounds.Height - _scaleHeight);
                WaveFormScaleView.Frame = new RectangleF(0, 0, Bounds.Width, _scaleHeight);
            }
            else if(ScrollViewMode == WaveFormScrollViewMode.SelectPosition)
            {
                WaveFormView.Frame = new RectangleF(Bounds.Width / 2, _scaleHeight, Bounds.Width, Bounds.Height - _scaleHeight);
                WaveFormScaleView.Frame = new RectangleF(Bounds.Width / 2, 0, Bounds.Width, _scaleHeight);
            }
            WaveFormView.LoadPeakFile(audioFile);
            WaveFormScaleView.AudioFile = audioFile;
			WaveFormScaleView.SetNeedsDisplay();
        }

//        public void RefreshWaveFormBitmap(float width)
//        {
//			Console.WriteLine("WaveFormScrollView - RefreshWaveFormBitmap - width: {0} offsetRatio: {1}", width, _offsetRatio);
//            WaveFormView.RefreshWaveFormBitmap(width);
//            WaveFormScaleView.SetNeedsDisplay();
//			UpdateZoomScale(width);
//			//ZoomScale = 1.0f;
//			//UpdateContentSizeAndOffset(width);
//        }
//
        public void SetWaveFormLength(long lengthBytes)
        {
			Console.WriteLine("WaveFormScrollView - SetWaveFormLength - length: {0}", lengthBytes);
            WaveFormView.Length = lengthBytes;
            WaveFormScaleView.AudioFileLength = lengthBytes;
        }

		public void SetActiveMarker(Guid markerId)
		{
			WaveFormView.SetActiveMarker(markerId);
		}

        public void SetMarkers(IEnumerable<Marker> markers)
        {
            WaveFormView.SetMarkers(markers);
        }

		public void SetMarkerPosition(Marker marker)
		{
			WaveFormView.SetMarkerPosition(marker);
			//ProcessAutoScroll(marker.PositionBytes);
		}

        public void SetPosition(long position)
        {
            WaveFormView.Position = position;

//            // Process autoscroll
//            if (_zoomScale > 1)
//            {
//                float positionPercentage = (float)position / (float)WaveFormView.Length;
//                float cursorX = (positionPercentage * WaveFormView.Bounds.Width);
//                float scrollStartX = ContentOffset.X;
//                float scrollCenterX = ContentOffset.X + Bounds.Width / 2;
//                float scrollEndX = Bounds.Width + ContentOffset.X;
//                Console.WriteLine("WaveFormScrollView - AutoScroll - positionPct: {0} cursorX: {1} contentOffset.X: {2} waveFormView.Width: {3} scrollStartX: {4} scrollCenterX: {5} scrollEndX: {6}", positionPercentage, cursorX, ContentOffset.X, WaveFormView.Bounds.Width, scrollStartX, scrollCenterX, scrollEndX);
////                if (cursorX > scrollEndX)
////                    Console.WriteLine("WaveFormScrollView - Cursor is offscreen to the RIGHT!");
////                else if (cursorX < scrollStartX)
////                    Console.WriteLine("WaveFormScrollView - Cursor is offscreen to the LEFT!");
//
//                if (cursorX != scrollCenterX)
//                {
//                    Console.WriteLine("WaveFormScrollView - Cursor isn't centered!");
//
//                    if (cursorX < scrollCenterX)
//                    {
//                        Console.WriteLine("WaveFormScrollView - Cursor isn't centered - The cursor is left of center X!");
//                        if (ContentOffset.X > (Bounds.Width / 2f))
//                        {
//                            //float newContentOffsetX = scrollCenterX - (Bounds.Width / 2f);
//                            float newContentOffsetX = cursorX - (Bounds.Width / 2f);
//                            Console.WriteLine("WaveFormScrollView - Cursor isn't centered - There is space on the left; AUTOCENTER! currentContentOffsetX: {0} newContentOffsetX: {1}", ContentOffset.X, newContentOffsetX);
//                            ContentOffset = new PointF(newContentOffsetX, 0);
//                        }
//                    }
//                    else if(cursorX > scrollCenterX)
//                    {
//                        Console.WriteLine("WaveFormScrollView - Cursor isn't centered - The cursor is right of center X!");
//                        if (ContentOffset.X < WaveFormView.Bounds.Width - Bounds.Width)
//                        {
//                            float newContentOffsetX = cursorX - (Bounds.Width / 2f);
//                            Console.WriteLine("WaveFormScrollView - Cursor isn't centered - There is space on the right; AUTOCENTER! currentContentOffsetX: {0} newContentOffsetX: {1}", ContentOffset.X, newContentOffsetX);
//                            ContentOffset = new PointF(newContentOffsetX, 0);
//                        }
//                    }
//                }
//            }
        }

        public void ShowSecondaryPosition(bool show)
        {
            WaveFormView.ShowSecondaryPosition = show;
        }

        public void SetSecondaryPosition(long position)
		{
			WaveFormView.SecondaryPosition = position;
			//ProcessAutoScroll(position);
		}

//		private void ProcessAutoScroll(long position)
//		{
//            // Process autoscroll
//            if (_zoomScale > 1)
//            {
//                float positionPercentage = (float)position / (float)WaveFormView.Length;
//                float cursorX = (positionPercentage * WaveFormView.Bounds.Width);
//				//float scrollStartX = ContentOffset.X;
//                float scrollCenterX = ContentOffset.X + Bounds.Width / 2;
//				//float scrollEndX = Bounds.Width + ContentOffset.X;
//                //Console.WriteLine("WaveFormScrollView - AutoScroll - positionPct: {0} cursorX: {1} contentOffset.X: {2} waveFormView.Width: {3} scrollStartX: {4} scrollCenterX: {5} scrollEndX: {6}", positionPercentage, cursorX, ContentOffset.X, WaveFormView.Bounds.Width, scrollStartX, scrollCenterX, scrollEndX);
//
//                if (cursorX != scrollCenterX)
//                {
//                    if (cursorX < scrollCenterX)
//                    {
//                        //Console.WriteLine("WaveFormScrollView - Cursor isn't centered - The cursor is left of center X!");
//                        if (ContentOffset.X >= 0)
//                        {
//                            float newContentOffsetX = cursorX - (Bounds.Width / 2f);
//                            newContentOffsetX = newContentOffsetX < 0 ? 0 : newContentOffsetX;
//                            //Console.WriteLine("WaveFormScrollView - Cursor isn't centered - There is space on the left; AUTOCENTER! currentContentOffsetX: {0} newContentOffsetX: {1}", ContentOffset.X, newContentOffsetX);
//                            ContentOffset = new PointF(newContentOffsetX, 0);
//                        }
//                    }
//                    else if(cursorX > scrollCenterX)
//                    {
//                        //Console.WriteLine("WaveFormScrollView - Cursor isn't centered - The cursor is right of center X!");
//                        if (ContentOffset.X < WaveFormView.Bounds.Width - Bounds.Width)
//                        {
//                            float newContentOffsetX = cursorX - (Bounds.Width / 2f);
//                            newContentOffsetX = newContentOffsetX > WaveFormView.Bounds.Width - Bounds.Width ? WaveFormView.Bounds.Width - Bounds.Width : newContentOffsetX;
//                            //Console.WriteLine("WaveFormScrollView - Cursor isn't centered - There is space on the right; AUTOCENTER! currentContentOffsetX: {0} newContentOffsetX: {1}", ContentOffset.X, newContentOffsetX);
//                            ContentOffset = new PointF(newContentOffsetX, 0);
//                        }
//                    }
//                }
//            }
//        }

    }

    public enum WaveFormScrollViewMode
    {
        Standard = 0,
        SelectPosition = 1
    }
}
