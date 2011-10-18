//
// Panel.cs: This panel control is based on the System.Windows.Forms.Panel control.
//           It adds custom drawing, gradient backgrounds and other features.
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
    /// This panel control is based on the System.Windows.Forms.Panel control.
    /// It adds custom drawing, gradient backgrounds and other features.
    /// </summary>
    public class Panel : System.Windows.Forms.Panel
    {
        #region Background Properties

        private Color m_gradientColor1 = Color.LightGray;
        /// <summary>
        /// First color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Configuration"), Browsable(true), Description("First color of the background gradient.")]
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
        [Category("Configuration"), Browsable(true), Description("Second color of the background gradient.")]
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

        #region Font Properties

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

        #endregion

        #region Header Properties

        private string m_headerCustomFontName = "";
        /// <summary>
        /// Name of the embedded font used in the header (as written in the Name property of a CustomFont).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("")]
        [Category("Header"), Browsable(true), Description("Name of the embedded font used in the header (as written in the Name property of a CustomFont).")]
        public string HeaderCustomFontName
        {
            get
            {
                return m_headerCustomFontName;
            }
            set
            {
                m_headerCustomFontName = value;
            }
        }

        private string m_headerTitle = "";
        /// <summary>
        /// Title displayed in the header.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("")]
        [Category("Header"), Browsable(true), Description("Title displayed in the header.")]
        public string HeaderTitle
        {
            get
            {
                return m_headerTitle;
            }
            set
            {
                m_headerTitle = value;
            }
        }

        private int m_headerHeight = 0;
        /// <summary>
        /// Height of the header.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("")]
        [Category("Header"), Browsable(true), Description("Height of the header.")]
        public int HeaderHeight
        {
            get
            {
                return m_headerHeight;
            }
            set
            {
                m_headerHeight = value;

            }
        }

        private ContentAlignment m_headerTextAlign = ContentAlignment.MiddleLeft;
        /// <summary>
        /// Alignment of the text in the header.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("")]
        [Category("Header"), Browsable(true), Description("Alignment of the text in the header.")]
        public ContentAlignment HeaderTextAlign
        {
            get
            {
                return m_headerTextAlign;
            }
            set
            {
                m_headerTextAlign = value;
            }
        }

        private Color m_headerForeColor = Color.Black;
        /// <summary>
        /// The fore color of the font used in the header.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("")]
        [Category("Header"), Browsable(true), Description("The fore color of the font used in the header.")]
        public Color HeaderForeColor
        {
            get
            {
                return m_headerForeColor;
            }
            set
            {
                m_headerForeColor = value;
            }
        }

        private Color m_headerGradientColor1 = Color.LightGray;
        /// <summary>
        /// First color of the background gradient in the header.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("")]
        [Category("Header"), Browsable(true), Description("First color of the background gradient in the header.")]
        public Color HeaderGradientColor1
        {
            get
            {
                return m_headerGradientColor1;
            }
            set
            {
                m_headerGradientColor1 = value;
            }
        }

        private Color m_headerGradientColor2 = Color.Gray;
        /// <summary>
        /// Second color of the background gradient in the header.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("")]
        [Category("Header"), Browsable(true), Description("Second color of the background gradient in the header.")]
        public Color HeaderGradientColor2
        {
            get
            {
                return m_headerGradientColor2;
            }
            set
            {
                m_headerGradientColor2 = value;
            }
        }

        private bool m_headerExpandable = true;
        /// <summary>
        /// Defines if the header is expandable or not.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(true)]
        [Category("Header"), Browsable(true), Description("Defines if the header is expandable or not.")]
        public bool HeaderExpandable
        {
            get
            {
                return m_headerExpandable;
            }
            set
            {
                m_headerExpandable = value;
            }
        }

        private bool m_headerExpanded = false;
        /// <summary>
        /// Defines if the header is expanded.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(false)]
        [Category("Header"), Browsable(true), Description("Defines if the header is expanded.")]
        public bool HeaderExpanded
        {
            get
            {
                return m_headerExpanded;
            }
            set
            {
                m_headerExpanded = value;
            }
        }

        private int m_expandedHeight = 200;
        /// <summary>
        /// Defines the header height when expanded.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(false)]
        [Category("Header"), Browsable(true), Description("Defines the header height when expanded.")]
        public int ExpandedHeight
        {
            get
            {
                return m_expandedHeight;
            }
            set
            {
                m_expandedHeight = value;
            }
        }

        #endregion

        /// <summary>
        /// Default constructor for Panel.
        /// </summary>
        public Panel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                ControlStyles.Opaque | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);    
        }

        #region Expand Methods

        /// <summary>
        /// Expands or hides the content panel.
        /// </summary>
        public void Expand()
        {
            if (m_headerExpandable)
            {
                m_headerExpanded = !m_headerExpanded;

                if (m_headerExpanded)
                {
                    Height = m_expandedHeight;
                }
                else
                {
                    Height = m_headerHeight;
                }
            }
        }

        /// <summary>
        /// Expands or hides the content panel, based on the value passed in parameter.
        /// </summary>
        /// <param name="value">Expand or hide the panel</param>
        public void Expand(bool value)
        {
            if (m_headerExpandable)
            {
                m_headerExpanded = value;

                if (m_headerExpanded)
                {
                    Height = m_expandedHeight;
                }
                else
                {
                    Height = m_headerHeight;
                }
            }
        }

        #endregion

        #region Paint Events
        
        /// <summary>
        /// Occurs when the control needs to be painted.
        /// </summary>
        /// <param name="pe">Paint Event Arguments</param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            // Get graphics from event
            Graphics g = pe.Graphics;

            // Set text anti-aliasing to ClearType (best looking AA)
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Set smoothing mode for paths
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Create custom font
            Font font = this.Font;
            if (FontCollection != null && m_headerCustomFontName.Length > 0)
            {
                FontFamily family = FontCollection.GetFontFamily(m_headerCustomFontName);

                if (family != null)
                {
                    font = new Font(family, Font.Size, Font.Style);
                }
            }
            // Draw body
            if (m_headerExpanded)
            {
                // Draw gradient
                Rectangle rectBody = new Rectangle(-1, -1, Width + 1, Height + 1);
                LinearGradientBrush brushBody = new LinearGradientBrush(rectBody, m_gradientColor1, m_gradientColor2, LinearGradientMode.Vertical);
                g.FillRectangle(brushBody, rectBody);
                brushBody.Dispose();
                brushBody = null;
            }

            // Draw header
            LinearGradientBrush brushHeader = new LinearGradientBrush(new Rectangle(0, 0, ClientRectangle.Width, m_headerHeight + 4), m_headerGradientColor1, m_headerGradientColor2, LinearGradientMode.Vertical);
            g.FillRectangle(brushHeader, 0, 0, ClientRectangle.Width, m_headerHeight);
            brushHeader.Dispose();
            brushHeader = null;

            SolidBrush brushFont = new SolidBrush(m_headerForeColor);
            SizeF sizeString = g.MeasureString(m_headerTitle, font);

            float headerTitleY = ((float)m_headerHeight - sizeString.Height) / 2;

            if (HeaderTextAlign == ContentAlignment.MiddleCenter)
            {
                g.DrawString(m_headerTitle, font, brushFont, (Width - sizeString.Width) / 2, headerTitleY);
            }
            else if (HeaderTextAlign == ContentAlignment.MiddleRight)
            {
                g.DrawString(m_headerTitle, font, brushFont, Width - sizeString.Width, headerTitleY);
            }
            else
            {
                g.DrawString(m_headerTitle, font, brushFont, 2, headerTitleY);
            }

            brushFont.Dispose();
            brushFont = null;

            // Dispose graphics
            //g.Dispose();
            //g = null;
        }

        #endregion

        #region Mouse Events
        
        /// <summary>
        /// Occurs when the user double clicks on the control.
        /// Expands the header if the cursor is in the header area.
        /// </summary>
        /// <param name="e">Mouse Event Arguments</param>
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (m_headerExpandable)
            {
                if (e.Y >= 0 && e.Y <= m_headerHeight)
                {
                    Expand();
                }
            }
        }

        #endregion

    }
}
