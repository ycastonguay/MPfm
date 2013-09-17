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

namespace MPfm.MVP.Navigation
{
    /// <summary>
    /// Manager class for managing view and presenter instances.
    /// </summary>
    public abstract class MobileNavigationManager
    {
        private readonly object _locker = new object();

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
        private IAddNewPlaylistView _addNewPlaylistView;
        private IAddNewPlaylistPresenter _addNewPlaylistPresenter;
        private IEqualizerPresetsView _equalizerPresetsView;
        private IEqualizerPresetsPresenter _equalizerPresetsPresenter;
        private IEqualizerPresetDetailsView _equalizerPresetDetailsView;
        private IEqualizerPresetDetailsPresenter _equalizerPresetDetailsPresenter;
        private IPlayerView _playerView;
        private IPlayerPresenter _playerPresenter;
        private ISyncView _syncView;
        private ISyncPresenter _syncPresenter;
        private ISyncWebBrowserView _syncWebBrowserView;
        private ISyncWebBrowserPresenter _syncWebBrowserPresenter;
        private ISyncMenuView _syncMenuView;
        private ISyncMenuPresenter _syncMenuPresenter;
        private ISyncDownloadView _syncDownloadView;
        private ISyncDownloadPresenter _syncDownloadPresenter;
        private IPlaylistView _playlistView;
        private IPlaylistPresenter _playlistPresenter;

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
        private IGeneralPreferencesView _generalPreferencesView;
        private ILibraryPreferencesView _libraryPreferencesView;
        private IPreferencesPresenter _preferencesPresenter;
        private IAudioPreferencesPresenter _audioPreferencesPresenter;
        private IGeneralPreferencesPresenter _generalPreferencesPresenter;
        private ILibraryPreferencesPresenter _libraryPreferencesPresenter;

        protected IEqualizerPresetsView EqualizerPresetsView { get { return _equalizerPresetsView; } }
        protected IPlayerView PlayerView { get { return _playerView; } }

        private Dictionary<Tuple<MobileNavigationTabType, MobileLibraryBrowserType>, Tuple<IMobileLibraryBrowserView, IMobileLibraryBrowserPresenter>> _mobileLibraryBrowserList = new Dictionary<Tuple<MobileNavigationTabType, MobileLibraryBrowserType>, Tuple<IMobileLibraryBrowserView, IMobileLibraryBrowserPresenter>>();

        public abstract void ShowSplash(ISplashView view);
        public abstract void HideSplash();
        public abstract void AddTab(MobileNavigationTabType type, string title, IBaseView view);
        public abstract void AddTab(MobileNavigationTabType type, string title, MobileLibraryBrowserType browserType, LibraryQuery query, IBaseView view);
        public abstract void PushTabView(MobileNavigationTabType type, IBaseView view);
        public abstract void PushTabView(MobileNavigationTabType type, MobileLibraryBrowserType browserType, LibraryQuery query, IBaseView view);
        public abstract void PushDialogView(string viewTitle, IBaseView sourceView, IBaseView view);
        public abstract void PushDialogSubview(string parentViewTitle, IBaseView view);
        public abstract void PushPlayerSubview(IPlayerView playerView, IBaseView view);
        public abstract void PushPreferencesSubview(IPreferencesView preferencesView, IBaseView view);
        public abstract void NotifyMobileLibraryBrowserQueryChange(MobileNavigationTabType type, MobileLibraryBrowserType browserType, LibraryQuery query);

