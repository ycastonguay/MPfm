//
// PlaylistItem.cs: This file contains the class defining a playlist item to 
//                  be used with the Player.
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
using MPfm.Core;
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;
using Un4seen.Bass;

namespace MPfm.Sound
{
    /// <summary>
    /// Defines a playlist item to be used with the Player.
    /// </summary>
    public class PlaylistItem
    {
        /// <summary>
        /// Private value for the Id property.
        /// </summary>
        private Guid m_id = Guid.Empty;
        /// <summary>
        /// Unique identifier for the playlist item (there might be the same 
        /// audio file multiple times in the same playlist).
        /// </summary>
        public Guid Id
        {
            get
            {
                return m_id;
            }
        }

        /// <summary>
        /// Pointer to the parent Playlist instance.
        /// </summary>
        private Playlist m_playlist = null;

        /// <summary>
        /// Private value for the SyncProc proprety.
        /// </summary>
        private SYNCPROC m_syncProc = null;
        /// <summary>
        /// Synchronization callback.
        /// </summary>
        public SYNCPROC SyncProc
        {
            get
            {
                return m_syncProc;
            }
            set
            {
                m_syncProc = value;
            }
        }

        /// <summary>
        /// Private value for the SyncProcHandle property.
        /// </summary>
        private int m_syncProcHandle = 0;
        /// <summary>
        /// Contains the handle to the SYNCPROC.
        /// </summary>
        public int SyncProcHandle
        {
            get
            {
                return m_syncProcHandle;
            }
            set
            {
                m_syncProcHandle = value;
            }
        }

        private long m_lengthSamples = 0;
        public long LengthSamples
        {
            get
            {
                return m_lengthSamples;
            }
        }

        private long m_lengthBytes = 0;
        public long LengthBytes
        {
            get
            {
                return m_lengthBytes;
            }
        }

        private int m_lengthMilliseconds = 0;
        public int LengthMilliseconds
        {
            get
            {
                return m_lengthMilliseconds;
            }
        }

        private string m_lengthString = string.Empty;
        public string LengthString
        {
            get
            {
                return m_lengthString;
            }
        }

        /// <summary>
        /// Private value for the Channel property.
        /// </summary>
        private Channel m_channel = null;
        /// <summary>
        /// BASS.NET channel used for playback decoding.
        /// </summary>
        public Channel Channel
        {
            get
            {
                return m_channel;
            }
        }

        /// <summary>
        /// Private value for the AudioFile property.
        /// </summary>
        private AudioFile m_audioFile = null;
        /// <summary>
        /// AudioFile structure containing metadata and other file information.
        /// </summary>
        public AudioFile AudioFile
        {
            get
            {
                return m_audioFile;
            }            
        }

        /// <summary>
        /// Private value for the IsLoaded property.
        /// </summary>
        private bool m_isLoaded = false;
        /// <summary>
        /// Indicates if the channel and the audio file metadata have been loaded.
        /// </summary>
        public bool IsLoaded
        {
            get
            {
                return m_isLoaded;
            }
        }

        /// <summary>
        /// Default constructor for the PlaylistItem class.
        /// Requires a hook to a Playlist instance.
        /// </summary>
        /// <param name="playlist">Playlist</param>
        /// <param name="audioFile">Audio file metadata</param>
        public PlaylistItem(Playlist playlist, AudioFile audioFile)
        {
            // Set properties
            m_id = Guid.NewGuid();
            m_playlist = playlist;
            m_audioFile = audioFile;
        }

        /// <summary>
        /// Loads the current channel and audio file metadata.
        /// </summary>
        public void Load()
        {
            // Load audio file metadata
            m_audioFile.RefreshMetadata();

            // Check if a channel already exists
            if (m_channel != null)
            {
                // Dispose channel
                Dispose();
            }

            // Load channel
            m_channel = MPfm.Sound.BassNetWrapper.Channel.CreateFileStreamForDecoding(m_audioFile.FilePath, true);

            // Load channel length
            m_lengthBytes = m_channel.GetLength();

            // For some reason this works instead of using the 96000 Hz and 24 bit values in the following equations.
            float ratio = (float)44100 / (float)m_audioFile.SampleRate;
            m_lengthBytes = (int)((float)m_lengthBytes * ratio);
            m_lengthSamples = ConvertAudio.ToPCM(m_lengthBytes, 16, 2);
            m_lengthMilliseconds = (int)ConvertAudio.ToMS(m_lengthSamples, 44100);
            m_lengthString = Conversion.MillisecondsToTimeString((ulong)m_lengthMilliseconds);

            // Set flag
            m_isLoaded = true;
        }

        /// <summary>
        /// Disposes the current channel.
        /// </summary>
        public void Dispose()
        {
            // Check if a channel already exists
            if (m_channel != null)
            {
                // Check if the channel is in use
                if (m_channel.IsActive() == BASSActive.BASS_ACTIVE_PLAYING)
                {
                    // Stop and free channel                    
                    m_channel.Stop();
                    m_channel.Free();
                    m_channel = null;
                }
            }

            // Set flags
            m_isLoaded = false;
        }
    }
}
