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
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using MPfm.Player.Objects;

namespace org.sessionsapp.android
{
    public class EqualizerPresetGraphView : SurfaceView
    {
        private EQPreset _preset;

        public EqualizerPresetGraphView(Context context) : base(context)
        {
            Initialize();
        }

        public EqualizerPresetGraphView(Context context, IAttributeSet attrs) : base (context, attrs)
        {
            Initialize();
        }

        public EqualizerPresetGraphView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            Initialize();
        }

        public EqualizerPresetGraphView(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
            Initialize();
        }

        private void Initialize()
        {
            if (!IsInEditMode)
                SetLayerType(LayerType.Hardware, null); // Use GPU instead of CPU (except in IDE such as Eclipse)
        }

        public void SetPreset(EQPreset preset)
        {
            _preset = preset;
            Invalidate();
        }

        public override void Draw(Canvas canvas)
        {
            //Console.WriteLine("EqualizerPresetGraphView - Draw - Width: {0} Height: {1}", Width, Height);

            float padding = 6 * Resources.DisplayMetrics.Density;
            float heightAvailable = Height - (padding*2);            

            var paintRect = new Paint {
                AntiAlias = true,
                Color = Color.ParseColor("#222222")
            };
            paintRect.SetStyle(Paint.Style.Fill);
            canvas.DrawRect(new Rect(0, 0, Width, Height), paintRect);

            // Draw center line
            var paintCenterLine = new Paint {
                AntiAlias = true,
                Color = Color.DarkGray
            };
            paintCenterLine.SetStyle(Paint.Style.Fill);
            paintCenterLine.StrokeWidth = 2f;
            canvas.DrawLine(padding, Height / 2, Width - padding, Height / 2, paintCenterLine);

            // Draw 20Hz and 20kHz lines
            paintCenterLine.StrokeWidth = 1f;
            canvas.DrawLine(padding, padding, padding, Height - padding, paintCenterLine);
            canvas.DrawLine(Width - padding, padding, Width - padding, Height - padding, paintCenterLine);

            var paintText = new Paint {
                AntiAlias = true,
                Color = Color.Gray,
                TextSize = 14 * Resources.DisplayMetrics.Density
            };
            float textWidth = paintText.MeasureText(_preset.Bands[_preset.Bands.Count - 1].CenterString);
            canvas.DrawText(_preset.Bands[0].CenterString, padding * 2, Height - (padding * 2), paintText);
            canvas.DrawText(_preset.Bands[_preset.Bands.Count - 1].CenterString, Width - textWidth - (padding * 2), Height - (padding * 2), paintText);

            if (_preset == null)
                return;

            // Draw equalizer line
            var points = new List<float>();
            var paintEQLine = new Paint
            {
                AntiAlias = true,
                Color = Color.Yellow
            };
            paintEQLine.SetStyle(Paint.Style.Stroke);
            paintEQLine.StrokeWidth = 2f * Resources.DisplayMetrics.Density;
            float x = padding;
            for (int a = 0; a < _preset.Bands.Count; a++)
            {
                // Value range is -6 to 6.
                var band = _preset.Bands[a];
                //float ratio = (band.Gain + 6) / (padding * 2);
                float ratio = (band.Gain + 6f) / 12f;
                float y = padding + heightAvailable - (ratio * (Height - (padding * 2)));

                //Console.WriteLine("EqualizerPresetGraphView - Draw - Width: {0} Height: {1} ratio: {2} x: {3} y: {4} padding: {5} heightAvailable: {6}", Width, Height, ratio, x, y, padding, heightAvailable);
                points.Add(x);
                points.Add(y);

                // Add the same point a second time because Android needs start/end for each segment
                if (a > 0 && a < _preset.Bands.Count - 1)
                {
                    points.Add(x);
                    points.Add(y);
                }

                x += (Width - (padding * 2)) / (_preset.Bands.Count - 1);
            }
            canvas.DrawLines(points.ToArray(), paintEQLine);
        }
    }
}