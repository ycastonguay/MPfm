//
// MPfmControlsConfig.cs: Class containing all control settings for MPfm.
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

namespace MPfm.MVP.Config
{
    /// <summary>
    /// Class containing all control settings for MPfm.
    /// </summary>
    public class MPfmControlsConfig
    {
        public MPfmTableViewConfig TableViewSongBrowser { get; private set; }
        public MPfmTableViewConfig TableViewPlaylistBrowser { get; private set; }

        public MPfmControlsConfig()
        {
            // Set defaults
            TableViewSongBrowser = new MPfmTableViewConfig();
            TableViewPlaylistBrowser = new MPfmTableViewConfig();

            // Add default columns for Song Browser
            TableViewSongBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "IsPlaying",
                FieldName = "IsPlaying",
                Order = 0,
                Width = 20
            });
            TableViewSongBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "FileType",
                FieldName = "Type",
                Order = 1,
                Width = 40,
                IsVisible = false
            });
            TableViewSongBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "DiscTrackNumber",
                FieldName = "Tr#",
                Order = 2,
                Width = 30
            });
            TableViewSongBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "TrackCount",
                FieldName = "Track Count",
                Order = 3,
                Width = 30,
                IsVisible = false
            });
            TableViewSongBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "File Path",
                FieldName = "FilePath",
                Order = 4,
                Width = 200,
                IsVisible = false
            });
            TableViewSongBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "Song Title",
                FieldName = "Title",
                Order = 5,
                Width = 200
            });
            TableViewSongBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "Length",
                FieldName = "Length",
                Order = 6,
                Width = 70
            });
            TableViewSongBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "Artist Name",
                FieldName = "ArtistName",
                Order = 7,
                Width = 140
            });
            TableViewSongBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "Album Title",
                FieldName = "AlbumTitle",
                Order = 8,
                Width = 140
            });
            TableViewSongBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "Genre",
                FieldName = "Genre",
                Order = 9,
                Width = 140,
                IsVisible = false
            });
            TableViewSongBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "Play Count",
                FieldName = "PlayCount",
                Order = 10,
                Width = 50
            });
            TableViewSongBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "Last Played",
                FieldName = "LastPlayed",
                Order = 11,
                Width = 80
            });
            TableViewSongBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "Bitrate",
                FieldName = "Bitrate",
                Order = 12,
                Width = 50,
                IsVisible = false
            });
            TableViewSongBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "Sample Rate",
                FieldName = "SampleRate",
                Order = 13,
                Width = 50,
                IsVisible = false
            });
            TableViewSongBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "Tempo",
                FieldName = "Tempo",
                Order = 14,
                Width = 50,
                IsVisible = false
            });
            TableViewSongBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "Year",
                FieldName = "Year",
                Order = 15,
                Width = 50,
                IsVisible = false
            });

            // Add default columns for Playlist Browser
            TableViewPlaylistBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "IsPlaying",
                FieldName = "IsPlaying",
                Order = 0,
                Width = 20
            });
            TableViewPlaylistBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "FileType",
                FieldName = "Type",
                Order = 1,
                Width = 40,
                IsVisible = false
            });
            TableViewPlaylistBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "DiscTrackNumber",
                FieldName = "Tr#",
                Order = 2,
                Width = 30
            });
            TableViewPlaylistBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "TrackCount",
                FieldName = "Track Count",
                Order = 3,
                Width = 30,
                IsVisible = false
            });
            TableViewPlaylistBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "File Path",
                FieldName = "FilePath",
                Order = 4,
                Width = 200,
                IsVisible = false
            });
            TableViewPlaylistBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "Song Title",
                FieldName = "Title",
                Order = 5,
                Width = 200
            });
            TableViewPlaylistBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "Length",
                FieldName = "Length",
                Order = 6,
                Width = 70
            });
            TableViewPlaylistBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "Artist Name",
                FieldName = "ArtistName",
                Order = 7,
                Width = 140
            });
            TableViewPlaylistBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "Album Title",
                FieldName = "AlbumTitle",
                Order = 8,
                Width = 140
            });
            TableViewPlaylistBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "Genre",
                FieldName = "Genre",
                Order = 9,
                Width = 140,
                IsVisible = false
            });
            TableViewPlaylistBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "Play Count",
                FieldName = "PlayCount",
                Order = 10,
                Width = 50
            });
            TableViewPlaylistBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "Last Played",
                FieldName = "LastPlayed",
                Order = 11,
                Width = 80
            });
            TableViewPlaylistBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "Bitrate",
                FieldName = "Bitrate",
                Order = 12,
                Width = 50,
                IsVisible = false
            });
            TableViewPlaylistBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "Sample Rate",
                FieldName = "SampleRate",
                Order = 13,
                Width = 50,
                IsVisible = false
            });
            TableViewPlaylistBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "Tempo",
                FieldName = "Tempo",
                Order = 14,
                Width = 50,
                IsVisible = false
            });
            TableViewPlaylistBrowser.Columns.Add(new MPfmTableViewColumnConfig() {
                Title = "Year",
                FieldName = "Year",
                Order = 15,
                Width = 50,
                IsVisible = false
            });

        }
    }
}
