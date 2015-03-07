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

using Sessions.Sound.AudioFiles;

namespace Sessions.Sound.Objects
{
    public class PlayerMetadata
    {
        public AudioFile CurrentAudioFile { get; set; }
        public PlayerStatus Status { get; set; }
        public int PlaylistIndex { get; set; }
        public int PlaylistCount { get; set; }
        public string Position { get; set; }
        public string Length { get; set; }

        public enum PlayerStatus
        {
            Stopped = 0, Playing = 1, Paused = 2
        }

        public PlayerMetadata()
        {
            Position = "0:00.000";
            Length = "0:00.000";
        }
    }
}
