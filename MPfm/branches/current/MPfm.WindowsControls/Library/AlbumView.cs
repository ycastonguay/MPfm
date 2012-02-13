////
//// AlbumView.cs: This custom control displays the albums contained in a MPfm library.
////
//// Copyright © 2011-2012 Yanick Castonguay
////
//// This file is part of MPfm.
////
//// MPfm is free software: you can redistribute it and/or modify
//// it under the terms of the GNU General Public License as published by
//// the Free Software Foundation, either version 3 of the License, or
//// (at your option) any later version.
////
//// MPfm is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//// GNU General Public License for more details.
////
//// You should have received a copy of the GNU General Public License
//// along with MPfm. If not, see <http://www.gnu.org/licenses/>.

//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Data.Linq;
//using System.Data.Objects;
//using System.Drawing;
//using System.Drawing.Text;
//using System.Drawing.Drawing2D;
//using System.Globalization;
//using System.Linq;
//using System.IO;
//using System.Reflection;
//using System.Text;
//using System.Windows.Forms;
//using MPfm.Core;
//using MPfm.Sound;
//using MPfm.Library;

//namespace MPfm.WindowsControls
//{
//    /// <summary>
//    /// This custom control displays the albums contained in a MPfm library.
//    /// </summary>
//    public partial class AlbumView : System.Windows.Forms.Control, IMessageFilter
//    {
//        #region Theme Properties

//        #region Header

//        private Color m_headerColor1 = Color.FromArgb(165, 165, 165);
//        /// <summary>
//        /// First color of the background gradient.
//        /// </summary>
//        [RefreshProperties(RefreshProperties.Repaint)]
//        [Category("Theme"), Browsable(true), Description("First color of the header background gradient.")]
//        public Color HeaderColor1
//        {
//            get
//            {
//                return m_headerColor1;
//            }
//            set
//            {
//                m_headerColor1 = value;
//            }
//        }

//        private Color m_headerColor2 = Color.FromArgb(195, 195, 195);
//        /// <summary>
//        /// Second color of the background gradient.
//        /// </summary>
//        [RefreshProperties(RefreshProperties.Repaint)]
//        [Category("Theme"), Browsable(true), Description("Second color of the header background gradient.")]
//        public Color HeaderColor2
//        {
//            get
//            {
//                return m_headerColor2;
//            }
//            set
//            {
//                m_headerColor2 = value;
//            }
//        }

//        private Color m_headerHoverColor1 = Color.FromArgb(145, 145, 145);
//        /// <summary>
//        /// First color of the background gradient.
//        /// </summary>
//        [RefreshProperties(RefreshProperties.Repaint)]
//        [Category("Theme"), Browsable(true), Description("First color of the header background gradient when the mouse cursor is over the header.")]
//        public Color HeaderHoverColor1
//        {
//            get
//            {
//                return m_headerHoverColor1;
//            }
//            set
//            {
//                m_headerHoverColor1 = value;
//            }
//        }

//        private Color m_headerHoverColor2 = Color.FromArgb(175, 175, 175);
//        /// <summary>
//        /// Second color of the background gradient.
//        /// </summary>
//        [RefreshProperties(RefreshProperties.Repaint)]
//        [Category("Theme"), Browsable(true), Description("First color of the header background gradient when the mouse cursor is over the header.")]
//        public Color HeaderHoverColor2
//        {
//            get
//            {
//                return m_headerHoverColor2;
//            }
//            set
//            {
//                m_headerHoverColor2 = value;
//            }
//        }

//        private Color m_headerForeColor = Color.FromArgb(60, 60, 60);
//        /// <summary>
//        /// Fore font color used in the header.
//        /// </summary>
//        [RefreshProperties(RefreshProperties.Repaint)]
//        [DefaultValue("")]
//        [Category("Theme"), Browsable(true), Description("Fore color used when drawing the header font and other glyphs (such as the orderby icon).")]
//        public Color HeaderForeColor
//        {
//            get
//            {
//                return m_headerForeColor;
//            }
//            set
//            {
//                m_headerForeColor = value;
//            }
//        }

//        private string m_headerCustomFontName = "";
//        /// <summary>
//        /// Name of the embedded font for the header (as written in the Name property of a CustomFont).
//        /// </summary>
//        [RefreshProperties(RefreshProperties.Repaint)]
//        [DefaultValue("")]
//        [Category("Header"), Browsable(true), Description("Name of the embedded font for the header (as written in the Name property of a CustomFont).")]
//        public string HeaderCustomFontName
//        {
//            get
//            {
//                return m_headerCustomFontName;
//            }
//            set
//            {
//                m_headerCustomFontName = value;
//            }
//        }

