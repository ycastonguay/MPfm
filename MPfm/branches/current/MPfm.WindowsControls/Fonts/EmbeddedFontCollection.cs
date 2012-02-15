//
// EmbeddedFontCollection.cs: This is a list of embeddable fonts.
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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Design;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This is a list of embeddable fonts.
    /// </summary>
    public class EmbeddedFontCollection : ArrayList
    {
        // Private variables
        private PrivateFontCollection m_fonts = null;

        /// <summary>
        /// Default constructor for EmbeddedFontCollection.
        /// </summary>
        public EmbeddedFontCollection()
        {
            m_fonts = new PrivateFontCollection();
        }

        /// <summary>
        /// Indexer for the collection. Returns a font.
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Font</returns>
        public new EmbeddedFont this[int index]
        {
            get
            {
                return (EmbeddedFont)base[index];
            }
            set
            {
                base[index] = value;
            }
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public override void Clear()
        {
            m_fonts = new PrivateFontCollection();
            base.Clear();
        }

        /// <summary>
        /// Adds a font to the collection.
        /// </summary>
        /// <param name="value">Font</param>
        /// <returns>New index</returns>
        public int Add(EmbeddedFont value)
        {
            // Declare variables
            string resourcePath = string.Empty;

            // Check if design time or run time
            if (Tools.IsDesignTime())
            {
                // Set path as current project path
                //ITypeResolutionService typeResService = GetService(typeof(ITypeResolutionService)) as ITypeResolutionService;
                //string path = typeResService.GetPathOfAssembly(Assembly.GetExecutingAssembly().GetName());
            }
            else
            {
                // This is runtime; the file is in the same directory as the executable
                resourcePath = Application.StartupPath + "\\" + value.AssemblyPath;

                // Add font from resource file
                AddFontFromResource(value.ResourceName, resourcePath);
            }

            return base.Add(value);
        }

        /// <summary>
        /// Checks if a font is part of the collection.
        /// </summary>
        /// <param name="item">Font</param>
        /// <returns>True if font is found</returns>
        public bool Contains(EmbeddedFont item)
        {
            return base.Contains(item);
        }

        /// <summary>
        /// Returns the index of a font.
        /// </summary>
        /// <param name="value">Font</param>
        /// <returns>Index</returns>
        public int IndexOf(EmbeddedFont value)
        {
            return base.IndexOf(value);
        }

        /// <summary>
        /// Removes a font from the collection.
        /// </summary>
        /// <param name="obj">Font</param>
        public void Remove(EmbeddedFont obj)
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
            // Declare variables
            Assembly assembly = null;

            // Check file path
            if (!File.Exists(assemblyPath))
            {
                throw new ArgumentException("The assemblyPath parameter does not contain a valid file path.");
            }

            // Try to load font from assembly
            try
            {
                assembly = Assembly.LoadFile(assemblyPath);
            }
            catch(Exception ex)
            {
                throw new Exception("The assembly was not found!", ex);
            }

            // Check if assembly was found
            if (assembly == null)
            {
                throw new Exception("The assembly was not found!");
            }

            // Get manifest
            Stream fontStream = assembly.GetManifestResourceStream(resourceName);

            if (fontStream == null)
            {
                //throw new Exception("The font " + resourceName + " could not be found in the calling assembly.");
                return;
            }

            // Read font from file
            byte[] fontdata = new byte[fontStream.Length];
            fontStream.Read(fontdata, 0, (int)fontStream.Length);
            fontStream.Close();

            // Add font to memory (requires unsafe code)
            unsafe
            {
                fixed (byte* pFontData = fontdata)
                {
                    m_fonts.AddMemoryFont((System.IntPtr)pFontData, fontdata.Length);
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
            foreach (FontFamily family in m_fonts.Families)
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
