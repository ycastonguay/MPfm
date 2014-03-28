// Copyright © 2011-2013 Yanick Castonguay
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
using System.Reflection;
using System.Runtime.InteropServices;
using MPfm.Core;
using MPfm.Sound;
using MPfm.Sound.AudioFiles;
using MPfm.Sound.BassNetWrapper;
using MPfm.Sound.Playlists;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;
using MPfm.Player.Events;
using MPfm.Player.Exceptions;
using MPfm.Player.Objects;
using System.Diagnostics;

#if !IOS && !ANDROID
using Un4seen.BassAsio;
using Un4seen.BassWasapi;
using MPfm.Sound.BassNetWrapper.ASIO;
using MPfm.Sound.BassNetWrapper.WASAPI;
#endif

#if IOS
using MonoTouch;
#endif

namespace MPfm.Player
{
    /// <summary>
    /// The Player class manages audio playback through playlists and supports
    /// multiple driver types and devices.
    /// </summary>    
    public class Player : MPfm.Player.IPlayer
    {
        /// <summary>
        /// This static instance of the player is used only on iOS, because MonoTouch requires all C callbacks
        /// to be static.
        /// http://docs.xamarin.com/ios/guides/advanced_topics/limitations
        /// </summary>
        public static Player CurrentPlayer = null;

        /// <summary>
        /// Defines the BASS plugin path. Useful for Android where this cannot be determined by Environment.SpecialFolder.
        /// </summary>
        /// </value>
        public static string PluginDirectoryPath { get; set; }

        /// <summary>
        /// Returns true if the position is currently changing.
        /// </summary>
        public bool IsSettingPosition { get; private set; }
        
        private System.Timers.Timer _timerPlayer = null;
        private Channel _streamChannel = null;
        private Channel _fxChannel = null;
        private Channel _mixerChannel = null;

        // Plugin handles
        private int _fxEQHandle;
        private int _aacPluginHandle = 0;        
        private int _apePluginHandle = 0;
        private int _flacPluginHandle = 0;
        private int _mpcPluginHandle = 0;
        private int _ofrPluginHandle = 0;
        private int _ttaPluginHandle = 0;
        private int _wvPluginHandle = 0;
        private int _wmaPluginHandle = 0;

        /// <summary>
        /// Indicates the current playlist item index used by the mixer.
        /// The playlist index is incremented when the mixer starts to play the next song.
        /// Depending on the buffer size used, the index can be incremented a few seconds before actually hearing
        /// the song change.
        /// </summary>
        private int _currentMixPlaylistIndex = 0;

        /// <summary>
        /// Position from which to resume after pausing the stream. This allows changing the player position
        /// while the player is paused.
        /// </summary>
        private long _positionAfterUnpause;

        /// <summary>
        /// Offset position (necessary to calculate the offset in the output stream position
        /// if the user has seeked the position in the decode stream). The output stream position
        /// is reset to 0 in these cases to clear the audio buffer.
        /// </summary>
        private long _positionOffset = 0;

        private List<PlayerSyncProc> _syncProcs = null;
        private STREAMPROC _streamProc;

        private BPMPROC _bpmProc;
        //private BPMBEATPROC _bpmBeatProc;

#if IOS
        private IOSNOTIFYPROC _iosNotifyProc;
#endif

#if !IOS && !ANDROID
        private ASIOPROC asioProc;
        private WASAPIPROC wasapiProc;
#endif

        #region Events

        /// <summary>
        /// The OnPlaylistIndexChanged event is triggered when the playlist index changes (i.e. when an audio file
        /// starts to play).
        /// </summary>
        public event PlaylistIndexChanged OnPlaylistIndexChanged;

        /// <summary>
        /// The OnAudioInterrupted event is triggered when the audio is interrupted by a system event.
        /// Only useful on iOS for now. An example of use of this event would be to react when the user stops the
        /// playback using the lock screen, remote control, etc.
        /// </summary>
        public event AudioInterrupted OnAudioInterrupted;

        /// <summary>
        /// The OnBPMDetected event is triggered when the current BPM has been deteted or has changed.
        /// </summary>
        public event BPMDetected OnBPMDetected;

        #endregion

        #region Properties

        private bool _isPlaying = false;
        /// <summary>
        /// Indicates if the player is currently playing an audio file.
        /// </summary>
        public bool IsPlaying
        {
            get
            {
                return _isPlaying;
            }
        }

        private bool _isPaused = false;
        /// <summary>
        /// Indicates if the player is currently paused.
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return _isPaused;
            }
        }

        private bool _useFloatingPoint;
        /// <summary>
        /// Determines if the device uses floating point.
        /// </summary>
        public bool UseFloatingPoint
        {
            get
            {
                return _useFloatingPoint;
            }
        }

        private Device _device = null;
        /// <summary>
        /// Defines the currently used device for playback.
        /// </summary>
        public Device Device
        {
            get
            {
                return _device;
            }
        }

        private bool _isDeviceInitialized = false;
        /// <summary>
        /// Indicates if the device (as in the Device property) is initialized.
        /// </summary>
        public bool IsDeviceInitialized
        {
            get
            {
                return _isDeviceInitialized;
            }
        }       

        private RepeatType _repeatType = RepeatType.Off;
        /// <summary>
        /// Repeat type (Off, Playlist, Song)
        /// </summary>
        public RepeatType RepeatType
        {
            get
            {
                return _repeatType;
            }
            set
            {
                _repeatType = value;
                Console.WriteLine("Player - RepeatType: {0}", value.ToString());

                // Check if the current song exists
                if (_playlist != null && _playlist.CurrentItem != null)
                {
                    // If the repeat type is Song, force song looping
                    if (_repeatType == RepeatType.Song)
                        _playlist.CurrentItem.Channel.SetFlags(BASSFlag.BASS_SAMPLE_LOOP, BASSFlag.BASS_SAMPLE_LOOP);
                    else
                        _playlist.CurrentItem.Channel.SetFlags(BASSFlag.BASS_DEFAULT, BASSFlag.BASS_SAMPLE_LOOP);
                }
            }
        }

        private float _volume = 1.0f;
        /// <summary>
        /// Defines the master volume (from 0 to 1).
        /// </summary>
        public float Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                // Set value
                _volume = value;

                // Check if the player is playing
                if (_mixerChannel != null)
                    _mixerChannel.Volume = value;

#if !IOS && !ANDROID

                // Check driver type
                if (_device.DriverType == DriverType.ASIO)
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
                else if (_device.DriverType == DriverType.WASAPI)
                {
                    // Set WASAPI volume
                    bool success = BassWasapi.BASS_WASAPI_SetVolume(BASSWASAPIVolume.BASS_WASAPI_CURVE_LINEAR, value);
                    if (!success)
                    {
                        // Check for error
                        Base.CheckForError();
                    }
                }

