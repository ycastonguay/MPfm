// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using Sessions.MVP.Messages;
using Sessions.MVP.Navigation;
using Sessions.MVP.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using TinyMessenger;
using Sessions.iOS.Classes.Controllers;
using Sessions.iOS.Classes.Controls;
using Sessions.iOS.Classes.Delegates;
using Sessions.Library.Objects;
using Sessions.MVP.Bootstrap;
using System;

namespace Sessions.iOS.Classes.Navigation
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
                                var navCtrl = (SessionsNavigationController)m.Sender;
                                CreatePlayerView(navCtrl.TabType);
                                break;
                            case MobileNavigationManagerCommandMessageType.ShowEqualizerPresetsView:
                                CreateEqualizerPresetsView(null);
                                break;
                            case MobileNavigationManagerCommandMessageType.ShowPlaylistView:
                                CreatePlaylistView(null);
                                break;
                        }
                    });
                }
            });
        }

        public override void PushTabView(MobileNavigationTabType type, IBaseView view)
        {
            AppDelegate.PushTabView(type, (UIViewController)view);
        }
		
        public override void PushDialogView(MobileDialogPresentationType presentationType, string viewTitle, IBaseView sourceView, IBaseView view)
        {
            AppDelegate.PushDialogView(presentationType, viewTitle, (UIViewController)view);
        }

        public override void PushDialogSubview(MobileDialogPresentationType presentationType, string parentViewTitle, IBaseView view)
        {
            AppDelegate.PushDialogSubview(presentationType, parentViewTitle, (UIViewController)view);
        }

        public override void CreateSplashView()
        {
            var view = Bootstrapper.GetContainer().Resolve<ISplashView>();
            AppDelegate.ShowSplash((SplashViewController)view);
        }

        public override void CreateFirstRunView()
        {
            var view = Bootstrapper.GetContainer().Resolve<IFirstRunView>();
            AppDelegate.ShowFirstRun((FirstRunViewController)view);
        }

        public override void CreateMobileMainView()
        {
            var view = Bootstrapper.GetContainer().Resolve<IMobileMainView>();
            // The view tries to bind to the navmgr BEFORE it hits this line.
            AppDelegate.ShowMain((MainViewController)view);
        }

        public override void CreatePlayerView(MobileNavigationTabType tabType)
        {
            var view = Bootstrapper.GetContainer().Resolve<IPlayerView>();
            AppDelegate.PushTabView(tabType, (PlayerViewController)view);
        }

        public override void CreateEqualizerPresetDetailsView(IBaseView sourceView, Guid presetId)
        {
            if (EqualizerPresetDetailsPresenter != null)
            {
                // Since views are recycled on iOS, use the current instance and change the preset
                EqualizerPresetDetailsPresenter.ChangePreset(presetId);
            }

            base.CreateEqualizerPresetDetailsView(sourceView, presetId);
        }

	}
}
