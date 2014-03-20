// Copyright � 2011-2013 Yanick Castonguay
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
using System.Timers;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using MPfm.GenericControls.Basics;
using MPfm.MVP.Services;
using MPfm.Sound.AudioFiles;

namespace org.sessionsapp.android
{
    public class WaveFormScrollView : LinearLayout
    {
        private DateTime _lastZoomUpdate;
        private Timer _timerFadeOutZoomLabel;
        private ScaleGestureDetector _scaleGestureDetector;
        private GestureDetector _panGestureDetector;

        public WaveFormScaleView ScaleView { get; private set; }
        public WaveFormView WaveView { get; private set; }

        private float _zoom = 1;
        public float Zoom
        {
            get
            {
                return _zoom;
            }
            set
            {
                //_lblZoom.StringValue = string.Format("{0:0}%", value * 100);
                _zoom = value;
                WaveView.Zoom = value;
                ScaleView.Zoom = value;
                _lastZoomUpdate = DateTime.Now;

                //if (_lblZoom.AlphaValue == 0)
                //{
                //    NSAnimationContext.BeginGrouping();
                //    NSAnimationContext.CurrentContext.Duration = 0.2;
                //    (_lblZoom.Animator as NSTextField).AlphaValue = 1;
                //    NSAnimationContext.EndGrouping();
                //}
            }
        }

        protected WaveFormScrollView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            Initialize();
        }

        public WaveFormScrollView(Context context) : base(context)
        {
            Initialize();
        }

