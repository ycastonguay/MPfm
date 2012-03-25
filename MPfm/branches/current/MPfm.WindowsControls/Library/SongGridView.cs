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
    public partial class SongGridView : Control, IMessageFilter
    {
        #region Private Variables

        // Mode
        private SongGridViewMode mode = SongGridViewMode.AudioFile;

        // Controls
        private System.Windows.Forms.VScrollBar vScrollBar = null;
        private System.Windows.Forms.HScrollBar hScrollBar = null;
        private ContextMenuStrip menuColumns = null;

        // Background worker for updating album art
        private int preloadLinesAlbumCover = 20;
        private BackgroundWorker workerUpdateAlbumArt = null;
        private List<SongGridViewBackgroundWorkerArgument> workerUpdateAlbumArtPile = null;
        private System.Windows.Forms.Timer timerUpdateAlbumArt = null;        

        // Cache        
        private SongGridViewCache songCache = null;
        private List<SongGridViewImageCache> imageCache = new List<SongGridViewImageCache>();        

        // Private variables used for mouse events
        private int columnMoveMarkerX = 0;
        private int startLineNumber = 0;
        private int numberOfLinesToDraw = 0;
        private int minimumColumnWidth = 30;
        private int dragStartX = -1;
        private int dragOriginalColumnWidth = -1;
        private bool isMouseOverControl = false;
        private bool isUserHoldingLeftMouseButton = false;        
        private int lastItemIndexClicked = -1;

        // Animation timer and counter for currently playing song
        private int timerAnimationNowPlayingCount = 0;
        private Rectangle rectNowPlaying = new Rectangle(0, 0, 1, 1);
        private System.Windows.Forms.Timer timerAnimationNowPlaying = null;

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
        private SongGridViewTheme theme = null;
        /// <summary>
        /// Defines the current theme used for rendering the control.
        /// </summary>
        public SongGridViewTheme Theme
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

        #region Other Properties (Items, Columns, Menus, etc.)

        /// <summary>
        /// Private value for the Items property.
        /// </summary>
        private List<SongGridViewItem> items = null;
        /// <summary>
        /// List of grid view items (representing songs).
        /// </summary>
        [Browsable(false)]
        public List<SongGridViewItem> Items
        {
            get
            {
                return items;
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
                if (items != null)
                {
                    return items.Where(x => x.IsSelected).ToList();
                }

                return null;
            }
        }

        /// <summary>
        /// Private value for the Columns property.
        /// </summary>
        private List<SongGridViewColumn> columns = null;
        /// <summary>
        /// List of grid view columns.
        /// </summary>
        [Browsable(false)]
        public List<SongGridViewColumn> Columns
        {
            get
            {
                return columns;
            }
        }


        /// <summary>
        /// Private value for the NowPlayingAudioFileId property.
        /// </summary>
        private Guid nowPlayingAudioFileId = Guid.Empty;
        /// <summary>
        /// Defines the currently playing audio file identifier.
        /// </summary>
        [Browsable(false)]
        public Guid NowPlayingAudioFileId
        {
            get
            {
                return nowPlayingAudioFileId;
            }
            set
            {
                nowPlayingAudioFileId = value;
            }
        }

        /// <summary>
        /// Private value for the NowPlayingPlaylistItemId property.
        /// </summary>
        private Guid nowPlayingPlaylistItemId = Guid.Empty;
        /// <summary>
        /// Defines the currently playing playlist item identifier.
        /// </summary>
        [Browsable(false)]
        public Guid NowPlayingPlaylistItemId
        {
            get
            {
                return nowPlayingPlaylistItemId;
            }
            set
            {
                nowPlayingPlaylistItemId = value;
            }
        }

        /// <summary>
        /// Private value for the ContextMenuStrip property.
        /// </summary>
        private ContextMenuStrip contextMenuStrip = null;
        /// <summary>
        /// ContextMenuStrip related to the grid. This context menu
        /// opens when the user right clicks an item.
        /// </summary>
        [Category("Misc"), Browsable(true), Description("Stuff.")]
        public override ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return contextMenuStrip;
            }
            set
            {
                contextMenuStrip = value;
            }
        }

        #endregion

        #region Filter / OrderBy Properties

        /// <summary>
        /// Private value for the OrderByFieldName property.
        /// </summary>
        private string orderByFieldName = string.Empty;
        /// <summary>
        /// Indicates which field should be used for ordering songs.
        /// </summary>
        public string OrderByFieldName
        {
            get
            {
                return orderByFieldName;
            }
            set
            {
                orderByFieldName = value;

                // Invalidate item list and cache
                items = null;
                songCache = null;

                // Refresh whole control
                Refresh();
            }
        }

        /// <summary>
        /// Private value for the OrderByAscending property.
        /// </summary>
        private bool orderByAscending = true;
        /// <summary>
        /// Indicates if the order should be ascending (true) or descending (false).
        /// </summary>
        public bool OrderByAscending
        {
            get
            {
                return orderByAscending;
            }
            set
            {
                orderByAscending = value;
            }
        }

        #endregion

        #region Settings Properties

        /// <summary>
        /// Private value for the DisplayDebugInformation property.
        /// </summary>
        private bool displayDebugInformation = false;
        /// <summary>
        /// If true, the debug information will be shown over the first column.
        /// </summary>
        public bool DisplayDebugInformation
        {
            get
            {
                return displayDebugInformation;
            }
            set
            {
                displayDebugInformation = value;
            }
        }

        /// <summary>
        /// Private value for the CanResizeColumns property.
        /// </summary>
        private bool canResizeColumns = true;
        /// <summary>
        /// Indicates if the user can resize the columns or not.
        /// </summary>
        public bool CanResizeColumns
        {
            get
            {
                return canResizeColumns;
            }
            set
            {
                canResizeColumns = value;
            }
        }

        /// <summary>
        /// Private value for the CanMoveColumns property.
        /// </summary>
        private bool canMoveColumns = true;
        /// <summary>
        /// Indicates if the user can move the columns or not.
        /// </summary>
        public bool CanMoveColumns
        {
            get
            {
                return canMoveColumns;
            }
            set
            {
                canMoveColumns = value;
            }
        }

        /// <summary>
        /// Private value for the CanChangeOrderBy property.
        /// </summary>
        private bool canChangeOrderBy = true;
        /// <summary>
        /// Indicates if the user can change the order by or not.
        /// </summary>
        public bool CanChangeOrderBy
        {
            get
            {
                return canChangeOrderBy;
            }
            set
            {
                canChangeOrderBy = value;
            }
        }

        /// <summary>
        /// Private value for the CanReorderItems property.
        /// </summary>
        private bool canReorderItems = true;
        /// <summary>
        /// Indicates if the user can reorder the items or not.
        /// </summary>
        public bool CanReorderItems
        {
            get
            {
                return canReorderItems;
            }
            set
            {
                canReorderItems = value;
            }
        }

        /// <summary>
        /// Private value for the ImageCacheSize property.
        /// </summary>             
        private int imageCacheSize = 10;
        /// <summary>
        /// Defines the size of the album art image cache (10 by default).
        /// </summary>
        public int ImageCacheSize
        {
            get
            {
                return imageCacheSize;
            }
            set
            {
                imageCacheSize = value;
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
                foreach (SongGridViewColumn column in columns)
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
                foreach (SongGridViewColumn column in columns)
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
            theme = new SongGridViewTheme();

            // Get embedded font collection
            embeddedFonts = EmbeddedFontHelper.GetEmbeddedFonts();

            // Create timer for animation
            timerAnimationNowPlaying = new System.Windows.Forms.Timer();
            timerAnimationNowPlaying.Interval = 50;
            timerAnimationNowPlaying.Tick += new EventHandler(timerAnimationNowPlaying_Tick);
            timerAnimationNowPlaying.Enabled = true;

            // Create vertical scrollbar
            vScrollBar = new System.Windows.Forms.VScrollBar();
            vScrollBar.Width = 16;
            vScrollBar.Scroll += new ScrollEventHandler(vScrollBar_Scroll);
            Controls.Add(vScrollBar);

            // Create horizontal scrollbar
            hScrollBar = new System.Windows.Forms.HScrollBar();
            hScrollBar.Width = ClientRectangle.Width;
            hScrollBar.Height = 16;
            hScrollBar.Top = ClientRectangle.Height - hScrollBar.Height;
            hScrollBar.Scroll += new ScrollEventHandler(hScrollBar_Scroll);
            Controls.Add(hScrollBar);

            // Override mouse messages for mouse wheel (get mouse wheel events out of control)
            Application.AddMessageFilter(this);

            // Create background worker for updating album art
            workerUpdateAlbumArtPile = new List<SongGridViewBackgroundWorkerArgument>();
            workerUpdateAlbumArt = new BackgroundWorker();            
            workerUpdateAlbumArt.DoWork += new DoWorkEventHandler(workerUpdateAlbumArt_DoWork);
            workerUpdateAlbumArt.RunWorkerCompleted += new RunWorkerCompletedEventHandler(workerUpdateAlbumArt_RunWorkerCompleted);
            
            // Create timer for updating album art
            timerUpdateAlbumArt = new System.Windows.Forms.Timer();
            timerUpdateAlbumArt.Interval = 10;
            timerUpdateAlbumArt.Tick += new EventHandler(timerUpdateAlbumArt_Tick);
            timerUpdateAlbumArt.Enabled = true;

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
            columns = new List<SongGridViewColumn>();
            columns.Add(columnSongAlbumCover);
            columns.Add(columnSongAlbumTitle);
            columns.Add(columnSongArtistName);
            columns.Add(columnSongBitrate);            
            columns.Add(columnSongFilePath);            
            columns.Add(columnSongGenre);
            columns.Add(columnSongLastPlayed);
            columns.Add(columnSongLength);
            columns.Add(columnSongNowPlaying);
            columns.Add(columnSongPlayCount);
            columns.Add(columnSongSampleRate);
            columns.Add(columnSongTitle); // Song title
            columns.Add(columnSongTempo);            
            columns.Add(columnSongTrackNumber);
            columns.Add(columnSongTrackCount);
            columns.Add(columnSongFileType); // Type
            columns.Add(columnSongYear);

            // Create contextual menu
            menuColumns = new System.Windows.Forms.ContextMenuStrip();

            // Loop through columns
            foreach (SongGridViewColumn column in columns)
            {
                // Add menu item                               
                ToolStripMenuItem menuItem = (ToolStripMenuItem)menuColumns.Items.Add(column.Title);
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
        public void timerUpdateAlbumArt_Tick(object sender, EventArgs e)
        {
            // Stop timer
            timerUpdateAlbumArt.Enabled = false;

            // Check for the next album art to fetch
            if (workerUpdateAlbumArtPile.Count > 0 && !workerUpdateAlbumArt.IsBusy)
            {
                // Do some cleanup: clean items that are not visible anymore
                bool cleanUpDone = false;
                while (!cleanUpDone)
                {
                    int indexToDelete = -1;
                    for (int a = 0; a < workerUpdateAlbumArtPile.Count; a++)
                    {
                        // Get argument
                        SongGridViewBackgroundWorkerArgument arg = workerUpdateAlbumArtPile[a];

                        // Check if this album is still visible (cancel if it is out of display).                             
                        if (arg.LineIndex < startLineNumber || arg.LineIndex > startLineNumber + numberOfLinesToDraw + preloadLinesAlbumCover)
                        {
                            indexToDelete = a;
                            break;
                        }
                    }

                    if (indexToDelete >= 0)
                    {
                        workerUpdateAlbumArtPile.RemoveAt(indexToDelete);                        
                    }
                    else
                    {
                        cleanUpDone = true;
                    }
                }
                // There must be more album art to fetch.. right?
                if (workerUpdateAlbumArtPile.Count > 0)
                {
                    // Start background worker                
                    SongGridViewBackgroundWorkerArgument arg = workerUpdateAlbumArtPile[0];
                    workerUpdateAlbumArt.RunWorkerAsync(arg);
                }
            }

            // Restart timer
            timerUpdateAlbumArt.Enabled = true;
        }

        /// <summary>
        /// Occurs when the Update Album Art background worker starts its work.
        /// Fetches the album cover in another thread and returns the image once done.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        public void workerUpdateAlbumArt_DoWork(object sender, DoWorkEventArgs e)
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
            if (arg.LineIndex < startLineNumber || arg.LineIndex > startLineNumber + numberOfLinesToDraw + preloadLinesAlbumCover)
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
        public void workerUpdateAlbumArt_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
            imageCache.Add(cache);

            // Check if the cache size has been reached
            if (imageCache.Count > imageCacheSize)
            {
                // Check if the image needs to be disposed
                if (imageCache[0].Image != null)
                {
                    // Dispose image
                    Image imageTemp = imageCache[0].Image;
                    imageTemp.Dispose();
                    imageTemp = null;
                }

                // Remove the oldest item
                imageCache.RemoveAt(0);
            }            

            // Remove song from list
            int indexRemove = -1;
            for (int a = 0; a < workerUpdateAlbumArtPile.Count; a++)
            {
                if (workerUpdateAlbumArtPile[a].AudioFile.FilePath.ToUpper() == result.AudioFile.FilePath.ToUpper())
                {
                    indexRemove = a;
                }
            }
            if (indexRemove >= 0)
            {
                workerUpdateAlbumArtPile.RemoveAt(indexRemove);
            }            

            // Refresh control (TODO: Invalidate instead)
            Refresh();
        }

        /// <summary>
        /// Clears the currently selected items.
        /// </summary>
        public void ClearSelectedItems()
        {
            // Loop through items
            foreach (SongGridViewItem item in items)
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
            mode = SongGridViewMode.AudioFile;

            // Create list of items
            items = new List<SongGridViewItem>();
            foreach (AudioFile audioFile in audioFiles)
            {
                // Create item and add to list
                SongGridViewItem item = new SongGridViewItem();
                item.AudioFile = audioFile;
                item.PlaylistItemId = Guid.NewGuid();
                items.Add(item);
            }

            // Reset scrollbar position
            vScrollBar.Value = 0;
            songCache = null;

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
            mode = SongGridViewMode.Playlist;

            // Create list of items
            items = new List<SongGridViewItem>();
            foreach (PlaylistItem playlistItem in playlist.Items)
            {
                // Create item and add to list
                SongGridViewItem item = new SongGridViewItem();
                item.AudioFile = playlistItem.AudioFile;
                item.PlaylistItemId = playlistItem.Id;
                items.Add(item);
            }

            // Reset scrollbar position
            vScrollBar.Value = 0;
            songCache = null;

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
            for (int a = startLineNumber; a < startLineNumber + numberOfLinesToDraw; a++)
            {
                // Calculate offset
                int offsetY = (a * songCache.LineHeight) - vScrollBar.Value + songCache.LineHeight;

                // Check if the line matches
                if (items[a].AudioFile.Id == audioFileId)
                {
                    // Invalidate this line
                    Invalidate(new Rectangle(columns[0].Width - hScrollBar.Value, offsetY, ClientRectangle.Width - columns[0].Width + hScrollBar.Value, songCache.LineHeight));

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
            if (items == null)
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
            Color colorNowPlaying1 = theme.LineNowPlayingColor1;
            Color colorNowPlaying2 = theme.LineNowPlayingColor2;
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
            if (theme.Font.UseEmbeddedFont && !String.IsNullOrEmpty(theme.Font.EmbeddedFontName))
            {
                try
                {
                    // Get embedded fonts
                    fontDefault = Tools.LoadEmbeddedFont(embeddedFonts, theme.Font.EmbeddedFontName, theme.Font.Size, theme.Font.ToFontStyle());
                    fontDefaultBold = Tools.LoadEmbeddedFont(embeddedFonts, theme.Font.EmbeddedFontName, theme.Font.Size, theme.Font.ToFontStyle() | FontStyle.Bold);
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
                    fontDefault = new Font(theme.Font.StandardFontName, theme.Font.Size, theme.Font.ToFontStyle());
                    fontDefaultBold = new Font(theme.Font.StandardFontName, theme.Font.Size, theme.Font.ToFontStyle() | FontStyle.Bold);
                }
                catch
                {
                    // Use default font instead
                    fontDefault = this.Font;
                    fontDefaultBold = new Font(this.Font, FontStyle.Bold);
                }
            }

            // Set string format
            StringFormat stringFormat = new StringFormat();

            // If there are no items..,
            if (items == null)
            {
                // Do not do anything.
                return;
            }

            // Check if a cache exists, or if the cache needs to be refreshed
            if (songCache == null)
            {
                // Create song cache
                InvalidateSongCache();
            }

            // Calculate how many lines must be skipped because of the scrollbar position
            startLineNumber = (int)Math.Floor((double)vScrollBar.Value / (double)(songCache.LineHeight));

            // Check if the total number of lines exceeds the number of icons fitting in height
            numberOfLinesToDraw = 0;
            if (startLineNumber + songCache.NumberOfLinesFittingInControl > items.Count)
            {
                // There aren't enough lines to fill the screen
                numberOfLinesToDraw = items.Count - startLineNumber;
            }
            else
            {
                // Fill up screen 
                numberOfLinesToDraw = songCache.NumberOfLinesFittingInControl;
            }

            // Add one line for overflow; however, make sure we aren't adding a line without content 
            if (startLineNumber + numberOfLinesToDraw + 1 <= items.Count)
            {
                // Add one line for overflow
                numberOfLinesToDraw++;
            }

            // Loop through lines
            for (int a = startLineNumber; a < startLineNumber + numberOfLinesToDraw; a++)
            {
                // Get audio file
                AudioFile audioFile = items[a].AudioFile;

                // Calculate Y offset (compensate for scrollbar position)
                offsetY = (a * songCache.LineHeight) - vScrollBar.Value + songCache.LineHeight;

                // Calculate album art cover column width
                int albumArtColumnWidth = columns[0].Visible ? columns[0].Width : 0;

                // Calculate line background width
                int lineBackgroundWidth = ClientRectangle.Width + hScrollBar.Value - albumArtColumnWidth;

                // Reduce width from scrollbar if visible
                if (vScrollBar.Visible)
                {
                    lineBackgroundWidth -= vScrollBar.Width;
                }

                // Set rectangle                
                Rectangle rectBackground = new Rectangle(albumArtColumnWidth - hScrollBar.Value, offsetY, lineBackgroundWidth, songCache.LineHeight);                
                
                // Set default line background color
                Color colorBackground1 = theme.LineColor1;
                Color colorBackground2 = theme.LineColor2;

                // Check conditions to determine background color
                if ((mode == SongGridViewMode.AudioFile && audioFile.Id == nowPlayingAudioFileId) || 
                    (mode == SongGridViewMode.Playlist && items[a].PlaylistItemId == nowPlayingPlaylistItemId))
                {
                    // Set color             
                    colorBackground1 = theme.LineNowPlayingColor1;
                    colorBackground2 = theme.LineNowPlayingColor2;
                }

                // Check if item is selected
                if (items[a].IsSelected)
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
                if (items[a].IsMouseOverItem)
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
                if ((mode == SongGridViewMode.AudioFile && audioFile.Id == nowPlayingAudioFileId) ||
                    (mode == SongGridViewMode.Playlist && items[a].PlaylistItemId == nowPlayingPlaylistItemId))
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
                for (int b = 0; b < songCache.ActiveColumns.Count; b++)
                {
                    // Get current column
                    SongGridViewColumn column = songCache.ActiveColumns[b];

                    // Check if the column is visible
                    if (column.Visible)
                    {
                        // Check if this is the "Now playing" column
                        if (column.Title == "Now Playing")
                        {
                            // Draw now playing icon
                            if ((mode == SongGridViewMode.AudioFile && audioFile.Id == nowPlayingAudioFileId) ||
                                (mode == SongGridViewMode.Playlist && items[a].PlaylistItemId == nowPlayingPlaylistItemId))
                            {
                                // Which size is the minimum? Width or height?                    
                                int availableWidthHeight = column.Width - 4;
                                if (songCache.LineHeight <= column.Width)
                                {                                    
                                    availableWidthHeight = songCache.LineHeight - 4;
                                }
                                else
                                {
                                    availableWidthHeight = column.Width - 4;
                                }

                                // Calculate the icon position                                
                                float iconNowPlayingX = ((column.Width - availableWidthHeight) / 2) + offsetX - hScrollBar.Value;
                                float iconNowPlayingY = offsetY + ((songCache.LineHeight - availableWidthHeight) / 2);

                                // Create NowPlaying rect (MUST be in integer)                    
                                rectNowPlaying = new Rectangle((int)iconNowPlayingX, (int)iconNowPlayingY, availableWidthHeight, availableWidthHeight);
                                nowPlayingSongFound = true;

                                // Draw outer circle
                                brushGradient = new LinearGradientBrush(rectNowPlaying, Color.FromArgb(50, theme.IconNowPlayingColor1.R, theme.IconNowPlayingColor1.G, theme.IconNowPlayingColor1.B), theme.IconNowPlayingColor2, timerAnimationNowPlayingCount % 360);
                                g.FillEllipse(brushGradient, rectNowPlaying);
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
                                    AudioFile previousAudioFile = items[c].AudioFile;

                                    // Check if the album title matches
                                    if (previousAudioFile.AlbumTitle != audioFile.AlbumTitle)
                                    {
                                        // Set album cover start index (+1 because the last song was the sound found in the previous loop iteration)
                                        albumCoverStartIndex = c + 1;
                                        break;
                                    }
                                }
                                // Find the end index
                                for (int c = a + 1; c < items.Count; c++)
                                {
                                    // Get audio file
                                    AudioFile nextAudioFile = items[c].AudioFile;

                                    // If the album title is different, this means we found the next album title
                                    if (nextAudioFile.AlbumTitle != audioFile.AlbumTitle)
                                    {
                                        // Set album cover end index (-1 because the last song was the song found in the previous loop iteration)
                                        albumCoverEndIndex = c - 1;
                                        break;
                                    }
                                    // If this is the last item of the grid...
                                    else if (c == items.Count - 1)
                                    {
                                        // Set album cover end index as the last item of the grid
                                        albumCoverEndIndex = c;
                                        break;
                                    }
                                }

                                // Calculate y and height
                                int scrollbarOffsetY = (startLineNumber * songCache.LineHeight) - vScrollBar.Value;
                                int y = ((albumCoverStartIndex - startLineNumber) * songCache.LineHeight) + songCache.LineHeight + scrollbarOffsetY;

                                // Calculate the height of the album cover zone (+1 on end index because the array is zero-based)
                                int albumCoverZoneHeight = (albumCoverEndIndex + 1 - albumCoverStartIndex) * songCache.LineHeight;

                                int heightWithPadding = albumCoverZoneHeight - (theme.Padding * 2);
                                if (heightWithPadding > songCache.ActiveColumns[0].Width - (theme.Padding * 2))
                                {
                                    heightWithPadding = songCache.ActiveColumns[0].Width - (theme.Padding * 2);
                                }

                                // Make sure the height is at least zero (not necessary to draw anything!)
                                if (albumCoverZoneHeight > 0)
                                {
                                    // Draw album cover background
                                    Rectangle rectAlbumCover = new Rectangle(0 - hScrollBar.Value, y, songCache.ActiveColumns[0].Width, albumCoverZoneHeight);
                                    brushGradient = new LinearGradientBrush(rectAlbumCover, theme.AlbumCoverBackgroundColor1, theme.AlbumCoverBackgroundColor2, 90);
                                    g.FillRectangle(brushGradient, rectAlbumCover);
                                    brushGradient.Dispose();
                                    brushGradient = null;

                                    // Try to extract image from cache
                                    Image imageAlbumCover = null;
                                    SongGridViewImageCache cachedImage = imageCache.FirstOrDefault(x => x.Key == audioFile.ArtistName + "_" + audioFile.AlbumTitle);
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
                                        foreach (SongGridViewBackgroundWorkerArgument arg in workerUpdateAlbumArtPile)
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
                                            workerUpdateAlbumArtPile.Add(arg);
                                        }
                                    }

                                    // Measure available width for text
                                    int widthAvailableForText = columns[0].Width - (theme.Padding * 2);

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
                                        rectArtistNameText = new RectangleF(theme.Padding - hScrollBar.Value, y + (theme.Padding / 2), widthAvailableForText, songCache.LineHeight);
                                        rectAlbumTitleText = new RectangleF(theme.Padding - hScrollBar.Value + sizeArtistName.Width + theme.Padding, y + (theme.Padding / 2), widthAvailableForText - sizeArtistName.Width, songCache.LineHeight);
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
                                            rectArtistNameText = new RectangleF(theme.Padding - hScrollBar.Value, y + theme.Padding, widthAvailableForText, heightWithPadding);
                                            rectAlbumTitleText = new RectangleF(theme.Padding - hScrollBar.Value, y + theme.Padding + sizeArtistName.Height, widthAvailableForText, heightWithPadding);
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
                                                rectArtistNameText = new RectangleF(((columns[0].Width - sizeArtistName.Width) / 2) - hScrollBar.Value, y, widthAvailableForText, heightWithPadding);
                                                rectAlbumTitleText = new RectangleF(((columns[0].Width - sizeAlbumTitle.Width) / 2) - hScrollBar.Value, y + songCache.LineHeight, widthAvailableForText, heightWithPadding);
                                            }
                                            // There is an album cover to display; between 2 and 6 lines AND the width of the column is at least 50 pixels (or
                                            // it will try to display text in a too thin area)
                                            else if (albumCoverEndIndex - albumCoverStartIndex <= 5 && columns[0].Width > 175)
                                            {
                                                // There is no album cover to display; display only text.
                                                // Set string format
                                                stringFormat.Alignment = StringAlignment.Near;
                                                stringFormat.Trimming = StringTrimming.EllipsisWord;

                                                float widthRemainingForText = columns[0].Width - theme.Padding - heightWithPadding;

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

                                                float albumCoverX = (columns[0].Width - heightWithPadding - theme.Padding - theme.Padding - maxWidth) / 2;
                                                float artistNameY = (albumCoverZoneHeight - sizeArtistName.Height - sizeAlbumTitle.Height) / 2;

                                                // Display the album title at the top of the zome
                                                rectArtistNameText = new RectangleF(albumCoverX + heightWithPadding + theme.Padding - hScrollBar.Value, y + artistNameY, widthRemainingForText, heightWithPadding);
                                                rectAlbumTitleText = new RectangleF(albumCoverX + heightWithPadding + theme.Padding - hScrollBar.Value, y + artistNameY + sizeArtistName.Height, widthRemainingForText, heightWithPadding);

                                                // Set cover art rectangle
                                                rectAlbumCoverArt = new RectangleF(albumCoverX - hScrollBar.Value, y + theme.Padding, heightWithPadding, heightWithPadding);
                                            }
                                            // 7 and more lines
                                            else
                                            {
                                                // Display artist name at the top of the album cover; display album title at the bottom of the album cover
                                                rectArtistNameText = new RectangleF(((columns[0].Width - sizeArtistName.Width) / 2) - hScrollBar.Value, y + (theme.Padding * 2), widthAvailableForText, heightWithPadding);
                                                rectAlbumTitleText = new RectangleF(((columns[0].Width - sizeAlbumTitle.Width) / 2) - hScrollBar.Value, y + heightWithPadding - sizeAlbumTitle.Height, widthAvailableForText, heightWithPadding);

                                                // Draw background overlay behind text
                                                useAlbumArtOverlay = true;

                                                // Try to horizontally center the album cover if it's not taking the whole width (less padding)
                                                float albumCoverX = theme.Padding;
                                                if (columns[0].Width > heightWithPadding)
                                                {
                                                    // Get position
                                                    albumCoverX = ((float)(columns[0].Width - heightWithPadding) / 2) - hScrollBar.Value;
                                                }

                                                // Set cover art rectangle
                                                rectAlbumCoverArt = new RectangleF(albumCoverX, y + theme.Padding, heightWithPadding, heightWithPadding);
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
                                        RectangleF rectArtistNameBackground = new RectangleF(rectArtistNameText.X - (theme.Padding / 2), rectArtistNameText.Y - (theme.Padding / 2), sizeArtistName.Width + theme.Padding, sizeArtistName.Height + theme.Padding);
                                        RectangleF rectAlbumTitleBackground = new RectangleF(rectAlbumTitleText.X - (theme.Padding / 2), rectAlbumTitleText.Y - (theme.Padding / 2), sizeAlbumTitle.Width + theme.Padding, sizeAlbumTitle.Height + theme.Padding);
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
                                    pen = new Pen(theme.AlbumCoverBackgroundColor1);
                                    g.DrawLine(pen, new Point(columns[0].Width, y), new Point(ClientRectangle.Width, y));
                                    pen.Dispose();
                                    pen = null;

                                    // Part 2: Draw line over album art zone, in a lighter color
                                    pen = new Pen(Color.FromArgb(115, 115, 115));
                                    g.DrawLine(pen, new Point(0, y), new Point(columns[0].Width, y));
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
                                if (b == songCache.ActiveColumns.Count - 1)
                                {
                                    // Calculate the remaining width
                                    int columnsWidth = 0;
                                    for (int c = 0; c < songCache.ActiveColumns.Count - 1; c++)
                                    {
                                        columnsWidth += songCache.ActiveColumns[c].Width;
                                    }
                                    columnWidth = ClientRectangle.Width - columnsWidth + hScrollBar.Value;
                                }

                                // Display text
                                rect = new Rectangle(offsetX - hScrollBar.Value, offsetY + (theme.Padding / 2), songCache.ActiveColumns[b].Width, songCache.LineHeight - theme.Padding + 2);
                                stringFormat.Trimming = StringTrimming.EllipsisCharacter;
                                stringFormat.Alignment = StringAlignment.Near;

                                // Check if this is the artist name column
                                brush = new SolidBrush(theme.LineForeColor);
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
                rectNowPlaying = new Rectangle(0, 0, 1, 1);
            }

            // Draw header (for some reason, the Y must be set -1 to cover an area which isn't supposed to be displayed)
            Rectangle rectBackgroundHeader = new Rectangle(0, -1, ClientRectangle.Width, songCache.LineHeight + 1);
            brushGradient = new LinearGradientBrush(rectBackgroundHeader, theme.HeaderColor1, theme.HeaderColor2, 90);
            g.FillRectangle(brushGradient, rectBackgroundHeader);
            brushGradient.Dispose();
            brushGradient = null;

            // Loop through columns
            offsetX = 0;
            for (int b = 0; b < songCache.ActiveColumns.Count; b++)
            {
                // Get current column
                SongGridViewColumn column = songCache.ActiveColumns[b];

                // Check if the column is visible
                if (column.Visible)
                {
                    // The last column always take the remaining width
                    int columnWidth = column.Width;
                    if (b == songCache.ActiveColumns.Count - 1)
                    {
                        // Calculate the remaining width
                        int columnsWidth = 0;
                        for (int c = 0; c < songCache.ActiveColumns.Count - 1; c++)
                        {
                            columnsWidth += songCache.ActiveColumns[c].Width;
                        }
                        columnWidth = ClientRectangle.Width - columnsWidth + hScrollBar.Value;
                    }

                    // Check if mouse is over this column header
                    if (column.IsMouseOverColumnHeader)
                    {
                        // Draw header (for some reason, the Y must be set -1 to cover an area which isn't supposed to be displayed)                        
                        rect = new Rectangle(offsetX - hScrollBar.Value, -1, column.Width, songCache.LineHeight + 1);
                        brushGradient = new LinearGradientBrush(rect, theme.HeaderHoverColor1, theme.HeaderHoverColor2, 90);
                        g.FillRectangle(brushGradient, rect);
                        brushGradient.Dispose();
                        brushGradient = null;
                    }
                    else if (column.IsUserMovingColumn)
                    {
                        // Draw header (for some reason, the Y must be set -1 to cover an area which isn't supposed to be displayed)                        
                        rect = new Rectangle(offsetX - hScrollBar.Value, -1, column.Width, songCache.LineHeight + 1);
                        brushGradient = new LinearGradientBrush(rect, Color.Blue, Color.Green, 90);
                        g.FillRectangle(brushGradient, rect);
                        brushGradient.Dispose();
                        brushGradient = null;
                    }

                    // Check if the header title must be displayed
                    if (songCache.ActiveColumns[b].IsHeaderTitleVisible)
                    {
                        // Display title                
                        Rectangle rectTitle = new Rectangle(offsetX - hScrollBar.Value, theme.Padding / 2, column.Width, songCache.LineHeight - theme.Padding + 2);
                        stringFormat.Trimming = StringTrimming.EllipsisCharacter;
                        brush = new SolidBrush(theme.HeaderForeColor);
                        g.DrawString(column.Title, fontDefaultBold, brush, rectTitle, stringFormat);
                        brush.Dispose();
                        brush = null;
                    }

                    // Draw column separator line; determine the height of the line
                    int columnHeight = ClientRectangle.Height;

                    // Determine the height of the line; if the items don't fit the control height...
                    if (items.Count < songCache.NumberOfLinesFittingInControl)
                    {
                        // Set height as the number of items (plus header)
                        columnHeight = (items.Count + 1) * songCache.LineHeight;
                    }

                    // Draw line
                    g.DrawLine(Pens.DarkGray, new Point(offsetX + column.Width - hScrollBar.Value, 0), new Point(offsetX + column.Width - hScrollBar.Value, columnHeight));

                    // Check if the column is ordered by
                    if (column.FieldName == orderByFieldName && !String.IsNullOrEmpty(column.FieldName))
                    {
                        // Create triangle points,,,
                        PointF[] ptTriangle = new PointF[3];

                        // ... depending on the order by ascending value
                        int triangleWidthHeight = 8;
                        int trianglePadding = (songCache.LineHeight - triangleWidthHeight) / 2;
                        if (orderByAscending)
                        {
                            // Create points for ascending
                            ptTriangle[0] = new PointF(offsetX + column.Width - triangleWidthHeight - (triangleWidthHeight / 2) - hScrollBar.Value, trianglePadding);
                            ptTriangle[1] = new PointF(offsetX + column.Width - triangleWidthHeight - hScrollBar.Value, songCache.LineHeight - trianglePadding);
                            ptTriangle[2] = new PointF(offsetX + column.Width - triangleWidthHeight - triangleWidthHeight - hScrollBar.Value, songCache.LineHeight - trianglePadding);
                        }
                        else
                        {
                            // Create points for descending
                            ptTriangle[0] = new PointF(offsetX + column.Width - triangleWidthHeight - (triangleWidthHeight / 2) - hScrollBar.Value, songCache.LineHeight - trianglePadding);
                            ptTriangle[1] = new PointF(offsetX + column.Width - triangleWidthHeight - hScrollBar.Value, trianglePadding);
                            ptTriangle[2] = new PointF(offsetX + column.Width - triangleWidthHeight - triangleWidthHeight - hScrollBar.Value, trianglePadding);
                        }

                        // Draw triangle
                        pen = new Pen(theme.HeaderForeColor);
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
                g.DrawRectangle(pen, new Rectangle(columnMoveMarkerX - hScrollBar.Value, 0, 1, ClientRectangle.Height));
                pen.Dispose();
                pen = null;
            }

            // Display debug information if enabled
            if (displayDebugInformation)
            {
                // Build debug string
                StringBuilder sbDebug = new StringBuilder();
                sbDebug.AppendLine("Line Count: " + items.Count.ToString());
                sbDebug.AppendLine("Line Height: " + songCache.LineHeight.ToString());
                sbDebug.AppendLine("Lines Fit In Height: " + songCache.NumberOfLinesFittingInControl.ToString());
                sbDebug.AppendLine("Total Width: " + songCache.TotalWidth);
                sbDebug.AppendLine("Total Height: " + songCache.TotalHeight);
                sbDebug.AppendLine("Scrollbar Offset Y: " + songCache.ScrollBarOffsetY);
                sbDebug.AppendLine("HScrollbar Maximum: " + hScrollBar.Maximum.ToString());
                sbDebug.AppendLine("HScrollbar LargeChange: " + hScrollBar.LargeChange.ToString());
                sbDebug.AppendLine("HScrollbar Value: " + hScrollBar.Value.ToString());
                sbDebug.AppendLine("VScrollbar Maximum: " + vScrollBar.Maximum.ToString());
                sbDebug.AppendLine("VScrollbar LargeChange: " + vScrollBar.LargeChange.ToString());
                sbDebug.AppendLine("VScrollbar Value: " + vScrollBar.Value.ToString());

                // Measure string
                stringFormat.Trimming = StringTrimming.Word;
                stringFormat.LineAlignment = StringAlignment.Near;
                SizeF sizeDebugText = g.MeasureString(sbDebug.ToString(), fontDefault, columns[0].Width - 1, stringFormat);
                rectF = new RectangleF(0, 0, columns[0].Width - 1, sizeDebugText.Height);

                // Draw background
                brush = new SolidBrush(Color.FromArgb(200, 0, 0, 0));
                g.FillRectangle(brush, rectF);
                brush.Dispose();
                brush = null;

                // Draw string
                g.DrawString(sbDebug.ToString(), fontDefault, Brushes.White, rectF, stringFormat);
            }

            // If both scrollbars are visible...
            if (hScrollBar.Visible && vScrollBar.Visible)
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
            if (vScrollBar.Maximum - vScrollBar.LargeChange < vScrollBar.Value)
            {
                // Set new scrollbar value
                vScrollBar.Value = vScrollBar.Maximum - vScrollBar.LargeChange + 1;
            }

            // Set horizontal scrollbar width and position
            hScrollBar.Top = ClientRectangle.Height - hScrollBar.Height;

            // Invalidate cache
            InvalidateSongCache();

            base.OnResize(e);
        }

        /// <summary>
        /// Occurs when the user changes the horizontal scrollbar value.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        protected void hScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            // Redraw control
            Refresh();
        }

        /// <summary>
        /// Occurs when the user changes the vertical scrollbar value.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        protected void vScrollBar_Scroll(object sender, ScrollEventArgs e)
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
            isMouseOverControl = true;

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
            isMouseOverControl = false;

            // Check if all the data is valid
            if (columns == null || songCache == null)
            {
                return;
            }

            // Calculate scrollbar offset Y
            int scrollbarOffsetY = (startLineNumber * songCache.LineHeight) - vScrollBar.Value;

            // Check if there's at least one item
            if (items.Count > 0)
            {
                // Reset mouse over item flags
                for (int b = startLineNumber; b < startLineNumber + numberOfLinesToDraw; b++)
                {
                    // Check if the mouse was over this item
                    if (items[b].IsMouseOverItem)
                    {
                        // Reset flag and invalidate region
                        items[b].IsMouseOverItem = false;
                        Invalidate(new Rectangle(columns[0].Width - hScrollBar.Value, ((b - startLineNumber + 1) * songCache.LineHeight) + scrollbarOffsetY, ClientRectangle.Width - columns[0].Width + hScrollBar.Value, songCache.LineHeight));
                        controlNeedsToBeUpdated = true;

                        // Exit loop
                        break;
                    }
                }
            }

            // Reset column flags
            int columnOffsetX2 = 0;
            for (int b = 0; b < songCache.ActiveColumns.Count; b++)
            {
                // Make sure column is visible
                if (songCache.ActiveColumns[b].Visible)
                {
                    // Was mouse over this column header?
                    if (songCache.ActiveColumns[b].IsMouseOverColumnHeader)
                    {
                        // Reset flag
                        songCache.ActiveColumns[b].IsMouseOverColumnHeader = false;

                        // Invalidate region
                        Invalidate(new Rectangle(columnOffsetX2 - hScrollBar.Value, 0, songCache.ActiveColumns[b].Width, songCache.LineHeight));
                        controlNeedsToBeUpdated = true;
                    }

                    // Increment offset
                    columnOffsetX2 += songCache.ActiveColumns[b].Width;
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
            dragStartX = e.X;

            // Check if all the data is valid
            if (columns == null || songCache == null)
            {
                return;
            }

            // Loop through columns
            foreach (SongGridViewColumn column in songCache.ActiveColumns)
            {
                // Check for resizing column
                if (column.IsMouseCursorOverColumnLimit && column.CanBeResized && CanResizeColumns)
                {
                    // Set resizing column flag
                    column.IsUserResizingColumn = true;

                    // Save the original column width
                    dragOriginalColumnWidth = column.Width;
                }
            }
            
            // Check if the left mouse button is held
            if(e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                // Set holding button flag
                isUserHoldingLeftMouseButton = true;
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
            dragStartX = -1;
            bool updateControl = false;
            isUserHoldingLeftMouseButton = false;

            // Check if all the data is valid
            if (columns == null || songCache == null)
            {
                return;
            }

            // Loop through columns
            SongGridViewColumn columnMoving = null;
            foreach (SongGridViewColumn column in songCache.ActiveColumns)
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
                for (int a = 0; a < songCache.ActiveColumns.Count; a++ )
                {
                    // Set current column
                    SongGridViewColumn currentColumn = songCache.ActiveColumns[a];

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
                        if (e.X >= x - hScrollBar.Value && e.X <= x + (currentColumn.Width / 2) - hScrollBar.Value)
                        {
                            // Check flag
                            if (isPastCurrentlyMovingColumn && currentColumn.FieldName != columnMoving.FieldName)
                            {
                                // Set column
                                columnOver = songCache.ActiveColumns[a - 1];
                            }
                            else
                            {
                                // Set column
                                columnOver = songCache.ActiveColumns[a];
                            }
                            break;                               
                        }
                        // Check if the cursor is over the right part of the column
                        else if (e.X >= x + (currentColumn.Width / 2) - hScrollBar.Value && e.X <= x + currentColumn.Width - hScrollBar.Value)
                        {
                            // Check if there is a next item
                            if (a < songCache.ActiveColumns.Count - 1)
                            {
                                // Check flag
                                if (isPastCurrentlyMovingColumn)
                                {
                                    // Set column
                                    columnOver = songCache.ActiveColumns[a];
                                }
                                else
                                {
                                    // Set column
                                    columnOver = songCache.ActiveColumns[a + 1];
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
                List<SongGridViewColumn> columnsOrdered = columns.OrderBy(q => q.Order).ToList();

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
            if (columns == null || songCache == null)
            {
                return;
            }

            // Calculate album cover art width
            int albumArtCoverWidth = columns[0].Visible ? columns[0].Width : 0;

            // Make sure the control is focused
            if (!Focused)
            {
                // Set control focus
                Focus();
            }

            // Show context menu strip if the button click is right and not the album art column
            if (e.Button == System.Windows.Forms.MouseButtons.Right &&
                e.X > columns[0].Width)
            {
                // Is there a context menu strip configured?
                if (contextMenuStrip != null)
                {
                    // Show context menu strip
                    contextMenuStrip.Show(Control.MousePosition.X, Control.MousePosition.Y);
                }
            }

            // Check if the user is resizing a column
            SongGridViewColumn columnResizing = columns.FirstOrDefault(x => x.IsUserResizingColumn == true);

            // Calculate scrollbar offset Y
            int scrollbarOffsetY = (startLineNumber * songCache.LineHeight) - vScrollBar.Value;

            // Check if the user has clicked on the header (for orderBy)
            if (e.Y >= 0 &&
                e.Y <= songCache.LineHeight &&
                columnResizing == null &&
                !IsColumnMoving)
            {
                // Check on what column the user has clicked
                int offsetX = 0;
                for (int a = 0; a < songCache.ActiveColumns.Count; a++)
                {
                    // Get current column
                    SongGridViewColumn column = songCache.ActiveColumns[a];

                    // Make sure column is visible
                    if (column.Visible)
                    {
                        // Check if the mouse pointer is over this column
                        if (e.X >= offsetX - hScrollBar.Value && e.X <= offsetX + column.Width - hScrollBar.Value)
                        {
                            // Check mouse button
                            if (e.Button == System.Windows.Forms.MouseButtons.Left && CanChangeOrderBy)
                            {
                                // Check if the column order was already set
                                if (orderByFieldName == column.FieldName)
                                {
                                    // Reverse ascending/descending
                                    orderByAscending = !orderByAscending;
                                }
                                else
                                {
                                    // Set order by field name
                                    orderByFieldName = column.FieldName;

                                    // By default, the order is ascending
                                    orderByAscending = true;
                                }

                                // Invalidate cache and song items
                                items = null;
                                songCache = null;

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
                                foreach(ToolStripMenuItem menuItem in menuColumns.Items)
                                {
                                    // Get column
                                    SongGridViewColumn menuItemColumn = columns.FirstOrDefault(x => x.Title == menuItem.Tag.ToString());
                                    if (menuItemColumn != null)
                                    {
                                        menuItem.Checked = menuItemColumn.Visible;
                                    }
                                }

                                // Display columns contextual menu
                                menuColumns.Show(this, e.X, e.Y);
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
            for (int a = startLineNumber; a < startLineNumber + numberOfLinesToDraw; a++)
            {
                // Check if the item is selected
                if (items[a].IsSelected)
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
                int startY = ((startIndex - startLineNumber + 1) * songCache.LineHeight) + scrollbarOffsetY;
                int endY = ((endIndex - startLineNumber + 2) * songCache.LineHeight) + scrollbarOffsetY;
                Invalidate(new Rectangle(albumArtCoverWidth - hScrollBar.Value, startY, ClientRectangle.Width - albumArtCoverWidth + hScrollBar.Value, endY - startY));
            }

            // Reset selection (make sure SHIFT or CTRL isn't held down)
            if ((Control.ModifierKeys & Keys.Shift) == 0 &&
               (Control.ModifierKeys & Keys.Control) == 0)
            {
                SongGridViewItem mouseOverItem = items.FirstOrDefault(x => x.IsMouseOverItem == true);
                if (mouseOverItem != null)
                {
                    // Reset selection, unless the CTRL key is held (TODO)
                    List<SongGridViewItem> selectedItems = items.Where(x => x.IsSelected == true).ToList();
                    foreach (SongGridViewItem item in selectedItems)
                    {
                        // Reset flag
                        item.IsSelected = false;
                    }
                }
            }

            // Loop through visible lines to update the new selected item
            bool invalidatedNewSelection = false;
            for (int a = startLineNumber; a < startLineNumber + numberOfLinesToDraw; a++)
            {
                // Check if mouse is over this item
                if (items[a].IsMouseOverItem)
                {
                    // Set flag
                    invalidatedNewSelection = true;

                    // Check if SHIFT is held
                    if ((Control.ModifierKeys & Keys.Shift) != 0)
                    {
                        // Find the start index of the selection
                        int startIndexSelection = lastItemIndexClicked;
                        if (a < startIndexSelection)
                        {
                            startIndexSelection = a;
                        }

                        // Find the end index of the selection
                        int endIndexSelection = lastItemIndexClicked;
                        if (a > endIndexSelection)
                        {
                            endIndexSelection = a + 1;
                        }

                        // Loop through items to selected
                        for (int b = startIndexSelection; b < endIndexSelection; b++)
                        {
                            // Set items as selected
                            items[b].IsSelected = true;
                        }

                        // Invalidate region
                        Invalidate();
                    }
                    // Check if CTRL is held
                    else if ((Control.ModifierKeys & Keys.Control) != 0)
                    {
                        // Invert selection
                        items[a].IsSelected = !items[a].IsSelected;

                        // Invalidate region
                        Invalidate(new Rectangle(albumArtCoverWidth - hScrollBar.Value, ((a - startLineNumber + 1) * songCache.LineHeight) + scrollbarOffsetY, ClientRectangle.Width - albumArtCoverWidth + hScrollBar.Value, songCache.LineHeight));
                    }
                    else
                    {
                        // Set this item as the new selected item
                        items[a].IsSelected = true;

                        // Invalidate region
                        Invalidate(new Rectangle(albumArtCoverWidth - hScrollBar.Value, ((a - startLineNumber + 1) * songCache.LineHeight) + scrollbarOffsetY, ClientRectangle.Width - albumArtCoverWidth + hScrollBar.Value, songCache.LineHeight));
                    }

                    // Set the last item clicked index
                    lastItemIndexClicked = a;

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
            if (columns == null || songCache == null)
            {
                return;
            }

            // Calculate album cover art width
            int albumArtCoverWidth = columns[0].Visible ? columns[0].Width : 0;

            // Calculate scrollbar offset Y
            int scrollbarOffsetY = (startLineNumber * songCache.LineHeight) - vScrollBar.Value;

            // Keep original songId in case the now playing value is set before invalidating the older value
            Guid originalId = Guid.Empty;

            // Check mode
            if (mode == SongGridViewMode.AudioFile)
            {
                // Set original id
                originalId = nowPlayingAudioFileId;
            }
            else if (mode == SongGridViewMode.Playlist)
            {
                // Set original id
                originalId = nowPlayingPlaylistItemId;
            }

            // Loop through visible lines
            for (int a = startLineNumber; a < startLineNumber + numberOfLinesToDraw; a++)
            {
                // Check if mouse is over this item
                if (items[a].IsMouseOverItem)
                {
                    // Set this item as the new now playing
                    nowPlayingAudioFileId = items[a].AudioFile.Id;
                    nowPlayingPlaylistItemId = items[a].PlaylistItemId;

                    // Invalidate region
                    Invalidate(new Rectangle(albumArtCoverWidth - hScrollBar.Value, ((a - startLineNumber + 1) * songCache.LineHeight) + scrollbarOffsetY, ClientRectangle.Width - albumArtCoverWidth + hScrollBar.Value, songCache.LineHeight));
                }
                else if (mode == SongGridViewMode.AudioFile && items[a].AudioFile.Id == originalId)
                {
                    // Invalidate region
                    Invalidate(new Rectangle(albumArtCoverWidth - hScrollBar.Value, ((a - startLineNumber + 1) * songCache.LineHeight) + scrollbarOffsetY, ClientRectangle.Width - albumArtCoverWidth + hScrollBar.Value, songCache.LineHeight));
                }
                else if (mode == SongGridViewMode.Playlist && items[a].PlaylistItemId == originalId)
                {
                    // Invalidate region
                    Invalidate(new Rectangle(albumArtCoverWidth - hScrollBar.Value, ((a - startLineNumber + 1) * songCache.LineHeight) + scrollbarOffsetY, ClientRectangle.Width - albumArtCoverWidth + hScrollBar.Value, songCache.LineHeight));
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
            if (columns == null || songCache == null)
            {
                return;
            }

            // Calculate album cover art width
            int albumArtCoverWidth = columns[0].Visible ? columns[0].Width : 0;

            // Check if the user is currently resizing a column (loop through columns)
            foreach (SongGridViewColumn column in songCache.ActiveColumns)
            {
                // Check if the user is currently resizing this column
                if (column.IsUserResizingColumn && column.Visible)
                {
                    // Calculate the new column width
                    int newWidth = dragOriginalColumnWidth - (dragStartX - e.X);

                    // Make sure the width isn't lower than the minimum width
                    if (newWidth < minimumColumnWidth)
                    {
                        // Set width as minimum column width
                        newWidth = minimumColumnWidth;
                    }

                    // Set column width
                    column.Width = newWidth;

                    // Refresh control (invalidate whole control)
                    controlNeedsToBeUpdated = true;
                    Invalidate();
                    InvalidateSongCache();

                    // Auto adjust horizontal scrollbar value if it exceeds the value range (i.e. do not show empty column)
                    if (hScrollBar.Value > hScrollBar.Maximum - hScrollBar.LargeChange)
                    {
                        // Set new value
                        int tempValue = hScrollBar.Maximum - hScrollBar.LargeChange;
                        if (tempValue < 0)
                        {
                            tempValue = 0;
                        }
                        hScrollBar.Value = tempValue;
                    }
                }

                // Check if the user is moving the column
                if (column.IsMouseOverColumnHeader && column.CanBeMoved && CanMoveColumns && isUserHoldingLeftMouseButton && !IsColumnResizing)
                {
                    // Check if the X position has changed by at least 2 pixels (i.e. dragging)
                    if (dragStartX >= e.X + 2 ||
                        dragStartX <= e.X - 2)
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
                    foreach (SongGridViewColumn columnOver in songCache.ActiveColumns)
                    {
                        // Check if column is visible
                        if (columnOver.Visible)
                        {
                            // Check if the cursor is over the left part of the column
                            if (e.X >= x - hScrollBar.Value && e.X <= x + (columnOver.Width / 2) - hScrollBar.Value)
                            {
                                // Set marker
                                columnMoveMarkerX = x;
                            }
                            // Check if the cursor is over the right part of the column
                            else if (e.X >= x + (columnOver.Width / 2) - hScrollBar.Value && e.X <= x + columnOver.Width - hScrollBar.Value)
                            {
                                // Set marker
                                columnMoveMarkerX = x + columnOver.Width;
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
                foreach (SongGridViewColumn column in songCache.ActiveColumns)
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
                            if (e.X >= offsetX - hScrollBar.Value && e.X <= offsetX + 1 - hScrollBar.Value)
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
                for (int b = 0; b < songCache.ActiveColumns.Count; b++)
                {
                    // Get current column
                    SongGridViewColumn column = songCache.ActiveColumns[b];

                    // Check if column is visible
                    if (column.Visible)
                    {
                        // Was mouse over this column header?
                        if (column.IsMouseOverColumnHeader)
                        {
                            // Reset flag
                            column.IsMouseOverColumnHeader = false;

                            // Invalidate region
                            Invalidate(new Rectangle(columnOffsetX2 - hScrollBar.Value, 0, column.Width, songCache.LineHeight));
                            controlNeedsToBeUpdated = true;
                        }

                        // Increment offset
                        columnOffsetX2 += column.Width;
                    }
                }

                // Check if the mouse pointer is over the header
                if (e.Y >= 0 &&
                    e.Y <= songCache.LineHeight)
                {
                    // Check on what column the user has clicked
                    int columnOffsetX = 0;
                    for (int a = 0; a < songCache.ActiveColumns.Count; a++)
                    {
                        // Get current column
                        SongGridViewColumn column = songCache.ActiveColumns[a];

                        // Make sure column is visible
                        if (column.Visible)
                        {
                            // Check if the mouse pointer is over this column
                            if (e.X >= columnOffsetX - hScrollBar.Value && e.X <= columnOffsetX + column.Width - hScrollBar.Value)
                            {
                                // Set flag
                                column.IsMouseOverColumnHeader = true;

                                // Invalidate region
                                Invalidate(new Rectangle(columnOffsetX - hScrollBar.Value, 0, column.Width, songCache.LineHeight));

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
                int scrollbarOffsetY = (startLineNumber * songCache.LineHeight) - vScrollBar.Value;

                // Check if there's at least one item
                if (items.Count > 0)
                {
                    // Reset mouse over item flags
                    for (int b = startLineNumber; b < startLineNumber + numberOfLinesToDraw; b++)
                    {
                        // Check if the mouse was over this item
                        if (items[b].IsMouseOverItem)
                        {
                            // Reset flag and invalidate region
                            items[b].IsMouseOverItem = false;
                            Invalidate(new Rectangle(albumArtCoverWidth - hScrollBar.Value, ((b - startLineNumber + 1) * songCache.LineHeight) + scrollbarOffsetY, ClientRectangle.Width - albumArtCoverWidth + hScrollBar.Value, songCache.LineHeight));

                            // Exit loop
                            break;
                        }
                    }

                    // Put new mouse over flag
                    for (int a = startLineNumber; a < startLineNumber + numberOfLinesToDraw; a++)
                    {
                        // Get audio file
                        AudioFile audioFile = items[a].AudioFile;

                        // Calculate offset
                        offsetY = (a * songCache.LineHeight) - vScrollBar.Value + songCache.LineHeight;

                        // Check if the mouse cursor is over this line (and not already mouse over)
                        if (e.X >= albumArtCoverWidth - hScrollBar.Value &&
                            e.Y >= offsetY &&
                            e.Y <= offsetY + songCache.LineHeight &&
                            !items[a].IsMouseOverItem)
                        {
                            // Set item as mouse over
                            items[a].IsMouseOverItem = true;

                            // Invalidate region                    
                            Invalidate(new Rectangle(albumArtCoverWidth - hScrollBar.Value, offsetY, ClientRectangle.Width - albumArtCoverWidth + hScrollBar.Value, songCache.LineHeight));

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
                SongGridViewColumn column = columns.FirstOrDefault(x => x.Title == menuItem.Tag.ToString());
                if (column != null)
                {
                    // Set visibility
                    column.Visible = menuItem.Checked;
                }

                // Reset cache
                songCache = null;
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
                if (columns == null || songCache == null)
                {
                    return false;
                }

                // Make sure the mouse cursor is over the control, and that the vertical scrollbar is enabled
                if (!isMouseOverControl || !vScrollBar.Enabled)
                {
                    return false;
                }

                // Get relative value
                int value = delta / SystemInformation.MouseWheelScrollDelta;

                // Set new value
                int newValue = vScrollBar.Value + (-value * songCache.LineHeight);

                // Check for maximum
                if (newValue > vScrollBar.Maximum - vScrollBar.LargeChange)
                {
                    newValue = vScrollBar.Maximum - vScrollBar.LargeChange;
                }
                // Check for minimum
                if (newValue < 0)
                {
                    newValue = 0;
                }

                // Set scrollbar value
                vScrollBar.Value = newValue;

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
            if (columns == null || columns.Count == 0 || items == null)
            {
                return;
            }

            // Create cache
            songCache = new SongGridViewCache();

            // Get active columns and order them
            songCache.ActiveColumns = columns.Where(x => x.Order >= 0).OrderBy(x => x.Order).ToList();

            // Load custom font
            //Font fontDefaultBold = Tools.LoadCustomFont(FontCollection, CustomFontName, Font.Size, FontStyle.Bold);
            Font fontDefaultBold = null;

            // Make sure the embedded font name needs to be loaded and is valid
            if (theme.Font.UseEmbeddedFont && !String.IsNullOrEmpty(theme.Font.EmbeddedFontName))
            {
                try
                {
                    // Get embedded font collection
                    EmbeddedFontCollection embeddedFonts = EmbeddedFontHelper.GetEmbeddedFonts();

                    // Get embedded fonts                    
                    fontDefaultBold = Tools.LoadEmbeddedFont(embeddedFonts, theme.Font.EmbeddedFontName, theme.Font.Size, theme.Font.ToFontStyle() | FontStyle.Bold);
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
                    fontDefaultBold = new Font(theme.Font.StandardFontName, theme.Font.Size, theme.Font.ToFontStyle() | FontStyle.Bold);
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
            songCache.LineHeight = (int)sizeFont.Height + theme.Padding;
            songCache.TotalHeight = songCache.LineHeight * items.Count;

            // Check if the total active columns width exceed the width available in the control
            songCache.TotalWidth = 0;
            for (int a = 0; a < songCache.ActiveColumns.Count; a++)
            {
                // Check if column is visible
                if (songCache.ActiveColumns[a].Visible)
                {
                    // Increment total width
                    songCache.TotalWidth += songCache.ActiveColumns[a].Width;
                }
            }

            // Calculate the number of lines visible (count out the header, which is one line height)
            songCache.NumberOfLinesFittingInControl = (int)Math.Floor((double)(ClientRectangle.Height) / (double)(songCache.LineHeight));

            // Set vertical scrollbar dimensions
            vScrollBar.Top = songCache.LineHeight;
            vScrollBar.Left = ClientRectangle.Width - vScrollBar.Width;
            vScrollBar.Minimum = 0;

            // Scrollbar maximum is the number of lines fitting in the screen + the remaining line which might be cut
            // by the control height because it's not a multiple of line height (i.e. the last line is only partly visible)
            int lastLineHeight = ClientRectangle.Height - (songCache.LineHeight * songCache.NumberOfLinesFittingInControl);

            // Check width
            if (songCache.TotalWidth > ClientRectangle.Width - vScrollBar.Width)
            {
                // Set scrollbar values
                hScrollBar.Maximum = songCache.TotalWidth;
                hScrollBar.SmallChange = 5;
                hScrollBar.LargeChange = ClientRectangle.Width;

                // Show scrollbar
                hScrollBar.Visible = true;
            }

            // Check if the horizontal scrollbar needs to be turned off
            if (songCache.TotalWidth <= ClientRectangle.Width - vScrollBar.Width && hScrollBar.Visible)
            {
                // Hide the horizontal scrollbar
                hScrollBar.Visible = false;
            }

            // If there are less items than items fitting on screen...            
            if (((songCache.NumberOfLinesFittingInControl - 1) * songCache.LineHeight) - hScrollBar.Height >= songCache.TotalHeight)
            {
                // Disable the scrollbar
                vScrollBar.Enabled = false;
                vScrollBar.Value = 0;
            }
            else
            {
                // Set scrollbar values
                vScrollBar.Enabled = true;

                // The real large change needs to be added to the LargeChange and Maximum property in order to work. 
                int realLargeChange = songCache.LineHeight * 5;

                // Calculate the vertical scrollbar maximum
                int vMax = songCache.LineHeight * (items.Count - songCache.NumberOfLinesFittingInControl + 1) - lastLineHeight + realLargeChange;

                // Add the horizontal scrollbar height if visible
                if (hScrollBar.Visible)
                {
                    // Add height
                    vMax += hScrollBar.Height;
                }
                
                // Compensate for the header, and for the last line which might be truncated by the control height
                vScrollBar.Maximum = vMax;
                vScrollBar.SmallChange = songCache.LineHeight;
                vScrollBar.LargeChange = 1 + realLargeChange;
            }

            // Calculate the scrollbar offset Y
            songCache.ScrollBarOffsetY = (startLineNumber * songCache.LineHeight) - vScrollBar.Value;

            // If both scrollbars need to be visible, the width and height must be changed
            if (hScrollBar.Visible && vScrollBar.Visible)
            {
                // Cut 16 pixels
                hScrollBar.Width = ClientRectangle.Width - 16;
                vScrollBar.Height = ClientRectangle.Height - songCache.LineHeight - 16;
            }
            else
            {
                vScrollBar.Height = ClientRectangle.Height - songCache.LineHeight;
            }
        }

        /// <summary>
        /// This timer triggers the animation of the currently playing song.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        protected void timerAnimationNowPlaying_Tick(object sender, EventArgs e)
        {
            // If the rectangle is "empty", do not trigger invalidation
            if (rectNowPlaying.X == 0 &&
                rectNowPlaying.Y == 0 &&
                rectNowPlaying.Width == 1 &&
                rectNowPlaying.Height == 1)
            {
                return;
            }

            // Increment counter            
            timerAnimationNowPlayingCount += 10;

            // Invalidate region for now playing
            Invalidate(rectNowPlaying);
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
