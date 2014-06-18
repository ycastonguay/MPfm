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
using Sessions.Player.Objects;

namespace MPfm.MVP.Helpers
{
    /// <summary>
    /// Helper static class for loading/saving EQ presets.
    /// </summary>
    public static class EQPresetHelper
    {
        /// <summary>
        /// Loads EQPreset from file.
        /// </summary>
        /// <param name="filePath">EQ preset file path</param>
        /// <returns>EQPreset object</returns>
        public static EQPreset Load(string filePath)
        {
#if WINDOWSSTORE
            return new EQPreset();
#else
            XmlSerializer deserializer = new XmlSerializer(typeof(EQPreset));
            TextReader textReader = new StreamReader(filePath);
            Object obj = deserializer.Deserialize(textReader);
            EQPreset theme = (EQPreset)obj;
            return theme;
#endif
        }
        
        /// <summary>
        /// Saves EQPreset to file.
        /// </summary>
        /// <param name="filePath">EQ preset file path</param>
        /// <param name="eqPreset">EQPreset object</param>
        public static void Save(string filePath, EQPreset eqPreset)
        {
#if !WINDOWSSTORE
            XmlSerializer serializer = new XmlSerializer(typeof(EQPreset));
            TextWriter textWriter = new StreamWriter(filePath);
            serializer.Serialize(textWriter, eqPreset);
            textWriter.Dispose();
#endif
        }
    }
}

