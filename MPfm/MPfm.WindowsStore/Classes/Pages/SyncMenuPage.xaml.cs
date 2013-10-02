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
using MPfm.MVP.Models;
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
    public sealed partial class SyncMenuPage : BasePage, ISyncMenuView
    {
        private WindowsStoreNavigationManager _navigationManager;

        public SyncMenuPage()
        {
            Debug.WriteLine("SyncMenuPage - Ctor");
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
            Debug.WriteLine("SyncMenuPage - LoadState");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.WriteLine("SyncMenuPage - OnNavigatedTo");
            base.OnNavigatedTo(e); // Leave this, or the app will crash when clicking the back button
            _navigationManager.SetViewInstance(this);
        }

        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            OnSelectButtonClick();
        }

        #region ISyncMenuView implementation

        public Action<SyncMenuItemEntity, object> OnExpandItem { get; set; }
        public Action<List<SyncMenuItemEntity>> OnSelectItems { get; set; }
        public Action<List<AudioFile>> OnRemoveItems { get; set; }
        public Action OnSync { get; set; }
        public Action OnSelectButtonClick { get; set; }
        public Action OnSelectAll { get; set; }
        public Action OnRemoveAll { get; set; }

        public void SyncMenuError(Exception ex)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                var dialog = new MessageDialog(string.Format("An error has occured in SyncMenuPage: {0}", ex), "Error");
                dialog.ShowAsync();
            });
        }

        public void SyncEmptyError(Exception ex)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                var dialog = new MessageDialog(ex.Message, "Error");
                dialog.ShowAsync();
            });
        }

        public void RefreshDevice(SyncDevice device)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                pageTitle.Text = device.Name;
            });
        }

        public void RefreshLoading(bool isLoading, int progressPercentage)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                Tracing.Log("SyncMenuPage - RefreshLoading - isLoading: {0} progressPercentage: {1}", isLoading, progressPercentage);
                if (isLoading)
                {
                    progressBar.Value = progressPercentage;
                    panelLoading.Visibility = Visibility.Visible;

                    //if (progressPercentage < 100)
                    //    lblStatus.Text = String.Format("Loading index ({0}%)...", progressPercentage);
                    //else
                    //    lblStatus.Text = "Processing index...";
                }
                else
                {
                    //lblStatus.Text = "Download finished!";
                    progressBar.Value = 100;
                    panelLoading.Visibility = Visibility.Collapsed;
                }
            });
        }

        public void RefreshSelectButton(string text)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                btnSelectAll.Content = text;
            });
        }

        public void RefreshItems(List<SyncMenuItemEntity> items)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                Tracing.Log("SyncMenuPage - RefreshItems - items.Count: {0}", items.Count);
                lblStatus.Text = string.Format("RefreshItems - items.Count: {0}", items.Count);
                listView.ItemsSource = items;
            });
        }

        public void RefreshSelection(List<AudioFile> audioFiles)
        {
        }

        public void RefreshSyncTotal(string title, string subtitle, bool enoughFreeSpace)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                lblTotal.Text = title;
                lblFreeSpace.Text = subtitle;
            });
        }

        public void InsertItems(int index, SyncMenuItemEntity parentItem, List<SyncMenuItemEntity> items, object userData)
        {
        }

        public void RemoveItems(int index, int count, object userData)
        {
        }

        #endregion

    }
}
