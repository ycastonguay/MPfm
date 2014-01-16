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

namespace MPfm.GenericControls.Controls
{
    /// <summary>
    /// The Fader control is a vertical track bar with the appearance of a fader.
    /// The control appearance can be changed using the public properties.
    /// </summary>
    public class FaderControl : IControl, IControlInteraction
    {
        private BasicColor _backgroundColor1 = new BasicColor(36, 47, 53);
        private BasicColor _backgroundColor2 = new BasicColor(36, 47, 53);
        private BasicColor _faderColor1 = new BasicColor(255, 255, 255);
        private BasicColor _faderColor2 = new BasicColor(245, 245, 245);
        private BasicColor _faderShadowColor1 = new BasicColor(188, 188, 188);
        private BasicColor _faderShadowColor2 = new BasicColor(220, 220, 220);
        private BasicColor _centerLineColor = new BasicColor(0, 0, 0);
        private BasicColor _centerLineShadowColor = new BasicColor(169, 169, 169);
        private BasicColor _faderMiddleLineColor = new BasicColor(0, 0, 0);
        private BasicColor _faderShadowColor = new BasicColor(169, 169, 169);

        private BasicRectangle _rectFader = new BasicRectangle();
        private bool _isTrackBarMoving = false;
        private float _trackHeight = 0;
        private float _valueRatio = 0;
        private float _valueRelativeToValueRange = 0;
        private float _valueRange = 0;
        private bool _mouseButtonDown;

        private int _faderWidth = 20;
        public int FaderWidth { get; set; }

        private int _faderHeight = 15;
        public int FaderHeight { get; set; }

        private int _minimum = 0;
        public int Minimum { get; set; }

        private int _maximum = 10;
        public int Maximum { get; set; }

        private int _value = 0;
        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                OnInvalidateVisual();

