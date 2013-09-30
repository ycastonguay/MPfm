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
using MPfm.Core;
using MPfm.Library.Objects;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Models;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;
using MPfm.WindowsPhone.Classes.Helpers;
using MPfm.WindowsPhone.Classes.Navigation;
using MPfm.WindowsPhone.Classes.Pages.Base;

namespace MPfm.WindowsPhone.Classes.Pages
{
    public partial class SyncMenuPage : BasePage, ISyncMenuView
    {
        public SyncMenuPage()
        {
            Debug.WriteLine("SyncMenuPage - Ctor - Initializing components...");
            InitializeComponent();
            SetTheme(LayoutRoot);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Debug.WriteLine("SyncMenuPage - OnNavigatedFrom");
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.WriteLine("SyncMenuPage - OnNavigatedTo");
            base.OnNavigatedTo(e);

            var navigationManager = (WindowsPhoneNavigationManager)Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.SetViewInstance(this);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            Debug.WriteLine("SyncMenuPage - OnNavigatingFrom");
            base.OnNavigatingFrom(e);
        }

        private void listMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get item of LongListSelector. 
            List<UserControl> userControlList = new List<UserControl>();
            XamlHelper.GetItemsRecursive<UserControl>(listMenu, ref userControlList);

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
            Dispatcher.BeginInvoke(() => MessageBox.Show(string.Format("An error occured in SyncMenuPage: {0}", ex), "Error", MessageBoxButton.OK));
        }

        public void SyncEmptyError(Exception ex)
        {
            Dispatcher.BeginInvoke(() => MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK));
        }

        public void RefreshDevice(SyncDevice device)
        {
            Dispatcher.BeginInvoke(() =>
            {
                lblTitle.Text = device.Name;
            });
        }

        public void RefreshLoading(bool isLoading, int progressPercentage)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (isLoading)
                {
                    lblStatus.Text = progressPercentage.ToString();
                    progressBar.Value = progressPercentage;
                }
                else
                {
                    lblStatus.Text = "Download finished!";
                    progressBar.Value = 100;
                }
            });
        }

        public void RefreshSelectButton(string text)
        {
        }

        public void RefreshItems(List<SyncMenuItemEntity> items)
        {
            Dispatcher.BeginInvoke(() =>
            {
                Tracing.Log("SyncMenuPage - RefreshItems - items.Count: {0}", items.Count);
                lblStatus.Text = string.Format("RefreshItems - items.Count: {0}", items.Count);
                listMenu.ItemsSource = items;
            });
        }

        public void RefreshSelection(List<AudioFile> audioFiles)
        {
        }

        public void RefreshSyncTotal(string title, string subtitle, bool enoughFreeSpace)
        {
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