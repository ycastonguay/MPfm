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
using System.Text;
using MPfm.Core;
using MPfm.Sound;
using MPfm.Sound.FMODWrapper;
using MPfm.Library.Data;

namespace MPfm.Library
{
    /// <summary>
    /// The Library class is a cache of the song library. It can update the library in a background worker.
    /// It uses the DataAccess class to access the PMP database.
    /// </summary>
    public class Library
    {   
        // Update library progress delegate/event
        public delegate void UpdateLibraryProgress(UpdateLibraryProgressData data);
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

        private List<SongDTO> m_songs = null;
        /// <summary>
        /// Local cache of the song library.
        /// </summary>
        public List<SongDTO> Songs
        {
            get
            {
                return m_songs;
            }
        }

        private Player m_player = null;
        /// <summary>
        /// Pointer to the Player instance. Needed to access the base sound system for FMOD tagging.
        /// </summary>
        public Player Player
        {
            get
            {
                return m_player;
            }
        }

        /// <summary>
        /// Constructs the Library class. Requires a pointer to an instance of a Player class.
        /// </summary>        
        /// <param name="player">Pointer to an instance of the Player class</param>
        public Library(Player player)
        {
            // Set private variables
            m_player = player;

            // Create worker process
            workerUpdateLibrary = new BackgroundWorker();
            workerUpdateLibrary.WorkerReportsProgress = true;
            workerUpdateLibrary.WorkerSupportsCancellation = true;
            workerUpdateLibrary.DoWork += new DoWorkEventHandler(workerUpdateLibrary_DoWork);
            workerUpdateLibrary.RunWorkerCompleted += new RunWorkerCompletedEventHandler(workerUpdateLibrary_RunWorkerCompleted);            

            // Refresh songs
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
                List<Folder> folders = DataAccess.SelectFolders();

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
                        DataAccess.DeleteFolder(new Guid(folder.FolderId));
                    }
                }

