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
using MonoMac.Foundation;
using MonoMac.AppKit;
using MPfm.MVP;
using MPfm.MVP.Views;
using MPfm.MVP.Config.Models;
using MPfm.MVP.Models;
using MPfm.Mac.Classes.Controls;
using MPfm.Mac.Classes.Objects;
using MPfm.Core.Helpers;
using MPfm.Mac.Classes.Helpers;
using System.IO;

namespace MPfm.Mac
{
    public partial class PreferencesWindowController : BaseWindowController, IDesktopPreferencesView
    {
        private GeneralAppConfig _generalAppConfig;
        private AudioAppConfig _audioAppConfig;
        private LibraryAppConfig _libraryAppConfig;
        private CloudAppConfig _cloudAppConfig;

        public PreferencesWindowController(IntPtr handle) 
            : base (handle)
        {
            Initialize();
        }
        
        public PreferencesWindowController(Action<IBaseView> onViewReady)
            : base ("PreferencesWindow", onViewReady)
        {
            Initialize();
        }
        
        private void Initialize()
        {
            LoadTrackBars();
            LoadComboBoxes();
            LoadTextBoxes();
            LoadFontsAndImages();
            BindButtons();
            HandleOnTabButtonSelected(btnTabGeneral);
            ShowWindowCentered();
        }

        public override void WindowDidLoad()
        {
            base.WindowDidLoad();
            OnViewReady(this);
        }

        private void LoadComboBoxes()
        {
            popupSampleRate.RemoveAllItems();
            popupSampleRate.AddItem("44100 Hz");
            popupSampleRate.AddItem("48000 Hz");
            popupSampleRate.AddItem("96000 Hz");

            popupOutputDevice.RemoveAllItems();
            popupOutputDevice.AddItem("Default device");
        }

        private void LoadTextBoxes()
        {
            txtBufferSize.WeakDelegate = this;
            txtCustomDirectory.WeakDelegate = this;
            txtMaximumPeakFolderSize.WeakDelegate = this;
            txtOutputMeter.WeakDelegate = this;
            txtSongPosition.WeakDelegate = this;
            txtUpdatePeriod.WeakDelegate = this;

            var cell = txtCustomDirectory.Cell as NSTextFieldCell;
            cell.PlaceholderString = PathHelper.PeakFileDirectory;
        }

        private void BindButtons()
        {
            btnTabGeneral.OnTabButtonSelected += HandleOnTabButtonSelected;
            btnTabAudio.OnTabButtonSelected += HandleOnTabButtonSelected;
            btnTabLibrary.OnTabButtonSelected += HandleOnTabButtonSelected;
            btnTabCloud.OnTabButtonSelected += HandleOnTabButtonSelected;
            btnLoginDropbox.OnButtonSelected += (button) => OnDropboxLoginLogout();
            btnDeletePeakFiles.OnButtonSelected += HandleOnDeletePeakFilesButtonSelected;
            btnBrowseCustomDirectory.OnButtonSelected += HandleOnBrowseCustomDirectoryButtonSelected;
        }

        private void LoadTrackBars()
        {
            trackSongPosition.Minimum = 10;
            trackSongPosition.Maximum = 100;
            trackSongPosition.Value = 10;
            trackOutputMeter.Minimum = 10;
            trackOutputMeter.Maximum = 100;
            trackOutputMeter.Value = 10;
            trackMaximumPeakFolderSize.Minimum = 50;
            trackMaximumPeakFolderSize.Maximum = 10000;
            trackMaximumPeakFolderSize.Value = 100;
            trackUpdatePeriod.Minimum = 10;
            trackUpdatePeriod.Maximum = 100;
            trackUpdatePeriod.Value = 10;
            trackBufferSize.Minimum = 100;
            trackBufferSize.Maximum = 5000;
            trackBufferSize.Value = 100;

            trackOutputMeter.OnTrackBarValueChanged += HandleOnOutputMeterTrackBarValueChanged; 
            trackSongPosition.OnTrackBarValueChanged += HandleOnSongPositionTrackBarValueChanged;
            trackBufferSize.OnTrackBarValueChanged += HandleOnBufferSizeTrackBarValueChanged;
            trackUpdatePeriod.OnTrackBarValueChanged += HandleOnUpdatePeriodTrackBarValueChanged;
            trackMaximumPeakFolderSize.OnTrackBarValueChanged += HandleOnMaximumPeakFolderSizeTrackBarValueChanged;
        }

