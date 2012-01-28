﻿//
// Player.cs: The Player class manages audio playback.
//
// Copyright © 2011-2012 Yanick Castonguay
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
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;
using Un4seen.Bass;
using Un4seen.BassAsio;
using Un4seen.BassWasapi;
using Un4seen.Bass.AddOn.Ape;
using Un4seen.Bass.AddOn.Flac;
using Un4seen.Bass.AddOn.Fx;

namespace MPfm.Player
{
    /// <summary>
    /// The Player class manages audio playback through playlists and supports
    /// multiple driver types and devices.
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Timer for the player.
        /// </summary>
        private System.Timers.Timer m_timerPlayer = null;
        private Channel m_streamChannel = null;
        private int m_syncProcHandle;
        private Dictionary<int, string> m_plugins = null;

        // Plugin handles
        private int m_fxEQHandle;        
        private int m_apePluginHandle = 0;
        private int m_flacPluginHandle = 0;
        private int m_mpcPluginHandle = 0;
        private int m_ofrPluginHandle = 0;
        private int m_ttaPluginHandle = 0;
        private int m_wvPluginHandle = 0;

        /// <summary>
        /// Offset position (necessary to calculate the offset in the output stream position
        /// if the user has seeked the position in the decode stream). The output stream position
        /// is reset to 0 in these cases to clear the audio buffer.
        /// </summary>
        private long m_positionOffset = 0;

        #region Callbacks
        
        // Callbacks
        private STREAMPROC m_streamProc;
        private SYNCPROC m_syncProc;        
        private ASIOPROC m_asioProc;
        private WASAPIPROC m_wasapiProc;

        #endregion

        #region Events
        
        /// <summary>
        /// Delegate method for the OnPlaylistIndexChanged event.
        /// </summary>
        /// <param name="data">OnPlaylistIndexChanged data</param>
        public delegate void PlaylistIndexChanged(PlayerPlaylistIndexChangedData data);
        /// <summary>
        /// The OnPlaylistIndexChanged event is triggered when the playlist index changes (i.e. when an audio file
        /// starts to play).
        /// </summary>
        public event PlaylistIndexChanged OnPlaylistIndexChanged;

        /// <summary>
        /// Delegate method for the OnStreamCallbackCalled event.
        /// </summary>
        /// <param name="data">OnStreamCallbackCalled data</param>
        public delegate void StreamCallbackCalled(PlayerStreamCallbackData data);
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

