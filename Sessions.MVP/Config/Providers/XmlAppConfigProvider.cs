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
using System.Xml.Serialization;
using Sessions.MVP.Config.Models;
using System.Threading.Tasks;

#if !WINDOWSSTORE
namespace Sessions.MVP.Config.Providers
{    
    public class XmlAppConfigProvider : IAppConfigProvider
    {
        private readonly object _locker = new object();
        private XmlSerializer _serializer;
        
        public XmlAppConfigProvider()
        {
            _serializer = new XmlSerializer(typeof(RootAppConfig));
        }
        
        /// <summary>
        /// Loads application settings from file.
        /// </summary>
        /// <param name="filePath">Configuration file path</param>
        /// <returns>AppConfigManager object</returns>
        public RootAppConfig Load(string filePath)
        {
            lock (_locker)
            {
                if (!File.Exists(filePath))
                    return new RootAppConfig();

                using (TextReader textReader = new StreamReader(filePath))
                {
                    Object obj = _serializer.Deserialize(textReader);
                    var theme = (RootAppConfig)obj;
                    return theme;
                }
            }
        }

        /// <summary>
        /// Saves AppConfigManager to file.
        /// </summary>
        /// <param name="filePath">Configuration file path</param>
        /// <param name="config">AppConfigManager object</param>
        public void Save(string filePath, RootAppConfig config)
        {
            Task.Factory.StartNew(() => {
                lock(_locker)
                {
                    using (TextWriter textWriter = new StreamWriter(filePath))
                    {
                        _serializer.Serialize(textWriter, config);
                    }
                }
            });
        }
    }
}
#endif
