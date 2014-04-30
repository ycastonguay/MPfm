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
using MPfm.MVP.Models;
using MPfm.Library.Objects;

namespace MPfm.MVP.Views
{
	/// <summary>
	/// Sync view interface.
	/// </summary>
	public interface ISyncView : IBaseView
	{
        Action<SyncDevice> OnConnectDevice { get; set; }
        Action<string> OnConnectDeviceManually { get; set; }
        Action OnOpenConnectDevice { get; set; }
        Action OnStartDiscovery { get; set; }
        Action OnCancelDiscovery { get; set; }

        void SyncError(Exception ex);
        void RefreshIPAddress(string address);
        void RefreshDiscoveryProgress(float percentageDone, string status);
        void RefreshDevices(IEnumerable<SyncDevice> devices);
        void RefreshDevicesEnded();
        void NotifyAddedDevice(SyncDevice device);
        void NotifyRemovedDevice(SyncDevice device);
        void NotifyUpdatedDevice(SyncDevice device);
        void SyncDevice(SyncDevice device);
	}
}
