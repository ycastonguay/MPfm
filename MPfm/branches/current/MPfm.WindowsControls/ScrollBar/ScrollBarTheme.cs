﻿//
// ScrollBarTheme.cs: Theme object for the HScrollBar and VScrollBar controls.
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
    /// Theme object for the HScrollBar and VScrollBar controls.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ScrollBarTheme
    {
        /// <summary>
        /// Default constructor for the ScrollBarTheme class.
        /// </summary>
        public ScrollBarTheme()
        {
        }
    }
}