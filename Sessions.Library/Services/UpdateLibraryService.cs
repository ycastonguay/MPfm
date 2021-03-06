// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Sessions.Library.Objects;
using Sessions.Library.Services.Events;
using Sessions.Library.Services.Interfaces;
using Sessions.Library.UpdateLibrary;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
#if (MACOSX || LINUX)
using Mono.Unix;
using Mono.Unix.Native;
#endif
using Sessions.Sound.AudioFiles;

namespace Sessions.Library.Services
{
	/// <summary>
	///	Service used for updating the library with new audio files using a background worker.
	/// </summary>
	public class UpdateLibraryService : IUpdateLibraryService
	{
		private readonly ILibraryService _libraryService = null;		
		private BackgroundWorker _workerUpdateLibrary = null;
		private bool _cancelUpdateLibrary = false;

		public bool IsUpdatingLibrary { get { return _workerUpdateLibrary.IsBusy; } }
		
		public event EventHandler<RefreshStatusEventArgs> RaiseRefreshStatusEvent;
	    public event EventHandler<EventArgs> RaiseProcessStartedEvent;
	    public event EventHandler<ProcessEndedEventArgs> RaiseProcessEndedEvent;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Sessions.UI.UpdateLibraryService"/> class.
		/// </summary>
		public UpdateLibraryService(ILibraryService libraryService)
		{
			if(libraryService == null)
				throw new ArgumentNullException("The _libraryService parameter cannot be null!");

			_libraryService = libraryService;
			_workerUpdateLibrary = new BackgroundWorker();
            _workerUpdateLibrary.WorkerReportsProgress = true;
            _workerUpdateLibrary.WorkerSupportsCancellation = true;
            _workerUpdateLibrary.DoWork += new DoWorkEventHandler(workerUpdateLibrary_DoWork);
            _workerUpdateLibrary.RunWorkerCompleted += new RunWorkerCompletedEventHandler(workerUpdateLibrary_RunWorkerCompleted);
		}
		
		protected virtual void OnRaiseRefreshStatusEvent(UpdateLibraryEntity entity)
		{
			EventHandler<RefreshStatusEventArgs> handler = RaiseRefreshStatusEvent;
			if(handler != null)
				handler(this, new RefreshStatusEventArgs(entity));
		}

        protected virtual void OnRaiseProcessStartedEvent(EventArgs e)
        {
            EventHandler<EventArgs> handler = RaiseProcessStartedEvent;
            if (handler != null)
                handler(this, e);
        }

		protected virtual void OnRaiseProcessEndedEvent(ProcessEndedEventArgs e)
		{
			EventHandler<ProcessEndedEventArgs> handler = RaiseProcessEndedEvent;
			if(handler != null)
				handler(this, e);
		}
		
		/// <summary>
		/// Cancels the update library process.
		/// </summary>
		public void Cancel()
		{			
			// Cancel process
			_cancelUpdateLibrary = true;
		}

