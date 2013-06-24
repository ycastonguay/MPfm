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
using Android.OS;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Java.Lang;
using MPfm.Android.Classes.Adapters;
using MPfm.Android.Classes.Fragments.Base;
using MPfm.Android.Classes.Helpers;
using MPfm.MVP.Messages;
using MPfm.MVP.Models;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;

namespace MPfm.Android.Classes.Fragments
{
    public class PlayerFragment : BaseFragment, View.IOnClickListener, IPlayerView
    {
        private bool _isPositionChanging;
        private BitmapCache _bitmapCache;
        private View _view;
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

        // Leave an empty constructor or the application will crash at runtime
        public PlayerFragment() : base(null)
        {
            _fragments = new List<Fragment>();
        }

        public PlayerFragment(Action<IBaseView> onViewReady) 
            : base(onViewReady)
        {
            _fragments = new List<Fragment>();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.Player, container, false);            
            _viewPager = _view.FindViewById<ViewPager>(Resource.Id.player_pager);
            _tabPagerAdapter = new TabPagerAdapter(FragmentManager, _fragments, _viewPager, Activity.ActionBar);
            _viewPager.Adapter = _tabPagerAdapter;
            _viewPager.SetOnPageChangeListener(_tabPagerAdapter);

            _imageViewAlbumArt = _view.FindViewById<ImageView>(Resource.Id.player_imageViewAlbumArt);
            _lblPosition = _view.FindViewById<TextView>(Resource.Id.player_lblPosition);
            _lblLength = _view.FindViewById<TextView>(Resource.Id.player_lblLength);
            _btnPlayPause = _view.FindViewById<Button>(Resource.Id.player_btnPlayPause);
            _btnPrevious = _view.FindViewById<Button>(Resource.Id.player_btnPrevious);
            _btnNext = _view.FindViewById<Button>(Resource.Id.player_btnNext);
            _seekBar = _view.FindViewById<SeekBar>(Resource.Id.player_seekBar);
            _btnPlayPause.SetOnClickListener(this);
            _btnPrevious.SetOnClickListener(this);
            _btnNext.SetOnClickListener(this);
            _seekBar.StartTrackingTouch += SeekBarOnStartTrackingTouch;
            _seekBar.StopTrackingTouch += SeekBarOnStopTrackingTouch;
            _seekBar.ProgressChanged += SeekBarOnProgressChanged;

            // Create bitmap cache
            int maxMemory = (int)(Runtime.GetRuntime().MaxMemory() / 1024);
            int cacheSize = maxMemory / 8;
            _bitmapCache = new BitmapCache(Activity, cacheSize, 800, 800);

            // Match height with width (cannot do that in xml)
            //_imageViewAlbumArt.LayoutParameters = new ViewGroup.LayoutParams(_imageViewAlbumArt.Width, _imageViewAlbumArt.Width);
            return _view;
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

        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.player_btnPlayPause)
                OnPlayerPause();
            else if (v.Id == Resource.Id.player_btnPrevious)
                OnPlayerPrevious();
            else if (v.Id == Resource.Id.player_btnNext)
                OnPlayerNext();
        }

        public void AddSubview(IBaseView view)
        {
            Console.WriteLine("PlayerFragment - AddSubview view: {0}", view.GetType().FullName);
            _fragments.Add((Fragment)view);

            if(_tabPagerAdapter != null)
                _tabPagerAdapter.NotifyDataSetChanged();
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

        public void PlayerError(System.Exception ex)
        {

        }

        public void RefreshPlayerStatus(PlayerStatusType status)
        {
        }

        public void RefreshPlayerPosition(PlayerPositionEntity entity)
        {
            Activity.RunOnUiThread(() => {
                if (!_isPositionChanging)
                {
                    _lblPosition.Text = entity.Position;
                    _seekBar.Progress = (int) entity.PositionPercentage;
                }
            });
        }

        public void RefreshSongInformation(AudioFile audioFile, long lengthBytes, int playlistIndex, int playlistCount)
        {
            Activity.RunOnUiThread(() => {
                if (audioFile != null)
                {
                    // Decode album art, add to cache, update image view
                    Task.Factory.StartNew(() =>{
                        byte[] bytesImage = AudioFile.ExtractImageByteArrayForAudioFile(audioFile.FilePath);
                        _bitmapCache.LoadBitmapFromByteArray(bytesImage, audioFile.FilePath, _imageViewAlbumArt);
                    });

                    _lblLength.Text = audioFile.Length;
                }
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
