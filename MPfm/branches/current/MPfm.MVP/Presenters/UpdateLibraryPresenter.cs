//
// UpdateLibraryPresenter.cs: Update Library window presenter.
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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using MPfm.Core;
using MPfm.Library;
using MPfm.Player;
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;
#if (MACOSX || LINUX)
using Mono.Unix;
using Mono.Unix.Native;
#endif

namespace MPfm.MVP
{
	/// <summary>
	/// Update Library window presenter.
	/// </summary>
	public class UpdateLibraryPresenter : IDisposable, IUpdateLibraryPresenter
	{
		// Private variables
		private IUpdateLibraryView view = null;
		private IMainPresenter mainPresenter = null;
		private ILibraryService libraryService = null;		
		private BackgroundWorker workerUpdateLibrary = null;
		private bool cancelUpdateLibrary = false;
		
		#region Constructor and Dispose

		/// <summary>
		/// Initializes a new instance of the <see cref="MPfm.UI.UpdateLibraryPresenter"/> class.
		/// </summary>
		public UpdateLibraryPresenter(IUpdateLibraryView view, IMainPresenter mainPresenter, ILibraryService libraryService)
		{
			// Check for null
			if(view == null)
				throw new ArgumentNullException("The view parameter cannot be null!");
			if(mainPresenter == null)
				throw new ArgumentNullException("The mainPresenter parameter cannot be null!");
			if(libraryService == null)
				throw new ArgumentNullException("The libraryService parameter cannot be null!");

			// Set properties
			this.view = view;
			this.mainPresenter = mainPresenter;
			this.libraryService = libraryService;
					
			// Create worker
			workerUpdateLibrary = new BackgroundWorker();
            workerUpdateLibrary.WorkerReportsProgress = true;
            workerUpdateLibrary.WorkerSupportsCancellation = true;
            workerUpdateLibrary.DoWork += new DoWorkEventHandler(workerUpdateLibrary_DoWork);
            workerUpdateLibrary.RunWorkerCompleted += new RunWorkerCompletedEventHandler(workerUpdateLibrary_RunWorkerCompleted);
		}

		/// <summary>
		/// Releases all resources used by the <see cref="MPfm.UI.UpdateLibraryPresenter"/> object.
		/// </summary>
		/// <remarks>
		/// Call <see cref="Dispose"/> when you are finished using the <see cref="MPfm.UI.UpdateLibraryPresenter"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="MPfm.UI.UpdateLibraryPresenter"/> in an unusable state. After
		/// calling <see cref="Dispose"/>, you must release all references to the <see cref="MPfm.UI.UpdateLibraryPresenter"/>
		/// so the garbage collector can reclaim the memory that the <see cref="MPfm.UI.UpdateLibraryPresenter"/> was occupying.
		/// </remarks>
		public void Dispose()
		{
		}

		#endregion
		
		#region IUpdateLibraryPresenter implementation
			
		public void Cancel()
		{			
			// Cancel process
			cancelUpdateLibrary = true;
		}
	
		public void SaveLog(string filePath)
		{			
		}
			
		#endregion
		
		public void UpdateLibrary(UpdateLibraryMode mode, List<string> filePaths, string folderPath)
		{					
            // Create argument
            UpdateLibraryArgument arg = new UpdateLibraryArgument();
            arg.Mode = mode;
            arg.FilePaths = filePaths;
            arg.FolderPath = folderPath;
			
			// Make sure all private variables are reset
			cancelUpdateLibrary = false;

			// If the mode is SpecificFolder
            if (mode == UpdateLibraryMode.SpecificFolder)
            {
				// Add folder to library
				libraryService.AddFolder(folderPath, true);
            }
			else if(mode == UpdateLibraryMode.SpecificFiles)
			{
				// Add files to library
				libraryService.AddFiles(filePaths);
			}

            // Start the background process using the arguments
            workerUpdateLibrary.RunWorkerAsync(arg);
		}		
		
