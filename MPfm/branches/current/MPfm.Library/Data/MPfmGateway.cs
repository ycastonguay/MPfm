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

        public List<SongDTO> GetSongs()
        {
            // Fetch data
            DataTable table = Select("SELECT * FROM Songs");

            // Convert to DTO
            List<SongDTO> songs = ConvertDTO.Songs(table);

            return songs;
        }
    }
}
