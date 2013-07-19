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

using MPfm.MVP.Messages;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using TinyMessenger;
using MPfm.iOS.Classes.Controllers;
using MPfm.iOS.Classes.Controls;
using MPfm.iOS.Classes.Delegates;
using MPfm.Library.Objects;

namespace MPfm.iOS.Classes.Navigation
{
	public sealed class iOSNavigationManager : MobileNavigationManager
	{
        ITinyMessengerHub _messageHub;

        public AppDelegate AppDelegate { get; set; }

        public iOSNavigationManager(ITinyMessengerHub messageHub)
        {
            _messageHub = messageHub;
            _messageHub.Subscribe<MobileNavigationManagerCommandMessage>((m) => {
                using(var pool = new NSAutoreleasePool())
                {
                    pool.InvokeOnMainThread(() => {
                        switch(m.CommandType)
                        {
                            case MobileNavigationManagerCommandMessageType.ShowPlayerView:
                                var navCtrl = (MPfmNavigationController)m.Sender;
                                if(PlayerView != null)
                                    PushTabView(navCtrl.TabType, PlayerView);
                                break;
                            case MobileNavigationManagerCommandMessageType.ShowEqualizerPresetsView:
                                if(EqualizerPresetsView == null)
                                    CreateEqualizerPresetsView(null);
                                else
                                    PushDialogView("Equalizer Presets", null, EqualizerPresetsView);
                                break;
                        }
                    });
                }
            });
        }
		
		public override void ShowSplash(ISplashView view)
		{
			AppDelegate.ShowSplash((SplashViewController)view);
		}
		
		public override void HideSplash()
		{
			AppDelegate.HideSplash();
		}
		
		public override void AddTab(MobileNavigationTabType type, string title, IBaseView view)
		{
            AppDelegate.AddTab(type, title, (UIViewController)view);
		}

        public override void AddTab(MobileNavigationTabType type, string title, MobileLibraryBrowserType browserType, LibraryQuery query, IBaseView view)
        {
            AppDelegate.AddTab(type, title, (UIViewController)view);
        }
		
		public override void PushTabView(MobileNavigationTabType type, IBaseView view)
		{
			AppDelegate.PushTabView(type, (UIViewController)view);
		}

        public override void PushTabView(MobileNavigationTabType type, MobileLibraryBrowserType browserType, LibraryQuery query, IBaseView view)
        {
            AppDelegate.PushTabView(type, (UIViewController)view);
        }

        public override void PushDialogView(string viewTitle, IBaseView sourceView, IBaseView view)
        {
            AppDelegate.PushDialogView(viewTitle, (UIViewController)view);
        }

        public override void PushDialogSubview(string parentViewTitle, IBaseView view)
        {
            AppDelegate.PushDialogSubview(parentViewTitle, (UIViewController)view);
        }

        public override void PushPlayerSubview(IPlayerView playerView, IBaseView view)
        {
            var playerViewController = (PlayerViewController)playerView;
            playerViewController.AddScrollView((UIViewController)view);
        }

        public override void PushPreferencesSubview(IPreferencesView preferencesView, IBaseView view)
        {
            // Not necessary on iOS.
        }
	}
}
