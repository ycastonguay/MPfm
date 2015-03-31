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
using System.IO;
using System.Linq;
using System.Text;
using org.sessionsapp.player;
using Sessions.Library.Database;
using Sessions.Library.Database.Interfaces;
using Sessions.Library.Objects;
using Sessions.Library.Services.Interfaces;
using Sessions.Sound.AudioFiles;
using Sessions.Sound.Objects;
using Sessions.Sound.Player;
using Sessions.Sound.Playlists;

namespace Sessions.Library.Services
{
    public class LibraryService : ILibraryService
    {
        private readonly ISQLiteGateway _gateway;

        public LibraryService(string databaseFilePath)
        {

#if IOS || ANDROID || MACOSX || LINUX
            _gateway = new MonoSQLiteGateway(databaseFilePath);
#elif WINDOWSSTORE || WINDOWS_PHONE
            _gateway = new WinRTSQLiteGateway(databaseFilePath);
#else
            _gateway = new SQLiteGateway(databaseFilePath);
#endif
        }

        public static void CreateDatabaseFile(string databaseFilePath)
        {
            // Create database file -- TODO: Replace by interface
            #if IOS || ANDROID || LINUX || MACOSX
            MonoSQLiteGateway.CreateDatabaseFile(databaseFilePath);
            #elif WINDOWSSTORE || WINDOWS_PHONE
            WinRTSQLiteGateway.CreateDatabaseFile(databaseFilePath);
            #else
            SQLiteGateway.CreateDatabaseFile(databaseFilePath);
            #endif
        }
        
        public void ExecuteSQL(string sql)
        {
            _gateway.Execute(sql);
        }

        public void ResetLibrary()
        {
            _gateway.Delete("AudioFiles");
            _gateway.Delete("PlaylistFiles");
            _gateway.Delete("Loops");
            _gateway.Delete("Markers");
        }

        public void RemoveAudioFilesWithBrokenFilePaths()
        {
            var files = SelectAudioFiles();
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
                    DeleteAudioFile(files[a].Id);
                #endif
            }
        }   

