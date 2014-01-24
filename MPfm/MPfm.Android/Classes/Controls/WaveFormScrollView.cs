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
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using MPfm.Sound.AudioFiles;

namespace org.sessionsapp.android
{
    public class WaveFormScrollView : HorizontalScrollView
    {
        private int _activePointerId;
        private float _lastTouchX;
        private float _lastTouchY;
        private ScaleGestureDetector _scaleGestureDetector;

        public WaveFormScaleView ScaleView { get; private set; }
        public WaveFormView WaveView { get; private set; }

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
            _scaleGestureDetector = new ScaleGestureDetector(Context, new ScaleListener());
            SetBackgroundColor(Resources.GetColor(MPfm.Android.Resource.Color.background));
            FillViewport = true;

            var layout = new LinearLayout(Context);
            layout.Orientation = Orientation.Vertical;
            layout.SetBackgroundColor(Resources.GetColor(MPfm.Android.Resource.Color.background));
            AddView(layout, new FrameLayout.LayoutParams(LayoutParams.FillParent, LayoutParams.FillParent));

            int height = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 22, Resources.DisplayMetrics);
            ScaleView = new WaveFormScaleView(Context);
            ScaleView.SetBackgroundColor(Color.Purple);
            layout.AddView(ScaleView, new LinearLayout.LayoutParams(LayoutParams.WrapContent, height));

            WaveView = new WaveFormView(Context);
            WaveView.SetBackgroundColor(Color.DarkRed);
            layout.AddView(WaveView, new LinearLayout.LayoutParams(LayoutParams.FillParent, LayoutParams.FillParent));
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

        public override bool OnTouchEvent(MotionEvent e)
        {
            _scaleGestureDetector.OnTouchEvent(e);

            float x, y;
            int pointerIndex, pointerId;
            switch(e.Action)
            {
                case MotionEventActions.Down:
                    _lastTouchX = e.GetX();
                    _lastTouchY = e.GetY();
                    _activePointerId = e.GetPointerId(0);
                    Console.WriteLine("WaveFormScrollView - OnTouchEvent - Down - x,y: {0},{1} activePointerId: {2}", _lastTouchX, _lastTouchY, _activePointerId);
                    break;
                case MotionEventActions.Move:
                    pointerIndex = e.FindPointerIndex(_activePointerId);
                    if (pointerIndex == -1)
                        return true;
                    x = e.GetX(pointerIndex);
                    y = e.GetY(pointerIndex);
                    float dx = x - _lastTouchX;
                    float dy = y - _lastTouchY;
                    _lastTouchX = x;
                    _lastTouchY = y;
                    Console.WriteLine("WaveFormScrollView - OnTouchEvent - Move - x,y: {0},{1}", x, y);
                    break;
                case MotionEventActions.Up:
                    Console.WriteLine("WaveFormScrollView - OnTouchEvent - Up");
                    break;
                case MotionEventActions.Cancel:
                    Console.WriteLine("WaveFormScrollView - OnTouchEvent - Cancel");
                    break;
                case MotionEventActions.PointerUp:
                    pointerIndex = ((int)e.Action & (int)MotionEventActions.PointerIndexMask) >> (int)MotionEventActions.PointerIndexShift;
                    pointerId = e.GetPointerId(pointerIndex);
                    if (pointerId == _activePointerId)
                    {
                        int newPointerIndex = pointerIndex == 0 ? 1 : 0;
                        _lastTouchX = e.GetX(newPointerIndex);
                        _lastTouchY = e.GetY(newPointerIndex);
                        _activePointerId = e.GetPointerId(newPointerIndex);
                    }
                    Console.WriteLine("WaveFormScrollView - OnTouchEvent - PointerUp - pointerIndex: {0}", pointerIndex);
                    break;
            }

            //return base.OnTouchEvent(e);
            return true;
        }

        private class ScaleListener : ScaleGestureDetector.SimpleOnScaleGestureListener
        {
            public override bool OnScaleBegin(ScaleGestureDetector detector)
            {
                Console.WriteLine("ScaleListener - OnScaleBegin");
                return base.OnScaleBegin(detector);
            }

            public override bool OnScale(ScaleGestureDetector detector)
            {
                Console.WriteLine("ScaleListener - OnScale");
                return base.OnScale(detector);
            }

            public override void OnScaleEnd(ScaleGestureDetector detector)
            {
                Console.WriteLine("ScaleListener - OnScaleEnd");
                base.OnScaleEnd(detector);
            }
        }
    }
}