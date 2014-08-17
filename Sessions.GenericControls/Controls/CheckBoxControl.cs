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
using System.Timers;
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Controls.Base;
using Sessions.GenericControls.Graphics;

namespace Sessions.GenericControls.Controls
{
    public class CheckBoxControl : ControlBase
    {
        private int _animationCounter;
        private Timer _timerRefresh;
        private BasicBrush _brushBackground;
        private BasicBrush _brushForeground;
        private BasicBrush _brushBorder;
        private BasicBrush _brushTransparent;
        private BasicPen _penBorder;
        private BasicPen _penForeground;
        private BasicPen _penTransparent;

        public BasicColor ColorBackground { get; set; }
        public BasicColor ColorForeground { get; set; }
        public BasicColor ColorBorder { get; set; }

        private bool _value = false;
        public bool Value
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
                InvalidateVisual();
            }
        }

        public CheckBoxControl()
        {
			Initialize();
        }

		private void Initialize()
		{
            _timerRefresh = new Timer();
            _timerRefresh.Interval = 50;
            _timerRefresh.Elapsed += HandleTimerRefreshElapsed;

		    CreateDrawingResources();
		}

        private void HandleTimerRefreshElapsed(object sender, ElapsedEventArgs e)
        {
            _animationCounter++;
            InvalidateVisual();
        }

        private void CreateDrawingResources()
        {
            ColorBackground = new BasicColor(32, 40, 46);
            ColorForeground = new BasicColor(230, 237, 242);//174, 196, 212);
            ColorBorder = new BasicColor(83, 104, 119);
            _brushBackground = new BasicBrush(ColorBackground);
            _brushForeground = new BasicBrush(ColorForeground);
            _brushBorder = new BasicBrush(ColorBorder);
            _brushTransparent = new BasicBrush(new BasicColor(0, 0, 0, 0));
            _penBorder = new BasicPen(_brushBorder, 2);
            _penForeground = new BasicPen(_brushForeground, 2);
            _penTransparent = new BasicPen();
        }

        public override void Render(IGraphicsContext context)
        {            
            base.Render(context);

            //var rectBackground = new BasicRectangle(0, 0, context.BoundsWidth, context.BoundsHeight);
            //context.DrawRectangle(rectBackground, _brushBackground, _penTransparent);

            const float padding = 2;
            const float checkPadding = padding + 4;
            var rectForeground = new BasicRectangle(padding, padding, context.BoundsWidth - (padding * 2), context.BoundsHeight - (padding * 2));
            context.DrawRectangle(rectForeground, _brushTransparent, _penBorder);

            if (_value)
            {
                context.DrawLine(new BasicPoint(checkPadding, checkPadding), new BasicPoint(context.BoundsWidth - checkPadding, context.BoundsHeight - checkPadding), _penForeground);
                context.DrawLine(new BasicPoint(context.BoundsWidth - checkPadding, checkPadding), new BasicPoint(checkPadding, context.BoundsHeight - checkPadding), _penForeground);
            }
        }
    }
}