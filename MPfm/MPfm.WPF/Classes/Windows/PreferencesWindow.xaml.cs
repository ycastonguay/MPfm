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
using System.Windows;
using System.Windows.Threading;
using MPfm.MVP.Models;
using MPfm.MVP.Views;
using MPfm.WPF.Classes.Windows.Base;

namespace MPfm.WPF.Classes.Windows
{
    public partial class PreferencesWindow : BaseWindow, IDesktopPreferencesView
    {
        public PreferencesWindow(Action<IBaseView> onViewReady) 
            : base (onViewReady)
        {
            InitializeComponent();
            ViewIsReady();
        }

        private void btnTab_OnClick(object sender, RoutedEventArgs e)
        {
            gridGeneral.Visibility = sender == btnTabGeneral ? Visibility.Visible : Visibility.Hidden;
            gridAudio.Visibility = sender == btnTabAudio ? Visibility.Visible : Visibility.Hidden;
            gridLibrary.Visibility = sender == btnTabLibrary ? Visibility.Visible : Visibility.Hidden;
            gridCloud.Visibility = sender == btnTabCloud ? Visibility.Visible : Visibility.Hidden;

            if (sender == btnTabGeneral)
                lblTitle.Content = "General Preferences";
            else if (sender == btnTabAudio)
                lblTitle.Content = "Audio Preferences";
            else if (sender == btnTabLibrary)
                lblTitle.Content = "Library Preferences";
            else if (sender == btnTabCloud)
                lblTitle.Content = "Cloud Preferences";
        }

        private void btnAddFolder_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void btnRemoveFolder_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void btnUpdateLibrary_OnClick(object sender, RoutedEventArgs e)
        {
            OnUpdateLibrary();
        }

        private void btnResetLibrary_OnClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure you wish to reset your library?\nThis will *NOT* delete audio files from your hard disk.", "Reset confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                return;

            OnResetLibrary();
        }

        private void btnDropboxLoginLogout_OnClick(object sender, RoutedEventArgs e)
        {
            OnDropboxLoginLogout();
        }

        private void chkDropboxResumePlayback_OnChecked(object sender, RoutedEventArgs e)
        {

        }

        #region ILibraryPreferencesView implementation

        public Action OnResetLibrary { get; set; }
        public Action OnUpdateLibrary { get; set; }
        public Action OnSelectFolders { get; set; }
        public Action OnEnableSyncListener { get; set; }
        public Action<int> OnSetSyncListenerPort { get; set; }

        public void LibraryPreferencesError(Exception ex)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                MessageBox.Show(this, string.Format("An error occured in LibraryPreferences: {0}", ex), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }));
        }

        #endregion

        #region ICloudPreferencesView implementation

        public Action<CloudPreferencesEntity> OnSetCloudPreferences { get; set; }
        public Action OnDropboxLoginLogout { get; set; }

        public void CloudPreferencesError(Exception ex)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                MessageBox.Show(this, string.Format("An error occured in CloudPreferences: {0}", ex), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }));
        }

        public void RefreshCloudPreferences(CloudPreferencesEntity entity)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                chkDropbox_ResumePlayback.IsChecked = entity.IsDropboxResumePlaybackEnabled;
            }));
        }

        public void RefreshCloudPreferencesState(CloudPreferencesStateEntity entity)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                if (entity.IsDropboxLinkedToApp)
                {
                    lblDropbox_Authenticated.Content = "True";
                    lblDropbox_Login.Content = "Logout from Dropbox";
                }
                else
                {
                    lblDropbox_Authenticated.Content = "False";
                    lblDropbox_Login.Content = "Login to Dropbox";
                }
            }));            
        }

        #endregion

    }
}
