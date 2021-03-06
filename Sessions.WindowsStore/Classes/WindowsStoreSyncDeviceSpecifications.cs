﻿// Copyright © 2011-2013 Yanick Castonguay
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
using Windows.Networking.Connectivity;
using MPfm.Library;
using MPfm.Library.Objects;

namespace MPfm.WindowsStore.Classes
{
    public class WindowsStoreSyncDeviceSpecifications : ISyncDeviceSpecifications
    {
        public event NetworkStateChanged OnNetworkStateChanged;

        public SyncDeviceType GetDeviceType()
        {
            return SyncDeviceType.WindowsStore;
        }

        public string GetDeviceName()
        {
            return "Windows Store Generic Device";
        }

        public long GetFreeSpace()
        {
            return 0;
        }

        public string GetIPAddress()
        {
            string ip = string.Empty;
            var icp = NetworkInformation.GetInternetConnectionProfile();
            if (icp != null && icp.NetworkAdapter != null)
            {
                var hostname =
                    NetworkInformation.GetHostNames()
                        .SingleOrDefault(
                            hn =>
                                hn.IPInformation != null && hn.IPInformation.NetworkAdapter != null
                                && hn.IPInformation.NetworkAdapter.NetworkAdapterId
                                == icp.NetworkAdapter.NetworkAdapterId);

                if (hostname != null)
                    ip = hostname.CanonicalName;
            }
            return ip;
        }

        public string GetMusicFolderPath()
        {
            return string.Empty;
        }

        public List<string> GetRootFolderPaths()
        {
            return new List<string>();
        }

        public void ReportNetworkStateChange(NetworkState networkState)
        {
        }
    }
}
