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
using MPfm.Core;
using MPfm.MVP.Bootstrap;
using TinyIoC;
using MPfm.MVP.Views;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Models;
using TinyMessenger;
using MPfm.MVP.Messages;
using MPfm.Player.Objects;
using MPfm.Library.Objects;
using MPfm.Sound.AudioFiles;
using MPfm.MVP.Config;
using MPfm.MVP.Services.Interfaces;
using MPfm.Library.Services.Interfaces;
using System.Linq;

namespace MPfm.MVP.Navigation
{
    /// <summary>
    /// Manager class for managing view and presenter instances.
    /// </summary>
    public abstract class MobileNavigationManager : INavigationManager
    {
        private readonly object _locker = new object();

        private IMobileMainView _mainView;
        private IMobileMainPresenter _mainPresenter;
        private ISplashView _splashView;
        private ISplashPresenter _splashPresenter;
        private IAboutView _aboutView;
        private IAboutPresenter _aboutPresenter;
        private IMobileOptionsMenuView _optionsMenuView;
        private IMobileOptionsMenuPresenter _optionsMenuPresenter;
        private IPlayerStatusView _playerStatusView;
        private IPlayerStatusPresenter _playerStatusPresenter;
        private IUpdateLibraryView _updateLibraryView;
        private IUpdateLibraryPresenter _updateLibraryPresenter;
        private ISelectPlaylistView _selectPlaylistView;
        private ISelectPlaylistPresenter _selectPlaylistPresenter;
        private IAddPlaylistView _addPlaylistView;
        private IAddPlaylistPresenter _addPlaylistPresenter;
        private ISelectFoldersView _selectFoldersView;
        private ISelectFoldersPresenter _selectFoldersPresenter;
        private IEqualizerPresetsView _equalizerPresetsView;
        private IEqualizerPresetsPresenter _equalizerPresetsPresenter;
        private IEqualizerPresetDetailsView _equalizerPresetDetailsView;
        private IEqualizerPresetDetailsPresenter _equalizerPresetDetailsPresenter;
        private IPlayerView _playerView;
        private IPlayerPresenter _playerPresenter;
        private ISyncView _syncView;
        private ISyncPresenter _syncPresenter;
        private ISyncConnectManualView _syncConnectManualView;
        private ISyncConnectManualPresenter _syncConnectManualPresenter;
        private ISyncCloudView _syncCloudView;
        private ISyncCloudPresenter _syncCloudPresenter;
        private ISyncWebBrowserView _syncWebBrowserView;
        private ISyncWebBrowserPresenter _syncWebBrowserPresenter;
        private ISyncMenuView _syncMenuView;
        private ISyncMenuPresenter _syncMenuPresenter;
        private ISyncDownloadView _syncDownloadView;
        private ISyncDownloadPresenter _syncDownloadPresenter;
        private IPlaylistView _playlistView;
        private IPlaylistPresenter _playlistPresenter;
        private IAddMarkerView _addMarkerView;
        private IAddMarkerPresenter _addMarkerPresenter;
        private IResumePlaybackView _resumePlaybackView;
        private IResumePlaybackPresenter _resumePlaybackPresenter;
        private IStartResumePlaybackView _startResumePlaybackView;
        private IStartResumePlaybackPresenter _startResumePlaybackPresenter;
        private ICloudConnectView _cloudConnectView;
        private ICloudConnectPresenter _cloudConnectPresenter;
        private IFirstRunView _firstRunView;
        private IFirstRunPresenter _firstRunPresenter;

        // Player sub views
        private IPlayerMetadataView _playerMetadataView;
        private IPlayerMetadataPresenter _playerMetadataPresenter;
        private ILoopsView _loopsView;
        private ILoopsPresenter _loopsPresenter;
        private ILoopDetailsView _loopDetailsView;
        private ILoopDetailsPresenter _loopDetailsPresenter;
        private IMarkersView _markersView;
        private IMarkersPresenter _markersPresenter;
        private IMarkerDetailsView _markerDetailsView;
        private IMarkerDetailsPresenter _markerDetailsPresenter;
        private ITimeShiftingView _timeShiftingView;
        private ITimeShiftingPresenter _timeShiftingPresenter;
        private IPitchShiftingView _pitchShiftingView;
        private IPitchShiftingPresenter _pitchShiftingPresenter;

        // Preferences sub views
        private IPreferencesView _preferencesView;
        private IAudioPreferencesView _audioPreferencesView;
        private ICloudPreferencesView _cloudPreferencesView;
        private IGeneralPreferencesView _generalPreferencesView;
        private ILibraryPreferencesView _libraryPreferencesView;
        private IPreferencesPresenter _preferencesPresenter;
        private IAudioPreferencesPresenter _audioPreferencesPresenter;
        private ICloudPreferencesPresenter _cloudPreferencesPresenter;
        private IGeneralPreferencesPresenter _generalPreferencesPresenter;
        private ILibraryPreferencesPresenter _libraryPreferencesPresenter;

        private readonly Dictionary<Tuple<MobileNavigationTabType, MobileLibraryBrowserType>, Tuple<IMobileLibraryBrowserView, IMobileLibraryBrowserPresenter>> _mobileLibraryBrowserList = new Dictionary<Tuple<MobileNavigationTabType, MobileLibraryBrowserType>, Tuple<IMobileLibraryBrowserView, IMobileLibraryBrowserPresenter>>();