        /// <summary>
        /// Private value for the IsEQBypassed property.
        /// </summary>
        private bool m_isEQBypassed = false;
        /// <summary>
        /// Indicates if the EQ is bypassed.
        /// </summary>
        public bool IsEQBypassed
        {
            get
            {
                return m_isEQBypassed;
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
        }

        #endregion

        #endregion

        #region Constructors, Initialization/Dispose

        /// <summary>
        /// Default constructor for the Player class. Initializes the player using the default
        /// values (see property comments).
        /// </summary>
        public Player()
        {
            // Initialize system with default values
            Initialize(new Device(), 44100, 100, 10, true);
        }

        /// <summary>
        /// Constructor for the Player class which requires the mixer sample rate value to be passed
        /// in parameter.
        /// </summary>
        /// <param name="device">Device output</param>
        /// <param name="mixerSampleRate">Mixer sample rate (default: 44100 Hz)</param>
        /// <param name="bufferSize">Buffer size (default: 500 ms)</param>
        /// <param name="updatePeriod">Update period (default: 10 ms)</param>
        /// <param name="initializeDevice">Indicates if the device should be initialized</param>
        public Player(Device device, int mixerSampleRate, int bufferSize, int updatePeriod, bool initializeDevice)
        {
            // Initialize system with specific values.
            Initialize(device, mixerSampleRate, bufferSize, updatePeriod, initializeDevice);
        }

        /// <summary>
        /// Initializes the player.
        /// </summary>        
        /// <param name="device">Device output</param>
        /// <param name="mixerSampleRate">Mixer sample rate (default: 44100 Hz)</param>
        /// <param name="bufferSize">Buffer size (default: 500 ms)</param>
        /// <param name="updatePeriod">Update period (default: 10 ms)</param>
        /// <param name="initializeDevice">Indicates if the device should be initialized</param>
        private void Initialize(Device device, int mixerSampleRate, int bufferSize, int updatePeriod, bool initializeDevice)
        {
            // Initialize system using specified values
            m_device = device;
            m_mixerSampleRate = mixerSampleRate;
            m_bufferSize = bufferSize;
            m_updatePeriod = updatePeriod;

            // Create lists            
            m_playlist = new Playlist();
            m_markers = new List<Marker>();
            m_loops = new List<Loop>();            

            // Create timer
            Tracing.Log("Player init -- Creating timer...");
            m_timerPlayer = new System.Timers.Timer();
            m_timerPlayer.Elapsed += new System.Timers.ElapsedEventHandler(m_timerPlayer_Elapsed);
            m_timerPlayer.Interval = 1000;
            m_timerPlayer.Enabled = false;

            // Load plugins
            //m_plugins = Base.LoadPluginDirectory(Path.GetDirectoryName((new Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath));
            Tracing.Log("Player init -- Loading plugins...");
            m_apePluginHandle = Base.LoadPlugin("bass_ape.dll");
            m_flacPluginHandle = Base.LoadPlugin("bassflac.dll");
            m_mpcPluginHandle = Base.LoadPlugin("bass_mpc.dll");
            //m_ofrPluginHandle = Base.LoadPlugin("bass_ofr.dll"); // Requires OptimFrog.DLL
            //m_ttaPluginHandle = Base.LoadPlugin("bass_tta.dll");
            m_wvPluginHandle = Base.LoadPlugin("basswv.dll");

            Tracing.Log("Player init -- Loading FX plugin...");
            Base.LoadFxPlugin();

            // Create default EQ
            Tracing.Log("Player init -- Creating default EQ preset...");
            m_currentEQPreset = new EQPreset();

            // Initialize device
            if (initializeDevice)
            {
                Tracing.Log("Player init -- Initializing device,,,");
                InitializeDevice(m_device, m_mixerSampleRate);
            }
        }

        /// <summary>
        /// Initializes the default audio device for playback.
        /// </summary>
        public void InitializeDevice()
        {
            // Initialize default device
            InitializeDevice(new Device(), m_mixerSampleRate);
        }

        /// <summary>
        /// Initializes a specific audio device for playback.
        /// </summary>
        /// <param name="device">Audio device</param>
        /// <param name="mixerSampleRate">Mixer sample rate (in Hz)</param>
        public void InitializeDevice(Device device, int mixerSampleRate)
        {
            // Set properties
            m_device = device;
            m_mixerSampleRate = mixerSampleRate;

            Tracing.Log("Player -- Initializing device (SampleRate: " + m_mixerSampleRate.ToString() + " Hz, DriverType: " + m_device.DriverType.ToString() + ", Id: " + m_device.Id.ToString() + ", Name: " + m_device.Name + ", BufferSize: " + m_bufferSize.ToString() + ", UpdatePeriod: " + m_updatePeriod.ToString() + ")");

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
            m_mainChannel = null;
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
            Base.FreePlugin(m_apePluginHandle);
            Base.FreePlugin(m_flacPluginHandle);
            Base.FreePlugin(m_mpcPluginHandle);
            //Base.FreePlugin(m_ofrPluginHandle);
            //Base.FreePlugin(m_ttaPluginHandle);
            Base.FreePlugin(m_wvPluginHandle);
            //Base.FreePluginDirectory(m_plugins);            
        }        

        /// <summary>
        /// Disposes the current device and the audio plugins.
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
            if (m_timerPlayer != null)
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
                    // Check if a loop is active
                    if (m_currentLoop != null)
                    {
                        // Stop loop
                        Tracing.Log("Player.Play -- Stopping current loop...");
                        StopLoop();
                    }

                    // Stop playback
                    Tracing.Log("Player.Play -- Stopping playback...");
                    Stop();
                }

                // Make sure there are no current loops                 
                m_currentLoop = null;

                // Set offset
                m_positionOffset = 0;

                // How many channels are left?                
                int channelsToLoad = Playlist.Items.Count - Playlist.CurrentItemIndex;                

                // If there are more than 2, just limit to 2 for now. The other channels are loaded dynamically.
                if (channelsToLoad > 2)
                {
                    channelsToLoad = 2;
                }

                // Check for channels to load
                if (channelsToLoad == 0)
                {
                    throw new Exception("Error in Player.Play: There aren't any channels to play!");
                }

                // Load the current channel and the next channel if it exists
                for (int a = Playlist.CurrentItemIndex; a < Playlist.CurrentItemIndex + channelsToLoad; a++)
                {
                    // Load channel and audio file metadata
                    m_playlist.Items[a].Load();
                }

