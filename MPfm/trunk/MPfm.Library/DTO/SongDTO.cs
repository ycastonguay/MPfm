//
// SongDTO.cs: Data transfer object representing a song.
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
    /// Defines a song.
    /// </summary>
    public class SongDTO
    {
        public Guid SongId { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public string ArtistName { get; set; }
        public string AlbumTitle { get; set; }
        public long? PlayCount { get; set; }
        public DateTime? LastPlayed { get; set; }
        public string Year { get; set; }
        public long? TrackNumber { get; set; }
        public long? DiscNumber { get; set; }
        public long? TrackCount { get; set; }
        public long? Rating { get; set; }
        public string Time { get; set; }
        public string Genre { get; set; }
        public long? Tempo { get; set; }
        public string Lyrics { get; set; }
        public string SoundFormat { get; set; }
    }
}