        public abstract void PushTabView(MobileNavigationTabType type, IBaseView view);
        public abstract void PushDialogView(MobileDialogPresentationType presentationType, string viewTitle, IBaseView sourceView, IBaseView view);

        public virtual void Start()
        {
            Tracing.Log("MobileNavigationManager - Start");
            CreateSplashView();
        }

        private void ContinueAfterSplash()
        {
            AppConfigManager.Instance.Load();

            #if IOS
            CreateMobileMainView();
            #endif

            Tracing.Log("MobileNavigationManager - ContinueAfterSplash - isFirstRun: {0} resumePlayback.currentAudioFileId: {1} resumePlayback.currentPlaylistId: {2}", AppConfigManager.Instance.Root.IsFirstRun, AppConfigManager.Instance.Root.ResumePlayback.CurrentAudioFileId, AppConfigManager.Instance.Root.ResumePlayback.CurrentPlaylistId);
            if (AppConfigManager.Instance.Root.IsFirstRun)
            {
                Tracing.Log("MobileNavigationManager - First run of the application; launching FirstRun activity...");
                CreateFirstRunView();
            }
            else if (!string.IsNullOrEmpty(AppConfigManager.Instance.Root.ResumePlayback.CurrentAudioFileId))
            {
                var playerService = Bootstrapper.GetContainer().Resolve<IPlayerService>();
                var audioFileCacheService = Bootstrapper.GetContainer().Resolve<IAudioFileCacheService>();
                var audioFile = audioFileCacheService.AudioFiles.FirstOrDefault(x => x.Id == new Guid(AppConfigManager.Instance.Root.ResumePlayback.CurrentAudioFileId));

                if (audioFile != null)
                {
                    Tracing.Log("MobileNavigationManager - Resume playback is available; launching Player activity...");
                    var audioFiles = audioFileCacheService.AudioFiles.Where(x => x.ArtistName == audioFile.ArtistName && x.AlbumTitle == audioFile.AlbumTitle).ToList();
                    playerService.Play(audioFiles, audioFile.FilePath);
                    // TO DO: Start paused; resume playback when player view is ready.
                    CreatePlayerView(MobileNavigationTabType.Playlists);
                }
            }

            // Shouldn't this be done by the presenter instead, who notifies the view? This should be the ONLY view that the NavMgr calls directly...
            _splashView.DestroyView();
        }

        private IMobileOptionsMenuView CreateOptionsMenuView()
        {
            // This is used only on iOS, where the Options menu is actually a tab named "More"
            Action<IBaseView> onViewReady = (view) => BindOptionsMenuView((IMobileOptionsMenuView)view);
            _optionsMenuView = Bootstrapper.GetContainer().Resolve<IMobileOptionsMenuView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            return _optionsMenuView;
        }

        public virtual void BindOptionsMenuView(IMobileOptionsMenuView view)
        {
            // This is used only on Android, where the Options menu is bound to the main activity.
            _optionsMenuView = view;
            _optionsMenuPresenter = Bootstrapper.GetContainer().Resolve<IMobileOptionsMenuPresenter>();
            _optionsMenuPresenter.BindView(view);
            _optionsMenuView.OnViewDestroy = (theView) =>
            {
                _optionsMenuPresenter.ViewDestroyed();
                _optionsMenuPresenter = null;
                _optionsMenuView = null;
            };
        }

        public virtual void BindPlayerStatusView(IPlayerStatusView view)
        {
            // This is used only on Android, where the Options menu is bound to the main activity.
            _playerStatusView = view;
            _playerStatusPresenter = Bootstrapper.GetContainer().Resolve<IPlayerStatusPresenter>();
            _playerStatusPresenter.BindView(view);
            _playerStatusView.OnViewDestroy = (theView) =>
            {
                _playerStatusPresenter.ViewDestroyed();
                _playerStatusPresenter = null;
                _playerStatusView = null;
            };
        }

        public virtual void CreateSplashView()
        {
            if (_splashView == null)
                _splashView = Bootstrapper.GetContainer().Resolve<ISplashView>();
        }

        public virtual void BindSplashView(ISplashView view)
        {
            Tracing.Log("MobileNavigationManager - BindSplashView");
            _splashView = view;
            _splashView.OnViewDestroy = (view2) =>
            {
                _splashPresenter.ViewDestroyed();
                _splashPresenter = null;
                _splashView = null;
            };
            _splashPresenter = Bootstrapper.GetContainer().Resolve<ISplashPresenter>();
            _splashPresenter.BindView(view);
            _splashPresenter.Initialize(ContinueAfterSplash);
        }

        public void CreateMainView()
        {
            // Not used on mobile devices.
        }

        public void BindMainView(IMainView view)
        {
            // Not used on mobile devices.
        }

