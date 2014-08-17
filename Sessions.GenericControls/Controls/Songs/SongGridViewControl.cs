// Copyri3w2qght © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Timers;
using Sessions.GenericControls.Basics;
using Sessions.GenericControls.Controls.Base;
using Sessions.GenericControls.Controls.Items;
using Sessions.GenericControls.Graphics;
using Sessions.GenericControls.Interaction;
using Sessions.GenericControls.Wrappers;
using Sessions.Core;
using Sessions.Sound.AudioFiles;

namespace Sessions.GenericControls.Controls.Songs
{
    /// <summary>
    /// This custom grid view control displays the Sessions library.
    /// </summary>
    public class SongGridViewControl : GridViewControlBase<SongGridViewItem, GridViewColumn>, IControlMouseInteraction, IControlKeyboardInteraction
    {
        private const int PreloadLinesAlbumCover = 20;
        private const int MinimumColumnWidth = 30;

        private readonly IDisposableImageFactory _disposableImageFactory;
        private readonly BackgroundWorker _workerUpdateAlbumArt = null;
        private readonly List<SongGridViewBackgroundWorkerArgument> _workerUpdateAlbumArtPile = null;
        private readonly Timer _timerUpdateAlbumArt = null;        

        private GridViewCache _cache = null;
        private List<SongGridViewImageCache> _imageCache = new List<SongGridViewImageCache>();

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
        private Timer _timerAnimationNowPlaying = null;
        
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

        private int _minimumRowsPerAlbum = 6;
        /// <summary>
        /// Defines the minimum number of rows per album; will fill space with empty rows.
        /// </summary>
        public int MinimumRowsPerAlbum
        {
            get
            {
                return _minimumRowsPerAlbum;
            }
            set
            {
                _minimumRowsPerAlbum = value;
                InvalidateVisual();
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
                var oldValue = _nowPlayingAudioFileId;
                _nowPlayingAudioFileId = value;
                InvalidateRow(oldValue, _nowPlayingAudioFileId);
            }
        }

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

        /// <summary>
        /// Default constructor for SongGridView.
        /// </summary>
        public SongGridViewControl(IHorizontalScrollBarWrapper horizontalScrollBar, IVerticalScrollBarWrapper verticalScrollBar, IDisposableImageFactory disposableImageFactory) :
            base(horizontalScrollBar, verticalScrollBar)
        {
            _disposableImageFactory = disposableImageFactory;
            _theme = new SongGridViewTheme();

            _timerAnimationNowPlaying = new Timer();
            _timerAnimationNowPlaying.Interval = 50;
            _timerAnimationNowPlaying.Elapsed += TimerAnimationNowPlayingOnElapsed;
            _timerAnimationNowPlaying.Enabled = true;

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

            CreateColumns();
        }

        private void CreateColumns()
        {
            var columnSongAlbumCover = new GridViewColumn("Album Cover", string.Empty, true, 0);
            var columnSongNowPlaying = new GridViewColumn("Now Playing", string.Empty, true, 1);
            var columnSongFileType = new GridViewColumn("Type", "FileType", false, 2);
            var columnSongTrackNumber = new GridViewColumn("Tr#", "DiscTrackNumber", true, 3);
            var columnSongTrackCount = new GridViewColumn("Track Count", "TrackCount", false, 4);
            var columnSongFilePath = new GridViewColumn("File Path", "FilePath", false, 5);
            var columnSongTitle = new GridViewColumn("Song Title", "Title", true, 6);
            var columnSongLength = new GridViewColumn("Length", "Length", true, 7);
            var columnSongArtistName = new GridViewColumn("Artist Name", "ArtistName", true, 8);
            var columnSongAlbumTitle = new GridViewColumn("Album Title", "AlbumTitle", true, 9);
            var columnSongGenre = new GridViewColumn("Genre", "Genre", false, 10);
            var columnSongPlayCount = new GridViewColumn("Play Count", "PlayCount", true, 11);
            var columnSongLastPlayed = new GridViewColumn("Last Played", "LastPlayed", true, 12);
            var columnSongBitrate = new GridViewColumn("Bitrate", "Bitrate", false, 13);
            var columnSongSampleRate = new GridViewColumn("Sample Rate", "SampleRate", false, 14);
            var columnSongTempo = new GridViewColumn("Tempo", "Tempo", false, 15);
            var columnSongYear = new GridViewColumn("Year", "Year", false, 16);

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
            Columns.Clear();
            Columns.Add(columnSongAlbumCover);
            Columns.Add(columnSongAlbumTitle);
            Columns.Add(columnSongArtistName);
            Columns.Add(columnSongBitrate);            
            Columns.Add(columnSongFilePath);            
            Columns.Add(columnSongGenre);
            Columns.Add(columnSongLastPlayed);
            Columns.Add(columnSongLength);
            Columns.Add(columnSongNowPlaying);
            Columns.Add(columnSongPlayCount);
            Columns.Add(columnSongSampleRate);
            Columns.Add(columnSongTitle);
            Columns.Add(columnSongTempo);            
            Columns.Add(columnSongTrackNumber);
            Columns.Add(columnSongTrackCount);
            Columns.Add(columnSongFileType);
            Columns.Add(columnSongYear);
        }

        protected override void ItemsCleared()
        {
            _cache = null;
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
                        var arg = _workerUpdateAlbumArtPile[a];

                        // Check if this album is still visible (cancel if it is out of display).                             
                        if (arg.LineIndex < _startLineNumber || arg.LineIndex > _startLineNumber + _numberOfLinesToDraw + PreloadLinesAlbumCover)
                        {
                            indexToDelete = a;
                            break;
                        }
                    }

                    if (indexToDelete >= 0)
                        _workerUpdateAlbumArtPile.RemoveAt(indexToDelete);                        
                    else
                        cleanUpDone = true;
                }

