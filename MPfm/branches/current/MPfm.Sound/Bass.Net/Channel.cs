//
// Channel.cs: This file contains the Channel class which is part of the
//             BASS.NET wrapper.
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
using Un4seen.Bass.AddOn.Flac;
using Un4seen.Bass.AddOn.Fx;

namespace MPfm.Sound.BassNetWrapper
{
    public class Channel
    {
        private int m_handle = 0;
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
        /// Default constructor for Channel. To create a new channel, use one
        /// of the static methods of this class.
        /// </summary>
        /// <param name="handle">Handle to the BASS.NET channel</param>
        public Channel(int handle)
        {
            // Set current handle
            m_handle = handle;
        }

        public static Channel CreateStream(int frequency, int numberOfChannels, STREAMPROC streamProc)
        {
            // Create file stream
            int handle = Bass.BASS_StreamCreate(frequency, numberOfChannels, BASSFlag.BASS_STREAM_DECODE, streamProc, IntPtr.Zero);
            if (handle == 0)
            {
                // Check for error
                System.CheckForError();
            }

            return new Channel(handle);
        }

        public static Channel CreateFileStreamForDecoding(string filePath)
        {
            // Create file stream
            int handle = Bass.BASS_StreamCreateFile(filePath, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_STREAM_PRESCAN);
            if (handle == 0)
            {
                // Check for error
                System.CheckForError();
            }

            return new Channel(handle);
        }

        public static Channel CreateStreamForTimeShifting(int streamHandle, bool decode)
        {
            // Create file stream
            //int handle = BassFx.BASS_FX_TempoCreate(streamHandle, decode ? BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_STREAM_PRESCAN | BASSFlag.BASS_SAMPLE_FLOAT : BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_PRESCAN);
            int handle = 0;

            if (decode)
            {
                handle = BassFx.BASS_FX_TempoCreate(streamHandle, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_STREAM_PRESCAN | BASSFlag.BASS_SAMPLE_FLOAT);
            }
            else
            {
                handle = BassFx.BASS_FX_TempoCreate(streamHandle, BASSFlag.BASS_FX_FREESOURCE | BASSFlag.BASS_STREAM_PRESCAN | BASSFlag.BASS_SAMPLE_FLOAT);
            }

            if (handle == 0)
            {
                // Check for error
                System.CheckForError();
            }

            return new Channel(handle);
        }

        public void Free()
        {
            // Free stream
            if (!Bass.BASS_StreamFree(m_handle))
            {
                // Check for error
                System.CheckForError();
            }
        }

        public int GetData(byte[] buffer, int length)
        {
            return Bass.BASS_ChannelGetData(m_handle, buffer, length);
        }

        public int GetData(IntPtr buffer, int length)
        {
            return Bass.BASS_ChannelGetData(m_handle, buffer, length);
        }

        public BASSActive IsActive()
        {
            return Bass.BASS_ChannelIsActive(m_handle);
        }
        
        public void SetTempo(float tempo)
        {
            // Set value
            if (!Bass.BASS_ChannelSetAttribute(m_handle, BASSAttribute.BASS_ATTRIB_TEMPO, tempo))
            {
                // Check for error
                System.CheckForError();
            }
        }

        public int SetFX(BASSFXType type, int priority)
        {
            return Bass.BASS_ChannelSetFX(m_handle, type, priority);
        }

        public long GetPosition()
        {            
            return Bass.BASS_ChannelGetPosition(m_handle);
        }

        public void SetPosition(long position)
        {
            // Set position
            if (!Bass.BASS_ChannelSetPosition(m_handle, position))
            {
                // Check for error
                System.CheckForError();
            }
        }

        public int SetSync(BASSSync type, long param, SYNCPROC syncProc)
        {
            return Bass.BASS_ChannelSetSync(m_handle, type, param, syncProc, IntPtr.Zero);
        }

        public void RemoveSync(int syncHandle)
        {
            Bass.BASS_ChannelRemoveSync(m_handle, syncHandle);
        }

        public long GetLength()
        {
            return Bass.BASS_ChannelGetLength(m_handle);
        }

        public void GetAttribute(BASSAttribute attribute, ref float value)
        {
            // Get attribute value
            if (!Bass.BASS_ChannelGetAttribute(m_handle, attribute, ref value))
            {
                // Check for error
                System.CheckForError();
            }
        }

        public void SetAttribute(BASSAttribute attribute, float value)
        {
            // Set attribute value
            if (!Bass.BASS_ChannelSetAttribute(m_handle, attribute, value))
            {
                // Check for error
                System.CheckForError();
            }
        }

        public BASSFlag SetFlags(BASSFlag flags, BASSFlag mask)
        {
            return Bass.BASS_ChannelFlags(m_handle, flags, mask);
        }

        public long Seconds2Bytes2(double position)
        {
            return Bass.BASS_ChannelSeconds2Bytes(m_handle, position);
        }

        public double Bytes2Seconds(long position)
        {
            return Bass.BASS_ChannelBytes2Seconds(m_handle, position);
        }

        public void Play(bool restart)
        {
            // Start playback
            if (!Bass.BASS_ChannelPlay(m_handle, restart))
            {
                // Check for error
                System.CheckForError();
            }
        }

        public void Stop()
        {
            // Stop playback
            if (!Bass.BASS_ChannelStop(m_handle))
            {
                // Check for error
                System.CheckForError();
            }
        }

        public bool Pause()
        {
            return Bass.BASS_ChannelPause(m_handle);
        }
    }
}
