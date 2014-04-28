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

#if WINDOWSSTORE || WINDOWS_PHONE
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using MPfm.Library.Objects;
using MPfm.Library.UpdateLibrary;
using MPfm.Sound.AudioFiles;
using System.Text.RegularExpressions;
using MPfm.Library.Services.Interfaces;
using MPfm.Library.Services.Events;

#if (MACOSX || LINUX)
using Mono.Unix;
using Mono.Unix.Native;
#endif

namespace MPfm.MVP.Services
{
	/// <summary>
	///	Service used for updating the library with new audio files using a background worker.
	/// </summary>
	public class UpdateLibraryService : IUpdateLibraryService
	{
		private ILibraryService libraryService = null;		
		//private BackgroundWorker workerUpdateLibrary = null;
		private bool cancelUpdateLibrary = false;
		
		public event EventHandler<RefreshStatusEventArgs> RaiseRefreshStatusEvent;	
		public event EventHandler<ProcessEndedEventArgs> RaiseProcessEndedEvent;
		
		#region Constructor and Dispose

		/// <summary>
		/// Initializes a new instance of the <see cref="MPfm.UI.UpdateLibraryService"/> class.
		/// </summary>
		public UpdateLibraryService(ILibraryService libraryService)
		{
			if(libraryService == null)
				throw new ArgumentNullException("The libraryService parameter cannot be null!");

			this.libraryService = libraryService;
					
            //workerUpdateLibrary = new BackgroundWorker();
            //workerUpdateLibrary.WorkerReportsProgress = true;
            //workerUpdateLibrary.WorkerSupportsCancellation = true;
            //workerUpdateLibrary.DoWork += new DoWorkEventHandler(workerUpdateLibrary_DoWork);
            //workerUpdateLibrary.RunWorkerCompleted += new RunWorkerCompletedEventHandler(workerUpdateLibrary_RunWorkerCompleted);
		}

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
				handler(this, new RefreshStatusEventArgs(entity));
		}
		
		/// <summary>
		/// Raises the process ended event. Wraps the event invocation.
		/// </summary>
		/// <param name='e'>Event arguments</param>		
		protected virtual void OnRaiseProcessEndedEvent(ProcessEndedEventArgs e)
		{
			EventHandler<ProcessEndedEventArgs> handler = RaiseProcessEndedEvent;
			if(handler != null)
				handler(this, e);
		}
		
		#endregion
		
		#region IUpdateLibraryPresenter implementation
		
		/// <summary>
		/// Cancels the update library process.
		/// </summary>
		public void Cancel()
		{			
			// Cancel process
			cancelUpdateLibrary = true;
		}

		/// <summary>
		/// Saves the update library process log to the specified file path.
		/// </summary>
		/// <param name='filePath'>Log file path</param>
		public void SaveLog(string filePath)
		{
            //// Generate the name of the log
            //saveLogDialog.FileName = "MPfm_UpdateLibraryLog_" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Day.ToString("00") + ".txt";
            //    TextWriter tw = null;
            //    try
            //    {
            //        // Open text writer
            //        tw = new StreamWriter(saveLogDialog.FileName);

            //        foreach (String item in lbLog.Items)
            //        {
            //            tw.WriteLine(item);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        // Display error
            //        MessageBox.Show("Failed to save the file to " + saveLogDialog.FileName + "!\n\nException:\n" + ex.Message + "\n" + ex.StackTrace, "Failed to save the file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //    finally
            //    {
            //        tw.Close();
            //    }
		}
			
		#endregion
		
