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
using System.Timers;
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Controls.Base;
using Sessions.GenericControls.Graphics;
using Sessions.Player.Objects;
using System.Collections.Generic;
using org.sessionsapp.player;

namespace Sessions.GenericControls.Controls
{
    public class EqualizerPresetGraphControl : ControlBase
    {
        private BasicBrush _brushBackground;
        private BasicBrush _brushForeground;
        private BasicBrush _brushMainLine;
        private BasicBrush _brushSecondaryLine;
        private BasicBrush _brushTransparent;
        private BasicPen _penMainLine;
        private BasicPen _penCenterLine;
        private BasicPen _penBorderLine;
        private BasicPen _penForeground;
        private BasicPen _penTransparent;

        private BasicColor _colorBackground;
        public BasicColor ColorBackground
        {
            get
            {
                return _colorBackground;
            }
            set
            {
                _colorBackground = value;
                CreateDrawingResources();
            }
        }

        private BasicColor _colorForeground;
        public BasicColor ColorForeground
        {
            get
            {
                return _colorForeground;
            }
            set
            {
                _colorForeground = value;
                CreateDrawingResources();
            }
        }

        private BasicColor _colorMainLine;
        public BasicColor ColorMainLine
        {
            get
            {
                return _colorMainLine;
            }
            set
            {
                _colorMainLine = value;
                CreateDrawingResources();
            }
        }

        private BasicColor _colorSecondaryLine;
        public BasicColor ColorSecondaryLine
        {
            get
            {
                return _colorSecondaryLine;
            }
            set
            {
                _colorSecondaryLine = value;
                CreateDrawingResources();
            }
        }

        private SSPEQPreset _preset;
        public SSPEQPreset Preset
        {
            get
            {
                return _preset;
            }
            set
            {
                if (_preset == value)
                    return;

                _preset = value;
                InvalidateVisual();
            }
        }

        public bool ShowText { get; set; }
        public bool ShowGuideLines { get; set; }

        public EqualizerPresetGraphControl()
        {
			Initialize();
        }

		private void Initialize()
		{
            ShowText = true;
            ShowGuideLines = true;
            SetDefaultColors();
		    CreateDrawingResources();
		}

        private void SetDefaultColors()
        {
            _colorBackground = new BasicColor(32, 40, 46);
            _colorForeground = new BasicColor(230, 237, 242);
            _colorMainLine = new BasicColor(255, 255, 0);
            _colorSecondaryLine = new BasicColor(85, 85, 85);
        }

        private void CreateDrawingResources()
        {
            _brushBackground = new BasicBrush(ColorBackground);
            _brushForeground = new BasicBrush(ColorForeground);
            _brushMainLine = new BasicBrush(ColorMainLine);
            _brushSecondaryLine = new BasicBrush(ColorSecondaryLine);
            _brushTransparent = new BasicBrush(new BasicColor(0, 0, 0, 0));
            _penForeground = new BasicPen(_brushForeground, 2);
            _penMainLine = new BasicPen(_brushMainLine, 2);
            _penCenterLine = new BasicPen(_brushSecondaryLine, 2);
            _penBorderLine = new BasicPen(_brushSecondaryLine, 1);
            _penTransparent = new BasicPen();
        }

        public override void Render(IGraphicsContext context)
        {            
            base.Render(context);

            const float padding = 6;
            float heightAvailable = Frame.Height - (padding * 2); // 14 = height

            // IDEA: Put the value of the band currently changing over the graph (i.e. +3.5dB)

            if (ShowGuideLines)
            {
                // Background
                context.DrawRectangle(Frame, _brushBackground, _penTransparent);

                // Draw center line
                context.DrawLine(new BasicPoint(padding, (Frame.Height / 2)), 
                    new BasicPoint(Frame.Width - padding, (Frame.Height / 2)), _penCenterLine);

                // Draw 20Hz and 20kHz lines
                context.DrawLine(new BasicPoint(padding, padding),
                    new BasicPoint(padding, Frame.Height - padding), _penBorderLine);
                context.DrawLine(new BasicPoint(Frame.Width - padding, padding),
                    new BasicPoint(Frame.Width - padding, Frame.Height - padding), _penBorderLine);
            }

            if (Preset == null)
                return;

            if (ShowText)
            {
                // Draw text
                string fontFace = "HelveticaNeue-Bold";
                int fontSize = 8;
                string leftText = Preset.Bands[0].Label;
                string rightText = Preset.Bands[Preset.Bands.Length - 1].Label;
                var sizeLeftText = context.MeasureText(leftText, new BasicRectangle(), fontFace, fontSize);
                var sizeRightText = context.MeasureText(rightText, new BasicRectangle(), fontFace, fontSize);
                context.DrawText(leftText, new BasicPoint(6 + 4, Frame.Height - sizeLeftText.Height - (padding / 2)), ColorForeground, fontFace, fontSize);
                context.DrawText(rightText, new BasicPoint(Frame.Width - padding - sizeRightText.Width - 4, Frame.Height - sizeRightText.Height - (padding / 2)), ColorForeground, fontFace, fontSize);
            }

            // Draw equalizer line
            var points = new List<BasicPoint>();
            float x = padding;
            foreach (var band in Preset.Bands)
            {
                // Value range is -6 to 6.
                float ratio = (band.Gain + 6) / (padding * 2);
                float y = padding + heightAvailable - (ratio * (Frame.Height - (padding * 2)));
                points.Add(new BasicPoint(x, y));
                x += (Frame.Width - (padding * 2)) / (Preset.Bands.Length - 1);
            }
            context.DrawLine(points, _penMainLine, true, false, false);
        }
    }
}