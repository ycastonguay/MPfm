//
// ControlsSongGridViewQueryConfigurationSection.cs: Defines the Query node inside the
//                                                   SongGridView configuration section.
//
// Copyright © 2011 Yanick Castonguay
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
// along with MPfm. If not, see <http:/s/www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using MPfm.Core;
using MPfm.Sound;

namespace MPfm
{
    /// <summary>
    /// Defines the Query node inside the SongGridView configuration section.
    /// </summary>
    public class ControlsSongGridViewQueryConfigurationSection
    {
        /// <summary>
        /// Audio file identifier.
        /// </summary>
        public Guid AudioFileId { get; set; }
        /// <summary>
        /// Playlist identifier.
        /// </summary>
        public Guid PlaylistId { get; set; }
        /// <summary>
        /// Artist name.
        /// </summary>
        public string ArtistName { get; set; }
        /// <summary>
        /// Album title.
        /// </summary>
        public string AlbumTitle { get; set; }

        /// <summary>
        /// Default constructor for the ControlsSongGridViewQueryConfigurationSection class.
        /// </summary>
        public ControlsSongGridViewQueryConfigurationSection()
        {
            // Set default values
            AudioFileId = Guid.Empty;
            PlaylistId = Guid.Empty;
            ArtistName = string.Empty;
            AlbumTitle = string.Empty;
        }
    }
}
