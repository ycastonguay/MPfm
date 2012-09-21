﻿//
// PlayerService.cs: Service used for interacting with a single instance of a Player.
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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;
using MPfm.Player;

namespace MPfm.MVP
{
    /// <summary>
    /// Service used for interacting with a single instance of a Player.
    /// </summary>
    public class PlayerService : IPlayerService
    {
        public IPlayer Player { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MPfm.MVP.PlayerService"/> class.
        /// </summary>
		public PlayerService()
		{
		}

        public void Initialize(Device device, int sampleRate, int bufferSize, int updatePeriod)
        {
            // Initialize player
            Player = new MPfm.Player.Player(device, sampleRate, bufferSize, updatePeriod, true);
        }

        /// <summary>
        /// Releases all resource used by the <see cref="MPfm.MVP.PlayerService"/> object.
        /// </summary>
        /// <remarks>
        /// Call <see cref="Dispose"/> when you are finished using the <see cref="MPfm.MVP.PlayerService"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="MPfm.MVP.PlayerService"/> in an unusable state. After
        /// calling <see cref="Dispose"/>, you must release all references to the <see cref="MPfm.MVP.PlayerService"/>
        /// so the garbage collector can reclaim the memory that the <see cref="MPfm.MVP.PlayerService"/> was occupying.
        /// </remarks>
        public void Dispose()
        {
            // Dispose player
            if (Player != null)
            {
                Player.Dispose();
                Player = null;
            }
        }
    }
}
