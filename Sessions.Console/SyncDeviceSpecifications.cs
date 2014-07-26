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
using System.Timers;
using System.Threading;
using System.Text;
using Mono;
using Mono.Terminal;
using Sessions.Library;
using Sessions.Library.Objects;

namespace Sessions.Console
{
    public class SyncDeviceSpecifications: ISyncDeviceSpecifications
    {
        public event NetworkStateChanged OnNetworkStateChanged;

        public SyncDeviceType GetDeviceType()
        {
            return SyncDeviceType.Unknown;
        }

        public string GetDeviceName()
        {
            return "Console";
        }

        public string GetDeviceUniqueId()
        {
            return Guid.NewGuid().ToString();
        }

        public long GetFreeSpace()
        {
            return 0;
        }

        public string GetIPAddress()
        {
            return "127.0.0.1";
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
