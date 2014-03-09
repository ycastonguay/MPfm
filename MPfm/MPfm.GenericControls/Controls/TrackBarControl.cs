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
    public class TrackBarControl : IControl, IControlMouseInteraction
    {
        private readonly object _locker = new object();
        private BasicBrush _brushBackground;
        private BasicPen _penTransparent;
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
        private float _trackWidth = 0;
        private float _valueRatio = 0;
        private float _valueRelativeToValueRange = 0;
        private float _valueRange = 0;
        private bool _mouseButtonDown = false;

        public int FaderWidth { get; set; }
        public int FaderHeight { get; set; }
        public int Margin { get; set; }
        public int StepSize { get; set; }
        public int Minimum { get; set; }
        public int Maximum { get; set; }

        private int _value = 0;
        private BasicBrush _brushFaderColor2;
        private BasicPen _penShadowColor1;
        private BasicPen _penCenterLineShadow;
        private BasicPen _penCenterLine;

        public int Value
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

                if (OnTrackBarValueChanged != null)
                    OnTrackBarValueChanged();
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

        public delegate void TrackBarValueChanged();
        public event TrackBarValueChanged OnTrackBarValueChanged;

        public event InvalidateVisual OnInvalidateVisual;
        public event InvalidateVisualInRect OnInvalidateVisualInRect;

        public TrackBarControl()
            : base()
        {
            FaderHeight = 14;
            FaderWidth = 24;
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
                if (x < _rectFader.X)
                {
                    if (Value - StepSize < Minimum)
                        Value = Minimum;
                    else
                        Value -= StepSize;
                }
                else if (x > _rectFader.X + _rectFader.Width)
                {
                    if (Value + StepSize > Maximum)
                        Value = Maximum;
                    else
                        Value += StepSize;
                }

                if (OnTrackBarValueChanged != null)
                    OnTrackBarValueChanged();

                OnInvalidateVisual();
            }

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
            // Set value changed flag default to false
            bool valueChanged = false;
            if (_isTrackBarMoving && _mouseButtonDown)
            {
                // Evaluate tick height
                double tickWidth = _trackWidth / _valueRange;
                for (int a = Minimum; a < Maximum + 1; a++)
                {
                    double startX = (a - Minimum) * tickWidth;
                    double endX = (a - Minimum + 1) * tickWidth;

                    // Adjust cursor position relative to margin
                    double cursorX = x - Margin;

                    // Does the cursor exceed min or max?
                    if (cursorX <= 0)
                    {
                        // Don't change the value if it's already the same!
                        if (Value != Minimum)
                        {
                            Value = Minimum;
                            valueChanged = true;
                            break;
                        }
                    }
                    else if (cursorX >= _trackWidth)
                    {
                        // Don't change the value if it's already the same!
                        if (Value != Maximum)
                        {
                            Value = Maximum;
                            valueChanged = true;
                            break;
                        }
                    }
                    // Is the cursor in the current value?
                    else if (cursorX >= startX && cursorX <= endX)
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
                    if (OnTrackBarValueChanged != null)
                        OnTrackBarValueChanged();

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
        }

        public void Render(IGraphicsContext context)
        {
            lock (_locker)
            {
                if (_penTransparent == null)
                {
                    _penTransparent = new BasicPen();
                    _penShadowColor1 = new BasicPen(new BasicBrush(_faderShadowColor1), 1);
                    _penCenterLine = new BasicPen(new BasicBrush(_centerLineColor), 1);
                    _penCenterLineShadow = new BasicPen(new BasicBrush(_centerLineShadowColor), 1);
                    _brushBackground = new BasicBrush(_backgroundColor);
                    _brushFaderColor2 = new BasicBrush(_faderColor2);
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
            float trackX = Margin; // add margin from left
            float trackX2 = context.BoundsWidth - Margin; // add margin from right
            float trackY = context.BoundsHeight / 2; // right in the center

            context.DrawLine(new BasicPoint(trackX + 1, trackY + 1), new BasicPoint(trackX2 + 1, trackY + 1), _penCenterLineShadow);
            context.DrawLine(new BasicPoint(trackX, trackY), new BasicPoint(trackX2, trackY), _penCenterLine);

            // Get the track width (remove margin from left and right)
            _trackWidth = context.BoundsWidth - (Margin * 2);

            // Get tick width
            float tickWidth = _trackWidth / _valueRange;

            // Get the percentage of the value relative to value range (needed to draw the fader).
            // We need to divide the value relative to value range to the value range to get the ratio.
            // Ex: Min = 50, Max = 150, Value = 100. Value relative to value range = 50. Value range = 100. Value ratio = 0.5
            _valueRatio = (_valueRelativeToValueRange / _valueRange);

            // Get the value X coordinate by multiplying the value ratio to the track bar width (removed 3 pixels from left
            // and right). Add margin from left.
            float valueX = (_valueRatio * _trackWidth) + Margin; // this gives the LEFT x for our zone
            float faderX = valueX + ((tickWidth - FaderWidth) / 2);
            float tickCenterX = valueX + (tickWidth / 2);

            // Create fader rectangle
            _rectFader = new BasicRectangle(faderX, (context.BoundsHeight / 2) - (FaderHeight / 2), FaderWidth, FaderHeight);

            //// Draw tick zone (for debug)
            ////RectangleF rectTickZone = new RectangleF(valueX, 0, tickWidth, Height);
            ////g.FillRectangle(Brushes.DarkGray, rectTickZone);            

            // Draw fader outline (with 8px border)
            var rectFaderLeft = new BasicRectangle(faderX, (context.BoundsHeight / 2) - (FaderHeight / 2), 8, FaderHeight);
            var rectFaderRight = new BasicRectangle(faderX + FaderWidth - 8, (context.BoundsHeight / 2) - (FaderHeight / 2), 8, FaderHeight);
            var rectFaderCenter = new BasicRectangle(faderX + 4, (context.BoundsHeight / 2) - (FaderHeight / 2), FaderWidth - 8, FaderHeight);

            //context.DrawEllipsis(rectFaderLeft, new BasicGradientBrush(_faderColor1, _faderColor2, 90), new BasicPen());
            //context.DrawEllipsis(rectFaderLeft, new BasicGradientBrush(new BasicColor(255, 0, 0), new BasicColor(0, 0, 255), 90), new BasicPen());
            //context.DrawEllipsis(rectFaderRight, new BasicGradientBrush(_faderColor1, _faderColor2, 90), new BasicPen());
            //context.DrawEllipsis(rectFaderRight, new BasicGradientBrush(new BasicColor(0, 255, 0), new BasicColor(255, 0, 255), 90), new BasicPen());
            context.DrawEllipsis(rectFaderCenter, _brushFaderColor2, _penTransparent);
            //context.DrawEllipsis(rectFaderCenter, new BasicBrush(_faderColor2), new BasicPen());

            // Draw fader inside (with 4px border)
            var rectFaderInsideLeft = new BasicRectangle(faderX + 2, (context.BoundsHeight / 2) - (FaderHeight / 2) + 2, 4, FaderHeight - 4);
            var rectFaderInsideRight = new BasicRectangle(faderX + FaderWidth - 6, (context.BoundsHeight / 2) - (FaderHeight / 2) + 2, 4, FaderHeight - 4);

            //context.DrawEllipsis(rectFaderInsideLeft, new BasicGradientBrush(_faderShadowColor1, _faderShadowColor2, 90), new BasicPen());
            //context.DrawEllipsis(rectFaderInsideRight, new BasicGradientBrush(_faderShadowColor1, _faderShadowColor2, 90), new BasicPen());
            context.DrawLine(new BasicPoint(tickCenterX, (context.BoundsHeight / 2) - (FaderHeight / 2)), new BasicPoint(tickCenterX, (context.BoundsHeight / 2) - (FaderHeight / 2) + FaderHeight), _penShadowColor1);
        }
    }
}