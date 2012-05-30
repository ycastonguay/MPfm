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
		private Stream fileTracing = null;
        private TextWriterTraceListener textTraceListener = null;
		private IPlayerView view = null;
		private Timer timerRefreshSongPosition = null;

		#region Other Properties

		private MPfm.Player.Player player = null;
		/// <summary>
		/// Returns the current instance of the Player class.
		/// </summary>
		public MPfm.Player.Player Player
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
			
			// Initialize components
			CreateConfiguration();
			CreatePlayer();
			CreateLibrary();
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
		
		protected void HandleLibraryOnUpdateLibraryProgress (MPfm.Library.UpdateLibraryProgressData data)
		{
			
		}

		protected void HandleLibraryOnUpdateLibraryFinished (MPfm.Library.UpdateLibraryFinishedData data)
		{
			
		}

		/// <summary>
		/// Creates the configuration.
		/// </summary>
		private void CreateConfiguration()
		{
            // Check if trace file exists
            if (!File.Exists(ConfigurationHelper.LogFilePath))
            {
                // Create file
                fileTracing = File.Create(ConfigurationHelper.LogFilePath);
            }
            else
            {
                // Open file
                fileTracing = File.Open(ConfigurationHelper.LogFilePath, FileMode.Append);
            }
            textTraceListener = new TextWriterTraceListener(fileTracing);
            Trace.Listeners.Add(textTraceListener);

			// Check for configuration file
		}
		
		/// <summary>
		/// Creates and initializes the player.
		/// </summary>
		private void CreatePlayer()
		{
			// Create device
			Device device = new Device();
			device.DriverType = DriverType.DirectSound;
			device.Id = -1;

			// Create player
			player = new MPfm.Player.Player(device, 44100, 100, 10, true);
			player.OnPlaylistIndexChanged += HandlePlayerOnPlaylistIndexChanged;
		}
		
		/// <summary>
		/// Creates and initializes the library.
		/// </summary>
		/// <exception cref='Exception'>
		/// Represents errors that occur during application execution.
		/// </exception>
		private void CreateLibrary()
		{
            try
            {
                // Check if the database file exists
                if (!File.Exists(ConfigurationHelper.DatabaseFilePath))
                {                    
                    // Create database file
                    //frmSplash.SetStatus("Creating database file...");
                    MPfm.Library.Library.CreateDatabaseFile(ConfigurationHelper.DatabaseFilePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error initializing MPfm: Could not create database file!", ex);
            }
			
            try
            {
                // Check current database version
                string databaseVersion = MPfm.Library.Library.GetDatabaseVersion(ConfigurationHelper.DatabaseFilePath);

                // Extract major/minor
                string[] currentVersionSplit = databaseVersion.Split('.');

                // Check integrity of the setting value (should be split in 2)
                if (currentVersionSplit.Length != 2)
                {
                    throw new Exception("Error fetching database version; the setting value is invalid!");
                }

                int currentMajor = 0;
                int currentMinor = 0;
                int.TryParse(currentVersionSplit[0], out currentMajor);
                int.TryParse(currentVersionSplit[1], out currentMinor);

                // Is this earlier than 1.04?
                if (currentMajor == 1 && currentMinor < 4)
                {
                    // Set buffer size
                    //Config.Audio.Mixer.BufferSize = 1000;
                }

                // Check if the database needs to be updated
                MPfm.Library.Library.CheckIfDatabaseVersionNeedsToBeUpdated(ConfigurationHelper.DatabaseFilePath);
            }
            catch (Exception ex)
            {
                throw new Exception("Error initializing MPfm: The MPfm database could not be updated!", ex);
            }
			
		    try
            {
                // Load library
                Tracing.Log("Main form init -- Loading library...");
                //frmSplash.SetStatus("Loading library...");                
                //library = new Library.Library(databaseFilePath);
				//library.OnUpdateLibraryFinished += HandleLibraryOnUpdateLibraryFinished;
				//library.OnUpdateLibraryProgress += HandleLibraryOnUpdateLibraryProgress;
            }
            catch
            {
				throw;
                // Set error in splash and hide splash
                //frmSplash.SetStatus("Error initializing library!");
                //frmSplash.HideSplash();

                // Display message box with error
                //this.TopMost = true;
                //MessageBox.Show("There was an error while initializing the library.\nYou can delete the MPfm.Database.db file in the MPfm application data folder (" + applicationDataFolderPath + ") to reset the library.\n\nException information:\nMessage: " + ex.Message + "\nStack trace: " + ex.StackTrace, "Error initializing library!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //Tracing.Log("Main form init -- Library init error: " + ex.Message + "\nStack trace: " + ex.StackTrace);
                
                // Exit application
                //Application.Exit();
                //return;
            }
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
				
				// Refresh song information
				RefreshSongInformation(null);
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
			
			// Update view
			view.RefreshSongInformation(entity);
		}			
	}
}