        public virtual void CreateMobileMainView()
        {
            if (_mainView == null)
                _mainView = Bootstrapper.GetContainer().Resolve<IMobileMainView>();

            // For details view:
            // .......Resolve<IMarkerDetailsView>(bla... markerId: 'guid');
            // View implementation ctor:
            // ctor(...., Guid markerId)
            // (...)
            // BindMarkerDetailsView(markerId);
            //
            //
            // Q: Is it a problem if on some platforms where you can't add params to the ctor (for a view implementation)?
            // A: No, because mobile platforms that don't return the view instance need to have a way to add details 
            // (i.e. intent params on Android, page params on Windows Store, etc.). 
            // However... will TinyIoC crash if a namedparam cannot be resolved?
        }

        public virtual void BindMobileMainView(IMobileMainView view)
        {
            // OnViewReady in IBaseView cannot be used anymore because the Android system can create activities by itself. 
            // The NavMgr or presenter never have a chance to register to OnViewReady! So the activity needs to call the NavMgr to let it know a new view is available.
            // Fine for normal views, but what about details views (i.e. Edit an entity)? The system simply cannot create a details view by itself! 
            // If an activity previously created with intent params is restored, the intent param values will also be restored. So these params will be sent to the presenter
            // when binding the view (i.e. INavigationManager.BindMarkerDetailsView(Guid markerId))
            Tracing.Log("MobileNavigationManager - BindMobileMainView");
            _mainView = view;
            _mainPresenter = Bootstrapper.GetContainer().Resolve<IMobileMainPresenter>();
            _mainView.OnViewDestroy = (view2) =>
            {
                _mainPresenter.ViewDestroyed();
                _mainPresenter = null;
                _mainView = null;
            };
            _mainPresenter.BindView(view);

#if ANDROID
            var artistsView = CreateMobileLibraryBrowserView(MobileNavigationTabType.Artists, MobileLibraryBrowserType.Artists, new LibraryQuery());
            _mainView.AddTab(MobileNavigationTabType.Artists, "Artists", MobileLibraryBrowserType.Artists, new LibraryQuery(), artistsView);
#elif IOS
            var playlistsView = CreateMobileLibraryBrowserView(MobileNavigationTabType.Playlists, MobileLibraryBrowserType.Playlists, new LibraryQuery());
            var artistsView = CreateMobileLibraryBrowserView(MobileNavigationTabType.Artists, MobileLibraryBrowserType.Artists, new LibraryQuery());
            var albumsView = CreateMobileLibraryBrowserView(MobileNavigationTabType.Albums, MobileLibraryBrowserType.Albums, new LibraryQuery());
            var songsView = CreateMobileLibraryBrowserView(MobileNavigationTabType.Songs, MobileLibraryBrowserType.Songs, new LibraryQuery());
            var moreView = CreateOptionsMenuView();
            _mainView.AddTab(MobileNavigationTabType.Playlists, "Sessions", MobileLibraryBrowserType.Playlists, new LibraryQuery(), playlistsView);
            _mainView.AddTab(MobileNavigationTabType.Artists, "Artists", MobileLibraryBrowserType.Artists, new LibraryQuery(), artistsView);
            _mainView.AddTab(MobileNavigationTabType.Albums, "Albums", MobileLibraryBrowserType.Albums, new LibraryQuery(), albumsView);
            _mainView.AddTab(MobileNavigationTabType.Songs, "Songs", MobileLibraryBrowserType.Songs, new LibraryQuery(), songsView);
            _mainView.AddTab(MobileNavigationTabType.More, "More", MobileLibraryBrowserType.Songs, new LibraryQuery(), moreView);
#endif
        }

        public virtual IUpdateLibraryView CreateUpdateLibraryView()
        {
            _updateLibraryView = Bootstrapper.GetContainer().Resolve<IUpdateLibraryView>();
            return _updateLibraryView;
        }

        public virtual void BindUpdateLibraryView(IUpdateLibraryView view)
        {
            _updateLibraryView = view;
            _updateLibraryPresenter = Bootstrapper.GetContainer().Resolve<IUpdateLibraryPresenter>();
            _updateLibraryPresenter.BindView(view);
            _updateLibraryView.OnViewDestroy = (view2) =>
            {
                _updateLibraryPresenter.ViewDestroyed();
                _updateLibraryPresenter = null;
                _updateLibraryView = null;
            };
        }

        public virtual ISelectPlaylistView CreateSelectPlaylistView(LibraryBrowserEntity item)
        {
            _selectPlaylistView = Bootstrapper.GetContainer().Resolve<ISelectPlaylistView>(new NamedParameterOverloads() { { "item", item } });
            return _selectPlaylistView;
        }

        public virtual void BindSelectPlaylistView(ISelectPlaylistView view, LibraryBrowserEntity item)
        {
            _selectPlaylistView = view;
            _selectPlaylistPresenter = Bootstrapper.GetContainer().Resolve<ISelectPlaylistPresenter>(new NamedParameterOverloads() { { "item", item } });
            _selectPlaylistPresenter.BindView(view);
            _selectPlaylistView.OnViewDestroy = (view2) =>
            {
                _selectPlaylistPresenter.ViewDestroyed();
                _selectPlaylistPresenter = null;
                _selectPlaylistView = null;
            };            
        }

        public virtual IAddPlaylistView CreateAddPlaylistView()
        {
            _addPlaylistView = Bootstrapper.GetContainer().Resolve<IAddPlaylistView>();
            return _addPlaylistView;
        }

