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
#if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
using Sessions.Sound.BassNetWrapper;
#endif

namespace Sessions.Player.Exceptions
{
    /// <summary>
    /// This Exception class is raised when the player has failed to create a stream.
    /// Related to the Player class.
    /// </summary>    
    public class PlayerCreateStreamException
        : Exception
    {
        #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
        /// <summary>
        /// Indicates what driver type was used to initialize the stream.
        /// </summary>
        public DriverType DriverType { get; set; }
        #endif
        /// <summary>
        /// Indicates what sample rate was used to initialize the stream.
        /// </summary>
        public int SampleRate { get; set; }
        /// <summary>
        /// Indicates if floating point was used to initialize the stream.
        /// </summary>
        public bool UseFloatingPoint { get; set; }
        /// <summary>
        /// Indicates if time shiting was used to initialize the stream.
        /// </summary>
        public bool UseTimeShifting { get; set; }
        /// <summary>
        /// Indicates if the stream is a decoding stream.
        /// </summary>
        public bool Decode { get; set; }

        /// <summary>
        /// Default constructor for the PlayerCreateStreamException exception class.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// /// <param name="innerException">Inner exception</param>
        public PlayerCreateStreamException(string message, Exception innerException)
            : base(message, innerException)
        {
            #if !PCL && !WINDOWSSTORE && !WINDOWS_PHONE
            DriverType = DriverType.DirectSound;
            #endif
            SampleRate = 0;
            UseFloatingPoint = false;
            UseTimeShifting = false;
            Decode = false;
        }
    }
}
