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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using MPfm.Core;
using MPfm.Library.Database;
using MPfm.Library.Objects;
using MPfm.Library.UpdateLibrary;
using MPfm.Sound.AudioFiles;

namespace MPfm.Library
{
    /// <summary>
    /// The Library class is a cache of the song library. It can update the library in a background worker.
    /// It uses the DataAccess class to access the MPfm database.
    /// </summary>
    public class Library : ILibrary
    {
        // Background worker for update library process
        private readonly BackgroundWorker workerUpdateLibrary = null;
        public DatabaseFacade Facade { get; private set; }
        public bool CancelUpdateLibrary { get; set; }
        private List<AudioFile> _audioFiles;

        #region Events
                
        /// <summary>
        /// Delegate for the OnUpdateLibraryProgress event.
        /// </summary>
        /// <param name="data">OnUpdateLibraryProgress data structure</param>
        public delegate void UpdateLibraryProgress(UpdateLibraryProgressData data);
        /// <summary>
        /// This event is fired every time a file is added to the library.
        /// </summary>
        public event UpdateLibraryProgress OnUpdateLibraryProgress;

        /// <summary>
        /// Delegate for the OnUpdateLibraryFinished event.
        /// </summary>
        /// <param name="data">OnUpdateLibraryFinished data structure</param>
        public delegate void UpdateLibraryFinished(UpdateLibraryFinishedData data);
        /// <summary>
        /// This event is fired when the library update process has finished its work.
        /// </summary>
        public event UpdateLibraryFinished OnUpdateLibraryFinished;

        #endregion        
        
        #region Properties

        /// <summary>
        /// Indicates what database major version is expected. Useful to update the database structure.
        /// Needs to be used with the DatabaseVersionMinor property.
        /// </summary>
        public static int DatabaseVersionMajor { get; private set; }

        /// <summary>
        /// Indicates what database minor version is expected. Useful to update the database structure.
        /// Needs to be used with the DatabaseVersionMajor property.
        /// </summary>
        public static int DatabaseVersionMinor { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs the Library class using the specified database file path.
        /// </summary>                
        /// <param name="databaseFilePath">Database file path</param>
        public Library(string databaseFilePath)
        {
            _audioFiles = null;
            CancelUpdateLibrary = false;
            Facade = null;
            // Check if file path exists
            if (!File.Exists(databaseFilePath))
            {
                throw new Exception("Error: The file path doesn't exist!");
            }

            // Create gateway
            Tracing.Log("Library init -- Initializing gateway...");
            Facade = new DatabaseFacade(databaseFilePath);

            // Create worker process
            Tracing.Log("Library init -- Creating background worker...");
            workerUpdateLibrary = new BackgroundWorker();
            workerUpdateLibrary.WorkerReportsProgress = true;
            workerUpdateLibrary.WorkerSupportsCancellation = true;
            workerUpdateLibrary.DoWork += new DoWorkEventHandler(workerUpdateLibrary_DoWork);
            workerUpdateLibrary.RunWorkerCompleted += new RunWorkerCompletedEventHandler(workerUpdateLibrary_RunWorkerCompleted);            

            // Refresh songs
            Tracing.Log("Library init -- Refreshing cache...");
            RefreshCache();
        }

        static Library()
        {
            DatabaseVersionMajor = 1;
            DatabaseVersionMinor = 5;
        }

        #endregion

        #region Database Script Creation/Update

        /// <summary>
        /// Returns the database version from the database file (actually in the Settings table).
        /// </summary>
        /// <param name="databaseFilePath">Database file path</param>
        /// <returns>Database version (ex: "1.04")</returns>
        public static string GetDatabaseVersion(string databaseFilePath)
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

            // Extract major/minor
            return settingDatabaseVersion.SettingValue;
        }

        /// <summary>
        /// Checks if the library database structure needs to be updated by comparing
        /// the database version in the Settings table to the expected database version for this
        /// version of MPfm.Library.dll. If the versions don't match, the database structure will
        /// be updated by running the appropriate migration scripts in the right order.
        /// </summary>
        /// <param name="databaseFilePath">Database file path</param>
        public static void CheckIfDatabaseVersionNeedsToBeUpdated(string databaseFilePath)
        {
            // Declare variables
            int currentMajor = 1;
            int currentMinor = 0;

            // Create gateway
            DatabaseFacade gateway = new DatabaseFacade(databaseFilePath);

            // Get setting
            Tracing.Log("Main form init -- Fetching database version...");
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
                if(currentVersionSplit.Length != 2)
                {
                    throw new Exception("Error fetching database version; the setting value is invalid!");
                }

                int.TryParse(currentVersionSplit[0], out currentMajor);
                int.TryParse(currentVersionSplit[1], out currentMinor);
            }

            Tracing.Log("Main form init -- Database version is " + currentMajor.ToString() + "." + currentMinor.ToString("00"));

