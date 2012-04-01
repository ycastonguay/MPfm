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
        /// Private value for the TextGradientDefault property.
        /// </summary>
        private TextGradient textGradientDefault = new TextGradient(Color.LightGray, Color.Gray, LinearGradientMode.Vertical, Color.DarkGray, 1, new CustomFont("Junction", 8.0f, Color.Black));
        /// <summary>
        /// Defines the text gradient (for the default state).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Defines the text gradient (for the default state).")]
        public TextGradient TextGradientDefault
        {
            get
            {
                return textGradientDefault;
            }
            set
            {
                textGradientDefault = value;
            }
        }

        /// <summary>
        /// Private value for the TextGradientMouseOver property.
        /// </summary>
        private TextGradient textGradientMouseOver = new TextGradient(Color.White, Color.LightGray, LinearGradientMode.Vertical, Color.FromArgb(70, 70, 70), 1, new CustomFont("Junction", 8.0f, Color.Black));
        /// <summary>
        /// Defines the text gradient (when the mouse cursor is over).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Defines the text gradient (when the mouse cursor is over).")]
        public TextGradient TextGradientMouseOver
        {
            get
            {
                return textGradientMouseOver;
            }
            set
            {
                textGradientMouseOver = value;
            }
        }

        /// <summary>
        /// Private value for the TextGradientDisabled property.
        /// </summary>
        private TextGradient textGradientDisabled = new TextGradient(Color.FromArgb(100, 100, 100), Color.FromArgb(50, 50, 50), LinearGradientMode.Vertical, Color.DarkGray, 1, new CustomFont("Junction", 8.0f, Color.LightGray));
        /// <summary>
        /// Defines the text gradient (when the control is disabled).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Defines the text gradient (when the control is disabled).")]
        public TextGradient TextGradientDisabled
        {
            get
            {
                return textGradientDisabled;
            }
            set
            {
                textGradientDisabled = value;
            }
        }

        //#region Border Properties

        //private Color borderColor = Color.Black;
        ///// <summary>
        ///// Color of the border.
        ///// </summary>
        //[RefreshProperties(RefreshProperties.Repaint)]
        //[Category("Border"), Browsable(true), Description("Color of the border.")]
        //public Color BorderColor
        //{
        //    get
        //    {
        //        return borderColor;
        //    }
        //    set
        //    {
        //        borderColor = value;
        //    }
        //}

        //private Color disabledBorderColor = Color.Gray;
        ///// <summary>
        ///// Color of the border (when the control is disabled).
        ///// </summary>
        //[RefreshProperties(RefreshProperties.Repaint)]
        //[Category("Border"), Browsable(true), Description("Color of the border when the control is disabled.")]
        //public Color DisabledBorderColor
        //{
        //    get
        //    {
        //        return disabledBorderColor;
        //    }
        //    set
        //    {
        //        disabledBorderColor = value;
        //    }
        //}

        //private Color mouseOverBorderColor = Color.Black;
        ///// <summary>
        ///// Color of the border (when the mouse is over the control).
        ///// </summary>
        //[RefreshProperties(RefreshProperties.Repaint)]
        //[Category("Border"), Browsable(true), Description("Color of the border when the mouse is over the control.")]
        //public Color MouseOverBorderColor
        //{
        //    get
        //    {
        //        return mouseOverBorderColor;
        //    }
        //    set
        //    {
        //        mouseOverBorderColor = value;
        //    }
        //}

        //private int borderWidth = 1;
        ///// <summary>
        ///// Width of the border.
        ///// </summary>
        //[RefreshProperties(RefreshProperties.Repaint)]
        //[Category("Border"), Browsable(true), Description("Width of the border.")]
        //public int BorderWidth
        //{
        //    get
        //    {
        //        return borderWidth;
        //    }
        //    set
        //    {
        //        borderWidth = value;
        //    }
        //}

        //#endregion

        //#region Background Properties

        //private Color gradientColor1 = Color.LightGray;
        ///// <summary>
        ///// First color of the background gradient.
        ///// </summary>
        //[RefreshProperties(RefreshProperties.Repaint)]
        //[Category("Background"), Browsable(true), Description("First color of the background gradient.")]
        //public Color GradientColor1
        //{
        //    get
        //    {
        //        return gradientColor1;
        //    }
        //    set
        //    {
        //        gradientColor1 = value;
        //    }
        //}

        //private Color gradientColor2 = Color.Gray;
        ///// <summary>
        ///// Second color of the background gradient.
        ///// </summary>
        //[RefreshProperties(RefreshProperties.Repaint)]
        //[Category("Background"), Browsable(true), Description("Second color of the background gradient.")]
        //public Color GradientColor2
        //{
        //    get
        //    {
        //        return gradientColor2;
        //    }
        //    set
        //    {
        //        gradientColor2 = value;
        //    }
        //}

        //private LinearGradientMode gradientMode = LinearGradientMode.Vertical;
        ///// <summary>
        ///// Background gradient mode.
        ///// </summary>
        //[Category("Background"), Browsable(true), Description("Background gradient mode.")]
        //public LinearGradientMode GradientMode
        //{
        //    get
        //    {
        //        return gradientMode;
        //    }
        //    set
        //    {
        //        gradientMode = value;
        //    }
        //}

        //private Color disabledGradientColor1 = Color.LightGray;
        ///// <summary>
        ///// First color of the background gradient (when control is disabled).
        ///// </summary>
        //[RefreshProperties(RefreshProperties.Repaint)]
        //[Category("Background"), Browsable(true), Description("First color of the background gradient (when control is disabled).")]
        //public Color DisabledGradientColor1
        //{
        //    get
        //    {
        //        return disabledGradientColor1;
        //    }
        //    set
        //    {
        //        disabledGradientColor1 = value;
        //    }
        //}

        //private Color disabledGradientColor2 = Color.Gray;
        ///// <summary>
        ///// Second color of the background gradient (when control is disabled).
        ///// </summary>
        //[RefreshProperties(RefreshProperties.Repaint)]
        //[Category("Background"), Browsable(true), Description("Second color of the background gradient (when control is disabled).")]
        //public Color DisabledGradientColor2
        //{
        //    get
        //    {
        //        return disabledGradientColor2;
        //    }
        //    set
        //    {
        //        disabledGradientColor2 = value;
        //    }
        //}

        //private Color mouseOverGradientColor1 = Color.LightGray;
        ///// <summary>
        ///// First color of the background gradient (when mouse cursor is over the control).
        ///// </summary>
        //[RefreshProperties(RefreshProperties.Repaint)]
        //[Category("Background"), Browsable(true), Description("First color of the background gradient (when mouse cursor is over the control).")]
        //public Color MouseOverGradientColor1
        //{
        //    get
        //    {
        //        return mouseOverGradientColor1;
        //    }
        //    set
        //    {
        //        mouseOverGradientColor1 = value;
        //    }
        //}

        //private Color mouseOverGradientColor2 = Color.Gray;
        ///// <summary>
        ///// Second color of the background gradient (when mouse cursor is over the control).
        ///// </summary>
        //[RefreshProperties(RefreshProperties.Repaint)]
        //[Category("Background"), Browsable(true), Description("Second color of the background gradient (when mouse cursor is over the control).")]
        //public Color MouseOverGradientColor2
        //{
        //    get
        //    {
        //        return mouseOverGradientColor2;
        //    }
        //    set
        //    {
        //        mouseOverGradientColor2 = value;
        //    }
        //}

        //#endregion

        //#region Fonts Properties

        //private Color fontColor = Color.Black;
        ///// <summary>
        ///// Fore color used when drawing the embedded font.
        ///// </summary>
        //[RefreshProperties(RefreshProperties.Repaint)]
        //[Category("Fonts"), Browsable(true), Description("Fore color used when drawing the embedded font.")]
        //public Color FontColor
        //{
        //    get
        //    {
        //        return fontColor;
        //    }
        //    set
        //    {
        //        fontColor = value;
        //    }               
        //}

        //private Color disabledFontColor = Color.Gray;
        ///// <summary>
        ///// Fore color used when drawing the embedded font (when control is disabled).
        ///// </summary>
        //[RefreshProperties(RefreshProperties.Repaint)]
        //[Category("Fonts"), Browsable(true), Description("Fore color used when drawing the embedded font (when control is disabled).")]
        //public Color DisabledFontColor
        //{
        //    get
        //    {
        //        return disabledFontColor;
        //    }
        //    set
        //    {
        //        disabledFontColor = value;
        //    }
        //}

        //private Color mouseOverFontColor = Color.Black;
        ///// <summary>
        ///// Fore color used when drawing the embedded font (when mouse cursor is over the control).
        ///// </summary>
        //[RefreshProperties(RefreshProperties.Repaint)]
        //[Category("Fonts"), Browsable(true), Description("Fore color used when drawing the embedded font (when mouse cursor is over the control).")]
        //public Color MouseOverFontColor
        //{
        //    get
        //    {
        //        return mouseOverFontColor;
        //    }
        //    set
        //    {
        //        mouseOverFontColor = value;
        //    }
        //}

        ///// <summary>
        ///// Private value for the CustomFont property.
        ///// </summary>
        //private CustomFont customFont = null;
        ///// <summary>
        ///// Defines the font to be used for rendering the control.
        ///// </summary>
        //[RefreshProperties(RefreshProperties.Repaint)]
        //[Category("Theme"), Browsable(true), Description("Font used for rendering the control.")]
        //public CustomFont CustomFont
        //{
        //    get
        //    {
        //        return customFont;
        //    }
        //    set
        //    {
        //        customFont = value;
        //        Refresh();
        //    }
        //}

        //#endregion

        /// <summary>
        /// Default constructor for the Button class.
        /// </summary>
        public Button()
        {
            // Set styles
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
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

            // Check state and select gradient
            TextGradient gradient = this.TextGradientDefault;
            if (!Enabled)
            {
                gradient = this.TextGradientDisabled;
            }
            else if (isMouseOver)
            {
                gradient = this.TextGradientMouseOver;
            }

            // Use anti-aliasing?
            if (gradient.Font.UseAntiAliasing)
            {
                // Set text anti-aliasing to ClearType (best looking AA)
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                // Set smoothing mode for paths
                g.SmoothingMode = SmoothingMode.AntiAlias;
            }

            // Create custom font
            Font font = null;

            // Make sure the embedded font name needs to be loaded and is valid
            if (gradient.Font.UseEmbeddedFont && !String.IsNullOrEmpty(gradient.Font.EmbeddedFontName))
            {
                try
                {
                    // Get embedded font
                    font = Tools.LoadEmbeddedFont(embeddedFonts, gradient.Font.EmbeddedFontName, gradient.Font.Size, gradient.Font.ToFontStyle());
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
                    font = new Font(gradient.Font.StandardFontName, gradient.Font.Size, gradient.Font.ToFontStyle());
                }
                catch
                {
                    // Use default font instead
                    font = this.Font;
                }
            }

            // Draw background gradient
            LinearGradientBrush brushBackground = null;
            brushBackground = new LinearGradientBrush(e.ClipRectangle, gradient.Color1, gradient.Color2, gradient.GradientMode);
            g.FillRectangle(brushBackground, ClientRectangle);
            brushBackground.Dispose();
            brushBackground = null;

            // Draw border
            if (gradient.BorderWidth > 0)
            {
                Pen penBorder = new Pen(gradient.BorderColor);
                g.DrawRectangle(penBorder, 0, 0, ClientRectangle.Width - 1, ClientRectangle.Height - 1);
                penBorder.Dispose();
                penBorder = null;
            }

            // Measure string and place it depending on alignment
            SizeF sizeString = g.MeasureString(this.Text, font);

            // Draw text
            SolidBrush brushFont = new SolidBrush(gradient.Font.Color);
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