        public virtual void BindAddPlaylistView(IAddPlaylistView view)
        {
            _addPlaylistView = view;
            _addPlaylistPresenter = Bootstrapper.GetContainer().Resolve<IAddPlaylistPresenter>();
            _addPlaylistPresenter.BindView(view);
            _addPlaylistView.OnViewDestroy = (view2) =>
            {
                _addPlaylistPresenter.ViewDestroyed();
                _addPlaylistPresenter = null;
                _addPlaylistView = null;
            };            
        }

        public virtual ISelectFoldersView CreateSelectFoldersView()
        {
            _selectFoldersView = Bootstrapper.GetContainer().Resolve<ISelectFoldersView>();
            return _selectFoldersView;
        }

        public virtual void BindSelectFoldersView(ISelectFoldersView view)
        {
            _selectFoldersView = view;
            _selectFoldersPresenter = Bootstrapper.GetContainer().Resolve<ISelectFoldersPresenter>();
            _selectFoldersPresenter.BindView(view);
            _selectFoldersView.OnViewDestroy = (view2) =>
            {
                _selectFoldersPresenter.ViewDestroyed();
                _selectFoldersPresenter = null;
                _selectFoldersView = null;
            };
        }

        public virtual IAddMarkerView CreateAddMarkerView()
        {
            _addMarkerView = Bootstrapper.GetContainer().Resolve<IAddMarkerView>();
            return _addMarkerView;
        }

        public virtual void BindAddMarkerView(IAddMarkerView view)
        {
            _addMarkerView = view;
            _addMarkerPresenter = Bootstrapper.GetContainer().Resolve<IAddMarkerPresenter>();
            _addMarkerPresenter.BindView(view);
            _addMarkerView.OnViewDestroy = (view2) =>
            {
                _addMarkerPresenter.ViewDestroyed();
                _addMarkerPresenter = null;
                _addMarkerView = null;
            };
        }

        public virtual void CreatePreferencesView()
        {
            if(_preferencesView == null)
                _preferencesView = Bootstrapper.GetContainer().Resolve<IPreferencesView>();

            PushTabView(MobileNavigationTabType.More, _preferencesView);
        }

        public virtual void BindPreferencesView(IPreferencesView view)
        {
            _preferencesView = view;
            _preferencesView.OnViewDestroy = (view2) =>
            {
                _preferencesPresenter.ViewDestroyed();
                _preferencesPresenter = null;
                _preferencesView = null;
            };
            _preferencesPresenter = Bootstrapper.GetContainer().Resolve<IPreferencesPresenter>();
            _preferencesPresenter.BindView(view);
                
//#if ANDROID
//            // On Android, push subviews for preferences since there's generally more space on screen and swiping horizontally is more natural.
//            var general = CreateGeneralPreferencesView();
//            var audio = CreateAudioPreferencesView();
//            var library = CreateLibraryPreferencesView();
//            _preferencesView.PushSubView(general);
//            _preferencesView.PushSubView(audio);
//            _preferencesView.PushSubView(library);
//#endif
        }
        
        public virtual void CreateAudioPreferencesView()
        {
            if(_audioPreferencesView == null)
                _audioPreferencesView = Bootstrapper.GetContainer().Resolve<IAudioPreferencesView>();

            PushTabView(MobileNavigationTabType.More, _audioPreferencesView);
        }

        public virtual void BindAudioPreferencesView(IAudioPreferencesView view)
        {
            _audioPreferencesView = view;
            _audioPreferencesPresenter = Bootstrapper.GetContainer().Resolve<IAudioPreferencesPresenter>();
            _audioPreferencesPresenter.BindView(view);
            _audioPreferencesView.OnViewDestroy = (view2) =>
            {
                _audioPreferencesPresenter.ViewDestroyed();
                _audioPreferencesPresenter = null;
                _audioPreferencesView = null;
            };            
        }

        public virtual void CreateCloudPreferencesView()
        {
            if(_cloudPreferencesView == null)
                _cloudPreferencesView = Bootstrapper.GetContainer().Resolve<ICloudPreferencesView>();

            PushTabView(MobileNavigationTabType.More, _cloudPreferencesView);
        }

        public virtual void BindCloudPreferencesView(ICloudPreferencesView view)
        {
            _cloudPreferencesView = view;
            _cloudPreferencesPresenter = Bootstrapper.GetContainer().Resolve<ICloudPreferencesPresenter>();
            _cloudPreferencesPresenter.BindView(view);
            _cloudPreferencesView.OnViewDestroy = (view2) =>
            {
                _cloudPreferencesPresenter.ViewDestroyed();
                _cloudPreferencesPresenter = null;
                _cloudPreferencesView = null;
            };            
        }

        public virtual void CreateGeneralPreferencesView()
        {
            if(_generalPreferencesView == null)
                _generalPreferencesView = Bootstrapper.GetContainer().Resolve<IGeneralPreferencesView>();

            PushTabView(MobileNavigationTabType.More, _generalPreferencesView);
        }

