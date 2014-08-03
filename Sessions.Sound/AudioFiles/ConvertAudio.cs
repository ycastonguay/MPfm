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
using Sessions.Core;

namespace Sessions.Sound.AudioFiles
{
    /// <summary>
    /// This static class contains methods for converting audio position/length values into different formats.
    /// </summary>
    public static class ConvertAudio
    {
        /// <summary>
        /// Converts a position in bytes to time string. Requires several parameters to work.
        /// </summary>
        /// <param name="positionBytes">Position (in bytes)</param>
        /// <param name="bitsPerSample">Bits per sample (ex: 16 for 16-bit)</param>
        /// <param name="channelCount">Channel count (ex: 2 for stereo)</param>
        /// <param name="sampleRate">Sample rate (ex: 44100 for 44100Hz)</param>
        /// <returns>Position in time string (00:00.000)</returns>
        public static string ToTimeString(long positionBytes, int bitsPerSample, int channelCount, int sampleRate)
        {
            long positionSamples = ToPCM(positionBytes, bitsPerSample, channelCount);
            long positionMS = ToMS(positionSamples, sampleRate);
            return ToTimeString(positionMS);
        }

        /// <summary>
        /// Converts a TimeSpan to a time string.
        /// </summary>
        public static string ToTimeString(TimeSpan timeSpan)
        {
            return ToTimeString((long)timeSpan.TotalMilliseconds);
        }
        
        /// <summary>
        /// Converts milliseconds to time string.
        /// </summary>
        public static string ToTimeString(long ms)
        {
            long pos = ms;
            int hours = 0;
            int minutes = 0;
            int seconds = 0;
            int milliseconds = 0;
            string timeString = "";

            if (pos >= 3600000)
            {
                hours = (int)pos / 3600000;
                pos %= 3600000;
            }
            if (pos >= 60000)
            {
                minutes = (int)pos / 60000;
                pos %= 60000;
            }
            if (pos >= 1000)
            {
                seconds = (int)pos / 1000;
                pos %= 1000;
            }
            milliseconds = Convert.ToInt32(pos);

            if (hours > 0)
                timeString = hours.ToString("0") + ":" + minutes.ToString("00");
            else
                timeString = minutes.ToString("0");
            timeString += ":" + seconds.ToString("00") + "." + milliseconds.ToString("000");
            return timeString;
        }

        /// <summary>
        /// Converts to the PCM (samples) format.
        /// </summary>
        /// <param name="ms">Total number of milliseconds</param>
        /// <param name="sampleRate">Sample rate</param>
        /// <returns>PCM (samples)</returns>
        public static int ToPCM(int ms, int sampleRate)
        {
            return ms * (sampleRate / 1000);
        }

        /// <summary>
        /// Converts to the PCM (samples) format.
        /// </summary>
        /// <param name="bytes">Value (in bytes)</param>
        /// <param name="bitsPerSample">Bits per sample (ex: 16-bit)</param>
        /// <param name="channelCount">Number of channels</param>
        /// <returns>PCM (samples)</returns>
        public static long ToPCM(long bytes, int bitsPerSample, int channelCount)
        {
            return bytes * 8 / bitsPerSample / channelCount;
        }

        /// <summary>
        /// Converts to the milliseconds format.
        /// </summary>
        /// <param name="samples">Total number of samples</param>
        /// <param name="sampleRate">Sample rate</param>
        /// <returns>Milliseconds</returns>
        public static long ToMS(long samples, int sampleRate)
        {
            return samples * 1000 / sampleRate;
        }

        /// <summary>
        /// Converts to the milliseconds format.
        /// </summary>
        /// <param name="position">Position in string (format: 0:00.000)</param>
        /// <returns>Milliseconds</returns>
        public static int ToMS(string position)
        {
            string strHours = String.Empty;
            string strMinutes = String.Empty;
            string strSeconds = String.Empty;
            string strMilliseconds = String.Empty;

            string[] positionSplit = position.Split(':');
            if (positionSplit.Length == 2)
            {
                string[] positionSplit2 = positionSplit[1].Split('.');
                strMinutes = positionSplit[0];
                strSeconds = positionSplit2[0];
                strMilliseconds = positionSplit2[1];
            }
            if (positionSplit.Length == 3)
            {
                string[] positionSplit2 = positionSplit[2].Split('.');
                strHours = positionSplit[0];
                strMinutes = positionSplit[1];
                strSeconds = positionSplit2[0];
                strMilliseconds = positionSplit2[1];
            }

            int hours = 0;
            Int32.TryParse(strHours, out hours);

            int minutes = 0;
            Int32.TryParse(strMinutes, out minutes);

            int seconds = 0;
            Int32.TryParse(strSeconds, out seconds);

            int milliseconds = 0;
            Int32.TryParse(strMilliseconds, out milliseconds);

            return milliseconds + (seconds * 1000) + (minutes * 1000 * 60) + (hours * 1000 * 60 * 60);
        }

        /// <summary>
        /// Converts to the PCM (bytes) format.
        /// </summary>
        /// <param name="samples">Total number of samples</param>
        /// <param name="bitRate">Bit rate (ex: 16-bit)</param>
        /// <param name="channelCount">Channel count (mono: 1, stereo: 2)</param>
        /// <returns></returns>
        public static int ToPCMBytes(int samples, int bitRate, int channelCount)
        {
            return samples * bitRate * channelCount / 8;
        }

        /// <summary>
        /// Converts milliseconds to a tempo (beats per minute)
        /// </summary>
        /// <param name="milliseconds">Value in milliseconds</param>
        /// <returns>Value in tempo (BPM)</returns>
        public static double MillisecondsToTempo(int milliseconds)
        {
            return ((double)60 / milliseconds) * (double)1000;
        }

        /// <summary>
        /// Converts a tempo (beats per minute) into milliseconds.
        /// </summary>
        /// <param name="tempo">Value in BPM (tempo)</param>
        /// <returns>Value in milliseconds</returns>
        public static double TempoToMilliseconds(double tempo)
        {
            return (double)60000 / tempo;
        }

        public static double LevelToDB(int level, int maxLevel)
        {
            return 20.0 * Math.Log10((double)level / (double)maxLevel);
        }

        public static double LevelToDB(double level, double maxLevel)
        {
            return 20.0 * Math.Log10(level / maxLevel);
        }
    }
}
