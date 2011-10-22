﻿//
// PlayerV4.cs: This class is used for playing songs, sound files, gapless sequences
//              and more.
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
using MPfm.Sound.BassNetWrapper;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Flac;
using Un4seen.Bass.AddOn.Fx;

namespace MPfm.Library
{
    /// <summary>
    /// This is the main Player class which manages audio playback and the audio library.
    /// This is the MPfm Playback Engine V4.
    /// </summary>
    public class PlayerV4
    {
        // Callbacks
        private STREAMPROC m_streamProc;

        // Events
        public delegate void SongFinished(PlayerV4SongFinishedData data);
        /// <summary>
        /// The OnSongFinished event is triggered when a song has finished playing.
        /// </summary>
        public event SongFinished OnSongFinished;

        // Private value for the System property.
        private MPfm.Sound.BassNetWrapper.System m_system = null;
        /// <summary>
        /// System main audio class.
        /// </summary>
        public MPfm.Sound.BassNetWrapper.System System
        {
            get
            {
                return m_system;
            }
        }

        // Private value for the IsPlaying property.
        private bool m_isPlaying = false;
        /// <summary>
        /// Indicates if the player is currently playing an audio file.
        /// </summary>
        public bool IsPlaying
        {
            get
            {
                return m_isPlaying;
            }
        }

