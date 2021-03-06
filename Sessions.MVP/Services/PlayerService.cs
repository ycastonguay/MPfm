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
using Sessions.MVP.Models;
using Sessions.MVP.Messages;
using Sessions.MVP.Services.Interfaces;
using Sessions.Core;
using Sessions.Library.Services.Interfaces;
using Sessions.Player;
using Sessions.Player.Events;
using Sessions.Player.Objects;
using Sessions.Sound.AudioFiles;
using Sessions.Sound.BassNetWrapper;
using Sessions.Sound.Playlists;
using TinyMessenger;

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
        private IPlayer _player;

        private Playlist _currentQueue;
        public Playlist CurrentQueue { get { return _currentQueue; } }

        public bool IsInitialized { get; private set; }
        public bool IsSettingPosition { get { return _player.IsSettingPosition; } }
        public bool IsPlaying { get { return _player.IsPlaying; } }
        public bool IsPaused { get { return _player.IsPaused; } }
        public bool IsShuffleEnabled { get { return _player.IsShuffleEnabled; } set { _player.IsShuffleEnabled = value; } }
        public bool UseFloatingPoint { get { return _player.UseFloatingPoint; } }
        public RepeatType RepeatType { get { return _player.RepeatType; } }
        public PlaylistItem CurrentPlaylistItem { get { return _player.Playlist.CurrentItem; } }
        public Playlist CurrentPlaylist { get { return _player.Playlist; } }
        public EQPreset EQPreset { get { return _player.EQPreset; } }
        public Loop Loop { get { return _player.Loop; } }
        public bool IsEQBypassed { get { return _player.IsEQBypassed; } }
        public bool IsEQEnabled { get { return _player.IsEQEnabled; } }
        public float Volume { get { return _player.Volume; } set { _player.Volume = value; }  }
        public float TimeShifting { get { return _player.TimeShifting; } }
        public int PitchShifting { get { return _player.PitchShifting; } }
        public PlayerStatusType Status { get; private set; }
        public int BufferSize { get { return _player.BufferSize; } set { _player.BufferSize = value; } }
        public int UpdatePeriod { get { return _player.UpdatePeriod; } set { _player.UpdatePeriod = value; } }

        public delegate void BPMDetected(float bpm);
        /// <summary>
        /// The OnBPMDetected event is triggered when the current BPM has been deteted or has changed.
        /// </summary>
        public event BPMDetected OnBPMDetected;

        public PlayerService(ITinyMessengerHub messageHub, ILibraryService libraryService, ICloudLibraryService cloudLibraryService)
		{
            _messengerHub = messageHub;
            _libraryService = libraryService;
		    _cloudLibraryService = cloudLibraryService;
		}
        
        public void Initialize(Device device, int sampleRate, int bufferSize, int updatePeriod)
        {
            _player = new Sessions.Player.Player(device, sampleRate, bufferSize, updatePeriod, true);
            _player.OnPlaylistEnded += HandlePlayerOnPlaylistEnded;
            _player.OnPlaylistIndexChanged += HandleOnPlaylistIndexChanged;
            _player.OnAudioInterrupted += HandleOnAudioInterrupted;
            _player.OnBPMDetected += HandleOnBPMDetected;
            _currentQueue = new Playlist();

            if (!string.IsNullOrEmpty(AppConfigManager.Instance.Root.ResumePlayback.EQPresetId))
            {
                var preset = _libraryService.SelectEQPreset(new Guid(AppConfigManager.Instance.Root.ResumePlayback.EQPresetId));
                if (preset != null)
                    _player.ApplyEQPreset(preset);
            }

            _messengerHub.Subscribe<PlayerCommandMessage>(PlayerCommandMessageReceived);
            _messengerHub.Subscribe<PlayerSetPositionMessage>(PlayerSetPositionMessageReceived);
            IsInitialized = true;
        }

        private void HandlePlayerOnPlaylistEnded()
        {
            if (Status == PlayerStatusType.Stopped)
                return;

            UpdatePlayerStatus(PlayerStatusType.Stopped);
        }

        void HandleOnBPMDetected(float bpm)
        {
            if (OnBPMDetected != null)
                OnBPMDetected(bpm);
        }

        void HandleOnAudioInterrupted(AudioInterruptedData data)
        {
            UpdatePlayerStatus(PlayerStatusType.Paused);
        }

        void HandleOnPlaylistIndexChanged(PlayerPlaylistIndexChangedData data)
        {
            _messengerHub.PublishAsync(new PlayerPlaylistIndexChangedMessage(this) { Data = data });
            Task.Factory.StartNew(() =>
            {
                try
                {
                    // Store player status locally for resuming playback later
                    AppConfigManager.Instance.Root.ResumePlayback.AudioFileId = data.AudioFileStarted.Id.ToString();
                    AppConfigManager.Instance.Root.ResumePlayback.PlaylistId = _player.Playlist.PlaylistId.ToString();
                    AppConfigManager.Instance.Root.ResumePlayback.EQPresetId = _player.EQPreset.EQPresetId.ToString();
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

        private void UpdatePlayerStatus(PlayerStatusType status)
        {
            Status = status;
            _messengerHub.PublishAsync(new PlayerStatusMessage(this){
                Status = status
            });
        }

        public void PlayerSetPositionMessageReceived(PlayerSetPositionMessage m)
        {
            _player.SetPosition(m.Position);
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
            int lengthToFetch = (int)_player.Seconds2Bytes(seconds);
            int l4 = lengthToFetch / 4;

            // create a data buffer as needed
            int[] sampleData = new int[l4];
            int length = _player.GetMixerData(lengthToFetch, sampleData);

            // From BASS.NET API: Note: an int is 32-bit meaning if we expect to receive 16-bit data stereo a single int value will contain 2 x 16-bit, so a full stereo pair of data
            l4 = length / 4;
            short[] left = new short[l4 / 2];
            short[] right = new short[l4 / 2];
            for (int a = 0; a < l4; a++)
            {
                short leftValue = Base.LowWord(sampleData[a]);
                short rightValue = Base.HighWord(sampleData[a]);

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
            int lengthToFetch = (int)_player.Seconds2Bytes(seconds); //0.02);
            // the number of 32-bit floats required (since length is in bytes!)
            int l4 = lengthToFetch / 4; // 32-bit = 4 bytes

            // create a data buffer as needed
            float[] sampleData = new float[l4];
            int length = _player.GetMixerData(lengthToFetch, sampleData);

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

        public void Play()
        {
            _player.Play();
            UpdatePlayerStatus(PlayerStatusType.Playing);
            NotifyPlaylistUpdate();
        }

        public void Play(IEnumerable<string> filePaths)
        {
            _player.Playlist.Clear();
            _player.Playlist.AddItems(filePaths.ToList());
            _player.Play();
            UpdatePlayerStatus(PlayerStatusType.Playing);
            NotifyPlaylistUpdate();
        }

        public void Play(IEnumerable<AudioFile> audioFiles, string startAudioFilePath, double initialPosition, bool startPaused, bool waitingToStart)
        {
            _player.Playlist.Clear();
            var items = new List<PlaylistItem>();
            foreach (var audioFile in audioFiles)
                items.Add(new PlaylistItem(audioFile));
            _player.Playlist.AddItems(items);

            if (!string.IsNullOrEmpty(startAudioFilePath))
            {
                var item = _player.Playlist.Items.FirstOrDefault(x => x.AudioFile.FilePath == startAudioFilePath);
                _player.Playlist.GoTo(item.Id);
            }

            _player.Play(initialPosition, startPaused);

            if(startPaused)
                UpdatePlayerStatus(PlayerStatusType.StartPaused);
            else if(waitingToStart)
                UpdatePlayerStatus(PlayerStatusType.WaitingToStart);
            else
                UpdatePlayerStatus(PlayerStatusType.Playing);

            NotifyPlaylistUpdate();
        }

        public void PlayQueue()
        {
            Stop();
            CurrentPlaylist.Clear();
            CurrentPlaylist.AddItems(CurrentQueue.Items);
            CurrentQueue.Clear();
            Play();
        }

        public void Stop()
        {
            if (_player.IsPlaying)
            {
                _player.Stop();
                UpdatePlayerStatus(PlayerStatusType.Stopped);
            }
        }

        public void Pause()
        {
            _player.Pause();
            PlayerStatusType statusType = (_player.IsPaused) ? PlayerStatusType.Paused : PlayerStatusType.Playing;
            UpdatePlayerStatus(statusType);
        }

        public void PlayPause()
        {
            if (_player.IsPlaying)
                Pause();
            else
                Play();
        }

        public void Next()
        {
            _player.Next();
            UpdatePlayerStatus(PlayerStatusType.Playing);
        }

        public void Previous()
        {
            _player.Previous();
            UpdatePlayerStatus(PlayerStatusType.Playing);
        }

        public void GoTo(int index)
        {
            _player.GoTo(index);
            UpdatePlayerStatus(PlayerStatusType.Playing);
        }

        public void GoTo(Guid playlistItemId)
        {
            _player.GoTo(playlistItemId);
            UpdatePlayerStatus(PlayerStatusType.Playing);
        }

        public void Resume()
        {
            switch (Status)
            {
                case PlayerStatusType.WaitingToStart:
                    UpdatePlayerStatus(PlayerStatusType.Playing);
                    break;
                case PlayerStatusType.StartPaused:
                    UpdatePlayerStatus(PlayerStatusType.Paused);
                    break;
            }
        }

        public void ToggleRepeatType()
        {
            if (_player.RepeatType == RepeatType.Off)
                _player.RepeatType = RepeatType.Playlist;
            else if (_player.RepeatType == RepeatType.Playlist)
                _player.RepeatType = RepeatType.Song;
            else
                _player.RepeatType = RepeatType.Off;
        }

        public int GetDataAvailable()
        {
            return _player.GetDataAvailable();
        }

        public static PlayerPositionEntity GetPositionEntity(long positionBytes, long lengthBytes, uint bitsPerSample, uint sampleRate)
        {
            PlayerPositionEntity entity = new PlayerPositionEntity();
            entity.PositionBytes = positionBytes;
            entity.PositionSamples = ConvertAudio.ToPCM(entity.PositionBytes, bitsPerSample, 2);
            entity.PositionMS = (int)ConvertAudio.ToMS(entity.PositionSamples, sampleRate);
            entity.Position = Conversion.MillisecondsToTimeString((ulong)entity.PositionMS);
            entity.PositionPercentage = ((float)positionBytes / (float)lengthBytes) * 100;
            return entity;
        }

        public PlayerPositionEntity GetPosition()
        {
            if (CurrentPlaylistItem == null)
                return new PlayerPositionEntity();

            PlayerPositionEntity entity = new PlayerPositionEntity();
            try
            {
                entity.PositionBytes = _player.GetPosition();
                entity.PositionSamples = ConvertAudio.ToPCM(entity.PositionBytes, (uint)CurrentPlaylistItem.AudioFile.BitsPerSample, 2);
                entity.PositionMS = (int)ConvertAudio.ToMS(entity.PositionSamples, (uint)CurrentPlaylistItem.AudioFile.SampleRate);
                entity.Position = Conversion.MillisecondsToTimeString((ulong)entity.PositionMS);
                entity.PositionPercentage = ((float)entity.PositionBytes / (float)CurrentPlaylistItem.LengthBytes) * 100;
            }
            catch (Exception ex)
            {
                Tracing.Log(string.Format("PlayerService - HandleTimerRefreshSongPositionElapsed - Failed to get player position: {0}", ex));
            }
            return entity;
        }

        public void SetPosition(double percentage)
        {
            _player.SetPosition(percentage);
        }

        public void SetPosition(long bytes)
        {
            _player.SetPosition(bytes);
        }

        public void SetTimeShifting(float timeShifting)
        {
            _player.TimeShifting = timeShifting;
        }

        public void SetPitchShifting(int pitchShifting)
        {
            _player.PitchShifting = pitchShifting;
        }

        public void GoToMarker(Marker marker)
        {
            _player.GoToMarker(marker);
        }
        
        public void StartLoop(Loop loop)
        {
            _player.StartLoop(loop);
        }

        public void StopLoop()
        {
            _player.StopLoop();
        }

        public void BypassEQ()
        {
            _player.BypassEQ();
        }

        public void ResetEQ()
        {
            _player.ResetEQ();
        }

        public void UpdateEQBand(int band, float gain, bool setCurrentEQPresetValue)
        {
            _player.UpdateEQBand(band, gain, setCurrentEQPresetValue);
        }

        public void ApplyEQPreset(EQPreset preset)
        {
            AppConfigManager.Instance.Root.ResumePlayback.EQPresetId = preset.EQPresetId.ToString();
            AppConfigManager.Instance.Save();

            _player.ApplyEQPreset(preset);
        }

        public void Dispose()
        {
            if (_player != null)
                _player.Dispose();
        }
    }
}
