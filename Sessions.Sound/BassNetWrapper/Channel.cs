// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;
using Un4seen.Bass.AddOn.Mix;

namespace Sessions.Sound.BassNetWrapper
{
    /// <summary>
    /// Defines a channel/stream to be used with BASS.NET.
    /// If you need to create a mixer channel, use the MixerChannel class.
    /// </summary>
    public class Channel : IChannel
    {
        #region Properties
     
        /// <summary>
        /// Private value for the ChannelType property.
        /// </summary>
        protected ChannelType channelType = ChannelType.Playback;
        /// <summary>
        /// Indicates the type of channel (playback, FX, etc.)
        /// </summary>
        public ChannelType ChannelType
        {
            get
            {
                return channelType;
            }
        }

        /// <summary>
        /// Private value for the IsDecode property.
        /// </summary>
        protected bool isDecode = false;
        /// <summary>
        /// Indicates if the channel/stream is decoding.
        /// </summary>
        public bool IsDecode
        {
            get
            {
                return isDecode;
            }            
        }

        /// <summary>
        /// Private value for the IsFloatingPoint property.
        /// </summary>
        protected bool isFloatingPoint = false;
        /// <summary>
        /// Indicates if the channel/stream is using floating point.
        /// </summary>
        public bool IsFloatingPoint
        {
            get
            {
                return isFloatingPoint;
            }
        }

        /// <summary>
        /// Private value for the Handle property.
        /// </summary>
        protected int handle = 0;
        /// <summary>
        /// Handle to the channel.
        /// </summary>
        public int Handle
        {
            get
            {
                return handle;
            }
        }

        /// <summary>
        /// Gets/sets the volume of the channel using GetAttribute/SetAttribute with
        /// the attribute BASS_ATTRIB_VOL.
        /// </summary>
        public float Volume
        {
            get
            {
                // Get volume
                float value = 0;
                GetAttribute(BASSAttribute.BASS_ATTRIB_VOL, ref value);
                return value;
            }
            set
            {
                // Set volume
                SetAttribute(BASSAttribute.BASS_ATTRIB_VOL, value);
            }
        }

        /// <summary>
        /// Private value for the SampleRate property.
        /// </summary>
        protected int sampleRate = 0;
        /// <summary>
        /// Defines the sample rate used for the channel.
        /// To fetch the "live" sample rate, use GetSampleRate().
        /// To set the sample rate, use SetSampleRate(). This will also set this value.
        /// </summary>
        public int SampleRate
        {
            get
            {
                if (sampleRate == 0)
                {
                    try
                    {
                        sampleRate = GetSampleRate();
                    }
                    catch
                    {
                        // Leave to 0
                    }
                }

                return sampleRate;
            }
        }

        #endregion

        #region Constructor
        
        /// <summary>
        /// Default constructor for the Channel class. To create a new channel, use one
        /// of the static methods of this class.
        /// </summary>
        /// <param name="handle">Handle to the BASS.NET channel</param>
        /// <param name="channelType">Type of channel (playback, FX, etc.)</param>
        /// <param name="isDecode">Indicates if the channel is decoding</param>
        /// <param name="isFloatingPoint">Indicates if the channel is using floating point</param>
        public Channel(int handle, ChannelType channelType, bool isDecode, bool isFloatingPoint)
        {
            // Set properties
            this.handle = handle;
            this.channelType = channelType;
            this.isDecode = isDecode;
            this.isFloatingPoint = isFloatingPoint;
        }

        #endregion

        #region Static Methods (CreateStream)