//        #endregion

//        #region Line

//        private Color m_lineColor1 = Color.FromArgb(215, 215, 215);
//        /// <summary>
//        /// First color of the background gradient.
//        /// </summary>
//        [RefreshProperties(RefreshProperties.Repaint)]
//        [Category("Theme"), Browsable(true), Description("First color of the line background gradient.")]
//        public Color LineColor1
//        {
//            get
//            {
//                return m_lineColor1;
//            }
//            set
//            {
//                m_lineColor1 = value;
//            }
//        }

//        private Color m_lineColor2 = Color.FromArgb(235, 235, 235);
//        /// <summary>
//        /// Second color of the background gradient.
//        /// </summary>
//        [RefreshProperties(RefreshProperties.Repaint)]
//        [Category("Theme"), Browsable(true), Description("Second color of the line background gradient.")]
//        public Color LineColor2
//        {
//            get
//            {
//                return m_lineColor2;
//            }
//            set
//            {
//                m_lineColor2 = value;
//            }
//        }

//        private Color m_lineHoverColor1 = Color.FromArgb(235, 235, 235);
//        /// <summary>
//        /// First color of the background gradient.
//        /// </summary>
//        [RefreshProperties(RefreshProperties.Repaint)]
//        [Category("Theme"), Browsable(true), Description("First color of the line background gradient when the mouse cursor is over the line.")]
//        public Color LineHoverColor1
//        {
//            get
//            {
//                return m_lineHoverColor1;
//            }
//            set
//            {
//                m_lineHoverColor1 = value;
//            }
//        }

//        private Color m_lineHoverColor2 = Color.FromArgb(255, 255, 255);
//        /// <summary>
//        /// Second color of the background gradient.
//        /// </summary>
//        [RefreshProperties(RefreshProperties.Repaint)]
//        [Category("Theme"), Browsable(true), Description("Second color of the line background gradient when the mouse cursor is over the line.")]
//        public Color LineHoverColor2
//        {
//            get
//            {
//                return m_lineHoverColor2;
//            }
//            set
//            {
//                m_lineHoverColor2 = value;
//            }
//        }

//        private Color m_lineSelectedColor1 = Color.FromArgb(165, 165, 165);
//        /// <summary>
//        /// First color of the background gradient.
//        /// </summary>
//        [RefreshProperties(RefreshProperties.Repaint)]
//        [Category("Theme"), Browsable(true), Description("First color of the line background gradient when the line is selected.")]
//        public Color LineSelectedColor1
//        {
//            get
//            {
//                return m_lineSelectedColor1;
//            }
//            set
//            {
//                m_lineSelectedColor1 = value;
//            }
//        }

//        private Color m_lineSelectedColor2 = Color.FromArgb(185, 185, 185);
//        /// <summary>
//        /// Second color of the background gradient.
//        /// </summary>
//        [RefreshProperties(RefreshProperties.Repaint)]
//        [Category("Theme"), Browsable(true), Description("Second color of the line background gradient when the line is selected.")]
//        public Color LineSelectedColor2
//        {
//            get
//            {
//                return m_lineSelectedColor2;
//            }
//            set
//            {
//                m_lineSelectedColor2 = value;
//            }
//        }

//        private Color m_lineNowPlayingColor1 = Color.FromArgb(135, 235, 135);
//        /// <summary>
//        /// First color of the background gradient.
//        /// </summary>
//        [RefreshProperties(RefreshProperties.Repaint)]
//        [Category("Theme"), Browsable(true), Description("First color of the line background gradient when the line is now playing.")]
//        public Color LineNowPlayingColor1
//        {
//            get
//            {
//                return m_lineNowPlayingColor1;
//            }
//            set
//            {
//                m_lineNowPlayingColor1 = value;
//            }
//        }

//        private Color m_lineNowPlayingColor2 = Color.FromArgb(155, 255, 155);
//        /// <summary>
//        /// Second color of the background gradient.
//        /// </summary>
//        [RefreshProperties(RefreshProperties.Repaint)]
//        [Category("Theme"), Browsable(true), Description("Second color of the line background gradient when the line is now playing.")]
//        public Color LineNowPlayingColor2
//        {
//            get
//            {
//                return m_lineNowPlayingColor2;
//            }
//            set
//            {
//                m_lineNowPlayingColor2 = value;
//            }
//        }

