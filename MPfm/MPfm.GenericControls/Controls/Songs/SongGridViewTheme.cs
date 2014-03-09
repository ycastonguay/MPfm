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

using System.Net.Mime;
using System.Web.UI.WebControls;
using MPfm.GenericControls.Basics;

namespace MPfm.GenericControls.Controls.Songs
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

            HeaderBackgroundColor = new BasicColor(69, 88, 101);
            MouseOverHeaderBackgroundColor = new BasicColor(92, 109, 120);

            TextColor = new BasicColor(0, 0, 0);
            HeaderTextColor = new BasicColor(255, 255, 255);

            FontName = "Roboto";
            FontNameBold = "Roboto Bold";
            FontSize = 11f;
        }
    }
}
