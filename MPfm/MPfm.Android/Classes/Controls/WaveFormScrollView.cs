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
using System.Timers;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Java.Lang;
using MPfm.Android;
using MPfm.GenericControls.Basics;
using MPfm.MVP.Services;
using MPfm.Sound.AudioFiles;
using Math = System.Math;

namespace org.sessionsapp.android
{
    public class WaveFormScrollView : FrameLayout
    {
        private DateTime _lastZoomUpdate;
        private Timer _timerFadeOutZoomLabel;
        private TextView _lblZoom;

        public WaveFormLayout WaveformLayout { get; private set; }
        public WaveFormScaleView ScaleView { get { return WaveformLayout.ScaleView; } }
        public WaveFormView WaveView { get { return WaveformLayout.WaveView; } }

        private float _zoom = 1;
        public float Zoom
        {
            get
            {
                return _zoom;
            }
            set
            {
                _lblZoom.Text = string.Format("{0:0}%", value * 100);
                _zoom = value;
                WaveView.Zoom = value;
                ScaleView.Zoom = value;
                _lastZoomUpdate = DateTime.Now;
                _lblZoom.Visibility = ViewStates.Visible;

                //var animFadeIn = new AlphaAnimation(0, 1);
                //animFadeIn.Interpolator = new DecelerateInterpolator();
                //animFadeIn.Duration = 200;
                //_lblZoom.Animation = animFadeIn;

                //Animation animation = AnimationUtils.LoadAnimation(Context, Resource.Animation.fade_in);
                //_lblZoom.StartAnimation(animation);

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
            //SetBackgroundColor(Color.HotPink);

            WaveformLayout = new WaveFormLayout(this);
            WaveformLayout.LayoutParameters = new FrameLayout.LayoutParams(LayoutParams.FillParent, LayoutParams.FillParent, GravityFlags.CenterHorizontal | GravityFlags.CenterVertical);
            AddView(WaveformLayout);

            _lblZoom = new TextView(Context);
            _lblZoom.Typeface = Typeface.Default;
            _lblZoom.SetTextSize(ComplexUnitType.Dip, 11);
            _lblZoom.LayoutParameters = new FrameLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent, GravityFlags.CenterHorizontal | GravityFlags.CenterVertical);
            _lblZoom.SetPadding(12, 4, 12, 4);
            _lblZoom.SetBackgroundColor(new Color(32, 40, 46, 150));
            _lblZoom.SetTextColor(Color.White);
            _lblZoom.Text = "100%";
            _lblZoom.Gravity = GravityFlags.CenterVertical | GravityFlags.CenterHorizontal;            
            _lblZoom.Visibility = ViewStates.Gone;
            //_lblZoom.Alpha = 0;
            AddView(_lblZoom);

            _timerFadeOutZoomLabel = new Timer(100);
            _timerFadeOutZoomLabel.Elapsed += HandleTimerFadeOutZoomLabelElapsed;
            _timerFadeOutZoomLabel.Start();
        }

        public void LoadPeakFile(AudioFile audioFile)
        {
            _zoom = 1;
            WaveView.ContentOffset = new BasicPoint(0, 0);
            WaveView.Zoom = 1;
            ScaleView.ContentOffset = new BasicPoint(0, 0);
            ScaleView.Zoom = 1;

            WaveView.LoadPeakFile(audioFile);
            ScaleView.AudioFile = audioFile;
            ScaleView.Invalidate();
        }

