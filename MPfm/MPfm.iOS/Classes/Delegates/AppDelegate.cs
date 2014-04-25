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
using MPfm.Library;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Config.Providers;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using TinyMessenger;
using MPfm.iOS.Classes.Controllers;
using MPfm.iOS.Classes.Controls;
using MPfm.iOS.Classes.Navigation;
using MPfm.iOS.Classes.Objects;
using MPfm.iOS.Classes.Providers;
using MPfm.iOS.Helpers;
using DropBoxSync.iOS;
using MPfm.iOS.Classes.Services;
using MPfm.Library.Services.Interfaces;
using System.Drawing;
using MPfm.GenericControls.Graphics;
using MPfm.iOS.Classes.Controls.Graphics;
using MPfm.GenericControls.Services.Interfaces;
using MPfm.GenericControls.Services;

namespace MPfm.iOS.Classes.Delegates
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		MPfmWindow _window;
		MainViewController _mainViewController;
        SplashViewController _splashViewController;
		iOSNavigationManager _navigationManager;
        List<KeyValuePair<MobileNavigationTabType, MPfmNavigationController>> _navigationControllers = new List<KeyValuePair<MobileNavigationTabType, MPfmNavigationController>>();
        List<KeyValuePair<string, MPfmNavigationController>> _dialogNavigationControllers = new List<KeyValuePair<string, MPfmNavigationController>>();

		public MainViewController MainViewController { get { return _mainViewController; } }

		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
