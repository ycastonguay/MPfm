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

using System.Collections.Generic;
using Sessions.Library.Objects;

namespace Sessions.Library.UpdateLibrary
{
    /// <summary>
    /// Arguments for the background worker that updates the library.
    /// </summary>
    public class UpdateLibraryArgument
    {
        public List<string> FilePaths { get; set; }
        public List<Folder> FolderPaths { get; set; }

        public UpdateLibraryArgument()
        {
            FilePaths = new List<string>();
            FolderPaths = new List<Folder>();
        }
    }
}
