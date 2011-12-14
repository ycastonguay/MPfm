﻿//
// UpdateLibraryException.cs: Exception used with the UpdateLibrary class.
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
    /// This Exception class is raised when the audio file metadata scanning has failed.
    /// Related to the UpdateLibrary class.
    /// </summary>
    public class UpdateLibraryException
        : Exception
    {
        /// <summary>
        /// Default constructor for the UpdateLibraryException exception class.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// /// <param name="innerException">Inner exception</param>
        public UpdateLibraryException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