//			if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
//				UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.Default, true);

            TinyIoC.TinyIoCContainer container = Bootstrapper.GetContainer();
            RegisterViews();

            UINavigationBar.Appearance.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
            UINavigationBar.Appearance.BackgroundColor = GlobalTheme.MainColor;
            UIToolbar.Appearance.SetBackgroundImage(new UIImage(), UIToolbarPosition.Bottom, UIBarMetrics.Default);
            UIToolbar.Appearance.BackgroundColor = GlobalTheme.MainColor;
            UITabBar.Appearance.TintColor = UIColor.White;
            UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.LightContent;

			_window = new MPfmWindow(UIScreen.MainScreen.Bounds);
			_window.BackgroundColor = GlobalTheme.MainColor;

			if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
			{
				_window.TintColor = GlobalTheme.SecondaryColor;
				//UINavigationBar.Appearance.BarTintColor = UIColor.Yellow;
			}

			// Start navigation manager
			_navigationManager = (iOSNavigationManager)container.Resolve<MobileNavigationManager>();
			_navigationManager.AppDelegate = this;
			_navigationManager.Start();
			
			return true;
		}

        public override void WillTerminate(UIApplication application)
        {
            if (MPfm.Player.Player.CurrentPlayer.IsPlaying)
                MPfm.Player.Player.CurrentPlayer.Stop();
            MPfm.Player.Player.CurrentPlayer.Dispose();
        }

        public void RegisterViews()
        {
            // Complete IoC configuration
            TinyIoC.TinyIoCContainer container = Bootstrapper.GetContainer();
			container.Register<IMemoryGraphicsContextFactory, MemoryGraphicsContextFactory>().AsSingleton();
            container.Register<ISyncDeviceSpecifications, iOSSyncDeviceSpecifications>().AsSingleton();
            container.Register<ICloudService, iOSDropboxService>().AsSingleton();
			container.Register<IWaveFormCacheService, WaveFormCacheService>().AsSingleton();
			container.Register<IWaveFormRenderingService, WaveFormRenderingService>().AsSingleton();
            container.Register<IAppConfigProvider, iOSAppConfigProvider>().AsSingleton();
            container.Register<MobileNavigationManager, iOSNavigationManager>().AsSingleton();
            container.Register<IMobileMainView, MainViewController>().AsMultiInstance();
            container.Register<ISplashView, SplashViewController>().AsMultiInstance();
            container.Register<IMobileOptionsMenuView, MoreViewController>().AsMultiInstance();
			container.Register<IPlayerView, PlayerViewController>().AsSingleton();
            container.Register<IUpdateLibraryView, UpdateLibraryViewController>().AsMultiInstance();
            container.Register<IMobileLibraryBrowserView, MobileLibraryBrowserViewController>().AsMultiInstance();
            container.Register<IPreferencesView, PreferencesViewController>().AsMultiInstance();
            container.Register<IAudioPreferencesView, AudioPreferencesViewController>().AsMultiInstance();
            container.Register<ICloudPreferencesView, CloudPreferencesViewController>().AsMultiInstance();
            container.Register<IGeneralPreferencesView, GeneralPreferencesViewController>().AsMultiInstance();
            container.Register<ILibraryPreferencesView, LibraryPreferencesViewController>().AsMultiInstance();
            container.Register<ICloudConnectView, CloudConnectViewController>().AsMultiInstance();
            container.Register<IEqualizerPresetsView, EqualizerPresetsViewController>().AsMultiInstance();
            container.Register<IEqualizerPresetDetailsView, EqualizerPresetDetailsViewController>().AsMultiInstance();
            container.Register<IPlaylistView, PlaylistViewController>().AsMultiInstance();
            container.Register<ILoopsView, LoopsViewController>().AsMultiInstance();
            container.Register<ILoopDetailsView, LoopDetailsViewController>().AsMultiInstance();
            container.Register<IMarkersView, MarkersViewController>().AsMultiInstance();
            container.Register<IMarkerDetailsView, MarkerDetailsViewController>().AsMultiInstance();
            container.Register<ITimeShiftingView, TimeShiftingViewController>().AsMultiInstance();
            container.Register<IPitchShiftingView, PitchShiftingViewController>().AsMultiInstance();
            container.Register<IPlayerMetadataView, PlayerMetadataViewController>().AsMultiInstance();
            container.Register<ISyncView, SyncViewController>().AsMultiInstance();
            container.Register<ISyncCloudView, SyncCloudViewController>().AsMultiInstance();
            container.Register<ISyncConnectManualView, SyncConnectManualViewController>().AsMultiInstance();
            container.Register<ISyncWebBrowserView, SyncWebBrowserViewController>().AsMultiInstance();
            container.Register<ISyncMenuView, SyncMenuViewController>().AsMultiInstance();
            container.Register<ISyncDownloadView, SyncDownloadViewController>().AsMultiInstance();
            container.Register<IAboutView, AboutViewController>().AsMultiInstance();
            container.Register<ISelectPlaylistView, SelectPlaylistViewController>().AsMultiInstance();
            container.Register<IAddPlaylistView, AddPlaylistViewController>().AsMultiInstance();
            container.Register<IResumePlaybackView, ResumePlaybackViewController>().AsMultiInstance();
            container.Register<IStartResumePlaybackView, StartResumePlaybackViewController>().AsMultiInstance();
			container.Register<IQueueView, QueueViewController>().AsMultiInstance();
            container.Register<IFirstRunView, FirstRunViewController>().AsMultiInstance();
        }

        public void ShowMain(MainViewController viewController)
        {
			_mainViewController = viewController;
            //_tabBarController.View.Hidden = true;
			_window.RootViewController = _mainViewController;
        }

        public void ShowSplash(SplashViewController viewController)
        {
            InvokeOnMainThread(() => {
                _splashViewController = viewController;
                _splashViewController.View.Frame = _window.Frame;
                _splashViewController.View.AutoresizingMask = UIViewAutoresizing.All;
				_window.AddSubview(_splashViewController.View); // This cannot receive rotation changes
                _window.MakeKeyAndVisible();

                // The application is now ready to receive remote events
                UIApplication.SharedApplication.BeginReceivingRemoteControlEvents();
            });
            }

        public void HideSplash()
        {
            InvokeOnMainThread(() => {
                _window.BringSubviewToFront(_splashViewController.View);
				_mainViewController.View.Hidden = false;
                UIView.Animate(0.25f, () => {
                    _splashViewController.View.Alpha = 0.0f;
                }, () => {
                    _splashViewController.View.RemoveFromSuperview();
                });
            });
        }

        public void AddTab(MobileNavigationTabType type, string title, UIViewController viewController)
        {
            InvokeOnMainThread(() => {
                UITextAttributes attr = new UITextAttributes();
                attr.Font = UIFont.FromName("HelveticaNeue", 11);
                attr.TextColor = UIColor.LightGray;
                attr.TextShadowColor = UIColor.Clear;
                UITextAttributes attrSelected = new UITextAttributes();
                attrSelected.Font = UIFont.FromName("HelveticaNeue", 11);
                attrSelected.TextColor = UIColor.White;
                attrSelected.TextShadowColor = UIColor.Clear;

                var navCtrl = new MPfmNavigationController(type);
                navCtrl.SetTitle(title);
                navCtrl.TabBarItem.SetTitleTextAttributes(attr, UIControlState.Normal);
                navCtrl.TabBarItem.SetTitleTextAttributes(attrSelected, UIControlState.Selected);
                navCtrl.TabBarItem.Title = title;
                if(title.ToUpper() == "MORE")
                    navCtrl.TabBarItem.Image = UIImage.FromBundle("Images/Tabs/more");
                else if(title.ToUpper() == "ARTISTS")
                    navCtrl.TabBarItem.Image = UIImage.FromBundle("Images/Tabs/artists");
                else if(title.ToUpper() == "ALBUMS")
                    navCtrl.TabBarItem.Image = UIImage.FromBundle("Images/Tabs/albums");
                else if(title.ToUpper() == "SONGS")
                    navCtrl.TabBarItem.Image = UIImage.FromBundle("Images/Tabs/songs");
				else if(title.ToUpper() == "PLAYLISTS")
                    navCtrl.TabBarItem.Image = UIImage.FromBundle("Images/Tabs/sessions");

                navCtrl.PushViewController(viewController, false);
                _navigationControllers.Add(new KeyValuePair<MobileNavigationTabType, MPfmNavigationController>(type, navCtrl));

                var list = new List<UIViewController>();
				if (_mainViewController.TabBarController.ViewControllers != null)
					list = _mainViewController.TabBarController.ViewControllers.ToList();
                list.Add(navCtrl);
				_mainViewController.TabBarController.ViewControllers = list.ToArray();
            });
        }

        public void PushTabView(MobileNavigationTabType type, UIViewController viewController)
        {
			Console.WriteLine("AppDelegate - PushTabView - tabType: {0} viewController: {1}", type, viewController.GetType().FullName);
            InvokeOnMainThread(() => {
                if (viewController is PlayerViewController)
                    viewController.HidesBottomBarWhenPushed = true;

                var navCtrl = _navigationControllers.FirstOrDefault(x => x.Key == type).Value;
                navCtrl.PushViewController(viewController, true);
            });
        }

        public void PushDialogView(MobileDialogPresentationType presentationType, string viewTitle, UIViewController viewController)
        {
			Console.WriteLine("AppDelegate - PushDialogView - presentationType: {0} viewTitle: {1} viewController: {2}", presentationType, viewTitle, viewController.GetType().FullName);
            InvokeOnMainThread(() => {
                switch (presentationType)
                {
                    case MobileDialogPresentationType.Standard:
                        var navCtrl = new MPfmNavigationController(MobileNavigationTabType.More); // TODO: Remove tab type
                        navCtrl.SetTitle(viewTitle);

                        if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
                            navCtrl.NavigationBar.TintColor = UIColor.FromRGBA(0.2f, 0.2f, 0.2f, 1);                

                        navCtrl.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
                        navCtrl.ModalInPopover = true;
                        navCtrl.ModalTransitionStyle = UIModalTransitionStyle.CrossDissolve;
                        navCtrl.ViewDismissedEvent += (sender, e) => {
                            _dialogNavigationControllers.Remove(new KeyValuePair<string, MPfmNavigationController>(viewTitle, navCtrl));
                        };
                        navCtrl.PushViewController(viewController, false);                
                        _dialogNavigationControllers.Add(new KeyValuePair<string, MPfmNavigationController>(viewTitle, navCtrl));
						_mainViewController.PresentViewController(navCtrl, true, null);
                        // TODO: Remove navCtrl from list when dialog is closed.
                        break;
					default:
						_mainViewController.AddViewController(viewController, presentationType);
						break;
                }
            });
        }

        public void PushDialogSubview(MobileDialogPresentationType presentationType, string parentViewTitle, UIViewController viewController)
        {
			Console.WriteLine("AppDelegate - PushDialogSubview - presentationType: {0} parentViewTitle: {1} viewController: {2}", presentationType, parentViewTitle, viewController.GetType().FullName);
            InvokeOnMainThread(() => {
                var navCtrl = _dialogNavigationControllers.FirstOrDefault(x => x.Key == parentViewTitle).Value;
                navCtrl.PushViewController(viewController, true);
            });
        }

		public void RemoveChildFromMainViewController(UIViewController viewController)
		{
			Console.WriteLine("AppDelegate - RemoveChildFromMainViewController - viewController: {0}", viewController.GetType().FullName);
			_mainViewController.RemoveViewController(viewController);
		}

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            Console.WriteLine("AppDelegate - OpenUrl");
            var cloudService = Bootstrapper.GetContainer().Resolve<ICloudService>();
            var account = DBAccountManager.SharedManager.HandleOpenURL(url);
            if (account != null) 
            {
                Console.WriteLine("AppDelegate - OpenUrl - Dropbox linked successfully!");
                var filesystem = new DBFilesystem(account);
                DBFilesystem.SharedFilesystem = filesystem;
                cloudService.ContinueLinkApp();
                return true;
            } 
            else 
            {
                Console.WriteLine("AppDelegate - OpenUrl - Dropbox is not linked!");
                cloudService.ContinueLinkApp();
                return false;
            }
        }
    }
}
