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
using System.Collections.Generic;
using CoreGraphics;
using System.Linq;
using Foundation;
using UIKit;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;
using TinyMessenger;
using Sessions.MVP.Messages;

namespace Sessions.iOS.Classes.Controls
{
    [Register("SessionsWindow")]
    public class SessionsWindow : UIWindow
    {
        private readonly ITinyMessengerHub _messengerHub;

        public SessionsWindow(CGRect frame) : base(frame)
        {
            _messengerHub = Bootstrapper.GetContainer().Resolve<ITinyMessengerHub>();
        }

        public override void RemoteControlReceived(UIEvent theEvent)
        {
            // Check for remote control event
            if (theEvent.Type == UIEventType.RemoteControl)
            {
                switch(theEvent.Subtype)
                {
                    case UIEventSubtype.RemoteControlTogglePlayPause:
                        Console.WriteLine("RemoteControlTogglePlayPause");
                        _messengerHub.PublishAsync(new PlayerCommandMessage(this, PlayerCommandMessageType.PlayPause));                       
                        break;
                    case UIEventSubtype.RemoteControlPreviousTrack:
                        Console.WriteLine("RemoteControlPreviousTrack");
                        _messengerHub.PublishAsync(new PlayerCommandMessage(this, PlayerCommandMessageType.Previous));
                        break;
                    case UIEventSubtype.RemoteControlNextTrack:
                        Console.WriteLine("RemoteControlNextTrack");
                        _messengerHub.PublishAsync(new PlayerCommandMessage(this, PlayerCommandMessageType.Next));
                        break;
                    case UIEventSubtype.RemoteControlPlay:
                        Console.WriteLine("RemoteControlPlay");
                        _messengerHub.PublishAsync(new PlayerCommandMessage(this, PlayerCommandMessageType.Play));
                        break;
                    case UIEventSubtype.RemoteControlPause:
                        Console.WriteLine("RemoteControlPause");
                        _messengerHub.PublishAsync(new PlayerCommandMessage(this, PlayerCommandMessageType.Pause));
                        break;
                    case UIEventSubtype.RemoteControlStop:
                        Console.WriteLine("RemoteControlStop");
                        _messengerHub.PublishAsync(new PlayerCommandMessage(this, PlayerCommandMessageType.Stop));
                        break;
                    default:
                        Console.WriteLine("RemoteControlReceived");                        
                        break;
                }
            }
            
            base.RemoteControlReceived(theEvent);
        }
    }
}
