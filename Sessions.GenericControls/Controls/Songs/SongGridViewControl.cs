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
using Sessions.GenericControls.Services.Interfaces;
using Sessions.GenericControls.Services.Objects;
using Sessions.GenericControls.Controls.Themes;

namespace Sessions.GenericControls.Controls.Songs
{
    /// <summary>
    /// This custom grid view control displays the Sessions library.
    /// </summary>
    public class SongGridViewControl : GridViewControlBase<SongGridViewItem, GridViewColumn>
    {
        //private const int PreloadLinesAlbumCover = 20;

        private readonly IAlbumArtRequestService _albumArtRequestService;
        private readonly IAlbumArtCacheService _albumArtCacheService;

        // Animation timer and counter for currently playing song
        private int _timerAnimationNowPlayingCount = 0;
        private BasicRectangle _rectNowPlaying = new BasicRectangle(0, 0, 1, 1);
        private Timer _timerAnimationNowPlaying = null;
        private string _currentAlbumArtKey;
        private bool _nowPlayingSongFound;
        
        public SongGridViewTheme ExtendedTheme { get; set; }

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
            
        /// <summary>
        /// Default constructor for SongGridView.
        /// </summary>
        public SongGridViewControl(IHorizontalScrollBarWrapper horizontalScrollBar, IVerticalScrollBarWrapper verticalScrollBar, 
                        IAlbumArtRequestService albumArtRequestService, IAlbumArtCacheService albumArtCacheService) :
            base(horizontalScrollBar, verticalScrollBar)
        {
            _albumArtCacheService = albumArtCacheService;
            _albumArtRequestService = albumArtRequestService;
            _albumArtRequestService.OnAlbumArtExtracted += HandleOnAlbumArtExtracted;
            ExtendedTheme = new SongGridViewTheme();

            _timerAnimationNowPlaying = new Timer();
            _timerAnimationNowPlaying.Interval = 50;
            _timerAnimationNowPlaying.Elapsed += TimerAnimationNowPlayingOnElapsed;
            _timerAnimationNowPlaying.Enabled = false;

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

        private void HandleOnAlbumArtExtracted(IBasicImage image, AlbumArtRequest request)
        {
            //Console.WriteLine("SongGridViewControl - HandleOnAlbumArtExtracted - artistName: {0} albumTitle: {1}", request.ArtistName, request.AlbumTitle);

            // TODO: Do proper partial invalidation
            if(image != null)
                InvalidateVisual();
        }

        public override void InvalidateCache()
        {
            base.InvalidateCache();
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

            InvalidateCache();
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
            for (int a = StartLineNumber; a < StartLineNumber + NumberOfLinesToDraw; a++)
            {
                int offsetY = (a * ListCache.LineHeight) - VerticalScrollBar.Value + ListCache.LineHeight;
                if (Items[a].AudioFile.Id == audioFileId)
                {
                    InvalidateVisualInRect(new BasicRectangle(Columns[0].Width - HorizontalScrollBar.Value, offsetY, Frame.Width - Columns[0].Width + HorizontalScrollBar.Value, ListCache.LineHeight));
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
        //        Cache = null;
        //        Refresh();
        //    }
        //}

        #region Rendering Methods

        public override void Render(IGraphicsContext context)
        {
            base.Render(context);
        }

        protected override void DrawRows(IGraphicsContext context)
        {
            base.DrawRows(context);

            // If no songs are playing, set the current now playing rectangle as "empty"
            if (!_nowPlayingSongFound)
                _rectNowPlaying = new BasicRectangle(0, 0, 1, 1);
        }
                
        protected override void DrawRowBackground(IGraphicsContext context, int row, float offsetY)
        {
            // Do not call base as we are changing the behavior of this method.
            // We need to make sure we don't draw the background over the album art column
            var penTransparent = new BasicPen();

            int albumArtColumnWidth = Columns[0].Visible ? Columns[0].Width : 0;
            int lineBackgroundWidth = (int)(Frame.Width + HorizontalScrollBar.Value - albumArtColumnWidth);
            if (VerticalScrollBar.Visible)
                lineBackgroundWidth -= VerticalScrollBar.Width;

            // Draw row background
            var color = GetRowBackgroundColor(row);
            var rectBackground = new BasicRectangle(albumArtColumnWidth - HorizontalScrollBar.Value, offsetY, lineBackgroundWidth, ListCache.LineHeight + 1);
            var brush = new BasicBrush(color);
            context.DrawRectangle(rectBackground, brush, penTransparent);
        }

        protected override BasicColor GetRowBackgroundColor(int row)
        {
            var audioFile = Items[row].AudioFile;
            if (audioFile != null && audioFile.Id == _nowPlayingAudioFileId)
            {
                return ExtendedTheme.NowPlayingBackgroundColor;
            }

            return base.GetRowBackgroundColor(row);
        }

        protected override void DrawCells(IGraphicsContext context, int row, float offsetX, float offsetY)
        {
            base.DrawCells(context, row, offsetX, offsetY);
        }

        protected override void DrawCell(IGraphicsContext context, int row, int col, float offsetX, float offsetY)
        {
            var penTransparent = new BasicPen();
            var column = GridCache.ActiveColumns[col];
            var audioFile = Items[row].AudioFile;
            if (column.Title == "Now Playing")
            {
                // Draw now playing icon
                if (audioFile != null && audioFile.Id == _nowPlayingAudioFileId)
                {
                    // Which size is the minimum? Width or height?                    
                    int availableWidthHeight = column.Width - 4;
                    if (ListCache.LineHeight <= column.Width)
                        availableWidthHeight = ListCache.LineHeight - 4;
                    else
                        availableWidthHeight = column.Width - 4;

                    // Calculate the icon position                                
                    float iconNowPlayingX = ((column.Width - availableWidthHeight) / 2) + offsetX - HorizontalScrollBar.Value;
                    float iconNowPlayingY = offsetY + ((ListCache.LineHeight - availableWidthHeight) / 2);

                    // Create NowPlaying rect (MUST be in integer)                    
                    _rectNowPlaying = new BasicRectangle((int)iconNowPlayingX, (int)iconNowPlayingY, availableWidthHeight, availableWidthHeight);
                    _nowPlayingSongFound = true;

                    // Draw outer circle
                    var brushGradient = new BasicGradientBrush(ExtendedTheme.NowPlayingIndicatorBackgroundColor, ExtendedTheme.NowPlayingIndicatorBackgroundColor, _timerAnimationNowPlayingCount % 360);
                    context.DrawEllipsis(_rectNowPlaying, brushGradient, penTransparent);

                    // Draw inner circle
                    var rect = new BasicRectangle((int)iconNowPlayingX + 4, (int)iconNowPlayingY + 4, availableWidthHeight - 8, availableWidthHeight - 8);
                    var brush = new BasicBrush(ExtendedTheme.NowPlayingBackgroundColor);
                    context.DrawEllipsis(rect, brush, penTransparent);
                }
            }
            else if (column.Title == "Album Cover")
            {
                DrawAlbumCoverZone(context, row);
            }
            else if (audioFile != null)
            {
                string value = GetCellContent(row, col, column.FieldName);

                //// The last column always take the remaining width
                //int columnWidth = column.Width;
                //if (b == Cache.ActiveColumns.Count - 1)
                //{
                //    // Calculate the remaining width
                //    int columnsWidth = 0;
                //    for (int c = 0; c < Cache.ActiveColumns.Count - 1; c++)
                //    {
                //        columnsWidth += Cache.ActiveColumns[c].Width;
                //    }
                //    //columnWidth = (int) (Frame.Width - columnsWidth + HorizontalScrollBar.Value);
                //}

                // Display text
                var rect = new BasicRectangle(offsetX - HorizontalScrollBar.Value + 2, offsetY + (Theme.Padding / 2), GridCache.ActiveColumns[col].Width, ListCache.LineHeight - Theme.Padding + 2);
                //stringFormat.Trimming = StringTrimming.EllipsisCharacter;
                //stringFormat.Alignment = StringAlignment.Near;

                // Use bold for ArtistName and DiscTrackNumber
                if (column.FieldName == "ArtistName" || column.FieldName == "DiscTrackNumber")
                    context.DrawText(value, rect, Theme.TextColor, Theme.FontNameBold, Theme.FontSize);
                else
                    context.DrawText(value, rect, Theme.TextColor, Theme.FontName, Theme.FontSize);
            }
        }

        protected override string GetCellContent(int row, int col, string fieldName)
        {
            // Print value depending on type
            var audioFile = Items[row].AudioFile;
            if (audioFile != null)
            {
                var propertyInfo = audioFile.GetType().GetProperty(fieldName);
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

                    return value;
                }
            }

            return string.Empty;
        }

        protected override void DrawHeader(IGraphicsContext context)
        {
            base.DrawHeader(context);
        }

        protected override void DrawDebugInformation(IGraphicsContext context)
        {
            base.DrawDebugInformation(context);
        }

        private void DrawAlbumCoverZone(IGraphicsContext context, int row)
        {
            var pen = new BasicPen();
            var penTransparent = new BasicPen();
            var brushGradient = new BasicGradientBrush();
            var item = Items[row];
            //string albumTitle = audioFile != null ? audioFile.AlbumTitle : state.CurrentAlbumTitle; // if this is an empty row, keep last album title

            // Check for an album title change (or the last item of the grid)
            if (_currentAlbumArtKey == item.AlbumArtKey)
                return;

            _currentAlbumArtKey = item.AlbumArtKey;

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
            int scrollbarOffsetY = (StartLineNumber * ListCache.LineHeight) - VerticalScrollBar.Value;
            int y = ((albumCoverStartIndex - StartLineNumber) * ListCache.LineHeight) + ListCache.LineHeight + scrollbarOffsetY;

            // Calculate the height of the album cover zone (+1 on end index because the array is zero-based)
            int linesToCover = Math.Min(MinimumRowsPerAlbum, (albumCoverEndIndex + 1 - albumCoverStartIndex));
            int albumCoverZoneHeight = linesToCover * ListCache.LineHeight;
            int heightWithPadding = Math.Min(albumCoverZoneHeight - (Theme.Padding * 2), GridCache.ActiveColumns[0].Width - (Theme.Padding * 2));

            // Make sure the height is at least zero (not necessary to draw anything!)
            if (albumCoverZoneHeight > 0)
            {
                // Draw album cover background
                var rectAlbumCover = new BasicRectangle(0 - HorizontalScrollBar.Value, y, GridCache.ActiveColumns[0].Width, albumCoverZoneHeight);
                brushGradient = new BasicGradientBrush(ExtendedTheme.AlbumCoverBackgroundColor, ExtendedTheme.AlbumCoverBackgroundColor, 90);
                context.DrawRectangle(rectAlbumCover, brushGradient, penTransparent);

                // Measure available width for text
                int widthAvailableForText = Columns[0].Width - (Theme.Padding * 2);

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
                try
                {
                    //Console.WriteLine("SongGridViewControl - Getting album art from cache - artistName: {0} albumTitle: {1}", audioFile.ArtistName, audioFile.AlbumTitle);
                    imageAlbumCover = _albumArtCacheService.GetAlbumArt(audioFile.ArtistName, audioFile.AlbumTitle);
                }
                catch (Exception ex)
                {
                    Tracing.Log(ex);
                }

                // Album art not found in cache; try to find an album cover in one of the file
                // If the album art cannot be extracted, this is called over and over again... we need to be able to add "null" the album art cache...
                if (imageAlbumCover == null)
                {
                    try
                    {
                        //Console.WriteLine("SongGridViewControl - Requesting new album art - artistName: {0} albumTitle: {1}", audioFile.ArtistName, audioFile.AlbumTitle);
                        _albumArtRequestService.RequestAlbumArt(new AlbumArtRequest(){
                            ArtistName = audioFile.ArtistName,
                            AlbumTitle = audioFile.AlbumTitle,
                            AudioFilePath = audioFile.FilePath,
                            Width = heightWithPadding,
                            Height = heightWithPadding,
                            UserData = row
                        });
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("SongGridViewControl - Failed to request image: {0}" , ex);
                    }
                }

                // There are at least two lines; is there an album cover to display?
                if (imageAlbumCover == null)
                {
                    // There is no album cover to display; display only text.
                    // Set string format
                    //stringFormat.Alignment = StringAlignment.Center;
                    //stringFormat.Trimming = StringTrimming.EllipsisWord;

                    sizeArtistName = context.MeasureText(audioFile.ArtistName, new BasicRectangle(0, 0, widthAvailableForText, heightWithPadding), ExtendedTheme.FontNameAlbumArtTitle, Theme.FontSize + 2);
                    sizeAlbumTitle = context.MeasureText(audioFile.AlbumTitle, new BasicRectangle(0, 0, widthAvailableForText, heightWithPadding), ExtendedTheme.FontNameAlbumArtSubtitle, Theme.FontSize);

                    // Display the album title at the top of the zome
                    rectArtistNameText = new BasicRectangle(Theme.Padding - HorizontalScrollBar.Value, y + Theme.Padding, widthAvailableForText, heightWithPadding);
                    rectAlbumTitleText = new BasicRectangle(Theme.Padding - HorizontalScrollBar.Value, y + Theme.Padding + sizeArtistName.Height, widthAvailableForText, heightWithPadding);
                    displayArtistNameAndAlbumTitle = true;
                    useAlbumArtOverlay = true;
                }
                else
                {
                    // There is an album cover to display with more than 2 lines.
                    // Set string format
                    //stringFormat.Alignment = StringAlignment.Near;
                    //stringFormat.Trimming = StringTrimming.EllipsisWord;

                    float widthRemainingForText = Columns[0].Width - (Theme.Padding * 3) - heightWithPadding;
                    sizeArtistName = context.MeasureText(audioFile.ArtistName, new BasicRectangle(0, 0, widthRemainingForText, heightWithPadding), ExtendedTheme.FontNameAlbumArtTitle, Theme.FontSize + 2);
                    sizeAlbumTitle = context.MeasureText(audioFile.AlbumTitle, new BasicRectangle(0, 0, widthRemainingForText, heightWithPadding), ExtendedTheme.FontNameAlbumArtSubtitle, Theme.FontSize);

                    // Try to center the cover art + padding + max text width
                    //float maxWidth = Math.Max(sizeArtistName.Width, sizeAlbumTitle.Width);
                    float albumCoverX = Theme.Padding - 2; // (Columns[0].Width - heightWithPadding - _theme.Padding - _theme.Padding - maxWidth) / 2;
                    float artistNameY = Theme.Padding + 1; // (albumCoverZoneHeight - sizeArtistName.Height - sizeAlbumTitle.Height) / 2;

                    // Display the album title at the top of the zome
                    rectArtistNameText = new BasicRectangle(albumCoverX + heightWithPadding + Theme.Padding - HorizontalScrollBar.Value, y + artistNameY, widthRemainingForText, heightWithPadding);
                    rectAlbumTitleText = new BasicRectangle(albumCoverX + heightWithPadding + Theme.Padding - HorizontalScrollBar.Value, y + artistNameY + sizeArtistName.Height + (Theme.Padding / 8f), widthRemainingForText, heightWithPadding);
                    rectAlbumCoverArt = new BasicRectangle(albumCoverX - HorizontalScrollBar.Value, y + Theme.Padding, heightWithPadding, heightWithPadding);
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
                    context.DrawText(audioFile.ArtistName, rectArtistNameText, Theme.HeaderTextColor, ExtendedTheme.FontNameAlbumArtTitle, Theme.FontSize + 2);
                    context.DrawText(audioFile.AlbumTitle, rectAlbumTitleText, Theme.HeaderTextColor, ExtendedTheme.FontNameAlbumArtSubtitle, Theme.FontSize);
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

        #endregion

        #region Interaction Methods

        public override void KeyDown(char key, SpecialKeys specialKeys, ModifierKeys modifierKeys, bool isRepeat)
        {
            base.KeyDown(key, specialKeys, modifierKeys, isRepeat);

            var startEndIndexes = GetStartIndexAndEndIndexOfSelectedRows();
            if (specialKeys == SpecialKeys.Enter)
            {
                _nowPlayingAudioFileId = Items[startEndIndexes.Item1].AudioFile.Id;
            }
        }

        public override void KeyUp(char key, SpecialKeys specialKeys, ModifierKeys modifierKeys, bool isRepeat)
        {
            base.KeyUp(key, specialKeys, modifierKeys, isRepeat);
        }

        public override void MouseWheel(float delta)
        {
            base.MouseWheel(delta);
        }

        public override void MouseEnter()
        {
            base.MouseEnter();
        }

        public override void MouseLeave()
        {
            base.MouseLeave();
        }

        public override void MouseDown(float x, float y, MouseButtonType button, KeysHeld keysHeld)
        {
            base.MouseDown(x, y, button, keysHeld);              
        }

        public override void MouseUp(float x, float y, MouseButtonType button, KeysHeld keysHeld)
        {
            base.MouseUp(x, y, button, keysHeld);
        }

        public override void MouseClick(float x, float y, MouseButtonType button, KeysHeld keysHeld)
        {
            base.MouseClick(x, y, button, keysHeld);
        }

        public override void MouseDoubleClick(float x, float y, MouseButtonType button, KeysHeld keysHeld)
        {
            base.MouseDoubleClick(x, y, button, keysHeld);

            if (Columns == null || GridCache == null)
                return;

            var partialRect = new BasicRectangle();
            bool controlNeedsToBePartiallyInvalidated = false;
            int albumArtCoverWidth = Columns[0].Visible ? Columns[0].Width : 0;
            int scrollbarOffsetY = (StartLineNumber * ListCache.LineHeight) - VerticalScrollBar.Value;

            // Keep original songId in case the now playing value is set before invalidating the older value
            Guid originalId = _nowPlayingAudioFileId;

            // Loop through visible lines
            for (int a = StartLineNumber; a < StartLineNumber + NumberOfLinesToDraw; a++)
            {
                if (Items[a].IsMouseOverItem)
                {
                    // Set this item as the new now playing
                    _nowPlayingAudioFileId = Items[a].AudioFile.Id;

                    ItemDoubleClick(a);
                    var newPartialRect = new BasicRectangle(albumArtCoverWidth - HorizontalScrollBar.Value, ((a - StartLineNumber + 1) * ListCache.LineHeight) + scrollbarOffsetY, Frame.Width - albumArtCoverWidth + HorizontalScrollBar.Value, ListCache.LineHeight);
                    partialRect.Merge(newPartialRect);
                    controlNeedsToBePartiallyInvalidated = true;
                }
                else if (Items[a].AudioFile != null && Items[a].AudioFile.Id == originalId)
                {
                    var newPartialRect = new BasicRectangle(albumArtCoverWidth - HorizontalScrollBar.Value, ((a - StartLineNumber + 1) * ListCache.LineHeight) + scrollbarOffsetY, Frame.Width - albumArtCoverWidth + HorizontalScrollBar.Value, ListCache.LineHeight);
                    partialRect.Merge(newPartialRect);
                    controlNeedsToBePartiallyInvalidated = true;
                }
            }
                
            if (controlNeedsToBePartiallyInvalidated)
                InvalidateVisualInRect(partialRect);
        }        

        public override void MouseMove(float x, float y, MouseButtonType button)
        {
            base.MouseMove(x, y, button);
        }

        #endregion

        /// <summary>
        /// Invalidates a row (useful for updating currently playing song).
        /// </summary>
        /// <param name="oldAudioFileId">Old audio file identifier.</param>
        /// <param name="newAudioFileId">New audio file identifier.</param>
        private void InvalidateRow(Guid oldAudioFileId, Guid newAudioFileId)
        {
            if (Items == null || GridCache == null)
                return;

            int oldIndex = Items.FindIndex(x => x.AudioFile != null && x.AudioFile.Id == oldAudioFileId);
            int newIndex = Items.FindIndex(x => x.AudioFile != null && x.AudioFile.Id == newAudioFileId);
            int scrollbarOffsetY = (StartLineNumber * ListCache.LineHeight) - VerticalScrollBar.Value;
            int firstIndex = oldIndex < newIndex ? oldIndex : newIndex;
            int secondIndex = newIndex > oldIndex ? newIndex : oldIndex;

            int firstY = -1;
            if (oldIndex >= StartLineNumber && oldIndex <= StartLineNumber + NumberOfLinesToDraw)
                firstY = ((firstIndex - StartLineNumber) * ListCache.LineHeight) + scrollbarOffsetY;

            int secondY = -1;
            if (newIndex >= StartLineNumber && newIndex <= StartLineNumber + NumberOfLinesToDraw)
                secondY = ((secondIndex - StartLineNumber) * ListCache.LineHeight) + scrollbarOffsetY;

            int finalY = 0;
            int finalHeight = 0;
            int lineHeight = ListCache.LineHeight;

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
                int headerHeight = ListCache.LineHeight;
                var rect = new BasicRectangle(0, finalY + headerHeight, Frame.Width, finalHeight);
                //Console.WriteLine("SongGridViewControl - InvalidateRow - rect: {0} nowPlayingIndex: {1} startLineNumber: {2} numberOfLinesToDraw: {3} scrollbarOffsetY: {4}", rect, oldIndex, _startLineNumber, _numberOfLinesToDraw, scrollbarOffsetY);
                InvalidateVisualInRect(rect);
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
    }
}
