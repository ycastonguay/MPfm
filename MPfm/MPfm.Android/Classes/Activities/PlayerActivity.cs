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
using Android.Support.V4.View;
using Android.Views;
using Android.OS;
using Android.Widget;
using Java.Lang;
using MPfm.Android.Classes.Adapters;
using MPfm.Android.Classes.Helpers;
using MPfm.Android.Classes.Navigation;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Messages;
using MPfm.MVP.Models;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;
using Exception = System.Exception;

namespace MPfm.Android
{
    [Activity(Label = "Player", ScreenOrientation = ScreenOrientation.Sensor, Theme = "@style/MyAppTheme", ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class PlayerActivity : BaseActivity, IPlayerView
    {
        private bool _isPositionChanging;
        private BitmapCache _bitmapCache;
        private ImageView _imageViewAlbumArt;
        private TextView _lblPosition;
        private TextView _lblLength;
        private Button _btnPlayPause;
        private Button _btnPrevious;
        private Button _btnNext;
        private SeekBar _seekBar;
        private List<Fragment> _fragments;
        private ViewPager _viewPager;
        private TabPagerAdapter _tabPagerAdapter;
        private MobileNavigationManager _navigationManager;

        protected override void OnCreate(Bundle bundle)
        {
            Console.WriteLine("PlayerActivity - OnCreate");
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Player);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetHomeButtonEnabled(true);

            _navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            _fragments = new List<Fragment>();
            _viewPager = FindViewById<ViewPager>(Resource.Id.player_pager);
            _viewPager.OffscreenPageLimit = 4;
            _tabPagerAdapter = new TabPagerAdapter(FragmentManager, _fragments, _viewPager, ActionBar);
            _viewPager.Adapter = _tabPagerAdapter;
            _viewPager.SetOnPageChangeListener(_tabPagerAdapter);

            _imageViewAlbumArt = FindViewById<ImageView>(Resource.Id.player_imageViewAlbumArt);
            _lblPosition = FindViewById<TextView>(Resource.Id.player_lblPosition);
            _lblLength = FindViewById<TextView>(Resource.Id.player_lblLength);
            _btnPlayPause = FindViewById<Button>(Resource.Id.player_btnPlayPause);
            _btnPrevious = FindViewById<Button>(Resource.Id.player_btnPrevious);
            _btnNext = FindViewById<Button>(Resource.Id.player_btnNext);
            _seekBar = FindViewById<SeekBar>(Resource.Id.player_seekBar);
            _btnPlayPause.Click += BtnPlayPauseOnClick;            
            _btnPrevious.Click += BtnPreviousOnClick;
            _btnNext.Click += BtnNextOnClick;
            _seekBar.StartTrackingTouch += SeekBarOnStartTrackingTouch;
            _seekBar.StopTrackingTouch += SeekBarOnStopTrackingTouch;
            _seekBar.ProgressChanged += SeekBarOnProgressChanged;

            // Create bitmap cache
            int maxMemory = (int)(Runtime.GetRuntime().MaxMemory() / 1024);
            int cacheSize = maxMemory / 8;
            _bitmapCache = new BitmapCache(this, cacheSize, 800, 800);

            // Match height with width (cannot do that in xml)
            //_imageViewAlbumArt.LayoutParameters = new ViewGroup.LayoutParams(_imageViewAlbumArt.Width, _imageViewAlbumArt.Width);

            if (bundle != null)
            {
                string state = bundle.GetString("key", "value");
                Console.WriteLine("MainActivity - OnCreate - State is {0}", state);
            }
            else
            {
                Console.WriteLine("MainActivity - OnCreate - State is null");
            }
        }

        protected override void OnStart()
        {
            Console.WriteLine("PlayerActivity - OnStart");
            base.OnStart();

            // Since the onViewReady action could not be added to an intent, tell the NavMgr the view is ready
            ((AndroidNavigationManager)_navigationManager).SetPlayerActivityInstance(this);            
        }

        public void AddSubview(IBaseView view)
        {
            Console.WriteLine("PlayerActivity - AddSubview view: {0}", view.GetType().FullName);
            _fragments.Add((Fragment)view);

            if (_tabPagerAdapter != null)
                _tabPagerAdapter.NotifyDataSetChanged();
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

        private void SeekBarOnProgressChanged(object sender, SeekBar.ProgressChangedEventArgs progressChangedEventArgs)
        {
            Console.WriteLine("SeekBarOnProgressChanged");
            PlayerPositionEntity entity = OnPlayerRequestPosition((float)_seekBar.Progress / 100f);
            _lblPosition.Text = entity.Position;
        }

        private void SeekBarOnStartTrackingTouch(object sender, SeekBar.StartTrackingTouchEventArgs startTrackingTouchEventArgs)
        {
            Console.WriteLine("SeekBarOnStartTrackingTouch");
            _isPositionChanging = true;
        }

        private void SeekBarOnStopTrackingTouch(object sender, SeekBar.StopTrackingTouchEventArgs stopTrackingTouchEventArgs)
        {
            Console.WriteLine("SeekBarOnStopTrackingTouch progress: {0}", _seekBar.Progress);
            OnPlayerSetPosition(_seekBar.Progress);
            _isPositionChanging = false;
        }

        #region IPlayerView implementation

        public Action OnPlayerPlay { get; set; }
        public Action<IEnumerable<string>> OnPlayerPlayFiles { get; set; }
        public Action OnPlayerPause { get; set; }
        public Action OnPlayerStop { get; set; }
        public Action OnPlayerPrevious { get; set; }
        public Action OnPlayerNext { get; set; }
        public Action<float> OnPlayerSetVolume { get; set; }
        public Action<float> OnPlayerSetPitchShifting { get; set; }
        public Action<float> OnPlayerSetTimeShifting { get; set; }
        public Action<float> OnPlayerSetPosition { get; set; }
        public Func<float, PlayerPositionEntity> OnPlayerRequestPosition { get; set; }

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

        public void RefreshPlayerStatus(PlayerStatusType status)
        {
        }

        public void RefreshPlayerPosition(PlayerPositionEntity entity)
        {
            RunOnUiThread(() => {
                if (!_isPositionChanging)
                {
                    _lblPosition.Text = entity.Position;
                    _seekBar.Progress = (int)entity.PositionPercentage;
                }
            });
        }

        public void RefreshSongInformation(AudioFile audioFile, long lengthBytes, int playlistIndex, int playlistCount)
        {
            RunOnUiThread(() => {
                if (audioFile == null)
                    return;

                _lblLength.Text = audioFile.Length;

                Task.Factory.StartNew(() =>
                {
                    byte[] bytesImage = AudioFile.ExtractImageByteArrayForAudioFile(audioFile.FilePath);
                    _bitmapCache.LoadBitmapFromByteArray(bytesImage, audioFile.FilePath, _imageViewAlbumArt);
                });
            });   
        }

        public void RefreshMarkers(IEnumerable<Marker> markers)
        {
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
