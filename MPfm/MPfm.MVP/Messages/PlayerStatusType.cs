// Copyright © 2011-2013 Yanick Castonguay
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

namespace MPfm.MVP.Messages
{
    /// <summary>
    /// Enumeration of player status types.
    /// </summary>
    public enum PlayerStatusType
    {
        Initialized = 0,
        Stopped = 1,
        Playing = 2,
        Paused = 3,
        /// <summary>
        /// Used when starting playback as paused. 
        /// This status tells the presenters to start playback when it will be ready to start updating the UI.
        /// </summary>
        WaitingToStart = 4,
        /// <summary>
        /// Used to start the player but pause the playback immediately.
        /// Useful when the application starts and resumes the previous sessions; we don't want to force playback on the user.
        /// </summary>
        StartPaused = 5
    }
}