//        private Color m_lineForeColor = Color.FromArgb(0, 0, 0);
//        /// <summary>
//        /// Fore font color used in the header.
//        /// </summary>
//        [RefreshProperties(RefreshProperties.Repaint)]
//        [DefaultValue("")]
//        [Category("Theme"), Browsable(true), Description("Fore color used when drawing the line font.")]
//        public Color LineForeColor
//        {
//            get
//            {
//                return m_lineForeColor;
//            }
//            set
//            {
//                m_lineForeColor = value;
//            }
//        }

//        private Color m_iconNowPlayingColor1 = Color.FromArgb(250, 200, 250);
//        /// <summary>
//        /// First color of the background gradient.
//        /// </summary>
//        [RefreshProperties(RefreshProperties.Repaint)]
//        [Category("Theme"), Browsable(true), Description("First color of the animated icon displaying the currently playing song.")]
//        public Color IconNowPlayingColor1
//        {
//            get
//            {
//                return m_iconNowPlayingColor1;
//            }
//            set
//            {
//                m_iconNowPlayingColor1 = value;
//            }
//        }

//        private Color m_iconNowPlayingColor2 = Color.FromArgb(25, 150, 25);
//        /// <summary>
//        /// Second color of the background gradient.
//        /// </summary>
//        [RefreshProperties(RefreshProperties.Repaint)]
//        [Category("Theme"), Browsable(true), Description("Second color of the animated icon displaying the currently playing song.")]
//        public Color IconNowPlayingColor2
//        {
//            get
//            {
//                return m_iconNowPlayingColor2;
//            }
//            set
//            {
//                m_iconNowPlayingColor2 = value;
//            }
//        }

//        #endregion

//        #region Album Covers

//        private Color m_albumCoverBackgroundColor1 = Color.FromArgb(55, 55, 55);
//        /// <summary>
//        /// First color of the background gradient.
//        /// </summary>
//        [RefreshProperties(RefreshProperties.Repaint)]
//        [Category("Theme"), Browsable(true), Description("First color of the album cover background gradient.")]
//        public Color AlbumCoverBackgroundColor1
//        {
//            get
//            {
//                return m_albumCoverBackgroundColor1;
//            }
//            set
//            {
//                m_albumCoverBackgroundColor1 = value;
//            }
//        }

//        private Color m_albumCoverBackgroundColor2 = Color.FromArgb(75, 75, 75);
//        /// <summary>
//        /// Second color of the background gradient.
//        /// </summary>
//        [RefreshProperties(RefreshProperties.Repaint)]
//        [Category("Theme"), Browsable(true), Description("Second color of the album cover background gradient.")]
//        public Color AlbumCoverBackgroundColor2
//        {
//            get
//            {
//                return m_albumCoverBackgroundColor2;
//            }
//            set
//            {
//                m_albumCoverBackgroundColor2 = value;
//            }
//        }

//        #endregion

//        private LinearGradientMode m_gradientMode = LinearGradientMode.Vertical;
//        /// <summary>
//        /// Background gradient mode.
//        /// </summary>
//        [Category("Background"), Browsable(true), Description("Background gradient mode.")]
//        public LinearGradientMode GradientMode
//        {
//            get
//            {
//                return m_gradientMode;
//            }
//            set
//            {
//                m_gradientMode = value;
//            }
//        }

//        #endregion

//        #region Font Properties

//        ///// <summary>
//        ///// Pointer to the embedded font collection.
//        ///// </summary>
//        //[RefreshProperties(RefreshProperties.Repaint)]
//        //[Category("Display"), Browsable(true), Description("Pointer to the embedded font collection.")]
//        //public FontCollection FontCollection { get; set; }

//        ///// <summary>
//        ///// Name of the embedded font (as written in the Name property of a CustomFont).
//        ///// </summary>
//        //[RefreshProperties(RefreshProperties.Repaint)]
//        //[Category("Display"), Browsable(true), Description("Name of the embedded font (as written in the Name property of a CustomFont).")]
//        //public string CustomFontName { get; set; }

