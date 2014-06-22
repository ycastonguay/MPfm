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
using Android.Content;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Messages;
using TinyMessenger;

namespace Sessions.Android.Classes.Receivers
{
    [BroadcastReceiver]
    public class LockReceiver : BroadcastReceiver
    {
        readonly ITinyMessengerHub _messageHub;
        bool _activateLockScreen;

        public LockReceiver()
        {            
            _messageHub = Bootstrapper.GetContainer().Resolve<ITinyMessengerHub>();
            _messageHub.Subscribe<ActivateLockScreenMessage>(message => {
                _activateLockScreen = message.ActivateLockScreen;
            });
        }

        public override void OnReceive(Context context, Intent intent)
        {
            Console.WriteLine("LockReceiver - OnReceive - intent.Action: {0}", intent.Action);

            if (!_activateLockScreen)
                return;

            // Create lock screen activity when the user turns off the screen.
            if (intent.Action == Intent.ActionScreenOff)
            {
                // to do: can the other activity task be hidden when showing lock screen to make sure the last activity doesn't "ghost" in the Finish animation?
                Intent newIntent = new Intent();
                newIntent.SetClass(context, typeof(LockScreenActivity));                
                // New task is required when starting an activity outside an activity.
                newIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTop | ActivityFlags.SingleTop | ActivityFlags.NoAnimation); 
                context.StartActivity(newIntent);
            }
        }
    }
}
