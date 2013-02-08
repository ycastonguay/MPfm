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
using MPfm.Sound;
using MPfm.WindowsControls;

namespace MPfm
{
    /// <summary>
    /// Defines the SongGridView node inside the Controls configuration section.
    /// </summary>
    public class ControlsSongGridViewConfigurationSection
    {
        /// <summary>
        /// Private value for the Query property.
        /// </summary>
        private ControlsSongGridViewQueryConfigurationSection query;
        /// <summary>
        /// Defines the current query used in the SongGridView.
        /// </summary>
        public ControlsSongGridViewQueryConfigurationSection Query
        {
            get
            {
                return query;
            }
        }

        /// <summary>
        /// Private value for the Columns property.
        /// </summary>
        private List<SongGridViewColumn> columns;
        /// <summary>
        /// Defines the columns associated with the SongGridView control.
        /// </summary>
        public List<SongGridViewColumn> Columns
        {
            get
            {
                return columns;
            }
            set
            {
                columns = value;
            }
        }

        /// <summary>
        /// Default constructor for the ControlsSongGridViewConfigurationSection class.
        /// </summary>
        public ControlsSongGridViewConfigurationSection()
        {
            // Create sections
            query = new ControlsSongGridViewQueryConfigurationSection();
            columns = new List<SongGridViewColumn>();
        }
    }
}
