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
using System;
using Sessions.Library.Objects;
using Sessions.Sound.AudioFiles;

namespace Sessions.MVP.Models
{
    /// <summary>
    /// Data structure repesenting a library browser item.
    /// </summary>
	public class LibraryBrowserEntity
	{
        public Guid Id { get; set; }
		public LibraryBrowserEntityType EntityType { get; set; }
		public string Title { get; set; }
        public string Subtitle { get; set; }
        public bool IsQueued { get; set; }
		public LibraryQuery Query { get; set; }
		public List<LibraryBrowserEntity> SubItems { get; set; }
        public List<string> AlbumTitles { get; set; }
        public AudioFile AudioFile { get; set; }
		
		public LibraryBrowserEntity()
		{
            Id = Guid.NewGuid();
            Query = new LibraryQuery();
			SubItems = new List<LibraryBrowserEntity>();
		    AlbumTitles = new List<string>();
		}
	}
	
	/// <summary>
	/// Library browser entity type.
	/// </summary>
	public enum LibraryBrowserEntityType
	{
		AllSongs = 0, Artists = 1, Albums = 2, Artist = 3, ArtistAlbum = 4, Album = 5, Song = 6, Dummy = 100
	}
}

