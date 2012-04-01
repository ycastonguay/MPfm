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

namespace MPfm.WindowsControls
{
    /// <summary>
    /// Theme object for the OutputMeter control.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class OutputMeterTheme
    {
        #region Background Properties

        /// <summary>
        /// Private value for the GradientColor1 property.
        /// </summary>
        private Color gradientColor1 = Color.Black;
        /// <summary>
        /// First color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Background"), Browsable(true), Description("First color of the background gradient.")]
        public Color GradientColor1
        {
            get
            {
                return gradientColor1;
            }
            set
            {
                gradientColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the GradientColor2 property.
        /// </summary>
        private Color gradientColor2 = Color.FromArgb(40, 40, 40);
        /// <summary>
        /// Second color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Background"), Browsable(true), Description("Second color of the background gradient.")]
        public Color GradientColor2
        {
            get
            {
                return gradientColor2;
            }
            set
            {
                gradientColor2 = value;
            }
        }

        /// <summary>
        /// Private value for the GradientMode property.
        /// </summary>
        private LinearGradientMode gradientMode = LinearGradientMode.Vertical;
        /// <summary>
        /// Background gradient mode.
        /// </summary>
        [Category("Background"), Browsable(true), Description("Background gradient mode.")]
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

        #endregion

        #region Font Properties
        
        /// <summary>
        /// Private value for the CustomFont property.
        /// </summary>
        private CustomFont customFont = null;
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
        /// Private value for the FontColor property.
        /// </summary>
        private Color fontColor = Color.White;
        /// <summary>
        /// Fore font color used when displaying the volume peak in decibels.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Fonts"), Browsable(true), Description("Fore font color used when displaying the volume peak in decibels.")]
        public Color FontColor
        {
            get
            {
                return fontColor;
            }
            set
            {
                fontColor = value;
            }
        }

        /// <summary>
        /// Private value for the FontShadowColor property.
        /// </summary>
        private Color fontShadowColor = Color.Gray;
        /// <summary>
        /// Fore font color used when displaying the volume peak in decibels (drop shadow under text).
        /// </summary>
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

        #endregion

        #region Meter Properties

        private Color meterGradientColor1 = Color.Green;
        /// <summary>
        /// The first color of the color gradient used when drawing the output meter.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("The first color of the color gradient used when drawing the output meter.")]
        public Color MeterGradientColor1
        {
            get
            {
                return meterGradientColor1;
            }
            set
            {
                meterGradientColor1 = value;
            }
        }

        private Color meterGradientColor2 = Color.LightGreen;
        /// <summary>
        /// The second color of the color gradient used when drawing the output meter.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("The second color of the color gradient used when drawing the output meter.")]
        public Color MeterGradientColor2
        {
            get
            {
                return meterGradientColor2;
            }
            set
            {
                meterGradientColor2 = value;
            }
        }

        private Color meterDistortionGradientColor1 = Color.DarkRed;
        /// <summary>
        /// The first color of the color gradient used when drawing the output meter.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("The first color of the color gradient used when drawing the output meter.")]
        public Color MeterDistortionGradientColor1
        {
            get
            {
                return meterDistortionGradientColor1;
            }
            set
            {
                meterDistortionGradientColor1 = value;
            }
        }

        private Color meterDistortionGradientColor2 = Color.Red;
        /// <summary>
        /// The second color of the color gradient used when drawing the output meter and value exceeds distortion threshold.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("The second color of the color gradient used when drawing the output meter and value exceeds distortion threshold.")]
        public Color MeterDistortionGradientColor2
        {
            get
            {
                return meterDistortionGradientColor2;
            }
            set
            {
                meterDistortionGradientColor2 = value;
            }
        }

        private Color meter0dbLineColor = Color.DarkGray;
        /// <summary>
        /// The color of the 0db line drawn on the output meter.
        /// </summary>
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

        private Color meterPeakLineColor = Color.LightGray;
        /// <summary>
        /// The color of the peak line drawn on the output meter.
        /// </summary>
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

        #endregion

        /// <summary>
        /// Default constructor for the OutputMeterTheme class.
        /// </summary>
        public OutputMeterTheme()
        {
            // Set default values
            customFont = new CustomFont();
            customFont.EmbeddedFontName = "Junction";
            customFont.Size = 8;
            customFont.UseEmbeddedFont = true;
        }
    }
}
