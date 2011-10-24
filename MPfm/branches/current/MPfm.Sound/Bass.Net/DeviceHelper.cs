//
// DeviceHelper.cs: This file contains the DeviceHelper class which is part of the
//                  BASS.NET wrapper.
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
using Un4seen.BassWasapi;
using Un4seen.Bass.AddOn.Flac;
using Un4seen.Bass.AddOn.Fx;

namespace MPfm.Sound.BassNetWrapper
{
    /// <summary>
    /// This class contains methods to help detect devices and test sounds.
    /// </summary>
    public static class DeviceHelper
    {
        /// <summary>
        /// Detects the devices present in the system (DirectSound, ASIO, WASAPI).
        /// </summary>
        /// <returns>List of DirectSound/ASIO/WASPI devices</returns>
        public static List<Device> DetectDevices()
        {
            // Create variables
            List<Device> devices = new List<Device>();

            // Detect DirectSound devices
            List<BASS_DEVICEINFO> devicesDirectSound = Bass.BASS_GetDeviceInfos().ToList();
            for(int a = 0; a < devicesDirectSound.Count; a++)
            {
                // Make sure the device is usable
                if (devicesDirectSound[a].IsEnabled)
                {
                    // Create device and add to list
                    Device device = new Device();
                    device.IsDefault = devicesDirectSound[a].IsDefault;
                    device.Id = a;
                    device.DriverType = DriverType.DirectSound;
                    device.Name = devicesDirectSound[a].name;
                    device.Driver = devicesDirectSound[a].driver;
                    devices.Add(device);
                }
            }

            // Detect ASIO devices
            List<BASS_ASIO_DEVICEINFO> devicesASIO = BassAsio.BASS_ASIO_GetDeviceInfos().ToList();
            for (int a = 0; a < devicesASIO.Count; a++)
            {
                // Create device and add to list
                Device device = new Device();
                device.IsDefault = false;
                device.Id = a;
                device.DriverType = DriverType.ASIO;
                device.Name = devicesASIO[a].name;
                device.Driver = devicesASIO[a].driver;
                devices.Add(device);                
            }

            // Detect WASAPI devices
            List<BASS_WASAPI_DEVICEINFO> devicesWASAPI = BassWasapi.BASS_WASAPI_GetDeviceInfos().ToList();
            for (int a = 0; a < devicesWASAPI.Count; a++)
            {
                // Make sure the device is usable
                if (devicesWASAPI[a].IsEnabled)
                {
                    // Create device and add to list
                    Device device = new Device();
                    device.IsDefault = devicesWASAPI[a].IsDefault;
                    device.Id = a;
                    device.DriverType = DriverType.WASAPI;
                    device.Name = devicesWASAPI[a].name;
                    device.Driver = devicesWASAPI[a].id;
                    devices.Add(device);
                }
            }

            return devices;
        }
    }
}
