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
using System.IO;
using System.Xml.Serialization;
using MPfm.MVP.Config;

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
#elif WINDOWSSTORE || WINDOWS_PHONE
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

        /// <summary>
        /// Loads MPfmConfig from file.
        /// </summary>
        /// <param name="filePath">Configuration file path</param>
        /// <returns>MPfmConfig object</returns>
        public static MPfmConfig Load(string filePath)
        {
#if WINDOWSSTORE
            return new MPfmConfig();
#else
            XmlSerializer deserializer = new XmlSerializer(typeof(MPfmConfig));
            TextReader textReader = new StreamReader(filePath);
            Object obj = deserializer.Deserialize(textReader);
            MPfmConfig theme = (MPfmConfig)obj;
            return theme;
#endif
        }
        
        /// <summary>
        /// Saves MPfmConfig to file.
        /// </summary>
        /// <param name="filePath">Configuration file path</param>
        /// <param name="config">MPfmConfig object</param>
        public static void Save(string filePath, MPfmConfig config)
        {
#if !WINDOWSSTORE
            XmlSerializer serializer = new XmlSerializer(typeof(MPfmConfig));
            TextWriter textWriter = new StreamWriter(filePath);
            serializer.Serialize(textWriter, config);
            textWriter.Dispose();
#endif
        }
    }
}

