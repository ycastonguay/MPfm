//
// Events.cs: This file contains the miscellaneous classes used for the PlayerV4 events.
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

namespace MPfm.Player
{
    /// <summary>
    /// Defines the data structure for the end-of-song event.
    /// </summary>
    public class SongFinishedData
    {
        /// <summary>
        /// Defines if the playback was stopped after the song was finished.
        /// i.e. if the RepeatType is off and the playlist is over, this property will be true.
        /// </summary>
        public bool IsPlaybackStopped { get; set; }
    }

    /// <summary>
    /// Defines the data structure for the stream callback data.
    /// </summary>
    public class StreamCallbackData
    {
        public byte[] Data { get; set; }
        public float[] Data2 { get; set; }
        public int Length { get; set; }
    }

}
