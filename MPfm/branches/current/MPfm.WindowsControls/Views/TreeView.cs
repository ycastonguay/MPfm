//
// TreeView.cs: This tree view control is based on the System.Windows.Forms.TreeView control.
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
    /// This tree view control is based on the System.Windows.Forms.TreeView control.
    /// It adds custom flickerless redrawing and other features.
    /// </summary>
    public class TreeView : System.Windows.Forms.TreeView
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

        /// <summary>
        /// Default constructor for TreeView.
        /// </summary>
        public TreeView()
        {
            //OwnerDraw = true;
            //SetStyle(/*ControlStyles.AllPaintingInWmPaint |*/ ControlStyles.ResizeRedraw |
            //    /*ControlStyles.Opaque | ControlStyles.UserPaint |*/ ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                /*ControlStyles.UserPaint |*/
            ControlStyles.AllPaintingInWmPaint, true);
            

            //this.DrawMode = TreeViewDrawMode.OwnerDrawText;

            //SetStyle(ControlStyles.EnableNotifyMessage, true);
        }

        /// <summary>
        /// Overrides kernel messages to avoid flicker on resize.
        /// http://social.msdn.microsoft.com/Forums/en/winforms/thread/6b2933f1-d659-4be4-8f14-c1adc2690a43
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        //protected override void OnNotifyMessage(Message m)
        //{
        //    // Do not let WM_ERASEBKGND pass (prevent flickering)
        //    //if (m.Msg != 0x14)
        //    //{
        //    //    base.OnNotifyMessage(m);
        //    //}
        //}

        //protected override void OnDrawNode(DrawTreeNodeEventArgs e)
        //{
        //    //TextRenderer.DrawText()

        //    //base.OnDrawNode(e);            

        //    Graphics g = e.Graphics;

        //    // Set text anti-aliasing to ClearType (best looking AA)
        //    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

        //    // Set smoothing mode for paths
        //    g.SmoothingMode = SmoothingMode.AntiAlias;

        //    //Bitmap bmp = new Bitmap(Width, Height);
        //    //Graphics g = Graphics.FromImage(bmp);	

        //    Font font = this.Font;

        //    if (m_fontCollection != null && m_customFontName.Length > 0)
        //    {
        //        FontFamily family = m_fontCollection.GetFontFamily(m_customFontName);

        //        if (family != null)
        //        {
        //            font = new Font(family, Font.Size, Font.Style);
        //        }
        //    }


        //    if (e.Node.IsSelected)
        //    {
        //        g.FillRectangle(Brushes.Gray, new Rectangle(e.Bounds.Left, e.Bounds.Top, this.ClientSize.Width - e.Bounds.Left, e.Bounds.Height));
        //    }






        //    SolidBrush brushFont = new SolidBrush(ForeColor);

        //           TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.SingleLine |
        //   TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;

        //    //g.DrawString(e.Node.Text, font, brushFont, e.Bounds);
        //           //TextRenderer.DrawText(e.Graphics, e.Node.Text, font,  new Rectangle( (e.Bounds.Left, e.Bounds.Top), Color.Black, Color.White, flags);

            
            
        //    Color colorBackground = Color.White;

        //    if (e.Node.IsSelected)
        //    {
        //        colorBackground = Color.DarkGray;
        //    }

        //    TextRenderer.DrawText(e.Graphics, e.Node.Text, font, new Point(e.Bounds.Left, e.Bounds.Top), Color.Black);
        //    //g.DrawString(e.Node.Text, font, Brushes.Black, new Point(e.Bounds.Left, e.Bounds.Top));


        //}        
    }
}
