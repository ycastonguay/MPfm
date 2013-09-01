﻿// Copyright © 2011-2013 Yanick Castonguay
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

namespace MPfm.Sound.PeakFiles
{
    /// <summary>
    /// This Exception class is raised when the peak file is incompatible.
    /// Related to the PeakFile class.
    /// </summary>
    #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
    [Serializable]
    #endif
    public class PeakFileFormatIncompatibleException
        : Exception
    {
        /// <summary>
        /// Default constructor for the PeakFileFormatIncompatibleException exception class.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public PeakFileFormatIncompatibleException(string message, Exception innerException) 
            : base(message, innerException)
        {

        }
    }
}