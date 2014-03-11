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
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace MPfm.WindowsControls
{
    /// <summary>
    /// This helper class contains methods for loading and saving theme files.
    /// </summary>
    public static class ThemeHelper
    {
        /// <summary>
        /// Loads a theme object from file.
        /// </summary>
        /// <param name="filePath">Theme file path</param>
        /// <returns>Theme object</returns>
        public static Theme Load(string filePath)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(Theme));
            TextReader textReader = new StreamReader(filePath);
            Object obj = deserializer.Deserialize(textReader);
            Theme theme = (Theme)obj;
            return theme;
        }

        /// <summary>
        /// Saves a theme object to file.
        /// </summary>
        /// <param name="filePath">Theme file path</param>
        /// <param name="theme">Theme object</param>
        public static void Save(string filePath, Theme theme)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Theme));
            TextWriter textWriter = new StreamWriter(filePath);
            serializer.Serialize(textWriter, theme);
            textWriter.Close();
        }
    }
}
