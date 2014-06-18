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

using Sessions.Sound.AudioFiles;

namespace Sessions.Library.Objects
{
	/// <summary>
	/// Data structure representing a library query.
	/// </summary>
	public class LibraryQuery
	{
		public AudioFileFormat Format { get; set; }
		public string OrderBy { get; set; }
		public bool OrderByAscending { get; set; }
		public string ArtistName { get; set ; }
		public string AlbumTitle { get; set ; }
		public string SearchTerms { get; set ; }
		
		public LibraryQuery()
		{
			Format = AudioFileFormat.All;
			OrderByAscending = true;
		}
	}
}

