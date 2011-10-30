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
using Un4seen.BassAsio;
using Un4seen.BassWasapi;
using Un4seen.Bass.AddOn.Flac;
using Un4seen.Bass.AddOn.Fx;

namespace MPfm.Library.PlayerV4
{
    /// <summary>
    /// This is the main Player class which manages audio playback and the audio library.
    /// This is the MPfm Playback Engine V4.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Timer for the player.
        /// </summary>
        private System.Timers.Timer m_timerPlayer = null;
        private Channel m_streamChannel = null;
        private int m_syncProcHandle;
        private int m_fxEQHandle;
        private int m_flacPluginHandle = 0;        

        #region Callbacks
        
        // Callbacks
        private STREAMPROC m_streamProc;
        private SYNCPROC m_syncProc;
        private ASIOPROC m_asioProc;
        private WASAPIPROC m_wasapiProc;

        #endregion

        #region Events
        
        /// <summary>
        /// Delegate method for the OnSongFinished event.
        /// </summary>
        /// <param name="data">OnSongFinished data</param>
        public delegate void SongFinished(PlayerV4SongFinishedData data);
        /// <summary>
        /// The OnSongFinished event is triggered when a song has finished playing.
        /// </summary>
        public event SongFinished OnSongFinished;

        #endregion

        #region Properties
        
        ///// <summary>
        ///// Private value for the System property.
        ///// </summary>
        //private MPfm.Sound.BassNetWrapper.System m_system = null;
        ///// <summary>
        ///// System main audio class.
        ///// </summary>
        //public MPfm.Sound.BassNetWrapper.System System
        //{
        //    get
        //    {
        //        return m_system;
        //    }
        //}

        /// <summary>
        /// Private value for the IsPlaying property.
        /// </summary>
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

        /// <summary>
        /// Private value for the IsPaused property.
        /// </summary>
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

        /// <summary>
        /// Private value for the Device property.
        /// </summary>
        private Device m_device = null;
        /// <summary>
        /// Defines the currently used device for playback.
        /// </summary>
        public Device Device
        {
            get
            {
                return m_device;
            }
        }

        /// <summary>
        /// Private value for the IsDeviceInitialized property.
        /// </summary>
        private bool m_isDeviceInitialized = false;
        /// <summary>
        /// Indicates if the device (as in the Device property) is initialized.
        /// </summary>
        public bool IsDeviceInitialized
        {
            get
            {
                return m_isDeviceInitialized;
            }
        }       

        /// <summary>
        /// Private value for the RepeatType property.
        /// </summary>
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

