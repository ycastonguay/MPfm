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
    /// The BackgroundGradient class adds border properties to the Gradient class.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]    
    public class BackgroundGradient 
        : Gradient
    {
        /// <summary>
        /// Private value for the BorderColor property.
        /// </summary>
        private Color borderColor = Color.FromArgb(20, 20, 20);
        /// <summary>
        /// Defines the border color.
        /// </summary>
        [XmlIgnore]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Border"), Browsable(true), Description("Defines the border color.")]
        public Color BorderColor
        {
            get
            {
                return borderColor;
            }
            set
            {
                borderColor = value;
            }
        }
        /// <summary>
        /// Gets/sets the border color using a 32-bit integer (ARGB).
        /// This is used for serializing the Color structure in XML.
        /// </summary>
        [Browsable(false)]
        [XmlElement(ElementName = "BorderColor")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int BorderColorInt
        {
            get
            {
                return borderColor.ToArgb();
            }
            set
            {
                borderColor = Color.FromArgb(value);
            }
        }

        /// <summary>
        /// Private value for the BorderWidth property.
        /// </summary>
        private int borderWidth = 1;
        /// <summary>
        /// Defines the border width (in pixels). To hide the border, enter 0.
        /// </summary>
        [Category("Border"), Browsable(true), Description("Defines the border width (in pixels). To hide the border, enter 0.")]
        public int BorderWidth
        {
            get
            {
                return borderWidth;
            }
            set
            {
                borderWidth = value;
            }
        }

        /// <summary>
        /// Default constructor for the BackgroundGradient class.
        /// </summary>
        public BackgroundGradient() 
            : base ()
        {
        }

        /// <summary>
        /// Constructor for the BackgroundGradient class.
        /// </summary>
        /// <param name="color1">Gradient first color</param>
        /// <param name="color2">Gradient second color</param>
        /// <param name="mode">Gradient mode</param>
        /// <param name="borderColor">Border color</param>
        /// <param name="borderWidth">Border width (enter 0 to hide the border)</param>
        public BackgroundGradient(Color color1, Color color2, LinearGradientMode mode, Color borderColor, int borderWidth) 
            : base(color1, color2, mode)
        {
            this.borderColor = borderColor;
            this.borderWidth = borderWidth;
        }
    }
}
