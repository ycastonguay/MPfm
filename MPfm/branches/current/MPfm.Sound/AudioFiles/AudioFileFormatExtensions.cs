//
// AudioFileFormatExtensions.cs: List of audio file formats and their file extensions.
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

using System.Collections.Generic;
using System.Linq;

namespace MPfm.Sound.AudioFiles
{
    /// <summary>
    /// List of audio file formats and their file extensions.
    /// </summary>
    public static class AudioFileFormatExtensions
    {
        /// <summary>
        /// MP3 audio file format.
        /// </summary>
        public static AudioFileFormatExtension MP3 = new AudioFileFormatExtension(AudioFileFormat.MP3, new List<string> { ".MP3" });

        /// <summary>
        /// WAV/RIFF audio file format.
        /// </summary>
        public static AudioFileFormatExtension WAV = new AudioFileFormatExtension(AudioFileFormat.WAV, new List<string> { ".WAV", ".RIFF" });

        /// <summary>
        /// FLAC audio file format.
        /// </summary>
        public static AudioFileFormatExtension FLAC = new AudioFileFormatExtension(AudioFileFormat.FLAC, new List<string> { ".FLAC" });

        /// <summary>
        /// OGG audio file format.
        /// </summary>
        public static AudioFileFormatExtension OGG = new AudioFileFormatExtension(AudioFileFormat.OGG, new List<string> { ".OGG" });

        /// <summary>
        /// MPC (MusePack) audio file format.
        /// </summary>
        public static AudioFileFormatExtension MPC = new AudioFileFormatExtension(AudioFileFormat.MPC, new List<string> { ".MPC" });

        /// <summary>
        /// WV (WavPack) audio file format.
        /// </summary>
        public static AudioFileFormatExtension WV = new AudioFileFormatExtension(AudioFileFormat.WV, new List<string> { ".WV" });

        /// <summary>
        /// APE (Monkey's Audio) audio file format.
        /// </summary>
        public static AudioFileFormatExtension APE = new AudioFileFormatExtension(AudioFileFormat.APE, new List<string> { ".APE" });

        /// <summary>
        /// OFR (OptimFROG) audio file format.
        /// </summary>
        public static AudioFileFormatExtension OFR = new AudioFileFormatExtension(AudioFileFormat.OFR, new List<string> { ".OFR" });

        /// <summary>
        /// TTA (TrueAudio) audio file format.
        /// </summary>
        public static AudioFileFormatExtension TTA = new AudioFileFormatExtension(AudioFileFormat.TTA, new List<string> { ".TTA" });

        /// <summary>
        /// WMA (Windows Media Audio) audio file format.
        /// </summary>
        public static AudioFileFormatExtension WMA = new AudioFileFormatExtension(AudioFileFormat.WMA, new List<string> { ".WMA" });

        /// <summary>
        /// AAC (Advanced Audio Codec) audio file format.
        /// </summary>
        public static AudioFileFormatExtension AAC = new AudioFileFormatExtension(AudioFileFormat.AAC, new List<string> { ".AAC", ".M4A", ".MP4" });

        /// <summary>
        /// ALAC (Apple Lossless Audio Codec) audio file format.
        /// </summary>
        public static AudioFileFormatExtension ALAC = new AudioFileFormatExtension(AudioFileFormat.ALAC, new List<string> { ".M4A" });

        /// <summary>
        /// Returns the complete list of file extensions.
        /// </summary>
        /// <returns>List of extensions (including the dot character)</returns>
        public static List<string> ToAll()
        {
            // Create list
            List<string> extensions = new List<string>();

            // Add extensions
            extensions.AddRange(MP3.Extensions);
            extensions.AddRange(WAV.Extensions);
            extensions.AddRange(FLAC.Extensions);
            extensions.AddRange(OGG.Extensions);
            extensions.AddRange(MPC.Extensions);
            extensions.AddRange(WV.Extensions);
            extensions.AddRange(APE.Extensions);
            extensions.AddRange(OFR.Extensions);
            extensions.AddRange(TTA.Extensions);
            extensions.AddRange(WMA.Extensions);
            extensions.AddRange(AAC.Extensions);
            extensions.AddRange(ALAC.Extensions);

            // Make sure each item of the list is unique
            extensions = extensions.Distinct().ToList();

            return extensions;
        }

    }
}

