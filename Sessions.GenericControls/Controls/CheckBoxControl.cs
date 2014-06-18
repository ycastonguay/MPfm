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
using MPfm.GenericControls.Basics;
using MPfm.GenericControls.Graphics;
using MPfm.GenericControls.Interaction;
using MPfm.Sound.AudioFiles;
using System.Timers;

namespace MPfm.GenericControls.Controls
{
    public class CheckBoxControl : IControl
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

        public BasicRectangle Frame { get; set; }
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
                OnInvalidateVisual();
            }
        }

        public event InvalidateVisual OnInvalidateVisual;
        public event InvalidateVisualInRect OnInvalidateVisualInRect;

        public CheckBoxControl()
        {
			Initialize();
        }

		private void Initialize()
		{
            Frame = new BasicRectangle();

            _timerRefresh = new Timer();
            _timerRefresh.Interval = 50;
            _timerRefresh.Elapsed += HandleTimerRefreshElapsed;

		    CreateDrawingResources();
		}

        private void HandleTimerRefreshElapsed(object sender, ElapsedEventArgs e)
        {
            _animationCounter++;
            OnInvalidateVisual();
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

        public void Render(IGraphicsContext context)
        {
            Frame = new BasicRectangle(0, 0, context.BoundsWidth, context.BoundsHeight);
            var rectBackground = new BasicRectangle(0, 0, context.BoundsWidth, context.BoundsHeight);
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