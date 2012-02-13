//
// UpdateLibraryStep.cs: Defines the current step in the Update Library process.
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
    /// Defines the current step in the Update Library process.
    /// </summary>
    public enum UpdateLibraryStep
    {
        /// <summary>
        /// Step #1: Search for broken file paths.
        /// </summary>
        SearchForBrokenFilePaths = 0,
        /// <summary>
        /// Step #2: Search for new audio files.
        /// </summary>
        SearchForNewFiles = 1,
        /// <summary>
        /// Step #3: Import new audio files to the library.
        /// </summary>
        ImportNewFiles = 2,
        /// <summary>
        /// Step #4: Compact database.
        /// </summary>
        CompactDatabase = 3,
        /// <summary>
        /// Step #5: Refresh library cache.
        /// </summary>
        RefreshLibraryCache = 4
    }
}

