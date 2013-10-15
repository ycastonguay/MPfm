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

#if !WINDOWSSTORE
namespace MPfm.MVP.Config.Providers
{
    public class XmlAppConfigProvider : IAppConfigProvider
    {
        /// <summary>
        /// Loads application settings from file.
        /// </summary>
        /// <param name="filePath">Configuration file path</param>
        /// <returns>AppConfigManager object</returns>
        public RootAppConfig Load(string filePath)
        {
            if (!File.Exists(filePath))
                return new RootAppConfig();

            XmlSerializer deserializer = new XmlSerializer(typeof(RootAppConfig));
            TextReader textReader = new StreamReader(filePath);
            Object obj = deserializer.Deserialize(textReader);
            RootAppConfig theme = (RootAppConfig)obj;
            return theme;
        }

        /// <summary>
        /// Saves AppConfigManager to file.
        /// </summary>
        /// <param name="filePath">Configuration file path</param>
        /// <param name="config">AppConfigManager object</param>
        public void Save(string filePath, RootAppConfig config)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(RootAppConfig));
            TextWriter textWriter = new StreamWriter(filePath);
            serializer.Serialize(textWriter, config);
            textWriter.Dispose();
        }
    }
}
#endif
