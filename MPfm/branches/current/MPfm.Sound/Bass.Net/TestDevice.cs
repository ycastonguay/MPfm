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
using System.IO;
using MPfm.Sound.BassWrapper;
using MPfm.Sound.BassWrapper.ASIO;
using MPfm.Sound.BassWrapper.Wasapi;

namespace MPfm.Sound.Bass.Net
{
    /// <summary>
    /// Defines a simple class to test an audio file against an audio device.
    /// </summary>
    public class TestDevice : ITestDevice
    {
        #region Private variables

        // Private varibles        
        private Device device = null;
        private int stream = 0;        
        private int streamDirectSound = 0;
        private STREAMPROC streamProc;
        private ASIOPROC asioProc;
        private WASAPIPROC wasapiProc;

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
            this.device = device;

            // Check driver type
            if (device.DriverType == DriverType.DirectSound)
            {
                // Initialize sound system
                Base.Init(device.Id, frequency, BASSInit.BASS_DEVICE_DEFAULT);
            }
            else if (device.DriverType == DriverType.ASIO)
            {
                // Initialize sound system
                Base.InitASIO(device.Id, frequency, BASSInit.BASS_DEVICE_DEFAULT, BASSASIOInit.BASS_ASIO_THREAD);              
            }
            else if (device.DriverType == DriverType.WASAPI)
            {
                // Create callback
                wasapiProc = new WASAPIPROC(WASAPICallback);

                // Initialize sound system
                Base.InitWASAPI(device.Id, frequency, 2, BASSInit.BASS_DEVICE_DEFAULT, BASSWASAPIInit.BASS_WASAPI_SHARED, 0, 0, wasapiProc);
            }
        }

        /// <summary>
        /// Disposes the test device.
        /// </summary>
        public void Dispose()
        {
            // Check driver type
            if (device.DriverType == DriverType.ASIO)
            {
                // Free ASIO device
                if (!BassAsio.BASS_ASIO_Free())
                {
                    // Get error
                    BASSError error = BassWrapper.Bass.BASS_ErrorGetCode();
                    throw new Exception("Error disposing TestDevice: " + error.ToString());
                }
            }
            else if (device.DriverType == DriverType.WASAPI)
            {
                // Free WASAPI device
                if (!BassWasapi.BASS_WASAPI_Free())
                {
                    // Get error
                    BASSError error = BassWrapper.Bass.BASS_ErrorGetCode();
                    throw new Exception("Error disposing TestDevice: " + error.ToString());
                }
            }

            // Free BASS
            BassWrapper.Bass.BASS_Free();
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
            if (device.DriverType == DriverType.DirectSound)
            {
                // Create stream
                stream = BassWrapper.Bass.BASS_StreamCreateFile(filePath, 0, 0, BASSFlag.BASS_DEFAULT);
                if (stream == 0)
                {
                    // Get error
                    BASSError error = BassWrapper.Bass.BASS_ErrorGetCode();
                    throw new Exception("Error playing TestDevice: " + error.ToString());
                }

                // Get stream info
                //BASS_CHANNELINFO channelInfo = Bass.BASS_ChannelGetInfo(stream);
                BASS_CHANNELINFO channelInfo = new BASS_CHANNELINFO();
                BassWrapper.Bass.BASS_ChannelGetInfo(stream, ref channelInfo);

                // Create callback
                streamProc = new STREAMPROC(DirectSoundCallback);

                // Create master stream
                streamDirectSound = BassWrapper.Bass.BASS_StreamCreate(channelInfo.freq, channelInfo.chans, BASSFlag.BASS_DEFAULT, streamProc, IntPtr.Zero);

                // Start playback
                if(!BassWrapper.Bass.BASS_ChannelPlay(stream, false))
                {
                    // Get error
                    BASSError error = BassWrapper.Bass.BASS_ErrorGetCode();
                    throw new Exception("Error playing TestDevice: " + error.ToString());
                }
            }
            else if (device.DriverType == DriverType.ASIO)
            {
                // Create stream
                stream = BassWrapper.Bass.BASS_StreamCreateFile(filePath, 0, 0, BASSFlag.BASS_STREAM_DECODE);                
                if (stream == 0)
                {
                    // Get error
                    BASSError error = BassWrapper.Bass.BASS_ErrorGetCode();
                    throw new Exception("Error playing TestDevice: " + error.ToString());
                }

                // Create callback
                asioProc = new ASIOPROC(AsioCallback);

                // Create channel
                BassAsio.BASS_ASIO_ChannelEnable(false, 0, asioProc, new IntPtr(stream));
                BassAsio.BASS_ASIO_ChannelJoin(false, 1, 0);

                // Start playback
                if (!BassAsio.BASS_ASIO_Start(0))
                {
                    // Get error
                    BASSError error = BassWrapper.Bass.BASS_ErrorGetCode();
                    throw new Exception("Error playing TestDevice: " + error.ToString());
                }
            }
            else if (device.DriverType == DriverType.WASAPI)
            {
                // Create stream
                stream = BassWrapper.Bass.BASS_StreamCreateFile(filePath, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
                if (stream == 0)
                {
                    // Get error
                    BASSError error = BassWrapper.Bass.BASS_ErrorGetCode();
                    throw new Exception("Error playing TestDevice: " + error.ToString());
                }

                // Start playback
                if (!BassWasapi.BASS_WASAPI_Start())
                {
                    // Get error
                    BASSError error = BassWrapper.Bass.BASS_ErrorGetCode();
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
            if (BassWrapper.Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PLAYING)
            {
                // Stop channel
                BassWrapper.Bass.BASS_ChannelStop(stream);                
            }

            // Check driver type
            if (device.DriverType == DriverType.ASIO)
            {
                // Stop playback
                BassAsio.BASS_ASIO_Stop();
            }
            else if (device.DriverType == DriverType.WASAPI)
            {
                // Stop playback
                BassWasapi.BASS_WASAPI_Stop(false);
            }

            // Free stream
            BassWrapper.Bass.BASS_StreamFree(stream);
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
            if (BassWrapper.Bass.BASS_ChannelIsActive(stream) == BASSActive.BASS_ACTIVE_PLAYING)
            {
                // Return data
                return BassWrapper.Bass.BASS_ChannelGetData(stream, buffer, length);
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