        private void LoadFontsAndImages()
        {
            viewGeneralPreferencesHeader.BackgroundColor1 = GlobalTheme.PanelHeaderColor1;
            viewGeneralPreferencesHeader.BackgroundColor2 = GlobalTheme.PanelHeaderColor2;
            viewAudioPreferencesHeader.BackgroundColor1 = GlobalTheme.PanelHeaderColor1;
            viewAudioPreferencesHeader.BackgroundColor2 = GlobalTheme.PanelHeaderColor2;
            viewLibraryPreferencesHeader.BackgroundColor1 = GlobalTheme.PanelHeaderColor1;
            viewLibraryPreferencesHeader.BackgroundColor2 = GlobalTheme.PanelHeaderColor2;
            viewCloudPreferencesHeader.BackgroundColor1 = GlobalTheme.PanelHeaderColor1;
            viewCloudPreferencesHeader.BackgroundColor2 = GlobalTheme.PanelHeaderColor2;

            viewOutputHeader.BackgroundColor1 = GlobalTheme.PanelHeader2Color1;
            viewOutputHeader.BackgroundColor2 = GlobalTheme.PanelHeader2Color2;
            viewMixerHeader.BackgroundColor1 = GlobalTheme.PanelHeader2Color1;
            viewMixerHeader.BackgroundColor2 = GlobalTheme.PanelHeader2Color2;
            viewStatusHeader.BackgroundColor1 = GlobalTheme.PanelHeader2Color1;
            viewStatusHeader.BackgroundColor2 = GlobalTheme.PanelHeader2Color2;
            viewDropboxHeader.BackgroundColor1 = GlobalTheme.PanelHeader2Color1;
            viewDropboxHeader.BackgroundColor2 = GlobalTheme.PanelHeader2Color2;
            viewUpdateFrequencyHeader.BackgroundColor1 = GlobalTheme.PanelHeader2Color1;
            viewUpdateFrequencyHeader.BackgroundColor2 = GlobalTheme.PanelHeader2Color2;
            viewFoldersHeader.BackgroundColor1 = GlobalTheme.PanelHeader2Color1;
            viewFoldersHeader.BackgroundColor2 = GlobalTheme.PanelHeader2Color2;
            viewPeakFilesHeader.BackgroundColor1 = GlobalTheme.PanelHeader2Color1;
            viewPeakFilesHeader.BackgroundColor2 = GlobalTheme.PanelHeader2Color2;
            viewLibraryServiceHeader.BackgroundColor1 = GlobalTheme.PanelHeader2Color1;
            viewLibraryServiceHeader.BackgroundColor2 = GlobalTheme.PanelHeader2Color2;

            var headerFont = NSFont.FromFontName("Roboto Light", 16f);
            lblGeneralPreferences.Font = headerFont;
            lblAudioPreferences.Font = headerFont;
            lblLibraryPreferences.Font = headerFont;
            lblCloudPreferences.Font = headerFont;

            var subtitleFont = NSFont.FromFontName("Roboto Light", 13f);
            lblGeneralUpdateFrequency.Font = subtitleFont;
            lblGeneralPeakFiles.Font = subtitleFont;
            lblAudioOutput.Font = subtitleFont;
            lblAudioMixer.Font = subtitleFont;
            lblAudioStatus.Font = subtitleFont;
            lblLibraryFolders.Font = subtitleFont;
            lblCloudDropbox.Font = subtitleFont;
            lblLibraryService.Font = subtitleFont;

            var textFont = NSFont.FromFontName("Roboto", 12f);
            var textColor = NSColor.FromDeviceRgba(0.85f, 0.85f, 0.85f, 1);
            lblOutputDevice.Font = textFont;
            lblOutputDevice.TextColor = textColor;
            lblSampleRate.Font = textFont;
            lblSampleRate.TextColor = textColor;
            lblStatusDescription.Font = textFont;
            lblStatusDescription.TextColor = textColor;
            lblUpdatePeriod.Font = textFont;
            lblUpdatePeriod.TextColor = textColor;
            lblSongPosition.Font = textFont;
            lblSongPosition.TextColor = textColor;
            lblOutputMeter.Font = textFont;
            lblOutputMeter.TextColor = textColor;
            lblBufferSize.Font = textFont;
            lblBufferSize.TextColor = textColor;
            lblMaximumPeakFolderSize.Font = textFont;
            lblMaximumPeakFolderSize.TextColor = textColor;
            lblEvery.Font = textFont;
            lblEvery.TextColor = textColor;
            lblEvery2.Font = textFont;
            lblEvery2.TextColor = textColor;
            lblEvery3.Font = textFont;
            lblEvery3.TextColor = textColor;
            lblEvery4.Font = textFont;
            lblEvery4.TextColor = textColor;
            lblMS.Font = textFont;
            lblMS.TextColor = textColor;
            lblMS2.Font = textFont;
            lblMS2.TextColor = textColor;
            lblMS3.Font = textFont;
            lblMS3.TextColor = textColor;
            lblMS4.Font = textFont;
            lblMS4.TextColor = textColor;
            lblHz.Font = textFont;
            lblHz.TextColor = textColor;
            lblMS.Font = textFont;
            lblMS.TextColor = textColor;
            lblHttpPort.Font = textFont;
            lblHttpPort.TextColor = textColor;

            var noteFont = NSFont.FromFontName("Roboto", 11f);
            var noteColor = NSColor.FromDeviceRgba(0.85f, 0.85f, 0.85f, 1);
            lblResumePlaybackNote.Font = noteFont;
            lblResumePlaybackNote.TextColor = noteColor;           
            lblPeakFileFolderSize.Font = noteFont;
            lblPeakFileFolderSize.TextColor = noteColor;           
            lblLibrarySize.Font = noteFont;
            lblLibrarySize.TextColor = noteColor;           
            lblLibraryServiceNote.Font = noteFont;
            lblLibraryServiceNote.TextColor = noteColor;           

            var textBoxFont = NSFont.FromFontName("Roboto", 12f);
            txtBufferSize.Font = textBoxFont;
            txtCustomDirectory.Font = textBoxFont;
            txtMaximumPeakFolderSize.Font = textBoxFont;
            txtOutputMeter.Font = textBoxFont;
            txtSongPosition.Font = textBoxFont;
            txtUpdatePeriod.Font = textBoxFont;
            txtHttpPort.Font = textBoxFont;

            // The NSButton checkbox type doesn't let you change the color, so use an attributed string instead
            var dictAttrStr1 = new NSMutableDictionary();
            dictAttrStr1.Add(NSAttributedString.ForegroundColorAttributeName, NSColor.White);
            dictAttrStr1.Add(NSAttributedString.FontAttributeName, NSFont.FromFontName("Roboto", 12));
            var attrStrEnableResumePlayback = new NSAttributedString("Enable Resume Playback with Dropbox", dictAttrStr1);
            checkEnableResumePlayback.AttributedTitle = attrStrEnableResumePlayback;

            // The NSButton checkbox type doesn't let you change the color, so use an attributed string instead
            var attrStrEnableLibraryService = new NSAttributedString("Enable Library Service", dictAttrStr1);
            checkEnableLibraryService.AttributedTitle = attrStrEnableLibraryService;

            // NSMatrix doesn't allow changing text color, so use an attributed string instead
            var dictAttrStr2 = new NSMutableDictionary();
            dictAttrStr2.Add(NSAttributedString.ForegroundColorAttributeName, NSColor.FromDeviceRgba(0.85f, 0.85f, 0.85f, 1));
            dictAttrStr2.Add(NSAttributedString.FontAttributeName, NSFont.FromFontName("Roboto", 12));
            var radioStr1 = new NSAttributedString(string.Format("Use default directory ({0})", PathHelper.PeakFileDirectory), dictAttrStr2);
            var radioStr2 = new NSAttributedString("Use custom directory:", dictAttrStr2);
            var firstCell = matrixPeakFiles.Cells[0] as NSButtonCell;
            var secondCell = matrixPeakFiles.Cells[1] as NSButtonCell;
            firstCell.AttributedTitle = radioStr1;
            secondCell.AttributedTitle = radioStr2;

            btnTabGeneral.LineLocation = MPfmTabButton.LineLocationEnum.Top;
            btnTabGeneral.ShowSelectedBackgroundColor = true;
            btnTabGeneral.BackgroundColor = GlobalTheme.SettingsTabColor;
            btnTabGeneral.BackgroundMouseDownColor = GlobalTheme.SettingsTabOverColor;
            btnTabGeneral.BackgroundMouseOverColor = GlobalTheme.SettingsTabOverColor;
            btnTabGeneral.BackgroundSelectedColor = GlobalTheme.SettingsTabSelectedColor;
            btnTabAudio.LineLocation = MPfmTabButton.LineLocationEnum.Top;
            btnTabAudio.ShowSelectedBackgroundColor = true;
            btnTabAudio.BackgroundColor = GlobalTheme.SettingsTabColor;
            btnTabAudio.BackgroundMouseDownColor = GlobalTheme.SettingsTabOverColor;
            btnTabAudio.BackgroundMouseOverColor = GlobalTheme.SettingsTabOverColor;
            btnTabAudio.BackgroundSelectedColor = GlobalTheme.SettingsTabSelectedColor;
            btnTabLibrary.LineLocation = MPfmTabButton.LineLocationEnum.Top;
            btnTabLibrary.ShowSelectedBackgroundColor = true;
            btnTabLibrary.BackgroundColor = GlobalTheme.SettingsTabColor;
            btnTabLibrary.BackgroundMouseDownColor = GlobalTheme.SettingsTabOverColor;
            btnTabLibrary.BackgroundMouseOverColor = GlobalTheme.SettingsTabOverColor;
            btnTabLibrary.BackgroundSelectedColor = GlobalTheme.SettingsTabSelectedColor;
            btnTabCloud.LineLocation = MPfmTabButton.LineLocationEnum.Top;
            btnTabCloud.ShowSelectedBackgroundColor = true;
            btnTabCloud.BackgroundColor = GlobalTheme.SettingsTabColor;
            btnTabCloud.BackgroundMouseDownColor = GlobalTheme.SettingsTabOverColor;
            btnTabCloud.BackgroundMouseOverColor = GlobalTheme.SettingsTabOverColor;
            btnTabCloud.BackgroundSelectedColor = GlobalTheme.SettingsTabSelectedColor;

            btnTabGeneral.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_general");
            btnTabAudio.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_speaker");
            btnTabLibrary.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_library");
            btnTabCloud.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_cloud");

            btnAddFolder.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_add");
            btnRemoveFolder.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_delete");
            btnResetLibrary.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_reset");
            btnTestAudioSettings.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_test");
            btnResetAudioSettings.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_reset");
            btnLoginDropbox.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_dropbox");
            btnBrowseCustomDirectory.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_open");
            btnDeletePeakFiles.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_delete");
        }

