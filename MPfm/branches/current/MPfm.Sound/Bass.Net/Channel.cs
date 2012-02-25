//
// Channel.cs: Defines a channel/stream to be used with BASS.NET.
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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using MPfm.Core;
using MPfm.Sound;
using Un4seen.Bass;
using Un4seen.BassAsio;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.Bass.AddOn.Fx;

namespace MPfm.Sound.BassNetWrapper
{
    /// <summary>
    /// Defines a channel/stream to be used with BASS.NET.
    /// If you need to create a mixer channel, use the MixerChannel class.
    /// </summary>
    public class Channel
    {
        #region Properties
     
        /// <summary>
        /// Private value for the ChannelType property.
        /// </summary>
        protected ChannelType m_channelType = ChannelType.Playback;
        /// <summary>
        /// Indicates the type of channel (playback, FX, etc.)
        /// </summary>
        public ChannelType ChannelType
        {
            get
            {
                return m_channelType;
            }
        }

        /// <summary>
        /// Private value for the IsDecode property.
        /// </summary>
        protected bool m_isDecode = false;
        /// <summary>
        /// Indicates if the channel/stream is decoding.
        /// </summary>
        public bool IsDecode
        {
            get
            {
                return m_isDecode;
            }            
        }

        /// <summary>
        /// Private value for the IsFloatingPoint property.
        /// </summary>
        protected bool m_isFloatingPoint = false;
        /// <summary>
        /// Indicates if the channel/stream is using floating point.
        /// </summary>
        public bool IsFloatingPoint
        {
            get
            {
                return m_isFloatingPoint;
            }
        }

