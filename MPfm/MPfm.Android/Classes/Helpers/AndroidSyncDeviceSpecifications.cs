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
using Android.App;
using Android.Content;
using Android.Net.Wifi;
using Android.OS;
using Android.Text.Format;
using Java.IO;
using MPfm.Library;
using MPfm.Library.Objects;
using Environment = System.Environment;

namespace MPfm.Android.Classes.Helpers
{
    /// <summary>
    /// Device specifications for iOS. Used for identifying sync devices.
    /// </summary>
    public class AndroidSyncDeviceSpecifications : ISyncDeviceSpecifications
    {
        private readonly Context _context;

        public AndroidSyncDeviceSpecifications()
        {
            _context = MPfmApplication.GetApplicationContext();
        }

        public SyncDeviceType GetDeviceType()
        {
            return SyncDeviceType.Android;
        }

        string _deviceName = string.Empty;
        public string GetDeviceName()
        {
            if (String.IsNullOrEmpty(_deviceName))
            {
                _deviceName = String.Format("{0} {1} ({2})", global::Android.OS.Build.Manufacturer, global::Android.OS.Build.Product, global::Android.OS.Build.Model);
            }
            return _deviceName;
        }

        public long GetFreeSpace()
        {
            StatFs stat = new StatFs(global::Android.OS.Environment.ExternalStorageDirectory.ToString());
            long bytesAvailable = (long)stat.BlockSize * (long)stat.AvailableBlocks;
            return bytesAvailable;
        }

        public string GetIPAddress()
        {
            WifiManager wifiManager = (WifiManager)_context.GetSystemService(Context.WifiService);
            int ip = wifiManager.ConnectionInfo.IpAddress;
            return Formatter.FormatIpAddress(ip);
        }

        public string GetMusicFolderPath()
        {
            return global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryMusic).ToString();
        }
    }
}
