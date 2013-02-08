//
// LinkLabel.cs: This label control is similar to the System.Windows.Forms.LinkLabel class but 
//               adds support for embedded Fonts and anti-aliasing.
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
using System.Reflection;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This link label control is similar to the System.Windows.Forms.LinkLabel class but 
    /// adds support for embedded Fonts and anti-aliasing.
    /// </summary>
    public class LinkLabel : Control
    {
        /// <summary>
        /// Private value for the Theme property.
        /// </summary>
        private LinkLabelTheme theme = null;
        /// <summary>
        /// Defines the current theme used for rendering the control.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Theme object for this control.")]
        public LinkLabelTheme Theme
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
        /// Default constructor for the LinkLabel class.
        /// </summary>
        public LinkLabel()
        {
            // Set styles
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                ControlStyles.Opaque | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);

            // Create default theme
            theme = new LinkLabelTheme();

            // Set default cursor
            Cursor = Cursors.Hand;
        }

        /// <summary>
        /// Occurs when the control needs to be painted.
        /// </summary>
        /// <param name="pe">Paint Event arguments</param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            // Get graphics from event
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

                // Add padding
                Size sizeControl = sizeString.ToSize();
                sizeControl.Width += theme.TextGradient.Padding * 2;
                sizeControl.Height += theme.TextGradient.Padding * 2;

                // Resize control only if size is different
                if (Size.Width != sizeControl.Width ||
                    Size.Height != sizeControl.Height)
                {
                    // Update control size
                    this.Size = sizeControl;
                }   
            }

            // Check if the gradient background should be used
            if (!theme.IsBackgroundTransparent)
            {
                // Draw background gradient (cover -1 pixel to fix graphic bug) 
                Rectangle rectBackground = new Rectangle(-1, -1, ClientRectangle.Width + 2, ClientRectangle.Height + 2);
                Rectangle rectBorder = new Rectangle(0, 0, ClientRectangle.Width - 1, ClientRectangle.Height - 1);
                PaintHelper.RenderBackgroundGradient(g, rectBackground, rectBorder, theme.TextGradient);   
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
