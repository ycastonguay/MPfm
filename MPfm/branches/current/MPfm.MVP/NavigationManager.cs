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
using Ninject;
using Ninject.Parameters;

namespace MPfm.MVP
{
    /// <summary>
    /// Manager class for managing view and presenter instances.
    /// </summary>
    public static class NavigationManager
    {
        static ISplashView splashView;
        static ISplashPresenter splashPresenter;
        
        static IMainView mainView;
        static IMainPresenter mainPresenter;
        static IPlayerPresenter playerPresenter;
        static ILibraryBrowserPresenter libraryBrowserPresenter;
        static ISongBrowserPresenter songBrowserPresenter;
        
        static IPreferencesView preferencesView;
        static IPreferencesPresenter preferencesPresenter;                       

        static NavigationManager()
        {
        }
        
        public static void Start()
        {
            CreateSplashWindow();
        }

        public static void CreateSplashWindow()
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action onInitDone = () => {
                Console.WriteLine("SplashInitDone");
                CreateMainWindow();
            };
            Action<IBaseView> onViewReady = (view) => {
                splashPresenter = Bootstrapper.GetKernel().Get<ISplashPresenter>();
                splashPresenter.BindView((ISplashView)view);
                splashPresenter.Initialize(onInitDone); // TODO: Should the presenter call NavMgr instead of using an action?
            };
            splashView = Bootstrapper.GetKernel().Get<ISplashView>(new ConstructorArgument("onViewReady", onViewReady));
        }
        
        public static void CreateMainWindow()
        {
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) => {
                mainPresenter = Bootstrapper.GetKernel().Get<IMainPresenter>();
                mainPresenter.BindView((IMainView)view);                
                playerPresenter = Bootstrapper.GetKernel().Get<IPlayerPresenter>();
                playerPresenter.BindView((IPlayerView)view);
                libraryBrowserPresenter = Bootstrapper.GetKernel().Get<ILibraryBrowserPresenter>();
                libraryBrowserPresenter.BindView((ILibraryBrowserView)view);                
                songBrowserPresenter = Bootstrapper.GetKernel().Get<ISongBrowserPresenter>();
                songBrowserPresenter.BindView((ISongBrowserView)view);                
            };            
            mainView = Bootstrapper.GetKernel().Get<IMainView>(new ConstructorArgument("onViewReady", onViewReady));            
        }
        
        public static void CreatePreferencesWindow()
        {
            // If the view is still visible, just make it the top level window
            if(preferencesView != null)
            {
                preferencesView.ShowView(true);
                return;
            }
            
            // The view invokes the OnViewReady action when the view is ready. This means the presenter can be created and bound to the view.
            Action<IBaseView> onViewReady = (view) => {
                preferencesPresenter = Bootstrapper.GetKernel().Get<IPreferencesPresenter>();
                preferencesPresenter.BindView((IPreferencesView)view);                
            };            
            
            // Create view and manage view destruction
            preferencesView = Bootstrapper.GetKernel().Get<IPreferencesView>(new ConstructorArgument("onViewReady", onViewReady));
            preferencesView.OnViewDestroy = (view) => {
                preferencesView = null;
                preferencesPresenter = null;
            };
        }
    }
}