#endif
            }
        }

        private float _timeShifting = 0.0f;
        /// <summary>
        /// Defines the time shifting applied to the currently playing stream.
        /// Value range from -100.0f (-100%) to 100.0f (+100%). To reset, set to 0.0f.
        /// </summary>
        public float TimeShifting
        {
            get
            {
                return _timeShifting;
            }
            set
            {
                _timeShifting = value;

                RemoveBPMCallbacks();
                if (_fxChannel != null)
                    _fxChannel.SetAttribute(BASSAttribute.BASS_ATTRIB_TEMPO, _timeShifting);
                AddBPMCallbacks();
            }           
        }

        private int _pitchShifting = 0;
        /// <summary>
        /// Defines the pitch shifting applied to the currently playing stream.
        /// Value range from -12 to +12 semitones. To reset, set to 0.
        /// </summary>
        public int PitchShifting
        {
            get
            {
                return _pitchShifting;
            }
            set
            {
                _pitchShifting = value;

                RemoveBPMCallbacks();
                if (_fxChannel != null)
                    _fxChannel.SetAttribute(BASSAttribute.BASS_ATTRIB_TEMPO_PITCH, _pitchShifting);
                AddBPMCallbacks();
            }           
        }

        private int _mixerSampleRate = 44100;
        /// <summary>
        /// Defines the sample rate of the mixer.
        /// </summary>
        public int MixerSampleRate
        {
            get
            {
                return _mixerSampleRate;
            }
        }

        private int _bufferSize = 1000;
        /// <summary>
        /// Defines the buffer size (in milliseconds). Increase this value if older computers have trouble
        /// filling up the buffer in time.        
        /// Default value: 1000ms. The default BASS value is 500ms.
        /// </summary>
        public int BufferSize
        {
            get
            {
                return _bufferSize;
            }
            set
            {
                _bufferSize = value;
                Base.SetConfig(BASSConfig.BASS_CONFIG_BUFFER, _bufferSize);
            }
        }

        private int _updatePeriod = 10;
        /// <summary>
        /// Defines how often BASS fills the buffer to make sure it is always full (in milliseconds).
        /// This affects the accuracy of the ChannelGetPosition value.
        /// Default value: 10ms. The default BASS value is 100ms.
        /// </summary>
        public int UpdatePeriod
        {
            get
            {
                return _updatePeriod;
            }
            set
            {
                _updatePeriod = value;
                Base.SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, _updatePeriod);
            }
        }

        private int _updateThreads = 1;
        /// <summary>
        /// Defines how many threads BASS can use to update playback buffers in parrallel.
        /// Note: The playback engine plays perfectly with just one update thread.
        /// Default value: 1 thread. The default BASS value is 1 thread.
        /// </summary>
        public int UpdateThreads
        {
            get
            {
                return _updateThreads;
            }
            set
            {
                _updateThreads = value;
                Base.SetConfig(BASSConfig.BASS_CONFIG_UPDATETHREADS, _updateThreads);
            }
        }

        private Playlist _playlist = null;
        /// <summary>
        /// Playlist used for playback. Contains the audio file metadata and decode channels for
        /// playback.
        /// </summary>
        public Playlist Playlist
        {
            get
            {
                return _playlist;
            }
        }

        private EQPreset _currentEQPreset = null;
        /// <summary>
        /// Defines the current EQ preset.
        /// </summary>
        public EQPreset EQPreset
        {
            get
            {
                return _currentEQPreset;
            }
            set
            {
                _currentEQPreset = value;
            }
        }

        private bool _isEQEnabled = false;
        /// <summary>
        /// Indicates if the EQ is enabled.
        /// </summary>
        public bool IsEQEnabled
        {
            get
            {
                return _isEQEnabled;
            }
        }

        private bool _isEQBypassed = false;
        /// <summary>
        /// Indicates if the EQ is bypassed.
        /// </summary>
        public bool IsEQBypassed
        {
            get
            {
                return _isEQBypassed;
            }
        }

        private Loop _currentLoop = null;
        /// <summary>
        /// Defines the currently playing loop.
        /// </summary>
        public Loop Loop
        {
            get
            {
                return _currentLoop;
            }
        }

        #endregion

        #region Constructors, Initialization/Dispose

        /// <summary>
        /// Default constructor for the Player class. Initializes the player using the default
        /// values (see property comments).
        /// </summary>
        public Player()
        {
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
            Player.CurrentPlayer = this;
            _device = device;
            _mixerSampleRate = mixerSampleRate;
            _bufferSize = bufferSize;
            _updatePeriod = updatePeriod;
            _playlist = new Playlist();
            _syncProcs = new List<PlayerSyncProc>();

#if !ANDROID
            _useFloatingPoint = true;
#endif

            _timerPlayer = new System.Timers.Timer();
            _timerPlayer.Elapsed += new System.Timers.ElapsedEventHandler(timerPlayer_Elapsed);
            _timerPlayer.Interval = 1000;
            _timerPlayer.Enabled = false;

            // Register BASS.NET
            Base.Register(BassNetKey.Email, BassNetKey.RegistrationKey);

            // Initialize BASS library by OS type
            if (OS.Type == OSType.Windows)
            {
                // Load decoding plugins
                //plugins = Base.LoadPluginDirectory(Path.GetDirectoryName((new Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath));
                Tracing.Log("Player init -- Loading plugins...");
                //aacPluginHandle = Base.LoadPlugin("bass_aac.dll");
                _apePluginHandle = Base.LoadPlugin("bass_ape.dll");
                _flacPluginHandle = Base.LoadPlugin("bassflac.dll");
                _mpcPluginHandle = Base.LoadPlugin("bass_mpc.dll");
                //ofrPluginHandle = Base.LoadPlugin("bass_ofr.dll"); // Requires OptimFrog.DLL
                //ttaPluginHandle = Base.LoadPlugin("bass_tta.dll");
                _wmaPluginHandle = Base.LoadPlugin("basswma.dll");
                _wvPluginHandle = Base.LoadPlugin("basswv.dll");

                int bassFxVersion = Base.GetFxPluginVersion();            
            	//Base.LoadFxPlugin();
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
                Console.WriteLine("Player init -- OS is " + OS.Type.ToString());
	            if (OS.Type == OSType.Linux)
	            {
                    string pluginPath = string.Empty;

#if ANDROID
                    pluginPath = PluginDirectoryPath;
                    _aacPluginHandle = Base.LoadPlugin(Path.Combine(pluginPath, "libbass_aac.so"));
                    //_alacPluginHandle = Base.LoadPlugin(Path.Combine(pluginPath, "libbass_alac.so"));
                    _apePluginHandle = Base.LoadPlugin(Path.Combine(pluginPath, "libbass_ape.so"));
                    _flacPluginHandle = Base.LoadPlugin(Path.Combine(pluginPath, "libbassflac.so"));
                    _wvPluginHandle = Base.LoadPlugin(Path.Combine(pluginPath, "libbasswv.so"));
#else

                    // Find plugins either in current directory (i.e. development) or in a system directory (ex: /usr/lib/mpfm or /opt/lib/mpfm)
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
                    _flacPluginHandle = Base.LoadPlugin(Path.Combine(pluginPath, "libbassflac.so"));
                    _wvPluginHandle = Base.LoadPlugin(Path.Combine(pluginPath, "libbasswv.so"));
                    _mpcPluginHandle = Base.LoadPlugin(Path.Combine(pluginPath, "libbass_mpc.so"));
#endif					
	            }
	            else if (OS.Type == OSType.MacOSX)
	            {
#if IOS
                    // Load decoding plugins (http://www.un4seen.com/forum/?topic=13851.msg96559#msg96559)
					Console.WriteLine("Player init -- Loading iOS plugins (FLAC)...");
                    _flacPluginHandle = Base.LoadPlugin("BASSFLAC");
					Console.WriteLine("Player init -- Loading iOS plugins (WV)...");
                    _wvPluginHandle = Base.LoadPlugin("BASSWV");
					Console.WriteLine("Player init -- Loading iOS plugins (APE)...");
                    _apePluginHandle = Base.LoadPlugin("BASS_APE");
					Console.WriteLine("Player init -- Loading iOS plugins (MPC)...");
                    _mpcPluginHandle = Base.LoadPlugin("BASS_MPC");

					Console.WriteLine("Player init -- Configuring IOSNOTIFY delegate...");
                    _iosNotifyProc = new IOSNOTIFYPROC(IOSNotifyProc);
                    IntPtr ptr = Marshal.GetFunctionPointerForDelegate(_iosNotifyProc);
                    Bass.BASS_SetConfigPtr((BASSConfig)46, ptr);
                    //Bass.BASS_SetConfigPtr(BASSIOSConfig.BASS_CONFIG_IOS_NOTIFY, ptr);

					Console.WriteLine("Player init -- Configuring AirPlay and remote control...");
                    Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_IOS_MIXAUDIO, 0); // 0 = AirPlay
#else

                    // Try to get the plugins in the current path
                    Console.WriteLine("Loading OS X plugins...");
                    string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    string pluginPath = exePath.Replace("MonoBundle", "Resources");

                    // Check in the current directory first
                    if(!File.Exists(pluginPath + "/libbassflac.dylib"))
                    {
                        // The plugins could not be found!
                        throw new Exception("The BASS plugins could not be found in the current directory!");
                    }

				    // Load decoding plugins
					_flacPluginHandle = Base.LoadPlugin(pluginPath + "/libbassflac.dylib");
                    _wvPluginHandle = Base.LoadPlugin(pluginPath + "/libbasswv.dylib");
                    _mpcPluginHandle = Base.LoadPlugin(pluginPath + "/libbass_mpc.dylib");
#endif
	            }
			}

#if IOS
            _bpmProc = new BPMPROC(BPMDetectionProcIOS);
            //bpmBeatProc = new BPMBEATPROC(BPMDetectionBeatProcIOS);
