// Copyright © 2011-2013 Yanick Castonguay
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
        /// List of equalizer bands (18 bands).
        /// </summary>
        public List<EQPresetBand> Bands { get; private set; }

        // These are for saving gain values to database
        public float Gain0 { get { return Bands[0].Gain; } set { Bands[0].Gain = value; } }
        public float Gain1 { get { return Bands[1].Gain; } set { Bands[1].Gain = value; } }
        public float Gain2 { get { return Bands[2].Gain; } set { Bands[2].Gain = value; } }
        public float Gain3 { get { return Bands[3].Gain; } set { Bands[3].Gain = value; } }
        public float Gain4 { get { return Bands[4].Gain; } set { Bands[4].Gain = value; } }
        public float Gain5 { get { return Bands[5].Gain; } set { Bands[5].Gain = value; } }
        public float Gain6 { get { return Bands[6].Gain; } set { Bands[6].Gain = value; } }
        public float Gain7 { get { return Bands[7].Gain; } set { Bands[7].Gain = value; } }
        public float Gain8 { get { return Bands[8].Gain; } set { Bands[8].Gain = value; } }
        public float Gain9 { get { return Bands[9].Gain; } set { Bands[9].Gain = value; } }
        public float Gain10 { get { return Bands[10].Gain; } set { Bands[10].Gain = value; } }
        public float Gain11 { get { return Bands[11].Gain; } set { Bands[11].Gain = value; } }
        public float Gain12 { get { return Bands[12].Gain; } set { Bands[12].Gain = value; } }
        public float Gain13 { get { return Bands[13].Gain; } set { Bands[13].Gain = value; } }
        public float Gain14 { get { return Bands[14].Gain; } set { Bands[14].Gain = value; } }
        public float Gain15 { get { return Bands[15].Gain; } set { Bands[15].Gain = value; } }
        public float Gain16 { get { return Bands[16].Gain; } set { Bands[16].Gain = value; } }
        public float Gain17 { get { return Bands[17].Gain; } set { Bands[17].Gain = value; } }

        /// <summary>
        /// Default constructor for the EQPreset class.
        /// </summary>
        public EQPreset()
        {
            Name = "New Preset";
            EQPresetId = Guid.NewGuid();
            Reset();
        }

        /// <summary>
        /// Gets the list of default frequencies for each band of the 18-band equalizer.
        /// </summary>
        /// <returns>List of frequencies (18 bands)</returns>
        public static List<Tuple<float, string>> GetDefaultFreqs()
        {
            List<Tuple<float, string>> freqs = new List<Tuple<float, string>>();
            freqs.Add(new Tuple<float, string>(55.0f, "55 Hz"));
            freqs.Add(new Tuple<float, string>(77.0f, "77 Hz"));
            freqs.Add(new Tuple<float, string>(110.0f, "110 Hz"));
            freqs.Add(new Tuple<float, string>(156.0f, "156 Hz"));
            freqs.Add(new Tuple<float, string>(220.0f, "220 Hz"));
            freqs.Add(new Tuple<float, string>(311.0f, "311 Hz"));
            freqs.Add(new Tuple<float, string>(440.0f, "440 Hz"));
            freqs.Add(new Tuple<float, string>(622.0f, "622 Hz"));
            freqs.Add(new Tuple<float, string>(880.0f, "880 Hz"));
            freqs.Add(new Tuple<float, string>(1200.0f, "1.2 kHz"));
            freqs.Add(new Tuple<float, string>(1800.0f, "1.8 kHz"));
            freqs.Add(new Tuple<float, string>(2500.0f, "2.5 kHz"));
            freqs.Add(new Tuple<float, string>(3500.0f, "3.5 kHz"));
            freqs.Add(new Tuple<float, string>(5000.0f, "5 kHz"));
            freqs.Add(new Tuple<float, string>(7000.0f, "7 kHz"));
            freqs.Add(new Tuple<float, string>(10000.0f, "10 kHz"));
            freqs.Add(new Tuple<float, string>(14000.0f, "14 kHz"));
            freqs.Add(new Tuple<float, string>(20000.0f, "20 kHz"));
            return freqs;
        }

        /// <summary>
        /// Resets EQ preset bands.
        /// </summary>
        public void Reset()
        {
            // Create default list of frequencies
            Bands = new List<EQPresetBand>();
            List<Tuple<float, string>> freqs = GetDefaultFreqs();
            for (int a = 0; a < freqs.Count; a++)
                Bands.Add(new EQPresetBand(freqs[a].Item1, freqs[a].Item2));
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
