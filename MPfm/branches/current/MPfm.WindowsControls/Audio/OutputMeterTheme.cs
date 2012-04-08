//
// OutputMeterTheme.cs: Theme object for the OutputMeter control.
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
    /// Theme object for the OutputMeter control.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class OutputMeterTheme
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
        /// Private value for the CustomFont property.
        /// </summary>
        private CustomFont customFont = new CustomFont("Junction", 8.0f, Color.White);
        /// <summary>
        /// Defines the font to be used for rendering the control.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Font used for rendering the control.")]
        public CustomFont CustomFont
        {
            get
            {
                return customFont;
            }
            set
            {
                customFont = value;
            }
        }

        /// <summary>
        /// Private value for the FontShadowColor property.
        /// </summary>
        private Color fontShadowColor = Color.Gray;
        /// <summary>
        /// Fore font color used when displaying the volume peak in decibels (drop shadow under text).
        /// </summary>
        [XmlIgnore]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Fonts"), Browsable(true), Description("Fore font color used when displaying the volume peak in decibels (drop shadow under text).")]
        public Color FontShadowColor
        {
            get
            {
                return fontShadowColor;
            }
            set
            {
                fontShadowColor = value;
            }
        }
        /// <summary>
        /// Gets/sets the font shadow color using a 32-bit integer (ARGB).
        /// This is used for serializing the Color structure in XML.
        /// </summary>
        [Browsable(false)]
        [XmlElement(ElementName = "FontShadowColor")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int FontShadowColorInt
        {
            get
            {
                return fontShadowColor.ToArgb();
            }
            set
            {
                fontShadowColor = Color.FromArgb(value);
            }
        }

        /// <summary>
        /// Private value for the MeterGradient property.
        /// </summary>
        private BackgroundGradient meterGradient = new BackgroundGradient(Color.Green, Color.LightGreen, LinearGradientMode.Vertical, Color.Gray, 0);
        /// <summary>
        /// Defines the output meter gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Output meter gradient.")]
        public BackgroundGradient MeterGradient
        {
            get
            {
                return meterGradient;
            }
            set
            {
                meterGradient = value;
            }
        }

        /// <summary>
        /// Private value for the MeterDistortionGradient property.
        /// </summary>
        private BackgroundGradient meterDistortionGradient = new BackgroundGradient(Color.DarkRed, Color.Red, LinearGradientMode.Vertical, Color.Gray, 0);
        /// <summary>
        /// Defines the output meter distortion gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Output meter distortion gradient.")]
        public BackgroundGradient MeterDistortionGradient
        {
            get
            {
                return meterDistortionGradient;
            }
            set
            {
                meterDistortionGradient = value;
            }
        }

        private Color meter0dbLineColor = Color.DarkGray;
        /// <summary>
        /// The color of the 0db line drawn on the output meter.
        /// </summary>
        [XmlIgnore]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("The color of the 0db line drawn on the output meter.")]
        public Color Meter0dbLineColor
        {
            get
            {
                return meter0dbLineColor;
            }
            set
            {
                meter0dbLineColor = value;
            }
        }
        /// <summary>
        /// Gets/sets the 0db line color using a 32-bit integer (ARGB).
        /// This is used for serializing the Color structure in XML.
        /// </summary>
        [Browsable(false)]
        [XmlElement(ElementName = "Meter0dbLineColor")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Meter0dbLineColorInt
        {
            get
            {
                return meter0dbLineColor.ToArgb();
            }
            set
            {
                meter0dbLineColor = Color.FromArgb(value);
            }
        }

        private Color meterPeakLineColor = Color.LightGray;
        /// <summary>
        /// The color of the peak line drawn on the output meter.
        /// </summary>
        [XmlIgnore]
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("The color of the peak line drawn on the output meter.")]
        public Color MeterPeakLineColor
        {
            get
            {
                return meterPeakLineColor;
            }
            set
            {
                meterPeakLineColor = value;
            }
        }
        /// <summary>
        /// Gets/sets the peak line color using a 32-bit integer (ARGB).
        /// This is used for serializing the Color structure in XML.
        /// </summary>
        [Browsable(false)]
        [XmlElement(ElementName = "MeterPeakLineColor")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int MeterPeakLineColorInt
        {
            get
            {
                return meterPeakLineColor.ToArgb();
            }
            set
            {
                meterPeakLineColor = Color.FromArgb(value);
            }
        }

        /// <summary>
        /// Default constructor for the OutputMeterTheme class.
        /// </summary>
        public OutputMeterTheme()
        {
        }
    }
}
