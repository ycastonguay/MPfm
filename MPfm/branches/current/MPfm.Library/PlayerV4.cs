//
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

        // Private value for the Volume property.
        private float m_volume = 1.0f;
        /// <summary>
        /// Defines the master volume (from 0 to 1).
        /// </summary>
        public float Volume
        {
            get
            {
                return m_volume;
            }
            set
            {
                // Set value
                m_volume = value;

                // Check if the player is playing
                if (m_mainChannel != null)
                {
                    // Set main volume
                    m_mainChannel.Volume = value;
                }
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

        // Private value for the FilePaths property.
        private List<string> m_filePaths = null;
        public List<string> FilePaths
        {
            get
            {
                return m_filePaths;
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

        private PlayerV4Channel m_currentSubChannel = null;
        /// <summary>
        /// Returns the currently playing sub channel.
        /// </summary>
        public PlayerV4Channel CurrentSubChannel
        {
            get
            {
                return m_currentSubChannel;
            }
        }

        /// <summary>
        /// Timer for the player.
        /// </summary>
        private System.Timers.Timer m_timerPlayer = null;

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
            m_filePaths = new List<string>();
            //m_subChannels = new List<PlayerV4Channel>();

            // Create timer
            Tracing.Log("[PlayerV4.Initialize] Creating timer...");
            m_timerPlayer = new System.Timers.Timer();
            m_timerPlayer.Elapsed += new System.Timers.ElapsedEventHandler(m_timerPlayer_Elapsed);
            m_timerPlayer.Interval = 1000;
            m_timerPlayer.Enabled = false;
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
        /// Occurs when the timer for loading the next song in advance expires.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        protected void m_timerPlayer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Reset timer
            m_timerPlayer.Enabled = false;            

            // Create the next channel
            CreateChannel(ref m_currentSubChannel, m_filePaths[m_currentSubChannelIndex + 1]);
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

                // Reset flags
                m_currentSubChannelIndex = 0;

                // Set file paths
                m_filePaths = filePaths;

                // Create the first channel
                PlayerV4Channel channelNull = null;
                PlayerV4Channel channelOne = CreateChannel(ref channelNull, filePaths[0]);

                // Check if there are other files in the playlist
                if (filePaths.Count > 1)
                {
                    // Create the second channel
                    PlayerV4Channel channelTwo = CreateChannel(ref channelOne, filePaths[1]);
                }

                // Set the current channel as the first one
                m_currentSubChannel = channelOne;

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
                Tracing.Log("Error in PlayerV4.PlayFiles: " + ex.Message + "\n" + ex.StackTrace);                
                throw ex;
            }
        }

        /// <summary>
        /// Creates a channel to be used in the player.
        /// </summary>
        /// <param name="previousChannel">Pointer to the previous channel</param>
        /// <param name="filePath">File path to the audio file</param>
        /// <returns>Instance of the channel</returns>
        private PlayerV4Channel CreateChannel(ref PlayerV4Channel previousChannel, string filePath)
        {
            // Create channel file properties and BASS channel
            PlayerV4Channel channel = new PlayerV4Channel();
            channel.FileProperties = new AudioFile(filePath);
            channel.Channel = MPfm.Sound.BassNetWrapper.Channel.CreateFileStreamForDecoding(filePath);

            // Set pointer to the previous channel
            channel.PreviousChannel = previousChannel;

            // Make sure there was a previous channel
            if (previousChannel != null)
            {
                // Set next channel for the previous channel
                previousChannel.NextChannel = channel;
            }

            // Return instance of the channel
            return channel;
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
            //m_mainChannel = null;

            if (m_currentSubChannel != null)
            {
                if (m_currentSubChannel.Channel != null)
                {
                    if (m_currentSubChannel.Channel.IsActive() == BASSActive.BASS_ACTIVE_PLAYING)
                    {
                        m_currentSubChannel.Channel.Stop();
                    }
                }

                if (m_currentSubChannel.NextChannel != null && 
                    m_currentSubChannel.NextChannel.Channel != null)
                {
                    if (m_currentSubChannel.NextChannel.Channel.IsActive() == BASSActive.BASS_ACTIVE_PLAYING)
                    {
                        m_currentSubChannel.NextChannel.Channel.Stop();
                    }
                }
            }

            //m_subChannels.Clear();
            //m_subChannels = null;

            // Set flag
            m_isPlaying = false;
        }

        /// <summary>
        /// Stops the playback and starts playback at a specific playlist song.
        /// </summary>
        /// <param name="index">Song index</param>
        public void GoTo(int index)
        {
            // Make sure index is in the list
            if (index <= m_filePaths.Count - 1)
            {
                // Stop playback
                Stop();

                // Set index
                m_currentSubChannelIndex = index;

                try
                {
                    // Create the first channel
                    PlayerV4Channel channelNull = null;
                    PlayerV4Channel channelOne = CreateChannel(ref channelNull, m_filePaths[m_currentSubChannelIndex]);

                    // Check if there are other files in the playlist
                    if (m_currentSubChannelIndex + 1 < m_filePaths.Count)
                    {
                        // Create the second channel
                        PlayerV4Channel channelTwo = CreateChannel(ref channelOne, m_filePaths[m_currentSubChannelIndex + 1]);
                    }

                    // Set the current channel as the first one
                    m_currentSubChannel = channelOne;

                    // Start playback
                    m_isPlaying = true;
                    m_isPaused = false;
                    m_mainChannel.Play(false);

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
                catch (Exception ex)
                {
                    Tracing.Log("Error in PlayerV4.GoTo: " + ex.Message + "\n" + ex.StackTrace);
                    throw ex;
                }
            }   
        }

        /// <summary>
        /// Goes back to the previous channel in the list.
        /// </summary>
        public void Previous()
        {
            // Check if there is a previous song
            if (m_currentSubChannelIndex > 0)
            {
                // Go to previous audio file
                GoTo(m_currentSubChannelIndex - 1);
            }
        }

        /// <summary>
        /// Skips to the next channel in the list.
        /// </summary>
        public void Next()
        {
            // Check if there is a next song
            if (m_currentSubChannelIndex < m_filePaths.Count - 1)
            {
                // Go to next audio file
                GoTo(m_currentSubChannelIndex + 1);
            }            
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
            // If the current sub channel is null, end the stream
            if (m_currentSubChannel == null)
            {
                // Return end-of-channel
                return (int)BASSStreamProc.BASS_STREAMPROC_END;
            }

            // Get active status
            BASSActive status = m_currentSubChannel.Channel.IsActive();

            // Check the current channel status
            if (status == BASSActive.BASS_ACTIVE_PLAYING)
            {
                // Check if the next channel needs to be loaded
                if (m_currentSubChannelIndex < m_filePaths.Count - 1 &&
                    m_currentSubChannel.NextChannel == null)
                {
                    // Create the next channel using a timer
                    m_timerPlayer.Start();
                }

                // Get data from the current channel since it is running
                return m_currentSubChannel.Channel.GetData(buffer, length);
            }
            else if (status == BASSActive.BASS_ACTIVE_STOPPED)
            {
                // Check if there is another channel to load
                if (m_currentSubChannel.NextChannel == null)
                {
                    // Raise song end event (if an event is subscribed)
                    if (OnSongFinished != null)
                    {
                        // Create data
                        PlayerV4SongFinishedData data = new PlayerV4SongFinishedData();
                        data.IsPlaybackStopped = true;

                        // Raise event
                        OnSongFinished(data);
                    }   

                    // This is the end of the playlist                    
                    return (int)BASSStreamProc.BASS_STREAMPROC_END;
                }

                // Load next channel
                m_currentSubChannel = m_currentSubChannel.NextChannel;

                // Increment index
                m_currentSubChannelIndex++;

                // Raise song end event (if an event is subscribed)
                if (OnSongFinished != null)
                {
                    // Create data
                    PlayerV4SongFinishedData data = new PlayerV4SongFinishedData();
                    data.IsPlaybackStopped = false;

                    // Raise event
                    OnSongFinished(data);
                }

                // Return data from the new channel
                return m_currentSubChannel.Channel.GetData(buffer, length);
            }
            else if (status == BASSActive.BASS_ACTIVE_STALLED)
            {
                // WTF
            }
            else if(status == BASSActive.BASS_ACTIVE_PAUSED)
            {
                // Do nothing, right?
            }

            //// Decode data from the current sub stream
            //if (m_currentSubChannel != null)
            //{
            //    return m_currentSubChannel.Channel.GetData(buffer, length);
            //}

            //// Loop through channels (TODO: use current channel instead)
            //for (int a = 0; a < m_subChannels.Count; a++)
            //{
            //    // Get active status
            //    BASSActive status = m_subChannels[a].Channel.IsActive();

            //    // Check if channel is playing
            //    if (status == BASSActive.BASS_ACTIVE_PLAYING)
            //    {
            //        // Check if the current channel needs to be updated
            //        if (m_currentSubChannelIndex != a)
            //        {
            //            // Check if the repeat type is set to Song
            //            if (RepeatType == MPfm.Library.RepeatType.Song)
            //            {
            //                // Restart playback on the original channel, restarting the position to 0
            //                //m_subChannels[a].Channel.Play(true);   // SEEMS NOT TO WORK BECAUSE IT DOESNT HAVE THE TIME TO PLAY?
            //                //return m_subChannels[a].Channel.GetData(buffer, length);

            //                // MAYBE THE SOLUTION IS TO PREPARE THOSE CHANNELS IN ADVANCE.
            //            }

            //            // Set current channel
            //            m_currentSubChannelIndex = a;

            //            // Raise song end event (if an event is subscribed)
            //            if (OnSongFinished != null)
            //            {
            //                // Create data
            //                PlayerV4SongFinishedData data = new PlayerV4SongFinishedData();
            //                data.IsPlaybackStopped = false;

            //                // Raise event
            //                OnSongFinished(data);
            //            }   
            //        }

            //        // Loop prototype: working. 
            //        //// Check position
            //        //long positionBytes = m_subChannels[a].Channel.GetPosition();

            //        //if (positionBytes >= 1500000)
            //        //{
            //        //    m_subChannels[a].Channel.SetPosition(50000);
            //        //}

            //        // Return data
            //        return m_subChannels[a].Channel.GetData(buffer, length);
            //    }
            //}

            // This is the end of the playlist. Check the repeat type if the playlist needs to be repeated
            if (RepeatType == MPfm.Library.RepeatType.Playlist)
            {
                // Restart playback from the first channel

                // IDEA: USE GOTO?
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
}