#else
            _bpmProc = new BPMPROC(BPMDetectionProc);
            //bpmBeatProc = new BPMBEATPROC(BPMDetectionBeatProc);
#endif
					
            // Create default EQ
            Tracing.Log("Player init -- Creating default EQ preset...");
            _currentEQPreset = new EQPreset();

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
            InitializeDevice(new Device(), _mixerSampleRate);
        }

        /// <summary>
        /// Initializes a specific audio device for playback.
        /// </summary>
        /// <param name="device">Audio device</param>
        /// <param name="mixerSampleRate">Mixer sample rate (in Hz)</param>
        public void InitializeDevice(Device device, int mixerSampleRate)
        {
            _device = device;
            _mixerSampleRate = mixerSampleRate;

            Tracing.Log("Player -- Initializing device (SampleRate: " + mixerSampleRate.ToString() + " Hz, DriverType: " + device.DriverType.ToString() + ", Id: " + device.Id.ToString() + ", Name: " + device.Name + ", BufferSize: " + _bufferSize.ToString() + ", UpdatePeriod: " + _updatePeriod.ToString() + ")");

            // Check driver type
            if (device.DriverType == DriverType.DirectSound)
            {
                // Initialize sound system                
                Base.Init(device.Id, mixerSampleRate, BASSInit.BASS_DEVICE_DEFAULT | BASSInit.BASS_DEVICE_LATENCY);
            }
#if !IOS && !ANDROID
            else if (device.DriverType == DriverType.ASIO)
            {
                // Initialize sound system
                BaseASIO.Init(device.Id, mixerSampleRate, BASSInit.BASS_DEVICE_DEFAULT, BASSASIOInit.BASS_ASIO_THREAD);
            }
            else if (device.DriverType == DriverType.WASAPI)
            {
                // Create callback
                wasapiProc = new WASAPIPROC(WASAPICallback);

                // Initialize sound system                
                //Base.InitWASAPI(device.Id, mixerSampleRate, 2, BASSInit.BASS_DEVICE_DEFAULT, BASSWASAPIInit.BASS_WASAPI_SHARED, 10.0f, 0, wasapiProc);
                BaseWASAPI.Init(device.Id, mixerSampleRate, 2, BASSInit.BASS_DEVICE_DEFAULT, BASSWASAPIInit.BASS_WASAPI_SHARED, 0.5f, 0, wasapiProc);
                //Base.InitWASAPI(device.Id, mixerSampleRate, 2, BASSInit.BASS_DEVICE_DEFAULT, BASSWASAPIInit.BASS_WASAPI_EXCLUSIVE, 1, 0, wasapiProc);

                //BASS_WASAPI_INFO info = BassWasapi.BASS_WASAPI_GetInfo();
            }
#endif

            // Default BASS.NET configuration values for Windows *AND* Linux:
            //
            // BASS_CONFIG_BUFFER: 500
            // BASS_CONFIG_UPDATEPERIOD: 100
            // BASS_CONFIG_UPDATETHREADS: 1
			
			if(OS.Type == OSType.Windows)
			{
	            // Set configuration for buffer and update period
            	// This only works for the default BASS output (http://www.un4seen.com/forum/?topic=13429.msg93740#msg93740)
            	Base.SetConfig(BASSConfig.BASS_CONFIG_BUFFER, _bufferSize); 
            	Base.SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, _updatePeriod);	
			}
            else if (OS.Type == OSType.Linux)
            {				
				// Default
				// 10ms update period does not work under Linux. Major stuttering
                Base.SetConfig(BASSConfig.BASS_CONFIG_BUFFER, _bufferSize);
                Base.SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, 100);

#if ANDROID
                Base.SetConfig(BASSConfig.BASS_CONFIG_DEV_BUFFER, 500); // Default on Android: 30ms
