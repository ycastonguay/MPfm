//
// SongGridView.cs: This custom control is a grid view displaying songs from the 
//                  user library. It can be used to display playlist contents.
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
using MPfm.Player;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This custom grid view control displays the MPfm library.
    /// </summary>
    public partial class SongGridView 
        : System.Windows.Forms.Control, IMessageFilter
    {
        #region Private Variables

        /// <summary>
        /// Embedded font collection used for drawing.
        /// </summary>
        private EmbeddedFontCollection m_embeddedFonts = null;

        // Mode
        private SongGridViewMode m_mode = SongGridViewMode.AudioFile;

        // Controls
        private System.Windows.Forms.VScrollBar m_vScrollBar = null;
        private System.Windows.Forms.HScrollBar m_hScrollBar = null;
        private ContextMenuStrip m_menuColumns = null;

        // Background worker for updating album art
        private int m_preloadLinesAlbumCover = 20;
        private BackgroundWorker m_workerUpdateAlbumArt = null;
        private List<SongGridViewBackgroundWorkerArgument> m_workerUpdateAlbumArtPile = null;
        private System.Windows.Forms.Timer m_timerUpdateAlbumArt = null;
        //private List<string> m_currentlyVisibleAlbumArt = null;

        // Cache        
        private SongGridViewCache m_songCache = null;
        private List<SongGridViewImageCache> m_imageCache = new List<SongGridViewImageCache>();        

        // Private variables used for mouse events
        private int m_columnMoveMarkerX = 0;
        private int m_startLineNumber = 0;
        private int m_numberOfLinesToDraw = 0;
        private int m_minimumColumnWidth = 30;
        private int m_dragStartX = -1;
        private int m_dragOriginalColumnWidth = -1;
        private bool m_isMouseOverControl = false;
        private bool m_isUserHoldingLeftMouseButton = false;        
        private int m_lastItemIndexClicked = -1;

        // Animation timer and counter for currently playing song
        private int m_timerAnimationNowPlayingCount = 0;
        private Rectangle m_rectNowPlaying = new Rectangle(0, 0, 1, 1);
        private System.Windows.Forms.Timer m_timerAnimationNowPlaying = null;

        #endregion

        #region Events

        /// <summary>
        /// Delegate method for the OnSelectedItemChanged event.
        /// </summary>
        /// <param name="data">SelectedIndexChanged data</param>
        public delegate void SelectedIndexChanged(SongGridViewSelectedIndexChangedData data);
        /// <summary>
        /// The OnSelectedIndexChanged event is triggered when the selected item(s) have changed.
        /// </summary>
        public event SelectedIndexChanged OnSelectedIndexChanged;

        /// <summary>
        /// Delegate method for the OnColumnClick event.
        /// </summary>
        /// <param name="data">SongGridViewColumnClickData data</param>
        public delegate void ColumnClick(SongGridViewColumnClickData data);
        /// <summary>
        /// The ColumnClick event is triggered when the user has clicked on one of the columns.
        /// </summary>
        public event ColumnClick OnColumnClick;

        #endregion

        #region Properties
        
        /// <summary>
        /// Private value for the Theme property.
        /// </summary>
        private SongGridViewTheme m_theme = null;
        /// <summary>
        /// Defines the current theme used for rendering the control.
        /// </summary>
        public SongGridViewTheme Theme
        {
            get
            {
                return m_theme;
            }
            set
            {
                m_theme = value;
            }
        }

        #region Other Properties (Items, Columns, Menus, etc.)

        /// <summary>
        /// Private value for the Items property.
        /// </summary>
        private List<SongGridViewItem> m_items = null;
        /// <summary>
        /// List of grid view items (representing songs).
        /// </summary>
        [Browsable(false)]
        public List<SongGridViewItem> Items
        {
            get
            {
                return m_items;
            }
        }

        /// <summary>
        /// Returns the list of selected items.
        /// </summary>
        [Browsable(false)]
        public List<SongGridViewItem> SelectedItems
        {
            get
            {
                // Check if the item list exists
                if (m_items != null)
                {
                    return m_items.Where(x => x.IsSelected).ToList();
                }

                return null;
            }
        }

        /// <summary>
        /// Private value for the Columns property.
        /// </summary>
        private List<SongGridViewColumn> m_columns = null;
        /// <summary>
        /// List of grid view columns.
        /// </summary>
        [Browsable(false)]
        public List<SongGridViewColumn> Columns
        {
            get
            {
                return m_columns;
            }
        }


        /// <summary>
        /// Private value for the NowPlayingAudioFileId property.
        /// </summary>
        private Guid m_nowPlayingAudioFileId = Guid.Empty;
        /// <summary>
        /// Defines the currently playing audio file identifier.
        /// </summary>
        [Browsable(false)]
        public Guid NowPlayingAudioFileId
        {
            get
            {
                return m_nowPlayingAudioFileId;
            }
            set
            {
                m_nowPlayingAudioFileId = value;
            }
        }

        /// <summary>
        /// Private value for the NowPlayingPlaylistItemId property.
        /// </summary>
        private Guid m_nowPlayingPlaylistItemId = Guid.Empty;
        /// <summary>
        /// Defines the currently playing playlist item identifier.
        /// </summary>
        [Browsable(false)]
        public Guid NowPlayingPlaylistItemId
        {
            get
            {
                return m_nowPlayingPlaylistItemId;
            }
            set
            {
                m_nowPlayingPlaylistItemId = value;
            }
        }

        /// <summary>
        /// Private value for the ContextMenuStrip property.
        /// </summary>
        private ContextMenuStrip m_contextMenuStrip = null;
        /// <summary>
        /// ContextMenuStrip related to the grid. This context menu
        /// opens when the user right clicks an item.
        /// </summary>
        [Category("Misc"), Browsable(true), Description("Stuff.")]
        public override ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return m_contextMenuStrip;
            }
            set
            {
                m_contextMenuStrip = value;
            }
        }

        #endregion

        #region Filter / OrderBy Properties

        /// <summary>
        /// Private value for the OrderByFieldName property.
        /// </summary>
        private string m_orderByFieldName = string.Empty;
        /// <summary>
        /// Indicates which field should be used for ordering songs.
        /// </summary>
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

        /// <summary>
        /// Private value for the OrderByAscending property.
        /// </summary>
        private bool m_orderByAscending = true;
        /// <summary>
        /// Indicates if the order should be ascending (true) or descending (false).
        /// </summary>
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

        /// <summary>
        /// Private value for the DisplayDebugInformation property.
        /// </summary>
        private bool m_displayDebugInformation = false;
        /// <summary>
        /// If true, the debug information will be shown over the first column.
        /// </summary>
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

        /// <summary>
        /// Private value for the CanResizeColumns property.
        /// </summary>
        private bool m_canResizeColumns = true;
        /// <summary>
        /// Indicates if the user can resize the columns or not.
        /// </summary>
        public bool CanResizeColumns
        {
            get
            {
                return m_canResizeColumns;
            }
            set
            {
                m_canResizeColumns = value;
            }
        }

        /// <summary>
        /// Private value for the CanMoveColumns property.
        /// </summary>
        private bool m_canMoveColumns = true;
        /// <summary>
        /// Indicates if the user can move the columns or not.
        /// </summary>
        public bool CanMoveColumns
        {
            get
            {
                return m_canMoveColumns;
            }
            set
            {
                m_canMoveColumns = value;
            }
        }

        /// <summary>
        /// Private value for the CanChangeOrderBy property.
        /// </summary>
        private bool m_canChangeOrderBy = true;
        /// <summary>
        /// Indicates if the user can change the order by or not.
        /// </summary>
        public bool CanChangeOrderBy
        {
            get
            {
                return m_canChangeOrderBy;
            }
            set
            {
                m_canChangeOrderBy = value;
            }
        }

        /// <summary>
        /// Private value for the CanReorderItems property.
        /// </summary>
        private bool m_canReorderItems = true;
        /// <summary>
        /// Indicates if the user can reorder the items or not.
        /// </summary>
        public bool CanReorderItems
        {
            get
            {
                return m_canReorderItems;
            }
            set
            {
                m_canReorderItems = value;
            }
        }

        /// <summary>
        /// Private value for the ImageCacheSize property.
        /// </summary>             
        private int m_imageCacheSize = 10;
        /// <summary>
        /// Defines the size of the album art image cache (10 by default).
        /// </summary>
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

        /// <summary>
        /// Indicates if a column is currently moving.
        /// </summary>
        public bool IsColumnMoving
        {
            get
            {
                // Loop through columns
                foreach (SongGridViewColumn column in m_columns)
                {
                    // Check if column is moving
                    if (column.IsUserMovingColumn)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Indicates if a column is currently resizing.
        /// </summary>
        public bool IsColumnResizing
        {
            get
            {
                // Loop through columns
                foreach (SongGridViewColumn column in m_columns)
                {
                    // Check if column is moving
                    if (column.IsUserResizingColumn)
                    {
                        return true;
                    }
                }

                return false;
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

            // Create default theme
            m_theme = new SongGridViewTheme();

            // Get embedded font collection
            m_embeddedFonts = EmbeddedFontHelper.GetEmbeddedFonts();

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

            // Create columns
            SongGridViewColumn columnSongAlbumCover = new SongGridViewColumn("Album Cover", string.Empty, true, 0);
            SongGridViewColumn columnSongNowPlaying = new SongGridViewColumn("Now Playing", string.Empty, true, 1);
            SongGridViewColumn columnSongFileType = new SongGridViewColumn("Type", "FileType", false, 2);
            SongGridViewColumn columnSongTrackNumber = new SongGridViewColumn("Tr#", "DiscTrackNumber", true, 3);
            SongGridViewColumn columnSongTrackCount = new SongGridViewColumn("Track Count", "TrackCount", false, 4);
            SongGridViewColumn columnSongFilePath = new SongGridViewColumn("File Path", "FilePath", false, 5);
            SongGridViewColumn columnSongTitle = new SongGridViewColumn("Song Title", "Title", true, 6);
            SongGridViewColumn columnSongLength = new SongGridViewColumn("Length", "Length", true, 7);
            SongGridViewColumn columnSongArtistName = new SongGridViewColumn("Artist Name", "ArtistName", true, 8);
            SongGridViewColumn columnSongAlbumTitle = new SongGridViewColumn("Album Title", "AlbumTitle", true, 9);
            SongGridViewColumn columnSongGenre = new SongGridViewColumn("Genre", "Genre", false, 10);
            SongGridViewColumn columnSongPlayCount = new SongGridViewColumn("Play Count", "PlayCount", true, 11);
            SongGridViewColumn columnSongLastPlayed = new SongGridViewColumn("Last Played", "LastPlayed", true, 12);
            SongGridViewColumn columnSongBitrate = new SongGridViewColumn("Bitrate", "Bitrate", false, 13);
            SongGridViewColumn columnSongSampleRate = new SongGridViewColumn("Sample Rate", "SampleRate", false, 14);
            SongGridViewColumn columnSongTempo = new SongGridViewColumn("Tempo", "Tempo", false, 15);
            SongGridViewColumn columnSongYear = new SongGridViewColumn("Year", "Year", false, 16);

            // Set visible column titles
            columnSongAlbumCover.IsHeaderTitleVisible = false;
            columnSongNowPlaying.IsHeaderTitleVisible = false;

            // Set additional flags
            columnSongAlbumCover.CanBeReordered = false;
            columnSongNowPlaying.CanBeReordered = false;
            columnSongNowPlaying.CanBeResized = false;

            // Set default widths
            columnSongAlbumCover.Width = 200;
            columnSongNowPlaying.Width = 20;
            columnSongFileType.Width = 40;
            columnSongTrackNumber.Width = 30;
            columnSongTrackCount.Width = 80;
            columnSongFilePath.Width = 200;
            columnSongTitle.Width = 200;
            columnSongLength.Width = 70;
            columnSongArtistName.Width = 140;
            columnSongAlbumTitle.Width = 140;
            columnSongGenre.Width = 140;
            columnSongPlayCount.Width = 50;
            columnSongLastPlayed.Width = 80;
            columnSongBitrate.Width = 50;
            columnSongSampleRate.Width = 50;
            columnSongTempo.Width = 40;
            columnSongYear.Width = 40;

            // Add columns to list
            m_columns = new List<SongGridViewColumn>();
            m_columns.Add(columnSongAlbumCover);
            m_columns.Add(columnSongAlbumTitle);
            m_columns.Add(columnSongArtistName);
            m_columns.Add(columnSongBitrate);            
            m_columns.Add(columnSongFilePath);            
            m_columns.Add(columnSongGenre);
            m_columns.Add(columnSongLastPlayed);
            m_columns.Add(columnSongLength);
            m_columns.Add(columnSongNowPlaying);
            m_columns.Add(columnSongPlayCount);
            m_columns.Add(columnSongSampleRate);
            m_columns.Add(columnSongTitle); // Song title
            m_columns.Add(columnSongTempo);            
            m_columns.Add(columnSongTrackNumber);
            m_columns.Add(columnSongTrackCount);
            m_columns.Add(columnSongFileType); // Type
            m_columns.Add(columnSongYear);

            // Create contextual menu
            m_menuColumns = new System.Windows.Forms.ContextMenuStrip();

            // Loop through columns
            foreach (SongGridViewColumn column in m_columns)
            {
                // Add menu item                               
                ToolStripMenuItem menuItem = (ToolStripMenuItem)m_menuColumns.Items.Add(column.Title);
                menuItem.Tag = column.Title;
                menuItem.Checked = column.Visible;
                menuItem.Click += new EventHandler(menuItemColumns_Click);
            }
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
                        //if (arg.LineIndex < m_startLineNumber || arg.LineIndex > m_startLineNumber + m_numberOfLinesToDraw + m_preloadLinesAlbumCover)
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
            result.AudioFile = arg.AudioFile;

            // Check if this album is still visible (cancel if it is out of display).     
            if (arg.LineIndex < m_startLineNumber || arg.LineIndex > m_startLineNumber + m_numberOfLinesToDraw + m_preloadLinesAlbumCover)
            {
                // Set result with empty image
                e.Result = result;
                return;
            }

            // Extract image from file
            Image imageAlbumCover = AudioFile.ExtractImageForAudioFile(arg.AudioFile.FilePath);

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

            // Create cover art cache (even if the albumart is null, just to make sure the grid doesn't refetch the album art endlessly)
            SongGridViewImageCache cache = new SongGridViewImageCache();
            cache.Key = result.AudioFile.ArtistName + "_" + result.AudioFile.AlbumTitle;
            cache.Image = result.AlbumArt;

            // We found cover art! Add to cache and get out of the loop
            m_imageCache.Add(cache);

            // Check if the cache size has been reached
            if (m_imageCache.Count > m_imageCacheSize)
            {
                // Check if the image needs to be disposed
                if (m_imageCache[0].Image != null)
                {
                    // Dispose image
                    Image imageTemp = m_imageCache[0].Image;
                    imageTemp.Dispose();
                    imageTemp = null;
                }

                // Remove the oldest item
                m_imageCache.RemoveAt(0);
            }            

            // Remove song from list
            int indexRemove = -1;
            for (int a = 0; a < m_workerUpdateAlbumArtPile.Count; a++)
            {
                if (m_workerUpdateAlbumArtPile[a].AudioFile.FilePath.ToUpper() == result.AudioFile.FilePath.ToUpper())
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

        /// <summary>
        /// Clears the currently selected items.
        /// </summary>
        public void ClearSelectedItems()
        {
            // Loop through items
            foreach (SongGridViewItem item in m_items)
            {
                // Check if item is selected
                if (item.IsSelected)
                {
                    // Unselect item
                    item.IsSelected = false;
                }
            }

            // Refresh control
            Refresh();
        }

        /// <summary>
        /// Imports audio files as SongGridViewItems.
        /// </summary>
        /// <param name="audioFiles">List of AudioFiles</param>
        public void ImportAudioFiles(List<AudioFile> audioFiles)
        {
            // Set mode
            m_mode = SongGridViewMode.AudioFile;

            // Create list of items
            m_items = new List<SongGridViewItem>();
            foreach (AudioFile audioFile in audioFiles)
            {
                // Create item and add to list
                SongGridViewItem item = new SongGridViewItem();
                item.AudioFile = audioFile;
                item.PlaylistItemId = Guid.NewGuid();
                m_items.Add(item);
            }

            // Reset scrollbar position
            m_vScrollBar.Value = 0;
            m_songCache = null;

            // Refresh control
            Refresh();
        }

        /// <summary>
        /// Imports playlist items as SongGridViewItems.
        /// </summary>
        /// <param name="playlist">Playlist</param>
        public void ImportPlaylist(Playlist playlist)
        {
            // Set mode
            m_mode = SongGridViewMode.Playlist;

            // Create list of items
            m_items = new List<SongGridViewItem>();
            foreach (PlaylistItem playlistItem in playlist.Items)
            {
                // Create item and add to list
                SongGridViewItem item = new SongGridViewItem();
                item.AudioFile = playlistItem.AudioFile;
                item.PlaylistItemId = playlistItem.Id;
                m_items.Add(item);
            }

            // Reset scrollbar position
            m_vScrollBar.Value = 0;
            m_songCache = null;

            // Refresh control
            Refresh();
        }

        #endregion

        #region Paint Events

        /// <summary>
        /// Update a specific line (if visible) by its audio file unique identifier.
        /// </summary>
        /// <param name="audioFileId">Audio file unique identifier</param>
        public void UpdateAudioFileLine(Guid audioFileId)
        {
            // Find the position of the line            
            for (int a = m_startLineNumber; a < m_startLineNumber + m_numberOfLinesToDraw; a++)
            {
                // Calculate offset
                int offsetY = (a * m_songCache.LineHeight) - m_vScrollBar.Value + m_songCache.LineHeight;

                // Check if the line matches
                if (m_items[a].AudioFile.Id == audioFileId)
                {
                    // Invalidate this line
                    Invalidate(new Rectangle(m_columns[0].Width - m_hScrollBar.Value, offsetY, ClientRectangle.Width - m_columns[0].Width + m_hScrollBar.Value, m_songCache.LineHeight));

                    // Update and exit loop
                    Update();
                    break;
                }
            }
        }

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
        /// <param name="e">Paint Event arguments</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            // Check if the library is valid
            if (m_items == null)
            {
                return;
            }

            // Draw bitmap for control
            Graphics g = e.Graphics;

            // Set text anti-aliasing to ClearType (best looking AA)
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Set smoothing mode for paths
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Paint songs
            PaintSongs(ref g);         

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
            Color colorNowPlaying1 = m_theme.LineNowPlayingColor1;
            Color colorNowPlaying2 = m_theme.LineNowPlayingColor2;
            int offsetX = 0;
            int offsetY = 0;
            int albumCoverStartIndex = 0;
            int albumCoverEndIndex = 0;
            string currentAlbumTitle = string.Empty;
            //bool regenerateItems = true;
            bool nowPlayingSongFound = false;

            //// Load custom font
            //Font fontDefault = Tools.LoadCustomFont(FontCollection, CustomFontName, Font.Size, FontStyle.Regular);
            //Font fontDefaultBold = Tools.LoadCustomFont(FontCollection, CustomFontName, Font.Size, FontStyle.Bold);
            Font fontDefault = null;
            Font fontDefaultBold = null;

            // Make sure the embedded font name needs to be loaded and is valid
            if (m_theme.Font.UseEmbeddedFont && !String.IsNullOrEmpty(m_theme.Font.EmbeddedFontName))
            {
                try
                {
                    // Get embedded fonts
                    fontDefault = Tools.LoadEmbeddedFont(m_embeddedFonts, m_theme.Font.EmbeddedFontName, m_theme.Font.Size, m_theme.Font.ToFontStyle());
                    fontDefaultBold = Tools.LoadEmbeddedFont(m_embeddedFonts, m_theme.Font.EmbeddedFontName, m_theme.Font.Size, m_theme.Font.ToFontStyle() | FontStyle.Bold);
                }
                catch
                {
                    // Use default font instead
                    fontDefault = this.Font;
                    fontDefaultBold = new Font(this.Font, FontStyle.Bold);
                }
            }

            // Check if font is null
            if (fontDefault == null)
            {
                try
                {
                    // Try to get standard font
                    fontDefault = new Font(m_theme.Font.StandardFontName, m_theme.Font.Size, m_theme.Font.ToFontStyle());
                    fontDefaultBold = new Font(m_theme.Font.StandardFontName, m_theme.Font.Size, m_theme.Font.ToFontStyle() | FontStyle.Bold);
                }
                catch
                {
                    // Use default font instead
                    fontDefault = this.Font;
                    fontDefaultBold = new Font(this.Font, FontStyle.Bold);
                }
            }

            //// Load default fonts if custom fonts were not found
            //if (fontDefault == null)
            //{
            //    // Load default font
            //    fontDefault = new Font(Font.FontFamily.Name, Font.Size, FontStyle.Regular);
            //}
            //if (fontDefaultBold == null)
            //{
            //    // Load default font
            //    fontDefaultBold = new Font(Font.FontFamily.Name, Font.Size, FontStyle.Bold);
            //}

            // Set string format
            StringFormat stringFormat = new StringFormat();

            //// Check for an existing collection of items
            //if (m_items != null)
            //{
            //    // Check the first item, if there's one
            //    if (m_items.Count > 0 && m_items[0] is SongGridViewItem)
            //    {
            //        regenerateItems = false;
            //    }
            //}

            // If there are no items..,
            if (m_items == null)
            {
                // Do not do anything.
                return;
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
                // Get audio file
                AudioFile audioFile = m_items[a].AudioFile;

                // Calculate Y offset (compensate for scrollbar position)
                offsetY = (a * m_songCache.LineHeight) - m_vScrollBar.Value + m_songCache.LineHeight;

                // Calculate album art cover column width
                int albumArtColumnWidth = m_columns[0].Visible ? m_columns[0].Width : 0;

                // Calculate line background width
                int lineBackgroundWidth = ClientRectangle.Width + m_hScrollBar.Value - albumArtColumnWidth;

                // Reduce width from scrollbar if visible
                if (m_vScrollBar.Visible)
                {
                    lineBackgroundWidth -= m_vScrollBar.Width;
                }

                // Set rectangle                
                Rectangle rectBackground = new Rectangle(albumArtColumnWidth - m_hScrollBar.Value, offsetY, lineBackgroundWidth, m_songCache.LineHeight);                
                
                // Set default line background color
                Color colorBackground1 = m_theme.LineColor1;
                Color colorBackground2 = m_theme.LineColor2;

                // Check conditions to determine background color
                if ((m_mode == SongGridViewMode.AudioFile && audioFile.Id == m_nowPlayingAudioFileId) || 
                    (m_mode == SongGridViewMode.Playlist && m_items[a].PlaylistItemId == m_nowPlayingPlaylistItemId))
                {
                    // Set color             
                    colorBackground1 = m_theme.LineNowPlayingColor1;
                    colorBackground2 = m_theme.LineNowPlayingColor2;
                }

                // Check if item is selected
                if (m_items[a].IsSelected)
                {
                    // Use darker color
                    int diff = 40;
                    colorBackground1 = Color.FromArgb(255,
                        (colorBackground1.R - diff < 0) ? 0 : colorBackground1.R - diff,
                        (colorBackground1.G - diff < 0) ? 0 : colorBackground1.G - diff,
                        (colorBackground1.B - diff < 0) ? 0 : colorBackground1.B - diff);
                    colorBackground2 = Color.FromArgb(255,
                        (colorBackground2.R - diff < 0) ? 0 : colorBackground2.R - diff,
                        (colorBackground2.G - diff < 0) ? 0 : colorBackground2.G - diff,
                        (colorBackground2.B - diff < 0) ? 0 : colorBackground2.B - diff);
                }

                // Check if mouse is over item
                if (m_items[a].IsMouseOverItem)
                {
                    // Use lighter color
                    int diff = 20;
                    colorBackground1 = Color.FromArgb(255,
                        (colorBackground1.R + diff > 255) ? 255 : colorBackground1.R + diff,
                        (colorBackground1.G + diff > 255) ? 255 : colorBackground1.G + diff,
                        (colorBackground1.B + diff > 255) ? 255 : colorBackground1.B + diff);
                    colorBackground2 = Color.FromArgb(255,
                        (colorBackground2.R + diff > 255) ? 255 : colorBackground2.R + diff,
                        (colorBackground2.G + diff > 255) ? 255 : colorBackground2.G + diff,
                        (colorBackground2.B + diff > 255) ? 255 : colorBackground2.B + diff);
                }

                // Check conditions to determine background color
                if ((m_mode == SongGridViewMode.AudioFile && audioFile.Id == m_nowPlayingAudioFileId) ||
                    (m_mode == SongGridViewMode.Playlist && m_items[a].PlaylistItemId == m_nowPlayingPlaylistItemId))
                {
                    // Set color             
                    colorNowPlaying1 = colorBackground1;
                    colorNowPlaying2 = colorBackground2;
                }

                // Create gradient
                brushGradient = new LinearGradientBrush(rectBackground, colorBackground1, colorBackground2, 90);
                g.FillRectangle(brushGradient, rectBackground);
                brushGradient.Dispose();
                brushGradient = null;

                // Loop through columns                
                offsetX = 0;
                for (int b = 0; b < m_songCache.ActiveColumns.Count; b++)
                {
                    // Get current column
                    SongGridViewColumn column = m_songCache.ActiveColumns[b];

                    // Check if the column is visible
                    if (column.Visible)
                    {
                        // Check if this is the "Now playing" column
                        if (column.Title == "Now Playing")
                        {
                            // Draw now playing icon
                            if ((m_mode == SongGridViewMode.AudioFile && audioFile.Id == m_nowPlayingAudioFileId) ||
                                (m_mode == SongGridViewMode.Playlist && m_items[a].PlaylistItemId == m_nowPlayingPlaylistItemId))
                            {
                                // Which size is the minimum? Width or height?                    
                                int availableWidthHeight = column.Width - 4;
                                if (m_songCache.LineHeight <= column.Width)
                                {
                                    //availableWidthHeight = m_columns[1].Width - m_padding;
                                    availableWidthHeight = m_songCache.LineHeight - 4;
                                }
                                else
                                {
                                    availableWidthHeight = column.Width - 4;
                                }

                                // Calculate the icon position                                
                                float iconNowPlayingX = ((column.Width - availableWidthHeight) / 2) + offsetX - m_hScrollBar.Value;
                                float iconNowPlayingY = offsetY + ((m_songCache.LineHeight - availableWidthHeight) / 2);

                                // Create NowPlaying rect (MUST be in integer)                    
                                m_rectNowPlaying = new Rectangle((int)iconNowPlayingX, (int)iconNowPlayingY, availableWidthHeight, availableWidthHeight);
                                nowPlayingSongFound = true;

                                // Draw outer circle
                                brushGradient = new LinearGradientBrush(m_rectNowPlaying, Color.FromArgb(50, m_theme.IconNowPlayingColor1.R, m_theme.IconNowPlayingColor1.G, m_theme.IconNowPlayingColor1.B), m_theme.IconNowPlayingColor2, m_timerAnimationNowPlayingCount % 360);
                                g.FillEllipse(brushGradient, m_rectNowPlaying);
                                brushGradient.Dispose();
                                brushGradient = null;

                                // Draw inner circle
                                rect = new Rectangle((int)iconNowPlayingX + 4, (int)iconNowPlayingY + 4, availableWidthHeight - 8, availableWidthHeight - 8);
                                brush = new SolidBrush(colorNowPlaying1);
                                g.FillEllipse(brush, rect);
                                brush.Dispose();
                                brush = null;
                            }
                        }
                        else if (column.Title == "Album Cover")
                        {
                            #region Album Cover Zone

                            // Check for an album title change (or the last item of the grid)
                            if (currentAlbumTitle != audioFile.AlbumTitle)
                            {
                                // Set the new current album title
                                currentAlbumTitle = audioFile.AlbumTitle;

                                // For displaying the album cover, we need to know how many songs of the same album are bundled together
                                // Start by getting the start index
                                for (int c = a; c > 0; c--)
                                {
                                    // Get audio file
                                    AudioFile previousAudioFile = m_items[c].AudioFile;

                                    // Check if the album title matches
                                    if (previousAudioFile.AlbumTitle != audioFile.AlbumTitle)
                                    {
                                        // Set album cover start index (+1 because the last song was the sound found in the previous loop iteration)
                                        albumCoverStartIndex = c + 1;
                                        break;
                                    }
                                }
                                // Find the end index
                                for (int c = a + 1; c < m_items.Count; c++)
                                {
                                    // Get audio file
                                    AudioFile nextAudioFile = m_items[c].AudioFile;

                                    // If the album title is different, this means we found the next album title
                                    if (nextAudioFile.AlbumTitle != audioFile.AlbumTitle)
                                    {
                                        // Set album cover end index (-1 because the last song was the song found in the previous loop iteration)
                                        albumCoverEndIndex = c - 1;
                                        break;
                                    }
                                    // If this is the last item of the grid...
                                    else if (c == m_items.Count - 1)
                                    {
                                        // Set album cover end index as the last item of the grid
                                        albumCoverEndIndex = c;
                                        break;
                                    }
                                }

                                // Calculate y and height
                                int scrollbarOffsetY = (m_startLineNumber * m_songCache.LineHeight) - m_vScrollBar.Value;
                                int y = ((albumCoverStartIndex - m_startLineNumber) * m_songCache.LineHeight) + m_songCache.LineHeight + scrollbarOffsetY;

                                // Calculate the height of the album cover zone (+1 on end index because the array is zero-based)
                                int albumCoverZoneHeight = (albumCoverEndIndex + 1 - albumCoverStartIndex) * m_songCache.LineHeight;

                                int heightWithPadding = albumCoverZoneHeight - (m_theme.Padding * 2);
                                if (heightWithPadding > m_songCache.ActiveColumns[0].Width - (m_theme.Padding * 2))
                                {
                                    heightWithPadding = m_songCache.ActiveColumns[0].Width - (m_theme.Padding * 2);
                                }

                                // Make sure the height is at least zero (not necessary to draw anything!)
                                if (albumCoverZoneHeight > 0)
                                {
                                    // Draw album cover background
                                    Rectangle rectAlbumCover = new Rectangle(0 - m_hScrollBar.Value, y, m_songCache.ActiveColumns[0].Width, albumCoverZoneHeight);
                                    brushGradient = new LinearGradientBrush(rectAlbumCover, m_theme.AlbumCoverBackgroundColor1, m_theme.AlbumCoverBackgroundColor2, 90);
                                    g.FillRectangle(brushGradient, rectAlbumCover);
                                    brushGradient.Dispose();
                                    brushGradient = null;

                                    // Try to extract image from cache
                                    Image imageAlbumCover = null;
                                    SongGridViewImageCache cachedImage = m_imageCache.FirstOrDefault(x => x.Key == audioFile.ArtistName + "_" + audioFile.AlbumTitle);
                                    if (cachedImage != null)
                                    {
                                        // Set image
                                        imageAlbumCover = cachedImage.Image;
                                    }

                                    // Album art not found in cache; try to find an album cover in one of the file
                                    if (cachedImage == null)
                                    {
                                        // Check if the album cover is already in the pile
                                        bool albumCoverFound = false;
                                        foreach (SongGridViewBackgroundWorkerArgument arg in m_workerUpdateAlbumArtPile)
                                        {
                                            // Match by file path
                                            if (arg.AudioFile.FilePath.ToUpper() == audioFile.FilePath.ToUpper())
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
                                            arg.AudioFile = audioFile;
                                            arg.LineIndex = a;
                                            m_workerUpdateAlbumArtPile.Add(arg);
                                        }
                                    }

                                    // Measure available width for text
                                    int widthAvailableForText = m_columns[0].Width - (m_theme.Padding * 2);

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
                                        sizeArtistName = g.MeasureString(audioFile.ArtistName, fontDefaultBold, widthAvailableForText, stringFormat);
                                        sizeAlbumTitle = g.MeasureString(currentAlbumTitle, fontDefault, widthAvailableForText - (int)sizeArtistName.Width, stringFormat);

                                        // Display artist name at full width first, then album name
                                        rectArtistNameText = new RectangleF(m_theme.Padding - m_hScrollBar.Value, y + (m_theme.Padding / 2), widthAvailableForText, m_songCache.LineHeight);
                                        rectAlbumTitleText = new RectangleF(m_theme.Padding - m_hScrollBar.Value + sizeArtistName.Width + m_theme.Padding, y + (m_theme.Padding / 2), widthAvailableForText - sizeArtistName.Width, m_songCache.LineHeight);
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
                                            sizeArtistName = g.MeasureString(audioFile.ArtistName, fontDefaultBold, widthAvailableForText, stringFormat);
                                            sizeAlbumTitle = g.MeasureString(currentAlbumTitle, fontDefault, widthAvailableForText, stringFormat);

                                            // Display the album title at the top of the zome
                                            rectArtistNameText = new RectangleF(m_theme.Padding - m_hScrollBar.Value, y + m_theme.Padding, widthAvailableForText, heightWithPadding);
                                            rectAlbumTitleText = new RectangleF(m_theme.Padding - m_hScrollBar.Value, y + m_theme.Padding + sizeArtistName.Height, widthAvailableForText, heightWithPadding);
                                        }
                                        else
                                        {
                                            // There is an album cover to display with more than 2 lines.
                                            // Set string format
                                            stringFormat.Alignment = StringAlignment.Near;
                                            stringFormat.Trimming = StringTrimming.EllipsisWord;

                                            // Measure strings
                                            sizeArtistName = g.MeasureString(audioFile.ArtistName, fontDefaultBold, widthAvailableForText, stringFormat);
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

                                                float widthRemainingForText = m_columns[0].Width - m_theme.Padding - heightWithPadding;

                                                // Measure strings
                                                sizeArtistName = g.MeasureString(audioFile.ArtistName, fontDefaultBold, new SizeF(widthRemainingForText, heightWithPadding), stringFormat);
                                                sizeAlbumTitle = g.MeasureString(currentAlbumTitle, fontDefault, new SizeF(widthRemainingForText, heightWithPadding), stringFormat);

                                                // Try to center the cover art + padding + max text width
                                                float maxWidth = sizeArtistName.Width;
                                                if (sizeAlbumTitle.Width > maxWidth)
                                                {
                                                    // Set new maximal width
                                                    maxWidth = sizeAlbumTitle.Width;
                                                }

                                                useAlbumArtOverlay = true;

                                                float albumCoverX = (m_columns[0].Width - heightWithPadding - m_theme.Padding - m_theme.Padding - maxWidth) / 2;
                                                float artistNameY = (albumCoverZoneHeight - sizeArtistName.Height - sizeAlbumTitle.Height) / 2;

                                                // Display the album title at the top of the zome
                                                rectArtistNameText = new RectangleF(albumCoverX + heightWithPadding + m_theme.Padding - m_hScrollBar.Value, y + artistNameY, widthRemainingForText, heightWithPadding);
                                                rectAlbumTitleText = new RectangleF(albumCoverX + heightWithPadding + m_theme.Padding - m_hScrollBar.Value, y + artistNameY + sizeArtistName.Height, widthRemainingForText, heightWithPadding);

                                                // Set cover art rectangle
                                                rectAlbumCoverArt = new RectangleF(albumCoverX - m_hScrollBar.Value, y + m_theme.Padding, heightWithPadding, heightWithPadding);
                                            }
                                            // 7 and more lines
                                            else
                                            {
                                                // Display artist name at the top of the album cover; display album title at the bottom of the album cover
                                                rectArtistNameText = new RectangleF(((m_columns[0].Width - sizeArtistName.Width) / 2) - m_hScrollBar.Value, y + (m_theme.Padding * 2), widthAvailableForText, heightWithPadding);
                                                rectAlbumTitleText = new RectangleF(((m_columns[0].Width - sizeAlbumTitle.Width) / 2) - m_hScrollBar.Value, y + heightWithPadding - sizeAlbumTitle.Height, widthAvailableForText, heightWithPadding);

                                                // Draw background overlay behind text
                                                useAlbumArtOverlay = true;

                                                // Try to horizontally center the album cover if it's not taking the whole width (less padding)
                                                float albumCoverX = m_theme.Padding;
                                                if (m_columns[0].Width > heightWithPadding)
                                                {
                                                    // Get position
                                                    albumCoverX = ((float)(m_columns[0].Width - heightWithPadding) / 2) - m_hScrollBar.Value;
                                                }

                                                // Set cover art rectangle
                                                rectAlbumCoverArt = new RectangleF(albumCoverX, y + m_theme.Padding, heightWithPadding, heightWithPadding);
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
                                        RectangleF rectArtistNameBackground = new RectangleF(rectArtistNameText.X - (m_theme.Padding / 2), rectArtistNameText.Y - (m_theme.Padding / 2), sizeArtistName.Width + m_theme.Padding, sizeArtistName.Height + m_theme.Padding);
                                        RectangleF rectAlbumTitleBackground = new RectangleF(rectAlbumTitleText.X - (m_theme.Padding / 2), rectAlbumTitleText.Y - (m_theme.Padding / 2), sizeAlbumTitle.Width + m_theme.Padding, sizeAlbumTitle.Height + m_theme.Padding);
                                        brush = new SolidBrush(Color.FromArgb(190, 0, 0, 0));
                                        g.FillRectangle(brush, rectArtistNameBackground);
                                        g.FillRectangle(brush, rectAlbumTitleBackground);
                                        brush.Dispose();
                                        brush = null;
                                    }

                                    // Check if this is the artist name column (set font to bold)
                                    g.DrawString(audioFile.ArtistName, fontDefaultBold, Brushes.White, rectArtistNameText, stringFormat);
                                    g.DrawString(currentAlbumTitle, fontDefault, Brushes.White, rectAlbumTitleText, stringFormat);

                                    // Draw horizontal line to distinguish albums
                                    // Part 1: Draw line over grid
                                    pen = new Pen(m_theme.AlbumCoverBackgroundColor1);
                                    g.DrawLine(pen, new Point(m_columns[0].Width, y), new Point(ClientRectangle.Width, y));
                                    pen.Dispose();
                                    pen = null;

                                    // Part 2: Draw line over album art zone, in a lighter color
                                    pen = new Pen(Color.FromArgb(115, 115, 115));
                                    g.DrawLine(pen, new Point(0, y), new Point(m_columns[0].Width, y));
                                    pen.Dispose();
                                    pen = null;
                                }
                            }

                            #endregion
                        }
                        else
                        {
                            // Get property through reflection
                            PropertyInfo propertyInfo = audioFile.GetType().GetProperty(column.FieldName);
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
                                        value = propertyInfo.GetValue(audioFile, null).ToString();
                                    }
                                    // Nullable Int64
                                    else if (propertyInfo.PropertyType.FullName.Contains("Int64") &&
                                            propertyInfo.PropertyType.FullName.Contains("Nullable"))
                                    {
                                        // Try to get the value
                                        long? longValue = (long?)propertyInfo.GetValue(audioFile, null);

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
                                        DateTime? dateTimeValue = (DateTime?)propertyInfo.GetValue(audioFile, null);

                                        // Check if null
                                        if (dateTimeValue.HasValue)
                                        {
                                            // Render to string
                                            value = dateTimeValue.Value.ToShortDateString() + " " + dateTimeValue.Value.ToShortTimeString();
                                        }
                                    }
                                    else if (propertyInfo.PropertyType.FullName.Contains("System.UInt32"))
                                    {
                                        // Try to get the value
                                        uint uintValue = (uint)propertyInfo.GetValue(audioFile, null);

                                        // Render to string
                                        value = uintValue.ToString();
                                    }
                                    else if (propertyInfo.PropertyType.FullName.Contains("System.Int32"))
                                    {
                                        // Try to get the value
                                        int intValue = (int)propertyInfo.GetValue(audioFile, null);

                                        // Render to string
                                        value = intValue.ToString();
                                    }
                                    else if (propertyInfo.PropertyType.FullName.Contains("MPfm.Sound.AudioFileFormat"))
                                    {
                                        // Try to get the value
                                        AudioFileFormat theValue = (AudioFileFormat)propertyInfo.GetValue(audioFile, null);

                                        // Render to string
                                        value = theValue.ToString();
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
                                rect = new Rectangle(offsetX - m_hScrollBar.Value, offsetY + (m_theme.Padding / 2), m_songCache.ActiveColumns[b].Width, m_songCache.LineHeight - m_theme.Padding + 2);
                                stringFormat.Trimming = StringTrimming.EllipsisCharacter;
                                stringFormat.Alignment = StringAlignment.Near;

                                // Check if this is the artist name column
                                brush = new SolidBrush(m_theme.LineForeColor);
                                if (column.FieldName == "ArtistName" || column.FieldName == "DiscTrackNumber")
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
                        }

                        // Increment offset by the column width
                        offsetX += column.Width;
                    }
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
            brushGradient = new LinearGradientBrush(rectBackgroundHeader, m_theme.HeaderColor1, m_theme.HeaderColor2, 90);
            g.FillRectangle(brushGradient, rectBackgroundHeader);
            brushGradient.Dispose();
            brushGradient = null;

            // Loop through columns
            offsetX = 0;
            for (int b = 0; b < m_songCache.ActiveColumns.Count; b++)
            {
                // Get current column
                SongGridViewColumn column = m_songCache.ActiveColumns[b];

                // Check if the column is visible
                if (column.Visible)
                {
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
                        rect = new Rectangle(offsetX - m_hScrollBar.Value, -1, column.Width, m_songCache.LineHeight + 1);
                        brushGradient = new LinearGradientBrush(rect, m_theme.HeaderHoverColor1, m_theme.HeaderHoverColor2, 90);
                        g.FillRectangle(brushGradient, rect);
                        brushGradient.Dispose();
                        brushGradient = null;
                    }
                    else if (column.IsUserMovingColumn)
                    {
                        // Draw header (for some reason, the Y must be set -1 to cover an area which isn't supposed to be displayed)                        
                        rect = new Rectangle(offsetX - m_hScrollBar.Value, -1, column.Width, m_songCache.LineHeight + 1);
                        brushGradient = new LinearGradientBrush(rect, Color.Blue, Color.Green, 90);
                        g.FillRectangle(brushGradient, rect);
                        brushGradient.Dispose();
                        brushGradient = null;
                    }

                    // Check if the header title must be displayed
                    if (m_songCache.ActiveColumns[b].IsHeaderTitleVisible)
                    {
                        // Display title                
                        Rectangle rectTitle = new Rectangle(offsetX - m_hScrollBar.Value, m_theme.Padding / 2, column.Width, m_songCache.LineHeight - m_theme.Padding + 2);
                        stringFormat.Trimming = StringTrimming.EllipsisCharacter;
                        brush = new SolidBrush(m_theme.HeaderForeColor);
                        g.DrawString(column.Title, fontDefaultBold, brush, rectTitle, stringFormat);
                        brush.Dispose();
                        brush = null;
                    }

                    // Draw column separator line; determine the height of the line
                    int columnHeight = ClientRectangle.Height;

                    // Determine the height of the line; if the items don't fit the control height...
                    if (m_items.Count < m_songCache.NumberOfLinesFittingInControl)
                    {
                        // Set height as the number of items (plus header)
                        columnHeight = (m_items.Count + 1) * m_songCache.LineHeight;
                    }

                    // Draw line
                    g.DrawLine(Pens.DarkGray, new Point(offsetX + column.Width - m_hScrollBar.Value, 0), new Point(offsetX + column.Width - m_hScrollBar.Value, columnHeight));

                    // Check if the column is ordered by
                    if (column.FieldName == m_orderByFieldName && !String.IsNullOrEmpty(column.FieldName))
                    {
                        // Create triangle points,,,
                        PointF[] ptTriangle = new PointF[3];

                        // ... depending on the order by ascending value
                        int triangleWidthHeight = 8;
                        int trianglePadding = (m_songCache.LineHeight - triangleWidthHeight) / 2;
                        if (m_orderByAscending)
                        {
                            // Create points for ascending
                            ptTriangle[0] = new PointF(offsetX + column.Width - triangleWidthHeight - (triangleWidthHeight / 2) - m_hScrollBar.Value, trianglePadding);
                            ptTriangle[1] = new PointF(offsetX + column.Width - triangleWidthHeight - m_hScrollBar.Value, m_songCache.LineHeight - trianglePadding);
                            ptTriangle[2] = new PointF(offsetX + column.Width - triangleWidthHeight - triangleWidthHeight - m_hScrollBar.Value, m_songCache.LineHeight - trianglePadding);
                        }
                        else
                        {
                            // Create points for descending
                            ptTriangle[0] = new PointF(offsetX + column.Width - triangleWidthHeight - (triangleWidthHeight / 2) - m_hScrollBar.Value, m_songCache.LineHeight - trianglePadding);
                            ptTriangle[1] = new PointF(offsetX + column.Width - triangleWidthHeight - m_hScrollBar.Value, trianglePadding);
                            ptTriangle[2] = new PointF(offsetX + column.Width - triangleWidthHeight - triangleWidthHeight - m_hScrollBar.Value, trianglePadding);
                        }

                        // Draw triangle
                        pen = new Pen(m_theme.HeaderForeColor);
                        g.DrawPolygon(pen, ptTriangle);
                        pen.Dispose();
                        pen = null;
                    }

                    // Increment offset by the column width
                    offsetX += column.Width;
                }
            }

            // Display column move marker
            if (IsColumnMoving)
            {
                // Draw marker
                pen = new Pen(Color.Red);
                g.DrawRectangle(pen, new Rectangle(m_columnMoveMarkerX - m_hScrollBar.Value, 0, 1, ClientRectangle.Height));
                pen.Dispose();
                pen = null;
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
        /// <param name="e">Event arguments</param>
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
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        protected void m_hScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            // If the 

            // Redraw control
            Refresh();
        }

        /// <summary>
        /// Occurs when the user changes the vertical scrollbar value.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
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
        /// <param name="e">Event arguments</param>
        protected override void OnMouseEnter(EventArgs e)
        {
            // Set flags
            m_isMouseOverControl = true;

            base.OnMouseEnter(e);
        }

        /// <summary>
        /// Occurs when the mouse cursor leaves the control.
        /// </summary>
        /// <param name="e">Event arguments</param>
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

            // Check if there's at least one item
            if (m_items.Count > 0)
            {
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
            }

            // Reset column flags
            int columnOffsetX2 = 0;
            for (int b = 0; b < m_songCache.ActiveColumns.Count; b++)
            {
                // Make sure column is visible
                if (m_songCache.ActiveColumns[b].Visible)
                {
                    // Was mouse over this column header?
                    if (m_songCache.ActiveColumns[b].IsMouseOverColumnHeader)
                    {
                        // Reset flag
                        m_songCache.ActiveColumns[b].IsMouseOverColumnHeader = false;

                        // Invalidate region
                        Invalidate(new Rectangle(columnOffsetX2 - m_hScrollBar.Value, 0, m_songCache.ActiveColumns[b].Width, m_songCache.LineHeight));
                        controlNeedsToBeUpdated = true;
                    }

                    // Increment offset
                    columnOffsetX2 += m_songCache.ActiveColumns[b].Width;
                }
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
        /// <param name="e">Event arguments</param>
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
            foreach (SongGridViewColumn column in m_songCache.ActiveColumns)
            {
                // Check for resizing column
                if (column.IsMouseCursorOverColumnLimit && column.CanBeResized && CanResizeColumns)
                {
                    // Set resizing column flag
                    column.IsUserResizingColumn = true;

                    // Save the original column width
                    m_dragOriginalColumnWidth = column.Width;
                }
            }
            
            // Check if the left mouse button is held
            if(e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                // Set holding button flag
                m_isUserHoldingLeftMouseButton = true;
            }

            base.OnMouseDown(e);
        }

        /// <summary>
        /// Occurs when the user releases a mouse button.        
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            // Reset flags
            m_dragStartX = -1;
            bool updateControl = false;
            m_isUserHoldingLeftMouseButton = false;

            // Check if all the data is valid
            if (m_columns == null || m_songCache == null)
            {
                return;
            }

            // Loop through columns
            SongGridViewColumn columnMoving = null;
            foreach (SongGridViewColumn column in m_songCache.ActiveColumns)
            {
                // Reset flags
                column.IsUserResizingColumn = false;

                // Check if this column is moving
                if(column.IsUserMovingColumn)
                {
                    // Set column
                    columnMoving = column;
                }
            }

            // Check if the user is moving a column
            if (columnMoving != null)
            {
                // Set flag
                columnMoving.IsUserMovingColumn = false;
                updateControl = true;

                // Find out on what column the mouse cursor is
                SongGridViewColumn columnOver = null;
                int x = 0;
                bool isPastCurrentlyMovingColumn = false;
                for (int a = 0; a < m_songCache.ActiveColumns.Count; a++ )
                {
                    // Set current column
                    SongGridViewColumn currentColumn = m_songCache.ActiveColumns[a];

                    // Check if current column
                    if (currentColumn.FieldName == columnMoving.FieldName)
                    {
                        // Set flag
                        isPastCurrentlyMovingColumn = true;
                    }

                    // Check if the column is visible
                    if (currentColumn.Visible)
                    {
                        // Check if the cursor is over the left part of the column
                        if (e.X >= x - m_hScrollBar.Value && e.X <= x + (currentColumn.Width / 2) - m_hScrollBar.Value)
                        {
                            // Check flag
                            if (isPastCurrentlyMovingColumn && currentColumn.FieldName != columnMoving.FieldName)
                            {
                                // Set column
                                columnOver = m_songCache.ActiveColumns[a - 1];
                            }
                            else
                            {
                                // Set column
                                columnOver = m_songCache.ActiveColumns[a];
                            }
                            break;                               
                        }
                        // Check if the cursor is over the right part of the column
                        else if (e.X >= x + (currentColumn.Width / 2) - m_hScrollBar.Value && e.X <= x + currentColumn.Width - m_hScrollBar.Value)
                        {
                            // Check if there is a next item
                            if (a < m_songCache.ActiveColumns.Count - 1)
                            {
                                // Check flag
                                if (isPastCurrentlyMovingColumn)
                                {
                                    // Set column
                                    columnOver = m_songCache.ActiveColumns[a];
                                }
                                else
                                {
                                    // Set column
                                    columnOver = m_songCache.ActiveColumns[a + 1];
                                }
                            }

                            break;
                        }

                        // Increment x
                        x += currentColumn.Width;
                    }
                }

                //// Check if the column was found (the cursor might be past the last column
                //if (columnOver == null)
                //{
                //    return;
                //}

                // Order columns by their current order
                List<SongGridViewColumn> columnsOrdered = m_columns.OrderBy(q => q.Order).ToList();

                // Move column
                int indexRemove = -1;
                int indexAdd = -1;
                for (int a = 0; a < columnsOrdered.Count; a++)
                {
                    // Find the moving column index
                    if (columnsOrdered[a].FieldName == columnMoving.FieldName)
                    {
                        // Set index
                        indexRemove = a;
                    }

                    // Find the column index with the mouse over
                    if(columnOver != null && columnsOrdered[a].FieldName == columnOver.FieldName)
                    {
                        // Set index
                        indexAdd = a;
                    }
                }

                // Remove column
                columnsOrdered.RemoveAt(indexRemove);
                
                // Check if the index is -1 
                if (indexAdd == -1)
                {
                    // Add column to the end
                    columnsOrdered.Insert(columnsOrdered.Count, columnMoving);
                }
                else
                {
                    // Add column to the new position
                    columnsOrdered.Insert(indexAdd, columnMoving);
                }

                // Loop through columns to change the order
                for (int a = 0; a < columnsOrdered.Count; a++)
                {
                    // Set order
                    columnsOrdered[a].Order = a;                
                }                
            }

            // Check if the control needs to be updated
            if (updateControl)
            {
                // Invalidate control and refresh
                InvalidateSongCache();
                Invalidate();
                Update();
            }

            base.OnMouseUp(e);
        }

        /// <summary>
        /// Occurs when the user clicks on the control.
        /// Selects or unselects items, reorders columns.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            // Check if all the data is valid
            if (m_columns == null || m_songCache == null)
            {
                return;
            }

            // Calculate album cover art width
            int albumArtCoverWidth = m_columns[0].Visible ? m_columns[0].Width : 0;

            // Make sure the control is focused
            if (!Focused)
            {
                // Set control focus
                Focus();
            }

            // Show context menu strip if the button click is right and not the album art column
            if (e.Button == System.Windows.Forms.MouseButtons.Right &&
                e.X > m_columns[0].Width)
            {
                // Is there a context menu strip configured?
                if (m_contextMenuStrip != null)
                {
                    // Show context menu strip
                    m_contextMenuStrip.Show(Control.MousePosition.X, Control.MousePosition.Y);
                }
            }

            // Check if the user is resizing a column
            SongGridViewColumn columnResizing = m_columns.FirstOrDefault(x => x.IsUserResizingColumn == true);

            // Calculate scrollbar offset Y
            int scrollbarOffsetY = (m_startLineNumber * m_songCache.LineHeight) - m_vScrollBar.Value;

            // Check if the user has clicked on the header (for orderBy)
            if (e.Y >= 0 &&
                e.Y <= m_songCache.LineHeight &&
                columnResizing == null &&
                !IsColumnMoving)
            {
                // Check on what column the user has clicked
                int offsetX = 0;
                for (int a = 0; a < m_songCache.ActiveColumns.Count; a++)
                {
                    // Get current column
                    SongGridViewColumn column = m_songCache.ActiveColumns[a];

                    // Make sure column is visible
                    if (column.Visible)
                    {
                        // Check if the mouse pointer is over this column
                        if (e.X >= offsetX - m_hScrollBar.Value && e.X <= offsetX + column.Width - m_hScrollBar.Value)
                        {
                            // Check mouse button
                            if (e.Button == System.Windows.Forms.MouseButtons.Left && CanChangeOrderBy)
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

                                // Raise column click event (if an event is subscribed)
                                if (OnColumnClick != null)
                                {
                                    // Create data
                                    SongGridViewColumnClickData data = new SongGridViewColumnClickData();
                                    data.ColumnIndex = a;

                                    // Raise event
                                    OnColumnClick(data);
                                }

                                // Refresh control
                                Refresh();
                                return;
                            }
                            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
                            {
                                // Refresh column visibility in menu before opening
                                foreach(ToolStripMenuItem menuItem in m_menuColumns.Items)
                                {
                                    // Get column
                                    SongGridViewColumn menuItemColumn = m_columns.FirstOrDefault(x => x.Title == menuItem.Tag.ToString());
                                    if (menuItemColumn != null)
                                    {
                                        menuItem.Checked = menuItemColumn.Visible;
                                    }
                                }

                                // Display columns contextual menu
                                m_menuColumns.Show(this, e.X, e.Y);
                            }
                        }

                        // Increment X offset
                        offsetX += column.Width;
                    }
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
                Invalidate(new Rectangle(albumArtCoverWidth - m_hScrollBar.Value, startY, ClientRectangle.Width - albumArtCoverWidth + m_hScrollBar.Value, endY - startY));
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
            bool invalidatedNewSelection = false;
            for (int a = m_startLineNumber; a < m_startLineNumber + m_numberOfLinesToDraw; a++)
            {
                // Check if mouse is over this item
                if (m_items[a].IsMouseOverItem)
                {
                    // Set flag
                    invalidatedNewSelection = true;

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
                    // Check if CTRL is held
                    else if ((Control.ModifierKeys & Keys.Control) != 0)
                    {
                        // Invert selection
                        m_items[a].IsSelected = !m_items[a].IsSelected;

                        // Invalidate region
                        Invalidate(new Rectangle(albumArtCoverWidth - m_hScrollBar.Value, ((a - m_startLineNumber + 1) * m_songCache.LineHeight) + scrollbarOffsetY, ClientRectangle.Width - albumArtCoverWidth + m_hScrollBar.Value, m_songCache.LineHeight));
                    }
                    else
                    {
                        // Set this item as the new selected item
                        m_items[a].IsSelected = true;

                        // Invalidate region
                        Invalidate(new Rectangle(albumArtCoverWidth - m_hScrollBar.Value, ((a - m_startLineNumber + 1) * m_songCache.LineHeight) + scrollbarOffsetY, ClientRectangle.Width - albumArtCoverWidth + m_hScrollBar.Value, m_songCache.LineHeight));
                    }

                    // Set the last item clicked index
                    m_lastItemIndexClicked = a;

                    // Exit loop
                    break;
                }
            }

            // Raise selected item changed event (if an event is subscribed)
            if (invalidatedNewSelection && OnSelectedIndexChanged != null)
            {
                // Create data
                SongGridViewSelectedIndexChangedData data = new SongGridViewSelectedIndexChangedData();                

                // Raise event
                OnSelectedIndexChanged(data);                
            }

            // Update invalid regions
            Update();

            base.OnMouseClick(e);
        }

        /// <summary>
        /// Occurs when the user double-clicks on the control.
        /// Starts the playback of a new song.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            // Check if all the data is valid
            if (m_columns == null || m_songCache == null)
            {
                return;
            }

            // Calculate album cover art width
            int albumArtCoverWidth = m_columns[0].Visible ? m_columns[0].Width : 0;

            // Calculate scrollbar offset Y
            int scrollbarOffsetY = (m_startLineNumber * m_songCache.LineHeight) - m_vScrollBar.Value;

            // Keep original songId in case the now playing value is set before invalidating the older value
            Guid originalId = Guid.Empty;

            // Check mode
            if (m_mode == SongGridViewMode.AudioFile)
            {
                // Set original id
                originalId = m_nowPlayingAudioFileId;
            }
            else if (m_mode == SongGridViewMode.Playlist)
            {
                // Set original id
                originalId = m_nowPlayingPlaylistItemId;
            }

            // Loop through visible lines
            for (int a = m_startLineNumber; a < m_startLineNumber + m_numberOfLinesToDraw; a++)
            {
                // Check if mouse is over this item
                if (m_items[a].IsMouseOverItem)
                {
                    // Set this item as the new now playing
                    m_nowPlayingAudioFileId = m_items[a].AudioFile.Id;
                    m_nowPlayingPlaylistItemId = m_items[a].PlaylistItemId;

                    // Invalidate region
                    Invalidate(new Rectangle(albumArtCoverWidth - m_hScrollBar.Value, ((a - m_startLineNumber + 1) * m_songCache.LineHeight) + scrollbarOffsetY, ClientRectangle.Width - albumArtCoverWidth + m_hScrollBar.Value, m_songCache.LineHeight));
                }
                else if (m_mode == SongGridViewMode.AudioFile && m_items[a].AudioFile.Id == originalId)
                {
                    // Invalidate region
                    Invalidate(new Rectangle(albumArtCoverWidth - m_hScrollBar.Value, ((a - m_startLineNumber + 1) * m_songCache.LineHeight) + scrollbarOffsetY, ClientRectangle.Width - albumArtCoverWidth + m_hScrollBar.Value, m_songCache.LineHeight));
                }
                else if (m_mode == SongGridViewMode.Playlist && m_items[a].PlaylistItemId == originalId)
                {
                    // Invalidate region
                    Invalidate(new Rectangle(albumArtCoverWidth - m_hScrollBar.Value, ((a - m_startLineNumber + 1) * m_songCache.LineHeight) + scrollbarOffsetY, ClientRectangle.Width - albumArtCoverWidth + m_hScrollBar.Value, m_songCache.LineHeight));
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
        /// <param name="e">Event arguments</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            // Declare variables
            bool controlNeedsToBeUpdated = false;

            // Check if all the data is valid
            if (m_columns == null || m_songCache == null)
            {
                return;
            }

            // Calculate album cover art width
            int albumArtCoverWidth = m_columns[0].Visible ? m_columns[0].Width : 0;

            // Check if the user is currently resizing a column (loop through columns)
            foreach (SongGridViewColumn column in m_songCache.ActiveColumns)
            {
                // Check if the user is currently resizing this column
                if (column.IsUserResizingColumn && column.Visible)
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

                // Check if the user is moving the column
                if (column.IsMouseOverColumnHeader && column.CanBeMoved && CanMoveColumns && m_isUserHoldingLeftMouseButton && !IsColumnResizing)
                {
                    // Check if the X position has changed by at least 2 pixels (i.e. dragging)
                    if (m_dragStartX >= e.X + 2 ||
                        m_dragStartX <= e.X - 2)
                    {
                        // Set resizing column flag
                        column.IsUserMovingColumn = true;
                    }                    
                }

                // Check if the user is currently moving this column 
                if (column.IsUserMovingColumn)
                {                    
                    // Loop through columns
                    int x = 0;
                    foreach (SongGridViewColumn columnOver in m_songCache.ActiveColumns)
                    {
                        // Check if column is visible
                        if (columnOver.Visible)
                        {
                            // Check if the cursor is over the left part of the column
                            if (e.X >= x - m_hScrollBar.Value && e.X <= x + (columnOver.Width / 2) - m_hScrollBar.Value)
                            {
                                // Set marker
                                m_columnMoveMarkerX = x;
                            }
                            // Check if the cursor is over the right part of the column
                            else if (e.X >= x + (columnOver.Width / 2) - m_hScrollBar.Value && e.X <= x + columnOver.Width - m_hScrollBar.Value)
                            {
                                // Set marker
                                m_columnMoveMarkerX = x + columnOver.Width;
                            }

                            // Increment x
                            x += columnOver.Width;
                        }
                    }

                    // Invalidate control
                    Invalidate();

                    // Set flags
                    controlNeedsToBeUpdated = true;                    
                }
            }

            // Check if a column is moving
            if (!IsColumnMoving)
            {

                // Check if the cursor needs to be changed            
                int offsetX = 0;
                bool mousePointerIsOverColumnLimit = false;
                foreach (SongGridViewColumn column in m_songCache.ActiveColumns)
                {
                    // Check if column is visible
                    if (column.Visible)
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
                }

                // Check if the default cursor needs to be restored
                if (!mousePointerIsOverColumnLimit && Cursor != Cursors.Default)
                {
                    // Restore the default cursor
                    Cursor = Cursors.Default;
                }

                // Reset flags
                int columnOffsetX2 = 0;
                for (int b = 0; b < m_songCache.ActiveColumns.Count; b++)
                {
                    // Get current column
                    SongGridViewColumn column = m_songCache.ActiveColumns[b];

                    // Check if column is visible
                    if (column.Visible)
                    {
                        // Was mouse over this column header?
                        if (column.IsMouseOverColumnHeader)
                        {
                            // Reset flag
                            column.IsMouseOverColumnHeader = false;

                            // Invalidate region
                            Invalidate(new Rectangle(columnOffsetX2 - m_hScrollBar.Value, 0, column.Width, m_songCache.LineHeight));
                            controlNeedsToBeUpdated = true;
                        }

                        // Increment offset
                        columnOffsetX2 += column.Width;
                    }
                }

                // Check if the mouse pointer is over the header
                if (e.Y >= 0 &&
                    e.Y <= m_songCache.LineHeight)
                {
                    // Check on what column the user has clicked
                    int columnOffsetX = 0;
                    for (int a = 0; a < m_songCache.ActiveColumns.Count; a++)
                    {
                        // Get current column
                        SongGridViewColumn column = m_songCache.ActiveColumns[a];

                        // Make sure column is visible
                        if (column.Visible)
                        {
                            // Check if the mouse pointer is over this column
                            if (e.X >= columnOffsetX - m_hScrollBar.Value && e.X <= columnOffsetX + column.Width - m_hScrollBar.Value)
                            {
                                // Set flag
                                column.IsMouseOverColumnHeader = true;

                                // Invalidate region
                                Invalidate(new Rectangle(columnOffsetX - m_hScrollBar.Value, 0, column.Width, m_songCache.LineHeight));

                                // Exit loop
                                controlNeedsToBeUpdated = true;
                                break;
                            }

                            // Increment X offset
                            columnOffsetX += column.Width;
                        }
                    }
                }

                // Check if the mouse cursor is over a line (loop through lines)                        
                int offsetY = 0;
                int scrollbarOffsetY = (m_startLineNumber * m_songCache.LineHeight) - m_vScrollBar.Value;

                // Check if there's at least one item
                if (m_items.Count > 0)
                {
                    // Reset mouse over item flags
                    for (int b = m_startLineNumber; b < m_startLineNumber + m_numberOfLinesToDraw; b++)
                    {
                        // Check if the mouse was over this item
                        if (m_items[b].IsMouseOverItem)
                        {
                            // Reset flag and invalidate region
                            m_items[b].IsMouseOverItem = false;
                            Invalidate(new Rectangle(albumArtCoverWidth - m_hScrollBar.Value, ((b - m_startLineNumber + 1) * m_songCache.LineHeight) + scrollbarOffsetY, ClientRectangle.Width - albumArtCoverWidth + m_hScrollBar.Value, m_songCache.LineHeight));

                            // Exit loop
                            break;
                        }
                    }

                    // Put new mouse over flag
                    for (int a = m_startLineNumber; a < m_startLineNumber + m_numberOfLinesToDraw; a++)
                    {
                        // Get audio file
                        AudioFile audioFile = m_items[a].AudioFile;

                        // Calculate offset
                        offsetY = (a * m_songCache.LineHeight) - m_vScrollBar.Value + m_songCache.LineHeight;

                        // Check if the mouse cursor is over this line (and not already mouse over)
                        if (e.X >= albumArtCoverWidth - m_hScrollBar.Value &&
                            e.Y >= offsetY &&
                            e.Y <= offsetY + m_songCache.LineHeight &&
                            !m_items[a].IsMouseOverItem)
                        {
                            // Set item as mouse over
                            m_items[a].IsMouseOverItem = true;

                            // Invalidate region                    
                            Invalidate(new Rectangle(albumArtCoverWidth - m_hScrollBar.Value, offsetY, ClientRectangle.Width - albumArtCoverWidth + m_hScrollBar.Value, m_songCache.LineHeight));

                            // Update control                    
                            controlNeedsToBeUpdated = true;

                            // Exit loop
                            break;
                        }
                    }
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
        /// Occurs when the user clicks on one of the menu items of the Columns contextual menu.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        protected void menuItemColumns_Click(object sender, EventArgs e)
        {
            // Make sure the sender is the menu item
            if (sender is ToolStripMenuItem)
            {
                // Get the reference to the menu item
                ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;

                // Inverse selection
                menuItem.Checked = !menuItem.Checked;

                // Get column
                SongGridViewColumn column = m_columns.FirstOrDefault(x => x.Title == menuItem.Tag.ToString());
                if (column != null)
                {
                    // Set visibility
                    column.Visible = menuItem.Checked;
                }

                // Reset cache
                m_songCache = null;
                Refresh();
            }
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

                // Check for maximum
                if (newValue > m_vScrollBar.Maximum - m_vScrollBar.LargeChange)
                {
                    newValue = m_vScrollBar.Maximum - m_vScrollBar.LargeChange;
                }
                // Check for minimum
                if (newValue < 0)
                {
                    newValue = 0;
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
        public void InvalidateSongCache()
        {
            // Check if columns have been created
            if (m_columns == null || m_columns.Count == 0 || m_items == null)
            {
                return;
            }

            // Create cache
            m_songCache = new SongGridViewCache();

            // Get active columns and order them
            m_songCache.ActiveColumns = m_columns.Where(x => x.Order >= 0).OrderBy(x => x.Order).ToList();

            // Load custom font
            //Font fontDefaultBold = Tools.LoadCustomFont(FontCollection, CustomFontName, Font.Size, FontStyle.Bold);
            Font fontDefaultBold = null;

            // Make sure the embedded font name needs to be loaded and is valid
            if (m_theme.Font.UseEmbeddedFont && !String.IsNullOrEmpty(m_theme.Font.EmbeddedFontName))
            {
                try
                {
                    // Get embedded font collection
                    EmbeddedFontCollection embeddedFonts = EmbeddedFontHelper.GetEmbeddedFonts();

                    // Get embedded fonts                    
                    fontDefaultBold = Tools.LoadEmbeddedFont(embeddedFonts, m_theme.Font.EmbeddedFontName, m_theme.Font.Size, m_theme.Font.ToFontStyle() | FontStyle.Bold);
                }
                catch
                {
                    // Use default font instead                    
                    fontDefaultBold = new Font(this.Font, FontStyle.Bold);
                }
            }

            // Check if font is null
            if (fontDefaultBold == null)
            {
                try
                {
                    // Try to get standard font                    
                    fontDefaultBold = new Font(m_theme.Font.StandardFontName, m_theme.Font.Size, m_theme.Font.ToFontStyle() | FontStyle.Bold);
                }
                catch
                {
                    // Use default font instead                    
                    fontDefaultBold = new Font(this.Font, FontStyle.Bold);
                }
            }

            //// Load default fonts if custom fonts were not found
            //if (fontDefaultBold == null)
            //{
            //    // Load default font
            //    fontDefaultBold = new Font(Font.FontFamily.Name, Font.Size, FontStyle.Bold);
            //}

            // Create temporary bitmap/graphics objects to measure a string (to determine line height)
            Bitmap bmpTemp = new Bitmap(200, 100);
            Graphics g = Graphics.FromImage(bmpTemp);
            SizeF sizeFont = g.MeasureString("QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm!@#$%^&*()", fontDefaultBold);
            g.Dispose();
            g = null;
            bmpTemp.Dispose();
            bmpTemp = null;

            // Calculate the line height (try to measure the total possible height of characters using the custom or default font)
            m_songCache.LineHeight = (int)sizeFont.Height + m_theme.Padding;
            m_songCache.TotalHeight = m_songCache.LineHeight * m_items.Count;

            // Check if the total active columns width exceed the width available in the control
            m_songCache.TotalWidth = 0;
            for (int a = 0; a < m_songCache.ActiveColumns.Count; a++)
            {
                // Check if column is visible
                if (m_songCache.ActiveColumns[a].Visible)
                {
                    // Increment total width
                    m_songCache.TotalWidth += m_songCache.ActiveColumns[a].Width;
                }
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
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
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

    /// <summary>
    /// Result data structure used for the SongGridView background worker.
    /// </summary>
    public class SongGridViewBackgroundWorkerResult
    {
        /// <summary>
        /// Audio file.
        /// </summary>
        public AudioFile AudioFile { get; set; }
        /// <summary>
        /// Album cover.
        /// </summary>
        public Image AlbumArt { get; set; }
    }

    /// <summary>
    /// Argument data structure used for the SongGridView background worker.
    /// </summary>
    public class SongGridViewBackgroundWorkerArgument
    {
        /// <summary>
        /// Audio file.
        /// </summary>
        public AudioFile AudioFile { get; set; }
        /// <summary>
        /// Line number (item index in the SongGridView control).
        /// </summary>
        public int LineIndex { get; set; }        
    }

    /// <summary>
    /// Data structure used for the SelectedIndexChanged event.
    /// </summary>
    public class SongGridViewSelectedIndexChangedData
    {        
    }

    /// <summary>
    /// Data structure used for the ColumnClick event.
    /// </summary>
    public class SongGridViewColumnClickData
    {
        /// <summary>
        /// Column index.
        /// </summary>
        public int ColumnIndex { get; set; }
    }

}
