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

using Sessions.GenericControls.Basics;

namespace Sessions.GenericControls.Controls.Themes
{
    public class PlaylistListViewTheme
    {
        public float Padding { get; set; }

        public BasicColor BackgroundColor { get; set; }
        public BasicColor SelectedBackgroundColor { get; set; }
        public BasicColor NowPlayingBackgroundColor { get; set; }
        public BasicColor NowPlayingIndicatorBackgroundColor { get; set; }

        public string ArtistNameAlbumTitleFontName { get; set; }
        public int ArtistNameAlbumTitleFontSize { get; set; }
        public BasicColor ArtistNameAlbumTitleTextColor { get; set; }
        public string SongTitleFontName { get; set; }
        public int SongTitleFontSize { get; set; }
        public BasicColor SongTitleTextColor { get; set; }
        public string LengthFontName { get; set; }
        public int LengthFontSize { get; set; }
        public BasicColor LengthTextColor { get; set; }

        public PlaylistListViewTheme()
        {
            Padding = 3;

            BackgroundColor = new BasicColor(32, 40, 46);
            SelectedBackgroundColor = new BasicColor(64, 74, 82);
            NowPlayingBackgroundColor = new BasicColor(135, 235, 135);
            NowPlayingIndicatorBackgroundColor = new BasicColor(25, 150, 25);

            ArtistNameAlbumTitleFontName = "Roboto Light";
            ArtistNameAlbumTitleFontSize = 11;
            ArtistNameAlbumTitleTextColor = new BasicColor(255, 255, 255);
            SongTitleFontName = "Roboto";
            SongTitleFontSize = 10;
            SongTitleTextColor = new BasicColor(230, 230, 230);
            LengthFontName = "Roboto";
            LengthFontSize = 9;
            LengthTextColor = new BasicColor(200, 200, 200);
        }
    }
}
