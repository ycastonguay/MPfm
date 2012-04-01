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
using System.Reflection;
using System.ComponentModel.Design;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This button control is based on System.Windows.Forms.Button.
    /// It adds support for embedded fonts and anti-aliasing, gradient backgrounds, and more.
    /// </summary>
    public class Button : System.Windows.Forms.Button
    {
        /// <summary>
        /// Embedded font collection used for drawing.
        /// </summary>
        private EmbeddedFontCollection embeddedFonts = null;

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

        #region Border Properties

        private Color borderColor = Color.Black;
        /// <summary>
        /// Color of the border.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Border"), Browsable(true), Description("Color of the border.")]
        public Color BorderColor
        {
            get
            {
                return borderColor;
            }
            set
            {
                borderColor = value;
            }
        }

        private Color disabledBorderColor = Color.Gray;
        /// <summary>
        /// Color of the border (when the control is disabled).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Border"), Browsable(true), Description("Color of the border when the control is disabled.")]
        public Color DisabledBorderColor
        {
            get
            {
                return disabledBorderColor;
            }
            set
            {
                disabledBorderColor = value;
            }
        }

        private Color mouseOverBorderColor = Color.Black;
        /// <summary>
        /// Color of the border (when the mouse is over the control).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Border"), Browsable(true), Description("Color of the border when the mouse is over the control.")]
        public Color MouseOverBorderColor
        {
            get
            {
                return mouseOverBorderColor;
            }
            set
            {
                mouseOverBorderColor = value;
            }
        }

        private int borderWidth = 1;
        /// <summary>
        /// Width of the border.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Border"), Browsable(true), Description("Width of the border.")]
        public int BorderWidth
        {
            get
            {
                return borderWidth;
            }
            set
            {
                borderWidth = value;
            }
        }

        #endregion

        #region Background Properties

        /// <summary>
        /// Private value for the Gradient property.
        /// </summary>
        private Gradient gradient = new Gradient(Color.LightGray, Color.Gray, LinearGradientMode.Vertical);
        /// <summary>
        /// Defines the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Configuration"), Browsable(true), Description("Defines the background gradient.")]
        public Gradient Gradient
        {
            get
            {
                return gradient;
            }
            set
            {
                gradient = value;
            }
        }

        /// <summary>
        /// Private value for the Gradient property.
        /// </summary>
        private Gradient mouseOverGradient = new Gradient(Color.LightGray, Color.Gray, LinearGradientMode.Vertical);
        /// <summary>
        /// Defines the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Configuration"), Browsable(true), Description("Defines the background gradient.")]
        public Gradient MouseOverGradient
        {
            get
            {
                return mouseOverGradient;
            }
            set
            {
                mouseOverGradient = value;
            }
        }

        /// <summary>
        /// Private value for the Gradient property.
        /// </summary>
        private Gradient disabledGradient = new Gradient(Color.LightGray, Color.Gray, LinearGradientMode.Vertical);
        /// <summary>
        /// Defines the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Configuration"), Browsable(true), Description("Defines the background gradient.")]
        public Gradient DisabledGradient
        {
            get
            {
                return disabledGradient;
            }
            set
            {
                disabledGradient = value;
            }
        }

        #endregion

        #region Fonts Properties

        private Color fontColor = Color.Black;
        /// <summary>
        /// Fore color used when drawing the embedded font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Fonts"), Browsable(true), Description("Fore color used when drawing the embedded font.")]
        public Color FontColor
        {
            get
            {
                return fontColor;
            }
            set
            {
                fontColor = value;
            }               
        }

        private Color disabledFontColor = Color.Gray;
        /// <summary>
        /// Fore color used when drawing the embedded font (when control is disabled).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Fonts"), Browsable(true), Description("Fore color used when drawing the embedded font (when control is disabled).")]
        public Color DisabledFontColor
        {
            get
            {
                return disabledFontColor;
            }
            set
            {
                disabledFontColor = value;
            }
        }

        private Color mouseOverFontColor = Color.Black;
        /// <summary>
        /// Fore color used when drawing the embedded font (when mouse cursor is over the control).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Fonts"), Browsable(true), Description("Fore color used when drawing the embedded font (when mouse cursor is over the control).")]
        public Color MouseOverFontColor
        {
            get
            {
                return mouseOverFontColor;
            }
            set
            {
                mouseOverFontColor = value;
            }
        }

        /// <summary>
        /// Private value for the CustomFont property.
        /// </summary>
        private CustomFont customFont = null;
        /// <summary>
        /// Defines the font to be used for rendering the control.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Font used for rendering the control.")]
        public CustomFont CustomFont
        {
            get
            {
                return customFont;
            }
            set
            {
                customFont = value;
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
            this.customFont = new CustomFont();
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
                brushBackground = new LinearGradientBrush(e.ClipRectangle, disabledGradient.Color1, disabledGradient.Color2, disabledGradient.GradientMode);
            }
            else if (isMouseOver)
            {
                brushBackground = new LinearGradientBrush(e.ClipRectangle, mouseOverGradient.Color1, mouseOverGradient.Color2, mouseOverGradient.GradientMode);
            }
            else
            {
                brushBackground = new LinearGradientBrush(e.ClipRectangle, gradient.Color1, gradient.Color2, gradient.GradientMode);
            }
            g.FillRectangle(brushBackground, ClientRectangle);
            brushBackground.Dispose();
            brushBackground = null;

            // Draw border
            if(borderWidth > 0)
            {
                Pen penBorder = null;
                if (!Enabled)
                {
                    penBorder = new Pen(disabledBorderColor);
                }
                else if (isMouseOver)
                {
                    penBorder = new Pen(mouseOverBorderColor);
                }
                else
                {
                    penBorder = new Pen(borderColor);
                }
                
                g.DrawRectangle(penBorder, 0, 0, ClientRectangle.Width - 1, ClientRectangle.Height - 1);
                penBorder.Dispose();
                penBorder = null;
            }

            // Draw text
            SolidBrush brushFont = null;
            if (!Enabled)
            {
                brushFont = new SolidBrush(disabledFontColor);
            }
            else if (isMouseOver)
            {
                brushFont = new SolidBrush(mouseOverFontColor);
            }
            else
            {
                brushFont = new SolidBrush(fontColor);
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
