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
using System.Linq;
using System.Text;
using MPfm.Library.Database.Interfaces;
using MPfm.Library.Objects;
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;

namespace MPfm.Library.Database
{
    /// <summary>
    /// The DatabaseFacade class implements the SQLiteGateway class.
    /// It acts as a facade to select, insert, update and delete data from the
    /// MPfm database.
    /// </summary>
    public class DatabaseFacade : IDatabaseFacade
    {
        private readonly ISQLiteGateway _gateway;

        /// <summary>
        /// Default constructor for the DatabaseFacade class.
        /// </summary>
        /// <param name="databaseFilePath">Database file path</param>
        public DatabaseFacade(string databaseFilePath)
        {

#if IOS || ANDROID || MACOSX || LINUX
            _gateway = new MonoSQLiteGateway(databaseFilePath);
#else
            _gateway = new SQLiteGateway(databaseFilePath);
#endif
        }
		
        /// <summary>
        /// Executes an SQL statement.
        /// </summary>
        /// <param name="sql">SQL statement to execute</param>
        public void ExecuteSQL(string sql)
        {
            _gateway.Execute(sql);
        }

        /// <summary>
        /// Resets the library.
        /// </summary>
        public void ResetLibrary()
        {
            // Delete all table content
            _gateway.Delete("AudioFiles");
            _gateway.Delete("PlaylistFiles");
            _gateway.Delete("Loops");
            _gateway.Delete("Markers");
        }

        #region Audio Files
        
        /// <summary>
        /// Selects all audio files from the database.
        /// </summary>
        /// <returns>List of AudioFiles</returns>
        public List<AudioFile> SelectAudioFiles()
        {
            List<AudioFile> audioFiles = _gateway.Select<AudioFile>("SELECT * FROM AudioFiles");
            return audioFiles;
        }
		
		public List<AudioFile> SelectAudioFiles(AudioFileFormat format, string artistName, string albumTitle, string search)
		{
			StringBuilder sql = new StringBuilder();
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

				}			
			}