                if (OnFaderValueChanged != null)
                    OnFaderValueChanged(this, new EventArgs());
            }
        }

        public int ValueWithoutEvent
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                OnInvalidateVisual();
            }
        }

        private int _margin = 16;
        public new int Margin { get; set; }

        private int _stepSize = 1;
        public int StepSize { get; set; }

        public delegate void FaderValueChanged(object sender, EventArgs e);
        public event FaderValueChanged OnFaderValueChanged;

        public event InvalidateVisual OnInvalidateVisual;

        public FaderControl()
            : base()
        {
            FaderHeight = 28;
            FaderWidth = 10;
            OnInvalidateVisual += () => { };
        }

        public void MouseDown(float x, float y, MouseButtonType button)
        {
            // Make sure the mouse button pressed was the left mouse button
            _mouseButtonDown = true;
            //CaptureMouse();
            if (button == MouseButtonType.Left)
            {
                // Check if the user clicked in the fader area
                if (x >= _rectFader.X &&
                    x <= _rectFader.Width + _rectFader.X &&
                    y >= _rectFader.Y &&
                    y <= _rectFader.Height + _rectFader.Y)
                {
                    _isTrackBarMoving = true;
                }
            }
        }

        public void MouseUp(float x, float y, MouseButtonType button)
        {
            // Check if the track bar was moving (mouse down)
            //ReleaseMouseCapture();
            if (!_isTrackBarMoving)
            {
                // The user clicked without dragging the mouse; we need to add or
                // substract a "step" depending on the mouse cursor position.
                if (y < _rectFader.Y)
                {
                    if (Value + StepSize > Maximum)
                        Value = Maximum;
                    else
                        Value += StepSize;
                }
                else if (y > _rectFader.Y + _rectFader.Height)
                {
                    if (Value - StepSize < Minimum)
                        Value = Minimum;
                    else
                        Value -= StepSize;
                }

                if (OnFaderValueChanged != null)
                    OnFaderValueChanged(this, new EventArgs());

                OnInvalidateVisual();
            }

            _mouseButtonDown = false;
            _isTrackBarMoving = false;
        }

        public void MouseMove(float x, float y, MouseButtonType button)
        {
            bool valueChanged = false;
            if (_isTrackBarMoving && _mouseButtonDown)
            {
                // Evaluate tick height
                double tickHeight = _trackHeight / _valueRange;

                // Loop through "steps"                
                for (int a = Minimum; a < Maximum + 1; a++)
                {
                    double startY = _trackHeight - ((a - Minimum + 1) * tickHeight);
                    double endY = _trackHeight - ((a - Minimum) * tickHeight);

                    // Adjust cursor position relative to margin
                    double cursorY = y - Margin;

                    // Does the cursor exceed min or max?
                    if (cursorY <= 0)
                    {
                        // Don't change the value if it's already the same!
                        if (Value != Maximum)
                        {
                            Value = Maximum;
                            valueChanged = true;
                            break;
                        }
                    }
                    else if (cursorY >= _trackHeight)
                    {
                        // Don't change the value if it's already the same!
                        if (Value != Minimum)
                        {
                            Value = Minimum;
                            valueChanged = true;
                            break;
                        }
                    }
                    // Is the cursor in the current value?
                    else if (cursorY >= startY && cursorY <= endY)
                    {
                        // Don't change the value if it's already the same!
                        if (Value != a)
                        {
                            Value = a;
                            valueChanged = true;
                            break;
                        }
                    }
                }

                // If the value has changed, refresh control and raise event
                if (valueChanged)
                {
                    if (OnFaderValueChanged != null)
                        OnFaderValueChanged(this, new EventArgs());

                    OnInvalidateVisual();
                }
            }
        }

        public void Render(IGraphicsContext context)
        {
            // Value range is the size between max and min track bar value.
            // Ex: Min = 50, Max = 150. Value range = 100 + 1 (because we include 50 and 100)
            _valueRange = (Maximum - Minimum) + 1;

            // Get track bar value relative to value range (value - minimum value).
            // Ex: Min = 50, Max = 150, Value = 100. Value relative to value range = 50.
            _valueRelativeToValueRange = Value - Minimum;

            // Draw background
            context.DrawRectangle(new BasicRectangle(0, 0, context.BoundsWidth, context.BoundsHeight), new BasicBrush(_backgroundColor1), new BasicPen());

            // Return if value range is zero
            if (_valueRange == 0)
                return;

            // Draw fader track
            float trackX = context.BoundsWidth / 2;
            float trackY = Margin;
            float trackY2 = context.BoundsHeight - Margin;

            // Draw shadow track
            context.DrawLine(new BasicPoint(trackX + 1, trackY + 1), new BasicPoint(trackX + 1, trackY2 + 1), new BasicPen(new BasicBrush(_centerLineShadowColor), 1));

            // Draw track
            context.DrawLine(new BasicPoint(trackX, trackY), new BasicPoint(trackX, trackY2), new BasicPen(new BasicBrush(_centerLineColor), 1));

            // Get the track height (remove margin from top and bottom)            
            _trackHeight = context.BoundsHeight - (Margin * 2);

            // Get tick width
            float tickHeight = _trackHeight / _valueRange;

            // Get the percentage of the value relative to value range (needed to draw the fader).
            // We need to divide the value relative to value range to the value range to get the ratio.
            // Ex: Min = 50, Max = 150, Value = 100. Value relative to value range = 50. Value range = 100. Value ratio = 0.5
            _valueRatio = (_valueRelativeToValueRange / _valueRange);

            // Calculate fader position
            // We need to invert the values (i.e. max is on top, min is bottom)
            //float valueY = (valueRatio * trackHeight) + Margin;            
            float valueY = _trackHeight - (_valueRatio * _trackHeight) + Margin;
            float faderY = valueY + ((tickHeight - FaderHeight) / 2);
            float tickCenterY = valueY + (tickHeight / 2);

            // Create fader rectangle            
            _rectFader = new BasicRectangle((context.BoundsWidth / 2) - (FaderWidth / 2), faderY, FaderWidth, FaderHeight);

            //    // Draw tick zone (for debug)
            //    //RectangleF rectTickZone = new RectangleF(valueX, 0, tickWidth, Height);
            //    //g.FillRectangle(Brushes.DarkGray, rectTickZone);            

            var rectFaderShadowTop = new BasicRectangle((context.BoundsWidth / 2) - (FaderWidth / 2) + 1, faderY + 1, FaderWidth, 8);
            var rectFaderShadowBottom = new BasicRectangle((context.BoundsWidth / 2) - (FaderWidth / 2) + 1, faderY + FaderHeight - 8 + 1, FaderWidth, 8);
            var rectFaderShadowCenter = new BasicRectangle((context.BoundsWidth / 2) - (FaderWidth / 2) + 1, faderY + 4 + 1, FaderWidth, FaderHeight - 8);

            context.DrawEllipsis(rectFaderShadowTop, new BasicBrush(_faderShadowColor), new BasicPen());
            context.DrawEllipsis(rectFaderShadowBottom, new BasicBrush(_faderShadowColor), new BasicPen());
            context.DrawRectangle(rectFaderShadowCenter, new BasicBrush(_faderShadowColor), new BasicPen());

            // Draw fader outline (with 8px border)            
            var rectFaderTop = new BasicRectangle((context.BoundsWidth / 2) - (FaderWidth / 2), faderY, FaderWidth, 8);
            var rectFaderBottom = new BasicRectangle((context.BoundsWidth / 2) - (FaderWidth / 2), faderY + FaderHeight - 8, FaderWidth, 8);
            var rectFaderBottomCenter = new BasicRectangle((context.BoundsWidth / 2) - (FaderWidth / 2), faderY + FaderHeight - 10, FaderWidth, 6);
            var rectFaderCenter = new BasicRectangle((context.BoundsWidth / 2) - (FaderWidth / 2), faderY + 4, FaderWidth, FaderHeight - 8);

            context.DrawEllipsis(rectFaderTop, new BasicGradientBrush(_faderColor1, _faderColor2, 90), new BasicPen());
            context.DrawEllipsis(rectFaderBottom, new BasicBrush(_faderShadowColor1), new BasicPen());
            context.DrawRectangle(rectFaderCenter, new BasicBrush(_faderColor2), new BasicPen());
            context.DrawRectangle(rectFaderBottomCenter, new BasicBrush(_faderShadowColor1), new BasicPen());

            // Draw fader inside (with 4px border)
            var rectFaderInsideBottom = new BasicRectangle((context.BoundsWidth / 2) - (FaderWidth / 2) + 1, faderY + FaderHeight - 8, FaderWidth - 2, 4);
            var rectFaderInsideBottomCenter = new BasicRectangle((context.BoundsWidth / 2) - (FaderWidth / 2) + 1, faderY + FaderHeight - 12, FaderWidth - 2, FaderHeight - 24);
            var rectFaderInsideTop = new BasicRectangle((context.BoundsWidth / 2) - (FaderWidth / 2) + 1, faderY + 4, FaderWidth - 2, 8);
            var rectFaderInsideTopCenter = new BasicRectangle((context.BoundsWidth / 2) - (FaderWidth / 2) + 1, faderY + 8, FaderWidth - 2, FaderHeight - 24);

            context.DrawEllipsis(rectFaderInsideTop, new BasicBrush(_faderShadowColor1), new BasicPen());
            context.DrawEllipsis(rectFaderInsideTopCenter, new BasicGradientBrush(_faderShadowColor1, _faderShadowColor2, 90), new BasicPen());
            context.DrawEllipsis(rectFaderInsideBottom, new BasicBrush(_faderColor2), new BasicPen());
            context.DrawRectangle(rectFaderInsideBottomCenter, new BasicBrush(_faderColor2), new BasicPen());
            context.DrawLine(new BasicPoint((context.BoundsWidth / 2) - (FaderWidth / 2), tickCenterY), new BasicPoint((context.BoundsWidth / 2) - (FaderWidth / 2) + FaderWidth, tickCenterY), new BasicPen(new BasicBrush(_faderMiddleLineColor), 1));
        }

    }
}