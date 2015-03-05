// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using org.sessionsapp.player;
using Sessions.Core.Helpers;
using Sessions.GenericControls.Basics;
using Sessions.Sound.BassNetWrapper;
using Sessions.WPF.Classes.Windows.Base;
using Sessions.Library.Objects;
using Sessions.MVP.Config.Models;
using Sessions.MVP.Models;
using Sessions.MVP.Views;
using Style = System.Windows.Style;

namespace Sessions.WPF.Classes.Windows
{
    public partial class PreferencesWindow : BaseWindow, IDesktopPreferencesView
    {
        private GeneralAppConfig _generalAppConfig;
        private AudioAppConfig _audioAppConfig;
        private LibraryAppConfig _libraryAppConfig;
        private CloudAppConfig _cloudAppConfig;
        private List<SSPDevice> _devices;
        private bool _isRefreshingComboBoxes;

        public PreferencesWindow(Action<IBaseView> onViewReady)
            : base(onViewReady)
        {
            InitializeComponent();
            InitializeTrackBars();
            ViewIsReady();
        }

        private void InitializeTrackBars()
        {
            //var backgroundColor = new BasicColor(242, 242, 242);
            //trackBufferSize.Theme.BackgroundColor = backgroundColor;
            //trackMaximumFolderSize.Theme.BackgroundColor = backgroundColor;
            //trackUpdateFrequency_OutputMeter.Theme.BackgroundColor = backgroundColor;
            //trackUpdateFrequency_SongPosition.Theme.BackgroundColor = backgroundColor;
            //trackUpdatePeriod.Theme.BackgroundColor = backgroundColor;
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

        private void chkLibraryServiceEnabled_OnChecked(object sender, RoutedEventArgs e)
        {
            bool value = false;
            if (chkLibraryServiceEnabled.IsChecked != null)
                value = chkLibraryServiceEnabled.IsChecked.Value;

            OnEnableSyncListener(value);
        }

        #region General Preferences

        private void ChkDownloadAlbumArt_OnChecked(object sender, RoutedEventArgs e)
        {
            _generalAppConfig.DownloadAlbumArtFromTheInternet = chkDownloadAlbumArt.IsChecked.GetValueOrDefault();
            OnSetGeneralPreferences(_generalAppConfig);
        }

        private void ChkShowTooltips_OnChecked(object sender, RoutedEventArgs e)
        {
            _generalAppConfig.ShowTooltips = chkShowTooltips.IsChecked.GetValueOrDefault();
            OnSetGeneralPreferences(_generalAppConfig);
        }

        private void ChkShowAppInSystemTray_OnChecked(object sender, RoutedEventArgs e)
        {
            _generalAppConfig.ShowAppInSystemTray = chkShowAppInSystemTray.IsChecked.GetValueOrDefault();
            MainWindow.EnablePlayerNotifyIcon(_generalAppConfig.ShowAppInSystemTray);
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

        private void TxtMaximumFolderSize_OnTextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void btnRemovePeakFiles_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure you wish to delete the peak files folder?\nPeak files can always be generated later.", "Peak files will be deleted", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
                return;

            OnDeletePeakFiles();
        }

        private void TrackUpdateFrequencySongPosition_OnTrackBarValueChanged()
        {
            if (lblUpdateFrequency_SongPosition == null) return;
            int value = trackUpdateFrequency_SongPosition.Value;
            lblUpdateFrequency_SongPosition.Content = value.ToString();

            _generalAppConfig.SongPositionUpdateFrequency = value;
            OnSetGeneralPreferences(_generalAppConfig);
        }

        private void TrackUpdateFrequencyOutputMeter_OnTrackBarValueChanged()
        {
            if (lblUpdateFrequency_OutputMeter == null) return;
            int value = trackUpdateFrequency_OutputMeter.Value;
            lblUpdateFrequency_OutputMeter.Content = value.ToString();

            _generalAppConfig.OutputMeterUpdateFrequency = value;
            OnSetGeneralPreferences(_generalAppConfig);
        }

        private void TrackMaximumFolderSize_OnTrackBarValueChanged()
        {
            if (lblMaximumFolderSize == null) return;
            int value = trackMaximumFolderSize.Value;
            lblMaximumFolderSize.Content = value.ToString();

            _generalAppConfig.MaximumPeakFolderSize = value;
            OnSetGeneralPreferences(_generalAppConfig);
        }

        #endregion

        #region Audio Preferences

        private void TrackBufferSize_OnTrackBarValueChanged()
        {
            if (lblBufferSize == null || _isRefreshingComboBoxes) return;
            int value = trackBufferSize.Value;
            lblBufferSize.Content = value.ToString();

            _audioAppConfig.BufferSize = value;
            OnSetAudioPreferences(_audioAppConfig);
        }

        private void TrackUpdatePeriod_OnTrackBarValueChanged()
        {
            if (lblUpdatePeriod == null || _isRefreshingComboBoxes) return;
            int value = trackUpdatePeriod.Value;
            lblUpdatePeriod.Content = value.ToString();

            _audioAppConfig.UpdatePeriod = value;
            OnSetAudioPreferences(_audioAppConfig);
        }

        private void btnResetAudioSettings_OnClick(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(this, "Resetting the audio settings will stop the playback.\nIf you click OK, the playback will stop and changes will be applied immediately.\nIf you click Cancel, the changes will be applied the next time the player is reinitialized.", "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Cancel)
                return;

            OnResetAudioSettings();
        }

        private void ComboOutputDevice_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetOutputDeviceAndSampleRate();
        }

        private void ComboSampleRate_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetOutputDeviceAndSampleRate();
        }

