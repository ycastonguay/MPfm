//
// TestDevice.cs: This file contains the TestDevice class which is part of the
//                BASS.NET wrapper.
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
        private Device m_device = null;
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
        /// <param name="driverType">Driver type to use for playback (DirectSound, ASIO, WASAPI)</param>
        /// <param name="deviceId">DeviceId to use for playback (use DeviceHelper to get DeviceId)</param>
        /// <param name="frequency">Mixer sample rate/frequency</param>
        public TestDevice(DriverType driverType, int deviceId, int frequency)
        {
            // Initialize the audio device
            InitializeDevice(new Device() { DriverType = driverType, Id = deviceId }, frequency);
        }

        /// <summary>
        /// Constructor for the TestDevice class which requires a Device class.
        /// </summary>
        /// <param name="device">Device to use for playback</param>
        /// <param name="frequency">Mixer sample rate/frequency</param>
        public TestDevice(Device device, int frequency)
        {
            // Initialize the audio device
            InitializeDevice(device, frequency);
        }

        /// <summary>
        /// Initializes a device for playback.
        /// </summary>
        /// <param name="device">Device to use for playback</param>
        /// <param name="frequency">Mixer sample rate/frequency</param>
        private void InitializeDevice(Device device, int frequency)
        {
            // Set properties
            m_device = device;

            // Check driver type
            if (m_device.DriverType == DriverType.DirectSound)
            {
                // Initialize sound system
                Base.Init(m_device.Id, frequency, BASSInit.BASS_DEVICE_DEFAULT);
            }
            else if (m_device.DriverType == DriverType.ASIO)
            {
                // Initialize sound system
                Base.InitASIO(m_device.Id, frequency, BASSInit.BASS_DEVICE_DEFAULT, BASSASIOInit.BASS_ASIO_THREAD);              
            }
            else if (m_device.DriverType == DriverType.WASAPI)
            {
                // Create callback
                m_wasapiProc = new WASAPIPROC(WASAPICallback);

                // Initialize sound system
                Base.InitWASAPI(m_device.Id, frequency, 2, BASSInit.BASS_DEVICE_DEFAULT, BASSWASAPIInit.BASS_WASAPI_SHARED, 0, 0, m_wasapiProc);
            }
        }

        /// <summary>
        /// Disposes the test device.
        /// </summary>
        public void Dispose()
        {
            // Check driver type
            if (m_device.DriverType == DriverType.ASIO)
            {
                // Free ASIO device
                if (!BassAsio.BASS_ASIO_Free())
                {
                    // Get error
                    BASSError error = Bass.BASS_ErrorGetCode();
                    throw new Exception("Error disposing TestDevice: " + error.ToString());
                }
            }
            else if (m_device.DriverType == DriverType.WASAPI)
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
            if (m_device.DriverType == DriverType.DirectSound)
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
                if(!Bass.BASS_ChannelPlay(m_stream, false))
                {
                    // Get error
                    BASSError error = Bass.BASS_ErrorGetCode();
                    throw new Exception("Error playing TestDevice: " + error.ToString());
                }
            }
            else if (m_device.DriverType == DriverType.ASIO)
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
            else if (m_device.DriverType == DriverType.WASAPI)
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
            if (m_device.DriverType == DriverType.ASIO)
            {
                // Stop playback
                BassAsio.BASS_ASIO_Stop();
            }
            else if (m_device.DriverType == DriverType.WASAPI)
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
            return GetData(buffer, length, user);
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
            return GetData(buffer, length, user);
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
            return GetData(buffer, length, user);
        }

        /// <summary>
        /// Returns the data for the DirectSound, ASIO and WASAPI callbacks.
        /// </summary>
        /// <param name="buffer">Buffer data</param>
        /// <param name="length">Buffer length</param>
        /// <param name="user">User data</param>
        /// <returns>Audio data</returns>
        private int GetData(IntPtr buffer, int length, IntPtr user)
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