        private void HandleOnSongPositionTrackBarValueChanged()
        {
            int value = (int)trackSongPosition.Value;
            txtSongPosition.StringValue = value.ToString();

            _generalAppConfig.SongPositionUpdateFrequency = value;
            OnSetGeneralPreferences(_generalAppConfig);    
        }

        private void HandleOnOutputMeterTrackBarValueChanged()
        {
            int value = (int)trackOutputMeter.Value;
            txtOutputMeter.StringValue = value.ToString();

            _generalAppConfig.OutputMeterUpdateFrequency = value;
            OnSetGeneralPreferences(_generalAppConfig);    
        }

        private void HandleOnMaximumPeakFolderSizeTrackBarValueChanged()
        {
            int value = (int)trackMaximumPeakFolderSize.Value;
            txtMaximumPeakFolderSize.StringValue = value.ToString();

            _generalAppConfig.MaximumPeakFolderSize = value;
            OnSetGeneralPreferences(_generalAppConfig);
        }

        private void HandleOnBufferSizeTrackBarValueChanged()
        {
            int value = (int)trackBufferSize.Value;
            txtBufferSize.StringValue = value.ToString();

            _audioAppConfig.BufferSize = value;
            OnSetAudioPreferences(_audioAppConfig);
        }

        private void HandleOnUpdatePeriodTrackBarValueChanged()
        {
            int value = (int)trackUpdatePeriod.Value;
            txtUpdatePeriod.StringValue = value.ToString();

            _audioAppConfig.UpdatePeriod = value;
            OnSetAudioPreferences(_audioAppConfig);
        }

