//
// TrackBar.cs: This trackbar control is based on the System.Windows.Forms.TrackBar control.
//              It adds custom drawing and other features.
//
// Copyright © 2011-2012 Yanick Castonguay
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
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This trackbar control is based on the System.Windows.Forms.TrackBar control.
    /// It adds custom drawing and other features.
    /// </summary>
    public class TrackBar : Control
    {
        // Private variables
        private RectangleF rectFader = new RectangleF();
        private bool isTrackBarMoving = false;
        private float trackWidth = 0;
        private float valueRatio = 0;
        private float valueRelativeToValueRange = 0;
        private float valueRange = 0;

        // Private variables
        private bool m_mouseButtonDown = false;
        /// <summary>
        /// Indicates if a mouse button is down.
        /// </summary>
        public bool MouseButtonDown
        {
            get
            {
                return m_mouseButtonDown;
            }
        }

        #region Background Properties

        private Color m_gradientColor1 = Color.DarkGray;
        /// <summary>
        /// First color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Background"), Browsable(true), Description("First color of the background gradient.")]
        public Color GradientColor1
        {
            get
            {
                return m_gradientColor1;
            }
            set
            {
                m_gradientColor1 = value;
            }
        }

        private Color m_gradientColor2 = Color.Gray;
        /// <summary>
        /// Second color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Background"), Browsable(true), Description("Second color of the background gradient.")]
        public Color GradientColor2 
        {  
            get
            {
                return m_gradientColor2;
            }
            set
            {
                m_gradientColor2 = value;
            }
        }

        private LinearGradientMode m_gradientMode = LinearGradientMode.Vertical;
        /// <summary>
        /// Background gradient mode.
        /// </summary>
        [Category("Background"), Browsable(true), Description("Background gradient mode.")]
        public LinearGradientMode GradientMode
        {
            get
            {
                return m_gradientMode;
            }
            set
            {
                m_gradientMode = value;
            }
        }

        #endregion

        #region Track Bar Properties

        private int m_faderWidth = 20;
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
                return m_faderWidth;
            }
            set
            {
                m_faderWidth = value;
            }
        }

        private int m_faderHeight = 15;
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
                return m_faderHeight;
            }
            set
            {
                m_faderHeight = value;
            }
        }

        private int m_margin = 16;
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
                return m_margin;
            }
            set
            {
                m_margin = value;
            }
        }

        private int m_stepSize = 1;
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
                return m_stepSize;
            }
            set
            {
                m_stepSize = value;
            }
        }       
        
        private Color m_faderGradientColor1 = Color.LightGray;
        /// <summary>
        /// First color of the fader gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("First color of the fader gradient.")]
        public Color FaderGradientColor1
        {
            get
            {
                return m_faderGradientColor1;
            }
            set
            {
                m_faderGradientColor1 = value;
            }
        }

        private Color m_faderGradientColor2 = Color.Gray;
        /// <summary>
        /// Second color of the fader gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Second color of the fader gradient.")]
        public Color FaderGradientColor2
        {
            get
            {
                return m_faderGradientColor2;
            }
            set
            {
                m_faderGradientColor2 = value;
            }
        }

        private Color m_faderShadowGradientColor1 = Color.DarkGray;
        /// <summary>
        /// First color of the fader shadow gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("First color of the fader shadow gradient.")]
        public Color FaderShadowGradientColor1
        {
            get
            {
                return m_faderShadowGradientColor1;
            }
            set
            {
                m_faderShadowGradientColor1 = value;
            }
        }

        private Color m_faderShadowGradientColor2 = Color.DarkGray;
        /// <summary>
        /// Second color of the fader shadow gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Second color of the fader shadow gradient.")]
        public Color FaderShadowGradientColor2
        {
            get
            {
                return m_faderShadowGradientColor2;
            }
            set
            {
                m_faderShadowGradientColor2 = value;
            }
        }

        private Color m_centerLineColor = Color.Black;
        /// <summary>
        /// Color used when drawing the center line.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Color used when drawing the center line.")]
        public Color CenterLineColor
        {
            get
            {
                return m_centerLineColor;
            }
            set
            {
                m_centerLineColor = value;
            }
        }

        private Color m_centerLineShadowColor = Color.DarkGray;
        /// <summary>
        /// Color used when drawing the center line shadow.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Color used when drawing the center line shadow.")]
        public Color CenterLineShadowColor
        {
            get
            {
                return m_centerLineShadowColor;
            }
            set
            {
                m_centerLineShadowColor = value;
            }
        }

        #endregion

        #region Other Properties

        private int m_minimum = 0;
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
                return m_minimum;
            }
            set
            {
                m_minimum = value;
            }
        }

        private int m_maximum = 10;
        /// <summary>
        /// Track bar maximum value.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), DefaultValue(10), Description("Track bar maximum value.")]
        public int Maximum
        {
            get
            {
                return m_maximum;
            }
            set
            {
                m_maximum = value;
            }
        }

        private int m_value = 0;
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
                return m_value;
            }
            set
            {
                m_value = value;

                // Raise refresh event
                this.Refresh();

                // Check if an event is subscribed
                if (OnTrackBarValueChanged != null)
                {
                    // Raise event                        
                    OnTrackBarValueChanged();
                }
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

        /// <summary>
        /// Default constructor for TrackBar.
        /// </summary>
        public TrackBar() : base()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                     ControlStyles.Opaque | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
        }

        #region Paint Events

        /// <summary>
        /// Occurs when the control needs to be painted.
        /// </summary>
        /// <param name="pe">Paint Event arguments</param>
        protected override void OnPaint(PaintEventArgs pe)
        {            
            // Declare variables
            LinearGradientBrush brushGradient = null;
            SolidBrush brush = null;
            Pen pen = null;

            // Value range is the size between max and min track bar value.
            // Ex: Min = 50, Max = 150. Value range = 100 + 1 (because we include 50 and 100)
            valueRange = (Maximum - Minimum) + 1;

            // Get track bar value relative to value range (value - minimum value).
            // Ex: Min = 50, Max = 150, Value = 100. Value relative to value range = 50.
            valueRelativeToValueRange = Value - Minimum;             

            // Create a bitmap the size of the form.
            Bitmap bmp = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
            Graphics g = Graphics.FromImage(bmp);

            // Set text anti-aliasing to ClearType (best looking AA)
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Set smoothing mode for paths
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw background gradient (cover -1 pixel for some refresh bug)
            Rectangle rectBody = new Rectangle(-1, -1, Width + 1, Height + 1);
            LinearGradientBrush brushBackground = new LinearGradientBrush(rectBody, GradientColor1, GradientColor2, GradientMode);
            g.FillRectangle(brushBackground, rectBody);
            brushBackground.Dispose();
            brushBackground = null;

            // Return if value range is zero
            if (valueRange == 0)
            {
                return;
            }           

            // Draw fader track
            float trackX = Margin; // add margin from left
            float trackX2 = Width - Margin; // add margin from right
            float trackY = Height / 2; // right in the center

            pen = new Pen(CenterLineShadowColor);
            g.DrawLine(pen, new PointF(trackX + 1, trackY + 1), new PointF(trackX2 + 1, trackY + 1));
            pen.Dispose();
            pen = null;

            pen = new Pen(CenterLineColor);
            g.DrawLine(pen, new PointF(trackX, trackY), new PointF(trackX2, trackY));
            pen.Dispose();
            pen = null;

            // Get the track width (remove margin from left and right)
            trackWidth = Width - (Margin * 2);

            // Get tick width
            float tickWidth = trackWidth / valueRange;           

            // Get the percentage of the value relative to value range (needed to draw the fader).
            // We need to divide the value relative to value range to the value range to get the ratio.
            // Ex: Min = 50, Max = 150, Value = 100. Value relative to value range = 50. Value range = 100. Value ratio = 0.5
            valueRatio = (valueRelativeToValueRange / valueRange);

            // Get the value X coordinate by multiplying the value ratio to the track bar width (removed 3 pixels from left
            // and right). Add margin from left.
            float valueX = (valueRatio * trackWidth) + Margin; // this gives the LEFT x for our zone
            float faderX = valueX + ((tickWidth - FaderWidth) / 2);
            float tickCenterX = valueX + (tickWidth / 2);

            // Create fader rectangle
            rectFader = new RectangleF(faderX, (Height / 2) - (FaderHeight / 2), FaderWidth, FaderHeight);           

            // Draw tick zone (for debug)
            //RectangleF rectTickZone = new RectangleF(valueX, 0, tickWidth, Height);
            //g.FillRectangle(Brushes.DarkGray, rectTickZone);            

            // Draw fader outline (with 8px border)
            RectangleF rectFaderLeft = new RectangleF(faderX, (Height / 2) - (FaderHeight / 2), 8, FaderHeight);
            RectangleF rectFaderRight = new RectangleF(faderX + FaderWidth - 8, (Height / 2) - (FaderHeight / 2), 8, FaderHeight);
            RectangleF rectFaderCenter = new RectangleF(faderX + 4, (Height / 2) - (FaderHeight / 2), FaderWidth - 8, FaderHeight);

            brushGradient = new LinearGradientBrush(rectFaderLeft, FaderGradientColor1, FaderGradientColor2, LinearGradientMode.Horizontal);
            g.FillEllipse(brushGradient, rectFaderLeft);
            brushGradient.Dispose();
            brushGradient = null;

            brushGradient = new LinearGradientBrush(rectFaderRight, FaderGradientColor1, FaderGradientColor2, LinearGradientMode.Horizontal);
            g.FillEllipse(brushGradient, rectFaderRight);
            brushGradient.Dispose();
            brushGradient = null;

            brush = new SolidBrush(FaderGradientColor2);
            g.FillRectangle(brush, rectFaderCenter);
            brush.Dispose();
            brush = null;

            // Draw fader inside (with 4px border)
            RectangleF rectFaderInsideLeft = new RectangleF(faderX + 2, (Height / 2) - (FaderHeight / 2) + 2, 4, FaderHeight - 4);
            RectangleF rectFaderInsideRight = new RectangleF(faderX + FaderWidth - 6, (Height / 2) - (FaderHeight / 2) + 2, 4, FaderHeight - 4);

            brushGradient = new LinearGradientBrush(rectFaderInsideLeft, FaderShadowGradientColor1, FaderShadowGradientColor2, LinearGradientMode.Horizontal);
            g.FillEllipse(brushGradient, rectFaderInsideLeft);
            brushGradient.Dispose();
            brushGradient = null;

            brushGradient = new LinearGradientBrush(rectFaderInsideRight, FaderShadowGradientColor1, FaderShadowGradientColor2, LinearGradientMode.Horizontal);
            g.FillEllipse(brushGradient, rectFaderInsideRight);
            brushGradient.Dispose();
            brushGradient = null;

            // Draw center of fader
            pen = new Pen(FaderShadowGradientColor2);
            g.DrawLine(pen, new PointF(tickCenterX, (Height / 2) - (FaderHeight / 2)), new PointF(tickCenterX, (Height / 2) - (FaderHeight / 2) + FaderHeight));
            pen.Dispose();
            pen = null;

            // Draw bitmap on control
            pe.Graphics.DrawImage(bmp, 0, 0, ClientRectangle, GraphicsUnit.Pixel);

            // Dispose graphics and bitmap
            bmp.Dispose();
            bmp = null;
            g.Dispose();
            g = null;

            base.OnPaint(pe);
        }

        #endregion

        #region Mouse Events

        /// <summary>
        /// Occurs when the mouse button is down.
        /// </summary>
        /// <param name="e">Mouse Event arguments</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            m_mouseButtonDown = true;

            // Make sure the mouse button pressed was the left mouse button
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                // Check if the user clicked in the fader area
                if (e.Location.X >= rectFader.X &&
                    e.Location.X <= rectFader.Width + rectFader.X &&
                    e.Location.Y >= rectFader.Y &&
                    e.Location.Y <= rectFader.Height + rectFader.Y)
                {
                    // Set track bar moving 
                    isTrackBarMoving = true;
                }
            }

            // Call the client side events
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Occurs when the mouse button is released.
        /// </summary>
        /// <param name="e">Mouse Event arguments</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            // Check if the track bar was moving (mouse down)
            if (!isTrackBarMoving)
            {
                // The user clicked without dragging the mouse; we need to add or
                // substract a "step" depending on the mouse cursor position.
                if (e.Location.X < rectFader.X)
                {
                    if (Value - StepSize < Minimum)
                    {
                        Value = Minimum;
                    }
                    else
                    {
                        Value -= StepSize;
                    }
                }
                else if (e.Location.X > rectFader.X + rectFader.Width)
                {
                    if (Value + StepSize > Maximum)
                    {
                        Value = Maximum;
                    }
                    else
                    {
                        Value += StepSize;
                    }
                }                                

                // Check if an event is subscribed
                if (OnTrackBarValueChanged != null)
                {
                    // Raise event                        
                    OnTrackBarValueChanged();
                }

                // Refresh control
                this.Refresh();
            }

            // Reset flags
            m_mouseButtonDown = false;
            isTrackBarMoving = false;

            // Call the client side events
            base.OnMouseUp(e);
        }

        /// <summary>
        /// Occurs when the mouse cursor moves over the control.
        /// </summary>
        /// <param name="e">Mouse Event arguments</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Set value changed flag default to false
            bool valueChanged = false;

            if (isTrackBarMoving && m_mouseButtonDown)
            {
                // Need to evaluate...

                float tickWidth = trackWidth / valueRange;                

                // Loop through "steps"  MAX+1????
                //for (int a = 0; a < Maximum + 1; a++)
                for (int a = Minimum; a < Maximum + 1; a++)
                {                    
                    float startX = (a - Minimum) * tickWidth;
                    float endX = (a - Minimum + 1) * tickWidth;

                    // Adjust cursor position relative to margin
                    int cursorX = e.Location.X - Margin;

                    // Does the cursor exceed min or max?
                    if(cursorX <= 0)
                    {
                        // Don't change the value if it's already the same!
                        if (Value != Minimum)
                        {
                            // Set value to minimum
                            Value = Minimum;

                            // Set value changed to true
                            valueChanged = true;
                            break;
                        }
                    }
                    else if(cursorX >= trackWidth)
                    {
                        // Don't change the value if it's already the same!
                        if (Value != Maximum)
                        {
                            // Set value to maximum
                            Value = Maximum;

                            // Set value changed to true
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
                            // Set value to new value
                            Value = a;

                            // Set value changed to true
                            valueChanged = true;
                            break;
                        }
                    }
                }

                // If the value has changed, refresh control and raise event
                if (valueChanged)
                {
                    // Check if an event is subscribed
                    if (OnTrackBarValueChanged != null)
                    {
                        // Raise event                        
                        OnTrackBarValueChanged();
                    }

                    // Refresh control
                    this.Refresh();
                }
            }

            // Call the client side events
            base.OnMouseMove(e);
        }

        #endregion
    }
}
