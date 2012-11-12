﻿//
// MPfmAudioConfig.cs: Class containing all audio settings for MPfm.
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
using MPfm.Core;
using MPfm.Library;
using MPfm.Player;
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;

namespace MPfm.MVP
{
    /// <summary>
    /// Class containing all audio settings for MPfm.
    /// </summary>
    public class MPfmAudioConfig
    {
        public Device AudioDevice { get; set; }
        public int SampleRate { get; set; }
        public float Volume { get; set; }
        public int BufferSize { get; set; }
        public bool IsEQEnabled { get; set; }
        public string EQPreset { get; set; }

        public MPfmAudioConfig()
        {
            // Set defaults
            AudioDevice = new Device();
            SampleRate = 44100;
            Volume = 1;
            BufferSize = 100;
        }
    }
}