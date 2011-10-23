//
// EQPreset.cs: This file contains the class defining the preset of the 18-band
//              equalizer.
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

namespace MPfm.Library.PlayerV4
{
    /// <summary>
    /// Defines a 18-band equalizer preset for PlayerV4 using BASS FX.
    /// </summary>
    public class EQPreset
    {
        /// <summary>
        /// Private value for the Bands property.
        /// </summary>
        private List<EQPresetBand> m_bands = null;
        /// <summary>
        /// List of equalizer bands (18 bands).
        /// </summary>
        public List<EQPresetBand> Bands
        {
            get
            {
                return m_bands;
            }
        }

        /// <summary>
        /// Default constructor for the PlayerV4EQPreset class.
        /// </summary>
        public EQPreset()
        {
            // Create default preset
            LoadDefault();
        }

        public static List<float> GetDefaultFreqs()
        {
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

            return freqs;
        }

        public void LoadDefault()
        {
            // Create default preset
            m_bands = new List<EQPresetBand>();

            // Create default list of frequencies
            List<float> freqs = GetDefaultFreqs();

            // Loop through default frequencies (i.e. 18 times)
            for (int a = 0; a < freqs.Count; a++)
            {
                // Add equalizer band
                m_bands.Add(new EQPresetBand(freqs[a]));
            }

        }

        public static void Load(string filePath)
        {

        }

        public static void Save(string filePath)
        {

        }
    }
}
