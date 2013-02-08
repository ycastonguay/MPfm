//
// Gradient.cs: The Gradient class groups different properties related to gradients into one object.
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
    /// The Gradient class groups different properties related to gradients into one object.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]    
    public class Gradient
    {
        /// <summary>
        /// Private value for the Color1 property.
        /// </summary>
        private Color color1 = Color.FromArgb(0, 0, 0);
        /// <summary>
        /// Defines the first color of the gradient.
        /// </summary>
        [XmlIgnore]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Gradient"), Browsable(true), Description("Defines the first color of the gradient.")]
        public Color Color1
        {
            get
            {
                return color1;
            }
            set
            {
                color1 = value;
            }
        }
        /// <summary>
        /// Gets/sets the first color of the gradient using a 32-bit integer (ARGB).
        /// This is used for serializing the Color structure in XML.
        /// </summary>
        [Browsable(false)]
        [XmlElement(ElementName = "Color1")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Color1Int
        {
            get
            {
                return color1.ToArgb();
            }
            set
            {
                color1 = Color.FromArgb(value);
            }
        }

        /// <summary>
        /// Private value for the Color2 property.
        /// </summary>
        private Color color2 = Color.FromArgb(0, 0, 0);
        /// <summary>
        /// Defines the second color of the gradient.
        /// </summary>
        [XmlIgnore]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Gradient"), Browsable(true), Description("Defines the second color of the gradient.")]
        public Color Color2
        {
            get
            {
                return color2;
            }
            set
            {
                color2 = value;
            }
        }
        /// <summary>
        /// Gets/sets the second color of the gradient using a 32-bit integer (ARGB).
        /// This is used for serializing the Color structure in XML.
        /// </summary>
        [Browsable(false)]
        [XmlElement(ElementName = "Color2")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Color2Int
        {
            get
            {
                return color2.ToArgb();
            }
            set
            {
                color2 = Color.FromArgb(value);
            }
        }

        /// <summary>
        /// Private value for the GradientMode property.
        /// </summary>
        private LinearGradientMode gradientMode = LinearGradientMode.Vertical;
        /// <summary>
        /// Defines the gradient mode.
        /// </summary>
        [Category("Gradient"), Browsable(true), Description("Defines the gradient mode.")]
        public LinearGradientMode GradientMode
        {
            get
            {
                return gradientMode;
            }
            set
            {
                gradientMode = value;
            }
        }

        /// <summary>
        /// Default constructor for the Gradient class.
        /// </summary>
        public Gradient()
        {
        }

        /// <summary>
        /// Constructor for the Gradient class.
        /// </summary>
        /// <param name="color1">Gradient first color</param>
        /// <param name="color2">Gradient second color</param>
        /// <param name="mode">Gradient mode</param>
        public Gradient(Color color1, Color color2, LinearGradientMode mode)
        {
            this.color1 = color1;
            this.color2 = color2;
            this.gradientMode = mode;            
        }
    }
}
