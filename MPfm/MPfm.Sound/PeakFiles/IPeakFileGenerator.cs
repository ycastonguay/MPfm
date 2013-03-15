// Copyright � 2011-2013 Yanick Castonguay
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
using MPfm.Sound.AudioFiles;

namespace MPfm.Sound.PeakFiles
{
    public interface IPeakFileGenerator
    {
        /// <summary>
        /// Indicates if a peak file is generating.
        /// </summary>
        bool IsLoading { get; }

        /// <summary>
        /// Indicates if the class is currently generating peak files.
        /// </summary>
        bool IsProcessing { get; }

        /// <summary>
        /// Defines when the OnProgressData event is called; it will be called
        /// every x blocks (where x is ProgressReportBlockInterval). 
        /// The default value is 20.
        /// </summary>
        int ProgressReportBlockInterval { get; set; }

        /// <summary>
        /// Event called when a thread starts its work.
        /// </summary>
        event PeakFileGenerator.ProcessStarted OnProcessStarted;

        /// <summary>
        /// Event called every 20 blocks when generating a peak file.
        /// </summary>
        event PeakFileGenerator.ProcessData OnProcessData;

        /// <summary>
        /// Event called when all the GeneratePeakFiles threads have completed their work.
        /// </summary>
        event PeakFileGenerator.ProcessDone OnProcessDone;

        /// <summary>
        /// Generates a peak file for an audio file.
        /// Note: BASS should be initialized already before calling this method. This uses a decode stream.
        /// </summary>
        /// <param name="audioFilePath">Audio file path</param>
        /// <param name="peakFilePath">Peak file path</param>
        void GeneratePeakFile(string audioFilePath, string peakFilePath);

        /// <summary>
        /// Cancels the peak file generation.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Reads a peak file and returns a min/max peak list.
        /// </summary>
        /// <param name="peakFilePath">Peak file path</param>
        /// <returns>List of min/max peaks</returns>
        List<WaveDataMinMax> ReadPeakFile(string peakFilePath);
    }

}