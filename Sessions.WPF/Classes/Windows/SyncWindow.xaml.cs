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
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using MPfm.MVP.Views;
using MPfm.WPF.Classes.Helpers;
using MPfm.WPF.Classes.Windows.Base;
using Sessions.Library.Objects;

namespace MPfm.WPF.Classes.Windows
{
    public partial class SyncWindow : BaseWindow, ISyncView
    {
        private ObservableCollection<SyncDevice> _items = new ObservableCollection<SyncDevice>();
        private Timer _timer;

        public SyncWindow(Action<IBaseView> onViewReady) 
            : base (onViewReady)
        {
            InitializeComponent();
            Initialize();
            ViewIsReady();
        }

        private void Initialize()
        {
            listViewDevices.ItemsSource = _items;
        }

        private void ResetFields()
        {
            lblDeviceName.Content = string.Empty;
            lblDeviceUrl.Content = string.Empty;
            lblArtistName.Content = string.Empty;
            lblAlbumTitle.Content = string.Empty;
            lblSongTitle.Content = string.Empty;
            lblPlaylist.Content = string.Empty;
            lblPosition.Content = string.Empty;
            lblLastUpdated.Content = string.Empty;
            lblStatus.Content = string.Empty;

            imageViewAlbum.Source = null;
            imageViewDeviceType.Source = null;
        }

        private void RefreshDeviceDetails(SyncDevice device)
        {
            string iconName = string.Empty;
            switch (device.DeviceType)
            {
                case SyncDeviceType.Linux:
                    iconName = "pc_linux_large";
                    break;
                case SyncDeviceType.OSX:
                    iconName = "pc_mac_large";
                    break;
                case SyncDeviceType.Windows:
                    iconName = "pc_windows_large";
                    break;
                case SyncDeviceType.iPhone:
                    iconName = "phone_iphone_large";
                    break;
                case SyncDeviceType.iPad:
                    iconName = "tablet_ipad_large";
                    break;
                case SyncDeviceType.AndroidPhone:
                    iconName = "phone_android_large";
                    break;
                case SyncDeviceType.AndroidTablet:
                    iconName = "tablet_android_large";
                    break;
                case SyncDeviceType.WindowsPhone:
                    iconName = "phone_windows_large";
                    break;
                case SyncDeviceType.WindowsStore:
                    iconName = "tablet_windows_large";
                    break;
            }

            lblDeviceName.Content = device.Name;
            lblDeviceUrl.Content = string.IsNullOrEmpty(device.Url) ? "Unknown" : device.Url;
            lblStatus.Content = device.IsOnline ? "Online" : "Offline";
            //imageViewDeviceType.Image = ImageResources.Images.FirstOrDefault(x => x.Name == iconName);
            lblLastUpdated.Content = device.IsOnline ? string.Format("Last updated: {0}", device.LastUpdated) : string.Format("Last seen online: {0}", device.LastUpdated);

            if (device.PlayerMetadata != null)
            {
                lblArtistName.Content = device.PlayerMetadata.CurrentAudioFile.ArtistName;
                lblAlbumTitle.Content = device.PlayerMetadata.CurrentAudioFile.AlbumTitle;
                lblSongTitle.Content = device.PlayerMetadata.CurrentAudioFile.Title;
                lblPosition.Content = string.Format("{0} / {1}", device.PlayerMetadata.Position + 1, device.PlayerMetadata.Length);
                lblPlaylist.Content = string.Format("On-the-fly Playlist ({0}/{1})", device.PlayerMetadata.PlaylistIndex, device.PlayerMetadata.PlaylistCount);
                //LoadAlbumArt(device.AlbumArt);
            }
            else
            {
                lblArtistName.Content = string.Empty;
                lblAlbumTitle.Content = string.Empty;
                lblSongTitle.Content = string.Empty;
                lblPosition.Content = string.Empty;
                lblPlaylist.Content = string.Empty;
            }
        }

        private void ListViewDevices_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Console.WriteLine("SelectionDidChange");
            //menuItemRemoveDeviceFromList.Enabled = tableViewDevices.SelectedRow >= 0;
            if (e.AddedItems.Count > 0)
            {
                var valid = e.AddedItems[0];
                foreach (var item in listViewDevices.SelectedItems)
                {
                    if(item != valid)
                        listViewDevices.SelectedItems.Remove(item);
                }
            }

            if (listViewDevices.SelectedIndex < 0)
            {
                ResetFields();
                return;
            }

            RefreshDeviceDetails(_items[listViewDevices.SelectedIndex]);
        }

        private void btnResumePlayback_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void btnSyncLibrary_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void BtnAddDevice_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void BtnPlayerPrevious_OnClick(object sender, RoutedEventArgs e)
        {
            if (listViewDevices.SelectedIndex == -1) return;
            OnRemotePrevious(_items[listViewDevices.SelectedIndex]); 
        }

        private void BtnPlayerPlayPause_OnClick(object sender, RoutedEventArgs e)
        {
            if (listViewDevices.SelectedIndex == -1) return;
            OnRemotePlayPause(_items[listViewDevices.SelectedIndex]);
        }

        private void BtnPlayerNext_OnClick(object sender, RoutedEventArgs e)
        {
            if (listViewDevices.SelectedIndex == -1) return;
            OnRemoteNext(_items[listViewDevices.SelectedIndex]);
        }

        private void BtnPlayerRepeat_OnClick(object sender, RoutedEventArgs e)
        {
            if (listViewDevices.SelectedIndex == -1) return;
            OnRemoteRepeat(_items[listViewDevices.SelectedIndex]);
        }

        private void BtnPlayerShuffle_OnClick(object sender, RoutedEventArgs e)
        {
            if (listViewDevices.SelectedIndex == -1) return;
            OnRemoteShuffle(_items[listViewDevices.SelectedIndex]);
        }

        private void ListViewDevices_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UIHelper.ListView_PreviewMouseDown_RemoveSelectionIfNotClickingOnAListViewItem(listViewDevices, e);
        }

        #region ISyncView implementation

        public Action<string> OnAddDeviceFromUrl { get; set; }
        public Action<SyncDevice> OnRemoveDevice { get; set; }
        public Action<SyncDevice> OnSyncLibrary { get; set; }
        public Action<SyncDevice> OnResumePlayback { get; set; }
        public Action OnOpenAddDeviceDialog { get; set; }

        public Action<SyncDevice> OnRemotePlayPause { get; set; }
        public Action<SyncDevice> OnRemotePrevious { get; set; }
        public Action<SyncDevice> OnRemoteNext { get; set; }
        public Action<SyncDevice> OnRemoteRepeat { get; set; }
        public Action<SyncDevice> OnRemoteShuffle { get; set; }

        public void SyncError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void RefreshIPAddress(string address)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                lblIPAddress.Content = address;
            }));
        }

        public void RefreshStatus(string status)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                lblStatus.Content = status;
            }));
        }

        public void NotifyAddedDevice(SyncDevice device)
        {
            _items.Add(device);
        }

        public void NotifyRemovedDevice(SyncDevice device)
        {
            _items.Remove(device);
        }

        public void NotifyUpdatedDevice(SyncDevice device)
        {
            // Not needed?
        }

        public void NotifyUpdatedDevices(IEnumerable<SyncDevice> devices)
        {
            foreach(var device in devices)
                _items.Add(device);
        }

        public void SyncDevice(SyncDevice device)
        {
        }

        #endregion

    }
}
