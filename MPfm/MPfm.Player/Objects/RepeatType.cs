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

namespace MPfm.Player.Objects
{
    /// <summary>
    /// Defines the repeat types (off, playlist or song).
    /// </summary>
    public enum RepeatType
    {
        /// <summary>
        /// No repeat.
        /// </summary>
        Off = 0, 
        /// <summary>
        /// Playlist repeat type. The playback returns to the first
        /// item of the playlist when the playlist has finished playing.
        /// </summary>
        Playlist = 1, 
        /// <summary>
        /// Song repeat type. When the current song finishes, the playback returns
        /// to the song start position.
        /// </summary>
        Song = 2
    }
}