        /// <summary>
        /// Creates a stream from memory using a custom callback procedure.
        /// </summary>
        /// <param name="frequency">Frequency (sample rate)</param>
        /// <param name="numberOfChannels">Number of channels (mono = 1, stereo = 2)</param>
        /// <param name="useFloatingPoint">Indicates if the channel should use floating point</param>
        /// <param name="streamProc">Handle to the STREAMPROC callback</param>
        /// <returns>Channel object</returns>
        public static Channel CreateStream(int frequency, int numberOfChannels, bool useFloatingPoint, STREAMPROC streamProc)
        {
            BASSFlag flags = BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_STREAM_PRESCAN;
            if (useFloatingPoint)
                flags |= BASSFlag.BASS_SAMPLE_FLOAT;

            int handle = Bass.BASS_StreamCreate(frequency, numberOfChannels, flags, streamProc, IntPtr.Zero);
            if (handle == 0)
                Base.CheckForError();
            
            return new Channel(handle, ChannelType.Memory, true, useFloatingPoint) { sampleRate = frequency };
        }

        public static Channel CreatePushStream(int frequency, int numberOfChannels, bool useFloatingPoint)
        {
            BASSFlag flags = BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_STREAM_PRESCAN;
            if (useFloatingPoint)
                flags |= BASSFlag.BASS_SAMPLE_FLOAT;

            int handle = Bass.BASS_StreamCreatePush(frequency, numberOfChannels, flags, IntPtr.Zero);
            if (handle == 0)
                Base.CheckForError();

            return new Channel(handle, ChannelType.Memory, true, useFloatingPoint) { sampleRate = frequency };
        }

        /// <summary>
        /// Creates a stream from file for decoding.
        /// </summary>
        /// <param name="filePath">File path to the audio file</param>
        /// <param name="useFloatingPoint">Indicates if the channel should use floating point</param>
        /// <returns>Channel object</returns>
        public static Channel CreateFileStreamForDecoding(string filePath, bool useFloatingPoint)
        {
            // Build flags; add base flags
            BASSFlag flags = BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_STREAM_PRESCAN;
            if (useFloatingPoint)
                flags |= BASSFlag.BASS_SAMPLE_FLOAT;

            // Create file stream
            int handle = Bass.BASS_StreamCreateFile(filePath, 0, 0, flags);
            if (handle == 0)
                Base.CheckForError();

            // Return new channel instance
            return new Channel(handle, ChannelType.Decode, true, useFloatingPoint);
        }

        /// <summary>
        /// Creates a stream for FX (such as time shifting). It needs an handle to the decode stream in order to work.
        /// </summary>
        /// <param name="streamHandle">Handle to the base stream</param>
        /// <param name="decode">Indicates if the file should be played or decoded</param>
        /// <param name="useFloatingPoint">Indicates if the channel should use floating point</param>
        /// <returns>Channel object</returns>
        public static Channel CreateStreamForTimeShifting(int streamHandle, bool decode, bool useFloatingPoint)
        {
            // Build flags; add base flags
            BASSFlag flags = BASSFlag.BASS_STREAM_PRESCAN;
            if (decode)
            {
                flags |= BASSFlag.BASS_STREAM_DECODE;
            }
            else
            {
                // No decode; it needs BASS_FX_FREESOURCE instead
                flags |= BASSFlag.BASS_FX_FREESOURCE;
            }
            if (useFloatingPoint)
                flags |= BASSFlag.BASS_SAMPLE_FLOAT;

            // Create file stream            
            int handle = BassFx.BASS_FX_TempoCreate(streamHandle, flags); 
            if (handle == 0)
                Base.CheckForError();

            // Return new channel instance
            return new Channel(handle, ChannelType.FX, decode, useFloatingPoint);
        }

        #endregion

        #region Free / Active Status
        
        /// <summary>
        /// Frees the stream.
        /// </summary>
        public void Free()
        {
            // Free stream
            if (!Bass.BASS_StreamFree(handle))
            {
                // Check for error
                Base.CheckForError();
            }
        }

        /// <summary>
        /// Indicates if the channel/stream is currently active or stopped.
        /// </summary>
        /// <returns>BASSActive structure</returns>
        public BASSActive IsActive()
        {
            return Bass.BASS_ChannelIsActive(handle);
        }

        #endregion

        #region Playback

        /// <summary>
        /// Starts the playback of a channel.
        /// </summary>
        /// <param name="restart">Restart playback from the beginning</param>
        public void Play(bool restart)
        {
            // Start playback
            if (!Bass.BASS_ChannelPlay(handle, restart))
            {
                // Check for error
                Base.CheckForError();
            }
        }

