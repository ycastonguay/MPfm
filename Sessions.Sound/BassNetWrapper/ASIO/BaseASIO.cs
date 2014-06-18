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
using Un4seen.Bass;
using Un4seen.BassAsio;

namespace MPfm.Sound.BassNetWrapper.ASIO
{
    /// <summary>
    /// The Base class contains methods for initializing ASIO audio devices.
    /// </summary>
    public static class BaseASIO
    {
        /// <summary>
        /// Initializes the default ASIO device at 44100 Hz. 
        /// </summary>
        public static void Init()
        {
            Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, BASSASIOInit.BASS_ASIO_THREAD);
        }

        /// <summary>
        /// Initializes an ASIO device by its identifier, using the specified sample rate (frequency) 
        /// and initialization flags. To get the deviceId, use the DeviceHelper class.
        /// </summary>
        /// <param name="deviceId">Device identifier</param>
        /// <param name="frequency">Sample rate (in Hz)</param>
        /// <param name="init">Intiailization flags</param>
        /// <param name="asioInit">ASIO initialization flags</param>
        public static void Init(int deviceId, int frequency, BASSInit init, BASSASIOInit asioInit)
        {
            // Initialize base device
            if (!Bass.BASS_Init(-1, frequency, init, IntPtr.Zero))
            {
                // Check for error (throw exception if the error is found)
                Base.CheckForError();            
            }

            // Initialize ASIO device
            if (!BassAsio.BASS_ASIO_Init(deviceId, asioInit))
            {
                // Check for error (throw exception if the error is found)
                Base.CheckForError();
            }   
        }

        /// <summary>
        /// Opens the ASIO control panel of the currently initialized device.
        /// </summary>
        public static void ASIO_ControlPanel()
        {
            bool success = BassAsio.BASS_ASIO_ControlPanel();
            if (!success)
            {
                throw new Exception("Could not load the ASIO Control Panel for this sound card.");
            }
        }
        
        /// <summary>
        /// Returns information about the ASIO device.
        /// </summary>
        /// <param name="deviceId">Device Id</param>
        /// <param name="sampleRate">Sample rate</param>
        /// <returns>ASIOInfo data structure</returns>
        public static ASIOInfo GetASIOInfo(bool initializeBass, int deviceId, int sampleRate)
        {
            ASIOInfo info = new ASIOInfo();

            if (initializeBass)
            {
                //Base.Init();
            }

            // Initialize ASIO device
            if (!BassAsio.BASS_ASIO_Init(deviceId, BASSASIOInit.BASS_ASIO_DEFAULT))
            {
                // Check for error (throw exception if the error is found)
                Base.CheckForError();
            }

            // Get ASIO device information                
            info.Latency = BassAsio.BASS_ASIO_GetLatency(false);
            info.SampleRate = (int)BassAsio.BASS_ASIO_GetRate();
            info.Info = BassAsio.BASS_ASIO_GetInfo();

            // Free ASIO device
            //bool success = BassAsio.BASS_ASIO_Free();
            //if (!success)
            //{
            //    CheckForError();
            //}

            if (initializeBass)
            {
                //Base.Free();
            }

            return info;
        }
    }
}

#endif
