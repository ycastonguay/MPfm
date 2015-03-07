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
using Sessions.Sound.AudioFiles;
using Sessions.Sound.Playlists;
using org.sessionsapp.player;
using Sessions.Sound.Objects;
using Sessions.Sound.Player;

namespace Sessions.Library.Database.Interfaces
{
    public interface IDatabaseFacade
    {
        void DeleteAudioFile(Guid audioFileId);
        void DeleteAudioFiles(string basePath);
        void DeleteAudioFiles(string artistName, string albumTitle);
        void DeleteEQPreset(Guid eqPresetId);
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
        void InsertAudioFiles(IEnumerable<AudioFile> audioFiles);
        void InsertEQPreset(SSPEQPreset eq);
        void InsertFolder(string folderPath, bool recursive);
        void InsertHistory(Guid audioFileId);
        void InsertHistory(Guid audioFileId, DateTime eventDateTime);
        void InsertLoop(SSPLoop dto);
        void InsertMarker(Marker dto);
        void InsertPlaylistFile(PlaylistFile dto);
        void InsertSetting(Setting dto);
        void InsertSetting(string name, string value);
        void ResetLibrary();
		
		IEnumerable<string> SelectFilePaths();
        IEnumerable<string> SelectFilePathsRelatedToCueFiles();
        AudioFile SelectAudioFile(Guid audioFileId);
        List<AudioFile> SelectAudioFiles();
		List<AudioFile> SelectAudioFiles(AudioFileFormat format, string artistName, string albumTitle, string search);
        Dictionary<string, List<string>> SelectDistinctAlbumTitles();
        Dictionary<string, List<string>> SelectDistinctAlbumTitles(AudioFileFormat audioFileFormat);
		Dictionary<string, List<string>> SelectDistinctAlbumTitles(AudioFileFormat audioFileFormat, string artistName);
        List<string> SelectDistinctArtistNames();
        List<string> SelectDistinctArtistNames(AudioFileFormat audioFileFormat);
        SSPEQPreset SelectEQPreset(string name);
        SSPEQPreset SelectEQPreset(Guid presetId);
        List<SSPEQPreset> SelectEQPresets();
        Folder SelectFolderByPath(string path);
        List<Folder> SelectFolders();
        SSPLoop SelectLoop(Guid loopId);
        List<SSPLoop> SelectLoops();
        List<SSPLoop> SelectLoops(Guid audioFileId);
        Marker SelectMarker(Guid markerId);
        List<Marker> SelectMarkers();
        List<Marker> SelectMarkers(Guid audioFileId);
        List<PlaylistFile> SelectPlaylistFiles();
        Setting SelectSetting(string name);
        List<Setting> SelectSettings();
        void UpdateAudioFile(AudioFile audioFile);
        void UpdateEQPreset(SSPEQPreset eq);
        void UpdateLoop(SSPLoop dto);
        void UpdateMarker(Marker dto);
        void UpdatePlayCount(Guid audioFileId);
        void UpdateSetting(Setting dto);

        List<Playlist> SelectPlaylists();
        Playlist SelectPlaylist(Guid playlistId);
        void InsertPlaylist(Playlist playlist);
        void UpdatePlaylist(Playlist playlist);
        void DeletePlaylist(Guid playlistId);

        List<PlaylistAudioFile> SelectPlaylistItems(Guid playlistId);
        void InsertPlaylistItem(PlaylistAudioFile playlist);
        void DeletePlaylistItem(Guid playlistId, Guid audioFileId);

		void CompactDatabase();
    }
}