        private void SetOutputDeviceAndSampleRate()
        {
            if (comboOutputDevice.SelectedIndex == -1 || comboSampleRate.SelectedIndex == -1 || _isRefreshingComboBoxes)
                return;

            if (OnCheckIfPlayerIsPlaying())
            {
                var result = MessageBox.Show(this, "Changing the output device or sample rate will stop the playback.\nIf you click OK, the playback will stop and changes will be applied immediately.\nIf you click Cancel, the changes will be applied the next time the player is reinitialized.", "Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Cancel)
                    return;
            }

            int sampleRate = 44100;
            if (comboSampleRate.SelectedIndex == 1)
                sampleRate = 48000;
            else if (comboSampleRate.SelectedIndex == 2)
                sampleRate = 96000;
            var device = _devices[comboOutputDevice.SelectedIndex];
            if (device != null)
                OnSetOutputDeviceAndSampleRate(device, sampleRate);
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

            OnAddFolder(dialog.SelectedPath, true);
        }

        private void btnRemoveFolder_OnClick(object sender, RoutedEventArgs e)
        {
            if (listViewFolders.SelectedItem == null)
                return;

            var result = MessageBox.Show(this, "Do you wish to also remove the audio files from your library?\nNote: This will only remove audio files from the application, not from your hard disk.", "Folder removal confirmation", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            if(result == MessageBoxResult.Cancel)
                return;

            bool removeAudioFilesFromLibrary = result == MessageBoxResult.Yes;
            var folders = listViewFolders.SelectedItems;
            var list = new List<Folder>();
            foreach (Folder folder in folders)
                list.Add(folder);
            OnRemoveFolders(list, removeAudioFilesFromLibrary);
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
        public Action<string, bool> OnAddFolder { get; set; }
        public Action<IEnumerable<Folder>, bool> OnRemoveFolders { get; set; }
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
                    btnDropboxLoginLogout.Title = "Logout from Dropbox";
                }
                else
                {
                    lblDropbox_Authenticated.Content = "False";
                    btnDropboxLoginLogout.Title = "Login to Dropbox";
                }
            }));            
        }

        #endregion

        #region IAudioPreferencesView implementation

        public Action<AudioAppConfig> OnSetAudioPreferences { get; set; }
        public Action<SSPDevice, int> OnSetOutputDeviceAndSampleRate { get; set; }
        public Action OnResetAudioSettings { get; set; }
        public Func<bool> OnCheckIfPlayerIsPlaying { get; set; } 

        public void AudioPreferencesError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void RefreshAudioPreferences(AudioAppConfig config)
        {
            _audioAppConfig = config;
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                _isRefreshingComboBoxes = true;
                trackBufferSize.Value = config.BufferSize;
                trackUpdatePeriod.Value = config.UpdatePeriod;
                lblBufferSize.Content = config.BufferSize.ToString();
                lblUpdatePeriod.Content = config.UpdatePeriod.ToString();
                comboOutputDevice.SelectedIndex = _devices.FindIndex(d => d.DeviceId == config.AudioDevice.DeviceId);
                if (comboOutputDevice.SelectedIndex == -1)
                    comboOutputDevice.SelectedIndex = 0;

                if (config.SampleRate == 44100)
                    comboSampleRate.SelectedIndex = 0;
                else if (config.SampleRate == 48000)
                    comboSampleRate.SelectedIndex = 1;
                else if (config.SampleRate == 96000)
                    comboSampleRate.SelectedIndex = 2;
                else
                    comboSampleRate.SelectedIndex = -1;

                _isRefreshingComboBoxes = false;
            }));
        }

        public void RefreshAudioDevices(IEnumerable<SSPDevice> devices)
        {
            _devices = devices.ToList();
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                _isRefreshingComboBoxes = true;
                comboOutputDevice.ItemsSource = devices.ToList();
                comboOutputDevice.SelectedIndex = 0;
                _isRefreshingComboBoxes = false;
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
                trackUpdateFrequency_SongPosition.Value = config.SongPositionUpdateFrequency;
                trackUpdateFrequency_OutputMeter.Value = config.OutputMeterUpdateFrequency;
                lblUpdateFrequency_OutputMeter.Content = config.OutputMeterUpdateFrequency.ToString();
                lblUpdateFrequency_SongPosition.Content = config.SongPositionUpdateFrequency.ToString();

                chkDownloadAlbumArt.IsChecked = config.DownloadAlbumArtFromTheInternet;
                chkShowTooltips.IsChecked = config.ShowTooltips;
                chkShowAppInSystemTray.IsChecked = config.ShowAppInSystemTray;
                chkMinimizeAppInSystemTray.IsChecked = config.MinimizeAppInSystemTray;

                radioPeakFiles_UseCustomDirectory.IsChecked = config.UseCustomPeakFileFolder;
                radioPeakFiles_UseDefaultDirectory.IsChecked = !config.UseCustomPeakFileFolder;
                trackMaximumFolderSize.Value = config.MaximumPeakFolderSize;
                lblMaximumFolderSize.Content = config.MaximumPeakFolderSize.ToString();
                txtPeakFiles_CustomDirectory.Text = config.CustomPeakFileFolder;

                lblPeakFolderSize.Content = string.Format("Peak file folder size: {0}", peakFolderSize);
                radioPeakFiles_UseDefaultDirectory.Content = string.Format("Use default directory ({0})", PathHelper.PeakFileDirectory);
            }));
        }

        #endregion

    }
}
