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
using System.Collections.Generic;
using System.Linq;
using Android.Net.Wifi.P2p;

namespace MPfm.Android.Classes.Listeners
{
    public class PeerListListener : Java.Lang.Object, WifiP2pManager.IPeerListListener
    {
        private List<WifiP2pDevice> _devices;

        public void OnPeersAvailable(WifiP2pDeviceList peers)
        {
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> PeerListListener - OnPeersAvailable");
            _devices = peers.DeviceList.ToList();
            foreach (var device in _devices)
            {
                Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> PeerListListener - OnPeersAvailable - deviceName: {0} deviceAddress: {1} deviceStatus: {2} devicePrimaryDeviceType: {3}", device.DeviceName, device.DeviceAddress, device.Status, device.PrimaryDeviceType);
            }
        }
    }
}