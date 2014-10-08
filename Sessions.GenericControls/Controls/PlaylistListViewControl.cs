// Copyright © 2011-2013 Yanick Castonguay
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
using Sessions.Sound.Playlists;

namespace Sessions.GenericControls.Controls
{
    /// <summary>
    /// This custom grid view control displays the Sessions library.
    /// </summary>
    public class PlaylistListViewControl : ListViewControlBase<PlaylistListViewItem>
    {
        private readonly IAlbumArtRequestService _albumArtRequestService;
        private readonly IAlbumArtCacheService _albumArtCacheService;
        private Playlist _playlist;

        public PlaylistListViewTheme ExtendedTheme { get; set; }
            
        /// <summary>
        /// Default constructor for SongGridView.
        /// </summary>
        public PlaylistListViewControl(IHorizontalScrollBarWrapper horizontalScrollBar, IVerticalScrollBarWrapper verticalScrollBar, 
                        IAlbumArtRequestService albumArtRequestService, IAlbumArtCacheService albumArtCacheService) :
            base(horizontalScrollBar, verticalScrollBar)
        {
            _albumArtCacheService = albumArtCacheService;
            _albumArtRequestService = albumArtRequestService;
            _albumArtRequestService.OnAlbumArtExtracted += HandleOnAlbumArtExtracted;
            ExtendedTheme = new PlaylistListViewTheme();
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

        public void SetPlaylist(Playlist playlist)
        {
            _playlist = playlist;

            InvalidateCache();
            VerticalScrollBar.Value = 0;
            InvalidateVisual();
        }

        protected override int GetRowCount()
        {
            return _playlist == null ? 0 : _playlist.Items.Count;
        }

        protected override bool IsRowEmpty(int row)
        {
            return false;
        }

        protected override bool IsRowSelectable(int row)
        {
            return true;
        }

        protected override int GetRowHeight()
        {
            return 40;
        }

        protected override bool ShouldDrawHeader()
        {
            return false;
        }

        #region Rendering Methods

        public override void Render(IGraphicsContext context)
        {
            base.Render(context);
        }

        protected override void DrawRows(IGraphicsContext context)
        {
            base.DrawRows(context);
        }
                
        protected override void DrawRowBackground(IGraphicsContext context, int row, float offsetY)
        {
            // Do not call base as we are changing the behavior of this method.
            // We need to make sure we don't draw the background over the album art column
            var penTransparent = new BasicPen();

            int lineBackgroundWidth = (int)(Frame.Width + HorizontalScrollBar.Value);
            if (VerticalScrollBar.Visible)
                lineBackgroundWidth -= VerticalScrollBar.Width;

            // Draw row background
            var color = GetRowBackgroundColor(row);
            var rectBackground = new BasicRectangle(HorizontalScrollBar.Value, offsetY, lineBackgroundWidth, ListCache.LineHeight + 1);
            var brush = new BasicBrush(color);
            context.DrawRectangle(rectBackground, brush, penTransparent);
        }

        protected override BasicColor GetRowBackgroundColor(int row)
        {
            return _playlist.CurrentItemIndex == row ? ExtendedTheme.NowPlayingBackgroundColor : ExtendedTheme.BackgroundColor;
        }

        protected override void DrawCells(IGraphicsContext context, int row, float offsetX, float offsetY)
        {
            base.DrawCells(context, row, offsetX, offsetY);
        }

        protected override void DrawCell(IGraphicsContext context, int row, int col, float offsetX, float offsetY)
        {
            int heightWithPadding = GetRowHeight() - Theme.Padding / 2;
            var audioFile = _playlist.Items[row].AudioFile;
            //Console.WriteLine("PlaylistListView - DrawCell - row: {0}", row);

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
                    _albumArtRequestService.RequestAlbumArt(new AlbumArtRequest()
                    {
                        ArtistName = audioFile.ArtistName,
                        AlbumTitle = audioFile.AlbumTitle,
                        AudioFilePath = audioFile.FilePath,
                        Width = heightWithPadding,
                        Height = heightWithPadding,
                        UserData = row
                    });
                } 
                catch (Exception ex)
                {
                    Console.WriteLine("SongGridViewControl - Failed to request image: {0}", ex);
                }
            }

