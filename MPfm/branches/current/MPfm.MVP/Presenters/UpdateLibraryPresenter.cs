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
using System.Reflection;
using MPfm.Core;
using MPfm.Library;
using MPfm.Player;
using MPfm.Sound;
using MPfm.Sound.BassNetWrapper;

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
		private IUpdateLibraryService updateLibraryService = null;
		private BackgroundWorker workerUpdateLibrary = null;
		
		#region Constructor and Dispose

		/// <summary>
		/// Initializes a new instance of the <see cref="MPfm.UI.UpdateLibraryPresenter"/> class.
		/// </summary>
		public UpdateLibraryPresenter(IUpdateLibraryView view, IMainPresenter mainPresenter, ILibraryService libraryService, IUpdateLibraryService updateLibraryService)
		{
			// Check for null
			if(view == null)
				throw new ArgumentNullException("The view parameter cannot be null!");
			if(mainPresenter == null)
				throw new ArgumentNullException("The mainPresenter parameter cannot be null!");
			if(libraryService == null)
				throw new ArgumentNullException("The libraryService parameter cannot be null!");
			if(updateLibraryService == null)
				throw new ArgumentNullException("The updateLibraryService parameter cannot be null!");

			// Set properties
			this.view = view;
			this.mainPresenter = mainPresenter;
			this.libraryService = libraryService;
			this.updateLibraryService = updateLibraryService;
					
			// Create worker
			workerUpdateLibrary = new BackgroundWorker();
			workerUpdateLibrary.DoWork += HandleWorkerDoWork;
			workerUpdateLibrary.ProgressChanged += HandleWorkerProgressChanged;
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
			
		public void OK()
		{			
		}
	
		public void Cancel()
		{			
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

			// If the mode is SpecificFolder
            if (mode == UpdateLibraryMode.SpecificFolder)
            {
				// Add folder to library
				updateLibraryService.AddFolder(folderPath, true);
            }
			else if(mode ==  UpdateLibraryMode.SpecificFiles)
			{
				// Add files to library
				updateLibraryService.AddFiles(filePaths);
			}

            // Start the background process using the arguments
            workerUpdateLibrary.RunWorkerAsync(arg);
						
			// Check for broken file paths
			//RefreshStatus("Checking for broken file paths", "Checking if songs have been deleted on your hard disk but not removed from the library...");
			//libraryService.RemoveAudioFilesWithBrokenFilePaths();
			//RefreshStatus("DONE", string.Empty);
		}		
		
		protected void HandleWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			
		}

		protected void HandleWorkerDoWork(object sender, DoWorkEventArgs e)
		{
//			 try
//            {
//                // Variables
//                List<string> mediaFiles = new List<string>();                
//
//                // Detect if an argument was passed
//                if (e.Argument == null)
//                {
//                    // Exit worker
//                    return;
//                }
//
//                // Get the argument
//                UpdateLibraryArgument arg = (UpdateLibraryArgument)e.Argument;
//
//                // Set the cancel operation to false
//                CancelUpdateLibrary = false;
//
//                // Refresh cache
//                UpdateLibraryReportProgress("Refreshing cache", "Refreshing cache...");
//                RefreshCache();
//
//                // Cancel thread if necessary
//                if (CancelUpdateLibrary) throw new UpdateLibraryException();
//
//                // Determine the update library mode
//                if (arg.Mode == UpdateLibraryMode.WholeLibrary)
//                {
//                    // Remove broken songs from the library
//                    UpdateLibraryReportProgress("Checking for broken file paths", "Checking if songs have been deleted on your hard disk but not removed from the library...");
//                    RemoveAudioFilesWithBrokenFilePaths();
//
//                    // Cancel thread if necessary
//                    if (CancelUpdateLibrary) throw new UpdateLibraryException();
//
//                    // Search for new media in the library folders
//                    UpdateLibraryReportProgress("Searching for media files", "Searching media files in library folders");
//                    mediaFiles = SearchMediaFilesInFolders();
//
//                    // Update the media count
//                    //UpdateLibraryReportProgress("Searching media files", "Searching media files in library folders", 0, mediaFiles.Count.ToString() + " song(s) found.");
//                }
//                else if (arg.Mode == UpdateLibraryMode.SpecificFiles)
//                {           
//                    // Set the media files passed in the argument
//                    mediaFiles = arg.FilePaths;
//                }
//                else if (arg.Mode == UpdateLibraryMode.SpecificFolder)
//                {
//                    // Search the files in the specified folder
//                    UpdateLibraryReportProgress("Searching for media files", "Searching media files in " + arg.FolderPath);
//                    mediaFiles = SearchMediaFilesInFolders(arg.FolderPath, true);
//                }
//
//                // Get the list of audio files from the database (actually the cache)
//                List<string> filePaths = AudioFiles.Select(x => x.FilePath).ToList();
//
//                // Get the list of playlist file paths from database
//                List<string> playlistFilePaths = Gateway.SelectPlaylistFiles().Select(x => x.FilePath).ToList();
//
//                // Compare list of files from database with list of files found on hard disk
//                List<string> audioFilesToUpdate = mediaFiles.Except(filePaths).ToList();
//
//                // Remove existing playlist files
//                audioFilesToUpdate = audioFilesToUpdate.Except(playlistFilePaths).ToList();                 
//
//                // Cancel thread if necessary
//                if (CancelUpdateLibrary) throw new UpdateLibraryException();
//
//                // Add new media (if media found!)
//                if (mediaFiles.Count > 0)
//                {
//                    AddAudioFilesToLibrary(audioFilesToUpdate);                    
//                }                
//
//                // Cancel thread if necessary
//                if (CancelUpdateLibrary) throw new UpdateLibraryException();
//
//                // Refreshing cache
//                UpdateLibraryReportProgress("Refreshing cache", "Refreshing cache...", 100);
//                RefreshCache();
//
//                // Cancel thread if necessary
//                if (CancelUpdateLibrary) throw new UpdateLibraryException();
//
//                // Compact database
//                UpdateLibraryReportProgress("Compacting database", "Compacting database...", 100);                
//                gateway.CompactDatabase();
//            }
//            catch (UpdateLibraryException ex)
//            {
//                UpdateLibraryReportProgress("The update process was canceled: " + ex.Message, "Canceled by user");
//                e.Cancel = true;
//            }
//            catch (Exception ex)
//            {
//                UpdateLibraryReportProgress("An error has occured: " + ex.Message, ex.StackTrace);
//            }
		}
		
		private void RefreshStatus(string title, string subtitle)
		{
			// Create entity and update view
			UpdateLibraryEntity entity = new UpdateLibraryEntity();
			entity.Title = title;
			entity.Subtitle = subtitle;
			view.RefreshStatus(entity);
		}
	}
}