        public virtual void BindGeneralPreferencesView(IGeneralPreferencesView view)
        {
            _generalPreferencesView = view;
            _generalPreferencesPresenter = Bootstrapper.GetContainer().Resolve<IGeneralPreferencesPresenter>();
            _generalPreferencesPresenter.BindView(view);
            _generalPreferencesView.OnViewDestroy = (view2) =>
            {
                _generalPreferencesPresenter.ViewDestroyed();
                _generalPreferencesPresenter = null;
                _generalPreferencesView = null;
            };            
        }

        public virtual void CreateLibraryPreferencesView()
        {
            if(_libraryPreferencesView == null)
                _libraryPreferencesView = Bootstrapper.GetContainer().Resolve<ILibraryPreferencesView>();

            PushTabView(MobileNavigationTabType.More, _libraryPreferencesView);
        }

        public virtual void BindLibraryPreferencesView(ILibraryPreferencesView view)
        {
            _libraryPreferencesPresenter = Bootstrapper.GetContainer().Resolve<ILibraryPreferencesPresenter>();
            _libraryPreferencesPresenter.BindView(view);
            _libraryPreferencesView.OnViewDestroy = (view2) =>
            {
                _libraryPreferencesPresenter.ViewDestroyed();
                _libraryPreferencesPresenter = null;
                _libraryPreferencesView = null;
            };            
        }

        public virtual IMobileLibraryBrowserView CreateMobileLibraryBrowserView(MobileNavigationTabType tabType, MobileLibraryBrowserType browserType, LibraryQuery query)
        {
            var key = new Tuple<MobileNavigationTabType, MobileLibraryBrowserType>(tabType, browserType);
            
            // Check if view already exists
            if(_mobileLibraryBrowserList.ContainsKey(key))
            {
                Tuple<IMobileLibraryBrowserView, IMobileLibraryBrowserPresenter> viewPresenter;
                if (_mobileLibraryBrowserList.TryGetValue(key, out viewPresenter))
                {
                    if (viewPresenter != null)
                    {
                        viewPresenter.Item2.ChangeQuery(browserType, query);
                        return viewPresenter.Item1;
                    }
                }
            }

            var view = Bootstrapper.GetContainer().Resolve<IMobileLibraryBrowserView>(new NamedParameterOverloads() { { "tabType", tabType }, { "browserType", browserType }, { "query", query } });
            return view;
        }

        public virtual void BindMobileLibraryBrowserView(IMobileLibraryBrowserView view, MobileNavigationTabType tabType, MobileLibraryBrowserType browserType, LibraryQuery query)
        {
            var key = new Tuple<MobileNavigationTabType, MobileLibraryBrowserType>(tabType, browserType);
            view.OnViewDestroy = (view2) =>
            {
                // The view list can be accessed from different threads.
                lock (_locker)
                {
                    Tracing.Log("MobileNavigationManager - CreateMobileLibraryBrowserView - Destroying view - type: {0}", tabType.ToString());
                    if (_mobileLibraryBrowserList.ContainsKey(key))
                    {
                        _mobileLibraryBrowserList[key].Item2.ViewDestroyed();
                        _mobileLibraryBrowserList.Remove(key);
                    }
                }
            };

            lock (_locker)
            {
                var presenter = Bootstrapper.GetContainer().Resolve<IMobileLibraryBrowserPresenter>(new NamedParameterOverloads() { { "tabType", tabType }, { "browserType", browserType }, { "query", query } });
                presenter.BindView(view);
                _mobileLibraryBrowserList.Add(key, new Tuple<IMobileLibraryBrowserView, IMobileLibraryBrowserPresenter>(view, presenter));
            }
        }

        public virtual void CreatePlayerView(MobileNavigationTabType tabType)
        {
            if (_playerView == null)
                _playerView = Bootstrapper.GetContainer().Resolve<IPlayerView>();

            // This is only used on iOS. Shouldn't this be routed to the main view? IMobileMainView.PushTabView?
            PushTabView(tabType, _playerView);
        }

        public virtual void BindPlayerView(MobileNavigationTabType tabType, IPlayerView view)
        {
            _playerView = view;
            _playerView.OnViewDestroy = (view2) =>
            {
                _playerPresenter.ViewDestroyed();
                _playerPresenter = null;
                _playerView = null;
            };
            _playerPresenter = Bootstrapper.GetContainer().Resolve<IPlayerPresenter>();
            _playerPresenter.BindView(view);

            // Create sub views
            var playerMetadata = CreatePlayerMetadataView();
            var loops = CreateLoopsView();
            var markers = CreateMarkersView();
            var timeShifting = CreateTimeShiftingView();
            var pitchShifting = CreatePitchShiftingView();

            _playerView.PushSubView(playerMetadata);
            _playerView.PushSubView(loops);
            _playerView.PushSubView(markers);
            _playerView.PushSubView(timeShifting);
            _playerView.PushSubView(pitchShifting);
        }

        public virtual IPlayerMetadataView CreatePlayerMetadataView()
        {
            _playerMetadataView = Bootstrapper.GetContainer().Resolve<IPlayerMetadataView>();
            return _playerMetadataView;
        }

        public virtual void BindPlayerMetadataView(IPlayerMetadataView view)
        {
            _playerMetadataView = view;
            _playerMetadataPresenter = Bootstrapper.GetContainer().Resolve<IPlayerMetadataPresenter>();
            _playerMetadataPresenter.BindView(view);
            _playerMetadataView.OnViewDestroy = (view2) =>
            {
                _playerMetadataPresenter.ViewDestroyed();
                _playerMetadataPresenter = null;
                _playerMetadataView = null;
            };
        }

