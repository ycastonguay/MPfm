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
using Android.Graphics;
using Android.Views;
using Android.OS;
using Android.Views.Animations;
using Android.Widget;
using Java.Lang;
using MPfm.Android.Classes.Cache;
using MPfm.Android.Classes.Fragments;
using MPfm.Android.Classes.Navigation;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Messages;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;
using TinyMessenger;
using org.sessionsapp.android;
using DialogFragment = Android.App.DialogFragment;
using Fragment = Android.App.Fragment;
using FragmentTransaction = Android.App.FragmentTransaction;

namespace MPfm.Android
{
    [Activity(MainLauncher = true, ScreenOrientation = ScreenOrientation.Sensor, Theme = "@style/MyAppTheme", ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class MainActivity : BaseActivity, IMobileOptionsMenuView, View.IOnTouchListener, ActionBar.IOnNavigationListener
    {
        private ITinyMessengerHub _messengerHub;
        private AndroidNavigationManager _navigationManager;
        private SplashFragment _splashFragment;
        private ViewFlipper _viewFlipper;
        private LinearLayout _miniPlayer;
        private LinearLayout _miniPlaylist;
        private List<KeyValuePair<MobileOptionsMenuType, string>> _options;
        private TextView _lblArtistName;
        private TextView _lblAlbumTitle;
        private TextView _lblSongTitle;
        private SquareImageView _imageAlbum;
        private ImageButton _btnPrevious;
        private ImageButton _btnPlayPause;
        private ImageButton _btnNext;
        private ImageButton _btnPlaylist;
        private ImageButton _btnLeft;
        private ImageButton _btnRight;
        private bool _isPlaying;
        private ArrayAdapter _spinnerAdapter;
        private Fragment _fragment;

        public BitmapCache BitmapCache { get; private set; }

        protected override void OnCreate(Bundle bundle)
        {
            Console.WriteLine("MainActivity - OnCreate");
            base.OnCreate(bundle);

            RequestWindowFeature(WindowFeatures.ActionBar);
            SetContentView(Resource.Layout.Main);
            ActionBar.NavigationMode = ActionBarNavigationMode.List;
            ActionBar.Title = string.Empty;
            _spinnerAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.action_list, Resource.Layout.spinner_dropdown_item);
            ActionBar.SetListNavigationCallbacks(_spinnerAdapter, this);
            ActionBar.SetSelectedNavigationItem(1);

            _viewFlipper = FindViewById<ViewFlipper>(Resource.Id.main_viewflipper);
            _miniPlayer = FindViewById<LinearLayout>(Resource.Id.main_miniplayer);
            _miniPlaylist = FindViewById<LinearLayout>(Resource.Id.main_miniplaylist);
            _lblArtistName = FindViewById<TextView>(Resource.Id.main_miniplayer_lblArtistName);
            _lblAlbumTitle = FindViewById<TextView>(Resource.Id.main_miniplayer_lblAlbumTitle);
            _lblSongTitle = FindViewById<TextView>(Resource.Id.main_miniplayer_lblSongTitle);
            _btnPrevious = FindViewById<ImageButton>(Resource.Id.main_miniplayer_btnPrevious);
            _btnPlayPause = FindViewById<ImageButton>(Resource.Id.main_miniplayer_btnPlayPause);
            _btnNext = FindViewById<ImageButton>(Resource.Id.main_miniplayer_btnNext);
            _btnPlaylist = FindViewById<ImageButton>(Resource.Id.main_miniplaylist_btnPlaylist);          
            _btnLeft = FindViewById<ImageButton>(Resource.Id.main_miniplaylist_btnLeft);
            _btnRight = FindViewById<ImageButton>(Resource.Id.main_miniplayer_btnRight);
            _imageAlbum = FindViewById<SquareImageView>(Resource.Id.main_miniplayer_imageAlbum);
            _miniPlayer.Click += (sender, args) => {
                //Console.WriteLine("MainActivity - Mini player click - Showing player view...");
                _messengerHub.PublishAsync<MobileNavigationManagerCommandMessage>(new MobileNavigationManagerCommandMessage(this, MobileNavigationManagerCommandMessageType.ShowPlayerView));
            };
            _btnLeft.SetOnTouchListener(this);
            _btnRight.SetOnTouchListener(this);
            _btnPrevious.SetOnTouchListener(this);
            _btnPlayPause.SetOnTouchListener(this);
            _btnNext.SetOnTouchListener(this);
            _btnPlaylist.SetOnTouchListener(this);
            _btnPrevious.Click += BtnPreviousOnClick;
            _btnPlayPause.Click += BtnPlayPauseOnClick;
            _btnNext.Click += BtnNextOnClick;
            _btnPlaylist.Click += BtnPlaylistOnClick;
            _btnLeft.Click += BtnLeftOnClick;
            _btnRight.Click += BtnRightOnClick;

            // Set initial view flipper item
            int realIndex = _viewFlipper.IndexOfChild(_miniPlayer);
            _viewFlipper.DisplayedChild = realIndex;

            // Create bitmap cache
            Point size = new Point();
            WindowManager.DefaultDisplay.GetSize(size);
            int maxMemory = (int)(Runtime.GetRuntime().MaxMemory() / 1024);
            int cacheSize = maxMemory / 16;
            BitmapCache = new BitmapCache(this, cacheSize, size.X / 6, size.X / 6);

            // Listen to player changes to show/hide the mini player
            _messengerHub = Bootstrapper.GetContainer().Resolve<ITinyMessengerHub>();
            _messengerHub.Subscribe<PlayerPlaylistIndexChangedMessage>((message) => RunOnUiThread(() => {
                // Make sure the UI is available
                if (_lblArtistName != null && message.Data.AudioFileStarted != null)
                {
                    _lblArtistName.Text = message.Data.AudioFileStarted.ArtistName;
                    _lblAlbumTitle.Text = message.Data.AudioFileStarted.AlbumTitle;
                    _lblSongTitle.Text = message.Data.AudioFileStarted.Title;

                    Task.Factory.StartNew(() => {                            
                        string key = message.Data.AudioFileStarted.ArtistName + "_" + message.Data.AudioFileStarted.AlbumTitle;
                        //Console.WriteLine("MainActivity - Player Bar - key: {0}", key);
                        if (_imageAlbum.Tag == null || _imageAlbum.Tag.ToString().ToUpper() != key.ToUpper())
                        {
                            //Console.WriteLine("MainActivity - Player Bar - key: {0} is different than tag {1} - Fetching album art...", key, (_imageAlbum.Tag == null) ? "null" : _imageAlbum.Tag.ToString());
                            _imageAlbum.Tag = key;
                            byte[] bytesImage = AudioFile.ExtractImageByteArrayForAudioFile(message.Data.AudioFileStarted.FilePath);
                            if (bytesImage.Length == 0)
                                _imageAlbum.SetImageBitmap(null);
                            else
                                BitmapCache.LoadBitmapFromByteArray(bytesImage, key, _imageAlbum);    
                        }
                    });
                }
            }));
            _messengerHub.Subscribe<PlayerStatusMessage>((message) => {
                bool hasStartedPlaying = !_isPlaying && message.Status == PlayerStatusType.Playing;
                _isPlaying = message.Status == PlayerStatusType.Playing;
                //Console.WriteLine("MainActivity - PlayerStatusMessage - Status=" + message.Status.ToString());
                RunOnUiThread(() => {
                    if(hasStartedPlaying)
                    {
                        //Console.WriteLine("MainActivity - PlayerStatusMessage - HasStartedPlaying");
                        if (_viewFlipper.Visibility == ViewStates.Gone)
                        {
                            //Console.WriteLine("MainActivity - PlayerStatusMessage - Showing view flipper");
                            _viewFlipper.Visibility = ViewStates.Visible;
                        }
                    }

                    switch (message.Status)
                    {
                        case PlayerStatusType.Playing:
                            _btnPlayPause.SetImageResource(Resource.Drawable.player_pause);
                            break;
                        default:
                            _btnPlayPause.SetImageResource(Resource.Drawable.player_play);
                            break;
                    }
                });
            });

            Console.WriteLine("MainActivity - OnCreate - Starting navigation manager...");
            _navigationManager = (AndroidNavigationManager) Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            _navigationManager.MainActivity = this; // TODO: Is this OK? Shouldn't the reference be cleared when MainActivity is destroyed? Can lead to memory leaks.
            _navigationManager.BindOptionsMenuView(this);
            _navigationManager.Start();
        }

        public bool OnNavigationItemSelected(int itemPosition, long itemId)
        {
            Console.WriteLine("MainActivity - OnNavigationItemSelected - itemPosition: {0} - itemId: {1}", itemPosition, itemId);
            
            if (_fragment is MobileLibraryBrowserFragment)
            {
                Console.WriteLine("MainActivity - OnNavigationItemSelected - Updating fragment - itemPosition: {0} - itemId: {1}", itemPosition, itemId);
                _navigationManager.ChangeMobileLibraryBrowserType(MobileNavigationTabType.Artists, (MobileLibraryBrowserType)itemPosition);
            }
            return true;
        }

        public void AddTab(MobileNavigationTabType type, string title, Fragment fragment)
        {
            Console.WriteLine("MainActivity - Adding tab {0}", title);

            // Add only one fragment which contains a ViewFlipper with all three view types
            if (type == MobileNavigationTabType.Artists)
            {
                _fragment = fragment;
                FragmentTransaction transaction = FragmentManager.BeginTransaction();
                transaction.Replace(Resource.Id.main_fragmentContainer, fragment);
                transaction.Commit();
            }
        }

        public void PushTabView(MobileNavigationTabType type, Fragment fragment)
        {
            Console.WriteLine("MainActivity - PushTabView type: {0} fragment: {1} fragmentCount: {2}", type.ToString(), fragment.GetType().FullName, FragmentManager.BackStackEntryCount);
            //_tabPagerAdapter.SetFragment(type, fragment);

            //FragmentTransaction transaction = FragmentManager.BeginTransaction();
            //transaction.Replace(Resource.Id.main_fragmentContainer, fragment);
            //transaction.Commit();
            //transaction.SetCustomAnimations()
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

        //protected override void OnStart()
        //{
        //    Console.WriteLine("MainActivity - OnStart");
        //    base.OnStart();
        //}

        //protected override void OnRestart()
        //{
        //    Console.WriteLine("MainActivity - OnRestart");
        //    base.OnRestart();
        //}

        //protected override void OnPause()
        //{
        //    Console.WriteLine("MainActivity - OnPause");
        //    base.OnPause();
        //}

        //protected override void OnResume()
        //{
        //    Console.WriteLine("MainActivity - OnResume");
        //    base.OnResume();
        //}

        //protected override void OnStop()
        //{
        //    Console.WriteLine("MainActivity - OnStop");
        //    base.OnStop();
        //}

        //protected override void OnDestroy()
        //{
        //    Console.WriteLine("MainActivity - OnDestroy");
        //    base.OnDestroy();
        //}

        public override void OnBackPressed()
        {
            var tabType = MobileNavigationTabType.Artists;
            if (_navigationManager.CanGoBackInMobileLibraryBrowserBackstack(tabType))
            {
                //Console.WriteLine("MainActivity - OnBackPressed - CanRemoveFragment");
                _navigationManager.PopMobileLibraryBrowserBackstack(tabType);
            }
            else
            {
                //Console.WriteLine("MainActivity - OnBackPressed - CannotRemoveFragment");
                base.OnBackPressed();
            }
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            Console.WriteLine("MainActivity - OnPrepareOptionsMenu");
            return true;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            Console.WriteLine("MainActivity - OnCreateOptionsMenu");
            
            // Crashed on all Samsung devices when using the options menu
            //MenuInflater.Inflate(Resource.Menu.main_menu, menu);
            //var menuItem = menu.Add(new Java.Lang.String("Test"));
            //var menuItem2 = menu.Add(new Java.Lang.String("Test2"));
            //var menuItem3 = menu.Add(new Java.Lang.String("Test3"));
            foreach (var option in _options)
            {
                var menuItem = menu.Add(new Java.Lang.String(option.Value));
                switch (option.Key)
                {
                    case MobileOptionsMenuType.About:
                        menuItem.SetIcon(Resource.Drawable.actionbar_info);
                        break;
                    case MobileOptionsMenuType.EqualizerPresets:
                        menuItem.SetShowAsAction(ShowAsAction.IfRoom);
                        menuItem.SetIcon(Resource.Drawable.actionbar_equalizer);
                        break;
                    case MobileOptionsMenuType.Preferences:
                        menuItem.SetShowAsAction(ShowAsAction.IfRoom);
                        menuItem.SetIcon(Resource.Drawable.actionbar_settings);
                        break;
                    case MobileOptionsMenuType.SyncLibrary:
                        menuItem.SetShowAsAction(ShowAsAction.IfRoom);
                        menuItem.SetIcon(Resource.Drawable.actionbar_sync);
                        break;
                    case MobileOptionsMenuType.SyncLibraryCloud:
                        menuItem.SetIcon(Resource.Drawable.actionbar_cloud);
                        break;
                    case MobileOptionsMenuType.SyncLibraryFileSharing:
                        menuItem.SetIcon(Resource.Drawable.actionbar_share);
                        break;
                    case MobileOptionsMenuType.SyncLibraryWebBrowser:
                        menuItem.SetIcon(Resource.Drawable.actionbar_earth);
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

        private void BtnPreviousOnClick(object sender, EventArgs eventArgs)
        {
            _messengerHub.PublishAsync<PlayerCommandMessage>(new PlayerCommandMessage(this, PlayerCommandMessageType.Previous));
        }

        private void BtnPlayPauseOnClick(object sender, EventArgs eventArgs)
        {
            _messengerHub.PublishAsync<PlayerCommandMessage>(new PlayerCommandMessage(this, PlayerCommandMessageType.PlayPause));
        }

        private void BtnNextOnClick(object sender, EventArgs eventArgs)
        {
            _messengerHub.PublishAsync<PlayerCommandMessage>(new PlayerCommandMessage(this, PlayerCommandMessageType.Next));
        }

        private void BtnPlaylistOnClick(object sender, EventArgs eventArgs)
        {
        }

        private void BtnLeftOnClick(object sender, EventArgs eventArgs)
        {
            _viewFlipper.SetInAnimation(this, Resource.Animation.flipper_back_slide_in);
            _viewFlipper.SetOutAnimation(this, Resource.Animation.flipper_back_slide_out);
            ShowMiniPlayerSlide(0);
        }

        private void BtnRightOnClick(object sender, EventArgs eventArgs)
        {
            _viewFlipper.SetInAnimation(this, Resource.Animation.flipper_slide_in);
            _viewFlipper.SetOutAnimation(this, Resource.Animation.flipper_slide_out);
            ShowMiniPlayerSlide(1);
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    switch (v.Id)
                    {
                        case Resource.Id.main_miniplayer_btnPrevious:
                            _btnPrevious.SetImageResource(Resource.Drawable.player_previous_on);
                            break;
                        case Resource.Id.main_miniplayer_btnPlayPause:
                            if(_isPlaying)
                                _btnPlayPause.SetImageResource(Resource.Drawable.player_pause_on);
                            else
                                _btnPlayPause.SetImageResource(Resource.Drawable.player_play_on);
                            break;
                        case Resource.Id.main_miniplayer_btnNext:
                            _btnNext.SetImageResource(Resource.Drawable.player_next_on);
                            break;
                        case Resource.Id.main_miniplaylist_btnPlaylist:
                            _btnPlaylist.SetImageResource(Resource.Drawable.player_playlist_on);
                            break;
                        case Resource.Id.main_miniplaylist_btnLeft:
                            _btnLeft.SetImageResource(Resource.Drawable.miniplayer_chevronleft_on);
                            break;
                        case Resource.Id.main_miniplayer_btnRight:
                            _btnRight.SetImageResource(Resource.Drawable.miniplayer_chevronright_on);
                            break;
                    }
                    break;
                case MotionEventActions.Up:
                    switch (v.Id)
                    {
                        case Resource.Id.main_miniplayer_btnPrevious:
                            _btnPrevious.SetImageResource(Resource.Drawable.player_previous);
                            break;
                        case Resource.Id.main_miniplayer_btnPlayPause:
                            if (_isPlaying)
                                _btnPlayPause.SetImageResource(Resource.Drawable.player_pause);
                            else
                                _btnPlayPause.SetImageResource(Resource.Drawable.player_play);
                            break;
                        case Resource.Id.main_miniplayer_btnNext:
                            _btnNext.SetImageResource(Resource.Drawable.player_next);
                            break;
                        case Resource.Id.main_miniplaylist_btnPlaylist:
                            _btnPlaylist.SetImageResource(Resource.Drawable.player_playlist);
                            break;
                        case Resource.Id.main_miniplaylist_btnLeft:
                            _btnLeft.SetImageResource(Resource.Drawable.miniplayer_chevronleft);
                            break;
                        case Resource.Id.main_miniplayer_btnRight:
                            _btnRight.SetImageResource(Resource.Drawable.miniplayer_chevronright);
                            break;
                    }
                    break;
            }
            return false;
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
            if(_splashFragment.Dialog != null)
                _splashFragment.Dialog.Dismiss();
        }

        private void ShowMiniPlayerSlide(int index)
        {
            // Refresh new index (if the same index, prevent animation)
            int realIndex = _viewFlipper.IndexOfChild(index == 0 ? _miniPlayer : _miniPlaylist);
            if(_viewFlipper.DisplayedChild != realIndex)
                _viewFlipper.DisplayedChild = realIndex;

            // Make sure view flipper is visible
            if (_viewFlipper.Visibility == ViewStates.Gone)
                _viewFlipper.Visibility = ViewStates.Visible;
        }

        public void ShowMiniPlaylist()
        {
            _viewFlipper.SetInAnimation(this, Resource.Animation.flipper_slide_in);
            _viewFlipper.SetOutAnimation(this, Resource.Animation.flipper_slide_out);
            ShowMiniPlayerSlide(1);
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
