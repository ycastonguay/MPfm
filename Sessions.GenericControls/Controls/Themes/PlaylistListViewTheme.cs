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
        public BasicColor BackgroundColor { get; set; }
        public BasicColor NowPlayingBackgroundColor { get; set; }
        public BasicColor NowPlayingIndicatorBackgroundColor { get; set; }
        public BasicColor TextColor { get; set; }

        public string ArtistNameFontName { get; set; }
        public int ArtistNameFontSize { get; set; }
        public string AlbumTitleFontName { get; set; }
        public int AlbumTitleFontSize { get; set; }
        public string SongTitleFontName { get; set; }
        public int SongTitleFontSize { get; set; }

        public PlaylistListViewTheme()
        {
            BackgroundColor = new BasicColor(32, 40, 46);
            NowPlayingBackgroundColor = new BasicColor(135, 235, 135);
            NowPlayingIndicatorBackgroundColor = new BasicColor(25, 150, 25);
            TextColor = new BasicColor(255, 255, 255);

            ArtistNameFontName = "Roboto Light";
            ArtistNameFontSize = 11;
            AlbumTitleFontName = "Roboto";
            AlbumTitleFontSize = 10;
            SongTitleFontName = "Roboto";
            SongTitleFontSize = 9;
        }
    }
}
