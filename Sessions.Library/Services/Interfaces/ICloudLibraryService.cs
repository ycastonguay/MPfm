﻿// Copyright © 2011-2013 Yanick Castonguay
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
using System.Collections;
using System.Collections.Generic;
using Sessions.Library.Objects;
using Sessions.Sound.AudioFiles;

namespace Sessions.Library.Services.Interfaces
{
    public delegate void DeviceInfosDownloadProgress(float percentage);
    public delegate void DeviceInfosAvailable(IEnumerable<CloudDeviceInfo> deviceInfos);
    public delegate void DeviceInfoUpdated(CloudDeviceInfo deviceInfo);

    /// <summary>
    /// Interface for the cloud service implementations.
    /// </summary>
    public interface ICloudLibraryService
    {
        // Do we expose the device infos directly instead of using Pull? A property or GetDeviceInfos would get a cached list.
        // Exposing the list directly would lead the consumer to add/remove stuff. We want GetDeviceInfos instad.

        bool HasLinkedAccount { get; }

        event DeviceInfosDownloadProgress OnDeviceInfosDownloadProgress;
        event DeviceInfosAvailable OnDeviceInfosAvailable;
        event DeviceInfoUpdated OnDeviceInfoUpdated;

        void InitializeAppFolder();
        void PushDeviceInfo(AudioFile audioFile, long positionBytes, string position);
        void PullDeviceInfos();
        IEnumerable<CloudDeviceInfo> GetDeviceInfos();
		void WatchDeviceInfos();
		void StopWatchingDeviceInfos();
    }
}