        /// <summary>
        /// Stops the playback of a channel.
        /// </summary>
        public void Stop()
        {
            // Stop playback
            if (!Bass.BASS_ChannelStop(handle))
            {
                // Check for error
                Base.CheckForError();
            }
        }

        /// <summary>
        /// Pauses the playback of a channel.
        /// </summary>        
        public void Pause()
        {                        
            // Pause playback
            if (!Bass.BASS_ChannelPause(handle))
            {
                // Check for error
                Base.CheckForError();
            }
        }

        #endregion

        #region Effects

        /// <summary>
        /// Sets an effect on the current channel.
        /// </summary>
        /// <param name="type">Effect type</param>
        /// <param name="priority">Effect priority</param>
        /// <returns>Effect handle</returns>
        public int SetFX(BASSFXType type, int priority)
        {
            return Bass.BASS_ChannelSetFX(handle, type, priority);
        }

        /// <summary>
        /// Removes an effect from the current channel.
        /// </summary>
        /// <param name="handleFX">Effect handle</param>
        public void RemoveFX(int handleFX)
        {
            // Remove FX
            if (!Bass.BASS_ChannelRemoveFX(handle, handleFX))
            {
                // Check for error
                Base.CheckForError();
            }
        }

        /// <summary>
        /// Resets the settings of an effect on the current channel.
        /// </summary>
        /// <param name="handleFX">Effect handle</param>
        public void ResetFX(int handleFX)
        {
            // Remove FX
            if (!Bass.BASS_FXReset(handleFX))
            {
                // Check for error
                Base.CheckForError();
            }
        }

        #endregion

        #region Tempo / SampleRate

        /// <summary>
        /// Sets the "tempo" of the current channel (for time shifting).
        /// Note: needs to be an FX channel.
        /// </summary>
        /// <param name="tempo">Tempo (in BPM)</param>
        public void SetTempo(float tempo)
        {
            // Set value
            if (!Bass.BASS_ChannelSetAttribute(handle, BASSAttribute.BASS_ATTRIB_TEMPO, tempo))
            {
                // Check for error
                Base.CheckForError();
            }
        }

        /// <summary>
        /// Returns the sample rate of the current channel.
        /// </summary>
        /// <returns>Sample rate (in Hz)</returns>
        public int GetSampleRate()
        {
            float sampleRate = 0;
            GetAttribute(BASSAttribute.BASS_ATTRIB_FREQ, ref sampleRate);
            this.sampleRate = (int)sampleRate;
            return this.sampleRate;
        }

        /// <summary>
        /// Sets the sample rate of the current channel.
        /// </summary>
        /// <param name="sampleRate">Sample rate (in Hz)</param>
        public void SetSampleRate(int sampleRate)
        {
            this.sampleRate = (int)sampleRate;
            SetAttribute(BASSAttribute.BASS_ATTRIB_FREQ, (float)sampleRate);
        }

        #endregion

        #region Position / Length
        
        /// <summary>
        /// Gets the length of the channel in bytes.
        /// </summary>
        /// <returns>Length (in bytes)</returns>
        public long GetLength()
        {
            // Get length
            long length = Bass.BASS_ChannelGetLength(handle);

            //// Check for floating point
            //if (m_isFloatingPoint)
            //{
            //    // Convert 32-bit into 16-bit
            //    length = length / 2;
            //}

            return length;
        }

        /// <summary>
        /// Returns the current position of the channel/stream in bytes.
        /// </summary>
        /// <returns>Position (in bytes)</returns>
        public long GetPosition()
        {
            // Get position
            long position = Bass.BASS_ChannelGetPosition(handle);

            //// Check for floating point
            //if (m_isFloatingPoint)
            //{
            //    // Convert 32-bit into 16-bit
            //    position = position / 2;
            //}
            
            return position;
        }

