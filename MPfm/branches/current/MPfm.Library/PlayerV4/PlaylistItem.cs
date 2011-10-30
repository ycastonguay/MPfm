﻿//
// PlaylistItem.cs: This file contains the class defining a playlist item to 
//                  be used with PlayerV4.
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
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;
using Un4seen.Bass;

namespace MPfm.Library.PlayerV4
{
    /// <summary>
    /// Defines a playlist item to be used with PlayerV4.
    /// </summary>
    public class PlaylistItem
    {
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
        /// Private value for the FilePath property.
        /// </summary>
        private string m_filePath = string.Empty;
        /// <summary>
        /// File path to the audio file to play.
        /// </summary>
        public string FilePath
        {
            get
            {
                return m_filePath;
            }
        }

        private bool m_isLoaded = false;
        public bool IsLoaded
        {
            get
            {
                return m_isLoaded;
            }
        }

        /// <summary>
        /// Default constructor for the PlaylistItem class.
        /// </summary>
        public PlaylistItem(Playlist playlist, string filePath)
        {
            m_playlist = playlist;            
            m_filePath = filePath;            
        }

        /// <summary>
        /// Loads the current channel and audio file metadata.
        /// </summary>
        public void Load()
        {
            // Check if the metadata has already been loaded
            if (m_audioFile == null)
            {
                // Load audio file metadata
                m_audioFile = new AudioFile(m_filePath);
            }

            // Check if a channel already exists
            if (m_channel != null)
            {
                // Dispose channel
                Dispose();
            }

            // Load channel
            m_channel = MPfm.Sound.BassNetWrapper.Channel.CreateFileStreamForDecoding(m_filePath);

            // Set flag
            m_isLoaded = true;
        }

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