#endif                
            }
            else if (OS.Type == OSType.MacOSX)
            {
				// Default
                Base.SetConfig(BASSConfig.BASS_CONFIG_BUFFER, 100);// _bufferSize);
                Base.SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, 10);					
            }		

            _isDeviceInitialized = true;
        }

        /// <summary>
        /// Frees the device currently used for playback.
        /// </summary>
        public void FreeDevice()
        {
            if (!_isDeviceInitialized)
                return;

#if !IOS && !ANDROID

            // Check driver type
            if (_device.DriverType == DriverType.ASIO)
            {
                // Free ASIO device
                if (!BassAsio.BASS_ASIO_Free())
                {
                    // Get error
                    BASSError error = Bass.BASS_ErrorGetCode();
                    throw new Exception("Error freeing ASIO device: " + error.ToString());
                }
            }
            else if (_device.DriverType == DriverType.WASAPI)
            {
                // Free WASAPI device
                if (!BassWasapi.BASS_WASAPI_Free())
                {
                    // Get error
                    BASSError error = Bass.BASS_ErrorGetCode();
                    throw new Exception("Error freeing WASAPI device: " + error.ToString());
                }
            }
#endif

            Base.Free();
            _mixerChannel = null;
            _device = null;
            _isDeviceInitialized = false;
        }

        /// <summary>
        /// Frees the BASS plugins used by the player.
        /// </summary>
        public void FreePlugins()
        {
            // Dispose plugins
            //Base.FreeFxPlugin();
			
			// Free plugins if they have been loaded successfully
			if(_aacPluginHandle > 0)			
            	Base.FreePlugin(_aacPluginHandle);
			if(_apePluginHandle > 0)			
            	Base.FreePlugin(_apePluginHandle);
			if(_flacPluginHandle > 0)			
            	Base.FreePlugin(_flacPluginHandle);
			if(_mpcPluginHandle > 0)
            	Base.FreePlugin(_mpcPluginHandle);
			if(_ofrPluginHandle > 0)
            	Base.FreePlugin(_ofrPluginHandle);
			if(_ttaPluginHandle > 0)
            	Base.FreePlugin(_ttaPluginHandle);
			if(_wvPluginHandle > 0)
            	Base.FreePlugin(_wvPluginHandle);
			if(_wmaPluginHandle > 0)
            	Base.FreePlugin(_wmaPluginHandle);
			
        }        

        /// <summary>
        /// Disposes the current device and the audio plugins.
        /// </summary>
        public void Dispose()
        {
            FreeDevice();
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
            if (_timerPlayer != null)
            {
                _timerPlayer.Enabled = false;

                // Check if the next channel needs to be loaded
                if (_playlist.CurrentItemIndex < _playlist.Items.Count - 1)
                {
                    // Check if the channel has already been loaded
                    if (!_playlist.Items[_playlist.CurrentItemIndex + 1].IsLoaded)
                        _playlist.Items[_playlist.CurrentItemIndex + 1].Load(_useFloatingPoint);
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
            Play(0, false);
        }

        /// <summary>
        /// Plays the playlist from the current item index.
        /// </summary>
        public void Play(double initialPosition, bool startPaused)
        {
            try
            {
                if (_isPlaying)
                {
                    if (_currentLoop != null)
                    {
						//Tracing.Log("Player.Play -- Stopping current loop...");
                        StopLoop();
                    }

					//Tracing.Log("Player.Play -- Stopping playback...");
                    Stop();
                }

                RemoveSyncCallbacks();
                _currentLoop = null;
                _positionOffset = 0;
                _currentMixPlaylistIndex = Playlist.CurrentItemIndex;

                // How many channels are left?                
                int channelsToLoad = Playlist.Items.Count - Playlist.CurrentItemIndex;                

                // If there are more than 2, just limit to 2 for now. The other channels are loaded dynamically.
                if (channelsToLoad > 2)
                    channelsToLoad = 2;

                // Check for channels to load
                if (channelsToLoad == 0)
                    throw new Exception("Error in Player.Play: There aren't any channels to play!");

                // Load the current channel and the next channel if it exists
                for (int a = Playlist.CurrentItemIndex; a < Playlist.CurrentItemIndex + channelsToLoad; a++)
                    _playlist.Items[a].Load(_useFloatingPoint);

                // Start decoding first playlist item
                //_playlist.Items[Playlist.CurrentItemIndex].Decode(0);

                try
                {
                    // Create the streaming channel (set the frequency to the first file in the list)
					//Tracing.Log("Player.Play -- Creating streaming channel (SampleRate: " + _playlist.CurrentItem.AudioFile.SampleRate + " Hz, FloatingPoint: true)...");

#if IOS
                    _streamProc = new STREAMPROC(StreamCallbackIOS);
#else
                    _streamProc = new STREAMPROC(StreamCallback);
#endif

                    _streamChannel = Channel.CreateStream(_playlist.CurrentItem.AudioFile.SampleRate, 2, _useFloatingPoint, _streamProc);
					//Tracing.Log("Player.Play -- Creating time shifting channel...");
                    _fxChannel = Channel.CreateStreamForTimeShifting(_streamChannel.Handle, true, _useFloatingPoint);
                    //_fxChannel = Channel.CreateStreamForTimeShifting(_streamChannel.Handle, false, _useFloatingPoint);
                    //_fxChannel = _streamChannel;
                }
                catch(Exception ex)
                {
                    // Raise custom exception with information (so the client application can maybe deactivate floating point for example)
                    PlayerCreateStreamException newEx = new PlayerCreateStreamException("The player has failed to create the stream channel (" + ex.Message + ").", ex);
                    newEx.Decode = true;
                    newEx.UseFloatingPoint = true;
                    newEx.SampleRate = _playlist.CurrentItem.AudioFile.SampleRate;
                    throw newEx;
                }

                // Check driver type
                if (_device.DriverType == DriverType.DirectSound)
                {
                    try
                    {
                        // Create mixer stream
						Tracing.Log("Player.Play -- Creating mixer channel (DirectSound)...");
                        _mixerChannel = MixerChannel.CreateMixerStream(_playlist.CurrentItem.AudioFile.SampleRate, 2, _useFloatingPoint, false);
                        _mixerChannel.AddChannel(_fxChannel.Handle);
                        //_mixerChannel = _fxChannel;
                        AddBPMCallbacks();
                    }
                    catch (Exception ex)
                    {   
                        // Raise custom exception with information (so the client application can maybe deactivate floating point for example)
                        PlayerCreateStreamException newEx = new PlayerCreateStreamException("The player has failed to create the time shifting channel.", ex);
                        newEx.UseFloatingPoint = true;
                        newEx.UseTimeShifting = true;
                        newEx.SampleRate = _playlist.CurrentItem.AudioFile.SampleRate;
                        throw newEx;
                    }
                }
#if !IOS && !ANDROID
                else if (_device.DriverType == DriverType.ASIO)
                {
                    try
                    {
                        // Create mixer stream
                        Tracing.Log("Player.Play -- Creating mixer channel (ASIO)...");
                        _mixerChannel = MixerChannel.CreateMixerStream(_playlist.CurrentItem.AudioFile.SampleRate, 2, _useFloatingPoint, true);
                        _mixerChannel.AddChannel(_fxChannel.Handle);
                    }
                    catch (Exception ex)
                    {
                        // Raise custom exception with information (so the client application can maybe deactivate floating point for example)
                        PlayerCreateStreamException newEx = new PlayerCreateStreamException("The player has failed to create the time shifting channel.", ex);
                        newEx.DriverType = DriverType.ASIO;
                        newEx.UseFloatingPoint = true;
                        newEx.UseTimeShifting = true;
                        newEx.Decode = true;
                        newEx.SampleRate = _playlist.CurrentItem.AudioFile.SampleRate;
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
                        BassAsio.BASS_ASIO_ChannelEnable(false, 0, asioProc, new IntPtr(_mixerChannel.Handle));
                        Tracing.Log("Player.Play -- Joining ASIO channels...");
                        BassAsio.BASS_ASIO_ChannelJoin(false, 1, 0);

                        // Set sample rate
                        Tracing.Log("Player.Play -- Set ASIO sample rates...");
                        BassAsio.BASS_ASIO_ChannelSetRate(false, 0, _playlist.CurrentItem.AudioFile.SampleRate);
                        BassAsio.BASS_ASIO_SetRate(_playlist.CurrentItem.AudioFile.SampleRate);
                    }
                    catch (Exception ex)
                    {
                        // Raise custom exception with information (so the client application can maybe deactivate floating point for example)
                        PlayerCreateStreamException newEx = new PlayerCreateStreamException("The player has failed to enable or join ASIO channels.", ex);
                        newEx.DriverType = DriverType.ASIO;
                        newEx.SampleRate = _playlist.CurrentItem.AudioFile.SampleRate;
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
                        newEx.SampleRate = _playlist.CurrentItem.AudioFile.SampleRate;
                        throw newEx;                        
                    }
                }
                else if (_device.DriverType == DriverType.WASAPI)
                {
                    // Create mixer stream
                    Tracing.Log("Player.Play -- Creating mixer channel (WASAPI)...");
                    _mixerChannel = MixerChannel.CreateMixerStream(_playlist.CurrentItem.AudioFile.SampleRate, 2, true, true);
                    _mixerChannel.AddChannel(_fxChannel.Handle);

                    // Start playback
                    if (!BassWasapi.BASS_WASAPI_Start())
                    {
                        // Get error
                        BASSError error = Bass.BASS_ErrorGetCode();
                        throw new Exception("Player.Play error: Error playing files in WASAPI: " + error.ToString());
                    }
                }
#endif
                // Set initial volume
                _mixerChannel.Volume = Volume;

                // Load 18-band equalizer
				//Tracing.Log("Player.Play -- Creating equalizer (Preset: " + _currentEQPreset + ")...");
                AddEQ(_currentEQPreset);

                // Check if EQ is bypassed
                if (_isEQBypassed)
                {
                    // Reset EQ
					//Tracing.Log("Player.Play -- Equalizer is bypassed; resetting EQ...");
                    ResetEQ();
                }

                // Check if the song must be looped
                if (_repeatType == RepeatType.Song)
                    _playlist.CurrentItem.Channel.SetFlags(BASSFlag.BASS_SAMPLE_LOOP, BASSFlag.BASS_SAMPLE_LOOP);

                long length = _playlist.CurrentItem.Channel.GetLength();
                SetSyncCallback(length);
                _isPlaying = true;

                // Only the DirectSound mode needs to start the main channel since it's not in decode mode.
                if (_device.DriverType == DriverType.DirectSound)
                {
                    // For iOS: This is required to update the AirPlay/remote player status
                    Base.Start();

                    // Start playback
					//Tracing.Log("Player.Play -- Starting DirectSound playback...");
                    _mixerChannel.Play(false);

                    if (startPaused)
                    {
                        if(initialPosition > 0)
                            SetPosition(initialPosition);

                        Base.Pause();
                    }

                    _isPaused = startPaused;
                }

                // Raise audio file finished event (if an event is subscribed)
                if (OnPlaylistIndexChanged != null)
                {
                    PlayerPlaylistIndexChangedData data = new PlayerPlaylistIndexChangedData();
                    data.IsPlaybackStopped = false;
                    data.AudioFileStarted = _playlist.CurrentItem.AudioFile;
                    data.PlaylistName = "New playlist 1";
                    data.PlaylistCount = _playlist.Items.Count;
                    data.PlaylistIndex = _playlist.CurrentItemIndex; 
                    if (Playlist.CurrentItemIndex < Playlist.Items.Count - 2)
                        data.NextAudioFile = Playlist.Items[Playlist.CurrentItemIndex + 1].AudioFile;
                    OnPlaylistIndexChanged(data);
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
                AudioFile audioFile = new AudioFile(filePath, Guid.NewGuid(), false);
                audioFiles.Add(audioFile);
            }

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
            if (audioFiles == null || audioFiles.Count == 0)
                throw new Exception("There must be at least one file in the audioFiles parameter.");

            // Check if all file paths exist
			//Tracing.Log("Player.PlayFiles -- Playing a list of " + audioFiles.Count.ToString() + " files.");
            foreach (AudioFile audioFile in audioFiles)
                if (!File.Exists(audioFile.FilePath))
                    throw new Exception("The file at " + audioFile.FilePath + " doesn't exist!");

            // Stop playback
            if (_isPlaying)
                Stop();

            // Add audio files to playlist
            _playlist.Clear();
            foreach (AudioFile audioFile in audioFiles)
                _playlist.AddItem(audioFile);

            // Start playback from first item
            _playlist.First();
            Play();
        }

        /// <summary>
        /// Pauses the audio playback. Resumes the audio playback if it was paused.
        /// </summary>
        public void Pause()
        {
            // Check driver type (the pause mechanism differs by driver)
            if (_device.DriverType == DriverType.DirectSound)
            {        
                if (!_isPaused)
                    Base.Pause();
                else
                    Base.Start();
            }

#if !IOS && !ANDROID
            else if (_device.DriverType == DriverType.ASIO)
            {
                // Check if the playback is already paused
                if (!_isPaused)
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
            else if (_device.DriverType == DriverType.WASAPI)
            {
                // Check if the playback is already paused
                if (!_isPaused)
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
#endif

            _isPaused = !_isPaused;
        }

        /// <summary>
        /// Stops the audio playback and frees the resources used by the playback engine.
        /// </summary>
        public void Stop()
        {
            // Check if the main channel exists, and make sure the player is playing
            if (_mixerChannel == null)// || !m_isPlaying)
                throw new Exception("Player.Stop error: The main channel is null!");

            _currentLoop = null;
            _isPlaying = false;
            if (_isEQEnabled)
            {
                // Remove EQ
                Tracing.Log("Player.Stop -- Removing equalizer...");
                RemoveEQ();
            }

            // Stop mixer channel
            _mixerChannel.Stop();

            // Check driver type
            if (_device.DriverType == DriverType.DirectSound)
            {
                // Stop main channel
                //Tracing.Log("Player.Stop -- Stopping DirectSound channel...");                
                //m_mainChannel.Stop();

                // For iOS: This is required to update the AirPlay/remote player status
                Base.Stop();
            }
#if !IOS && !ANDROID
            else if (_device.DriverType == DriverType.ASIO)
            {
                // Stop playback
                Tracing.Log("Player.Stop -- Stopping ASIO playback...");
                BassAsio.BASS_ASIO_Stop();
            }
            else if (_device.DriverType == DriverType.WASAPI)
            {
                // Stop playback
                Tracing.Log("Player.Stop -- Stopping WASAPI playback...");
                BassWasapi.BASS_WASAPI_Stop(false);
            }
#endif

            // Stop decoding the current file (doesn't throw an exception if decode has finished)
            //Playlist.CurrentItem.CancelDecode();

            RemoveSyncCallbacks();
            RemoveBPMCallbacks();
            _fxChannel.Free();

            // Dispose channels
            if (_playlist != null && _playlist.CurrentItem != null)
            {
                Tracing.Log("Player.Stop -- Disposing channels...");
                _playlist.DisposeChannels();
            }

#if !IOS && !ANDROID
            if (_device.DriverType == DriverType.WASAPI)
            {
                BassWasapi.BASS_WASAPI_Stop(true);
            }
#endif
           
        }

        /// <summary>
        /// Stops the playback and starts playback at a specific playlist song.
        /// </summary>
        /// <param name="index">Playlist item index</param>
        public void GoTo(int index)
        {
            Stop();
            Playlist.GoTo(index);
            _currentMixPlaylistIndex = index;
            Play();
        }

        /// <summary>
        /// Stops the playback and starts playback at a specific playlist song.
        /// </summary>
        /// <param name="playlistItemId">Playlist item identifier</param>
        public void GoTo(Guid playlistItemId)
        {
            Stop();
            Playlist.GoTo(playlistItemId);
            _currentMixPlaylistIndex = Playlist.CurrentItemIndex;
            Play();
        }

        /// <summary>
        /// Goes back to the previous channel in the list.
        /// </summary>
        public void Previous()
        {
            if (Playlist.CurrentItemIndex > 0)
                GoTo(Playlist.CurrentItemIndex - 1);
        }

        /// <summary>
        /// Skips to the next channel in the list.
        /// </summary>
        public void Next()
        {
            if (Playlist.CurrentItemIndex < Playlist.Items.Count - 1)
                GoTo(Playlist.CurrentItemIndex + 1);
        }       

        /// <summary>
        /// Returns the number of bytes in the custom stream buffer.
        /// </summary>
        /// <returns>Available data in buffer (in bytes)</returns>
        public int GetDataAvailable()
        {
            return _mixerChannel.GetDataAvailable();
        }

        /// <summary>
        /// Converts a time value from seconds to bytes, using the current channel properties (bits per sample, sample rate, etc.)
        /// </summary>
        /// <returns>Value (in bytes)</returns>
        /// <param name="value">Value (in seconds)</param>
        public long Seconds2Bytes(double value)
        {
            return _mixerChannel.Seconds2Bytes(value);
        }

        /// <summary>
        /// Returns the sample data from the mixer (32-bit integers for non-floating point channels).
        /// </summary>
        /// <returns>Sample data length</returns>
        /// <param name="length">Length of sample data to fetch</param>
        /// <param name="sampleData">Sample data</param>
        public int GetMixerData(int length, int[] sampleData)
        {
            // NOTE: Do *NOT* try to use the GetData/GetMixerData with short/Int16 array because it crashes under Xamarin.Android.
            int dataLength;
            if (Device.DriverType != DriverType.DirectSound)
                dataLength = _fxChannel.GetMixerData(sampleData, length);
            else
                dataLength = _mixerChannel.GetData(sampleData, length);
            return dataLength;
        }

        /// <summary>
        /// Returns the sample data from the mixer (floats for floating point channels).
        /// </summary>
        /// <returns>Sample data length</returns>
        /// <param name="length">Length of sample data to fetch</param>
        /// <param name="sampleData">Sample data</param>
        public int GetMixerData(int length, float[] sampleData)
        {
            int dataLength;
            if (Device.DriverType != DriverType.DirectSound)
                dataLength = _fxChannel.GetMixerData(sampleData, length);
            else
                dataLength = _mixerChannel.GetData(sampleData, length);
            return dataLength;
        }

        /// <summary>
        /// Gets the position of the currently playing channel, in bytes.
        /// </summary>
        /// <returns>Position (in bytes)</returns>
        public long GetPosition()
        {
            if (Playlist == null || Playlist.CurrentItem == null || Playlist.CurrentItem.Channel == null)
                return 0;

            if(IsSettingPosition)
                return 0;

            long outputPosition = 0;
            if (_mixerChannel is MixerChannel)
            {
                var mixerChannel = _mixerChannel as MixerChannel;
                outputPosition = mixerChannel.GetPosition(_fxChannel.Handle);
            } 
            else
            {
                outputPosition = _mixerChannel.GetPosition();
            }
            //long outputPosition = Playlist.CurrentItem.Channel.GetPosition();

            if(_useFloatingPoint)
                outputPosition /= 2;

            if (Playlist.CurrentItem.AudioFile.FileType == AudioFileFormat.FLAC && Playlist.CurrentItem.AudioFile.SampleRate > 44100)
            {
                // Multiply by 1.5 (I don't really know why, but this works for 48000Hz and 96000Hz. Maybe a bug in BASS with FLAC files?)
                outputPosition = (long)((float)outputPosition * 1.5f);                
            }

            // Add the offset position
            outputPosition += _positionOffset;

            return outputPosition;            
        }

        /// <summary>
        /// Sets the position of the currently playing channel.
        /// </summary>
        /// <param name="percentage">Position (in percentage)</param>
        public void SetPosition(double percentage)
        {
            if (Playlist == null || Playlist.CurrentItem == null || Playlist.CurrentItem.Channel == null)
                throw new Exception("Error: The playlist has no current item!");

            // Calculate position in bytes
            //long positionBytes = (long)Math.Ceiling((double)Playlist.CurrentItem.LengthBytes * (percentage / 100));
            //long positionBytes = (long)(positionPercentage * lengthBytes);
            long positionBytes = (long)(Playlist.CurrentItem.LengthBytes * (percentage / 100f));
            SetPosition(positionBytes);
        }

        /// <summary>
        /// Sets the position of the currently playing channel.
        /// If the value is in floating point, divide the value by 2 before using it with the "bytes" parameter.
        /// </summary>
        /// <param name="bytes">Position (in bytes)</param>
        public void SetPosition(long bytes)
        {
            if (Playlist == null || Playlist.CurrentItem == null || Playlist.CurrentItem.Channel == null)
                return;

#if !IOS && !ANDROID
            // Check if WASAPI
            if (_device.DriverType == DriverType.WASAPI)
            {
                BassWasapi.BASS_WASAPI_Stop(true);
                BassWasapi.BASS_WASAPI_Start();
            }
#endif

            if(IsPaused)
            {
                _positionAfterUnpause = bytes;
                //return;
            }

            // Get as much data available before locking channel
            RemoveSyncCallbacks();
            long length = Playlist.CurrentItem.Channel.GetLength();
            _mixerChannel.Lock(true);
            IsSettingPosition = true;
            _positionOffset = bytes;

            // Divide by 1.5 (I don't really know why, but this works for 48000Hz and 96000Hz. Maybe a bug in BASS with FLAC files?)            
            if (Playlist.CurrentItem.AudioFile.FileType == AudioFileFormat.FLAC && Playlist.CurrentItem.AudioFile.SampleRate > 44100)
                bytes = (long)((float)bytes / 1.5f);

            _mixerChannel.Stop();

            Playlist.CurrentItem.Channel.SetPosition(0); // Flush buffer
            _fxChannel.SetPosition(0);
            _mixerChannel.SetPosition(0);

            long bytesPosition = bytes;
            if (_useFloatingPoint)
                bytesPosition *= 2;

            Playlist.CurrentItem.Channel.SetPosition(bytesPosition);
            SetSyncCallback(length - bytesPosition);

            if(!IsPaused)
                _mixerChannel.Play(false);

            _mixerChannel.Lock(false);
            IsSettingPosition = false;
        }

        /// <summary>
        /// Go to a marker position.
        /// </summary>
        /// <param name="marker">Marker position</param>
        public void GoToMarker(Marker marker)
        {
            SetPosition(marker.PositionBytes);
        }

        /// <summary>
        /// Starts a loop. The playback must be activated.
        /// </summary>
        /// <param name="loop">Loop to apply</param>
        public void StartLoop(Loop loop)
        {
            if (Playlist == null || Playlist.CurrentItem == null || Playlist.CurrentItem.Channel == null)
                return;

            long startPositionBytes = loop.StartPositionBytes;
            long endPositionBytes = loop.EndPositionBytes;

            if (Playlist.CurrentItem.AudioFile.FileType == AudioFileFormat.FLAC && Playlist.CurrentItem.AudioFile.SampleRate > 44100)
            {
                // Divide by 1.5 (I don't really know why, but this works for 48000Hz and 96000Hz. Maybe a bug in BASS with FLAC files?)
                startPositionBytes = (long)((float)startPositionBytes / 1.5f);
                endPositionBytes = (long)((float)endPositionBytes / 1.5f);
            }

#if !IOS && !ANDROID
            if (_device.DriverType == DriverType.WASAPI)
            {
                BassWasapi.BASS_WASAPI_Stop(true);
                BassWasapi.BASS_WASAPI_Start();
            }
#endif

            // Remove any sync callback
            RemoveSyncCallbacks();

            // Get file length
            long length = Playlist.CurrentItem.Channel.GetLength();
            _mixerChannel.Lock(true);

            // Set position for the decode channel (needs to be in floating point)
            Playlist.CurrentItem.Channel.SetPosition(startPositionBytes * 2);
            _fxChannel.SetPosition(0); // Clear buffer
            _mixerChannel.SetPosition(0);

            // Set sync
#if IOS
            Playlist.CurrentItem.SyncProc = new SYNCPROC(LoopSyncProcIOS);
#else
            Playlist.CurrentItem.SyncProc = new SYNCPROC(LoopSyncProc);
#endif

            if (_mixerChannel is MixerChannel)
            {
                var mixerChannel = _mixerChannel as MixerChannel;
                Playlist.CurrentItem.SyncProcHandle = mixerChannel.SetSync(_fxChannel.Handle, BASSSync.BASS_SYNC_POS | BASSSync.BASS_SYNC_MIXTIME, (endPositionBytes - startPositionBytes) * 2, Playlist.CurrentItem.SyncProc);
            } 
            else
            {
                Playlist.CurrentItem.SyncProcHandle = _mixerChannel.SetSync(BASSSync.BASS_SYNC_POS | BASSSync.BASS_SYNC_MIXTIME, (endPositionBytes - startPositionBytes) * 2, Playlist.CurrentItem.SyncProc);
            }

            // Set new callback (length already in floating point)
            SetSyncCallback((length - (startPositionBytes * 2))); // + buffered));

            // Set offset position (for calulating current position)
            _positionOffset = startPositionBytes;
            _mixerChannel.Lock(false);
            _currentLoop = loop;
        }

        /// <summary>
        /// Stops any loop currently playing.
        /// </summary>
        public void StopLoop()
        {
            try
            {
                if (_currentLoop == null)
                    return;

                Tracing.Log("Player.StopLoop -- Removing sync...");
                if (_mixerChannel is MixerChannel)
                {
                    var mixerChannel = _mixerChannel as MixerChannel;
                    mixerChannel.RemoveSync(_fxChannel.Handle, Playlist.CurrentItem.SyncProcHandle);
                } 
                else
                {
                    _mixerChannel.RemoveSync(Playlist.CurrentItem.SyncProcHandle);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                _currentLoop = null;
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
            if (_mixerChannel == null)
                throw new Exception("Error adding EQ: The mixer channel doesn't exist!");

            if (_fxEQHandle != 0 || _isEQEnabled)
                throw new Exception("Error adding EQ: The equalizer already exists!");

            _fxEQHandle = _mixerChannel.SetFX(BASSFXType.BASS_FX_BFX_PEAKEQ, 0);
            ApplyEQPreset(preset);
            _isEQEnabled = true;
        }

        /// <summary>
        /// Applies a preset on the 18-band equalizer. 
        /// The equalizer needs to be created using the AddEQ method.
        /// </summary>
        public void ApplyEQPreset(EQPreset preset)
        {
            Console.WriteLine("Player - ApplyEQPreset name: {0}", preset.Name);
            BASS_BFX_PEAKEQ eq = new BASS_BFX_PEAKEQ();
            _currentEQPreset = preset;
            if (_isEQBypassed)
                return;

            if(!_isPlaying)
                return;

            Console.WriteLine("Player - ApplyEQPreset - Removing BPM callbacks...");
            RemoveBPMCallbacks();

            Console.WriteLine("Player - ApplyEQPreset - Looping through bands");
            for (int a = 0; a < _currentEQPreset.Bands.Count; a++)
            {
                Console.WriteLine("Player - ApplyEQPreset name: {0} - Applying band {1}", preset.Name, a);
                EQPresetBand currentBand = _currentEQPreset.Bands[a];
                eq.lBand = a;
                eq.lChannel = BASSFXChan.BASS_BFX_CHANALL;
                eq.fCenter = currentBand.Center;
                eq.fGain = currentBand.Gain;
                eq.fQ = currentBand.Q;

                bool success = BassWrapper.BASS_FXSetParametersPeakEQ(_fxEQHandle, eq);
                if(!success)
                    Base.CheckForError();
            }

            Console.WriteLine("Player - ApplyEQPreset - Readding BPM callbacks...");
            AddBPMCallbacks();
        }

        /// <summary>
        /// Removes the 18-band equalizer.
        /// </summary>
        protected void RemoveEQ()
        {
            if (_mixerChannel == null)
                return;

            if (!_isEQEnabled)
                return;

            Tracing.Log("Player.RemoveEQ -- Removing EQ...");
            _mixerChannel.RemoveFX(_fxEQHandle);
            _fxEQHandle = 0;
            _isEQEnabled = false;
        }

        /// <summary>
        /// Gets the parameters of an EQ band.
        /// </summary>
        /// <param name="band">Band index</param>
        /// <returns>EQ parameters</returns>
        private BASS_BFX_PEAKEQ GetEQParams(int band)
        {
            BASS_BFX_PEAKEQ eq = new BASS_BFX_PEAKEQ {lBand = band};
            BassWrapper.BASS_FXGetParametersPeakEQ(_fxEQHandle, eq);
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
            if (setCurrentEQPresetValue)
                _currentEQPreset.Bands[band].Gain = gain;

            if(!_isPlaying)
                return;

            RemoveBPMCallbacks();
            
            BASS_BFX_PEAKEQ eq = GetEQParams(band);
            eq.fGain = gain;
            bool success = BassWrapper.BASS_FXSetParametersPeakEQ(_fxEQHandle, eq);
            if(!success)
                Base.CheckForError();

            AddBPMCallbacks();
        }

        /// <summary>
        /// Bypasses the 18-band equalizer.        
        /// </summary>
        public void BypassEQ()
        {
            // The problem is that we're recreating the main channel every time the playback of a playlist
            // starts, and this changes the handle to the EQ effect. This means that bypassing the EQ
            // when there is no playback does nothing.

            if (_mixerChannel == null)
                return;

            _isEQBypassed = !_isEQBypassed;
            if (_isEQBypassed)
                ResetEQ();
            else
                ApplyEQPreset(_currentEQPreset);
        }



        /// <summary>
        /// Resets the gain of every EQ band.
        /// </summary>
        public void ResetEQ()
        {
            if (_mixerChannel == null)
                return;

            for (int a = 0; a < _currentEQPreset.Bands.Count; a++)
            {
                UpdateEQBand(a, 0.0f, false);
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
            PlayerSyncProc syncProc = new PlayerSyncProc();

#if IOS
            syncProc.SyncProc = new SYNCPROC(PlayerSyncProcIOS);
#else
            syncProc.SyncProc = new SYNCPROC(PlayerSyncProc);
#endif


            if (_mixerChannel is MixerChannel)
            {
                var mixerChannel = _mixerChannel as MixerChannel;
                syncProc.Handle = mixerChannel.SetSync(_fxChannel.Handle, BASSSync.BASS_SYNC_POS, position, syncProc.SyncProc);
            } 
            else
            {
                syncProc.Handle = _mixerChannel.SetSync(BASSSync.BASS_SYNC_POS, position, syncProc.SyncProc);
            }

            _syncProcs.Add(syncProc);

            return syncProc;
        }

        /// <summary>
        /// Removes all synchronization callbacks from the sync callback list.
        /// </summary>
        protected void RemoveSyncCallbacks()
        {
            while (true)
            {
                if (_syncProcs.Count > 0)
                    RemoveSyncCallback(_syncProcs[0].Handle);                    
                else
                    break;
            }
        }

        /// <summary>
        /// Removes a synchronization callback from the sync callback list.
        /// </summary>
        /// <param name="handle">Synchronization callback handle</param>
        protected void RemoveSyncCallback(int handle)
        {
            for (int a = 0; a < _syncProcs.Count; a++)
            {
                if (_syncProcs[a].Handle == handle)
                {
                    if (_mixerChannel is MixerChannel)
                    {
                        var mixerChannel = _mixerChannel as MixerChannel;
                        mixerChannel.RemoveSync(_fxChannel.Handle, _syncProcs[a].Handle);
                    } 
                    else
                    {
                        _mixerChannel.RemoveSync(_syncProcs[a].Handle);
                    }
                    _syncProcs[a].Handle = 0;
                    _syncProcs[a].SyncProc = null;
                    _syncProcs.RemoveAt(a);
                    break;
                }
            }
        }

        #endregion

        #region BPM Detection Methods 

        protected void AddBPMCallbacks()
        {
            //Console.WriteLine("Player - AddBPMCallbacks - Adding callback...");

            //BaseFx.BPM_CallbackSet(_fxChannel.Handle, _bpmProc, 2.0, Utils.MakeLong(70, 180), BASSFXBpm.BASS_FX_BPM_MULT2, IntPtr.Zero);
            //BaseFx.BPM_BeatCallbackSet(_fxChannel.Handle, _bpmBeatProc, IntPtr.Zero);
        }

        protected void RemoveBPMCallbacks()
        {
            //Console.WriteLine("Player - RemoveBPMCallbacks - Resetting callback...");
            //BaseFx.BPM_CallbackReset(_fxChannel.Handle); // <-- this crashes when trying to remove a callback that's being used.
//            Console.WriteLine("Player - RemoveBPMCallbacks - Freeing resources...");
//            BaseFx.BPM_Free(_fxChannel.Handle);
            //BaseFx.BPM_BeatCallbackReset(_fxChannel.Handle);
            //BaseFx.BPM_BeatFree(_fxChannel.Handle);
        }

        #endregion

        #region Callback Events

        DateTime _lastDateTime = DateTime.Now;

        /// <summary>
        /// Callback used for standard devices (including DirectSound).
        /// </summary>
        /// <param name="handle">Channel handle</param>
        /// <param name="buffer">Buffer data</param>
        /// <param name="length">Buffer length</param>
        /// <param name="user">User data</param>
        /// <returns>Audio data</returns>
        internal int StreamCallback(int handle, IntPtr buffer, int length, IntPtr user)
        {
            //var stopwatch = new Stopwatch();
            //stopwatch.Start();

            // If the current sub channel is null, end the stream            
			if(_playlist == null || _playlist.CurrentItem == null || _playlist.Items.Count < _currentMixPlaylistIndex || _playlist.Items[_currentMixPlaylistIndex] == null ||
			   _playlist.Items[_currentMixPlaylistIndex].Channel == null)
                return (int)BASSStreamProc.BASS_STREAMPROC_END;

            BASSActive status = _playlist.Items[_currentMixPlaylistIndex].Channel.IsActive();
            if (status == BASSActive.BASS_ACTIVE_PLAYING)
            {
                // Check if the next channel needs to be loaded
                if (_playlist.CurrentItemIndex < _playlist.Items.Count - 1)                    
                    _timerPlayer.Start();

                // Get data from the current channel since it is running
                int data = _playlist.Items[_currentMixPlaylistIndex].Channel.GetData(buffer, length);
                //return data;

                //byte[] bufferData = _playlist.Items[_currentMixPlaylistIndex].GetData(length);
                //Marshal.Copy(bufferData, 0, buffer, bufferData.Length);
                //return bufferData.Length;

                //stopwatch.Stop();
                //if(stopwatch.ElapsedMilliseconds > 0)
                //var info = Bass.BASS_GetInfo();
                //float cpu = Bass.BASS_GetCPU();
                //var timeSpan = DateTime.Now - _lastDateTime;
                //_lastDateTime = DateTime.Now;
                //Console.WriteLine("Player - StreamCallback - Returning wave data - elapsed: {0} ({1} ms) - latency: {2} minbuf: {3} cpu: {4} data: {5} length: {6}", stopwatch.Elapsed, stopwatch.ElapsedMilliseconds, info.latency, info.minbuf, cpu, data, length);
                //Console.WriteLine("Player - StreamCallback - Returning wave data - elapsed: {0} ({1} ms) - latency: {2} minbuf: {3} cpu: {4} length: {5} elapsed since last call: {6}.{7}", stopwatch.Elapsed, stopwatch.ElapsedMilliseconds, info.latency, info.minbuf, cpu, length, DateTime.Now.Second, DateTime.Now.Millisecond);
                return data;
                //return bufferData.Length;
            }
            else if (status == BASSActive.BASS_ACTIVE_STOPPED)
            {
                //Tracing.Log("StreamCallback -- BASS.Active.BASS_ACTIVE_STOPPED");
                _currentLoop = null;
                if (_playlist.CurrentItemIndex == _playlist.Items.Count - 1)
                {
                    // This is the end of the playlist. Check the repeat type if the playlist needs to be repeated
                    if (RepeatType == RepeatType.Playlist)
                    {
                        // Set next playlist index
                        _currentMixPlaylistIndex = 0;

                        // Dispose channels
                        //m_playlist.DisposeChannels();

                        // Load first item                        
                        Playlist.Items[0].Load(_useFloatingPoint);
                        //Playlist.Items[0].Decode(0);

                        // Load second item if it exists
                        if (Playlist.Items.Count > 1)
                            Playlist.Items[1].Load(_useFloatingPoint);

                        // Return data from the new channel                
                        return Playlist.CurrentItem.Channel.GetData(buffer, length);
                    }

					//Tracing.Log("StreamCallback -- Playlist is over!");
                    return (int)BASSStreamProc.BASS_STREAMPROC_END;
                }
                else
                {
					//Tracing.Log("StreamCallback -- Setting next playlist index...");
                    _currentMixPlaylistIndex++;
                }

				//Tracing.Log("StreamCallback -- Locking channel...");
                _mixerChannel.Lock(true);

				//Tracing.Log("StreamCallback -- Getting main channel position...");
                long position = 0;
                if (_mixerChannel is MixerChannel)
                {
                    var mixerChannel = _mixerChannel as MixerChannel;
                    position = mixerChannel.GetPosition(_fxChannel.Handle);
                } 
                else
                {
                    position = _mixerChannel.GetPosition();
                }
//                if (_useFloatingPoint)
//                    position /= 2;

                // Get remanining data in buffer
				//Tracing.Log("StreamCallback -- Getting BASS_DATA_AVAILABLE...");
                int buffered = _mixerChannel.GetData(IntPtr.Zero, (int)BASSData.BASS_DATA_AVAILABLE);                

				//Tracing.Log("StreamCallback -- Getting current channel length (mix index)...");
                long audioLength = Playlist.Items[_currentMixPlaylistIndex].Channel.GetLength();

				//Tracing.Log("StreamCallback -- Setting new sync...");
                long syncPos = position + buffered + audioLength;
                SetSyncCallback(syncPos);

				//Tracing.Log("StreamCallback -- Unlocking channel...");
                _mixerChannel.Lock(false);

                // Return data from the new channel
                var data = Playlist.Items[_currentMixPlaylistIndex].Channel.GetData(buffer, length);

                //stopwatch.Stop();
                //Console.WriteLine("Player - StreamCallback - Returning wave data - elapsed: {0} ({1} ms)", stopwatch.Elapsed, stopwatch.ElapsedMilliseconds);
                return data;
            }

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
            if (Bass.BASS_ChannelIsActive(_mixerChannel.Handle) == BASSActive.BASS_ACTIVE_PLAYING)
                return Bass.BASS_ChannelGetData(_mixerChannel.Handle, buffer, length);
            else
                return (int)BASSStreamProc.BASS_STREAMPROC_END;
        }

        /// <summary>
        /// Sync callback routine used for looping the current channel.
        /// </summary>
        /// <param name="handle">Handle to the sync</param>
        /// <param name="channel">Channel handle</param>
        /// <param name="data">Data</param>
        /// <param name="user">User data</param>
        internal void LoopSyncProc(int handle, int channel, int data, IntPtr user)
        {
            if (Loop == null || Playlist == null || Playlist.CurrentItem == null || Playlist.CurrentItem.Channel == null)
                return;

            // Get loop start position
            long bytes = Loop.StartPositionBytes;
            _mixerChannel.Lock(true);

            // Check if this is a FLAC file over 44100Hz
            if (Playlist.CurrentItem.AudioFile.FileType == AudioFileFormat.FLAC && Playlist.CurrentItem.AudioFile.SampleRate > 44100)
            {
                // Divide by 1.5 (I don't really know why, but this works for 48000Hz and 96000Hz. Maybe a bug in BASS with FLAC files?)
                bytes = (long)((float)bytes / 1.5f);
            }

            // Set position for the decode channel (needs to be in floating point)
            Playlist.CurrentItem.Channel.SetPosition(bytes * 2);
            _fxChannel.SetPosition(0); // Clear buffer
            _mixerChannel.SetPosition(0);

            // Set offset position (for calulating current position)
            _positionOffset = bytes;
            _mixerChannel.Lock(false);
        }

        /// <summary>
        /// Sync callback routine used for triggering the playlist index changed event and
        /// changing the output stream sample rate if necessary.
        /// </summary>
        /// <param name="handle">Handle to the sync</param>
        /// <param name="channel">Channel handle</param>
        /// <param name="data">Data</param>
        /// <param name="user">User data</param>
        internal void PlayerSyncProc(int handle, int channel, int data, IntPtr user)
        {
            bool playbackStopped = false;
            bool playlistBackToStart = false;
            int nextPlaylistIndex = 0;

            _mixerChannel.Lock(true);
            long position = _mixerChannel.GetPosition();
//            if (_useFloatingPoint)
//                position /= 2;

            // Get remanining data in buffer
            //int buffered = mainChannel.GetData(IntPtr.Zero, (int)BASSData.BASS_DATA_AVAILABLE);

            // Check if this the last song
            if (_playlist.CurrentItemIndex == _playlist.Items.Count - 1)
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
                nextPlaylistIndex = _playlist.CurrentItemIndex + 1;
            }

            // Calculate position offset
            long offset = 0 - (position / 2);
            if (!playbackStopped)
            {
                // Multiply by 1.5 (I don't really know why, but this works for 48000Hz and 96000Hz. Maybe a bug in BASS with FLAC files?)
                if (Playlist.CurrentItem.AudioFile.FileType == AudioFileFormat.FLAC && Playlist.Items[nextPlaylistIndex].AudioFile.SampleRate > 44100)
                    offset = (long)((float)offset * 1.5f);

                // Check if the sample rate needs to be changed (i.e. main channel sample rate different than the decoding file)
                if (!playbackStopped && _mixerChannel.SampleRate != Playlist.Items[nextPlaylistIndex].AudioFile.SampleRate)
                    _mixerChannel.SetSampleRate(Playlist.Items[nextPlaylistIndex].AudioFile.SampleRate);

                // Set position offset
                _positionOffset = offset;
            }

            _mixerChannel.Lock(false);
            RemoveSyncCallback(handle);
            
            // Check if this is the last item to play
            if (_playlist.CurrentItemIndex == _playlist.Items.Count - 1)
            {
                // This is the end of the playlist. Check the repeat type if the playlist needs to be repeated
                if (RepeatType == RepeatType.Playlist)
                    Playlist.First();
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
                eventData.PlaylistName = "New playlist 1";
                eventData.PlaylistCount = _playlist.Items.Count;
                eventData.PlaylistIndex = _playlist.CurrentItemIndex;

                // If the playback hasn't stopped, fill more event data
                if (playbackStopped)
                {
                    // Set event data
                    eventData.AudioFileStarted = null;
                    eventData.AudioFileEnded = Playlist.CurrentItem.AudioFile;

                    // Check if EQ is enabled
                    if (_isEQEnabled)
                        RemoveEQ();

                    // Dispose channels
                    _playlist.DisposeChannels();
                    _isPlaying = false;
                }
                else
                {
                    // Set event data
                    eventData.AudioFileStarted = Playlist.CurrentItem.AudioFile;
                    if (Playlist.CurrentItemIndex < Playlist.Items.Count - 2)
                        eventData.NextAudioFile = Playlist.Items[Playlist.CurrentItemIndex + 1].AudioFile;

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
                       
        internal void BPMDetectionProc(int handle, float bpm, IntPtr user)
        {
            //Console.WriteLine("Player BPM: " + bpm.ToString());
            if(Player.CurrentPlayer.OnBPMDetected != null)
                Player.CurrentPlayer.OnBPMDetected(bpm);
        }

        internal void BPMDetectionBeatProc(int handle, double beatpos, IntPtr user)
        {
            //Console.WriteLine("Player Beat: " + beatpos.ToString());
        }

#if IOS

        [MonoPInvokeCallback(typeof(STREAMPROC))]
        private static int StreamCallbackIOS(int handle, IntPtr buffer, int length, IntPtr user)
        {
            return Player.CurrentPlayer.StreamCallback(handle, buffer, length, user);
        }

        [MonoPInvokeCallback(typeof(SYNCPROC))]
        private static void PlayerSyncProcIOS(int handle, int channel, int data, IntPtr user)
        {
            Player.CurrentPlayer.PlayerSyncProc(handle, channel, data, user);
        }

        [MonoPInvokeCallback(typeof(SYNCPROC))]
        private static void LoopSyncProcIOS(int handle, int channel, int data, IntPtr user)
        {
            Player.CurrentPlayer.LoopSyncProc(handle, channel, data, user);
        }

        [MonoPInvokeCallback(typeof(BPMPROC))]
        private static void BPMDetectionProcIOS(int handle, float bpm, IntPtr user)
        {
            Player.CurrentPlayer.BPMDetectionProc(handle, bpm, user);
        }

        [MonoPInvokeCallback(typeof(BPMBEATPROC))]
        private static void BPMDetectionBeatProcIOS(int handle, double beatpos, IntPtr user)
        {
            Player.CurrentPlayer.BPMDetectionBeatProc(handle, beatpos, user);
        }

        [MonoPInvokeCallback(typeof(IOSNOTIFYPROC))]
        private static void IOSNotifyProc(BASSIOSNotify status)
        {
            switch(status)
            {
                case BASSIOSNotify.BASS_IOSNOTIFY_INTERRUPT:
                    Console.WriteLine("BASS_IOSNOTIFY_INTERRUPT");
                    Player.CurrentPlayer.Pause();
                    
                    // Invoke delegate to notify service/presenter
                    if(Player.CurrentPlayer.OnAudioInterrupted != null)
                        Player.CurrentPlayer.OnAudioInterrupted(new AudioInterruptedData());
                    break;
                case BASSIOSNotify.BASS_IOSNOTIFY_INTERRUPT_END:
                    Console.WriteLine("BASS_IOSNOTIFY_INTERRUPT_END");
                    break;
            }
        }

#endif
        #endregion
    }
}
