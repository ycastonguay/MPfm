//
// Form.cs: This form control is based on the System.Windows.Forms.Form control.
//          It adds custom drawing and other features.
//
// Copyright © 2011 Yanick Castonguay
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
    /// This form control is based on the System.Windows.Forms.Form control.
    /// It adds custom drawing and other features.
    /// </summary>
    public class Form : System.Windows.Forms.Form
    {
        #region Font Properties

        /// <summary>
        /// Name of the embedded font (as written in the Name property of a CustomFont).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Name of the embedded font (as written in the Name property of a CustomFont).")]
        public string CustomFontName { get; set; }

        /// <summary>
        /// Pointer to the embedded font collection.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Pointer to the embedded font collection.")]
        public FontCollection FontCollection { get; set; }

        private bool m_antiAliasingEnabled = true;
        /// <summary>
        /// Use anti-aliasing when drawing the embedded font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Configuration"), Browsable(true), Description("Use anti-aliasing when drawing the embedded font.")]
        public bool AntiAliasingEnabled
        {
            get
            {
                return m_antiAliasingEnabled;
            }
            set
            {
                m_antiAliasingEnabled = value;
            }
        }

        private bool m_customDrawEnabled = false;
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("")]
        [Category("Configuration"), Browsable(true), Description("Custom draw")]
        public bool CustomDrawEnabled
        {
            get
            {
                return m_customDrawEnabled;
            }
            set
            {
                m_customDrawEnabled = value;
            }
        }

        #endregion

        #region Border Properties

        private Color m_borderColor = Color.Black;
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Border"), Browsable(true), Description("The color of the border")]
        public Color BorderColor
        {
            get
            {
                return m_borderColor;
            }
            set
            {
                m_borderColor = value;
            }
        }

        private int m_borderWidth = 1;
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Border"), Browsable(true), Description("The width of the border")]
        public int BorderWidth
        {
            get
            {
                return m_borderWidth;
            }
            set
            {
                m_borderWidth = value;
            }
        }

        #endregion

        #region Background Properties

        private Color m_gradientColor1 = Color.LightGray;
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

        private bool m_dragging = false;
        /// <summary>
        /// Indicates if the user is dragging something inside the form.
        /// </summary>
        public bool Dragging
        {
            get
            {
                return m_dragging;
            }
        }

        /// <summary>
        /// Default constructor for Form.
        /// </summary>
        public Form()
        {
            //SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);         
        }

        #region Paint Events
        
        /// <summary>
        /// Occurs when the control background needs to be painted.
        /// </summary>
        /// <param name="e">Paint Event Arguments</param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }

        /// <summary>
        /// Occurs when the control needs to be painted.
        /// </summary>
        /// <param name="pe">Paint Event Arguments</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        #endregion

        #region Mouse Events

        /// <summary>
        /// Occurs when the mouse cursor enters the control.
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
        }

        /// <summary>
        /// Occurs when the mouse cursor leaves the control.
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Occurs when the mouse cursor moves over the control.
        /// </summary>
        /// <param name="e">Mouse Event Arguments</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!CustomDrawEnabled)
            {
                base.OnMouseMove(e);
                return;
            }
        }

        /// <summary>
        /// Occurs when the mouse button is released.
        /// </summary>
        /// <param name="e">Mouse Event Arguments</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!CustomDrawEnabled)
            {
                base.OnMouseUp(e);
                return;
            }

            m_dragging = false;
        }

        /// <summary>
        /// Occurs when the mouse button is down.
        /// </summary>
        /// <param name="e">Mouse Event Arguments</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!CustomDrawEnabled)
            {
                base.OnMouseDown(e);
                return;
            }

            m_dragging = true;
        }

        #endregion
    }
}
