﻿//
// NavigationManager.cs: Manager class for managing view and presenter instances.
//
// Copyright © 2011-2012 Yanick Castonguay
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
using TinyIoC;
using MPfm.MVP.Views;
using MPfm.MVP.Presenters.Interfaces;

namespace MPfm.MVP
{
    /// <summary>
    /// Manager class for managing view and presenter instances.
    /// </summary>
    public abstract class NavigationManager
    {
        private ISplashView _splashView;
        private ISplashPresenter _splashPresenter;

        private IMainView _mainView;
        private IMainPresenter _mainPresenter;
        private IPlayerPresenter _playerPresenter;
        private ILibraryBrowserPresenter _libraryBrowserPresenter;
        private ISongBrowserPresenter _songBrowserPresenter;

        private IPreferencesView _preferencesView;
        private IAudioPreferencesView _audioPreferencesView;
        private IGeneralPreferencesView _generalPreferencesView;
        private ILibraryPreferencesView _libraryPreferencesView;
        private IAudioPreferencesPresenter _audioPreferencesPresenter;
        private IGeneralPreferencesPresenter _generalPreferencesPresenter;
        private ILibraryPreferencesPresenter _libraryPreferencesPresenter;

        private IUpdateLibraryView _updateLibraryView;
        private IUpdateLibraryPresenter _updateLibraryPresenter;

        private Dictionary<IMobileLibraryBrowserView, IMobileLibraryBrowserPresenter> _mobileLibraryBrowserList = new Dictionary<IMobileLibraryBrowserView, IMobileLibraryBrowserPresenter>();

        public abstract void PushView(IBaseView context, IBaseView newView);

        public virtual void StartMobile()
        {
            // BindSplashView must have been called before.

            Action onInitDone = () =>
                {
                    var playlistsView = CreateMobileLibraryBrowserView(MobileLibraryBrowserType.Playlists);
                    PushView(null, playlistsView); // or AddTab() but that would be only on mobiles. 
                };

            _splashPresenter.Initialize(onInitDone);
        }

        public virtual void BindSplashView(ISplashView view, Action onInitDone)
        {
            _splashView = view;
            _splashPresenter = Bootstrapper.Bootstrapper.GetContainer().Resolve<ISplashPresenter>();
            _splashPresenter.BindView(view);
            //_splashPresenter.Initialize(onInitDone); // TODO: Should the presenter call NavMgr instead of using an action?
        }

        public virtual void BindUpdateLibraryView(IUpdateLibraryView view)
        {
            _updateLibraryView = view;
            _updateLibraryPresenter = Bootstrapper.Bootstrapper.GetContainer().Resolve<IUpdateLibraryPresenter>();
            _updateLibraryPresenter.BindView(view);
        }

        public virtual ISplashView CreateSplashView()
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action onInitDone = () =>
            {
                Console.WriteLine("SplashInitDone");
                CreateMainView();
            };
            return CreateSplashView(onInitDone);
        }

        public virtual ISplashView CreateSplashView(Action onInitDone)
        {
            Action<IBaseView> onViewReady = (view) => BindSplashView((ISplashView)view, onInitDone);
            _splashView = Bootstrapper.Bootstrapper.GetContainer().Resolve<ISplashView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _splashView.OnViewDestroy = (view) => {
                _splashView = null;
                _splashPresenter = null;
            };
            return _splashView;
        }
        
