// Copyright � 2011-2013 Yanick Castonguay
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
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Sessions.Android.Classes.Controls.Graphics;
using Sessions.Android.Classes.Controls.Helpers;
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Controls;
using Sessions.Sound.AudioFiles;

namespace org.sessionsapp.android
{
    public class WaveFormScaleView : SurfaceView
    {
        private WaveFormScaleControl _control;

        public AudioFile AudioFile
        {
            get
            {
                return _control.AudioFile;
            }
            set
            {
                _control.AudioFile = value;
            }
        }

        public long AudioFileLength
        {
            get
            {
                return _control.AudioFileLength;
            }
            set
            {
                _control.AudioFileLength = value;
            }
        }

        public float Zoom
        {
            get
            {
                return _control.Zoom;
            }
            set
            {
                _control.Zoom = value;
            }
        }

        public BasicPoint ContentOffset
        {
            get
            {
                return _control.ContentOffset;
            }
            set
            {
                _control.ContentOffset = value;
            }
        }  

        public WaveFormScaleView(Context context) : base(context)
        {
            Initialize();
        }

        public WaveFormScaleView(Context context, IAttributeSet attrs) : base (context, attrs)
        {
            Initialize();
        }

        public WaveFormScaleView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            Initialize();
        }

        public WaveFormScaleView(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
            Initialize();
        }

        private void Initialize()
        {
            _control = new WaveFormScaleControl();
            _control.OnInvalidateVisual += () => Post(Invalidate);            
            _control.OnInvalidateVisualInRect += (rect) => Post(() => Invalidate(GenericControlHelper.ToRect(rect)));
            //_control.FontSize = 12;

            if (!IsInEditMode)
                SetLayerType(LayerType.Hardware, null); // Use GPU instead of CPU (except in IDE such as Eclipse)
        }

        public override void Draw(Canvas canvas)
        {
            // Note: For hardware accelerated controls, canvas.ClipBounds returns the whole control surface, but the hardware only actually draws 
            // the dirty rectangle (Romain Guy from Google at http://stackoverflow.com/questions/7233830/partial-invalidation-in-custom-android-view-with-hardware-acceleration)
            float density = Resources.DisplayMetrics.Density;
            var wrapper = new GraphicsContextWrapper(canvas, Width, Height, density, GenericControlHelper.ToBasicRect(canvas.ClipBounds));
            _control.Render(wrapper);
        }
    }
}