//
// SV7Metadata.cs: Reads SV7 metadata for MPC (MusePack) audio files.
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
using System.Net;
using System.Text;
using System.Reflection;

namespace MPfm.Sound
{
    /// <summary>
    /// Reads SV7 metadata for MPC (MusePack) audio files.
    /// SV7 specifications: http://trac.musepack.net/trac/wiki/SV7Specification
    /// </summary>
    public static class SV7Metadata
    {
        /// <summary>
        /// Reads the SV7 metadata from a MPC (MusePack) audio file.        
        /// </summary>
        /// <param name="filePath">Audio file path</param>        
        /// <returns>SV7 data structure</returns>
        public static SV7Tag Read(string filePath)
        {
            // Check for nulls or empty
            if (String.IsNullOrEmpty(filePath))
            {
                throw new Exception("The file path cannot be null or empty!");
            }

            // Create data structure
            SV7Tag data = new SV7Tag();

            try
            {
                // Open binary reader                        
                BinaryReader reader = new BinaryReader(File.OpenRead(filePath));
                
                // Read signature
                byte[] bytesSigntaure = reader.ReadBytes(3);
                string signature = Encoding.UTF8.GetString(bytesSigntaure);

                // Validate signature
                if (signature.ToUpper() != "MP+")
                {
                    throw new SV7TagNotFoundException("The file is not in MPC/SV7 format!");
                }

                // Read stream major/minor version
                byte byteStreamMajorMinorVersion = reader.ReadByte();
                
                // Read frame count (32-bits)
                byte[] bytesFrameCount = reader.ReadBytes(4);
                Int32 frameCount = BitConverter.ToInt32(bytesFrameCount, 0);

                // Read Intensity stereo / Mid side stereo / Max band
                byte byteIntensityStereo_MidSideStereo_MaxBand = reader.ReadByte();
                int intensityStereo = (((int)byteIntensityStereo_MidSideStereo_MaxBand & 0x8000) >> 15);
                int midSideStereo = (((int)byteIntensityStereo_MidSideStereo_MaxBand & 0x4000) >> 14);
                int maxBand = (((int)byteIntensityStereo_MidSideStereo_MaxBand & 0x3F00) >> 8);

                // Read Profile / Link / SampleFreq
                byte byteProfile_Link_SampleFreq = reader.ReadByte();
                int profile = (((int)byteProfile_Link_SampleFreq & 0x0080) >> 4);
                int link = (((int)byteProfile_Link_SampleFreq & 0x0004) >> 2);
                int sampleFreq = (((int)byteProfile_Link_SampleFreq & 0x0003) >> 0);
                //bool intensityStereo = 

                // Read Max level
                byte[] bytesMaxLevel = reader.ReadBytes(2);
                Int16 maxLevel = BitConverter.ToInt16(bytesMaxLevel, 0);

                // Read title gain
                byte[] bytesTitleGain = reader.ReadBytes(2);
                Int16 titleGain = BitConverter.ToInt16(bytesTitleGain, 0);

                // Read title peak
                byte[] bytesTitlePeak = reader.ReadBytes(2);
                Int16 titlePeak = BitConverter.ToInt16(bytesTitlePeak, 0);

                // Read album gain
                byte[] bytesAlbumGain = reader.ReadBytes(2);
                Int16 albumGain = BitConverter.ToInt16(bytesAlbumGain, 0);

                // Read album peak
                byte[] bytesAlbumPeak = reader.ReadBytes(2);
                Int16 albumPeak = BitConverter.ToInt16(bytesAlbumPeak, 0);

                // Read true gapless / last frame length / fast seeking
                byte[] bytesTrueGapless_LastFrameLength_FastSeeking = reader.ReadBytes(2);

                // Read unused bytes
                reader.ReadBytes(2);

                // Read encoder version
                byte byteEncoderVersion = reader.ReadByte();
              
                // Dispose reader
                reader.Close();
                reader.Dispose();                
                reader = null;
            }
            catch (Exception ex)
            {                
                throw ex;
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
        /// Reads a variable length integer. Reads until the last byte is found. The length can vary between 8-bits to 64-bits.
        /// </summary>
        /// <param name="reader">Binary reader</param>
        /// <param name="intLength">Integer length (1 = 8-bit, 2 = 16-bit, etc.)</param>
        /// <returns>Value</returns>
        public static long ReadVariableLengthInteger(ref BinaryReader reader, ref int intLength)
        {
            long value = 0;
            for (intLength = 1; intLength <= 9; intLength++)
            {
                int currentByte = (int)reader.ReadByte();
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

    /// <summary>
    /// Exception raised when no SV7 tags have been found.
    /// </summary>
    public class SV7TagNotFoundException
        : Exception
    {
        /// <summary>
        /// Default constructor for the SV7TagNotFoundException exception class.
        /// </summary>
        /// <param name="message">Exception message</param>
        public SV7TagNotFoundException(string message)
            : base(message)
        {
        }
    }
}
