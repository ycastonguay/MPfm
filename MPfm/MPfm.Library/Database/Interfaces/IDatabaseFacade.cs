//
// IDatabaseFacade.cs: Interface for the DatabaseFacade class.
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
using MPfm.Library.Objects;
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;

namespace MPfm.Library.Database.Interfaces
{
    /// <summary>
    /// Interface for the DatabaseFacade class.
    /// </summary>
    public interface IDatabaseFacade
    {
        void DeleteAudioFile(Guid audioFileId);
        void DeleteAudioFiles(string basePath);
        void DeleteEqualizer(Guid eqPresetId);
        void DeleteFolder(Guid folderId);
        void DeleteFolders();
        void DeleteLoop(Guid loopId);
        void DeleteMarker(Guid markerId);
        void DeletePlaylistFile(string filePath);
        void DeleteSetting(Guid settingId);
        void ExecuteSQL(string sql);
        DateTime? GetAudioFileLastPlayedFromHistory(Guid audioFileId);
        int GetAudioFilePlayCountFromHistory(Guid audioFileId);
        void InsertAudioFile(AudioFile audioFile);
        void InsertEqualizer(EQPreset eq);
        void InsertFolder(string folderPath, bool recursive);
        void InsertHistory(Guid audioFileId);
        void InsertHistory(Guid audioFileId, DateTime eventDateTime);
        void InsertLoop(Loop dto);
        void InsertMarker(Marker dto);
        void InsertPlaylistFile(PlaylistFile dto);
        void InsertSetting(Setting dto);
        void InsertSetting(string name, string value);
        void ResetLibrary();
		
		IEnumerable<string> SelectFilePaths();
        AudioFile SelectAudioFile(Guid audioFileId);
        List<AudioFile> SelectAudioFiles();
		List<AudioFile> SelectAudioFiles(AudioFileFormat format, string artistName, string albumTitle, string search);
        Dictionary<string, List<string>> SelectDistinctAlbumTitles();
        Dictionary<string, List<string>> SelectDistinctAlbumTitles(AudioFileFormat audioFileFormat);
		Dictionary<string, List<string>> SelectDistinctAlbumTitles(AudioFileFormat audioFileFormat, string artistName);
        List<string> SelectDistinctArtistNames();
        List<string> SelectDistinctArtistNames(AudioFileFormat audioFileFormat);
        EQPreset SelectEQPreset(string name);
        List<EQPreset> SelectEQPresets();
        Folder SelectFolderByPath(string path);
        List<Folder> SelectFolders();
        Loop SelectLoop(Guid loopId);
        List<Loop> SelectLoops();
        List<Loop> SelectLoops(Guid audioFileId);
        Marker SelectMarker(Guid markerId);
        List<Marker> SelectMarkers();
        List<Marker> SelectMarkers(Guid audioFileId);
        List<PlaylistFile> SelectPlaylistFiles();
        Setting SelectSetting(string name);
        List<Setting> SelectSettings();
        void UpdateAudioFile(AudioFile audioFile);
        void UpdateEqualizer(EQPreset eq);
        void UpdateLoop(Loop dto);
        void UpdateMarker(Marker dto);
        void UpdatePlayCount(Guid audioFileId);
        void UpdateSetting(Setting dto);

		void CompactDatabase();
    }
}
