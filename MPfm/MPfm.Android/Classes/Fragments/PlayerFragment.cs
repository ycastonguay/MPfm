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

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.Lang;
using MPfm.Android.Classes.Helpers;
using MPfm.Core;
using MPfm.Player;
using MPfm.Sound.AudioFiles;
using MPfm.Sound.Bass.Net;
using Environment = Android.OS.Environment;

namespace MPfm.Android.Classes.Fragments
{
    public class PlayerFragment : Fragment, View.IOnClickListener
    {        
        private BitmapCache bitmapCache;

        private View _view;
        private ImageView _imageViewAlbumArt;
        private TextView _lblArtistName;
        private TextView _lblAlbumTitle;
        private TextView _lblSongTitle;
        private TextView _lblPosition;
        private TextView _lblLength;
        private Button _btnPlayPause;
        private Button _btnPrevious;
        private Button _btnNext;

        private Timer timer;
        private IPlayer player;

        private void Initialize()
        {
            // Configure player
            player = MPfm.Player.Player.CurrentPlayer;
            player.OnPlaylistIndexChanged += (data) =>
            {
                if (data.AudioFileEnded != null &&
                   data.AudioFileEnded.ArtistName == data.AudioFileStarted.ArtistName &&
                   data.AudioFileEnded.AlbumTitle == data.AudioFileStarted.AlbumTitle)
                {
                    RefreshAudioFile(data.AudioFileStarted, true);
                }
                else
                {
                    RefreshAudioFile(data.AudioFileStarted, false);
                }
            };

            // Create bitmap cache
            int maxMemory = (int)(Runtime.GetRuntime().MaxMemory() / 1024);
            int cacheSize = maxMemory / 8;
            bitmapCache = new BitmapCache(Activity, cacheSize, 800, 800);

            // Create timer for updating position
            timer = new Timer();
            timer.Interval = 100;
            timer.Elapsed += (sender, e) => Activity.RunOnUiThread(() =>
                {
                    try
                    {
                        long bytes = MPfm.Player.Player.CurrentPlayer.GetPosition();
                        long samples = ConvertAudio.ToPCM(bytes, (uint)MPfm.Player.Player.CurrentPlayer.Playlist.CurrentItem.AudioFile.BitsPerSample, 2);
                        long ms = ConvertAudio.ToMS(samples, (uint)MPfm.Player.Player.CurrentPlayer.Playlist.CurrentItem.AudioFile.SampleRate);
                        string pos = Conversion.MillisecondsToTimeString((ulong)ms);
                        _lblPosition.Text = pos;
                        //sliderPosition.Value = ms;
                    }
                    catch
                    {
                        _lblPosition.Text = "0:00.000";
                    }
                });
        }
    
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.Fragment_Player, container, false);
            _imageViewAlbumArt = _view.FindViewById<ImageView>(Resource.Id.fragment_player_imageViewAlbumArt);
            _lblArtistName = _view.FindViewById<TextView>(Resource.Id.fragment_player_lblArtistName);
            _lblAlbumTitle = _view.FindViewById<TextView>(Resource.Id.fragment_player_lblAlbumTitle);
            _lblSongTitle = _view.FindViewById<TextView>(Resource.Id.fragment_player_lblSongTitle);
            _lblPosition = _view.FindViewById<TextView>(Resource.Id.fragment_player_lblPosition);
            _lblLength = _view.FindViewById<TextView>(Resource.Id.fragment_player_lblLength);
            _btnPlayPause = _view.FindViewById<Button>(Resource.Id.fragment_player_btnPlayPause);
            _btnPrevious = _view.FindViewById<Button>(Resource.Id.fragment_player_btnPrevious);
            _btnNext = _view.FindViewById<Button>(Resource.Id.fragment_player_btnNext);
            _btnPlayPause.SetOnClickListener(this);
            _btnPrevious.SetOnClickListener(this);
            _btnNext.SetOnClickListener(this);

            // Match height with width (cannot do that in xml)
            //_imageViewAlbumArt.LayoutParameters = new ViewGroup.LayoutParams(_imageViewAlbumArt.Width, _imageViewAlbumArt.Width);
            return _view;
        }

        public override void OnResume()
        {
            base.OnResume();

            Initialize();

            if(!player.IsPlaying)
                Play();
        }

        public void OnClick(View v)
        {
            if (v.Id == Resource.Id.fragment_player_btnPlayPause)
            {
                MPfm.Player.Player.CurrentPlayer.Pause();
            }
            else if (v.Id == Resource.Id.fragment_player_btnPrevious)
            {
                MPfm.Player.Player.CurrentPlayer.Previous();
            }
            else if (v.Id == Resource.Id.fragment_player_btnNext)
            {
                MPfm.Player.Player.CurrentPlayer.Next();                
            }
        }

        private List<string> DirectorySearch(string directoryPath)
        {
            List<string> filePaths = new List<string>();
            foreach (string filePath in Directory.GetFiles(directoryPath))
            {
                if (filePath.ToUpper().Contains(".MP3") ||
                    filePath.ToUpper().Contains(".FLAC"))
                {
                    filePaths.Add(filePath);
                }
            }
            foreach (string subDirectoryPath in Directory.GetDirectories(directoryPath))
            {
                filePaths.AddRange(DirectorySearch(subDirectoryPath));
            }
            return filePaths;
        }

        private void Play()
        {
            string musicPath = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryMusic).ToString();
            List<string> listFiles = DirectorySearch(musicPath);

            if (listFiles.Count > 0)
            {
                player.PlayFiles(listFiles);
            }
            else
            {
                return;
            }

            timer.Start();
            RefreshAudioFile(player.Playlist.CurrentItem.AudioFile, false);
        }

        private void RefreshAudioFile(AudioFile audioFile, bool isSameAlbum)
        {
            Activity.RunOnUiThread(() =>
            {
                if (!isSameAlbum)
                {
                    Task.Factory.StartNew(() =>
                        {
                            // Decode album art, add to cache, update image view
                            byte[] bytesImage = AudioFile.ExtractImageByteArrayForAudioFile(audioFile.FilePath);
                            bitmapCache.LoadBitmapFromByteArray(bytesImage, audioFile.FilePath, _imageViewAlbumArt);
                        });
                }

                _lblArtistName.Text = audioFile.ArtistName;
                _lblAlbumTitle.Text = audioFile.AlbumTitle;
                _lblSongTitle.Text = audioFile.Title;
                _lblLength.Text = player.Playlist.CurrentItem.LengthString;
                //sliderPosition.MaxValue = player.Playlist.CurrentItem.LengthMilliseconds;
            });
        }
    }
}
