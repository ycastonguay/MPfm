//
// ListBox.cs: This list box control is based on the System.Windows.Forms.ListBox control.
//             It adds custom drawing, supports embedded fonts and other features.
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
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This list box control is based on the System.Windows.Forms.ListBox control.
    /// It adds custom drawing, supports embedded fonts and other features.
    ///
    /// Inspired from the following page:
    /// http://yacsharpblog.blogspot.com/2008/07/listbox-flicker.html
    /// </summary>
    public class ListBox : System.Windows.Forms.ListBox
    {
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

        /// <summary>
        /// Default constructor for ListBox.
        /// </summary>
        public ListBox()
        {
            // Set control styles
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            this.DrawMode = DrawMode.OwnerDrawFixed;  

            // Create default font
            m_customFont = new CustomFont();
        }

        #region Paint Events
        
        /// <summary>
        /// Occurs when the control needs to draw an item.
        /// </summary>
        /// <param name="e">Draw Item Event arguments</param>
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            // Get graphics from paint event
            Graphics g = e.Graphics;

            // Check item count
            if (Items.Count == 0)
            {
                return;
            }

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

            // Draw background
            e.DrawBackground();                

            // Draw item
            g.DrawString(this.Items[e.Index].ToString(), font, new SolidBrush(this.ForeColor), new PointF(e.Bounds.X, e.Bounds.Y));

            // Dispose font if necessary
            if (font != null && font != this.Font)
            {
                // Dispose font
                font.Dispose();
                font = null;
            }

            base.OnDrawItem(e);
        }

        /// <summary>
        /// Occurs when the control needs to be painted.
        /// </summary>
        /// <param name="e">Paint Event arguments</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Region iRegion = new Region(e.ClipRectangle);
            e.Graphics.FillRegion(new SolidBrush(this.BackColor), iRegion);

            if (this.Items.Count > 0)
            {
                for (int i = 0; i < this.Items.Count; ++i)
                {
                    System.Drawing.Rectangle irect = this.GetItemRectangle(i);
                    //if (e.ClipRectangle.IntersectsWith(irect))
                    //{
                    //    //if ((this.SelectionMode == SelectionMode.One && this.SelectedIndex == i)
                    //    //|| (this.SelectionMode == SelectionMode.MultiSimple && this.SelectedIndices.Contains(i))
                    //    //|| (this.SelectionMode == SelectionMode.MultiExtended && this.SelectedIndices.Contains(i)))
                    //    //{
                    //    //    OnDrawItem(new DrawItemEventArgs(e.Graphics, this.Font,
                    //    //        irect, i,
                    //    //        DrawItemState.Selected, this.ForeColor,
                    //    //        this.BackColor));
                    //    //}
                    //    //else
                    //    //{
                    //    //    OnDrawItem(new DrawItemEventArgs(e.Graphics, this.Font,
                    //    //        irect, i,
                    //    //        DrawItemState.Default, this.ForeColor,
                    //    //        this.BackColor));
                    //    //}

                    //    OnDrawItem(new DrawItemEventArgs(e.Graphics, this.Font,
                    //    irect, i,
                    //    DrawItemState.Default, this.ForeColor,
                    //    this.BackColor));

                    //    iRegion.Complement(irect);
                    //}

                    OnDrawItem(new DrawItemEventArgs(e.Graphics, this.Font, irect, i,                       
                                                     DrawItemState.Default, this.ForeColor,
                                                     this.BackColor));
                }
            }
            base.OnPaint(e);
        }          

        //protected override void OnNotifyMessage(Message m)
        //{
        //    // Do not let WM_ERASEBKGND pass (prevent flickering)
        //    if (m.Msg != 0x14)
        //    {
        //        base.OnNotifyMessage(m);
        //    }
        //}

        #endregion
    }
}
