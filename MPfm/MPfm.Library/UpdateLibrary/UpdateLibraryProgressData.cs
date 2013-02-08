//
// UpdateLibraryProgressData.cs: Defines the data structure for the 
//                               Update Library background process progress event.
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

namespace MPfm.Library.UpdateLibrary
{
    /// <summary>
    /// Defines the data structure for the Update Library background process progress event.
    /// </summary>
    public class UpdateLibraryProgressData
    {
        /// <summary>
        /// Title to display.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Message to display.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Percentage done.
        /// </summary>
        public double Percentage { get; set; }
        /// <summary>
        /// Total number of files to process.
        /// </summary>
        public int TotalNumberOfFiles { get; set; }
        /// <summary>
        /// Current file (total can be found in the TotalNumberOfFiles property).
        /// </summary>
        public int CurrentFilePosition { get; set; }
        /// <summary>
        /// Audio file path.
        /// </summary>
        public string FilePath { get; set; }               
        /// <summary>
        /// Log entry.
        /// </summary>
        public string LogEntry { get; set; }
        /// <summary>
        /// UpdateLibraryProgressDataSong data structure.
        /// </summary>
        public UpdateLibraryProgressDataSong Song { get; set; }
        /// <summary>
        /// Exception related to processing this file.
        /// </summary>
        public Exception Error;
    }
}
