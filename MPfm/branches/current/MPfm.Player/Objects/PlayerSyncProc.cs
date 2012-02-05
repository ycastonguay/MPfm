﻿//
// PlayerSyncProc.cs: Defines synchronization callback properties to be 
//                    used with the Player class.
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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using MPfm.Core;
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Flac;
using Un4seen.Bass.AddOn.Fx;

namespace MPfm.Player
{
    /// <summary>
    /// Defines synchronization callback properties to be used with the Player class.
    /// </summary>
    public class PlayerSyncProc
    {
        /// <summary>
        /// Private value for the Handle property.
        /// </summary>
        private int m_handle = 0;
        /// <summary>
        /// Defines the synchronization callback handle.
        /// </summary>
        public int Handle
        {
            get
            {
                return m_handle;
            }
            set
            {
                m_handle = value;
            }
        }

        /// <summary>
        /// Private value for the SyncProc property.
        /// </summary>
        private SYNCPROC m_syncProc = null;
        /// <summary>
        /// Defines the synchronization callback (BASS.NET object).
        /// </summary>
        public SYNCPROC SyncProc
        {
            get
            {
                return m_syncProc;
            }
            set
            {
                m_syncProc = value;
            }
        }

        /// <summary>
        /// Default constructor for the SyncProc class.
        /// </summary>
        public PlayerSyncProc()
        {
        }
    }
}
