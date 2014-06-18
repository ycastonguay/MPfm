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

namespace Sessions.Sound.AudioFiles
{
    /// <summary>
    /// Defines an audio file format and its file extensions.
    /// </summary>
    public class AudioFileFormatExtension
    {
        /// <summary>
        /// Audio file format.
        /// </summary>
        public readonly AudioFileFormat Format = AudioFileFormat.Unknown;

        /// <summary>
        /// List of extensions, including the dot character (ex: .FLAC).
        /// </summary>
        public readonly List<string> Extensions = null;

        /// <summary>
        /// Default constructor for the AudioFileFormatExtension class.
        /// </summary>
        /// <param name="format">Audio file format</param>
        /// <param name="extensions">List of extensions (including the dot character)</param>
        public AudioFileFormatExtension(AudioFileFormat format, List<string> extensions)
        {
            // Set values
            Format = format;
            Extensions = extensions;
        }
    }
}

