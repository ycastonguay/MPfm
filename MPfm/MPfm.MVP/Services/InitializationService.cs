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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using MPfm.Core;
using MPfm.MVP.Config;
using MPfm.MVP.Helpers;
using MPfm.MVP.Services.Interfaces;
using MPfm.Library.Services.Interfaces;

namespace MPfm.MVP.Services
{	
	/// <summary>
	/// Service used for creating/updating the database and log files.
	/// </summary>
	public class InitializationService : IInitializationService
	{
		// Private variables		
		private Stream _fileTracing;
        private IAudioFileCacheService _audioFileCacheService;
        private ISyncListenerService _syncListenerService;

#if (!IOS && !ANDROID)
        private TextWriterTraceListener textTraceListener = null;
#endif
        
		public InitializationService(IAudioFileCacheService audioFileCacheService, ISyncListenerService syncListenerService)
		{
            _audioFileCacheService = audioFileCacheService;
            _syncListenerService = syncListenerService;
		}
        
        /// <summary>
        /// Initializes the application (creates configuration, initializes library, cache, etc.).
        /// </summary>
        public void Initialize()
        {
            // Create missing directories
            if(!Directory.Exists(ConfigurationHelper.HomeDirectory))
                Directory.CreateDirectory(ConfigurationHelper.HomeDirectory);
            if (!Directory.Exists(ConfigurationHelper.PeakFileDirectory))
                Directory.CreateDirectory(ConfigurationHelper.PeakFileDirectory);

            CreateTraceListener();
            Tracing.Log("====================================================================");

#if !IOS && !ANDROID // MonoDroid doesn't like Assembly methods
            Tracing.Log("Sessions - " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + " ALPHA");
#endif

            Tracing.Log("Started on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToLongTimeString());

            // Load data needed to start the application
            LoadConfiguration();
            LoadLibrary();
            _audioFileCacheService.RefreshCache();
        }

        void CreateTraceListener()
        {
#if (!IOS && !ANDROID)
            // Check if trace file exists
            if (!File.Exists(ConfigurationHelper.LogFilePath))
                _fileTracing = File.Create(ConfigurationHelper.LogFilePath);
            else
                _fileTracing = File.Open(ConfigurationHelper.LogFilePath, FileMode.Append);
            textTraceListener = new TextWriterTraceListener(_fileTracing);
            Trace.Listeners.Add(textTraceListener);
#endif
        }
		
		void LoadConfiguration()
        {
            // Check for configuration file
            Tracing.Log("InitializationService.CreateConfiguration -- Checking for configuration file...");
            if (File.Exists(ConfigurationHelper.ConfigurationFilePath))
                MPfmConfig.Instance.Load();

            //ConfigurationHelper.Save(ConfigurationHelper.ConfigurationFilePath, MPfmConfig.Instance);
            //EQPreset preset = EQPresetHelper.Load("/Users/animal/Documents/test.txt");
            //EQPresetHelper.Save("/Users/animal/Documents/test.txt", new EQPreset());
		}
		
		void LoadLibrary()
		{
            try
            {
                // Check if the database file exists
                Tracing.Log(string.Format("InitializationService.CreateLibrary -- Checking if the database file exists ({0})...", ConfigurationHelper.DatabaseFilePath));
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
                string databaseVersion = MPfm.Library.Library.GetDatabaseVersion(ConfigurationHelper.DatabaseFilePath);
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

            try
            {
                _syncListenerService.Start();
            }
            catch (Exception ex)
            {
                throw new Exception("Error initializing MPfm: The sync listener could not be started!", ex);
            }       
		}
	}
}