//        //private bool m_antiAliasingEnabled = true;
//        ///// <summary>
//        ///// Use anti-aliasing when drawing the embedded font.
//        ///// </summary>
//        //[RefreshProperties(RefreshProperties.Repaint)]
//        //[Category("Configuration"), Browsable(true), Description("Use anti-aliasing when drawing the embedded font.")]
//        //public bool AntiAliasingEnabled
//        //{
//        //    get
//        //    {
//        //        return m_antiAliasingEnabled;
//        //    }
//        //    set
//        //    {
//        //        m_antiAliasingEnabled = value;
//        //    }
//        //}

//        #endregion

//        private int m_padding = 6;
//        private int m_iconSize = 128;
//        private MPfm.Library.Library m_library = null;
//        public MPfm.Library.Library Library
//        {
//            get
//            {
//                return m_library;
//            }
//            set
//            {
//                m_library = value;
//            }
//        }

//        private List<AlbumViewItem> m_items = null;
//        public List<AlbumViewItem> Items
//        {
//            get
//            {
//                return m_items;
//            }
//        }

//        private int m_fontSize = 8;
//        public int FontSize
//        {
//            get
//            {
//                return m_fontSize;
//            }
//            set
//            {
//                m_fontSize = value;

//                // Reset song cache and redraw
//                m_albumCache = null;
//                Refresh();
//            }
//        }

//        private bool m_displayDebugInformation = false;
//        public bool DisplayDebugInformation
//        {
//            get
//            {
//                return m_displayDebugInformation;
//            }
//            set
//            {
//                m_displayDebugInformation = value;
//            }
//        }

//        // Must replace to a list to do pushing        
//        private List<SongGridViewImageCache> m_imageCache = new List<SongGridViewImageCache>();
//        private int m_imageCacheSize = 10;
//        public int ImageCacheSize
//        {
//            get
//            {
//                return m_imageCacheSize;
//            }
//            set
//            {
//                m_imageCacheSize = value;
//            }
//        }

//        // Controls
//        private System.Windows.Forms.VScrollBar m_vScrollBar = null;
//        private System.Windows.Forms.HScrollBar m_hScrollBar = null;

//        // Create cache by display type
//        //private AlbumViewCache m_albumCache = null;

//        // Private members for Song display type
//        //private int m_startLineNumber = 0;
//        //private int m_numberOfLinesToDraw = 0;
//        //private int m_minimumColumnWidth = 30;
//        private int m_dragStartX = -1;
//        //private int m_dragOriginalColumnWidth = -1;
//        //private bool m_isMouseOverControl = false;
//        //private bool m_isUserMovingColumn = false;
//        //private int m_count = 0;
//        //private int m_lastItemIndexClicked = -1;

//        /// <summary>
//        /// Default constructor for SongGridView.
//        /// </summary>
//        public AlbumView()
//        {
//            // Set default styles
//            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
//                       ControlStyles.Opaque | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);

//            // Create vertical scrollbar
//            m_vScrollBar = new System.Windows.Forms.VScrollBar();
//            m_vScrollBar.Width = 16;
//            m_vScrollBar.Scroll += new ScrollEventHandler(m_vScrollBar_Scroll);
//            Controls.Add(m_vScrollBar);

//            // Create horizontal scrollbar
//            m_hScrollBar = new System.Windows.Forms.HScrollBar();
//            m_hScrollBar.Width = ClientRectangle.Width;
//            m_hScrollBar.Height = 16;
//            m_hScrollBar.Top = ClientRectangle.Height - m_hScrollBar.Height;
//            m_hScrollBar.Scroll += new ScrollEventHandler(m_hScrollBar_Scroll);
//            Controls.Add(m_hScrollBar);

//            // Override mouse messages for mouse wheel (get mouse wheel events out of control)
//            Application.AddMessageFilter(this);
//        }

//        /// <summary>
//        /// Occurs when the user changes the horizontal scrollbar value.
//        /// </summary>
//        /// <param name="sender">Event Sender</param>
//        /// <param name="e">Event Arguments</param>
//        protected void m_hScrollBar_Scroll(object sender, ScrollEventArgs e)
//        {
//            // If the 

//            // Redraw control
//            Refresh();
//        }

//        /// <summary>
//        /// Occurs when the user changes the vertical scrollbar value.
//        /// </summary>
//        /// <param name="sender">Event Sender</param>
//        /// <param name="e">Event Arguments</param>
//        protected void m_vScrollBar_Scroll(object sender, ScrollEventArgs e)
//        {
//            // Redraw control
//            Refresh();
//        }

//        #region Paint Events

