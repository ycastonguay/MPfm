// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

#if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
using Un4seen.Bass.AddOn.Fx;
#endif

namespace Sessions.Player.Objects
{
    /// <summary>
    /// Defines a band of the 18-band equalizer (see EQPreset).
    /// </summary>
    public class EQPresetBand
    {
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        /// <summary>
        /// Defines on which BASS channel the effect will be applied (default: all channels).
        /// </summary>
        public BASSFXChan FXChannel { get; set; }
        #endif

        /// <summary>
        /// Bandwidth
        /// </summary>
        public float Bandwidth { get; set; }

        /// <summary>
        /// Center (frequency in Hz).
        /// </summary>
        public float Center { get; set; }

        /// <summary>
        /// Center (frequency in string format)
        /// </summary>
        public string CenterString { get; set; }

        /// <summary>
        /// Gain (in dB).
        /// </summary>
        public float Gain { get; set; }

        /// <summary>
        /// Q (bell width).
        /// </summary>
        public float Q { get; set; }

        /// <summary>
        /// Default constructor for the EQPresetBand class.
        /// Sets the default values for an equalizer band.
        /// </summary>
        public EQPresetBand()
        {
            SetDefaultValues();
        }

        /// <summary>
        /// Default constructor for the EQPresetBand class.
        /// Sets the default values for an equalizer band.
        /// </summary>
        /// <param name="center">Equalizer center (in Hz)</param>
        /// <param name="centerString">Equalizer center (in Hz, string format)</param>
        public EQPresetBand(float center, string centerString)
        {
            SetDefaultValues();
            Center = center;
            CenterString = centerString;
        }

        /// <summary>
        /// Sets the default values for an equalizer band.
        /// </summary>
        private void SetDefaultValues()
        {
            #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
            FXChannel = BASSFXChan.BASS_BFX_CHANALL;
            #endif
            Center = 8000.0f;
            CenterString = "8kHz";
            Q = 1.0f;
            //Bandwidth = 2.5f;
            Gain = 0.0f;
        }
    }
}
