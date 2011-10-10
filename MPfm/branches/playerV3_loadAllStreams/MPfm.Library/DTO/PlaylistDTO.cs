//
// PlaylistDTO.cs: Data transfer object representing a playlist.
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
    /// Defines a playlist of songs.
    /// </summary>
    public class PlaylistDTO
    {
        public Guid PlaylistId { get; set; }
        public string PlaylistName { get; set; }

        // Metadata
        public List<PlaylistSongDTO> Songs { get; set; }        
        public PlaylistSongDTO CurrentSong { get; set; }
        public PlaylistType PlaylistType { get; set; }
        public int Position { get; set; }
        
        // Flags
        public bool PlaylistModified { get; set; }       

        /// <summary>
        /// Default constructor for PlaylistDTO. Initializes the Songs list and sets
        /// new identifiers.
        /// </summary>
        public PlaylistDTO()
        {
            // Set default values
            PlaylistId = Guid.NewGuid();
            Songs = new List<PlaylistSongDTO>();            
            PlaylistModified = false;
            Position = 0;
        }

        /// <summary>
        /// Gets the next song in the playlist (from the PlaylistSongDTO list in the Songs property).
        /// If the current song is the last in the playlist, the method returns null.
        /// </summary>
        /// <param name="currentPlaylistSongId">Current PlaylistSongId</param>
        /// <returns>Next song (PlaylistSongDTO)</returns>
        public PlaylistSongDTO GetNextSong(Guid currentPlaylistSongId)
        {
            // Iterate through playlist songs
            for (int a = 0; a < Songs.Count; a++)
            {
                // Check if the current song is found
                if (Songs[a].PlaylistSongId == currentPlaylistSongId)
                {
                    // If the current song is the last song in the playlist, return null
                    if (a + 1 == Songs.Count)
                    {
                        return null;
                    }
                    else
                    {
                        // Return the next song
                        return Songs[a + 1];
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the previous song in the playlist (from the PlaylistSongDTO list in the Songs property).
        /// If the current song is the first in the playlist, the method returns null.
        /// </summary>
        /// <param name="currentPlaylistSongId">Current PlaylistSongId</param>
        /// <returns>Previous song (PlaylistSongDTO)</returns>
        public PlaylistSongDTO GetPreviousSong(Guid currentPlaylistSongId)
        {
            // Iterate through playlist songs
            for (int a = 0; a < Songs.Count; a++)
            {
                // Check if the current song is found
                if (Songs[a].PlaylistSongId == currentPlaylistSongId)
                {
                    // If the current song is the first song in the playlist, return null
                    if (a == 0)
                    {
                        return null;
                    }
                    else
                    {
                        // Return the next song
                        return Songs[a - 1];
                    }
                }
            }

            return null;
        }

        public void GetRemainingSongsFromAlbum(ref int firstSongIndex, ref int lastSongIndex)
        {
            // Find the first song to play
            //int firstSongIndex = -1;
            //int lastSongIndex = -1;
            if (firstSongIndex < 0)
            {
                firstSongIndex = 0;
            }

            for (int a = firstSongIndex; a < Songs.Count; a++)
            {
                //// Check if the playlist song id match
                //if (Songs[a].PlaylistSongId == CurrentSong.PlaylistSongId)
                //{
                    //// This is the first song!
                    //firstSongIndex = a;

                    // Check how many songs match the same album title in the batch
                    for (int b = a; b < Songs.Count; b++)
                    {
                        // Does the album title match?
                        if (Songs[b].Song.AlbumTitle != Songs[a].Song.AlbumTitle)
                        {
                            // No match; this means the "streak" stops here!
                            lastSongIndex = b - 1;
                            break;
                        }
                    }

                    // If the next album title hasn't been found yet, it's because there is no other album after this one.
                    if (lastSongIndex == -1)
                    {
                        // Set the last song as the last song of the current album
                        lastSongIndex = Songs.Count - 1;
                    }

                    // We have the first and last song index, get out of the loop!
                    break;
                }
            //}
        }

        public List<string> GetRemainingSongFilePathsFromAlbum()
        {
            int firstSongIndex = -1;
            int lastSongIndex = -1;
            List<string> filePaths = new List<string>();

            GetRemainingSongsFromAlbum(ref firstSongIndex, ref lastSongIndex);

            if (firstSongIndex >= 0 && lastSongIndex >= 0)
            {
                for(int a = firstSongIndex; a <= lastSongIndex; a++)
                {
                    filePaths.Add(Songs[a].Song.FilePath);
                }
            }

            return filePaths;
        }
    }

    /// <summary>
    /// Defines a playlist type.
    /// </summary>
    public enum PlaylistType
    {
        Custom = 0, Artist = 1, Album = 2, All = 3
    }
}
