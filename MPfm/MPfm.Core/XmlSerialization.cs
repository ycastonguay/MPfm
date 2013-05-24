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
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Xml;
using System.Text;
using System.IO;

namespace MPfm.Core
{
    /// <summary>
    /// Static class for serializing objects to strings and vice versa.
    /// Taken from http://stackoverflow.com/questions/1564718/using-stringwriter-for-xml-serialization
    /// </summary>
    public static class XmlSerialization
    {
        public static string Serialize<T>(T value) {

            if(value == null)
                return null;

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = new UnicodeEncoding(false, false);
            settings.Indent = false;
            settings.OmitXmlDeclaration = false;

            using(StringWriter textWriter = new StringWriter()) {
                using(XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings)) {
                    serializer.Serialize(xmlWriter, value);
                }
                return textWriter.ToString();
            }
        }

        public static T Deserialize<T>(string xml) {
            if(string.IsNullOrEmpty(xml))
                return default(T);

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            XmlReaderSettings settings = new XmlReaderSettings();
            using(StringReader textReader = new StringReader(xml)) {
                using(XmlReader xmlReader = XmlReader.Create(textReader, settings)) {
                    return (T) serializer.Deserialize(xmlReader);
                }
            }
        }
    }
}
