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
using System.Linq;
using MPfm.Player;
using MPfm.Player.Events;
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;
using MPfm.Sound.BassNetWrapper;
using MPfm.Sound.Playlists;
using MPfm.MVP.Messages;
using MPfm.MVP.Services.Interfaces;
using TinyMessenger;

namespace MPfm.MVP.Services
{
    /// <summary>
    /// Service used for interacting with a single instance of a Player.
    /// </summary>
    public class PlayerService : IPlayerService
    {
        private readonly ITinyMessengerHub _messengerHub;
        private IPlayer _player;
        private PlayerStatusType _status;

        public bool IsSettingPosition { get { return _player.IsSettingPosition; } }
        public bool IsPlaying { get { return _player.IsPlaying; } }
        public bool IsPaused { get { return _player.IsPaused; } }
        public RepeatType RepeatType { get { return _player.RepeatType; } }
        public PlaylistItem CurrentPlaylistItem { get { return _player.Playlist.CurrentItem; } }
        public Playlist CurrentPlaylist { get { return _player.Playlist; } }
        public EQPreset EQPreset { get { return _player.EQPreset; } }
        public bool IsEQBypassed { get { return _player.IsEQBypassed; } }
        public bool IsEQEnabled { get { return _player.IsEQEnabled; } }
        public float Volume { get { return _player.Volume; } set { _player.Volume = value; }  }

        public delegate void BPMDetected(float bpm);
        /// <summary>
        /// The OnBPMDetected event is triggered when the current BPM has been deteted or has changed.
        /// </summary>
        public event BPMDetected OnBPMDetected;

		public PlayerService(ITinyMessengerHub messageHub)
		{
            _messengerHub = messageHub;
		}

        public void Initialize(Device device, int sampleRate, int bufferSize, int updatePeriod)
        {
            // Initialize player
            _player = new MPfm.Player.Player(device, sampleRate, bufferSize, updatePeriod, true);
            _player.OnPlaylistIndexChanged += HandleOnPlaylistIndexChanged;
            _player.OnAudioInterrupted += HandleOnAudioInterrupted;
            _player.OnBPMDetected += HandleOnBPMDetected;
            _messengerHub.Subscribe<PlayerCommandMessage>(PlayerCommandMessageReceived);
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
        }

        private void UpdatePlayerStatus(PlayerStatusType status)
        {
            _status = status;
            _messengerHub.PublishAsync(new PlayerStatusMessage(this){
                Status = status
            });
        }

        /// <summary>
        /// Receives player commands via TinyMessenger.
        /// </summary>
        /// <param name="m">Message</param>
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

        public Tuple<float[], float[]> GetMixerData(double seconds)
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

        public void Play()
        {
            _player.Play();
            UpdatePlayerStatus(PlayerStatusType.Playing);
            _messengerHub.PublishAsync<PlayerPlaylistUpdatedMessage>(new PlayerPlaylistUpdatedMessage(this));
        }

        public void Play(IEnumerable<AudioFile> audioFiles)
        {
            _player.Playlist.Clear();
            _player.Playlist.AddItems(audioFiles.ToList());
            _player.Play();
            UpdatePlayerStatus(PlayerStatusType.Playing);
            _messengerHub.PublishAsync<PlayerPlaylistUpdatedMessage>(new PlayerPlaylistUpdatedMessage(this));
        }

        public void Play(IEnumerable<string> filePaths)
        {
            _player.Playlist.Clear();
            _player.Playlist.AddItems(filePaths.ToList());
            _player.Play();
            UpdatePlayerStatus(PlayerStatusType.Playing);
            _messengerHub.PublishAsync<PlayerPlaylistUpdatedMessage>(new PlayerPlaylistUpdatedMessage(this));
        }

        public void Play(IEnumerable<AudioFile> audioFiles, string startAudioFilePath)
        {
            _player.Playlist.Clear();
            _player.Playlist.AddItems(audioFiles.ToList());
            _player.Playlist.GoTo(startAudioFilePath);
            _player.Play();
            UpdatePlayerStatus(PlayerStatusType.Playing);
            _messengerHub.PublishAsync<PlayerPlaylistUpdatedMessage>(new PlayerPlaylistUpdatedMessage(this));
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
                _player.Pause();
            else
                _player.Play();
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

        public long GetPosition()
        {
            return _player.GetPosition();
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
            _player.ApplyEQPreset(preset);
        }

        /// <summary>
        /// Releases all resource used by the <see cref="PlayerService"/> object.
        /// </summary>
        /// <remarks>
        /// Call <see cref="Dispose"/> when you are finished using the <see cref="PlayerService"/>. The
        /// <see cref="Dispose"/> method leaves the <see cref="PlayerService"/> in an unusable state. After
        /// calling <see cref="Dispose"/>, you must release all references to the <see cref="PlayerService"/>
        /// so the garbage collector can reclaim the memory that the <see cref="PlayerService"/> was occupying.
        /// </remarks>
        public void Dispose()
        {
            // Dispose player
            if (_player != null)
            {
                _player.Dispose();
                _player = null;
            }
        }
    }
}