        /// <summary>
        /// Sets the position of the channel/stream in bytes.
        /// </summary>
        /// <param name="position">Position (in bytes)</param>
        public void SetPosition(long position)
        {
            //// Check for floating point
            //long newPosition = position;
            //if (m_isFloatingPoint)
            //{
            //    newPosition = newPosition * 2;
            //}

            // Set position
            if (!Bass.BASS_ChannelSetPosition(handle, position))
            {
                // Check for error
                Base.CheckForError();
            }            
        }

        /// <summary>
        /// Locks or unlocks the position of the channel.
        /// </summary>
        /// <param name="state">If true, the position will be locked</param>
        public void Lock(bool state)
        {
            // Set position
            if (!Bass.BASS_ChannelLock(handle, state))
            {
                // Check for error
                Base.CheckForError();
            }    
        }

        #endregion

        #region Flags / Attributes

        /// <summary>
        /// Gets an attribute value of the current channel (specified in the attribute property).
        /// Returns the attribute value in the value property.
        /// </summary>
        /// <param name="attribute">Channel attribute</param>
        /// <param name="value">Value</param>
        public void GetAttribute(BASSAttribute attribute, ref float value)
        {
            // Get attribute value
            if (!Bass.BASS_ChannelGetAttribute(handle, attribute, ref value))
            {
                // Check for error
                Base.CheckForError();
            }
        }

        /// <summary>
        /// Sets an attribute value of the current channel (specified in the attribute property).
        /// </summary>
        /// <param name="attribute">Channel attribute</param>
        /// <param name="value">Value</param>
        public void SetAttribute(BASSAttribute attribute, float value)
        {
            // Set attribute value
            if (!Bass.BASS_ChannelSetAttribute(handle, attribute, value))
            {
                // Check for error
                Base.CheckForError();
            }
        }        

        /// <summary>
        /// Sets a flag on the current channel (specified in the flags property).
        /// </summary>
        /// <param name="flags">Flags</param>
        /// <param name="mask">Mask</param>
        /// <returns>Flag</returns>
        public BASSFlag SetFlags(BASSFlag flags, BASSFlag mask)
        {
            // Set flags
            return Bass.BASS_ChannelFlags(handle, flags, mask);
        }

        #endregion

        #region Data

        /// <summary>
        /// Gets data from the channel/stream buffer.
        /// </summary>
        /// <param name="buffer">Buffer to receive data</param>
        /// <param name="length">Data length</param>
        /// <returns>GetData return value</returns>
        public int GetData(byte[] buffer, int length)
        {
            return Bass.BASS_ChannelGetData(handle, buffer, length);
        }

        /// <summary>
        /// Gets data from the channel/stream buffer.
        /// </summary>
        /// <param name="buffer">Buffer to receive data</param>
        /// <param name="length">Data length</param>
        /// <returns>GetData return value</returns>
        public int GetData(IntPtr buffer, int length)
        {
            return Bass.BASS_ChannelGetData(handle, buffer, length);
        }

        /// <summary>
        /// Gets data from the channel/stream buffer.
        /// </summary>
        /// <param name="buffer">Buffer to receive data</param>
        /// <param name="length">Data length</param>
        /// <returns>GetData return value</returns>
        public int GetData(float[] buffer, int length)
        {
            return Bass.BASS_ChannelGetData(handle, buffer, length);
        }

        /// <summary>
        /// Gets data from the channel/stream buffer.
        /// </summary>
        /// <param name="buffer">Buffer to receive data</param>
        /// <param name="length">Data length</param>
        /// <returns>GetData return value</returns>
        public int GetData(int[] buffer, int length)
        {
            return Bass.BASS_ChannelGetData(handle, buffer, length);
        }

        /// <summary>
        /// Returns the amount of data the channel has buffered.
        /// </summary>
        /// <returns>
        /// The data available.
        /// </returns>
        public int GetDataAvailable()
        {
            return Bass.BASS_ChannelGetData(handle, new short[0], (int)BASSData.BASS_DATA_AVAILABLE);
        }

