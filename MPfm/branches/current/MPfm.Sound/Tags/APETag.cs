//
// APETag.cs: Data structure for APEv1 and APEv2 tags.
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
using System.IO;
using System.Text;
using System.Reflection;

namespace MPfm.Sound
{  
    /// <summary>
    /// Data structure for APEv1 and APEv2 tags.
    /// </summary>
    public class APETag
    {
        /// <summary>
        /// Private value for the TagSize property.
        /// </summary>
        private int m_tagSize = 0;
        /// <summary>
        /// Defines the APE tag size (including the header if APEv2).
        /// This value excludes the APEv1/APEv2 footer size.
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
        /// Private value for the Dictionary property.
        /// </summary>
        private Dictionary<string, string> m_dictionary = null;
        /// <summary>
        /// List of key/values contained in the APE tag.
        /// </summary>
        public Dictionary<string, string> Dictionary
        {        
            get
            {
                return m_dictionary;
            }
        }

        /// <summary>
        /// Private value for the Version property.
        /// </summary>
        private APETagVersion m_version = APETagVersion.Unknown;
        /// <summary>
        /// Defines the APE tag version (APEv1 or APEv2).
        /// Unknown if the APE tag was not found.
        /// </summary>
        public APETagVersion Version
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
        /// Default constructor for the APETag class.
        /// </summary>
        public APETag()
        {
            // Create dictionary
            m_dictionary = new Dictionary<string, string>();
        }
    }

    /// <summary>
    /// Defines the APE tag version.
    /// </summary>
    public enum APETagVersion
    {
        /// <summary>
        /// The APE tag wasn't found or is in an unknown version.
        /// </summary>
        Unknown = 0, 
        /// <summary>
        /// APE tag version 1.
        /// </summary>
        APEv1 = 1, 
        /// <summary>
        /// APE tag version 2.
        /// </summary>
        APEv2 = 2
    }
}
