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
using System.Threading.Tasks;
using MPfm.Library;
using MPfm.Library.Database;
using MPfm.Library.Objects;
using MPfm.MVP.Config;
using MPfm.MVP.Services.Interfaces;
using MPfm.Library.Services.Interfaces;
#if WINDOWSSTORE
using Windows.Storage;
using MPfm.Core.WinRT;
#endif
using Sessions.Core;
using Sessions.Core.Helpers;

namespace MPfm.MVP.Services
{	
	/// <summary>
	/// Service used for creating/updating the database and log files.
	/// </summary>
	public class InitializationService : IInitializationService
	{
		private Stream _fileTracing;
        private IAudioFileCacheService _audioFileCacheService;
        private ISyncListenerService _syncListenerService;

#if (!IOS && !ANDROID && !WINDOWSSTORE && !WINDOWS_PHONE)
        private TextWriterTraceListener textTraceListener = null;
#endif

	    /// <summary>
	    /// Indicates what database major version is expected. Useful to update the database structure.
	    /// </summary>
	    private int _databaseVersionMajor = 1;

	    /// <summary>
	    /// Indicates what database minor version is expected. Useful to update the database structure.
	    /// </summary>
	    private int _databaseVersionMinor = 7;

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
            // Maybe create different implementations per platform?
            CreateDirectories();
            CreateTraceListener();

            Tracing.Log("====================================================================");
            Tracing.Log(string.Format("Sessions - Started on {0}", DateTime.Now));

            LoadDatabase();
            StartSyncService();
            RefreshAudioFileCache();
        }

	    private void CreateTraceListener()
        {            
#if (!IOS && !ANDROID && !WINDOWSSTORE && !WINDOWS_PHONE)
            // Check if trace file exists
            if (!File.Exists(PathHelper.LogFilePath))
                _fileTracing = File.Create(PathHelper.LogFilePath);
            else
                _fileTracing = File.Open(PathHelper.LogFilePath, FileMode.Append);
            textTraceListener = new TextWriterTraceListener(_fileTracing);
            Trace.Listeners.Add(textTraceListener);
#endif
        }

	    public static void CreateDirectories()
        {
#if WINDOWSSTORE
            //var task = ApplicationData.Current.LocalFolder.CreateFolderAsync("PeakFiles", CreationCollisionOption.OpenIfExists);
            //var storageFolder = task.GetResults();
#elif WINDOWS_PHONE
            //
#else
            // Create missing directories
            if(!Directory.Exists(PathHelper.HomeDirectory))
                Directory.CreateDirectory(PathHelper.HomeDirectory);
            if (!Directory.Exists(PathHelper.PeakFileDirectory))
                Directory.CreateDirectory(PathHelper.PeakFileDirectory);
#endif
        }

	    private void StartSyncService()
	    {
            try
            {
                _syncListenerService.SetPort(AppConfigManager.Instance.Root.Library.SyncServicePort);
                if(AppConfigManager.Instance.Root.Library.IsSyncServiceEnabled)
                    _syncListenerService.Start();
            }
            catch (Exception ex)
            {
                //throw new Exception("Error initializing MPfm: The sync listener could not be started!", ex);
                Tracing.Log("The sync listener could not be started! Exception: {0}", ex);
            }   
	    }

        private void RefreshAudioFileCache()
        {
            _audioFileCacheService.RefreshCache();
        }

	    private void LoadDatabase()
		{
            try
            {
#if WINDOWSSTORE
                Tracing.Log(string.Format("InitializationService.CreateLibrary -- Checking if the database file exists ({0})...", PathHelper.DatabaseFilePath));
                var storageFolder = ApplicationData.Current.LocalFolder;
                var task = storageFolder.FileExistsAsync(Path.GetFileName(PathHelper.DatabaseFilePath));
                bool databaseExists = task.Result;
                if (!databaseExists)
                {
                    // Create database file
                    Tracing.Log("InitializationService.CreateLibrary -- Creating new database file...");
                    CreateDatabaseFile(PathHelper.DatabaseFilePath);
                }                
#else
                Tracing.Log(string.Format("InitializationService.CreateLibrary -- Checking if the database file exists ({0})...", PathHelper.DatabaseFilePath));
                if (!File.Exists(PathHelper.DatabaseFilePath))
                {
                    // Create database file
                    Tracing.Log("InitializationService.CreateLibrary -- Creating new database file...");
                    CreateDatabaseFile(PathHelper.DatabaseFilePath);
                }
#endif
            }
            catch (Exception ex)
            {
                throw new Exception("Error initializing MPfm: Could not create database file!", ex);
            }
			
            try
            {
                string databaseVersion = GetDatabaseVersion(PathHelper.DatabaseFilePath);
                string[] currentVersionSplit = databaseVersion.Split('.');

                // Check integrity of the setting value (should be split in 2)
                if (currentVersionSplit.Length != 2)
                    throw new Exception("Error fetching database version; the setting value is invalid!");

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
                CheckIfDatabaseVersionNeedsToBeUpdated(PathHelper.DatabaseFilePath);
            }
            catch (Exception ex)
            {
                throw new Exception("Error initializing MPfm: The MPfm database could not be updated!", ex);
            }		    
		}