		/// <summary>
		/// Saves the update library process log to the specified file path.
		/// </summary>
		/// <param name='filePath'>Log file path</param>
		public void SaveLog(string filePath)
		{
            //// Generate the name of the log
            //saveLogDialog.FileName = "Sessions_UpdateLibraryLog_" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("00") + "-" + DateTime.Now.Day.ToString("00") + ".txt";
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
		
		public void UpdateLibrary(List<string> filePaths, List<Folder> folderPaths)
		{					
            var arg = new UpdateLibraryArgument();
            arg.FilePaths = filePaths;
            arg.FolderPaths = folderPaths;
			
			_cancelUpdateLibrary = false;
            OnRaiseProcessStartedEvent(new EventArgs());
            _workerUpdateLibrary.RunWorkerAsync(arg);

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
		
		/// <summary>
		/// Occurs when the update library process background worker needs to do its job.
		/// </summary>
		/// <param name='sender'>Object sender</param>
		/// <param name='e'>Event arguments</param>
        protected void workerUpdateLibrary_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> filePaths = new List<string>();    
            UpdateLibraryArgument arg = (UpdateLibraryArgument)e.Argument;
            filePaths.AddRange(arg.FilePaths);
						
			try
			{			
                // Remove broken songs from the library
				OnRaiseRefreshStatusEvent(new UpdateLibraryEntity() {
					Title = "Checking for broken file paths",
					Subtitle = "Checking if songs have been deleted on your hard disk but not removed from the library",
					PercentageDone = 0
				});
                _libraryService.RemoveAudioFilesWithBrokenFilePaths();

                if (_cancelUpdateLibrary) throw new UpdateLibraryException();

                filePaths = SearchMediaFilesInFolders(arg.FolderPaths);
				
                if (_cancelUpdateLibrary) throw new UpdateLibraryException();
				
				// Get the list of audio files from the database
				var filePathsDatabase = _libraryService.SelectFilePaths();				
				var filePathsToUpdate = filePaths.Except(filePathsDatabase);
			    var audioFiles = new List<AudioFile>();
		        for(int a = 0; a < filePathsToUpdate.Count(); a++)
		        {
	                if (_cancelUpdateLibrary) throw new UpdateLibraryException();							           				
						
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
		                    var playlistFile = new PlaylistFile(filePath);
	                    	_libraryService.InsertPlaylistFile(playlistFile);							
		                }
		                else
		                {
		                    var audioFile = new AudioFile(filePath, Guid.NewGuid(), true);
                            //_libraryService.InsertAudioFile(audioFile);
                            audioFiles.Add(audioFile);
		                    if (audioFiles.Count >= 50)
		                    {
                                //Console.WriteLine("UpdateLibraryService - Inserting 20 audio files into database...");
		                        _libraryService.InsertAudioFiles(audioFiles);
		                        audioFiles.Clear();
		                    }
		                }
						
						OnRaiseRefreshStatusEvent(new UpdateLibraryEntity() {
							Title = "Adding audio files to the library",
							Subtitle = "Adding " + Path.GetFileName(filePath),
							FilePath = filePath,
							PercentageDone = percentCompleted,
							FileIndex = a,
							FileCount = filePathsToUpdate.Count()								
						});		                    
					}
					catch (Exception ex)
					{
                        Console.WriteLine("UpdateLibraryService - Failed to add {0}: {1}", filePath, ex);
						OnRaiseRefreshStatusEvent(new UpdateLibraryEntity() {
							Title = "Adding audio files to the library",
							Subtitle = "Adding " + filePath,
							FilePath = filePath,
                            PercentageDone = percentCompleted,
                            FileIndex = a,
                            FileCount = filePathsToUpdate.Count(),
                            Exception = ex
						});
					}
		        }

                if(audioFiles.Count > 0)
                    _libraryService.InsertAudioFiles(audioFiles);

                // Cancel thread if necessary
                if (_cancelUpdateLibrary) throw new UpdateLibraryException();

//                // Compact database						
				// TODO: Lags like hell on iOS, completely blocks the UI thread even though it is done in another thread... 
//				OnRaiseRefreshStatusEvent(new UpdateLibraryEntity() {
//					Title = "Compacting database",
//					Subtitle = "Compacting database...",
//					PercentageDone = 1
//				});             
//				Task.Factory.StartNew(() => {   
//                	_libraryService.CompactDatabase();
//				});
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
			OnRaiseProcessEndedEvent(new ProcessEndedEventArgs(e.Cancelled));
        }
				
		private List<string> SearchMediaFilesInFolders(List<Folder> folders)
        {
            List<string> files = new List<string>();            
            foreach (var folder in folders)
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
		private List<string> SearchMediaFilesInFolders(string folderPath, bool recursive)
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
                        Console.WriteLine("UpdateLibraryService - SearchMediaFilesInFolders {0}", fileInfo.FullName);
                        try
                        {
                    		List<string> listMediaFiles = SearchMediaFilesInFolders(fileInfo.FullName, true);
                    		arrayFiles.AddRange(listMediaFiles);					
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine("UpdateLibraryService - SearchMediaFilesInFolders {0} - Exception {1}", fileInfo.FullName, ex);
                        }
					}
				}
				
				// If the extension matches, add file to list
				Match match = Regex.Match(fileInfo.FullName, extensionsSupported, RegexOptions.IgnoreCase);
				if(match.Success)
                    arrayFiles.Add(fileInfo.FullName);
            }
            
#else
        	
            // Get Windows-style directory information
            DirectoryInfo rootDirectoryInfo = new DirectoryInfo(folderPath);                        
            
            // Check for sub directories if recursive
            if (recursive)
            {
                // For each directory, search for new directories
                DirectoryInfo[] directoryInfos = rootDirectoryInfo.GetDirectories();
                foreach (DirectoryInfo directoryInfo in directoryInfos)
                {
					// Make sure this isn't an Apple index directory
                    if (!directoryInfo.Name.StartsWith(".Apple"))
					{					
	                    // Search for songs in that directory                    
	                    List<string> listSongs = SearchMediaFilesInFolders(directoryInfo.FullName, recursive);
	                    arrayFiles.AddRange(listSongs);
					}
                }
            }
			
			string[] extensions = extensionsSupported.Split(new string[]{";"}, StringSplitOptions.RemoveEmptyEntries);
            //FileInfo[] fileInfos = rootDirectoryInfo.GetFiles("*." + extension);
            FileInfo[] fileInfos = rootDirectoryInfo.GetFiles();
            foreach (FileInfo fileInfo in fileInfos)
            {
                // Check if file matches supported extensions
                if (extensions.Contains(fileInfo.Extension.Replace(".", ""), StringComparer.InvariantCultureIgnoreCase))
                {
                    arrayFiles.Add(fileInfo.FullName);
                }
            }
						
#endif				

            return arrayFiles;
        }
	}
}
