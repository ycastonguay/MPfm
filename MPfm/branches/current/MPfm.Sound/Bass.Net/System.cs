//
// System.cs: This file contains the System class which is part of the
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
    public static class Base
    {
        //public System(DriverType driverType)
        //{
        //    // Register BASS.NET with key
        //    Register("yanick.castonguay@gmail.com", "2X3433427152222");

        //    // Initialize system with default frequency and default sound card
        //    Initialize(44100);
        //}

        //public System(DriverType driverType, int mixerSampleRate)
        //{
        //    // Register BASS.NET with key
        //    Register("yanick.castonguay@gmail.com", "2X3433427152222");

        //    // Initialize system with default frequency and default sound card
        //    Initialize(mixerSampleRate);
        //}

        public static void Register(string email, string registrationKey)
        {
            BassNet.Registration(email, registrationKey);
        }

        public static void Init()
        {
            Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT);
        }

        public static void Init(int deviceId, int frequency, BASSInit init)
        {
            // Initialize system
            if (!Bass.BASS_Init(deviceId, frequency, init, IntPtr.Zero))
            {
                // Check for error (throw exception if the error is found)
                CheckForError();
            }
        }

        public static void InitASIO()
        {
            InitASIO(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, BASSASIOInit.BASS_ASIO_THREAD);
        }

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

        public static void InitWASAPI(WASAPIPROC proc)
        {
            InitWASAPI(-1, 44100, 2, BASSInit.BASS_DEVICE_DEFAULT, BASSWASAPIInit.BASS_WASAPI_SHARED, 0, 0, proc);
        }

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

        public static void Free()
        {
            // Free system
            if(!Bass.BASS_Free())
            {
                // Check for error (throw exception if the error is found)
                CheckForError();            
            }
        }

        public static int GetConfig(BASSConfig option)
        {
            return Bass.BASS_GetConfig(option);
        }

        public static void SetConfig(BASSConfig option, int value)
        {
            // Set configuration value
            if(!Bass.BASS_SetConfig(option, value))
            {
                // Check for error (throw exception if the error is found)
                CheckForError();
            }
        }

        public static float GetVolume()
        {
            return Bass.BASS_GetVolume();
        }

        public static void SetVolume(float volume)
        {
            // Set volume
            if (!Bass.BASS_SetVolume(volume))
            {
                // Check for error (throw exception if the error is found)
                CheckForError();
            }
        }

        #region Plugins

        public static int LoadFlacPlugin()
        {
            // Load plugins
            string filePathFlacPlugin = Path.GetDirectoryName((new Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath) + "\\bassflac.dll";
            int pluginFlac = Bass.BASS_PluginLoad(filePathFlacPlugin);
            if (pluginFlac == 0)
            {
                // Check for error (throw exception if the error is found)
                CheckForError();
            }

            return pluginFlac;
        }

        public static void FreeFlacPlugin(int handle)
        {
            // Free Flac plugin
            if (!Bass.BASS_PluginFree(handle))
            {
                // Check for error (throw exception if the error is found)
                CheckForError();
            }
        }

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

        public static void CheckForError()
        {
            Un4seen.Bass.BASSError error = Bass.BASS_ErrorGetCode();
            if(error != BASSError.BASS_OK)
            {
                throw new Exception("An error has occured in BassNetWrapper: " + error.ToString());
            }
        }
    }

    public enum DriverType
    {
        DirectSound = 0, ASIO = 1, WASAPI = 2
    }
}