        public virtual void Start()
        {
            Action onInitDone = () =>
            {                
#if ANDROID
                // Only one 'tab' on Android since we re-use the same fragment for different queries
                var artistsView = CreateMobileLibraryBrowserView(MobileNavigationTabType.Artists, MobileLibraryBrowserType.Artists, new LibraryQuery());
                AddTab(MobileNavigationTabType.Artists, "Artists", MobileLibraryBrowserType.Artists, new LibraryQuery(), artistsView);
#elif IOS
                var playlistsView = CreateMobileLibraryBrowserView(MobileNavigationTabType.Playlists, MobileLibraryBrowserType.Playlists, new LibraryQuery());
                var artistsView = CreateMobileLibraryBrowserView(MobileNavigationTabType.Artists, MobileLibraryBrowserType.Artists, new LibraryQuery());
                var albumsView = CreateMobileLibraryBrowserView(MobileNavigationTabType.Albums, MobileLibraryBrowserType.Albums, new LibraryQuery());
                var songsView = CreateMobileLibraryBrowserView(MobileNavigationTabType.Songs, MobileLibraryBrowserType.Songs, new LibraryQuery());
                var moreView = CreateOptionsMenuView();
                AddTab(MobileNavigationTabType.Playlists, "Sessions", MobileLibraryBrowserType.Playlists, new LibraryQuery(), playlistsView);
                AddTab(MobileNavigationTabType.Artists, "Artists", MobileLibraryBrowserType.Artists, new LibraryQuery(), artistsView);
                AddTab(MobileNavigationTabType.Albums, "Albums", MobileLibraryBrowserType.Albums, new LibraryQuery(), albumsView);
                AddTab(MobileNavigationTabType.Songs, "Songs", MobileLibraryBrowserType.Songs, new LibraryQuery(), songsView);
                AddTab(MobileNavigationTabType.More, "More", moreView);
#endif

                // Finally hide the splash screen, our UI is ready
                HideSplash();
            };            
            ShowSplash(CreateSplashView(onInitDone));
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

        public virtual ISplashView CreateSplashView(Action onInitDone)
        {
            Action<IBaseView> onViewReady = (view) => {
                _splashPresenter = Bootstrapper.GetContainer().Resolve<ISplashPresenter>();
                _splashPresenter.BindView((ISplashView)view);
                _splashPresenter.Initialize(onInitDone);
            };
            _splashView = Bootstrapper.GetContainer().Resolve<ISplashView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _splashView.OnViewDestroy = (view) => {
                _splashPresenter.ViewDestroyed();
                _splashPresenter = null;
                _splashView = null;
            };
            return _splashView;
        }

        public virtual IUpdateLibraryView CreateUpdateLibraryView()
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
            {
                _updateLibraryPresenter = Bootstrapper.GetContainer().Resolve<IUpdateLibraryPresenter>();
                _updateLibraryPresenter.BindView((IUpdateLibraryView)view);
            };

            // Create view and manage view destruction
            _updateLibraryView = Bootstrapper.GetContainer().Resolve<IUpdateLibraryView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _updateLibraryView.OnViewDestroy = (view) =>
            {
                _updateLibraryPresenter.ViewDestroyed();
                _updateLibraryPresenter = null;
                _updateLibraryView = null;
            };
            return _updateLibraryView;
        }

        public virtual ISelectPlaylistView CreateSelectPlaylistView(LibraryBrowserEntity item)
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
            {
                _selectPlaylistPresenter = Bootstrapper.GetContainer().Resolve<ISelectPlaylistPresenter>(new NamedParameterOverloads() { { "item", item } });
                _selectPlaylistPresenter.BindView((ISelectPlaylistView)view);
            };

            // Create view and manage view destruction
            _selectPlaylistView = Bootstrapper.GetContainer().Resolve<ISelectPlaylistView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _selectPlaylistView.OnViewDestroy = (view) =>
            {
                _selectPlaylistPresenter.ViewDestroyed();
                _selectPlaylistPresenter = null;
                _selectPlaylistView = null;
            };
            return _selectPlaylistView;
        }

        public virtual IAddNewPlaylistView CreateAddNewPlaylistView()
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
            {
                _addNewPlaylistPresenter = Bootstrapper.GetContainer().Resolve<IAddNewPlaylistPresenter>();
                _addNewPlaylistPresenter.BindView((IAddNewPlaylistView)view);
            };

            // Create view and manage view destruction
            _addNewPlaylistView = Bootstrapper.GetContainer().Resolve<IAddNewPlaylistView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _addNewPlaylistView.OnViewDestroy = (view) =>
            {
                _addNewPlaylistPresenter.ViewDestroyed();
                _addNewPlaylistPresenter = null;
                _addNewPlaylistView = null;
            };
            return _addNewPlaylistView;
        }

        protected virtual void CreatePreferencesViewInternal(Action<IBaseView> onViewReady)
        {
            if(_preferencesView == null)
                _preferencesView = Bootstrapper.GetContainer().Resolve<IPreferencesView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });

            // Android activities are started by an intent and cannot be pushed like iOS
