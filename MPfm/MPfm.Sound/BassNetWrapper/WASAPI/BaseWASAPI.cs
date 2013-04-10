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

#if !IOS && !ANDROID

using System;
using System.Collections.Generic;
using MPfm.Core;
using Un4seen.Bass;
using Un4seen.BassWasapi;

namespace MPfm.Sound.BassNetWrapper.WASAPI
{
    /// <summary>
    /// The Base class contains methods for initializing WASAPI audio devices.
    /// </summary>
    public static class BaseWASAPI
    {
        /// <summary>
        /// Initializes the default WASAPI device at 44100 Hz. Requires a data callback.
        /// </summary>
        /// <param name="proc">WASAPI data callback</param>
        public static void Init(WASAPIPROC proc)
        {
            InitWASAPI(-1, 44100, 2, BASSInit.BASS_DEVICE_DEFAULT, BASSWASAPIInit.BASS_WASAPI_SHARED, 0, 0, proc);
        }

        /// <summary>
        /// Initializes a WASAPI device by its identifier, using the specified sample rate (frequency) 
        /// and initialization flags. To get the deviceId, use the DeviceHelper class. Requires a data callback.
        /// </summary>
        /// <param name="deviceId">Device identifier</param>
        /// <param name="frequency">Sample rate (in Hz)</param>
        /// <param name="channels">Number of channels</param>
        /// <param name="init">Intiailization flags</param>
        /// <param name="wasapiInit">WASAPI initialization flags</param>
        /// <param name="buffer">Buffer size</param>
        /// <param name="period">Update period</param>
        /// <param name="proc">WASAPI callback</param>
        public static void Init(int deviceId, int frequency, int channels, BASSInit init, 
            BASSWASAPIInit wasapiInit, float buffer, float period, WASAPIPROC proc)
        {
            // Initialize base device
            if (!Bass.BASS_Init(-1, frequency, init, IntPtr.Zero))
            {
                // Check for error (throw exception if the error is found)
                CheckForError();  
            }

            // Initialize WASAPI device
            if (!BassWasapi.BASS_WASAPI_Init(deviceId, frequency, 2, wasapiInit, buffer, period, proc, IntPtr.Zero))
            {
                // Check for error (throw exception if the error is found)
                CheckForError();
            }
        }
    }
}

#endif
