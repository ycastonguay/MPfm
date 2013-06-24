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
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.Lang;
using MPfm.Android.Classes.Fragments.Base;
using MPfm.Android.Classes.Helpers;
using MPfm.MVP.Messages;
using MPfm.MVP.Models;
using MPfm.MVP.Views;
using MPfm.Player.Objects;
using MPfm.Sound.AudioFiles;

namespace MPfm.Android.Classes.Fragments
{
    public class PlayerFragment : BaseFragment, View.IOnClickListener, IPlayerView
    {        
        private BitmapCache _bitmapCache;
        private View _view;
        private ImageView _imageViewAlbumArt;
        private TextView _lblPosition;
        private TextView _lblLength;
        private Button _btnPlayPause;
        private Button _btnPrevious;
        private Button _btnNext;
        
        // Leave an empty constructor or the application will crash at runtime
        public PlayerFragment() : base(null) { }

        public PlayerFragment(Action<IBaseView> onViewReady) 
            : base(onViewReady)
        {
            
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.Player, container, false);
            _imageViewAlbumArt = _view.FindViewById<ImageView>(Resource.Id.fragment_player_imageViewAlbumArt);
            _lblPosition = _view.FindViewById<TextView>(Resource.Id.fragment_player_lblPosition);
            _lblLength = _view.FindViewById<TextView>(Resource.Id.fragment_player_lblLength);
            _btnPlayPause = _view.FindViewById<Button>(Resource.Id.fragment_player_btnPlayPause);
            _btnPrevious = _view.FindViewById<Button>(Resource.Id.fragment_player_btnPrevious);
            _btnNext = _view.FindViewById<Button>(Resource.Id.fragment_player_btnNext);
            _btnPlayPause.SetOnClickListener(this);
            _btnPrevious.SetOnClickListener(this);
            _btnNext.SetOnClickListener(this);            

            // Create bitmap cache
            int maxMemory = (int)(Runtime.GetRuntime().MaxMemory() / 1024);
            int cacheSize = maxMemory / 8;
            _bitmapCache = new BitmapCache(Activity, cacheSize, 800, 800);

            // Match height with width (cannot do that in xml)
            //_imageViewAlbumArt.LayoutParameters = new ViewGroup.LayoutParams(_imageViewAlbumArt.Width, _imageViewAlbumArt.Width);
            return _view;
        }

        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.fragment_player_btnPlayPause)
            {
                OnPlayerPause();
            }
            else if (v.Id == Resource.Id.fragment_player_btnPrevious)
            {
                OnPlayerPrevious();
            }
            else if (v.Id == Resource.Id.fragment_player_btnNext)
            {
                OnPlayerNext();
            }
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

        public void RefreshPlayerStatus(PlayerStatusType status)
        {
        }

        public void RefreshPlayerPosition(MVP.Models.PlayerPositionEntity entity)
        {
            Activity.RunOnUiThread(() =>
                {
                    _lblPosition.Text = entity.Position;
                });
        }

        public void RefreshSongInformation(AudioFile audioFile, long lengthBytes, int playlistIndex, int playlistCount)
        {
            Activity.RunOnUiThread(() =>
                {
                    if (audioFile != null)
                    {
                        Task.Factory.StartNew(() =>
                            {
                                // Decode album art, add to cache, update image view
                                byte[] bytesImage = AudioFile.ExtractImageByteArrayForAudioFile(audioFile.FilePath);
                                _bitmapCache.LoadBitmapFromByteArray(bytesImage, audioFile.FilePath, _imageViewAlbumArt);
                            });

                        _lblLength.Text = audioFile.Length;
                        //sliderPosition.MaxValue = player.Playlist.CurrentItem.LengthMilliseconds;
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

        public void PlayerError(System.Exception ex)
        {

        }

        #endregion
    }
}