                try
                {
                    // Create the streaming channel (set the frequency to the first file in the list)
                    Tracing.Log("Player.Play -- Creating streaming channel (SampleRate: " + m_playlist.CurrentItem.AudioFile.SampleRate + " Hz, FloatingPoint: true)...");
                    m_streamProc = new STREAMPROC(StreamCallback);                
                    m_streamChannel = MPfm.Sound.BassNetWrapper.Channel.CreateStream(m_playlist.CurrentItem.AudioFile.SampleRate, 2, true, m_streamProc);
                }
                catch(Exception ex)
                {
                    // Raise custom exception with information (so the client application can maybe deactivate floating point for example)
                    PlayerCreateStreamException newEx = new PlayerCreateStreamException("The player has failed to create the stream channel (" + ex.Message + ").", ex);
                    newEx.Decode = true;
                    newEx.UseFloatingPoint = true;
                    newEx.SampleRate = m_playlist.CurrentItem.AudioFile.SampleRate;
                    throw newEx;
                }

                // Check driver type
                if (m_device.DriverType == DriverType.DirectSound)
                {
                    try
                    {
                        // Create main channel
                        Tracing.Log("Player.Play -- Creating time shifting channel (DriverType: DirectSound, Decode: false, FloatingPoint: false)...");
                        m_mainChannel = MPfm.Sound.BassNetWrapper.Channel.CreateStreamForTimeShifting(m_streamChannel.Handle, false, false);
                    }
                    catch (Exception ex)
                    {   
                        // Raise custom exception with information (so the client application can maybe deactivate floating point for example)
                        PlayerCreateStreamException newEx = new PlayerCreateStreamException("The player has failed to create the time shifting channel.", ex);
                        newEx.UseFloatingPoint = true;
                        newEx.UseTimeShifting = true;
                        newEx.SampleRate = m_playlist.CurrentItem.AudioFile.SampleRate;
                        throw newEx;
                    }
                }
                else if (m_device.DriverType == DriverType.ASIO)
                {
                    try
                    {
                        // Create main channel
                        Tracing.Log("Player.Play -- Creating ASIO time shifting channel...");
                        m_mainChannel = MPfm.Sound.BassNetWrapper.Channel.CreateStreamForTimeShifting(m_streamChannel.Handle, true, true);
                    }
                    catch (Exception ex)
                    {
                        // Raise custom exception with information (so the client application can maybe deactivate floating point for example)
                        PlayerCreateStreamException newEx = new PlayerCreateStreamException("The player has failed to create the time shifting channel.", ex);
                        newEx.DriverType = DriverType.ASIO;
                        newEx.UseFloatingPoint = true;
                        newEx.UseTimeShifting = true;
                        newEx.Decode = true;
                        newEx.SampleRate = m_playlist.CurrentItem.AudioFile.SampleRate;
                        throw newEx;  
                    }

                    // Create callback
                    m_asioProc = new ASIOPROC(AsioCallback);

                    try
                    {
                        // Enable and join channels (for stereo output
                        Tracing.Log("Player.Play -- Enabling ASIO channels...");
                        BassAsio.BASS_ASIO_ChannelEnable(false, 0, m_asioProc, new IntPtr(m_mainChannel.Handle));
                        Tracing.Log("Player.Play -- Joining ASIO channels...");
                        BassAsio.BASS_ASIO_ChannelJoin(false, 1, 0);
                    }
                    catch (Exception ex)
                    {
                        // Raise custom exception with information (so the client application can maybe deactivate floating point for example)
                        PlayerCreateStreamException newEx = new PlayerCreateStreamException("The player has failed to enable or join ASIO channels.", ex);
                        newEx.DriverType = DriverType.ASIO;
                        newEx.SampleRate = m_playlist.CurrentItem.AudioFile.SampleRate;
                        throw newEx;
                    }

                    // Start playback
                    Tracing.Log("Player.Play -- Starting ASIO buffering...");
                    if (!BassAsio.BASS_ASIO_Start(0))
                    {
                        // Get BASS error
                        BASSError error = Bass.BASS_ErrorGetCode();

                        // Raise custom exception with information (so the client application can maybe deactivate floating point for example)
                        PlayerCreateStreamException newEx = new PlayerCreateStreamException("The player has failed to create the ASIO channel (" + error.ToString() + ").", null);
                        newEx.DriverType = DriverType.ASIO;
                        newEx.UseFloatingPoint = true;
                        newEx.UseTimeShifting = true;
                        newEx.Decode = true;
                        newEx.SampleRate = m_playlist.CurrentItem.AudioFile.SampleRate;
                        throw newEx;                        
                    }
                }
                else if (m_device.DriverType == DriverType.WASAPI)
                {
                    // Create main channel
                    Tracing.Log("Player.Play -- Creating WASAPI time shifting channel...");
                    m_mainChannel = MPfm.Sound.BassNetWrapper.Channel.CreateStreamForTimeShifting(m_streamChannel.Handle, true, true);

                    // Start playback
                    if (!BassWasapi.BASS_WASAPI_Start())
                    {
                        // Get error
                        BASSError error = Bass.BASS_ErrorGetCode();
                        throw new Exception("Player.Play error: Error playing files in WASAPI: " + error.ToString());
                    }
                }

