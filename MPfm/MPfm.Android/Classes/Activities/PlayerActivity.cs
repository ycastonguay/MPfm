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
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.OS;
using Android.Widget;
using Java.Lang;
using MPfm.Android.Classes.Adapters;
using MPfm.Android.Classes.Cache;
using MPfm.Android.Classes.Helpers;
using MPfm.Android.Classes.Navigation;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Messages;
using MPfm.MVP.Models;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;
using TinyMessenger;
using org.sessionsapp.android;
using Exception = System.Exception;
using Orientation = Android.Content.Res.Orientation;

namespace MPfm.Android
{
    [Activity(Label = "Player", ScreenOrientation = ScreenOrientation.Sensor, Theme = "@style/MyAppTheme", ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class PlayerActivity : BaseActivity, IPlayerView, View.IOnTouchListener
    {
        private bool _isInitialized = false;
        private ITinyMessengerHub _messengerHub;
        private BitmapCache _bitmapCache;
        private WaveFormView _waveFormView;
        private SquareImageView _imageViewAlbumArt;
        private TextView _lblPosition;
        private TextView _lblLength;
        private ImageButton _btnPlayPause;
        private ImageButton _btnPrevious;
        private ImageButton _btnNext;
        private ImageButton _btnShuffle;
        private ImageButton _btnRepeat;
        private ImageButton _btnPlaylist;
        private Button _carrouselDot1;
        private Button _carrouselDot2;
        private Button _carrouselDot3;
        private Button _carrouselDot4;
        private Button _carrouselDot5;
        private SeekBar _seekBar;
        private List<Fragment> _fragments;
        private ViewPager _viewPager;
        private ViewPagerAdapter _viewPagerAdapter;
        private MobileNavigationManager _navigationManager;
        private bool _isPositionChanging;
        private bool _isPlaying;

        public PlayerActivity()
        {
            Console.WriteLine("PlayerActivity - Ctor");
        }

        protected override void OnCreate(Bundle bundle)
        {
            Console.WriteLine("PlayerActivity - OnCreate");

            _messengerHub = Bootstrapper.GetContainer().Resolve<ITinyMessengerHub>(); 
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Player);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetHomeButtonEnabled(true);

            _navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            _fragments = new List<Fragment>();
            _viewPager = FindViewById<ViewPager>(Resource.Id.player_pager);
            _viewPager.OffscreenPageLimit = 4;
            _viewPagerAdapter = new ViewPagerAdapter(FragmentManager, _fragments, _viewPager);
            _viewPagerAdapter.OnPageChanged += ViewPagerAdapterOnOnPageChanged;
            _viewPager.Adapter = _viewPagerAdapter;
            _viewPager.SetOnPageChangeListener(_viewPagerAdapter);

            _waveFormView = FindViewById<WaveFormView>(Resource.Id.player_waveFormView);
            _imageViewAlbumArt = FindViewById<SquareImageView>(Resource.Id.player_imageViewAlbumArt);
            _lblPosition = FindViewById<TextView>(Resource.Id.player_lblPosition);
            _lblLength = FindViewById<TextView>(Resource.Id.player_lblLength);
            _btnPlayPause = FindViewById<ImageButton>(Resource.Id.player_btnPlayPause);
            _btnPrevious = FindViewById<ImageButton>(Resource.Id.player_btnPrevious);
            _btnNext = FindViewById<ImageButton>(Resource.Id.player_btnNext);
            _btnShuffle = FindViewById<ImageButton>(Resource.Id.player_btnShuffle);
            _btnRepeat = FindViewById<ImageButton>(Resource.Id.player_btnRepeat);
            _btnPlaylist = FindViewById<ImageButton>(Resource.Id.player_btnPlaylist);
            _seekBar = FindViewById<SeekBar>(Resource.Id.player_seekBar);
            _carrouselDot1 = FindViewById<Button>(Resource.Id.player_carrouselDot1);
            _carrouselDot2 = FindViewById<Button>(Resource.Id.player_carrouselDot2);
            _carrouselDot3 = FindViewById<Button>(Resource.Id.player_carrouselDot3);
            _carrouselDot4 = FindViewById<Button>(Resource.Id.player_carrouselDot4);
            _carrouselDot5 = FindViewById<Button>(Resource.Id.player_carrouselDot5);
            _btnPlayPause.Click += BtnPlayPauseOnClick;            
            _btnPrevious.Click += BtnPreviousOnClick;
            _btnNext.Click += BtnNextOnClick;
            _btnPlaylist.Click += BtnPlaylistOnClick;
            _btnRepeat.Click += BtnRepeatOnClick;
            _btnShuffle.Click += BtnShuffleOnClick;
            _btnPlayPause.SetOnTouchListener(this);
            _btnPrevious.SetOnTouchListener(this);
            _btnNext.SetOnTouchListener(this);
            _btnPlaylist.SetOnTouchListener(this);
            _btnRepeat.SetOnTouchListener(this);
            _btnShuffle.SetOnTouchListener(this);
            _seekBar.StartTrackingTouch += SeekBarOnStartTrackingTouch;
            _seekBar.StopTrackingTouch += SeekBarOnStopTrackingTouch;
            _seekBar.ProgressChanged += SeekBarOnProgressChanged;

            // Get screen size
            Point size = new Point();
            WindowManager.DefaultDisplay.GetSize(size);

            // Create bitmap cache
            int maxMemory = (int)(Runtime.GetRuntime().MaxMemory() / 1024);
            int cacheSize = maxMemory / 12;
            _bitmapCache = new BitmapCache(this, cacheSize, size.X, size.X); // The album art takes the whole screen width

            // Match height with width (cannot do that in xml)
            //_imageViewAlbumArt.LayoutParameters = new ViewGroup.LayoutParams(_imageViewAlbumArt.Width, _imageViewAlbumArt.Width);

            if (bundle != null)
            {
                string state = bundle.GetString("key", "value");
                Console.WriteLine("PlayerActivity - OnCreate - State is {0} - isInitialized: {1}", state, _isInitialized);
            }
            else
            {
                Console.WriteLine("PlayerActivity - OnCreate - State is null - isInitialized: {0}", _isInitialized);
            }

            // Don't try to check the bundle contents, if the activity wasn't destroyed, it will be null.
            //if (bundle != null)
            //    Console.WriteLine("PlayerActivity - OnCreate - Bundle isn't null - value: {0}", bundle.GetString("key", "null"));
            //else
            //    Console.WriteLine("PlayerActivity - OnCreate - Bundle is null!");

            // When Android stops an activity, it recalls OnCreate after, even though the activity is not destroyed (OnDestroy). It actually goes through creating a new object (the ctor is called).
            ((AndroidNavigationManager)_navigationManager).SetPlayerActivityInstance(this);

            // Activate lock screen if not already activated
            _messengerHub.PublishAsync<ActivateLockScreenMessage>(new ActivateLockScreenMessage(this, true));

            _messengerHub.Subscribe<ApplicationCloseMessage>(message =>
            {
                Console.WriteLine("PlayerActivity - Received ApplicationCloseMessage; closing activity of type {0}", this.GetType().FullName);
            });

        }

        private void ViewPagerAdapterOnOnPageChanged(int position)
        {
            _carrouselDot1.Enabled = position == 0;
            _carrouselDot2.Enabled = position == 1;
            _carrouselDot3.Enabled = position == 2;
            _carrouselDot4.Enabled = position == 3;
            _carrouselDot5.Enabled = position == 4;
        }

        protected override void OnStart()
        {
            Console.WriteLine("PlayerActivity - OnStart");
            base.OnStart();
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);

            // The window manager returns the width depending on orientation
            _waveFormView.RefreshWaveFormBitmap(WindowManager.DefaultDisplay.Width);
        }

        protected override void OnRestart()
        {
            Console.WriteLine("PlayerActivity - OnRestart");
            base.OnRestart();
        }

        protected override void OnPause()
        {
            Console.WriteLine("PlayerActivity - OnPause");
            base.OnPause();
        }

        protected override void OnResume()
        {
            Console.WriteLine("PlayerActivity - OnResume");
            base.OnResume();
        }

        protected override void OnStop()
        {
            Console.WriteLine("PlayerActivity - OnStop");
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            Console.WriteLine("PlayerActivity - OnDestroy");
            base.OnDestroy();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            Console.WriteLine("PlayerActivity - OnSaveInstanceState");
            base.OnSaveInstanceState(outState);
            outState.PutString("key", DateTime.Now.ToLongTimeString());
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.player_menu, menu);
            Console.WriteLine("PlayerActivity - OnCreateOptionsMenu");
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case global::Android.Resource.Id.Home:
                    var intent = new Intent(this, typeof (MainActivity));
                    intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
                    this.StartActivity(intent);
                    this.Finish();
                    return true;
                    break;
                case Resource.Id.playerMenu_item_effects:
                    Console.WriteLine("PlayerActivity - Menu item click - Showing equalizer presets view...");
                    _messengerHub.PublishAsync<MobileNavigationManagerCommandMessage>(new MobileNavigationManagerCommandMessage(this, MobileNavigationManagerCommandMessageType.ShowEqualizerPresetsView));
                    // Why not open this from the presenter instead?
                    return true;
                    break;
                case Resource.Id.playerMenu_item_playlist:
                    Console.WriteLine("PlayerActivity - Menu item click - Showing playlist view...");
                    OnOpenPlaylist();
                    return true;
                    break;
                default:
                    return base.OnOptionsItemSelected(item);
                    break;
            }
        }

        private void BtnPlayPauseOnClick(object sender, EventArgs eventArgs)
        {
            OnPlayerPause();
        }

        private void BtnPreviousOnClick(object sender, EventArgs eventArgs)
        {
            OnPlayerPrevious();
        }

        private void BtnNextOnClick(object sender, EventArgs eventArgs)
        {
            OnPlayerNext();
        }

        private void BtnPlaylistOnClick(object sender, EventArgs eventArgs)
        {
            OnOpenPlaylist();
        }

        private void BtnShuffleOnClick(object sender, EventArgs eventArgs)
        {
            OnPlayerShuffle();
        }

        private void BtnRepeatOnClick(object sender, EventArgs eventArgs)
        {
            OnPlayerRepeat();
        }

        private void SeekBarOnProgressChanged(object sender, SeekBar.ProgressChangedEventArgs progressChangedEventArgs)
        {
            //Console.WriteLine("PlayerActivity - SeekBarOnProgressChanged");
            if (_isPositionChanging)
            {
                PlayerPositionEntity entity = OnPlayerRequestPosition((float) _seekBar.Progress/10000f);
                _lblPosition.Text = entity.Position;
                _waveFormView.SecondaryPosition = entity.PositionBytes;
            }
        }

        private void SeekBarOnStartTrackingTouch(object sender, SeekBar.StartTrackingTouchEventArgs startTrackingTouchEventArgs)
        {
            //Console.WriteLine("PlayerActivity - SeekBarOnStartTrackingTouch");
            _isPositionChanging = true;
            _waveFormView.ShowSecondaryPosition = true;
        }

        private void SeekBarOnStopTrackingTouch(object sender, SeekBar.StopTrackingTouchEventArgs stopTrackingTouchEventArgs)
        {
            //Console.WriteLine("PlayerActivity - SeekBarOnStopTrackingTouch progress: {0}", _seekBar.Progress);
            OnPlayerSetPosition(_seekBar.Progress / 100f);
            _isPositionChanging = false;
            _waveFormView.ShowSecondaryPosition = false;
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    switch (v.Id)
                    {
                        case Resource.Id.player_btnPrevious:
                            _btnPrevious.SetImageResource(Resource.Drawable.player_previous_on);
                            break;
                        case Resource.Id.player_btnPlayPause:
                            if(_isPlaying)
                                _btnPlayPause.SetImageResource(Resource.Drawable.player_pause_on);
                            else
                                _btnPlayPause.SetImageResource(Resource.Drawable.player_play_on);
                            break;
                        case Resource.Id.player_btnNext:
                            _btnNext.SetImageResource(Resource.Drawable.player_next_on);
                            break;
                        case Resource.Id.player_btnPlaylist:
                            _btnPlaylist.SetImageResource(Resource.Drawable.player_playlist_on);
                            break;
                        case Resource.Id.player_btnShuffle:
                            _btnShuffle.SetImageResource(Resource.Drawable.player_shuffle_on);
                            break;
                        case Resource.Id.player_btnRepeat:
                            _btnRepeat.SetImageResource(Resource.Drawable.player_repeat_on);
                            break;
                    }
                    break;
                case MotionEventActions.Up:
                    switch (v.Id)
                    {
                        case Resource.Id.player_btnPrevious:
                            _btnPrevious.SetImageResource(Resource.Drawable.player_previous);
                            break;
                        case Resource.Id.player_btnPlayPause:
                            if(_isPlaying)
                                _btnPlayPause.SetImageResource(Resource.Drawable.player_pause);
                            else
                                _btnPlayPause.SetImageResource(Resource.Drawable.player_play);
                            break;
                        case Resource.Id.player_btnNext:
                            _btnNext.SetImageResource(Resource.Drawable.player_next);
                            break;
                        case Resource.Id.player_btnPlaylist:
                            _btnPlaylist.SetImageResource(Resource.Drawable.player_playlist);
                            break;
                        case Resource.Id.player_btnShuffle:
                            _btnShuffle.SetImageResource(Resource.Drawable.player_shuffle);
                            break;
                        case Resource.Id.player_btnRepeat:
                            _btnRepeat.SetImageResource(Resource.Drawable.player_repeat);
                            break;
                    }
                    break;
            }
            return false;
        }

        #region IPlayerView implementation

        public Action OnPlayerPlay { get; set; }
        public Action<IEnumerable<string>> OnPlayerPlayFiles { get; set; }
        public Action OnPlayerPause { get; set; }
        public Action OnPlayerStop { get; set; }
        public Action OnPlayerPrevious { get; set; }
        public Action OnPlayerNext { get; set; }
        public Action OnPlayerShuffle { get; set; }
        public Action OnPlayerRepeat { get; set; }
        public Action<float> OnPlayerSetVolume { get; set; }
        public Action<float> OnPlayerSetPitchShifting { get; set; }
        public Action<float> OnPlayerSetTimeShifting { get; set; }
        public Action<float> OnPlayerSetPosition { get; set; }
        public Func<float, PlayerPositionEntity> OnPlayerRequestPosition { get; set; }
        public Action OnEditSongMetadata { get; set; }
        public Action OnOpenPlaylist { get; set; }

        public void PlayerError(Exception ex)
        {
            RunOnUiThread(() => {
                AlertDialog ad = new AlertDialog.Builder(this).Create();
                ad.SetCancelable(false);
                ad.SetMessage(string.Format("An error has occured in Player: {0}", ex));
                ad.SetButton("OK", (sender, args) => ad.Dismiss());
                ad.Show();
            });
        }

        public void PushSubView(IBaseView view)
        {
            Console.WriteLine("PlayerActivity - PushSubView view: {0}", view.GetType().FullName);
            _fragments.Add((Fragment)view);

            if (_viewPagerAdapter != null)
                _viewPagerAdapter.NotifyDataSetChanged();
        }

        public void RefreshPlayerStatus(PlayerStatusType status)
        {
            _isPlaying = status == PlayerStatusType.Playing;
            RunOnUiThread(() => {
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
        }

        public void RefreshPlayerPosition(PlayerPositionEntity entity)
        {
            RunOnUiThread(() => {
                if (!_isPositionChanging)
                {
                    _lblPosition.Text = entity.Position;
                    _seekBar.Progress = (int) (entity.PositionPercentage * 100);
                }

                _waveFormView.Position = entity.PositionBytes;
            });
        }

        public void RefreshSongInformation(AudioFile audioFile, long lengthBytes, int playlistIndex, int playlistCount)
        {
            RunOnUiThread(() => {
                if (audioFile == null)
                    return;

                _lblLength.Text = audioFile.Length;
                ActionBar.Title = audioFile.ArtistName;

                Task.Factory.StartNew(() =>
                {
                    string key = audioFile.ArtistName + "_" + audioFile.AlbumTitle;
                    //Console.WriteLine("PlayerActivity - Album art - key: {0}", key);
                    if (_imageViewAlbumArt.Tag == null || _imageViewAlbumArt.Tag.ToString().ToUpper() != key.ToUpper())
                    {
                        //Console.WriteLine("PlayerActivity - Album art - key: {0} is different than tag {1} - Fetching album art...", key, (_imageViewAlbumArt.Tag == null) ? "null" : _imageViewAlbumArt.Tag.ToString());
                        _imageViewAlbumArt.Tag = key;
                        byte[] bytesImage = AudioFile.ExtractImageByteArrayForAudioFile(audioFile.FilePath);
                        if (bytesImage.Length == 0)
                            _imageViewAlbumArt.SetImageBitmap(null);
                        else
                            _bitmapCache.LoadBitmapFromByteArray(bytesImage, key, _imageViewAlbumArt);                            
                    }
                });

                _waveFormView.SetWaveFormLength(lengthBytes);
                _waveFormView.LoadPeakFile(audioFile);
            });   
        }

        public void RefreshMarkers(IEnumerable<Marker> markers)
        {
            RunOnUiThread(() => _waveFormView.SetMarkers(markers));
        }

        public void RefreshLoops(IEnumerable<Loop> loops)
        {
        }

        public void RefreshPlayerVolume(PlayerVolumeEntity entity)
        {
        }

        public void RefreshPlayerTimeShifting(PlayerTimeShiftingEntity entity)
        {
        }

        #endregion

    }
}