            // Display album cover
            var rectAlbumCoverArt = new BasicRectangle(Theme.Padding, offsetY + Theme.Padding, heightWithPadding - Theme.Padding * 2, heightWithPadding - Theme.Padding * 2);
            if (imageAlbumCover != null)
            {
                context.DrawImage(rectAlbumCoverArt, new BasicRectangle(0, 0, imageAlbumCover.ImageSize.Width, imageAlbumCover.ImageSize.Height), imageAlbumCover.Image);
            }

            float textX = imageAlbumCover != null ? offsetX + Theme.Padding + heightWithPadding : offsetX + Theme.Padding;
            var rectArtistName = new BasicRectangle(textX, offsetY + (Theme.Padding / 4), Frame.Width, 16);
            var rectAlbumTitle = new BasicRectangle(textX, rectArtistName.Bottom + (Theme.Padding / 4), Frame.Width, 14);
            var rectSongTitle = new BasicRectangle(textX, rectAlbumTitle.Bottom + (Theme.Padding / 4), Frame.Width, 12);
            context.DrawText(audioFile.ArtistName, rectArtistName, ExtendedTheme.TextColor, ExtendedTheme.ArtistNameFontName, ExtendedTheme.ArtistNameFontSize);
            context.DrawText(audioFile.AlbumTitle, rectAlbumTitle, ExtendedTheme.TextColor, ExtendedTheme.AlbumTitleFontName, ExtendedTheme.AlbumTitleFontSize);
            context.DrawText(audioFile.Title, rectSongTitle, ExtendedTheme.TextColor, ExtendedTheme.SongTitleFontName, ExtendedTheme.SongTitleFontSize);
        }

        protected override string GetCellContent(int row, int col, string fieldName)
        {
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

        #endregion

        #region Interaction Methods

        public override void KeyDown(char key, SpecialKeys specialKeys, ModifierKeys modifierKeys, bool isRepeat)
        {
            base.KeyDown(key, specialKeys, modifierKeys, isRepeat);

            var startEndIndexes = GetStartIndexAndEndIndexOfSelectedRows();
            if (specialKeys == SpecialKeys.Enter)
            {
                //_nowPlayingAudioFileId = Items[startEndIndexes.Item1].AudioFile.Id;
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

            var partialRect = new BasicRectangle();
            bool controlNeedsToBePartiallyInvalidated = false;
            int scrollbarOffsetY = (StartLineNumber * ListCache.LineHeight) - VerticalScrollBar.Value;

            // Keep original songId in case the now playing value is set before invalidating the older value
            //Guid originalId = _nowPlayingAudioFileId;
            Guid originalId = _playlist.CurrentItem.Id;

            // Loop through visible lines
            for (int a = StartLineNumber; a < StartLineNumber + NumberOfLinesToDraw; a++)
            {
                if (MouseOverRowIndex == a)
                {
                    // Set this item as the new now playing
                    //_nowPlayingAudioFileId = Items[a].AudioFile.Id;

                    ItemDoubleClick(a);
                    var newPartialRect = new BasicRectangle(HorizontalScrollBar.Value, ((a - StartLineNumber + 1) * ListCache.LineHeight) + scrollbarOffsetY, Frame.Width + HorizontalScrollBar.Value, ListCache.LineHeight);
                    partialRect.Merge(newPartialRect);
                    controlNeedsToBePartiallyInvalidated = true;
                }
                //else if (Items[a].AudioFile != null && Items[a].AudioFile.Id == originalId)
                else if(_playlist.Items[a].AudioFile != null && _playlist.Items[a].Id == originalId)
                {
                    var newPartialRect = new BasicRectangle(HorizontalScrollBar.Value, ((a - StartLineNumber + 1) * ListCache.LineHeight) + scrollbarOffsetY, Frame.Width + HorizontalScrollBar.Value, ListCache.LineHeight);
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
            if (ListCache == null)
                return;

            int oldIndex = _playlist.Items.FindIndex(x => x.AudioFile != null && x.AudioFile.Id == oldAudioFileId);
            int newIndex = _playlist.Items.FindIndex(x => x.AudioFile != null && x.AudioFile.Id == newAudioFileId);
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
    }
}