#if !ANDROID
            PushTabView(MobileNavigationTabType.More, _preferencesView);
#endif
        }

        public virtual void CreatePreferencesView()
        {
            Action<IBaseView> onViewReady = (view) => {
                _preferencesView = (IPreferencesView)view;
                _preferencesView.OnViewDestroy = (view2) =>
                {
                    _preferencesPresenter.ViewDestroyed();
                    _preferencesPresenter = null;
                    _preferencesView = null;
                };
                _preferencesPresenter = Bootstrapper.GetContainer().Resolve<IPreferencesPresenter>();
                _preferencesPresenter.BindView((IPreferencesView)view);
                
#if ANDROID
                // On Android, push subviews for preferences since there's generally more space on screen and swiping horizontally is more natural.
                var general = CreateGeneralPreferencesView();
                var audio = CreateAudioPreferencesView();
                var library = CreateLibraryPreferencesView();
                PushPreferencesSubview(_preferencesView, general);
                PushPreferencesSubview(_preferencesView, audio);
                PushPreferencesSubview(_preferencesView, library);
#endif
            };
            
            CreatePreferencesViewInternal(onViewReady);
        }
        
        public virtual IAudioPreferencesView CreateAudioPreferencesView()
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
                {
                    _audioPreferencesPresenter = Bootstrapper.GetContainer().Resolve<IAudioPreferencesPresenter>();
                    _audioPreferencesPresenter.BindView((IAudioPreferencesView)view);
                };

            // Create view and manage view destruction
            if(_audioPreferencesView == null)
            {
                _audioPreferencesView = Bootstrapper.GetContainer().Resolve<IAudioPreferencesView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
                _audioPreferencesView.OnViewDestroy = (view) =>
                {
                    _audioPreferencesPresenter.ViewDestroyed();
                    _audioPreferencesPresenter = null;
                    _audioPreferencesView = null;
                };
            }
            return _audioPreferencesView;
        }

        public virtual IGeneralPreferencesView CreateGeneralPreferencesView()
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
            {
                _generalPreferencesPresenter = Bootstrapper.GetContainer().Resolve<IGeneralPreferencesPresenter>();
                _generalPreferencesPresenter.BindView((IGeneralPreferencesView)view);
            };

            // Create view and manage view destruction
            if(_generalPreferencesView == null)
            {
                _generalPreferencesView = Bootstrapper.GetContainer().Resolve<IGeneralPreferencesView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
                _generalPreferencesView.OnViewDestroy = (view) =>
                {
                    _generalPreferencesPresenter.ViewDestroyed();
                    _generalPreferencesPresenter = null;
                    _generalPreferencesView = null;
                };
            }
            return _generalPreferencesView;
        }

        public virtual ILibraryPreferencesView CreateLibraryPreferencesView()
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
            {
                _libraryPreferencesPresenter = Bootstrapper.GetContainer().Resolve<ILibraryPreferencesPresenter>();
                _libraryPreferencesPresenter.BindView((ILibraryPreferencesView)view);
            };

            // Create view and manage view destruction
            if(_libraryPreferencesView == null)
            {
                _libraryPreferencesView = Bootstrapper.GetContainer().Resolve<ILibraryPreferencesView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
                _libraryPreferencesView.OnViewDestroy = (view) =>
                {                    
                    _libraryPreferencesPresenter.ViewDestroyed();
                    _libraryPreferencesPresenter = null;
                    _libraryPreferencesView = null;
                };
            }
            return _libraryPreferencesView;
        }

        protected IMobileLibraryBrowserPresenter GetMobileLibraryBrowserPresenter(MobileNavigationTabType tabType, MobileLibraryBrowserType browserType)
        {
            var key = new Tuple<MobileNavigationTabType, MobileLibraryBrowserType>(tabType, browserType);
            var viewPresenter = new Tuple<IMobileLibraryBrowserView, IMobileLibraryBrowserPresenter>(null, null);
            _mobileLibraryBrowserList.TryGetValue(key, out viewPresenter);
            return viewPresenter == null ? null : viewPresenter.Item2;
        }

        public virtual IMobileLibraryBrowserView CreateMobileLibraryBrowserView(MobileNavigationTabType tabType, MobileLibraryBrowserType browserType, LibraryQuery query)
        {
            var key = new Tuple<MobileNavigationTabType, MobileLibraryBrowserType>(tabType, browserType);
            
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
            {
                // The view list can be accessed from different threads.
                lock (_locker)
                {
                    var presenter = Bootstrapper.GetContainer().Resolve<IMobileLibraryBrowserPresenter>(new NamedParameterOverloads() 
                        {{"tabType", tabType}, {"browserType", browserType}, {"query", query}});
                    presenter.BindView((IMobileLibraryBrowserView) view);
                    _mobileLibraryBrowserList.Add(key, new Tuple<IMobileLibraryBrowserView, IMobileLibraryBrowserPresenter>((IMobileLibraryBrowserView)view, presenter));
                }
            };

            // Check if view already exists
            if(_mobileLibraryBrowserList.ContainsKey(key))
            {
                Tuple<IMobileLibraryBrowserView, IMobileLibraryBrowserPresenter> viewPresenter;
                if(_mobileLibraryBrowserList.TryGetValue(key, out viewPresenter))
                {
                    if(viewPresenter != null)
                    {
                        // Force refresh of view
                        viewPresenter.Item2.SetQuery(browserType, query);
                        return viewPresenter.Item1;
                    }
                }
            }

            // Create view and manage view destruction
            IMobileLibraryBrowserView newView = null;
            newView = Bootstrapper.GetContainer().Resolve<IMobileLibraryBrowserView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            newView.OnViewDestroy = (view) =>
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

            return newView;
        }

        protected virtual void CreatePlayerViewInternal(MobileNavigationTabType tabType, Action<IBaseView> onViewReady)
        {
            if (_playerView == null)
                _playerView = Bootstrapper.GetContainer().Resolve<IPlayerView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });

