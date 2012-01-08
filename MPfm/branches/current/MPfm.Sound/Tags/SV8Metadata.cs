﻿//
// SV8Metadata.cs: Reads SV8 metadata for MPC (MusePack) audio files.
//
// Copyright © 2011 Yanick Castonguay
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
using System.IO;
using System.Text;
using System.Reflection;

namespace MPfm.Sound
{
    /// <summary>
    /// Reads SV8 metadata for MPC (MusePack) audio files.
    /// SV8 specifications: http://trac.musepack.net/trac/wiki/SV8Specification
    /// </summary>
    public static class SV8Metadata
    {
        /// <summary>
        /// Reads the SV8 metadata from a MPC (MusePack) audio file.        
        /// </summary>
        /// <param name="filePath">Audio file path</param>        
        /// <returns>SV8 data structure</returns>
        public static SV8Tag Read(string filePath)
        {
            // Check for nulls or empty
            if (String.IsNullOrEmpty(filePath))
            {
                throw new Exception("The file path cannot be null or empty!");
            }

            // Create data structure
            SV8Tag data = new SV8Tag();

            try
            {
                // Open binary reader                        
                using (BinaryReader reader = new BinaryReader(File.OpenRead(filePath)))
                {                                        
                    // Read "File magic number"
                    byte[] bytesMagicNumber = reader.ReadBytes(4);
                    string magicNumber = Encoding.UTF8.GetString(bytesMagicNumber);

                    // Validate number
                    if (magicNumber.ToUpper() != "MPCK")
                    {
                        throw new Exception("The file is not in MPC/SV8 format!");
                    }

                    // Loop through header keys
                    while (true)
                    {
                        // Read key (16-bits)
                        byte[] bytesKey = reader.ReadBytes(2);
                        string key = Encoding.UTF8.GetString(bytesKey);

                        // Read size
                        byte byteSize = reader.ReadByte();

                        // Read value (size - 3 bytes)
                        byte[] bytesValue = reader.ReadBytes(byteSize - 3);

                        // Check key
                        if (key.ToUpper() == "SH")
                        {
                            // Stream header
                            byte[] bytesCRC = new byte[4] { bytesValue[0], bytesValue[1], bytesValue[2], bytesValue[3] };
                            byte byteStreamVersion = bytesValue[4];

                            // Check if the header version is 8
                            if (byteStreamVersion != 8)
                            {
                                throw new Exception("This file header version is not SV8!");
                            }                            

                            // Get sample count (variable integer)
                            int intLength = 0;
                            byte[] bytesRemainingSampleCount = new byte[bytesValue.Length - 5];
                            Array.Copy(bytesValue, 5, bytesRemainingSampleCount, 0, bytesValue.Length - 5);
                            long sampleCount = SV8Metadata.GetVariableLengthInteger(bytesRemainingSampleCount, ref intLength);

                            // Get beginning silence (variable integer)                            
                            byte[] bytesRemainingBeginSilence = new byte[bytesRemainingSampleCount.Length - intLength];
                            Array.Copy(bytesRemainingSampleCount, intLength, bytesRemainingBeginSilence, 0, bytesRemainingSampleCount.Length - intLength);
                            long beginSilence = SV8Metadata.GetVariableLengthInteger(bytesRemainingBeginSilence, ref intLength);

                            byte[] bytesRemaining = new byte[bytesRemainingBeginSilence.Length - intLength];
                            Array.Copy(bytesRemainingBeginSilence, intLength, bytesRemaining, 0, bytesRemainingBeginSilence.Length - intLength);

                            // There should be 3 bytes left, but only the first two seem useful.
                            // Convert to big endian
                            int bigEndian = GetInt32(bytesRemaining, 0, false);
                            int sampleFrequency = ((bigEndian & 0xE000) >> 13);
                            data.MaxUsedBands = ((bigEndian & 0x1F00) >> 8);
                            data.AudioChannels = ((bigEndian & 0x00F0) >> 4) + 1;
                            int midSideStereoUsed = ((bigEndian & 0x0008) >> 3);
                            data.AudioBlockFrames = ((bigEndian & 0x0007) >> 0);

                            // Set metadata
                            // Sample rate
                            if (sampleFrequency == 0)
                            {
                                data.SampleRate = 44100;
                            }
                            else if (sampleFrequency == 1)
                            {
                                data.SampleRate = 48000;
                            }
                            else if (sampleFrequency == 2)
                            {
                                data.SampleRate = 37800;
                            }
                            else if (sampleFrequency == 3)
                            {
                                data.SampleRate = 32000;
                            }

                            // Set other metadata
                            data.Length = sampleCount;                         
                            data.BeginningSilence = beginSilence;
                            data.MidSideStereoEnabled = (midSideStereoUsed == 1) ? true : false;
                        }
                        else if (key.ToUpper() == "RG")
                        {
                            // Replay gain
                        }
                        else if (key.ToUpper() == "EI")
                        {
                            // Encoder info
                        }
                        else if (key.ToUpper() == "SO")
                        {
                            // Seek table offset
                        }
                        else if (key.ToUpper() == "ST")
                        {
                            // Seek table
                        }
                        else if (key.ToUpper() == "CT")
                        {
                            // Chapter tag
                        }
                        else if (key.ToUpper() == "AP")
                        {
                            // This is an audio packet; no more header information
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Leave metadata empty
            }

            return data;
        }

        /// <summary>
        /// Returns a variable length integer. The length can vary between 8-bits to 64-bits.
        /// </summary>
        /// <param name="data">Byte array containing the data</param>
        /// <param name="intLength">Integer length (1 = 8-bit, 2 = 16-bit, etc.)</param>
        /// <returns>Value</returns>
        public static long GetVariableLengthInteger(byte[] data, ref int intLength)
        {
            // http://trac.musepack.net/trac/wiki/SV8Specification
            // bits, big-endian
            // 0xxx xxxx                                           - value 0 to  2^7-1
            // 1xxx xxxx  0xxx xxxx                                - value 0 to 2^14-1
            // 1xxx xxxx  1xxx xxxx  0xxx xxxx                     - value 0 to 2^21-1
            // 1xxx xxxx  1xxx xxxx  1xxx xxxx  0xxx xxxx          - value 0 to 2^28-1            

            long value = 0;
            for (intLength = 1; intLength <= 9; intLength++)
            {
                int currentByte = (int)data[intLength - 1];
                value = (value << 7);
                value = (value | (currentByte & 0x7F));
                if ((currentByte & 0x80) == 0)
                {
                    break;
                }                
            }

            return value;
        }

        /// <summary>
        /// Returns a 32-bit integer from a byte array. Big endian or little endian can be used.
        /// </summary>
        /// <param name="buffer">Byte array</param>
        /// <param name="offset">Offset</param>
        /// <param name="littleEndian">If true, little endian will be used</param>
        /// <returns>Value</returns>
        public static Int32 GetInt32(byte[] buffer, int offset, bool littleEndian)
        {
            if (littleEndian)
            {
                return buffer[offset + 1] << 8 | buffer[offset];
            }
            else
            {
                return buffer[offset] << 8 | buffer[offset + 1];
            }
        }
    }
}
