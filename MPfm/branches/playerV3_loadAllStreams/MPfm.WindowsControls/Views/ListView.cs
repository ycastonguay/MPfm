//
// ListView.cs: This list view control is based on the System.Windows.Forms.ListView control.
//              It adds custom flickerless redrawing and other features.
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
    /// This list view control is based on the System.Windows.Forms.ListView control.
    /// It adds custom flickerless redrawing and other features.
    /// </summary>
    public class ListView : System.Windows.Forms.ListView
    {
        #region Background Properties

        private Color m_gradientColor1 = Color.LightGray;
        /// <summary>
        /// First color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Configuration"), Browsable(true), Description("First color of the background gradient.")]
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
        [Category("Configuration"), Browsable(true), Description("Second color of the background gradient.")]
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

        #endregion

        #region Font Properties

        /// <summary>
        /// Pointer to the embedded font collection.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Pointer to the embedded font collection.")]
        public FontCollection FontCollection { get; set; }

        /// <summary>
        /// Name of the embedded font (as written in the Name property of a CustomFont).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Name of the embedded font (as written in the Name property of a CustomFont).")]
        public string CustomFontName { get; set; }

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

        #region Other Properties
        
        private Color m_selectedColor = Color.DarkGray;
        /// <summary>
        /// Selected font fore color.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Configuration"), Browsable(true), Description("Selected font fore color.")]
        public Color SelectedColor
        {
            get
            {
                return m_selectedColor;
            }
            set
            {
                m_selectedColor = value;
            }
        }

        private string m_headerCustomFontName = "";
        /// <summary>
        /// Name of the embedded font for the header (as written in the Name property of a CustomFont).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("")]
        [Category("Header"), Browsable(true), Description("Name of the embedded font for the header (as written in the Name property of a CustomFont).")]
        public string HeaderCustomFontName
        {
            get
            {
                return m_headerCustomFontName;
            }
            set
            {
                m_headerCustomFontName = value;
            }
        }

        private int m_headerHeight = 0;
        /// <summary>
        /// Height of the header.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("")]
        [Category("Header"), Browsable(true), Description("Height of the header.")]
        public int HeaderHeight
        {
            get
            {
                return m_headerHeight;
            }
            set
            {
                m_headerHeight = value;

            }
        }

        private Color m_headerForeColor = Color.Black;
        /// <summary>
        /// Fore font color used in the header.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("")]
        [Category("Header"), Browsable(true), Description("Fore font color used in the header.")]
        public Color HeaderForeColor
        {
            get
            {
                return m_headerForeColor;
            }
            set
            {
                m_headerForeColor = value;
            }
        }

        private Color m_headerGradientColor1 = Color.LightGray;
        /// <summary>
        /// First color of the background gradient used in the header.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("")]
        [Category("Header"), Browsable(true), Description("First color of the background gradient used in the header.")]
        public Color HeaderGradientColor1
        {
            get
            {
                return m_headerGradientColor1;
            }
            set
            {
                m_headerGradientColor1 = value;
            }
        }

        private Color m_headerGradientColor2 = Color.Gray;
        /// <summary>
        /// Second color of the background gradient used in the header.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("")]
        [Category("Header"), Browsable(true), Description("Second color of the background gradient used in the header.")]
        public Color HeaderGradientColor2
        {
            get
            {
                return m_headerGradientColor2;
            }
            set
            {
                m_headerGradientColor2 = value;
            }
        }

        #endregion

        /// <summary>
        /// Default constructor for ListView.
        /// </summary>
        public ListView()
        {            
            SetStyle(/*ControlStyles.AllPaintingInWmPaint |*/ ControlStyles.ResizeRedraw |
                /*ControlStyles.Opaque | ControlStyles.UserPaint |*/ ControlStyles.DoubleBuffer, true);
        }

        #region Paint Events
        
        /// <summary>
        /// Occurs when the kernel sends message to the control.
        /// Intercepts WM_ERASEBKGND and cancels the message to prevent flickering.
        /// </summary>
        /// <param name="m"></param>
        protected override void OnNotifyMessage(Message m)
        {
            // Do not let WM_ERASEBKGND pass (prevent flickering)
            if (m.Msg != 0x14)
            {
                base.OnNotifyMessage(m);
            }
        }

        /// <summary>        
        /// Paints the background of the control. This event is empty to prevent
        /// calls to OnPaintBackground.
        /// </summary>
        /// <param name="pevent">Paint Event Arguments</param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            LinearGradientBrush brushGradient = new LinearGradientBrush(e.ClipRectangle, m_headerGradientColor1, m_headerGradientColor2, LinearGradientMode.Vertical);

            e.Graphics.FillRectangle(brushGradient, e.ClipRectangle);
        }

        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    // Draw Header
        //    SolidBrush brushHeaderBackground = new SolidBrush(Color.Yellow);
        //    e.Graphics.FillRectangle(brushHeaderBackground, 0, 0, Width, 24);

        //    foreach (ColumnHeader column in Columns)
        //    {

        //    }

        //    // Draw Items
        //    foreach (ListViewItem item in Items)
        //    {
        //        if (e.ClipRectangle.IntersectsWith(item.GetBounds(ItemBoundsPortion.Entire)))
        //        {
        //            DrawItem(item, e);
        //        }
        //    }

        //    /*LinearGradientBrush brushGradient = new LinearGradientBrush(e.ClipRectangle, m_headerGradientColor1, m_headerGradientColor2, LinearGradientMode.Vertical);

        //    e.Graphics.FillRectangle(brushGradient, e.ClipRectangle);*/
        //    //base.OnPaint(e);
        //}

        //private void DrawItem(ListViewItem item, PaintEventArgs e)
        //{
        //    LinearGradientBrush brushBackground;
        //    SolidBrush brushForeground;

        //    Invalidate(item.GetBounds(ItemBoundsPortion.Entire));            

        //    if(item.Selected)
        //    {
        //        if( this.Focused )
        //        {
        //            brushBackground = new LinearGradientBrush(item.GetBounds(ItemBoundsPortion.Entire), Color.Blue, Color.Aqua, LinearGradientMode.Vertical);
        //            brushForeground = new SolidBrush(m_headerForeColor);
        //        }
        //        else
        //        {
        //            brushBackground = new LinearGradientBrush(item.GetBounds(ItemBoundsPortion.Entire), Color.Gray, Color.LightBlue, LinearGradientMode.Vertical);
        //            brushForeground = new SolidBrush(m_headerForeColor);
        //        }
        //    }
        //    else
        //    {
        //        brushBackground = new LinearGradientBrush(item.GetBounds(ItemBoundsPortion.Entire), Color.Gray, Color.DarkCyan, LinearGradientMode.Vertical);
        //        brushForeground = new SolidBrush(m_headerForeColor);
        //    }

        //    e.Graphics.FillRectangle(brushBackground, item.GetBounds(ItemBoundsPortion.Label));
        //    e.Graphics.DrawString(item.Text, item.Font, brushForeground, item.GetBounds(ItemBoundsPortion.Label).Location) ;
        //    e.Graphics.DrawRectangle(System.Drawing.Pens.Blue, item.GetBounds(ItemBoundsPortion.Entire));            
        //}

        /// <summary>
        /// Occurs when the control needs to draw an item.
        /// </summary>
        /// <param name="e">Draw ListViewItem Event Arguments</param>
        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            //base.OnDrawItem(e);

   
            e.DrawDefault = false;

            Graphics g = e.Graphics;

            // Set text anti-aliasing to ClearType (best looking AA)
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Set smoothing mode for paths
            g.SmoothingMode = SmoothingMode.AntiAlias;

            //Bitmap bmp = new Bitmap(Width, Height);
            //Graphics g = Graphics.FromImage(bmp);	

            Font font = this.Font;
            if (FontCollection != null && CustomFontName.Length > 0)
            {
                FontFamily family = FontCollection.GetFontFamily(CustomFontName);

                if (family != null)
                {
                    font = new Font(family, Font.Size, Font.Style);
                }
            }

            if ((e.State & ListViewItemStates.Selected) != 0)
            {
                // Draw the background and focus rectangle for a selected item.
                e.Graphics.FillRectangle(Brushes.LightGray, e.Bounds);
                e.DrawFocusRectangle();
            }
            else
            {
                // Draw the background for an unselected item.
                //using (LinearGradientBrush brush =
                //    new LinearGradientBrush(e.Bounds, Color.Orange,
                //    Color.Maroon, LinearGradientMode.Horizontal))
                //{
                //    e.Graphics.FillRectangle(brush, e.Bounds);
                //}

                using(LinearGradientBrush brush = new LinearGradientBrush(e.Bounds, GradientColor1, GradientColor2, LinearGradientMode.Horizontal))
                {
                    //e.Graphics.FillRectangle(brush, e.Bounds);
                }
            }

            // Draw the item text for views other than the Details view.
            if (this.View != View.Details)
            {
                //e.DrawText();
                g.DrawString(e.Item.Text, font, Brushes.Red, 5, 5);
            }

            //e.DrawDefault = false;

            //if (e.State == ListViewItemStates.Focused || e.State == ListViewItemStates.Selected)
            //{
            //    e.DrawFocusRectangle();
            //}

            //e.DrawBackground();
            //e.DrawText();


            //base.OnDrawItem(e);
        }

        /// <summary>
        /// Occurs when the control needs to draw a sub item.
        /// </summary>
        /// <param name="e">Draw ListViewSubItem Event Arguments</param>
        protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e)
        {
            //base.OnDrawSubItem(e);

            e.DrawDefault = false;

            TextFormatFlags flags = TextFormatFlags.Left;

            using (StringFormat sf = new StringFormat())
            {
                // Store the column text alignment, letting it default
                // to Left if it has not been set to Center or Right.
                switch (e.Header.TextAlign)
                {
                    case HorizontalAlignment.Center:
                        sf.Alignment = StringAlignment.Center;
                        flags = TextFormatFlags.HorizontalCenter;
                        break;
                    case HorizontalAlignment.Right:
                        sf.Alignment = StringAlignment.Far;
                        flags = TextFormatFlags.Right;
                        break;
                }

                // Draw the text and background for a subitem with a 
                // negative value. 
                double subItemValue;
                if (e.ColumnIndex > 0 && Double.TryParse(
                    e.SubItem.Text, NumberStyles.Currency,
                    NumberFormatInfo.CurrentInfo, out subItemValue) &&
                    subItemValue < 0)
                {
                    // Unless the item is selected, draw the standard 
                    // background to make it stand out from the gradient.
                    if ((e.ItemState & ListViewItemStates.Selected) == 0)
                    {
                        e.DrawBackground();
                    }

                    // Draw the subitem text in red to highlight it. 
                    e.Graphics.DrawString(e.SubItem.Text,
                        this.Font, Brushes.Red, e.Bounds, sf);

                    return;
                }

                // Draw normal text for a subitem with a nonnegative 
                // or nonnumerical value.
                e.DrawText(flags);
            }

            //e.DrawDefault = false;

            //if (e.ItemState == ListViewItemStates.Focused)
            //{
            //    e.DrawFocusRectangle(e.Bounds);
            //}


            //e.DrawBackground();
            //e.DrawText();                        

            //base.OnDrawSubItem(e);
        }

        /// <summary>
        /// Occurs when the control needs to draw a column header.
        /// </summary>
        /// <param name="e">Draw ListViewColumnHeader Event Arguments</param>
        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            /*using (StringFormat sf = new StringFormat())
            {
                // Store the column text alignment, letting it default
                // to Left if it has not been set to Center or Right.
                switch (e.Header.TextAlign)
                {
                    case HorizontalAlignment.Center:
                        sf.Alignment = StringAlignment.Center;
                        break;
                    case HorizontalAlignment.Right:
                        sf.Alignment = StringAlignment.Far;
                        break;
                }

                // Draw the standard header background.
                e.DrawBackground();

                // Draw the header text.
                using (Font headerFont =
                            new Font("Helvetica", 10, FontStyle.Bold))
                {
                    e.Graphics.DrawString(e.Header.Text, headerFont,
                        Brushes.Black, e.Bounds, sf);
                }
            }
            return;*/

            e.DrawDefault = false;

            Invalidate(e.Bounds);

            Graphics g = e.Graphics;

            if (e.Bounds.Width == 0)
            {
                return;
            }


            LinearGradientBrush brushGradient = new LinearGradientBrush(e.Bounds, m_headerGradientColor1, m_headerGradientColor2, LinearGradientMode.Vertical);
            g.FillRectangle(brushGradient, e.Bounds);

            //e.DrawBackground();
            e.DrawText();
            //base.OnDrawColumnHeader(e);
        }

        #endregion

        #region Mouse Events

        /// <summary>
        /// Occurs when the mouse cursor moves over the control.
        /// </summary>
        /// <param name="e">Mouse Event Arguments</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            /*ListViewItem item = GetItemAt(e.X, e.Y);
            if (item != null && item.Tag == null)
            {
                Invalidate(item.Bounds);
                item.Tag = "tagged";
            }*/

            base.OnMouseMove(e);
        }

        /// <summary>
        /// Occurs when the mouse button is released.
        /// </summary>
        /// <param name="e">Mouse Event Arguments</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            ListViewItem clickedItem = GetItemAt(5, e.Y);
            if (clickedItem != null)
            {
                clickedItem.Selected = true;
                clickedItem.Focused = true;
            }
            base.OnMouseUp(e);
        }

        #endregion
    }
}
