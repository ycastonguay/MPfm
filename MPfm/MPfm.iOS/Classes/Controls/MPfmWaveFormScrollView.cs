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
                UpdateZoomScale(Bounds.Width);
                WaveFormView.RefreshWaveFormBitmap();
                WaveFormScaleView.SetNeedsDisplay();
                
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

            WaveFormView = new MPfmWaveFormView(new RectangleF(0, _scaleHeight, Bounds.Width, Bounds.Height - _scaleHeight));
            WaveFormView.WaveFormCacheManager.GeneratePeakFileEndedEvent += (object sender, GeneratePeakFileEventArgs e) => {
                WaveFormScaleView.Hidden = false;
                UserInteractionEnabled = true;
            };
            WaveFormView.WaveFormCacheManager.LoadedPeakFileSuccessfullyEvent += (object sender, LoadPeakFileEventArgs e) => {
                WaveFormScaleView.Hidden = false;
                UserInteractionEnabled = true;
            };
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
            _viewCenterLine.Alpha = 0;
            AddSubview(_viewCenterLine);

            ContentSize = WaveFormView.Bounds.Size;

            this.ViewForZoomingInScrollView = delegate {
                _offsetRatio = (ContentOffset.X / ContentSize.Width);
				//Tracing.Log("WaveFormScrollView - ViewForZoomingInScrollView - offsetRatio: {0} contentOffset: {1} contentSize: {2}", _offsetRatio, ContentOffset, ContentSize);
				return WaveFormView;
            };

            this.ZoomingStarted += delegate {
                UIView.Animate(0.15, () => {
                    _lblZoom.Alpha = 0.9f;
                });
            };

            this.ZoomingEnded += delegate(object sender, ZoomingEndedEventArgs e) {
                WaveFormView.RefreshWaveFormBitmap();
                WaveFormScaleView.SetNeedsDisplay();
                UIView.Animate(0.25, () => {
                    _lblZoom.Alpha = 0;
                });
            };

            this.DidZoom += delegate(object sender, EventArgs e) {
                UpdateZoomScale(Bounds.Width);
                WaveFormScaleView.SetNeedsDisplay();
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

			//Tracing.Log("WaveFormScrollView - LayoutSubviews - Bounds.Width: {0} - Frame.Width: {1}", Bounds.Width, Frame.Width);
            _lblZoom.Frame = new RectangleF(ContentOffset.X + ((Bounds.Width - 70) / 2), (Bounds.Height - 20) / 2, 54, 20);
            _viewCenterLine.Frame = new RectangleF(ContentOffset.X + (Bounds.Width / 2), 0, 1, Bounds.Height);
        }

        private void UpdateZoomScale(float width)
        {
            _zoomScale *= ZoomScale;
            _zoomScale = (_zoomScale < MinimumZoomScale) ? MinimumZoomScale : _zoomScale;
            _zoomScale = (_zoomScale > MaximumZoomScale) ? MaximumZoomScale : _zoomScale;
            ZoomScale = 1.0f;
            _lblZoom.Text = (_zoomScale * 100).ToString("0") + "%";
			//Tracing.Log("MPfmWaveFormScrollView - UpdateZoomScale - zoomScale: {0} offsetRatio: {1} width: {2}", _zoomScale, _offsetRatio, width);
			UpdateContentSizeAndOffset(width);
        }

		private void UpdateContentSizeAndOffset(float width)
		{
			//Tracing.Log("MPfmWaveFormScrollView - UpdateContentSizeAndOffset - zoomScale: {0} offsetRatio: {1} width: {2}", _zoomScale, _offsetRatio, width);
			if(ScrollViewMode == WaveFormScrollViewMode.Standard)
			{
				WaveFormView.Frame = new RectangleF(WaveFormView.Frame.X, WaveFormView.Frame.Y, width * _zoomScale, WaveFormView.Frame.Height);
				WaveFormScaleView.Frame = new RectangleF(WaveFormScaleView.Frame.X, WaveFormScaleView.Frame.Y, width * _zoomScale, _scaleHeight);
				ContentSize = new SizeF(WaveFormView.Frame.Width, Bounds.Height);
				ContentOffset = new PointF(WaveFormView.Frame.Width * _offsetRatio, 0);
			}
			else if(ScrollViewMode == WaveFormScrollViewMode.SelectPosition)
			{
				WaveFormView.Frame = new RectangleF(WaveFormView.Frame.X, WaveFormView.Frame.Y, width * _zoomScale, WaveFormView.Frame.Height);
				WaveFormScaleView.Frame = new RectangleF(WaveFormScaleView.Frame.X, WaveFormScaleView.Frame.Y, width * _zoomScale, _scaleHeight);
				ContentSize = new SizeF(WaveFormView.Frame.Width + width, Bounds.Height);
				ContentOffset = new PointF(WaveFormView.Frame.Width * _offsetRatio, 0);
			}
		}

        public void LoadPeakFile(AudioFile audioFile)
        {
            WaveFormScaleView.Hidden = true;
            UserInteractionEnabled = false;
            if(ScrollViewMode == WaveFormScrollViewMode.Standard)
            {
                WaveFormView.Frame = new RectangleF(0, _scaleHeight, Bounds.Width, Bounds.Height - _scaleHeight);
                WaveFormScaleView.Frame = new RectangleF(0, 0, Bounds.Width, _scaleHeight);
                ContentSize = Bounds.Size;
                ContentOffset = new PointF(0, 0);
            }
            else if(ScrollViewMode == WaveFormScrollViewMode.SelectPosition)
            {
                WaveFormView.Frame = new RectangleF(Bounds.Width / 2, _scaleHeight, Bounds.Width, Bounds.Height - _scaleHeight);
                WaveFormScaleView.Frame = new RectangleF(Bounds.Width / 2, 0, Bounds.Width, _scaleHeight);
                ContentSize = new SizeF(Bounds.Width * 2, Bounds.Height);
                ContentOffset = new PointF(0, 0);
            }
            WaveFormView.LoadPeakFile(audioFile);
            WaveFormScaleView.AudioFile = audioFile;
        }

        public void RefreshWaveFormBitmap(float width)
        {
			//Tracing.Log("WaveFormScrollView - RefreshWaveFormBitmap - width: {0} offsetRatio: {1}", width, _offsetRatio);
            WaveFormView.RefreshWaveFormBitmap(width);
            WaveFormScaleView.SetNeedsDisplay();
			UpdateZoomScale(width);
			//ZoomScale = 1.0f;
			//UpdateContentSizeAndOffset(width);
        }

        public void SetWaveFormLength(long lengthBytes)
        {
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
			ProcessAutoScroll(marker.PositionBytes);
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
			ProcessAutoScroll(position);
		}

		private void ProcessAutoScroll(long position)
		{
            // Process autoscroll
            if (_zoomScale > 1)
            {
                float positionPercentage = (float)position / (float)WaveFormView.Length;
                float cursorX = (positionPercentage * WaveFormView.Bounds.Width);
				//float scrollStartX = ContentOffset.X;
                float scrollCenterX = ContentOffset.X + Bounds.Width / 2;
				//float scrollEndX = Bounds.Width + ContentOffset.X;
                //Console.WriteLine("WaveFormScrollView - AutoScroll - positionPct: {0} cursorX: {1} contentOffset.X: {2} waveFormView.Width: {3} scrollStartX: {4} scrollCenterX: {5} scrollEndX: {6}", positionPercentage, cursorX, ContentOffset.X, WaveFormView.Bounds.Width, scrollStartX, scrollCenterX, scrollEndX);

                if (cursorX != scrollCenterX)
                {
                    if (cursorX < scrollCenterX)
                    {
                        //Console.WriteLine("WaveFormScrollView - Cursor isn't centered - The cursor is left of center X!");
                        if (ContentOffset.X >= 0)
                        {
                            float newContentOffsetX = cursorX - (Bounds.Width / 2f);
                            newContentOffsetX = newContentOffsetX < 0 ? 0 : newContentOffsetX;
                            //Console.WriteLine("WaveFormScrollView - Cursor isn't centered - There is space on the left; AUTOCENTER! currentContentOffsetX: {0} newContentOffsetX: {1}", ContentOffset.X, newContentOffsetX);
                            ContentOffset = new PointF(newContentOffsetX, 0);
                        }
                    }
                    else if(cursorX > scrollCenterX)
                    {
                        //Console.WriteLine("WaveFormScrollView - Cursor isn't centered - The cursor is right of center X!");
                        if (ContentOffset.X < WaveFormView.Bounds.Width - Bounds.Width)
                        {
                            float newContentOffsetX = cursorX - (Bounds.Width / 2f);
                            newContentOffsetX = newContentOffsetX > WaveFormView.Bounds.Width - Bounds.Width ? WaveFormView.Bounds.Width - Bounds.Width : newContentOffsetX;
                            //Console.WriteLine("WaveFormScrollView - Cursor isn't centered - There is space on the right; AUTOCENTER! currentContentOffsetX: {0} newContentOffsetX: {1}", ContentOffset.X, newContentOffsetX);
                            ContentOffset = new PointF(newContentOffsetX, 0);
                        }
                    }
                }
            }
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
