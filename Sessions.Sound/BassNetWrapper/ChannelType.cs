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

namespace Sessions.Sound.BassNetWrapper
{
    /// <summary>
    /// Defines the types of channels found in the BASS.NET library.
    /// </summary>
    public enum ChannelType
    {
        /// <summary>
        /// Playback channel (outputs to the sound card direcly).
        /// </summary>
        Playback = 0, 
        /// <summary>
        /// FX channel (can output to the sound card directly or be a decode channel).
        /// </summary>
        FX = 1, 
        /// <summary>
        /// Decode channel.
        /// </summary>
        Decode = 2, 
        /// <summary>
        /// Memory channel.
        /// </summary>
        Memory = 3
    }
}
