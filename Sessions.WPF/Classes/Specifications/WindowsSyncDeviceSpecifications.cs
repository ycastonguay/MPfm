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
using System.IO;
using Sessions.Library;
using Sessions.Library.Objects;
using Sessions.Library.Services;

namespace Sessions.WPF.Classes.Specifications
{
    /// <summary>
    /// Device specifications for Windows. Used for identifying sync devices.
    /// </summary>
    public class WindowsSyncDeviceSpecifications : ISyncDeviceSpecifications
    {
        public event NetworkStateChanged OnNetworkStateChanged;

        public SyncDeviceType GetDeviceType()
        {
            return SyncDeviceType.Windows;
        }

        public string GetDeviceName()
        {
            return System.Environment.MachineName;;
        }

        public string GetDeviceUniqueId()
        {
            string machineName = System.Environment.MachineName;
            machineName = machineName.Replace(" ", "");
            machineName = machineName.Normalize();
            return machineName;
        }

        public long GetFreeSpace()
        {
            string root = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
            //Console.WriteLine("MacSyncDeviceSpecifications - GetFreeSpace - My music folder: {0} - Root: {1}", Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), root);
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                //Console.WriteLine("MacSyncDeviceSpecifications - GetFreeSpace - DriveInfo name: {0} driveType: {1} driveFormat: {2} totalFreeSpace: {3} availableFreeSpace: {4} isReady: {5} rootDirectory: {6}", drive.Name, drive.DriveType.ToString(), drive.DriveFormat.ToString(), drive.TotalFreeSpace, drive.AvailableFreeSpace, drive.IsReady, drive.RootDirectory);
                if (drive.RootDirectory.Name == root)
                    return drive.AvailableFreeSpace;
            }

            return 0;
        }

        public string GetIPAddress()
        {
            return SyncListenerService.GetLocalIPAddress().ToString();
        }

        public string GetMusicFolderPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        }

        public List<string> GetRootFolderPaths()
        {
            return new List<string>();
        }

        public void ReportNetworkStateChange(NetworkState networkState)
        {
            if (OnNetworkStateChanged != null)
                OnNetworkStateChanged(networkState);
        }
    }
}