        public virtual ILoopsView CreateLoopsView()
        {
            _loopsView = Bootstrapper.GetContainer().Resolve<ILoopsView>();
            return _loopsView;
        }

        public virtual void BindLoopsView(ILoopsView view)
        {
            _loopsView = view;
            _loopsPresenter = Bootstrapper.GetContainer().Resolve<ILoopsPresenter>();
            _loopsPresenter.BindView(view);
            _loopsView.OnViewDestroy = (view2) =>
            {
                _loopsPresenter.ViewDestroyed();
                _loopsPresenter = null;
                _loopsView = null;
            };
        }

        public virtual ILoopDetailsView CreateLoopDetailsView()
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
            {
                _loopDetailsPresenter = Bootstrapper.GetContainer().Resolve<ILoopDetailsPresenter>();
                _loopDetailsPresenter.BindView((ILoopDetailsView)view);
            };
            
            // Create view and manage view destruction
            _loopDetailsView = Bootstrapper.GetContainer().Resolve<ILoopDetailsView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _loopDetailsView.OnViewDestroy = (view) =>
            {
                _loopDetailsPresenter.ViewDestroyed();
                _loopDetailsPresenter = null;
                _loopDetailsView = null;
            };
            return _loopDetailsView;
        }

        public virtual IMarkersView CreateMarkersView()
        {
            _markersView = Bootstrapper.GetContainer().Resolve<IMarkersView>();
            return _markersView;
        }

        public virtual void BindMarkersView(IMarkersView view)
        {
            _markersView = view;
            _markersPresenter = Bootstrapper.GetContainer().Resolve<IMarkersPresenter>();
            _markersPresenter.BindView(view);
            _markersView.OnViewDestroy = (view2) =>
            {
                _markersPresenter.ViewDestroyed();
                _markersPresenter = null;
                _markersView = null;
            };           
        }

        public virtual void CreateMarkerDetailsView(IBaseView sourceView, Guid markerId)
        {
            if (_markerDetailsView == null)
                _markerDetailsView = Bootstrapper.GetContainer().Resolve<IMarkerDetailsView>(new NamedParameterOverloads() { { "markerId", markerId } });
        }

        public virtual void BindMarkerDetailsView(IMarkerDetailsView view, Guid markerId)
        {
            _markerDetailsView = view;
            _markerDetailsPresenter = Bootstrapper.GetContainer().Resolve<IMarkerDetailsPresenter>(new NamedParameterOverloads() { { "markerId", markerId } });
            _markerDetailsPresenter.BindView(view);
            _markerDetailsView.OnViewDestroy = (view2) =>
            {
                _markerDetailsPresenter.ViewDestroyed();
                _markerDetailsPresenter = null;
                _markerDetailsView = null;
            };            
        }

        public virtual ITimeShiftingView CreateTimeShiftingView()
        {
            _timeShiftingView = Bootstrapper.GetContainer().Resolve<ITimeShiftingView>();
            return _timeShiftingView;
        }

        public virtual void BindTimeShiftingView(ITimeShiftingView view)
        {
            _timeShiftingView = view;
            _timeShiftingPresenter = Bootstrapper.GetContainer().Resolve<ITimeShiftingPresenter>();
            _timeShiftingPresenter.BindView(view);
            _timeShiftingView.OnViewDestroy = (view2) =>
            {
                _timeShiftingPresenter.ViewDestroyed();
                _timeShiftingPresenter = null;
                _timeShiftingView = null;
            };
        }

        public virtual IPitchShiftingView CreatePitchShiftingView()
        {
            _pitchShiftingView = Bootstrapper.GetContainer().Resolve<IPitchShiftingView>();
            return _pitchShiftingView;
        }

        public virtual void BindPitchShiftingView(IPitchShiftingView view)
        {
            _pitchShiftingView = view;
            _pitchShiftingPresenter = Bootstrapper.GetContainer().Resolve<IPitchShiftingPresenter>();
            _pitchShiftingPresenter.BindView(view);
            _pitchShiftingView.OnViewDestroy = (view2) =>
            {
                _pitchShiftingPresenter.ViewDestroyed();
                _pitchShiftingPresenter = null;
                _pitchShiftingView = null;
            };
        }

        public virtual void CreateEqualizerPresetsView(IBaseView sourceView)
        {
            if(_equalizerPresetsView == null)
                _equalizerPresetsView = Bootstrapper.GetContainer().Resolve<IEqualizerPresetsView>();

            PushDialogView(MobileDialogPresentationType.Standard, "Equalizer Presets", null, _equalizerPresetsView);
        }

        public virtual void BindEqualizerPresetsView(IBaseView sourceView, IEqualizerPresetsView view)
        {
            _equalizerPresetsView = view;
            _equalizerPresetsView.OnViewDestroy = (view2) =>
            {
                _equalizerPresetsPresenter.ViewDestroyed();
                _equalizerPresetsPresenter = null;
                _equalizerPresetsView = null;
            };
            _equalizerPresetsPresenter = Bootstrapper.GetContainer().Resolve<IEqualizerPresetsPresenter>();
            _equalizerPresetsPresenter.BindView(view);
        }