        public virtual IMainView CreateMainView()
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) => {
                _mainPresenter = Bootstrapper.Bootstrapper.GetContainer().Resolve<IMainPresenter>();
                _mainPresenter.BindView((IMainView)view);                
                _playerPresenter = Bootstrapper.Bootstrapper.GetContainer().Resolve<IPlayerPresenter>();
                _playerPresenter.BindView((IPlayerView)view);
                _libraryBrowserPresenter = Bootstrapper.Bootstrapper.GetContainer().Resolve<ILibraryBrowserPresenter>();
                _libraryBrowserPresenter.BindView((ILibraryBrowserView)view);                
                _songBrowserPresenter = Bootstrapper.Bootstrapper.GetContainer().Resolve<ISongBrowserPresenter>();
                _songBrowserPresenter.BindView((ISongBrowserView)view);                
            };            
            _mainView = Bootstrapper.Bootstrapper.GetContainer().Resolve<IMainView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _mainView.OnViewDestroy = (view) => {
                _mainView = null;
                _mainPresenter = null;
                _playerPresenter.Dispose(); // Dispose unmanaged stuff (i.e. BASS)
                _playerPresenter = null;
                _libraryBrowserPresenter = null;
                _songBrowserPresenter = null;
            };
            return _mainView;
        }
        
        public virtual IPreferencesView CreatePreferencesView()
        {
            // If the view is still visible, just make it the top level window
            if(_preferencesView != null)
            {
                _preferencesView.ShowView(true);
                return _preferencesView;
            }
            
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
                {                    
                    _audioPreferencesPresenter = Bootstrapper.Bootstrapper.GetContainer().Resolve<IAudioPreferencesPresenter>();
                    _audioPreferencesPresenter.BindView((IAudioPreferencesView)view);
                    _generalPreferencesPresenter = Bootstrapper.Bootstrapper.GetContainer().Resolve<IGeneralPreferencesPresenter>();
                    _generalPreferencesPresenter.BindView((IGeneralPreferencesView)view);
                    _libraryPreferencesPresenter = Bootstrapper.Bootstrapper.GetContainer().Resolve<ILibraryPreferencesPresenter>();
                    _libraryPreferencesPresenter.BindView((ILibraryPreferencesView)view);
                };
            
            // Create view and manage view destruction
            _preferencesView = Bootstrapper.Bootstrapper.GetContainer().Resolve<IPreferencesView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _preferencesView.OnViewDestroy = (view) => {
                _preferencesView = null;
                _audioPreferencesPresenter = null;
                _generalPreferencesPresenter = null;
                _libraryPreferencesPresenter = null;
            };
            return _preferencesView;
        }

        public virtual IAudioPreferencesView CreateAudioPreferencesView()
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
                {
                    _audioPreferencesPresenter = Bootstrapper.Bootstrapper.GetContainer().Resolve<IAudioPreferencesPresenter>();
                    _audioPreferencesPresenter.BindView((IAudioPreferencesView)view);
                };

            // Create view and manage view destruction
            _audioPreferencesView = Bootstrapper.Bootstrapper.GetContainer().Resolve<IAudioPreferencesView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
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
                _generalPreferencesPresenter = Bootstrapper.Bootstrapper.GetContainer().Resolve<IGeneralPreferencesPresenter>();
                _generalPreferencesPresenter.BindView((IGeneralPreferencesView)view);
            };

            // Create view and manage view destruction
            _generalPreferencesView = Bootstrapper.Bootstrapper.GetContainer().Resolve<IGeneralPreferencesView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
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
                _libraryPreferencesPresenter = Bootstrapper.Bootstrapper.GetContainer().Resolve<ILibraryPreferencesPresenter>();
                _libraryPreferencesPresenter.BindView((ILibraryPreferencesView)view);
            };

            // Create view and manage view destruction
            _libraryPreferencesView = Bootstrapper.Bootstrapper.GetContainer().Resolve<ILibraryPreferencesView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _libraryPreferencesView.OnViewDestroy = (view) =>
            {
                _libraryPreferencesView = null;
                _libraryPreferencesPresenter = null;
            };
            return _libraryPreferencesView;
        }

        public virtual IMobileLibraryBrowserView CreateMobileLibraryBrowserView(MobileLibraryBrowserType browserType)
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
            {
                var presenter = Bootstrapper.Bootstrapper.GetContainer().Resolve<IMobileLibraryBrowserPresenter>();
                presenter.BindView((IMobileLibraryBrowserView)view);
                _mobileLibraryBrowserList.Add((IMobileLibraryBrowserView)view, presenter);
            };

            // Create view and manage view destruction
            var newView = Bootstrapper.Bootstrapper.GetContainer().Resolve<IMobileLibraryBrowserView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            newView.BrowserType = browserType; // TODO: Shouldn't this be in the presenter instead...? browserType + filter (can be artist or album)
            newView.OnViewDestroy = (view) =>
            {
                if (_mobileLibraryBrowserList.ContainsKey((IMobileLibraryBrowserView)view))
                    _mobileLibraryBrowserList.Remove((IMobileLibraryBrowserView)view);
            };
            return newView;
        }
    
    }
}
