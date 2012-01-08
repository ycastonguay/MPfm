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
        /// <summary>
        /// Private value for the SampleRate property.
        /// </summary>
        private int m_sampleRate = 44100;
        /// <summary>
        /// Audio file sample rate. Can be 44100, 48000, 37800 or 32000 Hz.
        /// </summary>
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
        /// Private value for the LengthSamples property.
        /// </summary>
        private long m_lengthSamples = 0;
        /// <summary>
        /// Defines the audio file length in samples.
        /// </summary>
        public long LengthSamples
        {
            get
            {
                return m_lengthSamples;
            }
            set
            {
                m_lengthSamples = value;
            }
        }

        /// <summary>
        /// Default constructor for the SV8Tag class.
        /// </summary>
        public SV8Tag()
        {
        }
    }
}
