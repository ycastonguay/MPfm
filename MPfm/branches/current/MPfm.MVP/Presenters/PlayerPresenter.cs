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
using MPfm.Core;
using MPfm.Player;
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;
using AutoMapper;

namespace MPfm.MVP
{
	/// <summary>
	/// Player presenter.
	/// </summary>
	public class PlayerPresenter : IPlayerPresenter
	{
		// Private variables
		private IPlayerView view = null;
		private Timer timerRefreshSongPosition = null;

		#region Other Properties

		private MPfm.Player.IPlayer player = null;
		/// <summary>
		/// Returns the current instance of the Player class.
		/// </summary>
		public MPfm.Player.IPlayer Player
		{
			get
			{
				return player;
			}
		}
		
		#endregion

		#region Constructor and Dispose

		/// <summary>
		/// Initializes a new instance of the <see cref="MPfm.UI.PlayerPresenter"/> class.
		/// </summary>
		public PlayerPresenter()
		{	
			// Create update position timer
			timerRefreshSongPosition = new Timer();			
			timerRefreshSongPosition.Interval = 100;
			timerRefreshSongPosition.Elapsed += HandleTimerRefreshSongPositionElapsed;
						
			// Create device
			Device device = new Device();
			device.DriverType = DriverType.DirectSound;
			device.Id = -1;

			// Create player
			player = new MPfm.Player.Player(device, 44100, 100, 10, true);
			player.OnPlaylistIndexChanged += HandlePlayerOnPlaylistIndexChanged;	
		}

		/// <summary>
		/// Releases all resource used by the <see cref="MPfm.UI.MainWindowPresenter"/> object.
		/// </summary>
		/// <remarks>
		/// Call <see cref="Dispose"/> when you are finished using the <see cref="MPfm.UI.MainWindowPresenter"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="MPfm.UI.MainWindowPresenter"/> in an unusable state. After
		/// calling <see cref="Dispose"/>, you must release all references to the <see cref="MPfm.UI.MainWindowPresenter"/>
		/// so the garbage collector can reclaim the memory that the <see cref="MPfm.UI.MainWindowPresenter"/> was occupying.
		/// </remarks>
		public void Dispose()
		{
			// Dispose player
			player.Dispose();
			player = null;
		}

		#endregion
		
		/// <summary>
		/// Binds the view to its implementation.
		/// </summary>
		/// <param name='view'>Player view implementation</param>		
		public void BindView(IPlayerView view)
		{
			// Validate parameters
			if(view == null)			
				throw new ArgumentNullException("The view parameter is null!");			
						
			// Set properties
			this.view = view;	
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
			// Create entity
			PlayerPositionEntity entity = new PlayerPositionEntity();
			entity.PositionBytes = player.GetPosition();
    		entity.PositionSamples = ConvertAudio.ToPCM(entity.PositionBytes, (uint)player.Playlist.CurrentItem.AudioFile.BitsPerSample, 2);
    		entity.PositionMS = (int)ConvertAudio.ToMS(entity.PositionSamples, (uint)player.Playlist.CurrentItem.AudioFile.SampleRate);
    		entity.Position = Conversion.MillisecondsToTimeString((ulong)entity.PositionMS);
			entity.PositionPercentage = ((float)player.GetPosition() / (float)player.Playlist.CurrentItem.LengthBytes) * 100;
			
			// Send changes to view
			view.RefreshPlayerPosition(entity);
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
			RefreshSongInformation(Player.Playlist.CurrentItem.AudioFile);
		}
		
		/// <summary>
		/// Starts playback.
		/// </summary>
		public void Play()
		{
			// Start playback
			player.Play();
	
			// Refresh song information
			RefreshSongInformation(player.Playlist.CurrentItem.AudioFile);
			
			// Start timer
			timerRefreshSongPosition.Start();
		}
		
		/// <summary>
		/// Starts the playback of a new playlist.
		/// </summary>
		/// <param name='audioFiles'>List of audio files</param>		
		public void Play(IEnumerable<AudioFile> audioFiles)
		{
			// Replace playlist
			player.Playlist.Clear();
			player.Playlist.AddItems(audioFiles.ToList());
			
			// Start playback
			Play();
		}

		/// <summary>
		/// Starts the playback of a new playlist.
		/// </summary>
		/// <param name='filePaths'>List of audio file paths</param>
		public void Play(IEnumerable<string> filePaths)
		{
			// Replace playlist
			player.Playlist.Clear();
			player.Playlist.AddItems(filePaths.ToList());
			
			// Start playback
			Play();
		}
		
		/// <summary>
		/// Starts the playback of a new playlist at a specific position.
		/// </summary>
		/// <param name='audioFiles'>List of audio files</param>
		/// <param name='startAudioFilePath'>File path of the first playlist item to play</param>
		public void Play(IEnumerable<AudioFile> audioFiles, string startAudioFilePath)
		{
			// Replace playlist
			player.Playlist.Clear();
			player.Playlist.AddItems(audioFiles.ToList());
			player.Playlist.GoTo(startAudioFilePath);
			
			// Start playback
			Play();
		}
		
		/// <summary>
		/// Stops playback.
		/// </summary>
		public void Stop()
		{
			// Check if the player is playing
			if(player.IsPlaying)
			{
				// Stop timer
				timerRefreshSongPosition.Stop();
				
				// Stop player
				player.Stop();
				
				// Refresh view with empty information
				view.RefreshSongInformation(new SongInformationEntity());
				//view.RefreshPlayerPosition(new PlayerPositionEntity());				
			}
		}
		
		/// <summary>
		/// Pauses playback.
		/// </summary>
		public void Pause()
		{
			// Check if the player is playing
			if(player.IsPlaying)
			{
				// Pause player
				player.Pause();
			}
		}
		
		/// <summary>
		/// Skips to the next song in the playlist.
		/// </summary>
		public void Next()
		{
			// Go to next song
			player.Next();
	
			// Refresh controls
			RefreshSongInformation(player.Playlist.CurrentItem.AudioFile);
		}
		
		/// <summary>
		/// Skips to the previous song in the playlist.
		/// </summary>
		public void Previous()
		{
			// Go to previous song
			player.Previous();
	
			// Refresh controls
			RefreshSongInformation(player.Playlist.CurrentItem.AudioFile);
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
			view.RefreshSongInformation(entity);
		}
        
        public void SetPosition(float percentage)
        {
            // Set position            
            player.SetPosition((double)percentage);
        }

        public void SetVolume(float volume)
        {
            // Set volume and refresh UI
            player.Volume = volume / 100;
            view.RefreshPlayerVolume(new PlayerVolumeEntity(){ 
                Volume = volume, 
                VolumeString = volume.ToString("0") + " %" 
            });
        }

        public void SetTimeShifting(float timeShifting)
        {
            // Convert scale from +50/+150 to -100/+100
            float ratio = (timeShifting - 50) / 100;
            float result = (ratio * 200) - 100;
            
            // Set time shifting and refresh UI
            player.TimeShifting = result;
            view.RefreshPlayerTimeShifting(new PlayerTimeShiftingEntity(){
                TimeShifting = timeShifting,
                TimeShiftingString = timeShifting.ToString("0") + " %"
            });
        }
	}
}