        public void AddFolder(string folderPath, bool recursive)
        {       
            bool folderFound = false;
            List<Folder> folders = SelectFolders();
            foreach (Folder folder in folders)
            {
                // Check if the base path is found in the configured path
                if (folderPath.Contains(folder.FolderPath))
                {
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
                    DeleteFolder(folder.FolderId);
            }

            if (!folderFound)
                InsertFolder(folderPath, true);
        }       

        #region Audio Files

        public List<AudioFile> SelectAudioFiles()
        {
            return _gateway.Select<AudioFile>("SELECT * FROM AudioFiles");
        }

        public List<AudioFile> SelectAudioFiles(LibraryQuery query)
        {
            return SelectAudioFiles(query.Format, query.ArtistName, query.AlbumTitle, query.SearchTerms);
        }
        
        public List<AudioFile> SelectAudioFiles(AudioFileFormat format, string artistName, string albumTitle, string search)
        {
            var sql = new StringBuilder();
            sql.AppendLine("SELECT * FROM AudioFiles ");
            
            if(format != AudioFileFormat.All || !String.IsNullOrEmpty(artistName) || !String.IsNullOrEmpty(albumTitle) || !String.IsNullOrEmpty(search))
            {
                sql.AppendLine(" WHERE ");
                
                int count = 0;
                if(format != AudioFileFormat.All)
                {
                    count++;
                    sql.AppendLine(" [FileType] = '" + FormatSQLValue(format.ToString()) + "'");
                }
                if(!String.IsNullOrEmpty(artistName))
                {
                    count++;
                    if(count > 1)
                    {
                        sql.AppendLine(" AND ");
                    }
                    sql.AppendLine(" [ArtistName] = '" + FormatSQLValue(artistName) + "' ");
                }
                if(!String.IsNullOrEmpty(albumTitle))
                {
                    count++;
                    if(count > 1)
                    {
                        sql.AppendLine(" AND ");
                    }
                    sql.AppendLine(" [AlbumTitle] = '" + FormatSQLValue(albumTitle) + "' ");
                }
                if(!String.IsNullOrEmpty(search))
                {
                    // TODO: Fill this 
                }           
            }

            List<AudioFile> audioFiles = _gateway.Select<AudioFile>(sql.ToString());
            return audioFiles;
        }

        public AudioFile SelectAudioFile(Guid audioFileId)
        {
            AudioFile audioFile = _gateway.SelectOne<AudioFile>("SELECT * FROM AudioFiles WHERE AudioFileId = '" + audioFileId.ToString() + "'");
            return audioFile;
        }

        public void InsertAudioFile(AudioFile audioFile)
        {
            _gateway.Insert<AudioFile>(audioFile, "AudioFiles");            
        }

        public void InsertAudioFiles(IEnumerable<AudioFile> audioFiles)
        {
            _gateway.Insert<AudioFile>(audioFiles, "AudioFiles");
        }

        public void UpdateAudioFile(AudioFile audioFile)
        {
            _gateway.Update<AudioFile>(audioFile, "AudioFiles", "AudioFileId", audioFile.Id.ToString());
        }

        public void DeleteAudioFile(Guid audioFileId)
        {
            _gateway.Delete("AudioFiles", "AudioFileId", audioFileId);
        }

        public void DeleteAudioFiles(string basePath)
        {
            _gateway.Delete("AudioFiles", "FilePath LIKE '" + FormatSQLValue(basePath) + "%'");
        }

        public void DeleteAudioFiles(string artistName, string albumTitle)
        {
            string whereClause = string.Format("ArtistName LIKE '{0}'", FormatSQLValue(artistName));
            if (!string.IsNullOrEmpty(albumTitle))
                whereClause += string.Format(" AND AlbumTitle LIKE '{0}'", FormatSQLValue(albumTitle));
            _gateway.Delete("AudioFiles", whereClause);
        }

        public List<string> SelectDistinctArtistNames()
        {
            return SelectDistinctArtistNames(AudioFileFormat.All);
        }

        public List<string> SelectDistinctArtistNames(AudioFileFormat audioFileFormat)
        {
            var artists = new List<string>();
            string sql = "SELECT DISTINCT ArtistName FROM AudioFiles ORDER BY ArtistName";
            if (audioFileFormat != AudioFileFormat.All)
            {
                sql = "SELECT DISTINCT ArtistName FROM AudioFiles WHERE FileType = '" + audioFileFormat.ToString() + "' ORDER BY ArtistName";
            }

            // Select distinct
            IEnumerable<object> list = _gateway.SelectList(sql);
            foreach (object obj in list)
            {
                artists.Add(obj.ToString());
            }

            return artists;
        }

        public Dictionary<string, List<string>> SelectDistinctAlbumTitles()
        {
            return SelectDistinctAlbumTitles(AudioFileFormat.All, string.Empty);
        }
        
        public Dictionary<string, List<string>> SelectDistinctAlbumTitles(AudioFileFormat audioFileFormat)
        {
            return SelectDistinctAlbumTitles(audioFileFormat, string.Empty);
        }       

        public Dictionary<string, List<string>> SelectDistinctAlbumTitles(AudioFileFormat audioFileFormat, string artistName)
        {
            var albums = new Dictionary<string, List<string>>();
            var sql = new StringBuilder();
            sql.AppendLine("SELECT DISTINCT ArtistName, AlbumTitle FROM AudioFiles ");
            if (audioFileFormat != AudioFileFormat.All && !String.IsNullOrEmpty(artistName))
            {               
                sql.AppendLine(" WHERE FileType = '" + audioFileFormat.ToString() + "' AND ArtistName = '" + FormatSQLValue(artistName) + "'");
            }
            else if (audioFileFormat != AudioFileFormat.All)
            {
                sql.AppendLine(" WHERE FileType = '" + audioFileFormat.ToString() + "' ");
            }
            else if(!String.IsNullOrEmpty(artistName))
            {
                sql.AppendLine(" WHERE ArtistName = '" + FormatSQLValue(artistName) + "' ");
            }
            sql.AppendLine(" ORDER BY ArtistName, AlbumTitle ");

            // Select distinct
            List<Tuple<object, object>> listTuple = _gateway.SelectTuple(sql.ToString());
            foreach (Tuple<object, object> tuple in listTuple)
            {
                string artistNameDistinct = tuple.Item1.ToString();
                string albumTitleDistinct = tuple.Item2.ToString();

                if (albums.ContainsKey(artistNameDistinct))
                {
                    albums[artistNameDistinct].Add(albumTitleDistinct);
                }
                else
                {
                    albums.Add(artistNameDistinct, new List<string>() { albumTitleDistinct });
                }
            }      

            return albums;
        }
        
        public IEnumerable<string> SelectFilePaths()
        {
            IEnumerable<object> listObjects = _gateway.SelectList("SELECT FilePath FROM AudioFiles");
            return listObjects.Select(x => x.ToString());
        }

        public IEnumerable<string> SelectFilePathsRelatedToCueFiles()
        {
            IEnumerable<object> listObjects = _gateway.SelectList("SELECT FilePath FROM AudioFiles WHERE CueFilePath IS NOT NULL");
            return listObjects.Select(x => x.ToString());
        }

        public void UpdatePlayCount(Guid audioFileId)
        {
            AudioFile audioFile = SelectAudioFile(audioFileId);
            if (audioFile == null)
                throw new Exception("Error; The audiofile was not found!");

            string lastPlayed = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            _gateway.Execute("UPDATE AudioFiles SET PlayCount = " + (audioFile.PlayCount + 1).ToString() + ", LastPlayedDateTime = '" + lastPlayed + "' WHERE AudioFileId = '" + audioFileId.ToString() + "'");
        }

        #endregion

        #region Folders

        public Folder SelectFolderByPath(string path)
        {
            return _gateway.SelectOne<Folder>("SELECT * FROM Folders WHERE FolderPath = '" + FormatSQLValue(path) + "'");
        }

        public List<Folder> SelectFolders()
        {
            return _gateway.Select<Folder>("SELECT * FROM Folders");
        }

        public void InsertFolder(string folderPath, bool recursive)
        {
            var folder = new Folder();
            folder.FolderPath = folderPath;
            folder.IsRecursive = recursive;
            //folder.LastUpdated = DateTime.Now;
            _gateway.Insert<Folder>(folder, "Folders");
        }

        public void DeleteFolder(Guid folderId)
        {
            _gateway.Delete("Folders", "FolderId", folderId);
        }

        public void DeleteFolders()
        {
            _gateway.Delete("Folders");
        }

        #endregion

        #region Equalizers

        public List<SSPEQPreset> SelectEQPresets()
        {
            return _gateway.Select<SSPEQPreset>("SELECT * FROM EQPresets");
        }

        public SSPEQPreset SelectEQPreset(Guid presetId)
        {
            return _gateway.SelectOne<SSPEQPreset>("SELECT * FROM EQPresets WHERE EQPresetId = '" + FormatSQLValue(presetId.ToString()) + "'");
        }

        public SSPEQPreset SelectEQPreset(string name)
        {
            return _gateway.SelectOne<SSPEQPreset>("SELECT * FROM EQPresets WHERE Name = '" + FormatSQLValue(name) + "'");
        }

        public void InsertEQPreset(SSPEQPreset eq)
        {
            _gateway.Insert<SSPEQPreset>(eq, "EQPresets");
        }

        public void UpdateEQPreset(SSPEQPreset eq)
        {
            _gateway.Update<SSPEQPreset>(eq, "EQPresets", "EQPresetId", eq.EQPresetId);            
        }

        public void DeleteEQPreset(Guid eqPresetId)
        {
            _gateway.Delete("EQPresets", "EQPresetId", eqPresetId);
        }

        #endregion

        #region Markers

        public List<Marker> SelectMarkers()
        {
            return _gateway.Select<Marker>("SELECT * FROM Markers");
        }

        public List<Marker> SelectMarkers(Guid audioFileId)
        {
            return _gateway.Select<Marker>("SELECT * FROM Markers WHERE AudioFileId = '" + audioFileId.ToString() + "' ORDER BY PositionBytes");
        }

        public Marker SelectMarker(Guid markerId)
        {
            return _gateway.SelectOne<Marker>("SELECT * FROM Markers WHERE MarkerId = '" + markerId.ToString() + "'");
        }

        public void InsertMarker(Marker dto)
        {
            _gateway.Insert<Marker>(dto, "Markers");
        }

        public void UpdateMarker(Marker dto)
        {
            _gateway.Update<Marker>(dto, "Markers", "MarkerId", dto.MarkerId.ToString());
        }

        public void DeleteMarker(Guid markerId)
        {
            _gateway.Delete("Markers", "MarkerId", markerId);
        }

        #endregion

        #region Loops

        public List<SSPLoop> SelectLoops()
        {
            return _gateway.Select<SSPLoop>("SELECT * FROM Loops");
        }

        public List<SSPLoop> SelectLoops(Guid audioFileId)
        {
            return _gateway.Select<SSPLoop>("SELECT * FROM Loops WHERE AudioFileId = '" + audioFileId.ToString() + "' ORDER BY LengthBytes");
        }

        public SSPLoop SelectLoop(Guid loopId)
        {
            return _gateway.SelectOne<SSPLoop>("SELECT * FROM Loops WHERE LoopId = '" + loopId.ToString() + "'");
        }

        public void InsertLoop(SSPLoop dto)
        {
            _gateway.Insert<SSPLoop>(dto, "Loops");
        }

        public void UpdateLoop(SSPLoop dto)
        {
            _gateway.Update<SSPLoop>(dto, "Loops", "LoopId", dto.LoopId.ToString());
        }

        public void DeleteLoop(Guid loopId)
        {            
            _gateway.Delete("Loops", "LoopId", loopId);
        }

        #endregion

        // TODO: Is this still used? I thought Playlists were no longer saved in the database? Maybe it was in prevision of doing resume playback?
        #region Playlists

        public List<Playlist> SelectPlaylists()
        {
            return _gateway.Select<Playlist>("SELECT * FROM Playlists");
        }

        public Playlist SelectPlaylist(Guid playlistId)
        {
            return _gateway.SelectOne<Playlist>("SELECT * FROM Playlists WHERE PlaylistId = '" + playlistId.ToString() + "'");
        }

        public void InsertPlaylist(Playlist playlist)
        {
            _gateway.Insert<Playlist>(playlist, "Playlists");
        }

        public void UpdatePlaylist(Playlist playlist)
        {
            _gateway.Update<Playlist>(playlist, "Playlists", "PlaylistId", playlist.PlaylistId.ToString());
        }

        public void DeletePlaylist(Guid playlistId)
        {
            _gateway.Delete("Playlists", "PlaylistId", playlistId);
        }

        #endregion

        #region Settings

        public List<Setting> SelectSettings()
        {
            return _gateway.Select<Setting>("SELECT * FROM Settings");
        }

        public Setting SelectSetting(string name)
        {
            return _gateway.SelectOne<Setting>("SELECT * FROM Settings WHERE SettingName = '" + FormatSQLValue(name) + "'");
        }

        public void InsertSetting(string name, string value)
        {
            var setting = new Setting();
            setting.SettingName = name;
            setting.SettingValue = value;
            InsertSetting(setting);
        }

        public void InsertSetting(Setting dto)
        {
            _gateway.Insert<Setting>(dto, "Settings");
        }

        public void UpdateSetting(Setting dto)
        {
            _gateway.Update<Setting>(dto, "Settings", "SettingId", dto.SettingId.ToString());
        }

        public void DeleteSetting(Guid settingId)
        {
            _gateway.Delete("Settings", "SettingId", settingId);
        }

        #endregion

        #region Playlist Files

        public List<PlaylistFile> SelectPlaylistFiles()
        {
            return _gateway.Select<PlaylistFile>("SELECT * FROM PlaylistFiles");
        }

        public void InsertPlaylistFile(PlaylistFile dto)
        {
            _gateway.Insert<PlaylistFile>(dto, "PlaylistFiles");
        }

        public void DeletePlaylistFile(string filePath)
        {
            _gateway.Delete("PlaylistFiles", "FilePath = '" + FormatSQLValue(filePath) + "'");
        }

        #endregion

        #region History

        public int GetAudioFilePlayCountFromHistory(Guid audioFileId)
        {
            int playCount = 0;
            string query = "SELECT COUNT(*) FROM History WHERE AudioFileId = '" + audioFileId.ToString() + "'";
            object value = _gateway.ExecuteScalar(query);

            bool isNull = false;
            #if WINDOWSSTORE
            isNull = value == null;
            #else
            isNull = value == DBNull.Value;
            #endif

            if (isNull)
                return 0;

            try
            {
                playCount = (int)value;
            }
            catch
            {
                return 0;
            }
            
            return playCount;
        }

        /// <summary>
        /// Returns the last time the audio file has been played.
        /// The value is null if the file has never been played.
        /// </summary>
        /// <param name="audioFileId">Audio file identifier</param>
        /// <returns>Nullable date/time</returns>
        public DateTime? GetAudioFileLastPlayedFromHistory(Guid audioFileId)
        {
            DateTime? lastPlayed = null;
            string query = "SELECT TOP 1 FROM History WHERE AudioFileId = '" + audioFileId.ToString() + "' ORDER BY EventDateTime";
            object value = _gateway.ExecuteScalar(query);

            bool isNull = false;
#if WINDOWSSTORE
            isNull = value == null;
#else
            isNull = value == DBNull.Value;
#endif
            if (isNull)
                return null;

            try
            {
                DateTime valueDateTime;
                bool success = DateTime.TryParse(value.ToString(), out valueDateTime);
                if (success)
                    lastPlayed = valueDateTime;
            }
            catch
            {
                return null;
            }

            return lastPlayed;
        }

        public void InsertHistory(Guid audioFileId)
        {
            var history = new History();
            history.AudioFileId = audioFileId;
            history.EventDateTime = DateTime.Now;
            _gateway.Insert<History>(history, "History");
        }

        public void InsertHistory(Guid audioFileId, DateTime eventDateTime)
        {
            var history = new History();
            history.AudioFileId = audioFileId;
            history.EventDateTime = eventDateTime;
            _gateway.Insert<History>(history, "History");
        }

        #endregion

        public void CompactDatabase()
        {
            _gateway.CompactDatabase();
        }

        private string FormatSQLValue(string value)
        {
            return value.Replace("'", "''");
        }
    }
}
