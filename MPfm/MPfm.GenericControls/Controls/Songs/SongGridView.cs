// Copyright © 2011-2013 Yanick Castonguay
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
using System.Linq;
using System.Timers;
using MPfm.GenericControls.Basics;
using MPfm.GenericControls.Graphics;
using MPfm.GenericControls.Interaction;
using MPfm.GenericControls.Wrappers;
using MPfm.Sound.AudioFiles;
using MPfm.Sound.Playlists;
using MPfm.WindowsControls;

namespace MPfm.GenericControls.Controls.Songs
{
    /// <summary>
    /// This custom grid view control displays the MPfm library.
    /// </summary>
    public class SongGridView : IControl, IControlMouseInteraction
    {
        private SongGridViewMode _mode = SongGridViewMode.AudioFile;

        // Controls
        private IVerticalScrollBarWrapper _vScrollBar = null;
        private IHorizontalScrollBarWrapper _hScrollBar = null;
        //private ContextMenuStrip _menuColumns = null;

        // Background worker for updating album art
        private int _preloadLinesAlbumCover = 20;
        private BackgroundWorker _workerUpdateAlbumArt = null;
        private List<SongGridViewBackgroundWorkerArgument> _workerUpdateAlbumArtPile = null;
        private Timer _timerUpdateAlbumArt = null;        
        private SongGridViewCache _songCache = null;
        private List<SongGridViewImageCache> _imageCache = new List<SongGridViewImageCache>();

        private const int MinimumColumnWidth = 30;

        // Private variables used for mouse events
        private int _columnMoveMarkerX = 0;
        private int _startLineNumber = 0;
        private int _numberOfLinesToDraw = 0;
        private int _dragStartX = -1;
        private int _dragOriginalColumnWidth = -1;
        private bool _isMouseOverControl = false;
        private bool _isUserHoldingLeftMouseButton = false;        
        private int _lastItemIndexClicked = -1;

        // Animation timer and counter for currently playing song
        private int _timerAnimationNowPlayingCount = 0;
        private BasicRectangle _rectNowPlaying = new BasicRectangle(0, 0, 1, 1);
        private Timer timerAnimationNowPlaying = null;

        public delegate void SelectedIndexChanged(SongGridViewSelectedIndexChangedData data);
        public delegate void ColumnClick(SongGridViewColumnClickData data);

        public event InvalidateVisual OnInvalidateVisual;
        public event InvalidateVisualInRect OnInvalidateVisualInRect;

        /// <summary>
        /// The OnSelectedIndexChanged event is triggered when the selected item(s) have changed.
        /// </summary>
        public event SelectedIndexChanged OnSelectedIndexChanged;

        /// <summary>
        /// The ColumnClick event is triggered when the user has clicked on one of the columns.
        /// </summary>
        public event ColumnClick OnColumnClick;

        #region Properties
        
        public BasicRectangle Frame { get; set; }

        private SongGridViewTheme _theme;
        /// <summary>
        /// Defines the current theme used for rendering the control.
        /// </summary>
        public SongGridViewTheme Theme
        {
            get
            {
                return _theme;
            }
            set
            {
                _theme = value;
            }
        }

        #region Other Properties (Items, Columns, Menus, etc.)

        private List<SongGridViewItem> _items;
        /// <summary>
        /// List of grid view items (representing songs).
        /// </summary>
        [Browsable(false)]
        public List<SongGridViewItem> Items
        {
            get
            {
                return _items;
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
                if (_items != null)
                    return _items.Where(x => x.IsSelected).ToList();

                return null;
            }
        }

        private List<SongGridViewColumn> _columns;
        /// <summary>
        /// List of grid view columns.
        /// </summary>
        [Browsable(false)]
        public List<SongGridViewColumn> Columns
        {
            get
            {
                return _columns;
            }
        }

        private Guid _nowPlayingAudioFileId = Guid.Empty;
        /// <summary>
        /// Defines the currently playing audio file identifier.
        /// </summary>
        [Browsable(false)]
        public Guid NowPlayingAudioFileId
        {
            get
            {
                return _nowPlayingAudioFileId;
            }
            set
            {
                _nowPlayingAudioFileId = value;
            }
        }

        private Guid _nowPlayingPlaylistItemId = Guid.Empty;
        /// <summary>
        /// Defines the currently playing playlist item identifier.
        /// </summary>
        [Browsable(false)]
        public Guid NowPlayingPlaylistItemId
        {
            get
            {
                return _nowPlayingPlaylistItemId;
            }
            set
            {
                _nowPlayingPlaylistItemId = value;
            }
        }

        //private ContextMenuStrip contextMenuStrip = null;
        ///// <summary>
        ///// ContextMenuStrip related to the grid. This context menu
        ///// opens when the user right clicks an item.
        ///// </summary>
        //[Category("Misc"), Browsable(true), Description("Stuff.")]
        //public override ContextMenuStrip ContextMenuStrip
        //{
        //    get
        //    {
        //        return contextMenuStrip;
        //    }
        //    set
        //    {
        //        contextMenuStrip = value;
        //    }
        //}

        #endregion

        #region Filter / OrderBy Properties

        private string _orderByFieldName = string.Empty;
        /// <summary>
        /// Indicates which field should be used for ordering songs.
        /// </summary>
        public string OrderByFieldName
        {
            get
            {
                return _orderByFieldName;
            }
            set
            {
                _orderByFieldName = value;

                // Invalidate item list and cache
                _items = null;
                _songCache = null;

                // Refresh whole control
                OnInvalidateVisual();
            }
        }

        private bool _orderByAscending = true;
        /// <summary>
        /// Indicates if the order should be ascending (true) or descending (false).
        /// </summary>
        public bool OrderByAscending
        {
            get
            {
                return _orderByAscending;
            }
            set
            {
                _orderByAscending = value;
            }
        }

        #endregion

        #region Settings Properties

        private bool _displayDebugInformation = false;
        /// <summary>
        /// If true, the debug information will be shown over the first column.
        /// </summary>
        public bool DisplayDebugInformation
        {
            get
            {
                return _displayDebugInformation;
            }
            set
            {
                _displayDebugInformation = value;
            }
        }

        private bool _canResizeColumns = true;
        /// <summary>
        /// Indicates if the user can resize the columns or not.
        /// </summary>
        public bool CanResizeColumns
        {
            get
            {
                return _canResizeColumns;
            }
            set
            {
                _canResizeColumns = value;
            }
        }

        private bool _canMoveColumns = true;
        /// <summary>
        /// Indicates if the user can move the columns or not.
        /// </summary>
        public bool CanMoveColumns
        {
            get
            {
                return _canMoveColumns;
            }
            set
            {
                _canMoveColumns = value;
            }
        }

        private bool _canChangeOrderBy = true;
        /// <summary>
        /// Indicates if the user can change the order by or not.
        /// </summary>
        public bool CanChangeOrderBy
        {
            get
            {
                return _canChangeOrderBy;
            }
            set
            {
                _canChangeOrderBy = value;
            }
        }

        private bool _canReorderItems = true;
        /// <summary>
        /// Indicates if the user can reorder the items or not.
        /// </summary>
        public bool CanReorderItems
        {
            get
            {
                return _canReorderItems;
            }
            set
            {
                _canReorderItems = value;
            }
        }

        private int _imageCacheSize = 10;
        /// <summary>
        /// Defines the size of the album art image cache (10 by default).
        /// </summary>
        public int ImageCacheSize
        {
            get
            {
                return _imageCacheSize;
            }
            set
            {
                _imageCacheSize = value;
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
                foreach (var column in _columns)
                {
                    if (column.IsUserMovingColumn)
                        return true;
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
                foreach (SongGridViewColumn column in _columns)
                {
                    if (column.IsUserResizingColumn)
                        return true;
                }

                return false;
            }
        }

        #endregion

        /// <summary>
        /// Default constructor for SongGridView.
        /// </summary>
        public SongGridView(IHorizontalScrollBarWrapper horizontalScrollBar, IVerticalScrollBarWrapper verticalScrollBar)
        {
            OnInvalidateVisual += () => { };
            OnInvalidateVisualInRect += (rect) => { };

            _theme = new SongGridViewTheme();

            timerAnimationNowPlaying = new Timer();
            timerAnimationNowPlaying.Interval = 50;
            timerAnimationNowPlaying.Elapsed += TimerAnimationNowPlayingOnElapsed;
            timerAnimationNowPlaying.Enabled = true;

            _vScrollBar = verticalScrollBar;
            //_vScrollBar.Width = 16;
            //_vScrollBar.Scroll += new ScrollEventHandler(vScrollBar_Scroll);
            //Controls.Add(_vScrollBar);

            _hScrollBar = horizontalScrollBar;
            //// Create horizontal scrollbar
            //_hScrollBar = new System.Windows.Forms.HScrollBar();
            //_hScrollBar.Width = ClientRectangle.Width;
            //_hScrollBar.Height = 16;
            //_hScrollBar.Top = ClientRectangle.Height - _hScrollBar.Height;
            //_hScrollBar.Scroll += new ScrollEventHandler(hScrollBar_Scroll);
            //Controls.Add(_hScrollBar);

            //// Override mouse messages for mouse wheel (get mouse wheel events out of control)
            //Application.AddMessageFilter(this);

            // Create background worker for updating album art
            _workerUpdateAlbumArtPile = new List<SongGridViewBackgroundWorkerArgument>();
            _workerUpdateAlbumArt = new BackgroundWorker();            
            _workerUpdateAlbumArt.DoWork += new DoWorkEventHandler(workerUpdateAlbumArt_DoWork);
            _workerUpdateAlbumArt.RunWorkerCompleted += new RunWorkerCompletedEventHandler(workerUpdateAlbumArt_RunWorkerCompleted);
            
            // Create timer for updating album art
            _timerUpdateAlbumArt = new Timer();
            _timerUpdateAlbumArt.Interval = 10;
            _timerUpdateAlbumArt.Elapsed += TimerUpdateAlbumArtOnElapsed;
            _timerUpdateAlbumArt.Enabled = true;

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
            _columns = new List<SongGridViewColumn>();
            _columns.Add(columnSongAlbumCover);
            _columns.Add(columnSongAlbumTitle);
            _columns.Add(columnSongArtistName);
            _columns.Add(columnSongBitrate);            
            _columns.Add(columnSongFilePath);            
            _columns.Add(columnSongGenre);
            _columns.Add(columnSongLastPlayed);
            _columns.Add(columnSongLength);
            _columns.Add(columnSongNowPlaying);
            _columns.Add(columnSongPlayCount);
            _columns.Add(columnSongSampleRate);
            _columns.Add(columnSongTitle); // Song title
            _columns.Add(columnSongTempo);            
            _columns.Add(columnSongTrackNumber);
            _columns.Add(columnSongTrackCount);
            _columns.Add(columnSongFileType); // Type
            _columns.Add(columnSongYear);

            // Create contextual menu
            //_menuColumns = new System.Windows.Forms.ContextMenuStrip();

            //// Loop through columns
            //foreach (SongGridViewColumn column in _columns)
            //{
            //    // Add menu item                               
            //    ToolStripMenuItem menuItem = (ToolStripMenuItem)_menuColumns.Items.Add(column.Title);
            //    menuItem.Tag = column.Title;
            //    menuItem.Checked = column.Visible;
            //    menuItem.Click += new EventHandler(menuItemColumns_Click);
            //}
        }

        /// <summary>
        /// Occurs when the Update Album Art timer expires.
        /// Checks if there are more album art covers to load and starts the background
        /// thread if it is not running already.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void TimerUpdateAlbumArtOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            // Stop timer
            _timerUpdateAlbumArt.Enabled = false;

            // Check for the next album art to fetch
            if (_workerUpdateAlbumArtPile.Count > 0 && !_workerUpdateAlbumArt.IsBusy)
            {
                // Do some cleanup: clean items that are not visible anymore
                bool cleanUpDone = false;
                while (!cleanUpDone)
                {
                    int indexToDelete = -1;
                    for (int a = 0; a < _workerUpdateAlbumArtPile.Count; a++)
                    {
                        // Get argument
                        SongGridViewBackgroundWorkerArgument arg = _workerUpdateAlbumArtPile[a];

                        // Check if this album is still visible (cancel if it is out of display).                             
                        if (arg.LineIndex < _startLineNumber || arg.LineIndex > _startLineNumber + _numberOfLinesToDraw + _preloadLinesAlbumCover)
                        {
                            indexToDelete = a;
                            break;
                        }
                    }

                    if (indexToDelete >= 0)
                    {
                        _workerUpdateAlbumArtPile.RemoveAt(indexToDelete);                        
                    }
                    else
                    {
                        cleanUpDone = true;
                    }
                }
                // There must be more album art to fetch.. right?
                if (_workerUpdateAlbumArtPile.Count > 0)
                {
                    // Start background worker                
                    SongGridViewBackgroundWorkerArgument arg = _workerUpdateAlbumArtPile[0];
                    _workerUpdateAlbumArt.RunWorkerAsync(arg);
                }
            }

            // Restart timer
            _timerUpdateAlbumArt.Enabled = true;
        }

