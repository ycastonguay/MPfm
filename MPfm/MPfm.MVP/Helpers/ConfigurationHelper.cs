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
			//string assemblyDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			
#if IOS || ANDROID
        	HomeDirectory = Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
#else
			HomeDirectory = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), ".MPfm");
#endif

			// Generate file paths
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
            XmlSerializer deserializer = new XmlSerializer(typeof(MPfmConfig));
            TextReader textReader = new StreamReader(filePath);
            Object obj = deserializer.Deserialize(textReader);
            MPfmConfig theme = (MPfmConfig)obj;
            return theme;
        }
        
        /// <summary>
        /// Saves MPfmConfig to file.
        /// </summary>
        /// <param name="filePath">Configuration file path</param>
        /// <param name="config">MPfmConfig object</param>
        public static void Save(string filePath, MPfmConfig config)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MPfmConfig));
            TextWriter textWriter = new StreamWriter(filePath);
            serializer.Serialize(textWriter, config);
            textWriter.Close();
        }
    }
}

