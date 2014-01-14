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
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MPfm.WindowsControls;
using MPfm.WPF.Classes.Extensions;
using MPfm.WPF.Classes.Theme;
using Control = System.Windows.Controls.Control;

namespace MPfm.WPF.Classes.Controls
{
    /// <summary>
    /// The Fader control is a vertical track bar with the appearance of a fader.
    /// The control appearance can be changed using the public properties.
    /// </summary>
    public class Fader : Control
    {
        private Rect _rectFader = new Rect();
        private bool _isTrackBarMoving = false;
        private double _trackHeight = 0;
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

        private FaderTheme _theme = null;
        /// <summary>
        /// Defines the current theme used for rendering the control.
        /// </summary>
        public FaderTheme Theme
        {
            get
            {
                return _theme;
            }
            set
            {
                _theme = value;
            }
        }

        #region Fader Properties

        private int _faderWidth = 20;
        /// <summary>
        /// Fader width.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(20)]
        [Category("Display"), Browsable(true), Description("Fader width.")]
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
        /// Fader height.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(15)]
        [Category("Display"), Browsable(true), Description("Fader height.")]
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

        private int _minimum = 0;
        /// <summary>
        /// Fader minimum value.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(0)]
        [Category("Display"), Browsable(true), Description("Fader minimum value.")]
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
        /// Fader maximum value.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), DefaultValue(10), Description("Fader maximum value.")]
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
        /// Fader value.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(0)]
        [Category("Display"), Browsable(true), Description("Fader value.")]
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
                this._value = value;
                InvalidateVisual();
            }
        }

        private int _margin = 16;
        /// <summary>
        /// Fader track margin.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(16)]
        [Category("Display"), Browsable(true), Description("Fader track margin.")]
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
        /// Fader step size (when the user clicks to skip a value).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(1)]
        [Category("Display"), Browsable(true), Description("Fader step size (when the user clicks to skip a value).")]
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

        #region Events

        /// <summary>
        /// Occurs when the fader value changes.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        public delegate void FaderValueChanged(object sender, EventArgs e);
        /// <summary>
        /// Hook for the fader value changed event.
        /// </summary>
        public event FaderValueChanged OnFaderValueChanged;

        #endregion

        public Fader()
            : base()
        {
            // Create default theme
            _theme = new FaderTheme();
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
            dc.DrawRectangle(new SolidColorBrush(Colors.Brown), new Pen(), new Rect(0, 0, ActualWidth, ActualHeight));

            // Return if value range is zero
            if (_valueRange == 0)
                return;

            // Draw fader track
            double trackX = ActualWidth / 2;
            double trackY = Margin;
            double trackY2 = ActualHeight - Margin;

            // Draw shadow track
            dc.DrawLine(new Pen(new SolidColorBrush(Colors.Chartreuse), 1), new Point(trackX + 1, trackY + 1), new Point(trackX + 1, trackY2 + 1));
            //    pen = new Pen(theme.CenterLineShadowColor);
            //    g.DrawLine(pen, new PointF(trackX + 1, trackY + 1), new PointF(trackX + 1, trackY2 + 1));
            //    pen.Dispose();
            //    pen = null;

            // Draw track
            dc.DrawLine(new Pen(new SolidColorBrush(Colors.BurlyWood), 1), new Point(trackX, trackY), new Point(trackX, trackY2));
            //    pen = new Pen(theme.CenterLineColor);
            //    g.DrawLine(pen, new PointF(trackX, trackY), new PointF(trackX, trackY2));
            //    pen.Dispose();
            //    pen = null;

            // Get the track height (remove margin from top and bottom)            
            _trackHeight = ActualHeight - (Margin * 2);

            // Get tick width
            double tickHeight = _trackHeight / _valueRange;

            // Get the percentage of the value relative to value range (needed to draw the fader).
            // We need to divide the value relative to value range to the value range to get the ratio.
            // Ex: Min = 50, Max = 150, Value = 100. Value relative to value range = 50. Value range = 100. Value ratio = 0.5
            _valueRatio = (_valueRelativeToValueRange / _valueRange);

            // Calculate fader position
            // We need to invert the values (i.e. max is on top, min is bottom)
            //float valueY = (valueRatio * trackHeight) + Margin;            
            double valueY = _trackHeight - (_valueRatio * _trackHeight) + Margin;
            double faderY = valueY + ((tickHeight - FaderHeight) / 2);
            double tickCenterY = valueY + (tickHeight / 2);

            // Create fader rectangle            
            _rectFader = new Rect((ActualWidth / 2) - (FaderWidth / 2), faderY, FaderWidth, FaderHeight);

            //    // Draw tick zone (for debug)
            //    //RectangleF rectTickZone = new RectangleF(valueX, 0, tickWidth, Height);
            //    //g.FillRectangle(Brushes.DarkGray, rectTickZone);            

            var rectFaderShadowTop = new Rect((ActualWidth / 2) - (FaderWidth / 2) + 1, faderY + 1, FaderWidth, 8);
            var rectFaderShadowBottom = new Rect((ActualWidth / 2) - (FaderWidth / 2) + 1, faderY + FaderHeight - 8 + 1, FaderWidth, 8);
            var rectFaderShadowCenter = new Rect((ActualWidth / 2) - (FaderWidth / 2) + 1, faderY + 4 + 1, FaderWidth, FaderHeight - 8);

            dc.DrawEllipse(Brushes.DarkViolet, new Pen(), rectFaderShadowTop.Center(), rectFaderShadowTop.Width / 2, rectFaderShadowTop.Height / 2);
            dc.DrawEllipse(Brushes.Orange, new Pen(), rectFaderShadowBottom.Center(), rectFaderShadowBottom.Width / 2, rectFaderShadowBottom.Height / 2);
            dc.DrawRectangle(Brushes.ForestGreen, new Pen(), rectFaderShadowCenter);
            //    brush = new SolidBrush(theme.FaderShadowColor);
            //    g.FillEllipse(brush, rectFaderShadowTop);
            //    g.FillEllipse(brush, rectFaderShadowBottom);
            //    g.FillRectangle(brush, rectFaderShadowCenter);
            //    brush.Dispose();
            //    brush = null;

            // Draw fader outline (with 8px border)            
            var rectFaderTop = new Rect((ActualWidth / 2) - (FaderWidth / 2), faderY, FaderWidth, 8);
            var rectFaderBottom = new Rect((ActualWidth / 2) - (FaderWidth / 2), faderY + FaderHeight - 8, FaderWidth, 8);
            var rectFaderBottomCenter = new Rect((ActualWidth / 2) - (FaderWidth / 2), faderY + FaderHeight - 10, FaderWidth, 6);
            var rectFaderCenter = new Rect((ActualWidth / 2) - (FaderWidth / 2), faderY + 4, FaderWidth, FaderHeight - 8);

            dc.DrawEllipse(new LinearGradientBrush(Colors.CadetBlue, Colors.DarkBlue, 90), new Pen(), rectFaderTop.Center(), rectFaderTop.Width / 2, rectFaderTop.Height / 2);
            //    brushGradient = new LinearGradientBrush(rectFaderTop, theme.FaderGradient.Color1, theme.FaderGradient.Color2, theme.FaderGradient.GradientMode);
            //    g.FillEllipse(brushGradient, rectFaderTop);
            //    brushGradient.Dispose();
            //    brushGradient = null;

            dc.DrawEllipse(new SolidColorBrush(Colors.DarkGoldenrod), new Pen(), rectFaderBottom.Center(), rectFaderBottom.Width / 2, rectFaderBottom.Height / 2);
            //    brush = new SolidBrush(theme.FaderShadowGradient.Color1);
            //    g.FillEllipse(brush, rectFaderBottom);
            //    brush.Dispose();
            //    brush = null;

            dc.DrawRectangle(new SolidColorBrush(Colors.DarkKhaki), new Pen(), rectFaderCenter);
            //    brush = new SolidBrush(theme.FaderGradient.Color2);
            //    g.FillRectangle(brush, rectFaderCenter);
            //    brush.Dispose();
            //    brush = null;

            dc.DrawRectangle(new SolidColorBrush(Colors.DarkMagenta), new Pen(), rectFaderBottomCenter);
            //    brush = new SolidBrush(theme.FaderShadowGradient.Color1);
            //    g.FillRectangle(brush, rectFaderBottomCenter);
            //    brush.Dispose();
            //    brush = null;

            // Draw fader inside (with 4px border)
            var rectFaderInsideBottom = new Rect((ActualWidth / 2) - (FaderWidth / 2) + 1, faderY + FaderHeight - 8, FaderWidth - 2, 4);
            var rectFaderInsideBottomCenter = new Rect((ActualWidth / 2) - (FaderWidth / 2) + 1, faderY + FaderHeight - 12, FaderWidth - 2, FaderHeight - 24);

            var rectFaderInsideTop = new Rect((ActualWidth / 2) - (FaderWidth / 2) + 1, faderY + 4, FaderWidth - 2, 8);
            var rectFaderInsideTopCenter = new Rect((ActualWidth / 2) - (FaderWidth / 2) + 1, faderY + 8, FaderWidth - 2, FaderHeight - 24);

            dc.DrawEllipse(new SolidColorBrush(Colors.DodgerBlue), new Pen(), rectFaderInsideTop.Center(), rectFaderInsideTop.Width / 2, rectFaderInsideTop.Height / 2);
            //    brush = new SolidBrush(theme.FaderShadowGradient.Color1);
            //    g.FillEllipse(brush, rectFaderInsideTop);
            //    brush.Dispose();
            //    brush = null;

            dc.DrawEllipse(new LinearGradientBrush(Colors.FloralWhite, Colors.Gold, 90), new Pen(), rectFaderInsideTopCenter.Center(), rectFaderInsideTopCenter.Width / 2, rectFaderInsideTopCenter.Height / 2);
            //    brushGradient = new LinearGradientBrush(rectFaderInsideTopCenter, theme.FaderShadowGradient.Color1, theme.FaderShadowGradient.Color2, LinearGradientMode.Vertical);
            //    g.FillRectangle(brushGradient, rectFaderInsideTopCenter);
            //    brushGradient.Dispose();
            //    brushGradient = null;

            dc.DrawEllipse(new SolidColorBrush(Colors.Peru), new Pen(), rectFaderInsideBottom.Center(), rectFaderInsideBottom.Width / 2, rectFaderInsideBottom.Height / 2);
            //    brush = new SolidBrush(theme.FaderGradient.Color2);
            //    g.FillEllipse(brush, rectFaderInsideBottom);
            //    brush.Dispose();
            //    brush = null;

            dc.DrawRectangle(new SolidColorBrush(Colors.PowderBlue), new Pen(), rectFaderInsideBottomCenter);
            //    brush = new SolidBrush(theme.FaderGradient.Color2);
            //    g.FillRectangle(brush, rectFaderInsideBottomCenter);
            //    brush.Dispose();
            //    brush = null;

            dc.DrawLine(new Pen(Brushes.Firebrick, 1), new Point((ActualWidth / 2) - (FaderWidth / 2), tickCenterY), new Point((ActualWidth / 2) - (FaderWidth / 2) + FaderWidth, tickCenterY));
            //    // Draw center of fader
            //    pen = new Pen(theme.FaderMiddleLineColor);
            //    //g.DrawLine(pen, new PointF(tickCenterX, (Height / 2) - (FaderHeight / 2)), new PointF(tickCenterX, (Height / 2) - (FaderHeight / 2) + FaderHeight));
            //    g.DrawLine(pen, new PointF((Width / 2) - (FaderWidth / 2), tickCenterY), new PointF((Width / 2) - (FaderWidth / 2) + FaderWidth, tickCenterY));
            //    pen.Dispose();
            //    pen = null;

            //    // Draw bitmap on control
            //    pe.Graphics.DrawImage(bmp, 0, 0, ClientRectangle, GraphicsUnit.Pixel);

            //    // Dispose graphics and bitmap
            //    bmp.Dispose();
            //    bmp = null;
            //    g.Dispose();
            //    g = null;

            //    base.OnPaint(pe);
            //}
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            _mouseButtonDown = true;

            // Make sure the mouse button pressed was the left mouse button
            if (e.ChangedButton == MouseButton.Left)
            {
                var location = e.GetPosition(this);

                // Check if the user clicked in the fader area
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
            if (!_isTrackBarMoving)
            {
                var location = e.GetPosition(this);

                // The user clicked without dragging the mouse; we need to add or
                // substract a "step" depending on the mouse cursor position.
                if (location.Y < _rectFader.Y)
                {
                    if (Value + StepSize > Maximum)
                        Value = Maximum;
                    else
                        Value += StepSize;
                }
                else if (location.Y > _rectFader.Y + _rectFader.Height)
                {
                    if (Value - StepSize < Minimum)
                        Value = Minimum;
                    else
                        Value -= StepSize;
                }

                if (OnFaderValueChanged != null)
                    OnFaderValueChanged(this, new EventArgs());

                InvalidateVisual();
            }

            _mouseButtonDown = false;
            _isTrackBarMoving = false;
            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            bool valueChanged = false;
            if (_isTrackBarMoving && _mouseButtonDown)
            {
                // Evaluate tick height
                var location = e.GetPosition(this);
                double tickHeight = _trackHeight / _valueRange;

                // Loop through "steps"                
                for (int a = Minimum; a < Maximum + 1; a++)
                {
                    double startY = _trackHeight - ((a - Minimum + 1) * tickHeight);
                    double endY = _trackHeight - ((a - Minimum) * tickHeight);

                    // Adjust cursor position relative to margin
                    double cursorY = location.Y - Margin;

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

                    InvalidateVisual();
                }
            }

            base.OnMouseMove(e);
        }
    }
}
