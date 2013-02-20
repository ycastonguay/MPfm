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

namespace MPfm.MVP.Navigation
{
    /// <summary>
    /// Manager class for managing view and presenter instances.
    /// </summary>
    public abstract class MobileNavigationManager
    {
        private readonly object _locker = new object();

        private IMobileOptionsMenuView _optionsMenuView;
        private IMobileOptionsMenuPresenter _optionsMenuPresenter;

        private ISplashView _splashView;
        private ISplashPresenter _splashPresenter;

        private IPlayerView _playerView;
        private IPlayerPresenter _playerPresenter;

        private IAudioPreferencesView _audioPreferencesView;
        private IGeneralPreferencesView _generalPreferencesView;
        private ILibraryPreferencesView _libraryPreferencesView;
        private IAudioPreferencesPresenter _audioPreferencesPresenter;
        private IGeneralPreferencesPresenter _generalPreferencesPresenter;
        private ILibraryPreferencesPresenter _libraryPreferencesPresenter;

        private IUpdateLibraryView _updateLibraryView;
        private IUpdateLibraryPresenter _updateLibraryPresenter;

        private Dictionary<IMobileLibraryBrowserView, IMobileLibraryBrowserPresenter> _mobileLibraryBrowserList = new Dictionary<IMobileLibraryBrowserView, IMobileLibraryBrowserPresenter>();

        public abstract void ShowSplash(ISplashView view);
        public abstract void HideSplash();
        public abstract void AddTab(MobileNavigationTabType type, string title, IBaseView view);
        public abstract void PushTabView(MobileNavigationTabType type, IBaseView view);

        public virtual void Start()
        {
            Action onInitDone = () =>
            {                
                // Create 4 main tabs
                var playlistsView = CreateMobileLibraryBrowserView(MobileNavigationTabType.Playlists, MobileLibraryBrowserType.Playlists);
                var artistsView = CreateMobileLibraryBrowserView(MobileNavigationTabType.Artists, MobileLibraryBrowserType.Artists);
                var albumsView = CreateMobileLibraryBrowserView(MobileNavigationTabType.Albums, MobileLibraryBrowserType.Albums);
                var songsView = CreateMobileLibraryBrowserView(MobileNavigationTabType.Songs, MobileLibraryBrowserType.Songs);
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
        
        public virtual IAudioPreferencesView CreateAudioPreferencesView()
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
                {
                    _audioPreferencesPresenter = Bootstrapper.GetContainer().Resolve<IAudioPreferencesPresenter>();
                    _audioPreferencesPresenter.BindView((IAudioPreferencesView)view);
                };

            // Create view and manage view destruction
            _audioPreferencesView = Bootstrapper.GetContainer().Resolve<IAudioPreferencesView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _audioPreferencesView.OnViewDestroy = (view) =>
            {
                _audioPreferencesView = null;
                _audioPreferencesPresenter = null;
            };
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
            _generalPreferencesView = Bootstrapper.GetContainer().Resolve<IGeneralPreferencesView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _generalPreferencesView.OnViewDestroy = (view) =>
            {
                _generalPreferencesView = null;
                _generalPreferencesPresenter = null;
            };
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
            _libraryPreferencesView = Bootstrapper.GetContainer().Resolve<ILibraryPreferencesView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _libraryPreferencesView.OnViewDestroy = (view) =>
            {
                _libraryPreferencesView = null;
                _libraryPreferencesPresenter = null;
            };
            return _libraryPreferencesView;
        }

        public virtual IMobileLibraryBrowserView CreateMobileLibraryBrowserView(MobileNavigationTabType tabType, MobileLibraryBrowserType browserType)
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
            {
                // The view list can be accessed from different threads.
                lock (_locker)
                {
                    var presenter = Bootstrapper.GetContainer().Resolve<IMobileLibraryBrowserPresenter>(new NamedParameterOverloads() 
                        {{"tabType", tabType }, { "browserType", browserType}});
                    presenter.BindView((IMobileLibraryBrowserView) view);
                    _mobileLibraryBrowserList.Add((IMobileLibraryBrowserView) view, presenter);
                }
            };

            // Create view and manage view destruction
            IMobileLibraryBrowserView newView = null;
            newView = Bootstrapper.GetContainer().Resolve<IMobileLibraryBrowserView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            newView.OnViewDestroy = (view) =>
            {
                // The view list can be accessed from different threads.
                lock (_locker)
                {
                    if (_mobileLibraryBrowserList.ContainsKey((IMobileLibraryBrowserView) view))
                        _mobileLibraryBrowserList.Remove((IMobileLibraryBrowserView) view);
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
                if (onViewBindedToPresenter != null)
                    onViewBindedToPresenter(view);
            };

            // Create view and manage view destruction
            _playerView = Bootstrapper.GetContainer().Resolve<IPlayerView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _playerView.OnViewDestroy = (view) =>
            {
                _playerView = null;
                _playerPresenter = null;
            };
            return _playerView;
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
