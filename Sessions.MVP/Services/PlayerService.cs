﻿// Copyright © 2011-2013 Yanick Castonguay
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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sessions.MVP.Config;
using Sessions.MVP.Messages;
using Sessions.MVP.Services.Interfaces;
using Sessions.Core;
using Sessions.Library.Services.Interfaces;
using Sessions.Player.Events;
using Sessions.Sound.AudioFiles;
using TinyMessenger;
using org.sessionsapp.player;
using Sessions.Sound.Player;
using Sessions.Sound.Objects;

namespace Sessions.MVP.Services
{
    /// <summary>
    /// Service used for interacting with a single instance of a Player.
    /// </summary>
    public class PlayerService : IPlayerService
    {
        private readonly ITinyMessengerHub _messengerHub;
        private readonly ILibraryService _libraryService;
        private readonly ICloudLibraryService _cloudLibraryService;
        private ISSPPlayer _sspPlayer;

//        private Playlist _currentQueue;
//        public Playlist CurrentQueue { get { return _currentQueue; } }

        public SSPLoop Loop { get; private set; }
        public SSPEQPreset EQPreset { get { return _sspPlayer.EQPreset; } }

        public SSPPlayerState State { get { return _sspPlayer.State; } }
        public bool IsSettingPosition { get { return _sspPlayer.IsSettingPosition; } }
        public bool IsPlayingLoop { get { return _sspPlayer.IsPlayingLoop; } }
        public bool IsShuffleEnabled { get { return _sspPlayer.IsShuffle; } set { _sspPlayer.IsShuffle = value; } }
        public SSPRepeatType RepeatType { get { return _sspPlayer.RepeatType; } }
        public Playlist Playlist { get { return _sspPlayer.Playlist; } }
        public AudioFile CurrentAudioFile 
        { 
            get 
            {
                var item = _sspPlayer.Playlist.GetItemAt(_sspPlayer.Playlist.CurrentIndex);
                if(item != null)
                    return _sspPlayer.Playlist.GetItemAt(_sspPlayer.Playlist.CurrentIndex).AudioFile;

                return null;
            } 
        }

        public bool IsEQEnabled { get { return _sspPlayer.EQEnabled; } }
        public float Volume { get { return _sspPlayer.Volume; } set { _sspPlayer.Volume = value; }  }
        public float TimeShifting { get { return _sspPlayer.TimeShifting; } }
        public int PitchShifting { get { return _sspPlayer.PitchShifting; } }

        public SSPMixer Mixer { get { return _sspPlayer.Mixer; } }

        public event BPMDetectedDelegate OnBPMDetected;
        public event LoopPlaybackStartedDelegate OnLoopPlaybackStarted;
        public event LoopPlaybackStoppedDelegate OnLoopPlaybackStopped;

        public PlayerService(ITinyMessengerHub messageHub, ILibraryService libraryService, ICloudLibraryService cloudLibraryService)
		{
            _messengerHub = messageHub;
            _libraryService = libraryService;
		    _cloudLibraryService = cloudLibraryService;

            _sspPlayer = new SSPPlayer();
            _sspPlayer.StateChanged += HandleStateChanged;
            _sspPlayer.PlaylistIndexChanged += HandlePlaylistIndexChanged;
            _sspPlayer.Init();
		}     

        public void Dispose()
        {
            if (_sspPlayer != null)
            {
                _sspPlayer.Dispose();
                _sspPlayer = null;
            }
        }
        
        private void SubscribeToMessages()
        {
            _messengerHub.Subscribe<PlayerCommandMessage>(PlayerCommandMessageReceived);
            _messengerHub.Subscribe<PlayerSetPositionMessage>(PlayerSetPositionMessageReceived);
        }      

        private void HandleStateChanged(IntPtr user, SSPPlayerState state)
        {
            // TODO: Use events instead
            _messengerHub.PublishAsync(new PlayerStatusMessage(this){
                State = state
            });
        }

