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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sessions.MVP.Config;
using Sessions.MVP.Messages;
using Sessions.MVP.Services.Interfaces;
using Sessions.Core;
using Sessions.Library.Services.Interfaces;
using Sessions.Player;
using Sessions.Player.Events;
using Sessions.Player.Objects;
using Sessions.Sound.AudioFiles;
using Sessions.Sound.BassNetWrapper;
using TinyMessenger;
using org.sessionsapp.player;

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

        public Loop Loop { get; private set; }
        public EQPreset EQPreset { get; private set; }

        public SSPPlayerState State { get { return _sspPlayer.State; } }
        public bool IsSettingPosition { get { return _sspPlayer.IsSettingPosition; } }
        public bool IsPlayingLoop { get { return _sspPlayer.IsPlayingLoop; } }
        public bool IsShuffleEnabled { get { return _sspPlayer.IsShuffle; } set { _sspPlayer.IsShuffle = value; } }
        public SSPRepeatType RepeatType { get { return _sspPlayer.RepeatType; } }
        public SSPPlaylist Playlist { get { return _sspPlayer.Playlist; } }
        public AudioFile CurrentAudioFile 
        { 
            get 
            {
                return _sspPlayer.Playlist.GetItemAt(_sspPlayer.Playlist.CurrentIndex);
            } 
        }

        public bool IsEQEnabled { get { return _sspPlayer.EQEnabled; } }
        public float Volume { get { return _sspPlayer.Volume; } set { _sspPlayer.Volume = value; }  }
        public float TimeShifting { get { return _sspPlayer.TimeShifting; } }
        public int PitchShifting { get { return _sspPlayer.PitchShifting; } }

        public SSP_MIXER Mixer { get { return _sspPlayer.Mixer; } }

        public delegate void BPMDetected(float bpm);
        /// <summary>
        /// The OnBPMDetected event is triggered when the current BPM has been deteted or has changed.
        /// </summary>
        public event BPMDetected OnBPMDetected;

        public event LoopPlaybackStarted OnLoopPlaybackStarted;
        public event LoopPlaybackStopped OnLoopPlaybackStopped;

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
            data.AudioFileStarted = Playlist.GetItemAt(Playlist.CurrentIndex);
            if (Playlist.CurrentIndex > 0)
            {
                data.AudioFileEnded = Playlist.GetItemAt(Playlist.CurrentIndex - 1);
            }
            if (Playlist.CurrentIndex + 1 < Playlist.Count)
            {
                data.NextAudioFile = Playlist.GetItemAt(Playlist.CurrentIndex + 1);
            }

            _messengerHub.PublishAsync(new PlayerPlaylistIndexChangedMessage(this) { Data = data });
            Task.Factory.StartNew(() =>
            {
                try
                {
                    // Store player status locally for resuming playback later
                    AppConfigManager.Instance.Root.ResumePlayback.AudioFileId = data.AudioFileStarted.Id.ToString();
//                    AppConfigManager.Instance.Root.ResumePlayback.PlaylistId = _player.Playlist.PlaylistId.ToString();
//                    AppConfigManager.Instance.Root.ResumePlayback.EQPresetId = _player.EQPreset.EQPresetId.ToString();
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
                OnBPMDetected(bpm);
        }

        public void InitDevice(Device device, int sampleRate, int bufferSize, int updatePeriod)
        {
            _sspPlayer.InitDevice(device.Id, sampleRate, bufferSize, updatePeriod, true);
        
//            if (!string.IsNullOrEmpty(AppConfigManager.Instance.Root.ResumePlayback.EQPresetId))
//            {
//                var preset = _libraryService.SelectEQPreset(new Guid(AppConfigManager.Instance.Root.ResumePlayback.EQPresetId));
//                if (preset != null)
//                    _sspPlayer.SetEQPreset(preset);
//            }

            SubscribeToMessages();
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

        public void Play(IEnumerable<string> filePaths)
        {
            Stop();
            _sspPlayer.Playlist.Clear();
            _sspPlayer.Playlist.AddItems(filePaths.ToList());
            Play();
        }

        public void Play(IEnumerable<AudioFile> audioFiles, string startAudioFilePath, double initialPosition, bool startPaused, bool waitingToStart)
        {
            // TODO: Remove waitingToStart parameter
            Stop();
            _sspPlayer.Playlist.Clear();
            foreach (var audioFile in audioFiles)
                _sspPlayer.Playlist.AddItem(audioFile.FilePath);

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

        public SSP_POSITION GetPosition()
        {
            var position = _sspPlayer.GetPosition();
            return position;
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
        
        public void StartLoop(Loop loop)
        {
            Loop = loop;
            _sspPlayer.StartLoop(loop.ToSSPLoop());
        }

        public void UpdateLoop(Loop loop)
        {
            Loop = loop;
            _sspPlayer.UpdateLoop(loop.ToSSPLoop());
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

        public void UpdateEQBand(int band, float gain, bool setCurrentEQPresetValue)
        {
            _sspPlayer.SetEQPresetBand(band, gain);
        }

        public void ApplyEQPreset(EQPreset preset)
        {
            AppConfigManager.Instance.Root.ResumePlayback.EQPresetId = preset.EQPresetId.ToString();
            AppConfigManager.Instance.Save();

            //_sspPlayer.SetEQPreset(preset);
        }
    }
}
