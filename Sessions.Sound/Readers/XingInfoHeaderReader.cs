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

using System.IO;
using System.Text;

namespace Sessions.Sound.Readers
{
    /// <summary>
    /// Reads the Xing/Info header of a MP3 file to extract information.
    /// Xing/Info header specifications: http://gabriel.mp3-tech.org/mp3infotag.html
    /// </summary>
    public static class XingInfoHeaderReader
    {
        /// <summary>
        /// Reads the first frame of a MP3 file using the start position.
        /// The start position can be retrieved using TagLib (see the TagLib.Mpeg.AudioFile.InvariantStartPosition property).
        /// </summary>
        /// <param name="filePath">MP3 file path</param>
        /// <param name="startPosition">Start position</param>
        /// <returns>Xing/Info header data structure</returns>
        public static XingInfoHeaderData ReadXingInfoHeader(string filePath, long startPosition)
        {
            XingInfoHeaderData data = new XingInfoHeaderData();

            #if WINDOWSSTORE
            #else
            BinaryReader reader = null;
            FileStream stream = null;

            try
            {
                // Open binary reader
                stream = File.OpenRead(filePath);
                reader = new BinaryReader(stream);

                // Seek to first frame
                reader.BaseStream.Seek(startPosition, SeekOrigin.Begin);

                // Read the MP3 header
                byte[] byteHeader = reader.ReadBytes(4);

                // Detect the start of a MP3 frame
                if (byteHeader[0] != 0xFF && byteHeader[1] != 0xFB)
                {
                    // Set flag
                    data.Status = XingInfoHeaderStatus.StartPositionNotValidHeader;
                    return data;
                }

                // Great, we found a header!
                // Seek 20 bytes to check if the "Xing" or "Info" text is present
                reader.BaseStream.Seek(32, SeekOrigin.Current);

                // Get data for "Xing"/"Info"
                byte[] byteXingInfo = reader.ReadBytes(4);
                data.HeaderType = Encoding.UTF8.GetString(byteXingInfo, 0, byteXingInfo.Length).ToUpper();

                // Is this a Xing header?
                if (data.HeaderType != "XING" && data.HeaderType != "INFO")
                {
                    // Set flag
                    data.Status = XingInfoHeaderStatus.HeaderNotFound;
                    return data;
                }

                // The Xing header is 120 bytes. seek 116 bytes (4 bytes less because of the 4 byte string)
                reader.BaseStream.Seek(116, SeekOrigin.Current);

                // Get encoder name / encoder version
                byte[] byteEncoderVersion = reader.ReadBytes(9);
                data.EncoderVersion = CleanByteArrayForString(byteEncoderVersion);

                // Get info tag (1 byte)
                byte byteInfoTag = reader.ReadByte();

                // Get low pass filter value (1 byte)
                byte byteLowPassFilterValue = reader.ReadByte();

                // Get replay gain (8 bytes)
                byte[] byteReplayGain = reader.ReadBytes(8);

                // Get encoding flags (1 byte)
                byte byteEncodingFlags = reader.ReadByte();

                // Get specified bitrate / minimal bitrate (1 byte)
                byte byteSpecifiedBitRateMinimalBitrate = reader.ReadByte();

                // Get encoder delay / padding (2x 12-bit over 3 bytes)
                byte[] byteEncoderDelays = reader.ReadBytes(3);
                data.EncoderDelay = byteEncoderDelays[0] << 4;
                data.EncoderDelay += byteEncoderDelays[1] >> 4;
                data.EncoderPadding = (byteEncoderDelays[1] & 0x0F) << 8;
                data.EncoderPadding += byteEncoderDelays[2];

                // Set status as successful
                data.Status = XingInfoHeaderStatus.Successful;
            }
            catch
            {
                throw;
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
        /// Cleans a byte array and converts it into a string (UTF-8)
        /// </summary>
        /// <param name="byteText">Byte array containing text</param>
        /// <returns>String (UTF-8)</returns>
        public static string CleanByteArrayForString(byte[] byteText)
        {
            string text = string.Empty;

            // Loop through text
            for (int a = 0; a < byteText.Length; a++)
            {
                // Check if the string has invalid characters
                if (byteText[a] < 0x20 ||
                   byteText[a] > 0x7E)
                {
                    // Replace by question mark
                    byteText[a] = 0x3f;
                }
            }

            // Convert string
            return Encoding.UTF8.GetString(byteText, 0, byteText.Length);
        }
    }

    /// <summary>
    /// Data structure containing information from a Xing/Info header.
    /// </summary>
    public class XingInfoHeaderData
    {
        /// <summary>
        /// Indicates if the Xing/Info header was loaded successfully.
        /// </summary>
        public XingInfoHeaderStatus Status { get; set; }

        /// <summary>
        /// Indicates the type of header. 
        /// The XING header is found on MP3 files encoded using LAME and VBR/ABR settings.
        /// The INFO header is found on MP3 files encoded using LAME and CBR settings.
        /// Both headers are in fact the same.
        /// </summary>
        public string HeaderType { get; set; }

        /// <summary>
        /// Encoder version.
        /// Ex: LAME3.98
        /// </summary>
        public string EncoderVersion { get; set; }

        /// <summary>
        /// Encoder delay.
        /// Ex: 576
        /// </summary>
        public int? EncoderDelay { get; set; }

        /// <summary>
        /// Encoder padding.
        /// Ex: 1800
        /// </summary>
        public int? EncoderPadding { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public XingInfoHeaderData()
        {
            EncoderDelay = null;
            EncoderPadding = null;
            Status = XingInfoHeaderStatus.None;
        }
    }

    /// <summary>
    /// List of status labels used with XingInfoHeaderData to 
    /// indicate if the Xing/Info header was loaded successfully.
    /// </summary>
    public enum XingInfoHeaderStatus
    {
        /// <summary>
        /// No status.
        /// </summary>
        None = 0,
        /// <summary>
        /// Indicates that a Xing/Info header was found.
        /// </summary>
        Successful = 1, 
        /// <summary>
        /// Indicates that no header was found.
        /// </summary>
        HeaderNotFound = 2,
        /// <summary>
        /// Indicates that a valid header could not be found at the start position.
        /// </summary>
        StartPositionNotValidHeader = 3
    }
}
