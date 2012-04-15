//
// Button.cs: This button control is similar to the System.Windows.Forms.Button class 
//            but adds support for embedded Fonts and anti-aliasing.
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
using System.Reflection;
using System.ComponentModel.Design;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This button control is similar to the System.Windows.Forms.Button class but 
    /// adds support for embedded Fonts and anti-aliasing.
    /// </summary>
    public class Button : Control
    {
        /// <summary>
        /// Private value for the IsMouseOver property.
        /// </summary>
        private bool isMouseOver = false;
        /// <summary>
        /// Indicates if the mouse cursor is over the control.
        /// </summary>
        public bool IsMouseOver
        {
            get
            {
                return isMouseOver;
            }
        }

        /// <summary>
        /// Private value for the Theme property.
        /// </summary>
        private ButtonTheme theme = null;
        /// <summary>
        /// Defines the current theme used for rendering the control.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Theme object for this control.")]
        public ButtonTheme Theme
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
        /// Private value for the TextAlign property.
        /// </summary>
        private ContentAlignment textAlign = ContentAlignment.MiddleRight;
        /// <summary>
        /// Defines the text alignment used in the text gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Defines the text alignement used in the text gradient.")]
        public ContentAlignment TextAlign
        {
            get
            {
                return textAlign;
            }
            set
            {
                textAlign = value;
            }
        }

        /// <summary>
        /// Private value for the ImageAlign property.
        /// </summary>
        private ContentAlignment imageAlign = ContentAlignment.MiddleLeft;
        /// <summary>
        /// Defines the image alignement used when rendering the button icon
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Defines the image alignement used when rendering the button icon.")]
        public ContentAlignment ImageAlign
        {
            get
            {
                return imageAlign;
            }
            set
            {
                imageAlign = value;
            }
        }

        /// <summary>
        /// Private value for the Image property.
        /// </summary>
        private Image image = null;
        /// <summary>
        /// Defines the image used when rendering the button icon.
        /// If null, no image will be shown.
        /// </summary>
        public Image Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
            }
        }

        /// <summary>
        /// Private value for the IsAutoSized property.
        /// </summary>
        private bool isAutoSized = false;
        /// <summary>
        /// Defines if the control should be automatically resized depending on its content.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Defines if the control should be automatically resized depending on its content.")]
        public bool IsAutoSized
        {
            get
            {
                return isAutoSized;
            }
            set
            {
                isAutoSized = value;
            }
        }

        /// <summary>
        /// Default constructor for the Button class.
        /// </summary>
        public Button()
        {
            // Set styles
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            // Create default theme
            theme = new ButtonTheme();
        }

        #region Paint Events
        
        /// <summary>
        /// Occurs when the control needs to be painted.
        /// </summary>
        /// <param name="e">Paint Event arguments</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Create a bitmap the size of the form.
            Bitmap bmp = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
            Graphics g = Graphics.FromImage(bmp);

            // Check state and select gradient
            TextGradient gradient = this.Theme.TextGradientDefault;
            if (!Enabled)
            {
                gradient = this.Theme.TextGradientDisabled;
            }
            else if (isMouseOver)
            {
                gradient = this.Theme.TextGradientMouseOver;
            }

            // Use anti-aliasing?
            if (gradient.Font.UseAntiAliasing)
            {
                // Set anti-aliasing
                PaintHelper.SetAntiAliasing(g);
            }

            // Get font
            Font font = PaintHelper.LoadFont(embeddedFonts, gradient.Font);

            // If the embedded font could not be loaded, get the default font
            if (font == null)
            {
                // Use default Font instead
                font = this.Font;
            }

            // Check for auto-size
            if (isAutoSized)
            {
                // Measure string                
                SizeF sizeString = g.MeasureString(Text, font);
                Size sizeControl = sizeString.ToSize();

                // Add image size (if available)
                if (Image != null)
                {
                    bool sameLineWidth = false;
                    bool sameLineHeight = false;

                    // Check alignment (left/center/right)
                    if ((ImageAlign == ContentAlignment.TopLeft || ImageAlign == ContentAlignment.MiddleLeft || ImageAlign == ContentAlignment.BottomLeft) &&
                        (TextAlign == ContentAlignment.TopLeft || TextAlign == ContentAlignment.MiddleLeft || TextAlign == ContentAlignment.BottomLeft))
                    {
                        sameLineWidth = true;
                    }
                    else if ((ImageAlign == ContentAlignment.TopCenter || ImageAlign == ContentAlignment.MiddleCenter || ImageAlign == ContentAlignment.BottomCenter) &&
                              (TextAlign == ContentAlignment.TopCenter || TextAlign == ContentAlignment.MiddleCenter || TextAlign == ContentAlignment.BottomCenter))
                    {
                        sameLineWidth = true;
                    }
                    else if ((ImageAlign == ContentAlignment.TopRight || ImageAlign == ContentAlignment.MiddleRight || ImageAlign == ContentAlignment.BottomRight) &&
                             (TextAlign == ContentAlignment.TopRight || TextAlign == ContentAlignment.MiddleRight || TextAlign == ContentAlignment.BottomRight))
                    {
                        sameLineWidth = true;
                    }
                    // Check alignment (top/middle/bottom)
                    if ((ImageAlign == ContentAlignment.TopCenter || ImageAlign == ContentAlignment.TopLeft || ImageAlign == ContentAlignment.TopRight) &&
                        (TextAlign == ContentAlignment.TopCenter || TextAlign == ContentAlignment.TopLeft || TextAlign == ContentAlignment.TopRight))
                    {
                        sameLineHeight = true;
                    }
                    else if ((ImageAlign == ContentAlignment.MiddleCenter || ImageAlign == ContentAlignment.MiddleLeft || ImageAlign == ContentAlignment.MiddleRight) &&
                              (TextAlign == ContentAlignment.MiddleCenter || TextAlign == ContentAlignment.MiddleLeft || TextAlign == ContentAlignment.MiddleRight))
                    {
                        sameLineHeight = true;
                    }
                    else if ((ImageAlign == ContentAlignment.BottomCenter || ImageAlign == ContentAlignment.BottomLeft || ImageAlign == ContentAlignment.BottomRight) &&
                             (TextAlign == ContentAlignment.BottomCenter || TextAlign == ContentAlignment.BottomLeft || TextAlign == ContentAlignment.BottomRight))
                    {
                        sameLineHeight = true;
                    }

                    // Calculate size
                    if (sameLineWidth)
                    {
                        // Take the widest between the two
                        if (Image.Width > sizeString.Width)
                        {
                            sizeControl.Width = image.Width;
                        }
                        else
                        {
                            sizeControl.Width = (int)sizeString.Width;
                        }
                    }
                    else
                    {
                        // Add two width + padding
                        sizeControl.Width = image.Width + gradient.Padding + (int)sizeString.Width;
                    }
                    if (sameLineHeight)
                    {
                        // Take the highest between the two                        
                        if (Image.Height > sizeString.Height)
                        {
                            sizeControl.Height = image.Height;
                        }
                        else
                        {
                            sizeControl.Height = (int)sizeString.Height;
                        }
                    }
                    else
                    {
                        // Add two height + 3x padding                        
                        sizeControl.Height = image.Height + gradient.Padding + (int)sizeString.Height;
                    }
                }
                else
                {
                    // No image; easy, just center with padding.
                    sizeControl = sizeString.ToSize();
                }

                // Add padding                
                sizeControl.Width += gradient.Padding * 2;
                sizeControl.Height += gradient.Padding * 2;

                // Resize control only if size is different
                if (Size.Width != sizeControl.Width ||
                    Size.Height != sizeControl.Height)
                {
                    // Update control size
                    this.Size = sizeControl;
                }                
            }

            // Draw background gradient (cover -1 pixel to fix graphic bug) 
            Rectangle rectBackground = new Rectangle(-1, -1, ClientRectangle.Width + 2, ClientRectangle.Height + 2);
            Rectangle rectBorder = new Rectangle(0, 0, ClientRectangle.Width - 1, ClientRectangle.Height - 1);
            PaintHelper.RenderBackgroundGradient(g, rectBackground, rectBorder, gradient);

            // Render text
            PaintHelper.RenderTextWithAlignment(g, ClientRectangle, font, Text, TextAlign, gradient.Font.Color, gradient.Padding);

            // Render image
            if (image != null)
            {                
                PaintHelper.RenderImageWithAlignment(g, ClientRectangle, Image, ImageAlign);
            }

            // Draw bitmap on control
            e.Graphics.DrawImage(bmp, 0, 0, ClientRectangle, GraphicsUnit.Pixel);

            // Dispose font if necessary
            if (font != null && font != this.Font)
            {
                // Dispose font
                font.Dispose();
                font = null;
            }

            // Dispose graphics and bitmap
            bmp.Dispose();
            bmp = null;
            g.Dispose();
            g = null;
        }
                
        /// <summary>        
        /// Paints the background of the control. This event is empty to prevent
        /// calls to OnPaintBackground.
        /// </summary>
        /// <param name="pevent">Paint Event arguments</param>
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // Do not allow to paint the background            
        }

        #endregion

        #region Mouse Events
        
        /// <summary>
        /// Occurs when the mouse cursor enters the control.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnMouseEnter(EventArgs e)
        {
            isMouseOver = true;
            Refresh();
            base.OnMouseEnter(e);            
        }

        /// <summary>
        /// Occurs when the mouse cursor leaves the control.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnMouseLeave(EventArgs e)
        {                        
            isMouseOver = false;
            Refresh();
            base.OnMouseLeave(e);
        }

        #endregion
    }
}