//        /// <summary>
//        /// Occurs when the kernel sends message to the control.
//        /// Intercepts WM_ERASEBKGND and cancels the message to prevent flickering.
//        /// </summary>
//        /// <param name="m"></param>
//        protected override void OnNotifyMessage(Message m)
//        {
//            // Do not let WM_ERASEBKGND pass (prevent flickering)
//            if (m.Msg != 0x14)
//            {
//                base.OnNotifyMessage(m);
//            }
//        }


//        /// <summary>
//        /// Occurs when the control needs to be painted.
//        /// </summary>
//        /// <param name="e">Paint Event Arguments</param>
//        protected override void OnPaint(PaintEventArgs e)
//        {


//            //Region iRegion = new Region(e.ClipRectangle);
//            //e.Graphics.FillRegion(new SolidBrush(this.BackColor), iRegion);
//            //e.ClipRectangle
//            // Check if the library is valid
//            if (m_library == null)
//            {
//                return;
//            }

//            // Draw bitmap for control
//            //Bitmap bmp = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
//            //Graphics g = Graphics.FromImage(bmp);
//            Graphics g = e.Graphics;

//            // Set text anti-aliasing to ClearType (best looking AA)
//            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

//            // Set smoothing mode for paths
//            g.SmoothingMode = SmoothingMode.AntiAlias;

//            // Paint albums
//            PaintAlbums(ref g);

//            //g.DrawString(m_count.ToString(), Font, Brushes.Blue, new Point(20, 20));

//            //SolidBrush brush = new SolidBrush(Color.FromArgb(155, 255, 0, 0));
//            //g.FillRectangle(brush, g.ClipBounds);
//            //brush.Dispose();
//            //brush = null;

//            // Draw bitmap on control
//            //e.Graphics.DrawImage(bmp, 0, 0, ClientRectangle, GraphicsUnit.Pixel);
//            //bmp.Dispose();
//            //bmp = null;
//            //g.Dispose();
//            //g = null;               

//            base.OnPaint(e);
//        }

//        public void PaintAlbums(ref Graphics g)
//        {
//            //// Load custom font
//            //Font font = Tools.LoadCustomFont(FontCollection, CustomFontName, 9, FontStyle.Regular);
//            //if (font == null)
//            //{
//            //    // Load default font
//            //    font = new Font("Arial", 9, FontStyle.Regular);
//            //}

//            //// Check for an existing collection of items
//            //bool regenerateItems = true;
//            //if (m_items != null)
//            //{
//            //    // Check the first item, if there's one
//            //    if (m_items.Count > 0 && m_items[0] is AlbumViewItem)
//            //    {
//            //        regenerateItems = false;
//            //    }
//            //}

//            //// Check if the items have been generated, or that the items are not of album type
//            //if (regenerateItems)
//            //{
//            //    // Query how many albums there are in the library
//            //    Dictionary<string, string> listAlbums = m_library.SelectAlbumTitlesWithFilePaths();

//            //    // Create list of items
//            //    m_items = new List<AlbumViewItem>();
//            //    foreach (KeyValuePair<string, string> keyValue in listAlbums)
//            //    {
//            //        // Create item and add to list
//            //        AlbumViewItem item = new AlbumViewItem();
//            //        item.Title = keyValue.Key;
//            //        item.FilePath = keyValue.Value;
//            //        m_items.Add(item);
//            //    }
//            //}

//            //// Check if a cache exists, or if the cache needs to be refreshed
//            //if (m_albumCache == null)
//            //{
//            //    // Create cache data
//            //    m_albumCache = new AlbumViewCache();

//            //    // Check how many icons fit in width and height
//            //    m_albumCache.IconHeightWithPadding = m_iconSize + (m_padding * 2);
//            //    m_albumCache.NumberOfIconsWidth = (int)Math.Floor((double)ClientRectangle.Width / (double)(m_albumCache.IconHeightWithPadding));
//            //    m_albumCache.NumberOfIconsHeight = (int)Math.Floor((double)ClientRectangle.Height / (double)(m_albumCache.IconHeightWithPadding));

//            //    // Calculate margin (space not taken by the icon + padding)
//            //    m_albumCache.TotalMargin = ClientRectangle.Width - ((m_albumCache.NumberOfIconsWidth * m_iconSize) + (m_padding * 2 * m_albumCache.NumberOfIconsWidth));
//            //    m_albumCache.Margin = (int)((double)m_albumCache.TotalMargin / 2);

