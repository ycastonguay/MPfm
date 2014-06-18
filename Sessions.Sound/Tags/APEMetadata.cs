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
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sessions.Sound.Tags
{
    /// <summary>
    /// Reads and writes APEv1/APEv2 metadata for multiple audio formats.
    /// The only difference between APEv1 and APEv2 is that APEv2 has a header and APEv1 has no header.
    /// APEv2 specifications: http://wiki.hydrogenaudio.org/index.php?title=APEv2_specification
    /// </summary>
    public static class APEMetadata
    {
        /// <summary>
        /// Reads the APEv1/APEv2 metadata from an audio file.
        /// Supports the following audio formats: APE.
        /// </summary>
        /// <param name="filePath">Audio file path</param>        
        /// <returns>APE data structure</returns>
        public static APETag Read(string filePath)
        {            
            if (String.IsNullOrEmpty(filePath))
                throw new Exception("The file path cannot be null or empty!");

            APETag data = new APETag();

            #if WINDOWSSTORE
            #else
            BinaryReader reader = null;
            FileStream stream = null;

            try
            {
                // Open binary reader             
                stream = File.OpenRead(filePath);
                reader = new BinaryReader(stream);

                // APEv1/APEv2 have footers, but only APEv2 has a header. 
                // In fact, the header and footer contain the exact same information.
                // http://wiki.hydrogenaudio.org/index.php?title=APE_Tags_Header

                // APE metadata can always be found at the very end of the file.
                // TODO: maybe seek the reader near the end of file?
                //byte[] bytes = reader.ReadBytes(4);

                // The header/footer structure is as followed:
                // 1) Preamble (64-bits) APETAGEX
                // 2) Version number (32-bits) 1000 or 2000
                // 3) Tag size (32-bits)
                // 4) Item count (32-bits)
                // 5) Tags flags (32-bits)
                // 6) Reserved (64-bits) Must be zero
                // Total: 32 bytes

                // Seek at the end of the file (length - 32 bytes)
                reader.BaseStream.Seek(-32, SeekOrigin.End);

                // Read Preamble (64-bits)
                byte[] bytesPreamble = reader.ReadBytes(8);
                string preamble = Encoding.UTF8.GetString(bytesPreamble, 0, bytesPreamble.Length);

                // Validate preamble
                if (preamble != "APETAGEX")
                {
                    throw new APETagNotFoundException("The APE tag was not found.");
                }

                // Read Version number (32-bits)
                byte[] bytesVersionNumber = reader.ReadBytes(4);
                int versionNumber = BitConverter.ToInt32(bytesVersionNumber, 0);

                // Check version number
                if (versionNumber == 1000)
                {
                    data.Version = APETagVersion.APEv1;
                }
                else if (versionNumber == 2000)
                {
                    data.Version = APETagVersion.APEv2;
                }
                else
                {
                    throw new APETagNotFoundException("The APE tag version is unknown (" + versionNumber.ToString() + ").");
                }

                // Read Tag size (32-bits)
                // This is the total size of the items + header (if APEv2)
                byte[] bytesTagSize = reader.ReadBytes(4);
                data.TagSize = BitConverter.ToInt32(bytesTagSize, 0);

                // Read Item count (32-bits)
                byte[] bytesItemCount = reader.ReadBytes(4);
                int itemCount = BitConverter.ToInt32(bytesItemCount, 0);

                // Read Tags flags (32-bits)
                // http://wiki.hydrogenaudio.org/index.php?title=Ape_Tags_Flags
                byte[] bytesTagsFlags = reader.ReadBytes(4);

                // Read reserved (64-bits)
                byte[] bytesReserved = reader.ReadBytes(8);

                // Seek back to the start of the APE metadata (32 bytes - tag size)
                reader.BaseStream.Seek(-32 - data.TagSize, SeekOrigin.End);

                // Is this APEv2?
                if (data.Version == APETagVersion.APEv2)
                {
                    // Skip header (32 bytes)
                    reader.BaseStream.Seek(32, SeekOrigin.Current);
                }

                // Read items
                for (int a = 0; a < itemCount; a++)
                {
                    // Read item value size
                    byte[] bytesItemValueSize = reader.ReadBytes(4);
                    int itemValueSize = BitConverter.ToInt32(bytesItemValueSize, 0);

                    // Read item flags
                    byte[] bytesItemFlags = reader.ReadBytes(4);

                    // The key size is variable but always ends by a key terminator (0x00).
                    // The key characters vary from 0x20 (space) to 0x7E (tilde).

                    // Read key
                    List<byte> bytesKey = new List<byte>();
                    while (true)
                    {
                        // Read characters until the terminator is found (0x00)
                        byte byteChar = reader.ReadByte();
                        if (byteChar == 0x00)
                        {
                            // Exit loop
                            break;
                        }
                        else
                        {
                            bytesKey.Add(byteChar);
                        }
                    }

                    // Read value
                    byte[] bytesValue = reader.ReadBytes(itemValueSize);

                    // Cast key and value into UTF8 strings             
                    var bytesKeyArray = bytesKey.ToArray();
                    string key = Encoding.UTF8.GetString(bytesKeyArray, 0, bytesKeyArray.Length);
                    string value = Encoding.UTF8.GetString(bytesValue, 0, bytesValue.Length);

                    // Add key/value to dictionary
                    data.Dictionary.Add(key, value);
                }

                // Go through dictionary items
                foreach (KeyValuePair<string, string> keyValue in data.Dictionary)
                {
                    string key = keyValue.Key.ToUpper().Trim();
                    if (key == "TITLE")
                    {
                        data.Title = keyValue.Value;
                    }
                    else if (key == "SUBTITLE")
                    {
                        data.Subtitle = keyValue.Value;
                    }
                    else if (key == "ARTIST")
                    {
                        data.Artist = keyValue.Value;
                    }
                    else if (key == "ALBUM")
                    {
                        data.Album = keyValue.Value;
                    }
                    else if (key == "DEBUT ALBUM")
                    {
                        data.DebutAlbum = keyValue.Value;
                    }
                    else if (key == "PUBLISHER")
                    {
                        data.Publisher = keyValue.Value;
                    }
                    else if (key == "CONDUCTOR")
                    {
                        data.Conductor = keyValue.Value;
                    }
                    else if (key == "TRACK")
                    {
                        int value = 0;
                        int.TryParse(keyValue.Value, out value);
                        data.Track = value;
                    }
                    else if (key == "COMPOSER")
                    {
                        data.Composer = keyValue.Value;
                    }
                    else if (key == "COMMENT")
                    {
                        data.Comment = keyValue.Value;
                    }
                    else if (key == "COPYRIGHT")
                    {
                        data.Copyright = keyValue.Value;
                    }
                    else if (key == "PUBLICATIONRIGHT")
                    {
                        data.PublicationRight = keyValue.Value;
                    }
                    else if (key == "EAN/UPC")
                    {
                        int value = 0;
                        int.TryParse(keyValue.Value, out value);
                        data.EAN_UPC = value;
                    }
                    else if (key == "ISBN")
                    {
                        int value = 0;
                        int.TryParse(keyValue.Value, out value);
                        data.ISBN = value;
                    }
                    else if (key == "LC")
                    {
                        data.LC = keyValue.Value;
                    }
                    else if (key == "YEAR")
                    {
                        DateTime value = DateTime.MinValue;
                        DateTime.TryParse(keyValue.Value, out value);
                        data.Year = value;
                    }
                    else if (key == "RECORD DATE")
                    {
                        DateTime value = DateTime.MinValue;
                        DateTime.TryParse(keyValue.Value, out value);
                        data.RecordDate = value;
                    }
                    else if (key == "RECORD LOCATION")
                    {
                        data.RecordLocation = keyValue.Value;
                    }
                    else if (key == "GENRE")
                    {
                        data.Genre = keyValue.Value;
                    }
                    else if (key == "INDEX")
                    {
                        data.Index = keyValue.Value;
                    }
                    else if (key == "RELATED")
                    {
                        data.Related = keyValue.Value;
                    }
                    else if (key == "ABSTRACT")
                    {
                        data.Abstract = keyValue.Value;
                    }
                    else if (key == "LANGUAGE")
                    {
                        data.Language = keyValue.Value;
                    }
                    else if (key == "BIBLIOGRAPHY")
                    {
                        data.Bibliography = keyValue.Value;
                    }
                }

            }
            catch
            {
                // Leave metadata empty
            }
            finally
            {
                // Dispose stream (reader will be automatically disposed too)                
                stream.Close();   
            }

            #endif

            return data;
        }

        /// <summary>
        /// Writes APEv2 metadata to an audio file.
        /// Supports the following audio formats: APE.
        /// </summary>
        /// <param name="filePath">Audio file path</param>        
        /// <param name="dictionary">List of APE tags to write</param>
        public static void Write(string filePath, Dictionary<string, string> dictionary)
        {
            if (String.IsNullOrEmpty(filePath))
                throw new Exception("The file path cannot be null or empty!");
            if (dictionary == null)
                throw new Exception("The dictionary cannot be null!");

            // Read the metadata first (to get tag size)
            APETag tag = Read(filePath);

            #if WINDOWSSTORE
            #else

            BinaryWriter writer = null;
            FileStream stream = null;
            
            try
            {
                // Open binary writer
                stream = File.Open(filePath, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
                writer = new BinaryWriter(stream);
                
                // Was an existing APE header found?
                if (tag.Version == APETagVersion.Unknown)
                {
                    // Seek position to end
                    writer.Seek(0, SeekOrigin.End);
                }
                else
                {
                    // Seek position (tag size - footer)
                    writer.Seek(-32 - tag.TagSize, SeekOrigin.End);

                    // Just cut off all existing APE data
                    writer.BaseStream.SetLength(writer.BaseStream.Length - 32 - tag.TagSize);
                }

                // Calculate tag size in bytes
                Int32 tagSize = 32; // we must count the header
                foreach (KeyValuePair<string, string> keyValue in dictionary)
                {
                    // Add key/value and other information to tag size
                    tagSize += 8; // item value size + item flags (2x32-bits)
                    tagSize += keyValue.Key.Length;
                    tagSize += 1; // terminator
                    tagSize += keyValue.Value.Length;
                }

                // Write header
                // Preamble
                writer.Write(Encoding.UTF8.GetBytes("APETAGEX"));

                // Version number
                writer.Write((Int32)2000);

                // Tag size
                writer.Write(tagSize);

                // Item count
                writer.Write((Int32)dictionary.Count);

                // Tags Flags                         
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((byte)0xA0); // Header

                // Reserved
                writer.Write((long)0);

                // Write items
                foreach (KeyValuePair<string, string> keyValue in dictionary)
                {
                    // Item value size
                    writer.Write((Int32)keyValue.Value.Length);

                    // Write item flags
                    writer.Write((Int32)0);

                    // Write item key in UTF8
                    writer.Write(Encoding.UTF8.GetBytes(keyValue.Key));

                    // Write terminator
                    writer.Write((byte)0);

                    // Write item key in UTF8
                    writer.Write(Encoding.UTF8.GetBytes(keyValue.Value));
                }

                // Write header
                // Preamble
                writer.Write(Encoding.UTF8.GetBytes("APETAGEX"));

                // Version number
                writer.Write((Int32)2000);

                // Tag size
                writer.Write(tagSize);

                // Item count
                writer.Write((Int32)dictionary.Count);

                // Tags Flags                         
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((byte)0x80); // Footer

                // Reserved
                writer.Write((long)0);
                
            }
            catch
            {
                throw;
            }
            finally
            {
                // Dispose stream (writer will be automatically disposed too)                
                stream.Close();   
            }

            #endif
        }
    }

    /// <summary>
    /// Exception raised when no APEv1/APEv2 tags have been found.
    /// </summary>
    #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
    [Serializable]
    #endif
    public class APETagNotFoundException 
        : Exception
    {
        /// <summary>
        /// Default constructor for the APETagNotFoundException exception class.
        /// </summary>
        /// <param name="message">Exception message</param>
        public APETagNotFoundException(string message) 
            : base(message)
        {
        }
    }
}