        public WaveFormScrollView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize();
        }

        public WaveFormScrollView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            Initialize();
        }

        private void Initialize()
        {
            _panGestureDetector = new GestureDetector(Context, new PanListener(this));
            _scaleGestureDetector = new ScaleGestureDetector(Context, new ScaleListener(this));
            SetBackgroundColor(Resources.GetColor(MPfm.Android.Resource.Color.background));
            //SetBackgroundColor(Color.HotPink);
            Orientation = Orientation.Vertical;

            int height = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 22, Resources.DisplayMetrics);
            ScaleView = new WaveFormScaleView(Context);
            ScaleView.SetBackgroundColor(Color.Purple);
            AddView(ScaleView, new LinearLayout.LayoutParams(LayoutParams.WrapContent, height));

            WaveView = new WaveFormView(Context);
            WaveView.SetBackgroundColor(Color.DarkRed);
            AddView(WaveView, new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.FillParent));

            _timerFadeOutZoomLabel = new Timer(100);
            _timerFadeOutZoomLabel.Elapsed += HandleTimerFadeOutZoomLabelElapsed;
            _timerFadeOutZoomLabel.Start();
        }

        public void LoadPeakFile(AudioFile audioFile)
        {
            WaveView.LoadPeakFile(audioFile);
            ScaleView.AudioFile = audioFile;
            ScaleView.Invalidate();
        }

        public void SetWaveFormLength(long lengthBytes)
        {
            WaveView.SetWaveFormLength(lengthBytes);
            ScaleView.AudioFileLength = lengthBytes;
        }

        public override bool DispatchTouchEvent(MotionEvent e)
        {
            base.DispatchTouchEvent(e);
            _panGestureDetector.OnTouchEvent(e);
            _scaleGestureDetector.OnTouchEvent(e);
            return true;
            //return _scaleGestureDetector.OnTouchEvent(e);
        }

        private void SetContentOffsetX(float x)
        {
            float contentOffsetX = x;
            float maxX = (Width * Zoom) - Width;
            contentOffsetX = Math.Max(contentOffsetX, 0);
            contentOffsetX = Math.Min(contentOffsetX, maxX);
            WaveView.ContentOffset = new BasicPoint(contentOffsetX, 0);
            ScaleView.ContentOffset = new BasicPoint(contentOffsetX, 0);
        }

        private void HandleTimerFadeOutZoomLabelElapsed(object sender, ElapsedEventArgs e)
        {
            Post(() =>
            {
                if (DateTime.Now - _lastZoomUpdate > new TimeSpan(0, 0, 0, 0, 700))
                {
                    //Console.WriteLine("WaveFormScrollView - HandleTimerFadeOutZoomLabelElapsed - Refreshing wave form bitmap...");
                    //WaveView.RefreshWaveFormBitmap();
                }
            });
            //InvokeOnMainThread(() =>
            //{
            //    //Console.WriteLine("HandleTimerFadeOutZoomLabelElapsed - _lblZoom.AlphaValue: {0} - timeSpan since last update: {1}", _lblZoom.AlphaValue, DateTime.Now - _lastZoomUpdate);
            //    if (_lblZoom.AlphaValue == 1 && DateTime.Now - _lastZoomUpdate > new TimeSpan(0, 0, 0, 0, 700))
            //    {
            //        //Console.WriteLine("HandleTimerFadeOutZoomLabelElapsed - Fade out");
            //        NSAnimationContext.BeginGrouping();
            //        NSAnimationContext.CurrentContext.Duration = 0.2;
            //        (_lblZoom.Animator as NSTextField).AlphaValue = 0;
            //        NSAnimationContext.EndGrouping();

            //        WaveFormView.RefreshWaveFormBitmap();
            //    }
            //});
        }

        private class PanListener : GestureDetector.SimpleOnGestureListener
        {
            private readonly WaveFormScrollView _scrollView;

            public PanListener(WaveFormScrollView scrollView)
            {
                _scrollView = scrollView;
            }

            public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
            {
                Console.WriteLine("PanListener - OnFling - velocityX: {0} velocityY: {1}", velocityX, velocityY);
                return base.OnFling(e1, e2, velocityX, velocityY);
            }
            
            public override bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
            {
                //return base.OnScroll(e1, e2, distanceX, distanceY);                
                //Console.WriteLine("PanListener - OnScroll - distanceX: {0} distanceY: {1}", distanceX, distanceY);
                float x = _scrollView.WaveView.ContentOffset.X + distanceX;
                _scrollView.SetContentOffsetX(x);
                return true;
            }
        }

        private class ScaleListener : ScaleGestureDetector.SimpleOnScaleGestureListener
        {
            private readonly WaveFormScrollView _scrollView;
            private float _startZoom;
            private float _startContentOffsetX;

            public ScaleListener(WaveFormScrollView scrollView)
            {
                _scrollView = scrollView;
            }

            public override bool OnScaleBegin(ScaleGestureDetector detector)
            {
                //Console.WriteLine("ScaleListener - OnScaleBegin - scaleFactor: {0}", detector.ScaleFactor);
                _startZoom = _scrollView.Zoom;
                _startContentOffsetX = _scrollView.WaveView.ContentOffset.X;
                SetScrollViewScale(detector.ScaleFactor);
                return base.OnScaleBegin(detector);
            }

            public override bool OnScale(ScaleGestureDetector detector)
            {
                //Console.WriteLine("ScaleListener - OnScale - scaleFactor: {0}", detector.ScaleFactor);
                SetScrollViewScale(detector.ScaleFactor);
                return base.OnScale(detector);
            }

            public override void OnScaleEnd(ScaleGestureDetector detector)
            {
                //Console.WriteLine("ScaleListener - OnScaleEnd - scaleFactor: {0}", detector.ScaleFactor);
                SetScrollViewScale(detector.ScaleFactor);
                base.OnScaleEnd(detector);
            }

            private void SetScrollViewScale(float scale)
            {
                float adjustedScale = scale;
                //float deltaZoom = adjustedScale / _scrollView.Zoom;

                // Adjust content offset with new zoom value
                // TODO: Adjust content offset X when zooming depending on mouse location
                //contentOffsetX = WaveFormView.ContentOffset.X + (WaveFormView.ContentOffset.X * (newZoom - Zoom));
                //float contentOffsetX = _scrollView.WaveView.ContentOffset.X * deltaZoom;
                //float contentOffsetX = _scrollView.WaveView.ContentOffset.X * adjustedScale;
                float contentOffsetX = _startContentOffsetX * adjustedScale;
                float zoom = _startZoom * adjustedScale;
                zoom = Math.Max(1, zoom);
                zoom = Math.Min(32, zoom);
                _scrollView.Zoom = zoom;
                _scrollView.SetContentOffsetX(contentOffsetX);
                //Console.WriteLine("ScaleListener - SetScrollViewScale - scale: {0} zoom: {1} contentOffset: {2}", scale, _scrollView.Zoom, _scrollView.WaveView.ContentOffset);

                //_scrollView.ScaleX = scale > 1 ? scale : 1;
                //_scrollView.ScaleY = 1;
            }
        }
    }
}