        private void HandlePlaylistIndexChanged(IntPtr user)
        {
            Tracing.Log("PlayerService - HandlePlaylistIndexChanged - index: {0}", Playlist.CurrentIndex);
            var data = new PlayerPlaylistIndexChangedData();
            data.IsPlaybackStopped = State == SSPPlayerState.Stopped;
            data.PlaylistIndex = Playlist.CurrentIndex;
            data.PlaylistCount = Playlist.Count;
            data.PlaylistName = "New playlist";
            data.AudioFileStarted = Playlist.GetItemAt(Playlist.CurrentIndex).AudioFile;
            if (Playlist.CurrentIndex > 0)
            {
                data.AudioFileEnded = Playlist.GetItemAt(Playlist.CurrentIndex - 1).AudioFile;
            }
            if (Playlist.CurrentIndex + 1 < Playlist.Count)
            {
                data.NextAudioFile = Playlist.GetItemAt(Playlist.CurrentIndex + 1).AudioFile;
            }

            _messengerHub.PublishAsync(new PlayerPlaylistIndexChangedMessage(this) { Data = data });
            Task.Factory.StartNew(() =>
            {
                try
                {
                    // Store player status locally for resuming playback later
                    AppConfigManager.Instance.Root.ResumePlayback.AudioFileId = data.AudioFileStarted.Id.ToString();
//                    AppConfigManager.Instance.Root.ResumePlayback.PlaylistId = _player.Playlist.PlaylistId.ToString();
                    AppConfigManager.Instance.Root.ResumePlayback.EQPresetId = _sspPlayer.EQPreset.EQPresetId.ToString();
                    AppConfigManager.Instance.Root.ResumePlayback.PositionPercentage = 0;
                    AppConfigManager.Instance.Root.ResumePlayback.Timestamp = DateTime.Now;
                    AppConfigManager.Instance.Save();

                    // Store player status on Cloud if enabled in preferences
                    if (AppConfigManager.Instance.Root.Cloud.IsResumePlaybackEnabled)
                        _cloudLibraryService.PushDeviceInfo(data.AudioFileStarted, 0, "0:00.000");
                }
                catch (Exception ex)
                {
                    Tracing.Log("PlayerService - HandleOnPlaylistIndexChanged - Failed to push to Dropbox: {0}", ex);
                }
            });
        }

        private void HandleOnBPMDetected(float bpm)
        {
            if (OnBPMDetected != null)
                OnBPMDetected(IntPtr.Zero, bpm);
        }

        public void InitDevice(SSPDevice device, int sampleRate, int bufferSize, int updatePeriod)
        {
            bool useFloatingPoint = true;
            #if ANDROID
                useFloatingPoint = false; // Floating point is not supported on Android
            #endif

            _sspPlayer.InitDevice(device.DeviceId, sampleRate, bufferSize, updatePeriod, useFloatingPoint);
        
            if (!string.IsNullOrEmpty(AppConfigManager.Instance.Root.ResumePlayback.EQPresetId))
            {
                var preset = _libraryService.SelectEQPreset(new Guid(AppConfigManager.Instance.Root.ResumePlayback.EQPresetId));
                if (preset != null)
                    _sspPlayer.SetEQPreset(preset);
            }

            SubscribeToMessages();
        }

        public float GetCPU()
        {
            return _sspPlayer.GetCPU();
        }

        public UInt32 GetBufferDataAvailable()
        {
            return _sspPlayer.GetBufferDataAvailable();
        }

        public void PlayerSetPositionMessageReceived(PlayerSetPositionMessage m)
        {
            _sspPlayer.SetPosition(m.Position);
        }

        public void PlayerCommandMessageReceived(PlayerCommandMessage m)
        {
            switch (m.Command)
            {
                case PlayerCommandMessageType.Play:
                    Play();
                    break;
                case PlayerCommandMessageType.Pause:
                    Pause();
                    break;
                case PlayerCommandMessageType.Stop:
                    Stop();
                    break;
                case PlayerCommandMessageType.PlayPause:
                    PlayPause();
                    break;
                case PlayerCommandMessageType.Previous:
                    Previous();
                    break;
                case PlayerCommandMessageType.Next:
                    Next();
                    break;
            }
        }

