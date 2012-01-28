//
// GeneralConfigurationSection.cs: Configuration section used for MPfm general settings.
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
    /// This configuration section contains the general settings for MPfm.
    /// It works using key/value pairs similar to app.config/web.config files.
    /// </summary>
    public class GeneralConfigurationSection
    {
        /// <summary>
        /// Private value for the KeyValue property.
        /// </summary>
        private List<GeneralConfigurationKeyValue> m_keyValues;
        /// <summary>
        /// List of key/value pairs.
        /// </summary>
        public List<GeneralConfigurationKeyValue> KeyValues
        {
            get
            {
                return m_keyValues;
            }
        }

        /// <summary>
        /// Default constructor for the GeneralConfigurationSection class.
        /// </summary>
        public GeneralConfigurationSection()
        {
            // Create list
            m_keyValues = new List<GeneralConfigurationKeyValue>();
        }
    }
}
