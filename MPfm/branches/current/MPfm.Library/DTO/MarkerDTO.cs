//
// MarkerDTO.cs: Data transfer object representing a marker.
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

namespace MPfm.Library
{
    /// <summary>
    /// Defines a marker for an audio file.
    /// </summary>
    public class MarkerDTO
    {
        public Guid MarkerId { get; set; }
        public Guid SongId { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public int PositionBytes { get; set; }
        public int PositionMS { get; set; }

        /// <summary>
        /// Default constructor for MarkerDTO.
        /// </summary>
        public MarkerDTO()
        {
            // Set default values
            MarkerId = Guid.NewGuid();
            SongId = Guid.Empty;
            PositionBytes = 0;
            PositionMS = 0;
        }
    }
}
