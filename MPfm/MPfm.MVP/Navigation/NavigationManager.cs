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
using MPfm.Library.Objects;
using MPfm.Sound.AudioFiles;

namespace MPfm.MVP.Navigation
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

        private IDesktopPreferencesView _desktopPreferencesView;
        private IAudioPreferencesPresenter _audioPreferencesPresenter;
        private IGeneralPreferencesPresenter _generalPreferencesPresenter;
        private ILibraryPreferencesPresenter _libraryPreferencesPresenter;

        private IUpdateLibraryView _updateLibraryView;
        private IUpdateLibraryPresenter _updateLibraryPresenter;

        private ISyncView _syncView;
        private ISyncPresenter _syncPresenter;
        private ISyncMenuView _syncMenuView;
        private ISyncMenuPresenter _syncMenuPresenter;
        private ISyncDownloadView _syncDownloadView;
        private ISyncDownloadPresenter _syncDownloadPresenter;

        public virtual ISplashView CreateSplashView()
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action onInitDone = () =>
            {
                Console.WriteLine("SplashInitDone");
                CreateMainView();
            };
            Action<IBaseView> onViewReady = (view) =>
            {
                _splashPresenter = Bootstrapper.GetContainer().Resolve<ISplashPresenter>();
                _splashPresenter.BindView((ISplashView)view);
                _splashPresenter.Initialize(onInitDone); // TODO: Should the presenter call NavMgr instead of using an action?
            };
            _splashView = Bootstrapper.GetContainer().Resolve<ISplashView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _splashView.OnViewDestroy = (view) =>
            {
                _splashView = null;
                _splashPresenter = null;
            };
            return _splashView;
        }
        
        public virtual IMainView CreateMainView()
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) => {
                _mainPresenter = Bootstrapper.GetContainer().Resolve<IMainPresenter>();
                _mainPresenter.BindView((IMainView)view);                
                _playerPresenter = Bootstrapper.GetContainer().Resolve<IPlayerPresenter>();
                _playerPresenter.BindView((IPlayerView)view);
                _libraryBrowserPresenter = Bootstrapper.GetContainer().Resolve<ILibraryBrowserPresenter>();
                _libraryBrowserPresenter.BindView((ILibraryBrowserView)view);                
                _songBrowserPresenter = Bootstrapper.GetContainer().Resolve<ISongBrowserPresenter>();
                _songBrowserPresenter.BindView((ISongBrowserView)view);                
            };            
            _mainView = Bootstrapper.GetContainer().Resolve<IMainView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
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
        
        public virtual IDesktopPreferencesView CreatePreferencesView()
        {
            // If the view is still visible, just make it the top level window
            if(_desktopPreferencesView != null)
            {
                _desktopPreferencesView.ShowView(true);
                return _desktopPreferencesView;
            }
            
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) => {                    
                _audioPreferencesPresenter = Bootstrapper.GetContainer().Resolve<IAudioPreferencesPresenter>();
                _audioPreferencesPresenter.BindView((IAudioPreferencesView)view);
                _generalPreferencesPresenter = Bootstrapper.GetContainer().Resolve<IGeneralPreferencesPresenter>();
                _generalPreferencesPresenter.BindView((IGeneralPreferencesView)view);
                _libraryPreferencesPresenter = Bootstrapper.GetContainer().Resolve<ILibraryPreferencesPresenter>();
                _libraryPreferencesPresenter.BindView((ILibraryPreferencesView)view);
            };
            
            // Create view and manage view destruction
            _desktopPreferencesView = Bootstrapper.GetContainer().Resolve<IDesktopPreferencesView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _desktopPreferencesView.OnViewDestroy = (view) => {
                _desktopPreferencesView = null;
                _audioPreferencesPresenter = null;
                _generalPreferencesPresenter = null;
                _libraryPreferencesPresenter = null;
            };
            return _desktopPreferencesView;
        }

        public virtual ISyncView CreateSyncView()
        {
            // If the view is still visible, just make it the top level window
            if(_syncView != null)
            {
                _syncView.ShowView(true);
                return _syncView;
            }
            
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) =>
                {
                    _syncPresenter = Bootstrapper.GetContainer().Resolve<ISyncPresenter>();
                    _syncPresenter.BindView((ISyncView)view);
                };
            
            // Create view and manage view destruction
            _syncView = Bootstrapper.GetContainer().Resolve<ISyncView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _syncView.OnViewDestroy = (view) => {
                _syncView = null;
                _syncPresenter = null;
            };
            return _syncView;
        }

        public virtual ISyncMenuView CreateSyncMenuView(SyncDevice device)
        {
            if(_syncMenuView != null)
            {
                _syncMenuView.ShowView(true);
                return _syncMenuView;
            }

            Action<IBaseView> onViewReady = (view) =>
            {                    
                _syncMenuPresenter = Bootstrapper.GetContainer().Resolve<ISyncMenuPresenter>();
                _syncMenuPresenter.BindView((ISyncMenuView)view);
                _syncMenuPresenter.SetSyncDevice(device);
            };

            _syncMenuView = Bootstrapper.GetContainer().Resolve<ISyncMenuView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _syncMenuView.OnViewDestroy = (view) => {
                _syncMenuView = null;
                _syncMenuPresenter = null;
            };
            return _syncMenuView;
        }

        public virtual ISyncDownloadView CreateSyncDownloadView(SyncDevice device, IEnumerable<AudioFile> audioFiles)
        {
            if(_syncDownloadView != null)
            {
                _syncDownloadView.ShowView(true);
                return _syncDownloadView;
            }

            Action<IBaseView> onViewReady = (view) =>
            {                    
                _syncDownloadPresenter = Bootstrapper.GetContainer().Resolve<ISyncDownloadPresenter>();
                _syncDownloadPresenter.BindView((ISyncDownloadView)view);
                _syncDownloadPresenter.StartSync(device, audioFiles);
            };

            _syncDownloadView = Bootstrapper.GetContainer().Resolve<ISyncDownloadView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _syncDownloadView.OnViewDestroy = (view) => {
                _syncDownloadView = null;
                _syncDownloadPresenter = null;
            };
            return _syncDownloadView;
        }
    }
}
