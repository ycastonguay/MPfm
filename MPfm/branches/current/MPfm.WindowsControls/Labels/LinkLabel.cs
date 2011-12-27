//
// LinkLabel.cs: This link label control is based on the System.Windows.Forms.LinkLabel control.
//               It adds support for embedded fonts and anti-aliasing.
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
    /// This link label control is based on the System.Windows.Forms.LinkLabel control.
    /// It adds support for embedded fonts and anti-aliasing.
    /// </summary>
    public class LinkLabel : System.Windows.Forms.LinkLabel
    {
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

        /// <summary>
        /// Default constructor for the LinkLabel class.
        /// </summary>
        public LinkLabel()
        {
            // Set styles
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                ControlStyles.Opaque | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);

            // Set default font
            m_customFont = new CustomFont();
        }

        /// <summary>
        /// Occurs when the control needs to be painted.
        /// </summary>
        /// <param name="pe">Paint Event Arguments</param>
        protected override void OnPaint(PaintEventArgs pe)
        {
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

            // Create custom font
            Font font = null;

            // Make sure the embedded font name needs to be loaded and is valid
            if (CustomFont.UseEmbeddedFont && !String.IsNullOrEmpty(CustomFont.EmbeddedFontName))
            {
                try
                {
                    // Get embedded font collection
                    EmbeddedFontCollection embeddedFonts = EmbeddedFontHelper.GetEmbeddedFonts();

                    // Get embedded font
                    font = Tools.LoadEmbeddedFont(embeddedFonts, CustomFont.EmbeddedFontName, CustomFont.Size, CustomFont.ToFontStyle());
                }
                catch (Exception ex)
                {
                    // Use default font instead
                    font = this.Font;
                }
            }

            // Check if font is null
            if (font == null)
            {
                try
                {
                    // Try to get standard font
                    font = new Font(CustomFont.StandardFontName, CustomFont.Size, CustomFont.ToFontStyle());
                }
                catch (Exception ex)
                {
                    // Use default font instead
                    font = this.Font;
                }
            }

            // Call paint background
            base.OnPaintBackground(pe);

            // Measure string            
            SizeF sizeString = g.MeasureString(this.Text, font);

            // Create brush
            SolidBrush brushFont = new SolidBrush(ForeColor);

            // Draw string depending on alignment
            if (TextAlign == ContentAlignment.BottomLeft)
            {
                g.DrawString(Text, font, brushFont, 2, (this.Height - sizeString.Height) - 2);
            }
            else if (this.TextAlign == ContentAlignment.BottomCenter)
            {
                g.DrawString(Text, font, brushFont, (this.Width - sizeString.Width) / 2, (this.Height - sizeString.Height) - 2);
            }
            else if (this.TextAlign == ContentAlignment.BottomRight)
            {
                g.DrawString(Text, font, brushFont, (this.Width - sizeString.Width) - 2, (this.Height - sizeString.Height) - 2);
            }
            else if (this.TextAlign == ContentAlignment.MiddleLeft)
            {
                g.DrawString(Text, font, brushFont, 2, (this.Height - sizeString.Height) / 2);
            }
            else if (this.TextAlign == ContentAlignment.MiddleCenter)
            {
                g.DrawString(Text, font, brushFont, (this.Width - sizeString.Width) / 2, (this.Height - sizeString.Height) / 2);
            }
            else if (this.TextAlign == ContentAlignment.MiddleRight)
            {
                g.DrawString(Text, font, brushFont, (this.Width - sizeString.Width) - 2, (this.Height - sizeString.Height) / 2);
            }
            else if (this.TextAlign == ContentAlignment.TopLeft)
            {
                g.DrawString(Text, font, brushFont, 2, 2);
            }
            else if (this.TextAlign == ContentAlignment.TopCenter)
            {
                g.DrawString(Text, font, brushFont, (this.Width - sizeString.Width) / 2, 2);
            }
            else if (this.TextAlign == ContentAlignment.TopRight)
            {
                g.DrawString(Text, font, brushFont, (this.Width - sizeString.Width) - 2, 2);
            }

            // Dispose stuff
            brushFont.Dispose();
            brushFont = null;

            // Dispose font if necessary
            if (font != null && font != this.Font)
            {
                font.Dispose();
                font = null;
            }                
        }
    }
}
