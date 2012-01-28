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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPfm.Sound
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
        TTA = 10
	}
}

