//
// MainPresenter.cs: Main window presenter.
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
	/// Main window presenter.
	/// </summary>
	public class MainPresenter : IDisposable, IMainPresenter
	{
		// Private variables		
		private Stream fileTracing = null;
        private TextWriterTraceListener textTraceListener = null;
		private readonly IMainView view = null;
		private Timer timerRefreshSongPosition = null;

		#region Directories and File Paths

		private string assemblyDirectory = string.Empty;
		/// <summary>
		/// Returns the current assembly directory.
		/// </summary>
		public string AssemblyDirectory
		{
			get
			{
				return assemblyDirectory;
			}
		}

		private string homeDirectory = string.Empty;
		/// <summary>
		/// Returns the current user home directory.
		/// </summary>
		public string HomeDirectory
		{
			get
			{
				return homeDirectory;
			}
		}

		private string configurationFilePath = string.Empty;
		/// <summary>
		/// Returns the current user configuration file path.
		/// </summary>
		public string ConfigurationFilePath
		{
			get
			{
				return configurationFilePath;
			}
		}

		private string databaseFilePath = string.Empty;
		/// <summary>
		/// Returns the current user database file path.
		/// </summary>
		public string DatabaseFilePath
		{
			get
			{
				return databaseFilePath;
			}
		}

		private string logFilePath = string.Empty;
		/// <summary>
		/// Returns the current user log file path.
		/// </summary>
		public string LogFilePath
		{
			get
			{
				return logFilePath;
			}
		}

		#endregion

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
		
		private MPfm.Library.Library library = null;
		/// <summary>
		/// Returns the current instance of the Library class.
		/// </summary>
		public MPfm.Library.Library Library
		{
			get
			{
				return library;
			}
		}

		#endregion

		#region Constructor and Dispose

		/// <summary>
		/// Initializes a new instance of the <see cref="MPfm.UI.MainPresenter"/> class.
		/// </summary>
		public MainPresenter(IMainView view)
		{
			// Validate parameters
			if(view == null)
			{
				throw new ArgumentNullException("The view parameter is null!");
			}
						
			// Set properties
			this.view = view;					
			
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
			// Get assembly directory
			string assemblyDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

			// Get home directory depending on platform
			string homeDirectory = string.Empty;
			
#if (MACOSX)
        	homeDirectory = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), ".MPfm");
#elif (LINUX)
			homeDirectory = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), ".MPfm");
#elif (!MACOSX && !LINUX)						
			homeDirectory = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), ".MPfm");
#endif

			// Check if the MPfm home directory exists
			if(!Directory.Exists(homeDirectory))
			{
				try
				{
					// Create directory
					Directory.CreateDirectory(homeDirectory);
				}
				catch(Exception ex)
				{
					throw new Exception("Could not create the application home directory (" + homeDirectory + ")!", ex);
				}
			}

			// Generate file paths
			configurationFilePath = Path.Combine(homeDirectory, "MPfm.Configuration.xml");
			databaseFilePath = Path.Combine(homeDirectory, "MPfm.Database.db");
			logFilePath = Path.Combine(homeDirectory, "MPfm.Log.txt");

            // Check if trace file exists
            if (!File.Exists(logFilePath))
            {
                // Create file
                fileTracing = File.Create(logFilePath);
            }
            else
            {
                // Open file
                fileTracing = File.Open(logFilePath, FileMode.Append);
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
                if (!File.Exists(databaseFilePath))
                {                    
                    // Create database file
                    //frmSplash.SetStatus("Creating database file...");
                    MPfm.Library.Library.CreateDatabaseFile(databaseFilePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error initializing MPfm: Could not create database file!", ex);
            }
			
            try
            {
                // Check current database version
                string databaseVersion = MPfm.Library.Library.GetDatabaseVersion(databaseFilePath);

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
                MPfm.Library.Library.CheckIfDatabaseVersionNeedsToBeUpdated(databaseFilePath);
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
                library = new Library.Library(databaseFilePath);
				library.OnUpdateLibraryFinished += HandleLibraryOnUpdateLibraryFinished;
				library.OnUpdateLibraryProgress += HandleLibraryOnUpdateLibraryProgress;
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
		/// Adds audio files to the library.
		/// </summary>
		/// <param name='filePaths'>
		/// List of file paths.
		/// </param>
		public void AddFilesToLibrary(List<string> filePaths)
		{
		}
		
		/// <summary>
		/// Adds a folder to the library.
		/// </summary>
		/// <param name='folderPath'>
		/// Folder path.
		/// </param>
		public void AddFolderToLibrary(string folderPath)
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