        public virtual void CreateEqualizerPresetDetailsView(IBaseView sourceView, Guid presetId)
        {
            if (_equalizerPresetDetailsView == null)
                _equalizerPresetDetailsView = Bootstrapper.GetContainer().Resolve<IEqualizerPresetDetailsView>(new NamedParameterOverloads() { { "presetId", presetId } });

            //PushDialogView(MobileDialogPresentationType.Standard, "Equalizer Preset Details", null, _equalizerPresetDetailsView);
            PushTabView(MobileNavigationTabType.More, _equalizerPresetDetailsView);
        }

        public virtual void BindEqualizerPresetDetailsView(IBaseView sourceView, IEqualizerPresetDetailsView view, Guid presetId)
        {
            _equalizerPresetDetailsView = view;
            _equalizerPresetDetailsView.OnViewDestroy = (view2) =>
            {
                _equalizerPresetDetailsPresenter.ViewDestroyed();
                _equalizerPresetDetailsPresenter = null;
                _equalizerPresetDetailsView = null;
            };
            _equalizerPresetDetailsPresenter = Bootstrapper.GetContainer().Resolve<IEqualizerPresetDetailsPresenter>(new NamedParameterOverloads(){{"presetId", presetId}});
            _equalizerPresetDetailsPresenter.BindView(view);
        }

        public virtual void CreateSyncView()
        {
            if (_syncView == null)
                _syncView = Bootstrapper.GetContainer().Resolve<ISyncView>();

            PushTabView(MobileNavigationTabType.More, _syncView);
        }

        public virtual void BindSyncView(ISyncView view)
        {
            _syncView = view;
            _syncView.OnViewDestroy = (view2) =>
            {
                _syncPresenter.ViewDestroyed();
                _syncPresenter = null;
                _syncView = null;
            };
            _syncPresenter = Bootstrapper.GetContainer().Resolve<ISyncPresenter>();
            _syncPresenter.BindView(view);
        }

        public virtual void CreateSyncWebBrowserView()
        {
            if (_syncWebBrowserView == null)
                _syncWebBrowserView = Bootstrapper.GetContainer().Resolve<ISyncWebBrowserView>();

            PushTabView(MobileNavigationTabType.More, _syncWebBrowserView);
        }
        
        public virtual void BindSyncWebBrowserView(ISyncWebBrowserView view)
        {
            _syncWebBrowserView = view;
            _syncWebBrowserView.OnViewDestroy = (view2) =>
            {
                _syncWebBrowserPresenter.ViewDestroyed();
                _syncWebBrowserPresenter = null;
                _syncWebBrowserView = null;
            };
            _syncWebBrowserPresenter = Bootstrapper.GetContainer().Resolve<ISyncWebBrowserPresenter>();
            _syncWebBrowserPresenter.BindView(view);
        }

        public virtual void CreateSyncCloudView()
        {
            if (_syncCloudView == null)
                _syncCloudView = Bootstrapper.GetContainer().Resolve<ISyncCloudView>();

            PushTabView(MobileNavigationTabType.More, _syncCloudView);
        }

        public virtual void BindSyncCloudView(ISyncCloudView view)
        {
            _syncCloudView = view;
            _syncCloudView.OnViewDestroy = (view2) =>
            {
                _syncCloudPresenter.ViewDestroyed();
                _syncCloudPresenter = null;
                _syncCloudView = null;
            };
            _syncCloudPresenter = Bootstrapper.GetContainer().Resolve<ISyncCloudPresenter>();
            _syncCloudPresenter.BindView(view);
        }

        public virtual void CreateSyncMenuView(SyncDevice device)
        {
            if (_syncMenuView == null)
                _syncMenuView = Bootstrapper.GetContainer().Resolve<ISyncMenuView>(new NamedParameterOverloads() { { "device", device } });

            PushTabView(MobileNavigationTabType.More, _syncMenuView);
        }

        public virtual void BindSyncMenuView(ISyncMenuView view, SyncDevice device)
        {
            _syncMenuView = view;
            _syncMenuView.OnViewDestroy = (view2) =>
            {
                _syncMenuPresenter.ViewDestroyed();
                _syncMenuPresenter = null;
                _syncMenuView = null;
            };
            _syncMenuPresenter = Bootstrapper.GetContainer().Resolve<ISyncMenuPresenter>(new NamedParameterOverloads() { { "device", device } });
            _syncMenuPresenter.BindView(view);
        }

        public virtual void CreateSyncDownloadView(SyncDevice device, IEnumerable<AudioFile> audioFiles)
        {
            //if (_syncDownloadView == null)
            //    _syncDownloadView = Bootstrapper.GetContainer().Resolve<ISyncDownloadView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            //else
            //    _syncDownloadPresenter.StartSync(device, audioFiles);

            if (_syncDownloadView == null)
                _syncDownloadView = Bootstrapper.GetContainer().Resolve<ISyncDownloadView>();
        }

