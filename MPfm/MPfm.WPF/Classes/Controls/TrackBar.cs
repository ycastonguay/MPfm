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

using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MPfm.WPF.Classes.Extensions;
using Control = System.Windows.Controls.Control;

namespace MPfm.WPF.Classes.Controls
{
    /// <summary>
    /// This trackbar control is based on the System.Windows.Forms.TrackBar control.
    /// It adds custom drawing and other features.
    /// </summary>
    public class TrackBar : Control
    {
        private Color _backgroundColor1 = Color.FromRgb(36, 47, 53);
        private Color _backgroundColor2 = Color.FromRgb(36, 47, 53);
        private Color _faderColor1 = Colors.White;
        private Color _faderColor2 = Colors.WhiteSmoke;
        private Color _faderShadowColor1 = Color.FromRgb(188, 188, 188);
        private Color _faderShadowColor2 = Colors.Gainsboro;
        private Color _centerLineColor = Colors.Black;
        private Color _centerLineShadowColor = Colors.DarkGray;
        private Color _faderMiddleLineColor = Colors.Black;
        private Color _faderShadowColor = Colors.DarkGray;

        private Rect _rectFader = new Rect();
        private bool _isTrackBarMoving = false;
        private double _trackWidth = 0;
        private double _valueRatio = 0;
        private double _valueRelativeToValueRange = 0;
        private double _valueRange = 0;

        private bool _mouseButtonDown = false;
        /// <summary>
        /// Indicates if a mouse button is down.
        /// </summary>
        public bool MouseButtonDown
        {
            get
            {
                return _mouseButtonDown;
            }
        }

        #region Track Bar Properties

        private int _faderWidth = 20;
        /// <summary>
        /// Track bar width.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(20)]
        [Category("Display"), Browsable(true), Description("Track bar width.")]
        public int FaderWidth
        {
            get
            {
                return _faderWidth;
            }
            set
            {
                _faderWidth = value;
            }
        }

        private int _faderHeight = 15;
        /// <summary>
        /// Track bar height.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(15)]
        [Category("Display"), Browsable(true), Description("Track bar height.")]
        public int FaderHeight
        {
            get
            {
                return _faderHeight;
            }
            set
            {
                _faderHeight = value;
            }
        }

        private int _margin = 16;
        /// <summary>
        /// Track bar track margin.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(16)]
        [Category("Display"), Browsable(true), Description("Track bar track margin.")]
        public new int Margin
        {
            get
            {
                return _margin;
            }
            set
            {
                _margin = value;
            }
        }

        private int _stepSize = 1;
        /// <summary>
        /// Track bar step size (when the user clicks to skip a value).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(1)]
        [Category("Display"), Browsable(true), Description("Track bar step size (when the user clicks to skip a value).")]
        public int StepSize
        {
            get
            {
                return _stepSize;
            }
            set
            {
                _stepSize = value;
            }
        }

        #endregion

        #region Other Properties

        private int _minimum = 0;
        /// <summary>
        /// Track bar minimum value.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(0)]
        [Category("Display"), Browsable(true), Description("Track bar minimum value.")]
        public int Minimum
        {
            get
            {
                return _minimum;
            }
            set
            {
                _minimum = value;
            }
        }

        private int _maximum = 10;
        /// <summary>
        /// Track bar maximum value.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), DefaultValue(10), Description("Track bar maximum value.")]
        public int Maximum
        {
            get
            {
                return _maximum;
            }
            set
            {
                _maximum = value;
            }
        }

        private int _value = 0;
        /// <summary>
        /// Track bar value.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(0)]
        [Category("Display"), Browsable(true), Description("Track bar value")]
        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                this._value = value;
                InvalidateVisual();

                if (OnTrackBarValueChanged != null)
                    OnTrackBarValueChanged();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the track bar value changes.
        /// </summary>
        public delegate void TrackBarValueChanged();
        /// <summary>
        /// Hook for the track bar value changed event.
        /// </summary>
        public event TrackBarValueChanged OnTrackBarValueChanged;

        #endregion

        public TrackBar()
            : base()
        {
        }

