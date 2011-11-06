//
// EqualizerDTO.cs: Data transfer object representing an equalizer preset;
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

namespace MPfm.Library
{
    /// <summary>
    /// Defines an equalizer preset.
    /// </summary>
    public class EqualizerDTO
    {
        public Guid EqualizerId { get; set; }
        public string Name { get; set; }

        public float Gain55Hz { get; set; }
        public float Gain77Hz { get; set; }
        public float Gain110Hz { get; set; }
        public float Gain156Hz { get; set; }
        public float Gain220Hz { get; set; }
        public float Gain311Hz { get; set; }
        public float Gain440Hz { get; set; }
        public float Gain622Hz { get; set; }
        public float Gain880Hz { get; set; }
        public float Gain1_2kHz { get; set; }
        public float Gain1_8kHz { get; set; }
        public float Gain2_5kHz { get; set; }
        public float Gain3_5kHz { get; set; }
        public float Gain5kHz { get; set; }
        public float Gain7kHz { get; set; }
        public float Gain10kHz { get; set; }
        public float Gain14kHz { get; set; }
        public float Gain20kHz { get; set; }

        /// <summary>
        /// Default constructor for EqualizerDTO.
        /// </summary>
        public EqualizerDTO()
        {
            // Set default values
            EqualizerId = Guid.NewGuid();
            Name = string.Empty;
            Gain55Hz = 0.0f;
            Gain77Hz = 0.0f;
            Gain110Hz = 0.0f;
            Gain156Hz = 0.0f;
            Gain220Hz = 0.0f;
            Gain311Hz = 0.0f;
            Gain440Hz = 0.0f;
            Gain622Hz = 0.0f;
            Gain880Hz = 0.0f;
            Gain1_2kHz = 0.0f;
            Gain1_8kHz = 0.0f;
            Gain2_5kHz = 0.0f;
            Gain3_5kHz = 0.0f;
            Gain5kHz = 0.0f;
            Gain7kHz = 0.0f;
            Gain10kHz = 0.0f;
            Gain14kHz = 0.0f;
            Gain20kHz = 0.0f;
        }
    }
}
