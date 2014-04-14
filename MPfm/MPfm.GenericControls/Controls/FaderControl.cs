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

namespace MPfm.GenericControls.Controls
{
    /// <summary>
    /// The Fader control is a vertical track bar with the appearance of a fader.
    /// The control appearance can be changed using the public properties.
    /// </summary>
    public class FaderControl : IControl, IControlMouseInteraction
    {
        private readonly object _locker = new object();
        private BasicBrush _brushBackground;
        private BasicBrush _brushFaderShadowColor;
        private BasicBrush _brushFaderColor2;
        private BasicBrush _brushFaderShadowColor1;
        private BasicGradientBrush _brushFaderGradient;
        private BasicGradientBrush _brushFaderShadowGradient;
        private BasicPen _penTransparent;
        private BasicPen _penMiddleLineColor;
        private BasicColor _backgroundColor = new BasicColor(32, 40, 46);
        private BasicColor _backgroundColor2 = new BasicColor(32, 40, 46);
        private BasicColor _faderColor1 = new BasicColor(255, 255, 255);
        private BasicColor _faderColor2 = new BasicColor(245, 245, 245);
        private BasicColor _faderShadowColor1 = new BasicColor(188, 188, 188);
        private BasicColor _faderShadowColor2 = new BasicColor(220, 220, 220);
        private BasicColor _centerLineColor = new BasicColor(0, 0, 0, 150);
        private BasicColor _centerLineShadowColor = new BasicColor(169, 169, 169, 80); 
        private BasicColor _faderMiddleLineColor = new BasicColor(0, 0, 0);
        private BasicColor _faderShadowColor = new BasicColor(169, 169, 169);

        private BasicRectangle _rectFader = new BasicRectangle();
        private bool _isTrackBarMoving = false;
        private float _trackHeight = 0;
        private float _valueRatio = 0;
        private float _valueRelativeToValueRange = 0;
        private float _valueRange = 0;
        private bool _mouseButtonDown;

        public int FaderWidth { get; set; }
        public int FaderHeight { get; set; }
        public int Minimum { get; set; }
        public int Maximum { get; set; }
        public int Margin { get; set; }
        public int StepSize { get; set; }

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

        public delegate void FaderValueChanged(object sender, EventArgs e);
        public event FaderValueChanged OnFaderValueChanged;

        public event InvalidateVisual OnInvalidateVisual;
        public event InvalidateVisualInRect OnInvalidateVisualInRect;

        public FaderControl()
            : base()
        {
            FaderHeight = 28;
            FaderWidth = 10;
            Minimum = 0;
            Maximum = 1;
            Margin = 16;
            StepSize = 1;
            OnInvalidateVisual += () => { };
            OnInvalidateVisualInRect += (rect) => { };
        }