        /// <summary>
        /// Returns mixer data in short format (only use this for non-floating point channels! i.e. 16-bit).
        /// Note: This STILL returns 32-bit data to make it easier to calculate values afterwards.
        /// </summary>
        /// <param name="seconds">Number of seconds to fetch</param>
        /// <returns>Tuple of 32-bit integers (left/right)</returns>
        public Tuple<short[], short[]> GetMixerData(double seconds)
        {
#if !WINDOWSSTORE && !WINDOWS_PHONE
            int maxL = 0;
            int maxR = 0;

            // length of a 20ms window in bytes
            int lengthToFetch = (int)_sspPlayer.Seconds2Bytes(seconds);
            int l4 = lengthToFetch / 4;

            // create a data buffer as needed
            int[] sampleData = new int[l4];
            int length = _sspPlayer.GetMixerData(lengthToFetch, sampleData);

            // From BASS.NET API: Note: an int is 32-bit meaning if we expect to receive 16-bit data stereo a single int value will contain 2 x 16-bit, so a full stereo pair of data
            l4 = length / 4;
            short[] left = new short[l4 / 2];
            short[] right = new short[l4 / 2];
            for (int a = 0; a < l4; a++)
            {
                short leftValue = Conversion.LowWord(sampleData[a]);
                short rightValue = Conversion.HighWord(sampleData[a]);

                short absLevelLeft = Math.Abs(leftValue);
                short absLevelRight = Math.Abs(rightValue);

                left[a/2] = leftValue;
                if (absLevelLeft > maxL)
                    maxL = absLevelLeft;

                right[a/2] = rightValue;
                if (absLevelRight > maxR)
                    maxR = absLevelRight;
            }

            //            // Get min max info from wave block
            //            if (AudioTools.CheckForDistortion(left, right, true, -3.0f))
            //            {
            //                // Show distortion warning "LED"
            //                //picDistortionWarning.Visible = true;
            //            }

            return new Tuple<short[], short[]>(left, right);
#else
            return new Tuple<short[], short[]>(new short[1], new short[1]);
#endif

        }

        /// <summary>
        /// Returns mixer data in float format (only use this for floating point channels! i.e. 32-bit).
        /// </summary>
        /// <param name="seconds">Number of seconds to fetch</param>
        /// <returns>Tuple of floats (left/right)</returns>
        public Tuple<float[], float[]> GetFloatingPointMixerData(double seconds)
        {
            float maxL = 0f;
            float maxR = 0f;

            // length of a 20ms window in bytes
            int lengthToFetch = (int)_sspPlayer.Seconds2Bytes(seconds); //0.02);
            // the number of 32-bit floats required (since length is in bytes!)
            int l4 = lengthToFetch / 4; // 32-bit = 4 bytes

            // create a data buffer as needed
            float[] sampleData = new float[l4];
            int length = _sspPlayer.GetMixerData(lengthToFetch, sampleData);

            // the number of 32-bit floats received
            // as less data might be returned by BASS_ChannelGetData as requested
            l4 = length / 4;
            float[] left = new float[l4 / 2];
            float[] right = new float[l4 / 2];
            for (int a = 0; a < l4; a++)
            {
                float absLevel = Math.Abs(sampleData[a]);

                // decide on L/R channel
                if (a % 2 == 0)
                {                    
                    // Left channel
                    left[a/2] = sampleData[a];
                    if (absLevel > maxL)
                        maxL = absLevel;
                }
                else
                {
                    // Right channel
                    right[a/2] = sampleData[a];
                    if (absLevel > maxR)
                        maxR = absLevel;
                }
            }

//            // Get min max info from wave block
//            if (AudioTools.CheckForDistortion(left, right, true, -3.0f))
//            {
//                // Show distortion warning "LED"
//                //picDistortionWarning.Visible = true;
//            }

            return new Tuple<float[], float[]>(left, right);
        }

        private void NotifyPlaylistUpdate()
        {
            _messengerHub.PublishAsync<PlayerPlaylistUpdatedMessage>(new PlayerPlaylistUpdatedMessage(this));

//            Task.Factory.StartNew(() => {
//                //_cloudLibraryService.PushPlaylist(_player.Playlist);
//            });
        }

        public void SetBufferSize(int bufferSize)
        {
            _sspPlayer.SetBufferSize(bufferSize);
        }

        public void SetUpdatePeriod(int updatePeriod)
        {
            _sspPlayer.SetUpdatePeriod(updatePeriod);
        }

        public void Play()
        {
            _sspPlayer.Play();
            NotifyPlaylistUpdate();
        }

