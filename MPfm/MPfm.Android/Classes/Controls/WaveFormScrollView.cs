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
using Android;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using MPfm.Android.Classes.Controls.Graphics;
using MPfm.Android.Classes.Controls.Helpers;
using MPfm.GenericControls.Controls;
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;

namespace org.sessionsapp.android
{
    public class WaveFormScrollView : HorizontalScrollView
    {
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
            SetBackgroundColor(Color.DarkOrange);
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
    }
}