        public void MouseDown(float x, float y, MouseButtonType button, KeysHeld keysHeld)
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
                    //Console.WriteLine("FaderControl - MouseDown - Mouse down on track bar; track bar is now moving");
                    _isTrackBarMoving = true;
                }
            }
        }

        public void MouseUp(float x, float y, MouseButtonType button, KeysHeld keysHeld)
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

            //Console.WriteLine("FaderControl - MouseDown - Mouse up; stopping track bar movement");
            _mouseButtonDown = false;
            _isTrackBarMoving = false;
        }

        public void MouseClick(float x, float y, MouseButtonType button, KeysHeld keysHeld)
        {            
        }

        public void MouseDoubleClick(float x, float y, MouseButtonType button, KeysHeld keysHeld)
        {
        }

        public void MouseMove(float x, float y, MouseButtonType button)
        {
            bool valueChanged = false;
            //Console.WriteLine("FaderControl - MouseMove - _isTrackBarMoving: {0} _mouseButtonDown: {1}", _isTrackBarMoving, _mouseButtonDown);
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

        public void MouseLeave()
        {
        }

        public void MouseEnter()
        {
        }

        public void MouseWheel(float delta)
        {
            int newValue = (int)(Value + (delta * 2));
            if(newValue < Minimum)
                newValue = Minimum;
            else if(newValue > Maximum)
                newValue = Maximum;
            Value = newValue;
        }

        public void Render(IGraphicsContext context)
        {
            lock (_locker)
            {
                if (_penTransparent == null)
                {
                    // TODO: Reduce the number of brushes
                    _penTransparent = new BasicPen();
                    _penMiddleLineColor = new BasicPen(new BasicBrush(_faderMiddleLineColor), 1);
                    _brushBackground = new BasicBrush(_backgroundColor);
                    _brushFaderShadowColor = new BasicBrush(_faderShadowColor);
                    _brushFaderGradient = new BasicGradientBrush(_faderColor1, _faderColor2, 90);
                    _brushFaderColor2 = new BasicBrush(_faderColor2);
                    _brushFaderShadowColor1 = new BasicBrush(_faderShadowColor1);
                    _brushFaderShadowGradient = new BasicGradientBrush(_faderShadowColor1, _faderShadowColor2, 90);
                }
            }

            // Value range is the size between max and min track bar value.
            // Ex: Min = 50, Max = 150. Value range = 100 + 1 (because we include 50 and 100)
            _valueRange = (Maximum - Minimum) + 1;

            // Get track bar value relative to value range (value - minimum value).
            // Ex: Min = 50, Max = 150, Value = 100. Value relative to value range = 50.
            _valueRelativeToValueRange = Value - Minimum;

            // Draw background
            context.DrawRectangle(new BasicRectangle(0, 0, context.BoundsWidth, context.BoundsHeight), _brushBackground, _penTransparent);

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

            context.DrawEllipsis(rectFaderShadowTop, _brushFaderShadowColor, _penTransparent);
            context.DrawEllipsis(rectFaderShadowBottom, _brushFaderShadowColor, _penTransparent);
            context.DrawRectangle(rectFaderShadowCenter, _brushFaderShadowColor, _penTransparent);

            // Draw fader outline (with 8px border)            
            var rectFaderTop = new BasicRectangle((context.BoundsWidth / 2) - (FaderWidth / 2), faderY, FaderWidth, 8);
            var rectFaderBottom = new BasicRectangle((context.BoundsWidth / 2) - (FaderWidth / 2), faderY + FaderHeight - 8, FaderWidth, 8);
            var rectFaderBottomCenter = new BasicRectangle((context.BoundsWidth / 2) - (FaderWidth / 2), faderY + FaderHeight - 10, FaderWidth, 6);
            var rectFaderCenter = new BasicRectangle((context.BoundsWidth / 2) - (FaderWidth / 2), faderY + 4, FaderWidth, FaderHeight - 8);

            context.DrawEllipsis(rectFaderTop, _brushFaderGradient, _penTransparent);
            context.DrawEllipsis(rectFaderBottom, _brushFaderShadowColor1, _penTransparent);
            context.DrawRectangle(rectFaderCenter, _brushFaderColor2, _penTransparent);
            context.DrawRectangle(rectFaderBottomCenter, _brushFaderShadowColor1, _penTransparent);

            // Draw fader inside (with 4px border)
            var rectFaderInsideBottom = new BasicRectangle((context.BoundsWidth / 2) - (FaderWidth / 2) + 1, faderY + FaderHeight - 8, FaderWidth - 2, 4);
            var rectFaderInsideBottomCenter = new BasicRectangle((context.BoundsWidth / 2) - (FaderWidth / 2) + 1, faderY + FaderHeight - 12, FaderWidth - 2, FaderHeight - 24);
            var rectFaderInsideTop = new BasicRectangle((context.BoundsWidth / 2) - (FaderWidth / 2) + 1, faderY + 4, FaderWidth - 2, 8);
            var rectFaderInsideTopCenter = new BasicRectangle((context.BoundsWidth / 2) - (FaderWidth / 2) + 1, faderY + 8, FaderWidth - 2, FaderHeight - 24);

            context.DrawEllipsis(rectFaderInsideTop, _brushFaderShadowColor1, _penTransparent);
            context.DrawEllipsis(rectFaderInsideTopCenter, _brushFaderShadowGradient, _penTransparent);
            context.DrawEllipsis(rectFaderInsideBottom, _brushFaderColor2, _penTransparent);
            context.DrawRectangle(rectFaderInsideBottomCenter, _brushFaderColor2, _penTransparent);
            context.DrawLine(new BasicPoint((context.BoundsWidth / 2) - (FaderWidth / 2), tickCenterY), new BasicPoint((context.BoundsWidth / 2) - (FaderWidth / 2) + FaderWidth, tickCenterY), _penMiddleLineColor);
        }
    }
}