//
// MainWindowPresenter.cs: Main window presenter.
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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using MPfm.Core;
using MPfm.Player;
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;

namespace MPfm.UI
{
	/// <summary>
	/// Main window presenter.
	/// </summary>
	public class MainWindowPresenter : IDisposable
	{
		// Private variables		
		private Stream fileTracing = null;
        private TextWriterTraceListener textTraceListener = null;

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
		/// Initializes a new instance of the <see cref="MPfm.UI.MainWindowPresenter"/> class.
		/// </summary>
		public MainWindowPresenter()
		{
			// Create configuration
			CreateConfiguration();
		}

		/// <summary>
		/// Releases all resource used by the <see cref="MPfm.UI.MainWindowController"/> object.
		/// </summary>
		/// <remarks>
		/// Call <see cref="Dispose"/> when you are finished using the <see cref="MPfm.UI.MainWindowController"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="MPfm.UI.MainWindowController"/> in an unusable state. After
		/// calling <see cref="Dispose"/>, you must release all references to the <see cref="MPfm.UI.MainWindowController"/>
		/// so the garbage collector can reclaim the memory that the <see cref="MPfm.UI.MainWindowController"/> was occupying.
		/// </remarks>
		public void Dispose()
		{
			// Dispose player
			player.Dispose();
			player = null;
		}

		#endregion

		/// <summary>
		/// Creates the configuration.
		/// </summary>
		public void CreateConfiguration()
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
					//throw new Exception("Could not create the application home directory (" + homeDirectory + ")!", ex);
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

		public void CreatePlayer()
		{
			// Create device
			Device device = new Device();
			device.DriverType = DriverType.DirectSound;
			device.Id = -1;

			// Create player
			player = new MPfm.Player.Player(device, 44100, 100, 10, true);
			//player.OnPlaylistIndexChanged += HandlePlayerOnPlaylistIndexChanged;
		}
		
		public void CreateLibrary()
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

		protected void HandleLibraryOnUpdateLibraryProgress (MPfm.Library.OldUpdateLibraryProgressData data)
		{
			
		}

		protected void HandleLibraryOnUpdateLibraryFinished (MPfm.Library.UpdateLibraryFinishedData data)
		{
			
		}

		/// <summary>
		/// Returns the current player position, if playing.
		/// </summary>
		/// <returns>Entity containing the player position in different formats.</returns>
		public PlayerPositionEntity GetPlayerPosition()
		{
			// Create entity
			PlayerPositionEntity entity = new PlayerPositionEntity();
			entity.PositionBytes = player.GetPosition();
    		entity.PositionSamples = ConvertAudio.ToPCM(entity.PositionBytes, (uint)player.Playlist.CurrentItem.AudioFile.BitsPerSample, 2);
    		entity.PositionMS = (int)ConvertAudio.ToMS(entity.PositionSamples, (uint)player.Playlist.CurrentItem.AudioFile.SampleRate);
    		entity.Position = Conversion.MillisecondsToTimeString((ulong)entity.PositionMS);

			return entity;
		}
	}
}

