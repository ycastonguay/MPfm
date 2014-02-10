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
        private bool _isDiscovering;

        public SyncWindow(Action<IBaseView> onViewReady) 
            : base (onViewReady)
        {
            InitializeComponent();
            ViewIsReady();
        }

        private void RefreshDeviceListButton()
        {
            if (_isDiscovering)
            {
                btnRefresh.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Buttons/cancel.png"));
                btnRefresh.Title = "Cancel refresh";
            }
            else
            {
                btnRefresh.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/Images/Buttons/refresh.png"));
                btnRefresh.Title = "Refresh devices";
            }
        }

        private void btnRefresh_OnClick(object sender, RoutedEventArgs e)
        {
            if (_isDiscovering)
                OnCancelDiscovery();
            else
                OnStartDiscovery();   
        }

        private void btnConnect_OnClick(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItems.Count == 0)
                return;

            OnCancelDiscovery();
            OnConnectDevice((SyncDevice)listView.SelectedItems[0]);
        }

        private void btnConnectManual_OnClick(object sender, RoutedEventArgs e)
        {
            
        }

        #region ISyncView implementation

        public Action<SyncDevice> OnConnectDevice { get; set; }
        public Action<string> OnConnectDeviceManually { get; set; }
        public Action OnOpenConnectDevice { get; set; }
        public Action OnStartDiscovery { get; set; }
        public Action OnCancelDiscovery { get; set; }
        
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

        public void RefreshDiscoveryProgress(float percentageDone, string status)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                if (!_isDiscovering)
                {
                    _isDiscovering = true;
                    progressBar.Visibility = Visibility.Visible;                    
                    RefreshDeviceListButton();
                }
                progressBar.Value = (int)percentageDone;
            }));
        }

        public void RefreshDevices(IEnumerable<SyncDevice> devices)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                listView.ItemsSource = devices.ToList();
            }));
        }

        public void RefreshDevicesEnded()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                _isDiscovering = false;
                progressBar.Visibility = Visibility.Hidden;
                RefreshDeviceListButton();
            }));
        }

        public void SyncDevice(SyncDevice device)
        {
        }

        #endregion

    }
}