        /// <summary>
        /// Occurs when the Update Album Art background worker starts its work.
        /// Fetches the album cover in another thread and returns the image once done.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        public void workerUpdateAlbumArt_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument == null)
                return;

            // Cast argument
            var arg = (SongGridViewBackgroundWorkerArgument)e.Argument;

            // Create result
            var result = new SongGridViewBackgroundWorkerResult();
            result.AudioFile = arg.AudioFile;

            // Check if this album is still visible (cancel if it is out of display).     
            if (arg.LineIndex < _startLineNumber || arg.LineIndex > _startLineNumber + _numberOfLinesToDraw + _preloadLinesAlbumCover)
            {
                // Set result with empty image
                e.Result = result;
                return;
            }

            // Extract image from file
            //var imageAlbumCover = AudioFile.ExtractImageForAudioFile(arg.AudioFile.FilePath);

            // Set album art in return data            
            //result.AlbumArt = imageAlbumCover;
            //e.Result = result;
        }

        /// <summary>
        /// Occurs when the Update Album Art background worker has finished its work.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        public void workerUpdateAlbumArt_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result == null)
                return;

            // Create cover art cache (even if the albumart is null, just to make sure the grid doesn't refetch the album art endlessly)
            var result = (SongGridViewBackgroundWorkerResult)e.Result;
            var cache = new SongGridViewImageCache();
            cache.Key = result.AudioFile.ArtistName + "_" + result.AudioFile.AlbumTitle;
            cache.Image = result.AlbumArt;

            // We found cover art! Add to cache and get out of the loop
            _imageCache.Add(cache);

            // Check if the cache size has been reached
            if (_imageCache.Count > _imageCacheSize)
            {
                // Check if the image needs to be disposed
                if (_imageCache[0].Image != null)
                {
                    var imageTemp = _imageCache[0].Image;
                    imageTemp.Dispose();
                    imageTemp = null;
                }

                // Remove the oldest item
                _imageCache.RemoveAt(0);
            }            

            // Remove song from list
            int indexRemove = -1;
            for (int a = 0; a < _workerUpdateAlbumArtPile.Count; a++)
                if (_workerUpdateAlbumArtPile[a].AudioFile.FilePath.ToUpper() == result.AudioFile.FilePath.ToUpper())
                    indexRemove = a;
            if (indexRemove >= 0)
                _workerUpdateAlbumArtPile.RemoveAt(indexRemove);

