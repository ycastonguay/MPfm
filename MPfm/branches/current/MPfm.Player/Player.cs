//
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
using Un4seen.Bass.AddOn.Mix;
using System.Threading.Tasks;

namespace MPfm.Player
{
    /// <summary>
    /// The Player class manages audio playback through playlists and supports
    /// multiple driver types and devices.
    /// </summary>
    public class Player : MPfm.Player.IPlayer
    {
        private System.Timers.Timer timerPlayer = null;
        private Channel streamChannel = null;

        /// <summary>
        /// Private value for the FXChannel property.
        /// </summary>
        private Channel fxChannel = null;

        /// <summary>
        /// Effects stream/channel.
        /// </summary>
        public Channel FXChannel
        {
            get
            {
                return fxChannel;
            }
        }        

        /// <summary>
        /// Indicates the current playlist item index used by the mixer.
        /// The playlist index is incremented when the mixer starts to play the next song.
        /// Depending on the buffer size used, the index can be incremented a few seconds before actually hearing
        /// the song change.
        /// </summary>
        int currentMixPlaylistIndex = 0;

        // Plugin handles
        private int fxEQHandle;
        private int aacPluginHandle = 0;        
        private int apePluginHandle = 0;
        private int flacPluginHandle = 0;
        private int mpcPluginHandle = 0;
        private int ofrPluginHandle = 0;
        private int ttaPluginHandle = 0;
        private int wvPluginHandle = 0;
        private int wmaPluginHandle = 0;

        /// <summary>
        /// Offset position (necessary to calculate the offset in the output stream position
        /// if the user has seeked the position in the decode stream). The output stream position
        /// is reset to 0 in these cases to clear the audio buffer.
        /// </summary>
        private long positionOffset = 0;

        #region Callbacks

        private List<PlayerSyncProc> syncProcs = null;

        // Callbacks
        private STREAMPROC streamProc;
        private ASIOPROC asioProc;
        private WASAPIPROC wasapiProc;

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

        #endregion

        #region Properties

        public bool IsSettingPosition { get; private set; }

        /// <summary>
        /// Private value for the IsPlaying property.
        /// </summary>
        private bool isPlaying = false;
        /// <summary>
        /// Indicates if the player is currently playing an audio file.
        /// </summary>
        public bool IsPlaying
        {
            get
            {
                return isPlaying;
            }
        }