                // Set initial volume
                m_mainChannel.Volume = Volume;

                // Load 18-band equalizer
                Tracing.Log("Player.Play -- Creating equalizer (Preset: " + m_currentEQPreset + ")...");
                AddEQ(m_currentEQPreset);

                // Check if EQ is bypassed
                if (m_isEQBypassed)
                {
                    // Reset EQ
                    Tracing.Log("Player.Play -- Equalizer is bypassed; resetting EQ...");
                    ResetEQ();
                }

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
                    Tracing.Log("Player.Play -- Starting DirectSound playback...");
                    m_mainChannel.Play(false);
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("Player.Play error: " + ex.Message + "\n" + ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// Plays the list of audio files specified in the filePaths parameter.
        /// </summary>
        /// <param name="filePaths">List of audio file paths</param>
        public void PlayFiles(List<string> filePaths)
        {
            // Create instances of the AudioFile class, but do not read metadata right now
            List<AudioFile> audioFiles = new List<AudioFile>();
            foreach (string filePath in filePaths)
            {
                // Create instance and add to list
                AudioFile audioFile = new AudioFile(filePath, Guid.NewGuid(), false);
                audioFiles.Add(audioFile);
            }

            // Call method
            PlayFiles(audioFiles);
        }

        /// <summary>
        /// Plays the list of audio files specified in the audioFiles parameter.
        /// The AudioFile instances can come from MPfm.Library (reading from database) or by
        /// creating instances of the AudioFile class manually, which will read metadata from
        /// audio files to fill the properties.
        /// </summary>
        /// <param name="audioFiles">List of audio files</param>
        public void PlayFiles(List<AudioFile> audioFiles)
        {
            // Check for null or empty list of file paths
            if (audioFiles == null || audioFiles.Count == 0)
            {
                throw new Exception("There must be at least one file in the audioFiles parameter.");
            }

            // Check if all file paths exist
            Tracing.Log("Player.PlayFiles -- Playing a list of " + audioFiles.Count.ToString() + " files.");
            foreach (AudioFile audioFile in audioFiles)
            {
                // Check if the file exists                
                if (!File.Exists(audioFile.FilePath))
                {
                    // Throw exception
                    throw new Exception("The file at " + audioFile.FilePath + " doesn't exist!");
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
            foreach (AudioFile audioFile in audioFiles)
            {
                // Add playlist item
                m_playlist.AddItem(audioFile);
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
                throw new Exception("Player.Stop error: The main channel is null!");
            }

            // Check if EQ is enabled
            if (m_isEQEnabled)
            {
                // Remove EQ
                Tracing.Log("Player.Stop -- Removing equalizer...");
                RemoveEQ();
            }

            // Check driver type
            if (m_device.DriverType == DriverType.DirectSound)
            {
                // Stop main channel
                Tracing.Log("Player.Stop -- Stopping DirectSound channel...");                
                m_mainChannel.Stop();
            }
            else if (m_device.DriverType == DriverType.ASIO)
            {
                // Stop playback
                Tracing.Log("Player.Stop -- Stopping ASIO playback...");
                BassAsio.BASS_ASIO_Stop();
            }
            else if (m_device.DriverType == DriverType.WASAPI)
            {
                // Stop playback
                Tracing.Log("Player.Stop -- Stopping WASAPI playback...");
                BassWasapi.BASS_WASAPI_Stop(false);
            }

            // Check if the current song exists
            if (m_playlist != null && m_playlist.CurrentItem != null)
            {
                // Dispose channels
                Tracing.Log("Player.Stop -- Disposing channels...");
                m_playlist.DisposeChannels();
            }

            // Set flags
            m_currentLoop = null;
            m_isPlaying = false;
        }

        /// <summary>
        /// Stops the playback and starts playback at a specific playlist song.
        /// </summary>
        /// <param name="index">Song index</param>
        public void GoTo(int index)
        {
            // Check if the player is currently playing                
            if (m_isPlaying)
            {
                // Check if a loop is active
                if (m_currentLoop != null)
                {
                    // Stop loop
                    Tracing.Log("Player.GoTo -- Stopping current loop...");
                    StopLoop();
                }

                // Stop playback
                Tracing.Log("Player.GoTo -- Stopping playback...");
                Stop();
            }

            // Clear loop
            m_currentLoop = null;

            // Set offset
            m_positionOffset = 0;

            // Make sure index is in the list
            if (index <= Playlist.Items.Count - 1)
            {
                //// Set position to 0
                //// http://www.un4seen.com/forum/?topic=12508.0;hl=clear+stream
                //SetPosition((long)0);

                // Set main channel position to 0 (clear buffer)
                m_mainChannel.SetPosition(0);

                // Set index
                Tracing.Log("Player.GoTo -- Setting playlist index to " + index.ToString() + "...");
                Playlist.GoTo(index);

                try
                {
                    // Load current item
                    Tracing.Log("Player.GoTo -- Loading item index " + index.ToString() + "...");
                    Playlist.Items[index].Load();

                    // Get audio file
                    AudioFile audioFileStarted = Playlist.Items[index].AudioFile;

                    // Load the next item, if it exists
                    if (Playlist.CurrentItemIndex + 1 < Playlist.Items.Count)
                    {
                        // Load next item
                        Tracing.Log("Player.GoTo -- Loading item index " + (index+1).ToString() + "...");
                        Playlist.Items[index+1].Load();
                    }

                    // Check if the repeat type is Song
                    if (m_repeatType == RepeatType.Song)
                    {
                        // Force looping
                        Tracing.Log("Player.GoTo -- Set BASS_SAMPLE_LOOP...");
                        m_playlist.CurrentItem.Channel.SetFlags(BASSFlag.BASS_SAMPLE_LOOP, BASSFlag.BASS_SAMPLE_LOOP);
                    }

                    // Compare sample rate on stream channel
                    if (m_mainChannel.SampleRate != Playlist.CurrentItem.AudioFile.SampleRate)
                    {
                        // Set new sample rate
                        m_mainChannel.SetSampleRate(Playlist.CurrentItem.AudioFile.SampleRate);
                    }

                    try
                    {
                        // Start playback depending on driver type
                        if (m_device.DriverType == DriverType.DirectSound)
                        {
                            // Start playback
                            Tracing.Log("Player.GoTo -- Starting DirectSound playback...");
                            m_mainChannel.Play(false);
                        }
                        else if (m_device.DriverType == DriverType.ASIO)
                        {
                            // Start playback
                            Tracing.Log("Player.GoTo -- Starting ASIO playback...");
                            if (!BassAsio.BASS_ASIO_Start(0))
                            {
                                // Get error
                                BASSError error = Bass.BASS_ErrorGetCode();
                                throw new Exception("Error playing files in ASIO: " + error.ToString());
                            }
                        }
                        else if (m_device.DriverType == DriverType.WASAPI)
                        {
                            // Start playback
                            Tracing.Log("Player.GoTo -- Starting WASAPI playback...");
                            if (!BassWasapi.BASS_WASAPI_Start())
                            {
                                // Get error
                                BASSError error = Bass.BASS_ErrorGetCode();
                                throw new Exception("Error playing files in WASAPI: " + error.ToString());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Raise custom exception with information (so the client application can maybe deactivate floating point for example)
                        PlayerCreateStreamException newEx = new PlayerCreateStreamException("The player has failed to start the playback (" + ex.Message.ToString() + ").", ex);
                        newEx.DriverType = m_device.DriverType;                        
                        throw newEx;   
                    }

                    // Set flags
                    m_isPlaying = true;
                    m_isPaused = false;

                    // Raise audio file finished event (if an event is subscribed)
                    if (OnPlaylistIndexChanged != null)
                    {
                        // Create data
                        PlayerPlaylistIndexChangedData data = new PlayerPlaylistIndexChangedData();
                        data.IsPlaybackStopped = false;
                        data.AudioFileStarted = audioFileStarted;

                        // Raise event
                        OnPlaylistIndexChanged(data);
                    }
                }
                catch (Exception ex)
                {
                    Tracing.Log("Player.GoTo error: " + ex.Message + "\n" + ex.StackTrace);
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
        /// Gets the position of the currently playing channel, in bytes.
        /// </summary>
        /// <returns>Position (in bytes)</returns>
        public long GetPosition()
        {
            // Validate player
            if (Playlist == null || Playlist.CurrentItem == null || Playlist.CurrentItem.Channel == null)
            {
                return 0;
            }
            
            // Get main channel position
            long outputPosition = m_mainChannel.GetPosition();

            // Divide by 2 (floating point)
            outputPosition /= 2;

            // Check if this is a FLAC file over 44100Hz
            if (Playlist.CurrentItem.AudioFile.FileType == AudioFileFormat.FLAC && Playlist.CurrentItem.AudioFile.SampleRate > 44100)
            {
                // Multiply by 1.5 (I don't really know why, but this works for 48000Hz and 96000Hz. Maybe a bug in BASS with FLAC files?)
                outputPosition = (long)((float)outputPosition * 1.5f);
            }

            // Add the offset position
            outputPosition += m_positionOffset;

            return outputPosition;
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

            // Set offset position (for calulating current position)
            m_positionOffset = bytes;

            // Set main channel position to 0 (clear buffer)
            m_mainChannel.SetPosition(0);

            // Check if this is a FLAC file over 44100Hz
            if (Playlist.CurrentItem.AudioFile.FileType == AudioFileFormat.FLAC && Playlist.CurrentItem.AudioFile.SampleRate > 44100)
            {
                // Divide by 1.5 (I don't really know why, but this works for 48000Hz and 96000Hz. Maybe a bug in BASS with FLAC files?)
                bytes = (long)((float)bytes / 1.5f);
            }

            // Set position for the decode channel
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
                throw new Exception("Error: The playlist has no current item!");
            }
            
            // Calculate new position
            long newPosition = (long)Math.Ceiling((double)Playlist.CurrentItem.LengthBytes * (percentage / 100));

            // Set main channel position to 0 (clear buffer)
            m_mainChannel.SetPosition(0);

            // Set offset position (for calulating current position)
            m_positionOffset = newPosition;

            // Check if this is a FLAC file over 44100Hz
            if (Playlist.CurrentItem.AudioFile.FileType == AudioFileFormat.FLAC && Playlist.CurrentItem.AudioFile.SampleRate > 44100)
            {
                // Divide by 1.5 (I don't really know why, but this works for 48000Hz and 96000Hz. Maybe a bug in BASS with FLAC files?)
                newPosition = (long)((float)newPosition / 1.5f);
            }

            // Set position for the decode channel
            Playlist.CurrentItem.Channel.SetPosition(newPosition);
        }

        /// <summary>
        /// Go to a marker position.
        /// </summary>
        /// <param name="marker">Marker position</param>
        public void GoToMarker(Marker marker)
        {
            // Empty buffer
            m_mainChannel.SetPosition(0);

            // Set current song position
            Playlist.CurrentItem.Channel.SetPosition(marker.PositionBytes);

            // Set offset
            m_positionOffset = marker.PositionBytes;
        }

        /// <summary>
        /// Starts a loop. The playback must be activated.
        /// </summary>
        /// <param name="loop">Loop to apply</param>
        public void StartLoop(Loop loop)
        {
            try
            {
                // Set loop sync proc
                Tracing.Log("Player.StartLoop -- Setting sync...");
                Playlist.CurrentItem.SyncProc = new SYNCPROC(LoopSyncProc);
                Playlist.CurrentItem.SyncProcHandle = Playlist.CurrentItem.Channel.SetSync(BASSSync.BASS_SYNC_POS | BASSSync.BASS_SYNC_MIXTIME, loop.EndPositionBytes * 2, Playlist.CurrentItem.SyncProc);

                // Empty buffer
                m_mainChannel.SetPosition(0);

                // Set current song position to marker A
                Tracing.Log("Player.StartLoop -- Setting start position...");
                Playlist.CurrentItem.Channel.SetPosition(loop.StartPositionBytes);

                // Set offset
                m_positionOffset = loop.StartPositionBytes;

                // Set current loop
                m_currentLoop = loop;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Stops any loop currently playing.
        /// </summary>
        public void StopLoop()
        {
            try
            {
                // Make sure there is a loop to stop
                if (m_currentLoop == null)
                {
                    throw new Exception("The current loop is null!");
                }

                // Remove sync proc
                Tracing.Log("Player.StopLoop -- Removing sync...");
                Playlist.CurrentItem.Channel.RemoveSync(Playlist.CurrentItem.SyncProcHandle);

                // Clear the audio buffer                
                m_mainChannel.SetPosition(0);
                m_mainChannel.Lock(true);
                m_positionOffset = m_playlist.CurrentItem.Channel.GetPosition();
                m_mainChannel.Lock(false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                // Release loop
                m_currentLoop = null;
            }
        }

        #endregion

        #region Other Methods (EQ)

        /// <summary>
        /// Adds the 18-band equalizer.
        /// </summary>
        /// <param name="preset">EQ preset to apply</param>
        protected void AddEQ(EQPreset preset)
        {
            // Validate that the main channel exists
            if (m_mainChannel == null)
            {
                throw new Exception("Error adding EQ: The main channel doesn't exist!");
            }

            // Check if an handle already exists
            if (m_fxEQHandle != 0 || m_isEQEnabled)
            {
                throw new Exception("Error adding EQ: The equalizer already exists!");
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
                UpdateEQBand(a, currentBand.Gain, true);
            }

            // Set flags
            m_isEQEnabled = true;
        }

        /// <summary>
        /// Removes the 18-band equalizer.
        /// </summary>
        protected void RemoveEQ()
        {
            // Validate that the main channel exists
            if (m_mainChannel == null)
            {
                throw new Exception("Error removing EQ: The main channel doesn't exist!");
            }

            // Check if the EQ is enabled
            if (!m_isEQEnabled)
            {
                throw new Exception("Error removing EQ: The EQ isn't activated!");
            }

            // Remove EQ
            m_mainChannel.RemoveFX(m_fxEQHandle);
            m_fxEQHandle = 0;
            m_isEQEnabled = false;
        }

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
        /// <param name="setCurrentEQPresetValue">If true, the current EQ preset value will be updated</param>
        public void UpdateEQBand(int band, float gain, bool setCurrentEQPresetValue)
        {
            BASS_BFX_PEAKEQ eq = GetEQParams(band);
            eq.fGain = gain;
            Bass.BASS_FXSetParameters(m_fxEQHandle, eq);

            // Set EQ preset value too?
            if (setCurrentEQPresetValue)
            {
                // Set EQ preset value
                m_currentEQPreset.Bands[band].Gain = gain;
            }
        }

        /// <summary>
        /// Bypasses the 18-band equalizer.        
        /// </summary>
        public void BypassEQ()
        {
            // The problem is that we're recreating the main channel every time the playback of a playlist
            // starts, and this changes the handle to the EQ effect. This means that bypassing the EQ
            // when there is no playback does nothing.

            // Reset flag
            m_isEQBypassed = !m_isEQBypassed;

            // Check if the main channel exists
            if (m_mainChannel == null)
            {
                // Nothing to bypass
                return;
            }

            // Check the new bypass state
            if (m_isEQBypassed)
            {
                // Reset EQ
                ResetEQ();
            }
            else
            {
                // Reapply current EQ preset
                ApplyEQPreset(m_currentEQPreset);
            }
        }

        /// <summary>
        /// Applies a preset on the 18-band equalizer. 
        /// The equalizer needs to be created using the AddEQ method.
        /// </summary>
        public void ApplyEQPreset(EQPreset preset)
        {
            // Load 18-band equalizer
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
                UpdateEQBand(a, currentBand.Gain, true);
            }
        }

        /// <summary>
        /// Resets the gain of every EQ band.
        /// </summary>
        public void ResetEQ()
        {
            // Validate that the main channel exists
            if (m_mainChannel == null)
            {
                throw new Exception("Error resetting EQ: The main channel doesn't exist!");
            }

            // Loop through bands
            for (int a = 0; a < m_currentEQPreset.Bands.Count; a++)
            {
                // Reset gain
                UpdateEQBand(a, 0.0f, false);
                //m_currentEQPreset.Bands[a].Gain = 0.0f;
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

                // Check if a loop is enabled
                if (m_currentLoop != null)
                {
                    // Get current position
                    long position = m_playlist.CurrentItem.Channel.GetPosition();

                    // Check if the position is lower than the start position, or if the position is after the end position
                    if (position < m_currentLoop.StartPositionBytes || position > m_currentLoop.EndPositionBytes)
                    {
                        // Set position to start position
                        m_playlist.CurrentItem.Channel.SetPosition(m_currentLoop.StartPositionBytes);
                    }
                }

                // Get data from the current channel since it is running                
                int data = m_playlist.CurrentItem.Channel.GetData(buffer, length);  
              
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

                return data;
            }
            else if (status == BASSActive.BASS_ACTIVE_STOPPED)
            {
                // Clear current loop
                m_currentLoop = null;

                // Reset offset
                m_positionOffset = 0;

                //m_syncProc = new SYNCPROC(LoopSyncProc);
                //int handle = .SetSync(BASSSync.BASS_SYNC_POS | BASSSync.BASS_SYNC_MIXTIME, loop.EndPositionBytes * 2, Playlist.CurrentItem.SyncProc);

                // Need to get the main output current position and the data left in the buffer.
                // Lock channel
                m_mainChannel.Lock(true);
                
                // Get position
                long position = m_mainChannel.GetPosition();
                
                int buffered = m_mainChannel.GetData(IntPtr.Zero, (int)BASSData.BASS_DATA_AVAILABLE);
                
                m_mainChannel.Lock(false);                

                // Since we're using floating point, divide values by 2
                position /= 2;
                buffered /= 2;

                //if (position >= buffered) position -= buffered;

                // Set position seeked
                m_positionOffset = 0 - position - buffered;

                // Check if this is a FLAC file over 44100Hz
                if (Playlist.CurrentItem.AudioFile.FileType == AudioFileFormat.FLAC && Playlist.CurrentItem.AudioFile.SampleRate > 44100)
                {
                    // Multiply by 1.5 (I don't really know why, but this works for 48000Hz and 96000Hz. Maybe a bug in BASS with FLAC files?)
                    m_positionOffset = (long)((float)m_positionOffset * 1.5f);
                }

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

                        // Load second item if it exists
                        if (Playlist.Items.Count > 1)
                        {
                            Playlist.Items[1].Load();
                        }

                        // Raise song end event (if an event is subscribed)
                        if (OnPlaylistIndexChanged != null)
                        {
                            // Create data
                            PlayerPlaylistIndexChangedData data = new PlayerPlaylistIndexChangedData();
                            data.IsPlaybackStopped = false;
                            data.AudioFileStarted = Playlist.Items[0].AudioFile;

                            // Raise event
                            OnPlaylistIndexChanged(data);
                        }

                        // Return data from the new channel                        
                        return Playlist.CurrentItem.Channel.GetData(buffer, length);
                    }
                    else
                    {
                        // Raise song end event (if an event is subscribed)
                        if (OnPlaylistIndexChanged != null)
                        {
                            // Keep current item in memory
                            AudioFile audioFile = m_playlist.CurrentItem.AudioFile;

                            // Check if EQ is enabled
                            if (m_isEQEnabled)
                            {
                                // Remove EQ
                                RemoveEQ();
                            }

                            // Dispose channels
                            m_playlist.DisposeChannels();
                            
                            // Set flag
                            m_isPlaying = false;

                            // Create data
                            PlayerPlaylistIndexChangedData data = new PlayerPlaylistIndexChangedData();
                            data.AudioFileEnded = audioFile;
                            data.IsPlaybackStopped = true;

                            // Raise event
                            OnPlaylistIndexChanged(data);
                        }
                    }

                    // This is the end of the playlist                    
                    return (int)BASSStreamProc.BASS_STREAMPROC_END;
                }

                // There is another item to play; go on.                
                Playlist.Next();

                // Raise song end event (if an event is subscribed)
                if (OnPlaylistIndexChanged != null)
                {
                    // Create data
                    PlayerPlaylistIndexChangedData data = new PlayerPlaylistIndexChangedData();
                    data.IsPlaybackStopped = false;
                    data.AudioFileEnded = Playlist.Items[Playlist.CurrentItemIndex - 1].AudioFile;
                    data.AudioFileStarted = Playlist.CurrentItem.AudioFile;

                    // Raise event
                    OnPlaylistIndexChanged(data);
                }

                // Compare sample rate on stream channel
                if (m_mainChannel.SampleRate != Playlist.CurrentItem.AudioFile.SampleRate)
                {
                    // Set new sample rate
                    m_mainChannel.SetSampleRate(Playlist.CurrentItem.AudioFile.SampleRate);
                }

                // Return data from the new channel
                return Playlist.CurrentItem.Channel.GetData(buffer, length);
            }

            // Raise song end event (if an event is subscribed)
            if (OnPlaylistIndexChanged != null)
            {
                // Create data
                PlayerPlaylistIndexChangedData data = new PlayerPlaylistIndexChangedData();
                data.IsPlaybackStopped = true;

                // Raise event
                OnPlaylistIndexChanged(data);
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
            // Increment position offset
            //m_positionOffset -= CurrentLoop.LengthBytes;

            // Empty audio buffer
            m_mainChannel.SetPosition(0);

            m_positionOffset = CurrentLoop.StartPositionBytes;


            // Set loop position
            Bass.BASS_ChannelSetPosition(channel, CurrentLoop.StartPositionBytes * 2);
        }        

        #endregion
    }

}
