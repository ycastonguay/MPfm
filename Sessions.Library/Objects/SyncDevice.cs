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
using Sessions.Sound.Objects;

namespace Sessions.Library.Objects
{
    /// <summary>
    /// Data structure repesenting a syncable device.
    /// </summary>
	public class SyncDevice
	{
        public string SyncVersionId { get; set; }
        public SyncDeviceType DeviceType { get; set; }
        public bool IsOnline { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime LastTentativeUpdate { get; set; }
        public byte[] AlbumArt { get; set; }
        public string AlbumArtKey { get; set; }
        public PlayerMetadata PlayerMetadata { get; set; }

        public string Status { get { return IsOnline ? "Online" : "Offline"; } set { /* for WPF */ } }
        public string IconName
        {
            get
            {
                string iconName = string.Empty;
                switch (DeviceType)
                {
                    case SyncDeviceType.Linux:
                        iconName = "pc_linux";
                        break;
                    case SyncDeviceType.OSX:
                        iconName = "pc_mac";
                        break;
                    case SyncDeviceType.Windows:
                        iconName = "pc_windows";
                        break;
                    case SyncDeviceType.iPhone:
                        iconName = "phone_iphone";
                        break;
                    case SyncDeviceType.iPad:
                        iconName = "tablet_ipad";
                        break;
                    case SyncDeviceType.AndroidPhone:
                        iconName = "phone_android";
                        break;
                    case SyncDeviceType.AndroidTablet:
                        iconName = "tablet_android";
                        break;
                    case SyncDeviceType.WindowsPhone:
                        iconName = "phone_windows";
                        break;
                    case SyncDeviceType.WindowsStore:
                        iconName = "tablet_windows";
                        break;
                }
                return iconName;
            } 
            set { /* for WPF */ }
        }
	}
	
	/// <summary>
	/// Syncable device type.
	/// </summary>
	public enum SyncDeviceType
	{
        Unknown = 0,
		Linux = 1, 
        OSX = 2, 
        Windows = 3, 
        iPhone = 4, 
        iPad = 5,
        AndroidPhone = 6,
        AndroidTablet = 7,
        WindowsPhone = 8,
        WindowsStore = 9
	}
}
