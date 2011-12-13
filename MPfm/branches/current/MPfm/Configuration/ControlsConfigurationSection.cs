//
// ControlsConfigurationSection.cs: Configuration section used for MPfm controls 
//                                  settings.
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
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using MPfm.Core;
using MPfm.Sound;

namespace MPfm
{
    /// <summary>
    /// This configuration section contains the audio settings for MPfm.
    /// It has two main sections: SongGridView and PlaylistGridView.
    /// </summary>
    public class ControlsConfigurationSection
    {
        /// <summary>
        /// Private value for the SongGridView property.
        /// </summary>
        private ControlsSectionSongGridView m_songGridView = null;
        public ControlsSectionSongGridView SongGridView
        {
            get
            {
                return m_songGridView;
            }
        }

        /// <summary>
        /// Default constructor for the ControlsConfigurationSection class.
        /// </summary>
        public ControlsConfigurationSection()
        {
            m_songGridView = new ControlsSectionSongGridView();
        }
    }

    public class ControlsSectionSongGridView
    {
        private List<ControlsSectionSongGridViewColumn> m_columns = null;
        public List<ControlsSectionSongGridViewColumn> Columns
        {
            get
            {
                return m_columns;
            }
        }

        public ControlsSectionSongGridView()
        {
            m_columns = new List<ControlsSectionSongGridViewColumn>();
        }
    }

    public class ControlsSectionSongGridViewColumn
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public bool OrderBy { get; set; }
        public bool Visible { get; set; }
    }
}
