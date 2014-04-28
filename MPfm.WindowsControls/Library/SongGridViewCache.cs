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
using System.Linq;
using System.Text;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// General cache for the SongGridView control.
    /// Contains data which doesn't need to be calculated for every
    /// control refresh, such as the line height, scrollbar offset, etc.
    /// </summary>
    public class SongGridViewCache
    {
        /// <summary>
        /// Indicates the line height for the grid view items.
        /// </summary>
        public int LineHeight { get; set; }
        
        /// <summary>
        /// Indicates the total width of all the visible grid view columns,
        /// even those off screen.
        /// </summary>
        public int TotalWidth { get; set; }

        /// <summary>
        /// Indicates the total height of all the visible grid view items,
        /// even those off screen.
        /// </summary>
        public int TotalHeight { get; set; }

        /// <summary>
        /// Current scrollbar offset Y value (
        /// </summary>
        public int ScrollBarOffsetY { get; set; }

        /// <summary>
        /// Indicates how many lines fit the visible control area, including
        /// lines that are partly visible.
        /// </summary>
        public int NumberOfLinesFittingInControl { get; set; }

        /// <summary>
        /// List of currently active columns.
        /// </summary>
        public List<SongGridViewColumn> ActiveColumns { get; set; }
    }
}
