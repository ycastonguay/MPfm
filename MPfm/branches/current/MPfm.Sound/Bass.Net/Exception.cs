//
// Exception.cs: This file contains the Exception class which is part of the
//               BASS.NET wrapper.
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
using System.Linq;
using System.Text;

namespace MPfm.Sound.BassNetWrapper
{
    /// <summary>
    /// Custom exception class for the MPfm BASS.NET wrapper.
    /// </summary>
    public class BassNetWrapperException : Exception
    {
        /// <summary>
        /// Default constructor for the default exception class of the MPfm BASS.NET wrapper.
        /// </summary>
        /// <param name="message">Exception message</param>
        public BassNetWrapperException(string message) 
            : base("An error has occured in Bass.Net: " + message)
        {            
        }
    }
}
