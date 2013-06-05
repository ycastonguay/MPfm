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
using MPfm.MVP.Bootstrap;
using TinyIoC;
using MPfm.MVP.Views;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Models;
using TinyMessenger;
using MPfm.MVP.Messages;
using MPfm.Player.Objects;
using MPfm.Library.Objects;

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
        private IUpdateLibraryView _updateLibraryView;
        private IUpdateLibraryPresenter _updateLibraryPresenter;
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

        private Dictionary<Tuple<MobileNavigationTabType, MobileLibraryBrowserType>, Tuple<IMobileLibraryBrowserView, IMobileLibraryBrowserPresenter>> _mobileLibraryBrowserList = new Dictionary<Tuple<MobileNavigationTabType, MobileLibraryBrowserType>, Tuple<IMobileLibraryBrowserView, IMobileLibraryBrowserPresenter>>();

        public abstract void ShowSplash(ISplashView view);
        public abstract void HideSplash();
        public abstract void AddTab(MobileNavigationTabType type, string title, IBaseView view);
        public abstract void PushTabView(MobileNavigationTabType type, IBaseView view);
        public abstract void PushDialogView(string viewTitle, IBaseView view);
        public abstract void PushDialogSubview(string parentViewTitle, IBaseView view);
        public abstract void PushPlayerSubview(IPlayerView playerView, IBaseView view);

        public virtual void Start()
        {
            Action onInitDone = () =>
            {                
                // Create 4 main tabs
                var playlistsView = CreateMobileLibraryBrowserView(MobileNavigationTabType.Playlists, MobileLibraryBrowserType.Playlists, new LibraryQuery());
                var artistsView = CreateMobileLibraryBrowserView(MobileNavigationTabType.Artists, MobileLibraryBrowserType.Artists, new LibraryQuery());
                var albumsView = CreateMobileLibraryBrowserView(MobileNavigationTabType.Albums, MobileLibraryBrowserType.Albums, new LibraryQuery());
                var songsView = CreateMobileLibraryBrowserView(MobileNavigationTabType.Songs, MobileLibraryBrowserType.Songs, new LibraryQuery());
                AddTab(MobileNavigationTabType.Playlists, "Playlists", playlistsView);
                AddTab(MobileNavigationTabType.Artists, "Artists", artistsView);
                AddTab(MobileNavigationTabType.Albums, "Albums", albumsView);
                AddTab(MobileNavigationTabType.Songs, "Songs", songsView);

                // iOS has one more tab, the More tab (Options Menu equivalent on Android).
#if IOS
                var moreView = CreateOptionsMenuView();
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

        protected void ShowPlayerView(MobileNavigationTabType tabType)
        {
            // Show player view only if it already exists
            if(_playerView != null)
                PushTabView(tabType, _playerView);
        }

        protected void ShowEqualizerPresetsView()
        {
            var view = CreateEqualizerPresetsView();
            PushDialogView("Equalizer Presets", view);
        }

        public virtual void BindOptionsMenuView(IMobileOptionsMenuView view)
        {
            // This is used only on Android, where the Options menu is bound to the activity.
            _optionsMenuView = view;
            _optionsMenuPresenter = Bootstrapper.GetContainer().Resolve<IMobileOptionsMenuPresenter>();
            _optionsMenuPresenter.BindView(view);
            _optionsMenuView.OnViewDestroy = (theView) =>
            {
                _optionsMenuView = null;
                _optionsMenuPresenter = null;
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
                _splashView = null;
                _splashPresenter = null;
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
                _updateLibraryView = null;
                _updateLibraryPresenter = null;
            };
            return _updateLibraryView;
        }

        public virtual IPreferencesView CreatePreferencesView()
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
            {
                _preferencesPresenter = Bootstrapper.GetContainer().Resolve<IPreferencesPresenter>();
                _preferencesPresenter.BindView((IPreferencesView)view);
            };
            
            // Create view and manage view destruction
            if(_preferencesView == null)
            {
                _preferencesView = Bootstrapper.GetContainer().Resolve<IPreferencesView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
                _preferencesView.OnViewDestroy = (view) =>
                {
                    _libraryPreferencesView = null;
                    _preferencesPresenter = null;
                };
            }
            return _preferencesView;
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
                    _audioPreferencesView = null;
                    _audioPreferencesPresenter = null;
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
                    _generalPreferencesView = null;
                    _generalPreferencesPresenter = null;
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
                    _libraryPreferencesView = null;
                    _libraryPreferencesPresenter = null;
                };
            }
            return _libraryPreferencesView;
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
                        viewPresenter.Item2.RefreshView(query);
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
                    if (_mobileLibraryBrowserList.ContainsKey(key))
                        _mobileLibraryBrowserList.Remove(key);
                }
            };

            return newView;
        }

        public virtual IPlayerView CreatePlayerView(Action<IBaseView> onViewBindedToPresenter)
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
            {
                _playerPresenter = Bootstrapper.GetContainer().Resolve<IPlayerPresenter>();
                _playerPresenter.BindView((IPlayerView)view);

                // Add scroll view items
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

            // Re-use the same player instance as before
            if(_playerView == null)
            {
                // Create view and manage view destruction
                _playerView = Bootstrapper.GetContainer().Resolve<IPlayerView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
                _playerView.OnViewDestroy = (view) =>
                {
                    _playerView = null;
                    _playerPresenter = null;
                };
            }
            else
            {
                // Re-use the same instance
                onViewBindedToPresenter(_playerView);
            }

            return _playerView;
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
                _playerMetadataView = null;
                _playerMetadataPresenter = null;
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
                _loopsView = null;
                _loopsPresenter = null;
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
                _loopDetailsView = null;
                _loopDetailsPresenter = null;
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
                _markersView = null;
                _markersPresenter = null;
            };
            return _markersView;
        }

        public virtual IMarkerDetailsView CreateMarkerDetailsView(Guid markerId)
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
            {
                _markerDetailsPresenter = Bootstrapper.GetContainer().Resolve<IMarkerDetailsPresenter>(new NamedParameterOverloads(){{"markerId", markerId}});
                _markerDetailsPresenter.BindView((IMarkerDetailsView)view);
            };
            
            // Create view and manage view destruction
            _markerDetailsView = Bootstrapper.GetContainer().Resolve<IMarkerDetailsView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _markerDetailsView.OnViewDestroy = (view) =>
            {
                _markerDetailsView = null;
                _markerDetailsPresenter = null;
            };
            return _markerDetailsView;
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
                _timeShiftingView = null;
                _timeShiftingPresenter = null;
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
                _pitchShiftingView = null;
                _pitchShiftingPresenter = null;
            };
            return _pitchShiftingView;
        }

        public virtual IEqualizerPresetsView CreateEqualizerPresetsView()
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
            {
                _equalizerPresetsPresenter = Bootstrapper.GetContainer().Resolve<IEqualizerPresetsPresenter>();
                _equalizerPresetsPresenter.BindView((IEqualizerPresetsView)view);
            };
            
            // Re-use the same instance as before
            if(_equalizerPresetsView == null)
            {
                // Create view and manage view destruction
                _equalizerPresetsView = Bootstrapper.GetContainer().Resolve<IEqualizerPresetsView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
                _equalizerPresetsView.OnViewDestroy = (view) =>
                {
                    _equalizerPresetsView = null;
                    _equalizerPresetsPresenter = null;
                };
            }
            return _equalizerPresetsView;
        }

        public virtual IEqualizerPresetDetailsView CreateEqualizerPresetDetailsView(EQPreset preset)
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
            {
                _equalizerPresetDetailsPresenter = Bootstrapper.GetContainer().Resolve<IEqualizerPresetDetailsPresenter>(new NamedParameterOverloads(){{"preset", preset}});
                _equalizerPresetDetailsPresenter.BindView((IEqualizerPresetDetailsView)view);
            };

            // Create view and manage view destruction
            _equalizerPresetDetailsView = Bootstrapper.GetContainer().Resolve<IEqualizerPresetDetailsView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _equalizerPresetDetailsView.OnViewDestroy = (view) =>
            {
                _equalizerPresetDetailsView = null;
                _equalizerPresetDetailsPresenter = null;
            };
            return _equalizerPresetDetailsView;
        }

        public virtual ISyncView CreateSyncView()
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
            {
                _syncPresenter = Bootstrapper.GetContainer().Resolve<ISyncPresenter>();
                _syncPresenter.BindView((ISyncView)view);
            };

            // Re-use the same instance as before
            if(_syncView == null)
            {
                // Create view and manage view destruction
                _syncView = Bootstrapper.GetContainer().Resolve<ISyncView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
                _syncView.OnViewDestroy = (view) =>
                {
                    _syncView = null;
                    _syncPresenter = null;
                };
            }
            return _syncView;
        }

        public virtual ISyncWebBrowserView CreateSyncWebBrowserView()
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
            {
                _syncWebBrowserPresenter = Bootstrapper.GetContainer().Resolve<ISyncWebBrowserPresenter>();
                _syncWebBrowserPresenter.BindView((ISyncWebBrowserView)view);
            };

            // Re-use the same instance as before
            if(_syncWebBrowserView == null)
            {
                // Create view and manage view destruction
                _syncWebBrowserView = Bootstrapper.GetContainer().Resolve<ISyncWebBrowserView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
                _syncWebBrowserView.OnViewDestroy = (view) =>
                {
                    _syncWebBrowserView = null;
                    _syncWebBrowserPresenter = null;
                };
            }
            return _syncWebBrowserView;
        }

        public virtual IAboutView CreateAboutView()
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
            {
                _aboutPresenter = Bootstrapper.GetContainer().Resolve<IAboutPresenter>();
                _aboutPresenter.BindView((IAboutView)view);
            };
            
            // Re-use the same instance as before
            if(_aboutView == null)
            {
                // Create view and manage view destruction
                _aboutView = Bootstrapper.GetContainer().Resolve<IAboutView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
                _aboutView.OnViewDestroy = (view) =>
                {
                    _aboutView = null;
                    _aboutPresenter = null;
                };
            }
            return _aboutView;
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
