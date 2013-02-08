//
// PlayerTimeShiftingEntity.cs: Data structure repesenting the current player time shifting.
//
// Copyright © 2011-2012 Yanick Castonguay
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

namespace MPfm.MVP.Models
{
    /// <summary>
    /// Data structure repesenting the current player time shifting.
    /// </summary>
    public class PlayerTimeShiftingEntity
    {
        /// <summary>
        /// Player time shifting (in float format).
        /// Value range from -100.0f (-100%) to 100.0f (+100%). To reset, set to 0.0f.
        /// </summary>
        public float TimeShifting { get; set; }
        /// <summary>
        /// Player time shifting (in string format, 50% to 150%).
        /// </summary>
        public string TimeShiftingString { get; set; }
    }
}

