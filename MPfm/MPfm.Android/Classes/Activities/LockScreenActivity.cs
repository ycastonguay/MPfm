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
using MPfm.Core;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Messages;
using MPfm.MVP.Models;
using MPfm.MVP.Services.Interfaces;
using MPfm.Sound.AudioFiles;
using TinyMessenger;
using Exception = System.Exception;

namespace MPfm.Android
{
    [Activity(Label = "Lock Screen", NoHistory = true, ScreenOrientation = ScreenOrientation.Sensor, Theme = "@style/MyAppTheme", ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class LockScreenActivity : BaseActivity
    {
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

        protected override void OnCreate(Bundle bundle)
        {
            Console.WriteLine("LockScreenActivity - OnCreate");
            base.OnCreate(bundle);

            // Create bitmap cache
            int maxMemory = (int)(Runtime.GetRuntime().MaxMemory() / 1024);
            int cacheSize = maxMemory / 16;
            _bitmapCache = new BitmapCache(null, cacheSize, 800, 800);

            // Create layout and get controls
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
            _imageAlbum = FindViewById<ImageView>(Resource.Id.lockScreen_imageAlbum);
            _seekBar = FindViewById<SeekBar>(Resource.Id.lockScreen_seekBar);
            _seekBar.StartTrackingTouch += SeekBarOnStartTrackingTouch;
            _seekBar.StopTrackingTouch += SeekBarOnStopTrackingTouch;
            _seekBar.ProgressChanged += SeekBarOnProgressChanged;

            _playerService = Bootstrapper.GetContainer().Resolve<IPlayerService>();
            _messengerHub = Bootstrapper.GetContainer().Resolve<ITinyMessengerHub>();
            _messengerHub.Subscribe<PlayerPlaylistIndexChangedMessage>((message) => {
                RunOnUiThread(() => {
                    //Console.WriteLine("LockScreenActivity - PlayerPlaylistIndexChangedMessage");
                    if (message.Data.AudioFileStarted != null)
                    {
                        if (_lblArtistName != null)
                        {
                            //Console.WriteLine("LockScreenActivity - PlayerPlaylistIndexChangedMessage - Updating controls...");
                            _lblArtistName.Text = message.Data.AudioFileStarted.ArtistName;
                            _lblAlbumTitle.Text = message.Data.AudioFileStarted.AlbumTitle;
                            _lblSongTitle.Text = message.Data.AudioFileStarted.Title;
                            _lblLength.Text = message.Data.AudioFileStarted.Length;
                        }
                    }
                    GetAlbumArt(message.Data.AudioFileStarted);
                });
            });
            _messengerHub.Subscribe<PlayerStatusMessage>((message) =>
            {
                RunOnUiThread(() => {
                    //Console.WriteLine("LockScreenActivity - PlayerStatusMessage - Status: " + message.Status.ToString());
                    var status = message.Status;
                    if (status == PlayerStatusType.Initialized ||
                        status == PlayerStatusType.Paused ||
                        status == PlayerStatusType.Stopped)
                    {
                        _btnPlayPause.SetImageResource(Resource.Drawable.player_play);
                    }
                    else
                    {
                        _btnPlayPause.SetImageResource(Resource.Drawable.player_pause);
                    }
                });
            });

            _btnClose.Click += (sender, args) => Finish();
            _btnPrevious.Click += (sender, args) => _messengerHub.PublishAsync<PlayerCommandMessage>(new PlayerCommandMessage(this, PlayerCommandMessageType.Previous));
            _btnPlayPause.Click += (sender, args) => _messengerHub.PublishAsync<PlayerCommandMessage>(new PlayerCommandMessage(this, PlayerCommandMessageType.PlayPause));
            _btnNext.Click += (sender, args) => _messengerHub.PublishAsync<PlayerCommandMessage>(new PlayerCommandMessage(this, PlayerCommandMessageType.Next));
            _btnShuffle.Click += (sender, args) => _messengerHub.PublishAsync<PlayerCommandMessage>(new PlayerCommandMessage(this, PlayerCommandMessageType.Shuffle));
            _btnRepeat.Click += (sender, args) => _messengerHub.PublishAsync<PlayerCommandMessage>(new PlayerCommandMessage(this, PlayerCommandMessageType.Repeat));
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
                        _lblPosition.Text = "0:00.000";
                        _seekBar.Progress = 0;
                    }
                });
            };
            _timerSongPosition.Start();
        }

        public override void OnAttachedToWindow()
        {
            Console.WriteLine("LockScreenActivity - OnAttachedToWindow");
            var window = this.Window;
            window.AddFlags(WindowManagerFlags.ShowWhenLocked);
            window.SetWindowAnimations(0);
        }

        protected override void OnStart()
        {
            Console.WriteLine("LockScreenActivity - OnStart");
            base.OnStart();
        }

        protected override void OnRestart()
        {
            Console.WriteLine("LockScreenActivity - OnRestart");
            base.OnRestart();
        }

        protected override void OnPause()
        {
            Console.WriteLine("LockScreenActivity - OnPause");
            base.OnPause();
        }

        protected override void OnResume()
        {
            Console.WriteLine("LockScreenActivity - OnResume");
            base.OnResume();

            if (!_playerService.IsPlaying || _playerService.CurrentPlaylistItem == null)
                return;

            _lblArtistName.Text = _playerService.CurrentPlaylistItem.AudioFile.ArtistName;
            _lblAlbumTitle.Text = _playerService.CurrentPlaylistItem.AudioFile.AlbumTitle;
            _lblSongTitle.Text = _playerService.CurrentPlaylistItem.AudioFile.Title;
            _lblLength.Text = _playerService.CurrentPlaylistItem.AudioFile.Length;
            GetAlbumArt(_playerService.CurrentPlaylistItem.AudioFile);

            var status = _playerService.Status;
            if (status == PlayerStatusType.Initialized ||
                status == PlayerStatusType.Paused ||
                status == PlayerStatusType.Stopped)
            {
                _btnPlayPause.SetImageResource(Resource.Drawable.player_play);
            }
            else
            {
                _btnPlayPause.SetImageResource(Resource.Drawable.player_pause);
            }
        }

        protected override void OnStop()
        {
            Console.WriteLine("LockScreenActivity - OnStop");
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            Console.WriteLine("LockScreenActivity - OnDestroy");
            base.OnDestroy();
        }

        public override void OnBackPressed()
        {
            //base.OnBackPressed();
            Console.WriteLine("LockScreenActivity - OnBackPressed");
            Finish();
        }

        private void SeekBarOnStartTrackingTouch(object sender, SeekBar.StartTrackingTouchEventArgs e)
        {
            //Console.WriteLine("LockScreenActivity - SeekBarOnStartTrackingTouch");
            _isPositionChanging = true;
        }

        private void SeekBarOnStopTrackingTouch(object sender, SeekBar.StopTrackingTouchEventArgs e)
        {
            //Console.WriteLine("LockScreenActivity - SeekBarOnStopTrackingTouch progress: {0}", _seekBar.Progress);
            _messengerHub.PublishAsync<PlayerSetPositionMessage>(new PlayerSetPositionMessage(this, _seekBar.Progress / 100f));
            _isPositionChanging = false;
        }

        private void SeekBarOnProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            //Console.WriteLine("LockScreenActivity - SeekBarOnProgressChanged");
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
                Console.WriteLine("LockScreenActivity - RequestPosition - positionPercentage: {0}", positionPercentage);
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
            catch (Exception ex)
            {
                Console.WriteLine("LockScreenActivity - An error occured while calculating the player position: " + ex.Message);
            }
            return new PlayerPositionEntity();
        }

        private void GetAlbumArt(AudioFile audioFile)
        {
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
    }
}
