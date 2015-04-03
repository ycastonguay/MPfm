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
using System.IO;
using System.Collections.Generic;

namespace Sessions.Sound.Playlists
{
    /// <summary>
    /// Object representing a playlist file.
    /// </summary>
    public class PlaylistFile
    {
        public string FilePath { get; set; }
        public PlaylistFileFormat Format { get; set; }
        public List<string> Items { get; set; }

        /// <summary>
        /// Default constructor for the PlaylistFile class.
        /// </summary>
        public PlaylistFile()
        {
            FilePath = string.Empty;
            Format = PlaylistFileFormat.M3U;
            Items = new List<string>();
        }

        /// <summary>
        /// Constructor for the PlaylistFile class that takes the file path in parameter.
        /// Automatically assigns the playlist file format.
        /// </summary>
        /// <param name="filePath">Playlist file path</param>
        public PlaylistFile(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToUpper().Replace('.', ' ');

            // Determine playlist file format
            PlaylistFileFormat format = PlaylistFileFormat.Unknown;
            Enum.TryParse<PlaylistFileFormat>(extension, out format);
            Format = format;
            FilePath = filePath;
            Items = new List<string>();
        }
    }
}
