//
// UpdateLibraryProgressData.cs: Defines the progress data used with the
//                               UpdateLibrary class. 
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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using MPfm.Sound;

namespace MPfm.Library
{
    /// <summary>
    /// Defines the progress data used with the OnProcessData event.
    /// Also used in the IObservable list. Related to the UpdateLibrary class.
    /// </summary>
    public class UpdateLibraryProgressData
    {
        /// <summary>
        /// Defines the current step in the Update Library process.
        /// </summary>
        public UpdateLibraryStep Step { get; set; }
        /// <summary>
        /// Audio file path to read.
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// Audio file metadata. Null if the process has failed.
        /// </summary>
        public AudioFile AudioFile { get; set; }
        /// <summary>
        /// Exception when the metadata cannot be read. Null when successful.
        /// </summary>
        public UpdateLibraryException Exception { get; set; }
        /// <summary>
        /// Percentage done based on the file list length.
        /// </summary>
        public float PercentageDone { get; set; }
        /// <summary>
        /// Thread number.
        /// </summary>
        public int ThreadNumber { get; set; }

        /// <summary>
        /// Default constructor for the UpdateLibraryProgressData clas.
        /// </summary>
        public UpdateLibraryProgressData()
        {
            Step = 0;
            FilePath = string.Empty;
            AudioFile = null;
            Exception = null;
            PercentageDone = 0;
            ThreadNumber = 0;
        }
    }
}

