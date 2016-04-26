// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Reflection;
using Sessions.Core;
using org.sessionsapp.player;
using System.Collections.Generic;

#if IOS
using MonoTouch;
using ObjCRuntime;
#endif

namespace Sessions.Sound.Player
{
    public class SSPPlayer : ISSPPlayer 
    {
        /// <summary>
        /// This static instance of the player is used only on iOS, because MonoTouch requires all C callbacks
        /// to be static.
        /// http://docs.xamarin.com/ios/guides/advanced_topics/limitations
        /// </summary>
        public static SSPPlayer CurrentPlayer = null;

        public Playlist Playlist { get; private set; }

        private LogDelegate _logDelegate;
        private PlaylistIndexChangedDelegate _playlistIndexChangedDelegate;
        private PlaylistEndedDelegate _playlistEndedDelegate;
        private StateChangedDelegate _stateDelegate;
        private AudioInterruptedDelegate _audioInterruptedDelegate;
        private LoopPlaybackStartedDelegate _loopPlaybackStartedDelegate;
        private LoopPlaybackStoppedDelegate _loopPlaybackStoppedDelegate;
        private BPMDetectedDelegate _bpmDetectedDelegate;

        public event LogDelegate Log;
        public event PlaylistIndexChangedDelegate PlaylistIndexChanged;
        public event PlaylistEndedDelegate PlaylistEnded;
        public event StateChangedDelegate StateChanged;
        public event AudioInterruptedDelegate AudioInterrupted;
        public event LoopPlaybackStartedDelegate LoopPlaybackStarted;
        public event LoopPlaybackStoppedDelegate LoopPlaybackStopped;
        public event BPMDetectedDelegate BPMDetected;

        public int Version
        {
            get { return SSP.SSP_GetVersion(); }
        }

        public SSPPlayerState State
        {
            get { return SSP.SSP_GetState(); }
        }

        public SSPDevice Device
        {
            get
            {
                var device = new SSPDevice();
                SSP.SSP_GetDevice(ref device.Struct);
                return device;
            }
        }

        public SSPMixer Mixer
        {
            get
            {
                var mixer = new SSPMixer();
                SSP.SSP_GetMixer(ref mixer.Struct);
                return mixer;
            }
        }

        public SSPEQPreset EQPreset
        {
            get
            {
                var preset = new SSPEQPreset();
                SSP.SSP_GetEQPreset(ref preset.Struct);
                return preset;
            }
        }

        public SSPLoop Loop
        {
            get
            {
                var loop = new SSPLoop();
                SSP.SSP_GetLoop(ref loop.Struct);
                return loop;
            }
        }

        public bool EQEnabled
        {
            get { return SSP.SSP_GetEQEnabled(); }
            set { SSP.SSP_SetEQEnabled(value); }
        }

        public bool IsShuffle
        {
            get { return SSP.SSP_GetIsShuffle(); }
            set { SSP.SSP_SetIsShuffle(value); }
        }

        public SSPRepeatType RepeatType
        {
            get { return SSP.SSP_GetRepeatType(); }
            set { SSP.SSP_SetRepeatType(value); }
        }

        public float Volume
        {
            get
            {
                return SSP.SSP_GetVolume();
            }
            set
            {
                SSP.SSP_SetVolume(value);
            }
        }

        public int PitchShifting
        {
            get { return SSP.SSP_GetPitchShifting(); }
            set { SSP.SSP_SetPitchShifting(value); }
        }

        public float TimeShifting
        {
            get { return SSP.SSP_GetTimeShifting(); }
            set { SSP.SSP_SetTimeShifting(value); }
        }

        public bool IsSettingPosition
        {
            get { return SSP.SSP_GetIsSettingPosition(); }
        }

        public bool IsPlayingLoop
        {
            get
            {
                return SSP.SSP_GetIsPlayingLoop();
            }
        }

        public SSPPlayer()
        {
            Playlist = new Playlist();
            SSPPlayer.CurrentPlayer = this;
        }

        private void CheckForError(int error)
        {
            if (error != SSP.SSP_OK)
            {
                throw new SSPException(error, string.Format("An error occured in libssp_player (error {0})", error));
            }
        }

        #if IOS