		/// <summary>
		/// Starts the update library process in a background thread.		
		/// </summary>
		/// <param name='mode'>Update library mode</param>
		/// <param name='filePaths'>Audio file paths to add to the database</param>
		/// <param name='folderPath'>Folder path to add to the database</param>
		public void UpdateLibrary(UpdateLibraryMode mode, List<string> filePaths, string folderPath)
		{					
            UpdateLibraryArgument arg = new UpdateLibraryArgument();
            arg.Mode = mode;
            arg.FilePaths = filePaths;
            arg.FolderPath = folderPath;
			
			cancelUpdateLibrary = false;

            if (mode == UpdateLibraryMode.SpecificFolder)
				libraryService.AddFolder(folderPath, true);
			else if(mode == UpdateLibraryMode.SpecificFiles)
				libraryService.AddFiles(filePaths);

            // Start the background process
            //workerUpdateLibrary.RunWorkerAsync(arg);

            // TODO: Add time elapsed/time remaining
            //    //            // Set start time when the process has finished finding the files and is ready to add files into library
            //    //            if (startTimeAddFiles == DateTime.MinValue)
            //    //            {
            //    //                startTimeAddFiles = DateTime.Now;
            //    //            }

            //    //            // Calculate time elapsed
            //    //            TimeSpan timeElapsed = DateTime.Now.Subtract(startTimeAddFiles);

            //    //            // Update title
            //    //            lblTitle.Text = data.Title + " (file " + data.CurrentFilePosition.ToString() + " of " + data.TotalNumberOfFiles.ToString() + ")";

            //    //            // Calculate time remaining
            //    //            double msPerFile = timeElapsed.TotalMilliseconds / data.CurrentFilePosition;
            //    //            double remainingTime = (data.TotalNumberOfFiles - data.CurrentFilePosition) * msPerFile;
            //    //            TimeSpan timeRemaining = new TimeSpan(0, 0, 0, 0, (int)remainingTime);

            //    //            // Update estimated time left (from more precise to more vague)
            //    //            if (timeRemaining.TotalSeconds == 0)
            //    //            {
            //    //                lblEstimatedTimeLeft.Text = "Estimated time left : N/A";
            //    //            }
            //    //            else if (timeRemaining.Minutes == 1)
            //    //            {
            //    //                lblEstimatedTimeLeft.Text = "Estimated time left : 1 minute";
            //    //            }
            //    //            else if (timeRemaining.TotalSeconds <= 10)
            //    //            {
            //    //                lblEstimatedTimeLeft.Text = "Estimated time left : A few seconds";
            //    //            }
            //    //            else if (timeRemaining.TotalSeconds <= 30)
            //    //            {
            //    //                lblEstimatedTimeLeft.Text = "Estimated time left : Less than 30 seconds";
            //    //            }
            //    //            else if (timeRemaining.TotalSeconds <= 60)
            //    //            {
            //    //                lblEstimatedTimeLeft.Text = "Estimated time left : Less than a minute";
            //    //            }
            //    //            else
            //    //            {
            //    //                lblEstimatedTimeLeft.Text = "Estimated time left : " + timeRemaining.Minutes.ToString() + " minutes";
            //    //            }

		}		
		
        ///// <summary>
        ///// Occurs when the update library process background worker needs to do its job.
        ///// </summary>
        ///// <param name='sender'>Object sender</param>
        ///// <param name='e'>Event arguments</param>
        //protected void workerUpdateLibrary_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    List<string> filePaths = new List<string>();    
        //    UpdateLibraryArgument arg = (UpdateLibraryArgument)e.Argument;
						
        //    try
        //    {			
        //        if (cancelUpdateLibrary) throw new UpdateLibraryException();
        //        if (arg.Mode == UpdateLibraryMode.WholeLibrary)
        //        {
        //            // Remove broken songs from the library
        //            OnRaiseRefreshStatusEvent(new UpdateLibraryEntity() {
        //                Title = "Checking for broken file paths",
        //                Subtitle = "Checking if songs have been deleted on your hard disk but not removed from the library..",
        //                PercentageDone = 0
        //            });
        //            libraryService.RemoveAudioFilesWithBrokenFilePaths();

        //            if (cancelUpdateLibrary) throw new UpdateLibraryException();

        //            filePaths = SearchMediaFilesInFolders();
        //        }
        //        else if (arg.Mode == UpdateLibraryMode.SpecificFiles)
        //        {           
        //            // Set the media files passed in the argument
        //            filePaths = arg.FilePaths;
        //        }
        //        else if (arg.Mode == UpdateLibraryMode.SpecificFolder)
        //        {
        //            // Search the files in the specified folder                 
        //            Console.WriteLine("UpdateLibraryService - Looking for audio files in {0}...", arg.FolderPath);
        //            filePaths = SearchMediaFilesInFolders(arg.FolderPath, true);
        //            Console.WriteLine("UpdateLibraryService - Found {0} audio files in {0}", filePaths.Count, arg.FolderPath);
        //        }
				
