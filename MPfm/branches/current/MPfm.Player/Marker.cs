﻿//
// Marker.cs: This file contains the class defining a marker to be used with PlayerV4.
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
using System.Linq;
using System.Text;

namespace MPfm.Player.PlayerV4
{
    /// <summary>
    /// Defines a Marker, which points to a specific position in an audio file.
    /// Loops are made of two markers.
    /// </summary>
    public class Marker
    {
        /// <summary>
        /// Marker unique identifier (for database storage).
        /// </summary>
        public Guid MarkerId { get; set; }
        /// <summary>
        /// Relationship to the Song unique identifier (for database storage).
        /// </summary>
        public Guid SongId { get; set; }
        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Comments.
        /// </summary>
        public string Comments { get; set; }
        /// <summary>
        /// Position (in bytes).
        /// </summary>
        public long PositionBytes { get; set; }
        /// <summary>
        /// Position (in milliseconds).
        /// </summary>
        public int PositionMS { get; set; }

        /// <summary>
        /// Default constructor for the Marker class.
        /// </summary>
        public Marker()
        {
            // Set default values
            MarkerId = Guid.NewGuid();
            SongId = Guid.Empty;
            Name = string.Empty;
            Comments = string.Empty;
            PositionBytes = 0;
            PositionMS = 0;
        }
    }
}
