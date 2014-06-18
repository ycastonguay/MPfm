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
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Graphics;
using Sessions.GenericControls.Interaction;

namespace Sessions.GenericControls.Controls
{
    public class CircularProgressBarControl : IControl
    {
        private BasicBrush _brushBackground;
        private BasicBrush _brushForeground;
        private BasicPen _penTransparent;

        public BasicColor ColorBackground { get; set; }
        public BasicColor ColorForeground { get; set; }
		public float FontSize { get; set; }
		public string FontFace { get; set; }

        private float _value = 0;
        public float Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value == value)
                    return;

                _value = value;
                OnInvalidateVisual();
            }
        }

        public event InvalidateVisual OnInvalidateVisual;
        public event InvalidateVisualInRect OnInvalidateVisualInRect;

        public CircularProgressBarControl()
        {
			Initialize();
        }

		private void Initialize()
		{
			FontFace = "Roboto Condensed";
			FontSize = 10;

		    CreateDrawingResources();
		}

        private void CreateDrawingResources()
        {
            ColorBackground = new BasicColor(32, 40, 46);
            ColorForeground = new BasicColor(231, 76, 60);
            _brushBackground = new BasicBrush(ColorBackground);
            _brushForeground = new BasicBrush(ColorForeground);
            _penTransparent = new BasicPen();
        }

        public void Render(IGraphicsContext context)
        {
            context.DrawRectangle(new BasicRectangle(0, 0, context.BoundsWidth, context.BoundsHeight), _brushBackground, _penTransparent);

        }
    }
}