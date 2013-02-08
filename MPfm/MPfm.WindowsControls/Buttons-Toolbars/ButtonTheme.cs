//
// ButtonTheme.cs: Theme object for the Button control.
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
    /// Theme object for the Button control.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ButtonTheme
    {
        /// <summary>
        /// Private value for the TextGradientDefault property.
        /// </summary>
        private TextGradient textGradientDefault = new TextGradient(Color.LightGray, Color.Gray, LinearGradientMode.Vertical, Color.DarkGray, 1, new CustomFont("Junction", 8.0f, Color.Black));
        /// <summary>
        /// Defines the text gradient (for the default state).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Defines the text gradient (for the default state).")]
        public TextGradient TextGradientDefault
        {
            get
            {
                return textGradientDefault;
            }
            set
            {
                textGradientDefault = value;
            }
        }

        /// <summary>
        /// Private value for the TextGradientMouseOver property.
        /// </summary>
        private TextGradient textGradientMouseOver = new TextGradient(Color.White, Color.LightGray, LinearGradientMode.Vertical, Color.FromArgb(70, 70, 70), 1, new CustomFont("Junction", 8.0f, Color.Black));
        /// <summary>
        /// Defines the text gradient (when the mouse cursor is over).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Defines the text gradient (when the mouse cursor is over).")]
        public TextGradient TextGradientMouseOver
        {
            get
            {
                return textGradientMouseOver;
            }
            set
            {
                textGradientMouseOver = value;
            }
        }

        /// <summary>
        /// Private value for the TextGradientDisabled property.
        /// </summary>
        private TextGradient textGradientDisabled = new TextGradient(Color.FromArgb(100, 100, 100), Color.FromArgb(50, 50, 50), LinearGradientMode.Vertical, Color.DarkGray, 1, new CustomFont("Junction", 8.0f, Color.LightGray));
        /// <summary>
        /// Defines the text gradient (when the control is disabled).
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint)]
        [Category("Theme"), Browsable(true), Description("Defines the text gradient (when the control is disabled).")]
        public TextGradient TextGradientDisabled
        {
            get
            {
                return textGradientDisabled;
            }
            set
            {
                textGradientDisabled = value;
            }
        }

        /// <summary>
        /// Default constructor for the ButtonTheme class.
        /// </summary>
        public ButtonTheme()
        {
        }
    }
}
