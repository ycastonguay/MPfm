//
// PlayerCreateStreamException.cs: This Exception class is raised when the 
//                                 player has failed to create a stream.
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
using System.Linq;
using System.Text;

namespace MPfm.Player
{
    /// <summary>
    /// This Exception class is raised when the player has failed to create a stream.
    /// Related to the Player class.
    /// </summary>
    public class PlayerCreateStreamException
        : Exception
    {
        /// <summary>
        /// Indicates what driver type was used to initialize the stream.
        /// </summary>
        public MPfm.Sound.BassNetWrapper.DriverType DriverType { get; set; }
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
            // Set default values
            DriverType = Sound.BassNetWrapper.DriverType.DirectSound;
            SampleRate = 0;
            UseFloatingPoint = false;
            UseTimeShifting = false;
            Decode = false;
        }
    }
}
