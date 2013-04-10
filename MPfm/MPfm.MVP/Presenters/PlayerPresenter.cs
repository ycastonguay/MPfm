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
using MPfm.MVP.Messages;
using MPfm.MVP.Models;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Services.Interfaces;
using MPfm.MVP.Views;
using MPfm.Player.Events;
using MPfm.Sound.AudioFiles;
using MPfm.Core;
using TinyMessenger;
using MPfm.Sound.BassNetWrapper;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Player presenter.
	/// </summary>
	public class PlayerPresenter : BasePresenter<IPlayerView>, IPlayerPresenter
	{
		// Private variables
		//IPlayerView view;
		Timer timerRefreshSongPosition;
        readonly IPlayerService playerService;
        readonly IAudioFileCacheService audioFileCacheService;
        readonly ITinyMessengerHub messageHub;

		public PlayerPresenter(ITinyMessengerHub messageHub, IPlayerService playerService, IAudioFileCacheService audioFileCacheService)
		{	
            // Set properties
            this.messageHub = messageHub;
            this.playerService = playerService;
            this.audioFileCacheService = audioFileCacheService;

			// Create update position timer
			timerRefreshSongPosition = new Timer();			
			timerRefreshSongPosition.Interval = 100;
			timerRefreshSongPosition.Elapsed += HandleTimerRefreshSongPositionElapsed;

            // Initialize player
            Device device = new Device(){
                DriverType = DriverType.DirectSound,
                Id = -1
            };
            playerService.Initialize(device, 44100, 1000, 100);
            //playerService.OnPlaylistIndexChanged += HandlePlayerOnPlaylistIndexChanged;

            // Subscribe to events
            messageHub.Subscribe<LibraryBrowserItemDoubleClickedMessage>((LibraryBrowserItemDoubleClickedMessage m) => {
                Play(audioFileCacheService.SelectAudioFiles(m.Query));
            });
            messageHub.Subscribe<SongBrowserItemDoubleClickedMessage>((SongBrowserItemDoubleClickedMessage m) =>
            {
                string filePath = m != null ? m.Item.FilePath : null;
                Play(audioFileCacheService.SelectAudioFiles(m.Query), filePath);
            });
            messageHub.Subscribe<MobileLibraryBrowserItemClickedMessage>((MobileLibraryBrowserItemClickedMessage m) =>
            {
                Play(audioFileCacheService.SelectAudioFiles(m.Query), m.FilePath);
            });
            messageHub.Subscribe<PlayerPlaylistIndexChangedMessage>((PlayerPlaylistIndexChangedMessage m) =>
            {
                View.RefreshSongInformation(m.Data.AudioFileStarted, playerService.CurrentPlaylistItem.LengthBytes);
            });
            messageHub.Subscribe<PlayerStatusMessage>((PlayerStatusMessage m) =>
            {
                View.RefreshPlayerStatus(m.Status);
            });
        }
        
        public void Dispose()
        {
            playerService.Dispose();
        }
        
        public override void BindView(IPlayerView view)
        {
            base.BindView(view);
            
            view.OnPlayerPlay = Play;
            view.OnPlayerPause = Pause;
            view.OnPlayerStop = Stop;
            view.OnPlayerPrevious = Previous;
            view.OnPlayerNext = Next;
            //view.OnPlayerSetPitchShifting = (float) => { 
            view.OnPlayerSetPosition = SetPosition;
            view.OnPlayerSetTimeShifting = SetTimeShifting;
            view.OnPlayerSetVolume = SetVolume;
            view.OnPlayerRequestPosition = RequestPosition;
        }

		void HandleTimerRefreshSongPositionElapsed(object sender, ElapsedEventArgs e)
		{
            // Check player
            if(playerService.IsSettingPosition)
                return;

            //int available = playerService.GetDataAvailable();
            
			// Create entity
			PlayerPositionEntity entity = new PlayerPositionEntity();
            entity.PositionBytes = playerService.GetPosition();
            entity.PositionSamples = ConvertAudio.ToPCM(entity.PositionBytes, (uint)playerService.CurrentPlaylistItem.AudioFile.BitsPerSample, 2);
            entity.PositionMS = (int)ConvertAudio.ToMS(entity.PositionSamples, (uint)playerService.CurrentPlaylistItem.AudioFile.SampleRate);
    		//entity.Position = available.ToString() + " " + Conversion.MillisecondsToTimeString((ulong)entity.PositionMS);
            entity.Position = Conversion.MillisecondsToTimeString((ulong)entity.PositionMS);
            entity.PositionPercentage = ((float)playerService.GetPosition() / (float)playerService.CurrentPlaylistItem.LengthBytes) * 100;

			// Send changes to view
			View.RefreshPlayerPosition(entity);
		}

        private PlayerPositionEntity RequestPosition(float positionPercentage)
        {
            try
            {
                // Calculate new position from 0.0f/1.0f scale
                long lengthBytes = playerService.CurrentPlaylistItem.LengthBytes;
                var audioFile = playerService.CurrentPlaylistItem.AudioFile;
                long positionBytes = (long)(positionPercentage * lengthBytes);
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
		
		/// <summary>
		/// Starts playback.
		/// </summary>
		public void Play()
		{            
            try
            {
                playerService.Play();
                //RefreshSongInformation(playerService.CurrentPlaylistItem.AudioFile);
    			timerRefreshSongPosition.Start();
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
                playerService.Play(audioFiles);
                //RefreshSongInformation(playerService.CurrentPlaylistItem.AudioFile);
                timerRefreshSongPosition.Start();
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
                playerService.Play(filePaths);
                //RefreshSongInformation(playerService.CurrentPlaylistItem.AudioFile);
                timerRefreshSongPosition.Start();
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
                playerService.Play(audioFiles, startAudioFilePath);
                //RefreshSongInformation(playerService.CurrentPlaylistItem.AudioFile);
                timerRefreshSongPosition.Start();
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
				timerRefreshSongPosition.Stop();
				
				// Stop player
                Tracing.Log("PlayerPresenter.Stop -- Stopping playback...");
                playerService.Stop();
				
				// Refresh view with empty information
                Tracing.Log("PlayerPresenter.Stop -- Refresh song information and position with empty entity...");
			    View.RefreshSongInformation(null, 0);
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
                playerService.Pause();
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
    			// Go to next song
                Tracing.Log("PlayerPresenter.Next -- Skipping to next item in playlist...");
                playerService.Next();
    	
    			// Refresh controls
                Tracing.Log("PlayerPresenter.Next -- Refreshing song information...");
                //RefreshSongInformation(playerService.CurrentPlaylistItem.AudioFile);
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
    			// Go to previous song
                Tracing.Log("PlayerPresenter.Previous -- Skipping to previous item in playlist...");
                playerService.Previous();
    	
    			// Refresh controls
                //RefreshSongInformation(playerService.CurrentPlaylistItem.AudioFile);
                Tracing.Log("PlayerPresenter.Previous -- Refreshing song information...");
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
        
        public void SetPosition(float percentage)
        {
            try
            {
                // Make sure the percentage isn't 100% or BASS will return an error.               
                double pct = (double)percentage;
                if(pct > 99.9)
                    pct = 99.9;

                Tracing.Log("PlayerPresenter.SetPosition -- Setting position to " + percentage.ToString("0.00") + "%");
                timerRefreshSongPosition.Stop();
                playerService.SetPosition(pct);
                timerRefreshSongPosition.Start();
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
                // Set volume and refresh UI
                Tracing.Log("PlayerPresenter.SetVolume -- Setting volume to " + volume.ToString("0.00") + "%");
                playerService.SetVolume(volume / 100);
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

        public void SetTimeShifting(float timeShifting)
        {
            try
            {
                // Convert scale from +50/+150 to -100/+100
                float ratio = (timeShifting - 50) / 100;
                float result = (ratio * 200) - 100;
                
                // Set time shifting and refresh UI
                Tracing.Log("PlayerPresenter.SetTimeShifting -- Setting time shifting to " + timeShifting.ToString("0.00") + "%");
                playerService.SetTimeShifting(result);
                View.RefreshPlayerTimeShifting(new PlayerTimeShiftingEntity(){
                    TimeShifting = timeShifting,
                    TimeShiftingString = timeShifting.ToString("0") + " %"
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

