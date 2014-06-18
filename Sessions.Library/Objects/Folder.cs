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

namespace Sessions.Library.Objects
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

        public Folder()
        {
            FolderId = Guid.NewGuid();
        }

        public Folder(string folderPath, bool isRecursive)
        {
            FolderId = Guid.NewGuid();
            FolderPath = folderPath;
            IsRecursive = isRecursive;
        }
    }
}
