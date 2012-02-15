//
// Label.cs: This label control is based on the System.Windows.Forms.Label control.
//           It adds support for embedded Fonts and anti-aliasing.
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
using System.ComponentModel.Design;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This label control is based on the System.Windows.Forms.Label control.
    /// It adds support for embedded Fonts and anti-aliasing.
    /// </summary>
    public class Label : System.Windows.Forms.Label
    {
        private EmbeddedFontCollection m_embeddedFonts = null;

        #region Background Properties

        /// <summary>
        /// Private value for the UseBackgroundGradient property.
        /// </summary>
        private bool m_useBackgroundGradient = false;
        /// <summary>
        /// Defines if the background gradient should be used or not.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Background"), Browsable(true), Description("Defines if the background gradient should be used or not.")]
        public bool UseBackgroundGradient
        {
            get
            {
                return m_useBackgroundGradient;
            }
            set
            {
                m_useBackgroundGradient = value;
            }
        }

        /// <summary>
        /// Private value for the BackgroundGradientColor1 property.
        /// </summary>
        private Color m_backgroundGradientColor1 = Color.FromArgb(0, 0, 0);
        /// <summary>
        /// First color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Background"), Browsable(true), Description("First color of the background gradient.")]
        public Color BackgroundGradientColor1
        {
            get
            {
                return m_backgroundGradientColor1;
            }
            set
            {
                m_backgroundGradientColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the BackgroundGradientColor2 property.
        /// </summary>
        private Color m_backgroundGradientColor2 = Color.FromArgb(50, 50, 50);
        /// <summary>
        /// Second color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Background"), Browsable(true), Description("Second color of the background gradient.")]
        public Color BackgroundGradientColor2
        {
            get
            {
                return m_backgroundGradientColor2;
            }
            set
            {
                m_backgroundGradientColor2 = value;
            }
        }

        /// <summary>
        /// Private value for the BackgroundGradientMode property.
        /// </summary>
        private LinearGradientMode m_backgroundGradientMode = LinearGradientMode.Vertical;
        /// <summary>
        /// Background gradient mode.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Background"), Browsable(true), Description("Background gradient mode.")]
        public LinearGradientMode BackgroundGradientMode
        {
            get
            {
                return m_backgroundGradientMode;
            }
            set
            {
                m_backgroundGradientMode = value;
            }
        }

        #endregion

        #region Font Properties

        /// <summary>
        /// Private value for the CustomFont property.
        /// </summary>
        private CustomFont m_customFont = null;
        /// <summary>
        /// Defines the Font to be used for rendering the control.
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

        /// <summary>
        /// Default constructor for the Label class.
        /// </summary>
        public Label()
        {
            // Set control styles
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                ControlStyles.Opaque | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);        
            
            // Set default Font
            m_customFont = new CustomFont();

            // Get embedded Font collection
            m_embeddedFonts = EmbeddedFontHelper.GetEmbeddedFonts();

            //Font fontsss = EmbeddedFonts.LeagueGothic;
        }

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

                //m_cachedFont = Tools.LoadEmbeddedFont(EmbeddedFonts.embeddedFonts, "LeagueGothic", 10f, FontStyle.Bold);

                // Use anti-aliasing?
                if (CustomFont.UseAntiAliasing)
                {
                    // Set text anti-aliasing to ClearType (best looking AA)
                    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                    // Set smoothing mode for paths
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                }

                // Create custom Font
                Font Font = null;

                // Make sure the embedded Font name needs to be loaded and is valid
                if (CustomFont.UseEmbeddedFont && !String.IsNullOrEmpty(CustomFont.EmbeddedFontName))
                {
                    try
                    {
                        // Get embedded Font
                        Font = Tools.LoadEmbeddedFont(m_embeddedFonts, CustomFont.EmbeddedFontName, CustomFont.Size, CustomFont.ToFontStyle());
                    }
                    catch
                    {
                        // Use default Font instead
                        Font = this.Font;
                    }
                }

                // Check if Font is null
                if (Font == null)
                {
                    try
                    {
                        // Try to get standard Font
                        Font = new Font(CustomFont.StandardFontName, CustomFont.Size, CustomFont.ToFontStyle());
                    }
                    catch
                    {
                        // Use default Font instead
                        Font = this.Font;
                    }
                }

                // Check if the gradient background should be used
                if (m_useBackgroundGradient)
                {                    
                    // Draw background gradient (cover -1 pixel for some refresh bug)
                    Rectangle rectBody = new Rectangle(-1, -1, Width + 1, Height + 1);
                    LinearGradientBrush brushBackground = new LinearGradientBrush(rectBody, m_backgroundGradientColor1, m_backgroundGradientColor2, m_backgroundGradientMode);
                    g.FillRectangle(brushBackground, rectBody);
                    brushBackground.Dispose();
                    brushBackground = null;
                }
                else
                {
                    // Call paint background
                    base.OnPaintBackground(pe); // CPU intensive
                }

                // Create brush
                SolidBrush brushFont = new SolidBrush(ForeColor);

                //if (TextAlign == ContentAlignment.TopLeft)
                //{
                //    // Top left
                //    g.DrawString(Text, m_cachedFont, brushFont, 2, 2);
                //}
                //else
                //{
                //    // Measure string            
                //    SizeF sizeString = g.MeasureString(this.Text, Font);

                //    // Draw string depending on alignment
                //    if (TextAlign == ContentAlignment.BottomLeft)
                //    {
                //        // Bottom left
                //        g.DrawString(Text, m_cachedFont, brushFont, 2, (this.Height - sizeString.Height) - 2);
                //    }
                //    else if (this.TextAlign == ContentAlignment.BottomCenter)
                //    {
                //        // Bottom center
                //        g.DrawString(Text, m_cachedFont, brushFont, (this.Width - sizeString.Width) / 2, (this.Height - sizeString.Height) - 2);
                //    }
                //    else if (this.TextAlign == ContentAlignment.BottomRight)
                //    {
                //        // Bottom right
                //        g.DrawString(Text, m_cachedFont, brushFont, (this.Width - sizeString.Width) - 2, (this.Height - sizeString.Height) - 2);
                //    }
                //    else if (this.TextAlign == ContentAlignment.MiddleLeft)
                //    {
                //        // Middle left
                //        g.DrawString(Text, m_cachedFont, brushFont, 2, (this.Height - sizeString.Height) / 2);
                //    }
                //    else if (this.TextAlign == ContentAlignment.MiddleCenter)
                //    {
                //        // Middle center
                //        g.DrawString(Text, m_cachedFont, brushFont, (this.Width - sizeString.Width) / 2, (this.Height - sizeString.Height) / 2);
                //    }
                //    else if (this.TextAlign == ContentAlignment.MiddleRight)
                //    {
                //        // Middle right
                //        g.DrawString(Text, m_cachedFont, brushFont, (this.Width - sizeString.Width) - 2, (this.Height - sizeString.Height) / 2);
                //    }
                //    else if (this.TextAlign == ContentAlignment.TopLeft)
                //    {

                //    }
                //    else if (this.TextAlign == ContentAlignment.TopCenter)
                //    {
                //        // Top center
                //        g.DrawString(Text, m_cachedFont, brushFont, (this.Width - sizeString.Width) / 2, 2);
                //    }
                //    else if (this.TextAlign == ContentAlignment.TopRight)
                //    {
                //        // Top right
                //        g.DrawString(Text, m_cachedFont, brushFont, (this.Width - sizeString.Width) - 2, 2);
                //    }
                //}

                if (TextAlign == ContentAlignment.TopLeft)
                {
                    // Top left
                    g.DrawString(Text, Font, brushFont, 2, 2);
                }
                else
                {
                    // Measure string            
                    SizeF sizeString = g.MeasureString(this.Text, Font);

                    // Draw string depending on alignment
                    if (TextAlign == ContentAlignment.BottomLeft)
                    {
                        // Bottom left
                        g.DrawString(Text, Font, brushFont, 2, (this.Height - sizeString.Height) - 2);
                    }
                    else if (this.TextAlign == ContentAlignment.BottomCenter)
                    {
                        // Bottom center
                        g.DrawString(Text, Font, brushFont, (this.Width - sizeString.Width) / 2, (this.Height - sizeString.Height) - 2);
                    }
                    else if (this.TextAlign == ContentAlignment.BottomRight)
                    {
                        // Bottom right
                        g.DrawString(Text, Font, brushFont, (this.Width - sizeString.Width) - 2, (this.Height - sizeString.Height) - 2);
                    }
                    else if (this.TextAlign == ContentAlignment.MiddleLeft)
                    {
                        // Middle left
                        g.DrawString(Text, Font, brushFont, 2, (this.Height - sizeString.Height) / 2);
                    }
                    else if (this.TextAlign == ContentAlignment.MiddleCenter)
                    {
                        // Middle center
                        g.DrawString(Text, Font, brushFont, (this.Width - sizeString.Width) / 2, (this.Height - sizeString.Height) / 2);
                    }
                    else if (this.TextAlign == ContentAlignment.MiddleRight)
                    {
                        // Middle right
                        g.DrawString(Text, Font, brushFont, (this.Width - sizeString.Width) - 2, (this.Height - sizeString.Height) / 2);
                    }
                    else if (this.TextAlign == ContentAlignment.TopLeft)
                    {

                    }
                    else if (this.TextAlign == ContentAlignment.TopCenter)
                    {
                        // Top center
                        g.DrawString(Text, Font, brushFont, (this.Width - sizeString.Width) / 2, 2);
                    }
                    else if (this.TextAlign == ContentAlignment.TopRight)
                    {
                        // Top right
                        g.DrawString(Text, Font, brushFont, (this.Width - sizeString.Width) - 2, 2);
                    }
                }

                // Dispose stuff
                brushFont.Dispose();
                brushFont = null;

                // Dispose Font if necessary
                if (Font != null && Font != this.Font)
                {
                    // Dispose Font
                    Font.Dispose();
                    Font = null;
                }
            }
            catch
            {                
                throw;
            }

            //ITypeResolutionService typeResService = GetService(typeof(ITypeResolutionService)) as ITypeResolutionService;
            //string path = typeResService.GetPathOfAssembly(Assembly.GetExecutingAssembly().GetName());
        }

        private static Font m_cachedFont = null;
    }

    //public static class EmbeddedFonts
    //{                    
    //    public static readonly EmbeddedFontCollection embeddedFonts = EmbeddedFontHelper.GetEmbeddedFonts();

    //    //public static readonly Font LeagueGothic = new Font(FontFamily.GenericSansSerif, 10f);
    //    //public static readonly Font LeagueGothic = Tools.LoadEmbeddedFont(embeddedFonts, "LeagueGothic", 10f, FontStyle.Bold);
    //}
}
