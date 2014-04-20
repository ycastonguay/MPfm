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
            this.Window.Center();
            this.Window.MakeKeyAndOrderFront(this);

            btnTabGeneral.OnTabButtonSelected += HandleOnTabButtonSelected;
            btnTabAudio.OnTabButtonSelected += HandleOnTabButtonSelected;
            btnTabLibrary.OnTabButtonSelected += HandleOnTabButtonSelected;
            btnTabCloud.OnTabButtonSelected += HandleOnTabButtonSelected;

            LoadFontsAndImages();
            HandleOnTabButtonSelected(btnTabGeneral);
        }

        public override void WindowDidLoad()
        {
            base.WindowDidLoad();
            OnViewReady(this);
        }

        private void LoadFontsAndImages()
        {
            var headerFont = NSFont.FromFontName("Roboto", 14f);
            lblGeneralPreferences.Font = headerFont;
            lblAudioPreferences.Font = headerFont;
            lblLibraryPreferences.Font = headerFont;
            lblCloudPreferences.Font = headerFont;

            var subtitleFont = NSFont.FromFontName("Roboto", 12f);
            lblGeneralUpdateFrequency.Font = subtitleFont;
            lblAudioOutput.Font = subtitleFont;
            lblAudioMixer.Font = subtitleFont;
            lblAudioStatus.Font = subtitleFont;
            lblLibraryFolders.Font = subtitleFont;
            lblCloudDropbox.Font = subtitleFont;

            var textFont = NSFont.FromFontName("Roboto", 11f);
            lblOutputDevice.Font = textFont;
            lblSampleRate.Font = textFont;
            lblStatusDescription.Font = textFont;
            lblUpdatePeriod.Font = textFont;
            lblEvery.Font = textFont;
            lblEvery2.Font = textFont;
            lblMS.Font = textFont;
            lblMS2.Font = textFont;
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
        }

        public void RefreshCloudPreferences(CloudAppConfig config)
        {
        }

        public void RefreshCloudPreferencesState(CloudPreferencesStateEntity entity)
        {
        }

        #endregion

        #region IAudioPreferencesView implementation

        public System.Action<AudioAppConfig> OnSetAudioPreferences { get; set; }

        public void AudioPreferencesError(Exception ex)
        {
        }

        public void RefreshAudioPreferences(AudioAppConfig config)
        {
        }

        #endregion
    }
}
