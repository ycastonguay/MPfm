﻿//
// Device.cs: This file contains the Device class which is part of the
//            BASS.NET wrapper.
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

using MPfm.Sound.BassWrapper.ASIO;

namespace MPfm.Sound.BassNetWrapper
{
    /// <summary>
    /// Defines a data structure for holding information about an ASIO device.
    /// </summary>
    public class ASIOInfo
    {
        /// <summary>
        /// Device latency (in samples).
        /// </summary>
        public int Latency { get; set; }
        /// <summary>
        /// Device sample rate (in Hz).
        /// </summary>
        public int SampleRate { get; set; }
        /// <summary>
        /// Device information (BASS information structure).
        /// </summary>
        public BASS_ASIO_INFO Info { get; set; }

        /// <summary>
        /// Default constructor for the ASIOInfo class.
        /// </summary>
        public ASIOInfo()
        {
            Latency = 0;
            SampleRate = 0;            
        }
    }
}
