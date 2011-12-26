//
// FontCollection.cs: This control contains the list of embeddable fonts.
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Text;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This control contains the list of embeddable fonts.
    /// </summary>
    public class FontCollection : Component
    {        
        /// <summary>
        /// Default constructor for FontCollection.
        /// </summary>
        public FontCollection()
        {
            //fonts = new PrivateFontCollection();            
        }

        private EmbeddedFontCollection m_fontCollection = new EmbeddedFontCollection();
        /// <summary>
        /// List of embedded fonts.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Configuration"), Browsable(true), Description("Font collection to display custom fonts")]
        public EmbeddedFontCollection Fonts
        {
            get
            {
                return m_fontCollection;
            }
            set
            {
                m_fontCollection = value;
            }
        }

        /// <summary>
        /// Returns the FontFamily based on the name passed in parameter.
        /// </summary>
        /// <param name="familyName">Family Name</param>
        /// <returns>FontFamily</returns>
        public FontFamily GetFontFamily(string familyName)
        {
            return m_fontCollection.GetFontFamily(familyName);
        }
    }
}
