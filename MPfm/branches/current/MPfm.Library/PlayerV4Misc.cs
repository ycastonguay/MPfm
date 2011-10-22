//
// PlayerV4Misc.cs: This file contains the miscellaneous classes used for the PlayerV4.
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

namespace MPfm.Library
{
    /// <summary>
    /// Structure used for the PlayerV4 containing the audio file properties/metadata
    /// and the channel/stream properties.
    /// </summary>
    public class PlayerV4Channel
    {
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

        private PlayerV4Channel m_previousChannel = null;
        public PlayerV4Channel PreviousChannel
        {
            get
            {
                return m_previousChannel;
            }
            set
            {
                m_previousChannel = value;
            }
        }

        private PlayerV4Channel m_nextChannel = null;
        public PlayerV4Channel NextChannel
        {
            get
            {
                return m_nextChannel;
            }
            set
            {
                m_nextChannel = value;
            }
        }

        /// <summary>
        /// Default constructor for the PlayerV4Channel class.
        /// </summary>
        public PlayerV4Channel()
        {
        }
    }

    public class PlayerV4ChannelCollection
    {

        public PlayerV4ChannelCollection()
        {

        }
    }

    /// <summary>
    /// Defines the data structure for the end-of-song event.
    /// </summary>
    public class PlayerV4SongFinishedData
    {
        /// <summary>
        /// Defines if the playback was stopped after the song was finished.
        /// i.e. if the RepeatType is off and the playlist is over, this property will be true.
        /// </summary>
        public bool IsPlaybackStopped { get; set; }
    }
}
