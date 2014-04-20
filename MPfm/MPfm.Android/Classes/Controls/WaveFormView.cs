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
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using MPfm.Android.Classes.Controls.Graphics;
using MPfm.Android.Classes.Controls.Helpers;
using MPfm.GenericControls.Basics;
using MPfm.GenericControls.Controls;
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;

namespace org.sessionsapp.android
{
    public class WaveFormView : SurfaceView
    {
        private WaveFormControl _control;

        public long Position { get { return _control.Position; } set { _control.Position = value; } }
        public long SecondaryPosition { get { return _control.SecondaryPosition; } set { _control.SecondaryPosition = value; } }
        public bool ShowSecondaryPosition { get { return _control.ShowSecondaryPosition; } set { _control.ShowSecondaryPosition = value; } }
        public float Zoom { get { return _control.Zoom; } set { _control.Zoom = value; } }
        public BasicPoint ContentOffset { get { return _control.ContentOffset; } set { _control.ContentOffset = value; } }
        public bool IsLoading { get { return _control.IsLoading; } }

        public WaveFormView(Context context) : base(context)
        {
            Initialize();
        }

        public WaveFormView(Context context, IAttributeSet attrs) : base (context, attrs)
        {
            Initialize();
        }

        public WaveFormView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            Initialize();
        }

        public WaveFormView(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
            Initialize();
        }

        private void Initialize()
        {
            // Keep this or for strange reason the bitmaps won't appear.
            SetBackgroundColor(Color.Black);

            _control = new WaveFormControl();
            _control.Frame = new BasicRectangle(0, 0, Width, Height);
            _control.OnInvalidateVisual += () => Post(Invalidate);            
            _control.OnInvalidateVisualInRect += (rect) => Post(() => Invalidate(GenericControlHelper.ToRect(rect)));
            //_control.FontSize = 12;

            if (!IsInEditMode)
                SetLayerType(LayerType.Hardware, null); // Use GPU instead of CPU (except in IDE such as Eclipse)
        }

        public void SetMarkers(IEnumerable<Marker> markers)
        {
            _control.SetMarkers(markers);
        }

        public void SetWaveFormLength(long lengthBytes)
        {
            _control.Length = lengthBytes;
        }

        public void LoadPeakFile(AudioFile audioFile)
        {
            _control.LoadPeakFile(audioFile);
        }

        public void InvalidateBitmaps()
        {
            _control.Frame = new BasicRectangle(0, 0, Width, Height);
            _control.InvalidateBitmaps();
        }

        public override void Draw(Canvas canvas)
        {
            float density = Resources.DisplayMetrics.Density;
            //Console.WriteLine("WaveFormView - Draw - Width: {0} Height: {1} density: {2}", Width, Height, density);
            var wrapper = new GraphicsContextWrapper(canvas, Width, Height, density);
            _control.Render(wrapper);
        }
    }
}