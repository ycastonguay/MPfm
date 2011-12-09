//
// Library.cs: This class manages the library sound files and interacts with the
//             database.
//
// Copyright © 2011 Yanick Castonguay
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
using System.Data;
using System.Data.Linq;
using System.Data.Objects;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MPfm.Core;
using MPfm.Sound;
using MPfm.Player;
using MPfm.Player.PlayerV4;

namespace MPfm.Library
{
    /// <summary>
    /// The Library class is a cache of the song library. It can update the library in a background worker.
    /// It uses the DataAccess class to access the PMP database.
    /// </summary>
    public class Library
    {
        /// <summary>
        /// Private value for the Gateway property.
        /// </summary>
        private MPfmGateway m_gateway = null;
        /// <summary>
        /// Data access library.
        /// </summary>
        public MPfmGateway Gateway
        {
            get
            {
                return m_gateway;
            }
        }

        // Update library progress delegate/event
        public delegate void UpdateLibraryProgress(OldUpdateLibraryProgressData data);
        public event UpdateLibraryProgress OnUpdateLibraryProgress;

        // Update library finished delegate/event
        public delegate void UpdateLibraryFinished(UpdateLibraryFinishedData data);
        public event UpdateLibraryFinished OnUpdateLibraryFinished;

        // Background worker for update library process
        private BackgroundWorker workerUpdateLibrary = null;

        private bool m_cancelUpdateLibrary = false;
        /// <summary>
        /// When true, cancels an update library process if running.
        /// </summary>
        public bool CancelUpdateLibrary
        {
            get
            {
                return m_cancelUpdateLibrary;
            }
            set
            {
                m_cancelUpdateLibrary = value;
            }
        }

        /// <summary>
        /// Private value for the AudioFiles property.
        /// </summary>
        private List<AudioFile> m_audioFiles = null;
        /// <summary>
        /// Local cache of the audio file library.
        /// </summary>
        public List<AudioFile> AudioFiles
        {
            get
            {
                return m_audioFiles;
            }
        }

        /// <summary>
        /// Constructs the Library class using the specified database file path.
        /// </summary>                
        /// <param name="databaseFilePath">Database file path</param>
        public Library(string databaseFilePath)
        {
            // Check if file path exists
            if (!File.Exists(databaseFilePath))
            {
                throw new Exception("Error: The file path doesn't exist!");
            }

            // Create gateway
            Tracing.Log("Library.Constructor || Initializing gateway...");
            m_gateway = new MPfmGateway(databaseFilePath);

            // Create worker process
            Tracing.Log("Library.Constructor || Creating background worker...");
            workerUpdateLibrary = new BackgroundWorker();
            workerUpdateLibrary.WorkerReportsProgress = true;
            workerUpdateLibrary.WorkerSupportsCancellation = true;
            workerUpdateLibrary.DoWork += new DoWorkEventHandler(workerUpdateLibrary_DoWork);
            workerUpdateLibrary.RunWorkerCompleted += new RunWorkerCompletedEventHandler(workerUpdateLibrary_RunWorkerCompleted);            

            // Refresh songs
            Tracing.Log("Library.Constructor || Refreshing cache...");
            RefreshCache();
        }

        #region Update Library

        #region Report progress

        public void UpdateLibraryReportProgress(string title, string message)
        {
            UpdateLibraryReportProgress(title, message, 0, 0, 0, null);
        }

        public void UpdateLibraryReportProgress(string title, string message, double percentage)
        {
            UpdateLibraryReportProgress(title, message, percentage, 0, 0, null);
        }

        public void UpdateLibraryReportProgress(string title, string message, double percentage, string logEntry)
        {
            UpdateLibraryReportProgress(title, message, percentage, 0, 0, logEntry);
        }

        public void UpdateLibraryReportProgress(string title, string message, double percentage, int totalNumberOfFiles, int currentFilePosition)
        {
            UpdateLibraryReportProgress(title, message, percentage, totalNumberOfFiles, currentFilePosition, string.Empty);
        }

