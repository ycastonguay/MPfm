//
// Button.cs: This button control is based on the System.Windows.Forms.Button control.
//            It adds custom drawing and other features.
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

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This button control is based on System.Windows.Forms.Button.
    /// It adds support for embedded fonts and anti-aliasing, gradient backgrounds, and more.
    /// </summary>
    public class Button : System.Windows.Forms.Button
    {
        private bool m_isMouseOver = false;
        /// <summary>
        /// Indicates if the mouse cursor is over the control.
        /// </summary>
        public bool IsMouseOver
        {
            get
            {
                return m_isMouseOver;
            }
        }

        #region Border Properties

        private Color m_borderColor = Color.Black;
        /// <summary>
        /// Color of the border.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Border"), Browsable(true), Description("Color of the border.")]
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

        private Color m_disabledBorderColor = Color.Gray;
        /// <summary>
        /// Color of the border (when the control is disabled).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Border"), Browsable(true), Description("Color of the border when the control is disabled.")]
        public Color DisabledBorderColor
        {
            get
            {
                return m_disabledBorderColor;
            }
            set
            {
                m_disabledBorderColor = value;
            }
        }

        private Color m_mouseOverBorderColor = Color.Black;
        /// <summary>
        /// Color of the border (when the mouse is over the control).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Border"), Browsable(true), Description("Color of the border when the mouse is over the control.")]
        public Color MouseOverBorderColor
        {
            get
            {
                return m_mouseOverBorderColor;
            }
            set
            {
                m_mouseOverBorderColor = value;
            }
        }

        private int m_borderWidth = 1;
        /// <summary>
        /// Width of the border.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Border"), Browsable(true), Description("Width of the border.")]
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

        private Color m_disabledGradientColor1 = Color.LightGray;
        /// <summary>
        /// First color of the background gradient (when control is disabled).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Background"), Browsable(true), Description("First color of the background gradient (when control is disabled).")]
        public Color DisabledGradientColor1
        {
            get
            {
                return m_disabledGradientColor1;
            }
            set
            {
                m_disabledGradientColor1 = value;
            }
        }

        private Color m_disabledGradientColor2 = Color.Gray;
        /// <summary>
        /// Second color of the background gradient (when control is disabled).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Background"), Browsable(true), Description("Second color of the background gradient (when control is disabled).")]
        public Color DisabledGradientColor2
        {
            get
            {
                return m_disabledGradientColor2;
            }
            set
            {
                m_disabledGradientColor2 = value;
            }
        }

        private Color m_mouseOverGradientColor1 = Color.LightGray;
        /// <summary>
        /// First color of the background gradient (when mouse cursor is over the control).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Background"), Browsable(true), Description("First color of the background gradient (when mouse cursor is over the control).")]
        public Color MouseOverGradientColor1
        {
            get
            {
                return m_mouseOverGradientColor1;
            }
            set
            {
                m_mouseOverGradientColor1 = value;
            }
        }

        private Color m_mouseOverGradientColor2 = Color.Gray;
        /// <summary>
        /// Second color of the background gradient (when mouse cursor is over the control).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Background"), Browsable(true), Description("Second color of the background gradient (when mouse cursor is over the control).")]
        public Color MouseOverGradientColor2
        {
            get
            {
                return m_mouseOverGradientColor2;
            }
            set
            {
                m_mouseOverGradientColor2 = value;
            }
        }

        #endregion

        #region Fonts Properties

        private Color m_fontColor = Color.Black;
        /// <summary>
        /// Fore color used when drawing the embedded font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Fonts"), Browsable(true), Description("Fore color used when drawing the embedded font.")]
        public Color FontColor
        {
            get
            {
                return m_fontColor;
            }
            set
            {
                m_fontColor = value;
            }               
        }

        private Color m_disabledFontColor = Color.Gray;
        /// <summary>
        /// Fore color used when drawing the embedded font (when control is disabled).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Fonts"), Browsable(true), Description("Fore color used when drawing the embedded font (when control is disabled).")]
        public Color DisabledFontColor
        {
            get
            {
                return m_disabledFontColor;
            }
            set
            {
                m_disabledFontColor = value;
            }
        }

        private Color m_mouseOverFontColor = Color.Black;
        /// <summary>
        /// Fore color used when drawing the embedded font (when mouse cursor is over the control).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Fonts"), Browsable(true), Description("Fore color used when drawing the embedded font (when mouse cursor is over the control).")]
        public Color MouseOverFontColor
        {
            get
            {
                return m_mouseOverFontColor;
            }
            set
            {
                m_mouseOverFontColor = value;
            }
        }

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

        #endregion

        /// <summary>
        /// Default constructor for the Button class.
        /// </summary>
        public Button()
        {
            // Set styles
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            // Set default font
            m_customFont = new CustomFont();
        }

        #region Paint Events
        
        /// <summary>
        /// Occurs when the control needs to be painted.
        /// </summary>
        /// <param name="e">Paint Event Arguments</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Create a bitmap the size of the form.
            Bitmap bmp = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
            Graphics g = Graphics.FromImage(bmp);

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
                catch
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
                catch
                {
                    // Use default font instead
                    font = this.Font;
                }
            }

            // Draw background gradient
            LinearGradientBrush brushBackground = null;
            if (!Enabled)
            {
                brushBackground = new LinearGradientBrush(e.ClipRectangle, m_disabledGradientColor1, m_disabledGradientColor2, m_gradientMode);
            }
            else if (m_isMouseOver)
            {
                brushBackground = new LinearGradientBrush(e.ClipRectangle, m_mouseOverGradientColor1, m_mouseOverGradientColor2, m_gradientMode);
            }
            else
            {
                brushBackground = new LinearGradientBrush(e.ClipRectangle, m_gradientColor1, m_gradientColor2, m_gradientMode);
            }
            g.FillRectangle(brushBackground, ClientRectangle);
            brushBackground.Dispose();
            brushBackground = null;

            // Draw border
            if(m_borderWidth > 0)
            {
                Pen penBorder = null;
                if (!Enabled)
                {
                    penBorder = new Pen(m_disabledBorderColor);
                }
                else if (m_isMouseOver)
                {
                    penBorder = new Pen(m_mouseOverBorderColor);
                }
                else
                {
                    penBorder = new Pen(m_borderColor);
                }
                
                g.DrawRectangle(penBorder, 0, 0, ClientRectangle.Width - 1, ClientRectangle.Height - 1);
                penBorder.Dispose();
                penBorder = null;
            }

            // Draw text
            SolidBrush brushFont = null;
            if (!Enabled)
            {
                brushFont = new SolidBrush(m_disabledFontColor);
            }
            else if (m_isMouseOver)
            {
                brushFont = new SolidBrush(m_mouseOverFontColor);
            }
            else
            {
                brushFont = new SolidBrush(m_fontColor);
            }

            // Measure string and place it depending on alignment
            SizeF sizeString = g.MeasureString(this.Text, font);
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

            // Dispose brush
            brushFont.Dispose();
            brushFont = null;

            // Draw Image
            if (this.Image != null)
            {
                if (ImageAlign == ContentAlignment.BottomLeft)
                {
                    //g.DrawString(Text, font, brushFont, 2, (this.Height - sizeString.Height) - 2);
                }
                else if (ImageAlign == ContentAlignment.BottomCenter)
                {
                    //g.DrawString(Text, font, brushFont, (this.Width - sizeString.Width) / 2, (this.Height - sizeString.Height) - 2);
                }
                else if (ImageAlign == ContentAlignment.BottomRight)
                {
                    //g.DrawString(Text, font, brushFont, (this.Width - sizeString.Width) - 2, (this.Height - sizeString.Height) - 2);
                }
                else if (ImageAlign == ContentAlignment.MiddleLeft)
                {
                    //g.DrawString(Text, font, brushFont, 2, (this.Height - sizeString.Height) / 2);

                    // height = 100, image height = 10
                    // to center middle y: (height / 2) - (image height /2)

                    Point pt = new Point(4, (e.ClipRectangle.Height / 2) - (Image.Height / 2));
                    g.DrawImage(Image, pt);  
                }
                else if (ImageAlign == ContentAlignment.MiddleCenter)
                {
                    //g.DrawString(Text, font, brushFont, (this.Width - sizeString.Width) / 2, (this.Height - sizeString.Height) / 2);
                }
                else if (ImageAlign == ContentAlignment.MiddleRight)
                {
                    //g.DrawString(Text, font, brushFont, (this.Width - sizeString.Width) - 2, (this.Height - sizeString.Height) / 2);
                }
                else if (ImageAlign == ContentAlignment.TopLeft)
                {
                    g.DrawImage(Image, new Point(4, 4));                    
                }
                else if (ImageAlign == ContentAlignment.TopCenter)
                {                    
                    Point pt = new Point((e.ClipRectangle.Width - Image.Width) / 2, 5);
                    g.DrawImage(Image, pt);                    
                }
                else if (ImageAlign == ContentAlignment.TopRight)
                {
                    Point pt = new Point(e.ClipRectangle.Width - Image.Width - 2, 5);
                    g.DrawImage(Image, pt);
                }
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
        /// <param name="pevent">Paint Event Arguments</param>
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // Do not allow to paint the background            
        }

        #endregion

        #region Mouse Events
        
        /// <summary>
        /// Occurs when the mouse cursor enters the control.
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected override void OnMouseEnter(EventArgs e)
        {
            m_isMouseOver = true;
            Refresh();
            base.OnMouseEnter(e);            
        }

        /// <summary>
        /// Occurs when the mouse cursor leaves the control.
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected override void OnMouseLeave(EventArgs e)
        {                        
            m_isMouseOver = false;
            Refresh();
            base.OnMouseLeave(e);
        }

        #endregion
    }
}
