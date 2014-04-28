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
    /// Theme object for the TableLayoutPanel control.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TableLayoutPanelTheme
    {
        /// <summary>
        /// Private value for the IsBackgroundTransparent property.
        /// </summary>
        private bool isBackgroundTransparent = false;
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
        private BackgroundGradient backgroundGradient = new BackgroundGradient(Color.LightGray, Color.Gray, LinearGradientMode.Vertical, Color.DarkGray, 1);
        /// <summary>
        /// Defines the background gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Background gradient.")]
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
        /// Default constructor for the TableLayoutPanelTheme class.
        /// </summary>
        public TableLayoutPanelTheme()
        {
        }
    }
}
