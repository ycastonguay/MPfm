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
using System.IO;
using MPfm.Library.Database;
using MPfm.Library.Database.Interfaces;
using MPfm.Library.Objects;
using MPfm.MVP.Services.Interfaces;
using MPfm.Sound.AudioFiles;

namespace MPfm.MVP.Services
{
    /// <summary>
    /// Service used for interacting with the library database.
    /// </summary>
    public class LibraryService : ILibraryService
    {
		// Private variables
		private readonly IDatabaseFacade gateway = null;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MPfm.Library.UpdateLibraryService"/> class.
		/// </summary>
		/// <param name='gateway'>
		/// MPfm Gateway.
		/// </param>
		/// <exception cref='ArgumentNullException'>
		/// Is thrown when an argument passed to a method is invalid because it is <see langword="null" /> .
		/// </exception>
		public LibraryService(IDatabaseFacade gateway)
		{
			// Check for null
			if(gateway == null)
				throw new ArgumentNullException("The gateway parameter cannot be null!");
				
			// Set gateway
			this.gateway = gateway;
		}
		
		/// <summary>
		/// Compacts the database.
		/// </summary>
		public void CompactDatabase()
		{
			gateway.CompactDatabase();
		}
		
		/// <summary>
		/// Returns the list of configured folders in the database.
		/// </summary>
		/// <returns>List of folders</returns>
		public IEnumerable<Folder> SelectFolders()
		{
			return gateway.SelectFolders();
		}
		
		/// <summary>
		/// Returns the list of audio file paths from the Song table.
		/// </summary>
		/// <returns>List of file paths</returns>
		public IEnumerable<string> SelectFilePaths()
		{
			return gateway.SelectFilePaths();
		}
		
		/// <summary>
		/// Selects all audio file metadata from the database.
		/// </summary>
		/// <returns>List of audio files</returns>
		public IEnumerable<AudioFile> SelectAudioFiles()
		{
			return gateway.SelectAudioFiles();			
		}
		
		/// <summary>
		/// Selects audio file metadata from the database by creating a query based on the method parameters.
		/// </summary>		
		/// <param name='format'>Audio file format</param>
		/// <param name='artistName'>Artist name</param>
		/// <param name='albumTitle'>Album title</param>
		/// <param name='search'>Search terms</param>
		/// <returns>List of audio files</returns>
		public IEnumerable<AudioFile> SelectAudioFiles(AudioFileFormat format, string artistName, string albumTitle, string search)
		{
			return gateway.SelectAudioFiles(format, artistName, albumTitle, search);			
		}
				
		/// <summary>
		/// Inserts an audio file into the database.
		/// </summary>
		/// <param name='audioFile'>Audio file to insert</param>
		public void InsertAudioFile(AudioFile audioFile)
		{
			gateway.InsertAudioFile(audioFile);
		}
		
		/// <summary>
		/// Inserts a playlist file into the database.
		/// </summary>
		/// <param name='playlistFile'>Playlist file to insert</param>
		public void InsertPlaylistFile(PlaylistFile playlistFile)
		{
			gateway.InsertPlaylistFile(playlistFile);
		}
		
		/// <summary>
		/// Removes the audio files with broken file paths (i.e. that do not exist anymore on the hard drive).
		/// </summary>
		public void RemoveAudioFilesWithBrokenFilePaths()
		{
            // Get all audio files
            List<AudioFile> files = gateway.SelectAudioFiles();

            // For each audio file
            for (int a = 0; a < files.Count; a++)
            {
                // If the file doesn't exist, delete the audio file from the database
                if (!File.Exists(files[a].FilePath))
                {
                    gateway.DeleteAudioFile(files[a].Id);
                }
            }
		}	
		
		public void AddFiles(List<string> filePaths)
		{
			//gateway.InsertAudioFile(
		}
	
		public void AddFolder(string folderPath, bool recursive)
		{		
            // Check if the folder is already part of a configured folder
            bool folderFound = false;

            // Get the list of folders from the database                
            List<Folder> folders = gateway.SelectFolders();

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
                    gateway.DeleteFolder(folder.FolderId);
                }
            }

            // Add the folder to the list of configured folders
            if (!folderFound)
            {
                // Add folder to database                    
                gateway.InsertFolder(folderPath, true);
			}
		}		
		
		public List<string> SelectDistinctArtistNames(AudioFileFormat format)
		{
			return gateway.SelectDistinctArtistNames(format);
		}
		
		public Dictionary<string, List<string>> SelectDistinctAlbumTitles(AudioFileFormat format)
		{
			return gateway.SelectDistinctAlbumTitles(format);
		}
		
		public Dictionary<string, List<string>> SelectDistinctAlbumTitles(AudioFileFormat format, string artistName)
		{
			return gateway.SelectDistinctAlbumTitles(format, artistName);
		}
    }
}
