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
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Controls.Base;
using Sessions.GenericControls.Controls.Interfaces;
using Sessions.GenericControls.Graphics;
using System.Timers;

namespace Sessions.GenericControls.Controls
{
    public class ProgressBarControl : ControlBase
    {
        private int _animationCounter;
        private Timer _timerRefresh;
        private BasicBrush _brushBackground;
        private BasicBrush _brushForeground;
        private BasicBrush _brushIndeterminateForeground;
        private BasicPen _penTransparent;

        public BasicColor ColorBackground { get; set; }
        public BasicColor ColorForeground { get; set; }
        public BasicColor ColorIndeterminateForeground { get; set; }
        public float FontSize { get; set; }
        public string FontFace { get; set; }

        private float _value = 0;
        /// <summary>
        /// Value defining the progress, from 0 to 1.
        /// </summary>
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

                // Determine dirty rectangle
                float leftX = 0;
                float rightX = 0;
                if (_value > value)
                {
                    leftX = value * Frame.Width;
                    rightX = _value * Frame.Width;
                } 
                else if (_value < value)
                {
                    leftX = _value * Frame.Width;
                    rightX = value * Frame.Width;
                }
                var dirtyRect = new BasicRectangle(leftX, 0, leftX + rightX, Frame.Height);

                _value = value;
                InvalidateVisualInRect(dirtyRect);
            }
        }

        private bool _isIndeterminate = false;
        public bool IsIndeterminate
        {
            get
            {
                return _isIndeterminate;
            }
            set
            {
                // Also consider StartAnimation()/StopAnimation() maybe?
                if (value && !_timerRefresh.Enabled)
                    _timerRefresh.Start();
                else if (!value && _timerRefresh.Enabled)
                    _timerRefresh.Stop();

                _isIndeterminate = value;
                InvalidateVisual();
            }
        }

        public ProgressBarControl()
        {
			Initialize();
        }

		private void Initialize()
		{
            Frame = new BasicRectangle();
			FontFace = "Roboto Condensed";
			FontSize = 10;

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
            ColorForeground = new BasicColor(231, 76, 60);
            ColorIndeterminateForeground = new BasicColor(240, 117, 104);
            _brushBackground = new BasicBrush(ColorBackground);
            _brushForeground = new BasicBrush(ColorForeground);
            _brushIndeterminateForeground = new BasicBrush(ColorIndeterminateForeground);
            _penTransparent = new BasicPen();
        }

        public override void Render(IGraphicsContext context)
        {
            base.Render(context);

            var rectBackground = new BasicRectangle(0, 0, context.BoundsWidth, context.BoundsHeight);
            context.DrawRectangle(rectBackground, _brushBackground, _penTransparent);

            if (_isIndeterminate)
            {
                var rectForeground = new BasicRectangle(0, 0, context.BoundsWidth, context.BoundsHeight);
                context.DrawRectangle(rectForeground, _brushForeground, _penTransparent);

                const int pixelsPerIteration = 1;
                float inclination = 15 * context.Density;
                float barSize = 20 * context.Density;
                float widthBetweenBars = 0;//2 * context.Density;
                float totalBarWidth = barSize + inclination + widthBetweenBars;
                int barCount = (int)Math.Ceiling(context.BoundsWidth / totalBarWidth);
                for (int a = -1; a < barCount + 1; a++) // bleed paths out of bounds partially visible
                {
                    float offset = (_animationCounter % totalBarWidth) * pixelsPerIteration;
                    float x = (a * totalBarWidth) + offset;
                    var path = new BasicPath();
                    path.AddLine(new BasicPathLine(new BasicPoint(x, context.BoundsHeight), new BasicPoint(x + barSize, context.BoundsHeight)));
                    path.AddLine(new BasicPathLine(new BasicPoint(x + barSize, context.BoundsHeight), new BasicPoint(x + barSize + inclination, 0)));
                    path.AddLine(new BasicPathLine(new BasicPoint(x + barSize + inclination, 0), new BasicPoint(x + inclination, 0)));
                    path.AddLine(new BasicPathLine(new BasicPoint(x + inclination, 0), new BasicPoint(x, context.BoundsHeight)));
                    context.DrawPath(path, _brushIndeterminateForeground, _penTransparent);
                }
            } 
            else
            {
                var rectForeground = new BasicRectangle(0, 0, _value * context.BoundsWidth, context.BoundsHeight);
                context.DrawRectangle(rectForeground, _brushForeground, _penTransparent);
            }
        }
    }
}