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
using System.Drawing.Design;
using System.IO;
using System.Reflection;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This label control is based on the System.Windows.Forms.Label control.
    /// It adds support for embedded Fonts and anti-aliasing.
    /// </summary>
    public class Label : System.Windows.Forms.Label
    {
        /// <summary>
        /// Embedded font collection used for drawing.
        /// </summary>
        private EmbeddedFontCollection embeddedFonts = null;

        /// <summary>
        /// Private value for the Theme property.
        /// </summary>
        private LabelTheme theme = null;
        /// <summary>
        /// Defines the current theme used for rendering the control.
        /// </summary>
        public LabelTheme Theme
        {
            get
            {
                return theme;
            }
            set
            {
                theme = value;
            }
        }

        /// <summary>
        /// Default constructor for the Label class.
        /// </summary>
        public Label()
        {
            // Set control styles
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                ControlStyles.Opaque | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);        
            
            // Create default theme
            theme = new LabelTheme();
        }

        /// <summary>
        /// Occurs when the control is created.
        /// </summary>
        protected override void OnCreateControl()
        {
            // Call base event method
            base.OnCreateControl();

            // Load embedded fonts
            LoadEmbeddedFonts();
        }

        /// <summary>
        /// Loads the embedded fonts for rendering.
        /// </summary>
        protected void LoadEmbeddedFonts()
        {
            // Check if design time or run time            
            if (Tools.IsDesignTime())
            {
                // This only exists when running in design time and cannot be run in the constructor                
                ITypeResolutionService typeResService = GetService(typeof(ITypeResolutionService)) as ITypeResolutionService;
                string path = string.Empty;
                if (typeResService != null)
                {
                    // Get path
                    path = typeResService.GetPathOfAssembly(Assembly.GetExecutingAssembly().GetName());
                }

                // Example path: D:\Code\MPfm\Branches\Current\MPfm.WindowsControls\obj\Debug\MPfm.WindowsControls.dll               
                string fontsPath = path.Replace("MPfm.WindowsControls", "MPfm.Fonts").Replace("MPfm.Fonts.dll", "");

                // Get embedded font collection
                embeddedFonts = EmbeddedFontHelper.GetEmbeddedFonts(fontsPath);
            }
            else
            {
                // Get embedded font collection
                embeddedFonts = EmbeddedFontHelper.GetEmbeddedFonts();
            }
        }

        /// <summary>
        /// Occurs when the control needs to be painted.
        /// </summary>
        /// <param name="pe">Paint Event arguments</param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            try
            {                
                // Get graphics from paint event
                Graphics g = pe.Graphics;

                // Use anti-aliasing?
                if (theme.TextGradient.Font.UseAntiAliasing)
                {
                    // Set text anti-aliasing to ClearType (best looking AA)
                    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                    // Set smoothing mode for paths
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                }

                // Create custom Font
                Font Font = null;                

                // Make sure the embedded Font name needs to be loaded and is valid
                if (theme.TextGradient.Font.UseEmbeddedFont && !String.IsNullOrEmpty(theme.TextGradient.Font.EmbeddedFontName))
                {
                    try
                    {
                        // Get embedded Font
                        Font = Tools.LoadEmbeddedFont(embeddedFonts, theme.TextGradient.Font.EmbeddedFontName, theme.TextGradient.Font.Size, theme.TextGradient.Font.ToFontStyle());
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
                        Font = new Font(theme.TextGradient.Font.StandardFontName, theme.TextGradient.Font.Size, theme.TextGradient.Font.ToFontStyle());
                    }
                    catch
                    {
                        // Use default Font instead
                        Font = this.Font;
                    }
                }

                // Check if the gradient background should be used
                if (!theme.IsBackgroundTransparent)
                {                    
                    // Draw background gradient (cover -1 pixel for some refresh bug)
                    Rectangle rectBody = new Rectangle(-1, -1, Width + 1, Height + 1);
                    LinearGradientBrush brushBackground = new LinearGradientBrush(rectBody, theme.TextGradient.Color1, theme.TextGradient.Color2, theme.TextGradient.GradientMode);
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
        }
    }
}
