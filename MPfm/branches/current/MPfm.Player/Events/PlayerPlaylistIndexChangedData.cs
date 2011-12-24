﻿//
// PlayerAudioFileFinishedData.cs: Defines the data structure for the event 
//                                 when the audio file has finished playing.
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
using MPfm.Sound;

namespace MPfm.Player
{
    /// <summary>
    /// Defines the data structure for the event when the playlist index changes (i.e. when an audio file
    /// starts to play).
    /// Related to the Player class.
    /// </summary>
    public class PlayerPlaylistIndexChangedData
    {
        /// <summary>
        /// Defines if the playback was stopped after the audio file has finished playing.
        /// i.e. if the RepeatType is off and the playlist is over, this property will be true.
        /// </summary>
        public bool IsPlaybackStopped { get; set; }
        /// <summary>
        /// Defines the audio file that just ended.
        /// </summary>
        public AudioFile AudioFileEnded { get; set; }
        /// <summary>
        /// Defines the audio file that just started. Might be null if the playback has stopped.
        /// </summary>
        public AudioFile AudioFileStarted { get; set; }

        /// <summary>
        /// Default constructor for the PlayerPlaylistIndexChangedData class.
        /// </summary>
        public PlayerPlaylistIndexChangedData()
        {
            // Set default values
            IsPlaybackStopped = false;            
            AudioFileEnded = null;
            AudioFileStarted = null;
        }
    }
}