//
// SongGridViewTheme.cs: Theme object for the SongGridView control.
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;

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
        /// Private value for the HeaderColor1 property.
        /// </summary>
        private Color headerColor1 = Color.FromArgb(165, 165, 165);
        /// <summary>
        /// First color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Header"), Browsable(true), Description("First color of the header background gradient.")]
        public Color HeaderColor1
        {
            get
            {
                return headerColor1;
            }
            set
            {
                headerColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the HeaderColor2 property.
        /// </summary>
        private Color headerColor2 = Color.FromArgb(195, 195, 195);
        /// <summary>
        /// Second color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Header"), Browsable(true), Description("Second color of the header background gradient.")]
        public Color HeaderColor2
        {
            get
            {
                return headerColor2;
            }
            set
            {
                headerColor2 = value;
            }
        }

        /// <summary>
        /// Private value for the HeaderHoverColor1 property.
        /// </summary>
        private Color headerHoverColor1 = Color.FromArgb(145, 145, 145);
        /// <summary>
        /// First color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Header"), Browsable(true), Description("First color of the header background gradient when the mouse cursor is over the header.")]
        public Color HeaderHoverColor1
        {
            get
            {
                return headerHoverColor1;
            }
            set
            {
                headerHoverColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the HeaderHoverColor2 property.
        /// </summary>
        private Color headerHoverColor2 = Color.FromArgb(175, 175, 175);
        /// <summary>
        /// Second color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Header"), Browsable(true), Description("First color of the header background gradient when the mouse cursor is over the header.")]
        public Color HeaderHoverColor2
        {
            get
            {
                return headerHoverColor2;
            }
            set
            {
                headerHoverColor2 = value;
            }
        }

        /// <summary>
        /// Private value for the HeaderForeColor property.
        /// </summary>
        private Color headerForeColor = Color.FromArgb(60, 60, 60);
        /// <summary>
        /// Fore font color used in the header.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("")]
        [Category("Header"), Browsable(true), Description("Fore color used when drawing the header font and other glyphs (such as the orderby icon).")]
        public Color HeaderForeColor
        {
            get
            {
                return headerForeColor;
            }
            set
            {
                headerForeColor = value;
            }
        }

        #endregion

        #region Line

        /// <summary>
        /// Private value for the LineColor1 property.
        /// </summary>
        private Color lineColor1 = Color.FromArgb(215, 215, 215);
        /// <summary>
        /// First color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Line"), Browsable(true), Description("First color of the line background gradient.")]
        public Color LineColor1
        {
            get
            {
                return lineColor1;
            }
            set
            {
                lineColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the LineColor2 property.
        /// </summary>
        private Color lineColor2 = Color.FromArgb(235, 235, 235);
        /// <summary>
        /// Second color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Line"), Browsable(true), Description("Second color of the line background gradient.")]
        public Color LineColor2
        {
            get
            {
                return lineColor2;
            }
            set
            {
                lineColor2 = value;
            }
        }

        /// <summary>
        /// Private value for the LineNowPlayingColor1 property.
        /// </summary>
        private Color lineNowPlayingColor1 = Color.FromArgb(135, 235, 135);
        /// <summary>
        /// First color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Line"), Browsable(true), Description("First color of the line background gradient when the line is now playing.")]
        public Color LineNowPlayingColor1
        {
            get
            {
                return lineNowPlayingColor1;
            }
            set
            {
                lineNowPlayingColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the LineNowPlayingColor2 property.
        /// </summary>
        private Color lineNowPlayingColor2 = Color.FromArgb(155, 255, 155);
        /// <summary>
        /// Second color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Line"), Browsable(true), Description("Second color of the line background gradient when the line is now playing.")]
        public Color LineNowPlayingColor2
        {
            get
            {
                return lineNowPlayingColor2;
            }
            set
            {
                lineNowPlayingColor2 = value;
            }
        }

        /// <summary>
        /// Private value for the LineForeColor property.
        /// </summary>
        private Color lineForeColor = Color.FromArgb(0, 0, 0);
        /// <summary>
        /// Fore font color used in the header.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue("")]
        [Category("Line"), Browsable(true), Description("Fore color used when drawing the line font.")]
        public Color LineForeColor
        {
            get
            {
                return lineForeColor;
            }
            set
            {
                lineForeColor = value;
            }
        }

        /// <summary>
        /// Private value for the IconNowPlayingColor1 property.
        /// </summary>
        private Color iconNowPlayingColor1 = Color.FromArgb(250, 200, 250);
        /// <summary>
        /// First color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Line"), Browsable(true), Description("First color of the animated icon displaying the currently playing song.")]
        public Color IconNowPlayingColor1
        {
            get
            {
                return iconNowPlayingColor1;
            }
            set
            {
                iconNowPlayingColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the IconNowPlayingColor2 property.
        /// </summary>
        private Color iconNowPlayingColor2 = Color.FromArgb(25, 150, 25);
        /// <summary>
        /// Second color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Line"), Browsable(true), Description("Second color of the animated icon displaying the currently playing song.")]
        public Color IconNowPlayingColor2
        {
            get
            {
                return iconNowPlayingColor2;
            }
            set
            {
                iconNowPlayingColor2 = value;
            }
        }

        #endregion

        #region Album Covers

        /// <summary>
        /// Private value for the AlbumCoverBackgroundColor1 property.
        /// </summary>
        private Color albumCoverBackgroundColor1 = Color.FromArgb(55, 55, 55);
        /// <summary>
        /// First color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Album Covers"), Browsable(true), Description("First color of the album cover background gradient.")]
        public Color AlbumCoverBackgroundColor1
        {
            get
            {
                return albumCoverBackgroundColor1;
            }
            set
            {
                albumCoverBackgroundColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the AlbumCoverBackgroundColor2 property.
        /// </summary>
        private Color albumCoverBackgroundColor2 = Color.FromArgb(75, 75, 75);
        /// <summary>
        /// Second color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Album Covers"), Browsable(true), Description("Second color of the album cover background gradient.")]
        public Color AlbumCoverBackgroundColor2
        {
            get
            {
                return albumCoverBackgroundColor2;
            }
            set
            {
                albumCoverBackgroundColor2 = value;
            }
        }

        #endregion

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
        /// Private value for the Font property.
        /// </summary>
        private CustomFont font = new CustomFont();
        /// <summary>
        /// Defines the font used in the control.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("General"), Browsable(true), Description("Font used to render the control.")]
        public CustomFont Font
        {
            get
            {
                return font;
            }
            set
            {
                font = value;
            }
        }

        /// <summary>
        /// Default constructor for the SongGridViewTheme class.
        /// </summary>
        public SongGridViewTheme()
        {
            // Set default values
            font = new CustomFont();
            font.EmbeddedFontName = "Junction";
            font.Size = 8;
            font.UseEmbeddedFont = true;
        }
    }
}
