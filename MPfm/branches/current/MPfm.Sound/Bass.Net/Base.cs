//
// Base.cs: This file contains the Base class which is part of the
//          BASS.NET wrapper. It only contains static methods.
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
using System.Reflection;
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
    /// The Base class contains methods for initializing audio devices,
    /// creating stream, and more. All methods are static.
    /// </summary>
    public static class Base
    {
        #region BASS.NET Registration
        
        /// <summary>
        /// Registers the BASS.NET library using the email and registration key.
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="registrationKey">Registration key</param>
        public static void Register(string email, string registrationKey)
        {
            // Set registration information
            BassNet.Registration(email, registrationKey);
        }

        #endregion

        #region Initialize/Free Devices

        /// <summary>
        /// Initializes the default DirectSound device at 44100 Hz.
        /// </summary>
        public static void Init()
        {
            Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT);
        }

        /// <summary>
        /// Initializes a DirectSound device by its identifier, using the specified sample rate (frequency) 
        /// and initialization flags. To get the deviceId, use the DeviceHelper class.
        /// </summary>
        /// <param name="deviceId">Device identifier</param>
        /// <param name="frequency">Sample rate (in Hz)</param>
        /// <param name="init">Intiailization flags</param>
        public static void Init(int deviceId, int frequency, BASSInit init)
        {
            // Initialize system
            if (!Bass.BASS_Init(deviceId, frequency, init, IntPtr.Zero))
            {
                // Check for error (throw exception if the error is found)
                CheckForError();
            }
        }

        /// <summary>
        /// Initializes the default ASIO device at 44100 Hz. 
        /// </summary>
        public static void InitASIO()
        {
            InitASIO(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, BASSASIOInit.BASS_ASIO_THREAD);
        }

        /// <summary>
        /// Initializes an ASIO device by its identifier, using the specified sample rate (frequency) 
        /// and initialization flags. To get the deviceId, use the DeviceHelper class.
        /// </summary>
        /// <param name="deviceId">Device identifier</param>
        /// <param name="frequency">Sample rate (in Hz)</param>
        /// <param name="init">Intiailization flags</param>
        /// <param name="asioInit">ASIO initialization flags</param>
        public static void InitASIO(int deviceId, int frequency, BASSInit init, BASSASIOInit asioInit)
        {
            // Initialize base device
            if (!Bass.BASS_Init(-1, frequency, init, IntPtr.Zero))
            {
                // Check for error (throw exception if the error is found)
                CheckForError();            
            }

            // Initialize ASIO device
            if (!BassAsio.BASS_ASIO_Init(deviceId, asioInit))
            {
                // Check for error (throw exception if the error is found)
                CheckForError();
            }   
        }

        /// <summary>
        /// Initializes the default WASAPI device at 44100 Hz. Requires a data callback.
        /// </summary>
        /// <param name="proc">WASAPI data callback</param>
        public static void InitWASAPI(WASAPIPROC proc)
        {
            InitWASAPI(-1, 44100, 2, BASSInit.BASS_DEVICE_DEFAULT, BASSWASAPIInit.BASS_WASAPI_SHARED, 0, 0, proc);
        }

        /// <summary>
        /// Initializes a WASAPI device by its identifier, using the specified sample rate (frequency) 
        /// and initialization flags. To get the deviceId, use the DeviceHelper class. Requires a data callback.
        /// </summary>
        /// <param name="deviceId">Device identifier</param>
        /// <param name="frequency">Sample rate (in Hz)</param>
        /// <param name="init">Intiailization flags</param>
        /// <param name="asioInit">ASIO initialization flags</param>
        public static void InitWASAPI(int deviceId, int frequency, int channels, BASSInit init, 
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

        /// <summary>
        /// Free all devices.
        /// </summary>
        public static void Free()
        {
            // Free system
            if(!Bass.BASS_Free())
            {
                // Check for error (throw exception if the error is found)
                CheckForError();            
            }
        }

        #endregion

        #region Configuration/Information
        
        /// <summary>
        /// Gets a BASS configuration value.
        /// </summary>
        /// <param name="option">BASS option</param>
        /// <returns>Value (integer)</returns>
        public static int GetConfig(BASSConfig option)
        {
            return Bass.BASS_GetConfig(option);
        }

        /// <summary>
        /// Sets a BASS configuration value.
        /// </summary>
        /// <param name="option">BASS option</param>
        /// <param name="value">Value (integer)</param>
        public static void SetConfig(BASSConfig option, int value)
        {
            // Set configuration value
            if(!Bass.BASS_SetConfig(option, value))
            {
                // Check for error (throw exception if the error is found)
                CheckForError();
            }
        }

        /// <summary>
        /// Gets the BASS information.
        /// </summary>
        /// <returns>BASS_INFO structure</returns>
        public static BASS_INFO GetInfo()
        {
            BASS_INFO info = new BASS_INFO();
            if (!Bass.BASS_GetInfo(info))
            {
                // Check for error (throw exception if the error is found)
                CheckForError();
            }

            return info;
        }

        #endregion

        #region Master Volume
        
        /// <summary>
        /// Gets the device master volume of the default device.
        /// This is the same value as the volume fader in the Windows tray.
        /// </summary>
        /// <returns>Volume</returns>
        public static float GetVolume()
        {
            return Bass.BASS_GetVolume();
        }

        /// <summary>
        /// Sets the master volume of the default device.
        /// This sets the volume fader value in the Windows tray.
        /// </summary>
        /// <param name="volume">Volume</param>
        public static void SetVolume(float volume)
        {
            // Set volume
            if (!Bass.BASS_SetVolume(volume))
            {
                // Check for error (throw exception if the error is found)
                CheckForError();
            }
        }

        #endregion

        #region Plugins

        /// <summary>
        /// Loads a specific BASS plugin.
        /// </summary>
        /// <param name="pluginFilePath">Plugin file path</param>
        /// <returns>Plugin handle</returns>
        public static int LoadPlugin(string pluginFilePath)
        {
            // Load plugins            
            int plugin = Bass.BASS_PluginLoad(pluginFilePath);
            if (plugin == 0)
            {
                // Check for error (throw exception if the error is found)
                CheckForError();
            }

            return plugin;
        }

        /// <summary>
        /// Loads all the BASS plugins in the directory path.
        /// </summary>
        /// <param name="directoryPath">Directory path</param>
        /// <returns>Dictionary of plugin handles and plugin file paths</returns>
        public static Dictionary<int, string> LoadPluginDirectory(string directoryPath)
        {
            return Bass.BASS_PluginLoadDirectory(directoryPath);
        }

        /// <summary>
        /// Frees all the BASS plugins from the directory path.
        /// </summary>
        /// <param name="dictionary">Dictionary of plugin handles and plugin file paths</param>
        public static void FreePluginDirectory(Dictionary<int, string> dictionary)
        {
            // Loop through handles
            foreach (KeyValuePair<int, string> pair in dictionary)
            {
                // Free plugin
                FreePlugin(pair.Key);
            }
        }

        /// <summary>
        /// Frees a BASS plugin.
        /// </summary>
        /// <param name="handle">Plugin handle</param>
        public static void FreePlugin(int handle)
        {
            // Free Flac plugin
            if (!Bass.BASS_PluginFree(handle))
            {
                // Check for error (throw exception if the error is found)
                CheckForError();
            }
        }

        /// <summary>
        /// Loads the BASS FX plugin.
        /// </summary>
        public static void LoadFxPlugin()
        {
            //// Load plugins           
            //string filePathFlacPlugin = Path.GetDirectoryName((new Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath) + "\\bass_fx.dll";
            //int pluginFlac = Bass.BASS_PluginLoad(filePathFlacPlugin);
            //if (pluginFlac == 0)
            //{
            //    // Check for error (throw exception if the error is found)
            //    CheckForError();
            //}

            //return pluginFlac;

            // Load FX plugin
            if (!BassFx.LoadMe())
            {
                // Check for error (throw exception if the error is found)
                CheckForError();
            }
        }

        /// <summary>
        /// Frees the BASS FX plugin.
        /// </summary>
        public static void FreeFxPlugin()
        {
            // Free FX plugin
            if (!BassFx.FreeMe())
            {
                // Check for error (throw exception if the error is found)
                CheckForError();
            }
        }

        #endregion

        #region Conversion

        /// <summary>
        /// Converts a channel level to dB.
        /// </summary>
        /// <param name="level">Channel level</param>
        /// <returns>Level in dB</returns>
        public static Double LevelToDB_16Bit(double level)
        {
            return Utils.LevelToDB(level, 65535);
        }

        #endregion

        #region Error Management
        
        /// <summary>
        /// Checks for an error inside BASS.NET. Throws an exception with the
        /// BASS error code if an error is found.
        /// </summary>
        public static void CheckForError()
        {
            // Get error code
            Un4seen.Bass.BASSError error = Bass.BASS_ErrorGetCode();

            // Check if there is an error
            if(error != BASSError.BASS_OK)
            {
                // Throw exception
                throw new BassNetWrapperException(error.ToString());
            }
        }

        #endregion
    }
}
