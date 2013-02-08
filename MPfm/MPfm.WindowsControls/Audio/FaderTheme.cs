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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// Theme object for the Fader control.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class FaderTheme
    {
        /// <summary>
        /// Private value for the IsBackgroundTransparent property.
        /// </summary>
        private bool isBackgroundTransparent = true;
        /// <summary>
        /// Defines if the background is visible or not.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Background"), Browsable(true), Description("Defines if the background is visible or not.")]
        public bool IsBackgroundTransparent
        {
            get
            {
                return isBackgroundTransparent;
            }
            set
            {
                isBackgroundTransparent = value;
            }
        }

        /// <summary>
        /// Private value for the BackgroundGradient property.
        /// </summary>
        private BackgroundGradient backgroundGradient = new BackgroundGradient(Color.DarkGray, Color.Gray, LinearGradientMode.Vertical, Color.Gray, 0);
        /// <summary>
        /// Defines the fader background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Fader background gradient.")]
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
        /// Private value for the FaderGradient property.
        /// </summary>
        private BackgroundGradient faderGradient = new BackgroundGradient(Color.LightGray, Color.Gray, LinearGradientMode.Vertical, Color.Gray, 0);
        /// <summary>
        /// Defines the fader gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Fader gradient.")]
        public BackgroundGradient FaderGradient
        {
            get
            {
                return faderGradient;
            }
            set
            {
                faderGradient = value;
            }
        }

        /// <summary>
        /// Private value for the FaderShadowGradient property.
        /// </summary>
        private BackgroundGradient faderShadowGradient = new BackgroundGradient(Color.DarkGray, Color.DarkGray, LinearGradientMode.Vertical, Color.Gray, 0);
        /// <summary>
        /// Defines the fader shadow gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Fader shadow gradient.")]
        public BackgroundGradient FaderShadowGradient
        {
            get
            {
                return faderShadowGradient;
            }
            set
            {
                faderShadowGradient = value;
            }
        }

        /// <summary>
        /// Private value for the CenterLineColor property.
        /// </summary>
        private Color centerLineColor = Color.Black;
        /// <summary>
        /// Color used when drawing the center line.
        /// </summary>
        [XmlIgnore]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Color used when drawing the center line.")]
        public Color CenterLineColor
        {
            get
            {
                return centerLineColor;
            }
            set
            {
                centerLineColor = value;
            }
        }
        /// <summary>
        /// Gets/sets the center line color using a 32-bit integer (ARGB).
        /// This is used for serializing the Color structure in XML.
        /// </summary>
        [Browsable(false)]
        [XmlElement(ElementName = "CenterLineColor")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CenterLineColorInt
        {
            get
            {
                return centerLineColor.ToArgb();
            }
            set
            {
                centerLineColor = Color.FromArgb(value);
            }
        }

        /// <summary>
        /// Private value for the CenterLineShadowColor property.
        /// </summary>
        private Color centerLineShadowColor = Color.DarkGray;
        /// <summary>
        /// Color used when drawing the center line shadow.
        /// </summary>
        [XmlIgnore]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Color used when drawing the center line shadow.")]
        public Color CenterLineShadowColor
        {
            get
            {
                return centerLineShadowColor;
            }
            set
            {
                centerLineShadowColor = value;
            }
        }
        /// <summary>
        /// Gets/sets the center line shadow color using a 32-bit integer (ARGB).
        /// This is used for serializing the Color structure in XML.
        /// </summary>
        [Browsable(false)]
        [XmlElement(ElementName = "CenterLineShadowColor")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CenterLineShadowColorInt
        {
            get
            {
                return centerLineShadowColor.ToArgb();
            }
            set
            {
                centerLineShadowColor = Color.FromArgb(value);
            }
        }

        /// <summary>
        /// Private value for the FaderMiddleLineColor property.
        /// </summary>
        private Color faderMiddleLineColor = Color.Black;
        /// <summary>
        /// Color of the line in the center of the fader.
        /// </summary>
        [XmlIgnore]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Color of the line in the center of the fader.")]
        public Color FaderMiddleLineColor
        {
            get
            {
                return faderMiddleLineColor;
            }
            set
            {
                faderMiddleLineColor = value;
            }
        }
        /// <summary>
        /// Gets/sets the fader middle line color using a 32-bit integer (ARGB).
        /// This is used for serializing the Color structure in XML.
        /// </summary>
        [Browsable(false)]
        [XmlElement(ElementName = "FaderMiddleLineColor")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int FaderMiddleLineColorInt
        {
            get
            {
                return faderMiddleLineColor.ToArgb();
            }
            set
            {
                faderMiddleLineColor = Color.FromArgb(value);
            }
        }

        /// <summary>
        /// Private value for the FaderShadowColor property.
        /// </summary>
        private Color faderShadowColor = Color.DarkGray;
        /// <summary>
        /// Color of the fader shadow.
        /// </summary>
        [XmlIgnore]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Color of the fader shadow.")]
        public Color FaderShadowColor
        {
            get
            {
                return faderShadowColor;
            }
            set
            {
                faderShadowColor = value;
            }
        }
        /// <summary>
        /// Gets/sets the fader shadow color using a 32-bit integer (ARGB).
        /// This is used for serializing the Color structure in XML.
        /// </summary>
        [Browsable(false)]
        [XmlElement(ElementName = "FaderShadowColor")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int FaderShadowColorInt
        {
            get
            {
                return faderShadowColor.ToArgb();
            }
            set
            {
                faderShadowColor = Color.FromArgb(value);
            }
        }

        /// <summary>
        /// Default constructor for the FaderTheme class.
        /// </summary>
        public FaderTheme()
        {
        }
    }
}
