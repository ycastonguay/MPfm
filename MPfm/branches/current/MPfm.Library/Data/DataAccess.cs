//
// DataAccess.cs: Database access methods
//
// Copyright © 2011 Yanick Castonguay
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
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Linq;
using System.Data.Objects;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using MPfm.Core;
using MPfm.Library.Data;

namespace MPfm.Library
{
    /// <summary>
    /// Accesses the data of MPfm. Static class with static methods using Entity Framework and SQLite.
    /// 
    /// Notes: So far, System.Data.SQLite doesn't like:    
    /// - SingleOrDefault -- replaced with FirstOrDefault.
    /// - compare database varchar to Guid.ToString() -- need to cast guid into string before using value in LINQ
    /// </summary>
    public static class DataAccess
    {
        #region Tools

        public static void ExecuteSql(ObjectContext context, string sql)
        {
            ExecuteSql(context, sql, new List<SQLiteParameter>());
        }

        public static void ExecuteSql(ObjectContext context, string sql, SQLiteParameter parameter)
        {
            ExecuteSql(context, sql, new List<SQLiteParameter>() { parameter });
        }

        public static void ExecuteSql(ObjectContext context, string sql, List<SQLiteParameter> parameters)
        {
            // Get entity connection
            var entityConnection = (System.Data.EntityClient.EntityConnection)context.Connection;
            DbConnection conn = entityConnection.StoreConnection;

            // Reset connection state
            ConnectionState initialState = conn.State;
            try
            {
                // Open connection only if not opened
                if (initialState != ConnectionState.Open)
                {
                    conn.Open();
                }                

                // Create command and execute query
                using (DbCommand cmd = conn.CreateCommand())
                {
                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                    }

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();                    
                }
            }
            finally
            {
                // Close connection if still open
                if (initialState != ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// Compacts the database.
        /// </summary>
        public static void CompactDatabase()
        {
            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    ExecuteSql(context, "VACUUM;");                    
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in CompactDatabase(): " + ex.Message);
                throw ex;
            }
        }

        #endregion

        #region Songs

        /// <summary>
        /// Selects a song from the database by its file path.
        /// </summary>
        /// <param name="filePath">File path of the song</param>
        /// <returns>Returns a Song if found, or null if not found</returns>
        public static Song SelectSong(string filePath)
        {
            Song song = null;

            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    // Select song
                    song = context.Songs.FirstOrDefault(s => s.FilePath == filePath);
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectSong(string filePath): " + ex.Message);
                throw ex;
            }

            return song;
        }

        /// <summary>
        /// Selects a song from the database by its id.
        /// </summary>
        /// <param name="songId">Id of the song</param>
        /// <returns>Returns a Song if found, or null if not found</returns>
        public static Song SelectSong(Guid songId)
        {
            Song song = null;

            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    // Select song
                    string strSongId = songId.ToString();
                    song = context.Songs.FirstOrDefault(x => x.SongId == strSongId);
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectSong(Guid songId): " + ex.Message);
                throw ex;
            }

            return song;
        }

        /// <summary>
        /// Selects all songs in the library.
        /// </summary>
        /// <returns>List of songs</returns>
        public static List<Song> SelectSongs()
        {
            List<Song> songs = null;

            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    // Select all songs
                    songs = context.Songs.ToList();
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectSongs(): " + ex.Message);
                throw ex;
            }

