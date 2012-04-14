//
// Label.cs: This label control is similar to the System.Windows.Forms.Label class but 
//           adds support for embedded Fonts and anti-aliasing.
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
    /// This label control is similar to the System.Windows.Forms.Label class but 
    /// adds support for embedded Fonts and anti-aliasing.
    /// </summary>
    public class Label : Control
    {
        /// <summary>
        /// Private value for the Theme property.
        /// </summary>
        private LabelTheme theme = null;
        /// <summary>
        /// Defines the current theme used for rendering the control.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Theme object for this control.")]
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
        /// Private value for the TextAlign property.
        /// </summary>
        private ContentAlignment textAlign = ContentAlignment.MiddleLeft;
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
        /// Private value for the TextAlign property.
        /// </summary>
        private bool isAutoSized = false;
        /// <summary>
        /// Defines if the control should be auto-sized.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Defines if the control should be auto-sized.")]
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
        /// Default constructor for the Label class.
        /// </summary>
        public Label()
        {
            // Set control styles
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                ControlStyles.Opaque | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);        
            
            // Create default theme
            theme = new LabelTheme();
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
                    // Set anti-aliasing
                    PaintHelper.SetAntiAliasing(g);
                }

                // Get font
                Font font = PaintHelper.LoadFont(embeddedFonts, theme.TextGradient.Font);

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

                    // Resize control
                    this.Size = sizeString.ToSize();
                }

                // Check if the gradient background should be used
                if (!theme.IsBackgroundTransparent)
                {                   
                    // Draw background gradient (cover -1 pixel for some refresh bug)
                    Rectangle rectBody = new Rectangle(-1, -1, Width + 1, Height + 1);
                    PaintHelper.RenderBackgroundGradient(g, rectBody, theme.TextGradient);
                }
                else
                {
                    // Call paint background
                    base.OnPaintBackground(pe); // CPU intensive when transparent
                }

                // Render text
                PaintHelper.RenderTextWithAlignment(g, ClientRectangle, font, Text, TextAlign, theme.TextGradient.Font.Color, theme.TextGradient.Padding);

                // Dispose font
                if (font != null && font != this.Font)
                {
                    // Dispose Font
                    font.Dispose();
                    font = null;
                }
            }
            catch
            {                
                throw;
            }
        }

        /// <summary>
        /// Occurs when the Text property value changes.
        /// Refreshes the control.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            Refresh();
        }
    }
}
