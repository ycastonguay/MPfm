//
// SV8Tag.cs: Data structure for SV8 tags.
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
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MPfm.Sound
{  
    /// <summary>
    /// Data structure for SV8 tags.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SV8Tag
    {
        #region Stream Header
        
        /// <summary>
        /// Private value for the SampleRate property.
        /// </summary>
        private int m_sampleRate = 44100;
        /// <summary>
        /// Audio file sample rate. Can be 44100, 48000, 37800 or 32000 Hz.
        /// </summary>
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("Audio file sample rate.")]
        public int SampleRate
        {
            get
            {
                return m_sampleRate;
            }
            set
            {
                m_sampleRate = value;
            }
        }

        /// <summary>
        /// Private value for the AudioChannels property.
        /// </summary>
        private int m_audioChannels = 2;
        /// <summary>
        /// Defines the number of audio channels used for this audio file.
        /// </summary>
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("Number of audio channels.")]
        public int AudioChannels
        {
            get
            {
                return m_audioChannels;
            }
            set
            {
                m_audioChannels = value;
            }
        }

        /// <summary>
        /// Private value for the Length property.
        /// </summary>
        private long m_length = 0;
        /// <summary>
        /// Defines the audio file length in samples.
        /// </summary>
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("Audio file length (in samples).")]
        public long Length
        {
            get
            {
                return m_length;
            }
            set
            {
                m_length = value;
            }
        }

        /// <summary>
        /// Private value for the BeginningSilence property.
        /// </summary>
        private long m_beginningSilence = 0;
        /// <summary>
        /// Defines the number of samples to skip at the beginning of the stream (silence).
        /// </summary>
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("Number of samples to skip at the beginning of the stream (silence).")]
        public long BeginningSilence
        {
            get
            {
                return m_beginningSilence;
            }
            set
            {
                m_beginningSilence = value;
            }
        }

        /// <summary>
        /// Private value for the MidSideStereoEnabled property.
        /// </summary>
        private bool m_midSideStereoEnabled = false;
        /// <summary>
        /// Defines if the Mid Side Stereo mode is enabled.
        /// </summary>
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("Defines if the Mid Side Stereo mode is enabled.")]
        public bool MidSideStereoEnabled
        {
            get
            {
                return m_midSideStereoEnabled;
            }
            set
            {
                m_midSideStereoEnabled = value;
            }
        }

        /// <summary>
        /// Private value for the AudioBlockFrames property.
        /// </summary>
        private int m_audioBlockFrames = 0;
        /// <summary>
        /// Defines the number of frames per audio packet.
        /// </summary>
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("Number of frames per audio packet.")]
        public int AudioBlockFrames
        {
            get
            {
                return m_audioBlockFrames;
            }
            set
            {
                m_audioBlockFrames = value;
            }
        }

        /// <summary>
        /// Private value for the MaxUsedBands property.
        /// </summary>
        private int m_maxUsedBands = 0;
        /// <summary>
        /// Defines the maximum number of bands used in the audio file.
        /// </summary>
        [Category("Stream Header"), Browsable(true), ReadOnly(true), Description("Maximum number of bands used in the audio file.")]
        public int MaxUsedBands
        {
            get
            {
                return m_maxUsedBands;
            }
            set
            {
                m_maxUsedBands = value;
            }
        }

        #endregion

        /// <summary>
        /// Default constructor for the SV8Tag class.
        /// </summary>
        public SV8Tag()
        {
        }
    }
}
