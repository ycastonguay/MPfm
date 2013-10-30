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
using TinyMessenger;

namespace MPfm.Android.Classes.Navigation
{
    public sealed class AndroidNavigationManager : MobileNavigationManager
    {
        private readonly ITinyMessengerHub _messageHub;
        private Action<IBaseView> _onMainViewReady;
        private Action<IBaseView> _onSplashViewReady;
        private Action<IBaseView> _onAboutViewReady;
        private Action<IBaseView> _onPlayerViewReady;
        private Action<IBaseView> _onPreferencesViewReady;
        private Action<IBaseView> _onEqualizerPresetsViewReady;
        private Action<IBaseView> _onSyncViewReady;
        private Action<IBaseView> _onSyncConnectManualViewReady;
        private Action<IBaseView> _onSyncMenuViewReady;
        private Action<IBaseView> _onSyncDownloadViewReady;
        private Action<IBaseView> _onSyncWebBrowserViewReady;
        private Action<IBaseView> _onSyncCloudViewReady;
        private Action<IBaseView> _onMarkerDetailsViewReady;
        private Action<IBaseView> _onEqualizerPresetDetailsViewReady;
        private Action<IBaseView> _onPlaylistViewReady;
        private Action<IBaseView> _onResumePlaybackViewReady;
        private Action<IBaseView> _onFirstRunViewReady;
        private List<Tuple<MobileNavigationTabType, List<Tuple<MobileLibraryBrowserType, LibraryQuery>>>> _tabHistory;

        private IPlayerStatusView _lockScreenView;
        private IPlayerStatusPresenter _lockScreenPresenter;

        public MainActivity MainActivity { get; set; }
        public LaunchActivity LaunchActivity { get; set; }

        public AndroidNavigationManager(ITinyMessengerHub messageHub)
        {
            _tabHistory = new List<Tuple<MobileNavigationTabType, List<Tuple<MobileLibraryBrowserType, LibraryQuery>>>>();
            _messageHub = messageHub;
            _messageHub.Subscribe<MobileNavigationManagerCommandMessage>((m) => {
                switch (m.CommandType)
                {
                    case MobileNavigationManagerCommandMessageType.ShowPlayerView:
                        CreatePlayerView(MobileNavigationTabType.More, null);
                        break;
                    case MobileNavigationManagerCommandMessageType.ShowEqualizerPresetsView:
                        var sourceView = (IBaseView) m.Sender;
                        CreateEqualizerPresetsView(sourceView);
                        break;
                }
            });
        }

        public bool CanGoBackInMobileLibraryBrowserBackstack(MobileNavigationTabType tabType)
        {
            var tab = _tabHistory.FirstOrDefault(x => x.Item1 == tabType);
            if (tab != null)
                return tab.Item2.Count > 1;
            return false;
        }

        public void PopMobileLibraryBrowserBackstack(MobileNavigationTabType tabType)
        {
            var tab = _tabHistory.FirstOrDefault(x => x.Item1 == tabType);
            var tabItem = tab.Item2.Last();
            tab.Item2.Remove(tabItem);
            tabItem = tab.Item2.Last();

            //Console.WriteLine("ANDROID NAVMGR -- PopMobileLibraryBrowserBackstack - About to restore: tabType: {0} browserType: {1}", tabType.ToString(), tabItem.Item1.ToString());
            MobileLibraryBrowserType browserType = MobileLibraryBrowserType.Artists;
            switch (tabType)
            {
                case MobileNavigationTabType.Artists:
                    browserType = MobileLibraryBrowserType.Artists;
                    break;
                case MobileNavigationTabType.Albums:
                    browserType = MobileLibraryBrowserType.Albums;
                    break;
                case MobileNavigationTabType.Songs:
                    browserType = MobileLibraryBrowserType.Songs;
                    break;
                case MobileNavigationTabType.Playlists:
                    browserType = MobileLibraryBrowserType.Playlists;
                    break;
            }

            // Refresh query using presenter
            var presenter = GetMobileLibraryBrowserPresenter(tabType, browserType);
            presenter.PopBackstack(tabItem.Item1, tabItem.Item2);
        }

