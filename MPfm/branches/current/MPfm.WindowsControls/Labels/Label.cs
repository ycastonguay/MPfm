//
// Label.cs: This label control is based on the System.Windows.Forms.Label control.
//           It adds custom drawing, supports embedded fonts and other features.
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
    /// This label control is based on the System.Windows.Forms.Label control.
    /// It adds custom drawing, supports embedded fonts and other features.
    /// </summary>
    public class Label : System.Windows.Forms.Label
    {
        #region Font Properties

        /// <summary>
        /// Name of the embedded font (as written in the Name property of a CustomFont).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Name of the embedded font (as written in the Name property of a CustomFont).")]
        public string CustomFontName { get; set; }

        /// <summary>
        /// Private value for the AntiAliasingEnabled property.
        /// </summary>
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
        /// Default constructor for Label.
        /// </summary>
        public Label()
        {
            // Set control styles
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                ControlStyles.Opaque | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);        
            
            // Set default font
            m_customFont = new CustomFont();
        }

        #region Paint Events
        
        /// <summary>
        /// Occurs when the control needs to be painted.
        /// </summary>
        /// <param name="pe">Paint Event Arguments</param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            try
            {
                // Get graphics from paint event
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

                // Check for auto size
                if (AutoSize)
                {
                    // TODO: Reisze control
                }

                // Create brush
                SolidBrush brushFont = new SolidBrush(ForeColor);                

                // Draw string depending on alignment
                if (TextAlign == ContentAlignment.BottomLeft)
                {
                    // Bottom left
                    g.DrawString(Text, font, brushFont, 2, (this.Height - sizeString.Height) - 2);
                }
                else if (this.TextAlign == ContentAlignment.BottomCenter)
                {
                    // Bottom center
                    g.DrawString(Text, font, brushFont, (this.Width - sizeString.Width) / 2, (this.Height - sizeString.Height) - 2);
                }
                else if (this.TextAlign == ContentAlignment.BottomRight)
                {
                    // Bottom right
                    g.DrawString(Text, font, brushFont, (this.Width - sizeString.Width) - 2, (this.Height - sizeString.Height) - 2);
                }
                else if (this.TextAlign == ContentAlignment.MiddleLeft)
                {
                    // Middle left
                    g.DrawString(Text, font, brushFont, 2, (this.Height - sizeString.Height) / 2);
                }
                else if (this.TextAlign == ContentAlignment.MiddleCenter)
                {
                    // Middle center
                    g.DrawString(Text, font, brushFont, (this.Width - sizeString.Width) / 2, (this.Height - sizeString.Height) / 2);
                }
                else if (this.TextAlign == ContentAlignment.MiddleRight)
                {
                    // Middle right
                    g.DrawString(Text, font, brushFont, (this.Width - sizeString.Width) - 2, (this.Height - sizeString.Height) / 2);
                }
                else if (this.TextAlign == ContentAlignment.TopLeft)
                {
                    // Top left
                    g.DrawString(Text, font, brushFont, 2, 2);
                }
                else if (this.TextAlign == ContentAlignment.TopCenter)
                {
                    // Top center
                    g.DrawString(Text, font, brushFont, (this.Width - sizeString.Width) / 2, 2);
                }
                else if (this.TextAlign == ContentAlignment.TopRight)
                {
                    // Top right
                    g.DrawString(Text, font, brushFont, (this.Width - sizeString.Width) - 2, 2);
                }

                // Dispose stuff
                brushFont.Dispose();
                brushFont = null;

                // Dispose font if necessary
                if (font != null && font != this.Font)
                {
                    // Dispose font
                    font.Dispose();
                    font = null;
                }
            }
            catch (Exception ex)
            {
                //pe.Graphics.DrawString(ex.Message + "\n" + ex.StackTrace, Font, Brushes.White, new Point(1, 1));
                throw ex;
            }
        }

        #endregion
    }
}
