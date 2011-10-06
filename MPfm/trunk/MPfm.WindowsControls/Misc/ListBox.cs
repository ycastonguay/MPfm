//
// ListBox.cs: This list box control is based on the System.Windows.Forms.ListBox control.
//             It adds custom drawing, supports embedded fonts and other features.
//
// Copyright © 2011 Yanick Castonguay
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
        #region Font Properties

        /// <summary>
        /// Name of the embedded font (as written in the Name property of a CustomFont).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Name of the embedded font (as written in the Name property of a CustomFont).")]
        public string CustomFontName { get; set; }

        /// <summary>
        /// Pointer to the embedded font collection.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Pointer to the embedded font collection.")]
        public FontCollection FontCollection { get; set; }

        private bool m_antiAliasingEnabled = true;
        /// <summary>
        /// Use anti-aliasing when drawing the embedded font.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Configuration"), Browsable(true), Description("Use anti-aliasing when drawing the embedded font.")]
        public bool AntiAliasingEnabled
        {
            get
            {
                return m_antiAliasingEnabled;
            }
            set
            {
                m_antiAliasingEnabled = value;
            }
        }

        #endregion

        /// <summary>
        /// Default constructor for ListBox.
        /// </summary>
        public ListBox()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            this.DrawMode = DrawMode.OwnerDrawFixed;  
        }

        #region Paint Events
        
        /// <summary>
        /// Occurs when the control needs to draw an item.
        /// </summary>
        /// <param name="e">Draw Item Event Arguments</param>
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (this.Items.Count > 0)
            {
                Font font = this.Font;

                try
                {
                    if (FontCollection != null && CustomFontName.Length > 0)
                    {
                        FontFamily family = FontCollection.GetFontFamily(CustomFontName);

                        if (family != null)
                        {
                            font = new Font(family, Font.Size, Font.Style);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                e.DrawBackground();
                //e.Graphics.DrawString(this.Items[e.Index].ToString(), e.Font, new SolidBrush(this.ForeColor), new PointF(e.Bounds.X, e.Bounds.Y));
                e.Graphics.DrawString(this.Items[e.Index].ToString(), font, new SolidBrush(this.ForeColor), new PointF(e.Bounds.X, e.Bounds.Y));
            }
            base.OnDrawItem(e);
        }

        /// <summary>
        /// Occurs when the control needs to be painted.
        /// </summary>
        /// <param name="pe">Paint Event Arguments</param>
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
