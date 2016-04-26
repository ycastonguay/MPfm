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
using Sessions.GenericControls.Controls.Base;
using Sessions.GenericControls.Controls.Items;
using Sessions.Sound.AudioFiles;

namespace Sessions.GenericControls.Controls
{
    /// <summary>
    /// Item for the SongGridView control.
    /// </summary>
    public class SongGridViewItem : ListViewItem
    {
        /// <summary>
        /// Defines the associated playlist item identifier.
        /// </summary>
        public Guid PlaylistItemId { get; set; }

        /// <summary>
        /// Defines the key used for identifying common songs in albums.
        /// </summary>
        public string AlbumArtKey { get; set; }

        /// <summary>
        /// AudioFile related to this item.
        /// </summary>
        public AudioFile AudioFile { get; set; }        

        public SongGridViewItem()
        {
            PlaylistItemId = Guid.Empty;
            AudioFile = null;
            AlbumArtKey = string.Empty;
        }
    }
}
