//
// PlayerPresenter.cs: Player presenter.
//
// Copyright © 2011-2012 Yanick Castonguay
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Timers;
using AutoMapper;
using TinyMessenger;
using MPfm.Core;
using MPfm.Player;
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;

namespace MPfm.MVP
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

		#region Constructor and Dispose

		/// <summary>
		/// Initializes a new instance of the <see cref="MPfm.UI.PlayerPresenter"/> class.
		/// </summary>
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
            playerService.Initialize(device, 44100, 5000, 100);
            playerService.Player.OnPlaylistIndexChanged += HandlePlayerOnPlaylistIndexChanged;

            // Subscribe to events
            messageHub.Subscribe<LibraryBrowserItemDoubleClickedMessage>((LibraryBrowserItemDoubleClickedMessage m) => {
                Play(audioFileCacheService.SelectAudioFiles(m.Query));
            });
            messageHub.Subscribe<SongBrowserItemDoubleClickedMessage>((SongBrowserItemDoubleClickedMessage m) => {
                Play(audioFileCacheService.SelectAudioFiles(m.Query), m.Item.FilePath);
            });
        }

		#endregion
		
        public void Dispose()
        {
            playerService.Dispose();
        }
        
		/// <summary>
		/// Handles the timer update position elapsed.
		/// </summary>
		/// <param name='sender'>
		/// Sender.
		/// </param>
		/// <param name='e'>
		/// E.
		/// </param>
		void HandleTimerRefreshSongPositionElapsed(object sender, ElapsedEventArgs e)
		{
            // Check player
            if(playerService.Player.IsSettingPosition)
                return;

			// Create entity
			PlayerPositionEntity entity = new PlayerPositionEntity();
            entity.PositionBytes = playerService.Player.GetPosition();
            entity.PositionSamples = ConvertAudio.ToPCM(entity.PositionBytes, (uint)playerService.Player.Playlist.CurrentItem.AudioFile.BitsPerSample, 2);
            entity.PositionMS = (int)ConvertAudio.ToMS(entity.PositionSamples, (uint)playerService.Player.Playlist.CurrentItem.AudioFile.SampleRate);
    		entity.Position = Conversion.MillisecondsToTimeString((ulong)entity.PositionMS);
            entity.PositionPercentage = ((float)playerService.Player.GetPosition() / (float)playerService.Player.Playlist.CurrentItem.LengthBytes) * 100;
			
			// Send changes to view
			View.RefreshPlayerPosition(entity);
		}
		
		/// <summary>
		/// Handles the player playlist index changed event.
		/// </summary>
		/// <param name='data'>
		/// Playlist index changed data.
		/// </param>
		protected void HandlePlayerOnPlaylistIndexChanged(PlayerPlaylistIndexChangedData data)
		{
			// Refresh song information
            RefreshSongInformation(playerService.Player.Playlist.CurrentItem.AudioFile);
		}
		
		/// <summary>
		/// Starts playback.
		/// </summary>
		public void Play()
		{            
            try
            {
    			// Start playback
                Tracing.Log("PlayerPresenter.Play -- Starting playback...");
                playerService.Player.Play();
    	
    			// Refresh song information
    			Tracing.Log("PlayerPresenter.Play -- Refreshing song information...");
                RefreshSongInformation(playerService.Player.Playlist.CurrentItem.AudioFile);
    			
    			// Start timer
                Tracing.Log("PlayerPresenter.Play -- Starting timer...");
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
    			// Replace playlist
                Tracing.Log("PlayerPresenter.Play(IEnumerable<AudioFile>) -- Clearing playlist...");
                playerService.Player.Playlist.Clear();
                Tracing.Log("PlayerPresenter.Play(IEnumerable<AudioFile>) -- Adding items...");
                playerService.Player.Playlist.AddItems(audioFiles.ToList());
    			
    			// Start playback
                Tracing.Log("PlayerPresenter.Play(IEnumerable<AudioFile>) -- Starting playback...");
    			Play();
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
    			// Replace playlist
                Tracing.Log("PlayerPresenter.Play(IEnumerable<string>) -- Clearing playlist...");
                playerService.Player.Playlist.Clear();
                Tracing.Log("PlayerPresenter.Play(IEnumerable<string>) -- Adding items...");
                playerService.Player.Playlist.AddItems(filePaths.ToList());
    			
    			// Start playback
                Tracing.Log("PlayerPresenter.Play(IEnumerable<string>) -- Starting playback...");
    			Play();
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
    			// Replace playlist
                Tracing.Log("PlayerPresenter.Play(IEnumerable<AudioFile>, string) -- Clearing playlist...");
                playerService.Player.Playlist.Clear();
                Tracing.Log("PlayerPresenter.Play(IEnumerable<AudioFile>, string) -- Adding items...");
                if(audioFiles == null)
                {
                    Tracing.Log("PlayerPresenter.Play(IEnumerable<AudioFile>, string) -- Adding items: audioFiles == null");
                    List<AudioFile> listAudioFiles = audioFiles.ToList();
                    playerService.Player.Playlist.AddItems(listAudioFiles); // simulate bug
                }
                else
                {
                    Tracing.Log("PlayerPresenter.Play(IEnumerable<AudioFile>, string) -- Adding items...");
                    List<AudioFile> listAudioFiles = audioFiles.ToList();
                    Tracing.Log("PlayerPresenter.Play(IEnumerable<AudioFile>, string) -- Adding items (count = " + listAudioFiles.Count + "...");
                    playerService.Player.Playlist.AddItems(listAudioFiles);
                }
                Tracing.Log("PlayerPresenter.Play(IEnumerable<AudioFile>, string) -- Skipping to item " + startAudioFilePath + " in playlist...");
                playerService.Player.Playlist.GoTo(startAudioFilePath);
    			
    			// Start playback
                Tracing.Log("PlayerPresenter.Play(IEnumerable<AudioFile>, string) -- Starting playback...");
    			Play();
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
    			// Check if the player is playing
                if(playerService.Player.IsPlaying)
    			{
    				// Stop timer
                    Tracing.Log("PlayerPresenter.Stop -- Stopping timer...");
    				timerRefreshSongPosition.Stop();
    				
    				// Stop player
                    Tracing.Log("PlayerPresenter.Stop -- Stopping playback...");
                    playerService.Player.Stop();
    				
    				// Refresh view with empty information
                    Tracing.Log("PlayerPresenter.Stop -- Refresh song information and position with empty entity...");
                    View.RefreshSongInformation(new SongInformationEntity());
                    View.RefreshPlayerPosition(new PlayerPositionEntity());
    			}
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
    			// Check if the player is playing
                if(playerService.Player.IsPlaying)
    			{
    				// Pause player
                    Tracing.Log("PlayerPresenter.Stop -- Pausing playback...");
                    playerService.Player.Pause();
    			}
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
                playerService.Player.Next();
    	
    			// Refresh controls
                Tracing.Log("PlayerPresenter.Next -- Refreshing song information...");
                RefreshSongInformation(playerService.Player.Playlist.CurrentItem.AudioFile);
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
                playerService.Player.Previous();
    	
    			// Refresh controls
                RefreshSongInformation(playerService.Player.Playlist.CurrentItem.AudioFile);
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
		
		/// <summary>
		/// Refreshes the song information on the main view.
		/// </summary>
		/// <param name='audioFile'>
		/// Audio file.
		/// </param>
		private void RefreshSongInformation(AudioFile audioFile)
		{			
			// Map entity
			SongInformationEntity entity = Mapper.Map<AudioFile, SongInformationEntity>(audioFile);
			if(audioFile != null)
			{
				entity.BitsPerSampleString = entity.BitsPerSample.ToString() + " bits";
				entity.SampleRateString = entity.SampleRate.ToString() + " Hz";
				entity.BitrateString = entity.Bitrate.ToString() + " kbps";
				if(entity.FileType != AudioFileFormat.All)					
					entity.FileTypeString = entity.FileType.ToString();								
			}			
			
			// Update view
            View.RefreshSongInformation(entity);
		}
        
        public void SetPosition(float percentage)
        {
            try
            {
                // Set position
                Tracing.Log("PlayerPresenter.SetPosition -- Setting position to " + percentage.ToString("0.00") + "%");
                timerRefreshSongPosition.Stop();
                playerService.Player.SetPosition((double)percentage);
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
                playerService.Player.Volume = volume / 100;
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
                playerService.Player.TimeShifting = result;
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

