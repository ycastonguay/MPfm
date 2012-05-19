//
// UpdateLibraryService.cs: Service used for updating the library with new
//                          audio files using a background worker.
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
	///	Service used for updating the library with new audio files using a background worker.
	/// </summary>
	public class UpdateLibraryService : IDisposable, IUpdateLibraryService
	{
		// Private variables
		private ILibraryService libraryService = null;		
		private BackgroundWorker workerUpdateLibrary = null;
		private bool cancelUpdateLibrary = false;
		
		// Events
		public event EventHandler<RefreshStatusEventArgs> RaiseRefreshStatusEvent;	
		public event EventHandler<ProcessEndedEventArgs> RaiseProcessEndedEvent;
		
		#region Constructor and Dispose

		/// <summary>
		/// Initializes a new instance of the <see cref="MPfm.UI.UpdateLibraryService"/> class.
		/// </summary>
		public UpdateLibraryService(ILibraryService libraryService)
		{
			// Check for null
			if(libraryService == null)
				throw new ArgumentNullException("The libraryService parameter cannot be null!");

			// Set properties
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
		
		#region Events
		
		/// <summary>
		/// Raises the refresh status event. Wraps the event invocation by
		/// creating the event arguments from the entity.
		/// </summary>
		/// <param name='e'>Event arguments</param>		
		protected virtual void OnRaiseRefreshStatusEvent(UpdateLibraryEntity entity)
		{
			EventHandler<RefreshStatusEventArgs> handler = RaiseRefreshStatusEvent;
			
			if(handler != null)
			{
				handler(this, new RefreshStatusEventArgs(entity));
			}
		}
		
		/// <summary>
		/// Raises the process ended event. Wraps the event invocation.
		/// </summary>
		/// <param name='e'>Event arguments</param>		
		protected virtual void OnRaiseProcessEndedEvent(ProcessEndedEventArgs e)
		{
			EventHandler<ProcessEndedEventArgs> handler = RaiseProcessEndedEvent;
			
			if(handler != null)
			{
				handler(this, e);
			}
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
					OnRaiseRefreshStatusEvent(new UpdateLibraryEntity() {
						Title = "Checking for broken file paths",
						Subtitle = "Checking if songs have been deleted on your hard disk but not removed from the library..",
						PercentageDone = 0
					});
                    libraryService.RemoveAudioFilesWithBrokenFilePaths();

                    // Cancel update library process if necessary
                    if (cancelUpdateLibrary) throw new UpdateLibraryException();

                    // Search for new media in the library folders                    
                    filePaths = SearchMediaFilesInFolders();
                }
                else if (arg.Mode == UpdateLibraryMode.SpecificFiles)
                {           
                    // Set the media files passed in the argument
                    filePaths = arg.FilePaths;
                }
                else if (arg.Mode == UpdateLibraryMode.SpecificFolder)
                {
                    // Search the files in the specified folder                    
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
						
					// Get current file path and calculate stats
					string filePath = filePathsToUpdate.ElementAt(a);											
			        float percentCompleted = ((float)a / (float)filePathsToUpdate.Count());
					
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
		                }
		                else
		                {
		                    // Get audio file metadata and insert into database
		                    AudioFile audioFile = new AudioFile(filePath, Guid.NewGuid(), true);	                    
	                    	libraryService.InsertAudioFile(audioFile);
		                }
						
						// Display update
						OnRaiseRefreshStatusEvent(new UpdateLibraryEntity() {
							Title = "Adding media file to the library",
							Subtitle = "Adding " + filePath,
							FilePath = filePath,
							PercentageDone = percentCompleted,
							FileIndex = a,
							FileCount = filePathsToUpdate.Count()								
						});		                    
					}
					catch (Exception ex)
					{
						//view.AddToLog("File could not be added: " + filePath);
						OnRaiseRefreshStatusEvent(new UpdateLibraryEntity() {
							Title = "Adding media file to the library",
							Subtitle = "Adding " + filePath,
							FilePath = filePath,
							Exception = ex
						});
					}
		        }

                // Cancel thread if necessary
                if (cancelUpdateLibrary) throw new UpdateLibraryException();

                // Compact database						
				OnRaiseRefreshStatusEvent(new UpdateLibraryEntity() {
					Title = "Compacting database",
					Subtitle = "Compacting database...",
					PercentageDone = 100
				});                
                libraryService.CompactDatabase();
			}
			catch (UpdateLibraryException ex)
            {
				OnRaiseRefreshStatusEvent(new UpdateLibraryEntity() {
					Title = "Update process canceled",
					Subtitle = "The update process was canceled by the user."
				});                
                e.Cancel = true;
            }
            catch (Exception ex)
            {
				OnRaiseRefreshStatusEvent(new UpdateLibraryEntity() {
					Title = "Error updating library",
					Subtitle = "An error has occured: " + ex.Message + "\n" + ex.StackTrace
				});                
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
			OnRaiseProcessEndedEvent(new ProcessEndedEventArgs(e.Cancelled));
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
				// Search for media files
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
						
			// Refresh status
			OnRaiseRefreshStatusEvent(new UpdateLibraryEntity() {
				Title = "Searching for media files",
				Subtitle = "Searching for media files in library folder " + folderPath,
				PercentageDone = 0
			});    
			
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
					// Make sure this isn't an Apple index directory
					if(!fileInfo.Name.StartsWith(".Apple"))
					{					
                    	// Search for media filess in that directory                    
                		List<string> listMediaFiles = SearchMediaFilesInFolders(fileInfo.FullName, true);
                		arrayFiles.AddRange(listMediaFiles);					
					}
				}
				
				// Does the file match a supported extension
				Match match = Regex.Match(fileInfo.FullName, extensionsSupported, RegexOptions.IgnoreCase);
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
					// Make sure this isn't an Apple index directory
					if(!fileInfo.Name.StartsWith(".Apple"))
					{					
	                    // Search for songs in that directory                    
	                    List<string> listSongs = SearchMediaFilesInFolders(directoryInfo.FullName, recursive);
	                    arrayFiles.AddRange(listSongs);
					}
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