#if !ANDROID
            PushTabView(tabType, _playerView);
#endif
        }

        public virtual void CreatePlayerView(MobileNavigationTabType tabType, Action<IBaseView> onViewBindedToPresenter)
        {
            Action<IBaseView> onViewReady = (view) => {
                _playerView = (IPlayerView) view;
                _playerView.OnViewDestroy = (view2) =>
                {
                    _playerPresenter.ViewDestroyed();
                    _playerPresenter = null;
                    _playerView = null;
                };
                _playerPresenter = Bootstrapper.GetContainer().Resolve<IPlayerPresenter>();
                _playerPresenter.BindView((IPlayerView)view);

                var playerMetadata = CreatePlayerMetadataView();
                var loops = CreateLoopsView();
                var markers = CreateMarkersView();
                var timeShifting = CreateTimeShiftingView();
                var pitchShifting = CreatePitchShiftingView();

                PushPlayerSubview(_playerView, playerMetadata);
                PushPlayerSubview(_playerView, loops);
                PushPlayerSubview(_playerView, markers);
                PushPlayerSubview(_playerView, timeShifting);
                PushPlayerSubview(_playerView, pitchShifting);

                if (onViewBindedToPresenter != null)
                    onViewBindedToPresenter(view);
            };

            if(_playerView == null)
            {
                CreatePlayerViewInternal(tabType, onViewReady);
            }
            else
            {
                onViewBindedToPresenter(_playerView);
                PushTabView(tabType, MobileLibraryBrowserType.Artists, new LibraryQuery(), _playerView);
            }
        }

        public virtual IPlayerMetadataView CreatePlayerMetadataView()
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
            {
                _playerMetadataPresenter = Bootstrapper.GetContainer().Resolve<IPlayerMetadataPresenter>();
                _playerMetadataPresenter.BindView((IPlayerMetadataView)view);
            };
            
            // Create view and manage view destruction
            _playerMetadataView = Bootstrapper.GetContainer().Resolve<IPlayerMetadataView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _playerMetadataView.OnViewDestroy = (view) =>
            {
                _playerMetadataPresenter.ViewDestroyed();
                _playerMetadataPresenter = null;
                _playerMetadataView = null;
            };
            return _playerMetadataView;
        }

        public virtual ILoopsView CreateLoopsView()
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
            {
                _loopsPresenter = Bootstrapper.GetContainer().Resolve<ILoopsPresenter>();
                _loopsPresenter.BindView((ILoopsView)view);
            };
            
            // Create view and manage view destruction
            _loopsView = Bootstrapper.GetContainer().Resolve<ILoopsView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _loopsView.OnViewDestroy = (view) =>
            {
                _loopsPresenter.ViewDestroyed();
                _loopsPresenter = null;
                _loopsView = null;
            };
            return _loopsView;
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
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
            {
                _markersPresenter = Bootstrapper.GetContainer().Resolve<IMarkersPresenter>();
                _markersPresenter.BindView((IMarkersView)view);
            };
            
            // Create view and manage view destruction
            _markersView = Bootstrapper.GetContainer().Resolve<IMarkersView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _markersView.OnViewDestroy = (view) =>
            {
                _markersPresenter.ViewDestroyed();
                _markersPresenter = null;
                _markersView = null;
            };
            return _markersView;
        }

        protected virtual void CreateMarkerDetailsViewInternal(IBaseView sourceView, Action<IBaseView> onViewReady)
        {
            if (_markerDetailsView == null)
                _markerDetailsView = Bootstrapper.GetContainer().Resolve<IMarkerDetailsView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });

