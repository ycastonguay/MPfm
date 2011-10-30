//
// Device.cs: This file contains the Device class which is part of the
//            BASS.NET wrapper.
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
using Un4seen.Bass;
using Un4seen.BassAsio;
using Un4seen.Bass.AddOn.Flac;
using Un4seen.Bass.AddOn.Fx;

namespace MPfm.Sound.BassNetWrapper
{
    /// <summary>
    /// Defines an audio device that can be used with PlayerV4.
    /// </summary>
    public class Device
    {
        public DriverType DriverType { get; set; }
        public bool IsDefault { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Driver { get; set; }

        /// <summary>
        /// Default constructor for the Device class.
        /// </summary>
        public Device()
        {
            DriverType = BassNetWrapper.DriverType.DirectSound;
            IsDefault = true;
            Id = -1; // default id for default device in BASS
            Name = "Default device";
            Driver = string.Empty;
        }
    }
}
