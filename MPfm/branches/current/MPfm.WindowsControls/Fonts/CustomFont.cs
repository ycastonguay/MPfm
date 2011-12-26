//
// CustomFont.cs: This object represents a custom font (standard or embedded font).
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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Text;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This object represents a custom font (standard or embedded font).
    /// </summary>
    [Editor(typeof(CustomFontEditor), typeof(UITypeEditor))]
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
        /// Private value for the FontName property.
        /// </summary>
        private string m_fontName = null;
        /// <summary>
        /// Defines the standard font face name.
        /// </summary>
        public string FontName
        {
            get
            {
                return m_fontName;
            }
            set
            {
                m_fontName = value;
            }
        }

        /// <summary>
        /// Private value for the FontSize property.
        /// </summary>
        private int m_fontSize = 8;
        /// <summary>
        /// Defines the font size (in points).
        /// </summary>
        public int FontSize
        {
            get
            {
                return m_fontSize;
            }
            set
            {
                m_fontSize = value;
            }
        }

        /// <summary>
        /// Private value for the EmbeddedFont property.
        /// </summary>
        private EmbeddedFont m_embeddedFont = null;
        /// <summary>
        /// Defines an embedded font.
        /// </summary>
        public EmbeddedFont EmbeddedFont
        {
            get
            {
                return m_embeddedFont;
            }
            set
            {
                m_embeddedFont = value;
            }            
        }

        /// <summary>
        /// Default constructor for CustomFont.
        /// </summary>
        public CustomFont()
        {
        }
    }
}