                // Check if the current song exists
                if (m_currentSong != null)
                {
                    // Check if the repeat type is Song
                    if (m_repeatType == MPfm.Library.RepeatType.Song)
                    {
                        // Force looping
                        m_currentSong.Channel.SetFlags(BASSFlag.BASS_SAMPLE_LOOP, BASSFlag.BASS_SAMPLE_LOOP);

                        //m_syncProc = new SYNCPROC(EndSync);
                        //m_syncProcHandle = m_currentSubChannel.Channel.SetSync(BASSSync.BASS_SYNC_END | BASSSync.BASS_SYNC_MIXTIME, 0, m_syncProc);                    
                    }
                    else
                    {
                        // Remove looping
                        m_currentSong.Channel.SetFlags(BASSFlag.BASS_DEFAULT, BASSFlag.BASS_SAMPLE_LOOP);
                    }
                }
            }
        }

        /// <summary>
        /// Private value for the Volume property.
        /// </summary>
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

        /// <summary>
        /// Private value for the TimeShifting property.
        /// </summary>
        private float m_timeShifting = 0.0f;
        /// <summary>
        /// Defines the time shifting applied to the currently playing stream.
        /// Value range from -100.0f (-100%) to 100.0f (+100%). To reset, set to 0.0f.
        /// </summary>
        public float TimeShifting
        {
            get
            {
                return m_timeShifting;
            }
            set
            {
                m_timeShifting = value;

                // Check if the main channel exists
                if (m_mainChannel != null)
                {
                    // Set time shifting
                    m_mainChannel.SetAttribute(BASSAttribute.BASS_ATTRIB_TEMPO, m_timeShifting);
                }
            }           
        }

        /// <summary>
        /// Private value for the MixerSampleRate property.
        /// </summary>
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

        /// <summary>
        /// Private value for the BufferSize property.
        /// </summary>
        private int m_bufferSize = 100;
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

                //// Check if system exists
                //if (m_system != null)
                //{
                //    // Set configuration
                //    m_system.SetConfig(BASSConfig.BASS_CONFIG_BUFFER, m_bufferSize);
                //}   
                
                Base.SetConfig(BASSConfig.BASS_CONFIG_BUFFER, m_bufferSize);
            }
        }

        /// <summary>
        /// Private value for the UpdatePeriod property.
        /// </summary>
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

                // Set configuration
                Base.SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, m_updatePeriod);
            }
        }

        /// <summary>
        /// Private value for the UpdateThreads property.
        /// </summary>
        private int m_updateThreads = 1;
        /// <summary>
        /// Defines how many threads BASS can use to update playback buffers in parrallel.
        /// Note: The playback engine plays perfectly with just one update thread.
        /// Default value: 1 thread. The default BASS value is 1 thread.
        /// </summary>
        public int UpdateThreads
        {
            get
            {
                return m_updateThreads;
            }
            set
            {
                m_updateThreads = value;

                // Set configuration
                Base.SetConfig(BASSConfig.BASS_CONFIG_UPDATETHREADS, m_updateThreads);
            }
        }

        /// <summary>
        /// Private value for the MainChannel property.
        /// </summary>
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

        /// <summary>
        /// Private value for the FilePaths property.
        /// </summary>
        private List<string> m_filePaths = null;
        /// <summary>
        /// Defines the list of file paths in the playlist.
        /// </summary>
        public List<string> FilePaths
        {
            get
            {
                return m_filePaths;
            }
        }

        /// <summary>
        /// Private value for the CurrentSongIndex.
        /// </summary>
        private int m_currentSongIndex = 0;
        /// <summary>
        /// Currently playing song index.
        /// </summary>
        public int CurrentSongIndex
        {
            get
            {
                return m_currentSongIndex;
            }
        }

        /// <summary>
        /// Private value for the CurrentSong property.
        /// </summary>
        private Song m_currentSong = null;
        /// <summary>
        /// Returns the currently playing song.
        /// </summary>
        public Song CurrentSong
        {
            get
            {
                return m_currentSong;
            }
        }

        /// <summary>
        /// Private value for the CurrentEQPreset property.
        /// </summary>
        private EQPreset m_currentEQPreset = null;
        /// <summary>
        /// Defines the current EQ preset.
        /// </summary>
        public EQPreset CurrentEQPreset
        {
            get
            {
                return m_currentEQPreset;
            }
            set
            {
                m_currentEQPreset = value;
            }
        }

        #region Loops and Markers
        
        /// <summary>
        /// Private value for the Markers property.
        /// </summary>
        private List<Marker> m_markers = null;
        /// <summary>
        /// Defines a collection of markers.
        /// </summary>
        public List<Marker> Markers
        {
            get
            {
                return m_markers;
            }
        }

        /// <summary>
        /// Private value for the Loops property.
        /// </summary>
        private List<Loop> m_loops = null;
        /// <summary>
        /// Defines a collection of loops.
        /// </summary>
        public List<Loop> Loops
        {
            get
            {
                return m_loops;
            }
        }

        /// <summary>
        /// Private value for the CurrentLoop property.
        /// </summary>
        private Loop m_currentLoop = null;
        /// <summary>
        /// Defines the currently playing loop.
        /// </summary>
        public Loop CurrentLoop
        {
            get
            {
                return m_currentLoop;
            }
            set
            {
                m_currentLoop = value;
            }
        }

        #endregion

        #endregion

        #region Constructors, Initialization/Dispose

        /// <summary>
        /// Default constructor for the PlayerV4 class. Initializes the player using the default
        /// values (see property comments).
        /// </summary>
        public Player()
        {
            // Initialize system with default values
            Initialize(new Device(), 44100, 100, 10);
        }

        /// <summary>
        /// Constructor for the PlayerV4 class which requires the mixer sample rate value to be passed
        /// in parameter.
        /// </summary>
        /// <param name="device">Device output</param>
        /// <param name="mixerSampleRate">Mixer sample rate (default: 44100 Hz)</param>
        /// <param name="bufferSize">Buffer size (default: 500 ms)</param>
        /// <param name="updatePeriod">Update period (default: 10 ms)</param>
        public Player(Device device, int mixerSampleRate, int bufferSize, int updatePeriod)
        {
            // Initialize system with specific values.
            Initialize(device, mixerSampleRate, bufferSize, updatePeriod);
        }

        /// <summary>
        /// Initializes the player.
        /// </summary>        
        /// <param name="device">Device output</param>
        /// <param name="mixerSampleRate">Mixer sample rate (default: 44100 Hz)</param>
        /// <param name="bufferSize">Buffer size (default: 500 ms)</param>
        /// <param name="updatePeriod">Update period (default: 10 ms)</param> 
        private void Initialize(Device device, int mixerSampleRate, int bufferSize, int updatePeriod)
        {
            // Initialize system using specified values
            m_device = device;
            m_mixerSampleRate = mixerSampleRate;
            m_bufferSize = bufferSize;
            m_updatePeriod = updatePeriod;

            // Create lists
            m_filePaths = new List<string>();
            m_markers = new List<Marker>();
            m_loops = new List<Loop>();            

            // Create timer
            Tracing.Log("[PlayerV4.Initialize] Creating timer...");
            m_timerPlayer = new System.Timers.Timer();
            m_timerPlayer.Elapsed += new System.Timers.ElapsedEventHandler(m_timerPlayer_Elapsed);
            m_timerPlayer.Interval = 1000;
            m_timerPlayer.Enabled = false;

            // Load plugins
            m_flacPluginHandle = Base.LoadFlacPlugin();
            Base.LoadFxPlugin();

            // Create default EQ
            m_currentEQPreset = new EQPreset();

            // Initialize sound system
            InitializeDevice(m_device);
        }

        /// <summary>
        /// Initializes the default audio device for playback.
        /// </summary>
        public void InitializeDevice()
        {
            // Initialize default device
            InitializeDevice(new Device());
        }

        /// <summary>
        /// Initializes a specific audio device for playback.
        /// </summary>
        /// <param name="device">Audio device</param>
        public void InitializeDevice(Device device)
        {
            // Set properties
            m_device = device;

            // Check driver type
            if (m_device.DriverType == DriverType.DirectSound)
            {
                // Initialize sound system
                Base.Init(m_device.Id, m_mixerSampleRate, BASSInit.BASS_DEVICE_DEFAULT);
            }
            else if (m_device.DriverType == DriverType.ASIO)
            {
                // Initialize sound system
                Base.InitASIO(m_device.Id, m_mixerSampleRate, BASSInit.BASS_DEVICE_DEFAULT, BASSASIOInit.BASS_ASIO_THREAD);
            }
            else if (m_device.DriverType == DriverType.WASAPI)
            {
                // Create callback
                m_wasapiProc = new WASAPIPROC(WASAPICallback);

                // Initialize sound system
                Base.InitWASAPI(m_device.Id, m_mixerSampleRate, 2, BASSInit.BASS_DEVICE_DEFAULT, BASSWASAPIInit.BASS_WASAPI_SHARED, 0, 0, m_wasapiProc);
            }

            // Default BASS.NET configuration values:
            //
            // BASS_CONFIG_BUFFER: 500
            // BASS_CONFIG_UPDATEPERIOD: 100
            // BASS_CONFIG_UPDATETHREADS: 1

            // Set configuration for buffer and update period
            Base.SetConfig(BASSConfig.BASS_CONFIG_BUFFER, m_bufferSize);
            Base.SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, m_updatePeriod);

            // Set flags            
            m_isDeviceInitialized = true;
        }

        /// <summary>
        /// Frees the device currently used for playback.
        /// </summary>
        public void FreeDevice()
        {
            // Check if a device has been initialized
            if (!m_isDeviceInitialized)
            {
                return;
            }

            // Check driver type
            if (m_device.DriverType == DriverType.ASIO)
            {
                // Free ASIO device
                if (!BassAsio.BASS_ASIO_Free())
                {
                    // Get error
                    BASSError error = Bass.BASS_ErrorGetCode();
                    throw new Exception("Error freeing ASIO device: " + error.ToString());
                }
            }
            else if (m_device.DriverType == DriverType.WASAPI)
            {
                // Free WASAPI device
                if (!BassWasapi.BASS_WASAPI_Free())
                {
                    // Get error
                    BASSError error = Bass.BASS_ErrorGetCode();
                    throw new Exception("Error freeing WASAPI device: " + error.ToString());
                }
            }

            // Dispose system
            Base.Free();

            // Set flags
            m_device = null;
            m_isDeviceInitialized = false;
        }

        /// <summary>
        /// Frees the BASS plugins used by the player.
        /// </summary>
        public void FreePlugins()
        {
            // Dispose plugins
            Base.FreeFxPlugin();
            Base.FreeFlacPlugin(m_flacPluginHandle);
        }        

        /// <summary>
        /// Disposes the PlayerV4 class. 
        /// Closes the resources to the main audio system and its plugins.
        /// </summary>
        public void Dispose()
        {
            // Free device
            FreeDevice();

            // Free plugins
            FreePlugins();
        }        

        #endregion

        #region Timers
        
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
            CreateSong(ref m_currentSong, m_filePaths[m_currentSongIndex + 1]);

            // Set time shifting value
            //m_currentSubChannel.Channel.SetAttribute(BASSAttribute.BASS_ATTRIB_TEMPO, TimeShifting);
        }

        #endregion

        #region Playback Methods        

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
                m_currentSongIndex = 0;

                // Set file paths
                m_filePaths = filePaths;

                // Create the first channel
                Song channelNull = null;
                Song channelOne = CreateSong(ref channelNull, filePaths[0]);

                // Check if there are other files in the playlist
                if (filePaths.Count > 1)
                {
                    // Create the second channel
                    Song channelTwo = CreateSong(ref channelOne, filePaths[1]);
                }

                // Set the current channel as the first one
                m_currentSong = channelOne;

                // Create the streaming channel
                m_streamProc = new STREAMPROC(StreamCallback);
                m_streamChannel = MPfm.Sound.BassNetWrapper.Channel.CreateStream(m_mixerSampleRate, 2, m_streamProc);

                // Check driver type
                if (m_device.DriverType == DriverType.DirectSound)
                {
                    // Create main channel
                    m_mainChannel = MPfm.Sound.BassNetWrapper.Channel.CreateStreamForTimeShifting(m_streamChannel.Handle, false);
                }
                else if (m_device.DriverType == DriverType.ASIO)
                {
                    // Create main channel
                    m_mainChannel = MPfm.Sound.BassNetWrapper.Channel.CreateStreamForTimeShifting(m_streamChannel.Handle, true);

                    // Create callback
                    m_asioProc = new ASIOPROC(AsioCallback);

                    // Create channel
                    BassAsio.BASS_ASIO_ChannelEnable(false, 0, m_asioProc, new IntPtr(m_mainChannel.Handle));
                    BassAsio.BASS_ASIO_ChannelJoin(false, 1, 0);

                    // Start playback
                    if (!BassAsio.BASS_ASIO_Start(0))
                    {
                        // Get error
                        BASSError error = Bass.BASS_ErrorGetCode();
                        throw new Exception("[PlayerV4.PlayFiles] Error playing files in ASIO: " + error.ToString());
                    }
                }
                else if (m_device.DriverType == DriverType.WASAPI)
                {
                    // Create main channel
                    m_mainChannel = MPfm.Sound.BassNetWrapper.Channel.CreateStreamForTimeShifting(m_streamChannel.Handle, true);

                    // Start playback
                    if (!BassWasapi.BASS_WASAPI_Start())
                    {
                        // Get error
                        BASSError error = Bass.BASS_ErrorGetCode();
                        throw new Exception("[PlayerV4.PlayFiles] Error playing files in WASAPI: " + error.ToString());
                    }
                }

                // Load 18-band equalizer
                m_fxEQHandle = m_mainChannel.SetFX(BASSFXType.BASS_FX_BFX_PEAKEQ, 0);
                BASS_BFX_PEAKEQ eq = new BASS_BFX_PEAKEQ();
                m_currentEQPreset = new EQPreset();
                for (int a = 0; a < m_currentEQPreset.Bands.Count; a++)
                {
                    // Get current band
                    EQPresetBand currentBand = m_currentEQPreset.Bands[a];

                    // Set equalizer band properties
                    eq.lBand = a;
                    eq.lChannel = BASSFXChan.BASS_BFX_CHANALL;
                    eq.fCenter = currentBand.Center;
                    eq.fGain = currentBand.Gain;
                    eq.fQ = currentBand.Q;
                    Bass.BASS_FXSetParameters(m_fxEQHandle, eq);
                    UpdateEQ(a, currentBand.Gain);
                }

                // Check if the repeat type is Song
                if (m_repeatType == MPfm.Library.RepeatType.Song)
                {
                    // Force looping
                    m_currentSong.Channel.SetFlags(BASSFlag.BASS_SAMPLE_LOOP, BASSFlag.BASS_SAMPLE_LOOP);
                }

                // Start playback
                m_isPlaying = true;
                m_isPaused = false;

                // Only the DirectSound mode needs to start the main channel since it's not in decode mode.
                if (m_device.DriverType == DriverType.DirectSound)
                {
                    // Start playback
                    m_mainChannel.Play(false);
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("Error in PlayerV4.PlayFiles: " + ex.Message + "\n" + ex.StackTrace);                
                throw ex;
            }
        }

        /// <summary>
        /// Creates a song to be used in the player. This loads the audio file properties and
        /// metadata and creates a stream.
        /// </summary>
        /// <param name="previousSong">Pointer to the previous song</param>
        /// <param name="filePath">File path to the audio file</param>
        /// <returns>Instance of the channel</returns>
        private Song CreateSong(ref Song previousSong, string filePath)
        {
            // Create channel file properties and BASS channel
            Song channel = new Song();
            channel.FileProperties = new AudioFile(filePath);
            channel.Channel = MPfm.Sound.BassNetWrapper.Channel.CreateFileStreamForDecoding(filePath);
            //channel.ChannelDecode = MPfm.Sound.BassNetWrapper.Channel.CreateFileStreamForDecoding(filePath);
            //channel.Channel = MPfm.Sound.BassNetWrapper.Channel.CreateStreamForTimeShifting(channel.ChannelDecode.Handle, true);

            // Set pointer to the previous channel
            channel.PreviousSong = previousSong;

            // Make sure there was a previous channel
            if (previousSong != null)
            {
                // Set next channel for the previous channel
                previousSong.NextSong = channel;
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
            // Check if the main channel exists, and make sure the player is playing
            if (m_mainChannel == null)// || !m_isPlaying)
            {
                return;
            }

            // Check driver type
            if (m_device.DriverType == DriverType.DirectSound)
            {
                // Stop main channel
                m_mainChannel.Stop();
            }
            else if (m_device.DriverType == DriverType.ASIO)
            {
                // Stop playback
                BassAsio.BASS_ASIO_Stop();
            }
            else if (m_device.DriverType == DriverType.WASAPI)
            {
                // Stop playback
                BassWasapi.BASS_WASAPI_Stop(false);
            }            

            // Check if the current song exists
            if (m_currentSong != null)
            {
                if (m_currentSong.Channel != null)
                {
                    //if (m_currentSubChannel.Channel.IsActive() == BASSActive.BASS_ACTIVE_PLAYING)
                    //{
                    m_currentSong.Channel.Stop();
                    m_currentSong.Channel.Free();
                    //}
                }

                if (m_currentSong.NextSong != null &&
                    m_currentSong.NextSong.Channel != null)
                {
                    //if (m_currentSubChannel.NextChannel.Channel.IsActive() == BASSActive.BASS_ACTIVE_PLAYING)
                   // {
                    m_currentSong.NextSong.Channel.Stop();
                    m_currentSong.NextSong.Channel.Free();
                    //}
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
                m_currentSongIndex = index;

                try
                {
                    // TODO: MERGE THIS METHOD WITH PLAY.

                    // Create the first channel
                    Song channelNull = null;
                    Song channelOne = CreateSong(ref channelNull, m_filePaths[m_currentSongIndex]);

                    // Check if there are other files in the playlist
                    if (m_currentSongIndex + 1 < m_filePaths.Count)
                    {
                        // Create the second channel
                        Song channelTwo = CreateSong(ref channelOne, m_filePaths[m_currentSongIndex + 1]);
                    }

                    // Set the current channel as the first one
                    m_currentSong = channelOne;

                    // Check if the repeat type is Song
                    if (m_repeatType == MPfm.Library.RepeatType.Song)
                    {
                        // Force looping
                        m_currentSong.Channel.SetFlags(BASSFlag.BASS_SAMPLE_LOOP, BASSFlag.BASS_SAMPLE_LOOP);
                    }

                    if (m_device.DriverType == DriverType.ASIO)
                    {
                        // Start playback
                        if (!BassAsio.BASS_ASIO_Start(0))
                        {
                            // Get error
                            BASSError error = Bass.BASS_ErrorGetCode();
                            throw new Exception("[PlayerV4.GoTo] Error playing files in ASIO: " + error.ToString());
                        }
                    }
                    else if (m_device.DriverType == DriverType.WASAPI)
                    {
                        // Start playback
                        if (!BassWasapi.BASS_WASAPI_Start())
                        {
                            // Get error
                            BASSError error = Bass.BASS_ErrorGetCode();
                            throw new Exception("[PlayerV4.GoTo] Error playing files in WASAPI: " + error.ToString());
                        }
                    }

                    // Start playback
                    m_isPlaying = true;
                    m_isPaused = false;

                    if (m_device.DriverType == DriverType.DirectSound)
                    {
                        m_mainChannel.Play(false);
                    }

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
            if (m_currentSongIndex > 0)
            {
                // Go to previous audio file
                GoTo(m_currentSongIndex - 1);
            }
        }

        /// <summary>
        /// Skips to the next channel in the list.
        /// </summary>
        public void Next()
        {
            // Check if there is a next song
            if (m_currentSongIndex < m_filePaths.Count - 1)
            {
                // Go to next audio file
                GoTo(m_currentSongIndex + 1);
            }            
        }

        /// <summary>
        /// Go to a marker position.
        /// </summary>
        /// <param name="marker">Marker position</param>
        public void GoToMarker(Marker marker)
        {
            // Set current song position
            m_currentSong.Channel.SetPosition(marker.Position);
        }

        /// <summary>
        /// Starts a loop. The playback must be activated.
        /// </summary>
        /// <param name="loop">Loop to apply</param>
        public void StartLoop(Loop loop)
        {
            // Set loop sync proc            
            m_currentSong.SyncProc = new SYNCPROC(LoopSyncProc);
            m_currentSong.SyncProcHandle = m_currentSong.Channel.SetSync(BASSSync.BASS_SYNC_POS | BASSSync.BASS_SYNC_MIXTIME, loop.MarkerB.Position, m_currentSong.SyncProc);

            // Set current song position to marker A
            m_currentSong.Channel.SetPosition(loop.MarkerA.Position);

            // Set current loop
            m_currentLoop = loop;

        }

        /// <summary>
        /// Stops any loop currently playing.
        /// </summary>
        public void StopLoop()
        {
            // Make sure there is a loop to stop
            if (m_currentLoop == null)
            {
                return;
            }
            
            // Remove sync proc
            m_currentSong.Channel.RemoveSync(m_currentSong.SyncProcHandle);

            // Stop loop and release
            m_currentLoop = null;            
        }

        #endregion

        #region Other Methods (EQ)

        /// <summary>
        /// Gets the parameters of an EQ band.
        /// </summary>
        /// <param name="band">Band index</param>
        /// <returns>EQ parameters</returns>
        public BASS_BFX_PEAKEQ GetEQParams(int band)
        {
            BASS_BFX_PEAKEQ eq = new BASS_BFX_PEAKEQ();
            eq.lBand = band;
            Bass.BASS_FXGetParameters(m_fxEQHandle, eq);
            
            return eq;
        }

        /// <summary>
        /// Updates the gain of an EQ band.
        /// </summary>
        /// <param name="band">Band index</param>
        /// <param name="gain">Gain (in dB)</param>
        public void UpdateEQ(int band, float gain)
        {
            BASS_BFX_PEAKEQ eq = GetEQParams(band);
            eq.fGain = gain;
            Bass.BASS_FXSetParameters(m_fxEQHandle, eq);

            // Set EQ preset too
            m_currentEQPreset.Bands[band].Gain = gain;
        }

        /// <summary>
        /// Resets the gain of every EQ band.
        /// </summary>
        public void ResetEQ()
        {
            // Loop through bands
            for (int a = 0; a < m_currentEQPreset.Bands.Count; a++)
            {
                // Reset gain
                UpdateEQ(a, 0.0f);
                m_currentEQPreset.Bands[a].Gain = 0.0f;
            }
        }

        #endregion

        #region Callback Events

        /// <summary>
        /// Callback used for DirectSound devices.
        /// </summary>
        /// <param name="handle">Channel handle</param>
        /// <param name="buffer">Buffer data</param>
        /// <param name="length">Buffer length</param>
        /// <param name="user">User data</param>
        /// <returns>Audio data</returns>
        private int StreamCallback(int handle, IntPtr buffer, int length, IntPtr user)
        {
            // If the current sub channel is null, end the stream
            if (m_currentSong == null)
            {
                // Return end-of-channel
                return (int)BASSStreamProc.BASS_STREAMPROC_END;
            }

            // Get active status
            BASSActive status = m_currentSong.Channel.IsActive();

            // Check the current channel status
            if (status == BASSActive.BASS_ACTIVE_PLAYING)
            {
                // Check if the next channel needs to be loaded
                if (m_currentSongIndex < m_filePaths.Count - 1 &&
                    m_currentSong.NextSong == null)
                {
                    // Create the next channel using a timer
                    m_timerPlayer.Start();
                }

                // Get data from the current channel since it is running
                return m_currentSong.Channel.GetData(buffer, length);
            }
            else if (status == BASSActive.BASS_ACTIVE_STOPPED)
            {
                // Check if there is another channel to load
                if (m_currentSong.NextSong == null)
                {
                    // This is the end of the playlist. Check the repeat type if the playlist needs to be repeated
                    if (RepeatType == MPfm.Library.RepeatType.Playlist)
                    {
                        // Restart playback from the first channel                        
                        m_currentSongIndex = 0;
                        Song channelNull = null;
                        Song channelOne = CreateSong(ref channelNull, m_filePaths[m_currentSongIndex]);

                        // Check if there are other files in the playlist
                        if (m_currentSongIndex + 1 < m_filePaths.Count)
                        {
                            // Create the second channel
                            Song channelTwo = CreateSong(ref channelOne, m_filePaths[m_currentSongIndex + 1]);
                        }

                        // Set the current channel as the first one
                        m_currentSong = channelOne;

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
                        return m_currentSong.Channel.GetData(buffer, length);
                    }
                    else
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
                    }

                    // This is the end of the playlist                    
                    return (int)BASSStreamProc.BASS_STREAMPROC_END;
                }

                // Load next channel
                m_currentSong = m_currentSong.NextSong;

                // Increment index
                m_currentSongIndex++;

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
                return m_currentSong.Channel.GetData(buffer, length);
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

        /// <summary>
        /// Callback used for ASIO devices.
        /// </summary>
        /// <param name="input">Is an input channel</param>
        /// <param name="channel">Channel handle</param>
        /// <param name="buffer">Buffer data</param>
        /// <param name="length">Buffer length</param>
        /// <param name="user">User data</param>
        /// <returns>Audio data</returns>
        private int AsioCallback(bool input, int channel, IntPtr buffer, int length, IntPtr user)
        {
            return GetData(buffer, length, user);
        }

        /// <summary>
        /// Callback used for WASAPI devices.
        /// </summary>
        /// <param name="buffer">Buffer data</param>
        /// <param name="length">Buffer length</param>
        /// <param name="user">User data</param>
        /// <returns>Audio data</returns>
        private int WASAPICallback(IntPtr buffer, int length, IntPtr user)
        {
            return GetData(buffer, length, user);
        }

        /// <summary>
        /// Returns the audio data for the ASIO and WASAPI callbacks.
        /// </summary>
        /// <param name="buffer">Buffer data</param>
        /// <param name="length">Buffer length</param>
        /// <param name="user">User data</param>
        /// <returns>Audio data</returns>
        private int GetData(IntPtr buffer, int length, IntPtr user)
        {
            // Check if the channel is still playing
            if (Bass.BASS_ChannelIsActive(m_mainChannel.Handle) == BASSActive.BASS_ACTIVE_PLAYING)
            {
                // Return data
                return Bass.BASS_ChannelGetData(m_mainChannel.Handle, buffer, length);
            }
            else
            {
                // Return end of file
                return (int)BASSStreamProc.BASS_STREAMPROC_END;
            }
        }

        /// <summary>
        /// Sync callback routine used for looping the current channel.
        /// </summary>
        /// <param name="handle">Handle to the sync</param>
        /// <param name="channel">Channel handle</param>
        /// <param name="data">Data</param>
        /// <param name="user">User data</param>
        private void LoopSyncProc(int handle, int channel, int data, IntPtr user)
        {
            // Set loop position
            Bass.BASS_ChannelSetPosition(channel, CurrentLoop.MarkerA.Position);
        }

        #endregion
    }
}