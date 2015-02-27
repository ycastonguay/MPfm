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
using Sessions.Library.Objects;
using Sessions.Player.Objects;
using Sessions.Sound.AudioFiles;
using Sessions.Sound.Playlists;
using org.sessionsapp.player;

namespace Sessions.Library.Services.Interfaces
{
    /// <summary>
    /// Interface for the LibraryService class.
    /// </summary>
    public interface ILibraryService
    {
		IEnumerable<string> SelectFilePaths();
        IEnumerable<string> SelectFilePathsRelatedToCueFiles();
		IEnumerable<Folder> SelectFolders();
		void RemoveAudioFilesWithBrokenFilePaths();
        void ResetLibrary();
		
		IEnumerable<AudioFile> SelectAudioFiles();
        IEnumerable<AudioFile> SelectAudioFiles(LibraryQuery query);
		IEnumerable<AudioFile> SelectAudioFiles(AudioFileFormat format, string artistName, string albumTitle, string search);		
		void InsertAudioFile(AudioFile audioFile);
        void InsertAudioFiles(IEnumerable<AudioFile> audioFiles);
		void InsertPlaylistFile(PlaylistFile playlistFile);
        void DeleteAudioFile(Guid audioFileId);
        void DeleteAudioFiles(string basePath);
        void DeleteAudioFiles(string artistName, string albumTitle);

		void CompactDatabase();
		void AddFiles(List<string> filePaths);
		void AddFolder(string folderPath, bool recursive);
		
		List<string> SelectDistinctArtistNames(AudioFileFormat format);
		Dictionary<string, List<string>> SelectDistinctAlbumTitles(AudioFileFormat format);
		Dictionary<string, List<string>> SelectDistinctAlbumTitles(AudioFileFormat format, string artistName);

        Playlist SelectPlaylist(Guid playlistId);
        List<Playlist> SelectPlaylists();
        void InsertPlaylist(Playlist playlist);
        void UpdatePlaylist(Playlist playlist);
        void DeletePlaylist(Guid playlistId);

        List<PlaylistAudioFile> SelectPlaylistItems(Guid playlistId);
        void InsertPlaylistItem(PlaylistAudioFile playlist);
        void DeletePlaylistItem(Guid playlistId, Guid audioFileId);

        Marker SelectMarker(Guid markerId);
        List<Marker> SelectMarkers(Guid audioFileId);
        void InsertMarker(Marker marker);
        void UpdateMarker(Marker marker);
        void DeleteMarker(Guid markerId);

        SSPLoop SelectLoop(Guid LoopId);
        List<SSPLoop> SelectLoops(Guid audioFileId);
        void InsertLoop(SSPLoop Loop);
        void UpdateLoop(SSPLoop Loop);
        void DeleteLoop(Guid LoopId);

        SSPEQPreset SelectEQPreset(Guid presetId);
        IEnumerable<SSPEQPreset> SelectEQPresets();
        void InsertEQPreset(SSPEQPreset preset);
        void UpdateEQPreset(SSPEQPreset preset);
        void DeleteEQPreset(Guid presetId);
    }
}