            List<AudioFile> audioFiles = _gateway.Select<AudioFile>(sql.ToString());
			return audioFiles;
		}

        /// <summary>
        /// Selects a specific audio file from the database by its identifier.
        /// </summary>
        /// <param name="audioFileId">Audio file unique identifier</param>
        /// <returns>AudioFile</returns>
        public AudioFile SelectAudioFile(Guid audioFileId)
        {
            AudioFile audioFile = _gateway.SelectOne<AudioFile>("SELECT * FROM AudioFiles WHERE AudioFileId = '" + audioFileId.ToString() + "'");
            return audioFile;
        }

        /// <summary>
        /// Inserts a new audio file into the database.
        /// </summary>
        /// <param name="audioFile">AudioFile to insert</param>
        public void InsertAudioFile(AudioFile audioFile)
        {
            _gateway.Insert<AudioFile>(audioFile, "AudioFiles");            
        }

        ///// <summary>
        ///// Inserts new audio files into the database.
        ///// </summary>
        ///// <param name="audioFiles">List of AudioFiles to insert</param>
        //public void InsertAudioFiles(List<AudioFile> audioFiles)
        //{
        //    // Insert song
        //    //Insert("AudioFiles", "AudioFileId", audioFiles);
        //    string tableName = "AudioFiles";
        //    string idFieldName = "AudioFileId";

        //    // Get empty result set
        //    string baseQuery = "SELECT * FROM " + tableName;
        //    DataTable table = Select(baseQuery + " WHERE " + idFieldName + " = ''");

        //    // Loop through objects
        //    foreach (AudioFile audioFile in audioFiles)
        //    {
        //        // Add new row to data table
        //        DataRow newRow = table.NewRow();
        //        table.Rows.Add(newRow);
        //        ConvertLibrary.ToRow(ref newRow, audioFile);
        //    }

        //    // Insert new row into database
        //    UpdateDataTableTransaction(table, baseQuery);
        //}

        /// <summary>
        /// Updates an existing audio file to the database.
        /// </summary>
        /// <param name="audioFile">AudioFile to update</param>
        public void UpdateAudioFile(AudioFile audioFile)
        {
            _gateway.Update<AudioFile>(audioFile, "AudioFiles", "AudioFileId", audioFile.Id.ToString());
        }

        /// <summary>
        /// Deletes an audio file from the database.
        /// </summary>
        /// <param name="audioFileId">AudioFile to delete</param>
        public void DeleteAudioFile(Guid audioFileId)
        {
            _gateway.Delete("AudioFiles", "AudioFileId", audioFileId);
        }

        /// <summary>
        /// Deletes audio files from the database based on their file path.
        /// </summary>
        /// <param name="basePath">Base audio file path</param>
        public void DeleteAudioFiles(string basePath)
        {
            _gateway.Delete("AudioFiles", "FilePath LIKE '" + FormatSQLValue(basePath) + "%'");
        }

        /// <summary>
        /// Returns the distinct list of artist names from the database.        
        /// </summary>        
        /// <returns>List of distinct artist names</returns>
        public List<string> SelectDistinctArtistNames()
        {
            return SelectDistinctArtistNames(AudioFileFormat.All);
        }

        /// <summary>
        /// Returns the distinct list of artist names from the database, using the sound format
        /// filter passed in the soundFormat parameter.
        /// </summary>
        /// <param name="audioFileFormat">Audio file format filter</param>
        /// <returns>List of distinct artist names</returns>
        public List<string> SelectDistinctArtistNames(AudioFileFormat audioFileFormat)
        {
            // Create list
            List<string> artists = new List<string>();

            // Set query
            string sql = "SELECT DISTINCT ArtistName FROM AudioFiles ORDER BY ArtistName";

            // Check for audio file format filter
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

        /// <summary>
        /// Returns the distinct list of album titles per artist from the database.        
        /// </summary>        
        /// <returns>List of distinct album titles per artist</returns>
        public Dictionary<string, List<string>> SelectDistinctAlbumTitles()
        {
            return SelectDistinctAlbumTitles(AudioFileFormat.All, string.Empty);
        }
		
        /// <summary>
        /// Returns the distinct list of album titles per artist from the database.        
        /// </summary>        
        /// <returns>List of distinct album titles per artist</returns>
        public Dictionary<string, List<string>> SelectDistinctAlbumTitles(AudioFileFormat audioFileFormat)
        {
            return SelectDistinctAlbumTitles(audioFileFormat, string.Empty);
        }		

        /// <summary>
        /// Returns the distinct list of album titles per artist from the database, using the sound format
        /// filter passed in the soundFormat parameter.
        /// </summary>
        /// <param name="audioFileFormat">Audio file format filter</param>
        /// <param name="artistName">Artist name filter</param>
        /// <returns>List of distinct album titles per artist</returns>
        public Dictionary<string, List<string>> SelectDistinctAlbumTitles(AudioFileFormat audioFileFormat, string artistName)
        {
            // Create dictionary
            Dictionary<string, List<string>> albums = new Dictionary<string, List<string>>();

            // Set query
            StringBuilder sql = new StringBuilder();
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
			sql.AppendLine(" ORDER BY ArtistName");

            // Select distinct
            List<Tuple<object, object>> listTuple = _gateway.SelectTuple(sql.ToString());
            foreach (Tuple<object, object> tuple in listTuple)
            {
                // Get values
                string artistNameDistinct = tuple.Item1.ToString();
                string albumTitleDistinct = tuple.Item2.ToString();

                // Add value to dictionary
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
		
		/// <summary>
		/// Returns the list of file paths in the Song table.
		/// </summary>
		/// <returns>List of file paths</returns>
		public IEnumerable<string> SelectFilePaths()
		{
            IEnumerable<object> listObjects = _gateway.SelectList("SELECT FilePath FROM AudioFiles");
			return listObjects.Select(x => x.ToString());
		}

        ///// <summary>
        ///// Returns the distinct list of album titles with the path of at least one song of the album from the database, 
        ///// using the sound format filter passed in the soundFormat parameter. This is useful for displaying album art
        ///// for example (no need to return all songs from every album).
        ///// </summary>
        ///// <param name="audioFileFormat">Audio file format filter (use Unknown to skip filter)</param>
        ///// <returns>List of distinct album titles with file paths</returns>
        //public Dictionary<string, string> SelectDistinctAlbumTitlesWithFilePaths(AudioFileFormat audioFileFormat)
        //{
        //    // Create dictionary
        //    Dictionary<string, string> albums = new Dictionary<string, string>();

        //    // Set query
        //    string sql = "SELECT DISTINCT AlbumTitle, FilePath FROM AudioFiles";
        //    if (audioFileFormat != AudioFileFormat.All)
        //    {
        //        sql = "SELECT DISTINCT AlbumTitle, FilePath FROM AudioFiles WHERE FileType = '" + audioFileFormat.ToString() + "' ORDER BY ArtistName";
        //    }

        //    // Select distinct
        //    DataTable table = Select(sql);

        //    // Convert into a list of strings
        //    for (int a = 0; a < table.Rows.Count; a++)
        //    {
        //        // Get values                
        //        string albumTitle = table.Rows[a]["AlbumTitle"].ToString();
        //        string filePath = table.Rows[a]["FilePath"].ToString();

        //        // Add item to dictionary
        //        if (!albums.ContainsKey(albumTitle))
        //        {
        //            albums.Add(albumTitle, filePath);
        //        }
        //    }

        //    return albums;
        //}

        /// <summary>
        /// Updates the play count of an audio file and sets the last playback datetime.
        /// </summary>
        /// <param name="audioFileId">AudioFile identifier</param>
        public void UpdatePlayCount(Guid audioFileId)
        {
            // Get play count
            AudioFile audioFile = SelectAudioFile(audioFileId);

            // Check if the audiofile was found
            if (audioFile == null)
            {
                throw new Exception("Error; The audiofile was not found!");
            }

            string lastPlayed = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            _gateway.Execute("UPDATE AudioFiles SET PlayCount = " + (audioFile.PlayCount + 1).ToString() + ", LastPlayedDateTime = '" + lastPlayed + "' WHERE AudioFileId = '" + audioFileId.ToString() + "'");
        }

        #endregion

        #region Folders

        /// <summary>
        /// Selects a folder from a path.
        /// </summary>
        /// <param name="path">Path to the folder</param>
        /// <returns>Folder</returns>
        public Folder SelectFolderByPath(string path)
        {
            Folder folder = _gateway.SelectOne<Folder>("SELECT * FROM Folders WHERE FolderPath = '" + FormatSQLValue(path) + "'");
            return folder;
        }

        /// <summary>
        /// Selects a folders.
        /// </summary>        
        /// <returns>List of folders</returns>
        public List<Folder> SelectFolders()
        {
            List<Folder> folders = _gateway.Select<Folder>("SELECT * FROM Folders");
            return folders;
        }

        /// <summary>
        /// Inserts a folder in the database. Configuration for library location.
        /// </summary>
        /// <param name="folderPath">Path of the folder to add</param>
        /// <param name="recursive">If resursive</param>
        public void InsertFolder(string folderPath, bool recursive)
        {
            // Insert new folder
            Folder folder = new Folder();
            folder.FolderPath = folderPath;
            folder.IsRecursive = recursive;
            _gateway.Insert<Folder>(folder, "Folders");
        }

        /// <summary>
        /// Deletes a specific folder.
        /// </summary>
        /// <param name="folderId">FolderId</param>
        public void DeleteFolder(Guid folderId)
        {
            // Delete folder
            _gateway.Delete("Folders", "FolderId", folderId);
        }

        /// <summary>
        /// Deletes all folders.
        /// </summary>
        public void DeleteFolders()
        {
            // Delete all folders
            _gateway.Delete("Folders");
        }

        #endregion

        #region Equalizers

        /// <summary>
        /// Select all EQ presets from the database.
        /// </summary>
        /// <returns>List of EQPresets</returns>
        public List<EQPreset> SelectEQPresets()
        {
            List<EQPreset> eqs = _gateway.Select<EQPreset>("SELECT * FROM EQPresets");
            return eqs;
        }

        /// <summary>
        /// Selects an EQ preset from the database.
        /// </summary>
        /// <param name="name">EQ preset name</param>
        /// <returns>EQPreset</returns>
        public EQPreset SelectEQPreset(string name)
        {
            EQPreset preset = _gateway.SelectOne<EQPreset>("SELECT * FROM EQPresets WHERE Name = '" + FormatSQLValue(name) + "'");
            return preset;
        }

        /// <summary>
        /// Inserts a new EQ preset into the database.
        /// </summary>
        /// <param name="eq">EQ preset to insert</param>
        public void InsertEqualizer(EQPreset eq)
        {
            _gateway.Insert<EQPreset>(eq, "EQPresets");            
        }

        /// <summary>
        /// Updates an existing EQ preset in the database.
        /// </summary>
        /// <param name="eq">EQ preset to update</param>
        public void UpdateEqualizer(EQPreset eq)
        {
            _gateway.Update<EQPreset>(eq, "EQPresets", "EQPresetId", eq.EQPresetId);            
        }

        /// <summary>
        /// Deletes an EQ preset from the database.
        /// </summary>
        /// <param name="eqPresetId">EQ preset identifier</param>
        public void DeleteEqualizer(Guid eqPresetId)
        {
            // Delete item
            _gateway.Delete("EQPresets", "EQPresetId", eqPresetId);
        }

        #endregion

        #region Markers

        /// <summary>
        /// Selects all markers from the database.
        /// </summary>
        /// <returns>List of Markers</returns>
        public List<Marker> SelectMarkers()
        {
            List<Marker> markers = _gateway.Select<Marker>("SELECT * FROM Markers");
            return markers;
        }

        /// <summary>
        /// Selects markers related to an audio file from the database.
        /// </summary>
        /// <param name="audioFileId">AudioFile identifier</param>
        /// <returns>List of Markers</returns>
        public List<Marker> SelectMarkers(Guid audioFileId)
        {
            List<Marker> markers = _gateway.Select<Marker>("SELECT * FROM Markers");
            return markers;
        }

        /// <summary>
        /// Selects a marker from the database.
        /// </summary>
        /// <param name="markerId">Marker Id</param>
        /// <returns>Marker</returns>
        public Marker SelectMarker(Guid markerId)
        {
            Marker marker = _gateway.SelectOne<Marker>("SELECT * FROM Markers WHERE MarkerId = '" + markerId.ToString() + "'");
            return marker;
        }

        /// <summary>
        /// Inserts a new marker into the database.
        /// </summary>
        /// <param name="dto">Marker to insert</param>
        public void InsertMarker(Marker dto)
        {
            _gateway.Insert<Marker>(dto, "Markers");
        }

        /// <summary>
        /// Updates an existing marker from the database.
        /// </summary>
        /// <param name="dto">Marker to update</param>
        public void UpdateMarker(Marker dto)
        {
            _gateway.Update<Marker>(dto, "Markers", "MarkerId", dto.MarkerId.ToString());
        }

        /// <summary>
        /// Deletes a marker from the database.
        /// </summary>
        /// <param name="markerId">Marker to delete</param>
        public void DeleteMarker(Guid markerId)
        {
            // Delete marker
            _gateway.Delete("Markers", "MarkerId", markerId);
        }

        #endregion

        #region Loops

        /// <summary>
        /// Selects all loops from the database.
        /// </summary>
        /// <returns>List of Loops</returns>
        public List<Loop> SelectLoops()
        {
            List<Loop> loops = _gateway.Select<Loop>("SELECT * FROM Loops");
            return loops;
        }

        /// <summary>
        /// Selects loops related to an audio file from the database.
        /// </summary>
        /// <param name="audioFileId">AudioFile identifier</param>
        /// <returns>List of Loops</returns>
        public List<Loop> SelectLoops(Guid audioFileId)
        {
            List<Loop> loops = _gateway.Select<Loop>("SELECT * FROM Loops  WHERE AudioFileId = '" + audioFileId.ToString() + "' ORDER BY LengthBytes");
            return loops;
        }

        /// <summary>
        /// Selects a loop from the database.
        /// </summary>
        /// <param name="loopId">Loop identifier</param>
        /// <returns>Loop</returns>
        public Loop SelectLoop(Guid loopId)
        {
            Loop loop = _gateway.SelectOne<Loop>("SELECT * FROM Loops WHERE LoopId = '" + loopId.ToString() + "'");
            return loop;
        }

        /// <summary>
        /// Inserts a new loop into the database.
        /// </summary>
        /// <param name="dto">Loop to insert</param>
        public void InsertLoop(Loop dto)
        {
            _gateway.Insert<Loop>(dto, "Loops");
        }

        /// <summary>
        /// Updates an existing loop from the database.
        /// </summary>
        /// <param name="dto">Loop to update</param>
        public void UpdateLoop(Loop dto)
        {
            _gateway.Update<Loop>(dto, "Loops", "LoopId", dto.LoopId.ToString());
        }

        /// <summary>
        /// Deletes a loop from the database.
        /// </summary>
        /// <param name="loopId">Loop to delete</param>
        public void DeleteLoop(Guid loopId)
        {
            // Delete loop
            _gateway.Delete("Loops", "LoopId", loopId);
        }

        #endregion

        #region Playlists

        ///// <summary>
        ///// Selects all the playlists from the database.
        ///// </summary>
        ///// <returns>List of playlists</returns>
        //public static List<Playlist> SelectPlaylists()
        //{
        //    List<Playlist> playlists = null;

        //    try
        //    {
        //        // Open the connection
        //        using (MPFM_EF context = new MPFM_EF())
        //        {
        //            playlists = context.Playlists.OrderBy(x => x.PlaylistName).ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectPlaylists(): " + ex.Message);
        //        throw ex;
        //    }

        //    return playlists;
        //}

        ///// <summary>
        ///// Selects a playlist from the database, using its identifier.
        ///// </summary>
        ///// <param name="playlistId">Playlist identifier</param>
        ///// <returns>Playlist</returns>
        //public static Playlist SelectPlaylist(Guid playlistId)
        //{
        //    Playlist playlist = null;

        //    try
        //    {
        //        // Open the connection
        //        using (MPFM_EF context = new MPFM_EF())
        //        {
        //            string strId = playlistId.ToString();
        //            playlist = context.Playlists.FirstOrDefault(x => x.PlaylistId == strId);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectPlaylist(): " + ex.Message);
        //        throw ex;
        //    }

        //    return playlist;
        //}

        ///// <summary>
        ///// Selects a playlist from the database, using its name.
        ///// </summary>
        ///// <param name="playlistName">Playlist name</param>
        ///// <returns>Playlist</returns>
        //public static Playlist SelectPlaylist(string playlistName)
        //{
        //    Playlist playlist = null;

        //    try
        //    {
        //        // Open the connection
        //        using (MPFM_EF context = new MPFM_EF())
        //        {
        //            playlist = context.Playlists.FirstOrDefault(x => x.PlaylistName.ToUpper() == playlistName.ToUpper());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectPlaylist(): " + ex.Message);
        //        throw ex;
        //    }

        //    return playlist;
        //}

        ///// <summary>
        ///// Checks if a playlist exists in the database, using its name.
        ///// </summary>
        ///// <param name="playlistName">Playlist name</param>
        ///// <returns>True if the playlist exists</returns>
        //public static bool PlaylistExists(string playlistName)
        //{
        //    // Check if playlist exists
        //    if (SelectPlaylist(playlistName) != null)
        //    {
        //        return true;
        //    }

        //    return false;
        //}

        ///// <summary>
        ///// Inserts a playlist into the database.
        ///// </summary>
        ///// <param name="playlist">Playlist to insert</param>
        //public static void InsertPlaylist(Playlist playlist)
        //{
        //    try
        //    {
        //        // Open the connection
        //        using (MPFM_EF context = new MPFM_EF())
        //        {
        //            // Add to database
        //            context.AddToPlaylists(playlist);
        //            context.SaveChanges();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Tracing.Log("MPfm.Library (DataAccess) --  Error in InsertPlaylist(): " + ex.Message);
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// Updates a playlist in the database.
        ///// </summary>
        ///// <param name="playlist">Playlist to update</param>
        //public static void UpdatePlaylist(Playlist playlist)
        //{
        //    try
        //    {
        //        // Open the connection
        //        using (MPFM_EF context = new MPFM_EF())
        //        {
        //            Playlist playlistToModify = context.Playlists.FirstOrDefault(x => x.PlaylistId == playlist.PlaylistId);
        //            if (playlistToModify != null)
        //            {
        //                playlistToModify.PlaylistName = playlist.PlaylistName;
        //                context.SaveChanges();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Tracing.Log("MPfm.Library (DataAccess) --  Error in UpdatePlaylist(): " + ex.Message);
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// Deletes a playlist from the database.
        ///// </summary>
        ///// <param name="playlistId">Playlist identifier to delete</param>
        //public static void DeletePlaylist(Guid playlistId)
        //{
        //    try
        //    {
        //        // Open the connection
        //        using (MPFM_EF context = new MPFM_EF())
        //        {
        //            ExecuteSql(context, "DELETE FROM Playlists WHERE PlaylistId = @PlaylistId", new SQLiteParameter("PlaylistId", playlistId.ToString()));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Tracing.Log("MPfm.Library (DataAccess) --  Error in DeletePlaylist(): " + ex.Message);
        //        throw ex;
        //    }
        //}

        #endregion

        #region Settings

        /// <summary>
        /// Selects all settings.
        /// </summary>        
        /// <returns>List of settings</returns>
        public List<Setting> SelectSettings()
        {
            List<Setting> settings = _gateway.Select<Setting>("SELECT * FROM Settings");
            return settings;
        }

        /// <summary>
        /// Selects a setting by its name.
        /// </summary>
        /// <param name="name">Setting name</param>
        /// <returns>Setting object</returns>
        public Setting SelectSetting(string name)
        {
            Setting setting = _gateway.SelectOne<Setting>("SELECT * FROM Settings WHERE SettingName = '" + FormatSQLValue(name) + "'");
            return setting;
        }

        /// <summary>
        /// Inserts a new setting into the database.
        /// </summary>
        /// <param name="name">Setting name</param>
        /// <param name="value">Setting value</param>
        public void InsertSetting(string name, string value)
        {
            // Create setting
            Setting setting = new Setting();
            setting.SettingName = name;
            setting.SettingValue = value;

            // Insert setting
            InsertSetting(setting);
        }

        /// <summary>
        /// Inserts a new setting into the database.
        /// </summary>
        /// <param name="dto">Setting to insert</param>
        public void InsertSetting(Setting dto)
        {
            _gateway.Insert<Setting>(dto, "Settings");
        }

        /// <summary>
        /// Updates an existing setting from the database.
        /// </summary>
        /// <param name="dto">Setting to update</param>
        public void UpdateSetting(Setting dto)
        {
            _gateway.Update<Setting>(dto, "Settings", "SettingId", dto.SettingId.ToString());
        }

        /// <summary>
        /// Deletes a setting from the database.
        /// </summary>
        /// <param name="settingId">Setting to delete</param>
        public void DeleteSetting(Guid settingId)
        {
            // Delete loop
            _gateway.Delete("Settings", "SettingId", settingId);
        }

        #endregion

        #region Playlist Files

        /// <summary>
        /// Selects all playlist files.
        /// </summary>        
        /// <returns>List of playlist files</returns>
        public List<PlaylistFile> SelectPlaylistFiles()
        {
            List<PlaylistFile> playlistFiles = _gateway.Select<PlaylistFile>("SELECT * FROM PlaylistFiles");
            return playlistFiles;
        }

        /// <summary>
        /// Inserts a new playlist file into the database.
        /// </summary>
        /// <param name="dto">Playlist file</param>
        public void InsertPlaylistFile(PlaylistFile dto)
        {
            _gateway.Insert<PlaylistFile>(dto, "PlaylistFiles");
        }

        /// <summary>
        /// Deletes a playlist file from the database.
        /// </summary>
        /// <param name="filePath">Playlist file path to delete</param>
        public void DeletePlaylistFile(string filePath)
        {
            // Delete loop
            _gateway.Delete("PlaylistFiles", "FilePath = '" + FormatSQLValue(filePath) + "'");
        }

        #endregion

        #region History

        /// <summary>
        /// Returns the number of times the audio file has been played.
        /// </summary>
        /// <param name="audioFileId">Audio file identifier</param>
        /// <returns>Play count</returns>
        public int GetAudioFilePlayCountFromHistory(Guid audioFileId)
        {
            // Declare variables
            int playCount = 0;

            // Execute scalar query
            string query = "SELECT COUNT(*) FROM History WHERE AudioFileId = '" + audioFileId.ToString() + "'";
            object value = _gateway.ExecuteScalar(query);

            // Check value for null
            if (value == DBNull.Value)
            {
                // No play count
                return 0;
            }
            else
            {                
                try
                {
                    // Try to cast into an integer       
                    playCount = (int)value;
                }
                catch
                {
                    // Return no play count
                    return 0;
                }
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
            // Declare variables
            DateTime? lastPlayed = null;

            // Execute scalar query
            string query = "SELECT TOP 1 FROM History WHERE AudioFileId = '" + audioFileId.ToString() + "' ORDER BY EventDateTime";
            object value = _gateway.ExecuteScalar(query);

            // Check value for null
            if (value == DBNull.Value)
            {
                // No last played value
                return null;
            }
            else
            {
                try
                {
                    // Try to cast into a DateTime       
                    DateTime valueDateTime;
                    bool success = DateTime.TryParse(value.ToString(), out valueDateTime);
                    if (success)
                    {
                        lastPlayed = valueDateTime;
                    }
                }
                catch
                {
                    // No last played value
                    return null;
                }
            }

            return lastPlayed;
        }

        /// <summary>
        /// Inserts an history item into the database using the current datetime.
        /// </summary>
        /// <param name="audioFileId">Audio file identifier</param>
        public void InsertHistory(Guid audioFileId)
        {
            // Create history object
            History history = new History();
            history.AudioFileId = audioFileId;
            history.EventDateTime = DateTime.Now;

            // Insert history
            _gateway.Insert<History>(history, "History");
        }

        /// <summary>
        /// Inserts an history item into the database.
        /// </summary>
        /// <param name="audioFileId">Audio file identifier</param>
        /// <param name="eventDateTime">Event date/time</param>
        public void InsertHistory(Guid audioFileId, DateTime eventDateTime)
        {
            // Create history object
            History history = new History();
            history.AudioFileId = audioFileId;
            history.EventDateTime = eventDateTime;

            // Insert history            
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
