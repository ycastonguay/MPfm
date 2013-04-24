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
using MPfm.Player.Objects;

namespace MPfm.MVP.Services
{
    /// <summary>
    /// Service used for interacting with the library database.
    /// </summary>
    public class LibraryService : ILibraryService
    {
		private readonly IDatabaseFacade gateway = null;
		
		public LibraryService(IDatabaseFacade gateway)
		{
			if(gateway == null)
				throw new ArgumentNullException("The gateway parameter cannot be null!");
				
			this.gateway = gateway;
		}
		
		public void CompactDatabase()
		{
			gateway.CompactDatabase();
		}

        #region Audio Files
		
		public IEnumerable<Folder> SelectFolders()
		{
			return gateway.SelectFolders();
		}
		
		public IEnumerable<string> SelectFilePaths()
		{
			return gateway.SelectFilePaths();
		}
		
		public IEnumerable<AudioFile> SelectAudioFiles()
		{
			return gateway.SelectAudioFiles();			
		}
		
		public IEnumerable<AudioFile> SelectAudioFiles(AudioFileFormat format, string artistName, string albumTitle, string search)
		{
			return gateway.SelectAudioFiles(format, artistName, albumTitle, search);			
		}
				
		public void InsertAudioFile(AudioFile audioFile)
		{
			gateway.InsertAudioFile(audioFile);
		}
		
		public void InsertPlaylistFile(PlaylistFile playlistFile)
		{
			gateway.InsertPlaylistFile(playlistFile);
		}
		
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

        #endregion

        #region Markers

        public void InsertMarker(Marker marker)
        {
            gateway.InsertMarker(marker);
        }

        public Marker SelectMarker(Guid markerId)
        {
            return gateway.SelectMarker(markerId);
        }

        public List<Marker> SelectMarkers(Guid audioFileId)
        {
            return gateway.SelectMarkers(audioFileId);
        }

        public void UpdateMarker(Marker marker)
        {
            gateway.UpdateMarker(marker);
        }

        public void DeleteMarker(Guid markerId)
        {
            gateway.DeleteMarker(markerId);
        }

        #endregion

        #region Loops
        
        public void InsertLoop(Loop loop)
        {
            gateway.InsertLoop(loop);
        }
        
        public Loop SelectLoop(Guid loopId)
        {
            return gateway.SelectLoop(loopId);
        }
        
        public List<Loop> SelectLoops(Guid audioFileId)
        {
            return gateway.SelectLoops(audioFileId);
        }
        
        public void UpdateLoop(Loop loop)
        {
            gateway.UpdateLoop(loop);
        }
        
        public void DeleteLoop(Guid loopId)
        {
            gateway.DeleteLoop(loopId);
        }
        
        #endregion

        #region Equalizer Presets
        
        public void InsertEQPreset(EQPreset preset)
        {
            gateway.InsertEQPreset(preset);
        }

        public EQPreset SelectEQPreset(Guid presetId)
        {
            return gateway.SelectEQPreset(presetId);
        }

        public IEnumerable<EQPreset> SelectEQPresets()
        {
            return gateway.SelectEQPresets();
        }

        public void UpdateEQPreset(EQPreset preset)
        {
            gateway.UpdateEQPreset(preset);
        }
        
        public void DeleteEQPreset(Guid presetId)
        {
            gateway.DeleteEQPreset(presetId);
        }
        
        #endregion

    }
}
