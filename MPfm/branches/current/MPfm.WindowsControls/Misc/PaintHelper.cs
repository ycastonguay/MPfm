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
using System.Windows.Forms;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This static class contains miscelleanous methods for rendering controls.
    /// </summary>
    public static class PaintHelper
    {
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

        /// <summary>
        /// Renders a gradient using the BackgroundGradient properties.
        /// </summary>
        /// <param name="g">Graphics object to render to</param>
        /// <param name="rect">Rectangle to fill (used for gradient and border)</param>
        /// <param name="gradient">BackgroundGradient object</param>
        public static void RenderBackgroundGradient(Graphics g, RectangleF rect, BackgroundGradient gradient)
        {
            RenderBackgroundGradient(g, rect, rect, gradient);
        }

        /// <summary>
        /// Renders a gradient using the BackgroundGradient properties.
        /// </summary>
        /// <param name="g">Graphics object to render to</param>
        /// <param name="rect">Rectangle to fill with gradient</param>
        /// <param name="rectBorder">Rectangle used for the border</param>
        /// <param name="gradient">BackgroundGradient object</param>
        public static void RenderBackgroundGradient(Graphics g, RectangleF rect, RectangleF rectBorder, BackgroundGradient gradient)
        {
            RenderBackgroundGradient(g, rect, rectBorder, rect, gradient);
        }

        /// <summary>
        /// Renders a gradient using the BackgroundGradient properties.
        /// </summary>
        /// <param name="g">Graphics object to render to</param>
        /// <param name="rect">Rectangle to fill with the gradient</param>        
        /// <param name="rectBorder">Rectangle used for the border</param>
        /// <param name="rectGradient">Rectangle defining the gradient size</param>
        /// <param name="gradient">BackgroundGradient object</param>
        public static void RenderBackgroundGradient(Graphics g, RectangleF rect, RectangleF rectBorder, RectangleF rectGradient, BackgroundGradient gradient)
        {
            // Render gradient background
            LinearGradientBrush brushBackground = new LinearGradientBrush(rectGradient, gradient.Color1, gradient.Color2, gradient.GradientMode);
            g.FillRectangle(brushBackground, rect);
            brushBackground.Dispose();
            brushBackground = null;

            // Check if a border needs to be rendered
            if (gradient.BorderWidth > 0)
            {
                // Render border                
                Pen penBorder = new Pen(gradient.BorderColor, gradient.BorderWidth);
                g.DrawRectangle(penBorder, Rectangle.Round(rectBorder));
                penBorder.Dispose();
                penBorder = null;
            }
        }

        /// <summary>
        /// Renders text with alignment.
        /// </summary>
        /// <param name="g">Graphics object to render to</param>
        /// <param name="rect">Rectangle representing the area where the content should be aligned</param>
        /// <param name="font">Font</param>
        /// <param name="text">Text to render</param>
        /// <param name="align">Alignment</param>
        /// <param name="color">Fore color</param>
        /// <param name="padding">Padding (space between borders and text)</param>
        public static void RenderTextWithAlignment(Graphics g, RectangleF rect, Font font, string text, ContentAlignment align, Color color, int padding)
        {
            // Create brush
            SolidBrush brushFont = new SolidBrush(color);

            // Check alignment
            if (align == ContentAlignment.TopLeft)
            {
                // Top left
                g.DrawString(text, font, brushFont, padding, padding);
            }
            else
            {
                // Measure string            
                SizeF sizeString = g.MeasureString(text, font);
                Size sizeString2 = TextRenderer.MeasureText(text, font);

                float remainingWidth = rect.Width - sizeString.Width;
                float remainingHeight = rect.Height - sizeString.Height;

                float stuffWidth = rect.Width / 2;
                float stuffHeight = rect.Height / 2;
                float stuffBWidth = sizeString.Width / 2;
                float stuffBHeight = sizeString.Height / 2;

                // Draw string depending on alignment
                if (align == ContentAlignment.BottomLeft)
                {
                    // Bottom left
                    g.DrawString(text, font, brushFont, padding, (rect.Height - sizeString.Height) - padding);
                }
                else if (align == ContentAlignment.BottomCenter)
                {
                    // Bottom center
                    g.DrawString(text, font, brushFont, (rect.Width - sizeString.Width) / 2, (rect.Height - sizeString.Height) - padding);
                }
                else if (align == ContentAlignment.BottomRight)
                {
                    // Bottom right
                    g.DrawString(text, font, brushFont, (rect.Width - sizeString.Width) - padding, (rect.Height - sizeString.Height) - padding);
                }
                else if (align == ContentAlignment.MiddleLeft)
                {
                    // Middle left
                    //g.DrawString(text, font, brushFont, padding, (rect.Height - sizeString.Height) / 2);

                    StringFormat format = new StringFormat();
                    format.FormatFlags = StringFormatFlags.NoWrap;
                    format.LineAlignment = StringAlignment.Center;
                    format.Alignment = StringAlignment.Near;

                    //RectangleF rectWithPadding = rect;
                    RectangleF rectWithPadding = RectangleF.Inflate(rect, 0, 0);
                    rectWithPadding.X = padding;
                   
                    // Fix for the Junction font
                    if(font.Name.ToUpper() == "JUNCTION")
                        rectWithPadding.Y -= 1;

                    //g.FillRectangle(Brushes.HotPink, rectWithPadding);
                    g.DrawString(text, font, brushFont, rectWithPadding, format);                    

                }
                else if (align == ContentAlignment.MiddleCenter)
                {
                    StringFormat format = new StringFormat();
                    format.LineAlignment = StringAlignment.Center;
                    format.Alignment = StringAlignment.Center;

                    // Fix for the Junction font
                    if (font.Name.ToUpper() == "JUNCTION")
                        rect.Y -= 1;

                    //g.FillRectangle(Brushes.Blue, rect);
                    g.DrawString(text, font, brushFont, rect, format);
                }
                else if (align == ContentAlignment.MiddleRight)
                {
                    StringFormat format = new StringFormat();
                    format.LineAlignment = StringAlignment.Center;
                    format.Alignment = StringAlignment.Far;

                    RectangleF rectWithPadding = RectangleF.Inflate(rect, -padding, 0);
                    
                    // Fix for the Junction font
                    if (font.Name.ToUpper() == "JUNCTION")
                        rectWithPadding.Y -= 1;

                    //g.FillRectangle(Brushes.HotPink, rectWithPadding);
                    g.DrawString(text, font, brushFont, rectWithPadding, format);                    
                }
                else if (align == ContentAlignment.TopCenter)
                {
                    // Top center
                    g.DrawString(text, font, brushFont, (rect.Width - sizeString.Width) / 2, padding);
                }
                else if (align == ContentAlignment.TopRight)
                {
                    // Top right
                    g.DrawString(text, font, brushFont, (rect.Width - sizeString.Width) - padding, padding);
                }
            }

            // Dispose stuff
            brushFont.Dispose();
            brushFont = null;
        }

        /// <summary>
        /// Renders an image with alignment using a 1:1 ratio.
        /// </summary>
        /// <param name="g">Graphics object to render to</param>
        /// <param name="rect">Rectangle representing the area where the content should be aligned</param>
        /// <param name="image">Image to render</param>
        /// <param name="align">Alignment</param>
        public static void RenderImageWithAlignment(Graphics g, Rectangle rect, Image image, ContentAlignment align)
        {
            // Check alignment
            if (align == ContentAlignment.BottomLeft)
            {
                //g.DrawString(Text, font, brushFont, 2, (this.Height - sizeString.Height) - 2);
            }
            else if (align == ContentAlignment.BottomCenter)
            {
                //g.DrawString(Text, font, brushFont, (this.Width - sizeString.Width) / 2, (this.Height - sizeString.Height) - 2);
            }
            else if (align == ContentAlignment.BottomRight)
            {
                //g.DrawString(Text, font, brushFont, (this.Width - sizeString.Width) - 2, (this.Height - sizeString.Height) - 2);
            }
            else if (align == ContentAlignment.MiddleLeft)
            {
                Point pt = new Point(4, (rect.Height / 2) - (image.Height / 2));
                g.DrawImage(image, pt);
            }
            else if (align == ContentAlignment.MiddleCenter)
            {
                //g.DrawString(Text, font, brushFont, (this.Width - sizeString.Width) / 2, (this.Height - sizeString.Height) / 2);
            }
            else if (align == ContentAlignment.MiddleRight)
            {
                //g.DrawString(Text, font, brushFont, (this.Width - sizeString.Width) - 2, (this.Height - sizeString.Height) / 2);
            }
            else if (align == ContentAlignment.TopLeft)
            {
                g.DrawImage(image, new Point(4, 4));
            }
            else if (align == ContentAlignment.TopCenter)
            {
                Point pt = new Point((rect.Width - image.Width) / 2, 5);
                g.DrawImage(image, pt);
            }
            else if (align == ContentAlignment.TopRight)
            {
                Point pt = new Point(rect.Width - image.Width - 2, 5);
                g.DrawImage(image, pt);
            }
        }
    }
}
