//
// WaveFormDisplayTheme.cs: Theme object for the WaveFormDisplay control.
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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// Theme object for the WaveFormDisplay control.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class WaveFormDisplayTheme
    {
        /// <summary>
        /// Private value for the BackgroundGradient property.
        /// </summary>
        private BackgroundGradient backgroundGradient = new BackgroundGradient(Color.FromArgb(20, 20, 20), Color.FromArgb(50, 50, 50), LinearGradientMode.Vertical, Color.Gray, 0);
        /// <summary>
        /// Defines the output meter background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Output meter background gradient.")]
        public BackgroundGradient BackgroundGradient
        {
            get
            {
                return backgroundGradient;
            }
            set
            {
                backgroundGradient = value;
            }
        }

        /// <summary>
        /// Private value for the CurrentPositionTextGradient property.
        /// </summary>
        private TextGradient currentPositionTextGradient = new TextGradient(Color.RoyalBlue, Color.DarkBlue, LinearGradientMode.Vertical, Color.DarkGray, 0, new CustomFont("Droid Sans Mono", 8.0f, Color.White));
        /// <summary>
        /// Defines the current position text gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Current position text gradient.")]
        public TextGradient CurrentPositionTextGradient
        {
            get
            {
                return currentPositionTextGradient;
            }
            set
            {
                currentPositionTextGradient = value;
            }
        }

        /// <summary>
        /// Private value for the WaveFormColor property.
        /// </summary>
        private Color waveFormColor = Color.Yellow;
        /// <summary>
        /// Defines the color used when drawing the wave form.
        /// </summary>
        [XmlIgnore]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Color used when drawing the wave form.")]
        public Color WaveFormColor
        {
            get
            {
                return waveFormColor;
            }
            set
            {
                waveFormColor = value;
            }
        }
        /// <summary>
        /// Gets/sets the wave form color using a 32-bit integer (ARGB).
        /// This is used for serializing the Color structure in XML.
        /// </summary>
        [Browsable(false)]
        [XmlElement(ElementName = "WaveFormColor")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int WaveFormColorInt
        {
            get
            {
                return waveFormColor.ToArgb();
            }
            set
            {
                waveFormColor = Color.FromArgb(value);
            }
        }

        /// <summary>
        /// Private value for the CursorColor property.
        /// </summary>
        private Color cursorColor = Color.DarkBlue;
        /// <summary>
        /// Defines the color used when drawing the cursor over the wave form.
        /// </summary>
        [XmlIgnore]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Color used when drawing the cursor over the wave form.")]
        public Color CursorColor
        {
            get
            {
                return cursorColor;
            }
            set
            {
                cursorColor = value;
            }
        }
        /// <summary>
        /// Gets/sets the cursor color using a 32-bit integer (ARGB).
        /// This is used for serializing the Color structure in XML.
        /// </summary>
        [Browsable(false)]
        [XmlElement(ElementName = "CursorColor")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CursorColorInt
        {
            get
            {
                return cursorColor.ToArgb();
            }
            set
            {
                cursorColor = Color.FromArgb(value);
            }
        }

        /// <summary>
        /// Default constructor for the WaveFormDisplayTheme class.
        /// </summary>
        public WaveFormDisplayTheme()
        {            
        }
    }
}
