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

using System.Collections.Generic;

namespace MPfm.Library.UpdateLibrary
{
    /// <summary>
    /// Arguments for the background worker that updates the library.
    /// </summary>
    public class UpdateLibraryArgument
    {
        /// <summary>
        /// Update library mode.
        /// </summary>
        public UpdateLibraryMode Mode { get; set; }
        /// <summary>
        /// List of files to update (necessary for the SpecificFiles update library mode).
        /// </summary>
        public List<string> FilePaths { get; set; }
        /// <summary>
        /// Folder path to update (necessary for the SpecificFolder update library mode).
        /// </summary>
        public string FolderPath { get; set; }

        /// <summary>
        /// Constructor for UpdateLibraryArguments.
        /// </summary>
        public UpdateLibraryArgument()
        {
            // Set default arguments
            Mode = UpdateLibraryMode.WholeLibrary;
            FilePaths = new List<string>();
            FolderPath = string.Empty;
        }
    }
}
