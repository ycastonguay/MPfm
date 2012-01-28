//
// WindowsConfigurationSection.cs: Configuration section used for MPfm windows settings.
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
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using MPfm.Core;
using MPfm.Sound;

namespace MPfm
{
    /// <summary>
    /// This configuration section contains the windows settings for MPfm.    
    /// </summary>
    public class WindowsConfigurationSection
    {
        /// <summary>
        /// Private value for the Windows property.
        /// </summary>
        private List<WindowConfiguration> m_windows;
        /// <summary>
        /// List of key/value pairs.
        /// </summary>
        public List<WindowConfiguration> Windows
        {
            get
            {
                return m_windows;
            }
        }

        /// <summary>
        /// Default constructor for the WindowsConfigurationSection class.
        /// </summary>
        public WindowsConfigurationSection()
        {
            // Create list
            m_windows = new List<WindowConfiguration>();

            // Create main window
            WindowConfiguration windowMain = new WindowConfiguration();
            windowMain.Name = "Main";
            windowMain.Width = 1000;
            windowMain.Height = 750;

            // Create playlist window
            WindowConfiguration windowPlaylist = new WindowConfiguration();
            windowPlaylist.Name = "Playlist";
            windowPlaylist.Visible = false;            

            // Add windows to list
            m_windows.Add(windowMain);
            m_windows.Add(windowPlaylist);
        }
    }
}
