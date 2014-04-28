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
using MPfm.Library.Objects;
using MPfm.MVP.Models;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;

namespace MPfm.MVP.Navigation
{
    /// <summary>
    /// Interface defining how views can be created and bound (i.e. NavigationManager and MobileNavigationManager).
    /// This interface is used by the presenters so they don't have to know on which platform type the view is created (i.e. desktop vs mobile).
    /// </summary>
    public interface INavigationManager
    {
        // Has to be implemented on mobile, but the implementation can be empty.
        void PushDialogView(MobileDialogPresentationType presentationType, string viewTitle, IBaseView sourceView, IBaseView view);
        void Start();
        void BindOptionsMenuView(IMobileOptionsMenuView view);

        void CreatePlayerStatusView();
        void BindPlayerStatusView(IPlayerStatusView view);

        void CreateSplashView();
        void BindSplashView(ISplashView view);

        void CreateMainView();
        void BindMainView(IMainView view);

        void CreateMobileMainView();
        void BindMobileMainView(IMobileMainView view);

        IUpdateLibraryView CreateUpdateLibraryView();
        void BindUpdateLibraryView(IUpdateLibraryView view);

        ISelectPlaylistView CreateSelectPlaylistView(LibraryBrowserEntity item);
        void BindSelectPlaylistView(ISelectPlaylistView view, LibraryBrowserEntity item);

        IAddPlaylistView CreateAddPlaylistView();
        void BindAddPlaylistView(IAddPlaylistView view);

        ISelectFoldersView CreateSelectFoldersView();
        void BindSelectFoldersView(ISelectFoldersView view);

        IAddMarkerView CreateAddMarkerView();
        void BindAddMarkerView(IAddMarkerView view);

        void CreatePreferencesView();
        void BindPreferencesView(IPreferencesView view);

        void CreateAudioPreferencesView();
        void BindAudioPreferencesView(IAudioPreferencesView view);

        void CreateCloudPreferencesView();
        void BindCloudPreferencesView(ICloudPreferencesView view);

        void CreateGeneralPreferencesView();
        void BindGeneralPreferencesView(IGeneralPreferencesView view);

        void CreateLibraryPreferencesView();
        void BindLibraryPreferencesView(ILibraryPreferencesView view);

        IMobileLibraryBrowserView CreateMobileLibraryBrowserView(MobileNavigationTabType tabType, MobileLibraryBrowserType browserType, LibraryQuery query);
        void BindMobileLibraryBrowserView(IMobileLibraryBrowserView view, MobileNavigationTabType tabType, MobileLibraryBrowserType browserType, LibraryQuery query);

        void CreatePlayerView(MobileNavigationTabType tabType);
        void BindPlayerView(MobileNavigationTabType tabType, IPlayerView view);

        IPlayerMetadataView CreatePlayerMetadataView();
        void BindPlayerMetadataView(IPlayerMetadataView view);

        ILoopsView CreateLoopsView();
        void BindLoopsView(ILoopsView view);

        ILoopDetailsView CreateLoopDetailsView();
        
        IMarkersView CreateMarkersView();
        void BindMarkersView(IMarkersView view);

        void CreateMarkerDetailsView(IBaseView sourceView, Guid markerId);
        void BindMarkerDetailsView(IMarkerDetailsView view, Guid markerId);

        ITimeShiftingView CreateTimeShiftingView();
        void BindTimeShiftingView(ITimeShiftingView view);

        IPitchShiftingView CreatePitchShiftingView();
        void BindPitchShiftingView(IPitchShiftingView view);

        void CreateEqualizerPresetsView(IBaseView sourceView);
        void BindEqualizerPresetsView(IBaseView sourceView, IEqualizerPresetsView view);

        void CreateEqualizerPresetDetailsView(IBaseView sourceView, Guid presetId);
        void BindEqualizerPresetDetailsView(IBaseView sourceView, IEqualizerPresetDetailsView view, Guid presetId);

        void CreateSyncView();
        void BindSyncView(ISyncView view);

        void CreateSyncWebBrowserView();
        void BindSyncWebBrowserView(ISyncWebBrowserView view);

        void CreateSyncCloudView();
        void BindSyncCloudView(ISyncCloudView view);

        void CreateSyncMenuView(SyncDevice device);
        void BindSyncMenuView(ISyncMenuView view, SyncDevice device);

        void CreateSyncDownloadView();
        void BindSyncDownloadView(ISyncDownloadView view);

        void CreateAboutView();
        void BindAboutView(IAboutView view);

        void CreateFirstRunView();
        void BindFirstRunView(IFirstRunView view);

        void CreateResumePlaybackView();
        void BindResumePlaybackView(IResumePlaybackView view);

        IStartResumePlaybackView CreateStartResumePlaybackView();
        void BindStartResumePlaybackView(IStartResumePlaybackView view);

        ICloudConnectView CreateCloudConnectView();
        void BindCloudConnectView(ICloudConnectView view);

        void CreatePlaylistView(IBaseView sourceView);
        void BindPlaylistView(IBaseView sourceView, IPlaylistView view);
    }
}
