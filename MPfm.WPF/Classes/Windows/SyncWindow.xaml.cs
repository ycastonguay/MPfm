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
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MPfm.Library.Objects;
using MPfm.MVP.Messages;
using MPfm.MVP.Models;
using MPfm.MVP.Presenters;
using MPfm.MVP.Views;
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;
using MPfm.WPF.Classes.Windows.Base;

namespace MPfm.WPF.Classes.Windows
{
    public partial class SyncWindow : BaseWindow, ISyncView
    {
        public SyncWindow(Action<IBaseView> onViewReady) 
            : base (onViewReady)
        {
            InitializeComponent();
            ViewIsReady();
        }

        private void btnRefresh_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void btnConnect_OnClick(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItems.Count == 0)
                return;

            //OnConnectDevice((SyncDevice)listView.SelectedItems[0]);
        }

        private void btnConnectManual_OnClick(object sender, RoutedEventArgs e)
        {
            
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
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                MessageBox.Show(this, string.Format("An error occured in Sync: {0}", ex), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }));
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
        }

        public void NotifyAddedDevice(SyncDevice device)
        {
        }

        public void NotifyRemovedDevice(SyncDevice device)
        {
        }

        public void NotifyUpdatedDevice(SyncDevice device)
        {
        }

        public void NotifyUpdatedDevices(IEnumerable<SyncDevice> devices)
        {
        }

        public void SyncDevice(SyncDevice device)
        {
        }

        #endregion

    }
}
