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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using TinyMessenger;
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
        ISplashView splashView;
        ISplashPresenter splashPresenter;
        
        IMainView mainView;
        IMainPresenter mainPresenter;
        IPlayerPresenter playerPresenter;
        ILibraryBrowserPresenter libraryBrowserPresenter;
        ISongBrowserPresenter songBrowserPresenter;
        
        IPreferencesView preferencesView;
        IPreferencesPresenter preferencesPresenter;
        
        public virtual void Start()
        {
            CreateSplashWindow();
        }
        
        public virtual void CreateSplashWindow()
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action onInitDone = () => {
                Console.WriteLine("SplashInitDone");
                CreateMainWindow();
            };
            Action<IBaseView> onViewReady = (view) => {
                splashPresenter = Bootstrapper.GetContainer().Resolve<ISplashPresenter>();
                splashPresenter.BindView((ISplashView)view);
                splashPresenter.Initialize(onInitDone); // TODO: Should the presenter call NavMgr instead of using an action?
            };
            splashView = Bootstrapper.GetContainer().Resolve<ISplashView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            splashView.OnViewDestroy = () => {
                splashView = null;
                splashPresenter = null;
            };
        }
        
        public virtual void CreateMainWindow()
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) => {
                mainPresenter = Bootstrapper.GetContainer().Resolve<IMainPresenter>();
                mainPresenter.BindView((IMainView)view);                
                playerPresenter = Bootstrapper.GetContainer().Resolve<IPlayerPresenter>();
                playerPresenter.BindView((IPlayerView)view);
                libraryBrowserPresenter = Bootstrapper.GetContainer().Resolve<ILibraryBrowserPresenter>();
                libraryBrowserPresenter.BindView((ILibraryBrowserView)view);                
                songBrowserPresenter = Bootstrapper.GetContainer().Resolve<ISongBrowserPresenter>();
                songBrowserPresenter.BindView((ISongBrowserView)view);                
            };            
            mainView = Bootstrapper.GetContainer().Resolve<IMainView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            mainView.OnViewDestroy = () => {
                mainView = null;
                mainPresenter = null;
                playerPresenter.Dispose(); // Dispose unmanaged stuff (i.e. BASS)
                playerPresenter = null;
                libraryBrowserPresenter = null;
                songBrowserPresenter = null;
            };
        }
        
        public virtual void CreatePreferencesWindow()
        {
            // If the view is still visible, just make it the top level window
            if(preferencesView != null)
            {
                preferencesView.ShowView(true);
                return;
            }
            
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) => {
                preferencesPresenter = Bootstrapper.GetContainer().Resolve<IPreferencesPresenter>();
                preferencesPresenter.BindView((IPreferencesView)view);                
            };            
            
            // Create view and manage view destruction
            preferencesView = Bootstrapper.GetContainer().Resolve<IPreferencesView>(new NamedParameterOverloads() { { "onViewReady", onViewReady } });
            preferencesView.OnViewDestroy = () => {
                preferencesView = null;
                preferencesPresenter = null;
            };
        }
    }
}
