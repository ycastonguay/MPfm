//
// Folder.cs: Class representing a folder containing audio files.
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
using System.Linq;
using System.Text;

namespace MPfm.Library
{
    /// <summary>
    /// Defines a folder containing audio files.
    /// </summary>
    public class Folder
    {
        /// <summary>
        /// Unique identifier for the folder.
        /// </summary>
        public Guid FolderId { get; set; }
        
        /// <summary>
        /// Folder path.
        /// </summary>
        public string FolderPath { get; set; }

        /// <summary>
        /// Nullable DateTime indicating the last time the folder was updated.
        /// </summary>
        public DateTime? LastUpdated { get; set; }

        /// <summary>
        /// Indicates if the library should look into the directory recursively.
        /// </summary>
        public bool IsRecursive { get; set; }

        /// <summary>
        /// Default constructor for the Folder class.
        /// </summary>
        public Folder()
        {
            // Set default values
            FolderId = Guid.NewGuid();
        }
    }
}
