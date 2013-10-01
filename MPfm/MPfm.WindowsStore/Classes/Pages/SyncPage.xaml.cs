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
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MPfm.Core;
using MPfm.Library.Objects;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;
using MPfm.WindowsStore.Classes.Navigation;
using MPfm.WindowsStore.Classes.Pages.Base;

namespace MPfm.WindowsStore.Classes.Pages
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class SyncPage : BasePage, ISyncView
    {
        private WindowsStoreNavigationManager _navigationManager;

        public SyncPage()
        {
            Debug.WriteLine("SyncPage - Ctor");
            this.InitializeComponent();
            _navigationManager = (WindowsStoreNavigationManager) Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            Debug.WriteLine("SyncPage - LoadState");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.WriteLine("SyncPage - OnNavigatedTo");
            base.OnNavigatedTo(e); // Leave this, or the app will crash when clicking the back button
            _navigationManager.SetViewInstance(this);
        }

        private async void btnConnectManual_Click(object sender, RoutedEventArgs e)
        {
            //greetingOutput.Text = "Hello,, " + nameInput.Text + "!";

            //FileOpenPicker openPicker = new FileOpenPicker();
            //openPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
            //openPicker.ViewMode = PickerViewMode.List;

            //openPicker.FileTypeFilter.Clear();
            //openPicker.FileTypeFilter.Add(".mp3");
            //openPicker.FileTypeFilter.Add(".flac");

            //var file = await openPicker.PickSingleFileAsync();

            //if (file == null)
            //    return;

            //try
            //{
            //    AudioFile audioFile = new AudioFile(file.Path);
            //    string a = audioFile.FilePath;
            //}
            //catch (Exception ex)
            //{                
            //    throw;
            //}            
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {

        }

        #region ISyncView implementation

        public Action<SyncDevice> OnConnectDevice { get; set; }
        public Action<string> OnConnectDeviceManually { get; set; }
        public Action OnStartDiscovery { get; set; }
        public Action OnCancelDiscovery { get; set; }

        public void SyncError(Exception ex)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                var dialog = new MessageDialog(string.Format("An error has occured in SyncPage: {0}", ex), "Error");
                dialog.ShowAsync();
            });
        }

        public void RefreshIPAddress(string address)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                lblIPAddress.Text = address;
            });            
        }

        public void RefreshDiscoveryProgress(float percentageDone, string status)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                lblStatus.Text = status;
                progressBar.Value = percentageDone;
            });  
        }

        public void RefreshDevices(IEnumerable<SyncDevice> devices)
        {
            Tracing.Log("SyncPage - RefreshDevices - devices.Count: {0}", devices.Count());
            Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                listView.ItemsSource = devices.ToList();
            });
        }

        public void RefreshDevicesEnded()
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                lblStatus.Text = "Refreshed device list successfully.";
                progressBar.Value = 100;
            });
        }

        public void SyncDevice(SyncDevice device)
        {
        }

        #endregion
       
    }
}
