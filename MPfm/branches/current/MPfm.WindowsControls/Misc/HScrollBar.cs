//
// HScrollBar.cs: This control is a custom horizontal scrollbar.
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
using System.Text;
using System.Windows.Forms;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This custom horizontal scrollbar control is based on System.Windows.Forms.Control instead of HScrollBar.
    /// It adds custom drawing, and most impotantly, the MouseUp event.
    /// </summary>
    public class HScrollBar : System.Windows.Forms.Control
    {
        // Scrollbar zones
        Rectangle rectScrollBarLeftHandle;
        Rectangle rectScrollBarRightHandle;
        Rectangle rectScrollBarThumb;

        // Mouse over information
        private bool isMouseOverThumb = false;
        private bool isMouseOverLeftHandle = false;
        private bool isMouseOverRightHandle = false;
        
        public int mouseDownX = 0;
        public int originalValue = 0;

        private int thumbDraggingOffsetX = 0;
        private bool isUserDraggingThumb = false;

        private bool isMouseLeftButtonDown = false;

        private Timer timerMouseDown = null;

        #region Properties

        private int m_maximum = 100;
        /// <summary>
        /// Maximum value (value range = Maximum - Minimum)
        /// </summary>
        public int Maximum
        {
            get
            {
                return m_maximum;
            }
            set
            {
                m_maximum = value;

                // Check if the value exceeds the new maximum
                if (m_value > m_maximum)
                {
                    // Set value to the new maximum
                    m_value = m_maximum;
                }
                Refresh();
            }
        }

        private int m_minimum = 0;
        /// <summary>
        /// Minimum value (value range = Maximum - Minimum)
        /// </summary>
        public int Minimum
        {
            get
            {
                return m_minimum;
            }
            set
            {
                m_minimum = value;
                Refresh();
            }
        }

        private int m_largeChange = 100;
        /// <summary>
        /// Value change when the user clicks on the scrollbar background
        /// </summary>
        public int LargeChange
        {
            get
            {
                return m_largeChange;
            }
            set
            {
                m_largeChange = value;
                Refresh();
            }
        }

        private int m_smallChange = 10;
        /// <summary>
        /// Value change when the user clicks on one of the scrollbar handles
        /// </summary>
        public int SmallChange
        {
            get
            {
                return m_smallChange;
            }
            set
            {
                m_smallChange = value;                
            }
        }

        public int m_value = 0;
        /// <summary>
        /// Value (value range = Maximum - Minimum)
        /// </summary>
        public int Value
        {
            get
            {
                return m_value;
            }
            set
            {
                // Check if the value is the same (do not trigger events!)
                if (value == m_value)
                {
                    return;
                }

                // Set value
                m_value = value;

                // Check if an event is subscribed
                if (OnValueChanged != null)
                {
                    // Raise event                        
                    OnValueChanged(this, new EventArgs());
                }
                Refresh();
            }
        }

        #endregion

        #region Font Properties

        /// <summary>
        /// Private value for the CustomFont property.
        /// </summary>
        private CustomFont m_customFont = null;
        /// <summary>
        /// Defines the font to be used for rendering the control.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Font used for rendering the control.")]
        public CustomFont CustomFont
        {
            get
            {
                return m_customFont;
            }
            set
            {
                m_customFont = value;
                Refresh();
            }
        }

        #endregion

        #region Constructor
        
        /// <summary>
        /// Default constructor for Label.
        /// </summary>
        public HScrollBar()
        {
            // Set control styles
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                ControlStyles.Opaque | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);

            // Create default font
            m_customFont = new WindowsControls.CustomFont();

            // Create timer for mouse down
            timerMouseDown = new Timer();
            timerMouseDown.Enabled = false;            
            timerMouseDown.Interval = 200;
            timerMouseDown.Tick += new EventHandler(timerMouseDown_Tick);
        }

        /// <summary>
        /// Occurs when the timer for mouse down operations has finished.
        /// Changes the scrollbar value depending on where the user has clicked
        /// (left/right handle, scrollbar background).
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        public void timerMouseDown_Tick(object sender, EventArgs e)
        {
            // Declare variables
            int value = 0;

            // Check if the user has clicked on one of the handles (i.e. small change)
            if (mouseDownX <= 14)
            {
                // Left handle; negative small change
                value = Value - SmallChange;

                // Make sure the value doesn't go lower than the minimum
                if (value < Minimum)
                {
                    // Set value as minimum
                    value = Minimum;
                }

                // Set new value (will refresh the scrollbar automatically)
                Value = value;
            }
            else if (mouseDownX >= Width - 14)
            {
                // Right handle; positive small change
                value = Value + SmallChange;

                // Make sure the value doesn't go higher than the maximum
                if (value > Maximum)
                {
                    // Set value as maximum
                    value = Maximum;
                }

                // Set new value (will refresh the scrollbar automatically)
                Value = value;
            }

            // Check if the user has clicked on the scrollbar background (i.e. large change)
            if (mouseDownX > 14 &&
                mouseDownX < rectScrollBarThumb.Left)
            {
                // Left portion; negative large change
                value = Value - LargeChange;

                // Make sure the value doesn't go lower than the minimum
                if (value < Minimum)
                {
                    // Set value as minimum
                    value = Minimum;
                }

                // Set new value (will refresh the scrollbar automatically)
                Value = value;
            }
            else if (mouseDownX < Width - 14 &&
                     mouseDownX > rectScrollBarThumb.Right)
            {
                // Right portion; positive large change
                value = Value + LargeChange;

                // Make sure the value doesn't go higher than the maximum
                if (value > Maximum)
                {
                    // Set value as maximum
                    value = Maximum;
                }

                // Set new value (will refresh the scrollbar automatically)
                Value = value;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the scrollbar value changes.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        public delegate void ValueChanged(object sender, EventArgs e);
        /// <summary>
        /// Hook for the scrollbar value changed event.
        /// </summary>
        public event ValueChanged OnValueChanged;

        #endregion

        #region Paint Events

        /// <summary>
        /// Occurs when the control needs to be painted.
        /// </summary>
        /// <param name="pe">Paint Event Arguments</param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            // Create objects
            LinearGradientBrush brushGradient = null;
            Pen pen = null;
            Color color1;
            Color color2;

            // Get graphics from event
            Graphics g = pe.Graphics;

            // Use anti-aliasing?
            if (CustomFont.UseAntiAliasing)
            {
                // Set text anti-aliasing to ClearType (best looking AA)
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                // Set smoothing mode for paths
                g.SmoothingMode = SmoothingMode.AntiAlias;
            }

            // Call paint background
            base.OnPaintBackground(pe);

            // Render scrollbar main background
            Rectangle rect = new Rectangle(14, Height - 14, Width - 28, 14);
            brushGradient = new LinearGradientBrush(rect, Color.FromArgb(150, 75, 75, 75), Color.FromArgb(255, 100, 100, 100), 90.0f);
            g.FillRectangle(brushGradient, rect);
            brushGradient.Dispose();
            brushGradient = null;

            // ----------------------------------
            // Left Handle

            // Set left handle background colors            
            color1 = Color.FromArgb(150, 150, 150, 150);
            color2 = Color.FromArgb(200, 200, 200, 200);
            if (isMouseOverLeftHandle)
            {
                color1 = Color.FromArgb(200, 200, 200, 200);
                color2 = Color.FromArgb(255, 220, 220, 220);
            }

            // Render left handle background
            rectScrollBarLeftHandle = new Rectangle(0, Height - 14, 14, 14);
            brushGradient = new LinearGradientBrush(rectScrollBarLeftHandle, color1, color2, 90.0f);
            g.FillRectangle(brushGradient, rectScrollBarLeftHandle);
            brushGradient.Dispose();
            brushGradient = null;

            // Set left handle arrow colors            
            color1 = Color.FromArgb(200, 0, 0, 0);
            color2 = Color.FromArgb(225, 50, 50, 50);
            if (isMouseOverLeftHandle)
            {
                color1 = Color.FromArgb(240, 50, 50, 50);
                color2 = Color.FromArgb(255, 80, 80, 80);
            }

            // Render left handle arrow
            List<Point> listPoints = new List<Point>();
            listPoints.Add(new Point(3, (Height / 2) - 1));
            listPoints.Add(new Point(9, 3));
            listPoints.Add(new Point(9, Height - 5));
            brushGradient = new LinearGradientBrush(new Rectangle(0, 0, 10, 10), color1, color2, 90.0f);
            g.FillPolygon(brushGradient, listPoints.ToArray());            
            brushGradient.Dispose();
            brushGradient = null;

            // Draw outline around handle
            pen = new Pen(Color.FromArgb(175, 80, 80, 80));
            g.DrawLine(pen, new Point(rectScrollBarLeftHandle.X, 0), new Point(rectScrollBarLeftHandle.X, Height));
            g.DrawLine(pen, new Point(rectScrollBarLeftHandle.X + rectScrollBarLeftHandle.Width, 0), new Point(rectScrollBarLeftHandle.X + rectScrollBarLeftHandle.Width, Height));
            g.DrawLine(pen, new Point(rectScrollBarLeftHandle.X, Height - 1), new Point(rectScrollBarLeftHandle.X + rectScrollBarLeftHandle.Width, Height - 1));
            g.DrawLine(pen, new Point(rectScrollBarLeftHandle.X, 0), new Point(rectScrollBarLeftHandle.X + rectScrollBarLeftHandle.Width, 0));
            pen.Dispose();
            pen = null;

            // ----------------------------------
            // Right Handle

            // Set right handle background colors            
            color1 = Color.FromArgb(150, 150, 150, 150);
            color2 = Color.FromArgb(200, 200, 200, 200);
            if (isMouseOverRightHandle)
            {
                color1 = Color.FromArgb(200, 200, 200, 200);
                color2 = Color.FromArgb(255, 220, 220, 220);
            }

            // Render right handle background
            rectScrollBarRightHandle = new Rectangle(Width - 14, Height - 14, 14, 14);
            brushGradient = new LinearGradientBrush(rectScrollBarRightHandle, color1, color2, 90.0f);
            g.FillRectangle(brushGradient, rectScrollBarRightHandle);
            brushGradient.Dispose();
            brushGradient = null;

            // Set right handle arrow colors            
            color1 = Color.FromArgb(200, 0, 0, 0);
            color2 = Color.FromArgb(225, 50, 50, 50);
            if (isMouseOverRightHandle)
            {
                color1 = Color.FromArgb(240, 50, 50, 50);
                color2 = Color.FromArgb(255, 80, 80, 80);
            }

            // Render right handle arrow
            listPoints.Clear();
            listPoints.Add(new Point(Width - 3, (Height / 2) - 1));
            listPoints.Add(new Point(Width - 9, 3));
            listPoints.Add(new Point(Width - 9, Height - 5));
            brushGradient = new LinearGradientBrush(new Rectangle(0, 0, 10, 10), color1, color2, 90.0f);
            g.FillPolygon(brushGradient, listPoints.ToArray());
            brushGradient.Dispose();
            brushGradient = null;

            // Draw outline around handle
            pen = new Pen(Color.FromArgb(175, 80, 80, 80));
            g.DrawLine(pen, new Point(rectScrollBarRightHandle.X, 0), new Point(rectScrollBarRightHandle.X, Height));
            g.DrawLine(pen, new Point(rectScrollBarRightHandle.X + rectScrollBarRightHandle.Width, 0), new Point(rectScrollBarRightHandle.X + rectScrollBarRightHandle.Width, Height));
            g.DrawLine(pen, new Point(rectScrollBarRightHandle.X, Height - 1), new Point(rectScrollBarRightHandle.X + rectScrollBarRightHandle.Width, Height - 1));
            g.DrawLine(pen, new Point(rectScrollBarRightHandle.X, 0), new Point(rectScrollBarRightHandle.X + rectScrollBarRightHandle.Width, 0));
            pen.Dispose();
            pen = null;

            // Draw outline around handle
            pen = new Pen(Color.FromArgb(200, 50, 50, 50));
            g.DrawLine(pen, new Point(0, 0), new Point(Width, 0));
            g.DrawLine(pen, new Point(0, Height - 1), new Point(Width, Height - 1));
            pen.Dispose();

            // ----------------------------------
            // Thumb

            // Render thumb
            color1 = Color.FromArgb(125, 200, 200, 200);
            color2 = Color.FromArgb(220, 225, 225, 225);
            if (isMouseOverThumb || isUserDraggingThumb)
            {
                color1 = Color.FromArgb(125, 200, 200, 200);
                color2 = Color.FromArgb(255, 225, 225, 225);
            }         
            
            // Calculate values
            float thumbWidth = ((float)LargeChange / (float)Maximum) * (Width - 28);
            float ratio = (float)(Value + Minimum) / (float)Maximum;
            int thumbX = (int)(ratio * (Width - 28 - thumbWidth)) + 14;            

            // Draw thumb
            rectScrollBarThumb = new Rectangle(thumbX, Height - 14, (int)thumbWidth, 14);
            brushGradient = new LinearGradientBrush(rectScrollBarThumb, color1, color2, 90.0f);
            g.FillRectangle(brushGradient, rectScrollBarThumb);
            brushGradient.Dispose();
            brushGradient = null;

            // Draw outline around thumb
            pen = new Pen(Color.FromArgb(200, 75, 75, 75));
            g.DrawLine(pen, new Point(rectScrollBarThumb.X, 0), new Point(rectScrollBarThumb.X, Height));
            g.DrawLine(pen, new Point(rectScrollBarThumb.X + rectScrollBarThumb.Width, 0), new Point(rectScrollBarThumb.X + rectScrollBarThumb.Width, Height));
            g.DrawLine(pen, new Point(rectScrollBarThumb.X, 0), new Point(rectScrollBarThumb.X + rectScrollBarThumb.Width, 0));
            g.DrawLine(pen, new Point(rectScrollBarThumb.X, Height - 1), new Point(rectScrollBarThumb.X + rectScrollBarThumb.Width, Height - 1));
            pen.Dispose();
            pen = null;

            // Draw lines in the center to show the thumb
            int x = thumbX + (int)((float)thumbWidth / 2);
            pen = new Pen(Color.FromArgb(200, 75, 75, 75));
            g.DrawLine(pen, new PointF(x, 4), new PointF(x, Height - 4));
            g.DrawLine(pen, new PointF(x + 4, 4), new PointF(x + 4, Height - 4));
            g.DrawLine(pen, new PointF(x - 4, 4), new PointF(x - 4, Height - 4));
            pen.Dispose();
            pen = null;
        }

        #endregion

        #region Mouse Events

        /// <summary>
        /// Occurs when the mouse pointer is moving over the control.
        /// Manages the display of mouse on/off visual effects.
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Set default flag
            bool needToRefresh = false;

            // Handles work only if the user isn't dragging the thumb
            if (!isUserDraggingThumb)
            {
                // Check if the cursor is on the left handle
                if (e.X >= rectScrollBarLeftHandle.X &&
                    e.Y >= rectScrollBarLeftHandle.Y &&
                    e.X <= rectScrollBarLeftHandle.X + rectScrollBarLeftHandle.Width &&
                    e.Y <= rectScrollBarLeftHandle.Y + rectScrollBarLeftHandle.Height)
                {
                    isMouseOverLeftHandle = true;
                    needToRefresh = true;
                }
                else if (isMouseOverLeftHandle)
                {
                    isMouseOverLeftHandle = false;
                    needToRefresh = true;
                }

                // Check if the cursor is on the left handle
                if (e.X >= rectScrollBarRightHandle.X &&
                    e.Y >= rectScrollBarRightHandle.Y &&
                    e.X <= rectScrollBarRightHandle.X + rectScrollBarRightHandle.Width &&
                    e.Y <= rectScrollBarRightHandle.Y + rectScrollBarRightHandle.Height)
                {
                    isMouseOverRightHandle = true;
                    needToRefresh = true;
                }
                else if (isMouseOverRightHandle)
                {
                    isMouseOverRightHandle = false;
                    needToRefresh = true;
                }
            }

            // Check if the cursor is on the thumb
            if (e.X >= rectScrollBarThumb.X &&
                e.Y >= rectScrollBarThumb.Y &&
                e.X <= rectScrollBarThumb.X + rectScrollBarThumb.Width &&
                e.Y <= rectScrollBarThumb.Y + rectScrollBarThumb.Height)
            {
                // Set flags
                isMouseOverThumb = true;
                needToRefresh = true;

                // Check if the user is holding down the left mouse button, and the dragging thumb flag isn't set
                if (isMouseLeftButtonDown && !isUserDraggingThumb)
                {
                    // Set flag
                    isUserDraggingThumb = true;

                    // Get the x offset
                    //thumbDraggingOffsetX = e.X - rectScrollBarThumb.X;
                }
            }
            else if (isMouseOverThumb)
            {
                isMouseOverThumb = false;
                needToRefresh = true;
            }

            // Is the user dragging the thumb?
            if (isMouseLeftButtonDown && isUserDraggingThumb)
            {
                // Find the value per pixel
                int valueRange = Maximum - Minimum;
                int availableWidth = Width - 28 - rectScrollBarThumb.Width;
                float valuePerPixel = (float)valueRange / (float)availableWidth;

                // Find the delta
                int delta = e.Location.X - mouseDownX;

                // Set new value
                int value = originalValue + (int)(((float)delta * valuePerPixel));

                // Set min/max
                if (value < Minimum)
                {
                    value = Minimum;
                }
                else if (value > Maximum)
                {
                    value = Maximum;
                }

                // Set value
                if (value != Value)
                {
                    Value = value;
                }
            }

            // Check if the control needs to be refreshed
            if (needToRefresh)
            {
                Refresh();
            }

            base.OnMouseMove(e);
        }

        /// <summary>
        /// Occurs when the mouse cursor leaves the control.
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            bool needToRefresh = false;
            if (isMouseOverThumb)
            {
                isMouseOverThumb = false;
                needToRefresh = true;
            }
            if (isMouseOverLeftHandle)
            {
                isMouseOverLeftHandle = false;
                needToRefresh = true;
            }
            if (isMouseOverRightHandle)
            {
                isMouseOverRightHandle = false;
                needToRefresh = true;
            }

            // Refresh if needed
            if (needToRefresh)
            {
                Refresh();
            }
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Occurs when the user is pressing down a mouse button.
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            // Declare variables
            int value = 0;

            // Set flags
            isMouseLeftButtonDown = true;            
            mouseDownX = e.Location.X;
            originalValue = Value;

            // Check if the user has clicked on one of the handles (i.e. small change)
            if (e.Location.X <= 14)
            {
                // Left handle; negative small change
                value = Value - SmallChange;

                // Make sure the value doesn't go lower than the minimum
                if (value < Minimum)
                {
                    // Set value as minimum
                    value = Minimum;
                }

                // Set new value (will refresh the scrollbar automatically)
                Value = value;
            }
            else if (e.Location.X >= Width - 14)
            {
                // Right handle; positive small change
                value = Value + SmallChange;

                // Make sure the value doesn't go higher than the maximum
                if (value > Maximum)
                {
                    // Set value as maximum
                    value = Maximum;
                }

                // Set new value (will refresh the scrollbar automatically)
                Value = value;
            }

            // Check if the user has clicked on the scrollbar background (i.e. large change)
            if (e.Location.X > 14 &&
                e.Location.X < rectScrollBarThumb.Left)
            {
                // Left portion; negative large change
                value = Value - LargeChange;

                // Make sure the value doesn't go lower than the minimum
                if (value < Minimum)
                {
                    // Set value as minimum
                    value = Minimum;
                }

                // Set new value (will refresh the scrollbar automatically)
                Value = value;
            }
            else if (e.Location.X < Width - 14 &&
                     e.Location.X > rectScrollBarThumb.Right)
            {
                // Right portion; positive large change
                value = Value + LargeChange;

                // Make sure the value doesn't go higher than the maximum
                if (value > Maximum)
                {
                    // Set value as maximum
                    value = Maximum;
                }

                // Set new value (will refresh the scrollbar automatically)
                Value = value;
            }

            // Check if the user has clicked on one of the handles or the scrollbar background
            if (e.Location.X <= 14 ||  // left handle
                e.Location.X >= Width - 14 ||  // right handle
                (e.Location.X > 14 &&  // left background
                e.Location.X < rectScrollBarThumb.Left) ||
                (e.Location.X < Width - 14 && // right background
                e.Location.X > rectScrollBarThumb.Right))
            {
                // Start timer
                timerMouseDown.Enabled = true;
            }

            // Call base method
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Occurs when the user releases a mouse button.
        /// Manages the clicking on the different areas of the control.
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            // Reset flags
            isMouseLeftButtonDown = false;
            isUserDraggingThumb = false;

            // Stop timer
            timerMouseDown.Enabled = false;

            // Call base method
            base.OnMouseUp(e);
        }

        #endregion
    }
}
