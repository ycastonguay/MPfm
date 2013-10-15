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

namespace MPfm.MVP.Config
{
    /// <summary>
    /// Class containing all control settings for MPfm.
    /// </summary>
    public class ControlsAppConfig : IAppConfig
    {
        public TableViewAppConfig TableViewAppSongBrowser { get; set; }
        public TableViewAppConfig TableViewAppPlaylistBrowser { get; set; }

        public ControlsAppConfig()
        {
            // Set defaults
            TableViewAppSongBrowser = new TableViewAppConfig();
            TableViewAppPlaylistBrowser = new TableViewAppConfig();

            // Add default columns for Song Browser
            TableViewAppSongBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "IsPlaying",
                FieldName = "IsPlaying",
                Order = 0,
                Width = 20
            });
            TableViewAppSongBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "FileType",
                FieldName = "Type",
                Order = 1,
                Width = 40,
                IsVisible = false
            });
            TableViewAppSongBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "DiscTrackNumber",
                FieldName = "Tr#",
                Order = 2,
                Width = 30
            });
            TableViewAppSongBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "TrackCount",
                FieldName = "Track Count",
                Order = 3,
                Width = 30,
                IsVisible = false
            });
            TableViewAppSongBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "File Path",
                FieldName = "FilePath",
                Order = 4,
                Width = 200,
                IsVisible = false
            });
            TableViewAppSongBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "Song Title",
                FieldName = "Title",
                Order = 5,
                Width = 200
            });
            TableViewAppSongBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "Length",
                FieldName = "Length",
                Order = 6,
                Width = 70
            });
            TableViewAppSongBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "Artist Name",
                FieldName = "ArtistName",
                Order = 7,
                Width = 140
            });
            TableViewAppSongBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "Album Title",
                FieldName = "AlbumTitle",
                Order = 8,
                Width = 140
            });
            TableViewAppSongBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "Genre",
                FieldName = "Genre",
                Order = 9,
                Width = 140,
                IsVisible = false
            });
            TableViewAppSongBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "Play Count",
                FieldName = "PlayCount",
                Order = 10,
                Width = 50
            });
            TableViewAppSongBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "Last Played",
                FieldName = "LastPlayed",
                Order = 11,
                Width = 80
            });
            TableViewAppSongBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "Bitrate",
                FieldName = "Bitrate",
                Order = 12,
                Width = 50,
                IsVisible = false
            });
            TableViewAppSongBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "Sample Rate",
                FieldName = "SampleRate",
                Order = 13,
                Width = 50,
                IsVisible = false
            });
            TableViewAppSongBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "Tempo",
                FieldName = "Tempo",
                Order = 14,
                Width = 50,
                IsVisible = false
            });
            TableViewAppSongBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "Year",
                FieldName = "Year",
                Order = 15,
                Width = 50,
                IsVisible = false
            });

            // Add default columns for Playlist Browser
            TableViewAppPlaylistBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "IsPlaying",
                FieldName = "IsPlaying",
                Order = 0,
                Width = 20
            });
            TableViewAppPlaylistBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "FileType",
                FieldName = "Type",
                Order = 1,
                Width = 40,
                IsVisible = false
            });
            TableViewAppPlaylistBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "DiscTrackNumber",
                FieldName = "Tr#",
                Order = 2,
                Width = 30
            });
            TableViewAppPlaylistBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "TrackCount",
                FieldName = "Track Count",
                Order = 3,
                Width = 30,
                IsVisible = false
            });
            TableViewAppPlaylistBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "File Path",
                FieldName = "FilePath",
                Order = 4,
                Width = 200,
                IsVisible = false
            });
            TableViewAppPlaylistBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "Song Title",
                FieldName = "Title",
                Order = 5,
                Width = 200
            });
            TableViewAppPlaylistBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "Length",
                FieldName = "Length",
                Order = 6,
                Width = 70
            });
            TableViewAppPlaylistBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "Artist Name",
                FieldName = "ArtistName",
                Order = 7,
                Width = 140
            });
            TableViewAppPlaylistBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "Album Title",
                FieldName = "AlbumTitle",
                Order = 8,
                Width = 140
            });
            TableViewAppPlaylistBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "Genre",
                FieldName = "Genre",
                Order = 9,
                Width = 140,
                IsVisible = false
            });
            TableViewAppPlaylistBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "Play Count",
                FieldName = "PlayCount",
                Order = 10,
                Width = 50
            });
            TableViewAppPlaylistBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "Last Played",
                FieldName = "LastPlayed",
                Order = 11,
                Width = 80
            });
            TableViewAppPlaylistBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "Bitrate",
                FieldName = "Bitrate",
                Order = 12,
                Width = 50,
                IsVisible = false
            });
            TableViewAppPlaylistBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "Sample Rate",
                FieldName = "SampleRate",
                Order = 13,
                Width = 50,
                IsVisible = false
            });
            TableViewAppPlaylistBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "Tempo",
                FieldName = "Tempo",
                Order = 14,
                Width = 50,
                IsVisible = false
            });
            TableViewAppPlaylistBrowser.Columns.Add(new TableViewColumnAppConfig() {
                Title = "Year",
                FieldName = "Year",
                Order = 15,
                Width = 50,
                IsVisible = false
            });

        }
    }
}
