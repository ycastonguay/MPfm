//
// Theme.cs: Defines a theme to be used in MPfm. Contains the different theme objects
//           for controls and general look and feel.
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

namespace MPfm.WindowsControls
{
    /// <summary>
    /// Defines a theme to be used in MPfm. Contains the different theme objects
    /// for controls and general look and feel.
    /// </summary>
    public class Theme
    {
        /// <summary>
        /// Private value for the MainWindow property.
        /// </summary>
        private MainWindowTheme mainWindow = null;
        /// <summary>
        /// Theme object for the main window.
        /// </summary>
        public MainWindowTheme MainWindow
        {
            get
            {
                return mainWindow;
            }
            set
            {
                mainWindow = value;
            }
        }

        /// <summary>
        /// Private value for the SecondaryWindow property.
        /// </summary>
        private SecondaryWindowTheme secondaryWindow = null;
        /// <summary>
        /// Theme object for the main window.
        /// </summary>
        public SecondaryWindowTheme SecondaryWindow
        {
            get
            {
                return secondaryWindow;
            }
            set
            {
                secondaryWindow = value;
            }
        }

        /// <summary>
        /// Private value for the SongGridView property.
        /// </summary>
        private SongGridViewTheme songGridView = null;
        /// <summary>
        /// Theme object for the SongGridView control.
        /// </summary>
        public SongGridViewTheme SongGridView
        {
            get
            {
                return songGridView;
            }
            set
            {
                songGridView = value;
            }
        }

        /// <summary>
        /// Private value for the OutputMeter property.
        /// </summary>
        private OutputMeterTheme outputMeter = null;
        /// <summary>
        /// Theme object for the SongGridView control.
        /// </summary>
        public OutputMeterTheme OutputMeter
        {
            get
            {
                return outputMeter;
            }
            set
            {
                outputMeter = value;
            }
        }

        /// <summary>
        /// Private value for the WaveFormDisplay property.
        /// </summary>
        private WaveFormDisplayTheme waveFormDisplay = null;
        /// <summary>
        /// Theme object for the WaveFormDisplay control.
        /// </summary>
        public WaveFormDisplayTheme WaveFormDisplay
        {
            get
            {
                return waveFormDisplay;
            }
            set
            {
                waveFormDisplay = value;
            }
        }

        /// <summary>
        /// Default constructor for the Theme class.
        /// </summary>
        public Theme()
        {
            // Create default objects
            mainWindow = new MainWindowTheme();
            secondaryWindow = new SecondaryWindowTheme();
            songGridView = new SongGridViewTheme();
            outputMeter = new OutputMeterTheme();
            waveFormDisplay = new WaveFormDisplayTheme();
        }
    }
}
