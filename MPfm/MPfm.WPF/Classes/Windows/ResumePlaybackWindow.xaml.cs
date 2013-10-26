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
using System.Windows.Threading;
using MPfm.Library.Objects;
using MPfm.MVP.Views;
using MPfm.WPF.Classes.Windows.Base;

namespace MPfm.WPF.Classes.Windows
{
    public partial class ResumePlaybackWindow : BaseWindow, IResumePlaybackView
    {
        private List<CloudDeviceInfo> _devices;

        public ResumePlaybackWindow(Action<IBaseView> onViewReady) 
            : base (onViewReady)
        {
            InitializeComponent();
            ViewIsReady();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            OnCheckCloudLoginStatus();
        }

        private void btnResume_OnClick(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItems.Count == 0)
                return;

            OnResumePlayback((CloudDeviceInfo) listView.SelectedItems[0]);
        }

        private void btnOpenPreferencesWindow_OnClick(object sender, RoutedEventArgs e)
        {
            OnOpenPreferencesView();
        }

        #region IResumePlaybackView implementation

        public Action<CloudDeviceInfo> OnResumePlayback { get; set; }
        public Action OnOpenPreferencesView { get; set; }
        public Action OnCheckCloudLoginStatus { get; set; }

        public void ResumePlaybackError(Exception ex)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                MessageBox.Show(this, string.Format("An error occured in ResumePlayback: {0}", ex), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }));
        }

        public void RefreshAppLinkedStatus(bool isAppLinked)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                gridLoading.Visibility = Visibility.Hidden; // No more loading from that point
                gridLogin.Visibility = isAppLinked ? Visibility.Hidden : Visibility.Visible;
                gridResumePlayback.Visibility = isAppLinked ? Visibility.Visible : Visibility.Hidden;
            }));            
        }

        public void RefreshDevices(IEnumerable<CloudDeviceInfo> devices)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                _devices = devices.ToList();
                listView.ItemsSource = _devices;
            }));
        }

        #endregion

    }
}
