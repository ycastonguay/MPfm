//
// SongGridViewItem.cs: Item for the SongGridView control.
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
// along with MPfm. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MPfm.Library;
using MPfm.Player;
using MPfm.Sound;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// Item for the SongGridView control.
    /// </summary>
    public class SongGridViewItem
    {
        /// <summary>
        /// Indicates if the item is currently selected.
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Indicates if the mouse cursor is currently over the item.
        /// </summary>
        public bool IsMouseOverItem { get; set; }

        /// <summary>
        /// AudioFile related to this item.
        /// </summary>
        public AudioFile AudioFile { get; set; }        

        /// <summary>
        /// Default constructor for GridViewSongItem.
        /// </summary>
        public SongGridViewItem()
        {
            // Set default values
            IsSelected = false;
            IsMouseOverItem = false;
            AudioFile = null;
        }
    }
}
