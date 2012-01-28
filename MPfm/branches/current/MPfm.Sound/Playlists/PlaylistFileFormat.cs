//
// PlaylistFileFormat.cs: Defines the supported playlist file formats for MPfm.
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
using System.Linq;
using System.Text;

namespace MPfm.Sound
{
	/// <summary>
    /// Defines the supported playlist file formats for MPfm.
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
        ASX = 6
	}
}

