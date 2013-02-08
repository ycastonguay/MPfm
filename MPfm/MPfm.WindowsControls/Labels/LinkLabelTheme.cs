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
    /// Theme object for the Label control.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class LinkLabelTheme
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
        /// Private value for the TextGradient property.
        /// </summary>
        private TextGradient textGradient = new TextGradient(Color.LightGray, Color.Gray, LinearGradientMode.Vertical, Color.DarkGray, 1, new CustomFont("Junction", 8.0f, Color.Black));
        /// <summary>
        /// Defines the text gradient.
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Defines the text gradient.")]
        public TextGradient TextGradient
        {
            get
            {
                return textGradient;
            }
            set
            {
                textGradient = value;
            }
        }

        /// <summary>
        /// Default constructor for the LinkLabelTheme class.
        /// </summary>
        public LinkLabelTheme()
        {
        }
    }
}
