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
    /// Message used to notify the app that it should add a lock screen activity (only used on Android).
    /// </summary>
    public class ActivateLockScreenMessage : TinyMessageBase
    {
        public bool ActivateLockScreen { get; set; }

        public ActivateLockScreenMessage(object sender, bool activateLockScreen) 
            : base(sender)
        {
            this.ActivateLockScreen = activateLockScreen;
        }       
    }
}
