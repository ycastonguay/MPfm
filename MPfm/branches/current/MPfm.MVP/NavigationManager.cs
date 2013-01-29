//
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
        private IPreferencesPresenter _preferencesPresenter;

        private IUpdateLibraryView _updateLibraryView;
        private IUpdateLibraryPresenter _updateLibraryPresenter;

        public virtual void BindSplashView(ISplashView view, Action onInitDone)
        {
            _splashView = view;
            _splashPresenter = Bootstrapper.GetContainer().Resolve<ISplashPresenter>();
            _splashPresenter.BindView(view);
            _splashPresenter.Initialize(onInitDone); // TODO: Should the presenter call NavMgr instead of using an action?
        }

        public virtual void BindUpdateLibraryView(IUpdateLibraryView view)
        {
            _updateLibraryView = view;
            _updateLibraryPresenter = Bootstrapper.GetContainer().Resolve<IUpdateLibraryPresenter>();
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
            _splashView = Bootstrapper.GetContainer().Resolve<ISplashView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _splashView.OnViewDestroy = () => {
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
            _mainView.OnViewDestroy = () => {
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
            Action<IBaseView> onViewReady = (view) => {
                _preferencesPresenter = Bootstrapper.GetContainer().Resolve<IPreferencesPresenter>();
                _preferencesPresenter.BindView((IPreferencesView)view);                
            };            
            
            // Create view and manage view destruction
            _preferencesView = Bootstrapper.GetContainer().Resolve<IPreferencesView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            _preferencesView.OnViewDestroy = () => {
                _preferencesView = null;
                _preferencesPresenter = null;
            };
            return _preferencesView;
        }
    }
}
