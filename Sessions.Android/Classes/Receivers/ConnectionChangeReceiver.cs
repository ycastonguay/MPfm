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
using Android.Net;
using Sessions.Library;
using Sessions.Library.Objects;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Messages;
using TinyMessenger;

namespace Sessions.Android.Classes.Receivers
{
    [BroadcastReceiver]
    public class ConnectionChangeReceiver : BroadcastReceiver
    {
        readonly ITinyMessengerHub _messageHub;
        readonly ISyncDeviceSpecifications _deviceSpecifications;

        public ConnectionChangeReceiver()
        {            
            _messageHub = Bootstrapper.GetContainer().Resolve<ITinyMessengerHub>();
            _deviceSpecifications = Bootstrapper.GetContainer().Resolve<ISyncDeviceSpecifications>();
        }

        public override void OnReceive(Context context, Intent intent)
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
            NetworkInfo activeNetInfo = connectivityManager.ActiveNetworkInfo;
            NetworkInfo wifiNetInfo = connectivityManager.GetNetworkInfo(ConnectivityType.Wifi);
            NetworkInfo mobileNetInfo = connectivityManager.GetNetworkInfo(ConnectivityType.Mobile);
            var networkState = new NetworkState() {
                IsNetworkAvailable = (activeNetInfo != null) && activeNetInfo.IsAvailable,
                IsWifiAvailable = (wifiNetInfo != null) && wifiNetInfo.IsAvailable,
                IsCellularAvailable = (mobileNetInfo != null) && mobileNetInfo.IsAvailable
            };

            Console.WriteLine("ConnectionChangeReceiver - active: {0} - wifi: {1} - mobile: {2}", networkState.IsNetworkAvailable, networkState.IsWifiAvailable, networkState.IsCellularAvailable);
            _deviceSpecifications.ReportNetworkStateChange(networkState);
            _messageHub.PublishAsync<ConnectionStatusChangedMessage>(new ConnectionStatusChangedMessage(this) {
                NetworkState = networkState
            });
        }
    }
}
