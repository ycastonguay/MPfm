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
using System.Drawing;
using System.Drawing.Design;
using System.Text;
using System.Xml.Serialization;

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
        private bool isBold = false;
        /// <summary>
        /// Defines if the font uses bold.
        /// </summary>
        public bool IsBold
        {
            get
            {                
                return isBold;
            }
            set
            {
                isBold = value;
            }
        }

        /// <summary>
        /// Private value for the IsItalic property.
        /// </summary>
        private bool isItalic = false;
        /// <summary>
        /// Defines if the font uses italic.
        /// </summary>
        public bool IsItalic
        {
            get
            {
                return isItalic;
            }
            set
            {
                isItalic = value;
            }
        }

        /// <summary>
        /// Private value for the IsUnderline property.
        /// </summary>
        private bool isUnderline = false;
        /// <summary>
        /// Defines if the font uses underline.
        /// </summary>
        public bool IsUnderline
        {
            get
            {
                return isUnderline;
            }
            set
            {
                isUnderline = value;
            }
        }

        /// <summary>
        /// Private value for the Size property.
        /// </summary>
        private float size = 8;
        /// <summary>
        /// Defines the font size (in points).
        /// </summary>
        public float Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
            }
        }

        /// <summary>
        /// Private value for the StandardFontName property.
        /// </summary>
        private string standardFontName = "Arial";
        /// <summary>
        /// Defines the standard font face name.
        /// </summary>
        public string StandardFontName
        {
            get
            {
                return standardFontName;
            }
            set
            {
                standardFontName = value;
            }
        }

        /// <summary>
        /// Private value for the EmbeddedFontName property.
        /// </summary>
        private string embeddedFontName = string.Empty;
        /// <summary>
        /// Defines the embedded font name.
        /// </summary>
        public string EmbeddedFontName
        {
            get
            {
                return embeddedFontName;
            }
            set
            {
                embeddedFontName = value;
            }
        }

        /// <summary>
        /// Private value for the UseEmbeddedFont property.
        /// </summary>
        private bool useEmbeddedFont = false;
        /// <summary>
        /// Defines if the font should use the embedded font.
        /// </summary>
        public bool UseEmbeddedFont
        {
            get
            {
                return useEmbeddedFont;
            }
            set
            {
                useEmbeddedFont = value;
            }
        }

        /// <summary>
        /// Private value for the UseAntiAliasing property.
        /// </summary>
        private bool useAntiAliasing = true;
        /// <summary>
        /// Defines if the font should use anti-aliasing.
        /// </summary>
        public bool UseAntiAliasing
        {
            get
            {
                return useAntiAliasing;
            }
            set
            {
                useAntiAliasing = value;
            }
        }

        /// <summary>
        /// Private value for the Color property.
        /// </summary>
        private Color color = Color.FromArgb(0, 0, 0);
        /// <summary>
        /// Defines the font color.
        /// </summary>
        [XmlIgnore]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Gradient"), Browsable(true), Description("Defines the font color.")]
        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }
        /// <summary>
        /// Gets/sets the font color using a 32-bit integer (ARGB).
        /// This is used for serializing the Color structure in XML.
        /// </summary>
        [Browsable(false)]
        [XmlElement(ElementName = "Color")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ColorInt
        {
            get
            {
                return color.ToArgb();
            }
            set
            {
                color = Color.FromArgb(value);
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
        /// <param name="color">Font color</param>
        public CustomFont(string embeddedFontName, float size, Color color)
        {
            this.useEmbeddedFont = true;
            this.embeddedFontName = embeddedFontName;
            this.size = size;
            this.color = color;
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
