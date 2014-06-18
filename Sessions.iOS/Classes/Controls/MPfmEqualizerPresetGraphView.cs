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
using MPfm.Player.Objects;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Helpers;

namespace MPfm.iOS.Classes.Controls
{
    [Register("MPfmEqualizerPresetGraphView")]
    public class MPfmEqualizerPresetGraphView : UIView
    {
        public EQPreset Preset { get; set; }

        public MPfmEqualizerPresetGraphView(IntPtr handle) 
            : base (handle)
        {
            Initialize();
        }

        public MPfmEqualizerPresetGraphView(RectangleF frame) 
            : base(frame)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.BackgroundColor = UIColor.Black;
            Preset = new EQPreset();
        }

        public override void Draw(RectangleF rect)
        {
            float padding = 6;
            float heightAvailable = Bounds.Height - (padding * 2); // 14 = height
            var context = UIGraphics.GetCurrentContext();

            // IDEA: Put the equalizer line in the Equalizer Presets screen (in UITableViewCell)
            // IDEA: Put the value of the band currently changing over the graph (i.e. +3.5dB)
			CoreGraphicsHelper.FillRect(context, Bounds, BackgroundColor.CGColor);

            // Draw center line
            CoreGraphicsHelper.DrawLine(context, new List<PointF>(){
                new PointF(padding, (Bounds.Height / 2)),
                new PointF(Bounds.Width - padding, (Bounds.Height / 2))
            }, UIColor.DarkGray.CGColor, 2, false, false);

            // Draw 20Hz and 20kHz lines
            CoreGraphicsHelper.DrawLine(context, new List<PointF>(){
                new PointF(padding, padding),
                new PointF(padding, Bounds.Height - padding)
            }, UIColor.DarkGray.CGColor, 1, false, true);
            CoreGraphicsHelper.DrawLine(context, new List<PointF>(){
                new PointF(Bounds.Width - padding, padding),
                new PointF(Bounds.Width - padding, Bounds.Height - padding)
            }, UIColor.DarkGray.CGColor, 1, false, true);

            // Draw text
            SizeF sizeString = CoreGraphicsHelper.MeasureText(context, Preset.Bands[Preset.Bands.Count-1].CenterString, "HelveticaNeue-Bold", 8);
            CoreGraphicsHelper.DrawTextAtPoint(context, new PointF(6 + 4, Bounds.Height - sizeString.Height - (padding / 2)), Preset.Bands[0].CenterString, "HelveticaNeue-Bold", 8, new CGColor(0.3f, 0.3f, 0.3f));
            CoreGraphicsHelper.DrawTextAtPoint(context, new PointF(Bounds.Width - padding - sizeString.Width - 4, Bounds.Height - sizeString.Height - (padding / 2)), Preset.Bands[Preset.Bands.Count-1].CenterString, "HelveticaNeue-Bold", 8, new CGColor(0.3f, 0.3f, 0.3f));

            // Draw equalizer line
            var points = new List<PointF>();
            float x = padding;
            foreach (var band in Preset.Bands)
            {
                // Value range is -6 to 6.
                float ratio = (band.Gain + 6) / (padding * 2);
                float y = padding + heightAvailable - (ratio * (Bounds.Height - (padding * 2)));
                points.Add(new PointF(x, y));
                x += (Bounds.Width - (padding * 2)) / (Preset.Bands.Count - 1);
            }
            CoreGraphicsHelper.DrawRoundedLine(context, points, UIColor.Yellow.CGColor, 2, false, false);
        }
    }
}
