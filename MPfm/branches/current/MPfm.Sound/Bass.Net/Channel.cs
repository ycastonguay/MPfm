using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Un4seen.Bass;
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
            int handle = Bass.BASS_StreamCreate(frequency, numberOfChannels, BASSFlag.BASS_DEFAULT, streamProc, IntPtr.Zero);
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
            int handle = Bass.BASS_StreamCreateFile(filePath, 0, 0, BASSFlag.BASS_STREAM_DECODE);
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
            int handle = BassFx.BASS_FX_TempoCreate(streamHandle, decode ? BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT : BASSFlag.BASS_SAMPLE_FLOAT);
            if (handle == 0)
            {
                // Check for error
                System.CheckForError();
            }

            return new Channel(handle);
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
