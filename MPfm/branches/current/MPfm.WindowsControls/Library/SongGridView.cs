//
// GridView.cs: This list view control is based on the System.Windows.Forms.ListView control.
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
using System.Data.Linq;
using System.Data.Objects;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using MPfm.Core;
using MPfm.Sound;
using MPfm.Library;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This grid control displays the MPfm library.
    /// </summary>
    public partial class SongGridView : System.Windows.Forms.Control, IMessageFilter
    {
        #region Private Variables

        // Controls
        private System.Windows.Forms.VScrollBar m_vScrollBar = null;
        private System.Windows.Forms.HScrollBar m_hScrollBar = null;

        // Background worker for updating album art
        private int m_preloadLinesAlbumCover = 20;
        private BackgroundWorker m_workerUpdateAlbumArt = null;
        public List<SongGridViewBackgroundWorkerArgument> m_workerUpdateAlbumArtPile = null;
        private System.Windows.Forms.Timer m_timerUpdateAlbumArt = null;
        private List<string> m_currentlyVisibleAlbumArt = null;

        // Cache        
        private GridViewSongCache m_songCache = null;
        private List<GridViewImageCache> m_imageCache = new List<GridViewImageCache>();        

        // Private variables used for mouse events
        public int m_startLineNumber = 0;
        public int m_numberOfLinesToDraw = 0;
        private int m_minimumColumnWidth = 30;
        private int m_dragStartX = -1;
        private int m_dragOriginalColumnWidth = -1;
        private bool m_isMouseOverControl = false;
        private bool m_isUserMovingColumn = false;
        private int m_lastItemIndexClicked = -1;

        // Animation timer and counter for currently playing song
        private int m_timerAnimationNowPlayingCount = 0;
        private Rectangle m_rectNowPlaying = new Rectangle(0, 0, 1, 1);
        private System.Windows.Forms.Timer m_timerAnimationNowPlaying = null;

        #endregion

        #region Theme Properties

        #region Header

        /// <summary>
        /// Private value for the HeaderColor1 property.
        /// </summary>
        private Color m_headerColor1 = Color.FromArgb(165, 165, 165);
        /// <summary>
        /// First color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("First color of the header background gradient.")]
        public Color HeaderColor1
        {
            get
            {
                return m_headerColor1;
            }
            set
            {
                m_headerColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the HeaderColor2 property.
        /// </summary>
        private Color m_headerColor2 = Color.FromArgb(195, 195, 195);
        /// <summary>
        /// Second color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Second color of the header background gradient.")]
        public Color HeaderColor2
        {
            get
            {
                return m_headerColor2;
            }
            set
            {
                m_headerColor2 = value;
            }
        }

        /// <summary>
        /// Private value for the HeaderHoverColor1 property.
        /// </summary>
        private Color m_headerHoverColor1 = Color.FromArgb(145, 145, 145);
        /// <summary>
        /// First color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("First color of the header background gradient when the mouse cursor is over the header.")]
        public Color HeaderHoverColor1
        {
            get
            {
                return m_headerHoverColor1;
            }
            set
            {
                m_headerHoverColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the HeaderHoverColor2 property.
        /// </summary>
        private Color m_headerHoverColor2 = Color.FromArgb(175, 175, 175);
        /// <summary>
        /// Second color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("First color of the header background gradient when the mouse cursor is over the header.")]
        public Color HeaderHoverColor2
        {
            get
            {
                return m_headerHoverColor2;
            }
            set
            {
                m_headerHoverColor2 = value;
            }
        }

        /// <summary>
        /// Private value for the HeaderForeColor property.
        /// </summary>
        private Color m_headerForeColor = Color.FromArgb(60, 60, 60);
        /// <summary>
        /// Fore font color used in the header.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("")]
        [Category("Theme"), Browsable(true), Description("Fore color used when drawing the header font and other glyphs (such as the orderby icon).")]
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

        /// <summary>
        /// Private value for the HeaderCustomFontName property.
        /// </summary>
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

        #endregion

        #region Line

        /// <summary>
        /// Private value for the LineColor1 property.
        /// </summary>
        private Color m_lineColor1 = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// First color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("First color of the line background gradient.")]
        public Color LineColor1
        {
            get
            {
                return m_lineColor1;
            }
            set
            {
                m_lineColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the LineColor2 property.
        /// </summary>
        private Color m_lineColor2 = Color.FromArgb(235, 235, 235);
        /// <summary>
        /// Second color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Second color of the line background gradient.")]
        public Color LineColor2
        {
            get
            {
                return m_lineColor2;
            }
            set
            {
                m_lineColor2 = value;
            }
        }

        /// <summary>
        /// Private value for the LineHoverColor1 property.
        /// </summary>
        private Color m_lineHoverColor1 = Color.FromArgb(235, 235, 235);
        /// <summary>
        /// First color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("First color of the line background gradient when the mouse cursor is over the line.")]
        public Color LineHoverColor1
        {
            get
            {
                return m_lineHoverColor1;
            }
            set
            {
                m_lineHoverColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the LineHoverColor2 property.
        /// </summary>
        private Color m_lineHoverColor2 = Color.FromArgb(255, 255, 255);
        /// <summary>
        /// Second color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Second color of the line background gradient when the mouse cursor is over the line.")]
        public Color LineHoverColor2
        {
            get
            {
                return m_lineHoverColor2;
            }
            set
            {
                m_lineHoverColor2 = value;
            }
        }

        /// <summary>
        /// Private value for the LineSelectedColor1 property.
        /// </summary>
        private Color m_lineSelectedColor1 = Color.FromArgb(165, 165, 165);
        /// <summary>
        /// First color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("First color of the line background gradient when the line is selected.")]
        public Color LineSelectedColor1
        {
            get
            {
                return m_lineSelectedColor1;
            }
            set
            {
                m_lineSelectedColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the LineSelectedColor2 property.
        /// </summary>
        private Color m_lineSelectedColor2 = Color.FromArgb(185, 185, 185);
        /// <summary>
        /// Second color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Second color of the line background gradient when the line is selected.")]
        public Color LineSelectedColor2
        {
            get
            {
                return m_lineSelectedColor2;
            }
            set
            {
                m_lineSelectedColor2 = value;
            }
        }

        /// <summary>
        /// Private value for the LineNowPlayingColor1 property.
        /// </summary>
        private Color m_lineNowPlayingColor1 = Color.FromArgb(135, 235, 135);
        /// <summary>
        /// First color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("First color of the line background gradient when the line is now playing.")]
        public Color LineNowPlayingColor1
        {
            get
            {
                return m_lineNowPlayingColor1;
            }
            set
            {
                m_lineNowPlayingColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the LineNowPlayingColor2 property.
        /// </summary>
        private Color m_lineNowPlayingColor2 = Color.FromArgb(155, 255, 155);
        /// <summary>
        /// Second color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Second color of the line background gradient when the line is now playing.")]
        public Color LineNowPlayingColor2
        {
            get
            {
                return m_lineNowPlayingColor2;
            }
            set
            {
                m_lineNowPlayingColor2 = value;
            }
        }

        /// <summary>
        /// Private value for the LineForeColor property.
        /// </summary>
        private Color m_lineForeColor = Color.FromArgb(0, 0, 0);
        /// <summary>
        /// Fore font color used in the header.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("")]
        [Category("Theme"), Browsable(true), Description("Fore color used when drawing the line font.")]
        public Color LineForeColor
        {
            get
            {
                return m_lineForeColor;
            }
            set
            {
                m_lineForeColor = value;
            }
        }

        /// <summary>
        /// Private value for the IconNowPlayingColor1 property.
        /// </summary>
        private Color m_iconNowPlayingColor1 = Color.FromArgb(250, 200, 250);
        /// <summary>
        /// First color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("First color of the animated icon displaying the currently playing song.")]
        public Color IconNowPlayingColor1
        {
            get
            {
                return m_iconNowPlayingColor1;
            }
            set
            {
                m_iconNowPlayingColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the IconNowPlayingColor2 property.
        /// </summary>
        private Color m_iconNowPlayingColor2 = Color.FromArgb(25, 150, 25);
        /// <summary>
        /// Second color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Second color of the animated icon displaying the currently playing song.")]
        public Color IconNowPlayingColor2
        {
            get
            {
                return m_iconNowPlayingColor2;
            }
            set
            {
                m_iconNowPlayingColor2 = value;
            }
        }

        #endregion

        #region Album Covers

        /// <summary>
        /// Private value for the AlbumCoverBackgroundColor1 property.
        /// </summary>
        private Color m_albumCoverBackgroundColor1 = Color.FromArgb(55, 55, 55);
        /// <summary>
        /// First color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("First color of the album cover background gradient.")]
        public Color AlbumCoverBackgroundColor1
        {
            get
            {
                return m_albumCoverBackgroundColor1;
            }
            set
            {
                m_albumCoverBackgroundColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the AlbumCoverBackgroundColor2 property.
        /// </summary>
        private Color m_albumCoverBackgroundColor2 = Color.FromArgb(75, 75, 75);
        /// <summary>
        /// Second color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Second color of the album cover background gradient.")]
        public Color AlbumCoverBackgroundColor2
        {
            get
            {
                return m_albumCoverBackgroundColor2;
            }
            set
            {
                m_albumCoverBackgroundColor2 = value;
            }
        }

        #endregion

        /// <summary>
        /// Private value for the Padding property.
        /// </summary>
        private int m_padding = 6;
        /// <summary>
        /// Padding used around text and album covers (in pixels).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Padding used around text and album covers (in pixels).")]        
        public int Padding
        {
            get
            {
                return m_padding;
            }
            set
            {
                m_padding = value;
                m_songCache = null;
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

        /// <summary>
        /// Private value for the AntiAliasingEnabled property.
        /// </summary>
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

        /// <summary>
        /// Overrides the Font property to invalidate the cache when updating the font.
        /// </summary>
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
                m_songCache = null;
            }
        }

        #endregion

        #region Other Properties (Library, Items, Columns, etc.)
        
        /// <summary>
        /// Private value for the Library property.
        /// </summary>
        private MPfm.Library.Library m_library = null;
        /// <summary>
        /// Hook to the MPfm Library object.
        /// </summary>
        public MPfm.Library.Library Library
        {
            get
            {
                return m_library;
            }
            set
            {
                m_library = value;
            }
        }

        /// <summary>
        /// Private value for the Items property.
        /// </summary>
        private List<SongGridViewItem> m_items = null;
        /// <summary>
        /// List of grid view items (representing songs).
        /// </summary>
        public List<SongGridViewItem> Items
        {
            get
            {
                return m_items;
            }
        }

        /// <summary>
        /// Private value for the Columns property.
        /// </summary>
        private List<GridViewSongColumn> m_columns = null;
        /// <summary>
        /// List of grid view columns.
        /// </summary>
        public List<GridViewSongColumn> Columns
        {
            get
            {
                return m_columns;
            }
        }

        #endregion

        #region Filter / OrderBy Properties

        private string m_searchArtistName = string.Empty;
        public string SearchArtistName
        {
            get
            {
                return m_searchArtistName;
            }
            set
            {
                m_searchArtistName = value;

                // Invalidate item list and cache
                m_items = null;
                m_songCache = null;

                // Refresh control
                Refresh();
            }
        }

        /// <summary>
        /// Private value for the OrderByFieldName property.
        /// </summary>
        private string m_orderByFieldName = string.Empty;
        public string OrderByFieldName
        {
            get
            {
                return m_orderByFieldName;
            }
            set
            {
                m_orderByFieldName = value;

                // Invalidate item list and cache
                m_items = null;
                m_songCache = null;

                // Refresh whole control
                Refresh();
            }
        }

        private bool m_orderByAscending = true;
        public bool OrderByAscending
        {
            get
            {
                return m_orderByAscending;
            }
            set
            {
                m_orderByAscending = value;
            }
        }

        #endregion

        #region Settings Properties
        
        private Guid m_nowPlayingSongId = Guid.Empty;
        public Guid NowPlayingSongId
        {
            get
            {
                return m_nowPlayingSongId;
            }
            set
            {
                m_nowPlayingSongId = value;
            }
        }

        private bool m_displayDebugInformation = false;
        public bool DisplayDebugInformation
        {
            get
            {
                return m_displayDebugInformation;
            }
            set
            {
                m_displayDebugInformation = value;
            }
        }

        // Must replace to a list to do pushing                
        private int m_imageCacheSize = 10;
        public int ImageCacheSize
        {
            get
            {
                return m_imageCacheSize;
            }
            set
            {
                m_imageCacheSize = value;
            }
        }

        #endregion

        #region Constructors
        
        /// <summary>
        /// Default constructor for SongGridView.
        /// </summary>
        public SongGridView()
        {
            // Set default styles
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw |
                       ControlStyles.Opaque | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);

            // Create timer for animation
            m_timerAnimationNowPlaying = new System.Windows.Forms.Timer();
            m_timerAnimationNowPlaying.Interval = 50;
            m_timerAnimationNowPlaying.Tick += new EventHandler(m_timerAnimationNowPlaying_Tick);
            m_timerAnimationNowPlaying.Enabled = true;

            // Create vertical scrollbar
            m_vScrollBar = new System.Windows.Forms.VScrollBar();
            m_vScrollBar.Width = 16;
            m_vScrollBar.Scroll += new ScrollEventHandler(m_vScrollBar_Scroll);
            Controls.Add(m_vScrollBar);

            // Create horizontal scrollbar
            m_hScrollBar = new System.Windows.Forms.HScrollBar();
            m_hScrollBar.Width = ClientRectangle.Width;
            m_hScrollBar.Height = 16;
            m_hScrollBar.Top = ClientRectangle.Height - m_hScrollBar.Height;
            m_hScrollBar.Scroll += new ScrollEventHandler(m_hScrollBar_Scroll);
            Controls.Add(m_hScrollBar);

            // Override mouse messages for mouse wheel (get mouse wheel events out of control)
            Application.AddMessageFilter(this);

            // Create background worker for updating album art
            m_workerUpdateAlbumArtPile = new List<SongGridViewBackgroundWorkerArgument>();
            m_workerUpdateAlbumArt = new BackgroundWorker();            
            m_workerUpdateAlbumArt.DoWork += new DoWorkEventHandler(m_workerUpdateAlbumArt_DoWork);
            m_workerUpdateAlbumArt.RunWorkerCompleted += new RunWorkerCompletedEventHandler(m_workerUpdateAlbumArt_RunWorkerCompleted);
            
            // Create timer for updating album art
            m_timerUpdateAlbumArt = new System.Windows.Forms.Timer();
            m_timerUpdateAlbumArt.Interval = 10;
            m_timerUpdateAlbumArt.Tick += new EventHandler(m_timerUpdateAlbumArt_Tick);
            m_timerUpdateAlbumArt.Enabled = true;
        }

        /// <summary>
        /// Occurs when the Update Album Art timer expires.
        /// Checks if there are more album art covers to load and starts the background
        /// thread if it is not running already.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        public void m_timerUpdateAlbumArt_Tick(object sender, EventArgs e)
        {
            // Stop timer
            m_timerUpdateAlbumArt.Enabled = false;

            // Check for the next album art to fetch
            if (m_workerUpdateAlbumArtPile.Count > 0 && !m_workerUpdateAlbumArt.IsBusy)
            {
                // Do some cleanup: clean items that are not visible anymore
                bool cleanUpDone = false;
                while (!cleanUpDone)
                {
                    int indexToDelete = -1;
                    for (int a = 0; a < m_workerUpdateAlbumArtPile.Count; a++)
                    {
                        // Get argument
                        SongGridViewBackgroundWorkerArgument arg = m_workerUpdateAlbumArtPile[a];

                        // Check if this album is still visible (cancel if it is out of display).     
                        if (arg.LineIndex < m_startLineNumber || arg.LineIndex > m_startLineNumber + m_numberOfLinesToDraw + m_preloadLinesAlbumCover)
                        {
                            indexToDelete = a;
                            break;
                        }
                    }

                    if (indexToDelete >= 0)
                    {
                        m_workerUpdateAlbumArtPile.RemoveAt(indexToDelete);                        
                    }
                    else
                    {
                        cleanUpDone = true;
                    }
                }
                // There must be more album art to fetch.. right?
                if (m_workerUpdateAlbumArtPile.Count > 0)
                {
                    // Start background worker                
                    SongGridViewBackgroundWorkerArgument arg = m_workerUpdateAlbumArtPile[0];
                    m_workerUpdateAlbumArt.RunWorkerAsync(arg);
                }
            }

            // Restart timer
            m_timerUpdateAlbumArt.Enabled = true;
        }

        /// <summary>
        /// Occurs when the Update Album Art background worker starts its work.
        /// Fetches the album cover in another thread and returns the image once done.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        public void m_workerUpdateAlbumArt_DoWork(object sender, DoWorkEventArgs e)
        {
            // Make sure the argument is valid
            if (e.Argument == null)
            {
                return;
            }

            // Cast argument
            SongGridViewBackgroundWorkerArgument arg = (SongGridViewBackgroundWorkerArgument)e.Argument;

            // Create result
            SongGridViewBackgroundWorkerResult result = new SongGridViewBackgroundWorkerResult();
            result.Song = arg.Song;

            // Check if this album is still visible (cancel if it is out of display).     
            if (arg.LineIndex < m_startLineNumber || arg.LineIndex > m_startLineNumber + m_numberOfLinesToDraw + m_preloadLinesAlbumCover)
            {
                // Set result with empty image
                e.Result = result;
                return;
            }

            // Extract image from file
            Image imageAlbumCover = AudioFile.ExtractImageForAudioFile(arg.Song.FilePath);

            // Set album art in return data            
            result.AlbumArt = imageAlbumCover;
            e.Result = result;
        }

        /// <summary>
        /// Occurs when the Update Album Art background worker has finished its work.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        public void m_workerUpdateAlbumArt_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Check if the result was OK
            if (e.Result == null)
            {
                return;
            }

            // Get album art
            SongGridViewBackgroundWorkerResult result = (SongGridViewBackgroundWorkerResult)e.Result;

            // Check if an image was found
            if (result.AlbumArt != null)
            {
                // We found cover art! Add to cache and get out of the loop
                m_imageCache.Add(new GridViewImageCache() { Key = result.Song.ArtistName + "_" + result.Song.AlbumTitle, Image = result.AlbumArt });

                // Check if the cache size has been reached
                if (m_imageCache.Count > m_imageCacheSize)
                {
                    // Remove the oldest item
                    Image imageTemp = m_imageCache[0].Image;
                    imageTemp.Dispose();
                    imageTemp = null;
                    m_imageCache.RemoveAt(0);
                }
            }

            // Remove song from list
            int indexRemove = -1;
            for (int a = 0; a < m_workerUpdateAlbumArtPile.Count; a++)
            {
                if (m_workerUpdateAlbumArtPile[a].Song.FilePath.ToUpper() == result.Song.FilePath.ToUpper())
                {
                    indexRemove = a;
                }
            }
            if (indexRemove >= 0)
            {
                m_workerUpdateAlbumArtPile.RemoveAt(indexRemove);
            }            

            // Refresh control (TODO: Invalidate instead)
            Refresh();
            //Invalidate(new Rectangle(0, 0, m_columns[0].Width, Height));  <== this cuts the line between the album art and the other columns

            //// Remove all invisible items from list
            ////for (int a = 0; a < m_workerUpdateAlbumArtPile.Count; a++)
            //foreach(SongGridViewBackgroundWorkerArgument arg in m_workerUpdateAlbumArtPile)
            //{
            //    // TODO: Check if this album is still visible (cancel if it is out of display).     
            //    if (arg.LineIndex < m_startLineNumber || arg.LineIndex > m_startLineNumber + m_numberOfLinesToDraw)
            //    {
            //        m_workerUpdateAlbumArtPile.Remove(arg);
            //    }
            //}
        }

        #endregion

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
        /// Occurs when the control needs to be painted.
        /// </summary>
        /// <param name="pe">Paint Event Arguments</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Check if the library is valid
            if (m_library == null)
            {
                return;
            }

            // Draw bitmap for control
            //Bitmap bmp = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
            //Graphics g = Graphics.FromImage(bmp);
            Graphics g = e.Graphics;

            // Set text anti-aliasing to ClearType (best looking AA)
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Set smoothing mode for paths
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Paint songs
            PaintSongs(ref g);

            //g.DrawString(m_count.ToString(), Font, Brushes.Blue, new Point(20, 20));

            //SolidBrush brush = new SolidBrush(Color.FromArgb(155, 255, 0, 0));
            //g.FillRectangle(brush, g.ClipBounds);
            //brush.Dispose();
            //brush = null;

            // Draw bitmap on control
            //e.Graphics.DrawImage(bmp, 0, 0, ClientRectangle, GraphicsUnit.Pixel);
            //bmp.Dispose();
            //bmp = null;
            //g.Dispose();
            //g = null;               

            base.OnPaint(e);
        }

        /// <summary>
        /// Paints a grid view displaying a list of songs.
        /// </summary>
        /// <param name="g">Graphics object to write to</param>
        public void PaintSongs(ref Graphics g)
        {
            // Declare variables
            Rectangle rect = new Rectangle();
            RectangleF rectF = new RectangleF();
            Pen pen = null;
            SolidBrush brush = null;
            LinearGradientBrush brushGradient = null;
            int offsetX = 0;
            int offsetY = 0;
            int albumCoverStartIndex = 0;
            int albumCoverEndIndex = 0;
            string currentAlbumTitle = string.Empty;
            bool regenerateItems = true;
            bool nowPlayingSongFound = false;

            // Load custom font
            Font fontDefault = Tools.LoadCustomFont(FontCollection, CustomFontName, Font.Size, FontStyle.Regular);
            Font fontDefaultBold = Tools.LoadCustomFont(FontCollection, CustomFontName, Font.Size, FontStyle.Bold);

            // Load default fonts if custom fonts were not found
            if (fontDefault == null)
            {
                // Load default font
                fontDefault = new Font(Font.FontFamily.Name, Font.Size, FontStyle.Regular);
            }
            if (fontDefaultBold == null)
            {
                // Load default font
                fontDefaultBold = new Font(Font.FontFamily.Name, Font.Size, FontStyle.Bold);
            }

            // Set string format
            StringFormat stringFormat = new StringFormat();

            // Check for an existing collection of items
            if (m_items != null)
            {
                // Check the first item, if there's one
                if (m_items.Count > 0 && m_items[0] is SongGridViewItem)
                {
                    regenerateItems = false;
                }
            }

            // Check if columns exist
            if (m_columns == null)
            {
                // Create columns
                GridViewSongColumn columnSongAlbumCover = new GridViewSongColumn(string.Empty, string.Empty, true, 0);
                GridViewSongColumn columnSongNowPlaying = new GridViewSongColumn(string.Empty, string.Empty, true, 1);
                GridViewSongColumn columnSongTrackNumber = new GridViewSongColumn("Tr#", "TrackNumber", true, 2);
                GridViewSongColumn columnSongTitle = new GridViewSongColumn("Song Title", "Title", true, 3);
                GridViewSongColumn columnSongLength = new GridViewSongColumn("Length", "Time", true, 4);
                GridViewSongColumn columnSongArtistName = new GridViewSongColumn("Artist Name", "ArtistName", true, 5);
                GridViewSongColumn columnSongAlbumTitle = new GridViewSongColumn("Album Title", "AlbumTitle", true, 6);
                GridViewSongColumn columnSongPlayCount = new GridViewSongColumn("Play Count", "PlayCount", true, 7);
                GridViewSongColumn columnSongLastPlayed = new GridViewSongColumn("Last Played", "LastPlayed", true, 8);

                // Set additional flags
                columnSongAlbumCover.CanBeReordered = false;
                columnSongNowPlaying.CanBeReordered = false;
                columnSongNowPlaying.CanBeResized = false;

                // Set default widths
                columnSongAlbumCover.Width = 200;
                columnSongNowPlaying.Width = 20;
                columnSongTrackNumber.Width = 30;
                columnSongTitle.Width = 200;
                columnSongLength.Width = 70;
                columnSongArtistName.Width = 140;
                columnSongAlbumTitle.Width = 140;
                columnSongPlayCount.Width = 50;
                columnSongPlayCount.Width = 80;

                // Add columns to list
                m_columns = new List<GridViewSongColumn>();
                m_columns.Add(columnSongAlbumCover);
                m_columns.Add(columnSongNowPlaying);
                m_columns.Add(columnSongTrackNumber);
                m_columns.Add(columnSongTitle);
                m_columns.Add(columnSongLength);
                m_columns.Add(columnSongArtistName);
                m_columns.Add(columnSongAlbumTitle);
                m_columns.Add(columnSongPlayCount);
                m_columns.Add(columnSongLastPlayed);
            }

            // Check if the items have been generated, or that the items are not of album type
            if (regenerateItems)
            {
                // Query how many albums there are in the library
                //List<SongDTO> songs = m_library.SelectSongs(FilterSoundFormat.MP3, m_searchArtistName);
                List<SongDTO> songs = ConvertDTO.ConvertSongs(DataAccess.SelectSongs(m_searchArtistName, string.Empty, string.Empty, m_orderByFieldName, m_orderByAscending));

                // Create list of items
                m_items = new List<SongGridViewItem>();
                foreach (SongDTO song in songs)
                {
                    // Create item and add to list
                    SongGridViewItem item = new SongGridViewItem();
                    item.Song = song;
                    m_items.Add(item);
                }

                // Reset scrollbar position
                m_vScrollBar.Value = 0;
            }

            // Check if a cache exists, or if the cache needs to be refreshed
            if (m_songCache == null)
            {
                // Create song cache
                InvalidateSongCache();
            }

            // Calculate how many lines must be skipped because of the scrollbar position
            m_startLineNumber = (int)Math.Floor((double)m_vScrollBar.Value / (double)(m_songCache.LineHeight));

            // Check if the total number of lines exceeds the number of icons fitting in height
            m_numberOfLinesToDraw = 0;
            if (m_startLineNumber + m_songCache.NumberOfLinesFittingInControl > m_items.Count)
            {
                // There aren't enough lines to fill the screen
                m_numberOfLinesToDraw = m_items.Count - m_startLineNumber;
            }
            else
            {
                // Fill up screen 
                m_numberOfLinesToDraw = m_songCache.NumberOfLinesFittingInControl;
            }

            // Add one line for overflow; however, make sure we aren't adding a line without content 
            if (m_startLineNumber + m_numberOfLinesToDraw + 1 <= m_items.Count)
            {
                // Add one line for overflow
                m_numberOfLinesToDraw++;
            }

            // Loop through lines
            for (int a = m_startLineNumber; a < m_startLineNumber + m_numberOfLinesToDraw; a++)
            {
                // Get song
                //SongDTO song = ((GridViewSongItem)m_items[a]).Song;
                SongDTO song = m_items[a].Song;

                // Calculate Y offset (compensate for scrollbar position)
                offsetY = (a * m_songCache.LineHeight) - m_vScrollBar.Value + m_songCache.LineHeight;

                // Draw line background (determine if the mouse is over the current item)

                // Calculate line background width
                int lineBackgroundWidth = ClientRectangle.Width + m_hScrollBar.Value - m_columns[0].Width;

                // Reduce width from scrollbar if visible
                if (m_vScrollBar.Visible)
                {
                    lineBackgroundWidth -= m_vScrollBar.Width;
                }

                // Set rectangle
                Rectangle rectBackground = new Rectangle(m_columns[0].Width - m_hScrollBar.Value, offsetY, lineBackgroundWidth, m_songCache.LineHeight);
                if (song.SongId == m_nowPlayingSongId)
                {
                    // Now playing color
                    brushGradient = new LinearGradientBrush(rectBackground, LineNowPlayingColor1, LineNowPlayingColor2, 90);
                }
                else if (m_items[a].IsSelected)
                {
                    // Selected color
                    brushGradient = new LinearGradientBrush(rectBackground, LineSelectedColor1, LineSelectedColor2, 90);
                }
                else if (m_items[a].IsMouseOverItem)
                {
                    // Mouse over color
                    brushGradient = new LinearGradientBrush(rectBackground, LineHoverColor1, LineHoverColor2, 90);
                }
                else
                {
                    // Default color
                    brushGradient = new LinearGradientBrush(rectBackground, LineColor1, LineColor2, 90);
                }
                g.FillRectangle(brushGradient, rectBackground);
                brushGradient.Dispose();
                brushGradient = null;

                #region Album Cover Zone

                // Check for an album title change (or the last item of the grid)
                if (currentAlbumTitle != song.AlbumTitle)
                {
                    // Set the new current album title
                    currentAlbumTitle = song.AlbumTitle;

                    // For displaying the album cover, we need to know how many songs of the same album are bundled together
                    // Start by getting the start index
                    for (int b = a; b > 0; b--)
                    {
                        // Get song
                        //SongDTO previousSong = ((GridViewSongItem)m_items[b]).Song;
                        SongDTO previousSong = m_items[b].Song;

                        // Check if the album title matches
                        if (previousSong.AlbumTitle != song.AlbumTitle)
                        {
                            // Set album cover start index (+1 because the last song was the sound found in the previous loop iteration)
                            albumCoverStartIndex = b + 1;
                            break;
                        }
                    }
                    // Find the end index
                    for (int b = a + 1; b < m_items.Count; b++)
                    {
                        // Get song
                        //SongDTO nextSong = ((GridViewSongItem)m_items[b]).Song;
                        SongDTO nextSong = m_items[b].Song;

                        // If the album title is different, this means we found the next album title
                        if (nextSong.AlbumTitle != song.AlbumTitle)
                        {
                            // Set album cover end index (-1 because the last song was the song found in the previous loop iteration)
                            albumCoverEndIndex = b - 1;
                            break;
                        }
                        // If this is the last item of the grid...
                        else if (b == m_items.Count - 1)
                        {
                            // Set album cover end index as the last item of the grid
                            albumCoverEndIndex = b;
                            break;
                        }
                    }

                    // Calculate y and height
                    int scrollbarOffsetY = (m_startLineNumber * m_songCache.LineHeight) - m_vScrollBar.Value;
                    int y = ((albumCoverStartIndex - m_startLineNumber) * m_songCache.LineHeight) + m_songCache.LineHeight + scrollbarOffsetY;

                    //int height = (albumCoverEndIndex - albumCoverStartIndex) * m_songCache.LineHeight;

                    // Calculate the height of the album cover zone (+1 on end index because the array is zero-based)
                    int albumCoverZoneHeight = (albumCoverEndIndex + 1 - albumCoverStartIndex) * m_songCache.LineHeight;

                    int heightWithPadding = albumCoverZoneHeight - (m_padding * 2);
                    if (heightWithPadding > m_songCache.ActiveColumns[0].Width - (m_padding * 2))
                    {
                        heightWithPadding = m_songCache.ActiveColumns[0].Width - (m_padding * 2);
                    }

                    // Make sure the height is at least zero (not necessary to draw anything!)
                    if (albumCoverZoneHeight > 0)
                    {
                        // Draw album cover background
                        Rectangle rectAlbumCover = new Rectangle(0 - m_hScrollBar.Value, y, m_songCache.ActiveColumns[0].Width, albumCoverZoneHeight);
                        brushGradient = new LinearGradientBrush(rectAlbumCover, AlbumCoverBackgroundColor1, AlbumCoverBackgroundColor2, 90);
                        g.FillRectangle(brushGradient, rectAlbumCover);
                        brushGradient.Dispose();
                        brushGradient = null;

                        // Try to extract image from cache
                        Image imageAlbumCover = null;
                        GridViewImageCache cachedImage = m_imageCache.FirstOrDefault(x => x.Key == song.ArtistName + "_" + song.AlbumTitle);
                        if (cachedImage != null)
                        {
                            // Set image
                            imageAlbumCover = cachedImage.Image;
                        }

                        //m_imageCache.TryGetValue(song.ArtistName + "_" + song.AlbumTitle, out imageAlbumCover);

                        //// Check if the image file exists                            
                        //string imageFileDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Cover Art Files\\";
                        //string imageFilePath = imageFileDirectory + song.FilePath.Replace(@"\", "_").Replace(":", "_").Replace(".", "_") + ".bmp";
                        //if (File.Exists(imageFilePath))
                        //{
                        //    try
                        //    {
                        //        imageAlbumCover = Image.FromFile(imageFilePath);
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        throw ex;
                        //    }
                        //}

                        // Album art not found in cache; try to find an album cover in one of the file
                        if (imageAlbumCover == null)
                        {
                            // Check if the album cover is already in the pile
                            bool albumCoverFound = false;
                            foreach (SongGridViewBackgroundWorkerArgument arg in m_workerUpdateAlbumArtPile)
                            {
                                // Match by file path
                                if (arg.Song.FilePath.ToUpper() == song.FilePath.ToUpper())
                                {
                                    // We found the album cover
                                    albumCoverFound = true;
                                }
                            }

                            // Add to the pile only if the album cover isn't already in it
                            if (!albumCoverFound)
                            {
                                // Add item to update album art worker pile
                                SongGridViewBackgroundWorkerArgument arg = new SongGridViewBackgroundWorkerArgument();
                                arg.Song = song;
                                arg.LineIndex = a;
                                m_workerUpdateAlbumArtPile.Add(arg);
                            }


                            //// Loop through album files
                            //for (int d = albumCoverStartIndex; d < albumCoverEndIndex; d++)
                            //{                                                                


                            //    // Make sure the song isn't already in the pile
                            //    if (!m_workerUpdateAlbumArtPile.Contains(song))
                            //    {
                            //        // Add song to update album art worker pile
                            //        m_workerUpdateAlbumArtPile.Add(song);
                            //    }

                            //    //// Check if this is the first item of the pile
                            //    //if (m_workerUpdateAlbumArtPile.Count == 1)
                            //    //{
                            //    //    // Start background worker
                            //    //    m_workerUpdateAlbumArt.RunWorkerAsync(m_workerUpdateAlbumArtPile[0]);
                            //    //}

                            //    break;

                            //    //// Extract image from file
                            //    //imageAlbumCover = AudioFile.ExtractImageForAudioFile(song.FilePath);

                            //    //// Check if an image was found
                            //    //if (imageAlbumCover != null)
                            //    //{
                            //    //    // We found cover art! Add to cache and get out of the loop
                            //    //    m_imageCache.Add(new GridViewImageCache() { Key = song.ArtistName + "_" + song.AlbumTitle, Image = imageAlbumCover });

                            //    //    // Check if the cache size has been reached
                            //    //    if (m_imageCache.Count > m_imageCacheSize)
                            //    //    {
                            //    //        // Remove the oldest item
                            //    //        Image imageTemp = m_imageCache[0].Image;
                            //    //        imageTemp.Dispose();
                            //    //        imageTemp = null;
                            //    //        m_imageCache.RemoveAt(0);
                            //    //    }

                            //    //    //try
                            //    //    //{
                            //    //    //    // Try to save the album cover art
                            //    //    //    imageAlbumCover.Save(imageFilePath);
                            //    //    //}
                            //    //    //catch
                            //    //    //{
                            //    //    //    // Check if the cover art directory exists
                            //    //    //    if (!Directory.Exists(imageFileDirectory))
                            //    //    //    {
                            //    //    //        // Create the directory and try again
                            //    //    //        Directory.CreateDirectory(imageFileDirectory);
                            //    //    //        imageAlbumCover.Save(imageFilePath);
                            //    //    //    }
                            //    //    //}

                            //    //    break;
                            //    //}
                            //}

                        }

                        // Measure available width for text
                        int widthAvailableForText = m_columns[0].Width - (m_padding * 2);

                        // Display titles depending on if an album art was found
                        RectangleF rectAlbumCoverArt = new RectangleF();
                        RectangleF rectAlbumTitleText = new RectangleF();
                        RectangleF rectArtistNameText = new RectangleF();
                        SizeF sizeAlbumTitle = new SizeF();
                        SizeF sizeArtistName = new SizeF();
                        bool useAlbumArtOverlay = false;

                        // If there's only one line, we need to do place the artist name and album title on one line
                        if (albumCoverEndIndex - albumCoverStartIndex == 0)
                        {
                            // Set string format
                            stringFormat.Alignment = StringAlignment.Near;
                            stringFormat.Trimming = StringTrimming.EllipsisCharacter;

                            // Measure strings
                            sizeArtistName = g.MeasureString(song.ArtistName, fontDefaultBold, widthAvailableForText, stringFormat);
                            sizeAlbumTitle = g.MeasureString(currentAlbumTitle, fontDefault, widthAvailableForText - (int)sizeArtistName.Width, stringFormat);

                            // Display artist name at full width first, then album name
                            rectArtistNameText = new RectangleF(m_padding - m_hScrollBar.Value, y + (m_padding / 2), widthAvailableForText, m_songCache.LineHeight);
                            rectAlbumTitleText = new RectangleF(m_padding - m_hScrollBar.Value + sizeArtistName.Width + m_padding, y + (m_padding / 2), widthAvailableForText - sizeArtistName.Width, m_songCache.LineHeight);
                        }
                        else
                        {
                            // There are at least two lines; is there an album cover to display?
                            if (imageAlbumCover == null)
                            {
                                // There is no album cover to display; display only text.
                                // Set string format
                                stringFormat.Alignment = StringAlignment.Center;
                                stringFormat.Trimming = StringTrimming.EllipsisWord;

                                // Measure strings
                                sizeArtistName = g.MeasureString(song.ArtistName, fontDefaultBold, widthAvailableForText, stringFormat);
                                sizeAlbumTitle = g.MeasureString(currentAlbumTitle, fontDefault, widthAvailableForText, stringFormat);

                                // Display the album title at the top of the zome
                                rectArtistNameText = new RectangleF(m_padding - m_hScrollBar.Value, y + m_padding, widthAvailableForText, heightWithPadding);
                                rectAlbumTitleText = new RectangleF(m_padding - m_hScrollBar.Value, y + m_padding + sizeArtistName.Height, widthAvailableForText, heightWithPadding);
                            }
                            else
                            {
                                // There is an album cover to display with more than 2 lines.
                                // Set string format
                                stringFormat.Alignment = StringAlignment.Near;
                                stringFormat.Trimming = StringTrimming.EllipsisWord;

                                // Measure strings
                                sizeArtistName = g.MeasureString(song.ArtistName, fontDefaultBold, widthAvailableForText, stringFormat);
                                sizeAlbumTitle = g.MeasureString(currentAlbumTitle, fontDefault, widthAvailableForText, stringFormat);

                                // If there's only two lines, display text on only two lines
                                if (albumCoverEndIndex - albumCoverStartIndex == 1)
                                {
                                    // Display artist name on first line; display album title on second line
                                    rectArtistNameText = new RectangleF(((m_columns[0].Width - sizeArtistName.Width) / 2) - m_hScrollBar.Value, y, widthAvailableForText, heightWithPadding);
                                    rectAlbumTitleText = new RectangleF(((m_columns[0].Width - sizeAlbumTitle.Width) / 2) - m_hScrollBar.Value, y + m_songCache.LineHeight, widthAvailableForText, heightWithPadding);
                                }
                                // There is an album cover to display; between 2 and 6 lines AND the width of the column is at least 50 pixels (or
                                // it will try to display text in a too thin area)
                                else if (albumCoverEndIndex - albumCoverStartIndex <= 5 && m_columns[0].Width > 175)
                                {
                                    // There is no album cover to display; display only text.
                                    // Set string format
                                    stringFormat.Alignment = StringAlignment.Near;
                                    stringFormat.Trimming = StringTrimming.EllipsisWord;

                                    float widthRemainingForText = m_columns[0].Width - m_padding - heightWithPadding;

                                    // Measure strings
                                    sizeArtistName = g.MeasureString(song.ArtistName, fontDefaultBold, new SizeF(widthRemainingForText, heightWithPadding), stringFormat);
                                    sizeAlbumTitle = g.MeasureString(currentAlbumTitle, fontDefault, new SizeF(widthRemainingForText, heightWithPadding), stringFormat);

                                    // Try to center the cover art + padding + max text width
                                    float maxWidth = sizeArtistName.Width;
                                    if (sizeAlbumTitle.Width > maxWidth)
                                    {
                                        // Set new maximal width
                                        maxWidth = sizeAlbumTitle.Width;
                                    }

                                    useAlbumArtOverlay = true;

                                    float albumCoverX = (m_columns[0].Width - heightWithPadding - m_padding - m_padding - maxWidth) / 2;
                                    float artistNameY = (albumCoverZoneHeight - sizeArtistName.Height - sizeAlbumTitle.Height) / 2;

                                    // Display the album title at the top of the zome
                                    rectArtistNameText = new RectangleF(albumCoverX + heightWithPadding + m_padding - m_hScrollBar.Value, y + artistNameY, widthRemainingForText, heightWithPadding);
                                    rectAlbumTitleText = new RectangleF(albumCoverX + heightWithPadding + m_padding - m_hScrollBar.Value, y + artistNameY + sizeArtistName.Height, widthRemainingForText, heightWithPadding);

                                    // Set cover art rectangle
                                    rectAlbumCoverArt = new RectangleF(albumCoverX - m_hScrollBar.Value, y + m_padding, heightWithPadding, heightWithPadding);
                                }
                                // 7 and more lines
                                else
                                {
                                    // Display artist name at the top of the album cover; display album title at the bottom of the album cover
                                    rectArtistNameText = new RectangleF(((m_columns[0].Width - sizeArtistName.Width) / 2) - m_hScrollBar.Value, y + (m_padding * 2), widthAvailableForText, heightWithPadding);
                                    rectAlbumTitleText = new RectangleF(((m_columns[0].Width - sizeAlbumTitle.Width) / 2) - m_hScrollBar.Value, y + heightWithPadding - sizeAlbumTitle.Height, widthAvailableForText, heightWithPadding);

                                    // Draw background overlay behind text
                                    useAlbumArtOverlay = true;

                                    // Try to horizontally center the album cover if it's not taking the whole width (less padding)
                                    float albumCoverX = m_padding;
                                    if (m_columns[0].Width > heightWithPadding)
                                    {
                                        // Get position
                                        albumCoverX = ((float)(m_columns[0].Width - heightWithPadding) / 2) - m_hScrollBar.Value;
                                    }

                                    // Set cover art rectangle
                                    rectAlbumCoverArt = new RectangleF(albumCoverX, y + m_padding, heightWithPadding, heightWithPadding);
                                }

                            }
                        }

                        // Display album cover
                        if (imageAlbumCover != null)
                        {
                            // Draw album cover
                            g.DrawImage(imageAlbumCover, rectAlbumCoverArt);
                        }

                        if (useAlbumArtOverlay)
                        {
                            // Draw artist name and album title background
                            RectangleF rectArtistNameBackground = new RectangleF(rectArtistNameText.X - (m_padding / 2), rectArtistNameText.Y - (m_padding / 2), sizeArtistName.Width + m_padding, sizeArtistName.Height + m_padding);
                            RectangleF rectAlbumTitleBackground = new RectangleF(rectAlbumTitleText.X - (m_padding / 2), rectAlbumTitleText.Y - (m_padding / 2), sizeAlbumTitle.Width + m_padding, sizeAlbumTitle.Height + m_padding);
                            brush = new SolidBrush(Color.FromArgb(190, 0, 0, 0));
                            g.FillRectangle(brush, rectArtistNameBackground);
                            g.FillRectangle(brush, rectAlbumTitleBackground);
                            brush.Dispose();
                            brush = null;
                        }

                        // Check if this is the artist name column (set font to bold)
                        g.DrawString(song.ArtistName, fontDefaultBold, Brushes.White, rectArtistNameText, stringFormat);
                        g.DrawString(currentAlbumTitle, fontDefault, Brushes.White, rectAlbumTitleText, stringFormat);

                        // Draw horizontal line to distinguish albums
                        // Part 1: Draw line over grid
                        pen = new Pen(AlbumCoverBackgroundColor1);
                        g.DrawLine(pen, new Point(m_columns[0].Width, y), new Point(ClientRectangle.Width, y));
                        pen.Dispose();
                        pen = null;

                        // Part 2: Draw line over album art zone, in a lighter color
                        pen = new Pen(Color.FromArgb(115, 115, 115));
                        g.DrawLine(pen, new Point(0, y), new Point(m_columns[0].Width, y));
                        pen.Dispose();
                        pen = null;

                        // Check if the image is still valid
                        if (imageAlbumCover != null)
                        {
                            //    bool release = true;
                            //    foreach (GridViewImageCache cache in m_imageCache)
                            //    {
                            //        if (cache.Image == imageAlbumCover)
                            //        {
                            //            release = false;
                            //            break;
                            //        }
                            //    }

                            //    // Dispose image
                            //    if (release)
                            //    {
                            //        imageAlbumCover.Dispose();
                            //        imageAlbumCover = null;
                            //    }
                            //
                        }
                    }
                }

                #endregion

                // Loop through columns (skip first two columns)
                offsetX = m_columns[0].Width + m_columns[1].Width;
                for (int b = 2; b < m_songCache.ActiveColumns.Count; b++)
                {
                    // Get current column
                    GridViewSongColumn column = m_songCache.ActiveColumns[b];

                    // Get property through reflection
                    PropertyInfo propertyInfo = song.GetType().GetProperty(column.FieldName);
                    if (propertyInfo != null)
                    {
                        // Get property value
                        string value = string.Empty;
                        try
                        {
                            // Determine the type of value
                            if (propertyInfo.PropertyType.FullName == "System.String")
                            {
                                // Try to get the value
                                value = propertyInfo.GetValue(song, null).ToString();
                            }
                            // Nullable Int64
                            else if (propertyInfo.PropertyType.FullName.Contains("Int64") &&
                                    propertyInfo.PropertyType.FullName.Contains("Nullable"))
                            {
                                // Try to get the value
                                long? longValue = (long?)propertyInfo.GetValue(song, null);

                                // Check if null
                                if (longValue.HasValue)
                                {
                                    // Render to string
                                    value = longValue.Value.ToString();
                                }
                            }
                            // Nullable DateTime
                            else if (propertyInfo.PropertyType.FullName.Contains("DateTime") &&
                                    propertyInfo.PropertyType.FullName.Contains("Nullable"))
                            {
                                // Try to get the value
                                DateTime? dateTimeValue = (DateTime?)propertyInfo.GetValue(song, null);

                                // Check if null
                                if (dateTimeValue.HasValue)
                                {
                                    // Render to string
                                    value = dateTimeValue.Value.ToShortDateString() + " " + dateTimeValue.Value.ToShortTimeString();
                                }
                            }
                            else
                            {
                                // If the type of unknown, leave the value empty
                            }
                        }
                        catch
                        {
                            // Do nothing
                        }

                        // The last column always take the remaining width
                        int columnWidth = column.Width;
                        if (b == m_songCache.ActiveColumns.Count - 1)
                        {
                            // Calculate the remaining width
                            int columnsWidth = 0;
                            for (int c = 0; c < m_songCache.ActiveColumns.Count - 1; c++)
                            {
                                columnsWidth += m_songCache.ActiveColumns[c].Width;
                            }
                            columnWidth = ClientRectangle.Width - columnsWidth + m_hScrollBar.Value;
                        }

                        // Display text
                        rect = new Rectangle(offsetX - m_hScrollBar.Value, offsetY + (m_padding / 2), columnWidth, m_songCache.LineHeight - m_padding);
                        //rect = new Rectangle(offsetX - m_hScrollBar.Value, offsetY + (m_padding / 2), columnWidth, m_songCache.LineHeight - (m_padding / 2));
                        stringFormat.Trimming = StringTrimming.EllipsisCharacter;
                        stringFormat.Alignment = StringAlignment.Near;

                        // Check if this is the artist name column
                        brush = new SolidBrush(LineForeColor);
                        if (column.FieldName == "ArtistName" || column.FieldName == "TrackNumber")
                        {
                            // Use bold for artist name
                            g.DrawString(value, fontDefaultBold, brush, rect, stringFormat);
                        }
                        else
                        {
                            // Use default font for the other columns
                            g.DrawString(value, fontDefault, brush, rect, stringFormat);
                        }
                        brush.Dispose();
                        brush = null;
                    }

                    // Increment offset by the column width
                    offsetX += column.Width;
                }

                // Draw now playing icon
                if (song.SongId == m_nowPlayingSongId)
                {
                    // Which size is the minimum? Width or height?                    
                    int availableWidthHeight = m_columns[1].Width - 4;
                    if (m_songCache.LineHeight <= m_columns[1].Width)
                    {
                        //availableWidthHeight = m_columns[1].Width - m_padding;
                        availableWidthHeight = m_songCache.LineHeight - 4;
                    }
                    else
                    {                        
                        availableWidthHeight = m_columns[1].Width - 4;
                    }                    

                    // Calculate the icon position                    
                    float iconNowPlayingX = ((m_columns[1].Width - availableWidthHeight) / 2) + m_columns[0].Width - m_hScrollBar.Value;
                    float iconNowPlayingY = offsetY + ((m_songCache.LineHeight - availableWidthHeight) / 2);

                    // Create NowPlaying rect (MUST be in integer)
                    //m_rectNowPlaying = new Rectangle((int)iconNowPlayingX, offsetY + (m_padding / 2), m_columns[1].Width - m_padding, m_songCache.LineHeight - m_padding);
                    m_rectNowPlaying = new Rectangle((int)iconNowPlayingX, (int)iconNowPlayingY, availableWidthHeight, availableWidthHeight);
                    nowPlayingSongFound = true;

                    // Draw outer circle
                    brushGradient = new LinearGradientBrush(m_rectNowPlaying, Color.FromArgb(50, IconNowPlayingColor1.R, IconNowPlayingColor1.G, IconNowPlayingColor1.B), IconNowPlayingColor2, m_timerAnimationNowPlayingCount % 360);
                    g.FillEllipse(brushGradient, m_rectNowPlaying);
                    brushGradient.Dispose();
                    brushGradient = null;

                    // Draw inner circle
                    rect = new Rectangle((int)iconNowPlayingX + 4, (int)iconNowPlayingY + 4, availableWidthHeight - 8, availableWidthHeight - 8);
                    brush = new SolidBrush(LineNowPlayingColor1);
                    g.FillEllipse(brush, rect);
                    brush.Dispose();
                    brush = null;
                }
            }

            // If no songs are playing...
            if (!nowPlayingSongFound)
            {
                // Set the current now playing rectangle as "empty"
                m_rectNowPlaying = new Rectangle(0, 0, 1, 1);
            }

            // Draw header (for some reason, the Y must be set -1 to cover an area which isn't supposed to be displayed)
            Rectangle rectBackgroundHeader = new Rectangle(0, -1, ClientRectangle.Width, m_songCache.LineHeight + 1);
            brushGradient = new LinearGradientBrush(rectBackgroundHeader, HeaderColor1, HeaderColor2, 90);
            g.FillRectangle(brushGradient, rectBackgroundHeader);
            brushGradient.Dispose();
            brushGradient = null;

            // Loop through columns
            offsetX = 0;
            for (int b = 0; b < m_songCache.ActiveColumns.Count; b++)
            {
                // Get current column
                GridViewSongColumn column = m_songCache.ActiveColumns[b];

                // The last column always take the remaining width
                int columnWidth = column.Width;
                if (b == m_songCache.ActiveColumns.Count - 1)
                {
                    // Calculate the remaining width
                    int columnsWidth = 0;
                    for (int c = 0; c < m_songCache.ActiveColumns.Count - 1; c++)
                    {
                        columnsWidth += m_songCache.ActiveColumns[c].Width;
                    }
                    columnWidth = ClientRectangle.Width - columnsWidth + m_hScrollBar.Value;
                }

                // Check if mouse is over this column header
                if (column.IsMouseOverColumnHeader)
                {
                    // Draw header (for some reason, the Y must be set -1 to cover an area which isn't supposed to be displayed)
                    rect = new Rectangle(offsetX - m_hScrollBar.Value, -1, columnWidth, m_songCache.LineHeight + 1);
                    brushGradient = new LinearGradientBrush(rect, HeaderHoverColor1, HeaderHoverColor2, 90);
                    g.FillRectangle(brushGradient, rect);
                    brushGradient.Dispose();
                    brushGradient = null;
                }

                // Display title
                //Rectangle rectTitle = new Rectangle(offsetX - m_hScrollBar.Value, m_padding / 2, columnWidth, m_songCache.LineHeight - (m_padding / 2));
                Rectangle rectTitle = new Rectangle(offsetX - m_hScrollBar.Value, m_padding / 2, columnWidth, m_songCache.LineHeight - m_padding);
                stringFormat.Trimming = StringTrimming.EllipsisCharacter;
                brush = new SolidBrush(HeaderForeColor);
                g.DrawString(column.Title, fontDefaultBold, brush, rectTitle, stringFormat);
                brush.Dispose();
                brush = null;

                // Draw column separator line; determine the height of the line
                int columnHeight = ClientRectangle.Height;

                // Determine the height of the line; if the items don't fit the control height...
                if (m_items.Count < m_songCache.NumberOfLinesFittingInControl)
                {
                    // Set height as the number of items (plus header)
                    columnHeight = (m_items.Count + 1) * m_songCache.LineHeight;
                }

                // Draw line
                g.DrawLine(Pens.DarkGray, new Point(offsetX + m_songCache.ActiveColumns[b].Width - m_hScrollBar.Value, 0), new Point(offsetX + m_songCache.ActiveColumns[b].Width - m_hScrollBar.Value, columnHeight));

                // Check if the column is ordered by
                if (column.FieldName == m_orderByFieldName && !String.IsNullOrEmpty(column.FieldName))
                {
                    // Create triangle points,,,
                    PointF[] ptTriangle = new PointF[3];

                    //// ... depending on the order by ascending value
                    //if (m_orderByAscending)
                    //{
                    //    // Create points for ascending
                    //    ptTriangle[0] = new PointF(offsetX + columnWidth - m_padding - 4 - m_hScrollBar.Value, m_padding);
                    //    ptTriangle[1] = new PointF(offsetX + columnWidth - m_padding - m_hScrollBar.Value, m_songCache.LineHeight - m_padding);
                    //    ptTriangle[2] = new PointF(offsetX + columnWidth - m_padding - 8 - m_hScrollBar.Value, m_songCache.LineHeight - m_padding);
                    //}
                    //else
                    //{
                    //    // Create points for descending
                    //    ptTriangle[0] = new PointF(offsetX + columnWidth - m_padding - 4 - m_hScrollBar.Value, m_songCache.LineHeight - m_padding);
                    //    ptTriangle[1] = new PointF(offsetX + columnWidth - m_padding - m_hScrollBar.Value, m_padding);
                    //    ptTriangle[2] = new PointF(offsetX + columnWidth - m_padding - 8 - m_hScrollBar.Value, m_padding);
                    //}

                    // ... depending on the order by ascending value
                    int triangleWidthHeight = 8;                    
                    int trianglePadding = (m_songCache.LineHeight - triangleWidthHeight) / 2;
                    if (m_orderByAscending)
                    {
                        // Create points for ascending
                        ptTriangle[0] = new PointF(offsetX + columnWidth - triangleWidthHeight - (triangleWidthHeight / 2) - m_hScrollBar.Value, trianglePadding);
                        ptTriangle[1] = new PointF(offsetX + columnWidth - triangleWidthHeight - m_hScrollBar.Value, m_songCache.LineHeight - trianglePadding);
                        ptTriangle[2] = new PointF(offsetX + columnWidth - triangleWidthHeight - triangleWidthHeight - m_hScrollBar.Value, m_songCache.LineHeight - trianglePadding);
                    }
                    else
                    {
                        // Create points for descending
                        ptTriangle[0] = new PointF(offsetX + columnWidth - triangleWidthHeight - (triangleWidthHeight / 2) - m_hScrollBar.Value, m_songCache.LineHeight - trianglePadding);
                        ptTriangle[1] = new PointF(offsetX + columnWidth - triangleWidthHeight - m_hScrollBar.Value, trianglePadding);
                        ptTriangle[2] = new PointF(offsetX + columnWidth - triangleWidthHeight - triangleWidthHeight - m_hScrollBar.Value, trianglePadding);
                    }

                    // Draw triangle
                    pen = new Pen(HeaderForeColor);
                    g.DrawPolygon(pen, ptTriangle);
                    pen.Dispose();
                    pen = null;
                }

                // Increment offset by the column width
                offsetX += column.Width;
            }

            // Display debug information if enabled
            if (m_displayDebugInformation)
            {
                // Build debug string
                StringBuilder sbDebug = new StringBuilder();
                sbDebug.AppendLine("Line Count: " + m_items.Count.ToString());
                sbDebug.AppendLine("Line Height: " + m_songCache.LineHeight.ToString());
                sbDebug.AppendLine("Lines Fit In Height: " + m_songCache.NumberOfLinesFittingInControl.ToString());
                sbDebug.AppendLine("Total Width: " + m_songCache.TotalWidth);
                sbDebug.AppendLine("Total Height: " + m_songCache.TotalHeight);
                sbDebug.AppendLine("Scrollbar Offset Y: " + m_songCache.ScrollBarOffsetY);
                sbDebug.AppendLine("HScrollbar Maximum: " + m_hScrollBar.Maximum.ToString());
                sbDebug.AppendLine("HScrollbar LargeChange: " + m_hScrollBar.LargeChange.ToString());
                sbDebug.AppendLine("HScrollbar Value: " + m_hScrollBar.Value.ToString());
                sbDebug.AppendLine("VScrollbar Maximum: " + m_vScrollBar.Maximum.ToString());
                sbDebug.AppendLine("VScrollbar LargeChange: " + m_vScrollBar.LargeChange.ToString());
                sbDebug.AppendLine("VScrollbar Value: " + m_vScrollBar.Value.ToString());

                // Measure string
                stringFormat.Trimming = StringTrimming.Word;
                stringFormat.LineAlignment = StringAlignment.Near;
                SizeF sizeDebugText = g.MeasureString(sbDebug.ToString(), fontDefault, m_columns[0].Width - 1, stringFormat);
                rectF = new RectangleF(0, 0, m_columns[0].Width - 1, sizeDebugText.Height);

                // Draw background
                brush = new SolidBrush(Color.FromArgb(200, 0, 0, 0));
                g.FillRectangle(brush, rectF);
                brush.Dispose();
                brush = null;

                // Draw string
                g.DrawString(sbDebug.ToString(), fontDefault, Brushes.White, rectF, stringFormat);
            }

            // If both scrollbars are visible...
            if (m_hScrollBar.Visible && m_vScrollBar.Visible)
            {
                // Draw a bit of control color over the 16x16 area in the lower right corner
                brush = new SolidBrush(SystemColors.Control);
                g.FillRectangle(brush, new Rectangle(ClientRectangle.Width - 16, ClientRectangle.Height - 16, 16, 16));
                brush.Dispose();
                brush = null;
            }
        }

        #endregion

        #region Other Events
        
        /// <summary>
        /// Occurs when the control is resized.
        /// Invalidates the cache.
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected override void OnResize(EventArgs e)
        {
            // Bug when putting window in maximized mode: black area at the bottom
            // it's because the maximum value is set later in OnPaint
            // If the scrollY value is 
            if (m_vScrollBar.Maximum - m_vScrollBar.LargeChange < m_vScrollBar.Value)
            {
                // Set new scrollbar value
                m_vScrollBar.Value = m_vScrollBar.Maximum - m_vScrollBar.LargeChange + 1;
            }

            // Set horizontal scrollbar width and position
            m_hScrollBar.Top = ClientRectangle.Height - m_hScrollBar.Height;

            // Invalidate cache
            InvalidateSongCache();

            base.OnResize(e);
        }

        /// <summary>
        /// Occurs when the user changes the horizontal scrollbar value.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        protected void m_hScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            // If the 

            // Redraw control
            Refresh();
        }

        /// <summary>
        /// Occurs when the user changes the vertical scrollbar value.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        protected void m_vScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            // Redraw control
            Refresh();
        }

        #endregion

        #region Mouse Events

        /// <summary>
        /// Occurs when the mouse cursor enters on the control.        
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected override void OnMouseEnter(EventArgs e)
        {
            // Set flags
            m_isMouseOverControl = true;

            base.OnMouseEnter(e);
        }

        /// <summary>
        /// Occurs when the mouse cursor leaves the control.
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            // Set flags
            bool controlNeedsToBeUpdated = false;
            m_isMouseOverControl = false;

            // Check if all the data is valid
            if (m_columns == null || m_songCache == null)
            {
                return;
            }

            // Calculate scrollbar offset Y
            int scrollbarOffsetY = (m_startLineNumber * m_songCache.LineHeight) - m_vScrollBar.Value;

            // Reset mouse over item flags
            for (int b = m_startLineNumber; b < m_startLineNumber + m_numberOfLinesToDraw; b++)
            {
                // Check if the mouse was over this item
                if (m_items[b].IsMouseOverItem)
                {
                    // Reset flag and invalidate region
                    m_items[b].IsMouseOverItem = false;
                    Invalidate(new Rectangle(m_columns[0].Width - m_hScrollBar.Value, ((b - m_startLineNumber + 1) * m_songCache.LineHeight) + scrollbarOffsetY, ClientRectangle.Width - m_columns[0].Width + m_hScrollBar.Value, m_songCache.LineHeight));
                    controlNeedsToBeUpdated = true;

                    // Exit loop
                    break;
                }
            }

            // Reset flags
            int columnOffsetX2 = 0;
            for (int b = 0; b < m_columns.Count; b++)
            {
                // Was mouse over this column header?
                if (m_columns[b].IsMouseOverColumnHeader)
                {
                    // Reset flag
                    m_columns[b].IsMouseOverColumnHeader = false;

                    // Invalidate region
                    Invalidate(new Rectangle(columnOffsetX2 - m_hScrollBar.Value, 0, m_columns[b].Width, m_songCache.LineHeight));
                    controlNeedsToBeUpdated = true;
                }

                // Increment offset
                columnOffsetX2 += m_columns[b].Width;
            }

            // Check if control needs to be updated
            if (controlNeedsToBeUpdated)
            {
                // Update control
                Update();
            }

            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Occurs when the user is pressing down a mouse button.
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            // Set flags
            m_dragStartX = e.X;

            // Check if all the data is valid
            if (m_columns == null || m_songCache == null)
            {
                return;
            }

            // Loop through columns
            foreach (GridViewSongColumn column in m_songCache.ActiveColumns)
            {
                // Check if the mouse pointer is over the column limit
                if (column.IsMouseCursorOverColumnLimit && column.CanBeResized)
                {
                    // Set resizing column flag
                    column.IsUserResizingColumn = true;

                    // Save the original column width
                    m_dragOriginalColumnWidth = column.Width;
                }

                if (column.IsMouseOverColumnHeader && column.CanBeMoved)
                {
                    // Set resizing column flag
                    column.IsUserMovingColumn = true;

                    // Save the original column width
                    //m_dragOriginalColumnWidth = column.Width;
                }
            }

            base.OnMouseDown(e);
        }

        /// <summary>
        /// Occurs when the user releases a mouse button.        
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            // Reset flags
            m_dragStartX = -1;

            // Check if all the data is valid
            if (m_columns == null || m_songCache == null)
            {
                return;
            }

            // Loop through columns
            foreach (GridViewSongColumn column in m_songCache.ActiveColumns)
            {
                // Reset flags
                column.IsUserResizingColumn = false;
                column.IsUserMovingColumn = false;
            }

            //// Check if the cursor was over
            //IGridViewItem mouseOverItem = m_items.FirstOrDefault(x => x.IsMouseOverItem == true);
            //if (mouseOverItem != null)
            //{
            //    // Reset selection, unless the CTRL key is held (TODO)
            //    List<IGridViewItem> selectedItems = m_items.Where(x => x.IsSelected == true).ToList();
            //    foreach (IGridViewItem item in selectedItems)
            //    {
            //        // Reset flag
            //        item.IsSelected = false;
            //    }

            //    // Set flag
            //    mouseOverItem.IsSelected = true;
            //}

            base.OnMouseUp(e);
        }

        /// <summary>
        /// Occurs when the user clicks on the control.
        /// Selects or unselects items, reorders columns.
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            // Check if all the data is valid
            if (m_columns == null || m_songCache == null)
            {
                return;
            }

            // Check if the user is resizing a column
            GridViewSongColumn columnResizing = m_columns.FirstOrDefault(x => x.IsUserResizingColumn == true);

            // Calculate scrollbar offset Y
            int scrollbarOffsetY = (m_startLineNumber * m_songCache.LineHeight) - m_vScrollBar.Value;

            // Check if the user has clicked on the header, and the user isn't resizing a column
            if (e.Y >= 0 &&
                e.Y <= m_songCache.LineHeight &&
                columnResizing == null)
            {
                // Check on what column the user has clicked
                int offsetX = 0;
                for (int a = 0; a < m_columns.Count; a++)
                {
                    // Get current column
                    GridViewSongColumn column = m_columns[a];

                    // Check if the mouse pointer is over this column
                    if (e.X >= offsetX && e.X <= offsetX + column.Width)
                    {
                        // Check if the column order was already set
                        if (m_orderByFieldName == column.FieldName)
                        {
                            // Reverse ascending/descending
                            m_orderByAscending = !m_orderByAscending;
                        }
                        else
                        {
                            // Set order by field name
                            m_orderByFieldName = column.FieldName;

                            // By default, the order is ascending
                            m_orderByAscending = true;
                        }

                        // Invalidate cache and song items
                        m_items = null;
                        m_songCache = null;

                        // Refresh control
                        Refresh();
                        return;
                    }

                    // Increment X offset
                    offsetX += column.Width;
                }
            }

            // Loop through visible lines to find the original selected items
            int startIndex = -1;
            int endIndex = -1;
            for (int a = m_startLineNumber; a < m_startLineNumber + m_numberOfLinesToDraw; a++)
            {
                // Check if the item is selected
                if (m_items[a].IsSelected)
                {
                    // Check if the start index was set
                    if (startIndex == -1)
                    {
                        // Set start index
                        startIndex = a;
                    }
                    // Check if the end index is set or if it needs to be updated
                    if (endIndex == -1 || endIndex < a)
                    {
                        // Set end index
                        endIndex = a;
                    }
                }
            }

            // Make sure the indexes are set
            if (startIndex > -1 && endIndex > -1)
            {
                // Invalidate the original selected lines
                int startY = ((startIndex - m_startLineNumber + 1) * m_songCache.LineHeight) + scrollbarOffsetY;
                int endY = ((endIndex - m_startLineNumber + 2) * m_songCache.LineHeight) + scrollbarOffsetY;
                Invalidate(new Rectangle(m_columns[0].Width - m_hScrollBar.Value, startY, ClientRectangle.Width - m_columns[0].Width + m_hScrollBar.Value, endY - startY));
            }

            // Reset selection (make sure SHIFT or CTRL isn't held down)
            if ((Control.ModifierKeys & Keys.Shift) == 0 &&
               (Control.ModifierKeys & Keys.Control) == 0)
            {
                SongGridViewItem mouseOverItem = m_items.FirstOrDefault(x => x.IsMouseOverItem == true);
                if (mouseOverItem != null)
                {
                    // Reset selection, unless the CTRL key is held (TODO)
                    List<SongGridViewItem> selectedItems = m_items.Where(x => x.IsSelected == true).ToList();
                    foreach (SongGridViewItem item in selectedItems)
                    {
                        // Reset flag
                        item.IsSelected = false;
                    }
                }
            }

            // Loop through visible lines to update the new selected item
            for (int a = m_startLineNumber; a < m_startLineNumber + m_numberOfLinesToDraw; a++)
            {
                // Check if mouse is over this item
                if (m_items[a].IsMouseOverItem)
                {
                    // Check if SHIFT is held
                    if ((Control.ModifierKeys & Keys.Shift) != 0)
                    {
                        // Find the start index of the selection
                        int startIndexSelection = m_lastItemIndexClicked;
                        if (a < startIndexSelection)
                        {
                            startIndexSelection = a;
                        }

                        // Find the end index of the selection
                        int endIndexSelection = m_lastItemIndexClicked;
                        if (a > endIndexSelection)
                        {
                            endIndexSelection = a + 1;
                        }

                        // Loop through items to selected
                        for (int b = startIndexSelection; b < endIndexSelection; b++)
                        {
                            // Set items as selected
                            m_items[b].IsSelected = true;
                        }

                        // Invalidate region
                        Invalidate();
                    }
                    else
                    {
                        // Set this item as the new selected item
                        m_items[a].IsSelected = true;

                        // Invalidate region
                        Invalidate(new Rectangle(m_columns[0].Width - m_hScrollBar.Value, ((a - m_startLineNumber + 1) * m_songCache.LineHeight) + scrollbarOffsetY, ClientRectangle.Width - m_columns[0].Width + m_hScrollBar.Value, m_songCache.LineHeight));
                    }

                    // Set the last item clicked index
                    m_lastItemIndexClicked = a;

                    // Exit loop
                    break;
                }
            }

            // Update invalid regions
            Update();

            base.OnMouseClick(e);
        }

        /// <summary>
        /// Occurs when the user double-clicks on the control.
        /// Starts the playback of a new song.
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            // Check if all the data is valid
            if (m_columns == null || m_songCache == null)
            {
                return;
            }

            // Calculate scrollbar offset Y
            int scrollbarOffsetY = (m_startLineNumber * m_songCache.LineHeight) - m_vScrollBar.Value;

            // Keep original songId in case the now playing value is set before invalidating the older value
            Guid originalSongId = m_nowPlayingSongId;

            // Loop through visible lines
            for (int a = m_startLineNumber; a < m_startLineNumber + m_numberOfLinesToDraw; a++)
            {
                // Check if mouse is over this item
                if (m_items[a].IsMouseOverItem)
                {
                    // Set this item as the new now playing
                    m_nowPlayingSongId = m_items[a].Song.SongId;

                    // Invalidate region
                    Invalidate(new Rectangle(m_columns[0].Width - m_hScrollBar.Value, ((a - m_startLineNumber + 1) * m_songCache.LineHeight) + scrollbarOffsetY, ClientRectangle.Width - m_columns[0].Width + m_hScrollBar.Value, m_songCache.LineHeight));
                }
                else if (m_items[a].Song.SongId == originalSongId)
                {
                    // Invalidate region
                    Invalidate(new Rectangle(m_columns[0].Width - m_hScrollBar.Value, ((a - m_startLineNumber + 1) * m_songCache.LineHeight) + scrollbarOffsetY, ClientRectangle.Width - m_columns[0].Width + m_hScrollBar.Value, m_songCache.LineHeight));
                }
            }

            // Update invalid regions
            Update();

            base.OnMouseDoubleClick(e);
        }

        /// <summary>
        /// Occurs when the mouse pointer is moving over the control.
        /// Manages the display of mouse on/off visual effects.
        /// </summary>
        /// <param name="e">Event Arguments</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Declare variables
            bool controlNeedsToBeUpdated = false;

            // Check if all the data is valid
            if (m_columns == null || m_songCache == null)
            {
                return;
            }

            // Check if the user is currently resizing a column (loop through columns)
            foreach (GridViewSongColumn column in m_songCache.ActiveColumns)
            {
                // Check if the user is currently resizing this column
                if (column.IsUserResizingColumn)
                {
                    // Calculate the new column width
                    int newWidth = m_dragOriginalColumnWidth - (m_dragStartX - e.X);

                    // Make sure the width isn't lower than the minimum width
                    if (newWidth < m_minimumColumnWidth)
                    {
                        // Set width as minimum column width
                        newWidth = m_minimumColumnWidth;
                    }

                    // Set column width
                    column.Width = newWidth;

                    // Refresh control (invalidate whole control)
                    controlNeedsToBeUpdated = true;
                    Invalidate();
                    InvalidateSongCache();

                    // Auto adjust horizontal scrollbar value if it exceeds the value range (i.e. do not show empty column)
                    if (m_hScrollBar.Value > m_hScrollBar.Maximum - m_hScrollBar.LargeChange)
                    {
                        // Set new value
                        int tempValue = m_hScrollBar.Maximum - m_hScrollBar.LargeChange;
                        if (tempValue < 0)
                        {
                            tempValue = 0;
                        }
                        m_hScrollBar.Value = tempValue;
                    }
                }

                // Check if the user is currently moving this column 
                if (column.IsUserMovingColumn)
                {
                    //// Calculate the new column width
                    //int newWidth = m_dragOriginalColumnWidth - (m_dragStartX - e.X);

                    //// Make sure the width isn't lower than the minimum width
                    //if (newWidth < m_minimumColumnWidth)
                    //{
                    //    // Set width as minimum column width
                    //    newWidth = m_minimumColumnWidth;
                    //}

                    //// Set column width
                    //column.Width = newWidth;

                    //// Refresh control (invalidate whole control)
                    //controlNeedsToBeUpdated = true;
                    //Invalidate();
                }
            }

            // Check if the cursor needs to be changed            
            int offsetX = 0;
            bool mousePointerIsOverColumnLimit = false;
            foreach (GridViewSongColumn column in m_songCache.ActiveColumns)
            {
                // Increment offset by the column width
                offsetX += column.Width;

                // Check if the column can be resized
                if (column.CanBeResized)
                {
                    // Check if the mouse pointer is over a column (add 1 pixel so it's easier to select)
                    if (e.X >= offsetX - m_hScrollBar.Value && e.X <= offsetX + 1 - m_hScrollBar.Value)
                    {
                        // Set flag
                        mousePointerIsOverColumnLimit = true;
                        column.IsMouseCursorOverColumnLimit = true;

                        // Change the cursor if it's not the right cursor
                        if (Cursor != Cursors.VSplit)
                        {
                            // Change cursor
                            Cursor = Cursors.VSplit;
                        }
                    }
                    else
                    {
                        // Reset flag
                        column.IsMouseCursorOverColumnLimit = false;
                    }
                }
            }

            // Check if the default cursor needs to be restored
            if (!mousePointerIsOverColumnLimit && Cursor != Cursors.Default)
            {
                // Restore the default cursor
                Cursor = Cursors.Default;
            }

            // Reset flags
            int columnOffsetX2 = 0;
            for (int b = 0; b < m_columns.Count; b++)
            {
                // Was mouse over this column header?
                if (m_columns[b].IsMouseOverColumnHeader)
                {
                    // Reset flag
                    m_columns[b].IsMouseOverColumnHeader = false;

                    // Invalidate region
                    Invalidate(new Rectangle(columnOffsetX2 - m_hScrollBar.Value, 0, m_columns[b].Width, m_songCache.LineHeight));
                    controlNeedsToBeUpdated = true;
                }

                // Increment offset
                columnOffsetX2 += m_columns[b].Width;
            }

            // Check if the mouse pointer is over the header
            if (e.Y >= 0 &&
                e.Y <= m_songCache.LineHeight)
            {
                // Check on what column the user has clicked
                int columnOffsetX = 0;
                for (int a = 0; a < m_columns.Count; a++)
                {
                    // Get current column
                    GridViewSongColumn column = m_columns[a];

                    // Check if the mouse pointer is over this column
                    if (e.X >= columnOffsetX - m_hScrollBar.Value && e.X <= columnOffsetX + column.Width - m_hScrollBar.Value)
                    {
                        // Set flag
                        column.IsMouseOverColumnHeader = true;

                        // Invalidate region
                        Invalidate(new Rectangle(columnOffsetX - m_hScrollBar.Value, 0, m_columns[a].Width, m_songCache.LineHeight));

                        // Exit loop
                        controlNeedsToBeUpdated = true;
                        break;
                    }

                    // Increment X offset
                    columnOffsetX += column.Width;
                }
            }

            // Check if the mouse cursor is over a line (loop through lines)                        
            int offsetY = 0;
            int scrollbarOffsetY = (m_startLineNumber * m_songCache.LineHeight) - m_vScrollBar.Value;

            // Reset mouse over item flags
            for (int b = m_startLineNumber; b < m_startLineNumber + m_numberOfLinesToDraw; b++)
            {
                // Check if the mouse was over this item
                if (m_items[b].IsMouseOverItem)
                {
                    // Reset flag and invalidate region
                    m_items[b].IsMouseOverItem = false;
                    Invalidate(new Rectangle(m_columns[0].Width - m_hScrollBar.Value, ((b - m_startLineNumber + 1) * m_songCache.LineHeight) + scrollbarOffsetY, ClientRectangle.Width - m_columns[0].Width + m_hScrollBar.Value, m_songCache.LineHeight));

                    // Exit loop
                    break;
                }
            }

            // Put new mouse over flag
            for (int a = m_startLineNumber; a < m_startLineNumber + m_numberOfLinesToDraw; a++)
            {
                // Get song
                SongDTO song = m_items[a].Song;

                // Calculate offset
                offsetY = (a * m_songCache.LineHeight) - m_vScrollBar.Value + m_songCache.LineHeight;

                // Check if the mouse cursor is over this line (and not already mouse over)
                if (e.X >= m_columns[0].Width - m_hScrollBar.Value &&
                    e.Y >= offsetY &&
                    e.Y <= offsetY + m_songCache.LineHeight &&
                    !m_items[a].IsMouseOverItem)
                {
                    // Set item as mouse over
                    m_items[a].IsMouseOverItem = true;

                    // Invalidate region
                    Invalidate(new Rectangle(m_columns[0].Width - m_hScrollBar.Value, offsetY, ClientRectangle.Width - m_columns[0].Width + m_hScrollBar.Value, m_songCache.LineHeight));

                    // Update control
                    //Update();
                    controlNeedsToBeUpdated = true;

                    // Exit loop
                    break;
                }
            }

            // Check if the control needs to be refreshed
            if (controlNeedsToBeUpdated)
            {
                // Refresh control
                Update();
            }

            base.OnMouseMove(e);
        }

        /// <summary>
        /// Handles the global messages for the mouse wheel.
        /// </summary>
        /// <param name="m">Message</param>
        /// <returns>Nothing interesting</returns>
        public bool PreFilterMessage(ref Message m)
        {
            // Check message type
            if (m.Msg == Win32.WM_MOUSEWHEEL)
            {
                // Get mouse wheel delta
                int delta = Win32.GetWheelDeltaWParam((int)m.WParam);

                // Check if all the data is valid
                if (m_columns == null || m_songCache == null)
                {
                    return false;
                }

                // Make sure the mouse cursor is over the control, and that the vertical scrollbar is enabled
                if (!m_isMouseOverControl || !m_vScrollBar.Enabled)
                {
                    return false;
                }

                // Get relative value
                int value = delta / SystemInformation.MouseWheelScrollDelta;

                // Set new value
                int newValue = m_vScrollBar.Value + (-value * m_songCache.LineHeight);

                // Check for minimum
                if (newValue < 0)
                {
                    newValue = 0;
                }
                // Check for maximum
                else if (newValue > m_vScrollBar.Maximum - m_vScrollBar.LargeChange)
                {
                    newValue = m_vScrollBar.Maximum - m_vScrollBar.LargeChange;
                }

                // Set scrollbar value
                m_vScrollBar.Value = newValue;

                // Invalidate the whole control and refresh
                Refresh();
            }

            return false;
        }

        #endregion

        /// <summary>
        /// Creates a cache of values used for rendering the song grid view.
        /// Also sets scrollbar position, height, value, maximum, etc.
        /// </summary>
        private void InvalidateSongCache()
        {
            // Check if columns have been created
            if (m_columns == null || m_columns.Count == 0)
            {
                return;
            }

            // Create cache
            m_songCache = new GridViewSongCache();

            // Get active columns and order them
            m_songCache.ActiveColumns = m_columns.Where(x => x.Order >= 0).OrderBy(x => x.Order).ToList();

            // Load custom font
            Font fontDefaultBold = Tools.LoadCustomFont(FontCollection, CustomFontName, Font.Size, FontStyle.Bold);

            // Load default fonts if custom fonts were not found
            if (fontDefaultBold == null)
            {
                // Load default font
                fontDefaultBold = new Font(Font.FontFamily.Name, Font.Size, FontStyle.Bold);
            }

            // Create temporary bitmap/graphics objects to measure a string (to determine line height)
            Bitmap bmpTemp = new Bitmap(200, 100);
            Graphics g = Graphics.FromImage(bmpTemp);
            SizeF sizeFont = g.MeasureString("QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm!@#$%^&*()", fontDefaultBold);
            g.Dispose();
            g = null;
            bmpTemp.Dispose();
            bmpTemp = null;

            // Calculate the line height (try to measure the total possible height of characters using the custom or default font)
            m_songCache.LineHeight = (int)sizeFont.Height + m_padding;
            m_songCache.TotalHeight = m_songCache.LineHeight * m_items.Count;

            // Check if the total active columns width exceed the width available in the control
            m_songCache.TotalWidth = 0;
            for (int a = 0; a < m_songCache.ActiveColumns.Count; a++)
            {
                // Increment total width
                m_songCache.TotalWidth += m_songCache.ActiveColumns[a].Width;
            }

            // Calculate the number of lines visible (count out the header, which is one line height)
            m_songCache.NumberOfLinesFittingInControl = (int)Math.Floor((double)(ClientRectangle.Height) / (double)(m_songCache.LineHeight));

            // Set vertical scrollbar dimensions
            m_vScrollBar.Top = m_songCache.LineHeight;
            m_vScrollBar.Left = ClientRectangle.Width - m_vScrollBar.Width;
            m_vScrollBar.Minimum = 0;

            // Scrollbar maximum is the number of lines fitting in the screen + the remaining line which might be cut
            // by the control height because it's not a multiple of line height (i.e. the last line is only partly visible)
            int lastLineHeight = ClientRectangle.Height - (m_songCache.LineHeight * m_songCache.NumberOfLinesFittingInControl);

            // Check width
            if (m_songCache.TotalWidth > ClientRectangle.Width - m_vScrollBar.Width)
            {
                // Set scrollbar values
                m_hScrollBar.Maximum = m_songCache.TotalWidth;
                m_hScrollBar.SmallChange = 5;
                m_hScrollBar.LargeChange = ClientRectangle.Width;

                // Show scrollbar
                m_hScrollBar.Visible = true;
            }

            // Check if the horizontal scrollbar needs to be turned off
            if (m_songCache.TotalWidth <= ClientRectangle.Width - m_vScrollBar.Width && m_hScrollBar.Visible)
            {
                // Hide the horizontal scrollbar
                m_hScrollBar.Visible = false;

                //// TODO: Fix this
                //if (m_vScrollBar.Value >= m_vScrollBar.Maximum - m_vScrollBar.LargeChange - 1)
                //{
                //    m_vScrollBar.Value = m_vScrollBar.Maximum - m_vScrollBar.LargeChange - 1 + m_songCache.ScrollBarOffsetY;
                //}
            }

            // If there are less items than items fitting on screen...
            //if (m_songCache.NumberOfLinesFittingInControl - 1 >= m_items.Count)
            if (((m_songCache.NumberOfLinesFittingInControl - 1) * m_songCache.LineHeight) - m_hScrollBar.Height >= m_songCache.TotalHeight)
            {
                // Disable the scrollbar
                m_vScrollBar.Enabled = false;
                m_vScrollBar.Value = 0;
            }
            else
            {
                // Set scrollbar values
                m_vScrollBar.Enabled = true;

                // The real large change needs to be added to the LargeChange and Maximum property in order to work. 
                int realLargeChange = m_songCache.LineHeight * 5;

                // Calculate the vertical scrollbar maximum
                int vMax = m_songCache.LineHeight * (m_items.Count - m_songCache.NumberOfLinesFittingInControl + 1) - lastLineHeight + realLargeChange;

                // Add the horizontal scrollbar height if visible
                if (m_hScrollBar.Visible)
                {
                    // Add height
                    vMax += m_hScrollBar.Height;
                }

                // Compensate for the header, and for the last line which might be truncated by the control height
                m_vScrollBar.Maximum = vMax;
                m_vScrollBar.SmallChange = m_songCache.LineHeight;
                m_vScrollBar.LargeChange = 1 + realLargeChange;
            }

            //// Check if the horizontal scrollbar is visible
            //if (m_hScrollBar.Visible)
            //{
            //    lastLineHeight -= m_hScrollBar.Height;
            //}


            // Calculate the scrollbar offset Y
            m_songCache.ScrollBarOffsetY = (m_startLineNumber * m_songCache.LineHeight) - m_vScrollBar.Value;

            // If both scrollbars need to be visible, the width and height must be changed
            if (m_hScrollBar.Visible && m_vScrollBar.Visible)
            {
                // Cut 16 pixels
                m_hScrollBar.Width = ClientRectangle.Width - 16;
                m_vScrollBar.Height = ClientRectangle.Height - m_songCache.LineHeight - 16;
            }
            else
            {
                m_vScrollBar.Height = ClientRectangle.Height - m_songCache.LineHeight;
            }
        }

        /// <summary>
        /// This timer triggers the animation of the currently playing song.
        /// </summary>
        /// <param name="sender">Event Sender</param>
        /// <param name="e">Event Arguments</param>
        protected void m_timerAnimationNowPlaying_Tick(object sender, EventArgs e)
        {
            // If the rectangle is "empty", do not trigger invalidation
            if (m_rectNowPlaying.X == 0 &&
                m_rectNowPlaying.Y == 0 &&
                m_rectNowPlaying.Width == 1 &&
                m_rectNowPlaying.Height == 1)
            {
                return;
            }

            // Increment counter            
            m_timerAnimationNowPlayingCount += 10;

            // Invalidate region for now playing
            Invalidate(m_rectNowPlaying);
            Update(); // This is necessary after an invalidate.
        }
    }

    public class SongGridViewBackgroundWorkerResult
    {
        public SongDTO Song { get; set; }
        public Image AlbumArt { get; set; }
    }

    public class SongGridViewBackgroundWorkerArgument
    {
        public SongDTO Song { get; set; }
        public int LineIndex { get; set; }        
    }
}
