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

    /// <summary>
    /// Defines a 18-band equalizer preset for PlayerV4 using BASS FX.
    /// </summary>
    public class PlayerV4EQPreset
    {
        /// <summary>
        /// Private value for the Bands property.
        /// </summary>
        private List<PlayerV4EQPresetBand> m_bands = null;
        /// <summary>
        /// List of equalizer bands (18 bands).
        /// </summary>
        public List<PlayerV4EQPresetBand> Bands
        {
            get
            {
                return m_bands;
            }
        }

        /// <summary>
        /// Default constructor for the PlayerV4EQPreset class.
        /// </summary>
        public PlayerV4EQPreset()
        {
            // Create default preset
            LoadDefault();
        }

        public void LoadDefault()
        {
            // Create default preset
            m_bands = new List<PlayerV4EQPresetBand>();

            // Create default list of frequencies
            List<float> freqs = new List<float>();
            freqs.Add(55.0f);
            freqs.Add(77.0f);
            freqs.Add(110.0f);
            freqs.Add(156.0f);
            freqs.Add(220.0f);
            freqs.Add(311.0f);
            freqs.Add(440.0f);
            freqs.Add(622.0f);
            freqs.Add(880.0f);
            freqs.Add(1200.0f);
            freqs.Add(1800.0f);
            freqs.Add(2500.0f);
            freqs.Add(3500.0f);
            freqs.Add(5000.0f);
            freqs.Add(7000.0f);
            freqs.Add(10000.0f);
            freqs.Add(14000.0f);
            freqs.Add(20000.0f);

            // Loop through default frequencies (i.e. 18 times)
            for (int a = 0; a < freqs.Count; a++)
            {
                // Add equalizer band
                m_bands.Add(new PlayerV4EQPresetBand(freqs[a]));
            }
            
        }

        public static void Load(string filePath)
        {
        
        }

        public static void Save(string filePath)
        {

        }
    }

    /// <summary>
    /// Defines a band of the 18-band equalizer (see PlayerV4EQPreset).
    /// </summary>
    public class PlayerV4EQPresetBand
    {
        public BASSFXChan FXChannel { get; set; }
        public float Bandwidth { get; set; }
        public float Center { get; set; }
        public float Gain { get; set; }
        public float Q { get; set; }

        /// <summary>
        /// Default constructor for the PlayerV4EQPresetBand class.
        /// Sets the default values for an equalizer band.
        /// </summary>
        public PlayerV4EQPresetBand()
        {
            // Set default values
            SetDefaultValues();
        }

        /// <summary>
        /// Default constructor for the PlayerV4EQPresetBand class.
        /// Sets the default values for an equalizer band.
        /// </summary>
        /// <param name="center">Equalizer center (in Hz)</param>
        public PlayerV4EQPresetBand(float center)
        {
            // Set default values
            SetDefaultValues();
            Center = center;
        }

        /// <summary>
        /// Sets the default values for an equalizer band.
        /// </summary>
        private void SetDefaultValues()
        {
            // Set default values
            FXChannel = BASSFXChan.BASS_BFX_CHANALL;
            Center = 8000.0f;
            Q = 1.0f;
            Bandwidth = 2.5f;
            Gain = 0.0f;
        }
    }
}
