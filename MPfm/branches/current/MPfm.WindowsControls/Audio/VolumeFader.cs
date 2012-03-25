﻿//
// VolumeFader.cs: This volume fader control is basically a vertical track bar, but with the appearance of a fader.
//                 The control appearance can be changed using the public properties.
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
using MPfm.WindowsControls;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This volume fader control is basically a vertical track bar, but with the appearance of a fader.
    /// The control appearance can be changed using the public properties.
    /// </summary>
    public class VolumeFader : Control
    {
        // Private variables
        private RectangleF rectFader = new RectangleF();
        private bool isTrackBarMoving = false;
        private float trackHeight = 0;
        private float valueRatio = 0;
        private float valueRelativeToValueRange = 0;
        private float valueRange = 0;

        private bool mouseButtonDown = false;
        /// <summary>
        /// Indicates if a mouse button is down.
        /// </summary>
        public bool MouseButtonDown
        {
            get
            {
                return mouseButtonDown;
            }
        }

        #region Background Properties

        private Color gradientColor1 = Color.DarkGray;
        /// <summary>
        /// First color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Background"), Browsable(true), Description("First color of the background gradient.")]
        public Color GradientColor1
        {
            get
            {
                return gradientColor1;
            }
            set
            {
                gradientColor1 = value;
            }
        }

        private Color gradientColor2 = Color.Gray;
        /// <summary>
        /// Second color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Background"), Browsable(true), Description("Second color of the background gradient.")]
        public Color GradientColor2 
        {  
            get
            {
                return gradientColor2;
            }
            set
            {
                gradientColor2 = value;
            }
        }

        private LinearGradientMode gradientMode = LinearGradientMode.Vertical;
        /// <summary>
        /// Background gradient mode.
        /// </summary>
        [Category("Background"), Browsable(true), Description("Background gradient mode.")]
        public LinearGradientMode GradientMode
        {
            get
            {
                return gradientMode;
            }
            set
            {
                gradientMode = value;
            }
        }

        #endregion

        #region Fader Properties

        private Color faderMiddleLineColor = Color.Black;
        /// <summary>
        /// Color of the line in the center of the fader.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Color of the line in the center of the fader.")]
        public Color FaderMiddleLineColor
        {
            get
            {
                return faderMiddleLineColor;
            }
            set
            {
                faderMiddleLineColor = value;
            }
        }

        private Color faderShadowColor = Color.DarkGray;
        /// <summary>
        /// Color of the fader shadow.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Color of the fader shadow.")]
        public Color FaderShadowColor
        {
            get
            {
                return faderShadowColor;
            }
            set
            {
                faderShadowColor = value;
            }
        }

        private int faderWidth = 20;
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
                return faderWidth;
            }
            set
            {
                faderWidth = value;
            }
        }

        private int faderHeight = 15;
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
                return faderHeight;
            }
            set
            {
                faderHeight = value;
            }
        }

        private Color faderGradientColor1 = Color.LightGray;
        /// <summary>
        /// First color of the fader gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("First color of the fader gradient.")]        
        public Color FaderGradientColor1
        {
            get
            {
                return faderGradientColor1;
            }
            set
            {
                faderGradientColor1 = value;
            }
        }

        private Color faderGradientColor2 = Color.Gray;
        /// <summary>
        /// Second color of the fader gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Second color of the fader gradient.")]        
        public Color FaderGradientColor2
        {
            get
            {
                return faderGradientColor2;
            }
            set
            {
                faderGradientColor2 = value;
            }
        }

        private Color faderShadowGradientColor1 = Color.DarkGray;
        /// <summary>
        /// First color of the fader shadow gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("First color of the fader shadow gradient.")]        
        public Color FaderShadowGradientColor1
        {
            get
            {
                return faderShadowGradientColor1;
            }
            set
            {
                faderShadowGradientColor1 = value;
            }
        }

        private Color faderShadowGradientColor2 = Color.DarkGray;
        /// <summary>
        /// Second color of the fader shadow gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Second color of the fader shadow gradient.")]        
        public Color FaderShadowGradientColor2
        {
            get
            {
                return faderShadowGradientColor2;
            }
            set
            {
                faderShadowGradientColor2 = value;
            }
        }

        #endregion

        #region Other Properties
        
        private int minimum = 0;
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
                return minimum;
            }
            set
            {
                minimum = value;
            }
        }

        private int maximum = 10;
        /// <summary>
        /// Fader maximum value.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), DefaultValue(10), Description("Fader maximum value.")]
        public int Maximum
        {
            get
            {
                return maximum;
            }
            set
            {
                maximum = value;
            }
        }

        private int value = 0;
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
                return value;
            }
            set
            {
                this.value = value;

                // Raise refresh event
                this.Refresh();

                // Check if an event is subscribed
                if (OnFaderValueChanged != null)
                {
                    // Raise event                        
                    OnFaderValueChanged(this, new EventArgs());
                }
            }
        }

        private int margin = 16;
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
                return margin;
            }
            set
            {
                margin = value;
            }
        }

        private int stepSize = 1;
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
                return stepSize;
            }
            set
            {
                stepSize = value;
            }
        }

        private Color centerLineColor = Color.Black;
        /// <summary>
        /// Color used when drawing the center line.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Color used when drawing the center line.")]
        public Color CenterLineColor
        {
            get
            {
                return centerLineColor;
            }
            set
            {
                centerLineColor = value;
            }
        }

        private Color centerLineShadowColor = Color.DarkGray;
        /// <summary>
        /// Color used when drawing the center line shadow.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Color used when drawing the center line shadow.")]
        public Color CenterLineShadowColor
        {
            get
            {
                return centerLineShadowColor;
            }
            set
            {
                centerLineShadowColor = value;
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

        /// <summary>
        /// Default constructor for VolumeFader.
        /// </summary>
        public VolumeFader()
            : base()
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

            // Create a bitmap the size of the control
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
            //float trackX = Margin; // add margin from left
            //float trackX2 = Width - Margin; // add margin from right
            //float trackY = Height / 2; // right in the center

            float trackX = Width / 2;
            float trackY = Margin;
            float trackY2 = Height - Margin;

            // Draw shadow track
            pen = new Pen(CenterLineShadowColor);
            g.DrawLine(pen, new PointF(trackX + 1, trackY + 1), new PointF(trackX + 1, trackY2 + 1));
            pen.Dispose();
            pen = null;

            // Draw track
            pen = new Pen(CenterLineColor);
            g.DrawLine(pen, new PointF(trackX, trackY), new PointF(trackX, trackY2));
            pen.Dispose();
            pen = null;

            // Get the track height (remove margin from top and bottom)            
            trackHeight = Height - (Margin * 2);

            // Get tick width
            float tickHeight = trackHeight / valueRange;           

            // Get the percentage of the value relative to value range (needed to draw the fader).
            // We need to divide the value relative to value range to the value range to get the ratio.
            // Ex: Min = 50, Max = 150, Value = 100. Value relative to value range = 50. Value range = 100. Value ratio = 0.5
            valueRatio = (valueRelativeToValueRange / valueRange);

            // Calculate fader position
            // We need to invert the values (i.e. max is on top, min is bottom)
            //float valueY = (valueRatio * trackHeight) + Margin;            
            float valueY = trackHeight - (valueRatio * trackHeight) + Margin;
            float faderY = valueY + ((tickHeight - FaderHeight) / 2);
            float tickCenterY = valueY + (tickHeight / 2);

            // Create fader rectangle            
            rectFader = new RectangleF((Width / 2) - (FaderWidth / 2), faderY, FaderWidth, FaderHeight);           

            // Draw tick zone (for debug)
            //RectangleF rectTickZone = new RectangleF(valueX, 0, tickWidth, Height);
            //g.FillRectangle(Brushes.DarkGray, rectTickZone);            

            RectangleF rectFaderShadowTop = new RectangleF((Width / 2) - (FaderWidth / 2) + 1, faderY + 1, FaderWidth, 8);
            RectangleF rectFaderShadowBottom = new RectangleF((Width / 2) - (FaderWidth / 2) + 1, faderY + FaderHeight - 8 + 1, FaderWidth, 8);            
            RectangleF rectFaderShadowCenter = new RectangleF((Width / 2) - (FaderWidth / 2) + 1, faderY + 4 + 1, FaderWidth, FaderHeight - 8);

            brush = new SolidBrush(FaderShadowColor);
            g.FillEllipse(brush, rectFaderShadowTop);
            g.FillEllipse(brush, rectFaderShadowBottom);
            g.FillRectangle(brush, rectFaderShadowCenter);
            brush.Dispose();
            brush = null;

            // Draw fader outline (with 8px border)            
            RectangleF rectFaderTop = new RectangleF((Width / 2) - (FaderWidth / 2), faderY, FaderWidth, 8);            
            RectangleF rectFaderBottom = new RectangleF((Width / 2) - (FaderWidth / 2), faderY + FaderHeight - 8, FaderWidth, 8);
            RectangleF rectFaderBottomCenter = new RectangleF((Width / 2) - (FaderWidth / 2), faderY + FaderHeight - 10, FaderWidth, 6);            
            RectangleF rectFaderCenter = new RectangleF((Width / 2) - (FaderWidth / 2), faderY + 4, FaderWidth, FaderHeight - 8);

            brushGradient = new LinearGradientBrush(rectFaderTop, FaderGradientColor1, FaderGradientColor2, LinearGradientMode.Vertical);
            g.FillEllipse(brushGradient, rectFaderTop);            
            brushGradient.Dispose();
            brushGradient = null;

            brush = new SolidBrush(FaderShadowGradientColor1);
            g.FillEllipse(brush, rectFaderBottom);
            brush.Dispose();
            brush = null;

            brush = new SolidBrush(FaderGradientColor2);
            g.FillRectangle(brush, rectFaderCenter);            
            brush.Dispose();
            brush = null;

            brush = new SolidBrush(FaderShadowGradientColor1);
            g.FillRectangle(brush, rectFaderBottomCenter);
            brush.Dispose();
            brush = null;

            // Draw fader inside (with 4px border)
            RectangleF rectFaderInsideBottom = new RectangleF((Width / 2) - (FaderWidth / 2) + 1, faderY + FaderHeight - 8, FaderWidth - 2, 4);
            RectangleF rectFaderInsideBottomCenter = new RectangleF((Width / 2) - (FaderWidth / 2) + 1, faderY + FaderHeight - 12, FaderWidth - 2, FaderHeight - 24);

            RectangleF rectFaderInsideTop = new RectangleF((Width / 2) - (FaderWidth / 2) + 1, faderY + 4, FaderWidth - 2, 8);
            RectangleF rectFaderInsideTopCenter = new RectangleF((Width / 2) - (FaderWidth / 2) + 1, faderY + 8, FaderWidth - 2, FaderHeight - 24);

            brush = new SolidBrush(FaderShadowGradientColor1);
            g.FillEllipse(brush, rectFaderInsideTop);
            brush.Dispose();
            brush = null;

            brushGradient = new LinearGradientBrush(rectFaderInsideTopCenter, FaderShadowGradientColor1, FaderShadowGradientColor2, LinearGradientMode.Vertical);
            g.FillRectangle(brushGradient, rectFaderInsideTopCenter);
            brushGradient.Dispose();
            brushGradient = null;

            brush = new SolidBrush(FaderGradientColor2);
            g.FillEllipse(brush, rectFaderInsideBottom);
            brush.Dispose();
            brush = null;

            brush = new SolidBrush(FaderGradientColor2);
            g.FillRectangle(brush, rectFaderInsideBottomCenter);
            brush.Dispose();
            brush = null;

            // Draw center of fader
            pen = new Pen(FaderMiddleLineColor);
            //g.DrawLine(pen, new PointF(tickCenterX, (Height / 2) - (FaderHeight / 2)), new PointF(tickCenterX, (Height / 2) - (FaderHeight / 2) + FaderHeight));
            g.DrawLine(pen, new PointF((Width / 2) - (FaderWidth / 2), tickCenterY), new PointF((Width / 2) - (FaderWidth / 2) + FaderWidth, tickCenterY));
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
            mouseButtonDown = true;

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
                if (e.Location.Y < rectFader.Y)
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
                else if (e.Location.Y > rectFader.Y + rectFader.Height)
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

                // Check if an event is subscribed
                if (OnFaderValueChanged != null)
                {
                    // Raise event                        
                    OnFaderValueChanged(this, new EventArgs());
                }

                // Refresh control
                this.Refresh();
            }

            // Reset flags
            mouseButtonDown = false;
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

            if (isTrackBarMoving && mouseButtonDown)
            {
                // Evaluate tick height
                float tickHeight = trackHeight / valueRange;

                // Loop through "steps"                
                for (int a = Minimum; a < Maximum + 1; a++)
                {
                    float startY = trackHeight - ((a - Minimum + 1) * tickHeight);
                    float endY = trackHeight - ((a - Minimum) * tickHeight);                   

                    // Adjust cursor position relative to margin
                    //int cursorX = e.Location.X - Margin;
                    int cursorY = e.Location.Y - Margin;

                    // Does the cursor exceed min or max?
                    if (cursorY <= 0)
                    {
                        // Don't change the value if it's already the same!
                        if (Value != Maximum)
                        {
                            // Set value 
                            Value = Maximum;

                            // Set value changed to true
                            valueChanged = true;
                            break;
                        }
                    }
                    else if (cursorY >= trackHeight)
                    {
                        // Don't change the value if it's already the same!
                        if (Value != Minimum)
                        {
                            // Set value 
                            Value = Minimum;

                            // Set value changed to true
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
                    if (OnFaderValueChanged != null)
                    {
                        // Raise event                        
                        OnFaderValueChanged(this, new EventArgs());
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
