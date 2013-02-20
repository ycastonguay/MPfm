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
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controls;
using MPfm.iOS.Classes.Controllers;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.iOS.Classes.Navigation;

namespace MPfm.iOS.Classes.Delegates
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{		
		UIWindow _window;
        UITabBarController _tabBarController;
        SplashViewController _splashViewController;
		iOSNavigationManager _navigationManager;
        List<KeyValuePair<MobileNavigationTabType, MPfmNavigationController>> _navigationControllers = new List<KeyValuePair<MobileNavigationTabType, MPfmNavigationController>>();

		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			// Complete IoC configuration
			TinyIoC.TinyIoCContainer container = Bootstrapper.GetContainer();
			container.Register<MobileNavigationManager, iOSNavigationManager>().AsSingleton();
			container.Register<ISplashView, SplashViewController>().AsMultiInstance();
            container.Register<IMobileOptionsMenuView, MoreViewController>().AsMultiInstance();
			container.Register<IPlayerView, PlayerViewController>().AsMultiInstance();
			container.Register<IUpdateLibraryView, UpdateLibraryViewController>().AsMultiInstance();
			container.Register<IMobileLibraryBrowserView, MobileLibraryBrowserViewController>().AsMultiInstance();
			//container.Register<IAudioPreferencesView, AudioPreferencesFragment>().AsMultiInstance();
			//container.Register<IGeneralPreferencesView, GeneralPreferencesFragment>().AsMultiInstance();
			//container.Register<ILibraryPreferencesView, LibraryPreferencesFragment>().AsMultiInstance();

            // Create window 
			_window = new UIWindow(UIScreen.MainScreen.Bounds);

            // Create tab bar controller, but hide it while the splash screen is visible
            _tabBarController = new UITabBarController();
            _tabBarController.View.Hidden = true;
            _window.RootViewController = _tabBarController;

			// Start navigation manager
			_navigationManager = (iOSNavigationManager)container.Resolve<MobileNavigationManager>();
			_navigationManager.AppDelegate = this;
			_navigationManager.Start();
			
			return true;
		}

        public override void WillTerminate(UIApplication application)
        {
            // Clean up player
            if (MPfm.Player.Player.CurrentPlayer.IsPlaying)
            {
                MPfm.Player.Player.CurrentPlayer.Stop();
            }
            MPfm.Player.Player.CurrentPlayer.Dispose();
        }

        public void ShowSplash(SplashViewController viewController)
        {
            InvokeOnMainThread(() => {
                _splashViewController = viewController;
                _splashViewController.View.Frame = _window.Frame;
                _splashViewController.View.AutoresizingMask = UIViewAutoresizing.All;
                _window.AddSubview(_splashViewController.View);
                _window.MakeKeyAndVisible();
            });
            }

        public void HideSplash()
        {
            InvokeOnMainThread(() => {
                _tabBarController.View.Hidden = false;            
            });
        }

        public void AddTab(MobileNavigationTabType type, string title, UIViewController viewController)
        {
            InvokeOnMainThread(() => {
                // Create text attributes for tab
                UITextAttributes attr = new UITextAttributes();
                //attr.Font = UIFont.FromName("Junction", 11);
                attr.Font = UIFont.FromName("OstrichSans-Black", 13);
                attr.TextColor = UIColor.White;
                attr.TextShadowColor = UIColor.DarkGray;
                attr.TextShadowOffset = new UIOffset(1, 1);

                // Create navigation controller
                var navCtrl = new MPfmNavigationController("OstrichSans-Black", 26);
                navCtrl.SetTitle(title);
                navCtrl.NavigationBar.BackgroundColor = UIColor.Clear;
                navCtrl.NavigationBar.TintColor = UIColor.Clear;
                navCtrl.TabBarItem.SetTitleTextAttributes(attr, UIControlState.Normal);
                navCtrl.TabBarItem.Title = title;
                navCtrl.TabBarItem.Image = UIImage.FromBundle("Images/Tabs/more");
                navCtrl.PushViewController(viewController, false);

                // Add view controller to list
                _navigationControllers.Add(new KeyValuePair<MobileNavigationTabType, MPfmNavigationController>(type, navCtrl));

                // Add navigation controller as a tab
                var list = new List<UIViewController>();
                if (_tabBarController.ViewControllers != null)
                    list = _tabBarController.ViewControllers.ToList();
                list.Add(navCtrl);
                _tabBarController.ViewControllers = list.ToArray();
            });
        }

        public void PushTabView(MobileNavigationTabType type, UIViewController viewController)
        {
            InvokeOnMainThread(() => {
                if (viewController is PlayerViewController)
                {
                    viewController.HidesBottomBarWhenPushed = true;
                }

                var navCtrl = _navigationControllers.FirstOrDefault(x => x.Key == type).Value;
                navCtrl.PushViewController(viewController, true);
            });
        }

        public void PushDialogView(UIViewController viewController)
        {
            InvokeOnMainThread(() => {
                if (viewController is UpdateLibraryViewController)
                {
                    viewController.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                    viewController.ModalInPopover = true;
                    viewController.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
                    _tabBarController.PresentViewController(viewController, true, null);
                }
            });
        }
    }
}