        /// <summary>
        /// Sets the track bar value without triggering the OnTrackBarValueChanged event.
        /// </summary>
        /// <param name="value">Value to set</param>
        public void SetValueWithoutTriggeringEvent(int value)
        {
            this._value = value;
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            // Value range is the size between max and min track bar value.
            // Ex: Min = 50, Max = 150. Value range = 100 + 1 (because we include 50 and 100)
            _valueRange = (Maximum - Minimum) + 1;

            // Get track bar value relative to value range (value - minimum value).
            // Ex: Min = 50, Max = 150, Value = 100. Value relative to value range = 50.            
            _valueRelativeToValueRange = Value - Minimum;

            // Draw background
            dc.DrawRectangle(new SolidColorBrush(_backgroundColor1), new Pen(), new Rect(0, 0, ActualWidth, ActualHeight));

            // Return if value range is zero
            if (_valueRange == 0)
                return;

            // Draw fader track
            double trackX = Margin; // add margin from left
            double trackX2 = ActualWidth - Margin; // add margin from right
            double trackY = ActualHeight / 2; // right in the center

            dc.DrawLine(new Pen(new SolidColorBrush(_centerLineShadowColor), 1), new Point(trackX + 1, trackY + 1), new Point(trackX2 + 1, trackY + 1));
            dc.DrawLine(new Pen(new SolidColorBrush(_centerLineColor), 1), new Point(trackX, trackY), new Point(trackX2, trackY));

            // Get the track width (remove margin from left and right)
            _trackWidth = ActualWidth - (Margin * 2);

            // Get tick width
            double tickWidth = _trackWidth / _valueRange;

            // Get the percentage of the value relative to value range (needed to draw the fader).
            // We need to divide the value relative to value range to the value range to get the ratio.
            // Ex: Min = 50, Max = 150, Value = 100. Value relative to value range = 50. Value range = 100. Value ratio = 0.5
            _valueRatio = (_valueRelativeToValueRange / _valueRange);

            // Get the value X coordinate by multiplying the value ratio to the track bar width (removed 3 pixels from left
            // and right). Add margin from left.
            double valueX = (_valueRatio * _trackWidth) + Margin; // this gives the LEFT x for our zone
            double faderX = valueX + ((tickWidth - FaderWidth) / 2);
            double tickCenterX = valueX + (tickWidth / 2);

            // Create fader rectangle
            _rectFader = new Rect(faderX, (ActualHeight / 2) - (FaderHeight / 2), FaderWidth, FaderHeight);

            //// Draw tick zone (for debug)
            ////RectangleF rectTickZone = new RectangleF(valueX, 0, tickWidth, Height);
            ////g.FillRectangle(Brushes.DarkGray, rectTickZone);            

            // Draw fader outline (with 8px border)
            var rectFaderLeft = new Rect(faderX, (ActualHeight / 2) - (FaderHeight / 2), 8, FaderHeight);
            var rectFaderRight = new Rect(faderX + FaderWidth - 8, (ActualHeight / 2) - (FaderHeight / 2), 8, FaderHeight);
            var rectFaderCenter = new Rect(faderX + 4, (ActualHeight / 2) - (FaderHeight / 2), FaderWidth - 8, FaderHeight);

            dc.DrawEllipse(new LinearGradientBrush(_faderColor1, _faderColor2, 90), new Pen(), rectFaderLeft.Center(), rectFaderLeft.Width / 2, rectFaderLeft.Height / 2);
            dc.DrawEllipse(new LinearGradientBrush(_faderColor1, _faderColor2, 90), new Pen(), rectFaderRight.Center(), rectFaderRight.Width / 2, rectFaderRight.Height / 2);
            dc.DrawEllipse(new SolidColorBrush(_faderColor2), new Pen(), rectFaderCenter.Center(), rectFaderCenter.Width / 2, rectFaderCenter.Height / 2);

            // Draw fader inside (with 4px border)
            var rectFaderInsideLeft = new Rect(faderX + 2, (ActualHeight / 2) - (FaderHeight / 2) + 2, 4, FaderHeight - 4);
            var rectFaderInsideRight = new Rect(faderX + FaderWidth - 6, (ActualHeight / 2) - (FaderHeight / 2) + 2, 4, FaderHeight - 4);

            dc.DrawEllipse(new LinearGradientBrush(_faderShadowColor1, _faderShadowColor2, 90), new Pen(), rectFaderInsideLeft.Center(), rectFaderInsideLeft.Width / 2, rectFaderInsideLeft.Height / 2);
            dc.DrawEllipse(new LinearGradientBrush(_faderShadowColor1, _faderShadowColor2, 90), new Pen(), rectFaderInsideRight.Center(), rectFaderInsideRight.Width / 2, rectFaderInsideRight.Height / 2);
            dc.DrawLine(new Pen(new SolidColorBrush(_faderShadowColor2), 1), new Point(tickCenterX, (ActualHeight / 2) - (FaderHeight / 2)), new Point(tickCenterX, (ActualHeight / 2) - (FaderHeight / 2) + FaderHeight));
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            // Make sure the mouse button pressed was the left mouse button
            _mouseButtonDown = true;
            CaptureMouse();
            if (e.ChangedButton == MouseButton.Left)
            {
                // Check if the user clicked in the fader area
                var location = e.GetPosition(this);
                if (location.X >= _rectFader.X &&
                    location.X <= _rectFader.Width + _rectFader.X &&
                    location.Y >= _rectFader.Y &&
                    location.Y <= _rectFader.Height + _rectFader.Y)
                {
                    _isTrackBarMoving = true;
                }
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            // Check if the track bar was moving (mouse down)
            ReleaseMouseCapture();
            if (!_isTrackBarMoving)
            {
                var location = e.GetPosition(this);

                // The user clicked without dragging the mouse; we need to add or
                // substract a "step" depending on the mouse cursor position.
                if (location.X < _rectFader.X)
                {
                    if (Value - StepSize < Minimum)
                        Value = Minimum;
                    else
                        Value -= StepSize;
                }
                else if (location.X > _rectFader.X + _rectFader.Width)
                {
                    if (Value + StepSize > Maximum)
                        Value = Maximum;
                    else
                        Value += StepSize;
                }

                if (OnTrackBarValueChanged != null)
                    OnTrackBarValueChanged();

                InvalidateVisual();
            }

            _mouseButtonDown = false;
            _isTrackBarMoving = false;
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Set value changed flag default to false
            bool valueChanged = false;
            if (_isTrackBarMoving && _mouseButtonDown)
            {
                // Evaluate tick height
                var location = e.GetPosition(this);
                double tickWidth = _trackWidth / _valueRange;
                for (int a = Minimum; a < Maximum + 1; a++)
                {
                    double startX = (a - Minimum) * tickWidth;
                    double endX = (a - Minimum + 1) * tickWidth;

                    // Adjust cursor position relative to margin
                    double cursorX = location.X - Margin;

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

                    InvalidateVisual();
                }
            }

            base.OnMouseMove(e);
        }
    }
}