        protected void workerUpdateLibrary_DoWork(object sender, DoWorkEventArgs e)
        {
			// Declare variables
            List<string> filePaths = new List<string>();    
			
            // Get argument
            UpdateLibraryArgument arg = (UpdateLibraryArgument)e.Argument;
						
			try
			{			
				// Cancel update library process if necessary
            	if (cancelUpdateLibrary) throw new UpdateLibraryException();
				
				// Determine the update library mode
                if (arg.Mode == UpdateLibraryMode.WholeLibrary)
                {
                    // Remove broken songs from the library
                    RefreshStatus("Checking for broken file paths", "Checking if songs have been deleted on your hard disk but not removed from the library...");
                    libraryService.RemoveAudioFilesWithBrokenFilePaths();

                    // Cancel update library process if necessary
                    if (cancelUpdateLibrary) throw new UpdateLibraryException();

                    // Search for new media in the library folders
                    RefreshStatus("Searching for media files", "Searching media files in library folders");
                    filePaths = SearchMediaFilesInFolders();

                    // Update the media count
                    RefreshStatus("Searching media files", filePaths.Count.ToString() + " media files found.");
                }
                else if (arg.Mode == UpdateLibraryMode.SpecificFiles)
                {           
                    // Set the media files passed in the argument
                    filePaths = arg.FilePaths;
                }
                else if (arg.Mode == UpdateLibraryMode.SpecificFolder)
                {
                    // Search the files in the specified folder
                    RefreshStatus("Searching for media files", "Searching media files in " + arg.FolderPath);
                    filePaths = SearchMediaFilesInFolders(arg.FolderPath, true);
                }
				
				// Cancel update library process if necessary
                if (cancelUpdateLibrary) throw new UpdateLibraryException();
				
				// Get the list of audio files from the database
				IEnumerable<string> filePathsDatabase = libraryService.SelectFilePaths();				
				IEnumerable<string> filePathsToUpdate = filePaths.Except(filePathsDatabase);
					
		        // Loop through files
		        for(int a = 0; a < filePathsToUpdate.Count(); a++)
		        {
					// Cancel update library process if necessary
	                if (cancelUpdateLibrary) throw new UpdateLibraryException();							           				
						
					// Get current file path
					string filePath = filePathsToUpdate.ElementAt(a);
					
					try
					{									
		                // Check if this is a playlist file
		                if (filePath.ToUpper().Contains(".M3U") ||
		                    filePath.ToUpper().Contains(".M3U8") ||
		                    filePath.ToUpper().Contains(".PLS") ||
		                    filePath.ToUpper().Contains(".XSPF"))
		                {
		                    // Get playlist file and insert into database
		                    PlaylistFile playlistFile = new PlaylistFile(filePath);
	                    	libraryService.InsertPlaylistFile(playlistFile);
							
		                    // Display update
							RefreshStatus("Adding media to the library", "Adding " + filePath);
		                    //UpdateLibraryReportProgress("Adding media to the library", "Adding " + filePath, percentCompleted, totalNumberOfFiles, currentFilePosition, "Adding " + filePath, filePath, new UpdateLibraryProgressDataSong(), null);
		                }
		                else
		                {
		                    // Get audio file metadata and insert into database
		                    AudioFile audioFile = new AudioFile(filePath, Guid.NewGuid(), true);	                    
	                    	libraryService.InsertAudioFile(audioFile);
	
		                    // Display update
							RefreshStatus("Adding media to the library", "Adding " + filePath);
		                    //UpdateLibraryReportProgress("Adding media to the library", "Adding " + filePath, percentCompleted, totalNumberOfFiles, currentFilePosition, "Adding " + filePath, filePath, new UpdateLibraryProgressDataSong { AlbumTitle = audioFile.AlbumTitle, ArtistName = audioFile.ArtistName, Cover = null, SongTitle = audioFile.Title }, null);
		                }
	
						// Calculate stats
			            double percentCompleted = ((double)a / (double)filePathsToUpdate.Count()) * 100;
					}
					catch
					{
						RefreshStatus("File could not be added!", filePath);
					}
		        }

                // Cancel thread if necessary
                if (cancelUpdateLibrary) throw new UpdateLibraryException();

                // Compact database
                RefreshStatus("Compacting database", "Compacting database...");
                libraryService.CompactDatabase();
			}
			catch (UpdateLibraryException ex)
            {
                RefreshStatus("The update process was canceled: " + ex.Message, "Canceled by user");
                e.Cancel = true;
            }
            catch (Exception ex)
            {
                RefreshStatus("An error has occured: " + ex.Message, ex.StackTrace);
            }			
        }
		
