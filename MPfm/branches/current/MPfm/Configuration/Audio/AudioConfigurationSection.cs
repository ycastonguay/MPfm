//
// AudioConfigurationSection.cs: Configuration section used for MPfm audio settings.
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
using MPfm.Sound.BassNetWrapper;

namespace MPfm
{
    /// <summary>
    /// This configuration section contains the audio settings for MPfm.
    /// It has two main sections: Device and Mixer.
    /// </summary>
    public class AudioConfigurationSection
    {
        /// <summary>
        /// Driver type (DirectSound, ASIO, WASAPI).
        /// </summary>
        public MPfm.Sound.BassNetWrapper.DriverType DriverType { get; set; }

        /// <summary>
        /// Private value for the Device property.
        /// </summary>
        private Device device = null;
        /// <summary>
        /// Output device (name, id, etc.).
        /// </summary>
        public Device Device
        {
            get
            {
                return device;
            }
            set
            {
                device = value;
            }
        }

        /// <summary>
        /// Private value for the Mixer property.
        /// </summary>
        private AudioMixerConfigurationSection mixer = null;
        /// <summary>
        /// Mixer section (frequency, volume, etc.).
        /// </summary>
        public AudioMixerConfigurationSection Mixer
        {
            get
            {
                return mixer;
            }
        }

        /// <summary>
        /// Private value for the EQ property.
        /// </summary>
        private AudioEQConfigurationSection eq = null;
        /// <summary>
        /// EQ (equalizer) section (enabled, preset, etc.).
        /// </summary>
        public AudioEQConfigurationSection EQ
        {
            get
            {
                return eq;
            }
        }

        /// <summary>
        /// Default constructor for the AudioConfigurationSection class.
        /// </summary>
        public AudioConfigurationSection()
        {
            // Create sections
            device = new Device();
            mixer = new AudioMixerConfigurationSection();
            eq = new AudioEQConfigurationSection();
        }
    }
}
