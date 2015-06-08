// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;

#if WINDOWSSTORE
using Windows.Storage;
#endif

namespace Sessions.Core.Helpers
{
    /// <summary>
    /// Helper static class for configuration paths.
    /// </summary>
    public static class PathHelper
    {
        public static string HomeDirectory { get; set; }
        public static string AudioFileDirectory { get; set; } // Only used on iOS
        public static string PeakFileDirectory { get; set; }
        public static string ImageCacheDirectory { get; set; }
        public static string ConfigurationFilePath { get; set; }
        public static string UnitTestConfigurationFilePath { get; set; }
        public static string DeviceStoreFilePath { get; set; }
        public static string DatabaseFilePath { get; set; }
        public static string LogFilePath { get; set; }
		
		static PathHelper()
		{
            // Vista/Windows7: C:\Users\%username%\AppData\Roaming\
            // XP: C:\Documents and Settings\%username%\Application Data\
			
#if IOS || ANDROID
        	HomeDirectory = Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);            
            AudioFileDirectory = HomeDirectory.Replace(".Sessions", string.Empty);
#elif WINDOWSSTORE
		    HomeDirectory = ApplicationData.Current.LocalFolder.Path;
#elif WINDOWS_PHONE
		    HomeDirectory = "TODO";
#else
            HomeDirectory = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), ".Sessions");
#endif

            PeakFileDirectory = Path.Combine(HomeDirectory, "PeakFiles");
            ImageCacheDirectory = Path.Combine(HomeDirectory, "ImageCache");
			ConfigurationFilePath = Path.Combine(HomeDirectory, "Sessions.Configuration.xml");
            UnitTestConfigurationFilePath = Path.Combine(HomeDirectory, "Sessions.UnitTest.Configuration.xml");
            DeviceStoreFilePath = Path.Combine(HomeDirectory, "Sessions.Devices.json");
			DatabaseFilePath = Path.Combine(HomeDirectory, "Sessions.Database.db");
			LogFilePath = Path.Combine(HomeDirectory, "Sessions.Log.txt");            
		}
    }
}
