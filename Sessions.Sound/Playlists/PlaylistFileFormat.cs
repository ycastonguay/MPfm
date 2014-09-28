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

namespace Sessions.Sound.Playlists
{
	/// <summary>
    /// Defines the supported playlist file formats for Sessions.
	/// </summary>
	public enum PlaylistFileFormat
	{
        /// <summary>
        /// Every playlist file format (useful for library queries).
        /// </summary>
		All = 0, 
        /// <summary>
        /// Unknown playlist file format.
        /// </summary>
        Unknown = 1,
        /// <summary>
        /// M3U playlist file format.
        /// For more information, consult http://en.wikipedia.org/wiki/M3U.
        /// </summary>
        M3U = 2,
        /// <summary>
        /// M3U8 playlist file format (UTF-8).
        /// For more information, consult http://en.wikipedia.org/wiki/M3U.
        /// </summary>
        M3U8 = 3,
        /// <summary>
        /// PLS playlist file format.
        /// For more information, consult http://en.wikipedia.org/wiki/PLS_(file_format).
        /// </summary>
        PLS = 4,
        /// <summary>
        /// XSPF playlist file format.
        /// For more information, consult http://en.wikipedia.org/wiki/XSPF.
        /// </summary>
        XSPF = 5,
        /// <summary>
        /// ASX playlist file format.
        /// For more information, consult http://en.wikipedia.org/wiki/Advanced_Stream_Redirector.
        /// </summary>
        ASX = 6,
        /// <summary>
        /// CUE playlist file format. 
        /// For more information, consult http://en.wikipedia.org/wiki/Cue_sheet_(computing).
        /// </summary>
        CUE = 7
	}
}

