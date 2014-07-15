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

namespace Sessions.GenericControls.Controls.Songs
{
    /// <summary>
    /// Theme object for the SongGridView control.
    /// </summary>
    public class SongGridViewTheme
    {
        public BasicColor BackgroundColor { get; set; }
        public BasicColor SelectedBackgroundColor { get; set; }
        public BasicColor MouseOverBackgroundColor { get; set; }
        public BasicColor NowPlayingBackgroundColor { get; set; }
        public BasicColor NowPlayingIndicatorBackgroundColor { get; set; }
        public BasicColor AlbumCoverBackgroundColor { get; set; }

        public BasicColor HeaderBackgroundColor { get; set; }
        public BasicColor MouseOverHeaderBackgroundColor { get; set; }

        public BasicColor TextColor { get; set; }
        public BasicColor HeaderTextColor { get; set; }

        public string FontName { get; set; }
        public string FontNameBold { get; set; }
        public string FontNameAlbumArtTitle { get; set; }
        public string FontNameAlbumArtSubtitle { get; set; }
        public float FontSize { get; set; }
        public int Padding { get; set; }

        public SongGridViewTheme()
        {
            //gridViewSongs.Theme.AlbumCoverBackgroundGradient = new BackgroundGradient(System.Drawing.Color.FromArgb(255, 36, 47, 53), System.Drawing.Color.FromArgb(255, 36, 47, 53), LinearGradientMode.Horizontal, System.Drawing.Color.Gray, 0);
            //gridViewSongs.Theme.HeaderHoverTextGradient = new TextGradient(System.Drawing.Color.FromArgb(255, 69, 88, 101), System.Drawing.Color.FromArgb(255, 69, 88, 101), LinearGradientMode.Horizontal, System.Drawing.Color.Gray, 0, fontHeader);
            //gridViewSongs.Theme.HeaderTextGradient = new TextGradient(System.Drawing.Color.FromArgb(255, 69, 88, 101), System.Drawing.Color.FromArgb(255, 69, 88, 101), LinearGradientMode.Horizontal, System.Drawing.Color.Gray, 0, fontHeader);
            //gridViewSongs.Theme.IconNowPlayingGradient = new Gradient(System.Drawing.Color.FromArgb(255, 250, 200, 250), System.Drawing.Color.FromArgb(255, 25, 150, 25), LinearGradientMode.Horizontal);
            //gridViewSongs.Theme.RowNowPlayingTextGradient = new TextGradient(System.Drawing.Color.FromArgb(255, 135, 235, 135), System.Drawing.Color.FromArgb(255, 135, 235, 135), LinearGradientMode.Horizontal, System.Drawing.Color.Gray, 0, fontRow);
            //gridViewSongs.Theme.RowTextGradient = new TextGradient(System.Drawing.Color.White, System.Drawing.Color.White, LinearGradientMode.Horizontal, System.Drawing.Color.Gray, 0, fontRow);

            Padding = 6;
            BackgroundColor = new BasicColor(255, 255, 255);
            SelectedBackgroundColor = new BasicColor(200, 200, 200);
            MouseOverBackgroundColor = new BasicColor(255, 255, 255);
            NowPlayingBackgroundColor = new BasicColor(135, 235, 135);
            NowPlayingIndicatorBackgroundColor = new BasicColor(25, 150, 25);
            AlbumCoverBackgroundColor = new BasicColor(32, 40, 46);

            //HeaderBackgroundColor = new BasicColor(87, 107, 120); //new BasicColor(69, 88, 101);
            HeaderBackgroundColor = new BasicColor(69, 88, 101);
            MouseOverHeaderBackgroundColor = new BasicColor(104, 121, 133);

            TextColor = new BasicColor(0, 0, 0);
            HeaderTextColor = new BasicColor(255, 255, 255);

            FontName = "Roboto";
            FontNameBold = "Roboto Medium";
            FontNameAlbumArtTitle = "Roboto";
            FontNameAlbumArtSubtitle = "Roboto Light";
            FontSize = 11f;
        }
    }
}
