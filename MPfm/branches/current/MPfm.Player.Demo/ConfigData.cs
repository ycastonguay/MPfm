//
// ConfigData.cs: This class is part of the PlaybackEngineV4 demo application.
//                This is the class defining the configuration data.
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
using MPfm.Core;
using MPfm.Sound.BassNetWrapper;

namespace MPfm.Player.Demo
{
    /// <summary>
    /// This class contains the configuration data and allows it to 
    /// save and load it from the configuration file.
    /// </summary>
    public class ConfigData
    {
        // Configuration values
        public int bufferSize = 0;
        public int updatePeriod = 0;
        public int updateThreads = 0;
        public int deviceId = 0;
        public string driverType = null;
        public string deviceName = null;

        /// <summary>
        /// Default constructor for the ConfigData class.
        /// </summary>
        public ConfigData()
        {
            // Load configuration values
            int.TryParse(Config.Load("BufferSize"), out bufferSize);
            int.TryParse(Config.Load("UpdatePeriod"), out updatePeriod);
            int.TryParse(Config.Load("UpdateThreads"), out updateThreads);
            driverType = Config.Load("DriverType");
            deviceName = Config.Load("DeviceName");

            // Check buffer size for default value/minimum/maximum
            if (bufferSize == 0)
            {
                // Set default
                bufferSize = 100;
            }
            else if (bufferSize < 50)
            {
                // Set minimum
                bufferSize = 50;
            }
            else if (bufferSize > 2000)
            {
                // Set maximum
                bufferSize = 2000;
            }

            // Check update period for default value/minimum/maximum
            if (updatePeriod == 0)
            {
                // Set default
                updatePeriod = 10;
            }
            else if (updatePeriod < 10)
            {
                // Set minimum
                updatePeriod = 10;
            }
            else if (updatePeriod > 100)
            {
                // Set maximum
                updatePeriod = 100;
            }

            // Check update threads for default value/minimum/maximum
            if (updateThreads == 0)
            {
                // Set default
                updateThreads = 1;
            }
            else if (updateThreads < 1)
            {
                // Set minimum
                updateThreads = 1;
            }
            else if (updateThreads > 8)
            {
                // Set maximum
                updateThreads = 8;
            }

            // Check if the device is in the configuration file
            if (driverType == null || deviceName == null)
            {
                // Set default device (DirectSound)
                Tracing.Log("The appSettings deviceId/deviceName/driverType nodes were not found. Loading default DirectSound device.");
                Device device = DeviceHelper.GetDefaultDirectSoundOutputDevice();
                driverType = device.DriverType.ToString();
                deviceId = device.Id;
                deviceName = device.Name;
            }
        }

        /// <summary>
        /// Saves the configuration to the configuraiton file.
        /// </summary>
        public void Save()
        {
            // Save configuration values
            Config.Save("BufferSize", bufferSize.ToString());
            Config.Save("UpdatePeriod", updatePeriod.ToString());
            Config.Save("UpdateThreads", updateThreads.ToString());
            Config.Save("DriverType", driverType);
            Config.Save("DeviceId", deviceId.ToString());
            Config.Save("DeviceName", deviceName);
        }
    }
}
