//
// Loop.cs: This file contains the class defining a loop to be used with the Player.
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

namespace MPfm.Player
{
    /// <summary>
    /// Defines a Loop, which must be used with two Markers.
    /// </summary>
    public class Loop
    {   
        /// <summary>
        /// Marker unique identifier (for database storage).
        /// </summary>        
        public Guid LoopId { get; set; }
        /// <summary>
        /// Relationship to the AudioFile unique identifier (for database storage).
        /// </summary>
        public Guid AudioFileId { get; set; }
        /// <summary>
        /// Loop name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Loop comments.
        /// </summary>
        public string Comments { get; set; }
        /// <summary>
        /// Start position (in 0:00.000 string format).
        /// </summary>
        public string StartPosition { get; set; }
        /// <summary>
        /// Start position (in bytes).
        /// </summary>
        public uint StartPositionBytes { get; set; }
        /// <summary>
        /// Start position (in samples).
        /// </summary>
        public uint StartPositionSamples { get; set; }
        /// <summary>
        /// End position (in 0:00.000 string format).
        /// </summary>
        public string EndPosition { get; set; }
        /// <summary>
        /// End position (in bytes).
        /// </summary>
        public uint EndPositionBytes { get; set; }
        /// <summary>
        /// End position (in samples).
        /// </summary>
        public uint EndPositionSamples { get; set; }
        /// <summary>
        /// Loop length (in 0:00.000 string format).
        /// </summary>
        public string Length { get; set; }
        /// <summary>
        /// Loop length (in bytes).
        /// </summary>
        public uint LengthBytes { get; set; }
        /// <summary>
        /// Loop length (in samples).
        /// </summary>
        public uint LengthSamples { get; set; }

        /// <summary>
        /// Default constructor for the Loop class.
        /// </summary>
        public Loop()
        {
            // Set default values
            LoopId = Guid.NewGuid();
            AudioFileId = Guid.Empty;
            Name = string.Empty;
            StartPosition = "0:00.000";
            StartPositionBytes = 0;
            StartPositionSamples = 0;
            EndPosition = "0:00.000";
            StartPositionBytes = 0;
            StartPositionSamples = 0;
            Length = "0:00.000";
            LengthBytes = 0;
            LengthSamples = 0;
        }
    }
}
