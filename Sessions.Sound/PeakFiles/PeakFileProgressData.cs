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

using System.Collections.Generic;
using Sessions.Sound.AudioFiles;

namespace Sessions.Sound.PeakFiles
{
    /// <summary>
    /// Defines the progress data used with the OnProcessData event.
    /// </summary>
    public class PeakFileProgressData
    {
        /// <summary>
        /// Defines the audio file path to analyse in order to generate the peak file.
        /// </summary>
        public string AudioFilePath { get; set; }
        /// <summary>
        /// Defines the peak file path.
        /// </summary>
        public string PeakFilePath { get; set; }
        /// <summary>
        /// Defines the thread number currently reporting.
        /// </summary>
        public int ThreadNumber { get; set; }
        /// <summary>
        /// Defines the current thread progress in percentage.
        /// </summary>
        public float PercentageDone { get; set; }
        /// <summary>
        /// Defines the audio file path length in bytes.
        /// </summary>
        public long Length { get; set; }
        /// <summary>
        /// Defines the current block to read.
        /// </summary>
        public int CurrentBlock { get; set; }
        /// <summary>
        /// Defines the total number of blocks to read.
        /// </summary>
        public int TotalBlocks { get; set; }

        /// <summary>
        /// Defines the list of min/max wave data values for waveform.
        /// Useful for displaying the waveform generation in real-time.
        /// </summary>
        public List<WaveDataMinMax> MinMax { get; set; }
    }
}