        //        if (cancelUpdateLibrary) throw new UpdateLibraryException();
				
        //        // Get the list of audio files from the database
        //        IEnumerable<string> filePathsDatabase = libraryService.SelectFilePaths();				
        //        IEnumerable<string> filePathsToUpdate = filePaths.Except(filePathsDatabase);
					
        //        for(int a = 0; a < filePathsToUpdate.Count(); a++)
        //        {
        //            if (cancelUpdateLibrary) throw new UpdateLibraryException();							           				
						
        //            // Get current file path and calculate stats
        //            string filePath = filePathsToUpdate.ElementAt(a);											
        //            float percentCompleted = ((float)a / (float)filePathsToUpdate.Count());
					
        //            try
        //            {									
        //                // Check if this is a playlist file
        //                if (filePath.ToUpper().Contains(".M3U") ||
        //                    filePath.ToUpper().Contains(".M3U8") ||
        //                    filePath.ToUpper().Contains(".PLS") ||
        //                    filePath.ToUpper().Contains(".XSPF"))
        //                {
        //                    PlaylistFile playlistFile = new PlaylistFile(filePath);
        //                    libraryService.InsertPlaylistFile(playlistFile);							
        //                }
        //                else
        //                {
        //                    AudioFile audioFile = new AudioFile(filePath, Guid.NewGuid(), true);	                    
        //                    libraryService.InsertAudioFile(audioFile);
        //                }
						
        //                // Display update
        //                OnRaiseRefreshStatusEvent(new UpdateLibraryEntity() {
        //                    Title = "Adding media file to the library",
        //                    Subtitle = "Adding " + Path.GetFileName(filePath),
        //                    FilePath = filePath,
        //                    PercentageDone = percentCompleted,
        //                    FileIndex = a,
        //                    FileCount = filePathsToUpdate.Count()								
        //                });		                    
        //            }
        //            catch (Exception ex)
        //            {
        //                //view.AddToLog("File could not be added: " + filePath);
        //                Console.WriteLine("UpdateLibraryService - Could not add {0}: {1}", filePath, ex);
        //                OnRaiseRefreshStatusEvent(new UpdateLibraryEntity() {
        //                    Title = "Adding media file to the library",
        //                    Subtitle = "Adding " + filePath,
        //                    FilePath = filePath,
        //                    PercentageDone = percentCompleted,
        //                    FileIndex = a,
        //                    FileCount = filePathsToUpdate.Count(),
        //                    Exception = ex
        //                });
        //            }
        //        }

        //        // Cancel thread if necessary
        //        if (cancelUpdateLibrary) throw new UpdateLibraryException();

        //        // Compact database						
        //        OnRaiseRefreshStatusEvent(new UpdateLibraryEntity() {
        //            Title = "Compacting database",
        //            Subtitle = "Compacting database...",
        //            PercentageDone = 100
        //        });                
        //        libraryService.CompactDatabase();
        //    }
        //    catch (UpdateLibraryException ex)
        //    {
        //        OnRaiseRefreshStatusEvent(new UpdateLibraryEntity() {
        //            Title = "Update process canceled",
        //            Subtitle = "The update process was canceled by the user."
        //        });                
        //        e.Cancel = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        OnRaiseRefreshStatusEvent(new UpdateLibraryEntity() {
        //            Title = "Error updating library",
        //            Subtitle = "An error has occured: " + ex.Message + "\n" + ex.StackTrace
        //        });                
        //    }			
        //}
		
        ///// <summary>
        ///// Fires when the update library process is over.
        ///// </summary>
        ///// <param name="sender">Event sender</param>
        ///// <param name="e">Event argument</param>
        //private void workerUpdateLibrary_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{	
        //    OnRaiseProcessEndedEvent(new ProcessEndedEventArgs(e.Cancelled));
        //}
				