#if !ANDROID
            PushDialogView("Marker Details", sourceView, _markerDetailsView);
#endif
        }

        public virtual void CreateMarkerDetailsView(IBaseView sourceView, Guid markerId)
        {
            Action<IBaseView> onViewReady = (view) =>
            {
                _markerDetailsView = (IMarkerDetailsView)view;
                _markerDetailsView.OnViewDestroy = (view2) =>
                {
                    _markerDetailsPresenter.ViewDestroyed();
                    _markerDetailsPresenter = null;
                    _markerDetailsView = null;
                };
                _markerDetailsPresenter = Bootstrapper.GetContainer().Resolve<IMarkerDetailsPresenter>(new NamedParameterOverloads() { { "markerId", markerId } });
                _markerDetailsPresenter.BindView((IMarkerDetailsView)view);
            };

            CreateMarkerDetailsViewInternal(sourceView, onViewReady);
        }

        public virtual ITimeShiftingView CreateTimeShiftingView()
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
            {
                _timeShiftingPresenter = Bootstrapper.GetContainer().Resolve<ITimeShiftingPresenter>();
                _timeShiftingPresenter.BindView((ITimeShiftingView)view);
            };
            
            // Create view and manage view destruction
            _timeShiftingView = Bootstrapper.GetContainer().Resolve<ITimeShiftingView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _timeShiftingView.OnViewDestroy = (view) =>
            {
                _timeShiftingPresenter.ViewDestroyed();
                _timeShiftingPresenter = null;
                _timeShiftingView = null;
            };
            return _timeShiftingView;
        }

        public virtual IPitchShiftingView CreatePitchShiftingView()
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
            {
                _pitchShiftingPresenter = Bootstrapper.GetContainer().Resolve<IPitchShiftingPresenter>();
                _pitchShiftingPresenter.BindView((IPitchShiftingView)view);
            };
            
            // Create view and manage view destruction
            _pitchShiftingView = Bootstrapper.GetContainer().Resolve<IPitchShiftingView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _pitchShiftingView.OnViewDestroy = (view) =>
            {
                _pitchShiftingPresenter.ViewDestroyed();
                _pitchShiftingPresenter = null;
                _pitchShiftingView = null;
            };
            return _pitchShiftingView;
        }

        protected virtual void CreateEqualizerPresetsViewInternal(IBaseView sourceView, Action<IBaseView> onViewReady)
        {
            if(_equalizerPresetsView == null)
                _equalizerPresetsView = Bootstrapper.GetContainer().Resolve<IEqualizerPresetsView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            
#if !ANDROID
            PushDialogView("Equalizer Presets", null, _equalizerPresetsView);
#endif
        }

        public virtual void CreateEqualizerPresetsView(IBaseView sourceView)
        {
            Action<IBaseView> onViewReady = (view) =>
            {
                _equalizerPresetsView = (IEqualizerPresetsView)view;
                _equalizerPresetsView.OnViewDestroy = (view2) =>
                {
                    _equalizerPresetsPresenter.ViewDestroyed();
                    _equalizerPresetsPresenter = null;
                    _equalizerPresetsView = null;
                };
                _equalizerPresetsPresenter = Bootstrapper.GetContainer().Resolve<IEqualizerPresetsPresenter>();
                _equalizerPresetsPresenter.BindView((IEqualizerPresetsView)view);
            };

            CreateEqualizerPresetsViewInternal(sourceView, onViewReady);
        }

        protected virtual void CreateEqualizerPresetDetailsViewInternal(IBaseView sourceView, Action<IBaseView> onViewReady)
        {
            if (_equalizerPresetDetailsView == null)
                _equalizerPresetDetailsView = Bootstrapper.GetContainer().Resolve<IEqualizerPresetDetailsView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });

