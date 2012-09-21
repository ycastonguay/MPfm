//
// InitializationService.cs: Service used for creating/updating the database and log files.
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
using System.IO;
using System.Linq;
using System.Diagnostics;
using MPfm.Core;
using System.Reflection;

namespace MPfm.MVP
{	
	/// <summary>
	/// Service used for creating/updating the database and log files.
	/// </summary>
	public class InitializationService : IInitializationService
	{
		// Private variables		
		private Stream fileTracing = null;
        private TextWriterTraceListener textTraceListener = null;
		
        private IAudioFileCacheService audioFileCacheService = null;
        
		#region Constructor and Dispose

		/// <summary>
		/// Initializes a new instance of the <see cref="MPfm.MVP.InitializationService"/> class.
		/// </summary>
		public InitializationService(IAudioFileCacheService audioFileCacheService)
		{
            this.audioFileCacheService = audioFileCacheService;
		}
        
        /// <summary>
        /// Initializes the application (creates configuration, initializes library, cache, etc.).
        /// </summary>
        public void Initialize()
        {
            // Check if the .MPfm directory exists
            string directoryPath = ConfigurationHelper.HomeDirectory;
            if(!Directory.Exists(directoryPath))
            {
                // Create directory
                Directory.CreateDirectory(directoryPath);
            }

            // Create trace listener and start logging
            CreateTraceListener();
            Tracing.Log("====================================================================");
            Tracing.Log("MPfm: Music Player for Musicians - " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + " ALPHA");
            Tracing.Log("Started on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString());
            Tracing.Log("InitializationService.Initialize -- Starting initialization...");   

            // Create configuration
            CreateConfiguration();
            
            // Create library
            CreateLibrary();
            
            // Refresh cache
            audioFileCacheService.RefreshCache();
        }

        /// <summary>
        /// Creates the trace listener.
        /// </summary>
        private void CreateTraceListener()
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
        }
		
		/// <summary>
		/// Creates the configuration.
		/// </summary>
		private void CreateConfiguration()
        {
            // Check for configuration file
            Tracing.Log("InitializationService.CreateConfiguration -- Checking for configuration file...");
            if (File.Exists(ConfigurationHelper.ConfigurationFilePath))
            {
                // Load configuration file
                MPfmConfig.Instance.Load();
            }
            //ConfigurationHelper.Save(ConfigurationHelper.ConfigurationFilePath, MPfmConfig.Instance);
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
                Tracing.Log("InitializationService.CreateLibrary -- Checking if the database file exists...");
                if (!File.Exists(ConfigurationHelper.DatabaseFilePath))
                {                    
                    // Create database file
                    Tracing.Log("InitializationService.CreateLibrary -- Creating new database file...");
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
                Tracing.Log("InitializationService.CreateLibrary -- Database version is " + databaseVersion + ". Checking if the database version needs to be updated...");
                MPfm.Library.Library.CheckIfDatabaseVersionNeedsToBeUpdated(ConfigurationHelper.DatabaseFilePath);
            }
            catch (Exception ex)
            {
                throw new Exception("Error initializing MPfm: The MPfm database could not be updated!", ex);
            }			
		}

		#endregion
		
	}
	
}

