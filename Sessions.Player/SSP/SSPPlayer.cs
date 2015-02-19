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
using org.sessionsapp.player;
using Sessions.Core;
using System.IO;
using System.Reflection;

#if IOS
using MonoTouch;
#endif

namespace Sessions.Player
{
    public class SSPPlayer : ISSPPlayer 
    {
        /// <summary>
        /// This static instance of the player is used only on iOS, because MonoTouch requires all C callbacks
        /// to be static.
        /// http://docs.xamarin.com/ios/guides/advanced_topics/limitations
        /// </summary>
        public static SSPPlayer CurrentPlayer = null;

        public SSPPlaylist Playlist { get; private set; }

        private LogDelegate _logDelegate;
        private PlaylistIndexChangedDelegate _playlistIndexChangedDelegate;
        private StateChangedDelegate _stateDelegate;

        public event LogDelegate Log;
        public event PlaylistIndexChangedDelegate PlaylistIndexChanged;
        public event StateChangedDelegate StateChanged;

        public int Version
        {
            get { return SSP.SSP_GetVersion(); }
        }

        public SSPPlayerState State
        {
            get { return SSP.SSP_GetState(); }
        }

        public SSP_DEVICE Device
        {
            get
            {
                var device = new SSP_DEVICE();
                SSP.SSP_GetDevice(ref device);
                return device;
            }
        }

        public SSP_MIXER Mixer
        {
            get
            {
                var mixer = new SSP_MIXER();
                SSP.SSP_GetMixer(ref mixer);
                return mixer;
            }
        }

        public SSP_EQPRESET EQPreset
        {
            get
            {
                var preset = new SSP_EQPRESET();
                SSP.SSP_GetEQPreset(ref preset);
                return preset;
            }
        }

        public SSP_LOOP Loop
        {
            get
            {
                var loop = new SSP_LOOP();
                SSP.SSP_GetLoop(ref loop);
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
            get { return SSP.SSP_GetVolume(); }
            set { SSP.SSP_SetVolume(value); }
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
            get { return SSP.SSP_GetIsPlayingLoop(); }
        }

        public SSPPlayer()
        {
            Playlist = new SSPPlaylist();
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

        #endif

        public void HandleLog(IntPtr user, string str)
        {
            Tracing.Log("libssp_player - {0}", str);

            if (Log != null)
            {
                Log(user, str);
            }
        }

        public void HandleStateChanged(IntPtr user, SSPPlayerState state)
        {
            Tracing.Log("libssp_player - state changed {0}", state);
            if (StateChanged != null)
            {
                StateChanged(user, state);
            }
        }

        public void HandlePlaylistIndexChanged(IntPtr user)
        {
            Tracing.Log("libssp_player - playlist index changed");
            if (PlaylistIndexChanged != null)
            {
                PlaylistIndexChanged(user);
            }
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
            #else
            _logDelegate = new LogDelegate(HandleLog);
            _stateDelegate = new StateChangedDelegate(HandleStateChanged);
            _playlistIndexChangedDelegate = new PlaylistIndexChangedDelegate(HandlePlaylistIndexChanged);
            #endif

            SSP.SSP_SetLogCallback(_logDelegate, IntPtr.Zero);
            SSP.SSP_SetStateChangedCallback(_stateDelegate, IntPtr.Zero);
            SSP.SSP_SetPlaylistIndexChangedCallback(_playlistIndexChangedDelegate, IntPtr.Zero);
        }
        
        public void InitDevice(int deviceId, int sampleRate, int bufferSize, int updatePeriod, bool useFloatingPoint) 
        {
            CheckForError(SSP.SSP_InitDevice(deviceId, sampleRate, bufferSize, updatePeriod, useFloatingPoint));
        }

        public void FreeDevice()
        {
            CheckForError(SSP.SSP_FreeDevice());
        }

        public void Free()
        {
            CheckForError(SSP.SSP_Free());
        }

        public void Dispose()
        {
            _logDelegate = null;
            _stateDelegate = null;
            _playlistIndexChangedDelegate = null;
        }

        public void SetBufferSize(int bufferSize)
        {
            CheckForError(SSP.SSP_SetBufferSize(bufferSize));
        }

        public void SetUpdatePeriod(int updatePeriod)
        {
            CheckForError(SSP.SSP_SetUpdatePeriod(updatePeriod));
        }

        public void SetEQPreset(SSP_EQPRESET preset)
        {
            CheckForError(SSP.SSP_SetEQPreset(preset));
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

        public void StartLoop(SSP_LOOP loop)
        {
            CheckForError(SSP.SSP_StartLoop(loop));
        }

        public void UpdateLoop(SSP_LOOP loop)
        {
            CheckForError(SSP.SSP_UpdateLoop(loop));
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

        public SSP_POSITION GetPosition()
        {
            var position = new SSP_POSITION();
            CheckForError(SSP.SSP_GetPosition(ref position));
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
            // TODO: Complete this
//            int data = SSP.SSP_GetMixerData(sampleData, length);
//            return data;
            return 0;
        }

        public int GetMixerData(int length, int[] sampleData) 
        {
            // TODO: Complete this
//            int data = SSP.SSP_GetMixerData(sampleData, length);
//            return data;
            return 0;
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

        public void StartCast(SSP_CAST_SERVER server)
        {
            CheckForError(SSP.SSP_StartCast(server));
        }

        public void StopCast()
        {
            CheckForError(SSP.SSP_StopCast());
        }
    }
}