            // Check database version
            if (DatabaseVersionMajor != currentMajor || DatabaseVersionMinor != currentMinor)
            {
                // Loop through versions to update (only minor versions for now)
                for (int minor = currentMinor; minor < DatabaseVersionMinor; minor++)
                {        
                    string sql = string.Empty;
                    string scriptFileName = "MPfm.Library.Scripts.1." + minor.ToString("00") + "-1." + (minor + 1).ToString("00") + ".sql";

                    try
                    {
                        // Get the update script for this version
                        Tracing.Log("Main form init -- Getting database update script (" + scriptFileName + ")...");
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
                    for(int a = 0; a < sqlSplit.Length; a++)
                    {
                        try
                        {
                            // Execute create script
                            Tracing.Log("Main form init -- Executing database update script statement " + (a+1).ToString() + " (" + scriptFileName + ")...");
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
        public static void CreateDatabaseFile(string databaseFilePath)
        {
            // Create database file
#if IOS || ANDROID || LINUX || MACOSX
            MonoSQLiteGateway.CreateDatabaseFile(databaseFilePath);
#else
            SQLiteGateway.CreateDatabaseFile(databaseFilePath);
#endif

            // Create gateway
            DatabaseFacade gateway = new DatabaseFacade(databaseFilePath);
            
            // Get SQL
            string sql = GetEmbeddedSQLScript("MPfm.Library.Scripts.CreateDatabase.sql");

            // Remove the header comments
            string[] sqlSplitHeader = sql.Split(new string[] { "--*/" }, StringSplitOptions.None);

            // Split statements
            string[] sqlSplit = sqlSplitHeader[1].Split(new string[] { "/**/" }, StringSplitOptions.None);

            // Loop through statements
            foreach (string sqlStatement in sqlSplit)
            {
                // Execute create script
                gateway.ExecuteSQL(sqlStatement);
            }
        }

        /// <summary>
        /// Returns an embedded SQL script file from the assembly.
        /// </summary>
        /// <param name="fileName">Embedded SQL script file name (fully qualified)
        /// ex: MPfm.Library.Scripts.CreateDatabase.sql</param>
        /// <returns>SQL script (in string format)</returns>
        public static string GetEmbeddedSQLScript(string fileName)
        {
            // Declare variables
            string sql = string.Empty;

            // Fetch SQL
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    // Read text
                    sql = reader.ReadToEnd();
                }
            }           

            return sql;
        }

        #endregion

        #region Update Library

        #region Report progress

        /// <summary>
        /// Reports progress for the update library process.
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        public void UpdateLibraryReportProgress(string title, string message)
        {
            UpdateLibraryReportProgress(title, message, 0, 0, 0, null);
        }

        /// <summary>
        /// Reports progress for the update library process.
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        /// <param name="percentage">Percentage done</param>
        public void UpdateLibraryReportProgress(string title, string message, double percentage)
        {
            UpdateLibraryReportProgress(title, message, percentage, 0, 0, null);
        }

        /// <summary>
        /// Reports progress for the update library process.
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        /// <param name="percentage">Percentage done</param>
        /// <param name="logEntry">Log entry</param>
        public void UpdateLibraryReportProgress(string title, string message, double percentage, string logEntry)
        {
            UpdateLibraryReportProgress(title, message, percentage, 0, 0, logEntry);
        }

        /// <summary>
        /// Reports progress for the update library process.
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        /// <param name="percentage">Percentage done</param>
        /// <param name="totalNumberOfFiles">Total number of files</param>
        /// <param name="currentFilePosition">Current file position</param>
        public void UpdateLibraryReportProgress(string title, string message, double percentage, int totalNumberOfFiles, int currentFilePosition)
        {
            UpdateLibraryReportProgress(title, message, percentage, totalNumberOfFiles, currentFilePosition, string.Empty);
        }

        /// <summary>
        /// Reports progress for the update library process.
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        /// <param name="percentage">Percentage done</param>
        /// <param name="totalNumberOfFiles">Total number of files</param>
        /// <param name="currentFilePosition">Current file position</param>
        /// <param name="logEntry">Log entry</param>
        public void UpdateLibraryReportProgress(string title, string message, double percentage, int totalNumberOfFiles, int currentFilePosition, string logEntry)
        {
            UpdateLibraryReportProgress(title, message, percentage, totalNumberOfFiles, currentFilePosition, logEntry, string.Empty);
        }

        /// <summary>
        /// Reports progress for the update library process.
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        /// <param name="percentage">Percentage done</param>
        /// <param name="totalNumberOfFiles">Total number of files</param>
        /// <param name="currentFilePosition">Current file position</param>
        /// <param name="logEntry">Log entry</param>
        /// <param name="filePath">Audio file path</param>        
        public void UpdateLibraryReportProgress(string title, string message, double percentage, int totalNumberOfFiles, int currentFilePosition, string logEntry, string filePath)
        {
            UpdateLibraryReportProgress(title, message, percentage, totalNumberOfFiles, currentFilePosition, logEntry, filePath, null);
        }

        /// <summary>
        /// Reports progress for the update library process.
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        /// <param name="percentage">Percentage done</param>
        /// <param name="totalNumberOfFiles">Total number of files</param>
        /// <param name="currentFilePosition">Current file position</param>
        /// <param name="logEntry">Log entry</param>
        /// <param name="filePath">Audio file path</param>
        /// <param name="song">UpdateLibraryProgressDataSong structure</param>
        public void UpdateLibraryReportProgress(string title, string message, double percentage, int totalNumberOfFiles, int currentFilePosition, string logEntry, string filePath, UpdateLibraryProgressDataSong song)
        {
            UpdateLibraryReportProgress(title, message, percentage, totalNumberOfFiles, currentFilePosition, logEntry, filePath, null, null);
        }

        /// <summary>
        /// Reports progress for the update library process.
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        /// <param name="percentage">Percentage done</param>
        /// <param name="totalNumberOfFiles">Total number of files</param>
        /// <param name="currentFilePosition">Current file position</param>
        /// <param name="logEntry">Log entry</param>
        /// <param name="filePath">Audio file path</param>
        /// <param name="song">UpdateLibraryProgressDataSong structure</param>
        /// <param name="ex">Exception</param>
        public void UpdateLibraryReportProgress(string title, string message, double percentage, int totalNumberOfFiles, int currentFilePosition, string logEntry, string filePath, UpdateLibraryProgressDataSong song, Exception ex)
        {
            // Check if an event is subscribed
            if (OnUpdateLibraryProgress != null)
            {
                // Create data
                UpdateLibraryProgressData data = new UpdateLibraryProgressData();
                data.Title = title;
                data.Message = message;
                data.Percentage = percentage;
                data.TotalNumberOfFiles = totalNumberOfFiles;
                data.CurrentFilePosition = currentFilePosition;
                data.FilePath = filePath;
                data.LogEntry = logEntry;
                data.Error = ex;
                data.Song = song;

                // Raise event
                OnUpdateLibraryProgress(data);
            }                       
        }

        #endregion

        /// <summary>
        /// Starts a background worker process to update the library.
        /// Refreshes the whole library by default.
        /// </summary>
        public void UpdateLibrary()
        {
            // Update whole library by default
            UpdateLibrary(UpdateLibraryMode.WholeLibrary, null, null);
        }

        /// <summary>
        /// Starts a background worker process to update the library, using
        /// the mode passed in parameter. If the mode is SpecificFiles, then you
        /// must pass the file paths using a List of strings in the filePaths parameter.
        /// If the mode is SpecificFolder, then you must pass the folder path in the
        /// folderPath parameter.
        /// </summary>
        /// <param name="mode">Update Library mode</param>
        /// <param name="filePaths">List of file paths (for the SpecificFiles mode)</param>
        /// <param name="folderPath">Folder path (for the SpecificFolder mode)</param>
        public void UpdateLibrary(UpdateLibraryMode mode, List<string> filePaths, string folderPath)
        {
            // Create argument
            UpdateLibraryArgument arg = new UpdateLibraryArgument();
            arg.Mode = mode;
            arg.FilePaths = filePaths;
            arg.FolderPath = folderPath;

            // If the mode is SpecificFolder
            if (mode == UpdateLibraryMode.SpecificFolder)
            {
                // Check if the folder is already part of a configured folder
                bool folderFound = false;

                // Get the list of folders from the database                
                List<Folder> folders = Facade.SelectFolders();

                // Search through folders if the base found can be found
                foreach (Folder folder in folders)
                {
                    // Check if the base path is found in the configured path
                    if (folderPath.Contains(folder.FolderPath))
                    {
                        // Set flag
                        folderFound = true;
                        break;
                    }
                }

                // Check if the user has entered a folder deeper than those configured
                // i.e. The user enters F:\FLAC when F:\FLAC\Brian Eno is configured
                foreach (Folder folder in folders)
                {
                    // Check if the configured path is part of the specified path
                    if (folder.FolderPath.Contains(folderPath))
                    {
                        // Delete this configured folder                        
                        Facade.DeleteFolder(folder.FolderId);
                    }
                }

                // Add the folder to the list of configured folders
                if (!folderFound)
                {
                    // Add folder to database                    
                    Facade.InsertFolder(folderPath, true);
                }
            }

            // Start the background process using the arguments
            workerUpdateLibrary.RunWorkerAsync(arg);
        }

        /// <summary>
        /// Main function of the update library process.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void workerUpdateLibrary_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                // Variables
                List<string> mediaFiles = new List<string>();                

                // Detect if an argument was passed
                if (e.Argument == null)
                {
                    // Exit worker
                    return;
                }

                // Get the argument
                UpdateLibraryArgument arg = (UpdateLibraryArgument)e.Argument;

                // Set the cancel operation to false
                CancelUpdateLibrary = false;

                // Refresh cache
                UpdateLibraryReportProgress("Refreshing cache", "Refreshing cache...");
                RefreshCache();

                // Cancel thread if necessary
                if (CancelUpdateLibrary) throw new UpdateLibraryException();

                // Determine the update library mode
                if (arg.Mode == UpdateLibraryMode.WholeLibrary)
                {
                    // Remove broken songs from the library
                    UpdateLibraryReportProgress("Checking for broken file paths", "Checking if songs have been deleted on your hard disk but not removed from the library...");
                    RemoveAudioFilesWithBrokenFilePaths();

                    // Cancel thread if necessary
                    if (CancelUpdateLibrary) throw new UpdateLibraryException();

                    // Search for new media in the library folders
                    UpdateLibraryReportProgress("Searching for media files", "Searching media files in library folders");
                    mediaFiles = SearchMediaFilesInFolders();

                    // Update the media count
                    //UpdateLibraryReportProgress("Searching media files", "Searching media files in library folders", 0, mediaFiles.Count.ToString() + " song(s) found.");
                }
                else if (arg.Mode == UpdateLibraryMode.SpecificFiles)
                {           
                    // Set the media files passed in the argument
                    mediaFiles = arg.FilePaths;
                }
                else if (arg.Mode == UpdateLibraryMode.SpecificFolder)
                {
                    // Search the files in the specified folder
                    UpdateLibraryReportProgress("Searching for media files", "Searching media files in " + arg.FolderPath);
                    mediaFiles = SearchMediaFilesInFolders(arg.FolderPath, true);
                }

                // Get the list of audio files from the database (actually the cache)
                List<string> filePaths = _audioFiles.Select(x => x.FilePath).ToList();

                // Get the list of playlist file paths from database
                List<string> playlistFilePaths = Facade.SelectPlaylistFiles().Select(x => x.FilePath).ToList();

                // Compare list of files from database with list of files found on hard disk
                List<string> audioFilesToUpdate = mediaFiles.Except(filePaths).ToList();

                // Remove existing playlist files
                audioFilesToUpdate = audioFilesToUpdate.Except(playlistFilePaths).ToList();                 

                // Cancel thread if necessary
                if (CancelUpdateLibrary) throw new UpdateLibraryException();

                // Add new media (if media found!)
                if (mediaFiles.Count > 0)
                {
                    AddAudioFilesToLibrary(audioFilesToUpdate);                    
                }                

                // Cancel thread if necessary
                if (CancelUpdateLibrary) throw new UpdateLibraryException();

                // Refreshing cache
                UpdateLibraryReportProgress("Refreshing cache", "Refreshing cache...", 100);
                RefreshCache();

                // Cancel thread if necessary
                if (CancelUpdateLibrary) throw new UpdateLibraryException();

                // Compact database
                UpdateLibraryReportProgress("Compacting database", "Compacting database...", 100);                
                Facade.CompactDatabase();
            }
            catch (UpdateLibraryException ex)
            {
                UpdateLibraryReportProgress("The update process was canceled: " + ex.Message, "Canceled by user");
                e.Cancel = true;
            }
            catch (Exception ex)
            {
                UpdateLibraryReportProgress("An error has occured: " + ex.Message, ex.StackTrace);
            }
        }

        /// <summary>
        /// Fires when the update library process is over.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Argument</param>
        private void workerUpdateLibrary_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Check if the event is registered
            if (OnUpdateLibraryFinished != null)
            {
                // Create data
                UpdateLibraryFinishedData data = new UpdateLibraryFinishedData();

                if (!e.Cancelled)
                {
                    data.Successful = true;
                }

                data.Cancelled = e.Cancelled;

                // Fire event
                OnUpdateLibraryFinished(data);
            }
        }

