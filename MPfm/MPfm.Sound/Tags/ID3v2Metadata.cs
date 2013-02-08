//
// ID3v2Metadata.cs: Reads and writes ID3v2 metadata for multiple audio formats.
//
// Copyright © 2011-2012 Yanick Castonguay
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
using System.Text;

namespace MPfm.Sound.Tags
{
    /// <summary>
    /// Reads and writes ID3v2 metadata for multiple audio formats.    
    /// ID3v2 specifications: http://www.id3.org/id3v2.4.0-structure
    /// </summary>
    public static class ID3v2Metadata
    {
        /// <summary>
        /// Reads the ID3v2 metadata from an audio file.
        /// Supports the following audio formats: APE.
        /// </summary>
        /// <param name="filePath">Audio file path</param>        
        /// <returns>ID3v2 data structure</returns>
        public static ID3v2Tag Read(string filePath)
        {
            // Check for nulls or empty
            if (String.IsNullOrEmpty(filePath))
            {
                throw new Exception("The file path cannot be null or empty!");
            }

            // Declare variables
            ID3v2Tag data = new ID3v2Tag();
            FileStream stream = null;
            BinaryReader reader = null;

            try
            {
                // Open binary reader                        
                stream = File.OpenRead(filePath);
                reader = new BinaryReader(stream);

                // The metadata is at the start of the file

                // Read header (10 bytes)
                // Read file identifier
                byte[] bytesFileIdentifier = reader.ReadBytes(3);
                string fileIdentifier = Encoding.UTF8.GetString(bytesFileIdentifier);

                // Read version 
                byte[] bytesVersion = reader.ReadBytes(2);
                int version = BitConverter.ToInt16(bytesVersion, 0);

                // Read flags
                byte byteFlags = reader.ReadByte();
                int stuff = byteFlags & 64;

                // Read tag size
                byte[] bytesTagSize = reader.ReadBytes(4);

                // Check version                    
                if (version == 4)
                {
                    // ID3v2.4+ uses synchsafe integers
                    data.TagSize = ID3v2Metadata.UnSynchSafe(bytesTagSize);
                }
                else
                {
                    data.TagSize = BitConverter.ToInt32(bytesTagSize, 0);
                }

                // Set flag
                data.TagFound = true;

                // Read extended header if found
                if (data.ExtendedHeader)
                {
                    // Get extended header size (32-bit)
                }

                // Read ID3v2 frames
                byte[] bytesFrameID = reader.ReadBytes(4);
                string frameID = Encoding.UTF8.GetString(bytesFrameID).ToUpper();

                // Read size
                byte[] bytesSize = reader.ReadBytes(4);
                int size = BitConverter.ToInt32(bytesSize, 0);

                if (frameID == "TIT2")
                {

                }

                //int tagSize = BitConverter.ToInt32(bytesTagSize, 0);


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

            return data;
        }

        /// <summary>
        /// Converts an integer to a synchsafe integer byte array.        
        /// </summary>
        /// <param name="integer">Integer</param>
        /// <returns>Byte array (synchsafe integer)</returns>
        public static byte[] SynchSafe(int integer)
        {
            byte[] bytes = new byte[4];                                  
            bytes[0] = (byte)((integer >> 21) & 0x7F);
            bytes[1] = (byte)((integer >> 14) & 0x7F);
            bytes[2] = (byte)((integer >> 7) & 0x7F);
            bytes[3] = (byte)(integer & 0x7F);

            return bytes;
        }

        /// <summary>
        /// Converts a synchsafe integer byte array to a standard integer.        
        /// </summary>
        /// <param name="byteArray">Synchsafe integer byte array</param>
        /// <returns>Integer</returns>
        public static int UnSynchSafe(byte[] byteArray)
        {
            int value = 0;
            value += ((int)byteArray[0]) << 21;
            value += ((int)byteArray[1]) << 14;
            value += ((int)byteArray[2]) << 7;
            value += (int)byteArray[3];

            return value;
        }
    }
}