        /// <summary>
        /// Private value for the IsPaused property.
        /// </summary>
        private bool isPaused = false;
        /// <summary>
        /// Indicates if the player is currently paused.
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return isPaused;
            }
        }

        /// <summary>
        /// Private value for the Device property.
        /// </summary>
        private Device device = null;
        /// <summary>
        /// Defines the currently used device for playback.
        /// </summary>
        public Device Device
        {
            get
            {
                return device;
            }
        }

        /// <summary>
        /// Private value for the IsDeviceInitialized property.
        /// </summary>
        private bool isDeviceInitialized = false;
        /// <summary>
        /// Indicates if the device (as in the Device property) is initialized.
        /// </summary>
        public bool IsDeviceInitialized
        {
            get
            {
                return isDeviceInitialized;
            }
        }       

        /// <summary>
        /// Private value for the RepeatType property.
        /// </summary>
        private RepeatType repeatType = RepeatType.Off;
        /// <summary>
        /// Repeat type (Off, Playlist, Song)
        /// </summary>
        public RepeatType RepeatType
        {
            get
            {
                return repeatType;
            }
            set
            {
                repeatType = value;

                // Check if the current song exists
                if (playlist != null && playlist.CurrentItem != null)
                {
                    // Check if the repeat type is Song
                    if (repeatType == RepeatType.Song)
                    {
                        // Force looping
                        playlist.CurrentItem.Channel.SetFlags(BASSFlag.BASS_SAMPLE_LOOP, BASSFlag.BASS_SAMPLE_LOOP);
                    }
                    else
                    {
                        // Remove looping
                        playlist.CurrentItem.Channel.SetFlags(BASSFlag.BASS_DEFAULT, BASSFlag.BASS_SAMPLE_LOOP);
                    }
                }
            }
        }

        /// <summary>
        /// Private value for the Volume property.
        /// </summary>
        private float volume = 1.0f;
        /// <summary>
        /// Defines the master volume (from 0 to 1).
        /// </summary>
        public float Volume
        {
            get
            {
                return volume;
            }
            set
            {
                // Set value
                volume = value;

                // Check if the player is playing
                if (mixerChannel != null)
                {
                    // Set main volume
                    mixerChannel.Volume = value;
                }

                // Check driver type
                if (device.DriverType == DriverType.ASIO)
                {
                    // Set ASIO channel volume on left and right channel
                    bool success = BassAsio.BASS_ASIO_ChannelSetVolume(false, 0, value);
                    if (!success)
                    {
                        // Check for error
                        Base.CheckForError();
                    }
                    success = BassAsio.BASS_ASIO_ChannelSetVolume(false, 1, value);
                    if (!success)
                    {
                        // Check for error
                        Base.CheckForError();
                    }
                }
                else if (device.DriverType == DriverType.WASAPI)
                {
                    // Set WASAPI volume
                    bool success = BassWasapi.BASS_WASAPI_SetVolume(true, value);
                    if (!success)
                    {
                        // Check for error
                        Base.CheckForError();
                    }
                }
            }
        }

        /// <summary>
        /// Private value for the TimeShifting property.
        /// </summary>
        private float timeShifting = 0.0f;
        /// <summary>
        /// Defines the time shifting applied to the currently playing stream.
        /// Value range from -100.0f (-100%) to 100.0f (+100%). To reset, set to 0.0f.
        /// </summary>
        public float TimeShifting
        {
            get
            {
                return timeShifting;
            }
            set
            {
                timeShifting = value;

                // Check if the fx channel exists
                if (fxChannel != null)
                {
                    // Set time shifting
                    fxChannel.SetAttribute(BASSAttribute.BASS_ATTRIB_TEMPO, timeShifting);
                }
            }           
        }

        /// <summary>
        /// Private value for the MixerSampleRate property.
        /// </summary>
        private int mixerSampleRate = 44100;
        /// <summary>
        /// Defines the sample rate of the mixer.
        /// </summary>
        public int MixerSampleRate
        {
            get
            {
                return mixerSampleRate;
            }
        }

        /// <summary>
        /// Private value for the BufferSize property.
        /// </summary>
        private int bufferSize = 1000;
        /// <summary>
        /// Defines the buffer size (in milliseconds). Increase this value if older computers have trouble
        /// filling up the buffer in time.        
        /// Default value: 1000ms. The default BASS value is 500ms.
        /// </summary>
        public int BufferSize
        {
            get
            {
                return bufferSize;
            }
            set
            {
                bufferSize = value;

                // Set configuration
                Base.SetConfig(BASSConfig.BASS_CONFIG_BUFFER, bufferSize);
            }
        }

        /// <summary>
        /// Private value for the UpdatePeriod property.
        /// </summary>
        private int updatePeriod = 10;
        /// <summary>
        /// Defines how often BASS fills the buffer to make sure it is always full (in milliseconds).
        /// This affects the accuracy of the ChannelGetPosition value.
        /// Default value: 10ms. The default BASS value is 100ms.
        /// </summary>
        public int UpdatePeriod
        {
            get
            {
                return updatePeriod;
            }
            set
            {
                updatePeriod = value;

                // Set configuration
                Base.SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, updatePeriod);
            }
        }

        /// <summary>
        /// Private value for the UpdateThreads property.
        /// </summary>
        private int updateThreads = 1;
        /// <summary>
        /// Defines how many threads BASS can use to update playback buffers in parrallel.
        /// Note: The playback engine plays perfectly with just one update thread.
        /// Default value: 1 thread. The default BASS value is 1 thread.
        /// </summary>
        public int UpdateThreads
        {
            get
            {
                return updateThreads;
            }
            set
            {
                updateThreads = value;

                // Set configuration
                Base.SetConfig(BASSConfig.BASS_CONFIG_UPDATETHREADS, updateThreads);
            }
        }

        /// <summary>
        /// Private value for the MixerChannel property.
        /// </summary>
        private MixerChannel mixerChannel = null;
        /// <summary>
        /// Pointer to the mixer channel.
        /// </summary>
        public MixerChannel MixerChannel
        {
            get
            {
                return mixerChannel;
            }
        }

        /// <summary>
        /// Private value for the Playlist property.
        /// </summary>
        private Playlist playlist = null;
        /// <summary>
        /// Playlist used for playback. Contains the audio file metadata and decode channels for
        /// playback.
        /// </summary>
        public Playlist Playlist
        {
            get
            {
                return playlist;
            }
        }

        /// <summary>
        /// Private value for the CurrentEQPreset property.
        /// </summary>
        private EQPreset currentEQPreset = null;
        /// <summary>
        /// Defines the current EQ preset.
        /// </summary>
        public EQPreset CurrentEQPreset
        {
            get
            {
                return currentEQPreset;
            }
            set
            {
                currentEQPreset = value;
            }
        }

        /// <summary>
        /// Private value for the IsEQEnabled property.
        /// </summary>
        private bool isEQEnabled = false;
        /// <summary>
        /// Indicates if the EQ is enabled.
        /// </summary>
        public bool IsEQEnabled
        {
            get
            {
                return isEQEnabled;
            }
        }

        /// <summary>
        /// Private value for the IsEQBypassed property.
        /// </summary>
        private bool isEQBypassed = false;
        /// <summary>
        /// Indicates if the EQ is bypassed.
        /// </summary>
        public bool IsEQBypassed
        {
            get
            {
                return isEQBypassed;
            }
        }

        #region Loops and Markers
        
        /// <summary>
        /// Private value for the Markers property.
        /// </summary>
        private List<Marker> markers = null;
        /// <summary>
        /// Defines a collection of markers.
        /// </summary>
        public List<Marker> Markers
        {
            get
            {
                return markers;
            }
        }

        /// <summary>
        /// Private value for the Loops property.
        /// </summary>
        private List<Loop> loops = null;
        /// <summary>
        /// Defines a collection of loops.
        /// </summary>
        public List<Loop> Loops
        {
            get
            {
                return loops;
            }
        }

        /// <summary>
        /// Private value for the CurrentLoop property.
        /// </summary>
        private Loop currentLoop = null;
        /// <summary>
        /// Defines the currently playing loop.
        /// </summary>
        public Loop CurrentLoop
        {
            get
            {
                return currentLoop;
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
            Initialize(new Device(), 44100, 1000, 10, true);
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
            this.device = device;
            this.mixerSampleRate = mixerSampleRate;
            this.bufferSize = bufferSize;
            this.updatePeriod = updatePeriod;

            // Create lists            
            playlist = new Playlist();
            markers = new List<Marker>();
            loops = new List<Loop>();
            syncProcs = new List<PlayerSyncProc>();

            // Create timer
            Tracing.Log("Player init -- Creating timer...");
            timerPlayer = new System.Timers.Timer();
            timerPlayer.Elapsed += new System.Timers.ElapsedEventHandler(timerPlayer_Elapsed);
            timerPlayer.Interval = 1000;
            timerPlayer.Enabled = false;

			// Register BASS.NET
			Un4seen.Bass.BassNet.Registration("yanick.castonguay@gmail.com", "2X3433427152222");		
			
            // Initialize BASS library by OS type
            if (OS.Type == OSType.Windows)
            {
                // Load decoding plugins
                //plugins = Base.LoadPluginDirectory(Path.GetDirectoryName((new Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath));
                Tracing.Log("Player init -- Loading plugins...");
                //aacPluginHandle = Base.LoadPlugin("bass_aac.dll");
                apePluginHandle = Base.LoadPlugin("bass_ape.dll");
                flacPluginHandle = Base.LoadPlugin("bassflac.dll");
                mpcPluginHandle = Base.LoadPlugin("bass_mpc.dll");
                //ofrPluginHandle = Base.LoadPlugin("bass_ofr.dll"); // Requires OptimFrog.DLL
                //ttaPluginHandle = Base.LoadPlugin("bass_tta.dll");
                wmaPluginHandle = Base.LoadPlugin("basswma.dll");
                wvPluginHandle = Base.LoadPlugin("basswv.dll");     
								            
            	Base.LoadFxPlugin();
            }
			else // Linux or Mac OS X
			{				
				// Load BASS library
				Tracing.Log("Player init -- Checking BASS library and plugin versions...");
				int bassVersion = Base.GetBASSVersion();				
				int bassFxVersion = Base.GetFxPluginVersion();
				int bassMixVersion = Base.GetMixPluginVersion();
				Tracing.Log("Player init -- BASS Version: " + bassVersion);
				Tracing.Log("Player init -- BASS FX Version: " + bassFxVersion);
				Tracing.Log("Player init -- BASS Mix Version: " + bassMixVersion);

				// Check OS type
	            if (OS.Type == OSType.Linux)
	            {				
					// Find plugins either in current directory (i.e. development) or in a system directory (ex: /usr/lib/mpfm or /opt/lib/mpfm)								
					string pluginPath = string.Empty;				
					string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);				
					
					// Check in the current directory first
					if(!File.Exists(exePath + "/libbassflac.so"))
					{
						// Check in /usr/lib/mpfm
						if(!File.Exists("/usr/lib/mpfm/libbassflac.so"))
						{
							// Check in /opt/lib/mpfm
							if(!File.Exists("/opt/lib/mpfm/libbassflac.so"))
							{
								// The plugins could not be found!
								throw new Exception("The BASS plugins could not be found either in the current directory, in /usr/lib/mpfm or in /opt/lib/mpfm!");
							}
							else
							{
								pluginPath = "/opt/lib/mpfm";
							}						
						}
						else
						{
							pluginPath = "/usr/lib/mpfm";
						}
					}
					else
					{					
						pluginPath = exePath;
					}
					
	                // Load decoding plugins
					flacPluginHandle = Base.LoadPlugin(pluginPath + "/libbassflac.so");
					wvPluginHandle = Base.LoadPlugin(pluginPath + "/libbasswv.so");
					mpcPluginHandle = Base.LoadPlugin(pluginPath + "/libbass_mpc.so");
	            }
	            else if (OS.Type == OSType.MacOSX)
	            {
					// Find plugins either in current directory (i.e. development) or in a system directory (ex: /usr/lib/mpfm or /opt/lib/mpfm)								
					string pluginPath = string.Empty;				
					
                    // Try to get the plugins in the current path
                    string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

#if DEBUG
                        pluginPath = exePath;
#else
                        pluginPath = exePath.Replace("MonoBundle", "Resources");
#endif

                    // Check in the current directory first
                    if(!File.Exists(pluginPath + "/libbassflac.dylib"))
                    {
                        // The plugins could not be found!
                        throw new Exception("The BASS plugins could not be found in the current directory!");
                    }

				    // Load decoding plugins
					flacPluginHandle = Base.LoadPlugin(pluginPath + "/libbassflac.dylib");
                    wvPluginHandle = Base.LoadPlugin(pluginPath + "/libbasswv.dylib");
                    mpcPluginHandle = Base.LoadPlugin(pluginPath + "/libbass_mpc.dylib");
	            }
			}
						
            // Create default EQ
            Tracing.Log("Player init -- Creating default EQ preset...");
            currentEQPreset = new EQPreset();

            // Initialize device
            if (initializeDevice)
            {
                Tracing.Log("Player init -- Initializing device...");
                InitializeDevice(device, mixerSampleRate);
            }
        }

        /// <summary>
        /// Initializes the default audio device for playback.
        /// </summary>
        public void InitializeDevice()
        {
            // Initialize default device
            InitializeDevice(new Device(), mixerSampleRate);
        }

        /// <summary>
        /// Initializes a specific audio device for playback.
        /// </summary>
        /// <param name="device">Audio device</param>
        /// <param name="mixerSampleRate">Mixer sample rate (in Hz)</param>
        public void InitializeDevice(Device device, int mixerSampleRate)
        {
            // Set properties
            this.device = device;
            this.mixerSampleRate = mixerSampleRate;

            Tracing.Log("Player -- Initializing device (SampleRate: " + mixerSampleRate.ToString() + " Hz, DriverType: " + device.DriverType.ToString() + ", Id: " + device.Id.ToString() + ", Name: " + device.Name + ", BufferSize: " + bufferSize.ToString() + ", UpdatePeriod: " + updatePeriod.ToString() + ")");

            // Check driver type
            if (device.DriverType == DriverType.DirectSound)
            {
                // Initialize sound system                
                Base.Init(device.Id, mixerSampleRate, BASSInit.BASS_DEVICE_DEFAULT);
            }
            else if (device.DriverType == DriverType.ASIO)
            {
                // Initialize sound system
                Base.InitASIO(device.Id, mixerSampleRate, BASSInit.BASS_DEVICE_DEFAULT, BASSASIOInit.BASS_ASIO_THREAD);
            }
            else if (device.DriverType == DriverType.WASAPI)
            {
                // Create callback
                wasapiProc = new WASAPIPROC(WASAPICallback);

                // Initialize sound system                
                //Base.InitWASAPI(device.Id, mixerSampleRate, 2, BASSInit.BASS_DEVICE_DEFAULT, BASSWASAPIInit.BASS_WASAPI_SHARED, 10.0f, 0, wasapiProc);
                Base.InitWASAPI(device.Id, mixerSampleRate, 2, BASSInit.BASS_DEVICE_DEFAULT, BASSWASAPIInit.BASS_WASAPI_SHARED, 0.5f, 0, wasapiProc);
                //Base.InitWASAPI(device.Id, mixerSampleRate, 2, BASSInit.BASS_DEVICE_DEFAULT, BASSWASAPIInit.BASS_WASAPI_EXCLUSIVE, 1, 0, wasapiProc);

                //BASS_WASAPI_INFO info = BassWasapi.BASS_WASAPI_GetInfo();
            }

            // Default BASS.NET configuration values for Windows *AND* Linux:
            //
            // BASS_CONFIG_BUFFER: 500
            // BASS_CONFIG_UPDATEPERIOD: 100
            // BASS_CONFIG_UPDATETHREADS: 1
			
			if(OS.Type == OSType.Windows)
			{
	            // Set configuration for buffer and update period
            	// This only works for the default BASS output (http://www.un4seen.com/forum/?topic=13429.msg93740#msg93740)
            	Base.SetConfig(BASSConfig.BASS_CONFIG_BUFFER, bufferSize); 
            	Base.SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, updatePeriod);	
			}
            else if (OS.Type == OSType.Linux)
            {				
				// Default
				// 10ms update period does not work under Linux. Major stuttering
            	Base.SetConfig(BASSConfig.BASS_CONFIG_BUFFER, 1000); 
            	Base.SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, 100);	
            }
            else if (OS.Type == OSType.MacOSX)
            {
				// Default
				// 10ms update period does not work under Linux. Major stuttering
            	Base.SetConfig(BASSConfig.BASS_CONFIG_BUFFER, 1000); 
            	Base.SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, 100);					
            }		

            // Set flags            
            isDeviceInitialized = true;
        }

        /// <summary>
        /// Frees the device currently used for playback.
        /// </summary>
        public void FreeDevice()
        {
            // Check if a device has been initialized
            if (!isDeviceInitialized)
            {
                return;
            }

            // Check driver type
            if (device.DriverType == DriverType.ASIO)
            {
                // Free ASIO device
                if (!BassAsio.BASS_ASIO_Free())
                {
                    // Get error
                    BASSError error = Bass.BASS_ErrorGetCode();
                    throw new Exception("Error freeing ASIO device: " + error.ToString());
                }
            }
            else if (device.DriverType == DriverType.WASAPI)
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
            mixerChannel = null;
            device = null;
            isDeviceInitialized = false;
        }

        /// <summary>
        /// Loads the BASS plugins depending on the current platform.
        /// </summary>
        public void LoadPlugins()
        {

        }

        /// <summary>
        /// Frees the BASS plugins used by the player.
        /// </summary>
        public void FreePlugins()
        {
            // Dispose plugins
            Base.FreeFxPlugin();
			
			// Free plugins if they have been loaded successfully
			if(aacPluginHandle > 0)			
            	Base.FreePlugin(aacPluginHandle);
			if(apePluginHandle > 0)			
            	Base.FreePlugin(apePluginHandle);
			if(flacPluginHandle > 0)			
            	Base.FreePlugin(flacPluginHandle);
			if(mpcPluginHandle > 0)
            	Base.FreePlugin(mpcPluginHandle);
			if(ofrPluginHandle > 0)
            	Base.FreePlugin(ofrPluginHandle);
			if(ttaPluginHandle > 0)
            	Base.FreePlugin(ttaPluginHandle);
			if(wvPluginHandle > 0)
            	Base.FreePlugin(wvPluginHandle);
			if(wmaPluginHandle > 0)
            	Base.FreePlugin(wmaPluginHandle);
			
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
        protected void timerPlayer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {            
            if (timerPlayer != null)
            {
                // Reset timer
                timerPlayer.Enabled = false;

                // Check if the next channel needs to be loaded
                if (playlist.CurrentItemIndex < playlist.Items.Count - 1)
                {
                    // Check if the channel has already been loaded
                    if (!playlist.Items[playlist.CurrentItemIndex + 1].IsLoaded)
                    {
                        // Create the next channel
                        playlist.Items[playlist.CurrentItemIndex + 1].Load();
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
                if (isPlaying)
                {
                    // Check if a loop is active
                    if (currentLoop != null)
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
                currentLoop = null;

                // Make sure there are no sync procs
                RemoveSyncCallbacks();

                // Set offset
                positionOffset = 0;
                currentMixPlaylistIndex = Playlist.CurrentItemIndex;

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
                    playlist.Items[a].Load();
                }

                // Start decoding first playlist item
                //playlist.Items[Playlist.CurrentItemIndex].Decode(0);

                try
                {
                    // Create the streaming channel (set the frequency to the first file in the list)
                    Tracing.Log("Player.Play -- Creating streaming channel (SampleRate: " + playlist.CurrentItem.AudioFile.SampleRate + " Hz, FloatingPoint: true)...");
                    streamProc = new STREAMPROC(StreamCallback);
                    streamChannel = MPfm.Sound.BassNetWrapper.Channel.CreateStream(playlist.CurrentItem.AudioFile.SampleRate, 2, true, streamProc);

                    Tracing.Log("Player.Play -- Creating time shifting channel...");
                    fxChannel = MPfm.Sound.BassNetWrapper.Channel.CreateStreamForTimeShifting(streamChannel.Handle, true, true);
                }
                catch(Exception ex)
                {
                    // Raise custom exception with information (so the client application can maybe deactivate floating point for example)
                    PlayerCreateStreamException newEx = new PlayerCreateStreamException("The player has failed to create the stream channel (" + ex.Message + ").", ex);
                    newEx.Decode = true;
                    newEx.UseFloatingPoint = true;
                    newEx.SampleRate = playlist.CurrentItem.AudioFile.SampleRate;
                    throw newEx;
                }

                // Check driver type
                if (device.DriverType == DriverType.DirectSound)
                {
                    try
                    {
                        // Create mixer stream
                        Tracing.Log("Player.Play -- Creating mixer channel (DirectSound)...");
                        mixerChannel = MPfm.Sound.BassNetWrapper.MixerChannel.CreateMixerStream(playlist.CurrentItem.AudioFile.SampleRate, 2, true, false);
                        mixerChannel.AddChannel(fxChannel.Handle);
                    }
                    catch (Exception ex)
                    {   
                        // Raise custom exception with information (so the client application can maybe deactivate floating point for example)
                        PlayerCreateStreamException newEx = new PlayerCreateStreamException("The player has failed to create the time shifting channel.", ex);
                        newEx.UseFloatingPoint = true;
                        newEx.UseTimeShifting = true;
                        newEx.SampleRate = playlist.CurrentItem.AudioFile.SampleRate;
                        throw newEx;
                    }
                }
                else if (device.DriverType == DriverType.ASIO)
                {
                    try
                    {
                        // Create mixer stream
                        Tracing.Log("Player.Play -- Creating mixer channel (ASIO)...");
                        mixerChannel = MPfm.Sound.BassNetWrapper.MixerChannel.CreateMixerStream(playlist.CurrentItem.AudioFile.SampleRate, 2, true, true);
                        mixerChannel.AddChannel(fxChannel.Handle);

                    }
                    catch (Exception ex)
                    {
                        // Raise custom exception with information (so the client application can maybe deactivate floating point for example)
                        PlayerCreateStreamException newEx = new PlayerCreateStreamException("The player has failed to create the time shifting channel.", ex);
                        newEx.DriverType = DriverType.ASIO;
                        newEx.UseFloatingPoint = true;
                        newEx.UseTimeShifting = true;
                        newEx.Decode = true;
                        newEx.SampleRate = playlist.CurrentItem.AudioFile.SampleRate;
                        throw newEx;  
                    }
                    
                    // Set floating point
                    BassAsio.BASS_ASIO_ChannelSetFormat(false, 0, BASSASIOFormat.BASS_ASIO_FORMAT_FLOAT);

                    // Create callback
                    asioProc = new ASIOPROC(AsioCallback);

                    try
                    {
                        // Enable and join channels (for stereo output
                        Tracing.Log("Player.Play -- Enabling ASIO channels...");
                        BassAsio.BASS_ASIO_ChannelEnable(false, 0, asioProc, new IntPtr(mixerChannel.Handle));
                        Tracing.Log("Player.Play -- Joining ASIO channels...");
                        BassAsio.BASS_ASIO_ChannelJoin(false, 1, 0);

                        // Set sample rate
                        Tracing.Log("Player.Play -- Set ASIO sample rates...");
                        BassAsio.BASS_ASIO_ChannelSetRate(false, 0, playlist.CurrentItem.AudioFile.SampleRate);
                        BassAsio.BASS_ASIO_SetRate(playlist.CurrentItem.AudioFile.SampleRate);
                    }
                    catch (Exception ex)
                    {
                        // Raise custom exception with information (so the client application can maybe deactivate floating point for example)
                        PlayerCreateStreamException newEx = new PlayerCreateStreamException("The player has failed to enable or join ASIO channels.", ex);
                        newEx.DriverType = DriverType.ASIO;
                        newEx.SampleRate = playlist.CurrentItem.AudioFile.SampleRate;
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
                        newEx.SampleRate = playlist.CurrentItem.AudioFile.SampleRate;
                        throw newEx;                        
                    }
                }
                else if (device.DriverType == DriverType.WASAPI)
                {
                    // Create mixer stream
                    Tracing.Log("Player.Play -- Creating mixer channel (WASAPI)...");
                    mixerChannel = MPfm.Sound.BassNetWrapper.MixerChannel.CreateMixerStream(playlist.CurrentItem.AudioFile.SampleRate, 2, true, true);
                    mixerChannel.AddChannel(fxChannel.Handle);

                    // Start playback
                    if (!BassWasapi.BASS_WASAPI_Start())
                    {
                        // Get error
                        BASSError error = Bass.BASS_ErrorGetCode();
                        throw new Exception("Player.Play error: Error playing files in WASAPI: " + error.ToString());
                    }
                }

                // Set initial volume
                mixerChannel.Volume = Volume;

                // Load 18-band equalizer
                Tracing.Log("Player.Play -- Creating equalizer (Preset: " + currentEQPreset + ")...");
                //AddEQ(currentEQPreset);

                // Check if EQ is bypassed
                if (isEQBypassed)
                {
                    // Reset EQ
                    Tracing.Log("Player.Play -- Equalizer is bypassed; resetting EQ...");
                    ResetEQ();
                }

                // Check if the repeat type is Song
                if (repeatType == RepeatType.Song)
                {
                    // Force looping                    
                    playlist.CurrentItem.Channel.SetFlags(BASSFlag.BASS_SAMPLE_LOOP, BASSFlag.BASS_SAMPLE_LOOP);
                }

                // Get audio file length
                long length = playlist.CurrentItem.Channel.GetLength();

                // Set sync (length already in floating point)          
                SetSyncCallback(length);

                // Start playback
                isPlaying = true;
                isPaused = false;

                // Only the DirectSound mode needs to start the main channel since it's not in decode mode.
                if (device.DriverType == DriverType.DirectSound)
                {
                    // Start playback
                    Tracing.Log("Player.Play -- Starting DirectSound playback...");
                    mixerChannel.Play(false);
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("Player.Play error: " + ex.Message + "\n" + ex.StackTrace);
                throw;
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
            if (isPlaying)
            {
                // Stop playback
                Stop();
            }

            // Reset flags                
            playlist.Clear();

            // Create playlist items
            foreach (AudioFile audioFile in audioFiles)
            {
                // Add playlist item
                playlist.AddItem(audioFile);
            }

            // Set playlist to first item
            playlist.First();
          
            // Start playback
            Play();
        }

        /// <summary>
        /// Pauses the audio playback. Resumes the audio playback if it was paused.
        /// </summary>
        public void Pause()
        {
            // Check driver type (the pause mechanism differs by driver)
            if (device.DriverType == DriverType.DirectSound)
            {        
                // Check if the playback is already paused
                if (!isPaused)
                {
                    // Pause the playback channel
                    mixerChannel.Pause();
                }
                else
                {
                    // Unpause the playback channel
                    mixerChannel.Play(false);
                }
            }
            else if (device.DriverType == DriverType.ASIO)
            {
                // Check if the playback is already paused
                if (!isPaused)
                {
                    // Pause playback on ASIO (cannot pause in a decoding channel)
                    if (!BassAsio.BASS_ASIO_ChannelPause(false, 0))
                    {
                        // Check for error
                        Base.CheckForError();
                    }
                }
                else
                {
                    // Unpause playback
                    if (!BassAsio.BASS_ASIO_ChannelReset(false, 0, BASSASIOReset.BASS_ASIO_RESET_PAUSE))
                    {
                        // Check for error
                        Base.CheckForError();
                    }

                }
            }
            else if (device.DriverType == DriverType.WASAPI)
            {
                // Check if the playback is already paused
                if (!isPaused)
                {
                    // Pause playback on WASAPI (cannot pause in a decoding channel)
                    if (!BassWasapi.BASS_WASAPI_Stop(false))
                    {
                        // Check for error
                        Base.CheckForError();
                    }
                }
                else
                {
                    // Unpause playback
                    if (!BassWasapi.BASS_WASAPI_Start())
                    {
                        // Check for error
                        Base.CheckForError();
                    }

                }
            }

            // Set flag
            isPaused = !isPaused;
        }

        /// <summary>
        /// Stops the audio playback and frees the resources used by the playback engine.
        /// </summary>
        public void Stop()
        {
            // Check if the main channel exists, and make sure the player is playing
            if (mixerChannel == null)// || !m_isPlaying)
            {
                throw new Exception("Player.Stop error: The main channel is null!");
            }

            // Set flags
            currentLoop = null;
            isPlaying = false;

            // Check if EQ is enabled
            if (isEQEnabled)
            {
                // Remove EQ
                //Tracing.Log("Player.Stop -- Removing equalizer...");
                //RemoveEQ();
            }

            // Stop mixer channel
            mixerChannel.Stop();

            // Check driver type
            if (device.DriverType == DriverType.DirectSound)
            {
                // Stop main channel
                //Tracing.Log("Player.Stop -- Stopping DirectSound channel...");                
                //m_mainChannel.Stop();
            }
            else if (device.DriverType == DriverType.ASIO)
            {
                // Stop playback
                Tracing.Log("Player.Stop -- Stopping ASIO playback...");
                BassAsio.BASS_ASIO_Stop();
            }
            else if (device.DriverType == DriverType.WASAPI)
            {
                // Stop playback
                Tracing.Log("Player.Stop -- Stopping WASAPI playback...");
                BassWasapi.BASS_WASAPI_Stop(false);
            }

            // Stop decoding the current file (doesn't throw an exception if decode has finished)
            Playlist.CurrentItem.CancelDecode();

            // Remove syncs            
            RemoveSyncCallbacks();

            // Dispose FX channel
            fxChannel.Free();

            // Check if the current song exists
            if (playlist != null && playlist.CurrentItem != null)
            {
                // Dispose channels
                Tracing.Log("Player.Stop -- Disposing channels...");
                playlist.DisposeChannels();
            }

            // Check if WASAPI
            if (device.DriverType == DriverType.WASAPI)
            {
                BassWasapi.BASS_WASAPI_Stop(true);
            }
           
        }

        /// <summary>
        /// Stops the playback and starts playback at a specific playlist song.
        /// </summary>
        /// <param name="index">Playlist item index</param>
        public void GoTo(int index)
        {
            // Stop playback
            Stop();

            // Skip to playlist item
            Playlist.GoTo(index);
            currentMixPlaylistIndex = index;

            // Get audio file (to raise event later)
            AudioFile audioFileStarted = Playlist.Items[index].AudioFile;

            // Start playback
            Play();

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

            if(IsSettingPosition)
                return 0;

            // Get main channel position
            long outputPosition = mixerChannel.GetPosition(fxChannel.Handle);            
            //long outputPosition = Playlist.CurrentItem.Channel.GetPosition();

            // Divide by 2 (floating point)
            outputPosition /= 2;            

            // Check if this is a FLAC file over 44100Hz
            if (Playlist.CurrentItem.AudioFile.FileType == AudioFileFormat.FLAC && Playlist.CurrentItem.AudioFile.SampleRate > 44100)
            {
                // Multiply by 1.5 (I don't really know why, but this works for 48000Hz and 96000Hz. Maybe a bug in BASS with FLAC files?)
                outputPosition = (long)((float)outputPosition * 1.5f);                
            }

            // Add the offset position
            outputPosition += positionOffset;

            return outputPosition;            
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

            // Calculate position in bytes
            long positionBytes = (long)Math.Ceiling((double)Playlist.CurrentItem.LengthBytes * (percentage / 100));

            // Set position
            SetPosition(positionBytes);
        }

        /// <summary>
        /// Sets the position of the currently playing channel.
        /// If the value is in floating point, divide the value by 2 before using it with the "bytes" parameter.
        /// </summary>
        /// <param name="bytes">Position (in bytes)</param>
        public void SetPosition(long bytes)
        {
            // Validate player
            if (Playlist == null || Playlist.CurrentItem == null || Playlist.CurrentItem.Channel == null)
            {
                return;
            }


            // Check if WASAPI
            if (device.DriverType == DriverType.WASAPI)
            {
                BassWasapi.BASS_WASAPI_Stop(true);
                BassWasapi.BASS_WASAPI_Start();
            }

            // Remove any sync callback
            RemoveSyncCallbacks();

            // Get file length
            long length = Playlist.CurrentItem.Channel.GetLength();

            // Lock channel            
            mixerChannel.Lock(true);            
            IsSettingPosition = true;

            // Check if this is a FLAC file over 44100Hz
            positionOffset = bytes;
            if (Playlist.CurrentItem.AudioFile.FileType == AudioFileFormat.FLAC && Playlist.CurrentItem.AudioFile.SampleRate > 44100)
            {
                // Divide by 1.5 (I don't really know why, but this works for 48000Hz and 96000Hz. Maybe a bug in BASS with FLAC files?)
                bytes = (long)((float)bytes / 1.5f);
            }

            // http://www.un4seen.com/forum/?topic=10570.0;hl=decode+channel+position+mixer
            mixerChannel.Stop();
            //Playlist.CurrentItem.Channel.Stop();

            // Set position for the decode channel (needs to be in floating point)
            Playlist.CurrentItem.Channel.SetPosition(0); // Flush buffer
            fxChannel.SetPosition(0);
            mixerChannel.SetPosition(0);
            Playlist.CurrentItem.Channel.SetPosition(bytes * 2);

            // Set new callback (length already in floating point)
            SetSyncCallback((length - (bytes * 2))); // + buffered));

            mixerChannel.Play(false);

            // Unlock channel
            mixerChannel.Lock(false);

            IsSettingPosition = false;
        }

        /// <summary>
        /// Go to a marker position.
        /// </summary>
        /// <param name="marker">Marker position</param>
        public void GoToMarker(Marker marker)
        {
            // Set position
            SetPosition(marker.PositionBytes);
        }

        /// <summary>
        /// Starts a loop. The playback must be activated.
        /// </summary>
        /// <param name="loop">Loop to apply</param>
        public void StartLoop(Loop loop)
        {
            try
            {
                // Validate player
                if (Playlist == null || Playlist.CurrentItem == null || Playlist.CurrentItem.Channel == null)
                {
                    return;
                }

                // Get loop start/end position
                long startPositionBytes = loop.StartPositionBytes;
                long endPositionBytes = loop.EndPositionBytes;

                // Check if this is a FLAC file over 44100Hz
                if (Playlist.CurrentItem.AudioFile.FileType == AudioFileFormat.FLAC && Playlist.CurrentItem.AudioFile.SampleRate > 44100)
                {
                    // Divide by 1.5 (I don't really know why, but this works for 48000Hz and 96000Hz. Maybe a bug in BASS with FLAC files?)
                    startPositionBytes = (long)((float)startPositionBytes / 1.5f);
                    endPositionBytes = (long)((float)endPositionBytes / 1.5f);
                }

                // Check if WASAPI
                if (device.DriverType == DriverType.WASAPI)
                {
                    BassWasapi.BASS_WASAPI_Stop(true);
                    BassWasapi.BASS_WASAPI_Start();
                }

                // Remove any sync callback
                RemoveSyncCallbacks();

                // Get file length
                long length = Playlist.CurrentItem.Channel.GetLength();

                // Lock channel            
                mixerChannel.Lock(true);

                // Set position for the decode channel (needs to be in floating point)
                Playlist.CurrentItem.Channel.SetPosition(startPositionBytes * 2);

                // Set main channel position to 0 (clear buffer)            
                fxChannel.SetPosition(0);
                mixerChannel.SetPosition(0);

                // Set sync
                Playlist.CurrentItem.SyncProc = new SYNCPROC(LoopSyncProc);
                Playlist.CurrentItem.SyncProcHandle = mixerChannel.SetSync(fxChannel.Handle, BASSSync.BASS_SYNC_POS | BASSSync.BASS_SYNC_MIXTIME, (endPositionBytes - startPositionBytes) * 2, Playlist.CurrentItem.SyncProc);

                // Set new callback (length already in floating point)
                SetSyncCallback((length - (startPositionBytes * 2))); // + buffered));

                // Set offset position (for calulating current position)
                positionOffset = startPositionBytes;

                // Unlock channel
                mixerChannel.Lock(false);

                // Set current loop
                currentLoop = loop;
            }
            catch
            {
                throw;
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
                if (currentLoop == null)
                {
                    return;
                }

                // Remove sync proc
                Tracing.Log("Player.StopLoop -- Removing sync...");

                // Remove loop sync proc
                mixerChannel.RemoveSync(fxChannel.Handle, Playlist.CurrentItem.SyncProcHandle);
            }
            catch
            {
                throw;
            }
            finally
            {
                // Release loop
                currentLoop = null;
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
            if (mixerChannel == null)
            {
                throw new Exception("Error adding EQ: The mixer channel doesn't exist!");
            }

            // Check if an handle already exists
            if (fxEQHandle != 0 || isEQEnabled)
            {
                throw new Exception("Error adding EQ: The equalizer already exists!");
            }

            // Load 18-band equalizer
            fxEQHandle = mixerChannel.SetFX(BASSFXType.BASS_FX_BFX_PEAKEQ, 0);
            BASS_BFX_PEAKEQ eq = new BASS_BFX_PEAKEQ();
            currentEQPreset = preset;
            for (int a = 0; a < currentEQPreset.Bands.Count; a++)
            {
                // Get current band
                EQPresetBand currentBand = currentEQPreset.Bands[a];

                // Set equalizer band properties
                eq.lBand = a;
                eq.lChannel = BASSFXChan.BASS_BFX_CHANALL;
                eq.fCenter = currentBand.Center;
                eq.fGain = currentBand.Gain;
                eq.fQ = currentBand.Q;
                Bass.BASS_FXSetParameters(fxEQHandle, eq);
                UpdateEQBand(a, currentBand.Gain, true);
            }

            // Set flags
            isEQEnabled = true;
        }

        /// <summary>
        /// Removes the 18-band equalizer.
        /// </summary>
        protected void RemoveEQ()
        {
            // Validate that the main channel exists
            if (mixerChannel == null)
            {
                Tracing.Log("Player.RemoveEQ -- Error removing EQ: The main channel doesn't exist!");
                return;
            }

            // Check if the EQ is enabled
            if (!isEQEnabled)
            {
                Tracing.Log("Player.RemoveEQ -- Error removing EQ: The EQ isn't activated!");
                return;
            }

            try
            {
                // Remove EQ 
                Tracing.Log("Player.RemoveEQ -- Removing EQ...");
                mixerChannel.RemoveFX(fxEQHandle);
                fxEQHandle = 0;
                isEQEnabled = false;
            } 
            catch (Exception ex)
            {
                // Add error to log, but this error isn't critical, so we can continue execution
                Tracing.Log("Player.RemoveEQ -- Error removing EQ: " + ex.Message + "\n" + ex.StackTrace);
            }
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
            Bass.BASS_FXGetParameters(fxEQHandle, eq);
            
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
            Bass.BASS_FXSetParameters(fxEQHandle, eq);

            // Set EQ preset value too?
            if (setCurrentEQPresetValue)
            {
                // Set EQ preset value
                currentEQPreset.Bands[band].Gain = gain;
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
            isEQBypassed = !isEQBypassed;

            // Check if the main channel exists
            if (mixerChannel == null)
            {
                // Nothing to bypass
                return;
            }

            // Check the new bypass state
            if (isEQBypassed)
            {
                // Reset EQ
                ResetEQ();
            }
            else
            {
                // Reapply current EQ preset
                ApplyEQPreset(currentEQPreset);
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
            currentEQPreset = preset;
            for (int a = 0; a < currentEQPreset.Bands.Count; a++)
            {
                // Get current band
                EQPresetBand currentBand = currentEQPreset.Bands[a];

                // Set equalizer band properties
                eq.lBand = a;
                eq.lChannel = BASSFXChan.BASS_BFX_CHANALL;
                eq.fCenter = currentBand.Center;
                eq.fGain = currentBand.Gain;
                eq.fQ = currentBand.Q;
                Bass.BASS_FXSetParameters(fxEQHandle, eq);
                UpdateEQBand(a, currentBand.Gain, true);
            }
        }

        /// <summary>
        /// Resets the gain of every EQ band.
        /// </summary>
        public void ResetEQ()
        {
            // Validate that the main channel exists
            if (mixerChannel == null)
            {
                //throw new Exception("Error resetting EQ: The main channel doesn't exist!");
                return;
            }

            // Loop through bands
            for (int a = 0; a < currentEQPreset.Bands.Count; a++)
            {
                // Reset gain
                UpdateEQBand(a, 0.0f, false);
                //m_currentEQPreset.Bands[a].Gain = 0.0f;
            }
        }

        #endregion

        #region Synchronization Methods

        /// <summary>
        /// Sets a sync callback for triggering events when the playlist index increments.
        /// </summary>
        /// <param name="position">Sync callback position</param>
        protected PlayerSyncProc SetSyncCallback(long position)
        {                       
            // Set sync
            PlayerSyncProc syncProc = new PlayerSyncProc();
            syncProc.SyncProc = new SYNCPROC(PlayerSyncProc);
            syncProc.Handle = mixerChannel.SetSync(fxChannel.Handle, BASSSync.BASS_SYNC_POS, position, syncProc.SyncProc);            

            // Add to list
            syncProcs.Add(syncProc);

            return syncProc;
        }

        /// <summary>
        /// Removes all synchronization callbacks from the sync callback list.
        /// </summary>
        protected void RemoveSyncCallbacks()
        {
            // Loop
            while (true)
            {
                // Are there any more sync procs to remove?
                if (syncProcs.Count > 0)
                {
                    // Remove sync proc
                    RemoveSyncCallback(syncProcs[0].Handle);                    
                }
                else
                {
                    // Exit loop
                    break;
                }
            }
        }

        /// <summary>
        /// Removes a synchronization callback from the sync callback list.
        /// </summary>
        /// <param name="handle">Synchronization callback handle</param>
        protected void RemoveSyncCallback(int handle)
        {
            // Loop through sync procs
            for (int a = 0; a < syncProcs.Count; a++)
            {
                // Check handle
                if (syncProcs[a].Handle == handle)
                {
                    // Remove sync
                    mixerChannel.RemoveSync(fxChannel.Handle, syncProcs[a].Handle);
                    syncProcs[a].Handle = 0;
                    syncProcs[a].SyncProc = null;

                    // Remove from list and exit loop
                    syncProcs.RemoveAt(a);
                    break;
                }
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
			if(playlist == null || playlist.CurrentItem == null || playlist.Items[currentMixPlaylistIndex] == null ||
			   playlist.Items[currentMixPlaylistIndex].Channel == null)
            {
                // Return end-of-channel
                return (int)BASSStreamProc.BASS_STREAMPROC_END;
            }

            // Get active status            
            BASSActive status = playlist.Items[currentMixPlaylistIndex].Channel.IsActive();

            // Check the current channel status
            if (status == BASSActive.BASS_ACTIVE_PLAYING)
            {
                // Check if the next channel needs to be loaded
                if (playlist.CurrentItemIndex < playlist.Items.Count - 1)                    
                {
                    // Create the next channel using a timer
                    timerPlayer.Start();
                }

                // Get data from the current channel since it is running
                int data = playlist.Items[currentMixPlaylistIndex].Channel.GetData(buffer, length);
                return data;

//                byte[] bufferData = playlist.Items[currentMixPlaylistIndex].GetData(length);
//                Marshal.Copy(bufferData, 0, buffer, bufferData.Length);
//                return bufferData.Length;
            }
            else if (status == BASSActive.BASS_ACTIVE_STOPPED)
            {
                // Clear current loop
                currentLoop = null;

                Tracing.Log("StreamCallback -- BASS.Active.BASS_ACTIVE_STOPPED");

                // Check if this is the last item to play                
                if (playlist.CurrentItemIndex == playlist.Items.Count - 1)
                {
                    // This is the end of the playlist. Check the repeat type if the playlist needs to be repeated
                    if (RepeatType == RepeatType.Playlist)
                    {
                        // Set next playlist index
                        currentMixPlaylistIndex = 0;

                        // Dispose channels
                        //m_playlist.DisposeChannels();

                        // Load first item                        
                        Playlist.Items[0].Load();
                        //Playlist.Items[0].Decode(0);

                        // Load second item if it exists
                        if (Playlist.Items.Count > 1)
                        {
                            Playlist.Items[1].Load();
                        }

                        // Return data from the new channel                
                        return Playlist.CurrentItem.Channel.GetData(buffer, length);
                    }

                    // This is the end of the playlist                    
                    Tracing.Log("StreamCallback -- Playlist is over!");
                    return (int)BASSStreamProc.BASS_STREAMPROC_END;
                }
                else
                {
                    // Set next playlist index
                    Tracing.Log("StreamCallback -- Setting next playlist index...");
                    currentMixPlaylistIndex++;
                }

                // Lock main channel
                Tracing.Log("StreamCallback -- Locking channel...");
                mixerChannel.Lock(true);

                // Get main channel position
                Tracing.Log("StreamCallback -- Getting main channel position...");
                long position = mixerChannel.GetPosition(fxChannel.Handle);

                // Get remanining data in buffer
                Tracing.Log("StreamCallback -- Getting BASS_DATA_AVAILABLE...");
                int buffered = mixerChannel.GetData(IntPtr.Zero, (int)BASSData.BASS_DATA_AVAILABLE);                

                // Get length of the next audio file to play
                Tracing.Log("StreamCallback -- Getting current channel length (mix index)...");
                long audioLength = Playlist.Items[currentMixPlaylistIndex].Channel.GetLength();

                // Set sync                
                Tracing.Log("StreamCallback -- Setting new sync...");
                long syncPos = position + buffered + audioLength;
                SetSyncCallback(syncPos);

                // Unlock main channel
                Tracing.Log("StreamCallback -- Unlocking channel...");
                mixerChannel.Lock(false);

                // Return data from the new channel
                return Playlist.Items[currentMixPlaylistIndex].Channel.GetData(buffer, length);
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
            if (Bass.BASS_ChannelIsActive(mixerChannel.Handle) == BASSActive.BASS_ACTIVE_PLAYING)
            {
                // Return data
                return Bass.BASS_ChannelGetData(mixerChannel.Handle, buffer, length);
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
            // Validate nulls
            if (CurrentLoop == null || Playlist == null || Playlist.CurrentItem == null || Playlist.CurrentItem.Channel == null)
            {
                return;
            }

            // Get loop start position
            long bytes = CurrentLoop.StartPositionBytes;

            // Lock channel            
            mixerChannel.Lock(true);

            // Check if this is a FLAC file over 44100Hz
            if (Playlist.CurrentItem.AudioFile.FileType == AudioFileFormat.FLAC && Playlist.CurrentItem.AudioFile.SampleRate > 44100)
            {
                // Divide by 1.5 (I don't really know why, but this works for 48000Hz and 96000Hz. Maybe a bug in BASS with FLAC files?)
                bytes = (long)((float)bytes / 1.5f);
            }

            // Set position for the decode channel (needs to be in floating point)
            Playlist.CurrentItem.Channel.SetPosition(bytes * 2);

            // Set main channel position to 0 (clear buffer)            
            fxChannel.SetPosition(0);
            mixerChannel.SetPosition(0);

            // Set offset position (for calulating current position)
            positionOffset = bytes;

            // Unlock channel
            mixerChannel.Lock(false);
        }

        /// <summary>
        /// Sync callback routine used for triggering the playlist index changed event and
        /// changing the output stream sample rate if necessary.
        /// </summary>
        /// <param name="handle">Handle to the sync</param>
        /// <param name="channel">Channel handle</param>
        /// <param name="data">Data</param>
        /// <param name="user">User data</param>
        private void PlayerSyncProc(int handle, int channel, int data, IntPtr user)
        {
            // Declare variables
            bool playbackStopped = false;
            bool playlistBackToStart = false;
            int nextPlaylistIndex = 0;

            // Lock main channel. Do as less as possible when locking channels!
            mixerChannel.Lock(true);

            // Get main channel position
            long position = mixerChannel.GetPosition();

            // Get remanining data in buffer
            //int buffered = mainChannel.GetData(IntPtr.Zero, (int)BASSData.BASS_DATA_AVAILABLE);

            // Check if this the last song
            if (playlist.CurrentItemIndex == playlist.Items.Count - 1)
            {
                // This is the end of the playlist. Check the repeat type if the playlist needs to be repeated
                if (RepeatType == RepeatType.Playlist)
                {
                    // Set flags      
                    nextPlaylistIndex = 0;
                    playlistBackToStart = true;
                }
                else
                {
                    // Set flags
                    playbackStopped = true;
                }
            }
            else
            {
                // Set flags
                nextPlaylistIndex = playlist.CurrentItemIndex + 1;
            }

            // Calculate position offset
            long offset = 0 - (position / 2);

            // Check if the playback has stopped
            if (!playbackStopped)
            {
                // Check if this is a FLAC file over 44100Hz
                if (Playlist.CurrentItem.AudioFile.FileType == AudioFileFormat.FLAC && Playlist.Items[nextPlaylistIndex].AudioFile.SampleRate > 44100)
                {
                    // Multiply by 1.5 (I don't really know why, but this works for 48000Hz and 96000Hz. Maybe a bug in BASS with FLAC files?)
                    offset = (long)((float)offset * 1.5f);
                }

                // Check if the sample rate needs to be changed (i.e. main channel sample rate different than the decoding file)
                if (!playbackStopped && mixerChannel.SampleRate != Playlist.Items[nextPlaylistIndex].AudioFile.SampleRate)
                {
                    // Set new sample rate
                    mixerChannel.SetSampleRate(Playlist.Items[nextPlaylistIndex].AudioFile.SampleRate);
                }

                // Set position offset
                positionOffset = offset;
            }

            // Unlock main channel
            mixerChannel.Lock(false);

            // Remove own sync
            RemoveSyncCallback(handle);
            
            // Check if this is the last item to play
            if (playlist.CurrentItemIndex == playlist.Items.Count - 1)
            {
                // This is the end of the playlist. Check the repeat type if the playlist needs to be repeated
                if (RepeatType == RepeatType.Playlist)
                {
                    // Go to first item
                    Playlist.First();
                }
            }
            else
            {
                // Increment playlist index
                Playlist.Next();
            }

            // Check if an event is subscribed
            if (OnPlaylistIndexChanged != null)
            {
                // Create data
                PlayerPlaylistIndexChangedData eventData = new PlayerPlaylistIndexChangedData();
                eventData.IsPlaybackStopped = playbackStopped;

                // If the playback hasn't stopped, fill more event data
                if (playbackStopped)
                {
                    // Set event data
                    eventData.AudioFileStarted = null;
                    eventData.AudioFileEnded = Playlist.CurrentItem.AudioFile;

//                    // Check if EQ is enabled
//                    if (isEQEnabled)
//                    {
//                        // Remove EQ
//                        RemoveEQ();
//                    }

                    // Dispose channels
                    playlist.DisposeChannels();

                    // Set flag
                    isPlaying = false;
                }
                else
                {
                    // Set event data
                    eventData.AudioFileStarted = Playlist.CurrentItem.AudioFile;

                    // Is this the first item, and did the last song of the playlist just play?
                    if (Playlist.CurrentItemIndex == 0 && playlistBackToStart)
                    {
                        // The audio file that just finished was the last of the playlist
                        eventData.AudioFileEnded = Playlist.Items[Playlist.Items.Count - 1].AudioFile;
                    }
                    // Make sure this is not the first item
                    else if (Playlist.CurrentItemIndex > 0)
                    {
                        // The audio file that just finished was the last one
                        eventData.AudioFileEnded = Playlist.Items[Playlist.CurrentItemIndex - 1].AudioFile;
                    }
                }

                // Raise event
                OnPlaylistIndexChanged(eventData);
            }
        }

        #endregion
    }
}