		/// <summary>
        /// Fires when the update library process is over.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event argument</param>
        private void workerUpdateLibrary_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {	
			// Update view
			view.ProcessEnded(e.Cancelled);
        }
		
		/// <summary>
		/// Sends a status update to the view.
		/// </summary>
		/// <param name="title">Title</param>
		/// <param name="subtitle">Subtitle</param>
		private void RefreshStatus(string title, string subtitle)
		{
			// Create entity and update view
			UpdateLibraryEntity entity = new UpdateLibraryEntity();
			entity.Title = title;
			entity.Subtitle = subtitle;
			view.RefreshStatus(entity);
		}
				
        /// <summary>
        /// Searches for songs in all configured folders.
        /// </summary>
        /// <returns>List of songs (file paths)</returns>
        public List<string> SearchMediaFilesInFolders()
        {
            // List of files
            List<string> files = new List<string>();            

            // Get registered folders            
            IEnumerable<Folder> folders = libraryService.SelectFolders();

            // For each registered folder
            foreach (Folder folder in folders)
            {
                // Search for media files in the folder
                RefreshStatus("Searching for media files", "Searching for media files in library folder " + folder.FolderPath);
                List<string> newFiles = SearchMediaFilesInFolders(folder.FolderPath, (bool)folder.IsRecursive);
                files.AddRange(newFiles);
            }

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
			// Declare variables
            List<string> arrayFiles = new List<string>();			
			string extensionsSupported = string.Empty;
						
			// Set status
			RefreshStatus("Searching for media files", "Searching for media files in library folder " + folderPath);
			
			// Set supported extensions
#if MACOSX            
			extensionsSupported = @"^.+\.((wav)|(mp3)|(flac)|(ogg)|(mpc)|(wv)|(m3u)|(m3u8)|(pls)|(xspf))$";
#elif LINUX
            extensionsSupported = @"^.+\.((wav)|(mp3)|(flac)|(ogg)|(mpc)|(wv)|(m3u)|(m3u8)|(pls)|(xspf))$";
#elif (!MACOSX && !LINUX)
			extensionsSupported = @"WAV;MP3;FLAC;OGG;MPC;WV;WMA;APE;M3U;M3U8;PLS;XSPF";
#endif
						
#if (MACOSX || LINUX)
			
			// Get Unix-style directory information (i.e. case sensitive file names)
			UnixDirectoryInfo rootDirectoryInfo = new UnixDirectoryInfo(folderPath);
			
            // For each directory, search for new directories
            foreach (UnixFileSystemInfo fileInfo in rootDirectoryInfo.GetFileSystemEntries())
            {			
				// Check if entry is a directory
				if(fileInfo.IsDirectory && recursive)
				{
                    // Search for media filess in that directory                    
                	List<string> listMediaFiles = SearchMediaFilesInFolders(fileInfo.FullName, true);
                	arrayFiles.AddRange(listMediaFiles);					
				}
				
				// Does the file match a supported extension
				Match match = Regex.Match(fileInfo.FullName, @"^.+\.((wav)|(mp3)|(flac)|(ogg)|(mpc)|(wv)|(m3u)|(m3u8)|(pls)|(xspf))$", RegexOptions.IgnoreCase);
				if(match.Success)
				{
					// Add file
                    arrayFiles.Add(fileInfo.FullName);
				}				
            }
            
#else
        	
            // Get Windows-style directory information
            DirectoryInfo rootDirectoryInfo = new DirectoryInfo(folderPath);                        
            
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
			
			string[] extensions = extensionsSupported.Split(new string[]{";"}, StringSplitOptions.RemoveEmptyEntries);
            // For each extension supported
            foreach (string extension in extensions)
            {
                foreach (FileInfo fileInfo in rootDirectoryInfo.GetFiles("*." + extension))
                {
                    // Add file
                    arrayFiles.Add(fileInfo.FullName);
                }
            }
						
#endif				

            return arrayFiles;
        }
	}
}

