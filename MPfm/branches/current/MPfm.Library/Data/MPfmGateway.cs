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
        /// Fetches all songs from the database.
        /// </summary>
        /// <returns>List of SongDTO</returns>
        public List<SongDTO> GetSongs()
        {
            // Fetch data
            DataTable table = Select("SELECT * FROM Songs");

            // Convert to DTO
            List<SongDTO> songs = ConvertDTO.Songs(table);

            return songs;
        }

        /// <summary>
        /// Inserts a new song into the database.
        /// </summary>
        /// <param name="song">SongDTO to insert</param>
        public void InsertSong(SongDTO song)
        {
            // Get empty result set
            string baseQuery = "SELECT * FROM Songs";
            DataTable table = Select(baseQuery + " WHERE SongId = ''");

            // Add new row to data table
            DataRow newRow = table.NewRow();
            table.Rows.Add(newRow);
            ConvertDTO.ToRow(ref newRow, song);

            // Insert new row into database
            UpdateDataTable(table, baseQuery);
        }

        /// <summary>
        /// Updates a new song into the database.
        /// </summary>
        /// <param name="song">SongDTO to update</param>
        public void UpdateSong(SongDTO song)
        {
            // Get empty result set
            string baseQuery = "SELECT * FROM Songs";
            DataTable table = Select(baseQuery + " WHERE SongId = '" + song.SongId.ToString() + "'");

            // Check if the row was found
            if (table.Rows.Count == 0)
            {
                throw new Exception("Could not find the song to update (SongId: " + song.SongId.ToString() + ")");
            }

            // Update row in DataTable
            DataRow row = table.Rows[0];
            ConvertDTO.ToRow(ref row, song);

            // Update row into database
            UpdateDataTable(table, baseQuery);
        }

        /// <summary>
        /// Deletes a song from the database.
        /// </summary>
        /// <param name="songId">Song to delete</param>
        public void DeleteSong(Guid songId)
        {
            // Get empty result set
            string baseQuery = "SELECT * FROM Songs";
            DataTable table = Select(baseQuery + " WHERE SongId = '" + songId.ToString() + "'");

            // Check if the row was found
            if (table.Rows.Count == 0)
            {
                throw new Exception("Could not find the song to delete (SongId: " + songId.ToString() + ")");
            }

            // Delete row in DataTable
            table.Rows[0].Delete();

            // Update row into database
            UpdateDataTable(table, baseQuery);
        }
    }
}
