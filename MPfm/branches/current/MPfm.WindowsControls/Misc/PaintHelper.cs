//
// PaintHelper.cs: This static class contains miscelleanous methods for rendering controls.
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
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This static class contains miscelleanous methods for rendering controls.
    /// </summary>
    public static class PaintHelper
    {
        /// <summary>
        /// Renders a gradient using the BackgroundGradient properties.
        /// </summary>
        /// <param name="g">Graphics object to render to</param>
        /// <param name="rect">Rectangle filling the gradient</param>
        /// <param name="gradient">BackgroundGradient object</param>
        public static void RenderBackgroundGradient(Graphics g, Rectangle rect, BackgroundGradient gradient)
        {
            // Render gradient background
            LinearGradientBrush brushBackground = new LinearGradientBrush(rect, gradient.Color1, gradient.Color2, gradient.GradientMode);
            g.FillRectangle(brushBackground, rect);
            brushBackground.Dispose();
            brushBackground = null;

            // Check if a border needs to be rendered
            if (gradient.BorderWidth > 0)
            {
                // Render border
                Pen penBorder = new Pen(gradient.BorderColor, gradient.BorderWidth);
                g.DrawRectangle(penBorder, rect);
                penBorder.Dispose();
                penBorder = null;
            }
        }

        /// <summary>
        /// Sets the properties necessary for rendering anti-aliased text on the Graphics object.
        /// </summary>
        /// <param name="g">Graphics object</param>
        public static void SetAntiAliasing(Graphics g)
        {
            // Set text anti-aliasing to ClearType (best looking AA)
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Set smoothing mode for paths
            g.SmoothingMode = SmoothingMode.AntiAlias;
        }

        /// <summary>
        /// Loads a standard or embedded font from MPfm.Fonts.
        /// </summary>
        /// <param name="embeddedFonts">Embedded font collection</param>
        /// <param name="customFont">Custom font</param>
        /// <returns>Font object</returns>
        public static Font LoadFont(EmbeddedFontCollection embeddedFonts, CustomFont customFont)
        {
            // Create custom Font
            Font font = null;

            // Make sure the embedded Font name needs to be loaded and is valid
            if (customFont.UseEmbeddedFont && !String.IsNullOrEmpty(customFont.EmbeddedFontName))
            {
                try
                {
                    // Get embedded Font
                    font = Tools.LoadEmbeddedFont(embeddedFonts, customFont.EmbeddedFontName, customFont.Size, customFont.ToFontStyle());
                }
                catch
                {
                }
            }

            // Check if Font is null
            if (font == null)
            {
                try
                {
                    // Try to get standard Font
                    font = new Font(customFont.StandardFontName, customFont.Size, customFont.ToFontStyle());
                }
                catch
                {
                }
            }

            return font;
        }
    }
}