        private void HandleOnTabButtonSelected(MPfmTabButton button)
        {
            btnTabGeneral.IsSelected = button == btnTabGeneral;
            btnTabAudio.IsSelected = button == btnTabAudio;
            btnTabLibrary.IsSelected = button == btnTabLibrary;
            btnTabCloud.IsSelected = button == btnTabCloud;

            viewGeneralPreferences.Hidden = button != btnTabGeneral;
            viewAudioPreferences.Hidden = button != btnTabAudio;
            viewLibraryPreferences.Hidden = button != btnTabLibrary;
            viewCloudPreferences.Hidden = button != btnTabCloud;
        }

        private void HandleOnDeletePeakFilesButtonSelected(MPfmButton button)
        {
            using(var alert = new NSAlert())
            {
                alert.MessageText = "Peak files will be deleted";
                alert.InformativeText = "Are you sure you wish to delete the peak files folder? Peak files can always be generated later.";
                alert.AlertStyle = NSAlertStyle.Warning;
                var btnOK = alert.AddButton("OK");
                btnOK.Activated += (sender2, e2) => {
                    NSApplication.SharedApplication.StopModal();
                    OnDeletePeakFiles();
                };
                var btnCancel = alert.AddButton("Cancel");
                btnCancel.Activated += (sender3, e3) => NSApplication.SharedApplication.StopModal();
                alert.RunModal();
            }
        }