        public void Play(IEnumerable<AudioFile> audioFiles, string startAudioFilePath, double initialPosition, bool startPaused, bool waitingToStart)
        {
            // TODO: Remove waitingToStart parameter
            Stop();
            _sspPlayer.Playlist.Clear();
            _sspPlayer.Playlist.AddItems(audioFiles);

            int startIndex = Math.Max(0, audioFiles.ToList().FindIndex(x => x.FilePath == startAudioFilePath));
            _sspPlayer.Play(startIndex, 0, startPaused);
            NotifyPlaylistUpdate();
        }

        public void PlayQueue()
        {
            // TODO
//            Stop();
//            _sspPlayer.Playlist.Clear();
//            CurrentPlaylist.AddItems(CurrentQueue.Items);
//            CurrentQueue.Clear();
//            Play();
        }

        public void Stop()
        {
            _sspPlayer.Stop();
        }

        public void Pause()
        {
            _sspPlayer.Pause();
        }

        public void PlayPause()
        {
            // could move this to library
            if (_sspPlayer.State == SSPPlayerState.Playing)
                Pause();
            else
                Play();
        }

        public void Next()
        {
            _sspPlayer.Next();
        }

        public void Previous()
        {
            _sspPlayer.Previous();
        }

        public void GoTo(int index)
        {
            _sspPlayer.GoTo(index);
        }

        public void GoTo(Guid playlistItemId)
        {
            // TODO
            //_sspPlayer.GoTo(playlistItemId);
        }

        public void Resume()
        {
            // TODO: Is this still necessary?
//            switch (Status)
//            {
//                case PlayerStatusType.WaitingToStart:
//                    UpdatePlayerStatus(PlayerStatusType.Playing);
//                    break;
//                case PlayerStatusType.StartPaused:
//                    UpdatePlayerStatus(PlayerStatusType.Paused);
//                    break;
//            }
        }

        public void ToggleRepeatType()
        {
            _sspPlayer.ToggleRepeatType();
        }

        public long GetDataAvailable()
        {
            return _sspPlayer.GetDataAvailable();
        }

        public SSPPosition GetPosition()
        {
            return _sspPlayer.GetPosition();
        }

        public SSPPosition GetPositionFromBytes(long bytes)
        {
            return _sspPlayer.GetPositionFromBytes(bytes);
        }

        public SSPPosition GetPositionFromPercentage(double percentage)
        {
            return _sspPlayer.GetPositionFromPercentage(percentage);            
        }

        public void SetPosition(double percentage)
        {
            _sspPlayer.SetPosition((float)percentage);
        }

        public void SetPosition(long bytes)
        {
            _sspPlayer.SetPosition(bytes);
        }

        public void SetTimeShifting(float timeShifting)
        {
            _sspPlayer.TimeShifting = timeShifting;
        }

        public void SetPitchShifting(int pitchShifting)
        {
            _sspPlayer.PitchShifting = pitchShifting;
        }

        public void GoToMarker(Marker marker)
        {
            _sspPlayer.SetPosition(marker.PositionBytes);
        }
        
        public void StartLoop(SSPLoop loop)
        {
            Loop = loop;
            _sspPlayer.StartLoop(loop);
        }

        public void UpdateLoop(SSPLoop loop)
        {
            Loop = loop;
            _sspPlayer.UpdateLoop(loop);
        }

        public void StopLoop()
        {
            _sspPlayer.StopLoop();
        }

        public void EnableEQ(bool enabled)
        {
            _sspPlayer.EQEnabled = enabled;
        }

        public void ResetEQ()
        {
            _sspPlayer.ResetEQ();
        }

        public void NormalizeEQ()
        {
            _sspPlayer.NormalizeEQ();
        }

        public void UpdateEQBand(int band, float gain, bool setCurrentEQPresetValue)
        {
            _sspPlayer.SetEQPresetBand(band, gain);
        }

        public void ApplyEQPreset(SSPEQPreset preset)
        {
            AppConfigManager.Instance.Root.ResumePlayback.EQPresetId = preset.EQPresetId.ToString();
            AppConfigManager.Instance.Save();

            _sspPlayer.SetEQPreset(preset);
        }

        public IEnumerable<SSPDevice> GetOutputDevices()
        {
            return _sspPlayer.GetOutputDevices();
        }
    }
}
