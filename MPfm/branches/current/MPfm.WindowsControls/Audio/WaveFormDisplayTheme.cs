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
        /// Private value for the BackgroundGradientColor1 property.
        /// </summary>
        private Color m_backgroundGradientColor1 = Color.FromArgb(0, 0, 0);
        /// <summary>
        /// First color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Background"), Browsable(true), Description("First color of the background gradient.")]
        public Color BackgroundGradientColor1
        {
            get
            {
                return m_backgroundGradientColor1;
            }
            set
            {
                m_backgroundGradientColor1 = value;
            }
        }

        /// <summary>
        /// Private value for the BackgroundGradientColor2 property.
        /// </summary>
        private Color m_backgroundGradientColor2 = Color.FromArgb(50, 50, 50);
        /// <summary>
        /// Second color of the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Background"), Browsable(true), Description("Second color of the background gradient.")]
        public Color BackgroundGradientColor2
        {
            get
            {
                return m_backgroundGradientColor2;
            }
            set
            {
                m_backgroundGradientColor2 = value;
            }
        }

        /// <summary>
        /// Private value for the BackgroundGradientMode property.
        /// </summary>
        private LinearGradientMode m_backgroundGradientMode = LinearGradientMode.Vertical;
        /// <summary>
        /// Background gradient mode.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Background"), Browsable(true), Description("Background gradient mode.")]
        public LinearGradientMode BackgroundGradientMode
        {
            get
            {
                return m_backgroundGradientMode;
            }
            set
            {
                m_backgroundGradientMode = value;
            }
        }

        #endregion

        #region Font Properties

        /// <summary>
        /// Private value for the CustomFont property.
        /// </summary>
        private CustomFont m_customFont = null;
        /// <summary>
        /// Defines the font to be used for rendering the control.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Font used for rendering the control.")]
        public CustomFont CustomFont
        {
            get
            {
                return m_customFont;
            }
            set
            {
                m_customFont = value;
            }
        }

        #endregion


        /// <summary>
        /// Private value for the WaveFormColor property.
        /// </summary>
        private Color m_waveFormColor = Color.Yellow;
        /// <summary>
        /// Color used when drawing the wave form.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Color used when drawing the wave form.")]
        public Color WaveFormColor
        {
            get
            {
                return m_waveFormColor;
            }
            set
            {
                m_waveFormColor = value;
            }
        }

        /// <summary>
        /// Private value for the CursorColor property.
        /// </summary>
        private Color m_cursorColor = Color.RoyalBlue;
        /// <summary>
        /// Color used when drawing the current song position cursor over the wave form.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Display"), Browsable(true), Description("Color used when drawing the current song position cursor over the wave form.")]
        public Color CursorColor
        {
            get
            {
                return m_cursorColor;
            }
            set
            {
                m_cursorColor = value;
            }
        }

        /// <summary>
        /// Default constructor for the WaveFormDisplayTheme class.
        /// </summary>
        public WaveFormDisplayTheme()
        {            
            // Set default values
            m_customFont = new CustomFont();
            m_customFont.EmbeddedFontName = "Droid Sans Mono";
            m_customFont.Size = 8;
            m_customFont.UseEmbeddedFont = true;
        }
    }
}
