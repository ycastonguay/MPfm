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
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using MPfm.Core;
using MPfm.Sound;

namespace MPfm
{
    /// <summary>
    /// Defines the EQ node inside the Audio configuration section.
    /// </summary>
    public class AudioEQConfigurationSection
    {
        /// <summary>
        /// Defines if the equalizer is enabled.
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// Equalizer preset name.
        /// </summary>
        public string Preset { get; set; }

        /// <summary>
        /// Default constructor for the AudioEQConfigurationSection class.
        /// </summary>
        public AudioEQConfigurationSection()
        {
            // Set default values
            Enabled = false;
            Preset = string.Empty;
        }
    }
}