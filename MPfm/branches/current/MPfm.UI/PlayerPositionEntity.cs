﻿//
// PlayerPositionEntity.cs: Data structure repesenting the current player position.
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

using System;
using System.IO;
using System.Reflection;

namespace MPfm.UI
{
    /// <summary>
    /// Data structure repesenting the current player position.
    /// </summary>
    public class PlayerPositionEntity
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
        /// Player current position (in time string, such as 00:00.000).
        /// </summary>
        public string Position { get; set; }
    }
}

