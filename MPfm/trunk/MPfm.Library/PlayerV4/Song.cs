//
// Song.cs: This file contains the class defining a song used in the player V4.
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

namespace MPfm.Library.PlayerV4
{
    /// <summary>
    /// Defines a song to be played using the PlayerV4. This class contains the audio file properties/metadata
    /// and the channel/stream properties.
    /// </summary>
    public class Song
    {
        private SYNCPROC m_syncProc = null;
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

        private int m_syncProcHandle = 0;
        public int SyncProcHandle
        {
            get
            {
                return m_syncProcHandle;
            }
            set
            {
                m_syncProcHandle = value;
            }
        }

        private AudioFile m_fileProperties = null;
        public AudioFile FileProperties
        {
            get
            {
                return m_fileProperties;
            }
            set
            {
                m_fileProperties = value;
            }
        }

        private Channel m_channel = null;
        public Channel Channel
        {
            get
            {
                return m_channel;
            }
            set
            {
                m_channel = value;
            }
        }

        private Song m_previousSong = null;
        public Song PreviousSong
        {
            get
            {
                return m_previousSong;
            }
            set
            {
                m_previousSong = value;
            }
        }

        private Song m_nextSong = null;
        public Song NextSong
        {
            get
            {
                return m_nextSong;
            }
            set
            {
                m_nextSong = value;
            }
        }

        /// <summary>
        /// Default constructor for the PlayerV4Channel class.
        /// </summary>
        public Song()
        {
        }
    }
}
