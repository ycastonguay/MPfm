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
using MPfm.Library.Database.Interfaces;
using MPfm.Library.Objects;
using MPfm.Library.Services.Interfaces;
#if WINDOWSSTORE
using Windows.Storage;
using MPfm.Core.WinRT;
#endif
using Sessions.Player.Objects;
using Sessions.Sound.AudioFiles;
using Sessions.Sound.Playlists;

namespace MPfm.Library.Services
{
    /// <summary>
    /// Service used for interacting with the library database.
    /// </summary>
    public class LibraryService : ILibraryService
    {
		private readonly IDatabaseFacade _gateway = null;
		
		public LibraryService(IDatabaseFacade gateway)
		{
			if(gateway == null)
				throw new ArgumentNullException("The gateway parameter cannot be null!");
				
			this._gateway = gateway;
		}
		
		public void CompactDatabase()
		{
			_gateway.CompactDatabase();
		}

        public void ResetLibrary()
        {
            _gateway.ResetLibrary();
        }

        #region Audio Files
		
		public IEnumerable<Folder> SelectFolders()
		{
			return _gateway.SelectFolders();
		}
		
		public IEnumerable<string> SelectFilePaths()
		{
			return _gateway.SelectFilePaths();
		}
		
		public IEnumerable<AudioFile> SelectAudioFiles()
		{
			return _gateway.SelectAudioFiles();			
		}

        public IEnumerable<AudioFile> SelectAudioFiles(LibraryQuery query)
        {
            return _gateway.SelectAudioFiles(query.Format, query.ArtistName, query.AlbumTitle, query.SearchTerms);
        }
		
		public IEnumerable<AudioFile> SelectAudioFiles(AudioFileFormat format, string artistName, string albumTitle, string search)
		{
			return _gateway.SelectAudioFiles(format, artistName, albumTitle, search);			
		}
				
		public void InsertAudioFile(AudioFile audioFile)
		{
			_gateway.InsertAudioFile(audioFile);
		}

        public void InsertAudioFiles(IEnumerable<AudioFile> audioFiles)
        {
            _gateway.InsertAudioFiles(audioFiles);   
        }
		
		public void InsertPlaylistFile(PlaylistFile playlistFile)
		{
			_gateway.InsertPlaylistFile(playlistFile);
		}

        public void DeleteAudioFile(Guid audioFileId)
        {
            _gateway.DeleteAudioFile(audioFileId);
        }

        public void DeleteAudioFiles(string basePath)
        {
            _gateway.DeleteAudioFiles(basePath);
        }

        public void DeleteAudioFiles(string artistName, string albumTitle)
        {
            _gateway.DeleteAudioFiles(artistName, albumTitle);
        }
		
		public void RemoveAudioFilesWithBrokenFilePaths()
		{
            List<AudioFile> files = _gateway.SelectAudioFiles();
            for (int a = 0; a < files.Count; a++)
            {
                // If the file doesn't exist, delete the audio file from the database
                #if WINDOWSSTORE
                var task = ApplicationData.Current.LocalFolder.FileExistsAsync(files[a].FilePath);
                bool exists = task.Result; // Blocks the thread until the value is returned
                if(exists)
                    _gateway.DeleteAudioFile(files[a].Id);
                #else
                if (!File.Exists(files[a].FilePath))
                    _gateway.DeleteAudioFile(files[a].Id);
                #endif
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
            List<Folder> folders = _gateway.SelectFolders();

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
                    _gateway.DeleteFolder(folder.FolderId);
            }

            // Add the folder to the list of configured folders
            if (!folderFound)
                _gateway.InsertFolder(folderPath, true);
		}		
		
		public List<string> SelectDistinctArtistNames(AudioFileFormat format)
		{
			return _gateway.SelectDistinctArtistNames(format);
		}
		
		public Dictionary<string, List<string>> SelectDistinctAlbumTitles(AudioFileFormat format)
		{
			return _gateway.SelectDistinctAlbumTitles(format);
		}
		
		public Dictionary<string, List<string>> SelectDistinctAlbumTitles(AudioFileFormat format, string artistName)
		{
			return _gateway.SelectDistinctAlbumTitles(format, artistName);
		}

        #endregion

        #region Playlists

