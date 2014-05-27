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

namespace MPfm.Player.Objects
{
    public class Segment
    {   
        public Guid SegmentId { get; set; }
        public Guid LoopId { get; set; }

        public Guid StartPositionMarkerId { get; set; }
        public Guid EndPositionMarkerId { get; set; }
        
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

        public Segment()
        {
            SegmentId = Guid.NewGuid();
            LoopId = Guid.Empty;
            StartPositionMarkerId = Guid.Empty;
            EndPositionMarkerId = Guid.Empty;
            StartPosition = "0:00.000";
            EndPosition = "0:00.000";
            Length = "0:00.000";
        }
    }
}
