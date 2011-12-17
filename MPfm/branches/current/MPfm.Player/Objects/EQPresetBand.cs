//
// EQPresetBand.cs: This file contains the class defining a band of the 18-band
//                  equalizer.
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

namespace MPfm.Player
{
    /// <summary>
    /// Defines a band of the 18-band equalizer (see EQPreset).
    /// </summary>
    public class EQPresetBand
    {
        /// <summary>
        /// Defines on which BASS channel the effect will be applied (default: all channels).
        /// </summary>
        public BASSFXChan FXChannel { get; set; }

        /// <summary>
        /// Bandwidth.
        /// </summary>
        public float Bandwidth { get; set; }

        /// <summary>
        /// Center (frequency in Hz).
        /// </summary>
        public float Center { get; set; }

        /// <summary>
        /// Gain (in dB).
        /// </summary>
        public float Gain { get; set; }

        /// <summary>
        /// Q (bell width).
        /// </summary>
        public float Q { get; set; }

        /// <summary>
        /// Default constructor for the PlayerV4EQPresetBand class.
        /// Sets the default values for an equalizer band.
        /// </summary>
        public EQPresetBand()
        {
            // Set default values
            SetDefaultValues();
        }

        /// <summary>
        /// Default constructor for the PlayerV4EQPresetBand class.
        /// Sets the default values for an equalizer band.
        /// </summary>
        /// <param name="center">Equalizer center (in Hz)</param>
        public EQPresetBand(float center)
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
