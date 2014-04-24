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
using System.Windows.Controls;
using System.Windows.Threading;
using MPfm.Library.Objects;
using MPfm.MVP.Config.Models;
using MPfm.MVP.Models;
using MPfm.MVP.Views;
using MPfm.WPF.Classes.Windows.Base;
using Style = System.Windows.Style;

namespace MPfm.WPF.Classes.Windows
{
    public partial class PreferencesWindow : BaseWindow, IDesktopPreferencesView
    {
        private GeneralAppConfig _generalAppConfig;
        private AudioAppConfig _audioAppConfig;
        private LibraryAppConfig _libraryAppConfig;
        private CloudAppConfig _cloudAppConfig;

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

            ResetHeaderButtonStyles();
            var button = sender as Button;
            button.Style = Application.Current.Resources["TabButtonSelected"] as Style;
        }

        private void ResetHeaderButtonStyles()
        {
            var res = Application.Current.Resources;
            btnTabGeneral.Style = res["TabButton"] as Style;
            btnTabAudio.Style = res["TabButton"] as Style;
            btnTabLibrary.Style = res["TabButton"] as Style;
            btnTabCloud.Style = res["TabButton"] as Style;
        }

        #region General Preferences

        private void sliderOutputMeter_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (txtUpdateFrequency_OutputMeter == null) return;
            int value = (int)sliderUpdateFrequency_OutputMeter.Value;
            txtUpdateFrequency_OutputMeter.Text = value.ToString();

