﻿// Copyright © 2011-2013 Yanick Castonguay
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
using System.Xml.Serialization;
using MPfm.MVP.Config;

#if WINDOWSSTORE
using Windows.Storage;
#endif

namespace MPfm.MVP.Helpers
{
    /// <summary>
    /// Helper static class for configuration paths.
    /// </summary>
    public static class ConfigurationHelper
    {
		public static string HomeDirectory;
        public static string PeakFileDirectory;
		public static string ConfigurationFilePath;
		public static string DatabaseFilePath;
		public static string LogFilePath;		
		
		static ConfigurationHelper()
		{
			// Get assembly directory
			//string assemblyDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            //// Get application data folder path
            //// Vista/Windows7: C:\Users\%username%\AppData\Roaming\
            //// XP: C:\Documents and Settings\%username%\Application Data\
            //applicationDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MPfm";
			
#if IOS || ANDROID
        	HomeDirectory = Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            PeakFileDirectory = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "PeakFiles");
#elif WINDOWSSTORE
		    HomeDirectory = ApplicationData.Current.LocalFolder.Path;
		    PeakFileDirectory = Path.Combine(ApplicationData.Current.LocalFolder.Path, "PeakFiles");
#elif WINDOWS_PHONE
		    HomeDirectory = "TODO";
		    PeakFileDirectory = "TODO";
#else
            HomeDirectory = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), ".MPfm");
            PeakFileDirectory = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), ".MPfm", "PeakFiles");
#endif

			ConfigurationFilePath = Path.Combine(HomeDirectory, "MPfm.Configuration.xml");
			DatabaseFilePath = Path.Combine(HomeDirectory, "MPfm.Database.db");
			LogFilePath = Path.Combine(HomeDirectory, "MPfm.Log.txt");            
		}
    }
}
