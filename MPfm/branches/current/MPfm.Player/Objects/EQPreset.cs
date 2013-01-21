//
// EQPreset.cs: This file contains the class defining the preset of the 18-band
//              equalizer.
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

namespace MPfm.Player.Objects
{
    /// <summary>
    /// Defines a 18-band equalizer preset for the Player using BASS FX.
    /// </summary>
    public class EQPreset
    {
        /// <summary>
        /// EQ preset unique identifier (for database storage).
        /// </summary>
        public Guid EQPresetId { get; set; }

        /// <summary>
        /// EQ preset name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Private value for the Bands property.
        /// </summary>
        private List<EQPresetBand> bands = null;
        /// <summary>
        /// List of equalizer bands (18 bands).
        /// </summary>
        public List<EQPresetBand> Bands
        {
            get
            {
                return bands;
            }
        }

        /// <summary>
        /// Default constructor for the EQPreset class.
        /// </summary>
        public EQPreset()
        {
            // Create default preset
            LoadDefault();
        }

        /// <summary>
        /// Gets the list of default frequencies for each band of the 18-band equalizer.
        /// </summary>
        /// <returns>List of frequencies (18 bands)</returns>
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

        /// <summary>
        /// Loads the default EQ preset.
        /// </summary>
        public void LoadDefault()
        {
            // Create default preset
            bands = new List<EQPresetBand>();
            Name = "Default";
            EQPresetId = Guid.NewGuid();

            // Create default list of frequencies
            List<float> freqs = GetDefaultFreqs();

            // Loop through default frequencies (i.e. 18 times)
            for (int a = 0; a < freqs.Count; a++)
            {
                // Add equalizer band
                bands.Add(new EQPresetBand(freqs[a]));
            }
        }

        /// <summary>
        /// Loads an EQ preset from file.
        /// </summary>
        /// <param name="filePath">EQ preset file path</param>
        public void Load(string filePath)
        {

        }

        /// <summary>
        /// Saves the current EQ preset to file.
        /// </summary>
        /// <param name="filePath">EQ preset file path</param>
        public void Save(string filePath)
        {

        }
    }
}
