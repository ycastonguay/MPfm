//
// EmbeddedFontHelper.cs: Font helper class returning embedded fonts.
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
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// Font helper class returning embedded fonts.
    /// </summary>
    public static class EmbeddedFontHelper
    {
        /// <summary>
        /// Returns the League Gothic embedded font.
        /// </summary>
        /// <returns>Embedded font</returns>
        public static EmbeddedFont GetLeagueGothic()
        {
            // Create font object
            EmbeddedFont font = new EmbeddedFont();
            font.AssemblyPath = "MPfm.Fonts.dll";
            font.Name = "LeagueGothic";
            font.ResourceName = "MPfm.Fonts.LeagueGothic.ttf";            
            return font;
        }

        /// <summary>
        /// Returns the Droid Sans Mono embedded font.
        /// </summary>
        /// <returns>Embedded font</returns>
        public static EmbeddedFont GetDroidSansMono()
        {
            // Create font object
            EmbeddedFont font = new EmbeddedFont();
            font.AssemblyPath = "MPfm.Fonts.dll";
            font.Name = "Droid Sans Mono";
            font.ResourceName = "MPfm.Fonts.DroidSansMono.ttf";
            return font;
        }

        /// <summary>
        /// Returns the Titillium embedded font.
        /// </summary>
        /// <returns>Embedded font</returns>
        public static EmbeddedFont GetTitillium()
        {
            // Create font object
            EmbeddedFont font = new EmbeddedFont();
            font.AssemblyPath = "MPfm.Fonts.dll";
            font.Name = "TitilliumText22L Lt";
            font.ResourceName = "MPfm.Fonts.Titillium2.ttf";
            return font;
        }

        /// <summary>
        /// Returns the Junction embedded font.
        /// </summary>
        /// <returns>Embedded font</returns>
        public static EmbeddedFont GetJunction()
        {
            // Create font object
            EmbeddedFont font = new EmbeddedFont();
            font.AssemblyPath = "MPfm.Fonts.dll";
            font.Name = "Junction";
            font.ResourceName = "MPfm.Fonts.Junction.ttf";
            return font;
        }

        public static EmbeddedFontCollection GetEmbeddedFonts()
        {
            return GetEmbeddedFonts(string.Empty);
        }

        /// <summary>
        /// Returns the list of embedded fonts.
        /// </summary>
        /// <returns>Collection of embedded fonts</returns>
        public static EmbeddedFontCollection GetEmbeddedFonts(string assemblyPath)
        {
            // Declare variables
            if (String.IsNullOrEmpty(assemblyPath))
            {
                assemblyPath = Application.StartupPath + "\\";
            }

            // Create list
            EmbeddedFontCollection collection = new EmbeddedFontCollection();
            collection.Add(GetLeagueGothic(), assemblyPath);
            collection.Add(GetJunction(), assemblyPath);
            collection.Add(GetTitillium(), assemblyPath);
            collection.Add(GetDroidSansMono(), assemblyPath);
            return collection;
        }        
    }
}
