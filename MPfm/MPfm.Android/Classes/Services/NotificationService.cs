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
using Android.Support.V4.App;
using Android.Widget;
using Java.Lang;
using MPfm.Android;
using MPfm.Android.Classes.Cache;
using MPfm.Android.Classes.Widgets;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Messages;
using MPfm.MVP.Services.Interfaces;
using MPfm.Sound.AudioFiles;
using TinyMessenger;
using Exception = System.Exception;
using Process = Android.OS.Process;

namespace org.sessionsapp.android
{
    //[Service(Name = "org.sessionsapp.android.NotificationService", Label = "Sessions Widget Service")]//, Process = ":.widget.process")]
    [Service(Label = "Sessions Notification Service")]//, Process = ":widgetprocess")]
    public class NotificationService : Service
    {
        private ITinyMessengerHub _messengerHub;
        private IPlayerService _playerService;
        private int[] _widgetIds;
        private BitmapCache _bitmapCache;
        private string _previousAlbumArtKey;
        private Notification _notification;
        bool _isShutDowning;
        AudioFile _audioFile;
        PlayerStatusType _status;
        Bitmap _bitmapAlbumArt;

        public override void OnStart(Intent intent, int startId)
        {
            Console.WriteLine("NotificationService - OnStart - startId: {0}", startId);
            base.OnStart(intent, startId);
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Console.WriteLine("NotificationService - OnStartCommand - startId: {0} intent.action: {1}", startId, intent.Action);
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
            else if (intent.Action == SessionsWidgetActions.SessionsWidgetClose.ToString())
            {
                _isShutDowning = true;
                Console.WriteLine("NotificationService - Closing the application...");

                // Broadcast any component that the application is closing; do not add the lock screen activity until the application is 'restarted'
                Console.WriteLine("NotificationService - Notifying ApplicationCloseMessage...");
                _messengerHub.PublishAsync<ApplicationCloseMessage>(new ApplicationCloseMessage(this));
                _messengerHub.PublishAsync<ActivateLockScreenMessage>(new ActivateLockScreenMessage(this, false));

                // Stop playback and remove notification service from foreground
                _playerService.Stop();
                StopForeground(true); // maybe that is enough and the service doesn't have to be stopped?
                var notificationManager = (NotificationManager)ApplicationContext.GetSystemService(NotificationService);
                notificationManager.Cancel(1);   
                StopSelf();
            }

            return StartCommandResult.NotSticky;
        }

        public override void OnCreate()
        {
            Console.WriteLine("NotificationService - OnCreate");
            Initialize();
            base.OnCreate();
        }

        public override void OnDestroy()
        {
            Console.WriteLine("NotificationService - OnDestroy");
            base.OnDestroy();
        }

        private void Initialize()
        {
            Console.WriteLine("NotificationService - Initializing service...");
            int maxMemory = (int) (Runtime.GetRuntime().MaxMemory()/1024);
            int cacheSize = maxMemory/16;
            _bitmapCache = new BitmapCache(null, cacheSize, 200, 200);

            _messengerHub = Bootstrapper.GetContainer().Resolve<ITinyMessengerHub>();
            _playerService = Bootstrapper.GetContainer().Resolve<IPlayerService>();
            _messengerHub.Subscribe<PlayerPlaylistIndexChangedMessage>((message) => {
                Console.WriteLine("NotificationService - PlayerPlaylistIndexChangedMessage");
                if (message.Data.AudioFileStarted != null)
                {
                    _audioFile = message.Data.AudioFileStarted;
                    //UpdateNotificationView(message.Data.AudioFileStarted, null);
                    UpdateNotificationView();
                    GetAlbumArt(message.Data.AudioFileStarted);
                }
            });
            _messengerHub.Subscribe<PlayerStatusMessage>((message) => {
                Console.WriteLine("NotificationService - PlayerStatusMessage - Status=" + message.Status.ToString());
                _status = message.Status;
                UpdateNotificationView();
            });

            // Declare the service as foreground (i.e. the user is aware because of the audio)
            Console.WriteLine("NotificationService - Declaring service as foreground (API {0})...", (int) global::Android.OS.Build.VERSION.SdkInt);
            CreateNotificationView();
        }