        [MonoPInvokeCallback(typeof(LogDelegate))]
        private static void HandleLogIOS(IntPtr user, string str)
        {
            SSPPlayer.CurrentPlayer.HandleLog(user, str);
        }

        [MonoPInvokeCallback(typeof(StateChangedDelegate))]
        private static void HandleStateChangedIOS(IntPtr user, SSPPlayerState state)
        {
            SSPPlayer.CurrentPlayer.HandleStateChanged(user, state);
        }

        [MonoPInvokeCallback(typeof(PlaylistIndexChangedDelegate))]
        private static void HandlePlaylistIndexChangedIOS(IntPtr user)
        {
            SSPPlayer.CurrentPlayer.HandlePlaylistIndexChanged(user);
        }

        [MonoPInvokeCallback(typeof(PlaylistEndedDelegate))]
        private static void HandlePlaylistEndedIOS(IntPtr user)
        {
            SSPPlayer.CurrentPlayer.HandlePlaylistEnded(user);
        }

        [MonoPInvokeCallback(typeof(AudioInterruptedDelegate))]
        private static void HandleAudioInterruptedIOS(IntPtr user, bool ended)
        {
            SSPPlayer.CurrentPlayer.HandleAudioInterrupted(user, ended);
        }

        [MonoPInvokeCallback(typeof(LoopPlaybackStartedDelegate))]
        private static void HandleLoopPlaybackStartedIOS(IntPtr user)
        {
            SSPPlayer.CurrentPlayer.HandleLoopPlaybackStarted(user);
        }

        [MonoPInvokeCallback(typeof(LoopPlaybackStoppedDelegate))]
        private static void HandleLoopPlaybackStoppedIOS(IntPtr user)
        {
            SSPPlayer.CurrentPlayer.HandleLoopPlaybackStopped(user);
        }

        [MonoPInvokeCallback(typeof(BPMDetectedDelegate))]
        private static void HandleBPMDetectedIOS(IntPtr user, float bpm)
        {
            SSPPlayer.CurrentPlayer.HandleBPMDetected(user, bpm);
        }

#endif

        public void HandleLog(IntPtr user, string str)
        {
            if (Log != null)
                Log(user, str);
        }

        public void HandleStateChanged(IntPtr user, SSPPlayerState state)
        {
            if (StateChanged != null)
                StateChanged(user, state);
        }

        public void HandlePlaylistIndexChanged(IntPtr user)
        {
            if (PlaylistIndexChanged != null)
                PlaylistIndexChanged(user);
        }

        public void HandlePlaylistEnded(IntPtr user)
        {
            if (PlaylistEnded != null)
                PlaylistEnded(user);
        }

        public void HandleAudioInterrupted(IntPtr user, bool ended)
        {
            //Pause(); // TODO: not sure if this is even necessary
            if (AudioInterrupted != null)
                AudioInterrupted(user, ended);
        }

        public void HandleLoopPlaybackStarted(IntPtr user)
        {
            if (LoopPlaybackStarted != null)
                LoopPlaybackStarted(user);
        }

        public void HandleLoopPlaybackStopped(IntPtr user)
        {
            if (LoopPlaybackStopped != null)
                LoopPlaybackStopped(user);
        }

        public void HandleBPMDetected(IntPtr user, float bpm)
        {
            if (BPMDetected != null)
                BPMDetected(user, bpm);
        }

        private string GetPluginPath()
        {
            string pluginPath = null;

#if OSX || MACOSX
            // Try to get the plugins in the current path
            string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            pluginPath = exePath.Replace("MonoBundle", "Resources");
            if(!File.Exists(pluginPath + "/libbassflac.dylib"))
            {
                throw new Exception("The BASS plugins could not be found in the current directory!");
            }
#elif IOS   
            // Keep value to null, the static libraries are merged with the executable
#endif

            return pluginPath;
        }

