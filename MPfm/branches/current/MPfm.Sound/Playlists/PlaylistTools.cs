//
// PlaylistTools.cs: This static class contains useful methods for loading and 
//                   saving playlists.
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
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace MPfm.Sound
{
    /// <summary>
    /// This static class contains useful methods for loading and saving playlists.
    /// </summary>
    public static class PlaylistTools
    {     
        public static List<string> LoadPlaylist(string filePath)
        {
            // Declare variables
            List<string> files = new List<string>();
            PlaylistFileFormat format = PlaylistFileFormat.Unknown;

            // Determine the playlist format
            if (filePath.ToUpper().Contains(".M3U"))
            {
                // Set format
                format = PlaylistFileFormat.M3U;

                // Load playlist files
                files = LoadM3UPlaylist(filePath);
            }
            else if (filePath.ToUpper().Contains(".M3U8"))
            {
                // Set format
                format = PlaylistFileFormat.M3U8;

                // Load playlist files
                files = LoadM3UPlaylist(filePath);
            }
            else if (filePath.ToUpper().Contains(".PLS"))
            {
                // Set format
                format = PlaylistFileFormat.PLS;
            }
            else if (filePath.ToUpper().Contains(".XSPF"))
            {
                // Set format
                format = PlaylistFileFormat.XSPF;
            }
            else if (filePath.ToUpper().Contains(".ASX"))
            {
                // Set format
                format = PlaylistFileFormat.ASX;
            }

            return files;
        }

        /// <summary>
        /// Returns the list of audio files to play from a M3U playlist.
        /// </summary>
        /// <param name="filePath">Playlist file path</param>
        /// <returns>List of audio file paths</returns>
        public static List<string> LoadM3UPlaylist(string filePath)
        {
            // Declare variables
            List<string> files = new List<string>();

            // Open reader
            StreamReader reader = new StreamReader(filePath);

            // Loop through lines
            string line = string.Empty;
            while ((line = reader.ReadLine()) != null)
            {
                // Make sure the line isn't empty
                if (!String.IsNullOrEmpty(line))
                {
                    // Make sure the line isn't a comment (begins by the pound # sign)
                    if (line[0] != '#')
                    {
                        // Make sure the file exists
                        if (File.Exists(line))
                        {
                            // Add file to list
                            files.Add(line);
                        }
                        else if (Directory.Exists(line))
                        {
                            // The line is a directory! We gotta search recursively through this directory.
                            List<string> moreFiles = AudioTools.SearchAudioFilesRecursive(line, "MP3;FLAC;OGG;WAV;WV;APE");

                            // Check if the list is empty
                            if (moreFiles != null || moreFiles.Count == 0)
                            {
                                // Add files to list
                                files.AddRange(moreFiles);
                            }
                        }
                    }
                }                
            }

            // Close reader
            reader.Dispose();

            return files;
        }
    }
}
