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

namespace MPfm.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{		
		UIWindow window;
        UITabBarController tabBarController;
        MPfmNavigationController artistNavController;
        MPfmNavigationController albumNavController;
        MPfmNavigationController playlistNavController;
        MPfmNavigationController moreNavController;

        SplashViewController splashViewController;
		PlayerViewController playerViewController;
        ListViewController artistViewController;
        ListViewController albumViewController;
        ListViewController playlistViewController;
        ListViewController moreViewController;

		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
            // Create window 
			window = new UIWindow(UIScreen.MainScreen.Bounds);

            // Create tab bar controller, but hide it while the splash screen is visible
            tabBarController = new UITabBarController();
            tabBarController.View.Hidden = true;
            window.RootViewController = tabBarController;
            
            // Create splash screen and display it while loading 
            splashViewController = new SplashViewController();
            splashViewController.View.Frame = window.Frame;
            splashViewController.View.AutoresizingMask = UIViewAutoresizing.All;
            window.AddSubview(splashViewController.View);
            window.MakeKeyAndVisible();
                       
            playerViewController = new PlayerViewController();

            // Get list of fonts
            //List<String> fontFamilies = new List<String>(UIFont.FamilyNames);
            //fontFamilies.Sort();
            //List<string> fontNames = UIFont.FontNamesForFamilyName("Ostrich Sans Rounded").ToList();

            // Prepare navigation controllers
            //artistNavController = new MPfmNavigationController("OstrichSansRounded-Medium", 26);
            artistNavController = new MPfmNavigationController("OstrichSans-Black", 26);
            artistNavController.NavigationBar.BackgroundColor = UIColor.Clear;
            artistNavController.NavigationBar.TintColor = UIColor.Clear;
            albumNavController = new MPfmNavigationController("OstrichSans-Black", 26);
            albumNavController.NavigationBar.BackgroundColor = UIColor.Clear;
            albumNavController.NavigationBar.TintColor = UIColor.Clear;
            playlistNavController = new MPfmNavigationController("OstrichSans-Black", 26);
            playlistNavController.NavigationBar.BackgroundColor = UIColor.Clear;
            playlistNavController.NavigationBar.TintColor = UIColor.Clear;
            moreNavController = new MPfmNavigationController("OstrichSans-Black", 26);
            moreNavController.NavigationBar.BackgroundColor = UIColor.Clear;
            moreNavController.NavigationBar.TintColor = UIColor.Clear;

            // Prepare Artists view controller
            artistViewController = new ListViewController("Artists", new List<GenericListItem>() { 
                new GenericListItem() {
                    Title = "Amon Tobin",
                    Image = UIImage.FromBundle("/Images/icon114.png")
                },
                new GenericListItem() {
                    Title = "Aphex Twin",
                    Image = UIImage.FromBundle("/Images/icon114.png")
                },
                new GenericListItem() {
                    Title = "Blake James",
                    Image = UIImage.FromBundle("/Images/icon114.png")
                },
                new GenericListItem() {
                    Title = "Bob Marley & The Wailers",
                    Image = UIImage.FromBundle("/Images/icon114.png")
                },
                new GenericListItem() {
                    Title = "Broken Social Scene",
                    Image = UIImage.FromBundle("/Images/icon114.png")
                },
                new GenericListItem() {
                    Title = "Can",
                    Image = UIImage.FromBundle("/Images/icon114.png")
                },
                new GenericListItem() {
                    Title = "Dylan Bob",
                    Image = UIImage.FromBundle("/Images/icon114.png")
                },
                new GenericListItem() {
                    Title = "My Bloody Valentine",
                    Image = UIImage.FromBundle("/Images/icon114.png")
                },
                new GenericListItem() {
                    Title = "Public Image Ltd.",
                    Image = UIImage.FromBundle("/Images/icon114.png")
                },
                new GenericListItem() {
                    Title = "The Future Sound of London",
                    Image = UIImage.FromBundle("/Images/icon114.png")
                } 
            }, (item) => { 
                playerViewController.HidesBottomBarWhenPushed = true;
                artistNavController.PushViewController(playerViewController, true);
            });
            artistNavController.PushViewController(artistViewController, false);

            // Prepare Albums tab
            albumViewController = new ListViewController("Albums", new List<GenericListItem>() { 
                new GenericListItem() {
                    Title = "Album 1",
                    Subtitle = "Artist Name",
                    Image = UIImage.FromBundle("/Images/icon114.png")
                },
                new GenericListItem() {
                    Title = "Album 2",
                    Subtitle = "Artist Name",
                    Image = UIImage.FromBundle("/Images/icon114.png")
                } 
            }, null);
            albumNavController.PushViewController(albumViewController, false);

            // Prepare Playlist tab
            playlistViewController = new ListViewController("Playlists", new List<GenericListItem>() { 
                new GenericListItem() {
                    Title = "Playlist 1",
                    Image = UIImage.FromBundle("/Images/icon114.png")
                },
                new GenericListItem() {
                    Title = "Playlist 2",
                    Image = UIImage.FromBundle("/Images/icon114.png")
                } 
            }, null);
            playlistNavController.PushViewController(playlistViewController, false);

            // Prepare More tab
            moreViewController = new ListViewController("More", new List<GenericListItem>() { 
                new GenericListItem() {
                    Title = "Settings",
                    Image = UIImage.FromBundle("/Images/icon114.png")
                },
                new GenericListItem() {
                    Title = "About MP4M",
                    Image = UIImage.FromBundle("/Images/icon114.png")
                } 
            }, null);
            moreNavController.PushViewController(moreViewController, false);

            // Create text attributes for tab bar items
            UITextAttributes attr = new UITextAttributes();
            //attr.Font = UIFont.FromName("Junction", 11);
            attr.Font = UIFont.FromName("OstrichSans-Black", 13);
            attr.TextColor = UIColor.White;
            //attr.TextShadowColor = UIColor.DarkGray;
            //attr.TextShadowOffset = new UIOffset(1, 1);
            artistNavController.TabBarItem.SetTitleTextAttributes(attr, UIControlState.Normal);
            albumNavController.TabBarItem.SetTitleTextAttributes(attr, UIControlState.Normal);
            playlistNavController.TabBarItem.SetTitleTextAttributes(attr, UIControlState.Normal);
            moreNavController.TabBarItem.SetTitleTextAttributes(attr, UIControlState.Normal);

            // Add view controllers to tab bar
            tabBarController.ViewControllers = new UIViewController[] {
                artistNavController,
                albumNavController,
                playlistNavController,
                moreNavController
            };

            splashViewController.RemoveFromParentViewController();
            tabBarController.View.Hidden = false;

            // Show status bar again
            //UIApplication.SharedApplication.SetStatusBarHidden(false, true);
			
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
	}
}

