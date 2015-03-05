//// Copyright © 2011-2013 Yanick Castonguay
////
//// This file is part of Sessions.
////
//// Sessions is free software: you can redistribute it and/or modify
//// it under the terms of the GNU General Public License as published by
//// the Free Software Foundation, either version 3 of the License, or
//// (at your option) any later version.
////
//// Sessions is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//// GNU General Public License for more details.
////
//// You should have received a copy of the GNU General Public License
//// along with Sessions. If not, see <http://www.gnu.org/licenses/>.
//
//using System;
//using Un4seen.Bass;
//using Un4seen.Bass.AddOn.Mix;
//
//namespace Sessions.Sound.BassNetWrapper
//{
//    /// <summary>
//    /// Defines a mixer channel to be used with BASS.NET.
//    /// </summary>
//    public class MixerChannel : Channel, IMixerChannel
//    {        
//        #region Constructor
//        
//        /// <summary>
//        /// Default constructor for the MixerChannel class. To create a new channel, use one
//        /// of the static methods of this class.
//        /// </summary>
//        /// <param name="handle">Handle to the BASS.NET channel</param>
//        /// <param name="channelType">Type of channel (playback, FX, etc.)</param>
//        /// <param name="isDecode">Indicates if the channel is decoding</param>
//        /// <param name="isFloatingPoint">Indicates if the channel is using floating point</param>
//        public MixerChannel(int handle, ChannelType channelType, bool isDecode, bool isFloatingPoint) 
//            : base(handle, channelType, isDecode, isFloatingPoint)
//        {
//        }
//
//        #endregion
//
//        /// <summary>
//        /// Creates a mixer stream from one or multiple source channels.
//        /// </summary>
//        /// <param name="frequency">Frequency (sample rate)</param>
//        /// <param name="numberOfChannels">Number of channels (mono = 1, stereo = 2)</param>
//        /// <param name="useFloatingPoint">Indicates if the channel should use floating point</param>
//        /// <param name="useDecode">Indicates if the channel should be a decode channel</param>
//        /// <returns>Channel object</returns>
//        public static MixerChannel CreateMixerStream(int frequency, int numberOfChannels, bool useFloatingPoint, bool useDecode)
//        {
//            // Build flags; add base flags
//            BASSFlag flags = BASSFlag.BASS_DEFAULT;
//            if (useFloatingPoint && useDecode)
//            {
//                // Set flags
//                flags = BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE;
//            }
//            else if (useFloatingPoint && !useDecode)
//            {
//                // Set flags
//                flags = BASSFlag.BASS_SAMPLE_FLOAT;
//            }
//            else if (!useFloatingPoint && useDecode)
//            {
//                // Set flags
//                flags = BASSFlag.BASS_STREAM_DECODE;
//            }
//
//            // Create mixer stream            
//            int handle = BassMix.BASS_Mixer_StreamCreate(frequency, numberOfChannels, flags);
//            if (handle == 0)
//            {
//                // Check for error
//                Base.CheckForError();
//            }
//
//            // Return new channel instance
//            return new MixerChannel(handle, ChannelType.Memory, true, useFloatingPoint) { sampleRate = frequency };
//        }
//
//        /// <summary>
//        /// Returns the current position of a mixer channel in bytes.        
//        /// <para>
//        /// <para>
//        /// Note: The handle of the decode channel must be passed in parameter.        
//        /// </para>
//        /// </summary>
//        /// <param name="handle">Decode channel handle</param>
//        /// <returns>Position (in bytes)</returns>
//        public long GetPosition(int handle)
//        {
//            // Get position
//            long position = BassMix.BASS_Mixer_ChannelGetPosition(handle);
//
//            // Check for error
//            if (position == -1)
//            {
//                Base.CheckForError();
//            }
//
//            //// Check for floating point
//            //if (m_isFloatingPoint)
//            //{
//            //    // Convert 32-bit into 16-bit
//            //    position = position / 2;
//            //}
//
//            return position;
//        }
//
//        #region Synchronization Callbacks
//
//        ///// <summary>
//        ///// This method is not supported for a mixer channel because it requires an handle to the
//        ///// decoding channel.. Using this method will throw an exception. Use the other overloaded method instead.
//        ///// </summary>
//        ///// <returns>Null</returns>
//        //public new int SetSync(BASSSync type, long param, SYNCPROC syncProc)
//        //{
//        //    throw new NotSupportedException("This method is not supported for a mixer channel.");
//        //}
//
//        /// <summary>
//        /// Sets a synchronization callback for a mixer channel.
//        /// <para>
//        /// Note: The handle of the decode channel must be passed in parameter.        
//        /// </para>
//        /// </summary>        
//        /// <param name="handle">Decode channel handle</param>
//        /// <param name="type">Sync type</param>
//        /// <param name="param">Parameter (depends on sync type)</param>
//        /// <param name="syncProc">Instance of the synchronization callback</param>
//        /// <returns>Synchronization callback handle</returns>
//        public int SetSync(int handle, BASSSync type, long param, SYNCPROC syncProc)
//        {
//            // Set sync
//            int syncHandle = BassMix.BASS_Mixer_ChannelSetSync(handle, type, param, syncProc, IntPtr.Zero);
//
//            // Check for error
//            if (syncHandle == 0)
//            {
//                Base.CheckForError();
//            }
//
//            return syncHandle;
//        }
//
//        /// <summary>
//        /// Removes a synchronization callback for a mixer channel.
//        /// <para>
//        /// Note: The handle of the decode channel must be passed in parameter.        
//        /// </para>
//        /// </summary>
//        /// <param name="handle">Decode channel handle</param>
//        /// <param name="syncHandle">Handle to the synchronization callback</param>
//        public void RemoveSync(int handle, int syncHandle)
//        {
//            // Remove the sync callback
//            if (!BassMix.BASS_Mixer_ChannelRemoveSync(handle, syncHandle))
//            {
//                // Check for error
//                Base.CheckForError();
//            }
//        }
//
//        #endregion
//
// 
//    }
//}