                // Continue executing pile
                if (_workerUpdateAlbumArtPile.Count > 0)
                    _workerUpdateAlbumArt.RunWorkerAsync(_workerUpdateAlbumArtPile[0]);
            }

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

            var arg = (SongGridViewBackgroundWorkerArgument)e.Argument;
            var result = new SongGridViewBackgroundWorkerResult();
            result.AudioFile = arg.AudioFile;

            // Check if this album is still visible (cancel if it is out of display).     
            if (arg.LineIndex < _startLineNumber || arg.LineIndex > _startLineNumber + _numberOfLinesToDraw + PreloadLinesAlbumCover)
            {
                // Set result with empty image
                e.Result = result;
                return;
            }

            // Extract image from file
            var bytes = AudioFile.ExtractImageByteArrayForAudioFile(arg.AudioFile.FilePath);
            if (bytes != null && bytes.Length > 0)
            {
                var image = _disposableImageFactory.CreateImageFromByteArray(bytes, (int)arg.RectAlbumArt.Width, (int)arg.RectAlbumArt.Height);
                result.AlbumArt = image;
            }

            e.Result = result;
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
                    imageTemp.Image.Dispose();
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

            InvalidateVisual();
        }

        /// <summary>
        /// Imports audio files as SongGridViewItems.
        /// </summary>
        /// <param name="audioFiles">List of AudioFiles</param>
        public void ImportAudioFiles(List<AudioFile> audioFiles)
        {
            Items.Clear();

            var albums = audioFiles.GroupBy(x => new { x.ArtistName, x.AlbumTitle });
            foreach (var album in albums)
            {
                var songs = audioFiles.Where(x => x.ArtistName == album.Key.ArtistName && x.AlbumTitle == album.Key.AlbumTitle).ToArray();
                foreach (var song in songs)
                {
                    var item = new SongGridViewItem();
                    item.AudioFile = song;
                    item.PlaylistItemId = Guid.NewGuid();
                    item.AlbumArtKey = album.Key.ArtistName + "_" + album.Key.AlbumTitle;
                    Items.Add(item);
                }
                if (songs.Length < MinimumRowsPerAlbum)
                {
                    for (int a = 0; a < MinimumRowsPerAlbum - songs.Length; a++)
                    {
                        var item = new SongGridViewItem();
                        item.IsEmptyRow = true;
                        item.AlbumArtKey = album.Key.ArtistName + "_" + album.Key.AlbumTitle;
                        Items.Add(item);
                    }
                } 
            }

            _cache = null;
            VerticalScrollBar.Value = 0;
            InvalidateVisual();
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
                int offsetY = (a * _cache.LineHeight) - VerticalScrollBar.Value + _cache.LineHeight;
                if (Items[a].AudioFile.Id == audioFileId)
                {
                    InvalidateVisualInRect(new BasicRectangle(Columns[0].Width - HorizontalScrollBar.Value, offsetY, Frame.Width - Columns[0].Width + HorizontalScrollBar.Value, _cache.LineHeight));
                    break;
                }
            }
        }

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
        //        GridViewColumn column = Columns.FirstOrDefault(x => x.Title == menuItem.Tag.ToString());
        //        if (column != null)
        //        {
        //            // Set visibility
        //            column.Visible = menuItem.Checked;
        //        }

        //        // Reset cache
        //        _cache = null;
        //        Refresh();
        //    }
        //}

        public void MouseWheel(float delta)
        {
            if (Columns == null || _cache == null)
                return;

            // Make sure the mouse cursor is over the control, and that the vertical scrollbar is enabled
            if (!_isMouseOverControl || !VerticalScrollBar.Enabled)
                return;

            // Get relative value
            //int value = delta / SystemInformation.MouseWheelScrollDelta;

            int newValue = (int) (VerticalScrollBar.Value + (-delta * _cache.LineHeight));
            //Console.WriteLine("SongGridViewControl - MouseWheel - delta: {0} VerticalScrollBar.Value: {1} lineHeight: {2} newValue: {3}", delta, VerticalScrollBar.Value, _cache.LineHeight, newValue);

            newValue = Math.Min(newValue, VerticalScrollBar.Maximum - VerticalScrollBar.LargeChange);
            newValue = Math.Max(newValue, 0);
            
            VerticalScrollBar.Value = newValue;
            InvalidateVisual();
        }

        /// <summary>
        /// Occurs when the control needs to be painted.
        /// </summary>
        public override void Render(IGraphicsContext context)
        {
            base.Render(context);

            if (Items == null)
                return;

            //var stopwatch = new Stopwatch();
            //stopwatch.Start();

            // If frame doesn't match, refresh frame and song cache
            if (Frame.Width != context.BoundsWidth || Frame.Height != context.BoundsHeight || _cache == null)
            {
                Frame = new BasicRectangle(context.BoundsWidth, context.BoundsHeight);
                InvalidateSongCache();
            }            

            // Draw background
            context.DrawRectangle(Frame, new BasicBrush(_theme.AlbumCoverBackgroundColor), new BasicPen());

            DrawRows(context);
            DrawHeader(context);
            DrawDebugInformation(context);

            if (HorizontalScrollBar.Visible && VerticalScrollBar.Visible)
            {
                // Draw a bit of control color over the 16x16 area in the lower right corner
                var brush = new BasicBrush(new BasicColor(200, 200, 200));
                context.DrawRectangle(new BasicRectangle(Frame.Width - 16, Frame.Height - 16, 16, 16), brush, new BasicPen());
            }

            //stopwatch.Stop();
            //Console.WriteLine("SongGridViewControl - Render - Completed in {0} - frame: {1} numberOfLinesToDraw: {2}", stopwatch.Elapsed, Frame, _numberOfLinesToDraw);
        }

        private void DrawRows(IGraphicsContext context)
        {
            var state = new DrawCellState();
            var penTransparent = new BasicPen();    

            // Calculate how many lines must be skipped because of the scrollbar position
            _startLineNumber = Math.Max((int) Math.Floor((double) VerticalScrollBar.Value/(double) (_cache.LineHeight)), 0);

            // Check if the total number of lines exceeds the number of icons fitting in height
            _numberOfLinesToDraw = 0;
            if (_startLineNumber + _cache.NumberOfLinesFittingInControl > Items.Count)
            {
                // There aren't enough lines to fill the screen
                _numberOfLinesToDraw = Items.Count - _startLineNumber;
            }
            else
            {
                // Fill up screen 
                _numberOfLinesToDraw = _cache.NumberOfLinesFittingInControl;
            }

            // Add one line for overflow; however, make sure we aren't adding a line without content 
            if (_startLineNumber + _numberOfLinesToDraw + 1 <= Items.Count)
                _numberOfLinesToDraw++;

            // Loop through lines
            for (int a = _startLineNumber; a < _startLineNumber + _numberOfLinesToDraw; a++)
            {                
                // Calculate offsets, widths and other variants
                state.OffsetX = 0;
                state.OffsetY = (a * _cache.LineHeight) - VerticalScrollBar.Value + _cache.LineHeight; // compensate for scrollbar position
                int albumArtColumnWidth = Columns[0].Visible ? Columns[0].Width : 0;
                int lineBackgroundWidth = (int) (Frame.Width + HorizontalScrollBar.Value - albumArtColumnWidth);
                if (VerticalScrollBar.Visible)
                    lineBackgroundWidth -= VerticalScrollBar.Width;

                // Check conditions to determine background color
                var audioFile = Items[a].AudioFile;
                var colorBackground1 = _theme.BackgroundColor;
                var colorBackground2 = _theme.BackgroundColor;
                if (audioFile != null && audioFile.Id == _nowPlayingAudioFileId)
                {
                    colorBackground1 = _theme.NowPlayingBackgroundColor;
                    colorBackground2 = _theme.NowPlayingBackgroundColor;
                }

                if (Items[a].IsSelected)
                {
                    colorBackground1 = _theme.SelectedBackgroundColor;
                    colorBackground2 = _theme.SelectedBackgroundColor;

//                    // Use darker color
//                    byte diff = 4;
//                    colorBackground1 = new BasicColor(255,
//                        (byte)((colorBackground1.R - diff < 0) ? 0 : colorBackground1.R - diff),
//                        (byte)((colorBackground1.G - diff < 0) ? 0 : colorBackground1.G - diff),
//                        (byte)((colorBackground1.B - diff < 0) ? 0 : colorBackground1.B - diff));
//                    colorBackground2 = new BasicColor(255,
//                        (byte)((colorBackground2.R - diff < 0) ? 0 : colorBackground2.R - diff),
//                        (byte)((colorBackground2.G - diff < 0) ? 0 : colorBackground2.G - diff),
//                        (byte)((colorBackground2.B - diff < 0) ? 0 : colorBackground2.B - diff));
                }

                //// Check if mouse is over item
                //if (items[a].IsMouseOverItem)
                //{
                //    // Use lighter color
                //    int diff = 20;
                //    colorBackground1 = Color.FromArgb(255,
                //        (colorBackground1.R + diff > 255) ? 255 : colorBackground1.R + diff,
                //        (colorBackground1.G + diff > 255) ? 255 : colorBackground1.G + diff,
                //        (colorBackground1.B + diff > 255) ? 255 : colorBackground1.B + diff);
                //    colorBackground2 = Color.FromArgb(255,
                //        (colorBackground2.R + diff > 255) ? 255 : colorBackground2.R + diff,
                //        (colorBackground2.G + diff > 255) ? 255 : colorBackground2.G + diff,
                //        (colorBackground2.B + diff > 255) ? 255 : colorBackground2.B + diff);
                //}

                //// Check conditions to determine background color
                //if ((_mode == SongGridViewMode.AudioFile && audioFile.Id == _nowPlayingAudioFileId) ||
                //    (_mode == SongGridViewMode.Playlist && Items[a].PlaylistItemId == _nowPlayingPlaylistItemId))
                //{
                //    colorNowPlaying1 = colorBackground1;
                //    colorNowPlaying2 = colorBackground2;
                //}

                // Draw row background
                var rectBackground = new BasicRectangle(albumArtColumnWidth - HorizontalScrollBar.Value, state.OffsetY, lineBackgroundWidth, _cache.LineHeight + 1);
                var brushGradient = new BasicGradientBrush(colorBackground1, colorBackground2, 90);
                context.DrawRectangle(rectBackground, brushGradient, penTransparent);

                // Loop through columns                
                for (int b = 0; b < _cache.ActiveColumns.Count; b++)
                    DrawCell(context, a, b, audioFile, state);
            }

            // If no songs are playing, set the current now playing rectangle as "empty"
            if (!state.NowPlayingSongFound)
                _rectNowPlaying = new BasicRectangle(0, 0, 1, 1);
        }

        private void DrawCell(IGraphicsContext context, int row, int col, AudioFile audioFile, DrawCellState state)
        {
            var rect = new BasicRectangle();
            var brush = new BasicBrush();
            var brushGradient = new BasicGradientBrush();
            var penTransparent = new BasicPen();
            var column = _cache.ActiveColumns[col];
            if (column.Visible)
            {
                if (column.Title == "Now Playing")
                {
                    // Draw now playing icon
                    if (audioFile != null && audioFile.Id == _nowPlayingAudioFileId)
                    {
                        // Which size is the minimum? Width or height?                    
                        int availableWidthHeight = column.Width - 4;
                        if (_cache.LineHeight <= column.Width)
                            availableWidthHeight = _cache.LineHeight - 4;
                        else
                            availableWidthHeight = column.Width - 4;

                        // Calculate the icon position                                
                        float iconNowPlayingX = ((column.Width - availableWidthHeight) / 2) + state.OffsetX - HorizontalScrollBar.Value;
                        float iconNowPlayingY = state.OffsetY + ((_cache.LineHeight - availableWidthHeight) / 2);

                        // Create NowPlaying rect (MUST be in integer)                    
                        _rectNowPlaying = new BasicRectangle((int)iconNowPlayingX, (int)iconNowPlayingY, availableWidthHeight, availableWidthHeight);
                        state.NowPlayingSongFound = true;

                        // Draw outer circle
                        brushGradient = new BasicGradientBrush(_theme.NowPlayingIndicatorBackgroundColor, _theme.NowPlayingIndicatorBackgroundColor, _timerAnimationNowPlayingCount % 360);
                        context.DrawEllipsis(_rectNowPlaying, brushGradient, penTransparent);

                        // Draw inner circle
                        rect = new BasicRectangle((int)iconNowPlayingX + 4, (int)iconNowPlayingY + 4, availableWidthHeight - 8, availableWidthHeight - 8);
                        brush = new BasicBrush(_theme.NowPlayingBackgroundColor);
                        context.DrawEllipsis(rect, brush, penTransparent);
                    }
                }
                else if (column.Title == "Album Cover")
                {
                    DrawAlbumCoverZone(context, row, state);
                }
                else if (audioFile != null)
                {
                    // Print value depending on type
                    var propertyInfo = audioFile.GetType().GetProperty(column.FieldName);
                    if (propertyInfo != null)
                    {
                        string value = string.Empty;
                        try
                        {
                            if (propertyInfo.PropertyType.FullName == "System.String")
                            {
                                value = propertyInfo.GetValue(audioFile, null).ToString();
                            }
                            else if (propertyInfo.PropertyType.FullName.Contains("Int64") &&
                                propertyInfo.PropertyType.FullName.Contains("Nullable"))
                            {
                                long? longValue = (long?)propertyInfo.GetValue(audioFile, null);
                                if (longValue.HasValue)
                                    value = longValue.Value.ToString();
                            }
                            else if (propertyInfo.PropertyType.FullName.Contains("DateTime") &&
                                propertyInfo.PropertyType.FullName.Contains("Nullable"))
                            {
                                DateTime? dateTimeValue = (DateTime?)propertyInfo.GetValue(audioFile, null);
                                if (dateTimeValue.HasValue)
                                    value = dateTimeValue.Value.ToShortDateString() + " " + dateTimeValue.Value.ToShortTimeString();
                            }
                            else if (propertyInfo.PropertyType.FullName.Contains("System.UInt32"))
                            {
                                uint uintValue = (uint)propertyInfo.GetValue(audioFile, null);
                                value = uintValue.ToString();
                            }
                            else if (propertyInfo.PropertyType.FullName.Contains("System.Int32"))
                            {
                                int intValue = (int)propertyInfo.GetValue(audioFile, null);
                                value = intValue.ToString();
                            }
                            else if (propertyInfo.PropertyType.FullName.Contains("Sessions.Sound.AudioFileFormat"))
                            {
                                var theValue = (AudioFileFormat)propertyInfo.GetValue(audioFile, null);
                                value = theValue.ToString();
                            }
                        }
                        catch
                        {
                            // Do nothing
                        }

                        //// The last column always take the remaining width
                        //int columnWidth = column.Width;
                        //if (b == _cache.ActiveColumns.Count - 1)
                        //{
                        //    // Calculate the remaining width
                        //    int columnsWidth = 0;
                        //    for (int c = 0; c < _cache.ActiveColumns.Count - 1; c++)
                        //    {
                        //        columnsWidth += _cache.ActiveColumns[c].Width;
                        //    }
                        //    //columnWidth = (int) (Frame.Width - columnsWidth + HorizontalScrollBar.Value);
                        //}

                        // Display text
                        rect = new BasicRectangle(state.OffsetX - HorizontalScrollBar.Value + 2, state.OffsetY + (_theme.Padding / 2), _cache.ActiveColumns[col].Width, _cache.LineHeight - _theme.Padding + 2);
                        //stringFormat.Trimming = StringTrimming.EllipsisCharacter;
                        //stringFormat.Alignment = StringAlignment.Near;

                        // Use bold for ArtistName and DiscTrackNumber
                        if (column.FieldName == "ArtistName" || column.FieldName == "DiscTrackNumber")
                            context.DrawText(value, rect, _theme.TextColor, _theme.FontNameBold, _theme.FontSize);
                        else
                            context.DrawText(value, rect, _theme.TextColor, _theme.FontName, _theme.FontSize);
                    }
                }

                state.OffsetX += column.Width;
            }
        }

        private void DrawAlbumCoverZone(IGraphicsContext context, int row, DrawCellState state)
        {
            var pen = new BasicPen();
            var penTransparent = new BasicPen();
            var brushGradient = new BasicGradientBrush();
            var item = Items[row];
            //string albumTitle = audioFile != null ? audioFile.AlbumTitle : state.CurrentAlbumTitle; // if this is an empty row, keep last album title

            // Check for an album title change (or the last item of the grid)
            if (state.CurrentAlbumArtKey == item.AlbumArtKey)
                return;

            state.CurrentAlbumArtKey = item.AlbumArtKey;

            int albumCoverStartIndex = 0;
            int albumCoverEndIndex = 0;

            // For displaying the album cover, we need to know how many songs of the same album are bundled together
            // Start by getting the start index
            for (int c = row; c > 0; c--)
            {
                var previousItem = Items[c];
                if (previousItem.AlbumArtKey != item.AlbumArtKey)
                {
                    albumCoverStartIndex = c + 1;
                    break;
                }
            }

            // Find the end index
            for (int c = row + 1; c < Items.Count; c++)
            {
                var nextItem = Items[c];

                // If the album title is different, this means we found the next album title
                if (nextItem.AlbumArtKey != item.AlbumArtKey)
                {
                    albumCoverEndIndex = c - 1;
                    break;
                }
                // If this is the last item of the grid...
                else if (c == Items.Count - 1)
                {
                    albumCoverEndIndex = c;
                    break;
                }
            }

            var audioFile = Items[albumCoverStartIndex].AudioFile;

            // Calculate y and height
            int scrollbarOffsetY = (_startLineNumber * _cache.LineHeight) - VerticalScrollBar.Value;
            int y = ((albumCoverStartIndex - _startLineNumber) * _cache.LineHeight) + _cache.LineHeight + scrollbarOffsetY;

            // Calculate the height of the album cover zone (+1 on end index because the array is zero-based)
            int linesToCover = Math.Min(MinimumRowsPerAlbum, (albumCoverEndIndex + 1 - albumCoverStartIndex));
            int albumCoverZoneHeight = linesToCover * _cache.LineHeight;
            int heightWithPadding = Math.Min(albumCoverZoneHeight - (_theme.Padding * 2), _cache.ActiveColumns[0].Width - (_theme.Padding * 2));

            // Make sure the height is at least zero (not necessary to draw anything!)
            if (albumCoverZoneHeight > 0)
            {
                // Draw album cover background
                var rectAlbumCover = new BasicRectangle(0 - HorizontalScrollBar.Value, y, _cache.ActiveColumns[0].Width, albumCoverZoneHeight);
                brushGradient = new BasicGradientBrush(_theme.AlbumCoverBackgroundColor, _theme.AlbumCoverBackgroundColor, 90);
                context.DrawRectangle(rectAlbumCover, brushGradient, penTransparent);

                // Measure available width for text
                int widthAvailableForText = Columns[0].Width - (_theme.Padding * 2);

                // Display titles depending on if an album art was found
                var rectAlbumCoverArt = new BasicRectangle();
                var rectAlbumTitleText = new BasicRectangle();
                var rectArtistNameText = new BasicRectangle();
                var sizeAlbumTitle = new BasicRectangle();
                var sizeArtistName = new BasicRectangle();
                bool useAlbumArtOverlay = false;
                bool displayArtistNameAndAlbumTitle = false;

                // Try to extract image from cache
                IBasicImage imageAlbumCover = null;
                SongGridViewImageCache cachedImage = null;
                try
                {
                    cachedImage = _imageCache.FirstOrDefault(x => x.Key == item.AlbumArtKey);
                }
                catch (Exception ex)
                {
                    Tracing.Log(ex);
                }

                if (cachedImage != null)
                    imageAlbumCover = cachedImage.Image;

                // Album art not found in cache; try to find an album cover in one of the file
                if (cachedImage == null)
                {
                    try
                    {
                        // Check if the album cover is already in the pile
                        bool albumCoverFound = false;
                        foreach (var arg in _workerUpdateAlbumArtPile)
                        {
                            // Match by file path
                            if (arg.AudioFile.FilePath.ToUpper() == audioFile.FilePath.ToUpper())
                            {
                                albumCoverFound = true;
                            }
                        }

                        // Add to the pile only if the album cover isn't already in it
                        if (!albumCoverFound)
                        {
                            // Add item to update album art worker pile
                            var arg = new SongGridViewBackgroundWorkerArgument();
                            arg.AudioFile = audioFile;
                            arg.LineIndex = row;
                            arg.RectAlbumArt = new BasicRectangle(0, 0, heightWithPadding, heightWithPadding);
                            _workerUpdateAlbumArtPile.Add(arg);
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("SongGridViewControl - Failed to load cache image: {0}" , ex);
                    }
                }

                // There are at least two lines; is there an album cover to display?
                if (imageAlbumCover == null)
                {
                    // There is no album cover to display; display only text.
                    // Set string format
                    //stringFormat.Alignment = StringAlignment.Center;
                    //stringFormat.Trimming = StringTrimming.EllipsisWord;

                    sizeArtistName = context.MeasureText(audioFile.ArtistName, new BasicRectangle(0, 0, widthAvailableForText, heightWithPadding), _theme.FontNameAlbumArtTitle, _theme.FontSize + 2);
                    sizeAlbumTitle = context.MeasureText(audioFile.AlbumTitle, new BasicRectangle(0, 0, widthAvailableForText, heightWithPadding), _theme.FontNameAlbumArtSubtitle, _theme.FontSize);

                    // Display the album title at the top of the zome
                    rectArtistNameText = new BasicRectangle(_theme.Padding - HorizontalScrollBar.Value, y + _theme.Padding, widthAvailableForText, heightWithPadding);
                    rectAlbumTitleText = new BasicRectangle(_theme.Padding - HorizontalScrollBar.Value, y + _theme.Padding + sizeArtistName.Height, widthAvailableForText, heightWithPadding);
                    displayArtistNameAndAlbumTitle = true;
                    useAlbumArtOverlay = true;
                }
                else
                {
                    // There is an album cover to display with more than 2 lines.
                    // Set string format
                    //stringFormat.Alignment = StringAlignment.Near;
                    //stringFormat.Trimming = StringTrimming.EllipsisWord;

                    float widthRemainingForText = Columns[0].Width - (_theme.Padding * 3) - heightWithPadding;
                    sizeArtistName = context.MeasureText(audioFile.ArtistName, new BasicRectangle(0, 0, widthRemainingForText, heightWithPadding), _theme.FontNameAlbumArtTitle, _theme.FontSize + 2);
                    sizeAlbumTitle = context.MeasureText(audioFile.AlbumTitle, new BasicRectangle(0, 0, widthRemainingForText, heightWithPadding), _theme.FontNameAlbumArtSubtitle, _theme.FontSize);

                    // Try to center the cover art + padding + max text width
                    //float maxWidth = Math.Max(sizeArtistName.Width, sizeAlbumTitle.Width);
                    float albumCoverX = _theme.Padding - 2; // (Columns[0].Width - heightWithPadding - _theme.Padding - _theme.Padding - maxWidth) / 2;
                    float artistNameY = _theme.Padding + 1; // (albumCoverZoneHeight - sizeArtistName.Height - sizeAlbumTitle.Height) / 2;

                    // Display the album title at the top of the zome
                    rectArtistNameText = new BasicRectangle(albumCoverX + heightWithPadding + _theme.Padding - HorizontalScrollBar.Value, y + artistNameY, widthRemainingForText, heightWithPadding);
                    rectAlbumTitleText = new BasicRectangle(albumCoverX + heightWithPadding + _theme.Padding - HorizontalScrollBar.Value, y + artistNameY + sizeArtistName.Height + (_theme.Padding / 8f), widthRemainingForText, heightWithPadding);
                    rectAlbumCoverArt = new BasicRectangle(albumCoverX - HorizontalScrollBar.Value, y + _theme.Padding, heightWithPadding, heightWithPadding);
                    displayArtistNameAndAlbumTitle = widthRemainingForText > 20;
                    useAlbumArtOverlay = true;
                }

                // Display album cover
                if (imageAlbumCover != null)
                    context.DrawImage(rectAlbumCoverArt, new BasicRectangle(0, 0, imageAlbumCover.ImageSize.Width, imageAlbumCover.ImageSize.Height), imageAlbumCover.Image);
                    //context.DrawImage(rectAlbumCoverArt, new BasicRectangle(0, 0, rectAlbumCoverArt.Width, rectAlbumCoverArt.Height), imageAlbumCover.Image);

//                if (useAlbumArtOverlay)
//                {
//                    // Draw artist name and album title background
//                    var rectArtistNameBackground = new BasicRectangle(rectArtistNameText.X - (_theme.Padding / 2), rectArtistNameText.Y - (_theme.Padding / 4), sizeArtistName.Width + _theme.Padding, sizeArtistName.Height + (_theme.Padding / 2));
//                    var rectAlbumTitleBackground = new BasicRectangle(rectAlbumTitleText.X - (_theme.Padding / 2), rectAlbumTitleText.Y - (_theme.Padding / 4), sizeAlbumTitle.Width + _theme.Padding, sizeAlbumTitle.Height + (_theme.Padding / 2));
//                    var brushTextBackground = new BasicBrush(new BasicColor(0, 0, 0, 30));
//                    context.DrawRectangle(rectArtistNameBackground, brushTextBackground, penTransparent);
//                    context.DrawRectangle(rectAlbumTitleBackground, brushTextBackground, penTransparent);
//                }

                if (displayArtistNameAndAlbumTitle)
                {
                    context.DrawText(audioFile.ArtistName, rectArtistNameText, _theme.HeaderTextColor, _theme.FontNameAlbumArtTitle, _theme.FontSize + 2);
                    context.DrawText(audioFile.AlbumTitle, rectAlbumTitleText, _theme.HeaderTextColor, _theme.FontNameAlbumArtSubtitle, _theme.FontSize);
                }

                // Draw horizontal line to distinguish albums
                // Part 1: Draw line over grid
                pen = new BasicPen(new BasicBrush(new BasicColor(180, 180, 180)), 1);
                context.DrawLine(new BasicPoint(Columns[0].Width, y), new BasicPoint(Frame.Width, y), pen);

                // Part 2: Draw line over album art zone, in a lighter color
                pen = new BasicPen(new BasicBrush(new BasicColor(115, 115, 115)), 1);
                context.DrawLine(new BasicPoint(0, y), new BasicPoint(Columns[0].Width, y), pen);
            }
        }

        private void DrawHeader(IGraphicsContext context)
        {
            var rect = new BasicRectangle();
            var pen = new BasicPen();
            var penTransparent = new BasicPen();
            var brushGradient = new BasicGradientBrush(_theme.HeaderBackgroundColor, _theme.HeaderBackgroundColor, 90);

            // Draw header (for some reason, the Y must be set -1 to cover an area which isn't supposed to be displayed)
            var rectBackgroundHeader = new BasicRectangle(0, -1, Frame.Width, _cache.LineHeight + 1);
            context.DrawRectangle(rectBackgroundHeader, brushGradient, penTransparent);

            // Loop through columns
            int offsetX = 0;
            for (int b = 0; b < _cache.ActiveColumns.Count; b++)
            {
                var column = _cache.ActiveColumns[b];
                if (column.Visible)
                {
                    // The last column always take the remaining width
                    int columnWidth = column.Width;
                    if (b == _cache.ActiveColumns.Count - 1)
                    {
                        // Calculate the remaining width
                        int columnsWidth = 0;
                        for (int c = 0; c < _cache.ActiveColumns.Count - 1; c++)
                            columnsWidth += _cache.ActiveColumns[c].Width;
                        columnWidth = (int) (Frame.Width - columnsWidth + HorizontalScrollBar.Value);
                    }

                    // Check if mouse is over this column header
                    if (column.IsMouseOverColumnHeader)
                    {
                        // Draw header (for some reason, the Y must be set -1 to cover an area which isn't supposed to be displayed)                        
                        rect = new BasicRectangle(offsetX - HorizontalScrollBar.Value, -1, column.Width, _cache.LineHeight + 1);
                        brushGradient = new BasicGradientBrush(_theme.MouseOverHeaderBackgroundColor, _theme.MouseOverHeaderBackgroundColor, 90);
                        context.DrawRectangle(rect, brushGradient, penTransparent);
                    }
                    else if (column.IsUserMovingColumn)
                    {
                        // Draw header (for some reason, the Y must be set -1 to cover an area which isn't supposed to be displayed)                        
                        rect = new BasicRectangle(offsetX - HorizontalScrollBar.Value, -1, column.Width, _cache.LineHeight + 1);
                        brushGradient = new BasicGradientBrush(new BasicColor(0, 0, 255), new BasicColor(0, 255, 0), 90);
                        context.DrawRectangle(rect, brushGradient, penTransparent);
                    }

                    // Check if the header title must be displayed
                    if (_cache.ActiveColumns[b].IsHeaderTitleVisible)
                    {
                        // Display title                
                        var rectTitle = new BasicRectangle(offsetX - HorizontalScrollBar.Value + 2, _theme.Padding / 2, column.Width, _cache.LineHeight - _theme.Padding + 2);
                        //stringFormat.Trimming = StringTrimming.EllipsisCharacter;
                        context.DrawText(column.Title, rectTitle, _theme.HeaderTextColor, _theme.FontNameBold, _theme.FontSize);
                    }

                    // Draw column separator line; determine the height of the line
                    int columnHeight = (int) Frame.Height;

                    // Determine the height of the line; if the items don't fit the control height...
                    if (Items.Count < _cache.NumberOfLinesFittingInControl)
                    {
                        // Set height as the number of items (plus header)
                        columnHeight = (Items.Count + 1) * _cache.LineHeight;
                    }

                    // Draw column line
                    //g.DrawLine(Pens.DarkGray, new Point(offsetX + column.Width - HorizontalScrollBar.Value, 0), new Point(offsetX + column.Width - HorizontalScrollBar.Value, columnHeight));

                    // Check if the column is ordered by
                    if (column.FieldName == OrderByFieldName && !String.IsNullOrEmpty(column.FieldName))
                    {
                        // Create triangle points,,,
                        var ptTriangle = new BasicPoint[3];

                        // ... depending on the order by ascending value
                        int triangleWidthHeight = 8;
                        int trianglePadding = (_cache.LineHeight - triangleWidthHeight) / 2;
                        if (OrderByAscending)
                        {
                            // Create points for ascending
                            ptTriangle[0] = new BasicPoint(offsetX + column.Width - triangleWidthHeight - (triangleWidthHeight / 2) - HorizontalScrollBar.Value, trianglePadding);
                            ptTriangle[1] = new BasicPoint(offsetX + column.Width - triangleWidthHeight - HorizontalScrollBar.Value, _cache.LineHeight - trianglePadding);
                            ptTriangle[2] = new BasicPoint(offsetX + column.Width - triangleWidthHeight - triangleWidthHeight - HorizontalScrollBar.Value, _cache.LineHeight - trianglePadding);
                        }
                        else
                        {
                            // Create points for descending
                            ptTriangle[0] = new BasicPoint(offsetX + column.Width - triangleWidthHeight - (triangleWidthHeight / 2) - HorizontalScrollBar.Value, _cache.LineHeight - trianglePadding);
                            ptTriangle[1] = new BasicPoint(offsetX + column.Width - triangleWidthHeight - HorizontalScrollBar.Value, trianglePadding);
                            ptTriangle[2] = new BasicPoint(offsetX + column.Width - triangleWidthHeight - triangleWidthHeight - HorizontalScrollBar.Value, trianglePadding);
                        }

                        // Draw triangle
                        pen = new BasicPen(new BasicBrush(new BasicColor(255, 0, 0)), 1);
                    }

                    // Increment offset by the column width
                    offsetX += column.Width;
                }
            }

            // Display column move marker
            if (IsColumnMoving)
            {
                // Draw marker
                pen = new BasicPen(new BasicBrush(new BasicColor(255, 0, 0)), 1);
                context.DrawRectangle(new BasicRectangle(_columnMoveMarkerX - HorizontalScrollBar.Value, 0, 1, Frame.Height), new BasicBrush(), pen);
            }
        }

        private void DrawDebugInformation(IGraphicsContext context)
        {
            // Display debug information if enabled
            if (_displayDebugInformation)
            {
                // Build debug string
                var sbDebug = new StringBuilder();
                sbDebug.AppendLine("Line Count: " + Items.Count.ToString());
                sbDebug.AppendLine("Line Height: " + _cache.LineHeight.ToString());
                sbDebug.AppendLine("Lines Fit In Height: " + _cache.NumberOfLinesFittingInControl.ToString());
                sbDebug.AppendLine("Total Width: " + _cache.TotalWidth);
                sbDebug.AppendLine("Total Height: " + _cache.TotalHeight);
                sbDebug.AppendLine("Scrollbar Offset Y: " + _cache.ScrollBarOffsetY);
                sbDebug.AppendLine("HScrollbar Maximum: " + HorizontalScrollBar.Maximum.ToString());
                sbDebug.AppendLine("HScrollbar LargeChange: " + HorizontalScrollBar.LargeChange.ToString());
                sbDebug.AppendLine("HScrollbar Value: " + HorizontalScrollBar.Value.ToString());
                sbDebug.AppendLine("VScrollbar Maximum: " + VerticalScrollBar.Maximum.ToString());
                sbDebug.AppendLine("VScrollbar LargeChange: " + VerticalScrollBar.LargeChange.ToString());
                sbDebug.AppendLine("VScrollbar Value: " + VerticalScrollBar.Value.ToString());

                // Measure string
                var sizeDebugText = context.MeasureText(sbDebug.ToString(), new BasicRectangle(0, 0, Columns[0].Width, 40), _theme.FontName, _theme.FontSize);
                var rect = new BasicRectangle(0, 0, Columns[0].Width - 1, sizeDebugText.Height);

                // Draw background
                var brush = new BasicBrush(new BasicColor(200, 0, 0));
                var penTransparent = new BasicPen();
                context.DrawRectangle(rect, brush, penTransparent);

                // Draw string
                context.DrawText(sbDebug.ToString(), rect, new BasicColor(255, 255, 255), _theme.FontName, _theme.FontSize);
            }
        }

        public void KeyDown(char key, SpecialKeys specialKeys, ModifierKeys modifierKeys, bool isRepeat)
        {
            if (Columns == null || _cache == null)
                return;

            int selectedIndex = -1;
            int scrollbarOffsetY = (_startLineNumber * _cache.LineHeight) - VerticalScrollBar.Value;
            var startEndIndexes = GetStartIndexAndEndIndexOfSelectedRows();

            if (specialKeys == SpecialKeys.Enter)
            {
                _nowPlayingAudioFileId = Items[startEndIndexes.Item1].AudioFile.Id;
                ItemDoubleClick(startEndIndexes.Item1);
                return;
            }

            switch (specialKeys)
            {
                case SpecialKeys.Down:
                    if (startEndIndexes.Item1 < Items.Count - 1)
                    {
                        selectedIndex = startEndIndexes.Item1;
                        while (selectedIndex >= 0 && selectedIndex <= Items.Count - 1)
                        {
                            selectedIndex++;
                            if (!Items[selectedIndex].IsEmptyRow)
                                break;
                        }
                    }
                    break;
                case SpecialKeys.Up:
                    if (startEndIndexes.Item1 > 0)
                    {
                        selectedIndex = startEndIndexes.Item1;
                        while (selectedIndex >= 0 && selectedIndex <= Items.Count - 1)
                        {
                            selectedIndex--;
                            if (!Items[selectedIndex].IsEmptyRow)
                                break;
                        }
                    }
                    break;
                case SpecialKeys.PageDown:
                    selectedIndex = startEndIndexes.Item1 + _cache.NumberOfLinesFittingInControl - 2; // 2 is header + scrollbar height
                    selectedIndex = Math.Min(selectedIndex, Items.Count - 1);

                    if (selectedIndex == Items.Count - 1)
                    {
                        // If we are to select the last item, make sure the item we're selecting is NOT an empty row
                        selectedIndex = Items.FindLastIndex(x => !x.IsEmptyRow);
                    } 
                    else
                    {
                        // Continue to interate until we find a selectable row
                        while (selectedIndex >= 0 && selectedIndex <= Items.Count - 1)
                        {
                            selectedIndex++;
                            if (!Items[selectedIndex].IsEmptyRow)
                                break;
                        }
                    }
                    break;
                case SpecialKeys.PageUp:
                    selectedIndex = startEndIndexes.Item1 - _cache.NumberOfLinesFittingInControl + 2; 
                    selectedIndex = Math.Max(selectedIndex, 0);

                    if (selectedIndex > 0)
                    {
                        while (selectedIndex >= 0 && selectedIndex <= Items.Count - 1)
                        {
                            selectedIndex--;
                            if (!Items[selectedIndex].IsEmptyRow)
                                break;
                        }
                    }
                    break;
                case SpecialKeys.Home:
                    selectedIndex = 0; // First item cannot be empty
                    break;
                case SpecialKeys.End:
                    selectedIndex = Items.FindLastIndex(x => !x.IsEmptyRow);
                    break;
            }

            if (selectedIndex == -1)
                return;

            ResetSelection();
            Items[selectedIndex].IsSelected = true;

            // Check if new selection is out of bounds of visible area
            float y = ((selectedIndex - _startLineNumber + 1)*_cache.LineHeight) + scrollbarOffsetY;
            //Console.WriteLine("SongGridViewControl - KeyDown - y: {0} scrollbarOffsetY: {1} VerticalScrollBar.Value: {2}", y, scrollbarOffsetY, VerticalScrollBar.Value);

            int newVerticalScrollBarValue = VerticalScrollBar.Value;
            switch (specialKeys)
            {
                case SpecialKeys.Down:
                    // Check for out of bounds
                    if (y > Frame.Height - HorizontalScrollBar.Height - _cache.LineHeight)
                        newVerticalScrollBarValue = VerticalScrollBar.Value + _cache.LineHeight;
                    break;
                case SpecialKeys.Up:
                    // Check for out of bounds
                    if (y < _cache.LineHeight)
                        newVerticalScrollBarValue = VerticalScrollBar.Value - _cache.LineHeight;
                    break;
                case SpecialKeys.PageDown:
                    int heightToScrollDown = ((startEndIndexes.Item1 - _startLineNumber) * _cache.LineHeight) + scrollbarOffsetY;
                    newVerticalScrollBarValue = VerticalScrollBar.Value + heightToScrollDown;
                    break;
                case SpecialKeys.PageUp:
                    int heightToScrollUp = ((_startLineNumber + _cache.NumberOfLinesFittingInControl - startEndIndexes.Item1 - 2) * _cache.LineHeight) - scrollbarOffsetY;
                    newVerticalScrollBarValue = VerticalScrollBar.Value - heightToScrollUp;
                    break;
                case SpecialKeys.Home:
                    newVerticalScrollBarValue = 0;
                    break;
                case SpecialKeys.End:
                    newVerticalScrollBarValue = VerticalScrollBar.Maximum;
                    break;
            }

            // Make sure we don't scroll out of bounds
            if (newVerticalScrollBarValue > VerticalScrollBar.Maximum)
                newVerticalScrollBarValue = VerticalScrollBar.Maximum;
            if (newVerticalScrollBarValue < 0)
                newVerticalScrollBarValue = 0;
            VerticalScrollBar.Value = newVerticalScrollBarValue;

            // Is this necessary when scrolling the whole area? it will refresh all anyway
            //OnInvalidateVisualInRect(new BasicRectangle(albumArtCoverWidth - HorizontalScrollBar.Value, y, Frame.Width - albumArtCoverWidth + HorizontalScrollBar.Value, _cache.LineHeight));
            InvalidateVisual();
        }

        public void KeyUp(char key, SpecialKeys specialKeys, ModifierKeys modifierKeys, bool isRepeat)
        {
        }

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
            bool controlNeedsToBePartiallyInvalidated = false;
            var partialRect = new BasicRectangle();
            _isMouseOverControl = false;

            if (Columns == null || _cache == null)
                return;

            int scrollbarOffsetY = (_startLineNumber * _cache.LineHeight) - VerticalScrollBar.Value;
            if (Items.Count > 0)
            {
                for (int b = _startLineNumber; b < _startLineNumber + _numberOfLinesToDraw; b++)
                {
                    if (Items[b].IsMouseOverItem)
                    {
                        Items[b].IsMouseOverItem = false;
                        var newPartialRect = new BasicRectangle(Columns[0].Width - HorizontalScrollBar.Value, ((b - _startLineNumber + 1) * _cache.LineHeight) + scrollbarOffsetY, Frame.Width - Columns[0].Width + HorizontalScrollBar.Value, _cache.LineHeight);
                        partialRect.Merge(newPartialRect);
                        controlNeedsToBePartiallyInvalidated = true;
                        break;
                    }
                }
            }

            // Reset column flags
            int columnOffsetX2 = 0;
            for (int b = 0; b < _cache.ActiveColumns.Count; b++)
            {
                if (_cache.ActiveColumns[b].Visible)
                {
                    if (_cache.ActiveColumns[b].IsMouseOverColumnHeader)
                    {
                        _cache.ActiveColumns[b].IsMouseOverColumnHeader = false;
                        var newPartialRect = new BasicRectangle(columnOffsetX2 - HorizontalScrollBar.Value, 0, _cache.ActiveColumns[b].Width, _cache.LineHeight);
                        partialRect.Merge(newPartialRect);
                        controlNeedsToBePartiallyInvalidated = true;
                    }

                    columnOffsetX2 += _cache.ActiveColumns[b].Width;
                }
            }

            if (controlNeedsToBePartiallyInvalidated)
                InvalidateVisualInRect(partialRect);        
        }

        /// <summary>
        /// Occurs when the user is pressing down a mouse button.
        /// </summary>        
        public void MouseDown(float x, float y, MouseButtonType button, KeysHeld keysHeld)
        {
            _dragStartX = (int) x;
            if (Columns == null || _cache == null)
                return;

            // Loop through columns
            foreach (var column in _cache.ActiveColumns)
            {
                // Check for resizing column
                if (column.IsMouseCursorOverColumnLimit && column.CanBeResized && CanResizeColumns)
                {
                    column.IsUserResizingColumn = true;
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
        public void MouseUp(float x, float y, MouseButtonType button, KeysHeld keysHeld)
        {
            _dragStartX = -1;
            bool controlNeedsToBeFullyInvalidated = false;
            _isUserHoldingLeftMouseButton = false;

            if (Columns == null || _cache == null)
                return;

            // Get reference to the moving column
            GridViewColumn columnMoving = null;
            foreach (var column in _cache.ActiveColumns)
            {
                column.IsUserResizingColumn = false;
                if (column.IsUserMovingColumn)
                    columnMoving = column;
            }

            // Check if the user is moving a column
            if (columnMoving != null)
            {
                columnMoving.IsUserMovingColumn = false;
                controlNeedsToBeFullyInvalidated = true;

                // Find out on what column the mouse cursor is
                GridViewColumn columnOver = null;
                int currentX = 0;
                bool isPastCurrentlyMovingColumn = false;
                for (int a = 0; a < _cache.ActiveColumns.Count; a++)
                {
                    var currentColumn = _cache.ActiveColumns[a];
                    if (currentColumn.FieldName == columnMoving.FieldName)
                        isPastCurrentlyMovingColumn = true;

                    if (currentColumn.Visible)
                    {
                        // Check if the cursor is over the left part of the column
                        if (x >= currentX - HorizontalScrollBar.Value &&
                            x <= currentX + (currentColumn.Width/2) - HorizontalScrollBar.Value)
                        {
                            if (isPastCurrentlyMovingColumn && currentColumn.FieldName != columnMoving.FieldName)
                                columnOver = _cache.ActiveColumns[a - 1];
                            else
                                columnOver = _cache.ActiveColumns[a];
                            break;
                        }
                            // Check if the cursor is over the right part of the column
                        else if (x >= currentX + (currentColumn.Width/2) - HorizontalScrollBar.Value &&
                                 x <= currentX + currentColumn.Width - HorizontalScrollBar.Value)
                        {
                            // Check if there is a next item
                            if (a < _cache.ActiveColumns.Count - 1)
                            {
                                if (isPastCurrentlyMovingColumn)
                                    columnOver = _cache.ActiveColumns[a];
                                else
                                    columnOver = _cache.ActiveColumns[a + 1];
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
                var columnsOrdered = Columns.OrderBy(q => q.Order).ToList();

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

                // Check if the item needs to be inserted at the end
                if (indexAdd == -1)
                    columnsOrdered.Insert(columnsOrdered.Count, columnMoving);
                else
                    columnsOrdered.Insert(indexAdd, columnMoving);

                // Loop through columns to change the order of columnns
                for (int a = 0; a < columnsOrdered.Count; a++)
                    columnsOrdered[a].Order = a;
            }

            if (controlNeedsToBeFullyInvalidated)
            {
                InvalidateSongCache();
                InvalidateVisual();
            }
        }

        public void MouseClick(float x, float y, MouseButtonType button, KeysHeld keysHeld)
        {
            bool controlNeedsToBeFullyInvalidated = false;
            bool controlNeedsToBePartiallyInvalidated = false;
            var partialRect = new BasicRectangle();

            if (Columns == null || _cache == null)
                return;

            // Show context menu strip if the button click is right and not the album art column
            if (button == MouseButtonType.Right && x > Columns[0].Width && y > _cache.LineHeight)
                DisplayContextMenu(ContextMenuType.Item, x, y);

            int albumArtCoverWidth = Columns[0].Visible ? Columns[0].Width : 0;
            var columnResizing = Columns.FirstOrDefault(col => col.IsUserResizingColumn == true);
            int scrollbarOffsetY = (_startLineNumber * _cache.LineHeight) - VerticalScrollBar.Value;

            // Check if the user has clicked on the header (for orderBy)
            if (y >= 0 && y <= _cache.LineHeight &&
                columnResizing == null && !IsColumnMoving)
            {
                // Check on what column the user has clicked
                int offsetX = 0;
                for (int a = 0; a < _cache.ActiveColumns.Count; a++)
                {
                    var column = _cache.ActiveColumns[a];
                    if (column.Visible)
                    {
                        // Check if the mouse pointer is over this column
                        if (x >= offsetX - HorizontalScrollBar.Value && x <= offsetX + column.Width - HorizontalScrollBar.Value)
                        {
                            if (button == MouseButtonType.Left && CanChangeOrderBy)
                            {
                                // Check if the column order was already set
                                if (OrderByFieldName == column.FieldName)
                                {
                                    // Reverse ascending/descending
                                    OrderByAscending = !OrderByAscending;
                                }
                                else
                                {
                                    // Set order by field name
                                    OrderByFieldName = column.FieldName;
                                    OrderByAscending = true;
                                }

                                // Raise column click event and invalidate control
                                _cache = null;
                                ColumnClick(a);
                                InvalidateVisual();
                                return;
                            }
                            else if (button == MouseButtonType.Right)
                            {
                                //// Refresh column visibility in menu before opening
                                //foreach (ToolStripMenuItem menuItem in _menuColumns.Items)
                                //{
                                //    GridViewColumn menuItemColumn = Columns.FirstOrDefault(x => x.Title == menuItem.Tag.ToString());
                                //    if (menuItemColumn != null)
                                //        menuItem.Checked = menuItemColumn.Visible;
                                //}

                                DisplayContextMenu(ContextMenuType.Header, x, y);
                            }
                        }

                        offsetX += column.Width;
                    }
                }
            }

            // Loop through visible lines to find the original selected items
            var tuple = GetStartIndexAndEndIndexOfSelectedRows();
            int startIndex = tuple.Item1;
            int endIndex = tuple.Item2;

            // Make sure the indexes are set
            if (startIndex > -1 && endIndex > -1)
            {
                // Invalidate the original selected lines
                int startY = ((startIndex - _startLineNumber + 1) * _cache.LineHeight) + scrollbarOffsetY;
                int endY = ((endIndex - _startLineNumber + 2) * _cache.LineHeight) + scrollbarOffsetY;
                var newPartialRect = new BasicRectangle(albumArtCoverWidth - HorizontalScrollBar.Value, startY, Frame.Width - albumArtCoverWidth + HorizontalScrollBar.Value, endY - startY);
                partialRect.Merge(newPartialRect);
                controlNeedsToBePartiallyInvalidated = true;
            }

            // Reset selection (make sure SHIFT or CTRL isn't held down)
            if (!keysHeld.IsShiftKeyHeld && !keysHeld.IsCtrlKeyHeld)
            {
                // Make sure the mouse is over at least one item
                var mouseOverItem = Items.FirstOrDefault(item => item.IsMouseOverItem == true);
                if (mouseOverItem != null)
                    ResetSelection();
            }

            // Loop through visible lines to update the new selected items
            bool invalidatedNewSelection = false;
            for (int a = _startLineNumber; a < _startLineNumber + _numberOfLinesToDraw; a++)
            {
                // Check if mouse is over this item
                if (Items[a].IsMouseOverItem)
                {
                    invalidatedNewSelection = true;

                    // Check if SHIFT is held
                    if (keysHeld.IsShiftKeyHeld)
                    {
                        // Find the start index of the selection
                        int startIndexSelection = _lastItemIndexClicked;
                        if (a < startIndexSelection)
                            startIndexSelection = a;
                        if (startIndexSelection < 0)
                            startIndexSelection = 0;

                        // Find the end index of the selection
                        int endIndexSelection = _lastItemIndexClicked;
                        if (a > endIndexSelection)
                            endIndexSelection = a + 1;

                        // Loop through items to selected
                        for (int b = startIndexSelection; b < endIndexSelection; b++)
                            Items [b].IsSelected = true;

                        controlNeedsToBeFullyInvalidated = true;
                    }                
                    // Check if CTRL is held
                    else if(keysHeld.IsCtrlKeyHeld)
                    {
                        // Invert selection
                        Items[a].IsSelected = !Items[a].IsSelected;
                        var newPartialRect = new BasicRectangle(albumArtCoverWidth - HorizontalScrollBar.Value, ((a - _startLineNumber + 1) * _cache.LineHeight) + scrollbarOffsetY, Frame.Width - albumArtCoverWidth + HorizontalScrollBar.Value, _cache.LineHeight);
                        partialRect.Merge(newPartialRect);
                        controlNeedsToBePartiallyInvalidated = true;
                    }
                    else
                    {
                        // Set this item as the new selected item
                        Items[a].IsSelected = true;
                        var newPartialRect = new BasicRectangle(albumArtCoverWidth - HorizontalScrollBar.Value, ((a - _startLineNumber + 1) * _cache.LineHeight) + scrollbarOffsetY, Frame.Width - albumArtCoverWidth + HorizontalScrollBar.Value, _cache.LineHeight);
                        partialRect.Merge(newPartialRect);
                        controlNeedsToBePartiallyInvalidated = true;
                    }

                    // Set the last item clicked index
                    _lastItemIndexClicked = a;
                    break;
                }
            }

            // Raise selected item changed event (if an event is subscribed)
            if (invalidatedNewSelection)
            {
                if(SelectedItems.Count > 0)
                    SelectedIndexChanged();
                else
                    SelectedIndexChanged();
            }

            if (controlNeedsToBeFullyInvalidated)
                InvalidateVisual();
            else if (controlNeedsToBePartiallyInvalidated)
                InvalidateVisualInRect(partialRect);
        }

        /// <summary>
        /// Occurs when the user double-clicks on the control.
        /// Starts the playback of a new song.
        /// </summary>
        /// <param name="e">Event arguments</param>
        public void MouseDoubleClick(float x, float y, MouseButtonType button, KeysHeld keysHeld)
        {
            if (Columns == null || _cache == null)
                return;

            var partialRect = new BasicRectangle();
            bool controlNeedsToBePartiallyInvalidated = false;
            int albumArtCoverWidth = Columns[0].Visible ? Columns[0].Width : 0;
            int scrollbarOffsetY = (_startLineNumber * _cache.LineHeight) - VerticalScrollBar.Value;

            // Keep original songId in case the now playing value is set before invalidating the older value
            Guid originalId = _nowPlayingAudioFileId;

            // Loop through visible lines
            for (int a = _startLineNumber; a < _startLineNumber + _numberOfLinesToDraw; a++)
            {
                if (Items[a].IsMouseOverItem)
                {
                    // Set this item as the new now playing
                    _nowPlayingAudioFileId = Items[a].AudioFile.Id;

                    ItemDoubleClick(a);
                    var newPartialRect = new BasicRectangle(albumArtCoverWidth - HorizontalScrollBar.Value, ((a - _startLineNumber + 1) * _cache.LineHeight) + scrollbarOffsetY, Frame.Width - albumArtCoverWidth + HorizontalScrollBar.Value, _cache.LineHeight);
                    partialRect.Merge(newPartialRect);
                    controlNeedsToBePartiallyInvalidated = true;
                }
                else if (Items[a].AudioFile != null && Items[a].AudioFile.Id == originalId)
                {
                    var newPartialRect = new BasicRectangle(albumArtCoverWidth - HorizontalScrollBar.Value, ((a - _startLineNumber + 1) * _cache.LineHeight) + scrollbarOffsetY, Frame.Width - albumArtCoverWidth + HorizontalScrollBar.Value, _cache.LineHeight);
                    partialRect.Merge(newPartialRect);
                    controlNeedsToBePartiallyInvalidated = true;
                }
            }
                
            if (controlNeedsToBePartiallyInvalidated)
                InvalidateVisualInRect(partialRect);
        }        

        /// <summary>
        /// Occurs when the mouse pointer is moving over the control.
        /// Manages the display of mouse on/off visual effects.
        /// </summary>
        public void MouseMove(float x, float y, MouseButtonType button)
        {
            //Console.WriteLine("SongGridViewControl - MouseMove - x: {0} y: {1}", x, y);
            bool controlNeedsToBeFullyInvalidated = false;
            bool controlNeedsToBePartiallyInvalidated = false;
            var partialRect = new BasicRectangle();
            if (Columns == null || _cache == null)
                return;

            // Calculate album cover art width
            int albumArtCoverWidth = Columns[0].Visible ? Columns[0].Width : 0;

            // Check if the user is currently resizing a column (loop through columns)
            foreach (var column in _cache.ActiveColumns)
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
                    controlNeedsToBeFullyInvalidated = true;
                    InvalidateSongCache();

                    // Auto adjust horizontal scrollbar value if it exceeds the value range (i.e. do not show empty column)
                    if (HorizontalScrollBar.Value > HorizontalScrollBar.Maximum - HorizontalScrollBar.LargeChange)
                    {
                        // Set new value
                        int tempValue = HorizontalScrollBar.Maximum - HorizontalScrollBar.LargeChange;
                        if (tempValue < 0)
                            tempValue = 0;
                        HorizontalScrollBar.Value = tempValue;
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
                    foreach (GridViewColumn columnOver in _cache.ActiveColumns)
                    {
                        // Check if column is visible
                        if (columnOver.Visible)
                        {
                            // Check if the cursor is over the left part of the column
                            if (x >= currentX - HorizontalScrollBar.Value && x <= currentX + (columnOver.Width / 2) - HorizontalScrollBar.Value)
                                _columnMoveMarkerX = (int)x;
                            // Check if the cursor is over the right part of the column
                            else if (x >= currentX + (columnOver.Width / 2) - HorizontalScrollBar.Value && x <= currentX + columnOver.Width - HorizontalScrollBar.Value)
                                _columnMoveMarkerX = (int)x + columnOver.Width;

                            x += columnOver.Width;
                        }
                    }

                    controlNeedsToBeFullyInvalidated = true;
                }
            }

            if (!IsColumnMoving)
            {
                // Check if the cursor needs to be changed            
                int offsetX = 0;
                bool mousePointerIsOverColumnLimit = false;
                foreach (var column in _cache.ActiveColumns)
                {
                    if (column.Visible)
                    {
                        // Increment offset by the column width
                        offsetX += column.Width;
                        if (column.CanBeResized)
                        {
                            // Check if the mouse pointer is over a column (add 1 pixel so it's easier to select)
                            if (x >= offsetX - HorizontalScrollBar.Value && x <= offsetX + 1 - HorizontalScrollBar.Value)
                            {
                                mousePointerIsOverColumnLimit = true;
                                column.IsMouseCursorOverColumnLimit = true;
                                ChangeMouseCursorType(MouseCursorType.VSplit);
                            }
                            else
                            {
                                column.IsMouseCursorOverColumnLimit = false;
                            }
                        }
                    }
                }

                // Check if the default cursor needs to be restored
                if (!mousePointerIsOverColumnLimit)
                    ChangeMouseCursorType(MouseCursorType.Default);

                int columnOffsetX2 = 0;
                for (int b = 0; b < _cache.ActiveColumns.Count; b++)
                {
                    var column = _cache.ActiveColumns[b];
                    if (column.Visible)
                    {
                        // Was mouse over this column header?
                        if (column.IsMouseOverColumnHeader)
                        {
                            // Invalidate region
                            column.IsMouseOverColumnHeader = false;
                            var newPartialRect = new BasicRectangle(columnOffsetX2 - HorizontalScrollBar.Value, 0, column.Width, _cache.LineHeight);
                            partialRect.Merge(newPartialRect);
                            controlNeedsToBePartiallyInvalidated = true;
                        }

                        // Increment offset
                        columnOffsetX2 += column.Width;
                    }
                }

                // Check if the mouse pointer is over the header
                if (y >= 0 &&
                    y <= _cache.LineHeight)
                {
                    // Check on what column the user has clicked
                    int columnOffsetX = 0;
                    for (int a = 0; a < _cache.ActiveColumns.Count; a++)
                    {
                        var column = _cache.ActiveColumns[a];
                        if (column.Visible)
                        {
                            // Check if the mouse pointer is over this column
                            if (x >= columnOffsetX - HorizontalScrollBar.Value && x <= columnOffsetX + column.Width - HorizontalScrollBar.Value)
                            {
                                // Invalidate region
                                column.IsMouseOverColumnHeader = true;
                                var newPartialRect = new BasicRectangle(columnOffsetX - HorizontalScrollBar.Value, 0, column.Width, _cache.LineHeight);
                                partialRect.Merge(newPartialRect);
                                controlNeedsToBePartiallyInvalidated = true;
                                break;
                            }

                            columnOffsetX += column.Width;
                        }
                    }
                }

                // Check if the mouse cursor is over a line (loop through lines)                        
                int offsetY = 0;
                //int scrollbarOffsetY = (_startLineNumber * _cache.LineHeight) - VerticalScrollBar.Value;

                // Check if there's at least one item
                if (Items.Count > 0)
                {
                    // Reset mouse over item flags
                    for (int b = _startLineNumber; b < _startLineNumber + _numberOfLinesToDraw; b++)
                    {
                        //Console.WriteLine("SongGridViewControl - MouseMove - Checking for resetting mouse over flag for line {0}", b);
                        // Check if the mouse was over this item
                        if (Items[b].IsMouseOverItem)
                        {
                            // Reset flag and invalidate region
                            //Console.WriteLine("SongGridViewControl - MouseMove - Resetting mouse over flag for line {0}", b);
                            Items[b].IsMouseOverItem = false;
                            break;
                        }
                    }

                    // Put new mouse over flag
                    for (int a = _startLineNumber; a < _startLineNumber + _numberOfLinesToDraw; a++)
                    {
                        // Calculate offset
                        offsetY = (a * _cache.LineHeight) - VerticalScrollBar.Value + _cache.LineHeight;
                        //Console.WriteLine("SongGridViewControl - MouseMove - Checking for setting mouse over flag for line {0} - offsetY: {1}", a, offsetY);

                        // Check if the mouse cursor is over this line (and not already mouse over)
                        if (x >= albumArtCoverWidth - HorizontalScrollBar.Value &&
                            y >= offsetY &&
                            y <= offsetY + _cache.LineHeight &&
                            !Items[a].IsEmptyRow &&
                            !Items[a].IsMouseOverItem)
                        {
                            // Set item as mouse over
                            //Console.WriteLine("SongGridViewControl - MouseMove - Mouse is over item {0} {1}/{2}/{3}", a, Items[a].AudioFile.ArtistName, Items[a].AudioFile.AlbumTitle, Items[a].AudioFile.Title);
                            Items[a].IsMouseOverItem = true;
                            break;
                        }
                    }
                }
            }

            if (controlNeedsToBeFullyInvalidated)
                InvalidateVisual();
            else if (controlNeedsToBePartiallyInvalidated)
                InvalidateVisualInRect(partialRect);
        }

        /// <summary>
        /// Invalidates a row (useful for updating currently playing song).
        /// </summary>
        /// <param name="oldAudioFileId">Old audio file identifier.</param>
        /// <param name="newAudioFileId">New audio file identifier.</param>
        private void InvalidateRow(Guid oldAudioFileId, Guid newAudioFileId)
        {
            if (Items == null)
                return;

            int oldIndex = Items.FindIndex(x => x.AudioFile != null && x.AudioFile.Id == oldAudioFileId);
            int newIndex = Items.FindIndex(x => x.AudioFile != null && x.AudioFile.Id == newAudioFileId);
            int scrollbarOffsetY = (_startLineNumber * _cache.LineHeight) - VerticalScrollBar.Value;
            int firstIndex = oldIndex < newIndex ? oldIndex : newIndex;
            int secondIndex = newIndex > oldIndex ? newIndex : oldIndex;

            int firstY = -1;
            if (oldIndex >= _startLineNumber && oldIndex <= _startLineNumber + _numberOfLinesToDraw)
                firstY = ((firstIndex - _startLineNumber) * _cache.LineHeight) + scrollbarOffsetY;

            int secondY = -1;
            if (newIndex >= _startLineNumber && newIndex <= _startLineNumber + _numberOfLinesToDraw)
                secondY = ((secondIndex - _startLineNumber) * _cache.LineHeight) + scrollbarOffsetY;

            int finalY = 0;
            int finalHeight = 0;
            int lineHeight = _cache.LineHeight;

            if (firstY >= 0)
            {
                finalY = firstY;
                if (secondY >= 0)
                    finalHeight = (secondY + lineHeight) - firstY;
                else
                    finalHeight = lineHeight;
            } 
            else if (secondY >= 0)
            {
                finalY = secondY;
                finalHeight = lineHeight;
            }

            if (finalHeight > 0)
            {
                int headerHeight = _cache.LineHeight;
                var rect = new BasicRectangle(0, finalY + headerHeight, Frame.Width, finalHeight);
                //Console.WriteLine("SongGridViewControl - InvalidateRow - rect: {0} nowPlayingIndex: {1} startLineNumber: {2} numberOfLinesToDraw: {3} scrollbarOffsetY: {4}", rect, oldIndex, _startLineNumber, _numberOfLinesToDraw, scrollbarOffsetY);
                InvalidateVisualInRect(rect);
            }
        }

        /// <summary>
        /// Creates a cache of values used for rendering the song grid view.
        /// Also sets scrollbar position, height, value, maximum, etc.
        /// </summary>
        public void InvalidateSongCache()
        {
            // Check if columns have been created
            if (Columns == null || Columns.Count == 0 || Items == null)
                return;

            // Create cache
            _cache = new GridViewCache();

            // Get active columns and order them
            _cache.ActiveColumns = Columns.Where(x => x.Order >= 0).OrderBy(x => x.Order).ToList();

            //// Create temporary bitmap/graphics objects to measure a string (to determine line height)
            //Bitmap bmpTemp = new Bitmap(200, 100);
            //Graphics g = Graphics.FromImage(bmpTemp);
            //SizeF sizeFont = g.MeasureString("QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm!@#$%^&*()", fontDefaultBold);
            //g.Dispose();
            //g = null;
            //bmpTemp.Dispose();
            //bmpTemp = null;

            string allChars = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm!@#$%^&*()";
            //var rectText = context.MeasureText(allChars, new BasicRectangle(0, 0, 1000, 100), "Roboto", 12);
            var rectText = new BasicRectangle(0, 0, 100, 14);

            // Calculate the line height (try to measure the total possible height of characters using the custom or default font)
            _cache.LineHeight = (int)rectText.Height + _theme.Padding;
            _cache.TotalHeight = _cache.LineHeight * Items.Count;

            // Check if the total active columns width exceed the width available in the control
            _cache.TotalWidth = 0;
            for (int a = 0; a < _cache.ActiveColumns.Count; a++)
                if (_cache.ActiveColumns[a].Visible)
                    _cache.TotalWidth += _cache.ActiveColumns[a].Width;

            // Calculate the number of lines visible (count out the header, which is one line height)
            _cache.NumberOfLinesFittingInControl = (int)Math.Floor((double)(Frame.Height) / (double)(_cache.LineHeight));

            // Set vertical scrollbar dimensions
            //VerticalScrollBar.Top = _cache.LineHeight;
            //VerticalScrollBar.Left = ClientRectangle.Width - VerticalScrollBar.Width;
            VerticalScrollBar.Minimum = 0;            

            // Scrollbar maximum is the number of lines fitting in the screen + the remaining line which might be cut
            // by the control height because it's not a multiple of line height (i.e. the last line is only partly visible)
            int lastLineHeight = (int) (Frame.Height - (_cache.LineHeight * _cache.NumberOfLinesFittingInControl));

            // Check width
            if (_cache.TotalWidth > Frame.Width - VerticalScrollBar.Width)
            {
                // Set scrollbar values
                HorizontalScrollBar.Maximum = _cache.TotalWidth;
                HorizontalScrollBar.SmallChange = 5;
                HorizontalScrollBar.LargeChange = (int) Frame.Width;
                HorizontalScrollBar.Visible = true;
            }

            // Check if the horizontal scrollbar needs to be turned off
            if (_cache.TotalWidth <= Frame.Width - VerticalScrollBar.Width && HorizontalScrollBar.Visible)
                HorizontalScrollBar.Visible = false;

            // If there are less items than items fitting on screen...            
            if (((_cache.NumberOfLinesFittingInControl - 1) * _cache.LineHeight) - HorizontalScrollBar.Height >= _cache.TotalHeight)
            {
                // Disable the scrollbar
                VerticalScrollBar.Enabled = false;
                VerticalScrollBar.Value = 0;
            }
            else
            {
                VerticalScrollBar.Enabled = true;

                // Calculate the vertical scrollbar maximum
                int vMax = _cache.LineHeight * (Items.Count - _cache.NumberOfLinesFittingInControl + 1) - lastLineHeight;

                // Add the horizontal scrollbar height if visible
                if (HorizontalScrollBar.Visible)
                    vMax += HorizontalScrollBar.Height;

                // Compensate for the header, and for the last line which might be truncated by the control height
                VerticalScrollBar.Maximum = vMax;
                VerticalScrollBar.SmallChange = _cache.LineHeight;
                VerticalScrollBar.LargeChange = 1 + _cache.LineHeight * 5;
            }

            // Calculate the scrollbar offset Y
            _cache.ScrollBarOffsetY = (_startLineNumber * _cache.LineHeight) - VerticalScrollBar.Value;

            // If both scrollbars need to be visible, the width and height must be changed
            if (HorizontalScrollBar.Visible && VerticalScrollBar.Visible)
            {
                // Cut 16 pixels
                HorizontalScrollBar.Width = (int) (Frame.Width - 16);
                VerticalScrollBar.Height = Math.Max(0, (int) (Frame.Height - (_cache.LineHeight * 2) - 16));
            }
            else
            {
                VerticalScrollBar.Height = Math.Max(0, (int) (Frame.Height - (_cache.LineHeight * 2)));
            }
        }

        /// <summary>
        /// This timer triggers the animation of the currently playing song.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void TimerAnimationNowPlayingOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
//            // If the rectangle is "empty", do not trigger invalidation
//            if (_rectNowPlaying.X == 0 &&
//                _rectNowPlaying.Y == 0 &&
//                _rectNowPlaying.Width == 1 &&
//                _rectNowPlaying.Height == 1)
//            {
//                return;
//            }
//
//            // Increment counter            
//            _timerAnimationNowPlayingCount += 10;
//
//            // Invalidate region for now playing
//            OnInvalidateVisualInRect(_rectNowPlaying);
        }

        /// <summary>
        /// Result data structure used for the SongGridView background worker.
        /// </summary>
        public class SongGridViewBackgroundWorkerResult
        {
            public AudioFile AudioFile { get; set; }
            public IBasicImage AlbumArt { get; set; }
        }

        /// <summary>
        /// Argument data structure used for the SongGridView background worker.
        /// </summary>
        public class SongGridViewBackgroundWorkerArgument
        {
            public AudioFile AudioFile { get; set; }
            public int LineIndex { get; set; }
            public BasicRectangle RectAlbumArt { get; set; }
        }

        /// <summary>
        /// Data structure used for keeping a state while drawing cells.
        /// This is used to know when to draw the album art column.
        /// </summary>
        private class DrawCellState
        {
            public int OffsetX { get; set; }
            public int OffsetY { get; set; }
            public bool NowPlayingSongFound { get; set; }
            public string CurrentAlbumArtKey { get; set; }
        }
    }
}
