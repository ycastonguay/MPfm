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
        Action<string> OnAddDeviceFromUrl { get; set; }
        Action<SyncDevice> OnRemoveDevice { get; set; }
        Action<SyncDevice> OnSyncLibrary { get; set; }
        Action<SyncDevice> OnResumePlayback { get; set; }
        Action OnOpenAddDeviceDialog { get; set; }

        Action<SyncDevice> OnRemotePlayPause { get; set; }
        Action<SyncDevice> OnRemotePrevious { get; set; }
        Action<SyncDevice> OnRemoteNext { get; set; }
        Action<SyncDevice> OnRemoteRepeat { get; set; }
        Action<SyncDevice> OnRemoteShuffle { get; set; }

        void SyncError(Exception ex);
        void RefreshIPAddress(string address);
        void RefreshStatus(string status);
        void NotifyAddedDevice(SyncDevice device);
        void NotifyRemovedDevice(SyncDevice device);
        void NotifyUpdatedDevice(SyncDevice device);
        void NotifyUpdatedDevices(IEnumerable<SyncDevice> devices);
        void SyncDevice(SyncDevice device);
	}
}
