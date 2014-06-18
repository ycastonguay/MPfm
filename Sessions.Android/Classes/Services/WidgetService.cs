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
using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using Java.Lang;
using MPfm.Android;
using MPfm.Android.Classes.Cache;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Messages;
using MPfm.MVP.Services.Interfaces;
using Sessions.Sound.AudioFiles;
using TinyMessenger;
using Exception = System.Exception;
using Process = Android.OS.Process;

namespace org.sessionsapp.android
{
    //[Service(Name = "org.sessionsapp.android.WidgetService", Label = "Sessions Widget Service")]//, Process = ":.widget.process")]
    [Service(Label = "Sessions Widget Service")]//, Process = ":widgetprocess")]
    public class WidgetService : Service
    {
        ITinyMessengerHub _messengerHub;
        IPlayerService _playerService;
        int[] _widgetIds;
        BitmapCache _bitmapCache;
        string _previousAlbumArtKey;
        Notification _notification;
        bool _isShutDowning;
        AudioFile _audioFile;
        PlayerStatusType _status;
        Bitmap _bitmapAlbumArt;

        public override void OnStart(Intent intent, int startId)
        {
            Console.WriteLine("WidgetService - OnStart - startId: {0}", startId);
            base.OnStart(intent, startId);
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Console.WriteLine("WidgetService - OnStartCommand - startId: {0} intent.action: {1}", startId, intent.Action);
            _widgetIds = intent.GetIntArrayExtra(AppWidgetManager.ExtraAppwidgetIds);

            if (intent.Action == SessionsWidgetActions.SessionsWidgetPrevious.ToString())
            {
                _messengerHub.PublishAsync<PlayerCommandMessage>(new PlayerCommandMessage(this, PlayerCommandMessageType.Previous));
            }
            else if (intent.Action == SessionsWidgetActions.SessionsWidgetPlayPause.ToString())
            {
                _messengerHub.PublishAsync<PlayerCommandMessage>(new PlayerCommandMessage(this, PlayerCommandMessageType.PlayPause));
            }
            else if (intent.Action == SessionsWidgetActions.SessionsWidgetNext.ToString())
            {
                _messengerHub.PublishAsync<PlayerCommandMessage>(new PlayerCommandMessage(this, PlayerCommandMessageType.Next));
            }
            else if (intent.Action == "android.appwidget.action.APPWIDGET_UPDATE")
            {
                Console.WriteLine("WidgetService - Updating notification because of APPWIDGET_UPDATE...");
                UpdateWidgetView();
            }

            return StartCommandResult.NotSticky;
        }

        public override void OnCreate()
        {
            Console.WriteLine("WidgetService - OnCreate");
            Initialize();
            base.OnCreate();
        }

        public override void OnDestroy()
        {
            Console.WriteLine("WidgetService - OnDestroy");
            base.OnDestroy();
        }

        private void Initialize()
        {
            Console.WriteLine("WidgetService - Initializing service...");
            int maxMemory = (int) (Runtime.GetRuntime().MaxMemory()/1024);
            int cacheSize = maxMemory/16;
            _bitmapCache = new BitmapCache(null, cacheSize, 200, 200);

            _messengerHub = Bootstrapper.GetContainer().Resolve<ITinyMessengerHub>();
            _playerService = Bootstrapper.GetContainer().Resolve<IPlayerService>();
            _messengerHub.Subscribe<PlayerPlaylistIndexChangedMessage>((message) => {
                Console.WriteLine("WidgetService - PlayerPlaylistIndexChangedMessage");
                if (message.Data.AudioFileStarted != null)
                {
                    _audioFile = message.Data.AudioFileStarted;
                    //UpdateNotificationView(message.Data.AudioFileStarted, null);
                    UpdateWidgetView();
                    GetAlbumArt(message.Data.AudioFileStarted);
                }
            });
            _messengerHub.Subscribe<PlayerStatusMessage>((message) => {
                Console.WriteLine("WidgetService - PlayerStatusMessage - Status=" + message.Status.ToString());
                _status = message.Status;
                UpdateWidgetView();
            });
        }