        private void HandleOnBrowseCustomDirectoryButtonSelected(MPfmButton button)
        {
            string folderPath = string.Empty;
            using(NSOpenPanel openPanel = new NSOpenPanel())
            {
                openPanel.CanChooseDirectories = true;
                openPanel.CanChooseFiles = false;
                openPanel.ReleasedWhenClosed = true;
                openPanel.AllowsMultipleSelection = false;
                openPanel.Title = "Please select a folder for peak files";
                openPanel.Prompt = "Select folder for peak files"; 
                openPanel.RunModal();
                folderPath = openPanel.Url.Path;
            }

            if (!String.IsNullOrEmpty(folderPath))
                txtCustomDirectory.StringValue = folderPath;
        }

        [Export("controlTextDidChange:")]
        private void ControlTextDidChange(NSNotification notification)
        {
            Console.WriteLine("ControlTextDidChange");
            var textField = notification.Object as NSTextField;
            if (textField == null)
                return;

        }

        [Export("controlTextDidEndEditing:")]
        private void ControlTextDidEndEditing(NSNotification notification)
        {
            Console.WriteLine("ControlTextDidEndEditing");
            var textField = notification.Object as NSTextField;
            if (textField == null)
                return;

            if (textField == txtCustomDirectory)
            {
                // Should this be done in the presenter instead? 
                if (!Directory.Exists(textField.StringValue))
                {
                    using(var alert = new NSAlert())
                    {
                        alert.MessageText = "Path is invalid";
                        alert.InformativeText = "The path you have entered is invalid or the folder does not exist.";
                        alert.AlertStyle = NSAlertStyle.Critical;
                        var btnOK = alert.AddButton("OK");
                        btnOK.Activated += (sender2, e2) => textField.StringValue = string.Empty;
                    }
                    return;
                }

                _generalAppConfig.CustomPeakFileFolder = textField.StringValue;
                OnSetGeneralPreferences(_generalAppConfig);
            }
        }