        // Private value for the IsPaused property.
        private bool m_isPaused = false;
        /// <summary>
        /// Indicates if the player is currently paused.
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return m_isPaused;
            }
        }

        // Private value for the RepeatType property.
        private RepeatType m_repeatType = RepeatType.Off;
        /// <summary>
        /// Repeat type (Off, Playlist, Song)
        /// </summary>
        public RepeatType RepeatType
        {
            get
            {
                return m_repeatType;
            }
            set
            {
                m_repeatType = value;
            }
        }

        // Private value for the MixerSampleRate property.
        private int m_mixerSampleRate = 44100;
        /// <summary>
        /// Defines the sample rate of the mixer.
        /// </summary>
        public int MixerSampleRate
        {
            get
            {
                return m_mixerSampleRate;
            }
        }

        // Private value for the BufferSize property.
        private int m_bufferSize = 500;
        /// <summary>
        /// Defines the buffer size (in milliseconds). Increase this value if older computers have trouble
        /// filling up the buffer in time.        
        /// Default value: 500ms. The default BASS value is 500ms.
        /// </summary>
        public int BufferSize
        {
            get
            {
                return m_bufferSize;
            }
            set
            {
                m_bufferSize = value;
            }
        }

        // Private value for the UpdatePeriod property.
        private int m_updatePeriod = 10;
        /// <summary>
        /// Defines how often BASS fills the buffer to make sure it is always full (in milliseconds).
        /// This affects the accuracy of the ChannelGetPosition value.
        /// Default value: 10ms. The default BASS value is 100ms.
        /// </summary>
        public int UpdatePeriod
        {
            get
            {
                return m_updatePeriod;
            }
            set
            {
                m_updatePeriod = value;
            }
        }

        // Private value for the MainChannel property.
        private Channel m_mainChannel = null;
        /// <summary>
        /// Pointer to the main channel.
        /// </summary>
        public Channel MainChannel
        {
            get
            {
                return m_mainChannel;
            }
        }
        
        // Private value for the SubChannels property.
        private List<PlayerV4Channel> m_subChannels = null;
        /// <summary>
        /// List of sub channels used for gapless playback into the main channel.
        /// </summary>
        public List<PlayerV4Channel> SubChannels
        {
            get
            {
                return m_subChannels;
            }
        }

        // Private value for the CurrentSubChannelIndex.
        private int m_currentSubChannelIndex = 0;
        /// <summary>
        /// Currently playing sub channel index.
        /// </summary>
        public int CurrentSubChannelIndex
        {
            get
            {
                return m_currentSubChannelIndex;
            }
        }

        /// <summary>
        /// Returns the currently playing sub channel.
        /// </summary>
        public PlayerV4Channel CurrentSubChannel
        {
            get
            {
                return m_subChannels[m_currentSubChannelIndex];
            }
        }

        /// <summary>
        /// Default constructor for the PlayerV4 class. Initializes the player using the default
        /// values (see property comments).
        /// </summary>
        public PlayerV4()
        {
            // Initialize system with default values
            Initialize();
        }

        /// <summary>
        /// Constructor for the PlayerV4 class which requires the mixer sample rate value to be passed
        /// in parameter.
        /// </summary>
        /// <param name="mixerSampleRate">Mixer sample rate (default: 44100 Hz)</param>
        /// <param name="bufferSize">Buffer size (default: 500 ms)</param>
        /// <param name="updatePeriod">Update period (default: 10 ms)</param>
        public PlayerV4(int mixerSampleRate, int bufferSize, int updatePeriod)
        {
            // Initialize system using specified values
            m_mixerSampleRate = mixerSampleRate;
            m_bufferSize = bufferSize;
            m_updatePeriod = updatePeriod;
            Initialize();
        }

        /// <summary>
        /// Initializes the player.
        /// </summary>        
        private void Initialize()
        {
            // Initialize player using default driver (DirectSound)
            m_system = new Sound.BassNetWrapper.System(DriverType.DirectSound, m_mixerSampleRate);

            // Load plugins
            m_system.LoadFlacPlugin();
            m_system.LoadFxPlugin();

            // Default BASS.NET configuration values:
            //
            // BASS_CONFIG_BUFFER: 500
            // BASS_CONFIG_UPDATEPERIOD: 100
            // BASS_CONFIG_UPDATETHREADS: 1

            // Set configuration for buffer and update period
            m_system.SetConfig(BASSConfig.BASS_CONFIG_BUFFER, m_bufferSize);
            m_system.SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, m_updatePeriod);

            // Create lists
            m_subChannels = new List<PlayerV4Channel>();
        }

        /// <summary>
        /// Disposes the PlayerV4 class. 
        /// Closes the resources to the main audio system and its plugins.
        /// </summary>
        public void Dispose()
        {
            // Dispose plugins
            m_system.FreeFxPlugin();
            m_system.FreeFlacPlugin();

            // Dispose system
            m_system.Free();
        }

        /// <summary>
        /// Plays the list of audio files specified in the filePaths parameter.
        /// </summary>
        /// <param name="filePaths">List of audio file paths</param>
        public void PlayFiles(List<string> filePaths)
        {
            // Check for null or empty list of file paths
            if (filePaths == null || filePaths.Count == 0)
            {
                throw new Exception("There must be at least one file in the filePaths parameter.");
            }

            // Check if all file paths exist
            Tracing.Log("[PlayerV4.PlayFiles] Playing a list of " + filePaths.Count.ToString() + " files.");
            foreach (string filePath in filePaths)
            {
                // Check if the file exists                
                if (!File.Exists(filePath))
                {
                    // Throw exception
                    throw new Exception("The file at " + filePath + " doesn't exist!");
                }
            }

            // Check if the player is currently playing
            if (m_isPlaying)
            {
                // Stop playback
                Stop();
            }

            try
            {

                // Reset channels (TODO: Check if channel is in use)
                m_subChannels = new List<PlayerV4Channel>();
                m_currentSubChannelIndex = 0;

                if (filePaths.Count == 1)
                {
                    // TODO Something
                }

                // Loop through the first two file paths
                for (int a = 0; a < filePaths.Count; a++)
                {
                    // Create audio file and sound objects
                    Tracing.Log("[PlayerV4.PlayFiles] Loading file " + (a + 1).ToString() + ": " + filePaths[a]);

                    // Create channel for decoding
                    // TODO: A file is already in used when playing again a playlist from the start. This happens only with MP3 files.
                    // It is used by the BASS.NET library or the TagLib library.
                    PlayerV4Channel channel = new PlayerV4Channel();
                    channel.FileProperties = new AudioFile(filePaths[a]);
                    channel.Channel = MPfm.Sound.BassNetWrapper.Channel.CreateFileStreamForDecoding(filePaths[a]);                                        

                    // Add channel to list
                    m_subChannels.Add(channel);
                }

                // Create the main channel
                m_streamProc = new STREAMPROC(FileProc);
                m_mainChannel = MPfm.Sound.BassNetWrapper.Channel.CreateStream(44100, 2, m_streamProc);

                // Start playback
                m_isPlaying = true;
                m_isPaused = false;
                m_mainChannel.Play(false);
            }
            catch (Exception ex)
            {
                Tracing.Log("Error in PlayerV4.PlayFiles: " + ex.Message);
                Tracing.Log(ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// Pauses the audio playback. Resumes the audio playback if it was paused.
        /// </summary>
        public void Pause()
        {
            if (!m_isPaused)
            {
                m_mainChannel.Pause();
            }
            else
            {
                m_mainChannel.Play(false);
            }

            m_isPaused = !m_isPaused;
        }

        /// <summary>
        /// Stops the audio playback and frees the resources used by the playback engine.
        /// </summary>
        public void Stop()
        {            
            // Stop main channel
            m_mainChannel.Stop();
            m_mainChannel = null;
            m_subChannels.Clear();
            m_subChannels = null;

            // Set flag
            m_isPlaying = false;
        }

        /// <summary>
        /// File callback routine used for reading audio files.
        /// </summary>
        /// <param name="handle">Handle to the channel</param>
        /// <param name="buffer">Buffer pointer</param>
        /// <param name="length">Audio length</param>
        /// <param name="user">User data</param>
        /// <returns>Audio data</returns>
        private int FileProc(int handle, IntPtr buffer, int length, IntPtr user)
        {
            // Loop through channels (TODO: use current channel instead)
            for (int a = 0; a < m_subChannels.Count; a++)
            {
                // Get active status
                BASSActive status = m_subChannels[a].Channel.IsActive();

                // Check if channel is playing
                if (status == BASSActive.BASS_ACTIVE_PLAYING)
                {
                    // Check if the current channel needs to be updated
                    if (m_currentSubChannelIndex != a)
                    {
                        // Set current channel
                        m_currentSubChannelIndex = a;

                        // Raise song end event (if an event is subscribed)
                        if (OnSongFinished != null)
                        {
                            // Create data
                            PlayerV4SongFinishedData data = new PlayerV4SongFinishedData();
                            data.IsPlaybackStopped = false;

                            // Raise event
                            OnSongFinished(data);
                        }   
                    }

                    // Loop prototype: working. 
                    //// Check position
                    //long positionBytes = m_subChannels[a].Channel.GetPosition();

                    //if (positionBytes >= 1500000)
                    //{
                    //    m_subChannels[a].Channel.SetPosition(50000);
                    //}

                    // Return data
                    return m_subChannels[a].Channel.GetData(buffer, length);
                }
            }

            // Raise song end event (if an event is subscribed)
            if (OnSongFinished != null)
            {
                // Create data
                PlayerV4SongFinishedData data = new PlayerV4SongFinishedData();
                data.IsPlaybackStopped = true;

                // Raise event
                OnSongFinished(data);
            }   

            // Return end-of-channel
            return (int)BASSStreamProc.BASS_STREAMPROC_END;
        }
    }

    /// <summary>
    /// Structure used for the PlayerV4 containing the audio file properties/metadata
    /// and the channel/stream properties.
    /// </summary>
    public class PlayerV4Channel
    {
        public AudioFile FileProperties { get; set; }
        public Channel Channel { get; set; }
    }

    /// <summary>
    /// Defines the data structure for the end-of-song event.
    /// </summary>
    public class PlayerV4SongFinishedData
    {
        /// <summary>
        /// Defines if the playback was stopped after the song was finished.
        /// i.e. if the RepeatType is off and the playlist is over, this property will be true.
        /// </summary>
        public bool IsPlaybackStopped { get; set; }
    }
}
