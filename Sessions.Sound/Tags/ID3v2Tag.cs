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

namespace Sessions.Sound.Tags
{  
    /// <summary>
    /// Data structure for ID3v2 tags.
    /// </summary>
    public class ID3v2Tag
    {
        /// <summary>
        /// Private value for the TagSize property.
        /// </summary>
        private int tagSize = 0;
        /// <summary>
        /// Defines the ID3v2 tag size.
        /// </summary>
        public int TagSize
        {
            get
            {
                return tagSize;
            }            
            set
            {
                tagSize = value;
            }
        }

        /// <summary>
        /// Private value for the Version property.
        /// </summary>
        private int version = 0;
        /// <summary>
        /// Defines the ID3v2 tag version (0 = ID3v2.0, 1 = ID3v2.1, etc.)
        /// Unknown if the ID3v2 tag was not found.
        /// </summary>
        public int Version
        {
            get
            {
                return version;
            }
            set
            {
                version = value;
            }
        }

        /// <summary>
        /// Private value for the TagFound property.
        /// </summary>
        private bool tagFound = false;
        /// <summary>
        /// Indicates if the ID3v2 tags have been found.
        /// </summary>
        public bool TagFound
        {
            get
            {
                return tagFound;
            }
            set
            {
                tagFound = value;
            }
        }

        /// <summary>
        /// Private value for the ExtendedHeader property.
        /// </summary>
        private bool extendedHeader = false;
        /// <summary>
        /// Indicates if the file contains an extended header.
        /// </summary>
        public bool ExtendedHeader
        {
            get
            {
                return extendedHeader;
            }
            set
            {
                extendedHeader = value;
            }
        }

        /// <summary>
        /// Default constructor for the ID3v2Tag class.
        /// </summary>
        public ID3v2Tag()
        {
        }
    }
}
