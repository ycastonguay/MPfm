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

using System.Collections.Generic;

namespace Sessions.GenericControls.Controls.Items
{
    /// <summary>
    /// Cache for the GridView control.
    /// Contains data which doesn't need to be calculated for every
    /// control refresh, such as the line height, scrollbar offset, etc.
    /// </summary>
    public class GridViewCache : ListViewCache
    {
        /// <summary>
        /// Indicates the total width of all the visible grid view columns,
        /// even those off screen.
        /// </summary>
        public int TotalWidth { get; set; }

        /// <summary>
        /// List of currently active columns.
        /// </summary>
        public List<GridViewColumn> ActiveColumns { get; set; }
    }
}
