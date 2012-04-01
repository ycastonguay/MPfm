//
// TextGradient.cs: The TextGradient class adds font properties to the Gradient class.
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
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.ComponentModel;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// The TextGradient class adds font properties to the Gradient class.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class TextGradient 
        : Gradient
    {
        /// <summary>
        /// Private value for the Font property.
        /// </summary>
        private CustomFont font = new CustomFont("Arial", 8.0f, Color.Black);
        /// <summary>
        /// Defines the font used for text rendering.
        /// </summary>
        [Category("Text"), Browsable(true), Description("Defines the font used for text rendering.")]
        public CustomFont Font
        {
            get
            {
                return font;
            }
            set
            {
                font = value;
            }
        }
       
        /// <summary>
        /// Default constructor for the Gradient class.
        /// </summary>
        public TextGradient() 
            : base()
        {
        }

        /// <summary>
        /// Constructor for the Gradient class.
        /// </summary>
        /// <param name="color1">Gradient first color</param>
        /// <param name="color2">Gradient second color</param>
        /// <param name="mode">Gradient mode</param>
        /// <param name="borderColor">Border color</param>        
        /// <param name="font">Font</param>
        public TextGradient(Color color1, Color color2, LinearGradientMode mode, Color borderColor, CustomFont font)
            : base(color1, color2, mode, borderColor)
        {
            this.font = font;
        }
    }
}