        public Playlist SelectPlaylist(Guid playlistId)
        {
            return _gateway.SelectPlaylist(playlistId);
        }

        public List<Playlist> SelectPlaylists()
        {
            return _gateway.SelectPlaylists();
        }

        public void InsertPlaylist(Playlist playlist)
        {
            _gateway.InsertPlaylist(playlist);
        }

        public void UpdatePlaylist(Playlist playlist)
        {
            _gateway.UpdatePlaylist(playlist);
        }

        public void DeletePlaylist(Guid playlistId)
        {
            _gateway.DeletePlaylist(playlistId);
        }

        #endregion

        #region Playlist Items

        public List<PlaylistAudioFile> SelectPlaylistItems(Guid playlistId)
        {
            var items = _gateway.SelectPlaylistItems(playlistId);
            return items;
        }

        public void InsertPlaylistItem(PlaylistAudioFile playlist)
        {
            _gateway.InsertPlaylistItem(playlist);
        }

        public void DeletePlaylistItem(Guid playlistId, Guid audioFileId)
        {
            _gateway.DeletePlaylistItem(playlistId, audioFileId);
        }

        #endregion

        #region Markers

        public void InsertMarker(Marker marker)
        {
            _gateway.InsertMarker(marker);
        }

        public Marker SelectMarker(Guid markerId)
        {
            return _gateway.SelectMarker(markerId);
        }

        public List<Marker> SelectMarkers(Guid audioFileId)
        {
            return _gateway.SelectMarkers(audioFileId);
        }

        public void UpdateMarker(Marker marker)
        {
            _gateway.UpdateMarker(marker);
        }

        public void DeleteMarker(Guid markerId)
        {
            _gateway.DeleteMarker(markerId);
        }

        #endregion

        #region Loops
        
        public void InsertLoop(Loop loop)
        {
            _gateway.InsertLoop(loop);
        }
        
        public Loop SelectLoop(Guid loopId)
        {
            return _gateway.SelectLoop(loopId);
        }

        public Loop SelectLoopIncludingSegments(Guid loopId)
        {
            var loop = _gateway.SelectLoop(loopId);
            var segments = _gateway.SelectSegments(loopId);
            loop.Segments.AddRange(segments);
            return loop;
        }
                
        public List<Loop> SelectLoops(Guid audioFileId)
        {
            return _gateway.SelectLoops(audioFileId);
        }

        public List<Loop> SelectLoopsIncludingSegments(Guid audioFileId)
        {
            var loops = _gateway.SelectLoops(audioFileId);
            foreach (var loop in loops)
            {
                var segments = _gateway.SelectSegments(loop.LoopId);
                loop.Segments.AddRange(segments);
            }
            return loops;
        }
                
        public void UpdateLoop(Loop loop)
        {
            _gateway.UpdateLoop(loop);
        }
        
        public void DeleteLoop(Guid loopId)
        {
            _gateway.DeleteLoop(loopId);
        }
        
        #endregion

        #region Segments

        public Segment SelectSegment(Guid segmentId)
        {
            return _gateway.SelectSegment(segmentId);
        }

        public List<Segment> SelectSegments(Guid loopId)
        {
            return _gateway.SelectSegments(loopId);
        }

        public void InsertSegment(Segment segment)
        {
            _gateway.InsertSegment(segment);
        }

        public void UpdateSegment(Segment segment)
        {
            _gateway.UpdateSegment(segment);
        }

        public void DeleteSegment(Guid segmentId)
        {
            _gateway.DeleteSegment(segmentId);
        }

        #endregion

        #region Equalizer Presets

        public void InsertEQPreset(EQPreset preset)
        {
            _gateway.InsertEQPreset(preset);
        }

        public EQPreset SelectEQPreset(Guid presetId)
        {
            return _gateway.SelectEQPreset(presetId);
        }

        public IEnumerable<EQPreset> SelectEQPresets()
        {
            return _gateway.SelectEQPresets();
        }

        public void UpdateEQPreset(EQPreset preset)
        {
            _gateway.UpdateEQPreset(preset);
        }
        
        public void DeleteEQPreset(Guid presetId)
        {
            _gateway.DeleteEQPreset(presetId);
        }
        
        #endregion

    }
}