        public virtual void BindSyncDownloadView(ISyncDownloadView view, SyncDevice device, IEnumerable<AudioFile> audioFiles)
        {
            _syncDownloadView = view;
            _syncDownloadView.OnViewDestroy = (view2) =>
            {
                _syncDownloadPresenter.ViewDestroyed();
                _syncDownloadPresenter = null;
                _syncDownloadView = null;
            };
            _syncDownloadPresenter = Bootstrapper.GetContainer().Resolve<ISyncDownloadPresenter>();
            _syncDownloadPresenter.BindView(view);
            // Move to Ctor
            //_syncDownloadPresenter.StartSync(device, audioFiles);
        }

        public virtual void CreateAboutView()
        {
            if (_aboutView == null)
                _aboutView = Bootstrapper.GetContainer().Resolve<IAboutView>();

            PushTabView(MobileNavigationTabType.More, _aboutView);
        }

        public virtual void BindAboutView(IAboutView view)
        {
            _aboutView = view;
            _aboutView.OnViewDestroy = (view2) =>
            {
                _aboutPresenter.ViewDestroyed();
                _aboutPresenter = null;
                _aboutView = null;
            };
            _aboutPresenter = Bootstrapper.GetContainer().Resolve<IAboutPresenter>();
            _aboutPresenter.BindView(view);
        }

        public virtual void CreateFirstRunView()
        {
            if (_firstRunView == null)
                _firstRunView = Bootstrapper.GetContainer().Resolve<IFirstRunView>();
        }

        public virtual void BindFirstRunView(IFirstRunView view)
        {
            _firstRunView = view;
            _firstRunView.OnViewDestroy = (view2) =>
            {
                _firstRunPresenter.ViewDestroyed();
                _firstRunPresenter = null;
                _firstRunView = null;
            };
            _firstRunPresenter = Bootstrapper.GetContainer().Resolve<IFirstRunPresenter>();
            _firstRunPresenter.BindView(view);
        }

        public virtual void CreateResumePlaybackView()
        {
            if (_resumePlaybackView == null)
                _resumePlaybackView = Bootstrapper.GetContainer().Resolve<IResumePlaybackView>();

            PushTabView(MobileNavigationTabType.More, _resumePlaybackView);
        }

        public virtual void BindResumePlaybackView(IResumePlaybackView view)
        {
            _resumePlaybackView = view;
            _resumePlaybackView.OnViewDestroy = (view2) =>
            {
                _resumePlaybackPresenter.ViewDestroyed();
                _resumePlaybackPresenter = null;
                _resumePlaybackView = null;
            };
            _resumePlaybackPresenter = Bootstrapper.GetContainer().Resolve<IResumePlaybackPresenter>();
            _resumePlaybackPresenter.BindView(view);
        }

        public virtual IStartResumePlaybackView CreateStartResumePlaybackView()
        {
            _startResumePlaybackView = Bootstrapper.GetContainer().Resolve<IStartResumePlaybackView>();
            return _startResumePlaybackView;
        }

        public virtual void BindStartResumePlaybackView(IStartResumePlaybackView view)
        {
            _startResumePlaybackView = view;
            _startResumePlaybackPresenter = Bootstrapper.GetContainer().Resolve<IStartResumePlaybackPresenter>();
            _startResumePlaybackPresenter.BindView(view);
            _startResumePlaybackView.OnViewDestroy = (view2) =>
            {
                _startResumePlaybackPresenter.ViewDestroyed();
                _startResumePlaybackPresenter = null;
                _startResumePlaybackView = null;
            };            
        }

        public virtual void CreateCloudConnectView()
        {
            if (_cloudConnectView == null)
                _cloudConnectView = Bootstrapper.GetContainer().Resolve<ICloudConnectView>();
        }

        public virtual void BindCloudConnectView(ICloudConnectView view)
        {
            _cloudConnectView = view;
            _cloudConnectView.OnViewDestroy = (view2) =>
            {
                _cloudConnectPresenter.ViewDestroyed();
                _cloudConnectPresenter = null;
                _cloudConnectView = null;
            };
            _cloudConnectPresenter = Bootstrapper.GetContainer().Resolve<ICloudConnectPresenter>();
            _cloudConnectPresenter.BindView(view);
        }

        public virtual void CreatePlaylistView(IBaseView sourceView)
        {
            if (_playlistView == null)
                _playlistView = Bootstrapper.GetContainer().Resolve<IPlaylistView>();

            PushDialogView(MobileDialogPresentationType.Standard, "Playlist", null, _playlistView);
        }

        public virtual void BindPlaylistView(IBaseView sourceView, IPlaylistView view)
        {
            _playlistView = view;
            _playlistView.OnViewDestroy = (view2) =>
            {
                _playlistPresenter.ViewDestroyed();
                _playlistPresenter = null;
                _playlistView = null;
            };
            _playlistPresenter = Bootstrapper.GetContainer().Resolve<IPlaylistPresenter>();
            _playlistPresenter.BindView(view);
        }
    }

    public enum MobileNavigationTabType
    {
        Playlists = 0,
        Artists = 1,
        Albums = 2,
        Songs = 3,
        More = 4, // only used on iOS
        PreferencesGeneral = 5,
        PreferencesAudio = 6,
        PreferencesLibrary = 7
    }

    public enum MobileDialogPresentationType
    {
        Standard = 0,
        Overlay = 1
    }
}
