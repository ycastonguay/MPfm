//
// ControlsSongGridViewConfigurationSection.cs: Defines the SongGridView node 
//                                              inside the Controls configuration section.
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
// along with MPfm. If not, see <http:/s/www.gnu.org/licenses/>.

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
        private ControlsSongGridViewQueryConfigurationSection m_query;
        /// <summary>
        /// Defines the current query used in the SongGridView.
        /// </summary>
        public ControlsSongGridViewQueryConfigurationSection Query
        {
            get
            {
                return m_query;
            }
        }

        /// <summary>
        /// Private value for the Columns property.
        /// </summary>
        private List<SongGridViewColumn> m_columns;
        /// <summary>
        /// Defines the columns associated with the SongGridView control.
        /// </summary>
        public List<SongGridViewColumn> Columns
        {
            get
            {
                return m_columns;
            }
            set
            {
                m_columns = value;
            }
        }

        /// <summary>
        /// Default constructor for the ControlsSongGridViewConfigurationSection class.
        /// </summary>
        public ControlsSongGridViewConfigurationSection()
        {
            // Create sections
            m_query = new ControlsSongGridViewQueryConfigurationSection();
            m_columns = new List<SongGridViewColumn>();
        }
    }
}