        private Notification CreateNotificationView()
        {
            var notification = new Notification.Builder(this)
                .SetOngoing(true)
                .SetPriority((int)NotificationPriority.Max)
                .SetSmallIcon(Resource.Drawable.Icon)
                .Build();

            // Use the big notification style for Android 4.1+;
            //#if __ANDROID_16__
            if (((int) global::Android.OS.Build.VERSION.SdkInt) >= 16)
            {
                Console.WriteLine("NotificationService - Android 4.1+ detected; adding Big View style for notification");
                RemoteViews viewBigNotificationPlayer = new RemoteViews(ApplicationContext.PackageName, Resource.Layout.BigNotificationPlayer);
                notification.BigContentView = viewBigNotificationPlayer;
            }

            Console.WriteLine("NotificationService - Android 4.0.3+ detected; adding standard style for notification");
            RemoteViews viewNotificationPlayer = new RemoteViews(ApplicationContext.PackageName, Resource.Layout.NotificationPlayer);
            notification.ContentView = viewNotificationPlayer;

            _notification = notification;

            // Create intents for buttons
            Intent intentPlayPause = new Intent(this, typeof(NotificationService));
            intentPlayPause.SetAction(SessionsWidgetActions.SessionsWidgetPlayPause.ToString());
            PendingIntent pendingIntentPlayPause = PendingIntent.GetService(this, 0, intentPlayPause, PendingIntentFlags.UpdateCurrent);
            _notification.ContentView.SetOnClickPendingIntent(Resource.Id.notificationPlayer_btnPlayPause, pendingIntentPlayPause);
            _notification.BigContentView.SetOnClickPendingIntent(Resource.Id.bigNotificationPlayer_btnPlayPause, pendingIntentPlayPause);

            Intent intentPrevious = new Intent(this, typeof(NotificationService));
            intentPrevious.SetAction(SessionsWidgetActions.SessionsWidgetPrevious.ToString());
            PendingIntent pendingIntentPrevious = PendingIntent.GetService(this, 0, intentPrevious, PendingIntentFlags.UpdateCurrent);
            _notification.ContentView.SetOnClickPendingIntent(Resource.Id.notificationPlayer_btnPrevious, pendingIntentPrevious);
            _notification.BigContentView.SetOnClickPendingIntent(Resource.Id.bigNotificationPlayer_btnPrevious, pendingIntentPrevious);

            Intent intentNext = new Intent(this, typeof(NotificationService));
            intentNext.SetAction(SessionsWidgetActions.SessionsWidgetNext.ToString());
            PendingIntent pendingIntentNext = PendingIntent.GetService(this, 0, intentNext, PendingIntentFlags.UpdateCurrent);
            _notification.ContentView.SetOnClickPendingIntent(Resource.Id.notificationPlayer_btnNext, pendingIntentNext);
            _notification.BigContentView.SetOnClickPendingIntent(Resource.Id.bigNotificationPlayer_btnNext, pendingIntentNext);

            Intent intentClose = new Intent(this, typeof(NotificationService));
            intentClose.SetAction(SessionsWidgetActions.SessionsWidgetClose.ToString());
            PendingIntent pendingIntentClose = PendingIntent.GetService(this, 0, intentClose, PendingIntentFlags.UpdateCurrent);
            _notification.ContentView.SetOnClickPendingIntent(Resource.Id.notificationPlayer_btnClose, pendingIntentClose);
            _notification.BigContentView.SetOnClickPendingIntent(Resource.Id.bigNotificationPlayer_btnClose, pendingIntentClose);

            Intent notificationIntent = new Intent(this, typeof(PlayerActivity));
            //notificationIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTop | ActivityFlags.SingleTop); 
            notificationIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop); 
            PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, 0);
            _notification.ContentIntent = pendingIntent;            
            StartForeground(1, _notification);

