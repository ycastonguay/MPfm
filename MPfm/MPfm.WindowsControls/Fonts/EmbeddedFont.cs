// Copyright © 2011-2013 Yanick Castonguay
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
using System.Collections;
using System.ComponentModel;
using System.Text;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This object represents an embeddable font.
    /// </summary>    
    public class EmbeddedFont
    {
        /// <summary>
        /// Private value for the Name property.
        /// </summary>
        private string name = "";
        /// <summary>
        /// Font name.
        /// Example: LeagueGothic
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        /// <summary>
        /// Private value for the ResourceName property.
        /// </summary>
        private string resourceName = "";
        /// <summary>
        /// Assembly resource namespace.
        /// Example: MPfm.Fonts.LeagueGothic.ttf
        /// </summary>
        public string ResourceName
        {
            get
            {
                return resourceName;
            }
            set
            {
                resourceName = value;
            }
        }

        /// <summary>
        /// Private value for the AssemblyPath property.
        /// </summary>
        private string assemblyPath = "";
        /// <summary>
        /// Assembly path containing the embedded font. 
        /// Example: MPfm.Fonts.dll
        /// </summary>
        public string AssemblyPath
        {
            get
            {
                return assemblyPath;
            }
            set
            {
                assemblyPath = value;
            }
        }

        /// <summary>
        /// Default constructor for EmbeddedFont.
        /// </summary>
        public EmbeddedFont()
        {
        }
    }
}