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
using Sessions.Player.Objects;
using org.sessionsapp.player;

namespace Sessions.MVP.Helpers
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
        public static SSPEQPreset Load(string filePath)
        {
#if WINDOWSSTORE
            return new EQPreset();
#else
            var deserializer = new XmlSerializer(typeof(SSPEQPreset));
            TextReader textReader = new StreamReader(filePath);
            Object obj = deserializer.Deserialize(textReader);
            var theme = (SSPEQPreset)obj;
            return theme;
#endif
        }

        /// <summary>
        /// Saves EQPreset to file.
        /// </summary>
        /// <param name="filePath">EQ preset file path</param>
        /// <param name="eqPreset">EQPreset object</param>
        public static void Save(string filePath, SSPEQPreset eqPreset)
        {
#if !WINDOWSSTORE
            var serializer = new XmlSerializer(typeof(SSPEQPreset));
            TextWriter textWriter = new StreamWriter(filePath);
            serializer.Serialize(textWriter, eqPreset);
            textWriter.Dispose();
#endif
        }
    }
}