        /// <summary>
        /// Returns the database version from the database file (actually in the Settings table).
        /// </summary>
        /// <param name="databaseFilePath">Database file path</param>
        /// <returns>Database version (ex: "1.04")</returns>
        private string GetDatabaseVersion(string databaseFilePath)
        {
            // Create gateway
            DatabaseFacade gateway = new DatabaseFacade(databaseFilePath);

            // Fetch database version
            Setting settingDatabaseVersion = gateway.SelectSetting("DatabaseVersion");

            // Check if setting is null
            if (settingDatabaseVersion == null || String.IsNullOrEmpty(settingDatabaseVersion.SettingValue))
            {
                // Yes, this is 1.00 (there was no Setting entry)
                return "1.00";
            }

            return settingDatabaseVersion.SettingValue;
        }

        /// <summary>
        /// Checks if the library database structure needs to be updated by comparing
        /// the database version in the Settings table to the expected database version for this
        /// version of MPfm.Library.dll. If the versions don't match, the database structure will
        /// be updated by running the appropriate migration scripts in the right order.
        /// </summary>
        /// <param name="databaseFilePath">Database file path</param>
        private void CheckIfDatabaseVersionNeedsToBeUpdated(string databaseFilePath)
        {
            int currentMajor = 1;
            int currentMinor = 0;

            // Create gateway
            DatabaseFacade gateway = new DatabaseFacade(databaseFilePath);

            // Get setting
            Tracing.Log("InitializationService - Fetching database version...");
            Setting settingDatabaseVersion = gateway.SelectSetting("DatabaseVersion");

            // Check if setting is null
            if (settingDatabaseVersion == null || String.IsNullOrEmpty(settingDatabaseVersion.SettingValue))
            {
                // Yes, this is 1.00 (there was no Setting entry)
            }
            else
            {
                // Extract major/minor
                string[] currentVersionSplit = settingDatabaseVersion.SettingValue.Split('.');

                // Check integrity of the setting value (should be split in 2)
                if (currentVersionSplit.Length != 2)
                {
                    throw new Exception("Error fetching database version; the setting value is invalid!");
                }

                int.TryParse(currentVersionSplit[0], out currentMajor);
                int.TryParse(currentVersionSplit[1], out currentMinor);
            }

            Tracing.Log("InitializationService - Database version is " + currentMajor.ToString() + "." + currentMinor.ToString("00"));

            // Check database version
            if (_databaseVersionMajor != currentMajor || _databaseVersionMinor != currentMinor)
            {
                // Loop through versions to update (only minor versions for now)
                for (int minor = currentMinor; minor < _databaseVersionMinor; minor++)
                {
                    string sql = string.Empty;
                    string scriptFileName = "MPfm.Library.Scripts.1." + minor.ToString("00") + "-1." + (minor + 1).ToString("00") + ".sql";

                    try
                    {
                        // Get the update script for this version
                        Tracing.Log("InitializationService - Getting database update script (" + scriptFileName + ")...");
                        sql = GetEmbeddedSQLScript(scriptFileName);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error getting update script (" + scriptFileName + ")!", ex);
                    }

                    // Remove the header comments
                    string[] sqlSplitHeader = sql.Split(new string[] { "--*/" }, StringSplitOptions.None);

                    // Split statements
                    string[] sqlSplit = sqlSplitHeader[1].Split(new string[] { "/**/" }, StringSplitOptions.None);

                    // Loop through statements
                    for (int a = 0; a < sqlSplit.Length; a++)
                    {
                        try
                        {
                            // Execute create script
                            Tracing.Log("InitializationService - Executing database update script statement " + (a + 1).ToString() + " (" + scriptFileName + ")...");
                            gateway.ExecuteSQL(sqlSplit[a]);
                        }
                        catch (Exception ex)
                        {
                            // Check minor version (1.02 had a bug where the version was not updated in the database, so it needs to skip any exception)
                            if (minor != 1)
                            {
                                throw new Exception("Error executing the update script (" + scriptFileName + ")!", ex);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates the MPfm database file at the specified location.
        /// Executes the SQL needed to create the tables and basic entries.
        /// </summary>
        /// <param name="databaseFilePath">Database file path</param>
        private void CreateDatabaseFile(string databaseFilePath)
        {
            DatabaseFacade.CreateDatabaseFile(databaseFilePath);
            DatabaseFacade gateway = new DatabaseFacade(databaseFilePath);

            // Get SQL
            string sql = GetEmbeddedSQLScript("MPfm.Library.Scripts.CreateDatabase.sql");

            // Remove the header comments
            string[] sqlSplitHeader = sql.Split(new string[] { "--*/" }, StringSplitOptions.None);

            // Split statements
            string[] sqlSplit = sqlSplitHeader[1].Split(new string[] { "/**/" }, StringSplitOptions.None);

            // Loop through statements and execute each script
            foreach (string sqlStatement in sqlSplit)
                gateway.ExecuteSQL(sqlStatement);
        }

        /// <summary>
        /// Returns an embedded SQL script file from the assembly.
        /// </summary>
        /// <param name="fileName">Embedded SQL script file name (fully qualified)
        /// ex: MPfm.Library.Scripts.CreateDatabase.sql</param>
        /// <returns>SQL script (in string format)</returns>
        private string GetEmbeddedSQLScript(string fileName)
        {
            string sql = string.Empty;

            Assembly assembly;

#if WINDOWSSTORE || WINDOWS_PHONE
            assembly = typeof (ISyncDeviceSpecifications).GetTypeInfo().Assembly;
#else
            assembly = Assembly.GetAssembly(typeof(ISyncDeviceSpecifications));
#endif

            // Fetch SQL from MPfm.Library assembly
            using (Stream stream = assembly.GetManifestResourceStream(fileName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    sql = reader.ReadToEnd();
                }
            }

            return sql;
        }
	}
}
