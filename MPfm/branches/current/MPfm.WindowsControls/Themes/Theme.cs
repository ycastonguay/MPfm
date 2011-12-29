//
// Theme.cs: Defines a theme to be used in MPfm. Contains the different theme objects
//           for controls and general look and feel.
//
// Copyright © 2011 Yanick Castonguay
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

namespace MPfm.WindowsControls
{
    /// <summary>
    /// Defines a theme to be used in MPfm. Contains the different theme objects
    /// for controls and general look and feel.
    /// </summary>
    public class Theme
    {
        /// <summary>
        /// Private value for the General property.
        /// </summary>
        private GeneralTheme m_general = null;
        /// <summary>
        /// Theme object for the SongGridView control.
        /// </summary>
        public GeneralTheme General
        {
            get
            {
                return m_general;
            }
            set
            {
                m_general = value;
            }
        }

        /// <summary>
        /// Private value for the SongGridView property.
        /// </summary>
        private SongGridViewTheme m_songGridView = null;
        /// <summary>
        /// Theme object for the SongGridView control.
        /// </summary>
        public SongGridViewTheme SongGridView
        {
            get
            {
                return m_songGridView;
            }
            set
            {
                m_songGridView = value;
            }
        }

        /// <summary>
        /// Private value for the OutputMeter property.
        /// </summary>
        private OutputMeterTheme m_outputMeter = null;
        /// <summary>
        /// Theme object for the SongGridView control.
        /// </summary>
        public OutputMeterTheme OutputMeter
        {
            get
            {
                return m_outputMeter;
            }
            set
            {
                m_outputMeter = value;
            }
        }

        /// <summary>
        /// Default constructor for the Theme class.
        /// </summary>
        public Theme()
        {
            // Create default objects
            m_general = new GeneralTheme();
            m_songGridView = new SongGridViewTheme();
            m_outputMeter = new OutputMeterTheme();
        }
    }
}
