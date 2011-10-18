//
// LinkLabel.cs: This link label control is based on the System.Windows.Forms.LinkLabel control.
//               It adds custom drawing, supports embedded fonts and other features.
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
    /// It adds custom drawing, supports embedded fonts and other features.
    /// </summary>
    public class LinkLabel : System.Windows.Forms.LinkLabel
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

        #endregion

        /// <summary>
        /// Default constructor for LinkLabel.
        /// </summary>
        public LinkLabel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                ControlStyles.Opaque | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);      
        }

        #region Paint Events
        
        /// <summary>
        /// Occurs when the control needs to be painted.
        /// </summary>
        /// <param name="pe">Paint Event Arguments</param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            // Get graphics from event
            Graphics g = pe.Graphics;

            // Use anti-aliasing?
            if (AntiAliasingEnabled)
            {
                // Set text anti-aliasing to ClearType (best looking AA)
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                // Set smoothing mode for paths
                g.SmoothingMode = SmoothingMode.AntiAlias;
            }

            // Create custom font
            Font font = this.Font;
            if (FontCollection != null && CustomFontName.Length > 0)
            {
                FontFamily family = FontCollection.GetFontFamily(CustomFontName);

                if (family != null)
                {                    
                    font = new Font(family, Font.Size, Font.Style);
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

        #endregion
    }
}