        /// <summary>
        /// Refreshes the audio file cache from the database.
        /// </summary>
        public void RefreshCache()
        {
            // Refresh audio file cache
            Tracing.Log("Library --  Refreshing audio file cache...");            
            this._audioFiles = Facade.SelectAudioFiles();
        }        

        /// <summary>
        /// Removes the audio files that do not exist anymore on the hard drive.
        /// </summary>
        public void RemoveAudioFilesWithBrokenFilePaths()
        {
            // Get all audio files
            List<AudioFile> files = Facade.SelectAudioFiles();

            // For each audio file
            for (int a = 0; a < files.Count; a++)
            {
                // Check for cancel[
                if (CancelUpdateLibrary)
                {
                    // Sends a cancel exception
                    throw new UpdateLibraryException();
                }

                // If the file doesn't exist, delete the audio file from the database
                if (!File.Exists(files[a].FilePath))
                {
                    Tracing.Log("Removing audio files that do not exist anymore on the hard drive..." + files[a].FilePath);
                    UpdateLibraryReportProgress("Removing audio files that do not exist anymore on the hard drive...", files[a].FilePath, (double)((double)a / (double)files.Count) * 100);
                    Facade.DeleteAudioFile(files[a].Id);
                }
            }
        }   
     
        /// <summary>
        /// Searches for songs in all configured folders.
        /// </summary>
        /// <returns>List of songs (file paths)</returns>
        public List<string> SearchMediaFilesInFolders()
        {
            // List of files
            List<string> files = new List<string>();            

            // Update GUI
            UpdateLibraryReportProgress("Searching media files", "Searching media files in library folders");

            // Get registered folders            
            List<Folder> folders = Facade.SelectFolders();

            // For each registered folder
            foreach (Folder folder in folders)
            {
                // Search for media files in the folder
                UpdateLibraryReportProgress("Searching for media files", "Searching for media files in library folder " + folder.FolderPath);
                List<string> newSongs = SearchMediaFilesInFolders(folder.FolderPath, (bool)folder.IsRecursive);
                files.AddRange(newSongs);
            }

            //// Make a list of file names
            //List<string> filePaths = new List<string>();
            //foreach (Song song in Songs)
            //{
            //    filePaths.Add(song.FilePath);
            //}

            //// Compare list of files from database with list of files found on hard disk
            //List<string> except = files.Except(filePaths).ToList();

            // Return missing files
            //return except;
            return files;
        }

