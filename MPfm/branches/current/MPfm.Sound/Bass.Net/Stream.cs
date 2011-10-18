﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Flac;
using Un4seen.Bass.AddOn.Fx;

namespace MPfm.Sound.BassNetWrapper
{
    public class Stream
    {
        private int m_handle = 0;
        public int Handle
        {
            get
            {
                return m_handle;
            }
        }

        public Stream(int handle)
        {
            m_handle = handle;
        }

        public static Stream CreateFileStreamForDecoding(string filePath)
        {
            // Create file stream
            int handle = Bass.BASS_StreamCreateFile(filePath, 0, 0, BASSFlag.BASS_STREAM_DECODE);
            if (handle == 0)
            {
                // Check for error
                System.CheckForError();
            }

            return new Stream(handle);
        }

        public static Stream CreateStreamForTimeShifting(int streamHandle, bool decode)
        {
            // Create file stream
            int handle = BassFx.BASS_FX_TempoCreate(streamHandle, decode ? BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT : BASSFlag.BASS_SAMPLE_FLOAT);
            if (handle == 0)
            {
                // Check for error
                System.CheckForError();
            }

            return new Stream(handle);
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

        public void Play(bool restart)
        {
            // Start playback
            if (!Bass.BASS_ChannelPlay(m_handle, restart))
            {
                // Check for error
                System.CheckForError();
            }
        }
    }
}