        public void ChangeMobileLibraryBrowserType(MobileNavigationTabType tabType, MobileLibraryBrowserType newBrowserType)
        {            
            MobileLibraryBrowserType browserType = MobileLibraryBrowserType.Artists;
            switch (tabType)
            {
                case MobileNavigationTabType.Artists:
                    browserType = MobileLibraryBrowserType.Artists;
                    break;
                case MobileNavigationTabType.Albums:
                    browserType = MobileLibraryBrowserType.Albums;
                    break;
                case MobileNavigationTabType.Songs:
                    browserType = MobileLibraryBrowserType.Songs;
                    break;
                case MobileNavigationTabType.Playlists:
                    browserType = MobileLibraryBrowserType.Playlists;
                    break;
            }

            _tabHistory.Clear();
            _tabHistory.Add(new Tuple<MobileNavigationTabType, List<Tuple<MobileLibraryBrowserType, LibraryQuery>>>(tabType, new List<Tuple<MobileLibraryBrowserType, LibraryQuery>>() {
               new Tuple<MobileLibraryBrowserType, LibraryQuery>(newBrowserType, new LibraryQuery())
            }));

            var presenter = GetMobileLibraryBrowserPresenter(tabType, browserType);
            if(presenter != null)
                presenter.ChangeBrowserType(newBrowserType);            
        }

        public override void NotifyMobileLibraryBrowserQueryChange(MobileNavigationTabType tabType, MobileLibraryBrowserType browserType, LibraryQuery query)
        {
            //Console.WriteLine("ANDROID NAVMGR -- NotifyMobileLibraryBrowserQueryChange tabType: {0} browserType: {1}", tabType.ToString(), browserType.ToString());
            var tab = _tabHistory.FirstOrDefault(x => x.Item1 == tabType);
            tab.Item2.Add(new Tuple<MobileLibraryBrowserType, LibraryQuery>(browserType, query));
        }

        public override void ShowSplash(ISplashView view)
        {
            //MainActivity.ShowSplash((SplashFragment) view);
        }

        public override void HideSplash()
        {
            //MainActivity.HideSplash();
        }

        public override void AddTab(MobileNavigationTabType type, string title, IBaseView view)
        {
            // Not used on Android
        }

        public override void AddTab(MobileNavigationTabType type, string title, MobileLibraryBrowserType browserType, LibraryQuery query, IBaseView view)
        {
            _tabHistory.Add(new Tuple<MobileNavigationTabType, List<Tuple<MobileLibraryBrowserType, LibraryQuery>>>(type, new List<Tuple<MobileLibraryBrowserType, LibraryQuery>>() {
               new Tuple<MobileLibraryBrowserType, LibraryQuery>(browserType, query)
            }));
            MainActivity.AddTab(type, title, (Fragment) view);
        }

        public override void PushTabView(MobileNavigationTabType type, IBaseView view)
        {
            // Not used on Android
        }

        public override void PushTabView(MobileNavigationTabType type, MobileLibraryBrowserType browserType, LibraryQuery query, IBaseView view)
        {
            // Not used on Android
        }

        public override void PushDialogView(MobileDialogPresentationType presentationType, string viewTitle, IBaseView sourceView, IBaseView view)
        {
            MainActivity.PushDialogView(viewTitle, sourceView, view);
        }

        public override void PushDialogSubview(MobileDialogPresentationType presentationType, string parentViewTitle, IBaseView view)
        {
            MainActivity.PushDialogSubview(parentViewTitle, view);
        }

        public override void PushPlayerSubview(IPlayerView playerView, IBaseView view)
        {
            //MainActivity.PushPlayerSubview(playerView, view);
            var activity = (PlayerActivity)playerView;
            activity.AddSubview(view);
        }