//            //    // Calculate the total number of lines
//            //    m_albumCache.TotalNumberOfLines = (int)Math.Ceiling((double)m_items.Count / (double)m_albumCache.NumberOfIconsWidth);

//            //    // Calculate the total height (necessary for scrollbar)
//            //    m_albumCache.TotalHeight = m_albumCache.TotalNumberOfLines * m_albumCache.IconHeightWithPadding;

//            //    // Set scrollbar values
//            //    m_vScrollBar.Maximum = m_albumCache.TotalHeight;
//            //    m_vScrollBar.SmallChange = m_albumCache.IconHeightWithPadding / 10;
//            //    m_vScrollBar.LargeChange = m_albumCache.IconHeightWithPadding;
//            //}

//            //// Calculate how many lines must be skipped because of the scrollbar position
//            //int startLineNumber = (int)Math.Floor((double)m_vScrollBar.Value / (double)(m_iconSize + (m_padding * 2)));

//            //// Check if the total number of lines exceeds the number of icons fitting in height
//            //int numberOfLinesToDraw = 0;
//            //if (startLineNumber + m_albumCache.NumberOfIconsHeight > m_albumCache.TotalNumberOfLines)
//            //{
//            //    // There aren't enough lines to fill the screen
//            //    numberOfLinesToDraw = m_albumCache.TotalNumberOfLines - startLineNumber;
//            //}
//            //else
//            //{
//            //    // Fill up screen 
//            //    numberOfLinesToDraw = m_albumCache.NumberOfIconsHeight;
//            //}

//            //// Add one line for overflow; however, make sure we aren't adding a line without content 
//            //if (startLineNumber + numberOfLinesToDraw + 1 <= m_albumCache.TotalNumberOfLines)
//            //{
//            //    // Add one line for overflow
//            //    numberOfLinesToDraw++;
//            //}

//            //// Loop through icons (height)
//            //int iconCount = startLineNumber * m_albumCache.NumberOfIconsWidth;
//            //for (int a = startLineNumber; a < startLineNumber + numberOfLinesToDraw; a++)
//            //{
//            //    // Calculate the Y offset
//            //    int offsetY = (m_padding * 2 * a) + (m_iconSize * a);

//            //    // Calculate offset with scrollbar position
//            //    offsetY = offsetY - m_vScrollBar.Value;

//            //    // Loop through icons (width)
//            //    for (int b = 0; b < m_albumCache.NumberOfIconsWidth; b++)
//            //    {
//            //        // Get current item
//            //        AlbumViewItem item = (AlbumViewItem)m_items[iconCount];

//            //        // Extract image from audio file
//            //        Image imageAudioFile = null;

//            //        // Make sure the file exists
//            //        if (File.Exists(item.FilePath))
//            //        {
//            //            // Try to extract image from audio file
//            //            imageAudioFile = AudioFile.ExtractImageForAudioFile(item.FilePath);
//            //        }

//            //        // 2 times the padding per icon, plus 
//            //        int offsetX = (m_padding * 2 * b) + (m_iconSize * b);
//            //        Rectangle rectPositionAlbum = new Rectangle(offsetX, offsetY, m_iconSize + m_padding, m_iconSize + m_padding);

//            //        // Check if an image was found
//            //        if (imageAudioFile != null)
//            //        {
//            //            // Draw album cover
//            //            g.DrawImage(imageAudioFile, rectPositionAlbum);
//            //        }
//            //        else
//            //        {
//            //            // Draw default album cover
//            //            g.DrawRectangle(Pens.Gray, rectPositionAlbum);
//            //        }

//            //        // Draw caption
//            //        SizeF sizeString = g.MeasureString(item.Title, Font);
//            //        PointF pointTitle = new PointF(offsetX + ((m_iconSize - sizeString.Width) / 2), offsetY + m_iconSize - sizeString.Height);
//            //        g.FillRectangle(Brushes.Black, new RectangleF(pointTitle.X, pointTitle.Y, sizeString.Width, sizeString.Height));
//            //        g.DrawString(item.Title, font, Brushes.White, pointTitle);
//            //        iconCount++;
//            //    }
//            //}

//            //// Next: draw only the visible lines.

//            ////List<string> m_files = FileFinder.ByFileExtension(@"C:\MP3\The Beatles", new List<string>(){ "*.MP3" }, true);

