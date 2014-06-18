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
using Android.Content;
using Android.Net.Wifi;
using Android.OS;
using Android.Provider;
using Android.Telephony;
using Android.Text.Format;
using MPfm.Library;
using MPfm.Library.Objects;

namespace MPfm.Android.Classes
{
    /// <summary>
    /// Device specifications for iOS. Used for identifying sync devices.
    /// </summary>
    public class AndroidSyncDeviceSpecifications : ISyncDeviceSpecifications
    {
        private readonly Context _context;

        public event NetworkStateChanged OnNetworkStateChanged;

        public AndroidSyncDeviceSpecifications()
        {
            _context = MPfmApplication.GetApplicationContext();
        }

        private SyncDeviceType _deviceType = SyncDeviceType.Unknown;
        public SyncDeviceType GetDeviceType()
        {
            if (_deviceType != SyncDeviceType.Unknown)
                return _deviceType;

            var telephonyManager = (TelephonyManager)_context.GetSystemService(Context.TelephonyService);
            _deviceType = telephonyManager.PhoneType == PhoneType.None ? SyncDeviceType.AndroidTablet : SyncDeviceType.AndroidPhone;
            return _deviceType;
        }

        string _deviceName = string.Empty;
        public string GetDeviceName()
        {
            if (String.IsNullOrEmpty(_deviceName))
                _deviceName = String.Format("{0} {1} ({2})", global::Android.OS.Build.Manufacturer, global::Android.OS.Build.Product, global::Android.OS.Build.Model);
            return _deviceName;
        }

        public string GetDeviceUniqueId()
        {
            return Settings.Secure.GetString(_context.ContentResolver, Settings.Secure.AndroidId);
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

        public List<string> GetRootFolderPaths()
        {
            var paths = new List<string>();

            // Get internal storage path (sometimes refered to 'sdcard' on Samsung devices but it is actually internal storage!)
            string path = global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            paths.Add(path);

            // Get external storage paths (i.e. sd cards)
            //string path2 = global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.ExternalStorageDirectory).ToString();
            //bool isMediaMounted = global::Android.OS.Environment.ExternalStorageState.Equals(global::Android.OS.Environment.MediaMounted);            
            return paths;
        }

        public void ReportNetworkStateChange(NetworkState networkState)
        {
            if(OnNetworkStateChanged != null)
                OnNetworkStateChanged(networkState);
        }
    }
}
