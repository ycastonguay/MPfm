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
using System.Timers;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Messages;
using MPfm.MVP.Models;
using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Services.Interfaces;
using MPfm.MVP.Views;
using MPfm.Player.Events;
using MPfm.Sound.AudioFiles;
using MPfm.Core;
using TinyMessenger;
using MPfm.Sound.BassNetWrapper;
using MPfm.Library.Services.Interfaces;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Player presenter.
	/// </summary>
	public class PlayerPresenter : BasePresenter<IPlayerView>, IPlayerPresenter
	{
        readonly MobileNavigationManager _mobileNavigationManager;
        readonly NavigationManager _navigationManager;
        readonly IPlayerService _playerService;
        readonly ILibraryService _libraryService;
        readonly IAudioFileCacheService _audioFileCacheService;
        readonly ITinyMessengerHub _messageHub;
        Timer _timerRefreshSongPosition;

		public PlayerPresenter(ITinyMessengerHub messageHub, IPlayerService playerService, IAudioFileCacheService audioFileCacheService, ILibraryService libraryService)
		{	
            _messageHub = messageHub;
            _playerService = playerService;
            _audioFileCacheService = audioFileCacheService;
            _libraryService = libraryService;

			_timerRefreshSongPosition = new Timer();			
			_timerRefreshSongPosition.Interval = 100;
			_timerRefreshSongPosition.Elapsed += HandleTimerRefreshSongPositionElapsed;

            // Subscribe to events
            messageHub.Subscribe<LibraryBrowserItemDoubleClickedMessage>((LibraryBrowserItemDoubleClickedMessage m) => {
                Play(audioFileCacheService.SelectAudioFiles(m.Query));
            });
            messageHub.Subscribe<SongBrowserItemDoubleClickedMessage>((SongBrowserItemDoubleClickedMessage m) => {
                string filePath = m != null ? m.Item.FilePath : null;
                Play(audioFileCacheService.SelectAudioFiles(m.Query), filePath);
            });
            messageHub.Subscribe<MobileLibraryBrowserItemClickedMessage>((MobileLibraryBrowserItemClickedMessage m) => {
                Play(audioFileCacheService.SelectAudioFiles(m.Query), m.FilePath);
            });
            messageHub.Subscribe<PlayerPlaylistIndexChangedMessage>((PlayerPlaylistIndexChangedMessage m) => {
                View.RefreshSongInformation(m.Data.AudioFileStarted, playerService.CurrentPlaylistItem.LengthBytes, 
                                            playerService.CurrentPlaylist.Items.IndexOf(playerService.CurrentPlaylistItem), playerService.CurrentPlaylist.Items.Count);

                var markers = libraryService.SelectMarkers(m.Data.AudioFileStarted.Id);
                View.RefreshMarkers(markers);
            });
            messageHub.Subscribe<PlayerStatusMessage>((PlayerStatusMessage m) => {
                View.RefreshPlayerStatus(m.Status);
            });
            messageHub.Subscribe<MarkerUpdatedMessage>((MarkerUpdatedMessage m) => {
                var markers = libraryService.SelectMarkers(m.AudioFileId);
                View.RefreshMarkers(markers);
            });

#if IOS || ANDROID
            _mobileNavigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
#else
            _navigationManager = Bootstrapper.GetContainer().Resolve<NavigationManager>();
#endif
        }
        
        public void Dispose()
        {
            _playerService.Dispose();
        }
        
        public override void BindView(IPlayerView view)
        {
            base.BindView(view);
            
            view.OnPlayerPlay = Play;
            view.OnPlayerPause = Pause;
            view.OnPlayerStop = Stop;
            view.OnPlayerPrevious = Previous;
            view.OnPlayerNext = Next;
            view.OnPlayerSetPosition = SetPosition;
            view.OnPlayerSetVolume = SetVolume;
            view.OnPlayerRequestPosition = RequestPosition;
            view.OnEditSongMetadata = EditSongMetadata;

            // If the player is already playing, refresh initial data
            if (_playerService.IsPlaying)
            {
                View.RefreshSongInformation(_playerService.CurrentPlaylistItem.AudioFile, _playerService.CurrentPlaylistItem.LengthBytes,
                            _playerService.CurrentPlaylist.Items.IndexOf(_playerService.CurrentPlaylistItem), _playerService.CurrentPlaylist.Items.Count);

                var markers = _libraryService.SelectMarkers(_playerService.CurrentPlaylistItem.AudioFile.Id);
                View.RefreshMarkers(markers);
            }

            View.RefreshPlayerVolume(new PlayerVolumeEntity() {
                Volume = 100,
                VolumeString = "100%"
            });
        }

		void HandleTimerRefreshSongPositionElapsed(object sender, ElapsedEventArgs e)
		{
            if(_playerService.IsSettingPosition)
                return;

            //int available = playerService.GetDataAvailable();
            
			PlayerPositionEntity entity = new PlayerPositionEntity();
		    try
		    {
                entity.PositionBytes = _playerService.GetPosition();
                entity.PositionSamples = ConvertAudio.ToPCM(entity.PositionBytes, (uint)_playerService.CurrentPlaylistItem.AudioFile.BitsPerSample, 2);
                entity.PositionMS = (int)ConvertAudio.ToMS(entity.PositionSamples, (uint)_playerService.CurrentPlaylistItem.AudioFile.SampleRate);
                //entity.Position = available.ToString() + " " + Conversion.MillisecondsToTimeString((ulong)entity.PositionMS);
                entity.Position = Conversion.MillisecondsToTimeString((ulong)entity.PositionMS);
                entity.PositionPercentage = ((float)_playerService.GetPosition() / (float)_playerService.CurrentPlaylistItem.LengthBytes) * 100;
		    }
		    catch (Exception ex)
		    {
		        Tracing.Log(string.Format("PlayerPresenter - HandleTimerRefreshSongPositionElapsed - Failed to get player position: {0}", ex));
		    }

			View.RefreshPlayerPosition(entity);
		}

	    public void EditSongMetadata()
	    {
#if IOS || ANDROID
            // Not available yet on mobile devices
#else
            if(_playerService.CurrentPlaylistItem != null)
	            _navigationManager.CreateEditSongMetadataView(_playerService.CurrentPlaylistItem.AudioFile);
#endif
	    }

	    /// <summary>
		/// Starts playback.
		/// </summary>
		public void Play()
		{            
            try
            {
                _playerService.Play();
    			_timerRefreshSongPosition.Start();
            }
            catch(Exception ex)
            {
                SetError(ex);
            }
		}
		
		/// <summary>
		/// Starts the playback of a new playlist.
		/// </summary>
		/// <param name='audioFiles'>List of audio files</param>		
		public void Play(IEnumerable<AudioFile> audioFiles)
		{
            try
            {
                _playerService.Play(audioFiles);
                _timerRefreshSongPosition.Start();
            }
            catch(Exception ex)
            {
                SetError(ex);
            }
		}

		/// <summary>
		/// Starts the playback of a new playlist.
		/// </summary>
		/// <param name='filePaths'>List of audio file paths</param>
		public void Play(IEnumerable<string> filePaths)
		{
            try
            {
                _playerService.Play(filePaths);
                _timerRefreshSongPosition.Start();
            }
            catch(Exception ex)
            {
                SetError(ex);
            }
		}
		
		/// <summary>
		/// Starts the playback of a new playlist at a specific position.
		/// </summary>
		/// <param name='audioFiles'>List of audio files</param>
		/// <param name='startAudioFilePath'>File path of the first playlist item to play</param>
		public void Play(IEnumerable<AudioFile> audioFiles, string startAudioFilePath)
		{
            try
            {
                _playerService.Play(audioFiles, startAudioFilePath);
                _timerRefreshSongPosition.Start();
            }
            catch(Exception ex)
            {
                SetError(ex);
            }                
		}
		
		/// <summary>
		/// Stops playback.
		/// </summary>
		public void Stop()
		{
            try
            {
				// Stop timer
                Tracing.Log("PlayerPresenter.Stop -- Stopping timer...");
				_timerRefreshSongPosition.Stop();
				
				// Stop player
                Tracing.Log("PlayerPresenter.Stop -- Stopping playback...");
                _playerService.Stop();
				
				// Refresh view with empty information
                Tracing.Log("PlayerPresenter.Stop -- Refresh song information and position with empty entity...");
			    View.RefreshSongInformation(null, 0, 0, 0);
                View.RefreshPlayerPosition(new PlayerPositionEntity());
            }
            catch(Exception ex)
            {
                SetError(ex);
            }
        }
		
		/// <summary>
		/// Pauses playback.
		/// </summary>
		public void Pause()
		{
            try
            {
                _playerService.Pause();
            }
            catch(Exception ex)
            {
                SetError(ex);
            }
		}
		
		/// <summary>
		/// Skips to the next song in the playlist.
		/// </summary>
		public void Next()
		{
            try
            {
                Tracing.Log("PlayerPresenter.Next -- Skipping to next item in playlist...");
                _playerService.Next();
            }
            catch(Exception ex)
            {
                SetError(ex);
            }
        }
		
		/// <summary>
		/// Skips to the previous song in the playlist.
		/// </summary>
		public void Previous()
		{            
            try
            {
                Tracing.Log("PlayerPresenter.Previous -- Skipping to previous item in playlist...");
                _playerService.Previous();
            }
            catch(Exception ex)
            {
                SetError(ex);
            }                
		}
		
		/// <summary>
		/// Cycles through the repeat types (Off, Song, Playlist).
		/// </summary>
		public void RepeatType()
		{
		}

        private PlayerPositionEntity RequestPosition(float positionPercentage)
        {
            try
            {
                // Calculate new position from 0.0f/1.0f scale
                long lengthBytes = _playerService.CurrentPlaylistItem.LengthBytes;
                var audioFile = _playerService.CurrentPlaylistItem.AudioFile;
                long positionBytes = (long)(positionPercentage * lengthBytes);
                //long positionBytes = (long)Math.Ceiling((double)Playlist.CurrentItem.LengthBytes * (percentage / 100));
                long positionSamples = ConvertAudio.ToPCM(positionBytes, (uint)audioFile.BitsPerSample, audioFile.AudioChannels);
                int positionMS = (int)ConvertAudio.ToMS(positionSamples, (uint)audioFile.SampleRate);
                string positionString = Conversion.MillisecondsToTimeString((ulong)positionMS);
                PlayerPositionEntity entity = new PlayerPositionEntity();
                entity.Position = positionString;
                entity.PositionBytes = positionBytes;
                entity.PositionSamples = (uint)positionSamples;
                return entity;
            }
            catch(Exception ex)
            {
                Console.WriteLine("An error occured while calculating the player position: " + ex.Message);
                View.PlayerError(ex);
            }
            return new PlayerPositionEntity();
        }
        
        public void SetPosition(float percentage)
        {
            try
            {
                // Make sure the percentage isn't 100% or BASS will return an error.               
                double pct = (double)percentage;
                if(pct > 99.9)
                    pct = 99.9;

                Tracing.Log("PlayerPresenter.SetPosition -- Setting position to " + percentage.ToString("0.00") + "%");
                _timerRefreshSongPosition.Stop();
                _playerService.SetPosition(pct);
                _timerRefreshSongPosition.Start();
            }
            catch(Exception ex)
            {
                SetError(ex);
            }
        }

        public void SetVolume(float volume)
        {
            try
            {
                //Tracing.Log("PlayerPresenter.SetVolume -- Setting volume to " + volume.ToString("0.00") + "%");
                _playerService.Volume = volume / 100;
                View.RefreshPlayerVolume(new PlayerVolumeEntity(){ 
                    Volume = volume, 
                    VolumeString = volume.ToString("0") + " %" 
                });
            }
            catch(Exception ex)
            {
                SetError(ex);
            }                
        }

        private void SetError(Exception ex)
        {
            View.PlayerError(ex);
        }
	}
}

