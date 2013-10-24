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

        #region IDesktopPreferencesView implementation

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

    }
}
