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
using Android.Content.Res;
using Android.Graphics;
using Android.Runtime;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.OS;
using Android.Views.Animations;
using Android.Widget;
using Java.Lang;
using MPfm.Android.Classes.Cache;
using MPfm.Android.Classes.Fragments;
using MPfm.Android.Classes.Navigation;
using MPfm.Library.Objects;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Messages;
using MPfm.MVP.Models;
using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;
using MPfm.Sound.Playlists;
using TinyMessenger;
using org.sessionsapp.android;
using DialogFragment = Android.App.DialogFragment;
using Fragment = Android.App.Fragment;
using FragmentTransaction = Android.App.FragmentTransaction;

namespace MPfm.Android
{
    [Activity(ScreenOrientation = ScreenOrientation.Sensor, Theme = "@style/MyAppTheme", ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class MainActivity : BaseActivity, View.IOnTouchListener, ActionBar.IOnNavigationListener, IMobileOptionsMenuView, IPlayerStatusView, IMobileMainView
    {
        private int _mobileLibraryBrowserHistoryCount = 0;
        private ITinyMessengerHub _messengerHub;
        private ViewFlipper _viewFlipper;
        private LinearLayout _miniPlayer;
        private LinearLayout _miniPlaylist;
        private List<KeyValuePair<MobileOptionsMenuType, string>> _options;
        private TextView _lblArtistName;
        private TextView _lblAlbumTitle;
        private TextView _lblSongTitle;
        //private TextView _lblNextArtistName;
        //private TextView _lblNextAlbumTitle;
        //private TextView _lblNextSongTitle;
        private TextView _lblPlaylistCount;
        private Spinner _cboPlaylist;
        private SquareImageView _imageAlbum;
        private ImageButton _btnPrevious;
        private ImageButton _btnPlayPause;
        private ImageButton _btnNext;
        private ImageButton _btnPlaylist;
        private ImageButton _btnShuffle;
        private ImageButton _btnRepeat;
        private ImageButton _btnLeft;
        private ImageButton _btnRight;
        private ArrayAdapter _actionBarSpinnerAdapter;
        private ArrayAdapter _playlistSpinnerAdapter;
        private Fragment _fragment;
        private bool _isPlaying;

        public BitmapCache BitmapCache { get; private set; }

        protected override void OnCreate(Bundle bundle)
        {
            Console.WriteLine("MainActivity - OnCreate");
            base.OnCreate(bundle);

            _messengerHub = Bootstrapper.GetContainer().Resolve<ITinyMessengerHub>();
            _messengerHub.Subscribe<MobileLibraryBrowserItemClickedMessage>(MobileLibraryBrowserItemClickedMessageReceived);

            RequestWindowFeature(WindowFeatures.ActionBar);
            SetContentView(Resource.Layout.Main);
            _actionBarSpinnerAdapter = ArrayAdapter.CreateFromResource(this, Resource.Array.action_list, Resource.Layout.actionbar_spinner_item);
            ActionBar.NavigationMode = ActionBarNavigationMode.List;
            ActionBar.Title = string.Empty;
            ActionBar.SetListNavigationCallbacks(_actionBarSpinnerAdapter, this);
            ActionBar.SetSelectedNavigationItem(1);

            _viewFlipper = FindViewById<ViewFlipper>(Resource.Id.main_viewflipper);
            _miniPlayer = FindViewById<LinearLayout>(Resource.Id.main_miniplayer);
            _miniPlaylist = FindViewById<LinearLayout>(Resource.Id.main_miniplaylist);
            _lblArtistName = FindViewById<TextView>(Resource.Id.main_miniplayer_lblArtistName);
            _lblAlbumTitle = FindViewById<TextView>(Resource.Id.main_miniplayer_lblAlbumTitle);
            _lblSongTitle = FindViewById<TextView>(Resource.Id.main_miniplayer_lblSongTitle);
            //_lblNextArtistName = FindViewById<TextView>(Resource.Id.main_miniplaylist_lblNextArtistName);
            //_lblNextAlbumTitle = FindViewById<TextView>(Resource.Id.main_miniplaylist_lblNextAlbumTitle);
            //_lblNextSongTitle = FindViewById<TextView>(Resource.Id.main_miniplaylist_lblNextSongTitle);
            _lblPlaylistCount = FindViewById<TextView>(Resource.Id.main_miniplaylist_lblPlaylistCount);
            _btnPrevious = FindViewById<ImageButton>(Resource.Id.main_miniplayer_btnPrevious);
            _btnPlayPause = FindViewById<ImageButton>(Resource.Id.main_miniplayer_btnPlayPause);
            _btnNext = FindViewById<ImageButton>(Resource.Id.main_miniplayer_btnNext);
            _btnPlaylist = FindViewById<ImageButton>(Resource.Id.main_miniplaylist_btnPlaylist);
            _btnShuffle = FindViewById<ImageButton>(Resource.Id.main_miniplaylist_btnShuffle);
            _btnRepeat = FindViewById<ImageButton>(Resource.Id.main_miniplaylist_btnRepeat);
            _btnLeft = FindViewById<ImageButton>(Resource.Id.main_miniplaylist_btnLeft);
            _cboPlaylist = FindViewById<Spinner>(Resource.Id.main_miniplaylist_cboPlaylist);
            _btnRight = FindViewById<ImageButton>(Resource.Id.main_miniplayer_btnRight);
            _imageAlbum = FindViewById<SquareImageView>(Resource.Id.main_miniplayer_imageAlbum);
            _miniPlayer.Click += (sender, args) => _messengerHub.PublishAsync<MobileNavigationManagerCommandMessage>(new MobileNavigationManagerCommandMessage(this, MobileNavigationManagerCommandMessageType.ShowPlayerView));
            _btnLeft.SetOnTouchListener(this);
            _btnRight.SetOnTouchListener(this);
            _btnPrevious.SetOnTouchListener(this);
            _btnPlayPause.SetOnTouchListener(this);
            _btnNext.SetOnTouchListener(this);
            _btnPlaylist.SetOnTouchListener(this);
            _btnShuffle.SetOnTouchListener(this);
            _btnRepeat.SetOnTouchListener(this);
            _btnPrevious.Click += (sender, args) => OnPlayerPrevious();
            _btnPlayPause.Click += (sender, args) => OnPlayerPlayPause();
            _btnNext.Click += (sender, args) => OnPlayerNext();
            _btnPlaylist.Click += (sender, args) => OnOpenPlaylist();
            _btnShuffle.Click += (sender, args) => OnPlayerShuffle();
            _btnRepeat.Click += (sender, args) => OnPlayerRepeat();
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

            _playlistSpinnerAdapter = new ArrayAdapter<string>(this, Resource.Layout.playlist_spinner_item, new string[2] {"Hello", "World"});
            _cboPlaylist.Adapter = _playlistSpinnerAdapter;
            _cboPlaylist.ItemSelected += CboPlaylistOnItemSelected;

            Console.WriteLine("MainActivity - OnCreate - Starting navigation manager...");
            var navigationManager = (AndroidNavigationManager)Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.MainActivity = this; // Watch out, this can lead to memory leaks!
            navigationManager.BindOptionsMenuView(this);
            navigationManager.BindPlayerStatusView(this);
            navigationManager.BindMobileMainView(this);
        }

        private void MobileLibraryBrowserItemClickedMessageReceived(MobileLibraryBrowserItemClickedMessage mobileLibraryBrowserItemClickedMessage)
        {
            _mobileLibraryBrowserHistoryCount++;
        }

        public void PushDialogView(string viewTitle, IBaseView sourceView, IBaseView view)
        {
            Console.WriteLine("MainActivity - PushDialogView view: {0} fragmentCount: {1}", view.GetType().FullName, FragmentManager.BackStackEntryCount);
            var sourceFragment = (Fragment) sourceView;
            var dialogFragment = (DialogFragment)view;
            dialogFragment.Show(sourceFragment.Activity.FragmentManager, viewTitle);
        }

        public void PushDialogSubview(string parentViewTitle, IBaseView view)
        {
            //Console.WriteLine("MainActivity - PushDialogSubview parentViewTitle: {0} view: {1}", parentViewTitle, view.GetType().FullName);
        }

        public bool OnNavigationItemSelected(int itemPosition, long itemId)
        {
            Console.WriteLine("MainActivity - OnNavigationItemSelected - itemPosition: {0} - itemId: {1}", itemPosition, itemId);
            if (_fragment is MobileLibraryBrowserFragment)
            {
                _mobileLibraryBrowserHistoryCount = 0;
                Console.WriteLine("MainActivity - OnNavigationItemSelected - Updating fragment - itemPosition: {0} - itemId: {1}", itemPosition, itemId);
                _messengerHub.PublishAsync<MobileLibraryBrowserChangeQueryMessage>(new MobileLibraryBrowserChangeQueryMessage(this, (MobileLibraryBrowserType)itemPosition, new LibraryQuery()));
            }
            return true;
        }

        protected override void OnStart()
        {
            Console.WriteLine("MainActivity - OnStart");
            base.OnStart();
        }

        protected override void OnRestart()
        {
            Console.WriteLine("MainActivity - OnRestart");
            base.OnRestart();
        }

        protected override void OnPause()
        {
            Console.WriteLine("MainActivity - OnPause");
            base.OnPause();
        }

        protected override void OnResume()
        {
            Console.WriteLine("MainActivity - OnResume");
            base.OnResume();
        }

        protected override void OnStop()
        {
            Console.WriteLine("MainActivity - OnStop");
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            Console.WriteLine("MainActivity - OnDestroy");
            var navigationManager = (AndroidNavigationManager)Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.MainActivity = null;
            base.OnDestroy();
        }

        public override void OnBackPressed()
        {
            //var tabType = MobileNavigationTabType.Artists;
            // Replace by a simple counter. no need to know the tab context, there is only one tab.
            //if (CanGoBackInMobileLibraryBrowserBackstack(tabType))
            if (_mobileLibraryBrowserHistoryCount > 0)
            {
                //Console.WriteLine("MainActivity - OnBackPressed - CanRemoveFragment");
                //PopMobileLibraryBrowserBackstack(tabType);
                _messengerHub.PublishAsync<MobileLibraryBrowserPopBackstackMessage>(new MobileLibraryBrowserPopBackstackMessage(this, MobileLibraryBrowserType.Artists, new LibraryQuery()));
            }
            else
            {
                //Console.WriteLine("MainActivity - OnBackPressed - CannotRemoveFragment");
                base.OnBackPressed();
            }
        }

        //public override bool OnPrepareOptionsMenu(IMenu menu)
        //{
        //    Console.WriteLine("MainActivity - OnPrepareOptionsMenu");
        //    //MenuInflater.Inflate(Resource.Menu.main_menu, menu);
        //    //var menuItem = menu.Add(new Java.Lang.String("Test"));
        //    //menuItem.SetIcon(Resource.Drawable.actionbar_info);
        //    return true;
        //}

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            Console.WriteLine("MainActivity - OnCreateOptionsMenu");
            MenuInflater.Inflate(Resource.Menu.main_menu, menu);
            foreach (var option in _options)
            {                
                // Unfortunately this crashes when selecting the item.
                //var spannableString = new SpannableString(new Java.Lang.String(option.Value));
                //spannableString.SetSpan(new ForegroundColorSpan(Color.White), 0, spannableString.Length(), 0);
                //var menuItem = menu.Add(spannableString);

                var menuItem = menu.Add(option.Value);
                switch (option.Key)
                {
                    case MobileOptionsMenuType.About:
                        //menuItem.SetShowAsAction(ShowAsAction.Never);
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
                        //menuItem.SetShowAsAction(ShowAsAction.Never);
                        menuItem.SetIcon(Resource.Drawable.actionbar_cloud);
                        break;
                    case MobileOptionsMenuType.SyncLibraryFileSharing:
                        //menuItem.SetShowAsAction(ShowAsAction.Never);
                        menuItem.SetIcon(Resource.Drawable.actionbar_share);
                        break;
                    case MobileOptionsMenuType.SyncLibraryWebBrowser:
                        //menuItem.SetShowAsAction(ShowAsAction.Never);
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

        private void CboPlaylistOnItemSelected(object sender, AdapterView.ItemSelectedEventArgs itemSelectedEventArgs)
        {

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
                        case Resource.Id.main_miniplaylist_btnShuffle:
                            _btnShuffle.SetImageResource(Resource.Drawable.player_shuffle_on);
                            break;
                        case Resource.Id.main_miniplaylist_btnRepeat:
                            _btnRepeat.SetImageResource(Resource.Drawable.player_repeat_on);
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
                        case Resource.Id.main_miniplaylist_btnShuffle:
                            _btnShuffle.SetImageResource(Resource.Drawable.player_shuffle);
                            break;
                        case Resource.Id.main_miniplaylist_btnRepeat:
                            _btnRepeat.SetImageResource(Resource.Drawable.player_repeat);
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

        #region IMobileMainView implementation

        public void AddTab(MobileNavigationTabType type, string title, MobileLibraryBrowserType browserType, LibraryQuery query, IBaseView view)
        {
            Console.WriteLine("MainActivity - Adding tab {0}", title);

            //_tabHistory.Add(new Tuple<MobileNavigationTabType, List<Tuple<MobileLibraryBrowserType, LibraryQuery>>>(type, new List<Tuple<MobileLibraryBrowserType, LibraryQuery>>() {
            //   new Tuple<MobileLibraryBrowserType, LibraryQuery>(browserType, query)
            //}));

            _fragment = (Fragment)view;
            FragmentTransaction transaction = FragmentManager.BeginTransaction();
            transaction.Replace(Resource.Id.main_fragmentContainer, _fragment);
            transaction.Commit();
        }

        #endregion

        #region IMobileOptionsMenuView implementation

        public Action<MobileOptionsMenuType> OnItemClick { get; set; }
        public void RefreshMenu(List<KeyValuePair<MobileOptionsMenuType, string>> options)
        {
            Console.WriteLine("MainActivity - RefreshMenu");
            _options = options;
        }

        #endregion

        #region IPlayerStatusView implementation

        public Action OnPlayerPlayPause { get; set; }
        public Action OnPlayerPrevious { get; set; }
        public Action OnPlayerNext { get; set; }
        public Action OnPlayerShuffle { get; set; }
        public Action OnPlayerRepeat { get; set; }
        public Action OnOpenPlaylist { get; set; }

        public void RefreshPlayerStatus(PlayerStatusType status)
        {
            RunOnUiThread(() =>
            {
                bool hasStartedPlaying = !_isPlaying && status == PlayerStatusType.Playing;
                _isPlaying = status == PlayerStatusType.Playing;
                //Console.WriteLine("MainActivity - PlayerStatusMessage - Status=" + message.Status.ToString());
                RunOnUiThread(() =>
                {
                    if (hasStartedPlaying)
                    {
                        //Console.WriteLine("MainActivity - PlayerStatusMessage - HasStartedPlaying");
                        if (_viewFlipper.Visibility == ViewStates.Gone)
                        {
                            //Console.WriteLine("MainActivity - PlayerStatusMessage - Showing view flipper");
                            _viewFlipper.Visibility = ViewStates.Visible;
                        }
                    }

                    switch (status)
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
        }

        public void RefreshAudioFile(AudioFile audioFile)
        {
            RunOnUiThread(() =>
            {
                // Make sure the UI is available
                if (_lblArtistName != null && audioFile != null)
                {
                    _lblArtistName.Text = audioFile.ArtistName;
                    _lblAlbumTitle.Text = audioFile.AlbumTitle;
                    _lblSongTitle.Text = audioFile.Title;

                    //if (message.Data.NextAudioFile != null)
                    //{
                    //    _lblNextArtistName.Text = message.Data.NextAudioFile.ArtistName;
                    //    _lblNextAlbumTitle.Text = message.Data.NextAudioFile.AlbumTitle;
                    //    _lblNextSongTitle.Text = message.Data.NextAudioFile.Title;
                    //}
                    //else
                    //{
                    //    _lblNextArtistName.Text = string.Empty;
                    //    _lblNextAlbumTitle.Text = string.Empty;
                    //    _lblNextSongTitle.Text = string.Empty;
                    //}

                    Task.Factory.StartNew(() =>
                    {
                        string key = audioFile.ArtistName + "_" + audioFile.AlbumTitle;
                        //Console.WriteLine("MainActivity - Player Bar - key: {0}", key);
                        if (_imageAlbum.Tag == null ||
                            _imageAlbum.Tag.ToString().ToUpper() != key.ToUpper())
                        {
                            //Console.WriteLine("MainActivity - Player Bar - key: {0} is different than tag {1} - Fetching album art...", key, (_imageAlbum.Tag == null) ? "null" : _imageAlbum.Tag.ToString());
                            _imageAlbum.Tag = key;
                            byte[] bytesImage = AudioFile.ExtractImageByteArrayForAudioFile(audioFile.FilePath);
                            if (bytesImage.Length == 0)
                                _imageAlbum.SetImageBitmap(null);
                            else
                                BitmapCache.LoadBitmapFromByteArray(bytesImage, key, _imageAlbum);
                        }
                    });
                }
            });

        }

        public void RefreshPlaylist(Playlist playlist)
        {
            RunOnUiThread(() =>
            {
                //_lblPlaylistCount.Text = string.Format("{0}/{1}", playlist.CurrentItemIndex+1, playlist.Items.Count);
                _lblPlaylistCount.Text = string.Format("{0} items", playlist.Items.Count);
                //ShowMiniPlaylist();
            });
        }

        public void RefreshPlaylists(List<PlaylistEntity> playlists, Guid selectedPlaylistId)
        {
            RunOnUiThread(() =>
            {
                var items = playlists.Select(x => x.Name).ToList();
                _playlistSpinnerAdapter = new ArrayAdapter<string>(this, Resource.Layout.playlist_spinner_item, items.ToArray());
                _cboPlaylist.Adapter = _playlistSpinnerAdapter;
                int index = playlists.FindIndex(x => x.PlaylistId == selectedPlaylistId);
                if(index >= 0)
                    _cboPlaylist.SetSelection(index);
            });            
        }

        #endregion

    }
}
