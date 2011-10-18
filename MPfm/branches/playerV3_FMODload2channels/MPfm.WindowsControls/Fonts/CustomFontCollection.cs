//
// CustomFontCollection.cs: This class represents a list of embeddable fonts.
//                          It is based on an ArrayList.
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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This class represents a list of embeddable fonts.
    /// It is based on an ArrayList.
    /// </summary>
    public class CustomFontCollection : ArrayList
    {
        // Private variables
        public PrivateFontCollection fonts = null;

        /// <summary>
        /// Default constructor for CustomFontCollection.
        /// </summary>
        public CustomFontCollection()
        {
            fonts = new PrivateFontCollection();
        }

        /// <summary>
        /// Indexer for the collection. Returns a font.
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Font</returns>
        public CustomFont this[int index]
        {
            get
            {
                return (CustomFont)base[index];
            }
            set
            {
                base[index] = value;
            }
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public void Clear()
        {
            fonts = new PrivateFontCollection();
            base.Clear();
        }

        /// <summary>
        /// Adds a font to the collection.
        /// </summary>
        /// <param name="value">Font</param>
        /// <returns>New index</returns>
        public int Add(CustomFont value)
        {
            AddFontFromResource(value.ResourceName, Application.StartupPath + "\\" + value.AssemblyPath);
            return base.Add(value);
        }

        /// <summary>
        /// Checks if a font is part of the collection.
        /// </summary>
        /// <param name="item">Font</param>
        /// <returns>True if font is found</returns>
        public bool Contains(CustomFont item)
        {
            return base.Contains(item);
        }

        /// <summary>
        /// Returns the index of a font.
        /// </summary>
        /// <param name="value">Font</param>
        /// <returns>Index</returns>
        public int IndexOf(CustomFont value)
        {
            return base.IndexOf(value);
        }

        /// <summary>
        /// Removes a font from the collection.
        /// </summary>
        /// <param name="obj">Font</param>
        public void Remove(CustomFont obj)
        {
            base.Remove(obj);
        }

        /// <summary>
        /// Adds an embedded font from an assembly. The font file and
        /// assembly path must be specified.
        /// </summary>
        /// <param name="resourceName">Resource Name (i.e. MPfm.Fonts.Arial.ttf)</param>
        /// <param name="assemblyPath">Assembly Path (i.e. MPfm.Fonts.dll)</param>
        public void AddFontFromResource(string resourceName, string assemblyPath)
        {
            Assembly assembly = null;

            if (assemblyPath.Length == 0)
            {
                return;
            }

            try
            {
                assembly = Assembly.LoadFile(assemblyPath);
            }
            catch (Exception ex)
            {
                return;
            }

            if (assembly == null)
            {
                return;
            }


            Stream fontStream = assembly.GetManifestResourceStream(resourceName);

            if (fontStream == null)
            {
                //throw new Exception("The font " + resourceName + " could not be found in the calling assembly.");
                return;
            }

            byte[] fontdata = new byte[fontStream.Length];
            fontStream.Read(fontdata, 0, (int)fontStream.Length);
            fontStream.Close();

            unsafe
            {
                fixed (byte* pFontData = fontdata)
                {
                    fonts.AddMemoryFont((System.IntPtr)pFontData, fontdata.Length);
                }
            }

        }

        /// <summary>
        /// Returns the font family from a name.
        /// </summary>
        /// <param name="familyName">Family Name</param>
        /// <returns>Font Family</returns>
        public FontFamily GetFontFamily(string familyName)
        {
            //FontFamily family = null;

            foreach (FontFamily family in fonts.Families)
            {
                if (familyName == family.Name)
                {
                    return family;
                }
            }

            return null;
        }
    }

}