        public void Init()
        {
            // TODO: Manage plugin paths per platform here     
            string pluginPath = GetPluginPath();
            CheckForError(SSP.SSP_Init(pluginPath));

            // Note: We must keep a reference to these delegates to make sure they are not garbage collected while they are used in a unmanaged context
            #if IOS
            _logDelegate = new LogDelegate(HandleLogIOS);
            _stateDelegate = new StateChangedDelegate(HandleStateChangedIOS);
            _playlistIndexChangedDelegate = new PlaylistIndexChangedDelegate(HandlePlaylistIndexChangedIOS);
            _playlistEndedDelegate = new PlaylistEndedDelegate(HandlePlaylistEndedIOS);
            _audioInterruptedDelegate = new AudioInterruptedDelegate(HandleAudioInterruptedIOS);
            _loopPlaybackStartedDelegate = new LoopPlaybackStartedDelegate(HandleLoopPlaybackStartedIOS);
            _loopPlaybackStoppedDelegate = new LoopPlaybackStoppedDelegate(HandleLoopPlaybackStoppedIOS);
            _bpmDetectedDelegate = new BPMDetectedDelegate(HandleBPMDetectedIOS);
            #else
            _logDelegate = new LogDelegate(HandleLog);
            _stateDelegate = new StateChangedDelegate(HandleStateChanged);
            _playlistIndexChangedDelegate = new PlaylistIndexChangedDelegate(HandlePlaylistIndexChanged);
            _playlistEndedDelegate = new PlaylistEndedDelegate(HandlePlaylistEnded);
            _audioInterruptedDelegate = new AudioInterruptedDelegate(HandleAudioInterrupted);
            _loopPlaybackStartedDelegate = new LoopPlaybackStartedDelegate(HandleLoopPlaybackStarted);
            _loopPlaybackStoppedDelegate = new LoopPlaybackStoppedDelegate(HandleLoopPlaybackStopped);
            _bpmDetectedDelegate = new BPMDetectedDelegate(HandleBPMDetected);
            #endif

            SSP.SSP_SetLogCallback(_logDelegate, IntPtr.Zero);
            SSP.SSP_SetStateChangedCallback(_stateDelegate, IntPtr.Zero);
            SSP.SSP_SetPlaylistIndexChangedCallback(_playlistIndexChangedDelegate, IntPtr.Zero);
            SSP.SSP_SetPlaylistEndedCallback(_playlistEndedDelegate, IntPtr.Zero);
            SSP.SSP_SetAudioInterruptedCallback(_audioInterruptedDelegate, IntPtr.Zero);
            SSP.SSP_SetLoopPlaybackStartedCallback(_loopPlaybackStartedDelegate, IntPtr.Zero);
            SSP.SSP_SetLoopPlaybackStoppedCallback(_loopPlaybackStoppedDelegate, IntPtr.Zero);
            SSP.SSP_SetBPMDetectedCallback(_bpmDetectedDelegate, IntPtr.Zero);
        }
        
        public void InitDevice(int deviceId, int sampleRate, int bufferSize, int updatePeriod, bool useFloatingPoint) 
        {
            CheckForError(SSP.SSP_InitDevice(deviceId, sampleRate, bufferSize, updatePeriod, useFloatingPoint));

            // Create a dummy EQ preset so we can interact with it right away and make sure the app won't crash if we access an EQ preset
            CheckForError(SSP.SSP_SetEQEnabled(true));
            var emptyPreset = new SSPEQPreset();
            CheckForError(SSP.SSP_SetEQPreset(ref emptyPreset.Struct));

            #if IOS
            CheckForError(SSP.SSP_IOS_ConfigureAirPlay(true));
            CheckForError(SSP.SSP_IOS_ConfigureAudioInterruptionNotification(true));
            #endif
        }

        public void FreeDevice()
        {
            #if IOS
            CheckForError(SSP.SSP_IOS_ConfigureAirPlay(false));
            CheckForError(SSP.SSP_IOS_ConfigureAudioInterruptionNotification(false));
            #endif

            CheckForError(SSP.SSP_FreeDevice());
        }

        public void Free()
        {
            CheckForError(SSP.SSP_Free());
        }

        public void Dispose()
        {
            //          if (Sessions.Player.Player.CurrentPlayer.IsPlaying)
//              Sessions.Player.Player.CurrentPlayer.Stop();

            _logDelegate = null;
            _stateDelegate = null;
            _playlistIndexChangedDelegate = null;
        }

        public float GetCPU()
        {
            return SSP.SSP_GetCPU();
        }

        public UInt32 GetBufferDataAvailable()
        {
            return SSP.SSP_GetBufferDataAvailable();
        }

