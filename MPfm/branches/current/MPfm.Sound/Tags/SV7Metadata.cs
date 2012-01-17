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
using MPfm.Core;

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

                // Get file length
                long fileLength = reader.BaseStream.Length;
                
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
                int streamVersionMinor = ((int)byteStreamMajorMinorVersion >> 4) & 0x0F;
                int streamVersionMajor = ((int)byteStreamMajorMinorVersion & 0x0F);

                // Validate version
                if (streamVersionMajor != 7)
                {
                    throw new SV7TagNotFoundException("This file header version is not SV7!");
                }
                
                // Read frame count (32-bits)
                byte[] bytesFrameCount = reader.ReadBytes(4);
                data.FrameCount = BitConverter.ToInt32(bytesFrameCount, 0);

                // Read Max level
                byte[] bytesMaxLevel = reader.ReadBytes(2);
                data.MaxLevel = BitConverter.ToInt16(bytesMaxLevel, 0);

                // Read Profile / Link / SampleFreq
                byte byteProfile_Link_SampleFreq = reader.ReadByte();
                data.EncoderProfile = ((int)byteProfile_Link_SampleFreq & 0xF0) >> 4;
                int link = ((int)byteProfile_Link_SampleFreq & 0x0C) >> 2;
                int sampleFrequency = ((int)byteProfile_Link_SampleFreq & 0x03) >> 0;

                // Read Intensity stereo / Mid side stereo / Max band
                byte byteIntensityStereo_MidSideStereo_MaxBand = reader.ReadByte();
                int intensityStereo = ((int)byteIntensityStereo_MidSideStereo_MaxBand & 0x80) >> 7;
                int midSideStereo = ((int)byteIntensityStereo_MidSideStereo_MaxBand & 0x40) >> 6;
                data.MaxBand = ((int)byteIntensityStereo_MidSideStereo_MaxBand & 0x3F) >> 2;

                // Read title peak
                byte[] bytesTitlePeak = reader.ReadBytes(2);
                data.TitlePeak = BitConverter.ToInt16(bytesTitlePeak, 0);

                // Read title gain
                byte[] bytesTitleGain = reader.ReadBytes(2);
                data.TitleGain = BitConverter.ToInt16(bytesTitleGain, 0);

                // Read album peak
                byte[] bytesAlbumPeak = reader.ReadBytes(2);
                data.AlbumPeak = BitConverter.ToInt16(bytesAlbumPeak, 0);

                // Read album gain
                byte[] bytesAlbumGain = reader.ReadBytes(2);
                data.AlbumGain = BitConverter.ToInt16(bytesAlbumGain, 0);

                // Read unused bytes
                reader.ReadBytes(2);

                // Read true gapless / last frame length / fast seeking
                byte[] bytesTrueGapless_LastFrameLength_FastSeeking = reader.ReadBytes(2);
                int trueGapless = ((int)bytesTrueGapless_LastFrameLength_FastSeeking[0] & 0x80) >> 7;
                data.LastFrameLength = ((int)bytesTrueGapless_LastFrameLength_FastSeeking[0] & 0x80) >> 7;                

                // Get last frame length
                int lastFrameLength = BitConverter.ToInt16(bytesTrueGapless_LastFrameLength_FastSeeking, 0);
                data.LastFrameLength = (lastFrameLength & 0x7FF0) >> 4;

                // Read unused bytes
                reader.ReadBytes(3);                

                // Read encoder version
                byte byteEncoderVersion = reader.ReadByte();
                string encoderVersion = "mppenc ";

                // Check for beta
                if (byteEncoderVersion % 2 == 0)
                {
                    encoderVersion += "Beta ";
                }
                // Check for alpha
                else if (byteEncoderVersion % 2 == 1)
                {
                    encoderVersion += "Alpha ";
                }

                // Complete string with version
                encoderVersion += ((double)byteEncoderVersion / 100).ToString("0.00");
              
                // Dispose reader
                reader.Close();
                reader.Dispose();                
                reader = null;

                // Set metadata
                data.EncoderVersion = encoderVersion;
                data.IntensityStereo = (intensityStereo == 1) ? true : false;
                data.MidSideStereoEnabled = (midSideStereo == 1) ? true : false;
                data.TrueGapless = (trueGapless == 1) ? true : false;

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

                // Calculate length
                data.LengthSamples = ((((data.FrameCount - 1) * 1152) + data.LastFrameLength) * data.AudioChannels) / 2; // floating point
                data.LengthMS = ConvertAudio.ToMS(data.LengthSamples, (uint)data.SampleRate);
                data.Length = Conversion.MillisecondsToTimeString((ulong)data.LengthMS);
                long audioLengthBytes = fileLength - 28; // SV7 header is always 28 bytes
                data.Bitrate = (int)(audioLengthBytes / data.LengthMS) * 8;
            }
            catch (Exception ex)
            {                
                throw ex;
            }

            return data;
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
