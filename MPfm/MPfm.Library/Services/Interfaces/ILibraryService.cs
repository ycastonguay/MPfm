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
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;

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
		IEnumerable<AudioFile> SelectAudioFiles(AudioFileFormat format, string artistName, string albumTitle, string search);
		
		void InsertAudioFile(AudioFile audioFile);
		void InsertPlaylistFile(PlaylistFile playlistFile);
		
		void CompactDatabase();
		void AddFiles(List<string> filePaths);
		void AddFolder(string folderPath, bool recursive);
		
		List<string> SelectDistinctArtistNames(AudioFileFormat format);
		Dictionary<string, List<string>> SelectDistinctAlbumTitles(AudioFileFormat format);
		Dictionary<string, List<string>> SelectDistinctAlbumTitles(AudioFileFormat format, string artistName);

        Marker SelectMarker(Guid markerId);
        List<Marker> SelectMarkers(Guid audioFileId);
        void InsertMarker(Marker marker);
        void UpdateMarker(Marker marker);
        void DeleteMarker(Guid markerId);

        Loop SelectLoop(Guid LoopId);
        List<Loop> SelectLoops(Guid audioFileId);
        void InsertLoop(Loop Loop);
        void UpdateLoop(Loop Loop);
        void DeleteLoop(Guid LoopId);

        EQPreset SelectEQPreset(Guid presetId);
        IEnumerable<EQPreset> SelectEQPresets();
        void InsertEQPreset(EQPreset preset);
        void UpdateEQPreset(EQPreset preset);
        void DeleteEQPreset(Guid presetId);
    }
}