        /// <summary>
        /// Searches for songs in a specific folder, using recursivity or not.
        /// </summary>
        /// <param name="folderPath">Path to search</param>
        /// <param name="recursive">Recursive (if true, will search for sub-folders)</param>
        /// <returns>List of songs (file paths)</returns>
        public List<string> SearchMediaFilesInFolders(string folderPath, bool recursive)
        {
            List<string> arrayFiles = new List<string>();

            // Check for cancel
            if (CancelUpdateLibrary)
            {
                // Sends a cancel exception
                throw new UpdateLibraryException();
            }

            // Get the directory information
            DirectoryInfo rootDirectoryInfo = new DirectoryInfo(folderPath);
            //Tracing.Log("MPfm.Library -- UpdateLibrary -- Counting  new songs in " + folderPath + "");
                        
            UpdateLibraryReportProgress("Searching for media files", "Searching for media files in library folder " + folderPath);

            // Check for sub directories if recursive
            if (recursive)
            {
                // For each directory, search for new directories
                foreach (DirectoryInfo directoryInfo in rootDirectoryInfo.GetDirectories())
                {
                    // Search for songs in that directory                    
                    List<string> listSongs = SearchMediaFilesInFolders(directoryInfo.FullName, recursive);
                    arrayFiles.AddRange(listSongs);
                }
            }

            // Populate list of supported extensions
            List<string> extensionsSupported = new List<string>();
            extensionsSupported.Add("*.WAV");
            extensionsSupported.Add("*.MP3");
            extensionsSupported.Add("*.FLAC");
            extensionsSupported.Add("*.OGG");
            extensionsSupported.Add("*.MPC");
            extensionsSupported.Add("*.APE");
            extensionsSupported.Add("*.WV");
            extensionsSupported.Add("*.WMA");

            extensionsSupported.Add("*.M3U");
            extensionsSupported.Add("*.M3U8");
            extensionsSupported.Add("*.PLS");
            extensionsSupported.Add("*.XSPF");

            //// Populate list of supported extensions
            //List<string> extensionsSupported = new List<string>();
            //Array audioFileFormats = Enum.GetValues(typeof(AudioFileFormat));            
            //foreach (AudioFileFormat audioFileFormat in audioFileFormats)
            //{
            //    // Check if the item is not unknown
            //    if (audioFileFormat != AudioFileFormat.Unknown && audioFileFormat != AudioFileFormat.All)
            //    {
            //        // Add format
            //        extensionsSupported.Add("*." + audioFileFormat.ToString());                    
            //    }
            //}

            // For each extension supported
            foreach (string extension in extensionsSupported)
            {
                foreach (FileInfo fileInfo in rootDirectoryInfo.GetFiles(extension))
                {
                    // Add file
                    arrayFiles.Add(fileInfo.FullName);
                }
            }

            return arrayFiles;
        }

        /// <summary>
        /// Adds audio files into the library. Must specify the list of files to add
        /// using the filePaths parameter.
        /// </summary>
        /// <param name="filePaths">List of audio files to add</param>        
        public void AddAudioFilesToLibrary(List<string> filePaths)
        {
            // Declare variables
            int addNewFilesCount = 0;

            // Loop through files
            foreach (string file in filePaths)
            {
                // Check for cancel
                if (CancelUpdateLibrary)
                {
                    // Sends a cancel exception
                    throw new UpdateLibraryException();
                }

                addNewFilesCount++;
                double percentCompleted = ((double)addNewFilesCount / (double)filePaths.Count) * 100;

                // Add audio file to the library
                AddAudioFileToLibrary(file, percentCompleted, filePaths.Count, addNewFilesCount);
            }
        }