            return songs;
        }

        /// <summary>
        /// Returns all the unique artists of the library.
        /// </summary>
        /// <returns>List of artists (string)</returns>
        public static List<string> SelectArtists()
        {
            List<string> artists = null;

            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    // Query: get all artists from songs
                    var query = from song in context.Songs
                                orderby song.ArtistName 
                                select song.ArtistName;

                    // Get distinct artists
                    query = query.Distinct();

                    // Convert to a list of strings
                    artists = query.ToList<string>();
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectArtists(): " + ex.Message);
                throw ex;
            }

            return artists;
        }

        /// <summary>
        /// Returns all the unique albums of a specific artist in the library.
        /// </summary>
        /// <param name="artistName">Name of the artist</param>
        /// <returns>List of albums (string)</returns>
        public static List<string> SelectArtistAlbums(string artistName)
        {
            List<string> albums = null;

            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    // Query: get all albums from artist from the Songs table
                    var query = from song in context.Songs
                                orderby song.AlbumTitle
                                where song.ArtistName == artistName
                                select song.AlbumTitle;

                    // Get distinct albums
                    query = query.Distinct();

                    // Convert to a list of strings
                    albums = query.ToList<string>();
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectArtistAlbums(): " + ex.Message);
                throw ex;
            }

            return albums;
        }

        /// <summary>
        /// Selects songs based on the values of the method parameters (artist name, album title and song title).
        /// </summary>
        /// <param name="artistName">Name of the artist</param>
        /// <param name="albumTitle">Title of the album</param>
        /// <param name="songTitle">Title of the song</param>
        /// <returns>List of songs</returns>
        public static List<Song> SelectSongs(string artistName, string albumTitle, string songTitle, string orderByFieldName, bool orderByAscending)
        {
            List<Song> songs = null;

            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    ObjectQuery<Song> querySongs = context.Songs;

                    string[] artistNameSplits = artistName.Split(' ');
                    //foreach (string artistNameSplit in artistNameSplits)
                    for (int a = 0; a < artistNameSplits.Length; a++)
                    {
                        querySongs = querySongs.Where("it.ArtistName LIKE @artistName" + a.ToString(), new ObjectParameter("artistName" + a.ToString(), "%" + artistNameSplits[a] + "%"));
                    }


                    //IQueryable<Song> query = context.Songs;

                    //var predicate = PredicateBuilder.False<Song>();

                    //string[] artistNameSplits = artistName.Split(' ');
                    //foreach (string artistNameSplit in artistNameSplits)
                    //{
                    //    // Necessary to avoid the outer variable trap
                    //    string temp = artistNameSplit;
                    //    predicate = predicate.Or(s => s.ArtistName.Contains(artistNameSplit));
                    //}

                    //query = context.Songs.Where(predicate);
                    //query.ToList();
                    //songs = context.Songs.Where(predicate).OrderBy(s => s.ArtistName).ToList();

                    //// Base query: All songs
                    //ObjectQuery<Song> querySongs = context.Songs;

                    //// Check if artistName is null
                    //if (!String.IsNullOrEmpty(artistName))
                    //{
                    //    // Add the artist condition to the query
                    //    //querySongs = querySongs.Where("it.ArtistName = @artistName", new ObjectParameter("artistName", artistName));
                    //}

                    //// Check if albumTitle is null
                    //if (!String.IsNullOrEmpty(albumTitle))
                    //{
                    //    // Add the artist condition to the query
                    //    querySongs = querySongs.Where("it.AlbumTitle = @albumTitle", new ObjectParameter("albumTitle", albumTitle));
                    //}

                    //// Check if songTitle is null
                    //if (!String.IsNullOrEmpty(songTitle))
                    //{
                    //    // Add the artist condition to the query
                    //    querySongs = querySongs.Where("it.Title = @songTitle", new ObjectParameter("songTitle", songTitle));
                    //

                    // Add Order by
                    if (String.IsNullOrEmpty(orderByFieldName))
                    {
                        // Set order by
                        querySongs = querySongs.OrderBy("it.ArtistName, it.AlbumTitle, it.TrackNumber");
                    }
                    else if (orderByAscending)
                    {
                        // Set order by
                        querySongs = querySongs.OrderBy("it." + orderByFieldName);
                    }
                    else if (!orderByAscending)
                    {
                        // Set order by
                        querySongs = querySongs.OrderBy("it." + orderByFieldName + " DESC");
                    }

                    // Execute query
                    songs = querySongs.ToList();
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectSongs(): " + ex.Message);
                throw ex;
            }

            return songs;
        }

        /// <summary>
        /// Inserts a folder in the database. Configuration for library location.
        /// </summary>
        public static void InsertSong(Song song)
        {
            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    // Add to database
                    context.AddToSongs(song);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in InsertSong(): " + ex.Message);
                throw ex;
            }
        }

        public static void UpdateSong(Song song)
        {
            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {                    
                    Song songToModify = context.Songs.FirstOrDefault(x => x.SongId == song.SongId);
                    songToModify.AlbumTitle = song.AlbumTitle;
                    songToModify.ArtistName = song.ArtistName;
                    songToModify.FilePath = song.FilePath;
                    songToModify.Genre = song.Genre;
                    songToModify.LastPlayed = song.LastPlayed;
                    songToModify.Lyrics = song.Lyrics;
                    songToModify.SoundFormat = song.SoundFormat;
                    songToModify.PlayCount = song.PlayCount;
                    songToModify.Rating = song.Rating;
                    songToModify.Tempo = song.Tempo;
                    songToModify.Time = song.Time;
                    songToModify.Title = song.Title;
                    songToModify.DiscNumber = song.DiscNumber;
                    songToModify.TrackCount = song.TrackCount;
                    songToModify.TrackNumber = song.TrackNumber;
                    songToModify.Year = song.Year;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in UpdateSong(): " + ex.Message);
                throw ex;
            }
        }

        public static void DeleteSong(Guid songId)
        {
            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    ExecuteSql(context, "DELETE FROM Songs WHERE SongId = @SongId", new SQLiteParameter("SongId", songId.ToString()));
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in DeleteSong(): " + ex.Message);
                throw ex;
            }
        }

        public static void DeleteSongs(string basePath)
        {
            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    ExecuteSql(context, "DELETE FROM Songs WHERE FilePath LIKE '" + basePath + "%'");
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in DeleteSongs(): " + ex.Message);
                throw ex;
            }
        }

        public static void UpdateSongPlayCount(Guid songId)
        {
            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    // Get song to modify
                    // For some strange reason if Guid is ToString() in the LINQ query it crashes
                    string stringSongId = songId.ToString();
                    Song songToModify = context.Songs.FirstOrDefault(x => x.SongId == stringSongId);

                    // Check if song is valid
                    if (songToModify != null)
                    {
                        // Set last played timestamp
                        songToModify.LastPlayed = DateTime.Now;

                        // Is there a counter?
                        if (songToModify.PlayCount == null)
                        {
                            songToModify.PlayCount = 1;
                        }
                        else
                        {
                            songToModify.PlayCount++;
                        }
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in UpdateSong(): " + ex.Message);
                throw ex;
            }
        }

        #endregion

        #region Songs (Distinct)

        /// <summary>
        /// Returns the distinct list of artist names from the database.        
        /// </summary>        
        /// <returns>List of distinct artist names</returns>
        public static List<string> SelectDistinctArtistNames()
        {
            return SelectDistinctArtistNames(FilterSoundFormat.All);
        }

        /// <summary>
        /// Returns the distinct list of artist names from the database, using the sound format
        /// filter passed in the soundFormat parameter.
        /// </summary>
        /// <param name="soundFormat">Sound Format Filter</param>
        /// <returns>List of distinct artist names</returns>
        public static List<string> SelectDistinctArtistNames(FilterSoundFormat soundFormat)
        {
            List<string> artists = new List<string>();

            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    // Get entity connection
                    var entityConnection = (System.Data.EntityClient.EntityConnection)context.Connection;
                    DbConnection conn = entityConnection.StoreConnection;

                    // Reset connection state
                    ConnectionState initialState = conn.State;
                    try
                    {
                        // Open connection only if not opened
                        if (initialState != ConnectionState.Open)
                        {
                            conn.Open();
                        }

                        // Create command and execute query
                        using (DbCommand cmd = conn.CreateCommand())
                        {
                            if (soundFormat == FilterSoundFormat.All)
                            {
                                cmd.CommandText = "SELECT DISTINCT ArtistName FROM Songs ORDER BY ArtistName";
                            }
                            else
                            {
                                cmd.CommandText = "SELECT DISTINCT ArtistName FROM Songs WHERE SoundFormat = @SoundFormat ORDER BY ArtistName";
                                cmd.Parameters.Add(new SQLiteParameter() { DbType = DbType.String, ParameterName = "SoundFormat", Value = soundFormat.ToString() });
                            }

                            DbDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {
                                artists.Add(dataReader.GetString(0));
                            }
                        }
                    }
                    finally
                    {
                        // Close connection if still open
                        if (initialState != ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (Library) --  Error in SelectDistinctArtists(): " + ex.Message);
                throw ex;
            }

            return artists.Distinct().ToList();
        }

        /// <summary>
        /// Returns the distinct list of album titles per artist from the database.        
        /// </summary>        
        /// <returns>List of distinct album titles per artist</returns>
        public static Dictionary<string, List<string>> SelectDistinctAlbumTitles()
        {
            return SelectDistinctAlbumTitles(FilterSoundFormat.All);
        }

        /// <summary>
        /// Returns the distinct list of album titles per artist from the database, using the sound format
        /// filter passed in the soundFormat parameter.
        /// </summary>
        /// <param name="soundFormat">Sound Format Filter</param>
        /// <returns>List of distinct album titles per artist</returns>
        public static Dictionary<string, List<string>> SelectDistinctAlbumTitles(FilterSoundFormat soundFormat)
        {
            Dictionary<string, List<string>> albums = new Dictionary<string, List<string>>();

            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    // Get entity connection
                    var entityConnection = (System.Data.EntityClient.EntityConnection)context.Connection;
                    DbConnection conn = entityConnection.StoreConnection;

                    // Reset connection state
                    ConnectionState initialState = conn.State;
                    try
                    {
                        // Open connection only if not opened
                        if (initialState != ConnectionState.Open)
                        {
                            conn.Open();
                        }

                        // Create command and execute query
                        using (DbCommand cmd = conn.CreateCommand())
                        {
                            if (soundFormat == FilterSoundFormat.All)
                            {
                                cmd.CommandText = "SELECT DISTINCT ArtistName, AlbumTitle FROM Songs";
                            }
                            else
                            {
                                cmd.CommandText = "SELECT DISTINCT ArtistName, AlbumTitle FROM Songs WHERE SoundFormat = @SoundFormat";
                                cmd.Parameters.Add(new SQLiteParameter() { DbType = DbType.String, ParameterName = "SoundFormat", Value = soundFormat.ToString() });
                            }

                            DbDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {
                                string artistName = dataReader.GetString(0);
                                string albumTitle = dataReader.GetString(1);

                                if (albums.ContainsKey(artistName))
                                {
                                    albums[artistName].Add(albumTitle);
                                }
                                else
                                {
                                    albums.Add(artistName, new List<string>() { albumTitle });
                                }
                            }
                        }
                    }
                    finally
                    {
                        // Close connection if still open
                        if (initialState != ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (Library) --  Error in SelectDistinctAlbums(): " + ex.Message);
                throw ex;
            }

            return albums;
        }

        /// <summary>
        /// Returns the distinct list of album titles with the path of at least one song of the album from the database, 
        /// using the sound format filter passed in the soundFormat parameter. This is useful for displaying album art
        /// for example (no need to return all songs from every album).
        /// </summary>
        /// <param name="soundFormat">Sound Format Filter</param>
        /// <returns>List of distinct album titles with file paths/returns>
        public static Dictionary<string, string> SelectDistinctAlbumTitlesWithFilePaths(FilterSoundFormat soundFormat)
        {
            Dictionary<string, string> albums = new Dictionary<string, string>();

            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    // Get entity connection
                    var entityConnection = (System.Data.EntityClient.EntityConnection)context.Connection;
                    DbConnection conn = entityConnection.StoreConnection;

                    // Reset connection state
                    ConnectionState initialState = conn.State;
                    try
                    {
                        // Open connection only if not opened
                        if (initialState != ConnectionState.Open)
                        {
                            conn.Open();
                        }

                        // Create command and execute query
                        using (DbCommand cmd = conn.CreateCommand())
                        {
                            if (soundFormat == FilterSoundFormat.All)
                            {
                                cmd.CommandText = "SELECT DISTINCT AlbumTitle, FilePath FROM Songs";
                            }
                            else
                            {
                                cmd.CommandText = "SELECT DISTINCT AlbumTitle, FilePath FROM Songs WHERE SoundFormat = @SoundFormat";
                                cmd.Parameters.Add(new SQLiteParameter() { DbType = DbType.String, ParameterName = "SoundFormat", Value = soundFormat.ToString() });
                            }

                            DbDataReader dataReader = cmd.ExecuteReader();

                            while (dataReader.Read())
                            {
                                string albumTitle = dataReader.GetString(0);
                                string filePath = dataReader.GetString(1);

                                if (!albums.ContainsKey(albumTitle))
                                {
                                    albums.Add(albumTitle, filePath);
                                }
                            }
                        }
                    }
                    finally
                    {
                        // Close connection if still open
                        if (initialState != ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (Library) --  Error in SelectDistinctAlbums(): " + ex.Message);
                throw ex;
            }

            return albums;
        }

        #endregion

        #region Folders

        /// <summary>
        /// Selects a folder from a path.
        /// </summary>
        /// <param name="path">Path to the folder</param>
        /// <returns>Folder</returns>
        public static Folder SelectFolderByPath(string path)
        {
            Folder folder = null;

            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    // Query: get all albums from artist from the Songs table
                    var query = from foldersQuery in context.Folders
                                where foldersQuery.FolderPath == path
                                select foldersQuery;

                    // Convert to a list of strings
                    folder = query.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectFolderByPath(): " + ex.Message);
                throw ex;
            }

            return folder;
        }

        /// <summary>
        /// Selects a folders.
        /// </summary>        
        /// <returns>List of folders</returns>
        public static List<Folder> SelectFolders()
        {
            List<Folder> folders = null;

            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    folders = context.Folders.ToList();
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectFolders(): " + ex.Message);
                throw ex;
            }

            return folders;
        }

        /// <summary>
        /// Inserts a folder in the database. Configuration for library location.
        /// </summary>
        /// <param name="folderPath">Path of the folder to add</param>
        /// <param name="recursive">If resursive</param>
        public static void InsertFolder(string folderPath, bool recursive)
        {
            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    // Create folder
                    Folder folder = new Folder();
                    folder.FolderId = Guid.NewGuid().ToString();
                    folder.FolderPath = folderPath;
                    folder.Recursive = recursive;
                    folder.LastUpdate = DateTime.MinValue;

                    // Add to database
                    context.AddToFolders(folder);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in InsertFolder(): " + ex.Message);
                throw ex;
            }
        }

        public static void DeleteFolder(Guid folderId)
        {
            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    ExecuteSql(context, "DELETE FROM Folders WHERE FolderId = @FolderId", new SQLiteParameter("FolderId", folderId.ToString()));
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in DeleteFolders(): " + ex.Message);
                throw ex;
            }
        }

        public static void DeleteFolders()
        {
            try
            {                
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    ExecuteSql(context, "DELETE FROM Folders");
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in DeleteFolders(): " + ex.Message);
                throw ex;
            }
        }

        #endregion

        #region Equalizers

        /// <summary>
        /// Select all EQ presets in the database.
        /// </summary>
        /// <returns>List of Presets</returns>
        public static List<Equalizer> SelectEqualizers()
        {
            List<Equalizer> eqs = null;

            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    eqs = context.Equalizers.ToList();
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectFolders(): " + ex.Message);
                throw ex;
            }

            return eqs;
        }

        public static Equalizer SelectEqualizer(string name)
        {
            Equalizer eq = null;

            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    // Convert to a list of strings
                    eq = context.Equalizers.Where(q => q.Name == name).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectFolderByPath(): " + ex.Message);
                throw ex;
            }

            return eq;
        }

        public static void InsertEqualizer(Equalizer eq)
        {
            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    // Add to database
                    eq.EqualizerId = Guid.NewGuid().ToString();
                    context.AddToEqualizers(eq);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in InsertEqualizer(): " + ex.Message);
                throw ex;
            }
        }

        public static void UpdateEqualizer(Equalizer eq)
        {
            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    Equalizer eqToModify = context.Equalizers.Where(e => e.EqualizerId == eq.EqualizerId).FirstOrDefault();
                    if (eqToModify != null)
                    {
                        eqToModify.Gain55Hz = eq.Gain55Hz;
                        eqToModify.Gain110Hz = eq.Gain110Hz;
                        eqToModify.Gain156Hz = eq.Gain156Hz;
                        eqToModify.Gain220Hz = eq.Gain220Hz;
                        eqToModify.Gain440Hz = eq.Gain440Hz;
                        eqToModify.Gain622Hz = eq.Gain622Hz;
                        eqToModify.Gain880Hz = eq.Gain880Hz;
                        eqToModify.Gain1_2kHz = eq.Gain1_2kHz;
                        eqToModify.Gain1_8kHz = eq.Gain1_8kHz;
                        eqToModify.Gain3_5kHz = eq.Gain3_5kHz;
                        eqToModify.Gain5kHz = eq.Gain5kHz;
                        eqToModify.Gain7kHz = eq.Gain7kHz;
                        eqToModify.Gain10kHz = eq.Gain10kHz;
                        eqToModify.Gain14kHz = eq.Gain14kHz;
                        eqToModify.Gain20kHz = eq.Gain20kHz;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in UpdateEqualizer(): " + ex.Message);
                throw ex;
            }
        }

        public static void DeleteEqualizer(string equalizerId)
        {
            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    ExecuteSql(context, "DELETE FROM Equalizers WHERE EqualizerId = @EqualizerId", new SQLiteParameter("EqualizerId", equalizerId));
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in DeleteEqualizer(): " + ex.Message);
                throw ex;
            }
        }

        #endregion

        #region Playlists

        /// <summary>
        /// Selects all the playlists from the database.
        /// </summary>
        /// <returns>List of playlists</returns>
        public static List<Playlist> SelectPlaylists()
        {
            List<Playlist> playlists = null;

            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    playlists = context.Playlists.OrderBy(x => x.PlaylistName).ToList();
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectPlaylists(): " + ex.Message);
                throw ex;
            }

            return playlists;
        }

        /// <summary>
        /// Selects a playlist from the database, using its identifier.
        /// </summary>
        /// <param name="playlistId">Playlist identifier</param>
        /// <returns>Playlist</returns>
        public static Playlist SelectPlaylist(Guid playlistId)
        {
            Playlist playlist = null;

            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    string strId = playlistId.ToString();
                    playlist = context.Playlists.FirstOrDefault(x => x.PlaylistId == strId);
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectPlaylist(): " + ex.Message);
                throw ex;
            }

            return playlist;
        }

        /// <summary>
        /// Selects a playlist from the database, using its name.
        /// </summary>
        /// <param name="playlistName">Playlist name</param>
        /// <returns>Playlist</returns>
        public static Playlist SelectPlaylist(string playlistName)
        {
            Playlist playlist = null;

            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    playlist = context.Playlists.FirstOrDefault(x => x.PlaylistName.ToUpper() == playlistName.ToUpper());
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectPlaylist(): " + ex.Message);
                throw ex;
            }

            return playlist;
        }

        /// <summary>
        /// Checks if a playlist exists in the database, using its name.
        /// </summary>
        /// <param name="playlistName">Playlist name</param>
        /// <returns>True if the playlist exists</returns>
        public static bool PlaylistExists(string playlistName)
        {
            // Check if playlist exists
            if (SelectPlaylist(playlistName) != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Inserts a playlist into the database.
        /// </summary>
        /// <param name="playlist">Playlist to insert</param>
        public static void InsertPlaylist(Playlist playlist)
        {
            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    // Add to database
                    context.AddToPlaylists(playlist);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in InsertPlaylist(): " + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Updates a playlist in the database.
        /// </summary>
        /// <param name="playlist">Playlist to update</param>
        public static void UpdatePlaylist(Playlist playlist)
        {
            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {                    
                    Playlist playlistToModify = context.Playlists.FirstOrDefault(x => x.PlaylistId == playlist.PlaylistId);
                    if (playlistToModify != null)
                    {
                        playlistToModify.PlaylistName = playlist.PlaylistName;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in UpdatePlaylist(): " + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Deletes a playlist from the database.
        /// </summary>
        /// <param name="playlistId">Playlist identifier to delete</param>
        public static void DeletePlaylist(Guid playlistId)
        {
            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    ExecuteSql(context, "DELETE FROM Playlists WHERE PlaylistId = @PlaylistId", new SQLiteParameter("PlaylistId", playlistId.ToString()));
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in DeletePlaylist(): " + ex.Message);
                throw ex;
            }
        }

        #endregion

        #region Playlist songs
        
        public static List<PlaylistSong> SelectPlaylistSongs(Guid playlistId)
        {
            List<PlaylistSong> playlistSongs = null;

            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    string strId = playlistId.ToString();
                    playlistSongs = context.PlaylistSongs.Where(x => x.PlaylistId == strId).OrderBy(x => x.Position).ToList();
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectPlaylistSongs(): " + ex.Message);
                throw ex;
            }

            return playlistSongs;
        }

        /// <summary>
        /// Inserts a playlist song into the database.
        /// </summary>
        /// <param name="playlistSong">PlaylistSong to insert</param>
        public static void InsertPlaylistSong(PlaylistSong playlistSong)
        {
            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    // Add to database
                    context.AddToPlaylistSongs(playlistSong);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in InsertPlaylistSong(): " + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Inserts a list of playlist songs into the database.
        /// </summary>
        /// <param name="playlistSongs">PlaylistSongs to insert</param>
        public static void InsertPlaylistSongs(List<PlaylistSong> playlistSongs)
        {
            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    // Add to database
                    foreach (PlaylistSong playlistSong in playlistSongs)
                    {
                        context.AddToPlaylistSongs(playlistSong);
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in InsertPlaylistSongs(): " + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Deletes the playlist songs of a specific playlist.
        /// </summary>
        /// <param name="playlistId">Playlist identifier</param>
        public static void DeletePlaylistSongs(Guid playlistId)
        {
            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    ExecuteSql(context, "DELETE FROM PlaylistSongs WHERE PlaylistId = @PlaylistId", new SQLiteParameter("PlaylistId", playlistId.ToString()));
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in DeletePlaylistSongs(): " + ex.Message);
                throw ex;
            }
        }

        #endregion

        public static void ResetLibrary()
        {
            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    ExecuteSql(context, "DELETE FROM Songs");
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in ResetLibrary(): " + ex.Message);
                throw ex;
            }
        }
    }
}
