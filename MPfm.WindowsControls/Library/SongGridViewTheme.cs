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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// Theme object for the SongGridView control.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SongGridViewTheme
    {
        #region Header

        /// <summary>
        /// Private value for the HeaderTextGradient property.
        /// </summary>
        private TextGradient headerTextGradient = new TextGradient(Color.FromArgb(165, 165, 165), Color.FromArgb(195, 195, 195), LinearGradientMode.Horizontal, 
                                                                   Color.Gray, 0, new CustomFont("Junction", 8.0f, Color.FromArgb(60, 60, 60)));
        /// <summary>
        /// Defines the header text gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Header"), Browsable(true), Description("Header text gradient.")]
        public TextGradient HeaderTextGradient
        {
            get
            {
                return headerTextGradient;
            }
            set
            {
                headerTextGradient = value;
            }
        }

        /// <summary>
        /// Private value for the HeaderHoverTextGradient property.
        /// </summary>
        private TextGradient headerHoverTextGradient = new TextGradient(Color.FromArgb(145, 145, 145), Color.FromArgb(175, 175, 175), LinearGradientMode.Horizontal,
                                                                        Color.Gray, 0, new CustomFont("Junction", 8.0f, Color.FromArgb(60, 60, 60)));
        /// <summary>
        /// Defines the header text gradient (when the mouse cursor is over the header).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Header"), Browsable(true), Description("Header text gradient (when the mouse cursor is over the header).")]
        public TextGradient HeaderHoverTextGradient
        {
            get
            {
                return headerHoverTextGradient;
            }
            set
            {
                headerHoverTextGradient = value;
            }
        }

        #endregion

        #region Row

        /// <summary>
        /// Private value for the RowTextGradient property.
        /// </summary>
        private TextGradient rowTextGradient = new TextGradient(Color.FromArgb(215, 215, 215), Color.FromArgb(235, 235, 235), LinearGradientMode.Horizontal, 
                                                                Color.Gray, 0, new CustomFont("Junction", 8.0f, Color.FromArgb(0, 0, 0)));
        /// <summary>
        /// Defines the row text gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Row"), Browsable(true), Description("Row text gradient.")]
        public TextGradient RowTextGradient
        {
            get
            {
                return rowTextGradient;
            }
            set
            {
                rowTextGradient = value;
            }
        }

        /// <summary>
        /// Private value for the RowNowPlayingTextGradient property.
        /// </summary>
        private TextGradient rowNowPlayingTextGradient = new TextGradient(Color.FromArgb(135, 235, 135), Color.FromArgb(155, 255, 155), LinearGradientMode.Horizontal,
                                                                          Color.Gray, 0, new CustomFont("Junction", 8.0f, Color.FromArgb(0, 0, 0)));
        /// <summary>
        /// Defines the row text gradient (when the song is currently playing).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Row"), Browsable(true), Description("Row text gradient (when the song is currently playing).")]
        public TextGradient RowNowPlayingTextGradient
        {
            get
            {
                return rowNowPlayingTextGradient;
            }
            set
            {
                rowNowPlayingTextGradient = value;
            }
        }

        /// <summary>
        /// Private value for the IconNowPlayingGradient property.
        /// </summary>
        private Gradient iconNowPlayingGradient = new Gradient(Color.FromArgb(250, 200, 250), Color.FromArgb(25, 150, 25), LinearGradientMode.Horizontal);
        /// <summary>
        /// Defines the icon gradient next to the song currently playing.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Row"), Browsable(true), Description("Icon gradient next to the song currently playing.")]
        public Gradient IconNowPlayingGradient
        {
            get
            {
                return iconNowPlayingGradient;
            }
            set
            {
                iconNowPlayingGradient = value;
            }
        }

        #endregion

        /// <summary>
        /// Private value for the AlbumCoverBackgroundGradient property.
        /// </summary>
        private BackgroundGradient albumCoverBackgroundGradient = new BackgroundGradient(Color.FromArgb(55, 55, 55), Color.FromArgb(75, 75, 75), LinearGradientMode.Horizontal, Color.Gray, 0);
        /// <summary>
        /// Defines the album cover background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("General"), Browsable(true), Description("Album cover background gradient.")]
        public BackgroundGradient AlbumCoverBackgroundGradient
        {
            get
            {
                return albumCoverBackgroundGradient;
            }
            set
            {
                albumCoverBackgroundGradient = value;
            }
        }

        /// <summary>
        /// Private value for the Padding property.
        /// </summary>
        private int padding = 6;
        /// <summary>
        /// Padding used around text and album covers (in pixels).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("General"), Browsable(true), Description("Padding used around text and album covers (in pixels).")]
        public int Padding
        {
            get
            {
                return padding;
            }
            set
            {
                padding = value;                
            }
        }

        /// <summary>
        /// Default constructor for the SongGridViewTheme class.
        /// </summary>
        public SongGridViewTheme()
        {
        }
    }
}
