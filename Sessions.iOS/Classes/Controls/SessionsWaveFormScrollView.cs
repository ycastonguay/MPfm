// Copyright © 2011-2013 Yanick Castonguay
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
using CoreGraphics;
using Sessions.Sound.AudioFiles;
using CoreGraphics;
using Foundation;
using UIKit;
using Sessions.iOS.Helpers;
using Sessions.iOS.Classes.Objects;
using Sessions.GenericControls.Basics;
using Sessions.Sound.Objects;
using org.sessionsapp.player;

namespace Sessions.iOS.Classes.Controls
{
    [Register("SessionsWaveFormScrollView")]
    public class SessionsWaveFormScrollView : UIView
    {
		private float _initialZoom;
		private float _scaleHeight = 22f;
		private BasicPoint _initialContentOffset = new BasicPoint();
        private UITapGestureRecognizer _doubleTapGesture;
        private UIPinchGestureRecognizer _pinchGesture;
		private UIPanGestureRecognizer _panGesture;
        private UILabel _lblZoom;
        private UIView _viewCenterLine;

		public bool IsAutoScrollEnabled { get; set; }

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
                OnZoomChanged(new EventArgs());
            }
        }        

        public bool IsEmpty { get { return WaveFormView.IsEmpty; } }
        public bool ShowMarkers { get { return WaveFormView.ShowMarkers; } set { WaveFormView.ShowMarkers = value; } }
        public bool ShowLoops { get { return WaveFormView.ShowLoops; } set { WaveFormView.ShowLoops = value; } }

        public event EventHandler ZoomChanged;

        // TODO: Make this entirely private and add methods to set wave forms
        public SessionsWaveFormView WaveFormView { get; private set; }
        public SessionsWaveFormScaleView WaveFormScaleView { get; private set; }

        public SessionsWaveFormScrollView(IntPtr handle) 
            : base (handle)
        {
            Initialize();
        }
        
        public SessionsWaveFormScrollView(CGRect frame) 
            : base(frame)
        {
            Initialize();
        }

        private void Initialize()
        {
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

            WaveFormView = new SessionsWaveFormView(new CGRect(0, _scaleHeight, Bounds.Width, Bounds.Height - _scaleHeight));
            AddSubview(WaveFormView);

            WaveFormScaleView = new SessionsWaveFormScaleView(new CGRect(0, 0, Bounds.Width, _scaleHeight));
            AddSubview(WaveFormScaleView);

			WaveFormView.AddGestureRecognizer(_doubleTapGesture);

            _lblZoom = new UILabel(new CGRect(0, 0, 60, 20));
            _lblZoom.BackgroundColor = GlobalTheme.BackgroundColor;
            _lblZoom.TextColor = UIColor.White;
			_lblZoom.Font = UIFont.FromName("HelveticaNeue", 11);
            _lblZoom.TextAlignment = UITextAlignment.Center;
            _lblZoom.Text = "100.0%";
            _lblZoom.Alpha = 0;
            AddSubview(_lblZoom);

            _viewCenterLine = new UIView(new CGRect(Bounds.Width / 2, 0, 1, Bounds.Height));
            _viewCenterLine.BackgroundColor = GlobalTheme.LightColor;
            _viewCenterLine.Alpha = 0;
            AddSubview(_viewCenterLine);
        }

        protected void OnZoomChanged(EventArgs e)
        {
            if(ZoomChanged != null)
                ZoomChanged(this, e);
        }

		private void HandleDoubleTapGestureRecognizer(UITapGestureRecognizer sender)
		{
			//Console.WriteLine("HandleDoubleTapGestureRecognizer");
			WaveFormView.ContentOffset = new BasicPoint();
			WaveFormScaleView.ContentOffset = new BasicPoint();
			Zoom = 1.0f;
			_lblZoom.Text = "100.0%";
			UIView.Animate(0.15, () => {
				_lblZoom.Alpha = 0.9f;
			}, () => {
				UIView.Animate(1.5, () => {
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
			if (sender.State == UIGestureRecognizerState.Began)
			{
				_initialZoom = Zoom;
				_initialContentOffset = WaveFormView.ContentOffset;
				UIView.Animate(0.2, () => _lblZoom.Alpha = 0.9f);
			}
			else if (sender.State == UIGestureRecognizerState.Ended)
			{
				UIView.Animate(0.2, 0.8, UIViewAnimationOptions.CurveEaseOut, () => _lblZoom.Alpha = 0, () => {});
			}

			var location = sender.LocationInView(this);
			nfloat newZoom = _initialZoom * _pinchGesture.Scale;
			nfloat deltaZoom = newZoom / Zoom;
			nfloat originPointX = IsAutoScrollEnabled ? WaveFormView.ContentOffset.X + (Frame.Width / 2) : location.X + WaveFormView.ContentOffset.X;
			nfloat distanceToOffsetX = originPointX - WaveFormView.ContentOffset.X;
			nfloat contentOffsetX = (originPointX * deltaZoom) - distanceToOffsetX;
            Zoom = (float)Math.Max(1, newZoom);
			SetContentOffsetX(contentOffsetX);
			_lblZoom.Text = (Zoom * 100).ToString("0") + "%";
			//Console.WriteLine("HandlePinchGestureRecognizer - initialZoom: {0} newZoom: {1}", _initialZoom, newZoom);
		}

		private void HandlePanGestureRecognizer(UIPanGestureRecognizer sender)
		{
			var ptPan = sender.TranslationInView(this);

			if(sender.State == UIGestureRecognizerState.Began)
				_initialContentOffset = WaveFormView.ContentOffset;

			SetContentOffsetX(_initialContentOffset.X - ptPan.X); // invert pan direction

//			if (sender.State == UIGestureRecognizerState.Ended)
//			{
//				var velocity = sender.VelocityInView(this);
//				float velocityX = velocity.X * 0.2f;
//				float finalX = ptTranslated.X + velocityX;
//				float finalY = _initialContentOffset.Y;
//				float animationDuration = (Math.Abs(velocityX) * 0.0002f) + 0.2f;
//				Console.WriteLine("HandlePanGestureRecognizer - velocityX: {0} animationDuration: {1} finalX: {2}", velocityX, animationDuration, finalX);
//			}
			//Console.WriteLine("HandlePanGestureRecognizer - state: {0} initialContentOffset: {1} ptPan: {2}", _panGesture.State, _initialContentOffset, ptPan);
		}

		private void SetContentOffsetX(nfloat x)
		{
			nfloat contentOffsetX = x;
			nfloat maxX = (Frame.Width * Zoom) - Frame.Width;
            contentOffsetX = (nfloat)Math.Max(contentOffsetX, 0);
            contentOffsetX = (nfloat)Math.Min(contentOffsetX, maxX);
            WaveFormView.ContentOffset = new BasicPoint((float)contentOffsetX, 0);
            WaveFormScaleView.ContentOffset = new BasicPoint((float)contentOffsetX, 0);
		}

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

			WaveFormView.Frame = new CGRect(0, _scaleHeight, Bounds.Width, Bounds.Height - _scaleHeight);
			WaveFormScaleView.Frame = new CGRect(0, 0, Bounds.Width, _scaleHeight);
            _lblZoom.Frame = new CGRect(((Bounds.Width - 70) / 2), (Bounds.Height - 20) / 2, 54, 20);
            _viewCenterLine.Frame = new CGRect((Bounds.Width / 2), 0, 1, Bounds.Height);

			//Console.WriteLine("=========>>>>>>>> WaveFormScrollView - LayoutSubviews - frame: {0} orientation: {1}", Frame, UIDevice.CurrentDevice.Orientation);
			WaveFormScaleView.SetNeedsDisplay();
        }

        public void LoadPeakFile(AudioFile audioFile)
        {
			//WaveFormScaleView.Hidden = true;
			//UserInteractionEnabled = false;
            if(ScrollViewMode == WaveFormScrollViewMode.Standard)
            {
                WaveFormView.Frame = new CGRect(0, _scaleHeight, Bounds.Width, Bounds.Height - _scaleHeight);
                WaveFormScaleView.Frame = new CGRect(0, 0, Bounds.Width, _scaleHeight);
            }
            else if(ScrollViewMode == WaveFormScrollViewMode.SelectPosition)
            {
                WaveFormView.Frame = new CGRect(Bounds.Width / 2, _scaleHeight, Bounds.Width, Bounds.Height - _scaleHeight);
                WaveFormScaleView.Frame = new CGRect(Bounds.Width / 2, 0, Bounds.Width, _scaleHeight);
            }
            WaveFormView.LoadPeakFile(audioFile);
            WaveFormScaleView.AudioFile = audioFile;
			WaveFormScaleView.SetNeedsDisplay();
        }

        public void CancelPeakFile()
        {
            WaveFormView.CancelPeakFile();
        }

        public void SetFloatingPoint(bool useFloatingPoint)
        {
            WaveFormView.UseFloatingPoint = useFloatingPoint;
            WaveFormScaleView.UseFloatingPoint = useFloatingPoint;
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
		}

        public void SetLoop(SSPLoop loop)
        {
            WaveFormView.SetLoop(loop);
        }

        public void SetPosition(long position)
        {
            WaveFormView.Position = position;
        }

        public void ShowSecondaryPosition(bool show)
        {
            WaveFormView.ShowSecondaryPosition = show;
        }

        public void SetSecondaryPosition(long position)
		{
            // Setting the position will process the auto scroll
			WaveFormView.SecondaryPosition = position;

            // Sync the content offset of the scale view to the wave form view
            WaveFormScaleView.ContentOffset = new BasicPoint(WaveFormView.ContentOffset.X, 0);
            //Console.WriteLine("===>>> SetSecondaryPosition -- position: {0} - WaveFormScaleView.ContentOffset.X: {1} - WaveFormView.ContentOffset.X: {2}", position, WaveFormScaleView.ContentOffset.X, WaveFormView.ContentOffset.X);
		}
    }

    public enum WaveFormScrollViewMode
    {
        Standard = 0,
        SelectPosition = 1
    }
}