                // Add the folder to the list of configured folders
                if (!folderFound)
                {
                    // Add folder to database
                    DataAccess.InsertFolder(folderPath, true);
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
                    RemoveBrokenSongs();

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

                // Make a list of file names
                List<string> filePaths = new List<string>();
                foreach (SongDTO song in Songs)
                {
                    filePaths.Add(song.FilePath);
                }

                // Compare list of files from database with list of files found on hard disk
                List<string> soundFilesToUpdate = mediaFiles.Except(filePaths).ToList();

                // Cancel thread if necessary
                if (CancelUpdateLibrary) throw new UpdateLibraryException();

                // Add new media (if media found!)
                if (mediaFiles.Count > 0)
                {
                    AddSoundFilesToLibrary(soundFilesToUpdate);
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
                DataAccess.CompactDatabase();
            }
            catch (UpdateLibraryException ex)
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
        /// Refreshes the song cache with the database values.
        /// </summary>
        public void RefreshCache()
        {
            // Refresh song cache
            Tracing.Log("MPfm.Library (Library) --  Refreshing song cache...");
            m_songs = ConvertDTO.ConvertSongs(DataAccess.SelectSongs());
        }        

        /// <summary>
        /// Removes the songs that do not exist anymore on the hard drive.
        /// </summary>
        public void RemoveBrokenSongs()
        {
            // Get all songs
            List<Song> songs = DataAccess.SelectSongs();

            // For each song
            for(int a = 0; a < songs.Count; a++)
            {
                // Check for cancel
                if (CancelUpdateLibrary)
                {
                    // Sends a cancel exception
                    throw new UpdateLibraryException();
                }

                // If the file doesn't exist, delete the song
                if (!File.Exists(songs[a].FilePath))
                {
                    Tracing.Log("Removing broken songs..." + songs[a].FilePath);
                    UpdateLibraryReportProgress("Removing broken songs...", songs[a].FilePath, (double)((double)a / (double)songs.Count) * 100);
                    DataAccess.DeleteSong(new Guid(songs[a].SongId));
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
            List<Folder> folders = DataAccess.SelectFolders();

            // For each registered folder
            foreach (Folder folder in folders)
            {
                // Search for media files in the folder
                UpdateLibraryReportProgress("Searching for media files", "Searching for media files in library folder " + folder.FolderPath);
                List<string> newSongs = SearchMediaFilesInFolders(folder.FolderPath, (bool)folder.Recursive);
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
        /// Adds sound files into the library. Must specify the list of files to add
        /// using the soundFiles parameter.
        /// </summary>
        /// <param name="soundFiles">List of sound file paths to add</param>        
        public void AddSoundFilesToLibrary(List<string> soundFiles)
        {
            int addNewFilesCount = 0;

            foreach (string file in soundFiles)
            {
                // Check for cancel
                if (CancelUpdateLibrary)
                {
                    // Sends a cancel exception
                    throw new UpdateLibraryException();
                }

                addNewFilesCount++;
                double percentCompleted = ((double)addNewFilesCount / (double)soundFiles.Count) * 100;

                // Add MP3 to the library
                AddSoundFileToLibrary(file, percentCompleted, soundFiles.Count, addNewFilesCount);
            }
        }

        /// <summary>
        /// Adds a sound file to the library. Supports MP3, FLAC and OGG sound files.
        /// </summary>
        /// <param name="filePath">Sound file path</param>
        /// <param name="percentCompleted">Percent completed value (for updating progress)</param>
        /// <param name="totalNumberOfFiles">Total number of files (for updating progress)</param>
        /// <param name="currentFilePosition">Current file position (for updating progress)</param>
        private void AddSoundFileToLibrary(string filePath, double percentCompleted, int totalNumberOfFiles, int currentFilePosition)
        {                
            // Check for cancel
            if (CancelUpdateLibrary)
            {
                // Sends a cancel exception
                throw new UpdateLibraryException();
            }

            // Create song and set default properties
            Song newSong = new Song();
            newSong.SongId = Guid.NewGuid().ToString();
            newSong.FilePath = filePath;            
            newSong.PlayCount = 0;
            newSong.Rating = -1;
            newSong.Tempo = 120;

            // Get the format of the file using the file extension
            string extension = Path.GetExtension(filePath).ToUpper();                        
            newSong.SoundFormat = extension.Replace(".", "");

            try
            {
                // Update song properties using tags
                newSong = UpdateSongFromTags(newSong, false);
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library -- UpdateLibrary -- AddSoundFileToLibrary() error -- UpdateSongFromTags failed -- " + ex.Message + " -- " + ex.StackTrace);
                UpdateLibraryReportProgress("Adding media to the library", "Error: " + ex.Message, percentCompleted, totalNumberOfFiles, currentFilePosition, "Error reading " + filePath + ".\nMessage: " + ex.Message + "\nStack trace: " + ex.StackTrace, filePath, null, ex);
                return;
            }

            // If the song has no name, give filename as the name                
            if (String.IsNullOrEmpty(newSong.Title))
            {
                newSong.Title = Path.GetFileNameWithoutExtension(filePath);
            }
            // If the artist has no name, give it "Unknown Artist"
            if (String.IsNullOrEmpty(newSong.ArtistName))
            {
                newSong.ArtistName = "Unknown Artist";
            }
            // If the song has no album title, give it "Unknown Album"
            if (String.IsNullOrEmpty(newSong.AlbumTitle))
            {
                newSong.AlbumTitle = "Unknown Album";
            }

            // Display update
            UpdateLibraryReportProgress("Adding media to the library", "Adding " + filePath, percentCompleted, totalNumberOfFiles, currentFilePosition, "Adding " + filePath, filePath, new UpdateLibraryProgressDataSong { AlbumTitle = newSong.AlbumTitle, ArtistName = newSong.ArtistName, Cover = null, SongTitle = newSong.Title }, null);

            try
            {
                // Insert song into the database
                DataAccess.InsertSong(newSong);
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library -- UpdateLibrary -- AddMP3ToLibrary() error -- Database insertion failed -- " + ex.Message + " -- " + ex.StackTrace);
                UpdateLibraryReportProgress("Adding media to the library", "Database insertion failed", percentCompleted, totalNumberOfFiles, currentFilePosition, "Could not add " + filePath + "; database insertion failed.", filePath, null, ex);
            }
        }

        ///// <summary>
        ///// Adds a FLAC media to the library, from a file path.
        ///// </summary>
        ///// <param name="filePath">Path to the song</param>
        ///// <param name="percentCompleted">Percent completed value (for updating progress)</param>
        ///// <param name="totalNumberOfFiles">Total number of files (for updating progress)</param>
        ///// <param name="currentFilePosition">Current file position (for updating progress)</param>
        //private void AddFLACToLibrary(string filePath, double percentCompleted, int totalNumberOfFiles, int currentFilePosition)
        //{
        //    // Check for cancel
        //    if (CancelUpdateLibrary)
        //    {
        //        // Sends a cancel exception
        //        throw new UpdateLibraryException();
        //    }

        //    // Create song and set default properties
        //    Song newSong = new Song();
        //    newSong.SongId = Guid.NewGuid().ToString();
        //    newSong.FilePath = filePath;
        //    newSong.SoundFormat = "FLAC";           
        //    newSong.PlayCount = 0;
        //    newSong.Rating = -1;
        //    newSong.Tempo = 120;

        //    try
        //    {
        //        // Update song properties using tags
        //        newSong = UpdateSongFromTags(newSong, false);
        //    }
        //    catch (Exception ex)
        //    {
        //        Tracing.Log("MPfm.Library -- UpdateLibrary -- AddFLACToLibrary() error -- UpdateSongFromTags failed -- " + ex.Message + " -- " + ex.StackTrace);
        //        UpdateLibraryReportProgress("Adding media to the library", "Error: " + ex.Message, percentCompleted, totalNumberOfFiles, currentFilePosition, "Error reading " + filePath + ".\nMessage: " + ex.Message + "\nStack trace: " + ex.StackTrace, filePath, null, ex);
        //        return;
        //    }

        //    // If the song has no name, give filename as the name                
        //    if (String.IsNullOrEmpty(newSong.Title))
        //    {
        //        newSong.Title = Path.GetFileNameWithoutExtension(filePath);
        //    }
        //    // If the artist has no name, give it "Unknown Artist"
        //    if (String.IsNullOrEmpty(newSong.ArtistName))
        //    {
        //        newSong.ArtistName = "Unknown Artist";
        //    }
        //    // If the song has no album title, give it "Unknown Album"
        //    if (String.IsNullOrEmpty(newSong.AlbumTitle))
        //    {
        //        newSong.AlbumTitle = "Unknown Album";
        //    }

        //    // Display update
        //    UpdateLibraryReportProgress("Adding media to the library", "Adding " + filePath, percentCompleted, totalNumberOfFiles, currentFilePosition, "Adding " + filePath, filePath, new UpdateLibraryProgressDataSong { AlbumTitle = newSong.AlbumTitle, ArtistName = newSong.ArtistName, Cover = null, SongTitle = newSong.Title }, null);

        //    try
        //    {
        //        // Insert song into the database
        //        DataAccess.InsertSong(newSong);
        //    }
        //    catch (Exception ex)
        //    {
        //        Tracing.Log("MPfm.Library -- UpdateLibrary -- AddFLACToLibrary() error -- Database insertion failed -- " + ex.Message + " -- " + ex.StackTrace);
        //        UpdateLibraryReportProgress("Adding media to the library", "Database insertion failed", percentCompleted, totalNumberOfFiles, currentFilePosition, "Could not add " + filePath + "; database insertion failed.", filePath, null, ex);
        //    }
        //}

        #endregion

        /// <summary>
        /// Updates song properties based on the file tags. Reads the tags using TagLib#. If this
        /// fails, MPfm uses the FMOD tagging system. If you want to update the database, set
        /// updateDatabase to true.
        /// </summary>
        /// <param name="song">Song (from EF)</param>
        /// <param name="updateDatabase">Update database if true</param>
        /// <returns>Song with updated properties</returns>
        public Song UpdateSongFromTags(Song song, bool updateDatabase)
        {
            // Create temporary sound
            MPfm.Sound.FMODWrapper.Sound sound = Player.SoundSystem.CreateSound(song.FilePath, false);

            // Get tags
            Tags tags = sound.GetTags();

            // Set song properties                            
            song.ArtistName = tags.ArtistName;
            song.AlbumTitle = tags.AlbumTitle;
            song.Title = tags.Title;
            song.Genre = tags.Genre;
            song.Time = sound.Length;      

            long trackNumber = 0;
            long.TryParse(tags.TrackNumber, out trackNumber);
            song.TrackNumber = trackNumber;

            long discNumber = 0;
            long.TryParse(tags.DiscNumber, out discNumber);
            song.DiscNumber = discNumber;                             

            // Release sound
            sound.Release();

            try
            {
                // Update song in database?
                if (updateDatabase)
                {
                    // Update song
                    DataAccess.UpdateSong(song);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return song;
        }

        /// <summary>
        /// Removes the songs in the library that match the path passed in parameter.        
        /// </summary>
        /// <param name="folderPath">Folder path</param>
        public void RemoveSongsFromLibrary(string folderPath)
        {
            // Simple: Just DELETE Songs WHERE FilePath LIKE 'Filepath%'
            DataAccess.DeleteSongs(folderPath);
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
            return DataAccess.SelectDistinctArtistNames(soundFormat);
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
        /// <param name="artistName">Artist name</param>
        /// <param name="soundFormat">Sound Format Filter</param>
        /// <returns>List of album titles</returns>        
        public List<string> SelectArtistAlbumTitles(string artistName, FilterSoundFormat soundFormat)
        {
            List<string> albums = null;

            try
            {
                // Basic request
                IEnumerable<SongDTO> querySongs = from s in Songs
                                                  select s;

                // Do we have to filter by sound format?
                if (soundFormat != FilterSoundFormat.All)
                {
                    querySongs = querySongs.Where(x => x.SoundFormat == soundFormat.ToString());
                }

                // Query: get all albums from artist from the Songs table
                var query = from song in querySongs
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
            return DataAccess.SelectDistinctAlbumTitles(soundFormat);
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
            return DataAccess.SelectDistinctAlbumTitlesWithFilePaths(soundFormat);
        }

        #endregion

        #region Select (songs)

        /// <summary>
        /// Selects a song from the cache using its identifier.
        /// </summary>
        /// <param name="songId">Song Identifier</param>
        /// <returns>Song (SongDTO)</returns>
        public SongDTO SelectSong(Guid songId)
        {
            SongDTO song = null;

            try
            {
                // Get song
                song = Songs.Where(s => s.SongId == songId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (Library) --  Error in SelectSong(): " + ex.Message);
                throw ex;
            }

            //return ConvertDTO.ConvertSong(song);
            return song;
        }

        /// <summary>
        /// Selects all the songs from the song cache.
        /// </summary>
        /// <returns>List of songs</returns>
        public List<SongDTO> SelectSongs()
        {
            return SelectSongs(FilterSoundFormat.All, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Selects the songs from the song cache, filtered by the sound format passed in parameter.
        /// </summary>
        /// <param name="soundFormat">Sound Format Filter</param>
        /// <returns>List of songs</returns>
        public List<SongDTO> SelectSongs(FilterSoundFormat soundFormat)
        {
            return SelectSongs(soundFormat, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// Selects the songs from the song cache, filtered by the sound format and artist
        /// name passed in parameter.
        /// </summary>
        /// <param name="soundFormat">Sound Format Filter</param>
        /// <param name="artistName">Artist Name</param>
        /// <returns>List of songs</returns>
        public List<SongDTO> SelectSongs(FilterSoundFormat soundFormat, string artistName)
        {
            return SelectSongs(soundFormat, artistName, string.Empty, string.Empty);
        }

        /// <summary>
        /// Selects the songs from the song cache, filtered by the sound format, artist name
        /// and album title passed in parameter.
        /// </summary>
        /// <param name="soundFormat">Sound Format Filter</param>
        /// <param name="artistName">Artist Name</param>
        /// <param name="albumTitle">Album Title</param>
        /// <returns>List of songs</returns>
        public List<SongDTO> SelectSongs(FilterSoundFormat soundFormat, string artistName, string albumTitle)
        {
            return SelectSongs(soundFormat, artistName, albumTitle, string.Empty);
        }

        /// <summary>
        /// Selects the songs from the song cache, filtered by the sound format, artist name,
        /// album title and song title passed in parameter.
        /// </summary>
        /// <param name="soundFormat">Sound Format Filter</param>
        /// <param name="artistName">Artist Name</param>
        /// <param name="albumTitle">Album Title</param>
        /// <param name="songTitle">Song Title</param>
        /// <returns>List of songs</returns>
        public List<SongDTO> SelectSongs(FilterSoundFormat soundFormat, string artistName, string albumTitle, string songTitle)
        {
            // Create variables
            List<SongDTO> songs = null;

            try
            {
                IEnumerable<SongDTO> querySongs = from s in Songs                            
                                                  orderby s.ArtistName, s.AlbumTitle, s.DiscNumber, s.TrackNumber
                                                  select s;

                // Check if artistName is null
                if (!String.IsNullOrEmpty(artistName))
                {
                    // Add the artist condition to the query
                    querySongs = querySongs.Where(s => s.ArtistName == artistName);                    
                }

                // Check if albumTitle is null
                if (!String.IsNullOrEmpty(albumTitle))
                {
                    // Add the artist condition to the query
                    querySongs = querySongs.Where(s => s.AlbumTitle == albumTitle);                    
                }

                // Check if songTitle is null
                if (!String.IsNullOrEmpty(songTitle))
                {
                    // Add the artist condition to the query
                    querySongs = querySongs.Where(s => s.Title == songTitle);                    
                }

                // Check for media filter
                if (soundFormat != FilterSoundFormat.All)
                {
                    querySongs = querySongs.Where(s => s.SoundFormat == soundFormat.ToString());
                }

                // Execute query
                songs = querySongs.ToList();
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (Library) --  Error in SelectSongs(): " + ex.Message);
                throw ex;
            }

            //return ConvertDTO.ConvertSongs(songs);
            return songs;
        }

        /// <summary>
        /// Selects the songs from a specific playlist.
        /// </summary>
        /// <returns>List of songs</returns>
        public List<SongDTO> SelectSongs(Guid playlistId)
        {
            // Create variables
            List<SongDTO> dtos = new List<SongDTO>();

            // Fetch playlist songs from the database
            List<PlaylistSong> playlistSongs = DataAccess.SelectPlaylistSongs(playlistId);

            // Loop through playlist songs
            foreach (PlaylistSong playlistSong in playlistSongs)
            {
                // Fetch the song from the cache
                Guid songId = new Guid(playlistSong.SongId);
                SongDTO song = Songs.FirstOrDefault(x => x.SongId == songId);

                // Convert to DTO and add to collection
                //SongDTO dto = ConvertDTO.ConvertSong(song);
                dtos.Add(song);
            }

            return dtos;
        }

        #endregion

        #region Generate Playlist

        /// <summary>
        /// Generates a playlist containing all songs from the library.
        /// </summary>
        /// <param name="soundFormat">Sound format filter</param>        
        /// <returns>Playlist</returns>
        public PlaylistDTO GeneratePlaylistAll(FilterSoundFormat soundFormat)
        {
            // Create playlist
            PlaylistDTO playlist = new PlaylistDTO();            
            playlist.PlaylistName = "All songs";
            playlist.PlaylistType = PlaylistType.All;

            // Get songs for this artist
            List<SongDTO> songs = SelectSongs(soundFormat);

            // Transform songs into playlist songs
            foreach (SongDTO song in songs)
            {
                // Create playlist song
                PlaylistSongDTO playlistSong = new PlaylistSongDTO();
                //playlistSong.PlaylistId = playlist.PlaylistId;
                playlistSong.Song = song;

                // Add playlist song to list
                playlist.Songs.Add(playlistSong);
            }

            // Set first song
            if (playlist.Songs.Count > 0)
            {
                playlist.CurrentSong = playlist.Songs[0];
            }

            return playlist;
        }

        /// <summary>
        /// Generates a playlist containing all songs from a specific artist.
        /// </summary>
        /// <param name="soundFormat">Sound format filter</param>
        /// <param name="artistName">Artist name</param>
        /// <returns>Playlist</returns>
        public PlaylistDTO GeneratePlaylistFromArtist(FilterSoundFormat soundFormat, string artistName)
        {
            // Create playlist
            PlaylistDTO playlist = new PlaylistDTO();
            playlist.PlaylistName = "All songs from " + artistName;
            playlist.PlaylistType = PlaylistType.Artist;           

            // Get songs for this artist
            List<SongDTO> songs = SelectSongs(soundFormat, artistName);

            // Transform songs into playlist songs
            foreach (SongDTO song in songs)
            {
                // Create playlist song
                PlaylistSongDTO playlistSong = new PlaylistSongDTO();
                //playlistSong.PlaylistId = playlist.PlaylistId;
                playlistSong.Song = song;

                // Add playlist song to list
                playlist.Songs.Add(playlistSong);
            }

            // Set first song
            if (playlist.Songs.Count > 0)
            {
                playlist.CurrentSong = playlist.Songs[0];
            }

            return playlist;           
        }

        /// <summary>
        /// Generates a playlist containing all songs from a specific album.
        /// </summary>
        /// <param name="soundFormat">Sound format filter</param>
        /// <param name="artistName">Artist name</param>
        /// <param name="albumTitle">Album title</param>
        /// <returns>Playlist</returns>
        public PlaylistDTO GeneratePlaylistFromAlbum(FilterSoundFormat soundFormat, string artistName, string albumTitle)
        {
            // Create playlist
            PlaylistDTO playlist = new PlaylistDTO();
            playlist.PlaylistName = "All songs from " + artistName + "'s " + albumTitle;
            playlist.PlaylistType = PlaylistType.Album;

            // Get songs for this artist
            List<SongDTO> songs = SelectSongs(soundFormat, artistName, albumTitle);

            // Transform songs into playlist songs
            foreach (SongDTO song in songs)
            {
                // Create playlist song
                PlaylistSongDTO playlistSong = new PlaylistSongDTO();
                //playlistSong.PlaylistId = playlist.PlaylistId;
                playlistSong.Song = song;

                // Add playlist song to list
                playlist.Songs.Add(playlistSong);
            }

            // Set first song
            if (playlist.Songs.Count > 0)
            {
                playlist.CurrentSong = playlist.Songs[0];
            }

            return playlist;
        }

        #endregion

        #region Playlists

        /// <summary>
        /// Returns all the playlists from the database.
        /// </summary>
        /// <param name="includePlaylistSongs">If true, the playlist songs are included</param>
        /// <returns>List of playlists</returns>
        public List<PlaylistDTO> SelectPlaylists(bool includePlaylistSongs)
        {
            // Create the list of DTO
            List<PlaylistDTO> dtos = new List<PlaylistDTO>();

            // Get the playlists from the database
            List<Playlist> playlists = DataAccess.SelectPlaylists();

            // For each playlist
            foreach (Playlist playlist in playlists)
            {
                // Fetch the playlist songs from the database, if specified
                List<PlaylistSong> playlistSongs = null;
                if (includePlaylistSongs)
                {
                    // Fetch the playlist songs from the database
                    playlistSongs = DataAccess.SelectPlaylistSongs(new Guid(playlist.PlaylistId));
                }

                // Convert into DTO and add to list
                PlaylistDTO dto = ConvertDTO.ConvertPlaylist(playlist, playlistSongs);
                dtos.Add(dto);
            }

            return dtos;
        }

        /// <summary>
        /// Selects a playlist from the database, using its identifier.
        /// </summary>
        /// <param name="playlistId">Playlist Identifier</param>
        /// <returns>PlaylistDTO</returns>
        public PlaylistDTO SelectPlaylist(Guid playlistId)
        {
            PlaylistDTO dto = null;

            // Get playlist from database
            Playlist playlist = DataAccess.SelectPlaylist(playlistId);
            List<PlaylistSong> playlistSongs = DataAccess.SelectPlaylistSongs(playlistId);

            // If not not, convert to dto
            if (playlist != null)
            {
                // Convert to dto
                dto = ConvertDTO.ConvertPlaylist(playlist, null);

                // Check if there are playlist songs
                if (playlistSongs != null)
                {
                    // Loop through playlist songs
                    foreach (PlaylistSong playlistSong in playlistSongs)
                    {
                        // Create DTO
                        PlaylistSongDTO playlistSongDTO = new PlaylistSongDTO();
                        playlistSongDTO.PlaylistSongId = new Guid(playlistSong.PlaylistSongId);

                        // Fetch song from cache
                        Guid songId = new Guid(playlistSong.SongId);
                        playlistSongDTO.Song = Songs.FirstOrDefault(x => x.SongId == songId);

                        // Add song to list
                        dto.Songs.Add(playlistSongDTO);
                    }
                }
            }

            return dto;
        }

        /// <summary>
        /// Saves a playlist and its playlist songs into the database.
        /// </summary>
        /// <param name="playlist">Playlist</param>
        public void SavePlaylist(PlaylistDTO playlist)
        {
            // Flag the playlist as custom
            playlist.PlaylistType = PlaylistType.Custom;

            // Select playlist from database
            Playlist playlistEF = DataAccess.SelectPlaylist(playlist.PlaylistId);

            // Check if the playlist is null
            if (playlistEF == null)
            {
                // The playlist doesn't exist in the database; we need to do an INSERT
                playlistEF = new Playlist();
                playlistEF.PlaylistId = playlist.PlaylistId.ToString();
                playlistEF.PlaylistName = playlist.PlaylistName;

                // Insert the playlist
                DataAccess.InsertPlaylist(playlistEF);
            }
            else
            {
                // The playlist already exists in the database; we need to do an UPDATE
                playlistEF.PlaylistName = playlist.PlaylistName;

                // Update the playlist
                DataAccess.UpdatePlaylist(playlistEF);

                // Delete all the playlist songs for the playlist
                DataAccess.DeletePlaylistSongs(playlist.PlaylistId);
            }

            // Generate the playlist songs
            List<PlaylistSong> playlistSongs = new List<PlaylistSong>();
            for (int a = 0; a < playlist.Songs.Count; a++)
            {
                // Create playlist song for database
                PlaylistSong playlistSongEF = new PlaylistSong();
                playlistSongEF.PlaylistSongId = playlist.Songs[a].PlaylistSongId.ToString();
                playlistSongEF.PlaylistId = playlist.PlaylistId.ToString();
                playlistSongEF.SongId = playlist.Songs[a].Song.SongId.ToString();
                playlistSongEF.Position = a + 1;
                playlistSongs.Add(playlistSongEF);
            }

            // Insert playlist songs into database
            DataAccess.InsertPlaylistSongs(playlistSongs);
        }

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
            DataAccess.ResetLibrary();

            // Compact database
            DataAccess.CompactDatabase();

            // Refresh cache
            RefreshCache();
        }

        /// <summary>
        /// Updates the play count of a song into the database.
        /// </summary>
        /// <param name="songId">Song Identifier</param>
        public void UpdateSongPlayCount(Guid songId)
        {
            // Update play count in database
            DataAccess.UpdateSongPlayCount(songId);

            // Update play count in cache           
            //string strSongId = songId.ToString();
            SongDTO song = Songs.SingleOrDefault(x => x.SongId == songId);
            int indexOf = Songs.IndexOf(song);
            Songs[indexOf] = ConvertDTO.ConvertSong(DataAccess.SelectSong(songId));
        }
    }

    /// <summary>
    /// Defines a custom exception for the Update Library background process.
    /// </summary>
    public class UpdateLibraryException : Exception
    {

    }

    /// <summary>
    /// Defines the data structure for the Update Library background process progress event.
    /// </summary>
    public class UpdateLibraryProgressData
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
