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

        //#region Equalizers

        ///// <summary>
        ///// Select all EQ presets from the database.
        ///// </summary>
        ///// <returns>List of Presets</returns>
        //public static List<Equalizer> SelectEqualizers()
        //{
        //    List<Equalizer> eqs = null;

        //    try
        //    {
        //        // Open the connection
        //        using (MPFM_EF context = new MPFM_EF())
        //        {
        //            eqs = context.Equalizers.ToList();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectFolders(): " + ex.Message);
        //        throw ex;
        //    }

        //    return eqs;
        //}

        ///// <summary>
        ///// Selects an EQ preset from the database.
        ///// </summary>
        ///// <param name="name">EQ preset name</param>
        ///// <returns>EQ preset</returns>
        //public static Equalizer SelectEqualizer(string name)
        //{
        //    Equalizer eq = null;

        //    try
        //    {
        //        // Open the connection
        //        using (MPFM_EF context = new MPFM_EF())
        //        {
        //            // Convert to a list of strings
        //            eq = context.Equalizers.FirstOrDefault(q => q.Name == name);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectFolderByPath(): " + ex.Message);
        //        throw ex;
        //    }

        //    return eq;
        //}

        ///// <summary>
        ///// Inserts a new EQ preset into the database.
        ///// </summary>
        ///// <param name="eq">EQ preset to insert</param>
        //public static void InsertEqualizer(Equalizer eq)
        //{
        //    try
        //    {
        //        // Open the connection
        //        using (MPFM_EF context = new MPFM_EF())
        //        {
        //            // Add to database
        //            eq.EqualizerId = Guid.NewGuid().ToString();
        //            context.AddToEqualizers(eq);
        //            context.SaveChanges();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Tracing.Log("MPfm.Library (DataAccess) --  Error in InsertEqualizer(): " + ex.Message);
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// Updates an existing EQ preset in the database.
        ///// </summary>
        ///// <param name="eq">EQ preset to update</param>
        //public static void UpdateEqualizer(Equalizer eq)
        //{
        //    try
        //    {
        //        // Open the connection
        //        using (MPFM_EF context = new MPFM_EF())
        //        {
        //            Equalizer eqToModify = context.Equalizers.FirstOrDefault(e => e.EqualizerId == eq.EqualizerId);
        //            if (eqToModify != null)
        //            {
        //                eqToModify.Gain55Hz = eq.Gain55Hz;
        //                eqToModify.Gain110Hz = eq.Gain110Hz;
        //                eqToModify.Gain156Hz = eq.Gain156Hz;
        //                eqToModify.Gain220Hz = eq.Gain220Hz;
        //                eqToModify.Gain440Hz = eq.Gain440Hz;
        //                eqToModify.Gain622Hz = eq.Gain622Hz;
        //                eqToModify.Gain880Hz = eq.Gain880Hz;
        //                eqToModify.Gain1_2kHz = eq.Gain1_2kHz;
        //                eqToModify.Gain1_8kHz = eq.Gain1_8kHz;
        //                eqToModify.Gain3_5kHz = eq.Gain3_5kHz;
        //                eqToModify.Gain5kHz = eq.Gain5kHz;
        //                eqToModify.Gain7kHz = eq.Gain7kHz;
        //                eqToModify.Gain10kHz = eq.Gain10kHz;
        //                eqToModify.Gain14kHz = eq.Gain14kHz;
        //                eqToModify.Gain20kHz = eq.Gain20kHz;
        //                context.SaveChanges();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Tracing.Log("MPfm.Library (DataAccess) --  Error in UpdateEqualizer(): " + ex.Message);
        //        throw ex;
        //    }
        //}

        ///// <summary>
        ///// Deletes an EQ preset from the database.
        ///// </summary>
        ///// <param name="equalizerId">EQ preset identifier</param>
        //public static void DeleteEqualizer(Guid equalizerId)
        //{
        //    try
        //    {
        //        // Open the connection
        //        using (MPFM_EF context = new MPFM_EF())
        //        {
        //            ExecuteSql(context, "DELETE FROM Equalizers WHERE EqualizerId = @EqualizerId", new SQLiteParameter("EqualizerId", equalizerId.ToString()));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Tracing.Log("MPfm.Library (DataAccess) --  Error in DeleteEqualizer(): " + ex.Message);
        //        throw ex;
        //    }
        //}

        //#endregion

        #region Markers

        /// <summary>
        /// Selects all markers from the database.
        /// </summary>
        /// <returns>List of markers</returns>
        public static List<Marker> SelectMarkers()
        {
            List<Marker> markers = null;

            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    markers = context.Markers.ToList();
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectMarkers(): " + ex.Message);
                throw ex;
            }

            return markers;
        }

        /// <summary>
        /// Selects markers of a specific song from the database.
        /// </summary>
        /// <param name="songId">Song identifier</param>
        /// <returns>List of markers</returns>
        public static List<Marker> SelectSongMarkers(Guid songId)
        {
            List<Marker> markers = null;

            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    string strSongId = songId.ToString();
                    markers = context.Markers.Where(x => x.SongId == strSongId).OrderBy(x => x.PositionPCM).ToList();
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectSongMarkers(): " + ex.Message);
                throw ex;
            }

            return markers;
        }

        /// <summary>
        /// Selects a marker from the database by its identifier.
        /// </summary>
        /// <param name="markerId">Marker identifier</param>
        /// <returns>Marker</returns>
        public static Marker SelectMarker(Guid markerId)
        {
            Marker marker = null;

            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    // Convert to a list of strings
                    string strMarkerId = markerId.ToString();
                    marker = context.Markers.FirstOrDefault(m => m.MarkerId == strMarkerId);
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectMarker(): " + ex.Message);
                throw ex;
            }

            return marker;
        }

        /// <summary>
        /// Inserts a new marker into the database.
        /// </summary>
        /// <param name="marker">Marker to insert</param>
        public static void InsertMarker(Marker marker)
        {
            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    // Add to database                    
                    context.AddToMarkers(marker);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in InsertMarker(): " + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Updates an existing marker in the database.
        /// </summary>
        /// <param name="marker">Marker to update</param>
        public static void UpdateMarker(Marker marker)
        {
            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    Marker markerToModify = context.Markers.FirstOrDefault(m => m.MarkerId == marker.MarkerId);
                    if (markerToModify != null)
                    {
                        markerToModify.Name = marker.Name;
                        markerToModify.Position = marker.Position;
                        markerToModify.SongId = marker.SongId;
                        markerToModify.PositionPCM = marker.PositionPCM;
                        markerToModify.PositionPCMBytes = marker.PositionPCMBytes;
                        markerToModify.Comments = marker.Comments;                        
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in UpdateMarker(): " + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Deletes a marker from the database.
        /// </summary>
        /// <param name="markerId">Marker identifier</param>
        public static void DeleteMarker(Guid markerId)
        {
            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    ExecuteSql(context, "DELETE FROM Markers WHERE MarkerId = @MarkerId", new SQLiteParameter("MarkerId", markerId.ToString()));
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in DeleteMarker(): " + ex.Message);
                throw ex;
            }
        }

        #endregion

        #region Loops

        /// <summary>
        /// Selects all loops from the database.
        /// </summary>
        /// <returns>List of loops</returns>
        public static List<Loop> SelectLoops()
        {
            List<Loop> loops = null;

            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    loops = context.Loops.ToList();
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectLoops(): " + ex.Message);
                throw ex;
            }

            return loops;
        }

        /// <summary>
        /// Selects loops of a specific song from the database.
        /// </summary>
        /// <param name="songId">Song identifier</param>
        /// <returns>List of markers</returns>
        public static List<Loop> SelectSongLoops(Guid songId)
        {
            List<Loop> loops = null;

            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    string strSongId = songId.ToString();
                    loops = context.Loops.Where(m => m.SongId == strSongId).ToList();
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectSongLoops(): " + ex.Message);
                throw ex;
            }

            return loops;
        }

        /// <summary>
        /// Selects a loop from the database by its identifier.
        /// </summary>
        /// <param name="loopId">Loop identifier</param>
        /// <returns>Marker</returns>
        public static Loop SelectLoop(Guid loopId)
        {
            Loop loop = null;

            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    // Convert to a list of strings
                    string strLoopId = loopId.ToString();
                    loop = context.Loops.FirstOrDefault(m => m.LoopId == strLoopId);
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in SelectLoop(): " + ex.Message);
                throw ex;
            }

            return loop;
        }

        /// <summary>
        /// Inserts a new loop into the database.
        /// </summary>
        /// <param name="loop">Loop to insert</param>
        public static void InsertLoop(Loop loop)
        {
            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    // Add to database                    
                    context.AddToLoops(loop);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in InsertLoop(): " + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Updates an existing loop in the database.
        /// </summary>
        /// <param name="loop">Loop to update</param>
        public static void UpdateLoop(Loop loop)
        {
            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    Loop loopToModify = context.Loops.FirstOrDefault(m => m.LoopId == loop.LoopId);
                    if (loopToModify != null)
                    {
                        loopToModify.Name = loop.Name;
                        loopToModify.MarkerAId = loop.MarkerAId;
                        loopToModify.MarkerBId = loop.MarkerBId;
                        loopToModify.SongId = loop.SongId;
                        loopToModify.Length = loop.Length;
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in UpdateLoop(): " + ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Deletes a loop from the database.
        /// </summary>
        /// <param name="loopId">Loop identifier</param>
        public static void DeleteLoop(Guid loopId)
        {
            try
            {
                // Open the connection
                using (MPFM_EF context = new MPFM_EF())
                {
                    ExecuteSql(context, "DELETE FROM Loops WHERE LoopId = @LoopId", new SQLiteParameter("LoopId", loopId.ToString()));
                }
            }
            catch (Exception ex)
            {
                Tracing.Log("MPfm.Library (DataAccess) --  Error in DeleteLoop(): " + ex.Message);
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