        /// <summary>
        /// Searches for songs in all configured folders.
        /// </summary>
        /// <returns>List of songs (file paths)</returns>
        public List<string> SearchMediaFilesInFolders()
        {
            List<string> files = new List<string>();            
            IEnumerable<Folder> folders = libraryService.SelectFolders();

            foreach (Folder folder in folders)
            {
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
						
//            // Refresh status
//            OnRaiseRefreshStatusEvent(new UpdateLibraryEntity() {
//                Title = "Searching for media files",
//                Subtitle = "Searching for media files in library folder " + folderPath,
//                PercentageDone = 0
//            });    
			
//            // Set supported extensions
//#if MACOSX            
//            extensionsSupported = @"^.+\.((wav)|(mp3)|(flac)|(ogg)|(mpc)|(wv)|(m3u)|(m3u8)|(pls)|(xspf))$";
//#elif LINUX
//            extensionsSupported = @"^.+\.((wav)|(mp3)|(flac)|(ogg)|(mpc)|(wv)|(m3u)|(m3u8)|(pls)|(xspf))$";
//#elif (!MACOSX && !LINUX)
//            extensionsSupported = @"WAV;MP3;FLAC;OGG;MPC;WV;WMA;APE;M3U;M3U8;PLS;XSPF";
//#endif
						
//#if (MACOSX || LINUX)
			
//            // Get Unix-style directory information (i.e. case sensitive file names)
//            UnixDirectoryInfo rootDirectoryInfo = new UnixDirectoryInfo(folderPath);
			
//            // For each directory, search for new directories
//            foreach (UnixFileSystemInfo fileInfo in rootDirectoryInfo.GetFileSystemEntries())
//            {			
//                // Check if entry is a directory
//                if(fileInfo.IsDirectory && recursive)
//                {
//                    // Make sure this isn't an Apple index directory
//                    if(!fileInfo.Name.StartsWith(".Apple"))
//                    {
//                        Console.WriteLine("UpdateLibraryService - SearchMediaFilesInFolders {0}", fileInfo.FullName);
//                        try
//                        {
//                            List<string> listMediaFiles = SearchMediaFilesInFolders(fileInfo.FullName, true);
//                            arrayFiles.AddRange(listMediaFiles);					
//                        }
//                        catch(Exception ex)
//                        {
//                            Console.WriteLine("UpdateLibraryService - SearchMediaFilesInFolders {0} - Exception {1}", fileInfo.FullName, ex);
//                        }
//                    }
//                }
				
//                // If the extension matches, add file to list
//                Match match = Regex.Match(fileInfo.FullName, extensionsSupported, RegexOptions.IgnoreCase);
//                if(match.Success)
//                    arrayFiles.Add(fileInfo.FullName);
//            }
            
//#else
        	
//            // Get Windows-style directory information
//            DirectoryInfo rootDirectoryInfo = new DirectoryInfo(folderPath);                        
            
//            // Check for sub directories if recursive
//            if (recursive)
//            {
//                // For each directory, search for new directories
//                DirectoryInfo[] directoryInfos = rootDirectoryInfo.GetDirectories();
//                foreach (DirectoryInfo directoryInfo in directoryInfos)
//                {
//                    // Make sure this isn't an Apple index directory
//                    if (!directoryInfo.Name.StartsWith(".Apple"))
//                    {					
//                        // Search for songs in that directory                    
//                        List<string> listSongs = SearchMediaFilesInFolders(directoryInfo.FullName, recursive);
//                        arrayFiles.AddRange(listSongs);
//                    }
//                }
//            }
			
//            string[] extensions = extensionsSupported.Split(new string[]{";"}, StringSplitOptions.RemoveEmptyEntries);
//            //FileInfo[] fileInfos = rootDirectoryInfo.GetFiles("*." + extension);
//            FileInfo[] fileInfos = rootDirectoryInfo.GetFiles();
//            foreach (FileInfo fileInfo in fileInfos)
//            {
//                // Check if file matches supported extensions
//                if (extensions.Contains(fileInfo.Extension.Replace(".", ""), StringComparer.InvariantCultureIgnoreCase))
//                {
//                    arrayFiles.Add(fileInfo.FullName);
//                }
//            }
						
//#endif				

            return arrayFiles;
        }
	}
}
#endif