        public void UpdateLibraryReportProgress(string title, string message, double percentage, int totalNumberOfFiles, int currentFilePosition, string logEntry)
        {
            UpdateLibraryReportProgress(title, message, percentage, totalNumberOfFiles, currentFilePosition, logEntry, string.Empty);
        }

        public void UpdateLibraryReportProgress(string title, string message, double percentage, int totalNumberOfFiles, int currentFilePosition, string logEntry, string filePath)
        {
            UpdateLibraryReportProgress(title, message, percentage, totalNumberOfFiles, currentFilePosition, logEntry, filePath, null);
        }

        public void UpdateLibraryReportProgress(string title, string message, double percentage, int totalNumberOfFiles, int currentFilePosition, string logEntry, string filePath, UpdateLibraryProgressDataSong song)
        {
            UpdateLibraryReportProgress(title, message, percentage, totalNumberOfFiles, currentFilePosition, logEntry, filePath, null, null);
        }

        /// <summary>
        /// Updates the progress of the library update.
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message</param>
        public void UpdateLibraryReportProgress(string title, string message, double percentage, int totalNumberOfFiles, int currentFilePosition, string logEntry, string filePath, UpdateLibraryProgressDataSong song, Exception ex)
        {
            // Check if an event is subscribed
            if (OnUpdateLibraryProgress != null)
            {
                // Create data
                OldUpdateLibraryProgressData data = new OldUpdateLibraryProgressData();
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
                //List<Folder> folders = DataAccess.SelectFolders();
                List<Folder> folders = m_gateway.SelectFolders();

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
                        //DataAccess.DeleteFolder(new Guid(folder.FolderId));
                        m_gateway.DeleteFolder(folder.FolderId);
                    }
                }

