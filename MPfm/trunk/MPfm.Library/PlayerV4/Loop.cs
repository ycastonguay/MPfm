//
// Loop.cs: This file contains the class defining a loop to be used with PlayerV4.
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

namespace MPfm.Library.PlayerV4
{
    /// <summary>
    /// Defines a loop to be used with PlayerV4
    /// </summary>
    public class Loop
    {
        public string Name { get; set; }
        public Marker MarkerA { get; set; }
        public Marker MarkerB { get; set; }

        /// <summary>
        /// Default constructor for the Loop class.
        /// </summary>
        public Loop()
        {
            Name = string.Empty;
            MarkerA = null;
            MarkerB = null;
        }
    }
}
