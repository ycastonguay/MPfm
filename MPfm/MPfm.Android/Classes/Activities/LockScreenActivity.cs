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
using System.Timers;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Views;
using Android.OS;
using Android.Widget;
using Java.Lang;
using MPfm.Android.Classes.Cache;
using MPfm.Android.Classes.Navigation;
using MPfm.Core;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Messages;
using MPfm.MVP.Models;
using MPfm.MVP.Navigation;
using MPfm.MVP.Services.Interfaces;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;
using MPfm.Sound.Playlists;
using TinyMessenger;

namespace MPfm.Android
{
    [Activity(Label = "Lock Screen", NoHistory = true, ScreenOrientation = ScreenOrientation.Sensor, Theme = "@style/MyAppTheme", ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class LockScreenActivity : BaseActivity, View.IOnTouchListener, IPlayerStatusView
    {
        MobileNavigationManager _navigationManager;
        ITinyMessengerHub _messengerHub;
        IPlayerService _playerService;
        BitmapCache _bitmapCache;
        TextView _lblArtistName;
        TextView _lblAlbumTitle;
        TextView _lblSongTitle;
        TextView _lblPosition;
        TextView _lblLength;
        ImageButton _btnPrevious;
        ImageButton _btnPlayPause;
        ImageButton _btnNext;
        ImageButton _btnPlaylist;
        ImageButton _btnShuffle;
        ImageButton _btnRepeat;
        Button _btnClose;
        SeekBar _seekBar;
        ImageView _imageAlbum;
        Bitmap _bitmapAlbumArt;
        string _previousAlbumArtKey;
        Timer _timerSongPosition;
        bool _isPositionChanging;
        bool _isPlaying;

        protected override void OnCreate(Bundle bundle)
        {
            Console.WriteLine("LockScreenActivity - OnCreate");
            base.OnCreate(bundle);

            _playerService = Bootstrapper.GetContainer().Resolve<IPlayerService>();
            _messengerHub = Bootstrapper.GetContainer().Resolve<ITinyMessengerHub>();

            // Create bitmap cache
            int maxMemory = (int)(Runtime.GetRuntime().MaxMemory() / 1024);
            int cacheSize = maxMemory / 16;
            _bitmapCache = new BitmapCache(null, cacheSize, 800, 800);

            // Create layout and get controls
            _navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            SetContentView(Resource.Layout.LockScreen);
            _lblArtistName = FindViewById<TextView>(Resource.Id.lockScreen_lblArtistName);
            _lblAlbumTitle = FindViewById<TextView>(Resource.Id.lockScreen_lblAlbumTitle);
            _lblSongTitle = FindViewById<TextView>(Resource.Id.lockScreen_lblSongTitle);
            _lblPosition = FindViewById<TextView>(Resource.Id.lockScreen_lblPosition);
            _lblLength = FindViewById<TextView>(Resource.Id.lockScreen_lblLength);
            _btnPrevious = FindViewById<ImageButton>(Resource.Id.lockScreen_btnPrevious);
            _btnPlayPause = FindViewById<ImageButton>(Resource.Id.lockScreen_btnPlayPause);
            _btnNext = FindViewById<ImageButton>(Resource.Id.lockScreen_btnNext);
            _btnPlaylist = FindViewById<ImageButton>(Resource.Id.lockScreen_btnPlaylist);
            _btnShuffle = FindViewById<ImageButton>(Resource.Id.lockScreen_btnShuffle);
            _btnRepeat = FindViewById<ImageButton>(Resource.Id.lockScreen_btnRepeat);
            _btnClose = FindViewById<Button>(Resource.Id.lockScreen_btnClose);
            _btnPlayPause.SetOnTouchListener(this);
            _btnPrevious.SetOnTouchListener(this);
            _btnNext.SetOnTouchListener(this);
            _btnPlaylist.SetOnTouchListener(this);
            _btnRepeat.SetOnTouchListener(this);
            _btnShuffle.SetOnTouchListener(this);

            _imageAlbum = FindViewById<ImageView>(Resource.Id.lockScreen_imageAlbum);
            _seekBar = FindViewById<SeekBar>(Resource.Id.lockScreen_seekBar);
            _seekBar.StartTrackingTouch += SeekBarOnStartTrackingTouch;
            _seekBar.StopTrackingTouch += SeekBarOnStopTrackingTouch;
            _seekBar.ProgressChanged += SeekBarOnProgressChanged;

            _btnClose.Click += (sender, args) => Finish();
            _btnPrevious.Click += (sender, args) => OnPlayerPrevious();
            _btnPlayPause.Click += (sender, args) => OnPlayerPlayPause();
            _btnNext.Click += (sender, args) => OnPlayerNext();
            _btnShuffle.Click += (sender, args) => OnPlayerShuffle();
            _btnRepeat.Click += (sender, args) => OnPlayerRepeat();
            _btnPlaylist.Click += (sender, args) => {
                Intent intent = new Intent(this, typeof (PlaylistActivity));
                intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
                StartActivity(intent);
            };

            _timerSongPosition = new Timer();
            _timerSongPosition.Interval = 100;
            _timerSongPosition.Elapsed += (sender, args) => {
                if (_isPositionChanging)
                    return;

                RunOnUiThread(() => {
                    try
                    {
                        var position = _playerService.GetPosition();
                        //Console.WriteLine("LockScreenActivity - timerSongPosition - position: {0}", position);
                        _lblPosition.Text = position.Position;
                        float percentage = ((float)position.PositionBytes / (float)_playerService.CurrentPlaylistItem.LengthBytes) * 10000f;
                        _seekBar.Progress = (int)percentage;
                    }
                    catch
                    {
                        // Just ignore exception. It's not really worth it to start/stop the timer when the player is playing.
                        // TODO: In fact reuse the last position instead of returning 0.
                        _lblPosition.Text = "0:00.000";
                        _seekBar.Progress = 0;
                    }
                });
            };
            _timerSongPosition.Start();

            // Since the onViewReady action could not be added to an intent, tell the NavMgr the view is ready
            ((AndroidNavigationManager)_navigationManager).SetLockScreenActivityInstance(this);
        }

        public override void OnAttachedToWindow()
        {
            Console.WriteLine("LockScreenActivity - OnAttachedToWindow");
            var window = this.Window;
            window.AddFlags(WindowManagerFlags.ShowWhenLocked);
            window.SetWindowAnimations(0);
        }

        public override void OnBackPressed()
        {
            Console.WriteLine("LockScreenActivity - OnBackPressed");
            Finish();
        }

        private void SeekBarOnStartTrackingTouch(object sender, SeekBar.StartTrackingTouchEventArgs e)
        {
            _isPositionChanging = true;
        }

        private void SeekBarOnStopTrackingTouch(object sender, SeekBar.StopTrackingTouchEventArgs e)
        {
            _messengerHub.PublishAsync<PlayerSetPositionMessage>(new PlayerSetPositionMessage(this, _seekBar.Progress / 100f));
            _isPositionChanging = false;
        }

        private void SeekBarOnProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            if(_isPositionChanging)
            {
                PlayerPositionEntity entity = RequestPosition((float) _seekBar.Progress/10000f);
                _lblPosition.Text = entity.Position;
            }
        }

        private PlayerPositionEntity RequestPosition(float positionPercentage)
        {
            try
            {
                // TODO: Move this into PlayerStatusPresenter
                //Console.WriteLine("LockScreenActivity - RequestPosition - positionPercentage: {0}", positionPercentage);
                // Calculate new position from 0.0f/1.0f scale
                long lengthBytes = _playerService.CurrentPlaylistItem.LengthBytes;
                var audioFile = _playerService.CurrentPlaylistItem.AudioFile;
                long positionBytes = (long)(positionPercentage * lengthBytes);
                //long positionBytes = (long)Math.Ceiling((double)Playlist.CurrentItem.LengthBytes * (percentage / 100));
                long positionSamples = ConvertAudio.ToPCM(positionBytes, (uint)audioFile.BitsPerSample, audioFile.AudioChannels);
                int positionMS = (int)ConvertAudio.ToMS(positionSamples, (uint)audioFile.SampleRate);
                string positionString = Conversion.MillisecondsToTimeString((ulong)positionMS);
                PlayerPositionEntity entity = new PlayerPositionEntity();
                entity.Position = positionString;
                entity.PositionBytes = positionBytes;
                entity.PositionSamples = (uint)positionSamples;
                return entity;
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("LockScreenActivity - An error occured while calculating the player position: " + ex.Message);
            }
            return new PlayerPositionEntity();
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    switch (v.Id)
                    {
                        case Resource.Id.lockScreen_btnPrevious:
                            _btnPrevious.SetImageResource(Resource.Drawable.player_previous_on);
                            break;
                        case Resource.Id.lockScreen_btnPlayPause:
                            if (_isPlaying)
                                _btnPlayPause.SetImageResource(Resource.Drawable.player_pause_on);
                            else
                                _btnPlayPause.SetImageResource(Resource.Drawable.player_play_on);
                            break;
                        case Resource.Id.lockScreen_btnNext:
                            _btnNext.SetImageResource(Resource.Drawable.player_next_on);
                            break;
                        case Resource.Id.lockScreen_btnPlaylist:
                            _btnPlaylist.SetImageResource(Resource.Drawable.player_playlist_on);
                            break;
                        case Resource.Id.lockScreen_btnShuffle:
                            _btnShuffle.SetImageResource(Resource.Drawable.player_shuffle_on);
                            break;
                        case Resource.Id.lockScreen_btnRepeat:
                            _btnRepeat.SetImageResource(Resource.Drawable.player_repeat_on);
                            break;
                    }
                    break;
                case MotionEventActions.Up:
                    switch (v.Id)
                    {
                        case Resource.Id.lockScreen_btnPrevious:
                            _btnPrevious.SetImageResource(Resource.Drawable.player_previous);
                            break;
                        case Resource.Id.lockScreen_btnPlayPause:
                            if (_isPlaying)
                                _btnPlayPause.SetImageResource(Resource.Drawable.player_pause);
                            else
                                _btnPlayPause.SetImageResource(Resource.Drawable.player_play);
                            break;
                        case Resource.Id.lockScreen_btnNext:
                            _btnNext.SetImageResource(Resource.Drawable.player_next);
                            break;
                        case Resource.Id.lockScreen_btnPlaylist:
                            _btnPlaylist.SetImageResource(Resource.Drawable.player_playlist);
                            break;
                        case Resource.Id.lockScreen_btnShuffle:
                            _btnShuffle.SetImageResource(Resource.Drawable.player_shuffle);
                            break;
                        case Resource.Id.lockScreen_btnRepeat:
                            _btnRepeat.SetImageResource(Resource.Drawable.player_repeat);
                            break;
                    }
                    break;
            }
            return false;
        }

        private void GetAlbumArt(AudioFile audioFile)
        {
            // TODO: Move this to PlayerStatusPresenter
            _bitmapAlbumArt = null;
            Task.Factory.StartNew(() =>
            {
                Console.WriteLine("LockScreenActivity - GetAlbumArt - audioFile.Path: {0}", audioFile.FilePath);
                string key = audioFile.ArtistName + "_" + audioFile.AlbumTitle;
                if (string.IsNullOrEmpty(_previousAlbumArtKey) || _previousAlbumArtKey.ToUpper() != key.ToUpper())
                {
                    Console.WriteLine("LockScreenActivity - GetAlbumArt - key: {0} is different than tag {1} - Fetching album art...", key, _previousAlbumArtKey);
                    _previousAlbumArtKey = key;
                    byte[] bytesImage = AudioFile.ExtractImageByteArrayForAudioFile(audioFile.FilePath);
                    if (bytesImage.Length == 0)
                    {
                        Console.WriteLine("LockScreenActivity - GetAlbumArt - Setting album art to NULL!");
                        RunOnUiThread(() => _imageAlbum.SetImageBitmap(_bitmapAlbumArt));
                    }
                    else
                    {
                        Console.WriteLine("LockScreenActivity - GetAlbumArt - Getting album art in another thread...");
                        _bitmapCache.LoadBitmapFromByteArray(bytesImage, key, bitmap =>
                        {
                            Console.WriteLine("WidgetService - GetAlbumArt - RECEIVED ALBUM ART! SETTING ALBUM ART");
                            _bitmapAlbumArt = bitmap;
                            RunOnUiThread(() => _imageAlbum.SetImageBitmap(_bitmapAlbumArt));
                        });
                    }
                }
            });
        }

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
                //Console.WriteLine("LockScreenActivity - PlayerStatusMessage - Status: " + message.Status.ToString());
                if (status == PlayerStatusType.Initialized ||
                    status == PlayerStatusType.Paused ||
                    status == PlayerStatusType.Stopped)
                {
                    _isPlaying = false;
                    _btnPlayPause.SetImageResource(Resource.Drawable.player_play);
                }
                else
                {
                    _isPlaying = true;
                    _btnPlayPause.SetImageResource(Resource.Drawable.player_pause);
                }
            });
        }

        public void RefreshAudioFile(AudioFile audioFile)
        {
            if (_lblArtistName == null)
                return;

            RunOnUiThread(() => {
                if (audioFile != null)
                {
                    _lblArtistName.Text = audioFile.ArtistName;
                    _lblAlbumTitle.Text = audioFile.AlbumTitle;
                    _lblSongTitle.Text = audioFile.Title;
                    _lblLength.Text = audioFile.Length;
                    GetAlbumArt(audioFile);
                }
                else
                {
                    _lblArtistName.Text = string.Empty;
                    _lblAlbumTitle.Text = string.Empty;
                    _lblSongTitle.Text = string.Empty;
                    _lblLength.Text = string.Empty;
                }                
            });
        }

        public void RefreshPlaylist(Playlist playlist)
        {
            RunOnUiThread(() =>
            {
                
            });
        }

        public void RefreshPlaylists(List<PlaylistEntity> playlists, Guid selectedPlaylistId)
        {
            
        }

        #endregion
    }
}
