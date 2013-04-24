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
using MPfm.iOS.Classes.Objects;
using MonoTouch.CoreAnimation;

namespace MPfm.iOS.Classes.Delegates
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{		
		MPfmWindow _window;
        MPfmTabBarController _tabBarController;
        public MPfmTabBarController TabBarController { get { return _tabBarController; } }
        SplashViewController _splashViewController;
		iOSNavigationManager _navigationManager;
        List<KeyValuePair<MobileNavigationTabType, MPfmNavigationController>> _navigationControllers = new List<KeyValuePair<MobileNavigationTabType, MPfmNavigationController>>();
        List<KeyValuePair<string, MPfmNavigationController>> _dialogNavigationControllers = new List<KeyValuePair<string, MPfmNavigationController>>();

		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
            TinyIoC.TinyIoCContainer container = Bootstrapper.GetContainer();
            RegisterViews();

            UINavigationBar.Appearance.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
            UINavigationBar.Appearance.BackgroundColor = GlobalTheme.MainColor;
            UIToolbar.Appearance.SetBackgroundImage(new UIImage(), UIToolbarPosition.Bottom, UIBarMetrics.Default);
            UIToolbar.Appearance.BackgroundColor = GlobalTheme.MainColor;
            //UITabBar.Appearance.SelectionIndicatorImage = new UIImage();

            // Create window 
			_window = new MPfmWindow(UIScreen.MainScreen.Bounds);

            // Create tab bar controller, but hide it while the splash screen is visible
            _tabBarController = new MPfmTabBarController();
            _tabBarController.View.Hidden = true;
            //_tabBarController.TabBar.TintColor = UIColor.FromRGBA(0.2f, 0.2f, 0.2f, 1);
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

        public void RegisterViews()
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
            container.Register<IEqualizerPresetsView, EqualizerPresetsViewController>().AsMultiInstance();
            container.Register<IEqualizerPresetDetailsView, EqualizerPresetDetailsViewController>().AsMultiInstance();
            container.Register<ILoopsView, LoopsViewController>().AsMultiInstance();
            container.Register<ILoopDetailsView, LoopDetailsViewController>().AsMultiInstance();
            container.Register<IMarkersView, MarkersViewController>().AsMultiInstance();
            container.Register<IMarkerDetailsView, MarkerDetailsViewController>().AsMultiInstance();
            container.Register<ITimeShiftingView, TimeShiftingViewController>().AsMultiInstance();
            container.Register<IPitchShiftingView, PitchShiftingViewController>().AsMultiInstance();
            container.Register<IPlayerMetadataView, PlayerMetadataViewController>().AsMultiInstance();
        }

        public void ShowSplash(SplashViewController viewController)
        {
            InvokeOnMainThread(() => {
                _splashViewController = viewController;
                _splashViewController.View.Frame = _window.Frame;
                _splashViewController.View.AutoresizingMask = UIViewAutoresizing.All;
                _window.AddSubview(_splashViewController.View);
                _window.MakeKeyAndVisible();

                // The application is now ready to receive remote events
                UIApplication.SharedApplication.BeginReceivingRemoteControlEvents();
            });
            }

        public void HideSplash()
        {
            InvokeOnMainThread(() => {
                _tabBarController.View.Hidden = false;
                _splashViewController.View.RemoveFromSuperview();
            });
        }

        public void AddTab(MobileNavigationTabType type, string title, UIViewController viewController)
        {
            InvokeOnMainThread(() => {
                // Create text attributes for tab
                UITextAttributes attr = new UITextAttributes();
                attr.Font = UIFont.FromName("HelveticaNeue-Medium", 11);
                attr.TextColor = UIColor.LightGray;
                attr.TextShadowColor = UIColor.Clear;
                UITextAttributes attrSelected = new UITextAttributes();
                attrSelected.Font = UIFont.FromName("HelveticaNeue-Medium", 11);
                attrSelected.TextColor = UIColor.White;
                attrSelected.TextShadowColor = UIColor.Clear;

                var navCtrl = new MPfmNavigationController(type);
                navCtrl.SetTitle(title, "");
                navCtrl.TabBarItem.SetTitleTextAttributes(attr, UIControlState.Normal);
                navCtrl.TabBarItem.SetTitleTextAttributes(attrSelected, UIControlState.Selected);
                navCtrl.TabBarItem.Title = title;
                //navCtrl.TabBarItem.SetFinishedImages(UIImage.FromBundle("Images/Tabs/tab_selected"), UIImage.FromBundle("Images/Tabs/tab"));
                if(title.ToUpper() == "MORE")
                    navCtrl.TabBarItem.Image = UIImage.FromBundle("Images/Tabs/more");
                else
                    navCtrl.TabBarItem.Image = UIImage.FromBundle("Images/Tabs/audio");

//                CAGradientLayer gradient = new CAGradientLayer();
//                gradient.Frame = navCtrl.View.Frame;
//                gradient.Colors = new MonoTouch.CoreGraphics.CGColor[2] { GlobalTheme.MainColor.CGColor, GlobalTheme.SecondaryColor.CGColor };
//                navCtrl.View.Layer.InsertSublayer(gradient, 0);               

                navCtrl.PushViewController(viewController, false);
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

        public void PushDialogView(string viewTitle, UIViewController viewController)
        {
            InvokeOnMainThread(() => {
                var navCtrl = new MPfmNavigationController(MobileNavigationTabType.More); // TODO: Remove tab type
                navCtrl.SetTitle(viewTitle, "");
                navCtrl.NavigationBar.TintColor = UIColor.FromRGBA(0.2f, 0.2f, 0.2f, 1);                
                navCtrl.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                navCtrl.ModalInPopover = true;
                navCtrl.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
                navCtrl.ViewDismissedEvent += (sender, e) => {
                    _dialogNavigationControllers.Remove(new KeyValuePair<string, MPfmNavigationController>(viewTitle, navCtrl));
                };
                navCtrl.PushViewController(viewController, false);                
                _dialogNavigationControllers.Add(new KeyValuePair<string, MPfmNavigationController>(viewTitle, navCtrl));
                _tabBarController.PresentViewController(navCtrl, true, null);
            });

            // TODO: Remove navCtrl from list when dialog is closed.
        }

        public void PushDialogSubview(string parentViewTitle, UIViewController viewController)
        {
            InvokeOnMainThread(() => {
                var navCtrl = _dialogNavigationControllers.FirstOrDefault(x => x.Key == parentViewTitle).Value;
                navCtrl.PushViewController(viewController, true);
            });
        }

        // pushdialogsubview (with nav mgr)/ this requires viewTitle. ex: PushDialogSubView("Effects", viewInstance)
        // this makes a navctrl mandatory for every dialog view (update library, effects, about, etc.).
    }
}
