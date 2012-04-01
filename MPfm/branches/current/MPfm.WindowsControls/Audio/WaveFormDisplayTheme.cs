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

namespace MPfm.WindowsControls
{
    /// <summary>
    /// Theme object for the WaveFormDisplay control.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class WaveFormDisplayTheme
    {
        #region Background Properties

        /// <summary>
        /// Private value for the Gradient property.
        /// </summary>
        private Gradient gradient = new Gradient(Color.Black, Color.FromArgb(50, 50, 50), LinearGradientMode.Vertical);
        /// <summary>
        /// Defines the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Configuration"), Browsable(true), Description("Defines the background gradient.")]
        public Gradient Gradient
        {
            get
            {
                return gradient;
            }
            set
            {
                gradient = value;
            }
        }

        ///// <summary>
        ///// Private value for the BackgroundGradientColor1 property.
        ///// </summary>
        //private Color backgroundGradientColor1 = Color.FromArgb(0, 0, 0);
        ///// <summary>
        ///// First color of the background gradient.
        ///// </summary>
        //[RefreshProperties(RefreshProperties.Repaint)]
        //[Category("Background"), Browsable(true), Description("First color of the background gradient.")]
        //public Color BackgroundGradientColor1
        //{
        //    get
        //    {
        //        return backgroundGradientColor1;
        //    }
        //    set
        //    {
        //        backgroundGradientColor1 = value;
        //    }
        //}

        ///// <summary>
        ///// Private value for the BackgroundGradientColor2 property.
        ///// </summary>
        //private Color backgroundGradientColor2 = Color.FromArgb(50, 50, 50);
        ///// <summary>
        ///// Second color of the background gradient.
        ///// </summary>
        //[RefreshProperties(RefreshProperties.Repaint)]
        //[Category("Background"), Browsable(true), Description("Second color of the background gradient.")]
        //public Color BackgroundGradientColor2
        //{
        //    get
        //    {
        //        return backgroundGradientColor2;
        //    }
        //    set
        //    {
        //        backgroundGradientColor2 = value;
        //    }
        //}

        ///// <summary>
        ///// Private value for the BackgroundGradientMode property.
        ///// </summary>
        //private LinearGradientMode backgroundGradientMode = LinearGradientMode.Vertical;
        ///// <summary>
        ///// Background gradient mode.
        ///// </summary>
        //[RefreshProperties(RefreshProperties.Repaint)]
        //[Category("Background"), Browsable(true), Description("Background gradient mode.")]
        //public LinearGradientMode BackgroundGradientMode
        //{
        //    get
        //    {
        //        return backgroundGradientMode;
        //    }
        //    set
        //    {
        //        backgroundGradientMode = value;
        //    }
        //}

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

        #endregion


        /// <summary>
        /// Private value for the WaveFormColor property.
        /// </summary>
        private Color waveFormColor = Color.Yellow;
        /// <summary>
        /// Color used when drawing the wave form.
        /// </summary>
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
        /// Private value for the CursorColor property.
        /// </summary>
        private Color cursorColor = Color.RoyalBlue;
        /// <summary>
        /// Color used when drawing the current song position cursor over the wave form.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Color used when drawing the current song position cursor over the wave form.")]
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
        /// Default constructor for the WaveFormDisplayTheme class.
        /// </summary>
        public WaveFormDisplayTheme()
        {            
            // Set default values
            customFont = new CustomFont();
            customFont.EmbeddedFontName = "Droid Sans Mono";
            customFont.Size = 8;
            customFont.UseEmbeddedFont = true;
        }
    }
}
