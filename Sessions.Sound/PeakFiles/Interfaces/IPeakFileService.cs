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

namespace Sessions.Sound.PeakFiles.Interfaces
{
    public delegate void PeakFileGenerationStarted(PeakFileGenerationStartedData data);
    public delegate void PeakFileGenerationProgress(PeakFileGenerationProgressData data);
    public delegate void PeakFileGenerationFinished(PeakFileGenerationFinishedData data);

    public interface IPeakFileService
    {
        bool IsLoading { get; }
        bool IsCanceling { get; }

        /// <summary>
        /// Defines when the OnProgressData event is called; it will be called
        /// every x blocks (where x is ProgressReportBlockInterval). 
        /// The default value is 20.
        /// </summary>
        uint ProgressReportBlockInterval { get; set; }

        event PeakFileGenerationStarted OnGenerationStarted;
        event PeakFileGenerationProgress OnGenerationProgress;
        event PeakFileGenerationFinished OnGenerationFinished;

        /// <summary>
        /// Generates a peak file for an audio file.
        /// Note: BASS should be initialized already before calling this method. This uses a decode stream.
        /// </summary>
        /// <param name="audioFilePath">Audio file path</param>
        /// <param name="peakFilePath">Peak file path</param>
        void GeneratePeakFile(string audioFilePath, string peakFilePath);
        void Cancel();
        List<WaveDataMinMax> ReadPeakFile(string peakFilePath);
    }

}