        /// <summary>
        /// Gets data from a mixer channel/stream buffer (32-bit integers for non-floating point channels).
        /// </summary>
        /// <param name="buffer">Buffer to receive data</param>
        /// <param name="length">Data length</param>
        /// <returns>GetData return value</returns>
        public int GetMixerData(int[] buffer, int length)
        {
            return BassMix.BASS_Mixer_ChannelGetData(handle, buffer, length);
        }

        /// <summary>
        /// Gets data from a mixer channel/stream buffer (floats for floating point channels).
        /// </summary>
        /// <param name="buffer">Buffer to receive data</param>
        /// <param name="length">Data length</param>
        /// <returns>GetData return value</returns>
        public int GetMixerData(float[] buffer, int length)
        {
            return BassMix.BASS_Mixer_ChannelGetData(handle, buffer, length);
        }

        /// <summary>
        /// Returns the amount of data the mixer channel has buffered.
        /// </summary>
        /// <returns>
        /// The mixer data available.
        /// </returns>
        public int GetMixerDataAvailable()
        {
            int code = BassMix.BASS_Mixer_ChannelGetData(handle, new short[0], (int)BASSData.BASS_DATA_AVAILABLE); 
            if(code == -1)
                Base.CheckForError();

            return code;
        }

        public void PushData(IntPtr buffer, int length)
        {
            int code = Bass.BASS_StreamPutData(handle, buffer, length);
            if(code == -1)
                Base.CheckForError();
        }

        public void PushData(byte[] buffer, int length)
        {
            int code = Bass.BASS_StreamPutData(handle, buffer, length);
            if(code == -1)
                Base.CheckForError();
        }

        public void PushData(float[] buffer, int length)
        {
            int code = Bass.BASS_StreamPutData(handle, buffer, length);
            if(code == -1)
                Base.CheckForError();
        }

        #endregion

        #region Synchronization Callbacks

        /// <summary>
        /// Sets a synchronization callback.
        /// </summary>
        /// <param name="type">Sync type</param>
        /// <param name="param">Parameter (depends on sync type)</param>
        /// <param name="syncProc">Instance of the synchronization callback</param>
        /// <returns>Synchronization callback handle</returns>
        public int SetSync(BASSSync type, long param, SYNCPROC syncProc)
        {
            // Set sync
            int syncHandle = Bass.BASS_ChannelSetSync(handle, type, param, syncProc, IntPtr.Zero);

            // Check for error
            if (syncHandle == 0)
            {
                Base.CheckForError();
            }

            return syncHandle;
        }

        /// <summary>
        /// Removes a synchronization callback.
        /// </summary>
        /// <param name="syncHandle">Handle to the synchronization callback</param>
        public void RemoveSync(int syncHandle)
        {
            // Remove the sync callback
            if (!Bass.BASS_ChannelRemoveSync(handle, syncHandle))
            {
                // Check for error
                Base.CheckForError();
            }
        }

        #endregion

        #region Conversion

        /// <summary>
        /// Converts milliseconds into bytes using the current channel properties.
        /// </summary>
        /// <param name="position">Position (in milliseconds)</param>
        /// <returns>Position (in bytes)</returns>
        public long Seconds2Bytes(double position)
        {
            return Bass.BASS_ChannelSeconds2Bytes(handle, position);
        }

        /// <summary>
        /// Converts bytes into milliseconds using the current channel properties.
        /// </summary>
        /// <param name="position">Position (in bytes)</param>
        /// <returns>Position (in milliseconds)</returns>
        public double Bytes2Seconds(long position)
        {
            return Bass.BASS_ChannelBytes2Seconds(handle, position);
        }

        #endregion

        #region Mixer

        /// <summary>
        /// Adds a source channel to the mixer. Only for mixer channels.
        /// </summary>
        /// <param name="channelHandle">Source channel handle</param>
        public void AddChannel(int channelHandle)
        {
            // Add channel
            bool success = BassMix.BASS_Mixer_StreamAddChannel(handle, channelHandle, BASSFlag.BASS_MIXER_BUFFER);
            if (!success)
            {
                // Check for error
                Base.CheckForError();
            }
        }

        #endregion
    }
}
