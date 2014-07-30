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

namespace Sessions.Player.Objects
{
    /// <summary>
    /// Data structure repesenting the current player position.
    /// </summary>
    public class PlayerPosition
    {
        /// <summary>
        /// Player current position (in bytes).
        /// </summary>
        public long PositionBytes { get; set; }
        /// <summary>
        /// Player current position (in samples).
        /// </summary>
        public long PositionSamples { get; set; }
        /// <summary>
        /// Player current position (in milliseconds).
        /// </summary>
        public long PositionMS { get; set; }
		/// <summary>
		/// Player current position (in percentage).
		/// </summary>
		public float PositionPercentage { get; set; }		
        /// <summary>
        /// Player current position (in time string, such as 00:00.000).
        /// </summary>
        public string Position { get; set; }

        public PlayerPosition()
        {
            Position = "0:00.000";
        }
    }
}