        public void SetWaveFormLength(long lengthBytes)
        {
            WaveView.SetWaveFormLength(lengthBytes);
            ScaleView.AudioFileLength = lengthBytes;
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
                if (_lblZoom.Alpha == 1 && DateTime.Now - _lastZoomUpdate > new TimeSpan(0, 0, 0, 0, 700))
                {
                    //Console.WriteLine("WaveFormScrollView - HandleTimerFadeOutZoomLabelElapsed - Refreshing wave form bitmap...");
                    _lblZoom.Visibility = ViewStates.Gone;
                    //WaveView.RefreshWaveFormBitmap();

                    //var animFadeOut = new AlphaAnimation(1, 0);
                    //animFadeOut.Interpolator = new DecelerateInterpolator();
                    //animFadeOut.Duration = 200;
                    //_lblZoom.Animation = animFadeOut;

                    //Animation anim = AnimationUtils.LoadAnimation(Context, Resource.Animation.fade_out);
                    //anim.AnimationEnd += (animSender, animArgs) =>
                    //{
                    //    _lblZoom.Visibility = ViewStates.Gone;
                    //};
                    //_lblZoom.StartAnimation(anim);
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
        
        public class WaveFormLayout : LinearLayout
        {
            private readonly WaveFormScrollView _scrollView;
            private ScaleGestureDetector _scaleGestureDetector;
            private GestureDetector _panGestureDetector;
            private OverScroller _scroller; // keep OverScroll because it works better on older devices than Scroller

            public WaveFormScaleView ScaleView { get; private set; }
            public WaveFormView WaveView { get; private set; }

            public WaveFormLayout(WaveFormScrollView scrollView) : base(scrollView.Context)
            {
                _scrollView = scrollView;
                Orientation = Orientation.Vertical;
                //SetBackgroundColor(Color.DarkOrange);

                int height = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 22, Resources.DisplayMetrics);
                ScaleView = new WaveFormScaleView(Context);
                ScaleView.SetBackgroundColor(Color.Purple);
                AddView(ScaleView, new LinearLayout.LayoutParams(LayoutParams.WrapContent, height));

                WaveView = new WaveFormView(Context);
                //WaveView.SetBackgroundColor(Color.DarkRed);
                AddView(WaveView, new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.FillParent));

                _panGestureDetector = new GestureDetector(Context, new PanListener(_scrollView));
                _scaleGestureDetector = new ScaleGestureDetector(Context, new ScaleListener(_scrollView));
                _scroller = new OverScroller(Context);
            }

            public override bool DispatchTouchEvent(MotionEvent e)
            {
                base.DispatchTouchEvent(e);
                _panGestureDetector.OnTouchEvent(e);
                _scaleGestureDetector.OnTouchEvent(e);
                return true;
            }

            public override void ComputeScroll()
            {
                base.ComputeScroll();

                // Call compute scroll offset or the position won't change
                bool moreToScroll = _scroller.ComputeScrollOffset();
                Console.WriteLine("WaveFormScrollView - WaveFormLayout - ComputeScroll - moreToScroll: {0} scroller.IsFinished: {1}", moreToScroll, _scroller.IsFinished);
                if (!_scroller.IsFinished)
                {
                    Console.WriteLine("WaveFormScrollView - WaveFormLayout - ComputeScroll - Scrolling to {0}", _scroller.CurrX);
                    WaveView.ContentOffset = new BasicPoint(_scroller.CurrX, 0);
                    ScaleView.ContentOffset = new BasicPoint(_scroller.CurrX, 0);
                }
            }

            public void Down()
            {
                _scroller.ForceFinished(true);
                PostInvalidateOnAnimation();
            }

            public void Fling(int velocityX, int velocityY)
            {
                int startX = (int)WaveView.ContentOffset.X;

                // Reset any animation
                _scroller.ForceFinished(true);
                //_scroller.Fling(startX, 0, velocityX, velocityY, 0, (int)((Width * Zoom) - Width), 0, (int)Height, (int)(Width / 2), (int)(Height / 2));
                _scroller.Fling(startX, 0, velocityX, velocityY, 0, (int)((Width * _scrollView.Zoom) - Width), 0, (int)Height, 0, 0); // no overscroll
                PostInvalidateOnAnimation();
            }
        }

        private class PanListener : GestureDetector.SimpleOnGestureListener
        {
            private readonly WaveFormScrollView _scrollView;

            public PanListener(WaveFormScrollView scrollView)
            {
                _scrollView = scrollView;
            }

            public override bool OnDown(MotionEvent e)
            {
                //Console.WriteLine("PanListener - OnDown");
                _scrollView.WaveformLayout.Down();
                return base.OnDown(e);
            }

            public override bool OnDoubleTap(MotionEvent e)
            {
                //Console.WriteLine("PanListener - OnDoubleTap");
                _scrollView.Zoom = 1;
                _scrollView.SetContentOffsetX(0);
                return base.OnDoubleTap(e);
            }

            public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
            {
                Console.WriteLine("PanListener - OnFling - velocityX: {0} velocityY: {1}", velocityX, velocityY);
                _scrollView.WaveformLayout.Fling((int)-velocityX, (int)velocityY);
                return base.OnFling(e1, e2, velocityX, velocityY);
            }
            
            public override bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
            {
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
                // Adjust content offset with new zoom value
                float contentOffsetX = _startContentOffsetX * scale;
                float zoom = _startZoom * scale;
                zoom = Math.Max(1, zoom);
                zoom = Math.Min(32, zoom);
                _scrollView.Zoom = zoom;
                _scrollView.SetContentOffsetX(contentOffsetX);
                //Console.WriteLine("ScaleListener - SetScrollViewScale - scale: {0} zoom: {1} contentOffset: {2}", scale, _scrollView.Zoom, _scrollView.WaveView.ContentOffset);
            }
        }
    }
}