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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using MPfm.Core;
using MPfm.Library;
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;
using Un4seen.Bass;
using Un4seen.BassAsio;
using Un4seen.BassWasapi;
using Un4seen.Bass.AddOn.Flac;
using Un4seen.Bass.AddOn.Fx;

namespace MPfm.Player.PlayerV4
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
        private Dictionary<int, string> m_plugins = null;

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
        public delegate void SongFinished(SongFinishedData data);
        /// <summary>
        /// The OnSongFinished event is triggered when a song has finished playing.
        /// </summary>
        public event SongFinished OnSongFinished;

        /// <summary>
        /// Delegate method for the OnStreamCallbackCalled event.
        /// </summary>
        /// <param name="data">OnStreamCallbackCalled data</param>
        public delegate void StreamCallbackCalled(StreamCallbackData data);
        /// <summary>
        /// The OnStreamCallbackCalled event is triggered when the stream callback has been called and
        /// must return data.
        /// </summary>
        public event StreamCallbackCalled OnStreamCallbackCalled;

        #endregion

        #region Properties
        
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
                if (m_playlist != null && m_playlist.CurrentItem != null)
                {
                    // Check if the repeat type is Song
                    if (m_repeatType == RepeatType.Song)
                    {
                        // Force looping
                        m_playlist.CurrentItem.Channel.SetFlags(BASSFlag.BASS_SAMPLE_LOOP, BASSFlag.BASS_SAMPLE_LOOP);

                        //m_syncProc = new SYNCPROC(EndSync);
                        //m_syncProcHandle = m_currentSubChannel.Channel.SetSync(BASSSync.BASS_SYNC_END | BASSSync.BASS_SYNC_MIXTIME, 0, m_syncProc);                    
                    }
                    else
                    {
                        // Remove looping
                        m_playlist.CurrentItem.Channel.SetFlags(BASSFlag.BASS_DEFAULT, BASSFlag.BASS_SAMPLE_LOOP);
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

                // Set configuration
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
        /// Private value for the Playlist property.
        /// </summary>
        private Playlist m_playlist = null;
        /// <summary>
        /// Playlist used for playback. Contains the audio file metadata and decode channels for
        /// playback.
        /// </summary>
        public Playlist Playlist
        {
            get
            {
                return m_playlist;
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

        /// <summary>
        /// Private value for the IsEQEnabled property.
        /// </summary>
        private bool m_isEQEnabled = false;
        /// <summary>
        /// Indicates if the EQ is enabled.
        /// </summary>
        public bool IsEQEnabled
        {
            get
            {
                return m_isEQEnabled;
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
            m_playlist = new PlayerV4.Playlist();
            m_markers = new List<Marker>();
            m_loops = new List<Loop>();            

            // Create timer
            Tracing.Log("Player.Initialize || Creating timer...");
            m_timerPlayer = new System.Timers.Timer();
            m_timerPlayer.Elapsed += new System.Timers.ElapsedEventHandler(m_timerPlayer_Elapsed);
            m_timerPlayer.Interval = 1000;
            m_timerPlayer.Enabled = false;

            // Load plugins
            //m_plugins = Base.LoadPluginDirectory(Path.GetDirectoryName((new Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath));
            Tracing.Log("Player.Initialize || Loading plugins...");
            m_flacPluginHandle = Base.LoadPlugin("bassflac.dll");            
            Base.LoadFxPlugin();

            // Create default EQ
            Tracing.Log("Player.Initialize || Creating default EQ preset...");
            m_currentEQPreset = new EQPreset();

            // Initialize sound system
            Tracing.Log("Player.Initialize || Device: " + m_device.DriverType.ToString() + " // " + m_device.Name + " (id: " + m_device.Id.ToString() + ")");
            Tracing.Log("Player.Initialize || Initializing device...");
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
            Base.FreePlugin(m_flacPluginHandle);
            //Base.FreePluginDirectory(m_plugins);            
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

            // Check if the next channel needs to be loaded
            if (m_playlist.CurrentItemIndex < m_playlist.Items.Count - 1)
            {
                // Check if the channel has already been loaded
                if (!m_playlist.Items[m_playlist.CurrentItemIndex + 1].IsLoaded)
                {
                    // Create the next channel
                    m_playlist.Items[m_playlist.CurrentItemIndex + 1].Load();
                }
            }

            // Set time shifting value
            //m_currentSubChannel.Channel.SetAttribute(BASSAttribute.BASS_ATTRIB_TEMPO, TimeShifting);
        }

        #endregion

        #region Playback Methods

        /// <summary>
        /// Plays the playlist from the current item index.
        /// </summary>
        public void Play()
        {
            try
            {
                // Check if the player is currently playing
                if (m_isPlaying)
                {
                    // Stop playback
                    Stop();
                }

                // Load the first two channels
                for (int a = Playlist.CurrentItemIndex; a < Playlist.CurrentItemIndex + 2; a++)
                {
                    // Load channel and audio file metadata
                    m_playlist.Items[a].Load();
                }

                // Create the streaming channel
                m_streamProc = new STREAMPROC(StreamCallback);
                m_streamChannel = MPfm.Sound.BassNetWrapper.Channel.CreateStream(m_mixerSampleRate, 2, true, m_streamProc);

                // Check driver type
                if (m_device.DriverType == DriverType.DirectSound)
                {
                    // Create main channel
                    m_mainChannel = MPfm.Sound.BassNetWrapper.Channel.CreateStreamForTimeShifting(m_streamChannel.Handle, false, false);
                }
                else if (m_device.DriverType == DriverType.ASIO)
                {
                    // Create main channel
                    m_mainChannel = MPfm.Sound.BassNetWrapper.Channel.CreateStreamForTimeShifting(m_streamChannel.Handle, true, true);

                    // Create callback
                    m_asioProc = new ASIOPROC(AsioCallback);

                    // Create channel
                    BassAsio.BASS_ASIO_ChannelEnable(false, 0, m_asioProc, new IntPtr(m_mainChannel.Handle));
                    BassAsio.BASS_ASIO_ChannelJoin(false, 1, 0);

                    // Start playback
                    //m_mainChannel.Play(false);
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
                    m_mainChannel = MPfm.Sound.BassNetWrapper.Channel.CreateStreamForTimeShifting(m_streamChannel.Handle, true, true);

                    // Start playback
                    if (!BassWasapi.BASS_WASAPI_Start())
                    {
                        // Get error
                        BASSError error = Bass.BASS_ErrorGetCode();
                        throw new Exception("[PlayerV4.PlayFiles] Error playing files in WASAPI: " + error.ToString());
                    }
                }

                // Load 18-band equalizer
                //AddEQ();

                // Check if the repeat type is Song
                if (m_repeatType == RepeatType.Song)
                {
                    // Force looping                    
                    m_playlist.CurrentItem.Channel.SetFlags(BASSFlag.BASS_SAMPLE_LOOP, BASSFlag.BASS_SAMPLE_LOOP);
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

            // Reset flags                
            m_playlist.Clear();            

            // Create playlist items
            foreach (string filePath in filePaths)
            {
                // Add playlist item
                m_playlist.AddItem(filePath);
            }

            // Set playlist to first item
            m_playlist.First();
          
            // Start playback
            Play();
        }

        /// <summary>
        /// Plays the list of songs specified in the songs parameter.
        /// </summary>
        /// <param name="songs">List of songs</param>
        public void PlaySongs(List<SongDTO> songs)
        {
            // Check for null or empty list of file paths
            if (songs == null || songs.Count == 0)
            {
                throw new Exception("There must be at least one song the songs parameter.");
            }

            // Check if all file paths exist
            Tracing.Log("[PlayerV4.PlaySongs] Playing a list of " + songs.Count.ToString() + " files.");
            foreach (SongDTO song in songs)
            {
                // Check if the file exists                
                if (!File.Exists(song.FilePath))
                {
                    // Throw exception
                    throw new Exception("The file at " + song.FilePath + " doesn't exist!");
                }
            }

            // Check if the player is currently playing
            if (m_isPlaying)
            {
                // Stop playback
                Stop();
            }

            // Reset flags                
            m_playlist.Clear();

            // Create playlist items
            foreach (SongDTO song in songs)
            {
                // Add playlist item
                m_playlist.AddItem(song);
            }

            // Set playlist to first item
            m_playlist.First();

            // Start playback
            Play();
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
            if (m_playlist != null && m_playlist.CurrentItem != null)
            {
                // Dispose channels
                m_playlist.DisposeChannels();
            }

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
            if (index <= Playlist.Items.Count - 1)
            {
                // Stop playback
                Stop();

                // Set index
                Playlist.GoTo(index);

                try
                {
                    // Load current item
                    Playlist.Items[index].Load();

                    // Load the next item, if it exists
                    if (Playlist.CurrentItemIndex + 1 < Playlist.Items.Count)
                    {
                        // Load next item
                        Playlist.Items[index+1].Load();
                    }

                    // Check if the repeat type is Song
                    if (m_repeatType == RepeatType.Song)
                    {
                        // Force looping
                        m_playlist.CurrentItem.Channel.SetFlags(BASSFlag.BASS_SAMPLE_LOOP, BASSFlag.BASS_SAMPLE_LOOP);
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
                        SongFinishedData data = new SongFinishedData();
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
            if (Playlist.CurrentItemIndex > 0)
            {
                // Go to previous audio file
                GoTo(Playlist.CurrentItemIndex - 1);
            }
        }

        /// <summary>
        /// Skips to the next channel in the list.
        /// </summary>
        public void Next()
        {
            // Check if there is a next song
            if (Playlist.CurrentItemIndex < Playlist.Items.Count - 1)
            {
                // Go to next audio file
                GoTo(Playlist.CurrentItemIndex + 1);
            }            
        }

        /// <summary>
        /// Sets the position of the currently playing channel.
        /// </summary>
        /// <param name="bytes">Position (in bytes)</param>
        public void SetPosition(long bytes)
        {
            // Validate player
            if (Playlist == null || Playlist.CurrentItem == null || Playlist.CurrentItem.Channel == null)
            {
                return;
            }

            // Set position
            Playlist.CurrentItem.Channel.SetPosition(bytes);
        }

        /// <summary>
        /// Sets the position of the currently playing channel.
        /// </summary>
        /// <param name="percentage">Position (in percentage)</param>
        public void SetPosition(double percentage)
        {
            // Validate player
            if (Playlist == null || Playlist.CurrentItem == null || Playlist.CurrentItem.Channel == null)
            {
                return;
            }
            
            // Calculate new position
            long newPosition = (long)Math.Ceiling((double)Playlist.CurrentItem.LengthBytes * (percentage / 100));

            // Set position
            Playlist.CurrentItem.Channel.SetPosition(newPosition);
        }

        /// <summary>
        /// Go to a marker position.
        /// </summary>
        /// <param name="marker">Marker position</param>
        public void GoToMarker(Marker marker)
        {
            // Set current song position
            Playlist.CurrentItem.Channel.SetPosition(marker.Position);
        }

        /// <summary>
        /// Starts a loop. The playback must be activated.
        /// </summary>
        /// <param name="loop">Loop to apply</param>
        public void StartLoop(Loop loop)
        {
            // Set loop sync proc            
            Playlist.CurrentItem.SyncProc = new SYNCPROC(LoopSyncProc);
            Playlist.CurrentItem.SyncProcHandle = Playlist.CurrentItem.Channel.SetSync(BASSSync.BASS_SYNC_POS | BASSSync.BASS_SYNC_MIXTIME, loop.MarkerB.Position, Playlist.CurrentItem.SyncProc);

            // Set current song position to marker A
            Playlist.CurrentItem.Channel.SetPosition(loop.MarkerA.Position);

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
            Playlist.CurrentItem.Channel.RemoveSync(Playlist.CurrentItem.SyncProcHandle);

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

        public void AddEQ(EQPreset preset)
        {
            // Validate stuff
            if (m_mainChannel == null)
            {
                return;
            }

            // Check if an handle already exists
            if (m_fxEQHandle != 0)
            {
                throw new Exception("The equalizer already exists!");
            }

            // Load 18-band equalizer
            m_fxEQHandle = m_mainChannel.SetFX(BASSFXType.BASS_FX_BFX_PEAKEQ, 0);
            BASS_BFX_PEAKEQ eq = new BASS_BFX_PEAKEQ();
            m_currentEQPreset = preset;
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

            // Set flags
            m_isEQEnabled = true;
        }

        public void RemoveEQ()
        {
            // Validate stuff
            if (m_mainChannel == null)
            {
                return;
            }

            // Remove EQ
            m_mainChannel.RemoveFX(m_fxEQHandle);
            m_fxEQHandle = 0;
            m_isEQEnabled = false;
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
            if(m_playlist == null || m_playlist.CurrentItem == null)
            {
                // Return end-of-channel
                return (int)BASSStreamProc.BASS_STREAMPROC_END;
            }

            // Get active status            
            BASSActive status = m_playlist.CurrentItem.Channel.IsActive();

            // Check the current channel status
            if (status == BASSActive.BASS_ACTIVE_PLAYING)
            {
                // Check if the next channel needs to be loaded
                if (m_playlist.CurrentItemIndex < m_playlist.Items.Count - 1)                    
                {
                    // Create the next channel using a timer
                    m_timerPlayer.Start();
                }

                // Get data from the current channel since it is running                
                int returns = m_playlist.CurrentItem.Channel.GetData(buffer, length);  
              
                //float[] stuff = new float[length];
                //int returns2 = m_playlist.CurrentItem.Channel.GetData(stuff, length);

                //// Raise song end event (if an event is subscribed)
                //if (OnStreamCallbackCalled != null)
                //{
                //    // Create data
                //    StreamCallbackData data = new StreamCallbackData();
                //    data.Length = length;
                //    data.Data = new byte[length];
                //    Marshal.Copy(buffer, data.Data, 0, length);

                //    float[] floats = new float[length / 4];
                //    for (int a = 0; a < length / 4; a++)
                //    {
                //        floats[a] = BitConverter.ToSingle(data.Data, a);
                //    }

                //    //UnionArray arry = new UnionArray() { Bytes = data.Data };
                //    //data.Data2 = stuff;

                //    // Raise event
                //    OnStreamCallbackCalled(data);
                //}

                return returns;
            }
            else if (status == BASSActive.BASS_ACTIVE_STOPPED)
            {
                // Check if this is the last item to play
                if (m_playlist.CurrentItemIndex == m_playlist.Items.Count - 1)
                {
                    // This is the end of the playlist. Check the repeat type if the playlist needs to be repeated
                    if (RepeatType == RepeatType.Playlist)
                    {                        
                        // Dispose channels
                        m_playlist.DisposeChannels();

                        // Restart playback from the first item
                        Playlist.First();
                        Playlist.Items[0].Load();
                        Playlist.Items[1].Load();

                        // Raise song end event (if an event is subscribed)
                        if (OnSongFinished != null)
                        {
                            // Create data
                            SongFinishedData data = new SongFinishedData();
                            data.IsPlaybackStopped = false;

                            // Raise event
                            OnSongFinished(data);
                        }

                        // Return data from the new channel                        
                        return Playlist.CurrentItem.Channel.GetData(buffer, length);
                    }
                    else
                    {
                        // Raise song end event (if an event is subscribed)
                        if (OnSongFinished != null)
                        {
                            // Create data
                            SongFinishedData data = new SongFinishedData();
                            data.IsPlaybackStopped = true;

                            // Raise event
                            OnSongFinished(data);
                        }
                    }

                    // This is the end of the playlist                    
                    return (int)BASSStreamProc.BASS_STREAMPROC_END;
                }

                // There is another item to play; go on.                
                Playlist.Next();

                // Raise song end event (if an event is subscribed)
                if (OnSongFinished != null)
                {
                    // Create data
                    SongFinishedData data = new SongFinishedData();
                    data.IsPlaybackStopped = false;

                    // Raise event
                    OnSongFinished(data);
                }

                // Return data from the new channel
                return Playlist.CurrentItem.Channel.GetData(buffer, length);
            }

            // Raise song end event (if an event is subscribed)
            if (OnSongFinished != null)
            {
                // Create data
                SongFinishedData data = new SongFinishedData();
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

    /// <summary>
    /// Defines the repeat types (off, playlist or song).
    /// </summary>
    public enum RepeatType
    {
        Off = 0, Playlist = 1, Song = 2
    }
}