            return notification;
        }

        private void UpdateNotificationView()
        {
            if (_isShutDowning)
                return;

            Console.WriteLine("NotificationService - UpdateNotificationView");
            var viewNotification = _notification.ContentView;
            var viewBigNotification = _notification.BigContentView;

            // Update metadata on notification bar 
            if (_audioFile != null)
            {
                if (viewNotification != null)
                {
                    viewNotification.SetTextViewText(Resource.Id.notificationPlayer_lblTitle, _audioFile.ArtistName + " / " + _audioFile.AlbumTitle);
                    viewNotification.SetTextViewText(Resource.Id.notificationPlayer_lblSubtitle, _audioFile.Title);
                }

                if (viewBigNotification != null)
                {
                    viewBigNotification.SetTextViewText(Resource.Id.bigNotificationPlayer_lblArtistName, _audioFile.ArtistName);
                    viewBigNotification.SetTextViewText(Resource.Id.bigNotificationPlayer_lblAlbumTitle, _audioFile.AlbumTitle);
                    viewBigNotification.SetTextViewText(Resource.Id.bigNotificationPlayer_lblSongTitle, _audioFile.Title);
                }
            }
            else
            {
                if (viewNotification != null)
                {
                    viewNotification.SetTextViewText(Resource.Id.notificationPlayer_lblTitle, "");
                    viewNotification.SetTextViewText(Resource.Id.notificationPlayer_lblSubtitle, "");
                }

                if (viewBigNotification != null)
                {
                    viewBigNotification.SetTextViewText(Resource.Id.bigNotificationPlayer_lblArtistName, "");
                    viewBigNotification.SetTextViewText(Resource.Id.bigNotificationPlayer_lblAlbumTitle, "");
                    viewBigNotification.SetTextViewText(Resource.Id.bigNotificationPlayer_lblSongTitle, "");
                }
            }

            if (_status == PlayerStatusType.Initialized ||
                _status == PlayerStatusType.Paused ||
                _status == PlayerStatusType.Stopped)
            {
                if (viewNotification != null)
                    viewNotification.SetImageViewResource(Resource.Id.notificationPlayer_btnPlayPause, Resource.Drawable.player_play);
                if (viewBigNotification != null)
                    viewBigNotification.SetImageViewResource(Resource.Id.bigNotificationPlayer_btnPlayPause, Resource.Drawable.player_play);
            }
            else
            {
                if (viewNotification != null)
                    viewNotification.SetImageViewResource(Resource.Id.notificationPlayer_btnPlayPause, Resource.Drawable.player_pause);
                if (viewBigNotification != null)
                    viewBigNotification.SetImageViewResource(Resource.Id.bigNotificationPlayer_btnPlayPause, Resource.Drawable.player_pause);
            }

            // Update album art on notification bar
            if (_bitmapAlbumArt == null && viewBigNotification != null)
                viewBigNotification.SetImageViewResource(Resource.Id.bigNotificationPlayer_imageAlbum, 0);
            else if (viewBigNotification != null)
                viewBigNotification.SetImageViewBitmap(Resource.Id.bigNotificationPlayer_imageAlbum, _bitmapAlbumArt);

            Console.WriteLine("NotificationService - UpdateNotificationView - Updating notification...");
            var notificationManager = (NotificationManager)ApplicationContext.GetSystemService(NotificationService);
            notificationManager.Notify(1, _notification);
        }

        private void GetAlbumArt(AudioFile audioFile)
        {
            _bitmapAlbumArt = null;
            Task.Factory.StartNew(() =>
            {
                //Console.WriteLine("NotificationService - GetAlbumArt - audioFile.Path: {0}", audioFile.FilePath);
                string key = audioFile.ArtistName + "_" + audioFile.AlbumTitle;
                Console.WriteLine("MobileLibraryFragment - Album art - key: {0}", key);
                if (string.IsNullOrEmpty(_previousAlbumArtKey) || _previousAlbumArtKey.ToUpper() != key.ToUpper())
                {
                    //Console.WriteLine("NotificationService - GetAlbumArt - key: {0} is different than tag {1} - Fetching album art...", key, _previousAlbumArtKey);
                    _previousAlbumArtKey = key;
                    byte[] bytesImage = AudioFile.ExtractImageByteArrayForAudioFile(audioFile.FilePath);
                    if (bytesImage.Length == 0)
                        //_imageAlbum.SetImageBitmap(null);
                    {
                        //Console.WriteLine("NotificationService - GetAlbumArt - Setting album art to NULL!");
                        UpdateNotificationView();
                    }
                    else
                    {
                        //((MainActivity)Activity).BitmapCache.LoadBitmapFromByteArray(bytesImage, key, _imageAlbum);
                        //_bitmapCache.LoadBitmapFromByteArray(bytesImage, key, _imageAlbum);
                        //Console.WriteLine("NotificationService - GetAlbumArt - Getting album art in another thread...");
                        _bitmapCache.LoadBitmapFromByteArray(bytesImage, key, bitmap => {
                            Console.WriteLine("NotificationService - GetAlbumArt - RECEIVED ALBUM ART! SETTING ALBUM ART");
                            _bitmapAlbumArt = bitmap;
                            UpdateNotificationView();
                        });
                    }
                }
            });

        }

        public override IBinder OnBind(Intent intent)
        {
            // We don't need to bind to this service
            Console.WriteLine("NotificationService - OnBind");
            return null;
        }
    }

    public enum SessionsWidgetActions
    {
        SessionsWidgetClose = 0,
        SessionsWidgetPlayPause = 1,
        SessionsWidgetPrevious = 2,
        SessionsWidgetNext = 3
    }
}