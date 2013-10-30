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
using Android.App;
using Android.Content;
using MPfm.Android.Classes.Fragments;
using MPfm.Library.Objects;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Messages;
using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;
using TinyMessenger;

namespace MPfm.Android.Classes.Navigation
{
    public sealed class AndroidNavigationManager : MobileNavigationManager
    {
        private readonly ITinyMessengerHub _messageHub;

        private IPlayerStatusView _lockScreenView;
        private IPlayerStatusPresenter _lockScreenPresenter;

        public MainActivity MainActivity { get; set; }
        public LaunchActivity LaunchActivity { get; set; }

        public AndroidNavigationManager(ITinyMessengerHub messageHub)
        {
            _messageHub = messageHub;
            _messageHub.Subscribe<MobileNavigationManagerCommandMessage>((m) => {
                switch (m.CommandType)
                {
                    case MobileNavigationManagerCommandMessageType.ShowPlayerView:
                        CreatePlayerView(MobileNavigationTabType.More);
                        break;
                    case MobileNavigationManagerCommandMessageType.ShowEqualizerPresetsView:
                        var sourceView = (IBaseView) m.Sender;
                        CreateEqualizerPresetsView(sourceView);
                        break;
                }
            });
        }


        public override void NotifyMobileLibraryBrowserQueryChange(MobileNavigationTabType tabType, MobileLibraryBrowserType browserType, LibraryQuery query)
        {
            //Console.WriteLine("ANDROID NAVMGR -- NotifyMobileLibraryBrowserQueryChange tabType: {0} browserType: {1}", tabType.ToString(), browserType.ToString());
            //var tab = _tabHistory.FirstOrDefault(x => x.Item1 == tabType);
            //tab.Item2.Add(new Tuple<MobileLibraryBrowserType, LibraryQuery>(browserType, query));
        }

        public override void PushDialogView(MobileDialogPresentationType presentationType, string viewTitle, IBaseView sourceView, IBaseView view)
        {
            MainActivity.PushDialogView(viewTitle, sourceView, view);
        }

        public override void PushDialogSubview(MobileDialogPresentationType presentationType, string parentViewTitle, IBaseView view)
        {
            MainActivity.PushDialogSubview(parentViewTitle, view);
        }

        private Activity GetActivityFromView(IBaseView view)
        {
            if (view is Activity)
                return (Activity)view;
            else if (view is Fragment)
                return ((Fragment) view).Activity;
            else
                return null;
        }

        private void StartActivity(Activity sourceActivity, Type activityType)
        {
            var intent = new Intent(sourceActivity, activityType);
            intent.PutExtra("sourceActivity", sourceActivity.GetType().FullName);
            sourceActivity.StartActivity(intent);
        }

        public override void CreateMobileMainView()
        {
            var intent = new Intent(MPfmApplication.GetApplicationContext(), typeof(MainActivity));
            intent.AddFlags(ActivityFlags.NewTask);
            MPfmApplication.GetApplicationContext().StartActivity(intent);
        }

        public override void CreateSplashView()
        {
            var intent = new Intent(MPfmApplication.GetApplicationContext(), typeof (SplashActivity));
            intent.AddFlags(ActivityFlags.NewTask);
            MPfmApplication.GetApplicationContext().StartActivity(intent);
        }

        public override void CreateAboutView()
        {
            var intent = new Intent(MainActivity, typeof(AboutActivity));
            MainActivity.StartActivity(intent);
        }

        public override void CreatePlayerView(MobileNavigationTabType tabType)
        {
            var intent = new Intent(MPfmApplication.GetApplicationContext(), typeof (PlayerActivity));
            intent.AddFlags(ActivityFlags.NewTask);
            MPfmApplication.GetApplicationContext().StartActivity(intent);
        }

        public override void CreatePreferencesView()
        {
            var intent = new Intent(MainActivity, typeof (PreferencesActivity));
            MainActivity.StartActivity(intent);
        }

        public override void CreateEqualizerPresetsView(IBaseView sourceView)
        {
            var activity = GetActivityFromView(sourceView);
            StartActivity(activity, typeof(EqualizerPresetsActivity));
        }

        public override void CreateEqualizerPresetDetailsView(IBaseView sourceView, Guid presetId)
        {
            var activity = GetActivityFromView(sourceView);
            var intent = new Intent(activity, typeof(EqualizerPresetDetailsActivity));
            intent.PutExtra("sourceActivity", activity.GetType().FullName);
            intent.PutExtra("presetId", presetId.ToString());
            activity.StartActivity(intent);
        }

        public override void CreateMarkerDetailsView(IBaseView sourceView, Guid markerId)
        {
            var activity = GetActivityFromView(sourceView);
            var intent = new Intent(activity, typeof(MarkerDetailsActivity));
            intent.PutExtra("sourceActivity", activity.GetType().FullName);
            intent.PutExtra("markerId", markerId.ToString());
            activity.StartActivity(intent);
        }

        public override void CreatePlaylistView(IBaseView sourceView)
        {
            var activity = GetActivityFromView(sourceView);
            StartActivity(activity, typeof(PlaylistActivity));
        }

        public override void CreateSyncView()
        {
            var intent = new Intent(MainActivity, typeof(SyncActivity));
            MainActivity.StartActivity(intent);
        }

        public override void CreateSyncMenuView(SyncDevice device)
        {
            var intent = new Intent(MainActivity, typeof(SyncMenuActivity));
            MainActivity.StartActivity(intent);
        }

        public override void CreateSyncWebBrowserView()
        {
            var intent = new Intent(MainActivity, typeof(SyncWebBrowserActivity));
            MainActivity.StartActivity(intent);
        }

        public override void CreateSyncCloudView()
        {
            var intent = new Intent(MainActivity, typeof(SyncCloudActivity));
            MainActivity.StartActivity(intent);
        }

        public override void CreateSyncDownloadView(SyncDevice device, IEnumerable<AudioFile> audioFiles)
        {
            var intent = new Intent(MainActivity, typeof(SyncDownloadActivity));
            // Pass params to intent
            MainActivity.StartActivity(intent);
        }

        public override void CreateResumePlaybackView()
        {
            var intent = new Intent(MainActivity, typeof(ResumePlaybackActivity));
            MainActivity.StartActivity(intent);
        }

        public override void CreateFirstRunView()
        {
            var intent = new Intent(MainActivity, typeof(FirstRunActivity));
            MainActivity.StartActivity(intent);
        }

        public void SetLockScreenActivityInstance(LockScreenActivity activity)
        {
            _lockScreenView = activity;
            _lockScreenPresenter = Bootstrapper.GetContainer().Resolve<IPlayerStatusPresenter>();
            _lockScreenPresenter.BindView(_lockScreenView);
            _lockScreenView.OnViewDestroy = (theView) =>
            {
                _lockScreenPresenter.ViewDestroyed();
                _lockScreenPresenter = null;
                _lockScreenView = null;
            };
        }
        
    }
}
