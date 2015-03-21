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

namespace Sessions.Library.Services.Interfaces
{
    public interface ILibraryService
    {
        void ExecuteSQL(string sql);
        void ResetLibrary();
    
        void RemoveAudioFilesWithBrokenFilePaths();
        void AddFolder(string folderPath, bool recursive);

        IEnumerable<string> SelectFilePaths();
        IEnumerable<string> SelectFilePathsRelatedToCueFiles();
        AudioFile SelectAudioFile(Guid audioFileId);
        List<AudioFile> SelectAudioFiles();
        List<AudioFile> SelectAudioFiles(LibraryQuery query);
        List<AudioFile> SelectAudioFiles(AudioFileFormat format, string artistName, string albumTitle, string search);
        Dictionary<string, List<string>> SelectDistinctAlbumTitles();
        Dictionary<string, List<string>> SelectDistinctAlbumTitles(AudioFileFormat audioFileFormat);
        Dictionary<string, List<string>> SelectDistinctAlbumTitles(AudioFileFormat audioFileFormat, string artistName);
        List<string> SelectDistinctArtistNames();
        List<string> SelectDistinctArtistNames(AudioFileFormat audioFileFormat);

        DateTime? GetAudioFileLastPlayedFromHistory(Guid audioFileId);
        int GetAudioFilePlayCountFromHistory(Guid audioFileId);
        void InsertAudioFile(AudioFile audioFile);
        void InsertAudioFiles(IEnumerable<AudioFile> audioFiles);
        void UpdateAudioFile(AudioFile audioFile);
        void DeleteAudioFile(Guid audioFileId);
        void DeleteAudioFiles(string basePath);
        void DeleteAudioFiles(string artistName, string albumTitle);

        SSPEQPreset SelectEQPreset(string name);
        SSPEQPreset SelectEQPreset(Guid presetId);
        List<SSPEQPreset> SelectEQPresets();
        void InsertEQPreset(SSPEQPreset eq);
        void UpdateEQPreset(SSPEQPreset eq);
        void DeleteEQPreset(Guid eqPresetId);

        Folder SelectFolderByPath(string path);
        List<Folder> SelectFolders();
        void InsertFolder(string folderPath, bool recursive);
        void DeleteFolder(Guid folderId);
        void DeleteFolders();

        SSPLoop SelectLoop(Guid loopId);
        List<SSPLoop> SelectLoops();
        List<SSPLoop> SelectLoops(Guid audioFileId);
        void InsertLoop(SSPLoop dto);
        void UpdateLoop(SSPLoop dto);
        void DeleteLoop(Guid loopId);

        Marker SelectMarker(Guid markerId);
        List<Marker> SelectMarkers();
        List<Marker> SelectMarkers(Guid audioFileId);
        void InsertMarker(Marker dto);
        void UpdateMarker(Marker dto);
        void DeleteMarker(Guid markerId);

        List<PlaylistFile> SelectPlaylistFiles();
        void InsertPlaylistFile(PlaylistFile dto);
        void DeletePlaylistFile(string filePath);

        Setting SelectSetting(string name);
        List<Setting> SelectSettings();
        void InsertSetting(Setting dto);
        void InsertSetting(string name, string value);
        void UpdateSetting(Setting dto);
        void DeleteSetting(Guid settingId);

        void InsertHistory(Guid audioFileId);
        void InsertHistory(Guid audioFileId, DateTime eventDateTime);
        
        void UpdatePlayCount(Guid audioFileId);

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
