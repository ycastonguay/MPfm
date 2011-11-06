//
// MPfmGateway.cs: MPfm implementation of the Gateway class.
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
    /// The MPfmGateway class implements the SQLiteGateway class.
    /// It acts as a facade to select, insert, update and delete data from the
    /// MPfm database.
    /// </summary>
    public class MPfmGateway : SQLiteGateway
    {        
        /// <summary>
        /// Default constructor for the MPfmGateway class.
        /// </summary>
        /// <param name="databaseFilePath">Database file path</param>
        public MPfmGateway(string databaseFilePath) 
            : base(databaseFilePath)
        {
        }

        /// <summary>
        /// Resets the library.
        /// </summary>
        public void ResetLibrary()
        {
            // Delete all table content
            Delete("Songs");
            Delete("Playlists");
            Delete("PlaylistSongs");
            Delete("Loops");
            Delete("Markers");
        }

        #region Songs
        
        /// <summary>
        /// Selects all songs from the database.
        /// </summary>
        /// <returns>List of SongDTO</returns>
        public List<SongDTO> SelectSongs()
        {
            // Fetch data
            DataTable table = Select("SELECT * FROM Songs");

            // Convert to DTO
            List<SongDTO> songs = ConvertDTO.Songs(table);

            return songs;
        }

        public SongDTO SelectSong(Guid songId)
        {
            // Fetch data
            DataTable table = Select("SELECT * FROM Songs WHERE SongId = '" + songId.ToString() + "'");

            // Convert to DTO
            List<SongDTO> songs = ConvertDTO.Songs(table);

            // Check results
            if (songs.Count > 0)
            {
                // Return first result
                return songs[0];
            }

            return null;
        }

        /// <summary>
        /// Inserts a new song into the database.
        /// </summary>
        /// <param name="song">SongDTO to insert</param>
        public void InsertSong(SongDTO song)
        {
            // Insert song
            Insert("Songs", "SongId", song);
        }

        /// <summary>
        /// Updates a new song into the database.
        /// </summary>
        /// <param name="song">SongDTO to update</param>
        public void UpdateSong(SongDTO song)
        {
            // Update song
            Update("Songs", "SongId", song.SongId, song);
        }

        /// <summary>
        /// Deletes a song from the database.
        /// </summary>
        /// <param name="songId">Song to delete</param>
        public void DeleteSong(Guid songId)
        {
            // Delete song
            Delete("Songs", "SongId", songId);
        }

        public void DeleteSongs(string basePath)
        {
            Delete("Songs", "FilePath LIKE '" + basePath + "%'");
        }

        /// <summary>
        /// Returns the distinct list of artist names from the database.        
        /// </summary>        
        /// <returns>List of distinct artist names</returns>
        public List<string> SelectDistinctArtistNames()
        {
            return SelectDistinctArtistNames(FilterSoundFormat.All);
        }

        /// <summary>
        /// Returns the distinct list of artist names from the database, using the sound format
        /// filter passed in the soundFormat parameter.
        /// </summary>
        /// <param name="soundFormat">Sound Format Filter</param>
        /// <returns>List of distinct artist names</returns>
        public List<string> SelectDistinctArtistNames(FilterSoundFormat soundFormat)
        {
            // Create list
            List<string> artists = new List<string>();

            // Set query
            string sql = "SELECT DISTINCT ArtistName FROM Songs ORDER BY ArtistName";
            if(soundFormat != FilterSoundFormat.All)
            {
                sql = "SELECT DISTINCT ArtistName FROM Songs WHERE SoundFormat = '" + soundFormat.ToString() + "' ORDER BY ArtistName";
            }

            // Select distinct
            DataTable table = Select(sql);

            // Convert into a list of strings
            for (int a = 0; a < table.Rows.Count; a++)
            {
                // Add string to list
                artists.Add(table.Rows[a][0].ToString());
            }

            return artists;
        }

        /// <summary>
        /// Returns the distinct list of album titles per artist from the database.        
        /// </summary>        
        /// <returns>List of distinct album titles per artist</returns>
        public Dictionary<string, List<string>> SelectDistinctAlbumTitles()
        {
            return SelectDistinctAlbumTitles(FilterSoundFormat.All);
        }

        /// <summary>
        /// Returns the distinct list of album titles per artist from the database, using the sound format
        /// filter passed in the soundFormat parameter.
        /// </summary>
        /// <param name="soundFormat">Sound Format Filter</param>
        /// <returns>List of distinct album titles per artist</returns>
        public Dictionary<string, List<string>> SelectDistinctAlbumTitles(FilterSoundFormat soundFormat)
        {
            // Create dictionary
            Dictionary<string, List<string>> albums = new Dictionary<string, List<string>>();

            // Set query
            string sql = "SELECT DISTINCT ArtistName, AlbumTitle FROM Songs";
            if (soundFormat != FilterSoundFormat.All)
            {
                sql = "SELECT DISTINCT ArtistName, AlbumTitle FROM Songs WHERE SoundFormat = '" + soundFormat.ToString() + "' ORDER BY ArtistName";
            }

            // Select distinct
            DataTable table = Select(sql);

            // Convert into a list of strings
            for (int a = 0; a < table.Rows.Count; a++)
            {
                // Get values
                string artistName = table.Rows[a]["ArtistName"].ToString();
                string albumTitle = table.Rows[a]["AlbumTitle"].ToString();

                // Add value to dictionary
                if (albums.ContainsKey(artistName))
                {
                    albums[artistName].Add(albumTitle);
                }
                else
                {
                    albums.Add(artistName, new List<string>() { albumTitle });
                }
            }            

            return albums;
        }

        /// <summary>
        /// Returns the distinct list of album titles with the path of at least one song of the album from the database, 
        /// using the sound format filter passed in the soundFormat parameter. This is useful for displaying album art
        /// for example (no need to return all songs from every album).
        /// </summary>
        /// <param name="soundFormat">Sound Format Filter</param>
        /// <returns>List of distinct album titles with file paths</returns>
        public Dictionary<string, string> SelectDistinctAlbumTitlesWithFilePaths(FilterSoundFormat soundFormat)
        {
            // Create dictionary
            Dictionary<string, string> albums = new Dictionary<string, string>();

            // Set query
            string sql = "SELECT DISTINCT AlbumTitle, FilePath FROM Songs";
            if (soundFormat != FilterSoundFormat.All)
            {
                sql = "SELECT DISTINCT AlbumTitle, FilePath FROM Songs WHERE SoundFormat = '" + soundFormat.ToString() + "' ORDER BY ArtistName";
            }

            // Select distinct
            DataTable table = Select(sql);

            // Convert into a list of strings
            for (int a = 0; a < table.Rows.Count; a++)
            {
                // Get values                
                string albumTitle = table.Rows[a]["AlbumTitle"].ToString();
                string filePath = table.Rows[a]["FilePath"].ToString();

                // Add item to dictionary
                if (!albums.ContainsKey(albumTitle))
                {
                    albums.Add(albumTitle, filePath);
                }
            }

            return albums;
        }

        /// <summary>
        /// Updates the play count of a song and sets the last playback datetime.
        /// </summary>
        /// <param name="songId">SongId</param>
        public void UpdateSongPlayCount(Guid songId)
        {
            // Get play count
            SongDTO song = SelectSong(songId);

            // Check if the song was found
            if (song == null)
            {
                throw new Exception("Error; The song was not found!");
            }

            Execute("UPDATE Songs SET PlayCount = " + (song.PlayCount+1).ToString() + ", LastPlayed = " + DateTime.Now.ToString("yyyy-MM-dd HH:ss.fff"));
        }

        #endregion

        #region Folders

        /// <summary>
        /// Selects a folder from a path.
        /// </summary>
        /// <param name="path">Path to the folder</param>
        /// <returns>Folder</returns>
        public FolderDTO SelectFolderByPath(string path)
        {
            // Fetch data
            DataTable table = Select("SELECT * FROM Folders WHERE FolderPath = '" + path + "'");
            
            // Convert to DTO
            List<FolderDTO> folders = ConvertDTO.Folders(table);

            // Check results
            if (folders.Count > 0)
            {
                // Return first result
                return folders[0];
            }

            return null;
        }

        /// <summary>
        /// Selects a folders.
        /// </summary>        
        /// <returns>List of folders</returns>
        public List<FolderDTO> SelectFolders()
        {
            // Fetch data
            DataTable table = Select("SELECT * FROM Folders");

            // Convert to DTO
            List<FolderDTO> folders = ConvertDTO.Folders(table);

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
            FolderDTO folder = new FolderDTO();
            folder.FolderPath = folderPath;
            folder.IsRecursive = recursive;
            Insert("Folders", "FolderId", folder);
        }

        /// <summary>
        /// Deletes a specific folder.
        /// </summary>
        /// <param name="folderId">FolderId</param>
        public void DeleteFolder(Guid folderId)
        {
            // Delete folder
            Delete("Folders", "FolderId", folderId);
        }

        /// <summary>
        /// Deletes all folders.
        /// </summary>
        public void DeleteFolders()
        {
            // Delete all folders
            Delete("Folders");
        }

        #endregion

        #region Equalizers

        /// <summary>
        /// Select all EQ presets from the database.
        /// </summary>
        /// <returns>List of Presets</returns>
        public List<EqualizerDTO> SelectEqualizers()
        {
            // Fetch data
            DataTable table = Select("SELECT * FROM Equalizers");

            // Convert to DTO
            List<EqualizerDTO> eqs = ConvertDTO.Equalizers(table);

            return eqs;
        }

        /// <summary>
        /// Selects an EQ preset from the database.
        /// </summary>
        /// <param name="name">EQ preset name</param>
        /// <returns>EQ preset</returns>
        public EqualizerDTO SelectEqualizer(string name)
        {
            // Fetch data
            DataTable table = Select("SELECT * FROM Equalizers WHERE Name = '" + name + "'");

            // Convert to DTO
            List<EqualizerDTO> eqs = ConvertDTO.Equalizers(table);

            // Check results
            if (eqs.Count > 0)
            {
                // Return first result
                return eqs[0];
            }

            return null;
        }

        /// <summary>
        /// Inserts a new EQ preset into the database.
        /// </summary>
        /// <param name="eq">EQ preset to insert</param>
        public void InsertEqualizer(EqualizerDTO eq)
        {
            // Insert item
            Insert("Equalizers", "EqualizerId", eq);
        }

        /// <summary>
        /// Updates an existing EQ preset in the database.
        /// </summary>
        /// <param name="eq">EQ preset to update</param>
        public void UpdateEqualizer(EqualizerDTO eq)
        {
            // Update item
            Update("Equalizers", "EqualizerId", eq.EqualizerId, eq);
        }

        /// <summary>
        /// Deletes an EQ preset from the database.
        /// </summary>
        /// <param name="equalizerId">EQ preset identifier</param>
        public void DeleteEqualizer(Guid equalizerId)
        {
            // Delete item
            Delete("Equalizers", "EqualizerId", equalizerId);
        }

        #endregion

        #region Markers

        ///// <summary>
        ///// Selects all markers from the database.
        ///// </summary>
        ///// <returns>List of markers</returns>
        //public static List<Marker> SelectMarkers()
        //{
        //    List<Marker> markers = null;

        //    try
        //    {
        //        // Open the connection
        //        using (MPFM_EF context = new MPFM_EF())
        //        {
        //            markers = context.Markers.ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectMarkers(): " + ex.Message);
        //        throw ex;
        //    }

        //    return markers;
        //}

        ///// <summary>
        ///// Selects markers of a specific song from the database.
        ///// </summary>
        ///// <param name="songId">Song identifier</param>
        ///// <returns>List of markers</returns>
        //public static List<Marker> SelectSongMarkers(Guid songId)
        //{
        //    List<Marker> markers = null;

        //    try
        //    {
        //        // Open the connection
        //        using (MPFM_EF context = new MPFM_EF())
        //        {
        //            string strSongId = songId.ToString();
        //            markers = context.Markers.Where(x => x.SongId == strSongId).OrderBy(x => x.PositionPCM).ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectSongMarkers(): " + ex.Message);
        //        throw ex;
        //    }

        //    return markers;
        //}

        ///// <summary>
        ///// Selects a marker from the database by its identifier.
        ///// </summary>
        ///// <param name="markerId">Marker identifier</param>
        ///// <returns>Marker</returns>
        //public static Marker SelectMarker(Guid markerId)
        //{
        //    Marker marker = null;

        //    try
        //    {
        //        // Open the connection
        //        using (MPFM_EF context = new MPFM_EF())
        //        {
        //            // Convert to a list of strings
        //            string strMarkerId = markerId.ToString();
        //            marker = context.Markers.FirstOrDefault(m => m.MarkerId == strMarkerId);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectMarker(): " + ex.Message);
        //        throw ex;
        //    }

        //    return marker;
        //}

        ///// <summary>
        ///// Inserts a new marker into the database.
        ///// </summary>
        ///// <param name="marker">Marker to insert</param>
        //public static void InsertMarker(Marker marker)
        //{
        //    try
        //    {
        //        // Open the connection
        //        using (MPFM_EF context = new MPFM_EF())
        //        {
        //            // Add to database                    
        //            context.AddToMarkers(marker);
        //            context.SaveChanges();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Tracing.Log("MPfm.Library (DataAccess) --  Error in InsertMarker(): " + ex.Message);
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// Updates an existing marker in the database.
        ///// </summary>
        ///// <param name="marker">Marker to update</param>
        //public static void UpdateMarker(Marker marker)
        //{
        //    try
        //    {
        //        // Open the connection
        //        using (MPFM_EF context = new MPFM_EF())
        //        {
        //            Marker markerToModify = context.Markers.FirstOrDefault(m => m.MarkerId == marker.MarkerId);
        //            if (markerToModify != null)
        //            {
        //                markerToModify.Name = marker.Name;
        //                markerToModify.Position = marker.Position;
        //                markerToModify.SongId = marker.SongId;
        //                markerToModify.PositionPCM = marker.PositionPCM;
        //                markerToModify.PositionPCMBytes = marker.PositionPCMBytes;
        //                markerToModify.Comments = marker.Comments;
        //                context.SaveChanges();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Tracing.Log("MPfm.Library (DataAccess) --  Error in UpdateMarker(): " + ex.Message);
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// Deletes a marker from the database.
        ///// </summary>
        ///// <param name="markerId">Marker identifier</param>
        //public static void DeleteMarker(Guid markerId)
        //{
        //    try
        //    {
        //        // Open the connection
        //        using (MPFM_EF context = new MPFM_EF())
        //        {
        //            ExecuteSql(context, "DELETE FROM Markers WHERE MarkerId = @MarkerId", new SQLiteParameter("MarkerId", markerId.ToString()));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Tracing.Log("MPfm.Library (DataAccess) --  Error in DeleteMarker(): " + ex.Message);
        //        throw ex;
        //    }
        //}

        #endregion

        #region Loops

        ///// <summary>
        ///// Selects all loops from the database.
        ///// </summary>
        ///// <returns>List of loops</returns>
        //public static List<Loop> SelectLoops()
        //{
        //    List<Loop> loops = null;

        //    try
        //    {
        //        // Open the connection
        //        using (MPFM_EF context = new MPFM_EF())
        //        {
        //            loops = context.Loops.ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectLoops(): " + ex.Message);
        //        throw ex;
        //    }

        //    return loops;
        //}

        ///// <summary>
        ///// Selects loops of a specific song from the database.
        ///// </summary>
        ///// <param name="songId">Song identifier</param>
        ///// <returns>List of markers</returns>
        //public static List<Loop> SelectSongLoops(Guid songId)
        //{
        //    List<Loop> loops = null;

        //    try
        //    {
        //        // Open the connection
        //        using (MPFM_EF context = new MPFM_EF())
        //        {
        //            string strSongId = songId.ToString();
        //            loops = context.Loops.Where(m => m.SongId == strSongId).ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectSongLoops(): " + ex.Message);
        //        throw ex;
        //    }

        //    return loops;
        //}

        ///// <summary>
        ///// Selects a loop from the database by its identifier.
        ///// </summary>
        ///// <param name="loopId">Loop identifier</param>
        ///// <returns>Marker</returns>
        //public static Loop SelectLoop(Guid loopId)
        //{
        //    Loop loop = null;

        //    try
        //    {
        //        // Open the connection
        //        using (MPFM_EF context = new MPFM_EF())
        //        {
        //            // Convert to a list of strings
        //            string strLoopId = loopId.ToString();
        //            loop = context.Loops.FirstOrDefault(m => m.LoopId == strLoopId);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectLoop(): " + ex.Message);
        //        throw ex;
        //    }

        //    return loop;
        //}

        ///// <summary>
        ///// Inserts a new loop into the database.
        ///// </summary>
        ///// <param name="loop">Loop to insert</param>
        //public static void InsertLoop(Loop loop)
        //{
        //    try
        //    {
        //        // Open the connection
        //        using (MPFM_EF context = new MPFM_EF())
        //        {
        //            // Add to database                    
        //            context.AddToLoops(loop);
        //            context.SaveChanges();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Tracing.Log("MPfm.Library (DataAccess) --  Error in InsertLoop(): " + ex.Message);
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// Updates an existing loop in the database.
        ///// </summary>
        ///// <param name="loop">Loop to update</param>
        //public static void UpdateLoop(Loop loop)
        //{
        //    try
        //    {
        //        // Open the connection
        //        using (MPFM_EF context = new MPFM_EF())
        //        {
        //            Loop loopToModify = context.Loops.FirstOrDefault(m => m.LoopId == loop.LoopId);
        //            if (loopToModify != null)
        //            {
        //                loopToModify.Name = loop.Name;
        //                loopToModify.MarkerAId = loop.MarkerAId;
        //                loopToModify.MarkerBId = loop.MarkerBId;
        //                loopToModify.SongId = loop.SongId;
        //                loopToModify.Length = loop.Length;
        //                context.SaveChanges();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Tracing.Log("MPfm.Library (DataAccess) --  Error in UpdateLoop(): " + ex.Message);
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// Deletes a loop from the database.
        ///// </summary>
        ///// <param name="loopId">Loop identifier</param>
        //public static void DeleteLoop(Guid loopId)
        //{
        //    try
        //    {
        //        // Open the connection
        //        using (MPFM_EF context = new MPFM_EF())
        //        {
        //            ExecuteSql(context, "DELETE FROM Loops WHERE LoopId = @LoopId", new SQLiteParameter("LoopId", loopId.ToString()));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Tracing.Log("MPfm.Library (DataAccess) --  Error in DeleteLoop(): " + ex.Message);
        //        throw ex;
        //    }
        //}

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

        #region Playlist songs

        //public static List<PlaylistSong> SelectPlaylistSongs(Guid playlistId)
        //{
        //    List<PlaylistSong> playlistSongs = null;

        //    try
        //    {
        //        // Open the connection
        //        using (MPFM_EF context = new MPFM_EF())
        //        {
        //            string strId = playlistId.ToString();
        //            playlistSongs = context.PlaylistSongs.Where(x => x.PlaylistId == strId).OrderBy(x => x.Position).ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectPlaylistSongs(): " + ex.Message);
        //        throw ex;
        //    }

        //    return playlistSongs;
        //}

        ///// <summary>
        ///// Inserts a playlist song into the database.
        ///// </summary>
        ///// <param name="playlistSong">PlaylistSong to insert</param>
        //public static void InsertPlaylistSong(PlaylistSong playlistSong)
        //{
        //    try
        //    {
        //        // Open the connection
        //        using (MPFM_EF context = new MPFM_EF())
        //        {
        //            // Add to database
        //            context.AddToPlaylistSongs(playlistSong);
        //            context.SaveChanges();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Tracing.Log("MPfm.Library (DataAccess) --  Error in InsertPlaylistSong(): " + ex.Message);
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// Inserts a list of playlist songs into the database.
        ///// </summary>
        ///// <param name="playlistSongs">PlaylistSongs to insert</param>
        //public static void InsertPlaylistSongs(List<PlaylistSong> playlistSongs)
        //{
        //    try
        //    {
        //        // Open the connection
        //        using (MPFM_EF context = new MPFM_EF())
        //        {
        //            // Add to database
        //            foreach (PlaylistSong playlistSong in playlistSongs)
        //            {
        //                context.AddToPlaylistSongs(playlistSong);
        //            }
        //            context.SaveChanges();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Tracing.Log("MPfm.Library (DataAccess) --  Error in InsertPlaylistSongs(): " + ex.Message);
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// Deletes the playlist songs of a specific playlist.
        ///// </summary>
        ///// <param name="playlistId">Playlist identifier</param>
        //public static void DeletePlaylistSongs(Guid playlistId)
        //{
        //    try
        //    {
        //        // Open the connection
        //        using (MPFM_EF context = new MPFM_EF())
        //        {
        //            ExecuteSql(context, "DELETE FROM PlaylistSongs WHERE PlaylistId = @PlaylistId", new SQLiteParameter("PlaylistId", playlistId.ToString()));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Tracing.Log("MPfm.Library (DataAccess) --  Error in DeletePlaylistSongs(): " + ex.Message);
        //        throw ex;
        //    }
        //}

        #endregion

    }
}
