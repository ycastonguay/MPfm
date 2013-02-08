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

namespace MPfm.Library.UpdateLibrary
{
    /// <summary>
    /// Defines the data structure for the Update Library finished event.
    /// </summary>
    public class UpdateLibraryFinishedData
    {
        /// <summary>
        /// Indicates if the update library process was successful.
        /// </summary>
        public bool Successful { get; set; }
        /// <summary>
        /// Indicates if the update library process was canceled.
        /// </summary>
        public bool Cancelled { get; set; }
    }
}