            _generalAppConfig.OutputMeterUpdateFrequency = value;
            OnSetGeneralPreferences(_generalAppConfig);
        }

        private void sliderSongPosition_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (txtUpdateFrequency_SongPosition == null) return;
            int value = (int)sliderUpdateFrequency_SongPosition.Value;
            txtUpdateFrequency_SongPosition.Text = value.ToString();

            _generalAppConfig.SongPositionUpdateFrequency = value;
            OnSetGeneralPreferences(_generalAppConfig);
        }

        private void TxtUpdateFrequency_SongPosition_OnTextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void TxtUpdateFrequency_OutputMeter_OnTextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void ChkShowTooltips_OnChecked(object sender, RoutedEventArgs e)
        {
            _generalAppConfig.ShowTooltips = chkShowTooltips.IsChecked.GetValueOrDefault();
            OnSetGeneralPreferences(_generalAppConfig);
        }

        private void ChkShowAppInSystemTray_OnChecked(object sender, RoutedEventArgs e)
        {
            _generalAppConfig.ShowAppInSystemTray = chkShowAppInSystemTray.IsChecked.GetValueOrDefault();
            OnSetGeneralPreferences(_generalAppConfig);
        }

        private void ChkMinimizeAppInSystemTray_OnChecked(object sender, RoutedEventArgs e)
        {
            _generalAppConfig.MinimizeAppInSystemTray = chkMinimizeAppInSystemTray.IsChecked.GetValueOrDefault();
            OnSetGeneralPreferences(_generalAppConfig);
        }

        private void RadioPeakFiles_UseDefaultDirectory_OnChecked(object sender, RoutedEventArgs e)
        {
            bool value = radioPeakFiles_UseCustomDirectory.IsChecked.GetValueOrDefault();
            btnBrowseCustomDirectory.IsEnabled = value;
            _generalAppConfig.UseCustomPeakFileFolder = value;
            OnSetGeneralPreferences(_generalAppConfig);
        }

        private void RadioPeakFiles_UseCustomDirectory_OnChecked(object sender, RoutedEventArgs e)
        {
            bool value = radioPeakFiles_UseCustomDirectory.IsChecked.GetValueOrDefault();
            btnBrowseCustomDirectory.IsEnabled = value;
            _generalAppConfig.UseCustomPeakFileFolder = radioPeakFiles_UseCustomDirectory.IsChecked.GetValueOrDefault();
            OnSetGeneralPreferences(_generalAppConfig);
        }

        private void btnBrowseCustomDirectory_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "Please select a folder for peak files";
            dialog.ShowNewFolderButton = true;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtPeakFiles_CustomDirectory.Text = dialog.SelectedPath;
                _generalAppConfig.CustomPeakFileFolder = dialog.SelectedPath;
                OnSetGeneralPreferences(_generalAppConfig);
            }
        }

        private void sliderMaximumFolderSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (txtMaximumFolderSize == null) return;
            int value = (int)sliderMaximumFolderSize.Value;
            txtMaximumFolderSize.Text = value.ToString();

            _generalAppConfig.MaximumPeakFolderSize = value;
            OnSetGeneralPreferences(_generalAppConfig);
        }

        private void TxtMaximumFolderSize_OnTextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void btnRemovePeakFiles_Click(object sender, RoutedEventArgs e)
        {
        }

        #endregion

        #region Library Preferences

        private void btnAddFolder_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "Please select a folder to add to the music library";
            dialog.ShowNewFolderButton = false;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return;

            _libraryAppConfig.Folders.Add(new Folder()
            {
                FolderPath = dialog.SelectedPath,
                IsRecursive = true
            });
            OnSetLibraryPreferences(_libraryAppConfig);
        }

        private void btnRemoveFolder_OnClick(object sender, RoutedEventArgs e)
        {
            if (listViewFolders.SelectedItem == null)
                return;

            var result = MessageBox.Show(this, "Do you wish to also remove the audio files from your library?\nNote: This will only remove audio files from the application, not from your hard disk.", "Folder removal confirmation", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            if(result == MessageBoxResult.Cancel)
                return;

            if (result == MessageBoxResult.Yes)
            {
                // TODO: Remove files from library
            }

            var folders = listViewFolders.SelectedItems;
            foreach (Folder folder in folders)
                _libraryAppConfig.Folders.Remove(folder);
            OnSetLibraryPreferences(_libraryAppConfig);
        }

        private void btnUpdateLibrary_OnClick(object sender, RoutedEventArgs e)
        {
            OnUpdateLibrary();
        }

        private void btnResetLibrary_OnClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure you wish to reset your library?\nNote: This will only remove audio files from the application, not from your hard disk.", "Reset confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                return;

            OnResetLibrary();
        }

        #endregion

        #region Cloud Preferences

        private void btnDropboxLoginLogout_OnClick(object sender, RoutedEventArgs e)
        {
            OnDropboxLoginLogout();
        }

        private void chkDropboxResumePlayback_OnChecked(object sender, RoutedEventArgs e)
        {
            bool value = chkDropbox_ResumePlayback.IsChecked.HasValue && chkDropbox_ResumePlayback.IsChecked.Value;
            _cloudAppConfig.IsResumePlaybackEnabled = value;
            OnSetCloudPreferences(_cloudAppConfig);
        }

        #endregion

        #region ILibraryPreferencesView implementation

        public Action<LibraryAppConfig> OnSetLibraryPreferences { get; set; }
        public Action OnResetLibrary { get; set; }
        public Action OnUpdateLibrary { get; set; }        
        public Action OnSelectFolders { get; set; }
        public Action<bool> OnEnableSyncListener { get; set; }
        public Action<int> OnSetSyncListenerPort { get; set; }

        public void LibraryPreferencesError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void RefreshLibraryPreferences(LibraryAppConfig config, string librarySize)
        {
            _libraryAppConfig = config;
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                lblLibrarySize.Content = string.Format("Library size: {0}", librarySize);
                listViewFolders.ItemsSource = config.Folders;
                listViewFolders.Items.Refresh();
            }));
        }

        #endregion

        #region ICloudPreferencesView implementation

        public Action<CloudAppConfig> OnSetCloudPreferences { get; set; }
        public Action OnDropboxLoginLogout { get; set; }

        public void CloudPreferencesError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void RefreshCloudPreferences(CloudAppConfig config)
        {
            _cloudAppConfig = config;
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                chkDropbox_ResumePlayback.IsChecked = config.IsResumePlaybackEnabled;
            }));
        }

        public void RefreshCloudPreferencesState(CloudPreferencesStateEntity entity)
        {
            Console.WriteLine("PreferencesWindow - IsDropboxLinkedToApp: {0}", entity.IsDropboxLinkedToApp);
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

        #region IAudioPreferencesView implementation

        public Action<AudioAppConfig> OnSetAudioPreferences { get; set; }

        public void AudioPreferencesError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void RefreshAudioPreferences(AudioAppConfig config)
        {
            _audioAppConfig = config;
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
            }));
        }

        #endregion

        #region IGeneralPreferencesView implementation

        public Action<GeneralAppConfig> OnSetGeneralPreferences { get; set; }
        public Action OnDeletePeakFiles { get; set; }

        public void GeneralPreferencesError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void RefreshGeneralPreferences(GeneralAppConfig config, string peakFolderSize)
        {
            _generalAppConfig = config;
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                sliderUpdateFrequency_SongPosition.Value = config.SongPositionUpdateFrequency;
                sliderUpdateFrequency_OutputMeter.Value = config.OutputMeterUpdateFrequency;
                txtUpdateFrequency_OutputMeter.Text = config.OutputMeterUpdateFrequency.ToString();
                txtUpdateFrequency_SongPosition.Text = config.SongPositionUpdateFrequency.ToString();

                chkShowTooltips.IsChecked = config.ShowTooltips;
                chkShowAppInSystemTray.IsChecked = config.ShowAppInSystemTray;
                chkMinimizeAppInSystemTray.IsChecked = config.MinimizeAppInSystemTray;

                radioPeakFiles_UseCustomDirectory.IsChecked = config.UseCustomPeakFileFolder;
                sliderMaximumFolderSize.Value = config.MaximumPeakFolderSize;
                txtMaximumFolderSize.Text = config.MaximumPeakFolderSize.ToString();
                txtPeakFiles_CustomDirectory.Text = config.CustomPeakFileFolder;
            }));
        }

        #endregion

        private void ComboOutputDevice_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void ComboSampleRate_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void sliderBufferSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

        private void TxtBufferSize_OnTextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void sliderUpdatePeriod_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

        private void TxtUpdatePeriod_OnTextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void btnTestAudioSettings_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void btnResetAudioSettings_OnClick(object sender, RoutedEventArgs e)
        {
        }
    }
}
