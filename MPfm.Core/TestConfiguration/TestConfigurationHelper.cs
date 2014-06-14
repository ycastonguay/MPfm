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
using System.Collections.Generic;
using MPfm.Core.Helpers;

namespace MPfm.Core.TestConfiguration
{
    public static class TestConfigurationHelper
    {
        public static TestConfigurationEntity Load()
        {
            return Load(PathHelper.UnitTestConfigurationFilePath);
        }
        
        public static TestConfigurationEntity Load(string filePath)
        {
            if (!File.Exists(filePath))
                return new TestConfigurationEntity();

            using (TextReader textReader = new StreamReader(filePath))
            {
                string xml = textReader.ReadToEnd();
                Object obj = XmlSerialization.Deserialize<TestConfigurationEntity>(xml);
                var config = (TestConfigurationEntity)obj;
                return config;
            }            
        }

        public static void Save(TestConfigurationEntity entity)
        {
            Save(PathHelper.UnitTestConfigurationFilePath, entity);
        }

        public static void Save(string filePath, TestConfigurationEntity entity)
        {
            using (TextWriter textWriter = new StreamWriter(filePath))
            {
                string xml = XmlSerialization.Serialize<TestConfigurationEntity>(entity);
                textWriter.Write(xml);
            }
        }
    }
}
