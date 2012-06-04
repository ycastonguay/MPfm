﻿//
// ConfigurationHelper.cs: Helper static class for configuration paths.
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
using System.IO;
using System.Reflection;

namespace MPfm.MVP
{
    /// <summary>
    /// Helper static class for configuration paths.
    /// </summary>
    public static class ConfigurationHelper
    {
		/// <summary>
		/// Current user home directory.
		/// </summary>
		public static string HomeDirectory;
		/// <summary>
		/// Current user configuration file path.
		/// </summary>
		public static string ConfigurationFilePath;
		/// <summary>
		/// Current database file path.
		/// </summary>
		public static string DatabaseFilePath;
		/// <summary>
		/// Current log file path.
		/// </summary>
		public static string LogFilePath;		
		
		static ConfigurationHelper()
		{
			// Get assembly directory
			string assemblyDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			
#if (MACOSX)
        	HomeDirectory = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), ".MPfm");
#elif (LINUX)
			HomeDirectory = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), ".MPfm");
#elif (!MACOSX && !LINUX)						
			HomeDirectory = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), ".MPfm");
#endif

			// Generate file paths
			ConfigurationFilePath = Path.Combine(HomeDirectory, "MPfm.Configuration.xml");
			DatabaseFilePath = Path.Combine(HomeDirectory, "MPfm.Database.db");
			LogFilePath = Path.Combine(HomeDirectory, "MPfm.Log.txt");
		}
    }
}