        /// <summary>
        /// Private value for the Handle property.
        /// </summary>
        protected int m_handle = 0;
        /// <summary>
        /// Handle to the channel.
        /// </summary>
        public int Handle
        {
            get
            {
                return m_handle;
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
        protected int m_sampleRate = 0;
        /// <summary>
        /// Defines the sample rate used for the channel.
        /// To fetch the "live" sample rate, use GetSampleRate().
        /// To set the sample rate, use SetSampleRate(). This will also set this value.
        /// </summary>
        public int SampleRate
        {
            get
            {
                if (m_sampleRate == 0)
                {
                    try
                    {
                        m_sampleRate = GetSampleRate();
                    }
                    catch
                    {
                        // Leave to 0
                    }
                }

                return m_sampleRate;
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
            m_handle = handle;
            m_channelType = channelType;
            m_isDecode = isDecode;
            m_isFloatingPoint = isFloatingPoint;
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
            // Build flags; add base flags
            BASSFlag flags = BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_STREAM_PRESCAN;
            if (useFloatingPoint)
            {
                // Add floating point
                flags |= BASSFlag.BASS_SAMPLE_FLOAT;
            }

            // Create file stream
            int handle = Bass.BASS_StreamCreate(frequency, numberOfChannels, flags, streamProc, IntPtr.Zero);
            if (handle == 0)
            {
                // Check for error
                Base.CheckForError();
            }            
            
            // Return new channel instance
            return new Channel(handle, ChannelType.Memory, true, useFloatingPoint) { m_sampleRate = frequency };
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
            {
                // Add floating point
                flags |= BASSFlag.BASS_SAMPLE_FLOAT;
            }

            // Create file stream
            int handle = Bass.BASS_StreamCreateFile(filePath, 0, 0, flags);
            if (handle == 0)
            {
                // Check for error
                Base.CheckForError();
            }

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
                // Add decode
                flags |= BASSFlag.BASS_STREAM_DECODE;
            }
            else
            {
                // No decode; it needs BASS_FX_FREESOURCE instead
                flags |= BASSFlag.BASS_FX_FREESOURCE;
            }
            if (useFloatingPoint)
            {
                // Add floating point
                flags |= BASSFlag.BASS_SAMPLE_FLOAT;
            }

            // Create file stream            
            int handle = BassFx.BASS_FX_TempoCreate(streamHandle, flags); 
            if (handle == 0)
            {
                // Check for error
                Base.CheckForError();
            }

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
            if (!Bass.BASS_StreamFree(m_handle))
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
            return Bass.BASS_ChannelIsActive(m_handle);
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
            if (!Bass.BASS_ChannelPlay(m_handle, restart))
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
            if (!Bass.BASS_ChannelStop(m_handle))
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
            if (!Bass.BASS_ChannelPause(m_handle))
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
            return Bass.BASS_ChannelSetFX(m_handle, type, priority);
        }

        /// <summary>
        /// Removes an effect from the current channel.
        /// </summary>
        /// <param name="handleFX">Effect handle</param>
        public void RemoveFX(int handleFX)
        {
            // Remove FX
            if (!Bass.BASS_ChannelRemoveFX(m_handle, handleFX))
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
            if (!Bass.BASS_ChannelSetAttribute(m_handle, BASSAttribute.BASS_ATTRIB_TEMPO, tempo))
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
            m_sampleRate = (int)sampleRate;
            return m_sampleRate;
        }

        /// <summary>
        /// Sets the sample rate of the current channel.
        /// </summary>
        /// <param name="sampleRate">Sample rate (in Hz)</param>
        public void SetSampleRate(int sampleRate)
        {
            m_sampleRate = (int)sampleRate;
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
            long length = Bass.BASS_ChannelGetLength(m_handle);

            // Check for floating point
            if (m_isFloatingPoint)
            {
                // Convert 32-bit into 16-bit
                length = length / 2;
            }

            return length;
        }

        /// <summary>
        /// Returns the current position of the channel/stream in bytes.
        /// </summary>
        /// <returns>Position (in bytes)</returns>
        public long GetPosition()
        {
            // Get position
            long position = Bass.BASS_ChannelGetPosition(m_handle);

            // Check for floating point
            if (m_isFloatingPoint)
            {
                // Convert 32-bit into 16-bit
                position = position / 2;
            }
            
            return position;
        }

        /// <summary>
        /// Sets the position of the channel/stream in bytes.
        /// </summary>
        /// <param name="position">Position (in bytes)</param>
        public void SetPosition(long position)
        {
            // Check for floating point
            long newPosition = position;
            if (m_isFloatingPoint)
            {
                newPosition = newPosition * 2;
            }

            // Set position
            if (!Bass.BASS_ChannelSetPosition(m_handle, newPosition))
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
            if (!Bass.BASS_ChannelLock(m_handle, state))
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
            if (!Bass.BASS_ChannelGetAttribute(m_handle, attribute, ref value))
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
            if (!Bass.BASS_ChannelSetAttribute(m_handle, attribute, value))
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
            return Bass.BASS_ChannelFlags(m_handle, flags, mask);
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
            return Bass.BASS_ChannelGetData(m_handle, buffer, length);
        }

        /// <summary>
        /// Gets data from the channel/stream buffer.
        /// </summary>
        /// <param name="buffer">Buffer to receive data</param>
        /// <param name="length">Data length</param>
        /// <returns>GetData return value</returns>
        public int GetData(IntPtr buffer, int length)
        {
            return Bass.BASS_ChannelGetData(m_handle, buffer, length);
        }

        /// <summary>
        /// Gets data from the channel/stream buffer.
        /// </summary>
        /// <param name="buffer">Buffer to receive data</param>
        /// <param name="length">Data length</param>
        /// <returns>GetData return value</returns>
        public int GetData(float[] buffer, int length)
        {
            return Bass.BASS_ChannelGetData(m_handle, buffer, length);
        }

        /// <summary>
        /// Gets data from a mixer channel/stream buffer.
        /// </summary>
        /// <param name="buffer">Buffer to receive data</param>
        /// <param name="length">Data length</param>
        /// <returns>GetData return value</returns>
        public int GetMixerData(float[] buffer, int length)
        {
            return BassMix.BASS_Mixer_ChannelGetData(m_handle, buffer, length);
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
            return Bass.BASS_ChannelSetSync(m_handle, type, param, syncProc, IntPtr.Zero);
        }

        /// <summary>
        /// Removes a synchronization callback.
        /// </summary>
        /// <param name="syncHandle">Handle to the synchronization callback</param>
        public void RemoveSync(int syncHandle)
        {
            // Remove the sync callback
            if (!Bass.BASS_ChannelRemoveSync(m_handle, syncHandle))
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
        public long Seconds2Bytes2(double position)
        {
            return Bass.BASS_ChannelSeconds2Bytes(m_handle, position);
        }

        /// <summary>
        /// Converts bytes into milliseconds using the current channel properties.
        /// </summary>
        /// <param name="position">Position (in bytes)</param>
        /// <returns>Position (in milliseconds)</returns>
        public double Bytes2Seconds(long position)
        {
            return Bass.BASS_ChannelBytes2Seconds(m_handle, position);
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
            bool success = BassMix.BASS_Mixer_StreamAddChannel(m_handle, channelHandle, BASSFlag.BASS_MIXER_BUFFER);
            if (!success)
            {
                // Check for error
                Base.CheckForError();
            }
        }

        #endregion
    }
}
