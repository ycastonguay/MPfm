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
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MPfm.Library.Objects;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.WindowsPhone.Classes.Helpers;
using MPfm.WindowsPhone.Classes.Navigation;
using MPfm.WindowsPhone.Classes.Pages.Base;

namespace MPfm.WindowsPhone.Classes.Pages
{
    public partial class SyncPage : BasePage, ISyncView
    {
        public SyncPage()
        {
            Debug.WriteLine("SyncPage - Ctor - Initializing components...");
            InitializeComponent();
            SetTheme(LayoutRoot);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Debug.WriteLine("SyncPage - OnNavigatedFrom");
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.WriteLine("SyncPage - OnNavigatedTo");
            base.OnNavigatedTo(e);

            var navigationManager = (WindowsPhoneNavigationManager)Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.SetViewInstance(this);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Debug.WriteLine("SyncPage - OnNavigatingFrom");
            base.OnNavigatingFrom(e);
        }

        #region ISyncView implementation

        public Action<SyncDevice> OnConnectDevice { get; set; }
        public Action<string> OnConnectDeviceManually { get; set; }
        public Action OnStartDiscovery { get; set; }
        public Action OnCancelDiscovery { get; set; }

        public void SyncError(Exception ex)
        {
            Dispatcher.BeginInvoke(() => MessageBox.Show(string.Format("An error occured in SyncPage: {0}", ex), "Error", MessageBoxButton.OK));
        }

        public void RefreshIPAddress(string address)
        {
            Dispatcher.BeginInvoke(() =>
            {
                lblIPAddress.Text = address;
            });
        }

        public void RefreshDiscoveryProgress(float percentageDone, string status)
        {
            Dispatcher.BeginInvoke(() =>
            {
                lblStatus.Text = status;
                progressBar.Value = percentageDone;
            });
        }

        public void RefreshDevices(IEnumerable<SyncDevice> devices)
        {
            Dispatcher.BeginInvoke(() =>
            {
                listDevices.ItemsSource = devices.ToList();
            });
        }

        public void RefreshDevicesEnded()
        {
        }

        public void SyncDevice(SyncDevice device)
        {
        }

        #endregion

        private void listDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get item of LongListSelector. 
            List<UserControl> userControlList = new List<UserControl>();
            XamlHelper.GetItemsRecursive<UserControl>(listDevices, ref userControlList);

            // Selected
            if (e.AddedItems.Count > 0 && e.AddedItems[0] != null)
                foreach (UserControl userControl in userControlList)
                    if (e.AddedItems[0].Equals(userControl.DataContext))
                        VisualStateManager.GoToState(userControl, "Selected", true);
            // Unselected
            if (e.RemovedItems.Count > 0 && e.RemovedItems[0] != null)
                foreach (UserControl userControl in userControlList)
                    if (e.RemovedItems[0].Equals(userControl.DataContext))
                        VisualStateManager.GoToState(userControl, "Normal", true);

            var addedItem = e.AddedItems.Count > 0 ? e.AddedItems[0] : null;
            if (addedItem == null)
                return;

            //var item = _menuOptions.FirstOrDefault(x => x.Value.ToLower() == (string)addedItem);
            //if (!item.Equals(default(KeyValuePair<MobileOptionsMenuType, string>)))
                //OnItemClick(item.Key);
        }

    }
}