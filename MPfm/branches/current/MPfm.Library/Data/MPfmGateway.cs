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
    }
}