        partial void actionMatrixPeakFiles(NSObject sender)
        {
            Console.WriteLine("actionMatrixPeakFiles - matrixPeakFiles.SelectedRow: {0}", matrixPeakFiles.SelectedRow); 
            bool useCustomPeakFolder = matrixPeakFiles.SelectedRow == 1;
            btnBrowseCustomDirectory.Enabled = useCustomPeakFolder;

            // Update 
            if(_generalAppConfig.UseCustomPeakFileFolder != useCustomPeakFolder)
            {
                _generalAppConfig.UseCustomPeakFileFolder = useCustomPeakFolder;
                OnSetGeneralPreferences(_generalAppConfig);
            }
        }

        #region ILibraryPreferencesView implementation

        public Action OnResetLibrary { get; set; }
        public Action OnUpdateLibrary { get; set; }
        public Action OnSelectFolders { get; set; }
        public Action<bool> OnEnableSyncListener { get; set; }
        public Action<int> OnSetSyncListenerPort { get; set; }
        public Action<LibraryAppConfig> OnSetLibraryPreferences { get; set; }

        public void LibraryPreferencesError(Exception ex)
        {
            ShowError(ex);
        }

        public void RefreshLibraryPreferences(LibraryAppConfig config, string librarySize)
        {
            InvokeOnMainThread(() => {
                lblLibrarySize.StringValue = string.Format("Library size: {0}", librarySize);
            });
        }

        #endregion

        #region IGeneralPreferencesView implementation

        public Action<GeneralAppConfig> OnSetGeneralPreferences { get; set; }
        public Action OnDeletePeakFiles { get; set; }

        public void GeneralPreferencesError(Exception ex)
        {
            ShowError(ex);
        }

        public void RefreshGeneralPreferences(GeneralAppConfig config, string peakFolderSize)
        {
            _generalAppConfig = config;
            InvokeOnMainThread(() => {
                lblPeakFileFolderSize.StringValue = string.Format("Peak file folder size: {0}", peakFolderSize);
                trackSongPosition.Value = config.SongPositionUpdateFrequency;
                trackOutputMeter.Value = config.OutputMeterUpdateFrequency;
                trackMaximumPeakFolderSize.Value = config.MaximumPeakFolderSize;
                txtSongPosition.StringValue = config.SongPositionUpdateFrequency.ToString();
                txtOutputMeter.StringValue = config.OutputMeterUpdateFrequency.ToString();
                txtMaximumPeakFolderSize.StringValue = config.MaximumPeakFolderSize.ToString();
                matrixPeakFiles.SelectCell(config.UseCustomPeakFileFolder ? 1 : 0, 0);
                btnBrowseCustomDirectory.Enabled = matrixPeakFiles.SelectedRow == 1;
            });
        }

        #endregion

        #region ICloudPreferencesView implementation

        public Action<CloudAppConfig> OnSetCloudPreferences { get; set; }
        public Action OnDropboxLoginLogout { get; set; }

        public void CloudPreferencesError(Exception ex)
        {
            ShowError(ex);
        }

        public void RefreshCloudPreferences(CloudAppConfig config)
        {
            InvokeOnMainThread(() => {
                checkEnableResumePlayback.State = config.IsResumePlaybackEnabled ? NSCellStateValue.On : NSCellStateValue.Off;
            });
        }

        public void RefreshCloudPreferencesState(CloudPreferencesStateEntity entity)
        {
            InvokeOnMainThread(() => {
                btnLoginDropbox.Title = entity.IsDropboxLinkedToApp ? "Logout from Dropbox" : "Login to Dropbox";
            });
        }

        #endregion

        #region IAudioPreferencesView implementation

        public Action<AudioAppConfig> OnSetAudioPreferences { get; set; }

        public void AudioPreferencesError(Exception ex)
        {
            ShowError(ex);
        }

        public void RefreshAudioPreferences(AudioAppConfig config)
        {
            _audioAppConfig = config;
            InvokeOnMainThread(() => {
                trackBufferSize.Value = config.BufferSize;
                trackUpdatePeriod.Value = config.UpdatePeriod;
                txtBufferSize.StringValue = config.BufferSize.ToString();
                txtUpdatePeriod.StringValue = config.UpdatePeriod.ToString();
            });

        }

        #endregion
    }
}
