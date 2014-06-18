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

using TinyMessenger;

namespace Sessions.MVP.Messages
{
    /// <summary>
    /// Message used to send commands to the player.
    /// </summary>
    public class PlayerCommandMessage : TinyMessageBase
    {
        public PlayerCommandMessageType Command { get; set; }
        
        public PlayerCommandMessage(object sender, PlayerCommandMessageType command) 
            : base(sender)
        {
            this.Command = command;
        }       
    }
    
    public enum PlayerCommandMessageType
    {
        Play = 0,
        Pause = 1,
        Stop = 2,
        PlayPause = 3,
        Previous = 4,
        Next = 5,
        Shuffle = 6,
        Repeat = 7
    }
}
