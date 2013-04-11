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

using MPfm.MVP.Models;
using TinyMessenger;

namespace MPfm.MVP.Messages
{
    /// <summary>
    /// Message to MobileNavigationManager to request a view change.
    /// This message should be useful when a control wants to display a specific view without bound to a presenter
    /// (ex: UINavigationController on iOS).
    /// </summary>
    public class MobileNavigationManagerCommandMessage : TinyMessageBase
    {
        public MobileNavigationManagerCommandMessageType CommandType { get; set; }

        public MobileNavigationManagerCommandMessage(object sender, MobileNavigationManagerCommandMessageType commandType) 
            : base(sender)
        {
            CommandType = commandType;
        }
    }

    public enum MobileNavigationManagerCommandMessageType
    {
        ShowPlayerView = 0,
        ShowEqualizerPresetsView = 1
    }
}
