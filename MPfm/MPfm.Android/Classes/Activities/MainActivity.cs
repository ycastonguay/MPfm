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
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Support.V4.View;
using Android.Views;
using Android.OS;
using Android.Views.Animations;
using Android.Widget;
using Java.Lang;
using MPfm.Android.Classes.Adapters;
using MPfm.Android.Classes.Fragments;
using MPfm.Android.Classes.Helpers;
using MPfm.Android.Classes.Navigation;
using MPfm.Android.Classes.Objects;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Messages;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;
using TinyMessenger;

namespace MPfm.Android
{
    [Activity(MainLauncher = true, ScreenOrientation = ScreenOrientation.Sensor, Theme = "@style/MyAppTheme", ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class MainActivity : BaseActivity, IMobileOptionsMenuView
    {
        private ITinyMessengerHub _messengerHub;
        private AndroidNavigationManager _navigationManager;
        private SplashFragment _splashFragment;
        private LinearLayout _miniPlayer;
        private List<KeyValuePair<MobileOptionsMenuType, string>> _options;
        private ViewPager _viewPager;
        private MainTabStatePagerAdapter _tabPagerAdapter;
        private TextView _lblArtistName;
        private TextView _lblAlbumTitle;
        private TextView _lblSongTitle;
        private ImageView _imageAlbum;

        public BitmapCache BitmapCache { get; private set; }

        protected override void OnCreate(Bundle bundle)
        {
            Console.WriteLine("%%%%%%%%%%%%%%%%%%>>> MainActivity - OnCreate");
            base.OnCreate(bundle);

            RequestWindowFeature(WindowFeatures.ActionBar);
            SetContentView(Resource.Layout.Main);

            // Setup view pager
            _viewPager = FindViewById<ViewPager>(Resource.Id.main_pager);
            _viewPager.OffscreenPageLimit = 4;
            _tabPagerAdapter = new MainTabStatePagerAdapter(FragmentManager, _viewPager);
            _viewPager.Adapter = _tabPagerAdapter;
            _viewPager.SetOnPageChangeListener(_tabPagerAdapter);

            // Setup mini player
            _miniPlayer = FindViewById<LinearLayout>(Resource.Id.main_miniplayer);
            _lblArtistName = FindViewById<TextView>(Resource.Id.main_miniplayer_lblArtistName);
            _lblAlbumTitle = FindViewById<TextView>(Resource.Id.main_miniplayer_lblAlbumTitle);
            _lblSongTitle = FindViewById<TextView>(Resource.Id.main_miniplayer_lblSongTitle);
            _imageAlbum = FindViewById<ImageView>(Resource.Id.main_miniplayer_imageAlbum);
            _miniPlayer.Visibility = ViewStates.Gone;
            _miniPlayer.Click += (sender, args) => {
                Console.WriteLine("MainActivity - Mini player click - Showing player view...");
                _messengerHub.PublishAsync<MobileNavigationManagerCommandMessage>(new MobileNavigationManagerCommandMessage(this, MobileNavigationManagerCommandMessageType.ShowPlayerView));
            };

            // Create bitmap cache
            int maxMemory = (int)(Runtime.GetRuntime().MaxMemory() / 1024);
            int cacheSize = maxMemory / 8;
            BitmapCache = new BitmapCache(this, cacheSize, 400, 400);

            // Listen to player changes to show/hide the mini player
            _messengerHub = Bootstrapper.GetContainer().Resolve<ITinyMessengerHub>();
            _messengerHub.Subscribe<PlayerPlaylistIndexChangedMessage>((message) => {
                Console.WriteLine("MainActivity - PlayerPlaylistIndexChangedMessage");
                RunOnUiThread(() => {
                    // Make sure the UI is available
                    if (_lblArtistName != null)
                    {
                        _lblArtistName.Text = message.Data.AudioFileStarted.ArtistName;
                        _lblAlbumTitle.Text = message.Data.AudioFileStarted.AlbumTitle;
                        _lblSongTitle.Text = message.Data.AudioFileStarted.Title;

                        Task.Factory.StartNew(() =>
                        {
                            byte[] bytesImage = AudioFile.ExtractImageByteArrayForAudioFile(message.Data.AudioFileStarted.FilePath);
                            BitmapCache.LoadBitmapFromByteArray(bytesImage, message.Data.AudioFileStarted.FilePath, _imageAlbum);
                        });
                    }
                });                
            });
            _messengerHub.Subscribe<PlayerStatusMessage>((message) => {
                Console.WriteLine("MainActivity - PlayerStatusMessage - Status=" + message.Status.ToString());
                RunOnUiThread(() => {
                    if (message.Status == PlayerStatusType.Stopped || message.Status == PlayerStatusType.Initialized)
                    {
                        Animation anim = AnimationUtils.LoadAnimation(this, Resource.Animation.slide_out_right);
                        anim.AnimationEnd += (sender, args) => {
                            _miniPlayer.Visibility = ViewStates.Gone;
                        };
                        _miniPlayer.StartAnimation(anim);
                    }
                    else
                    {
                        _miniPlayer.Visibility = ViewStates.Visible;
                        Animation anim = AnimationUtils.LoadAnimation(this, Resource.Animation.slide_in_left);
                        _miniPlayer.StartAnimation(anim);  
                    }
                });
            });

            if (bundle == null || !bundle.GetBoolean("applicationStarted"))
            {
                Console.WriteLine("MainActivity - OnCreate - Starting navigation manager...");
                _navigationManager = (AndroidNavigationManager) Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
                _navigationManager.MainActivity = this; // TODO: Is this OK? Shouldn't the reference be cleared when MainActivity is destroyed? Can lead to memory leaks.
                _navigationManager.BindOptionsMenuView(this);
                _navigationManager.Start();
            }
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            Console.WriteLine("%%%%%%%%%%%%%%%%%%>>> MainActivity - OnSaveInstanceState - Saving state...");
            outState.PutBoolean("applicationStarted", true);
        }

        public void AddTab(MobileNavigationTabType type, string title, Fragment fragment)
        {
            //Console.WriteLine("MainActivity - Adding tab {0}", title);
            _tabPagerAdapter.SetFragment(type, fragment);
            _tabPagerAdapter.NotifyDataSetChanged();
        }

        public void PushTabView(MobileNavigationTabType type, Fragment fragment)
        {
            //Console.WriteLine("MainActivity - PushTabView type: {0} fragment: {1} fragmentCount: {2}", type.ToString(), fragment.GetType().FullName, FragmentManager.BackStackEntryCount);
            _tabPagerAdapter.SetFragment(type, fragment);
        }

        public void PushDialogView(string viewTitle, IBaseView sourceView, IBaseView view)
        {
            //Console.WriteLine("MainActivity - PushDialogView view: {0} fragmentCount: {1}", view.GetType().FullName, FragmentManager.BackStackEntryCount);
            var sourceFragment = (Fragment) sourceView;
            var dialogFragment = (DialogFragment)view;
            dialogFragment.Show(sourceFragment.Activity.FragmentManager, viewTitle);
        }

        public void PushDialogSubview(string parentViewTitle, IBaseView view)
        {
            //Console.WriteLine("MainActivity - PushDialogSubview parentViewTitle: {0} view: {1}", parentViewTitle, view.GetType().FullName);
        }

        public void PushPlayerSubview(IPlayerView playerView, IBaseView view)
        {
            //Console.WriteLine("MainActivity - PushPlayerSubview - view: {0}", view.GetType().FullName);
            var activity = (PlayerActivity)playerView;
            activity.AddSubview(view);
        }

        public void PushPreferencesSubview(IPreferencesView preferencesView, IBaseView view)
        {
            //Console.WriteLine("MainActivity - PushPreferencesSubview - view: {0}", view.GetType().FullName);
            var activity = (PreferencesActivity)preferencesView;
            activity.AddSubview(view);
        }

        protected override void OnStart()
        {
            Console.WriteLine("%%%%%%%%%%%%>>> MainActivity - OnStart");
            base.OnStart();
        }

        protected override void OnRestart()
        {
            Console.WriteLine("%%%%%%%%%%%%>>> MainActivity - OnRestart");
            base.OnRestart();
        }

        protected override void OnPause()
        {
            Console.WriteLine("%%%%%%%%%%%%>>> MainActivity - OnPause");
            base.OnPause();
        }

        protected override void OnResume()
        {
            Console.WriteLine("%%%%%%%%%%%%>>> MainActivity - OnResume");
            base.OnResume();
        }

        protected override void OnStop()
        {
            Console.WriteLine("%%%%%%%%%%%%>>> MainActivity - OnStop");
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            Console.WriteLine("%%%%%%%%%%%%>>> MainActivity - OnDestroy");
            base.OnDestroy();
        }

        public override void OnBackPressed()
        {
            // Check if the history has another tab
            if (_navigationManager.CanRemoveMobileLibraryBrowserFragmentFromBackstack(_tabPagerAdapter.GetCurrentTab()))
            {
                Console.WriteLine("MainActivity - OnBackPressed - CanRemoveFragment");
                _navigationManager.RecreateMobileLibraryBrowserFragment(_tabPagerAdapter.GetCurrentTab());
            }
            else
            {
                Console.WriteLine("MainActivity - OnBackPressed - CannotRemoveFragment");
                base.OnBackPressed();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.main_menu, menu);
            //Console.WriteLine("MainActivity - OnCreateOptionsMenu");

            foreach (var option in _options)
            {
                var menuItem = menu.Add(new Java.Lang.String(option.Value));
                switch (option.Key)
                {
                    case MobileOptionsMenuType.About:
                        menuItem.SetIcon(Resource.Drawable.icon_info);
                        break;
                    case MobileOptionsMenuType.EqualizerPresets:
                        menuItem.SetShowAsAction(ShowAsAction.IfRoom);
                        menuItem.SetIcon(Resource.Drawable.icon_equalizer);
                        break;
                    case MobileOptionsMenuType.Preferences:
                        menuItem.SetShowAsAction(ShowAsAction.IfRoom);
                        menuItem.SetIcon(Resource.Drawable.icon_settings);
                        break;
                    case MobileOptionsMenuType.SyncLibrary:
                        menuItem.SetShowAsAction(ShowAsAction.IfRoom);
                        menuItem.SetIcon(Resource.Drawable.icon_mobile);
                        break;
                    case MobileOptionsMenuType.SyncLibraryCloud:
                        menuItem.SetIcon(Resource.Drawable.icon_cloud);
                        break;
                    case MobileOptionsMenuType.SyncLibraryFileSharing:
                        menuItem.SetIcon(Resource.Drawable.icon_share);
                        break;
                    case MobileOptionsMenuType.SyncLibraryWebBrowser:
                        menuItem.SetIcon(Resource.Drawable.icon_earth);
                        break;
                }
            }

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem menuItem)
        {
            var option = _options.FirstOrDefault(x => x.Value == menuItem.TitleFormatted.ToString());
            OnItemClick(option.Key);
            return base.OnOptionsItemSelected(menuItem);
        }

        public void ShowSplash(SplashFragment fragment)
        {
            //Console.WriteLine("MainActivity - ShowSplash");
            _splashFragment = fragment;
            _splashFragment.Show(FragmentManager, "Splash");
        }

        public void HideSplash()
        {
            //Console.WriteLine("MainActivity - HideSplash");
            _splashFragment.Dialog.Dismiss();
        }

        #region IMobileOptionsMenuView implementation

        public Action<MobileOptionsMenuType> OnItemClick { get; set; }
        public void RefreshMenu(List<KeyValuePair<MobileOptionsMenuType, string>> options)
        {
            Console.WriteLine("MainActivity - RefreshMenu");
            _options = options;
        }

        #endregion
    }
}
