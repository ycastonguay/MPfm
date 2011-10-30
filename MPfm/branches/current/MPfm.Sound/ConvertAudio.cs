//
// ConvertAudioPosition.cs: Static class for converting audio position values.
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
using System.Text;

namespace MPfm.Sound
{
    /// <summary>
    /// This static class contains methods for converting audio position/length values into different formats.
    /// </summary>
    public static class ConvertAudio
    {
        /// <summary>
        /// Converts to the PCM (samples) format.
        /// </summary>
        /// <param name="ms">Total number of milliseconds</param>
        /// <param name="sampleRate">Sample rate</param>
        /// <returns>PCM (samples)</returns>
        public static uint ToPCM(uint ms, uint sampleRate)
        {
            return ms * (sampleRate / 1000);
        }

        public static long ToPCM(long bytes, uint bitRate, int channelCount)
        {
            return bytes * 8 / bitRate / channelCount;
        }

        /// <summary>
        /// Converts to the milliseconds format.
        /// </summary>
        /// <param name="samples">Total number of samples</param>
        /// <param name="sampleRate">Sample rate</param>
        /// <returns>Milliseconds</returns>
        public static long ToMS(long samples, uint sampleRate)
        {
            return samples * 1000 / sampleRate;
        }

        /// <summary>
        /// Converts to the milliseconds format.
        /// </summary>
        /// <param name="position">Position in string (format: 0:00.000)</param>
        /// <returns>Milliseconds</returns>
        public static uint ToMS(string position)
        {
            string[] positionSplit = position.Split(':');
            string[] positionSplit2 = positionSplit[1].Split('.');

            uint minutes = 0;
            uint.TryParse(positionSplit[0], out minutes);

            uint seconds = 0;
            uint.TryParse(positionSplit2[0], out seconds);

            uint milliseconds = 0;
            uint.TryParse(positionSplit2[1], out milliseconds);

            return milliseconds + (seconds * 1000) + (minutes * 1000 * 60);
        }

        /// <summary>
        /// Converts to the PCM (bytes) format.
        /// </summary>
        /// <param name="samples">Total number of samples</param>
        /// <param name="bitRate">Bit rate (ex: 16-bit)</param>
        /// <param name="channelCount">Channel count (mono: 1, stereo: 2)</param>
        /// <returns></returns>
        public static uint ToPCMBytes(uint samples, uint bitRate, uint channelCount)
        {
            return samples * bitRate * channelCount / 8;
        }
    }
}