        public void SetBufferSize(int bufferSize)
        {
            CheckForError(SSP.SSP_SetBufferSize(bufferSize));
        }

        public void SetUpdatePeriod(int updatePeriod)
        {
            CheckForError(SSP.SSP_SetUpdatePeriod(updatePeriod));
        }

        public void SetEQPreset(SSPEQPreset preset)
        {
            CheckForError(SSP.SSP_SetEQPreset(ref preset.Struct));
        }

        public void SetEQPresetBand(int band, float gain)
        {
            CheckForError(SSP.SSP_SetEQPresetBand(band, gain));
        }

        public void ResetEQ()
        {
            CheckForError(SSP.SSP_ResetEQ());
        }

        public void NormalizeEQ()
        {
            CheckForError(SSP.SSP_NormalizeEQ());
        }

        public void StartLoop(SSPLoop loop)
        {
            CheckForError(SSP.SSP_StartLoop(ref loop.Struct));
        }

        public void UpdateLoop(SSPLoop loop)
        {
            CheckForError(SSP.SSP_UpdateLoop(ref loop.Struct));
        }

        public void StopLoop()
        {
            CheckForError(SSP.SSP_StopLoop());
        }

        public void Play()
        {
            CheckForError(SSP.SSP_Play());
        }

        public void Play(int startIndex, long startPosition, bool startPaused)
        {
            CheckForError(SSP.SSP_PlayWithOptions(startIndex, startPosition, startPaused));
        }

        public void Pause()
        {
            CheckForError(SSP.SSP_Pause());
        }

        public void Stop()
        {
            CheckForError(SSP.SSP_Stop());
        }

        public void Previous()
        {
            CheckForError(SSP.SSP_Previous());
        }

        public void Next()
        {
            CheckForError(SSP.SSP_Next());
        }

        public void GoTo(int index)
        {
            CheckForError(SSP.SSP_GoTo(index));
        }

        public void ToggleRepeatType() 
        {
            CheckForError(SSP.SSP_ToggleRepeatType());
        }

        public SSPPosition GetPosition()
        {
            var position = new SSPPosition();
            CheckForError(SSP.SSP_GetPosition(ref position.Struct));
            return position;
        }

        public SSPPosition GetPositionFromBytes(long bytes)
        {
            var position = new SSPPosition();
            CheckForError(SSP.SSP_GetPositionFromBytes(bytes, ref position.Struct));
            return position;            
        }

        public SSPPosition GetPositionFromPercentage(double percentage)
        {
            var position = new SSPPosition();
            CheckForError(SSP.SSP_GetPositionFromPercentage((float)percentage, ref position.Struct));
            return position;
        }

        public void SetPosition(long position)
        {
            CheckForError(SSP.SSP_SetPosition(position));
        }

        public void SetPosition(float position)
        {
            CheckForError(SSP.SSP_SetPositionPercentage(position));
        }

        public long Seconds2Bytes(double value) 
        {
            return SSP.SSP_GetBytesFromSecondsForCurrentChannel((float)value);
        }

        public int GetMixerData(int length, float[] sampleData)
        {
            return SSP.SSP_GetMixerData(sampleData, length);
        }

        public int GetMixerData(int length, int[] sampleData) 
        {
            return SSP.SSP_GetMixerData(sampleData, length);
        }

        public long GetDataAvailable() 
        {
            return SSP.SSP_GetDataAvailable();
        }

        public void StartEncode(SSPEncoderType encoder)
        {
            CheckForError(SSP.SSP_StartEncode(encoder));
        }

        public void StopEncode()
        {
            CheckForError(SSP.SSP_StopEncode());
        }

        public void StartCast(SSPCastServer server)
        {
            CheckForError(SSP.SSP_StartCast(server.Struct));
        }

        public void StopCast()
        {
            CheckForError(SSP.SSP_StopCast());
        }

        public IEnumerable<SSPDevice> GetOutputDevices()
        {
            int count = SSP.SSP_GetOutputDeviceCount();
            var devices = new List<SSPDevice>();
            for(int a = 0; a < count; a++) 
            {
                var device = new SSPDevice();
                bool success = SSP.SSP_GetOutputDevice(a, ref device.Struct);
                if(success)
                    devices.Add(device);
            }
            return devices;
        }
    }
}
