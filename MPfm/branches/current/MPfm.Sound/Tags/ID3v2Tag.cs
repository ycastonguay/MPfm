//
// ID3v2Tag.cs: Data structure for ID3v2 tags.
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
    /// Data structure for ID3v2 tags.
    /// </summary>
    public class ID3v2Tag
    {
        /// <summary>
        /// Private value for the TagSize property.
        /// </summary>
        private int m_tagSize = 0;
        /// <summary>
        /// Defines the ID3v2 tag size.
        /// </summary>
        public int TagSize
        {
            get
            {
                return m_tagSize;
            }            
            set
            {
                m_tagSize = value;
            }
        }

        /// <summary>
        /// Private value for the Version property.
        /// </summary>
        private int m_version = 0;
        /// <summary>
        /// Defines the ID3v2 tag version (0 = ID3v2.0, 1 = ID3v2.1, etc.)
        /// Unknown if the ID3v2 tag was not found.
        /// </summary>
        public int Version
        {
            get
            {
                return m_version;
            }
            set
            {
                m_version = value;
            }
        }

        /// <summary>
        /// Private value for the TagFound property.
        /// </summary>
        private bool m_tagFound = false;
        /// <summary>
        /// Indicates if the ID3v2 tags have been found.
        /// </summary>
        public bool TagFound
        {
            get
            {
                return m_tagFound;
            }
            set
            {
                m_tagFound = value;
            }
        }

        /// <summary>
        /// Private value for the ExtendedHeader property.
        /// </summary>
        private bool m_extendedHeader = false;
        /// <summary>
        /// Indicates if the file contains an extended header.
        /// </summary>
        public bool ExtendedHeader
        {
            get
            {
                return m_extendedHeader;
            }
            set
            {
                m_extendedHeader = value;
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