            OnInvalidateVisual();
        }

        /// <summary>
        /// Clears the currently selected items.
        /// </summary>
        public void ClearSelectedItems()
        {
            foreach (var item in _items)
            {
                if (item.IsSelected)
                    item.IsSelected = false;
            }

            OnInvalidateVisual();
        }

        /// <summary>
        /// Imports audio files as SongGridViewItems.
        /// </summary>
        /// <param name="audioFiles">List of AudioFiles</param>
        public void ImportAudioFiles(List<AudioFile> audioFiles)
        {
            _mode = SongGridViewMode.AudioFile;
            _items = new List<SongGridViewItem>();
            foreach (var audioFile in audioFiles)
            {
                var item = new SongGridViewItem();
                item.AudioFile = audioFile;
                item.PlaylistItemId = Guid.NewGuid();
                _items.Add(item);
            }

            // Reset scrollbar position
            _vScrollBar.Value = 0;
            _songCache = null;
            OnInvalidateVisual();
        }

        /// <summary>
        /// Imports playlist items as SongGridViewItems.
        /// </summary>
        /// <param name="playlist">Playlist</param>
        public void ImportPlaylist(Playlist playlist)
        {
            _mode = SongGridViewMode.Playlist;
            _items = new List<SongGridViewItem>();
            foreach (var playlistItem in playlist.Items)
            {
                var item = new SongGridViewItem();
                item.AudioFile = playlistItem.AudioFile;
                item.PlaylistItemId = playlistItem.Id;
                _items.Add(item);
            }

            // Reset scrollbar position
            _vScrollBar.Value = 0;
            _songCache = null;
            OnInvalidateVisual();
        }

        /// <summary>
        /// Update a specific line (if visible) by its audio file unique identifier.
        /// </summary>
        /// <param name="audioFileId">Audio file unique identifier</param>
        public void UpdateAudioFileLine(Guid audioFileId)
        {
            // Find the position of the line            
            for (int a = _startLineNumber; a < _startLineNumber + _numberOfLinesToDraw; a++)
            {
                // Calculate offset
                int offsetY = (a * _songCache.LineHeight) - _vScrollBar.Value + _songCache.LineHeight;
                if (_items[a].AudioFile.Id == audioFileId)
                {
                    OnInvalidateVisualInRect(new BasicRectangle(_columns[0].Width - _hScrollBar.Value, offsetY, Frame.Width - _columns[0].Width + _hScrollBar.Value, _songCache.LineHeight));
                    //Update();
                    break;
                }
            }
        }

        ///// <summary>
        ///// Occurs when the kernel sends message to the control.
        ///// Intercepts WM_ERASEBKGND and cancels the message to prevent flickering.
        ///// </summary>
        ///// <param name="m"></param>
        //protected override void OnNotifyMessage(Message m)
        //{
        //    // Do not let WM_ERASEBKGND pass (prevent flickering)
        //    if (m.Msg != 0x14)
        //        base.OnNotifyMessage(m);
        //}

        ///// <summary>
        ///// Occurs when the control is resized.
        ///// Invalidates the cache.
        ///// </summary>
        ///// <param name="e">Event arguments</param>
        //protected override void OnResize(EventArgs e)
        //{
        //    // Bug when putting window in maximized mode: black area at the bottom
        //    // it's because the maximum value is set later in OnPaint
        //    // If the scrollY value is 
        //    if (_vScrollBar.Maximum - _vScrollBar.LargeChange < _vScrollBar.Value)
        //    {
        //        // Set new scrollbar value
        //        _vScrollBar.Value = _vScrollBar.Maximum - _vScrollBar.LargeChange + 1;
        //    }

        //    // Set horizontal scrollbar width and position
        //    _hScrollBar.Top = ClientRectangle.Height - _hScrollBar.Height;

        //    InvalidateSongCache();

        //    base.OnResize(e);
        //}

        ///// <summary>
        ///// Occurs when the user changes the horizontal scrollbar value.
        ///// </summary>
        ///// <param name="sender">Event sender</param>
        ///// <param name="e">Event arguments</param>
        //protected void hScrollBar_Scroll(object sender, ScrollEventArgs e)
        //{
        //    Refresh();
        //}

        ///// <summary>
        ///// Occurs when the user changes the vertical scrollbar value.
        ///// </summary>
        ///// <param name="sender">Event sender</param>
        ///// <param name="e">Event arguments</param>
        //protected void vScrollBar_Scroll(object sender, ScrollEventArgs e)
        //{
        //    Refresh();
        //}

        ///// <summary>
        ///// Occurs when the user clicks on the control.
        ///// Selects or unselects items, reorders columns.
        ///// </summary>
        ///// <param name="e">Event arguments</param>
        //protected override void OnMouseClick(MouseEventArgs e)
        //{
        //    if (_columns == null || _songCache == null)
        //        return;

        //    // Calculate album cover art width
        //    int albumArtCoverWidth = _columns[0].Visible ? _columns[0].Width : 0;

        //    // Make sure the control is focused
        //    if (!Focused)
        //        Focus();

        //    // Show context menu strip if the button click is right and not the album art column
        //    if (e.Button == System.Windows.Forms.MouseButtons.Right &&
        //        e.X > _columns[0].Width)
        //    {
        //        // Is there a context menu strip configured?
        //        if (contextMenuStrip != null)
        //            contextMenuStrip.Show(Control.MousePosition.X, Control.MousePosition.Y);
        //    }

        //    // Check if the user is resizing a column
        //    SongGridViewColumn columnResizing = _columns.FirstOrDefault(x => x.IsUserResizingColumn == true);

        //    // Calculate scrollbar offset Y
        //    int scrollbarOffsetY = (_startLineNumber * _songCache.LineHeight) - _vScrollBar.Value;

        //    // Check if the user has clicked on the header (for orderBy)
        //    if (e.Y >= 0 &&
        //        e.Y <= _songCache.LineHeight &&
        //        columnResizing == null &&
        //        !IsColumnMoving)
        //    {
        //        // Check on what column the user has clicked
        //        int offsetX = 0;
        //        for (int a = 0; a < _songCache.ActiveColumns.Count; a++)
        //        {
        //            SongGridViewColumn column = _songCache.ActiveColumns[a];
        //            if (column.Visible)
        //            {
        //                // Check if the mouse pointer is over this column
        //                if (e.X >= offsetX - _hScrollBar.Value && e.X <= offsetX + column.Width - _hScrollBar.Value)
        //                {
        //                    // Check mouse button
        //                    if (e.Button == System.Windows.Forms.MouseButtons.Left && CanChangeOrderBy)
        //                    {
        //                        // Check if the column order was already set
        //                        if (_orderByFieldName == column.FieldName)
        //                        {
        //                            // Reverse ascending/descending
        //                            _orderByAscending = !_orderByAscending;
        //                        }
        //                        else
        //                        {
        //                            // Set order by field name
        //                            _orderByFieldName = column.FieldName;
        //                            _orderByAscending = true;
        //                        }

        //                        // Invalidate cache and song items
        //                        _items = null;
        //                        _songCache = null;

        //                        // Raise column click event (if an event is subscribed)
        //                        if (OnColumnClick != null)
        //                        {
        //                            SongGridViewColumnClickData data = new SongGridViewColumnClickData();
        //                            data.ColumnIndex = a;
        //                            OnColumnClick(data);
        //                        }

        //                        Refresh();
        //                        return;
        //                    }
        //                    else if (e.Button == System.Windows.Forms.MouseButtons.Right)
        //                    {
        //                        // Refresh column visibility in menu before opening
        //                        foreach(ToolStripMenuItem menuItem in _menuColumns.Items)
        //                        {
        //                            SongGridViewColumn menuItemColumn = _columns.FirstOrDefault(x => x.Title == menuItem.Tag.ToString());
        //                            if (menuItemColumn != null)
        //                                menuItem.Checked = menuItemColumn.Visible;
        //                        }

        //                        // Display columns contextual menu
        //                        _menuColumns.Show(this, e.X, e.Y);
        //                    }
        //                }

        //                // Increment X offset
        //                offsetX += column.Width;
        //            }
        //        }
        //    }

        //    // Loop through visible lines to find the original selected items
        //    int startIndex = -1;
        //    int endIndex = -1;
        //    for (int a = _startLineNumber; a < _startLineNumber + _numberOfLinesToDraw; a++)
        //    {
        //        // Check if the item is selected
        //        if (_items[a].IsSelected)
        //        {
        //            // Check if the start index was set
        //            if (startIndex == -1)
        //            {
        //                // Set start index
        //                startIndex = a;
        //            }
        //            // Check if the end index is set or if it needs to be updated
        //            if (endIndex == -1 || endIndex < a)
        //            {
        //                // Set end index
        //                endIndex = a;
        //            }
        //        }
        //    }

        //    // Make sure the indexes are set
        //    if (startIndex > -1 && endIndex > -1)
        //    {
        //        // Invalidate the original selected lines
        //        int startY = ((startIndex - _startLineNumber + 1) * _songCache.LineHeight) + scrollbarOffsetY;
        //        int endY = ((endIndex - _startLineNumber + 2) * _songCache.LineHeight) + scrollbarOffsetY;
        //        Invalidate(new Rectangle(albumArtCoverWidth - _hScrollBar.Value, startY, ClientRectangle.Width - albumArtCoverWidth + _hScrollBar.Value, endY - startY));
        //    }

        //    // Reset selection (make sure SHIFT or CTRL isn't held down)
        //    if ((Control.ModifierKeys & Keys.Shift) == 0 &&
        //       (Control.ModifierKeys & Keys.Control) == 0)
        //    {
        //        SongGridViewItem mouseOverItem = _items.FirstOrDefault(x => x.IsMouseOverItem == true);
        //        if (mouseOverItem != null)
        //        {
        //            // Reset selection, unless the CTRL key is held (TODO)
        //            List<SongGridViewItem> selectedItems = _items.Where(x => x.IsSelected == true).ToList();
        //            foreach (SongGridViewItem item in selectedItems)
        //                item.IsSelected = false;
        //        }
        //    }

        //    // Loop through visible lines to update the new selected item
        //    bool invalidatedNewSelection = false;
        //    for (int a = _startLineNumber; a < _startLineNumber + _numberOfLinesToDraw; a++)
        //    {
        //        // Check if mouse is over this item
        //        if (_items[a].IsMouseOverItem)
        //        {
        //            invalidatedNewSelection = true;

        //            // Check if SHIFT is held
        //            if ((Control.ModifierKeys & Keys.Shift) != 0)
        //            {
        //                // Find the start index of the selection
        //                int startIndexSelection = _lastItemIndexClicked;
        //                if (a < startIndexSelection)
        //                    startIndexSelection = a;

        //                // Find the end index of the selection
        //                int endIndexSelection = _lastItemIndexClicked;
        //                if (a > endIndexSelection)
        //                    endIndexSelection = a + 1;

        //                // Loop through items to selected
        //                for (int b = startIndexSelection; b < endIndexSelection; b++)
        //                    _items[b].IsSelected = true;

        //                // Invalidate region
        //                Invalidate();
        //            }
        //            // Check if CTRL is held
        //            else if ((Control.ModifierKeys & Keys.Control) != 0)
        //            {
        //                // Invert selection
        //                _items[a].IsSelected = !_items[a].IsSelected;
        //                Invalidate(new Rectangle(albumArtCoverWidth - _hScrollBar.Value, ((a - _startLineNumber + 1) * _songCache.LineHeight) + scrollbarOffsetY, ClientRectangle.Width - albumArtCoverWidth + _hScrollBar.Value, _songCache.LineHeight));
        //            }
        //            else
        //            {
        //                // Set this item as the new selected item
        //                _items[a].IsSelected = true;
        //                Invalidate(new Rectangle(albumArtCoverWidth - _hScrollBar.Value, ((a - _startLineNumber + 1) * _songCache.LineHeight) + scrollbarOffsetY, ClientRectangle.Width - albumArtCoverWidth + _hScrollBar.Value, _songCache.LineHeight));
        //            }

        //            // Set the last item clicked index
        //            _lastItemIndexClicked = a;

        //            // Exit loop
        //            break;
        //        }
        //    }

        //    // Raise selected item changed event (if an event is subscribed)
        //    if (invalidatedNewSelection && OnSelectedIndexChanged != null)
        //    {
        //        SongGridViewSelectedIndexChangedData data = new SongGridViewSelectedIndexChangedData();                
        //        OnSelectedIndexChanged(data);                
        //    }

        //    Update();

        //    base.OnMouseClick(e);
        //}


        ///// <summary>
        ///// Occurs when the user clicks on one of the menu items of the Columns contextual menu.
        ///// </summary>
        ///// <param name="sender">Event sender</param>
        ///// <param name="e">Event arguments</param>
        //protected void menuItemColumns_Click(object sender, EventArgs e)
        //{
        //    // Make sure the sender is the menu item
        //    if (sender is ToolStripMenuItem)
        //    {
        //        // Get the reference to the menu item
        //        ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;

        //        // Inverse selection
        //        menuItem.Checked = !menuItem.Checked;

        //        // Get column
        //        SongGridViewColumn column = _columns.FirstOrDefault(x => x.Title == menuItem.Tag.ToString());
        //        if (column != null)
        //        {
        //            // Set visibility
        //            column.Visible = menuItem.Checked;
        //        }

        //        // Reset cache
        //        _songCache = null;
        //        Refresh();
        //    }
        //}

        /////// <summary>
        /////// Handles the global messages for the mouse wheel.
        /////// </summary>
        /////// <param name="m">Message</param>
        /////// <returns>Nothing interesting</returns>
        ////public bool PreFilterMessage(ref Message m)
        ////{
        ////    // Check message type
        ////    if (m.Msg == Win32.WM_MOUSEWHEEL)
        ////    {
        ////        // Get mouse wheel delta
        ////        int delta = Win32.GetWheelDeltaWParam((int)m.WParam);

        ////        // Check if all the data is valid
        ////        if (_columns == null || _songCache == null)
        ////        {
        ////            return false;
        ////        }

        ////        // Make sure the mouse cursor is over the control, and that the vertical scrollbar is enabled
        ////        if (!_isMouseOverControl || !_vScrollBar.Enabled)
        ////        {
        ////            return false;
        ////        }

        ////        // Get relative value
        ////        int value = delta / SystemInformation.MouseWheelScrollDelta;

        ////        // Set new value
        ////        int newValue = _vScrollBar.Value + (-value * _songCache.LineHeight);

        ////        // Check for maximum
        ////        if (newValue > _vScrollBar.Maximum - _vScrollBar.LargeChange)
        ////        {
        ////            newValue = _vScrollBar.Maximum - _vScrollBar.LargeChange;
        ////        }
        ////        // Check for minimum
        ////        if (newValue < 0)
        ////        {
        ////            newValue = 0;
        ////        }

        ////        // Set scrollbar value
        ////        _vScrollBar.Value = newValue;

        ////        // Invalidate the whole control and refresh                
        ////        Refresh();                
        ////    }

        ////    return false;
        ////}

        //#endregion


        public void Render(IGraphicsContext context)
        {

        }

        ///// <summary>
        ///// Occurs when the control needs to be painted.
        ///// </summary>
        ///// <param name="e">Paint Event arguments</param>
        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    if (_items == null)
        //        return;

        //    Graphics g = e.Graphics;
        //    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        //    g.SmoothingMode = SmoothingMode.AntiAlias;
        //    PaintSongs(ref g);         

        //    base.OnPaint(e);
        //}

        ///// <summary>
        ///// Paints a grid view displaying a list of songs.
        ///// </summary>
        ///// <param name="g">Graphics object to write to</param>
        //public void PaintSongs(ref Graphics g)
        //{
        //    Rectangle rect = new Rectangle();
        //    RectangleF rectF = new RectangleF();
        //    Pen pen = null;
        //    SolidBrush brush = null;
        //    LinearGradientBrush brushGradient = null;
        //    Color colorNowPlaying1 = _theme.RowNowPlayingTextGradient.Color1;
        //    Color colorNowPlaying2 = _theme.RowNowPlayingTextGradient.Color2;
        //    int offsetX = 0;
        //    int offsetY = 0;
        //    int albumCoverStartIndex = 0;
        //    int albumCoverEndIndex = 0;
        //    string currentAlbumTitle = string.Empty;
        //    bool nowPlayingSongFound = false;
        //    Font fontDefault = null;
        //    Font fontDefaultBold = null;

        //    // Make sure the embedded font name needs to be loaded and is valid
        //    if (_theme.RowTextGradient.Font.UseEmbeddedFont && !String.IsNullOrEmpty(_theme.RowTextGradient.Font.EmbeddedFontName))
        //    {
        //        try
        //        {
        //            // Get embedded fonts
        //            fontDefault = Tools.LoadEmbeddedFont(embeddedFonts, _theme.RowTextGradient.Font.EmbeddedFontName, _theme.RowTextGradient.Font.Size, _theme.RowTextGradient.Font.ToFontStyle());
        //            fontDefaultBold = Tools.LoadEmbeddedFont(embeddedFonts, _theme.RowTextGradient.Font.EmbeddedFontName, _theme.RowTextGradient.Font.Size, _theme.RowTextGradient.Font.ToFontStyle() | FontStyle.Bold);
        //        }
        //        catch
        //        {
        //            // Use default font instead
        //            fontDefault = this.Font;
        //            fontDefaultBold = new Font(this.Font, FontStyle.Bold);
        //        }
        //    }

        //    // Check if font is null
        //    if (fontDefault == null)
        //    {
        //        try
        //        {
        //            // Try to get standard font
        //            fontDefault = new Font(_theme.RowTextGradient.Font.StandardFontName, _theme.RowTextGradient.Font.Size, _theme.RowTextGradient.Font.ToFontStyle());
        //            fontDefaultBold = new Font(_theme.RowTextGradient.Font.StandardFontName, _theme.RowTextGradient.Font.Size, _theme.RowTextGradient.Font.ToFontStyle() | FontStyle.Bold);
        //        }
        //        catch
        //        {
        //            // Use default font instead
        //            fontDefault = this.Font;
        //            fontDefaultBold = new Font(this.Font, FontStyle.Bold);
        //        }
        //    }

        //    // Set string format
        //    StringFormat stringFormat = new StringFormat();

        //    // If there are no items, no need to draw this control.
        //    if (_items == null)
        //        return;

        //    // Check if a cache exists, or if the cache needs to be refreshed
        //    if (_songCache == null)
        //        InvalidateSongCache();

        //    // Calculate how many lines must be skipped because of the scrollbar position
        //    _startLineNumber = (int)Math.Floor((double)_vScrollBar.Value / (double)(_songCache.LineHeight));

        //    // Check if the total number of lines exceeds the number of icons fitting in height
        //    _numberOfLinesToDraw = 0;
        //    if (_startLineNumber + _songCache.NumberOfLinesFittingInControl > _items.Count)
        //    {
        //        // There aren't enough lines to fill the screen
        //        _numberOfLinesToDraw = _items.Count - _startLineNumber;
        //    }
        //    else
        //    {
        //        // Fill up screen 
        //        _numberOfLinesToDraw = _songCache.NumberOfLinesFittingInControl;
        //    }

        //    // Add one line for overflow; however, make sure we aren't adding a line without content 
        //    if (_startLineNumber + _numberOfLinesToDraw + 1 <= _items.Count)
        //        _numberOfLinesToDraw++;

        //    // Loop through lines
        //    for (int a = _startLineNumber; a < _startLineNumber + _numberOfLinesToDraw; a++)
        //    {
        //        // Get audio file
        //        AudioFile audioFile = _items[a].AudioFile;

        //        // Calculate Y offset (compensate for scrollbar position)
        //        offsetY = (a * _songCache.LineHeight) - _vScrollBar.Value + _songCache.LineHeight;

        //        // Calculate album art cover column width
        //        int albumArtColumnWidth = _columns[0].Visible ? _columns[0].Width : 0;

        //        // Calculate line background width
        //        int lineBackgroundWidth = ClientRectangle.Width + _hScrollBar.Value - albumArtColumnWidth;

        //        // Reduce width from scrollbar if visible
        //        if (_vScrollBar.Visible)
        //            lineBackgroundWidth -= _vScrollBar.Width;

        //        // Set rectangle                
        //        //Rectangle rectBackground = new Rectangle(albumArtColumnWidth - _hScrollBar.Value, offsetY, lineBackgroundWidth, _songCache.LineHeight);                
        //        Rectangle rectBackground = new Rectangle(albumArtColumnWidth - _hScrollBar.Value, offsetY, lineBackgroundWidth, _songCache.LineHeight + 1);
                
        //        // Set default line background color
        //        Color colorBackground1 = _theme.RowTextGradient.Color1;
        //        Color colorBackground2 = _theme.RowTextGradient.Color2;

        //        // Check conditions to determine background color
        //        if ((_mode == SongGridViewMode.AudioFile && audioFile.Id == _nowPlayingAudioFileId) || 
        //            (_mode == SongGridViewMode.Playlist && _items[a].PlaylistItemId == _nowPlayingPlaylistItemId))
        //        {
        //            colorBackground1 = _theme.RowNowPlayingTextGradient.Color1;
        //            colorBackground2 = _theme.RowNowPlayingTextGradient.Color2;
        //        }

        //        // Check if item is selected
        //        if (_items[a].IsSelected)
        //        {
        //            // Use darker color
        //            int diff = 40;
        //            colorBackground1 = Color.FromArgb(255,
        //                (colorBackground1.R - diff < 0) ? 0 : colorBackground1.R - diff,
        //                (colorBackground1.G - diff < 0) ? 0 : colorBackground1.G - diff,
        //                (colorBackground1.B - diff < 0) ? 0 : colorBackground1.B - diff);
        //            colorBackground2 = Color.FromArgb(255,
        //                (colorBackground2.R - diff < 0) ? 0 : colorBackground2.R - diff,
        //                (colorBackground2.G - diff < 0) ? 0 : colorBackground2.G - diff,
        //                (colorBackground2.B - diff < 0) ? 0 : colorBackground2.B - diff);
        //        }

        //        //// Check if mouse is over item
        //        //if (items[a].IsMouseOverItem)
        //        //{
        //        //    // Use lighter color
        //        //    int diff = 20;
        //        //    colorBackground1 = Color.FromArgb(255,
        //        //        (colorBackground1.R + diff > 255) ? 255 : colorBackground1.R + diff,
        //        //        (colorBackground1.G + diff > 255) ? 255 : colorBackground1.G + diff,
        //        //        (colorBackground1.B + diff > 255) ? 255 : colorBackground1.B + diff);
        //        //    colorBackground2 = Color.FromArgb(255,
        //        //        (colorBackground2.R + diff > 255) ? 255 : colorBackground2.R + diff,
        //        //        (colorBackground2.G + diff > 255) ? 255 : colorBackground2.G + diff,
        //        //        (colorBackground2.B + diff > 255) ? 255 : colorBackground2.B + diff);
        //        //}

        //        // Check conditions to determine background color
        //        if ((_mode == SongGridViewMode.AudioFile && audioFile.Id == _nowPlayingAudioFileId) ||
        //            (_mode == SongGridViewMode.Playlist && _items[a].PlaylistItemId == _nowPlayingPlaylistItemId))
        //        {
        //            colorNowPlaying1 = colorBackground1;
        //            colorNowPlaying2 = colorBackground2;
        //        }

        //        // Create gradient
        //        brushGradient = new LinearGradientBrush(rectBackground, colorBackground1, colorBackground2, 90);
        //        g.FillRectangle(brushGradient, rectBackground);
        //        brushGradient.Dispose();
        //        brushGradient = null;

        //        // Loop through columns                
        //        offsetX = 0;
        //        for (int b = 0; b < _songCache.ActiveColumns.Count; b++)
        //        {
        //            // Get current column
        //            SongGridViewColumn column = _songCache.ActiveColumns[b];

        //            // Check if the column is visible
        //            if (column.Visible)
        //            {
        //                // Check if this is the "Now playing" column
        //                if (column.Title == "Now Playing")
        //                {
        //                    // Draw now playing icon
        //                    if ((_mode == SongGridViewMode.AudioFile && audioFile.Id == _nowPlayingAudioFileId) ||
        //                        (_mode == SongGridViewMode.Playlist && _items[a].PlaylistItemId == _nowPlayingPlaylistItemId))
        //                    {
        //                        // Which size is the minimum? Width or height?                    
        //                        int availableWidthHeight = column.Width - 4;
        //                        if (_songCache.LineHeight <= column.Width)
        //                        {                                    
        //                            availableWidthHeight = _songCache.LineHeight - 4;
        //                        }
        //                        else
        //                        {
        //                            availableWidthHeight = column.Width - 4;
        //                        }

        //                        // Calculate the icon position                                
        //                        float iconNowPlayingX = ((column.Width - availableWidthHeight) / 2) + offsetX - _hScrollBar.Value;
        //                        float iconNowPlayingY = offsetY + ((_songCache.LineHeight - availableWidthHeight) / 2);

        //                        // Create NowPlaying rect (MUST be in integer)                    
        //                        _rectNowPlaying = new Rectangle((int)iconNowPlayingX, (int)iconNowPlayingY, availableWidthHeight, availableWidthHeight);
        //                        nowPlayingSongFound = true;

        //                        // Draw outer circle
        //                        brushGradient = new LinearGradientBrush(_rectNowPlaying, Color.FromArgb(50, _theme.IconNowPlayingGradient.Color1.R, _theme.IconNowPlayingGradient.Color1.G, _theme.IconNowPlayingGradient.Color1.B), _theme.IconNowPlayingGradient.Color2, _timerAnimationNowPlayingCount % 360);
        //                        g.FillEllipse(brushGradient, _rectNowPlaying);
        //                        brushGradient.Dispose();
        //                        brushGradient = null;

        //                        // Draw inner circle
        //                        rect = new Rectangle((int)iconNowPlayingX + 4, (int)iconNowPlayingY + 4, availableWidthHeight - 8, availableWidthHeight - 8);
        //                        brush = new SolidBrush(colorNowPlaying1);
        //                        g.FillEllipse(brush, rect);
        //                        brush.Dispose();
        //                        brush = null;
        //                    }
        //                }
        //                else if (column.Title == "Album Cover")
        //                {
        //                    #region Album Cover Zone

        //                    // Check for an album title change (or the last item of the grid)
        //                    if (currentAlbumTitle != audioFile.AlbumTitle)
        //                    {
        //                        // Set the new current album title
        //                        currentAlbumTitle = audioFile.AlbumTitle;

        //                        // For displaying the album cover, we need to know how many songs of the same album are bundled together
        //                        // Start by getting the start index
        //                        for (int c = a; c > 0; c--)
        //                        {
        //                            // Get audio file
        //                            AudioFile previousAudioFile = _items[c].AudioFile;

        //                            // Check if the album title matches
        //                            if (previousAudioFile.AlbumTitle != audioFile.AlbumTitle)
        //                            {
        //                                // Set album cover start index (+1 because the last song was the sound found in the previous loop iteration)
        //                                albumCoverStartIndex = c + 1;
        //                                break;
        //                            }
        //                        }
        //                        // Find the end index
        //                        for (int c = a + 1; c < _items.Count; c++)
        //                        {
        //                            // Get audio file
        //                            AudioFile nextAudioFile = _items[c].AudioFile;

        //                            // If the album title is different, this means we found the next album title
        //                            if (nextAudioFile.AlbumTitle != audioFile.AlbumTitle)
        //                            {
        //                                // Set album cover end index (-1 because the last song was the song found in the previous loop iteration)
        //                                albumCoverEndIndex = c - 1;
        //                                break;
        //                            }
        //                            // If this is the last item of the grid...
        //                            else if (c == _items.Count - 1)
        //                            {
        //                                // Set album cover end index as the last item of the grid
        //                                albumCoverEndIndex = c;
        //                                break;
        //                            }
        //                        }

        //                        // Calculate y and height
        //                        int scrollbarOffsetY = (_startLineNumber * _songCache.LineHeight) - _vScrollBar.Value;
        //                        int y = ((albumCoverStartIndex - _startLineNumber) * _songCache.LineHeight) + _songCache.LineHeight + scrollbarOffsetY;

        //                        // Calculate the height of the album cover zone (+1 on end index because the array is zero-based)
        //                        int albumCoverZoneHeight = (albumCoverEndIndex + 1 - albumCoverStartIndex) * _songCache.LineHeight;

        //                        int heightWithPadding = albumCoverZoneHeight - (_theme.Padding * 2);
        //                        if (heightWithPadding > _songCache.ActiveColumns[0].Width - (_theme.Padding * 2))
        //                        {
        //                            heightWithPadding = _songCache.ActiveColumns[0].Width - (_theme.Padding * 2);
        //                        }

        //                        // Make sure the height is at least zero (not necessary to draw anything!)
        //                        if (albumCoverZoneHeight > 0)
        //                        {
        //                            // Draw album cover background
        //                            Rectangle rectAlbumCover = new Rectangle(0 - _hScrollBar.Value, y, _songCache.ActiveColumns[0].Width, albumCoverZoneHeight);
        //                            brushGradient = new LinearGradientBrush(rectAlbumCover, _theme.AlbumCoverBackgroundGradient.Color1, _theme.AlbumCoverBackgroundGradient.Color2, _theme.AlbumCoverBackgroundGradient.GradientMode);
        //                            g.FillRectangle(brushGradient, rectAlbumCover);
        //                            brushGradient.Dispose();
        //                            brushGradient = null;

        //                            // Try to extract image from cache
        //                            Image imageAlbumCover = null;
        //                            SongGridViewImageCache cachedImage = _imageCache.FirstOrDefault(x => x.Key == audioFile.ArtistName + "_" + audioFile.AlbumTitle);
        //                            if (cachedImage != null)
        //                            {
        //                                // Set image
        //                                imageAlbumCover = cachedImage.Image;
        //                            }

        //                            // Album art not found in cache; try to find an album cover in one of the file
        //                            if (cachedImage == null)
        //                            {
        //                                // Check if the album cover is already in the pile
        //                                bool albumCoverFound = false;
        //                                foreach (SongGridViewBackgroundWorkerArgument arg in _workerUpdateAlbumArtPile)
        //                                {
        //                                    // Match by file path
        //                                    if (arg.AudioFile.FilePath.ToUpper() == audioFile.FilePath.ToUpper())
        //                                    {
        //                                        // We found the album cover
        //                                        albumCoverFound = true;
        //                                    }
        //                                }

        //                                // Add to the pile only if the album cover isn't already in it
        //                                if (!albumCoverFound)
        //                                {
        //                                    // Add item to update album art worker pile
        //                                    SongGridViewBackgroundWorkerArgument arg = new SongGridViewBackgroundWorkerArgument();
        //                                    arg.AudioFile = audioFile;
        //                                    arg.LineIndex = a;
        //                                    _workerUpdateAlbumArtPile.Add(arg);
        //                                }
        //                            }

        //                            // Measure available width for text
        //                            int widthAvailableForText = _columns[0].Width - (_theme.Padding * 2);

        //                            // Display titles depending on if an album art was found
        //                            RectangleF rectAlbumCoverArt = new RectangleF();
        //                            RectangleF rectAlbumTitleText = new RectangleF();
        //                            RectangleF rectArtistNameText = new RectangleF();
        //                            SizeF sizeAlbumTitle = new SizeF();
        //                            SizeF sizeArtistName = new SizeF();
        //                            bool useAlbumArtOverlay = false;

        //                            // If there's only one line, we need to do place the artist name and album title on one line
        //                            if (albumCoverEndIndex - albumCoverStartIndex == 0)
        //                            {
        //                                // Set string format
        //                                stringFormat.Alignment = StringAlignment.Near;
        //                                stringFormat.Trimming = StringTrimming.EllipsisCharacter;

        //                                // Measure strings
        //                                sizeArtistName = g.MeasureString(audioFile.ArtistName, fontDefaultBold, widthAvailableForText, stringFormat);
        //                                sizeAlbumTitle = g.MeasureString(currentAlbumTitle, fontDefault, widthAvailableForText - (int)sizeArtistName.Width, stringFormat);

        //                                // Display artist name at full width first, then album name
        //                                rectArtistNameText = new RectangleF(_theme.Padding - _hScrollBar.Value, y + (_theme.Padding / 2), widthAvailableForText, _songCache.LineHeight);
        //                                rectAlbumTitleText = new RectangleF(_theme.Padding - _hScrollBar.Value + sizeArtistName.Width + _theme.Padding, y + (_theme.Padding / 2), widthAvailableForText - sizeArtistName.Width, _songCache.LineHeight);
        //                            }
        //                            else
        //                            {
        //                                // There are at least two lines; is there an album cover to display?
        //                                if (imageAlbumCover == null)
        //                                {
        //                                    // There is no album cover to display; display only text.
        //                                    // Set string format
        //                                    stringFormat.Alignment = StringAlignment.Center;
        //                                    stringFormat.Trimming = StringTrimming.EllipsisWord;

        //                                    // Measure strings
        //                                    sizeArtistName = g.MeasureString(audioFile.ArtistName, fontDefaultBold, widthAvailableForText, stringFormat);
        //                                    sizeAlbumTitle = g.MeasureString(currentAlbumTitle, fontDefault, widthAvailableForText, stringFormat);

        //                                    // Display the album title at the top of the zome
        //                                    rectArtistNameText = new RectangleF(_theme.Padding - _hScrollBar.Value, y + _theme.Padding, widthAvailableForText, heightWithPadding);
        //                                    rectAlbumTitleText = new RectangleF(_theme.Padding - _hScrollBar.Value, y + _theme.Padding + sizeArtistName.Height, widthAvailableForText, heightWithPadding);
        //                                }
        //                                else
        //                                {
        //                                    // There is an album cover to display with more than 2 lines.
        //                                    // Set string format
        //                                    stringFormat.Alignment = StringAlignment.Near;
        //                                    stringFormat.Trimming = StringTrimming.EllipsisWord;

        //                                    // Measure strings
        //                                    sizeArtistName = g.MeasureString(audioFile.ArtistName, fontDefaultBold, widthAvailableForText, stringFormat);
        //                                    sizeAlbumTitle = g.MeasureString(currentAlbumTitle, fontDefault, widthAvailableForText, stringFormat);

        //                                    // If there's only two lines, display text on only two lines
        //                                    if (albumCoverEndIndex - albumCoverStartIndex == 1)
        //                                    {
        //                                        // Display artist name on first line; display album title on second line
        //                                        rectArtistNameText = new RectangleF(((_columns[0].Width - sizeArtistName.Width) / 2) - _hScrollBar.Value, y, widthAvailableForText, heightWithPadding);
        //                                        rectAlbumTitleText = new RectangleF(((_columns[0].Width - sizeAlbumTitle.Width) / 2) - _hScrollBar.Value, y + _songCache.LineHeight, widthAvailableForText, heightWithPadding);
        //                                    }
        //                                    // There is an album cover to display; between 2 and 6 lines AND the width of the column is at least 50 pixels (or
        //                                    // it will try to display text in a too thin area)
        //                                    else if (albumCoverEndIndex - albumCoverStartIndex <= 5 && _columns[0].Width > 175)
        //                                    {
        //                                        // There is no album cover to display; display only text.
        //                                        // Set string format
        //                                        stringFormat.Alignment = StringAlignment.Near;
        //                                        stringFormat.Trimming = StringTrimming.EllipsisWord;

        //                                        float widthRemainingForText = _columns[0].Width - _theme.Padding - heightWithPadding;

        //                                        // Measure strings
        //                                        sizeArtistName = g.MeasureString(audioFile.ArtistName, fontDefaultBold, new SizeF(widthRemainingForText, heightWithPadding), stringFormat);
        //                                        sizeAlbumTitle = g.MeasureString(currentAlbumTitle, fontDefault, new SizeF(widthRemainingForText, heightWithPadding), stringFormat);

        //                                        // Try to center the cover art + padding + max text width
        //                                        float maxWidth = sizeArtistName.Width;
        //                                        if (sizeAlbumTitle.Width > maxWidth)
        //                                        {
        //                                            // Set new maximal width
        //                                            maxWidth = sizeAlbumTitle.Width;
        //                                        }

        //                                        useAlbumArtOverlay = true;

        //                                        float albumCoverX = (_columns[0].Width - heightWithPadding - _theme.Padding - _theme.Padding - maxWidth) / 2;
        //                                        float artistNameY = (albumCoverZoneHeight - sizeArtistName.Height - sizeAlbumTitle.Height) / 2;

        //                                        // Display the album title at the top of the zome
        //                                        rectArtistNameText = new RectangleF(albumCoverX + heightWithPadding + _theme.Padding - _hScrollBar.Value, y + artistNameY, widthRemainingForText, heightWithPadding);
        //                                        rectAlbumTitleText = new RectangleF(albumCoverX + heightWithPadding + _theme.Padding - _hScrollBar.Value, y + artistNameY + sizeArtistName.Height, widthRemainingForText, heightWithPadding);

        //                                        // Set cover art rectangle
        //                                        rectAlbumCoverArt = new RectangleF(albumCoverX - _hScrollBar.Value, y + _theme.Padding, heightWithPadding, heightWithPadding);
        //                                    }
        //                                    // 7 and more lines
        //                                    else
        //                                    {
        //                                        // Display artist name at the top of the album cover; display album title at the bottom of the album cover
        //                                        rectArtistNameText = new RectangleF(((_columns[0].Width - sizeArtistName.Width) / 2) - _hScrollBar.Value, y + (_theme.Padding * 2), widthAvailableForText, heightWithPadding);
        //                                        rectAlbumTitleText = new RectangleF(((_columns[0].Width - sizeAlbumTitle.Width) / 2) - _hScrollBar.Value, y + heightWithPadding - sizeAlbumTitle.Height, widthAvailableForText, heightWithPadding);

        //                                        // Draw background overlay behind text
        //                                        useAlbumArtOverlay = true;

        //                                        // Try to horizontally center the album cover if it's not taking the whole width (less padding)
        //                                        float albumCoverX = _theme.Padding;
        //                                        if (_columns[0].Width > heightWithPadding)
        //                                        {
        //                                            // Get position
        //                                            albumCoverX = ((float)(_columns[0].Width - heightWithPadding) / 2) - _hScrollBar.Value;
        //                                        }

        //                                        // Set cover art rectangle
        //                                        rectAlbumCoverArt = new RectangleF(albumCoverX, y + _theme.Padding, heightWithPadding, heightWithPadding);
        //                                    }

        //                                }
        //                            }

        //                            // Display album cover
        //                            if (imageAlbumCover != null)
        //                                g.DrawImage(imageAlbumCover, rectAlbumCoverArt);

        //                            if (useAlbumArtOverlay)
        //                            {
        //                                //// Draw artist name and album title background
        //                                //RectangleF rectArtistNameBackground = new RectangleF(rectArtistNameText.X - (theme.Padding / 2), rectArtistNameText.Y - (theme.Padding / 2), sizeArtistName.Width + theme.Padding, sizeArtistName.Height + theme.Padding);
        //                                //RectangleF rectAlbumTitleBackground = new RectangleF(rectAlbumTitleText.X - (theme.Padding / 2), rectAlbumTitleText.Y - (theme.Padding / 2), sizeAlbumTitle.Width + theme.Padding, sizeAlbumTitle.Height + theme.Padding);
        //                                //brush = new SolidBrush(Color.FromArgb(190, 0, 0, 0));
        //                                //g.FillRectangle(brush, rectArtistNameBackground);
        //                                //g.FillRectangle(brush, rectAlbumTitleBackground);
        //                                //brush.Dispose();
        //                                //brush = null;
        //                            }

        //                            // Check if this is the artist name column (set font to bold)
        //                            //g.DrawString(audioFile.ArtistName, fontDefaultBold, Brushes.White, rectArtistNameText, stringFormat);
        //                            //g.DrawString(currentAlbumTitle, fontDefault, Brushes.White, rectAlbumTitleText, stringFormat);

        //                            // Draw horizontal line to distinguish albums
        //                            // Part 1: Draw line over grid
        //                            //pen = new Pen(theme.AlbumCoverBackgroundGradient.Color1);
        //                            pen = new Pen(Color.FromArgb(180, 180, 180));
        //                            g.DrawLine(pen, new Point(_columns[0].Width, y), new Point(ClientRectangle.Width, y));
        //                            pen.Dispose();
        //                            pen = null;

        //                            // Part 2: Draw line over album art zone, in a lighter color
        //                            pen = new Pen(Color.FromArgb(115, 115, 115));
        //                            g.DrawLine(pen, new Point(0, y), new Point(_columns[0].Width, y));
        //                            pen.Dispose();
        //                            pen = null;
        //                        }
        //                    }

        //                    #endregion
        //                }
        //                else
        //                {
        //                    // Get property through reflection
        //                    PropertyInfo propertyInfo = audioFile.GetType().GetProperty(column.FieldName);
        //                    if (propertyInfo != null)
        //                    {
        //                        // Get property value
        //                        string value = string.Empty;
        //                        try
        //                        {
        //                            // Determine the type of value
        //                            if (propertyInfo.PropertyType.FullName == "System.String")
        //                            {
        //                                // Try to get the value
        //                                value = propertyInfo.GetValue(audioFile, null).ToString();
        //                            }
        //                            // Nullable Int64
        //                            else if (propertyInfo.PropertyType.FullName.Contains("Int64") &&
        //                                    propertyInfo.PropertyType.FullName.Contains("Nullable"))
        //                            {
        //                                // Try to get the value
        //                                long? longValue = (long?)propertyInfo.GetValue(audioFile, null);

        //                                // Check if null
        //                                if (longValue.HasValue)
        //                                {
        //                                    // Render to string
        //                                    value = longValue.Value.ToString();
        //                                }
        //                            }
        //                            // Nullable DateTime
        //                            else if (propertyInfo.PropertyType.FullName.Contains("DateTime") &&
        //                                    propertyInfo.PropertyType.FullName.Contains("Nullable"))
        //                            {
        //                                // Try to get the value
        //                                DateTime? dateTimeValue = (DateTime?)propertyInfo.GetValue(audioFile, null);

        //                                // Check if null
        //                                if (dateTimeValue.HasValue)
        //                                {
        //                                    // Render to string
        //                                    value = dateTimeValue.Value.ToShortDateString() + " " + dateTimeValue.Value.ToShortTimeString();
        //                                }
        //                            }
        //                            else if (propertyInfo.PropertyType.FullName.Contains("System.UInt32"))
        //                            {
        //                                // Try to get the value
        //                                uint uintValue = (uint)propertyInfo.GetValue(audioFile, null);

        //                                // Render to string
        //                                value = uintValue.ToString();
        //                            }
        //                            else if (propertyInfo.PropertyType.FullName.Contains("System.Int32"))
        //                            {
        //                                // Try to get the value
        //                                int intValue = (int)propertyInfo.GetValue(audioFile, null);

        //                                // Render to string
        //                                value = intValue.ToString();
        //                            }
        //                            else if (propertyInfo.PropertyType.FullName.Contains("MPfm.Sound.AudioFileFormat"))
        //                            {
        //                                // Try to get the value
        //                                AudioFileFormat theValue = (AudioFileFormat)propertyInfo.GetValue(audioFile, null);

        //                                // Render to string
        //                                value = theValue.ToString();
        //                            }
        //                            else
        //                            {
        //                                // If the type of unknown, leave the value empty
        //                            }
        //                        }
        //                        catch
        //                        {
        //                            // Do nothing
        //                        }

        //                        // The last column always take the remaining width
        //                        int columnWidth = column.Width;
        //                        if (b == _songCache.ActiveColumns.Count - 1)
        //                        {
        //                            // Calculate the remaining width
        //                            int columnsWidth = 0;
        //                            for (int c = 0; c < _songCache.ActiveColumns.Count - 1; c++)
        //                            {
        //                                columnsWidth += _songCache.ActiveColumns[c].Width;
        //                            }
        //                            columnWidth = ClientRectangle.Width - columnsWidth + _hScrollBar.Value;
        //                        }

        //                        // Display text
        //                        rect = new Rectangle(offsetX - _hScrollBar.Value, offsetY + (_theme.Padding / 2), _songCache.ActiveColumns[b].Width, _songCache.LineHeight - _theme.Padding + 2);
        //                        stringFormat.Trimming = StringTrimming.EllipsisCharacter;
        //                        stringFormat.Alignment = StringAlignment.Near;

        //                        // Check if this is the artist name column
        //                        brush = new SolidBrush(_theme.RowTextGradient.Font.Color);
        //                        if (column.FieldName == "ArtistName" || column.FieldName == "DiscTrackNumber")
        //                        {
        //                            // Use bold for artist name
        //                            g.DrawString(value, fontDefaultBold, brush, rect, stringFormat);
        //                        }
        //                        else
        //                        {
        //                            // Use default font for the other columns
        //                            g.DrawString(value, fontDefault, brush, rect, stringFormat);
        //                        }
        //                        brush.Dispose();
        //                        brush = null;
        //                    }
        //                }

        //                // Increment offset by the column width
        //                offsetX += column.Width;
        //            }
        //        }
        //    }

        //    // If no songs are playing, set the current now playing rectangle as "empty"
        //    if (!nowPlayingSongFound)
        //        _rectNowPlaying = new Rectangle(0, 0, 1, 1);

        //    // Draw header (for some reason, the Y must be set -1 to cover an area which isn't supposed to be displayed)
        //    Rectangle rectBackgroundHeader = new Rectangle(0, -1, ClientRectangle.Width, _songCache.LineHeight + 1);
        //    brushGradient = new LinearGradientBrush(rectBackgroundHeader, _theme.HeaderTextGradient.Color1, _theme.HeaderTextGradient.Color2, 90);
        //    g.FillRectangle(brushGradient, rectBackgroundHeader);
        //    brushGradient.Dispose();
        //    brushGradient = null;

        //    // Loop through columns
        //    offsetX = 0;
        //    for (int b = 0; b < _songCache.ActiveColumns.Count; b++)
        //    {
        //        SongGridViewColumn column = _songCache.ActiveColumns[b];
        //        if (column.Visible)
        //        {
        //            // The last column always take the remaining width
        //            int columnWidth = column.Width;
        //            if (b == _songCache.ActiveColumns.Count - 1)
        //            {
        //                // Calculate the remaining width
        //                int columnsWidth = 0;
        //                for (int c = 0; c < _songCache.ActiveColumns.Count - 1; c++)
        //                    columnsWidth += _songCache.ActiveColumns[c].Width;
        //                columnWidth = ClientRectangle.Width - columnsWidth + _hScrollBar.Value;
        //            }

        //            // Check if mouse is over this column header
        //            if (column.IsMouseOverColumnHeader)
        //            {
        //                // Draw header (for some reason, the Y must be set -1 to cover an area which isn't supposed to be displayed)                        
        //                rect = new Rectangle(offsetX - _hScrollBar.Value, -1, column.Width, _songCache.LineHeight + 1);
        //                brushGradient = new LinearGradientBrush(rect, _theme.HeaderHoverTextGradient.Color1, _theme.HeaderHoverTextGradient.Color2, 90);
        //                g.FillRectangle(brushGradient, rect);
        //                brushGradient.Dispose();
        //                brushGradient = null;
        //            }
        //            else if (column.IsUserMovingColumn)
        //            {
        //                // Draw header (for some reason, the Y must be set -1 to cover an area which isn't supposed to be displayed)                        
        //                rect = new Rectangle(offsetX - _hScrollBar.Value, -1, column.Width, _songCache.LineHeight + 1);
        //                brushGradient = new LinearGradientBrush(rect, Color.Blue, Color.Green, 90);
        //                g.FillRectangle(brushGradient, rect);
        //                brushGradient.Dispose();
        //                brushGradient = null;
        //            }

        //            // Check if the header title must be displayed
        //            if (_songCache.ActiveColumns[b].IsHeaderTitleVisible)
        //            {
        //                // Display title                
        //                Rectangle rectTitle = new Rectangle(offsetX - _hScrollBar.Value, _theme.Padding / 2, column.Width, _songCache.LineHeight - _theme.Padding + 2);
        //                stringFormat.Trimming = StringTrimming.EllipsisCharacter;
        //                brush = new SolidBrush(_theme.HeaderTextGradient.Font.Color);
        //                g.DrawString(column.Title, fontDefaultBold, brush, rectTitle, stringFormat);
        //                brush.Dispose();
        //                brush = null;
        //            }

        //            // Draw column separator line; determine the height of the line
        //            int columnHeight = ClientRectangle.Height;

        //            // Determine the height of the line; if the items don't fit the control height...
        //            if (_items.Count < _songCache.NumberOfLinesFittingInControl)
        //            {
        //                // Set height as the number of items (plus header)
        //                columnHeight = (_items.Count + 1) * _songCache.LineHeight;
        //            }

        //            // Draw column line
        //            //g.DrawLine(Pens.DarkGray, new Point(offsetX + column.Width - _hScrollBar.Value, 0), new Point(offsetX + column.Width - _hScrollBar.Value, columnHeight));

        //            // Check if the column is ordered by
        //            if (column.FieldName == _orderByFieldName && !String.IsNullOrEmpty(column.FieldName))
        //            {
        //                // Create triangle points,,,
        //                PointF[] ptTriangle = new PointF[3];

        //                // ... depending on the order by ascending value
        //                int triangleWidthHeight = 8;
        //                int trianglePadding = (_songCache.LineHeight - triangleWidthHeight) / 2;
        //                if (_orderByAscending)
        //                {
        //                    // Create points for ascending
        //                    ptTriangle[0] = new PointF(offsetX + column.Width - triangleWidthHeight - (triangleWidthHeight / 2) - _hScrollBar.Value, trianglePadding);
        //                    ptTriangle[1] = new PointF(offsetX + column.Width - triangleWidthHeight - _hScrollBar.Value, _songCache.LineHeight - trianglePadding);
        //                    ptTriangle[2] = new PointF(offsetX + column.Width - triangleWidthHeight - triangleWidthHeight - _hScrollBar.Value, _songCache.LineHeight - trianglePadding);
        //                }
        //                else
        //                {
        //                    // Create points for descending
        //                    ptTriangle[0] = new PointF(offsetX + column.Width - triangleWidthHeight - (triangleWidthHeight / 2) - _hScrollBar.Value, _songCache.LineHeight - trianglePadding);
        //                    ptTriangle[1] = new PointF(offsetX + column.Width - triangleWidthHeight - _hScrollBar.Value, trianglePadding);
        //                    ptTriangle[2] = new PointF(offsetX + column.Width - triangleWidthHeight - triangleWidthHeight - _hScrollBar.Value, trianglePadding);
        //                }

        //                // Draw triangle
        //                pen = new Pen(_theme.HeaderTextGradient.Font.Color);
        //                g.DrawPolygon(pen, ptTriangle);
        //                pen.Dispose();
        //                pen = null;
        //            }

        //            // Increment offset by the column width
        //            offsetX += column.Width;
        //        }
        //    }

        //    // Display column move marker
        //    if (IsColumnMoving)
        //    {
        //        // Draw marker
        //        pen = new Pen(Color.Red);
        //        g.DrawRectangle(pen, new Rectangle(_columnMoveMarkerX - _hScrollBar.Value, 0, 1, ClientRectangle.Height));
        //        pen.Dispose();
        //        pen = null;
        //    }

        //    // Display debug information if enabled
        //    if (_displayDebugInformation)
        //    {
        //        // Build debug string
        //        StringBuilder sbDebug = new StringBuilder();
        //        sbDebug.AppendLine("Line Count: " + _items.Count.ToString());
        //        sbDebug.AppendLine("Line Height: " + _songCache.LineHeight.ToString());
        //        sbDebug.AppendLine("Lines Fit In Height: " + _songCache.NumberOfLinesFittingInControl.ToString());
        //        sbDebug.AppendLine("Total Width: " + _songCache.TotalWidth);
        //        sbDebug.AppendLine("Total Height: " + _songCache.TotalHeight);
        //        sbDebug.AppendLine("Scrollbar Offset Y: " + _songCache.ScrollBarOffsetY);
        //        sbDebug.AppendLine("HScrollbar Maximum: " + _hScrollBar.Maximum.ToString());
        //        sbDebug.AppendLine("HScrollbar LargeChange: " + _hScrollBar.LargeChange.ToString());
        //        sbDebug.AppendLine("HScrollbar Value: " + _hScrollBar.Value.ToString());
        //        sbDebug.AppendLine("VScrollbar Maximum: " + _vScrollBar.Maximum.ToString());
        //        sbDebug.AppendLine("VScrollbar LargeChange: " + _vScrollBar.LargeChange.ToString());
        //        sbDebug.AppendLine("VScrollbar Value: " + _vScrollBar.Value.ToString());

        //        // Measure string
        //        stringFormat.Trimming = StringTrimming.Word;
        //        stringFormat.LineAlignment = StringAlignment.Near;
        //        SizeF sizeDebugText = g.MeasureString(sbDebug.ToString(), fontDefault, _columns[0].Width - 1, stringFormat);
        //        rectF = new RectangleF(0, 0, _columns[0].Width - 1, sizeDebugText.Height);

        //        // Draw background
        //        brush = new SolidBrush(Color.FromArgb(200, 0, 0, 0));
        //        g.FillRectangle(brush, rectF);
        //        brush.Dispose();
        //        brush = null;

        //        // Draw string
        //        g.DrawString(sbDebug.ToString(), fontDefault, Brushes.White, rectF, stringFormat);
        //    }

        //    // If both scrollbars are visible...
        //    if (_hScrollBar.Visible && _vScrollBar.Visible)
        //    {
        //        // Draw a bit of control color over the 16x16 area in the lower right corner
        //        brush = new SolidBrush(SystemColors.Control);
        //        g.FillRectangle(brush, new Rectangle(ClientRectangle.Width - 16, ClientRectangle.Height - 16, 16, 16));
        //        brush.Dispose();
        //        brush = null;
        //    }
        //}        

        /// <summary>
        /// Occurs when the mouse cursor enters on the control.        
        /// </summary>
        public void MouseEnter()
        {
            _isMouseOverControl = true;
        }

        /// <summary>
        /// Occurs when the mouse cursor leaves the control.
        /// </summary>
        public void MouseLeave()
        {
            bool controlNeedsToBeUpdated = false;
            _isMouseOverControl = false;

            if (_columns == null || _songCache == null)
                return;

            // Calculate scrollbar offset Y
            int scrollbarOffsetY = (_startLineNumber * _songCache.LineHeight) - _vScrollBar.Value;

            // Check if there's at least one item
            if (_items.Count > 0)
            {
                // Reset mouse over item flags
                for (int b = _startLineNumber; b < _startLineNumber + _numberOfLinesToDraw; b++)
                {
                    // Check if the mouse was over this item
                    if (_items[b].IsMouseOverItem)
                    {
                        // Reset flag and invalidate region
                        _items[b].IsMouseOverItem = false;
                        OnInvalidateVisualInRect(new BasicRectangle(_columns[0].Width - _hScrollBar.Value, ((b - _startLineNumber + 1) * _songCache.LineHeight) + scrollbarOffsetY, Frame.Width - _columns[0].Width + _hScrollBar.Value, _songCache.LineHeight));
                        controlNeedsToBeUpdated = true;

                        // Exit loop
                        break;
                    }
                }
            }

            // Reset column flags
            int columnOffsetX2 = 0;
            for (int b = 0; b < _songCache.ActiveColumns.Count; b++)
            {
                // Make sure column is visible
                if (_songCache.ActiveColumns[b].Visible)
                {
                    // Was mouse over this column header?
                    if (_songCache.ActiveColumns[b].IsMouseOverColumnHeader)
                    {
                        // Reset flag
                        _songCache.ActiveColumns[b].IsMouseOverColumnHeader = false;

                        // Invalidate region
                        OnInvalidateVisualInRect(new BasicRectangle(columnOffsetX2 - _hScrollBar.Value, 0, _songCache.ActiveColumns[b].Width, _songCache.LineHeight));
                        controlNeedsToBeUpdated = true;
                    }

                    // Increment offset
                    columnOffsetX2 += _songCache.ActiveColumns[b].Width;
                }
            }

            //// Check if control needs to be updated
            //if (controlNeedsToBeUpdated)
            //    Update();
        }

        /// <summary>
        /// Occurs when the user is pressing down a mouse button.
        /// </summary>        
        public void MouseDown(float x, float y, MouseButtonType button)
        {
            _dragStartX = (int) x;

            // Check if all the data is valid
            if (_columns == null || _songCache == null)
                return;

            // Loop through columns
            foreach (SongGridViewColumn column in _songCache.ActiveColumns)
            {
                // Check for resizing column
                if (column.IsMouseCursorOverColumnLimit && column.CanBeResized && CanResizeColumns)
                {
                    // Set resizing column flag
                    column.IsUserResizingColumn = true;

                    // Save the original column width
                    _dragOriginalColumnWidth = column.Width;
                }
            }

            // Check if the left mouse button is held
            if(button == MouseButtonType.Left)
                _isUserHoldingLeftMouseButton = true;
        }

        /// <summary>
        /// Occurs when the user releases a mouse button.        
        /// </summary>
        public void MouseUp(float x, float y2, MouseButtonType button)
        {
            // Reset flags
            _dragStartX = -1;
            bool updateControl = false;
            _isUserHoldingLeftMouseButton = false;

            if (_columns == null || _songCache == null)
                return;

            // Loop through columns
            SongGridViewColumn columnMoving = null;
            foreach (SongGridViewColumn column in _songCache.ActiveColumns)
            {
                // Reset flags
                column.IsUserResizingColumn = false;

                // Check if this column is moving
                if (column.IsUserMovingColumn)
                    columnMoving = column;
            }

            // Check if the user is moving a column
            if (columnMoving != null)
            {
                // Set flag
                columnMoving.IsUserMovingColumn = false;
                updateControl = true;

                // Find out on what column the mouse cursor is
                SongGridViewColumn columnOver = null;
                int currentX = 0;
                bool isPastCurrentlyMovingColumn = false;
                for (int a = 0; a < _songCache.ActiveColumns.Count; a++)
                {
                    var currentColumn = _songCache.ActiveColumns[a];
                    if (currentColumn.FieldName == columnMoving.FieldName)
                        isPastCurrentlyMovingColumn = true;

                    if (currentColumn.Visible)
                    {
                        // Check if the cursor is over the left part of the column
                        if (x >= currentX - _hScrollBar.Value && x <= currentX + (currentColumn.Width / 2) - _hScrollBar.Value)
                        {
                            // Check flag
                            if (isPastCurrentlyMovingColumn && currentColumn.FieldName != columnMoving.FieldName)
                                columnOver = _songCache.ActiveColumns[a - 1];
                            else
                                columnOver = _songCache.ActiveColumns[a];
                            break;
                        }
                        // Check if the cursor is over the right part of the column
                        else if (x >= currentX + (currentColumn.Width / 2) - _hScrollBar.Value && x <= currentX + currentColumn.Width - _hScrollBar.Value)
                        {
                            // Check if there is a next item
                            if (a < _songCache.ActiveColumns.Count - 1)
                            {
                                // Check flag
                                if (isPastCurrentlyMovingColumn)
                                    columnOver = _songCache.ActiveColumns[a];
                                else
                                    columnOver = _songCache.ActiveColumns[a + 1];
                            }
                            break;
                        }

                        // Increment x
                        currentX += currentColumn.Width;
                    }
                }

                //// Check if the column was found (the cursor might be past the last column
                //if (columnOver == null)
                //{
                //    return;
                //}

                // Order columns by their current order
                var columnsOrdered = _columns.OrderBy(q => q.Order).ToList();

                // Move column
                int indexRemove = -1;
                int indexAdd = -1;
                for (int a = 0; a < columnsOrdered.Count; a++)
                {
                    // Find the moving column index
                    if (columnsOrdered[a].FieldName == columnMoving.FieldName)
                        indexRemove = a;

                    // Find the column index with the mouse over
                    if (columnOver != null && columnsOrdered[a].FieldName == columnOver.FieldName)
                        indexAdd = a;
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
                    columnsOrdered[a].Order = a;
            }

            // Check if the control needs to be updated
            if (updateControl)
            {
                InvalidateSongCache();
                OnInvalidateVisual();
            }
        }

        /// <summary>
        /// Occurs when the user double-clicks on the control.
        /// Starts the playback of a new song.
        /// </summary>
        /// <param name="e">Event arguments</param>
        public void MouseDoubleClick(float x, float y, MouseButtonType button)
        {
            if (_columns == null || _songCache == null)
                return;

            // Calculate album cover art width
            int albumArtCoverWidth = _columns[0].Visible ? _columns[0].Width : 0;

            // Calculate scrollbar offset Y
            int scrollbarOffsetY = (_startLineNumber * _songCache.LineHeight) - _vScrollBar.Value;

            // Keep original songId in case the now playing value is set before invalidating the older value
            Guid originalId = Guid.Empty;

            // Set original id
            if (_mode == SongGridViewMode.AudioFile)
                originalId = _nowPlayingAudioFileId;
            else if (_mode == SongGridViewMode.Playlist)
                originalId = _nowPlayingPlaylistItemId;

            // Loop through visible lines
            for (int a = _startLineNumber; a < _startLineNumber + _numberOfLinesToDraw; a++)
            {
                if (_items[a].IsMouseOverItem)
                {
                    // Set this item as the new now playing
                    _nowPlayingAudioFileId = _items[a].AudioFile.Id;
                    _nowPlayingPlaylistItemId = _items[a].PlaylistItemId;

                    OnInvalidateVisualInRect(new BasicRectangle(albumArtCoverWidth - _hScrollBar.Value, ((a - _startLineNumber + 1) * _songCache.LineHeight) + scrollbarOffsetY, Frame.Width - albumArtCoverWidth + _hScrollBar.Value, _songCache.LineHeight));
                }
                else if (_mode == SongGridViewMode.AudioFile && _items[a].AudioFile.Id == originalId)
                {
                    OnInvalidateVisualInRect(new BasicRectangle(albumArtCoverWidth - _hScrollBar.Value, ((a - _startLineNumber + 1) * _songCache.LineHeight) + scrollbarOffsetY, Frame.Width - albumArtCoverWidth + _hScrollBar.Value, _songCache.LineHeight));
                }
                else if (_mode == SongGridViewMode.Playlist && _items[a].PlaylistItemId == originalId)
                {
                    OnInvalidateVisualInRect(new BasicRectangle(albumArtCoverWidth - _hScrollBar.Value, ((a - _startLineNumber + 1) * _songCache.LineHeight) + scrollbarOffsetY, Frame.Width - albumArtCoverWidth + _hScrollBar.Value, _songCache.LineHeight));
                }
            }

            // Update invalid regions
            //Update();
        }        

        /// <summary>
        /// Occurs when the mouse pointer is moving over the control.
        /// Manages the display of mouse on/off visual effects.
        /// </summary>
        public void MouseMove(float x, float y, MouseButtonType button)
        {
            bool controlNeedsToBeUpdated = false;

            if (_columns == null || _songCache == null)
                return;

            // Calculate album cover art width
            int albumArtCoverWidth = _columns[0].Visible ? _columns[0].Width : 0;

            // Check if the user is currently resizing a column (loop through columns)
            foreach (var column in _songCache.ActiveColumns)
            {
                // Check if the user is currently resizing this column
                if (column.IsUserResizingColumn && column.Visible)
                {
                    // Calculate the new column width
                    int newWidth = _dragOriginalColumnWidth - (_dragStartX - (int)x);

                    // Make sure the width isn't lower than the minimum width
                    if (newWidth < MinimumColumnWidth)
                        newWidth = MinimumColumnWidth;

                    // Set column width
                    column.Width = newWidth;

                    // Refresh control (invalidate whole control)
                    controlNeedsToBeUpdated = true;
                    OnInvalidateVisual();
                    InvalidateSongCache();

                    // Auto adjust horizontal scrollbar value if it exceeds the value range (i.e. do not show empty column)
                    if (_hScrollBar.Value > _hScrollBar.Maximum - _hScrollBar.LargeChange)
                    {
                        // Set new value
                        int tempValue = _hScrollBar.Maximum - _hScrollBar.LargeChange;
                        if (tempValue < 0)
                            tempValue = 0;
                        _hScrollBar.Value = tempValue;
                    }
                }

                // Check if the user is moving the column
                if (column.IsMouseOverColumnHeader && column.CanBeMoved && CanMoveColumns && _isUserHoldingLeftMouseButton && !IsColumnResizing)
                {
                    // Check if the X position has changed by at least 2 pixels (i.e. dragging)
                    if (_dragStartX >= x + 2 ||
                        _dragStartX <= x - 2)
                    {
                        // Set resizing column flag
                        column.IsUserMovingColumn = true;
                    }
                }

                // Check if the user is currently moving this column 
                if (column.IsUserMovingColumn)
                {
                    // Loop through columns
                    int currentX = 0;
                    foreach (SongGridViewColumn columnOver in _songCache.ActiveColumns)
                    {
                        // Check if column is visible
                        if (columnOver.Visible)
                        {
                            // Check if the cursor is over the left part of the column
                            if (x >= currentX - _hScrollBar.Value && x <= currentX + (columnOver.Width / 2) - _hScrollBar.Value)
                                _columnMoveMarkerX = (int)x;
                            // Check if the cursor is over the right part of the column
                            else if (x >= currentX + (columnOver.Width / 2) - _hScrollBar.Value && x <= currentX + columnOver.Width - _hScrollBar.Value)
                                _columnMoveMarkerX = (int)x + columnOver.Width;

                            x += columnOver.Width;
                        }
                    }

                    OnInvalidateVisual();
                    controlNeedsToBeUpdated = true;
                }
            }

            if (!IsColumnMoving)
            {
                // Check if the cursor needs to be changed            
                int offsetX = 0;
                bool mousePointerIsOverColumnLimit = false;
                foreach (var column in _songCache.ActiveColumns)
                {
                    if (column.Visible)
                    {
                        // Increment offset by the column width
                        offsetX += column.Width;
                        if (column.CanBeResized)
                        {
                            // Check if the mouse pointer is over a column (add 1 pixel so it's easier to select)
                            if (x >= offsetX - _hScrollBar.Value && x <= offsetX + 1 - _hScrollBar.Value)
                            {
                                mousePointerIsOverColumnLimit = true;
                                column.IsMouseCursorOverColumnLimit = true;

                                //// Change the cursor if it's not the right cursor
                                //if (Cursor != Cursors.VSplit)
                                //    Cursor = Cursors.VSplit;
                            }
                            else
                            {
                                column.IsMouseCursorOverColumnLimit = false;
                            }
                        }
                    }
                }

                //// Check if the default cursor needs to be restored
                //if (!mousePointerIsOverColumnLimit && Cursor != Cursors.Default)
                //    Cursor = Cursors.Default;

                int columnOffsetX2 = 0;
                for (int b = 0; b < _songCache.ActiveColumns.Count; b++)
                {
                    var column = _songCache.ActiveColumns[b];
                    if (column.Visible)
                    {
                        // Was mouse over this column header?
                        if (column.IsMouseOverColumnHeader)
                        {
                            // Invalidate region
                            column.IsMouseOverColumnHeader = false;
                            OnInvalidateVisualInRect(new BasicRectangle(columnOffsetX2 - _hScrollBar.Value, 0, column.Width, _songCache.LineHeight));
                            controlNeedsToBeUpdated = true;
                        }

                        // Increment offset
                        columnOffsetX2 += column.Width;
                    }
                }

                // Check if the mouse pointer is over the header
                if (y >= 0 &&
                    y <= _songCache.LineHeight)
                {
                    // Check on what column the user has clicked
                    int columnOffsetX = 0;
                    for (int a = 0; a < _songCache.ActiveColumns.Count; a++)
                    {
                        var column = _songCache.ActiveColumns[a];
                        if (column.Visible)
                        {
                            // Check if the mouse pointer is over this column
                            if (x >= columnOffsetX - _hScrollBar.Value && x <= columnOffsetX + column.Width - _hScrollBar.Value)
                            {
                                // Invalidate region
                                column.IsMouseOverColumnHeader = true;
                                OnInvalidateVisualInRect(new BasicRectangle(columnOffsetX - _hScrollBar.Value, 0, column.Width, _songCache.LineHeight));

                                // Exit loop
                                controlNeedsToBeUpdated = true;
                                break;
                            }

                            columnOffsetX += column.Width;
                        }
                    }
                }

                // Check if the mouse cursor is over a line (loop through lines)                        
                int offsetY = 0;
                int scrollbarOffsetY = (_startLineNumber * _songCache.LineHeight) - _vScrollBar.Value;

                // Check if there's at least one item
                if (_items.Count > 0)
                {
                    // Reset mouse over item flags
                    for (int b = _startLineNumber; b < _startLineNumber + _numberOfLinesToDraw; b++)
                    {
                        // Check if the mouse was over this item
                        if (_items[b].IsMouseOverItem)
                        {
                            // Reset flag and invalidate region
                            _items[b].IsMouseOverItem = false;
                            OnInvalidateVisualInRect(new BasicRectangle(albumArtCoverWidth - _hScrollBar.Value, ((b - _startLineNumber + 1) * _songCache.LineHeight) + scrollbarOffsetY, Frame.Width - albumArtCoverWidth + _hScrollBar.Value, _songCache.LineHeight));

                            // Exit loop
                            break;
                        }
                    }

                    // Put new mouse over flag
                    for (int a = _startLineNumber; a < _startLineNumber + _numberOfLinesToDraw; a++)
                    {
                        var audioFile = _items[a].AudioFile;

                        // Calculate offset
                        offsetY = (a * _songCache.LineHeight) - _vScrollBar.Value + _songCache.LineHeight;

                        // Check if the mouse cursor is over this line (and not already mouse over)
                        if (x >= albumArtCoverWidth - _hScrollBar.Value &&
                            y >= offsetY &&
                            y <= offsetY + _songCache.LineHeight &&
                            !_items[a].IsMouseOverItem)
                        {
                            // Set item as mouse over
                            _items[a].IsMouseOverItem = true;

                            // Invalidate region and update control
                            OnInvalidateVisualInRect(new BasicRectangle(albumArtCoverWidth - _hScrollBar.Value, offsetY, Frame.Width - albumArtCoverWidth + _hScrollBar.Value, _songCache.LineHeight));
                            controlNeedsToBeUpdated = true;

                            // Exit loop
                            break;
                        }
                    }
                }
            }

            //// Check if the control needs to be refreshed
            //if (controlNeedsToBeUpdated)
            //    Update();
        }

        /// <summary>
        /// Creates a cache of values used for rendering the song grid view.
        /// Also sets scrollbar position, height, value, maximum, etc.
        /// </summary>
        public void InvalidateSongCache()
        {
            // Check if columns have been created
            if (_columns == null || _columns.Count == 0 || _items == null)
                return;

            // Create cache
            _songCache = new SongGridViewCache();

            // Get active columns and order them
            _songCache.ActiveColumns = _columns.Where(x => x.Order >= 0).OrderBy(x => x.Order).ToList();

            // Load custom font
            //Font fontDefaultBold = Tools.LoadCustomFont(FontCollection, CustomFontName, Font.Size, FontStyle.Bold);
            //Font fontDefaultBold = null;

            //// Make sure the embedded font name needs to be loaded and is valid
            //if (_theme.RowTextGradient.Font.UseEmbeddedFont && !String.IsNullOrEmpty(_theme.RowTextGradient.Font.EmbeddedFontName))
            //{
            //    try
            //    {
            //        // Get embedded font collection
            //        EmbeddedFontCollection embeddedFonts = EmbeddedFontHelper.GetEmbeddedFonts();

            //        // Get embedded fonts                    
            //        fontDefaultBold = Tools.LoadEmbeddedFont(embeddedFonts, _theme.RowTextGradient.Font.EmbeddedFontName, _theme.RowTextGradient.Font.Size, _theme.RowTextGradient.Font.ToFontStyle() | FontStyle.Bold);
            //    }
            //    catch
            //    {
            //        // Use default font instead                    
            //        fontDefaultBold = new Font(this.Font, FontStyle.Bold);
            //    }
            //}

            //// Check if font is null
            //if (fontDefaultBold == null)
            //{
            //    try
            //    {
            //        // Try to get standard font                    
            //        fontDefaultBold = new Font(_theme.RowTextGradient.Font.StandardFontName, _theme.RowTextGradient.Font.Size, _theme.RowTextGradient.Font.ToFontStyle() | FontStyle.Bold);
            //    }
            //    catch
            //    {
            //        // Use default font instead                    
            //        fontDefaultBold = new Font(this.Font, FontStyle.Bold);
            //    }
            //}

            //// Load default fonts if custom fonts were not found
            //if (fontDefaultBold == null)
            //{
            //    // Load default font
            //    fontDefaultBold = new Font(Font.FontFamily.Name, Font.Size, FontStyle.Bold);
            //}

            //// Create temporary bitmap/graphics objects to measure a string (to determine line height)
            //Bitmap bmpTemp = new Bitmap(200, 100);
            //Graphics g = Graphics.FromImage(bmpTemp);
            //SizeF sizeFont = g.MeasureString("QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm!@#$%^&*()", fontDefaultBold);
            //g.Dispose();
            //g = null;
            //bmpTemp.Dispose();
            //bmpTemp = null;

            //// Calculate the line height (try to measure the total possible height of characters using the custom or default font)
            //_songCache.LineHeight = (int)sizeFont.Height + _theme.Padding;
            //_songCache.TotalHeight = _songCache.LineHeight * _items.Count;

            //// Check if the total active columns width exceed the width available in the control
            //_songCache.TotalWidth = 0;
            //for (int a = 0; a < _songCache.ActiveColumns.Count; a++)
            //{
            //    // Check if column is visible
            //    if (_songCache.ActiveColumns[a].Visible)
            //    {
            //        // Increment total width
            //        _songCache.TotalWidth += _songCache.ActiveColumns[a].Width;
            //    }
            //}

            //// Calculate the number of lines visible (count out the header, which is one line height)
            //_songCache.NumberOfLinesFittingInControl = (int)Math.Floor((double)(ClientRectangle.Height) / (double)(_songCache.LineHeight));

            //// Set vertical scrollbar dimensions
            //_vScrollBar.Top = _songCache.LineHeight;
            //_vScrollBar.Left = ClientRectangle.Width - _vScrollBar.Width;
            //_vScrollBar.Minimum = 0;

            //// Scrollbar maximum is the number of lines fitting in the screen + the remaining line which might be cut
            //// by the control height because it's not a multiple of line height (i.e. the last line is only partly visible)
            //int lastLineHeight = ClientRectangle.Height - (_songCache.LineHeight * _songCache.NumberOfLinesFittingInControl);

            //// Check width
            //if (_songCache.TotalWidth > ClientRectangle.Width - _vScrollBar.Width)
            //{
            //    // Set scrollbar values
            //    _hScrollBar.Maximum = _songCache.TotalWidth;
            //    _hScrollBar.SmallChange = 5;
            //    _hScrollBar.LargeChange = ClientRectangle.Width;

            //    // Show scrollbar
            //    _hScrollBar.Visible = true;
            //}

            //// Check if the horizontal scrollbar needs to be turned off
            //if (_songCache.TotalWidth <= ClientRectangle.Width - _vScrollBar.Width && _hScrollBar.Visible)
            //{
            //    // Hide the horizontal scrollbar
            //    _hScrollBar.Visible = false;
            //}

            //// If there are less items than items fitting on screen...            
            //if (((_songCache.NumberOfLinesFittingInControl - 1) * _songCache.LineHeight) - _hScrollBar.Height >= _songCache.TotalHeight)
            //{
            //    // Disable the scrollbar
            //    _vScrollBar.Enabled = false;
            //    _vScrollBar.Value = 0;
            //}
            //else
            //{
            //    // Set scrollbar values
            //    _vScrollBar.Enabled = true;

            //    // The real large change needs to be added to the LargeChange and Maximum property in order to work. 
            //    int realLargeChange = _songCache.LineHeight * 5;

            //    // Calculate the vertical scrollbar maximum
            //    int vMax = _songCache.LineHeight * (_items.Count - _songCache.NumberOfLinesFittingInControl + 1) - lastLineHeight + realLargeChange;

            //    // Add the horizontal scrollbar height if visible
            //    if (_hScrollBar.Visible)
            //    {
            //        // Add height
            //        vMax += _hScrollBar.Height;
            //    }
                
            //    // Compensate for the header, and for the last line which might be truncated by the control height
            //    _vScrollBar.Maximum = vMax;
            //    _vScrollBar.SmallChange = _songCache.LineHeight;
            //    _vScrollBar.LargeChange = 1 + realLargeChange;
            //}

            //// Calculate the scrollbar offset Y
            //_songCache.ScrollBarOffsetY = (_startLineNumber * _songCache.LineHeight) - _vScrollBar.Value;

            //// If both scrollbars need to be visible, the width and height must be changed
            //if (_hScrollBar.Visible && _vScrollBar.Visible)
            //{
            //    // Cut 16 pixels
            //    _hScrollBar.Width = ClientRectangle.Width - 16;
            //    _vScrollBar.Height = ClientRectangle.Height - _songCache.LineHeight - 16;
            //}
            //else
            //{
            //    _vScrollBar.Height = ClientRectangle.Height - _songCache.LineHeight;
            //}
        }

        /// <summary>
        /// This timer triggers the animation of the currently playing song.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void TimerAnimationNowPlayingOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            // If the rectangle is "empty", do not trigger invalidation
            if (_rectNowPlaying.X == 0 &&
                _rectNowPlaying.Y == 0 &&
                _rectNowPlaying.Width == 1 &&
                _rectNowPlaying.Height == 1)
            {
                return;
            }

            // Increment counter            
            _timerAnimationNowPlayingCount += 10;

            // Invalidate region for now playing
            OnInvalidateVisualInRect(_rectNowPlaying);
        }
    }

    /// <summary>
    /// Result data structure used for the SongGridView background worker.
    /// </summary>
    public class SongGridViewBackgroundWorkerResult
    {
        public AudioFile AudioFile { get; set; }
        public IDisposable AlbumArt { get; set; }
    }

    /// <summary>
    /// Argument data structure used for the SongGridView background worker.
    /// </summary>
    public class SongGridViewBackgroundWorkerArgument
    {
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
        public int ColumnIndex { get; set; }
    }

}
