//
// Config.cs: Configuration helper class.
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
using System.Configuration;
using System.Linq;
using System.Text;

namespace MPfm.Core
{
    /// <summary>
    /// The Config class is a helper class to simplify loading and saving configuration files.
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// Save configuration value to configuration file.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        public static void Save(string key, string value)
        {            
            // Load configuration file
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            // Check if the key exists
            if (config.AppSettings.Settings[key] != null)
            {
                // Remove key
                config.AppSettings.Settings.Remove(key);
            }

            // Add key
            config.AppSettings.Settings.Add(key, value);

            // Save configuration file
            config.Save();
        }

        /// <summary>
        /// Loads configuration value from configuration file.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Value</returns>
        public static string Load(string key)
        {            
            // Load configuration file
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            // Check if the key exists
            if (config.AppSettings.Settings[key] == null)
            {
                // Return null value
                return null;
            }

            // Return configuration value
            return config.AppSettings.Settings[key].Value;
        }
    }
}
