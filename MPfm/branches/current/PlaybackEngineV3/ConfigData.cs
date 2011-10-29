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

namespace PlaybackEngineV4
{
    /// <summary>
    /// This class contains the configuration data and allows it to 
    /// save and load it from the configuration file.
    /// </summary>
    public class ConfigData
    {
        // Configuration values
        public int bufferSize = 100;
        public int updatePeriod = 10;
        public int updateThreads = 1;
        public int deviceId = 1;
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
