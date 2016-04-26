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
using Foundation;
using Sessions.Library;
using Sessions.Library.Objects;
using UIKit;
using Sessions.Library.Services;
using System.Collections.Generic;

namespace Sessions.iOS.Helpers
{
    /// <summary>
    /// Device specifications for iOS. Used for identifying sync devices.
    /// </summary>
    public class iOSSyncDeviceSpecifications : NSObject, ISyncDeviceSpecifications
    {
        public event NetworkStateChanged OnNetworkStateChanged;

        public iOSSyncDeviceSpecifications()
        {
            ReachabilityHelper.ReachabilityChanged += HandleReachabilityChanged;
        }

        private void HandleReachabilityChanged(object sender, EventArgs e)
        {
            Console.WriteLine("iOSSyncDeviceSpecifications - Reachability changed!");
        }

        public SyncDeviceType GetDeviceType()
        {
			var deviceType = SyncDeviceType.Unknown;
			InvokeOnMainThread(() =>
			deviceType = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? SyncDeviceType.iPad : SyncDeviceType.iPhone);
			return deviceType;
        }

        string _deviceName = string.Empty;
        public string GetDeviceName()
        {
            if (String.IsNullOrEmpty(_deviceName))
            {
                this.InvokeOnMainThread(() => {
                    _deviceName = UIDevice.CurrentDevice.Name;
                });
            }
            return _deviceName;
        }

        public long GetFreeSpace()
        {
            long freeSpace = 0;
            this.InvokeOnMainThread(() => {
                var attributes = NSFileManager.DefaultManager.GetFileSystemAttributes(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
                freeSpace = (long)attributes.FreeSize;
            });
            return freeSpace;
        }

        public string GetIPAddress()
        {
             return SyncListenerService.GetLocalIPAddress().ToString();
        }

        public string GetDeviceUniqueId()
        {
            return UIDevice.CurrentDevice.IdentifierForVendor.AsString();
        }

        public string GetMusicFolderPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        public IEnumerable<string> GetMusicFolderPaths()
        {
            var folders = new List<string>();
            folders.Add(GetMusicFolderPath());
            return folders;
        }

        public IEnumerable<string> GetRootFolderPaths()
        {
            return new List<string>();
        }

        public void ReportNetworkStateChange(NetworkState networkState)
        {
            if(OnNetworkStateChanged != null)
                OnNetworkStateChanged(networkState);
        }
    }
}
