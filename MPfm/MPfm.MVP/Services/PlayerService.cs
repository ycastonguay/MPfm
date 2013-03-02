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

using System.Collections.Generic;
using MPfm.Player;
using MPfm.Sound.Bass.Net;
using MPfm.MVP.Messages;
using MPfm.MVP.Services.Interfaces;
using TinyMessenger;
using MPfm.Sound.AudioFiles;
using System.Linq;
using MPfm.Sound.Playlists;
using MPfm.Player.Events;

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
        public bool IsPaused { get { return _player.IsPaused; } }
        public PlaylistItem CurrentPlaylistItem { get { return _player.Playlist.CurrentItem; } }
        public float Volume { get { return _player.Volume; } }

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
            _messengerHub.Subscribe<PlayerCommandMessage>(PlayerCommandMessageReceived);
        }

        /// <summary>
        /// This player notification is used to notify that the audio has been interrupted (only used on iOS).
        /// </summary>
        /// <param name="data">Event data</param>
        void HandleOnAudioInterrupted(AudioInterruptedData data)
        {
            UpdatePlayerStatus(PlayerStatusType.Paused);
        }

        /// <summary>
        /// This player notification is used to notify that a new song is playing.
        /// </summary>
        /// <param name="data">Event data</param>
        void HandleOnPlaylistIndexChanged(PlayerPlaylistIndexChangedData data)
        {
            _messengerHub.PublishAsync(new PlayerPlaylistIndexChangedMessage(this) { Data = data });
        }

        private void UpdatePlayerStatus(PlayerStatusType status)
        {
            this._status = status;
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

        public void Play()
        {
            _player.Play();
            UpdatePlayerStatus(PlayerStatusType.Playing);
        }

        public void Play(IEnumerable<AudioFile> audioFiles)
        {
            _player.Playlist.Clear();
            _player.Playlist.AddItems(audioFiles.ToList());
            _player.Play();
            UpdatePlayerStatus(PlayerStatusType.Playing);
        }

        public void Play(IEnumerable<string> filePaths)
        {
            _player.Playlist.Clear();
            _player.Playlist.AddItems(filePaths.ToList());
            _player.Play();
            UpdatePlayerStatus(PlayerStatusType.Playing);
        }

        public void Play(IEnumerable<AudioFile> audioFiles, string startAudioFilePath)
        {
            _player.Playlist.Clear();
            _player.Playlist.AddItems(audioFiles.ToList());
            _player.Playlist.GoTo(startAudioFilePath);
            _player.Play();
            UpdatePlayerStatus(PlayerStatusType.Playing);
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

        public void RepeatType()
        {
            // TODO: Cycle through repeat types
            //_player.RepeatType
        }

        public int GetDataAvailable()
        {
            return _player.MixerChannel.GetDataAvailable();
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

        public void SetVolume(float volume)
        {
            _player.Volume = volume;
        }

        public void SetTimeShifting(float timeShifting)
        {
            _player.TimeShifting = timeShifting;
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