        /// <summary>
        /// Adds an audio file to the library.
        /// </summary>
        /// <param name="filePath">Audio file path</param>
        /// <param name="percentCompleted">Percent completed value (for updating progress)</param>
        /// <param name="totalNumberOfFiles">Total number of files (for updating progress)</param>
        /// <param name="currentFilePosition">Current file position (for updating progress)</param>
        private void AddAudioFileToLibrary(string filePath, double percentCompleted, int totalNumberOfFiles, int currentFilePosition)
        {    
            // Declare variables
            AudioFile audioFile = null;
            PlaylistFile playlistFile = null;

            // Check for cancel
            if (CancelUpdateLibrary)
            {
                // Sends a cancel exception
                throw new UpdateLibraryException();
            }
                        
            try
            {
                // Check if this is a playlist file
                if (filePath.ToUpper().Contains(".M3U") ||
                    filePath.ToUpper().Contains(".M3U8") ||
                    filePath.ToUpper().Contains(".PLS") ||
                    filePath.ToUpper().Contains(".XSPF"))
                {
                    // Get playlist file
                    playlistFile = new PlaylistFile(filePath);

                    // Display update
                    UpdateLibraryReportProgress("Adding media to the library", "Adding " + filePath, percentCompleted, totalNumberOfFiles, currentFilePosition, "Adding " + filePath, filePath, new UpdateLibraryProgressDataSong(), null);
                }
                else
                {
                    // Get audio file metadata
                    audioFile = new AudioFile(filePath, Guid.NewGuid(), true);

                    // Display update
                    UpdateLibraryReportProgress("Adding media to the library", "Adding " + filePath, percentCompleted, totalNumberOfFiles, currentFilePosition, "Adding " + filePath, filePath, new UpdateLibraryProgressDataSong { AlbumTitle = audioFile.AlbumTitle, ArtistName = audioFile.ArtistName, SongTitle = audioFile.Title }, null);
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library -- AddAudioFileToLibrary() error -- Metadata read failed -- " + ex.Message + " -- " + ex.StackTrace);
                UpdateLibraryReportProgress("Adding media to the library", "Metadata read failed", percentCompleted, totalNumberOfFiles, currentFilePosition, "Could not add " + filePath + "; metadata read failed.", filePath, null, ex);
                return;
            }

            try
            {
                // Determine what to insert in database
                if (audioFile != null)
                {
                    // Insert audio file                
                    Facade.InsertAudioFile(audioFile);
                }
                else if (playlistFile != null)
                {
                    // Insert playlist file                
                    Facade.InsertPlaylistFile(playlistFile);
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library -- AddAudioFileToLibrary() error -- Database insertion failed -- " + ex.Message + " -- " + ex.StackTrace);
                UpdateLibraryReportProgress("Adding media to the library", "Database insertion failed", percentCompleted, totalNumberOfFiles, currentFilePosition, "Could not add " + filePath + "; database insertion failed.", filePath, null, ex);
            }
        }

        #endregion

        /// <summary>
        /// Removes the songs in the library that match the path passed in parameter.        
        /// </summary>
        /// <param name="folderPath">Folder path</param>
        public void RemoveSongsFromLibrary(string folderPath)
        {
            // Delete audio files based on path            
            Facade.DeleteAudioFiles(folderPath);
        }

        #region Select (strings)

        /// <summary>
        /// Returns the unique artist names in the library.
        /// </summary>        
        /// <returns>List of artist names</returns>
        public List<string> SelectArtistNames()
        {
            return SelectArtistNames(AudioFileFormat.All);
        }

        /// <summary>
        /// Returns the unique artist names in the library, using the filter passed in the soundFormat parameter.
        /// </summary>
        /// <param name="audioFileFormat">Audio file format filter</param>
        /// <returns>List of artist names</returns>
        public List<string> SelectArtistNames(AudioFileFormat audioFileFormat)
        {
            return Facade.SelectDistinctArtistNames(audioFileFormat);
        }

        /// <summary>
        /// Returns the unique album titles of a specific artist in the library.
        /// </summary>
        /// <param name="artistName">Artist name</param>        
        /// <returns>List of album titles</returns>    
        public List<string> SelectArtistAlbumTitles(string artistName)
        {
            return SelectArtistAlbumTitles(artistName, AudioFileFormat.All);
        }

        /// <summary>
        /// Returns the unique album titles of a specific artist in the library, using the filter passed in the soundFormat parameter.
        /// </summary>
        /// <param name="artistName">Artist Name</param>
        /// <param name="audioFileFormat">Audio file format filter</param>
        /// <returns>List of album titles</returns>        
        public List<string> SelectArtistAlbumTitles(string artistName, AudioFileFormat audioFileFormat)
        {
            List<string> albums = null;

            try
            {
                // Basic request
                IEnumerable<AudioFile> queryAudioFiles = from s in _audioFiles
                                                         select s;

                // Do we have to filter by sound format?
                if (audioFileFormat != AudioFileFormat.All)
                {
                    // Set filter
                    queryAudioFiles = queryAudioFiles.Where(x => x.FileType == audioFileFormat);
                }

                // Query: get all albums from artist from the Songs table
                var query = from song in queryAudioFiles
                            orderby song.AlbumTitle
                            where song.ArtistName == artistName
                            select song.AlbumTitle;

                // Get distinct albums
                query = query.Distinct();

                // Convert to a list of strings
                albums = query.ToList<string>();
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (Library) --  Error in SelectArtistAlbums(): " + ex.Message);
                throw;
            }

            return albums;
        }

        /// <summary>
        /// Returns a list of distinct album titles.
        /// </summary>
        /// <returns>List of album titles</returns>
        public Dictionary<string, List<string>> SelectAlbumTitles()
        {
            return SelectAlbumTitles(AudioFileFormat.All);
        }

        /// <summary>
        /// Returns a list of distinct album titles, using the filter passed in the soundFormat parameter.
        /// </summary>
        /// <param name="audioFileFormat">Audio file format filter</param>
        /// <returns>List of album titles</returns>
        public Dictionary<string, List<string>> SelectAlbumTitles(AudioFileFormat audioFileFormat)
        {
            return Facade.SelectDistinctAlbumTitles(audioFileFormat);
        }

        ///// <summary>
        ///// Returns a list of distinct album titles with the file path of one of the audio files
        ///// of the album. This method is useful to display album covers.
        ///// </summary>
        ///// <returns>List of album titles with file path</returns>
        //public Dictionary<string, string> SelectAlbumTitlesWithFilePaths()
        //{
        //    return SelectAlbumTitlesWithFilePaths(AudioFileFormat.All);
        //}

        ///// <summary>
        ///// Returns a list of distinct album titles with the file path of one of the audio files
        ///// of the album, using the filter passed in the soundFormat parameter.
        ///// </summary>
        ///// <param name="audioFileFormat">Audio file format filter</param>
        ///// <returns>List of album titles with file path</returns>
        //public Dictionary<string, string> SelectAlbumTitlesWithFilePaths(AudioFileFormat audioFileFormat)
        //{
        //    return gateway.SelectDistinctAlbumTitlesWithFilePaths(audioFileFormat);
        //}

        #endregion

        #region Select (AudioFile)

        /// <summary>
        /// Selects an audio file from the cache using its identifier.
        /// </summary>
        /// <param name="audioFileId">AudioFile identifier</param>
        /// <returns>AudioFile</returns>
        public AudioFile SelectAudioFile(Guid audioFileId)
        {
            // Declare variables
            AudioFile audioFile = null;

            try
            {
                // Get audio file
                audioFile = _audioFiles.FirstOrDefault(s => s.Id == audioFileId);
            }
            catch (Exception ex)
            {
                Tracing.Log(ex);
                throw;
            }

            return audioFile;
        }

        /// <summary>
        /// Selects all the audio files from the cache.
        /// </summary>
        /// <returns>List of AudioFiles</returns>
        public List<AudioFile> SelectAudioFiles()
        {
            return SelectAudioFiles(AudioFileFormat.All, string.Empty, true, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Selects audio files from the song cache, filtered by different parameters.
        /// </summary>
        /// <param name="audioFileFormat">Audio file format filter</param>
        /// <returns>List of AudioFiles</returns>
        public List<AudioFile> SelectAudioFiles(AudioFileFormat audioFileFormat)
        {
            return SelectAudioFiles(audioFileFormat, string.Empty, true, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Selects audio files from the song cache, filtered by different parameters.
        /// </summary>
        /// <param name="audioFileFormat">Audio file format filter</param>
        /// <param name="orderBy">Order by field name</param>
        /// <param name="orderByAscending">Order by (ascending) field name</param>
        /// <returns>List of AudioFiles</returns>
        public List<AudioFile> SelectAudioFiles(AudioFileFormat audioFileFormat, string orderBy, bool orderByAscending)
        {
            return SelectAudioFiles(audioFileFormat, orderBy, orderByAscending, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Selects audio files from the song cache, filtered by different parameters.
        /// </summary>
        /// <param name="audioFileFormat">Audio file format filter</param>
        /// <param name="orderBy">Order by field name</param>
        /// <param name="orderByAscending">Order by (ascending) field name</param>
        /// <param name="artistName">Artist name</param>
        /// <returns>List of AudioFiles</returns>
        public List<AudioFile> SelectAudioFiles(AudioFileFormat audioFileFormat, string orderBy, bool orderByAscending, string artistName)
        {
            return SelectAudioFiles(audioFileFormat, orderBy, orderByAscending, artistName, string.Empty, string.Empty);
        }

        /// <summary>
        /// Selects audio files from the song cache, filtered by different parameters.
        /// </summary>
        /// <param name="audioFileFormat">Audio file format filter</param>
        /// <param name="orderBy">Order by field name</param>
        /// <param name="orderByAscending">Order by (ascending) field name</param>
        /// <param name="artistName">Artist name</param>
        /// <param name="albumTitle">Album title</param>        
        /// <returns>List of AudioFiles</returns>
        public List<AudioFile> SelectAudioFiles(AudioFileFormat audioFileFormat, string orderBy, bool orderByAscending, string artistName, string albumTitle)
        {
            return SelectAudioFiles(audioFileFormat, orderBy, orderByAscending, artistName, albumTitle, string.Empty);
        }

        /// <summary>
        /// Selects audio files from the song cache, filtered by different parameters.
        /// </summary>
        /// <param name="audioFileFormat">Audio file format filter</param>
        /// <param name="orderBy">Order by field name</param>
        /// <param name="orderByAscending">Order by (ascending) field name</param>
        /// <param name="artistName">Artist name</param>
        /// <param name="albumTitle">Album title</param>
        /// <param name="searchTerms">Search terms</param>
        /// <returns>List of AudioFiles</returns>
        public List<AudioFile> SelectAudioFiles(AudioFileFormat audioFileFormat, string orderBy, bool orderByAscending, string artistName, string albumTitle, string searchTerms)
        {
            // Create variables
            List<AudioFile> audioFiles = null;

            try
            {
                IEnumerable<AudioFile> queryAudioFiles = null;

                // Check for default order by (ignore ascending)
                if (String.IsNullOrEmpty(orderBy))
                {
                    // Set query
                    queryAudioFiles = from s in _audioFiles
                                      orderby s.ArtistName, s.AlbumTitle, s.FileType, s.DiscNumber, s.TrackNumber
                                      select s;
                }
                else
                {
                    // Check for orderby ascending/descending
                    if (orderByAscending)
                    {
                        // Set query
                        queryAudioFiles = from s in _audioFiles
                                          orderby GetPropertyValue(s, orderBy)
                                          select s;
                    }
                    else
                    {
                        // Set query
                        queryAudioFiles = from s in _audioFiles
                                          orderby GetPropertyValue(s, orderBy) descending
                                          select s;                        
                    }
                }

                // Check if artistName is null
                if (!String.IsNullOrEmpty(artistName))
                {
                    // Add the artist condition to the query
                    queryAudioFiles = queryAudioFiles.Where(s => s.ArtistName == artistName);                    
                }

                // Check if albumTitle is null
                if (!String.IsNullOrEmpty(albumTitle))
                {
                    // Add the artist condition to the query
                    queryAudioFiles = queryAudioFiles.Where(s => s.AlbumTitle == albumTitle);                    
                }

                // Check if searchTerms is null
                if (!String.IsNullOrEmpty(searchTerms))
                {
                    // Split search terms
                    string[] searchTermsSplit = searchTerms.Split(new string[] { " " }, StringSplitOptions.None);
                    
                    // Loop through search terms
                    foreach (string searchTerm in searchTermsSplit)
                    {
                        // Add the artist condition to the query
                        queryAudioFiles = queryAudioFiles.Where(s => s.ArtistName.ToUpper().Contains(searchTerm.ToUpper()) ||
                                                                     s.AlbumTitle.ToUpper().Contains(searchTerm.ToUpper()) ||
                                                                     s.Title.ToUpper().Contains(searchTerm.ToUpper()));
                    }
                }

                // Check for audio file format filter
                if (audioFileFormat == AudioFileFormat.All)
                {
                    // 
                }
                else
                {
                    // Set filter by file type
                    queryAudioFiles = queryAudioFiles.Where(s => s.FileType == audioFileFormat);
                }

                //// Check for default order by
                //if (String.IsNullOrEmpty(orderBy))
                //{
                //    // Add order by
                //    querySongs = querySongs.OrderBy(s => s.ArtistName).ThenBy(s => s.AlbumTitle).ThenBy(s => s.DiscNumber).ThenBy(s => s.TrackNumber);
                //}
                //else
                //{
                //    // Custom order by
                //    querySongs = querySongs.OrderBy("");
                //}

                // Execute query
                audioFiles = queryAudioFiles.ToList();
            }
            catch (Exception ex)
            {
                Tracing.Log(ex);
                throw;
            }

            //return ConvertDTO.ConvertSongs(songs);
            return audioFiles;
        }

        #endregion

        #region Generate Playlist

        ///// <summary>
        ///// Generates a playlist containing all songs from the library.
        ///// </summary>
        ///// <param name="soundFormat">Sound format filter</param>        
        ///// <returns>Playlist</returns>
        //public PlaylistDTO GeneratePlaylistAll(FilterSoundFormat soundFormat)
        //{
        //    // Create playlist
        //    PlaylistDTO playlist = new PlaylistDTO();            
        //    playlist.PlaylistName = "All songs";
        //    playlist.PlaylistType = PlaylistType.All;

        //    // Get songs for this artist
        //    List<SongDTO> songs = SelectSongs(soundFormat);

        //    // Transform songs into playlist songs
        //    foreach (SongDTO song in songs)
        //    {
        //        // Create playlist song
        //        PlaylistSongDTO playlistSong = new PlaylistSongDTO();
        //        //playlistSong.PlaylistId = playlist.PlaylistId;
        //        playlistSong.Song = song;

        //        // Add playlist song to list
        //        playlist.Songs.Add(playlistSong);
        //    }

        //    // Set first song
        //    if (playlist.Songs.Count > 0)
        //    {
        //        playlist.CurrentSong = playlist.Songs[0];
        //    }

        //    return playlist;
        //}

        ///// <summary>
        ///// Generates a playlist containing all songs from a specific artist.
        ///// </summary>
        ///// <param name="soundFormat">Sound format filter</param>
        ///// <param name="artistName">Artist name</param>
        ///// <returns>Playlist</returns>
        //public PlaylistDTO GeneratePlaylistFromArtist(FilterSoundFormat soundFormat, string artistName)
        //{
        //    // Create playlist
        //    PlaylistDTO playlist = new PlaylistDTO();
        //    playlist.PlaylistName = "All songs from " + artistName;
        //    playlist.PlaylistType = PlaylistType.Artist;           

        //    // Get songs for this artist
        //    List<SongDTO> songs = SelectSongs(soundFormat, string.Empty, true, artistName);

        //    // Transform songs into playlist songs
        //    foreach (SongDTO song in songs)
        //    {
        //        // Create playlist song
        //        PlaylistSongDTO playlistSong = new PlaylistSongDTO();
        //        //playlistSong.PlaylistId = playlist.PlaylistId;
        //        playlistSong.Song = song;

        //        // Add playlist song to list
        //        playlist.Songs.Add(playlistSong);
        //    }

        //    // Set first song
        //    if (playlist.Songs.Count > 0)
        //    {
        //        playlist.CurrentSong = playlist.Songs[0];
        //    }

        //    return playlist;           
        //}

        ///// <summary>
        ///// Generates a playlist containing all songs from a specific album.
        ///// </summary>
        ///// <param name="soundFormat">Sound format filter</param>
        ///// <param name="artistName">Artist name</param>
        ///// <param name="albumTitle">Album title</param>
        ///// <returns>Playlist</returns>
        //public PlaylistDTO GeneratePlaylistFromAlbum(FilterSoundFormat soundFormat, string artistName, string albumTitle)
        //{
        //    // Create playlist
        //    PlaylistDTO playlist = new PlaylistDTO();
        //    playlist.PlaylistName = "All songs from " + artistName + "'s " + albumTitle;
        //    playlist.PlaylistType = PlaylistType.Album;

        //    // Get songs for this artist
        //    List<SongDTO> songs = SelectSongs(soundFormat, string.Empty, true, artistName, albumTitle);

        //    // Transform songs into playlist songs
        //    foreach (SongDTO song in songs)
        //    {
        //        // Create playlist song
        //        PlaylistSongDTO playlistSong = new PlaylistSongDTO();
        //        //playlistSong.PlaylistId = playlist.PlaylistId;
        //        playlistSong.Song = song;

        //        // Add playlist song to list
        //        playlist.Songs.Add(playlistSong);
        //    }

        //    // Set first song
        //    if (playlist.Songs.Count > 0)
        //    {
        //        playlist.CurrentSong = playlist.Songs[0];
        //    }

        //    return playlist;
        //}

        #endregion

        #region Playlists

        ///// <summary>
        ///// Returns all the playlists from the database.
        ///// </summary>
        ///// <param name="includePlaylistSongs">If true, the playlist songs are included</param>
        ///// <returns>List of playlists</returns>
        //public List<PlaylistDTO> SelectPlaylists(bool includePlaylistSongs)
        //{
        //    // Create the list of DTO
        //    List<PlaylistDTO> dtos = new List<PlaylistDTO>();

        //    // Get the playlists from the database
        //    List<Playlist> playlists = DataAccess.SelectPlaylists();

        //    // For each playlist
        //    foreach (Playlist playlist in playlists)
        //    {
        //        // Fetch the playlist songs from the database, if specified
        //        List<PlaylistSong> playlistSongs = null;
        //        if (includePlaylistSongs)
        //        {
        //            // Fetch the playlist songs from the database
        //            playlistSongs = DataAccess.SelectPlaylistSongs(new Guid(playlist.PlaylistId));
        //        }

        //        // Convert into DTO and add to list
        //        PlaylistDTO dto = ConvertDTO.ConvertPlaylist(playlist, playlistSongs);
        //        dtos.Add(dto);
        //    }

        //    return dtos;
        //}

        ///// <summary>
        ///// Selects a playlist from the database, using its identifier.
        ///// </summary>
        ///// <param name="playlistId">Playlist Identifier</param>
        ///// <returns>PlaylistDTO</returns>
        //public PlaylistDTO SelectPlaylist(Guid playlistId)
        //{
        //    PlaylistDTO dto = null;

        //    // Get playlist from database
        //    Playlist playlist = DataAccess.SelectPlaylist(playlistId);
        //    List<PlaylistSong> playlistSongs = DataAccess.SelectPlaylistSongs(playlistId);

        //    // If not not, convert to dto
        //    if (playlist != null)
        //    {
        //        // Convert to dto
        //        dto = ConvertDTO.ConvertPlaylist(playlist, null);

        //        // Check if there are playlist songs
        //        if (playlistSongs != null)
        //        {
        //            // Loop through playlist songs
        //            foreach (PlaylistSong playlistSong in playlistSongs)
        //            {
        //                // Create DTO
        //                PlaylistSongDTO playlistSongDTO = new PlaylistSongDTO();
        //                playlistSongDTO.PlaylistSongId = new Guid(playlistSong.PlaylistSongId);

        //                // Fetch song from cache
        //                Guid songId = new Guid(playlistSong.SongId);
        //                playlistSongDTO.Song = Songs.FirstOrDefault(x => x.SongId == songId);

        //                // Add song to list
        //                dto.Songs.Add(playlistSongDTO);
        //            }
        //        }
        //    }

        //    return dto;
        //}

        ///// <summary>
        ///// Saves a playlist and its playlist songs into the database.
        ///// </summary>
        ///// <param name="playlist">Playlist</param>
        //public void SavePlaylist(PlaylistDTO playlist)
        //{
        //    // Flag the playlist as custom
        //    playlist.PlaylistType = PlaylistType.Custom;

        //    // Select playlist from database
        //    Playlist playlistEF = DataAccess.SelectPlaylist(playlist.PlaylistId);

        //    // Check if the playlist is null
        //    if (playlistEF == null)
        //    {
        //        // The playlist doesn't exist in the database; we need to do an INSERT
        //        playlistEF = new Playlist();
        //        playlistEF.PlaylistId = playlist.PlaylistId.ToString();
        //        playlistEF.PlaylistName = playlist.PlaylistName;

        //        // Insert the playlist
        //        DataAccess.InsertPlaylist(playlistEF);
        //    }
        //    else
        //    {
        //        // The playlist already exists in the database; we need to do an UPDATE
        //        playlistEF.PlaylistName = playlist.PlaylistName;

        //        // Update the playlist
        //        DataAccess.UpdatePlaylist(playlistEF);

        //        // Delete all the playlist songs for the playlist
        //        DataAccess.DeletePlaylistSongs(playlist.PlaylistId);
        //    }

        //    // Generate the playlist songs
        //    List<PlaylistSong> playlistSongs = new List<PlaylistSong>();
        //    for (int a = 0; a < playlist.Songs.Count; a++)
        //    {
        //        // Create playlist song for database
        //        PlaylistSong playlistSongEF = new PlaylistSong();
        //        playlistSongEF.PlaylistSongId = playlist.Songs[a].PlaylistSongId.ToString();
        //        playlistSongEF.PlaylistId = playlist.PlaylistId.ToString();
        //        playlistSongEF.SongId = playlist.Songs[a].Song.SongId.ToString();
        //        playlistSongEF.Position = a + 1;
        //        playlistSongs.Add(playlistSongEF);
        //    }

        //    // Insert playlist songs into database
        //    DataAccess.InsertPlaylistSongs(playlistSongs);
        //}

        #endregion

        #region ID3 tags / Album art

        /// <summary>
        /// Returns the TagLib# data structure of a file.
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <returns>TagLib# data structure</returns>
        public static TagLib.File GetTagLibInfo(string filePath)
        {
            return TagLib.File.Create(filePath);
        }

#if (!IOS && !ANDROID)
        /// <summary>
        /// Returns the album art in the ID3 tags, or the folder.jpg
        /// image if the tags are empty.
        /// </summary>
        /// <param name="filePath">Media file path</param>
        /// <returns>Image</returns>
        public static System.Drawing.Image GetAlbumArtFromID3OrFolder(string filePath)
        {
            // Declare variables
			System.Drawing.Image image = null;
            TagLib.File file = null;

            try
            {
                // Get ID3 tags from file
                file = TagLib.File.Create(filePath);

                // Can we get the image from the ID3 tags?
                if (file != null && file.Tag != null && file.Tag.Pictures != null && file.Tag.Pictures.Length > 0)
                {
                    // Get image from ID3 tags
					System.Drawing.ImageConverter ic = new System.Drawing.ImageConverter();
                    image = (System.Drawing.Image)ic.ConvertFrom(file.Tag.Pictures[0].Data.Data);
                }

                // If an image was found, return the image. If not, use folder.jpg
                if (image != null)
                {
                    return image;
                }
            }
            catch
            {
                // The TagLib# read has failed. Continue to try to get the image from folder.jpg.
            }
            finally
            {
                // Dispose file
                if(file != null)
                    file.Dispose();
            }

            // We still haven't found an image; check in the directory for folder.jpg
            if (image == null)
            {
                // Find out if the image is in the folder
                string imagePath = string.Empty;
                string imageDirectory = Path.GetDirectoryName(filePath).ToLower();
                if (File.Exists(imageDirectory + "\\folder.jpg"))
                {
                    imagePath = imageDirectory + "\\folder.jpg";
                }
                else if (File.Exists(imageDirectory + "\\folder.jpeg"))
                {
                    imagePath = imageDirectory + "\\folder.jpeg";
                }
                else if (File.Exists(imageDirectory + "\\folder.png"))
                {
                    imagePath = imageDirectory + "\\folder.png";
                }
                if (!String.IsNullOrEmpty(imagePath))
                {
                    try
                    {
                        // Get image from file
						image = System.Drawing.Image.FromFile(imagePath);
                    }
                    catch
                    {
                        // The image format is wrong or the image is corrupted. Return null.
                        return null;
                    }
                }
            }

            return image;
        }
#endif

        #endregion

        /// <summary>
        /// Resets the library (deletes all songs from the database).
        /// </summary>
        public void ResetLibrary()
        {
            // Reset library            
            Facade.ResetLibrary();

            // Compact database            
            Facade.CompactDatabase();

            // Refresh cache
            RefreshCache();
        }

        /// <summary>
        /// Updates the play count of an audio file into the database.
        /// </summary>
        /// <param name="audioFileId">AudioFile identifier</param>
        public void UpdateAudioFilePlayCount(Guid audioFileId)
        {
            // Update play count in database            
            Facade.UpdatePlayCount(audioFileId);

            // Update play count in cache                  
            AudioFile audioFile = _audioFiles.SingleOrDefault(x => x.Id == audioFileId);
            if (audioFile != null)
            {
                // Fetch audio file from database
                AudioFile audioFileDatabase = Facade.SelectAudioFile(audioFile.Id);
                
                // Is the audio file in the database?
                if (audioFileDatabase != null)
                {
                    // Set properties
                    audioFile.PlayCount = audioFileDatabase.PlayCount;
                    audioFile.LastPlayed = audioFileDatabase.LastPlayed;
                }
            }
        }

        /// <summary>
        /// Fetches the property value of an object.
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="property">Property</param>
        /// <returns>Value</returns>
        private static object GetPropertyValue(object obj, string property)
        {
            PropertyInfo propertyInfo = obj.GetType().GetProperty(property);
            return propertyInfo.GetValue(obj, null);
        }
    }
}
