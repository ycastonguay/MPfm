//  
//  MyClass.cs
//  
//  Author:
//       Yanick Castonguay <ycastonguay@mp4m.org>
// 
//  Copyright (c) 2012 2011 - 2012 Yanick Castonguay
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
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
	/// Main window controller.
	/// </summary>
	public class MainWindowController : IDisposable
	{
		// Private variables
		private OSType osType = OSType.Windows;
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

		#endregion

		#region Constructor and Dispose

		/// <summary>
		/// Initializes a new instance of the <see cref="MPfm.UI.MainWindowController"/> class.
		/// </summary>
		public MainWindowController()
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
			if(OS.Type == OSType.Windows)
			{
			}
			else if(OS.Type == OSType.Linux)
			{
				homeDirectory = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), ".MPfm");
			}
			else if(OS.Type == OSType.MacOSX)
			{
				// TODO: Check if this directory is OK for future Mac App Store 
				homeDirectory = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), ".MPfm");
			}

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