        public override void PushPreferencesSubview(IPreferencesView preferencesView, IBaseView view)
        {
            var activity = (PreferencesActivity)preferencesView;
            activity.AddSubview(view);
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

        protected override void CreateAboutViewInternal(Action<IBaseView> onViewReady)
        {
            _onAboutViewReady = onViewReady;
            var intent = new Intent(MainActivity, typeof(AboutActivity));
            MainActivity.StartActivity(intent);
        }

        protected override void CreatePlayerViewInternal(MobileNavigationTabType tabType, Action<IBaseView> onViewReady)
        {
            // Why is this method necessary on Android? No way to get the activity instance when starting a new activity.
            // No way to create an activity instance other than using intents. No way to pass an object (other than serializable) in intent (i.e. Action onViewReady).
            _onPlayerViewReady = onViewReady;
            //var intent = new Intent(MainActivity, typeof(PlayerActivity));
            //MainActivity.StartActivity(intent);
            var intent = new Intent(MPfmApplication.GetApplicationContext(), typeof (PlayerActivity));
            intent.AddFlags(ActivityFlags.NewTask);
            MPfmApplication.GetApplicationContext().StartActivity(intent);
        }

        protected override void CreatePreferencesViewInternal(Action<IBaseView> onViewReady)
        {
            _onPreferencesViewReady = onViewReady;
            var intent = new Intent(MainActivity, typeof (PreferencesActivity));
            MainActivity.StartActivity(intent);
        }

        protected override void CreateEqualizerPresetsViewInternal(IBaseView sourceView, Action<IBaseView> onViewReady)
        {
            _onEqualizerPresetsViewReady = onViewReady;
            var activity = GetActivityFromView(sourceView);
            StartActivity(activity, typeof(EqualizerPresetsActivity));
        }

        protected override void CreateEqualizerPresetDetailsViewInternal(IBaseView sourceView, Action<IBaseView> onViewReady)
        {
            _onEqualizerPresetDetailsViewReady = onViewReady;
            var activity = GetActivityFromView(sourceView);
            StartActivity(activity, typeof(EqualizerPresetDetailsActivity));
        }

        protected override void CreateMarkerDetailsViewInternal(IBaseView sourceView, Action<IBaseView> onViewReady)
        {
            _onMarkerDetailsViewReady = onViewReady;
            var activity = GetActivityFromView(sourceView);
            StartActivity(activity, typeof(MarkerDetailsActivity));
        }

        protected override void CreatePlaylistViewInternal(IBaseView sourceView, Action<IBaseView> onViewReady)
        {
            _onPlaylistViewReady = onViewReady;
            var activity = GetActivityFromView(sourceView);
            StartActivity(activity, typeof(PlaylistActivity));
        }

        protected override void CreateSyncViewInternal(Action<IBaseView> onViewReady)
        {
            _onSyncViewReady = onViewReady;
            var intent = new Intent(MainActivity, typeof(SyncActivity));
            MainActivity.StartActivity(intent);
        }

        protected override void CreateSyncMenuViewInternal(Action<IBaseView> onViewReady, SyncDevice device)
        {
            _onSyncMenuViewReady = onViewReady;
            var intent = new Intent(MainActivity, typeof(SyncMenuActivity));
            MainActivity.StartActivity(intent);
        }

        protected override void CreateSyncWebBrowserViewInternal(Action<IBaseView> onViewReady)
        {
            _onSyncWebBrowserViewReady = onViewReady;
            var intent = new Intent(MainActivity, typeof(SyncWebBrowserActivity));
            MainActivity.StartActivity(intent);
        }

        protected override void CreateSyncCloudViewInternal(Action<IBaseView> onViewReady)
        {
            _onSyncCloudViewReady = onViewReady;
            var intent = new Intent(MainActivity, typeof(SyncCloudActivity));
            MainActivity.StartActivity(intent);
        }

        protected override void CreateSyncDownloadViewInternal(Action<IBaseView> onViewReady, SyncDevice device, IEnumerable<Sound.AudioFiles.AudioFile> audioFiles)
        {
            _onSyncDownloadViewReady = onViewReady;
            var intent = new Intent(MainActivity, typeof(SyncDownloadActivity));
            MainActivity.StartActivity(intent);
        }

        protected override void CreateResumePlaybackViewInternal(Action<IBaseView> onViewReady)
        {
            _onResumePlaybackViewReady = onViewReady;
            var intent = new Intent(MainActivity, typeof(ResumePlaybackActivity));
            MainActivity.StartActivity(intent);
        }

        protected override void CreateFirstRunViewInternal(Action<IBaseView> onViewReady)
        {
            _onFirstRunViewReady = onViewReady;
            var intent = new Intent(MainActivity, typeof(FirstRunActivity));
            MainActivity.StartActivity(intent);
        }

        public void SetMainActivityInstance(MainActivity activity)
        {
            if (_onMainViewReady != null)
                _onMainViewReady(activity);
        }

        public void SetSplashActivityInstance(SplashActivity activity)
        {
            if (_onSplashViewReady != null)
                _onSplashViewReady(activity);
        }
       
        public void SetAboutActivityInstance(AboutActivity activity)
        {
            if (_onAboutViewReady != null)
                _onAboutViewReady(activity);
        }

        public void SetPlayerActivityInstance(PlayerActivity activity)
        {
            if (_onPlayerViewReady != null)
                _onPlayerViewReady(activity);
        }

        public void SetPreferencesActivityInstance(PreferencesActivity activity)
        {
            if (_onPreferencesViewReady != null)
                _onPreferencesViewReady(activity);
        }

        public void SetEqualizerPresetsActivityInstance(EqualizerPresetsActivity activity)
        {
            if (_onEqualizerPresetsViewReady != null)
                _onEqualizerPresetsViewReady(activity);
        }

        public void SetEqualizerPresetDetailsActivityInstance(EqualizerPresetDetailsActivity activity)
        {
            if (_onEqualizerPresetDetailsViewReady != null)
                _onEqualizerPresetDetailsViewReady(activity);
        }

        public void SetMarkerDetailsActivityInstance(MarkerDetailsActivity activity)
        {
            if (_onMarkerDetailsViewReady != null)
                _onMarkerDetailsViewReady(activity);
        }

        public void SetSyncActivityInstance(SyncActivity activity)
        {
            if (_onSyncViewReady != null)
                _onSyncViewReady(activity);
        }

        public void SetSyncMenuActivityInstance(SyncMenuActivity activity)
        {
            if (_onSyncMenuViewReady != null)
                _onSyncMenuViewReady(activity);
        }

        public void SetSyncDownloadActivityInstance(SyncDownloadActivity activity)
        {
            if (_onSyncDownloadViewReady != null)
                _onSyncDownloadViewReady(activity);
        }

        public void SetSyncWebBrowserActivityInstance(SyncWebBrowserActivity activity)
        {
            if (_onSyncWebBrowserViewReady != null)
                _onSyncWebBrowserViewReady(activity);
        }

        public void SetSyncCloudActivityInstance(SyncCloudActivity activity)
        {
            if (_onSyncCloudViewReady != null)
                _onSyncCloudViewReady(activity);
        }

        public void SetPlaylistActivityInstance(PlaylistActivity activity)
        {
            if (_onPlaylistViewReady != null)
                _onPlaylistViewReady(activity);
        }

        public void SetResumePlaybackActivityInstance(ResumePlaybackActivity activity)
        {
            if (_onResumePlaybackViewReady != null)
                _onResumePlaybackViewReady(activity);
        }

        public void SetFirstRunActivityInstance(FirstRunActivity activity)
        {
            if (_onFirstRunViewReady != null)
                _onFirstRunViewReady(activity);
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
