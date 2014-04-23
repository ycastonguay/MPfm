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

namespace MPfm.Mac
{
    public partial class PreferencesWindowController : BaseWindowController, IDesktopPreferencesView
    {
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
            btnTabGeneral.OnTabButtonSelected += HandleOnTabButtonSelected;
            btnTabAudio.OnTabButtonSelected += HandleOnTabButtonSelected;
            btnTabLibrary.OnTabButtonSelected += HandleOnTabButtonSelected;
            btnTabCloud.OnTabButtonSelected += HandleOnTabButtonSelected;

            btnLoginDropbox.OnButtonSelected += (button) => OnDropboxLoginLogout();

            LoadFontsAndImages();
            HandleOnTabButtonSelected(btnTabGeneral);

            this.Window.Center();
            this.Window.MakeKeyAndOrderFront(this);
        }

        public override void WindowDidLoad()
        {
            base.WindowDidLoad();
            OnViewReady(this);
        }

        private void LoadFontsAndImages()
        {
            viewGeneralPreferencesHeader.BackgroundColor1 = GlobalTheme.PanelHeader2Color1;
            viewGeneralPreferencesHeader.BackgroundColor2 = GlobalTheme.PanelHeader2Color2;
            viewAudioPreferencesHeader.BackgroundColor1 = GlobalTheme.PanelHeader2Color1;
            viewAudioPreferencesHeader.BackgroundColor2 = GlobalTheme.PanelHeader2Color2;
            viewLibraryPreferencesHeader.BackgroundColor1 = GlobalTheme.PanelHeader2Color1;
            viewLibraryPreferencesHeader.BackgroundColor2 = GlobalTheme.PanelHeader2Color2;
            viewCloudPreferencesHeader.BackgroundColor1 = GlobalTheme.PanelHeader2Color1;
            viewCloudPreferencesHeader.BackgroundColor2 = GlobalTheme.PanelHeader2Color2;

            var headerFont = NSFont.FromFontName("Roboto Medium", 14f);
            lblGeneralPreferences.Font = headerFont;
            lblAudioPreferences.Font = headerFont;
            lblLibraryPreferences.Font = headerFont;
            lblCloudPreferences.Font = headerFont;

            var subtitleFont = NSFont.FromFontName("Roboto", 13f);
            lblGeneralUpdateFrequency.Font = subtitleFont;
            lblAudioOutput.Font = subtitleFont;
            lblAudioMixer.Font = subtitleFont;
            lblAudioStatus.Font = subtitleFont;
            lblLibraryFolders.Font = subtitleFont;
            lblCloudDropbox.Font = subtitleFont;

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

            var noteFont = NSFont.FromFontName("Roboto", 11f);
            var noteColor = NSColor.FromDeviceRgba(0.85f, 0.85f, 0.85f, 1);
            lblResumePlaybackNote.Font = noteFont;
            lblResumePlaybackNote.TextColor = noteColor;           

            // The NSButton checkbox type doesn't let you change the color, so use an attributed string instead
            var dict = new NSMutableDictionary();
            dict.Add(NSAttributedString.ForegroundColorAttributeName, NSColor.White);
            dict.Add(NSAttributedString.FontAttributeName, NSFont.FromFontName("Roboto", 12));
            var attrStr = new NSAttributedString("Enable Resume Playback with Dropbox", dict);
            checkEnableResumePlayback.AttributedTitle = attrStr;

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

            btnTabGeneral.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_general");
            btnTabAudio.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_speaker");
            btnTabLibrary.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_library");
            btnTabCloud.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_cloud");

            btnAddFolder.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_add");
            btnRemoveFolder.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_delete");
            btnResetLibrary.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_reset");
            btnTestAudioSettings.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_test");
            btnResetAudioSettings.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_reset");
            btnLoginDropbox.Image = ImageResources.Icons.FirstOrDefault(x => x.Name == "icon_button_dropbox");
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

        public System.Action<AudioAppConfig> OnSetAudioPreferences { get; set; }

        public void AudioPreferencesError(Exception ex)
        {
            ShowError(ex);
        }

        public void RefreshAudioPreferences(AudioAppConfig config)
        {
        }

        #endregion
    }
}
