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

using System;
using Android.Content;
using Android.Net;
using Android.Net.Wifi.P2p;
using MPfm.Android.Classes.Listeners;
using MPfm.Library;
using MPfm.Library.Objects;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Messages;
using TinyMessenger;

namespace MPfm.Android.Classes.Receivers
{
    [BroadcastReceiver]
    public class WifiDirectReceiver : BroadcastReceiver
    {
        private WifiP2pManager _manager;
        private WifiP2pManager.Channel _channel;
        private PeerListListener _peerListListener;

        public WifiDirectReceiver()
        {
            // Broadcast receiver ctor has to be empty
        }

        public void SetManager(WifiP2pManager manager, WifiP2pManager.Channel channel, PeerListListener peerListListener)
        {
            // Maybe the receiver should only use events instead.
            _manager = manager;
            _channel = channel;
            _peerListListener = peerListListener;            
        }

        public override void OnReceive(Context context, Intent intent)
        {
            string action = intent.Action;
            if (WifiP2pManager.WifiP2pStateChangedAction == action)
            {
                // Determine if Wifi Direct mode is enabled
                int state = intent.GetIntExtra(WifiP2pManager.ExtraWifiState, -1);
                Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> WifiDirectReceiver - OnReceive - WifiP2pStateChangedAction - state: {0}", state);
                if (state == (int)WifiP2pState.Enabled)
                {
                    // enabled
                }
                else
                {
                    // disabled
                }
            }
            else if (WifiP2pManager.WifiP2pPeersChangedAction == action)
            {
                Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> WifiDirectReceiver - OnReceive - WifiP2pPeersChangedAction");
                _manager.RequestPeers(_channel, _peerListListener);
            }
            else if (WifiP2pManager.WifiP2pConnectionChangedAction == action)
            {
                Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> WifiDirectReceiver - OnReceive - WifiP2pConnectionChangedAction");
            }
            else if (WifiP2pManager.WifiP2pThisDeviceChangedAction == action)
            {
                Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> WifiDirectReceiver - OnReceive - WifiP2pThisDeviceChangedAction");
            }
        }
    }
}
