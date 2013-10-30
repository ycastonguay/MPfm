﻿// Copyright © 2011-2013 Yanick Castonguay
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
using System.Security;
using System.Windows;
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
    public partial class SyncMenuWindow : BaseWindow, ISyncMenuView
    {
        public SyncMenuWindow(Action<IBaseView> onViewReady) 
            : base (onViewReady)
        {
            InitializeComponent();
            ViewIsReady();
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
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                MessageBox.Show(this, string.Format("An error occured in SyncMenu: {0}", ex), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }));
        }

        public void SyncEmptyError(Exception ex)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }));
        }

        public void RefreshDevice(SyncDevice device)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                string title = string.Format("Sync with {0}", device.Name);
                lblTitle.Content = title;
                this.Title = title;
            }));
        }

        public void RefreshLoading(bool isLoading, int progressPercentage)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                progressBar.Value = progressPercentage;
                gridLoading.Visibility = isLoading ? Visibility.Visible : Visibility.Hidden;
                progressBar.Visibility = isLoading ? Visibility.Visible : Visibility.Hidden;
                lblLoading.Visibility = isLoading ? Visibility.Visible : Visibility.Hidden;

                if (progressPercentage < 100)
                    lblLoading.Content = String.Format("Loading index ({0}%)...", progressPercentage);
                else
                    lblLoading.Content = "Processing index...";
            }));
        }

        public void RefreshSelectButton(string text)
        {
        }

        public void RefreshItems(List<SyncMenuItemEntity> items)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                treeViewItems.ItemsSource = items.ToList();
            }));
        }

        public void RefreshSelection(List<AudioFile> audioFiles)
        {
        }

        public void RefreshSyncTotal(string title, string subtitle, bool enoughFreeSpace)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                lblTotal.Content = title;
                lblFreeSpace.Content = subtitle;
            }));
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