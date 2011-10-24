//
// TestDevice.cs: This file contains the TestDevice class which is part of the
//                BASS.NET wrapper.
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using Un4seen.Bass;
using Un4seen.BassAsio;
using Un4seen.BassWasapi;
using Un4seen.Bass.AddOn.Flac;
using Un4seen.Bass.AddOn.Fx;

namespace MPfm.Sound.BassNetWrapper
{
    /// <summary>
    /// Defines a simple class to test an audio file against an audio device.
    /// </summary>
    public class TestDevice
    {
        #region Private variables

        // Private varibles
        private int m_pluginFlac = 0;
        private DriverType m_driverType = DriverType.DirectSound;
        private int m_deviceId = 0;
        private int m_stream = 0;
        private int m_streamDirectSound = 0;
        private STREAMPROC m_streamProc;
        private ASIOPROC m_asioProc;
        private WASAPIPROC m_wasapiProc;

        #endregion

        #region Constructor / Dispose
        
        /// <summary>
        /// Default constructor for the TestDevice class.
        /// </summary>
        public TestDevice(DriverType driverType, int deviceId)
        {
            // Set properties
            m_driverType = driverType;
            m_deviceId = deviceId;

            // Initialize BASS
            BassNet.Registration("yanick.castonguay@gmail.com", "2X3433427152222");

            // Check driver type
            if (m_driverType == DriverType.DirectSound)
            {
                // Initialize device
                if (!Bass.BASS_Init(deviceId, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
                {
                    // Get error
                    BASSError error = Bass.BASS_ErrorGetCode();
                    throw new Exception("Error initializing TestDevice: " + error.ToString());
                }
            }
            else if (m_driverType == DriverType.ASIO)
            {
                // Initialize base device
                if (!Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
                {
                    // Get error
                    BASSError error = Bass.BASS_ErrorGetCode();
                    throw new Exception("Error initializing TestDevice: " + error.ToString());
                }

                // Initialize ASIO device
                if (!BassAsio.BASS_ASIO_Init(deviceId, BASSASIOInit.BASS_ASIO_THREAD))
                {
                    // Get error
                    BASSError error = Bass.BASS_ErrorGetCode();
                    throw new Exception("Error initializing TestDevice: " + error.ToString());
                }                
            }
            else if (m_driverType == DriverType.WASAPI)
            {
                // Initialize base device
                if (!Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
                {
                    // Get error
                    BASSError error = Bass.BASS_ErrorGetCode();
                    throw new Exception("Error initializing TestDevice: " + error.ToString());
                }

                // Create callback
                m_wasapiProc = new WASAPIPROC(WASAPICallback);

                // Initialize WASAPI device
                if (!BassWasapi.BASS_WASAPI_Init(deviceId, 44100, 2, BASSWASAPIInit.BASS_WASAPI_SHARED, 0, 0, m_wasapiProc, IntPtr.Zero))
                {
                    // Get error
                    BASSError error = Bass.BASS_ErrorGetCode();
                    throw new Exception("Error initializing TestDevice: " + error.ToString());
                }
            }

            // Load plugins
            string filePathFlacPlugin = Path.GetDirectoryName((new Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath) + "\\bassflac.dll";
            m_pluginFlac = Bass.BASS_PluginLoad(filePathFlacPlugin);
            if (m_pluginFlac == 0)
            {
                // Error loading plugin
                BASSError error = Bass.BASS_ErrorGetCode();
            }
        }

        /// <summary>
        /// Disposes the test device.
        /// </summary>
        public void Dispose()
        {
            // Free stream
            //Bass.BASS_StreamFree(stream);

            // Free plugins
            Bass.BASS_PluginFree(m_pluginFlac);

            // Check driver type
            if (m_driverType == DriverType.ASIO)
            {
                // Free ASIO device
                if (!BassAsio.BASS_ASIO_Free())
                {
                    // Get error
                    BASSError error = Bass.BASS_ErrorGetCode();
                    throw new Exception("Error disposing TestDevice: " + error.ToString());
                }
            }
            else if (m_driverType == DriverType.WASAPI)
            {
                // Free WASAPI device
                if (!BassWasapi.BASS_WASAPI_Free())
                {
                    // Get error
                    BASSError error = Bass.BASS_ErrorGetCode();
                    throw new Exception("Error disposing TestDevice: " + error.ToString());
                }
            }

            // Free BASS
            Bass.BASS_Free();
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Starts the playback of an audio file.
        /// </summary>
        /// <param name="filePath">Audio file path</param>
        public void Play(string filePath)
        {
            // Check if the file exists
            if (!File.Exists(filePath))
            {
                throw new Exception("Error in TestDevice.Play: The file doesn't exist!");
            }
            
            // Check driver type
            if (m_driverType == DriverType.DirectSound)
            {
                // Create stream
                m_stream = Bass.BASS_StreamCreateFile(filePath, 0, 0, BASSFlag.BASS_DEFAULT);
                if (m_stream == 0)
                {
                    // Get error
                    BASSError error = Bass.BASS_ErrorGetCode();
                    throw new Exception("Error playing TestDevice: " + error.ToString());
                }

                // Get stream info
                BASS_CHANNELINFO channelInfo = Bass.BASS_ChannelGetInfo(m_stream);

                // Create callback
                m_streamProc = new STREAMPROC(DirectSoundCallback);

                // Create master stream
                m_streamDirectSound = Bass.BASS_StreamCreate(channelInfo.freq, channelInfo.chans, BASSFlag.BASS_DEFAULT, m_streamProc, IntPtr.Zero);

                // Start playback
                Bass.BASS_ChannelPlay(m_stream, false);
                //Bass.BASS_ChannelPlay(m_streamDirectSound, false);
            }
            else if (m_driverType == DriverType.ASIO)
            {
                // Create stream
                m_stream = Bass.BASS_StreamCreateFile(filePath, 0, 0, BASSFlag.BASS_STREAM_DECODE);                
                if (m_stream == 0)
                {
                    // Get error
                    BASSError error = Bass.BASS_ErrorGetCode();
                    throw new Exception("Error playing TestDevice: " + error.ToString());
                }

                // Create callback
                m_asioProc = new ASIOPROC(AsioCallback);

                // Create channel
                BassAsio.BASS_ASIO_ChannelEnable(false, 0, m_asioProc, new IntPtr(m_stream));
                BassAsio.BASS_ASIO_ChannelJoin(false, 1, 0);

                // Start playback
                if (!BassAsio.BASS_ASIO_Start(0))
                {
                    // Get error
                    BASSError error = Bass.BASS_ErrorGetCode();
                    throw new Exception("Error playing TestDevice: " + error.ToString());
                }
            }
            else if (m_driverType == DriverType.WASAPI)
            {
                // Create stream
                m_stream = Bass.BASS_StreamCreateFile(filePath, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
                if (m_stream == 0)
                {
                    // Get error
                    BASSError error = Bass.BASS_ErrorGetCode();
                    throw new Exception("Error playing TestDevice: " + error.ToString());
                }

                // Start playback
                if (!BassWasapi.BASS_WASAPI_Start())
                {
                    // Get error
                    BASSError error = Bass.BASS_ErrorGetCode();
                    throw new Exception("Error playing TestDevice: " + error.ToString());
                }

                //BassWasapi.BASS_WASAPI_SetVolume(true, 1);
            }
        }

        /// <summary>
        /// Stops the playback of the audio file.
        /// </summary>
        public void Stop()
        {
            // Check if the channel is still playing
            if (Bass.BASS_ChannelIsActive(m_stream) == BASSActive.BASS_ACTIVE_PLAYING)
            {
                // Stop channel
                Bass.BASS_ChannelStop(m_stream);                
            }

            // Check driver type
            if (m_driverType == DriverType.ASIO)
            {
                // Stop playback
                BassAsio.BASS_ASIO_Stop();
            }
            else if (m_driverType == DriverType.WASAPI)
            {
                // Stop playback
                BassWasapi.BASS_WASAPI_Stop(false);
            }

            // Free stream
            Bass.BASS_StreamFree(m_stream);
        }

        #endregion

        #region Callbacks

        /// <summary>
        /// Callback used for DirectSound devices.
        /// </summary>
        /// <param name="handle">Channel handle</param>
        /// <param name="buffer">Buffer data</param>
        /// <param name="length">Buffer length</param>
        /// <param name="user">User data</param>
        /// <returns>Audio data</returns>
        private int DirectSoundCallback(int handle, IntPtr buffer, int length, IntPtr user)
        {
            // Check if the channel is still playing
            if (Bass.BASS_ChannelIsActive(m_stream) == BASSActive.BASS_ACTIVE_PLAYING)
            {
                // Return data
                return Bass.BASS_ChannelGetData(m_stream, buffer, length);
            }
            else
            {
                // Return end of file
                return (int)BASSStreamProc.BASS_STREAMPROC_END;
            }
        }

        /// <summary>
        /// Callback used for ASIO devices.
        /// </summary>
        /// <param name="input">Is an input channel</param>
        /// <param name="channel">Channel handle</param>
        /// <param name="buffer">Buffer data</param>
        /// <param name="length">Buffer length</param>
        /// <param name="user">User data</param>
        /// <returns>Audio data</returns>
        private int AsioCallback(bool input, int channel, IntPtr buffer, int length, IntPtr user)
        {
            // Check if the channel is still playing
            if (Bass.BASS_ChannelIsActive(m_stream) == BASSActive.BASS_ACTIVE_PLAYING)
            {
                // Return data
                return Bass.BASS_ChannelGetData(m_stream, buffer, length);
            }
            else
            {
                // Return end of file
                return (int)BASSStreamProc.BASS_STREAMPROC_END;
            }
        }

        /// <summary>
        /// Callback used for WASAPI devices.
        /// </summary>
        /// <param name="buffer">Buffer data</param>
        /// <param name="length">Buffer length</param>
        /// <param name="user">User data</param>
        /// <returns>Audio data</returns>
        private int WASAPICallback(IntPtr buffer, int length, IntPtr user)
        {
            // Check if the channel is still playing
            if (Bass.BASS_ChannelIsActive(m_stream) == BASSActive.BASS_ACTIVE_PLAYING)
            {
                // Return data
                return Bass.BASS_ChannelGetData(m_stream, buffer, length);
            }
            else
            {
                // Return end of file
                return (int)BASSStreamProc.BASS_STREAMPROC_END;
            }
        }

        #endregion
    }
}