#if !ANDROID
            PushDialogSubview("Equalizer Presets", _equalizerPresetDetailsView);
#endif
        }

        public virtual void CreateEqualizerPresetDetailsView(IBaseView sourceView, EQPreset preset)
        {
            Action<IBaseView> onViewReady = (view) =>
            {
                _equalizerPresetDetailsView = (IEqualizerPresetDetailsView)view;
                _equalizerPresetDetailsView.OnViewDestroy = (view2) =>
                {
                    _equalizerPresetDetailsPresenter.ViewDestroyed();
                    _equalizerPresetDetailsPresenter = null;
                    _equalizerPresetDetailsView = null;
                };
                _equalizerPresetDetailsPresenter = Bootstrapper.GetContainer().Resolve<IEqualizerPresetDetailsPresenter>(new NamedParameterOverloads(){{"preset", preset}});
                _equalizerPresetDetailsPresenter.BindView((IEqualizerPresetDetailsView)view);
            };

            CreateEqualizerPresetDetailsViewInternal(sourceView, onViewReady);
        }

        protected virtual void CreateSyncViewInternal(Action<IBaseView> onViewReady)
        {
            if (_syncView == null)
                _syncView = Bootstrapper.GetContainer().Resolve<ISyncView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });

#if !ANDROID
            PushTabView(MobileNavigationTabType.More, _syncView);
#endif
        }

        public virtual void CreateSyncView()
        {
            Action<IBaseView> onViewReady = (view) =>
            {
                _syncView = (ISyncView)view;
                _syncView.OnViewDestroy = (view2) =>
                {
                    _syncPresenter.ViewDestroyed();
                    _syncPresenter = null;
                    _syncView = null;
                };
                _syncPresenter = Bootstrapper.GetContainer().Resolve<ISyncPresenter>();
                _syncPresenter.BindView((ISyncView)view);
            };

            CreateSyncViewInternal(onViewReady);
        }

        protected virtual void CreateSyncWebBrowserViewInternal(Action<IBaseView> onViewReady)
        {
            if (_syncWebBrowserView == null)
                _syncWebBrowserView = Bootstrapper.GetContainer().Resolve<ISyncWebBrowserView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });

#if !ANDROID
            PushTabView(MobileNavigationTabType.More, _syncWebBrowserView);
