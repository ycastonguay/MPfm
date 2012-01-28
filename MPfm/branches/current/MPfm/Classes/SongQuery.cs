//
// SongQuery.cs: Defines the type of query for the song browser.
//
// Copyright © 2011-2012 Yanick Castonguay
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
using System.Text;

namespace MPfm
{
    /// <summary>
    /// Defines the type of query for the song browser.
    /// </summary>
    public enum SongQueryType
    {
        None = 0, Artist = 1, Album = 2, Playlist = 3, All = 4
    }

    /// <summary>
    /// Query definition for the song browser, including metadata.
    /// </summary>
    public class SongQuery
    {
        /// <summary>
        /// Defines the type of query.
        /// </summary>
        public SongQueryType Type { get; set; }
        
        /// <summary>
        /// Defines the artist name for the query.
        /// </summary>
        public string ArtistName { get; set; }

        /// <summary>
        /// Defines the album title for the query.
        /// </summary>
        public string AlbumTitle { get; set; }

        /// <summary>
        /// Defines the Playlist identifier for the query.
        /// </summary>
        public Guid PlaylistId { get; set; }

        /// <summary>
        /// Default constructor for SongQuery.
        /// Returns all songs from the database.
        /// </summary>
        public SongQuery()
        {
            Type = SongQueryType.All;
        }

        /// <summary>
        /// Constructor for SongQuery that requires the song query type.
        /// </summary>
        /// <param name="type">Song Query Type</param>
        public SongQuery(SongQueryType type)
        {
            Type = type;
        }

        /// <summary>
        /// Constructor for SongQuery that defines the query as Artist
        /// using the artist name passed in parameter.
        /// </summary>
        /// <param name="artistName">Artist Name</param>
        public SongQuery(string artistName)
        {
            Type = SongQueryType.Artist;
            ArtistName = artistName;
        }

        /// <summary>
        /// Constructor for SongQuery that defines the query as Artist/Album
        /// using the artist name and album title passed in parameter.
        /// </summary>
        /// <param name="artistName">Artist Name</param>
        public SongQuery(string artistName, string albumTitle)
        {
            Type = SongQueryType.Album;
            ArtistName = artistName;
            AlbumTitle = albumTitle;
        }

        /// <summary>
        /// Constructor for SongQuery that defines the query as Playlist using
        /// the playlist identifier passed in parameter.
        /// </summary>
        /// <param name="playlistId">Playlist Identifier</param>
        public SongQuery(Guid playlistId)
        {
            Type = SongQueryType.Playlist;
            PlaylistId = playlistId;
        }
    }
}
