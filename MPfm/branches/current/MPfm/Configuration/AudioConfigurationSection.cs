//
// AudioConfigurationSection.cs: Configuration section used for MPfm audio settings.
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
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using MPfm.Core;
using MPfm.Sound;

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
        private AudioDeviceConfigurationSection m_device = null;
        /// <summary>
        /// Device section (name, id, etc.).
        /// </summary>
        public AudioDeviceConfigurationSection Device
        {
            get
            {
                return m_device;
            }
        }

        /// <summary>
        /// Private value for the Mixer property.
        /// </summary>
        private AudioMixerConfigurationSection m_mixer = null;
        /// <summary>
        /// Mixer section (frequency, volume, etc.).
        /// </summary>
        public AudioMixerConfigurationSection Mixer
        {
            get
            {
                return m_mixer;
            }
        }

        /// <summary>
        /// Default constructor for the AudioConfigurationSection class.
        /// </summary>
        public AudioConfigurationSection()
        {
            m_device = new AudioDeviceConfigurationSection();
            m_mixer = new AudioMixerConfigurationSection();
        }
    }

    public class AudioDeviceConfigurationSection
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class AudioMixerConfigurationSection
    {
        public int Frequency { get; set; }
        public int Volume { get; set; }
    }
}
