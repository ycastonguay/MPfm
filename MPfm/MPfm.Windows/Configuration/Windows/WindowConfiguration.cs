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
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using MPfm.Core;

namespace MPfm
{
    /// <summary>
    /// Defines the settings related to a MPfm window to be used in the Windows configuration section.
    /// </summary>
    public class WindowConfiguration
    {
        /// <summary>
        /// Window name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Defines if the window should use the default position.
        /// </summary>
        public bool UseDefaultPosition { get; set; }
        /// <summary>
        /// Window X position.
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// Window Y position.
        /// </summary>
        public int Y { get; set; }
        /// <summary>
        /// Window width.
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Window height.
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// Defines if the window is maximized.
        /// </summary>
        public bool Maximized { get; set; }
        /// <summary>
        /// Defines if the window is visible.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Default constructor for the GeneralConfigurationKeyValue class.
        /// </summary>
        public WindowConfiguration()
        {
            // Set default values            
            Name = string.Empty;
            UseDefaultPosition = true;
            X = 0;
            Y = 0;
            Width = 100;
            Height = 100;
            Maximized = false;
            Visible = true;
        }
    }
}
