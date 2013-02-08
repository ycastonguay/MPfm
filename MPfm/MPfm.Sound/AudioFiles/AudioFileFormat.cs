//
// AudioFileFormat.cs: Defines the supported audio file formats for MPfm.
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

namespace MPfm.Sound.AudioFiles
{
	/// <summary>
    /// Defines the supported audio file formats for MPfm.
	/// </summary>
	public enum AudioFileFormat
	{
        /// <summary>
        /// Every audio file format (useful for library queries).
        /// </summary>
		All = 0, 
        /// <summary>
        /// Unknown file format.
        /// </summary>
        Unknown = 1,
        /// <summary>
        /// FLAC lossless audio codec.
        /// For more information, consult http://flac.sourceforge.net/.
        /// </summary>
        FLAC = 2, 
        /// <summary>
        /// WAV uncompressed audio codec.
        /// For more information, consult http://en.wikipedia.org/wiki/WAV.
        /// </summary>
        WAV = 3, 
        /// <summary>
        /// MP3 lossy audio codec. Requires royalty payments for commercial applications.
        /// For more information, consult http://mp3licensing.com/.
        /// </summary>
        MP3 = 4, 
        /// <summary>
        /// OGG Vorbis lossy audio codec.
        /// For more information. consult http://www.vorbis.com/.
        /// </summary>
        OGG = 5,
        /// <summary>
        /// APE (Monkey's Audio) lossless audio codec.
        /// For more information, consult http://www.monkeysaudio.com/.
        /// </summary>
        APE = 6, 
        /// <summary>
        /// MPC (MusePack) lossy audio codec.
        /// For more information, consult http://www.musepack.net/.
        /// </summary>
        MPC = 7, 
        /// <summary>
        /// WV (WavPack) lossless audio codec.
        /// For more information, consult http://www.wavpack.com/.
        /// </summary>
        WV = 8, 
        /// <summary>
        /// OFR (OptimFROG) lossless audio codec.
        /// For more information, consult http://www.losslessaudio.org/.
        /// </summary>
        OFR = 9, 
        /// <summary>
        /// TTA (True Audio) lossy audio codec.
        /// For more information, consult http://en.true-audio.com/.
        /// </summary>
        TTA = 10,
        /// <summary>
        /// WMA (Windows Media Audio) audio codec. Also called ASF.
        /// For more information, consult http://en.wikipedia.org/wiki/Windows_Media_Audio.
        /// </summary>
        WMA = 11,
        /// <summary>
        /// AAC (Advanced Audio Coding) audio codec. Has different extensions (M4A, MP4, AAC).
        /// For more information, consult http://en.wikipedia.org/wiki/Advanced_Audio_Coding.
        /// </summary>
        AAC = 12,
        /// <summary>
        /// ALAC (Apple Lossless Audio Codec) audio codec. Also sometimes called ALE (Apple Lossless Encoder). Has the M4A extension.
        /// For more information, consult http://en.wikipedia.org/wiki/Apple_Lossless.
        /// </summary>
        ALAC = 13
	}
}

