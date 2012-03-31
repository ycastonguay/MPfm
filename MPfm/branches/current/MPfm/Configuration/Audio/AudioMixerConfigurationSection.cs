//
// AudioMixerConfigurationSection.cs: Defines the Mixer node inside the Audio 
//                                    configuration section.
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
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using MPfm.Core;
using MPfm.Sound;

namespace MPfm
{
    /// <summary>
    /// Defines the Mixer node inside the Audio configuration section.
    /// </summary>
    public class AudioMixerConfigurationSection
    {
        /// <summary>
        /// Mixer frequency (sample rate) in Hz (default: 44100Hz).
        /// </summary>
        public int Frequency { get; set; }
        /// <summary>
        /// Mixer volume in percentage (0-100).
        /// </summary>
        public int Volume { get; set; }
        /// <summary>
        /// Mixer buffer size (default: 1000ms).
        /// </summary>
        public int BufferSize { get; set; }
        /// <summary>
        /// Mixer update period (default: 10ms).
        /// </summary>
        public int UpdatePeriod { get; set; }

        /// <summary>
        /// Default constructor for the AudioMixerConfigurationSection class.
        /// </summary>
        public AudioMixerConfigurationSection()
        {
            // Set default values
            Frequency = 44100;
            Volume = 85;
            BufferSize = 1000;
            UpdatePeriod = 10;            
        }
    }
}