//            ////foreach (string filePath in m_files)
//            ////{
//            ////    Image imageCover = AudioFile.GetImageForAudioFile(filePath);    
//            ////}

//            //// Since this mode displays a list of albums, we need only one song per album.
//            //// Usually people put one album per folder. However we still need to scan all files for distinct values.

//            ////int numberOfLines = 


//            ////g.FillRectangle(Brushes.White, new RectangleF(new PointF(0, 0), new SizeF((float)(Width / 2), (float)(Height / 2))));/
//        }

//        #endregion

//        /// <summary>
//        /// Creates a cache of values used for rendering the song grid view.
//        /// Also sets scrollbar position, height, value, maximum, etc.
//        /// </summary>
//        private void InvalidateAlbumCache()
//        {
//            //// Check if columns have been created
//            //if (m_columns == null || m_columns.Count == 0)
//            //{
//            //    return;
//            //}

//            //// Create cache
//            //m_songCache = new GridViewSongCache();

//            //// Get active columns and order them
//            //m_songCache.ActiveColumns = m_columns.Where(x => x.Order >= 0).OrderBy(x => x.Order).ToList();

//            //// Load custom font
//            //Font fontDefaultBold = Tools.LoadCustomFont(FontCollection, CustomFontName, m_fontSize, FontStyle.Bold);

//            //// Load default fonts if custom fonts were not found
//            //if (fontDefaultBold == null)
//            //{
//            //    // Load default font
//            //    fontDefaultBold = new Font(Font.FontFamily.Name, m_fontSize, FontStyle.Bold);
//            //}

//            //// Create temporary bitmap/graphics objects to measure a string (to determine line height)
//            //Bitmap bmpTemp = new Bitmap(200, 10);
//            //Graphics g = Graphics.FromImage(bmpTemp);
//            //SizeF sizeFont = g.MeasureString("QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm!@#$%^&*()", fontDefaultBold);
//            //g.Dispose();
//            //g = null;
//            //bmpTemp.Dispose();
//            //bmpTemp = null;

//            //// Calculate the line height (try to measure the total possible height of characters using the custom or default font)
//            //m_songCache.LineHeight = (int)sizeFont.Height + 6;
//            //m_songCache.TotalHeight = m_songCache.LineHeight * m_items.Count;

//            //// Check if the total active columns width exceed the width available in the control
//            //m_songCache.TotalWidth = 0;
//            //for (int a = 0; a < m_songCache.ActiveColumns.Count; a++)
//            //{
//            //    // Increment total width
//            //    m_songCache.TotalWidth += m_songCache.ActiveColumns[a].Width;
//            //}

//            //// Calculate the number of lines visible (count out the header, which is one line height)
//            //m_songCache.NumberOfLinesFittingInControl = (int)Math.Floor((double)(ClientRectangle.Height) / (double)(m_songCache.LineHeight));

//            //// Set vertical scrollbar dimensions
//            //m_vScrollBar.Top = m_songCache.LineHeight;
//            //m_vScrollBar.Left = ClientRectangle.Width - m_vScrollBar.Width;
//            //m_vScrollBar.Minimum = 0;

//            //// Scrollbar maximum is the number of lines fitting in the screen + the remaining line which might be cut
//            //// by the control height because it's not a multiple of line height (i.e. the last line is only partly visible)
//            //int lastLineHeight = ClientRectangle.Height - (m_songCache.LineHeight * m_songCache.NumberOfLinesFittingInControl);

//            //// Check width
//            //if (m_songCache.TotalWidth > ClientRectangle.Width - m_vScrollBar.Width)
//            //{
//            //    // Set scrollbar values
//            //    m_hScrollBar.Maximum = m_songCache.TotalWidth;
//            //    m_hScrollBar.SmallChange = 5;
//            //    m_hScrollBar.LargeChange = ClientRectangle.Width;

//            //    // Show scrollbar
//            //    m_hScrollBar.Visible = true;
//            //}

//            //// Check if the horizontal scrollbar needs to be turned off
//            //if (m_songCache.TotalWidth <= ClientRectangle.Width - m_vScrollBar.Width && m_hScrollBar.Visible)
//            //{
//            //    // Hide the horizontal scrollbar
//            //    m_hScrollBar.Visible = false;