                // Add the folder to the list of configured folders
                if (!folderFound)
                {
                    // Add folder to database
                    //DataAccess.InsertFolder(folderPath, true);
                    m_gateway.InsertFolder(folderPath, true);
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
                if (CancelUpdateLibrary) throw new OldUpdateLibraryException();

                // Determine the update library mode
                if (arg.Mode == UpdateLibraryMode.WholeLibrary)
                {
                    // Remove broken songs from the library
                    UpdateLibraryReportProgress("Checking for broken file paths", "Checking if songs have been deleted on your hard disk but not removed from the library...");
                    RemoveAudioFilesWithBrokenFilePaths();

                    // Cancel thread if necessary
                    if (CancelUpdateLibrary) throw new OldUpdateLibraryException();

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

                // Make a list of file names
                List<string> filePaths = new List<string>();
                foreach (AudioFile audioFile in AudioFiles)
                {
                    filePaths.Add(audioFile.FilePath);
                }

                // Compare list of files from database with list of files found on hard disk
                List<string> audioFilesToUpdate = mediaFiles.Except(filePaths).ToList();

                // Cancel thread if necessary
                if (CancelUpdateLibrary) throw new OldUpdateLibraryException();

                // Add new media (if media found!)
                if (mediaFiles.Count > 0)
                {
                    AddAudioFilesToLibrary(audioFilesToUpdate);
                }

                // Cancel thread if necessary
                if (CancelUpdateLibrary) throw new OldUpdateLibraryException();

                // Refreshing cache
                UpdateLibraryReportProgress("Refreshing cache", "Refreshing cache...", 100);
                RefreshCache();

                // Cancel thread if necessary
                if (CancelUpdateLibrary) throw new OldUpdateLibraryException();

                // Compact database
                UpdateLibraryReportProgress("Compacting database", "Compacting database...", 100);                
                m_gateway.CompactDatabase();
            }
            catch (OldUpdateLibraryException ex)
            {
                UpdateLibraryReportProgress("The update process was canceled", "Canceled by user");
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
            Tracing.Log("MPfm.Library (Library) --  Refreshing audio file cache...");            
            m_audioFiles = m_gateway.SelectAudioFiles();
        }        

        /// <summary>
        /// Removes the audio files that do not exist anymore on the hard drive.
        /// </summary>
        public void RemoveAudioFilesWithBrokenFilePaths()
        {
            // Get all audio files
            List<AudioFile> audioFiles = m_gateway.SelectAudioFiles();

            // For each audio file
            for(int a = 0; a < audioFiles.Count; a++)
            {
                // Check for cancel[
                if (CancelUpdateLibrary)
                {
                    // Sends a cancel exception
                    throw new OldUpdateLibraryException();
                }

                // If the file doesn't exist, delete the audio file from the database
                if (!File.Exists(audioFiles[a].FilePath))
                {
                    Tracing.Log("Removing audio files that do not exist anymore on the hard drive..." + audioFiles[a].FilePath);
                    UpdateLibraryReportProgress("Removing audio files that do not exist anymore on the hard drive...", audioFiles[a].FilePath, (double)((double)a / (double)audioFiles.Count) * 100);
                    //DataAccess.DeleteSong(new Guid(songs[a].SongId));
                    //m_gateway.DeleteSong(songs[a].SongId);
                    m_gateway.DeleteAudioFile(audioFiles[a].Id);
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
            //List<Folder> folders = DataAccess.SelectFolders();
            List<Folder> folders = m_gateway.SelectFolders();

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
                throw new OldUpdateLibraryException();
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

            // Hardcode list of supported extensions
            List<string> extensionsSupported = new List<string>();
            extensionsSupported.Add("*.MP3");
            extensionsSupported.Add("*.OGG");
            extensionsSupported.Add("*.FLAC");

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
                    throw new OldUpdateLibraryException();
                }

                addNewFilesCount++;
                double percentCompleted = ((double)addNewFilesCount / (double)filePaths.Count) * 100;

                // Add audio file to the library
                AddAudioFileToLibrary(file, percentCompleted, filePaths.Count, addNewFilesCount);
            }
        }

        /// <summary>
        /// Adds an audio file to the library. Supports MP3, FLAC and OGG sound files.
        /// </summary>
        /// <param name="filePath">Audio file path</param>
        /// <param name="percentCompleted">Percent completed value (for updating progress)</param>
        /// <param name="totalNumberOfFiles">Total number of files (for updating progress)</param>
        /// <param name="currentFilePosition">Current file position (for updating progress)</param>
        private void AddAudioFileToLibrary(string filePath, double percentCompleted, int totalNumberOfFiles, int currentFilePosition)
        {    
            // Declare variables
            AudioFile audioFile = null;

            // Check for cancel
            if (CancelUpdateLibrary)
            {
                // Sends a cancel exception
                throw new OldUpdateLibraryException();
            }
                        
            try
            {
                // Get audio file metadata
                audioFile = new AudioFile(filePath, Guid.NewGuid(), true);
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library -- AddAudioFileToLibrary() error -- Metadata read failed -- " + ex.Message + " -- " + ex.StackTrace);
                UpdateLibraryReportProgress("Adding media to the library", "Metadata read failed", percentCompleted, totalNumberOfFiles, currentFilePosition, "Could not add " + filePath + "; metadata read failed.", filePath, null, ex);
            }

            // Display update
            UpdateLibraryReportProgress("Adding media to the library", "Adding " + filePath, percentCompleted, totalNumberOfFiles, currentFilePosition, "Adding " + filePath, filePath, new UpdateLibraryProgressDataSong { AlbumTitle = audioFile.AlbumTitle, ArtistName = audioFile.ArtistName, Cover = null, SongTitle = audioFile.Title }, null);

            try
            {
                // Insert song into the database                
                //m_gateway.InsertSong(newSong);
                m_gateway.InsertAudioFile(audioFile);
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
            m_gateway.DeleteAudioFiles(folderPath);
        }

        #region Select (strings)

        /// <summary>
        /// Returns the unique artist names in the library.
        /// </summary>        
        /// <returns>List of artist names</returns>
        public List<string> SelectArtistNames()
        {
            return SelectArtistNames(FilterSoundFormat.All);
        }

        /// <summary>
        /// Returns the unique artist names in the library, using the filter passed in the soundFormat parameter.
        /// </summary>
        /// <param name="soundFormat">Sound Format Filter</param>
        /// <returns>List of artist names</returns>
        public List<string> SelectArtistNames(FilterSoundFormat soundFormat)
        {
            //return DataAccess.SelectDistinctArtistNames(soundFormat);
            return m_gateway.SelectDistinctArtistNames(soundFormat);
        }

        /// <summary>
        /// Returns the unique album titles of a specific artist in the library.
        /// </summary>
        /// <param name="artistName">Artist name</param>        
        /// <returns>List of album titles</returns>    
        public List<string> SelectArtistAlbumTitles(string artistName)
        {
            return SelectArtistAlbumTitles(artistName, FilterSoundFormat.All);
        }

        /// <summary>
        /// Returns the unique album titles of a specific artist in the library, using the filter passed in the soundFormat parameter.
        /// </summary>
        /// <param name="artistName">Artist Name</param>
        /// <param name="soundFormat">AudioFileType filter</param>
        /// <returns>List of album titles</returns>        
        public List<string> SelectArtistAlbumTitles(string artistName, FilterSoundFormat soundFormat)
        {
            List<string> albums = null;

            try
            {
                // Basic request
                IEnumerable<AudioFile> queryAudioFiles = from s in AudioFiles
                                                         select s;

                // Do we have to filter by sound format?
                if (soundFormat != FilterSoundFormat.All)
                {
                    // Get file type
                    AudioFileType fileType = AudioFileType.Unknown;
                    if (soundFormat == FilterSoundFormat.FLAC)
                    {
                        fileType = AudioFileType.FLAC;
                    }
                    else if (soundFormat == FilterSoundFormat.MP3)
                    {
                        fileType = AudioFileType.MP3;
                    }
                    else if (soundFormat == FilterSoundFormat.OGG)
                    {
                        fileType = AudioFileType.OGG;
                    }
                    queryAudioFiles = queryAudioFiles.Where(x => x.FileType == fileType);
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
                throw ex;
            }

            return albums;
        }

        /// <summary>
        /// Returns a list of distinct album titles.
        /// </summary>
        /// <returns>List of album titles</returns>
        public Dictionary<string, List<string>> SelectAlbumTitles()
        {
            return SelectAlbumTitles(FilterSoundFormat.All);
        }

        /// <summary>
        /// Returns a list of distinct album titles, using the filter passed in the soundFormat parameter.
        /// </summary>
        /// <param name="soundFormat">Sound Format Filter</param>
        /// <returns>List of album titles</returns>
        public Dictionary<string, List<string>> SelectAlbumTitles(FilterSoundFormat soundFormat)
        {
            //return DataAccess.SelectDistinctAlbumTitles(soundFormat);
            return m_gateway.SelectDistinctAlbumTitles(soundFormat);
        }

        /// <summary>
        /// Returns a list of distinct album titles with the file path of one of the audio files
        /// of the album. This method is useful to display album covers.
        /// </summary>
        /// <returns>List of album titles with file path</returns>
        public Dictionary<string, string> SelectAlbumTitlesWithFilePaths()
        {
            return SelectAlbumTitlesWithFilePaths(FilterSoundFormat.All);
        }

        /// <summary>
        /// Returns a list of distinct album titles with the file path of one of the audio files
        /// of the album, using the filter passed in the soundFormat parameter.
        /// </summary>
        /// <param name="soundFormat">Sound Format Filter</param>
        /// <returns>List of album titles with file path</returns>
        public Dictionary<string, string> SelectAlbumTitlesWithFilePaths(FilterSoundFormat soundFormat)
        {
            //return DataAccess.SelectDistinctAlbumTitlesWithFilePaths(soundFormat);
            return m_gateway.SelectDistinctAlbumTitlesWithFilePaths(soundFormat);
        }

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
                audioFile = AudioFiles.FirstOrDefault(s => s.Id == audioFileId);
            }
            catch (Exception ex)
            {
                Tracing.Log(ex);
                throw ex;
            }

            return audioFile;
        }

        /// <summary>
        /// Selects all the audio files from the cache.
        /// </summary>
        /// <returns>List of AudioFiles</returns>
        public List<AudioFile> SelectAudioFiles()
        {
            return SelectAudioFiles(FilterSoundFormat.All, string.Empty, true, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Selects audio files from the cache, filtered by parameters.
        /// </summary>
        /// <param name="soundFormat">Sound Format Filter</param>
        /// <returns>List of AudioFiles</returns>
        public List<AudioFile> SelectAudioFiles(FilterSoundFormat soundFormat)
        {
            return SelectAudioFiles(soundFormat, string.Empty, true, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Selects audio files from the cache, filtered by parameters.
        /// </summary>
        /// <param name="soundFormat">Sound Format Filter</param>
        /// <returns>List of AudioFiles</returns>
        public List<AudioFile> SelectAudioFiles(FilterSoundFormat soundFormat, string orderBy, bool orderByAscending)
        {
            return SelectAudioFiles(soundFormat, orderBy, orderByAscending, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Selects audio files from the cache, filtered by parameters.
        /// </summary>
        /// <param name="soundFormat">Sound Format Filter</param>
        /// <param name="artistName">Artist Name</param>
        /// <returns>List of AudioFiles</returns>
        public List<AudioFile> SelectAudioFiles(FilterSoundFormat soundFormat, string orderBy, bool orderByAscending, string artistName)
        {
            return SelectAudioFiles(soundFormat, orderBy, orderByAscending, artistName, string.Empty, string.Empty);
        }

        /// <summary>
        /// Selects audio files from the cache, filtered by parameters.
        /// </summary>
        /// <param name="soundFormat">Sound Format Filter</param>
        /// <param name="artistName">Artist Name</param>
        /// <param name="albumTitle">Album Title</param>
        /// <returns>List of AudioFiles</returns>
        public List<AudioFile> SelectAudioFiles(FilterSoundFormat soundFormat, string orderBy, bool orderByAscending, string artistName, string albumTitle)
        {
            return SelectAudioFiles(soundFormat, orderBy, orderByAscending, artistName, albumTitle, string.Empty);
        }

        /// <summary>
        /// Selects the songs from the song cache, filtered by the sound format, artist name,
        /// album title and song title passed in parameter.
        /// </summary>
        /// <param name="soundFormat">Sound Format Filter</param>
        /// <param name="artistName">Artist Name</param>
        /// <param name="albumTitle">Album Title</param>
        /// <param name="songTitle">Song Title</param>
        /// <returns>List of AudioFiles</returns>
        public List<AudioFile> SelectAudioFiles(FilterSoundFormat soundFormat, string orderBy, bool orderByAscending, string artistName, string albumTitle, string songTitle)
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
                    queryAudioFiles = from s in AudioFiles
                                      orderby s.ArtistName, s.AlbumTitle, s.DiscNumber, s.TrackNumber
                                      select s;
                }
                else
                {
                    // Check for orderby ascending/descending
                    if (orderByAscending)
                    {
                        // Set query
                        queryAudioFiles = from s in AudioFiles
                                          orderby GetPropertyValue(s, orderBy)
                                          select s;
                    }
                    else
                    {
                        // Set query
                        queryAudioFiles = from s in AudioFiles
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

                // Check if songTitle is null
                if (!String.IsNullOrEmpty(songTitle))
                {
                    // Add the artist condition to the query
                    queryAudioFiles = queryAudioFiles.Where(s => s.Title == songTitle);                    
                }

                // Check for media filter
                if (soundFormat != FilterSoundFormat.All)
                {
                    // Get file type
                    AudioFileType fileType = AudioFileType.Unknown;
                    if (soundFormat == FilterSoundFormat.FLAC)
                    {
                        fileType = AudioFileType.FLAC;
                    }
                    else if (soundFormat == FilterSoundFormat.MP3)
                    {
                        fileType = AudioFileType.MP3;
                    }
                    else if (soundFormat == FilterSoundFormat.OGG)
                    {
                        fileType = AudioFileType.OGG;
                    }
                    queryAudioFiles = queryAudioFiles.Where(s => s.FileType == fileType);
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
                throw ex;
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

        /// <summary>
        /// Returns the album art in the ID3 tags, or the folder.jpg
        /// image if the tags are empty.
        /// </summary>
        /// <param name="filePath">Media file path</param>
        /// <returns>Image</returns>
        public static Image GetAlbumArtFromID3OrFolder(string filePath)
        {
            // Declare variables
            Image image = null;

            try
            {
                // Get ID3 tags from file
                TagLib.File file = TagLib.File.Create(filePath);

                // Can we get the image from the ID3 tags?
                if (file != null && file.Tag != null && file.Tag.Pictures != null && file.Tag.Pictures.Length > 0)
                {
                    // Get image from ID3 tags
                    ImageConverter ic = new ImageConverter();
                    image = (System.Drawing.Image)ic.ConvertFrom(file.Tag.Pictures[0].Data.Data);
                }

                // If an image was found, return the image. If not, use folder.jpg
                if (image != null)
                {
                    return image;
                }
            }
            catch (Exception ex)
            {
                // The TagLib# read has failed. Continue to try to get the image from folder.jpg.
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
                        image = Image.FromFile(imagePath);
                    }
                    catch (Exception ex)
                    {
                        // The image format is wrong or the image is corrupted. Return null.
                        return null;
                    }
                }
            }

            return image;
        }

        #endregion

        /// <summary>
        /// Resets the library (deletes all songs from the database).
        /// </summary>
        public void ResetLibrary()
        {
            // Reset library            
            m_gateway.ResetLibrary();

            // Compact database            
            m_gateway.CompactDatabase();

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
            m_gateway.UpdatePlayCount(audioFileId);

            // Update play count in cache                  
            AudioFile audioFile = m_audioFiles.SingleOrDefault(x => x.Id == audioFileId);
            if (audioFile != null)
            {
                audioFile = m_gateway.SelectAudioFile(audioFile.Id);
            }

            ////string strSongId = songId.ToString();
            //SongDTO song = Songs.SingleOrDefault(x => x.SongId == songId);
            //int indexOf = Songs.IndexOf(song);
            ////Songs[indexOf] = ConvertDTO.ConvertSong(DataAccess.SelectSong(songId));
            //Songs[indexOf] = m_gateway.SelectSong(songId);
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

    /// <summary>
    /// Defines a custom exception for the Update Library background process.
    /// </summary>
    public class OldUpdateLibraryException : Exception
    {

    }

    /// <summary>
    /// Defines the data structure for the Update Library background process progress event.
    /// </summary>
    public class OldUpdateLibraryProgressData
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public double Percentage { get; set; }
        public int TotalNumberOfFiles { get; set; }
        public int CurrentFilePosition { get; set; }
        public string FilePath { get; set; }        
        
        public Exception Error;

        public string LogEntry { get; set; }
        public UpdateLibraryProgressDataSong Song { get; set; }
    }

    /// <summary>
    /// Defines the data for a song passed in the Update Library background progress data structure.
    /// </summary>
    public class UpdateLibraryProgressDataSong
    {
        public string ArtistName { get; set; }
        public string AlbumTitle { get; set; }
        public string SongTitle { get; set; }
        public Image Cover { get; set; }
    }

    /// <summary>
    /// Defines the data structure for the Update Library finished event.
    /// </summary>
    public class UpdateLibraryFinishedData
    {
        public bool Successful { get; set; }
        public bool Cancelled { get; set; }
    }

    /// <summary>
    /// Arguments for the background worker that updates the library.
    /// </summary>
    public class UpdateLibraryArgument
    {
        // Properties
        public UpdateLibraryMode Mode { get; set; }
        public List<string> FilePaths { get; set; }
        public string FolderPath { get; set; }

        /// <summary>
        /// Constructor for UpdateLibraryArguments.
        /// </summary>
        public UpdateLibraryArgument()
        {
            // Set default arguments
            Mode = UpdateLibraryMode.WholeLibrary;
            FilePaths = new List<string>();
            FolderPath = string.Empty;
        }
    }

    /// <summary>
    /// Defines the type of sound formats.
    /// </summary>
    public enum FilterSoundFormat
    {
        All = 0, MP3 = 1, FLAC = 2, OGG = 3
    }

    /// <summary>
    /// Defines the modes of the Update Library process.
    /// </summary>
    public enum UpdateLibraryMode
    {
        WholeLibrary = 0, SpecificFiles = 1, SpecificFolder = 2
    }
}
