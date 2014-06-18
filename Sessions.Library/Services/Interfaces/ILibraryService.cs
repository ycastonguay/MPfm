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
using MPfm.Library.Objects;
using Sessions.Player.Objects;
using Sessions.Sound.AudioFiles;
using Sessions.Sound.Playlists;

namespace MPfm.Library.Services.Interfaces
{
    /// <summary>
    /// Interface for the LibraryService class.
    /// </summary>
    public interface ILibraryService
    {
		IEnumerable<string> SelectFilePaths();
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

        Loop SelectLoop(Guid LoopId);
        Loop SelectLoopIncludingSegments(Guid loopId);
        List<Loop> SelectLoops(Guid audioFileId);
        List<Loop> SelectLoopsIncludingSegments(Guid audioFileId);
        void InsertLoop(Loop Loop);
        void UpdateLoop(Loop Loop);
        void DeleteLoop(Guid LoopId);

        Segment SelectSegment(Guid segmentId);
        List<Segment> SelectSegments(Guid loopId);
        void InsertSegment(Segment segment);
        void UpdateSegment(Segment segment);
        void DeleteSegment(Guid segmentId);

        EQPreset SelectEQPreset(Guid presetId);
        IEnumerable<EQPreset> SelectEQPresets();
        void InsertEQPreset(EQPreset preset);
        void UpdateEQPreset(EQPreset preset);
        void DeleteEQPreset(Guid presetId);
    }
}