//            //    //// TODO: Fix this
//            //    //if (m_vScrollBar.Value >= m_vScrollBar.Maximum - m_vScrollBar.LargeChange - 1)
//            //    //{
//            //    //    m_vScrollBar.Value = m_vScrollBar.Maximum - m_vScrollBar.LargeChange - 1 + m_songCache.ScrollBarOffsetY;
//            //    //}
//            //}

//            //// If there are less items than items fitting on screen...
//            ////if (m_songCache.NumberOfLinesFittingInControl - 1 >= m_items.Count)
//            //if (((m_songCache.NumberOfLinesFittingInControl - 1) * m_songCache.LineHeight) - m_hScrollBar.Height >= m_songCache.TotalHeight)
//            //{
//            //    // Disable the scrollbar
//            //    m_vScrollBar.Enabled = false;
//            //    m_vScrollBar.Value = 0;
//            //}
//            //else
//            //{
//            //    // Set scrollbar values
//            //    m_vScrollBar.Enabled = true;

//            //    // The real large change needs to be added to the LargeChange and Maximum property in order to work. 
//            //    int realLargeChange = m_songCache.LineHeight * 5;

//            //    // Calculate the vertical scrollbar maximum
//            //    int vMax = m_songCache.LineHeight * (m_items.Count - m_songCache.NumberOfLinesFittingInControl + 1) - lastLineHeight + realLargeChange;

//            //    // Add the horizontal scrollbar height if visible
//            //    if (m_hScrollBar.Visible)
//            //    {
//            //        // Add height
//            //        vMax += m_hScrollBar.Height;
//            //    }

//            //    // Compensate for the header, and for the last line which might be truncated by the control height
//            //    m_vScrollBar.Maximum = vMax;
//            //    m_vScrollBar.SmallChange = m_songCache.LineHeight;
//            //    m_vScrollBar.LargeChange = 1 + realLargeChange;
//            //}

//            ////// Check if the horizontal scrollbar is visible
//            ////if (m_hScrollBar.Visible)
//            ////{
//            ////    lastLineHeight -= m_hScrollBar.Height;
//            ////}


//            //// Calculate the scrollbar offset Y
//            //m_songCache.ScrollBarOffsetY = (m_startLineNumber * m_songCache.LineHeight) - m_vScrollBar.Value;

//            //// If both scrollbars need to be visible, the width and height must be changed
//            //if (m_hScrollBar.Visible && m_vScrollBar.Visible)
//            //{
//            //    // Cut 16 pixels
//            //    m_hScrollBar.Width = ClientRectangle.Width - 16;
//            //    m_vScrollBar.Height = ClientRectangle.Height - m_songCache.LineHeight - 16;
//            //}
//            //else
//            //{
//            //    m_vScrollBar.Height = ClientRectangle.Height - m_songCache.LineHeight;
//            //}
//        }

//        /// <summary>
//        /// Occurs when the control is resized.
//        /// Invalidates the cache.
//        /// </summary>
//        /// <param name="e">Event Arguments</param>
//        protected override void OnResize(EventArgs e)
//        {
//            base.OnResize(e);
//        }

//        #region Mouse Events

//        protected override void OnMouseEnter(EventArgs e)
//        {
//            // Set flags
//            m_isMouseOverControl = true;

//            base.OnMouseEnter(e);
//        }

//        protected override void OnMouseLeave(EventArgs e)
//        {
//            // Set flags            
//            m_isMouseOverControl = false;

//            base.OnMouseLeave(e);
//        }

//        protected override void OnMouseDown(MouseEventArgs e)
//        {
//            // Set flags
//            m_dragStartX = e.X;
            
//            base.OnMouseDown(e);
//        }

//        protected override void OnMouseUp(MouseEventArgs e)
//        {
//            // Reset flags
//            m_dragStartX = -1;

//            base.OnMouseUp(e);
//        }

//        protected override void OnMouseClick(MouseEventArgs e)
//        {
//            base.OnMouseClick(e);
//        }

//        protected override void OnMouseDoubleClick(MouseEventArgs e)
//        {
//            base.OnMouseDoubleClick(e);
//        }

//        protected override void OnMouseMove(MouseEventArgs e)
//        {
//            base.OnMouseMove(e);
//        }

//        public bool PreFilterMessage(ref Message m)
//        {
//            // Check message type
//            if (m.Msg == Win32.WM_MOUSEWHEEL)
//            {
//                // Get mouse wheel delta
//                int delta = Win32.GetWheelDeltaWParam((int)m.WParam);
//            }

//            return false;
//        }

//        #endregion
//    }
//}