#endif
        }
        
        public virtual void CreateSyncWebBrowserView()
        {
            Action<IBaseView> onViewReady = (view) =>
            {
                _syncWebBrowserView = (ISyncWebBrowserView)view;
                _syncWebBrowserView.OnViewDestroy = (view2) =>
                {
                    _syncWebBrowserPresenter.ViewDestroyed();
                    _syncWebBrowserPresenter = null;
                    _syncWebBrowserView = null;
                };
                _syncWebBrowserPresenter = Bootstrapper.GetContainer().Resolve<ISyncWebBrowserPresenter>();
                _syncWebBrowserPresenter.BindView((ISyncWebBrowserView)view);
            };

            CreateSyncWebBrowserViewInternal(onViewReady);
        }

        protected virtual void CreateSyncMenuViewInternal(Action<IBaseView> onViewReady, SyncDevice device)
        {
            if (_syncMenuView == null)
                _syncMenuView = Bootstrapper.GetContainer().Resolve<ISyncMenuView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            else
                _syncMenuPresenter.SetSyncDevice(device);
            
#if !ANDROID
            PushTabView(MobileNavigationTabType.More, _syncMenuView);
#endif
        }

        public virtual void CreateSyncMenuView(SyncDevice device)
        {
            Action<IBaseView> onViewReady = (view) =>
            {
                _syncMenuView = (ISyncMenuView)view;
                _syncMenuView.OnViewDestroy = (view2) =>
                {
                    _syncMenuPresenter.ViewDestroyed();
                    _syncMenuPresenter = null;
                    _syncMenuView = null;
                };
                _syncMenuPresenter = Bootstrapper.GetContainer().Resolve<ISyncMenuPresenter>();
                _syncMenuPresenter.BindView((ISyncMenuView)view);
                _syncMenuPresenter.SetSyncDevice(device);
            };

            CreateSyncMenuViewInternal(onViewReady, device);
        }

        protected virtual void CreateSyncDownloadViewInternal(Action<IBaseView> onViewReady, SyncDevice device, IEnumerable<AudioFile> audioFiles)
        {
            if (_syncDownloadView == null)
                _syncDownloadView = Bootstrapper.GetContainer().Resolve<ISyncDownloadView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            else
                _syncDownloadPresenter.StartSync(device, audioFiles);

#if !ANDROID
            PushTabView(MobileNavigationTabType.More, _syncDownloadView);
#endif
        }

        public virtual void CreateSyncDownloadView(SyncDevice device, IEnumerable<AudioFile> audioFiles)
        {
            Action<IBaseView> onViewReady = (view) =>
            {
                _syncDownloadView = (ISyncDownloadView)view;
                _syncDownloadView.OnViewDestroy = (view2) =>
                {
                    _syncDownloadPresenter.ViewDestroyed();
                    _syncDownloadPresenter = null;
                    _syncDownloadView = null;
                };
                _syncDownloadPresenter = Bootstrapper.GetContainer().Resolve<ISyncDownloadPresenter>();
                _syncDownloadPresenter.BindView((ISyncDownloadView)view);
                _syncDownloadPresenter.StartSync(device, audioFiles);
            };

            CreateSyncDownloadViewInternal(onViewReady, device, audioFiles);
        }

        protected virtual void CreateAboutViewInternal(Action<IBaseView> onViewReady)
        {
            if (_aboutView == null)
                _aboutView = Bootstrapper.GetContainer().Resolve<IAboutView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });

#if !ANDROID
            PushTabView(MobileNavigationTabType.More, _aboutView);
#endif
        }

        public virtual void CreateAboutView()
        {
            Action<IBaseView> onViewReady = (view) =>
            {
                _aboutView = (IAboutView)view;
                _aboutView.OnViewDestroy = (view2) =>
                {
                    _aboutPresenter.ViewDestroyed();
                    _aboutPresenter = null;
                    _aboutView = null;
                };
                _aboutPresenter = Bootstrapper.GetContainer().Resolve<IAboutPresenter>();
                _aboutPresenter.BindView((IAboutView)view);
            };

            CreateAboutViewInternal(onViewReady);
        }

        protected virtual void CreatePlaylistViewInternal(IBaseView sourceView, Action<IBaseView> onViewReady)
        {
            if (_playlistView == null)
                _playlistView = Bootstrapper.GetContainer().Resolve<IPlaylistView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });

#if !ANDROID
            PushTabView(MobileNavigationTabType.More, _playlistView);
            //PushDialogView("Playlist", View, view);
#endif
        }

        public virtual void CreatePlaylistView(IBaseView sourceView)
        {
            Action<IBaseView> onViewReady = (view) => 
            {
                _playlistView = (IPlaylistView)view;
                _playlistView.OnViewDestroy = (view2) =>
                {
                    _playlistPresenter.ViewDestroyed();
                    _playlistPresenter = null;
                    _playlistView = null;
                };
                _playlistPresenter = Bootstrapper.GetContainer().Resolve<IPlaylistPresenter>();
                _playlistPresenter.BindView((IPlaylistView)view);
            };
            
            CreatePlaylistViewInternal(sourceView, onViewReady);
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
}
