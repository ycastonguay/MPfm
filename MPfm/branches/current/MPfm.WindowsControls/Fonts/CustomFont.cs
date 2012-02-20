//
// CustomFont.cs: This object represents a custom font (standard or embedded font).
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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Text;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This object represents a custom font (standard or embedded font).
    /// </summary>
    [Editor(typeof(CustomFontUIEditor), typeof(UITypeEditor))]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class CustomFont
    {
        /// <summary>
        /// Private value for the IsBold property.
        /// </summary>
        private bool m_isBold = false;
        /// <summary>
        /// Defines if the font uses bold.
        /// </summary>
        public bool IsBold
        {
            get
            {                
                return m_isBold;
            }
            set
            {
                m_isBold = value;
            }
        }

        /// <summary>
        /// Private value for the IsItalic property.
        /// </summary>
        private bool m_isItalic = false;
        /// <summary>
        /// Defines if the font uses italic.
        /// </summary>
        public bool IsItalic
        {
            get
            {
                return m_isItalic;
            }
            set
            {
                m_isItalic = value;
            }
        }

        /// <summary>
        /// Private value for the IsUnderline property.
        /// </summary>
        private bool m_isUnderline = false;
        /// <summary>
        /// Defines if the font uses underline.
        /// </summary>
        public bool IsUnderline
        {
            get
            {
                return m_isUnderline;
            }
            set
            {
                m_isUnderline = value;
            }
        }

        /// <summary>
        /// Private value for the Size property.
        /// </summary>
        private float m_size = 8;
        /// <summary>
        /// Defines the font size (in points).
        /// </summary>
        public float Size
        {
            get
            {
                return m_size;
            }
            set
            {
                m_size = value;
            }
        }

        /// <summary>
        /// Private value for the StandardFontName property.
        /// </summary>
        private string m_standardFontName = "Arial";
        /// <summary>
        /// Defines the standard font face name.
        /// </summary>
        public string StandardFontName
        {
            get
            {
                return m_standardFontName;
            }
            set
            {
                m_standardFontName = value;
            }
        }

        /// <summary>
        /// Private value for the EmbeddedFontName property.
        /// </summary>
        private string m_embeddedFontName = string.Empty;
        /// <summary>
        /// Defines the embedded font name.
        /// </summary>
        public string EmbeddedFontName
        {
            get
            {
                return m_embeddedFontName;
            }
            set
            {
                m_embeddedFontName = value;
            }
        }

        /// <summary>
        /// Private value for the UseEmbeddedFont property.
        /// </summary>
        private bool m_useEmbeddedFont = false;
        /// <summary>
        /// Defines if the font should use the embedded font.
        /// </summary>
        public bool UseEmbeddedFont
        {
            get
            {
                return m_useEmbeddedFont;
            }
            set
            {
                m_useEmbeddedFont = value;
            }
        }

        /// <summary>
        /// Private value for the UseAntiAliasing property.
        /// </summary>
        private bool m_useAntiAliasing = true;
        /// <summary>
        /// Defines if the font should use anti-aliasing.
        /// </summary>
        public bool UseAntiAliasing
        {
            get
            {
                return m_useAntiAliasing;
            }
            set
            {
                m_useAntiAliasing = value;
            }
        }

        /// <summary>
        /// Default constructor for CustomFont.
        /// </summary>
        public CustomFont()
        {
        }

        /// <summary>
        /// Constructor for the CustomFont class. Requires the embedded font name and font size.
        /// Will automatically set UseEmbeddedFont to true.
        /// </summary>
        /// <param name="embeddedFontName">Embedded font name</param>
        /// <param name="size">Font size</param>
        public CustomFont(string embeddedFontName, float size)
        {
            m_useEmbeddedFont = true;
            m_embeddedFontName = embeddedFontName;
            m_size = size;
        }

        /// <summary>
        /// Returns a FontStyle structure with the IsBold, IsItalic and IsUnderline property values.
        /// </summary>
        /// <returns>FontStyle</returns>
        public FontStyle ToFontStyle()
        {
            // Set default value
            FontStyle style = FontStyle.Regular;            

            // Check for styles
            if (IsBold)
            {
                // Add bold
                style |= FontStyle.Bold;
            }
            if (IsItalic)
            {
                // Add bold
                style |= FontStyle.Italic;
            }
            if (IsUnderline)
            {
                // Add bold
                style |= FontStyle.Underline;
            }

            return style;
        }
    }
}
