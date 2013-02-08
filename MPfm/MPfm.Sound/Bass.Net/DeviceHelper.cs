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

using System.Collections.Generic;
using System.Linq;
using MPfm.Sound.BassWrapper;
using MPfm.Sound.BassWrapper.ASIO;
using MPfm.Sound.BassWrapper.Wasapi;

namespace MPfm.Sound.Bass.Net
{
    /// <summary>
    /// This class contains methods to help detect devices and test sounds.
    /// </summary>
    public static class DeviceHelper
    {
        /// <summary>
        /// Detects the output devices present in the system (DirectSound, ASIO, WASAPI).
        /// </summary>
        /// <returns>List of DirectSound/ASIO/WASPI output devices</returns>
        public static List<Device> DetectOutputDevices()
        {
            // Create variables
            List<Device> devices = new List<Device>();

            // Detect DirectSound devices
            List<BASS_DEVICEINFO> devicesDirectSound = BassWrapper.Bass.BASS_GetDeviceInfos().ToList();
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

                    // If the driver name is No Sound, don't add it to the list of devices
                    if (device.Name.ToUpper() != "NO SOUND")
                    {
                        devices.Add(device);
                    }                    
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
                // Make sure the device is usable, and an output device
                if (devicesWASAPI[a].IsEnabled && !devicesWASAPI[a].IsInput)
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

        /// <summary>
        /// Find an output device by its driver type and its device name.
        /// This is useful to get the actual deviceId because it can change if the
        /// user plugs/unplugs a sound card.
        /// </summary>
        /// <param name="driverType">Driver type</param>
        /// <param name="deviceName">Device name</param>
        /// <returns>Device (null if none found)</returns>
        public static Device FindOutputDevice(DriverType driverType, string deviceName)
        {
            // Check driver type
            if (driverType == DriverType.DirectSound)
            {
                // Detect DirectSound devices
                List<BASS_DEVICEINFO> devicesDirectSound = BassWrapper.Bass.BASS_GetDeviceInfos().ToList();
                for (int a = 0; a < devicesDirectSound.Count; a++)
                {
                    // Check if the driver name is the same, and make sure the device is also enabled (i.e. available)
                    if (devicesDirectSound[a].name.ToUpper() == deviceName.ToUpper() &&
                        devicesDirectSound[a].IsEnabled)
                    {                        
                        // Create device and add to list
                        Device device = new Device();
                        device.IsDefault = devicesDirectSound[a].IsDefault;
                        device.Id = a;
                        device.DriverType = DriverType.DirectSound;
                        device.Name = devicesDirectSound[a].name;
                        device.Driver = devicesDirectSound[a].driver;
                        return device;
                    }
                }
            }
            else if (driverType == DriverType.ASIO)
            {
                // Detect ASIO devices
                List<BASS_ASIO_DEVICEINFO> devicesASIO = BassAsio.BASS_ASIO_GetDeviceInfos().ToList();
                for (int a = 0; a < devicesASIO.Count; a++)
                {
                    // Check if the driver name is the same
                    if (devicesASIO[a].name.ToUpper() == deviceName.ToUpper())
                    {
                        // Create device and add to list
                        Device device = new Device();
                        device.IsDefault = false;
                        device.Id = a;
                        device.DriverType = DriverType.ASIO;
                        device.Name = devicesASIO[a].name;
                        device.Driver = devicesASIO[a].driver;
                        return device;
                    }
                }
            } 
            else if (driverType == DriverType.WASAPI)
            {
                // Detect WASAPI devices
                List<BASS_WASAPI_DEVICEINFO> devicesWASAPI = BassWasapi.BASS_WASAPI_GetDeviceInfos().ToList();
                for (int a = 0; a < devicesWASAPI.Count; a++)
                {
                    // Check if the driver name is the same, that the device is an output device, and make sure that it is enabled
                    if (devicesWASAPI[a].name.ToUpper() == deviceName.ToUpper() &&
                        devicesWASAPI[a].IsEnabled && !devicesWASAPI[a].IsInput)
                    {
                        // Create device and add to list
                        Device device = new Device();
                        device.IsDefault = devicesWASAPI[a].IsDefault;
                        device.Id = a;
                        device.DriverType = DriverType.WASAPI;
                        device.Name = devicesWASAPI[a].name;
                        device.Driver = devicesWASAPI[a].id;
                        return device;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the default DirectSound device.
        /// If no device was no found, the value is null.
        /// </summary>
        /// <returns>Default DirectSound device</returns>
        public static Device GetDefaultDirectSoundOutputDevice()
        {
            // Detect DirectSound devices
            List<BASS_DEVICEINFO> devicesDirectSound = BassWrapper.Bass.BASS_GetDeviceInfos().ToList();
            for (int a = 0; a < devicesDirectSound.Count; a++)
            {
                // Check if the device is the default one
                if (devicesDirectSound[a].IsEnabled && devicesDirectSound[a].IsDefault)
                {
                    // Create device and add to list
                    Device device = new Device();
                    device.IsDefault = devicesDirectSound[a].IsDefault;
                    device.Id = a;
                    device.DriverType = DriverType.DirectSound;
                    device.Name = devicesDirectSound[a].name;
                    device.Driver = devicesDirectSound[a].driver;
                    return device;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the default WASAPI device.
        /// If no device was no found, the value is null.
        /// </summary>
        /// <returns>Default WASAPI device</returns>
        public static Device GetDefaultWASAPIOutputDevice()
        {
            // Detect WASAPI devices
            List<BASS_WASAPI_DEVICEINFO> devicesWASAPI = BassWasapi.BASS_WASAPI_GetDeviceInfos().ToList();
            for (int a = 0; a < devicesWASAPI.Count; a++)
            {
                // Check if the device is the default one
                if (devicesWASAPI[a].IsEnabled && devicesWASAPI[a].IsDefault && !devicesWASAPI[a].IsInput)
                {
                    // Create device and add to list
                    Device device = new Device();
                    device.IsDefault = devicesWASAPI[a].IsDefault;
                    device.Id = a;
                    device.DriverType = DriverType.WASAPI;
                    device.Name = devicesWASAPI[a].name;
                    device.Driver = devicesWASAPI[a].id;
                    return device;
                }
            }

            return null;
        }
    }
}