        private void UpdateWidgetView()
        {
            if (_isShutDowning)
                return;

            Console.WriteLine("WidgetService - UpdateWidgetView");

            try
            {
                // Update widget
                Console.WriteLine("WidgetService - UpdateWidgetView - Getting widgets (0)...");
                RemoteViews viewWidget = new RemoteViews(PackageName, Resource.Layout.WidgetPlayer);
                Console.WriteLine("WidgetService - UpdateWidgetView - Getting widgets (1)...");
                AppWidgetManager manager = AppWidgetManager.GetInstance(this);
                Console.WriteLine("WidgetService - UpdateWidgetView - Getting widgets (2) - appWidgetIds count: {0}", _widgetIds.Length);

                // Update metadata on widget
                if (_audioFile != null)
                {
                    viewWidget.SetTextViewText(Resource.Id.widgetPlayer_lblArtistName, _audioFile.ArtistName);
                    viewWidget.SetTextViewText(Resource.Id.widgetPlayer_lblAlbumTitle, _audioFile.AlbumTitle);
                    viewWidget.SetTextViewText(Resource.Id.widgetPlayer_lblSongTitle, _audioFile.Title);
                }
                else
                {
                    viewWidget.SetTextViewText(Resource.Id.widgetPlayer_lblArtistName, "");
                    viewWidget.SetTextViewText(Resource.Id.widgetPlayer_lblAlbumTitle, "");
                    viewWidget.SetTextViewText(Resource.Id.widgetPlayer_lblSongTitle, "");                                    
                }

                if (_status == PlayerStatusType.Initialized ||
                    _status == PlayerStatusType.Paused ||
                    _status == PlayerStatusType.Stopped)
                {
                    viewWidget.SetImageViewResource(Resource.Id.widgetPlayer_btnPlayPause, Resource.Drawable.player_play);
                }
                else
                {
                    viewWidget.SetImageViewResource(Resource.Id.widgetPlayer_btnPlayPause, Resource.Drawable.player_pause);
                }

                // Update album art on widget
                if (_bitmapAlbumArt == null)
                    viewWidget.SetImageViewResource(Resource.Id.widgetPlayer_imageAlbum, 0);
                else
                    viewWidget.SetImageViewBitmap(Resource.Id.widgetPlayer_imageAlbum, _bitmapAlbumArt);

                Console.WriteLine("WidgetService - UpdateWidgetView - Getting widgets (3) - appWidgetIds count: {0}", _widgetIds.Length);

                foreach (int id in _widgetIds)
                {
                    Console.WriteLine("WidgetService - UpdateWidgetView - Updating widgets - id: {0}", id);
                    manager.UpdateAppWidget(id, viewWidget);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("WidgetService - UpdateWidgetView - Widget exception: {0}", ex);
            }
        }

        private void GetAlbumArt(AudioFile audioFile)
        {
            _bitmapAlbumArt = null;
            Task.Factory.StartNew(() =>
            {
                //Console.WriteLine("WidgetService - GetAlbumArt - audioFile.Path: {0}", audioFile.FilePath);
                string key = audioFile.ArtistName + "_" + audioFile.AlbumTitle;
                Console.WriteLine("MobileLibraryFragment - Album art - key: {0}", key);
                if (string.IsNullOrEmpty(_previousAlbumArtKey) || _previousAlbumArtKey.ToUpper() != key.ToUpper())
                {
                    //Console.WriteLine("WidgetService - GetAlbumArt - key: {0} is different than tag {1} - Fetching album art...", key, _previousAlbumArtKey);
                    _previousAlbumArtKey = key;
                    byte[] bytesImage = AudioFile.ExtractImageByteArrayForAudioFile(audioFile.FilePath);
                    if (bytesImage.Length == 0)
                        //_imageAlbum.SetImageBitmap(null);
                    {
                        //Console.WriteLine("WidgetService - GetAlbumArt - Setting album art to NULL!");
                        UpdateWidgetView();
                    }
                    else
                    {
                        //((MainActivity)Activity).BitmapCache.LoadBitmapFromByteArray(bytesImage, key, _imageAlbum);
                        //_bitmapCache.LoadBitmapFromByteArray(bytesImage, key, _imageAlbum);
                        //Console.WriteLine("WidgetService - GetAlbumArt - Getting album art in another thread...");
                        _bitmapCache.LoadBitmapFromByteArray(bytesImage, key, bitmap => {
                            Console.WriteLine("WidgetService - GetAlbumArt - RECEIVED ALBUM ART! SETTING ALBUM ART");
                            _bitmapAlbumArt = bitmap;
                            UpdateWidgetView();
                        });
                    }
                }
            });
        }

        public override IBinder OnBind(Intent intent)
        {
            // We don't need to bind to this service
            Console.WriteLine("WidgetService - OnBind");
            return null;
        }
    }
}