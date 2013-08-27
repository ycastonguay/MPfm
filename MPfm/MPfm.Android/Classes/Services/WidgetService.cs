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
    //[Service(Name = "org.sessionsapp.android.WidgetService", Label = "Sessions Widget Service")]//, Process = ":.widget.process")]
    [Service(Label = "Sessions Widget Service")]//, Process = ":widgetprocess")]
    public class WidgetService : Service
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
            else if (intent.Action == SessionsWidgetActions.SessionsWidgetClose.ToString())
            {
                Console.WriteLine("WidgetService - Closing the application...");
                _isShutDowning = true;
                _playerService.Stop();
                StopForeground(true);

                var notificationManager = (NotificationManager)ApplicationContext.GetSystemService(NotificationService);
                notificationManager.Cancel(1);   

                StopSelf();

                // Nuke the application process, this will also nuke any running activities
                //Java.Lang.JavaSystem.Exit(0);
                //Process.KillProcess(Process.MyPid()); 
            }
            else if (intent.Action == "android.appwidget.action.APPWIDGET_UPDATE")
            {
                Console.WriteLine("WidgetService - Updating notification because of APPWIDGET_UPDATE...");
                UpdateNotificationView();
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
                    UpdateNotificationView();
                    GetAlbumArt(message.Data.AudioFileStarted);
                }
            });
            _messengerHub.Subscribe<PlayerStatusMessage>((message) => {
                Console.WriteLine("WidgetService - PlayerStatusMessage - Status=" + message.Status.ToString());
                _status = message.Status;
                UpdateNotificationView();
            });

            // Declare the service as foreground (i.e. the user is aware because of the audio)
            Console.WriteLine("WidgetService - Declaring service as foreground (API {0})...", (int) global::Android.OS.Build.VERSION.SdkInt);
            CreateNotificationView();
        }

        private Notification CreateNotificationView()
        {
            var notification = new Notification.Builder(this)
                .SetOngoing(true)
                .SetPriority((int)NotificationPriority.Max)
                .SetSmallIcon(Resource.Drawable.Icon)
                .Build();

            // Use the big notification style for Android 4.1+; use the standard notification style for Android 4.0.3
            //#if __ANDROID_16__
            if (((int) global::Android.OS.Build.VERSION.SdkInt) >= 16)
            {
                Console.WriteLine("WidgetService - Android 4.1+ detected; using Big View style for notification");
                RemoteViews viewBigNotificationPlayer = new RemoteViews(ApplicationContext.PackageName, Resource.Layout.BigNotificationPlayer);
                viewBigNotificationPlayer.SetTextViewText(Resource.Id.bigNotificationPlayer_lblArtistName, "Hello World!");
                notification.BigContentView = viewBigNotificationPlayer;
            }
            //#else
            else 
            {
                Console.WriteLine("WidgetService - Android 4.0.3+ (<4.1) detected; using standard style for notification");
                RemoteViews viewNotificationPlayer = new RemoteViews(ApplicationContext.PackageName, Resource.Layout.NotificationPlayer);
                viewNotificationPlayer.SetTextViewText(Resource.Id.notificationPlayer_lblTitle, "Hello World!");
                notification.ContentView = viewNotificationPlayer;
            }
            //#endif        

            _notification = notification;

            Intent intentPlayPause = new Intent(this, typeof(WidgetService));
            intentPlayPause.SetAction(SessionsWidgetActions.SessionsWidgetPlayPause.ToString());
            PendingIntent pendingIntentPlayPause = PendingIntent.GetService(this, 0, intentPlayPause, PendingIntentFlags.UpdateCurrent);
            _notification.ContentView.SetOnClickPendingIntent(Resource.Id.notificationPlayer_btnPlayPause, pendingIntentPlayPause);
            _notification.BigContentView.SetOnClickPendingIntent(Resource.Id.bigNotificationPlayer_btnPlayPause, pendingIntentPlayPause);

            Intent intentPrevious = new Intent(this, typeof(WidgetService));
            intentPrevious.SetAction(SessionsWidgetActions.SessionsWidgetPrevious.ToString());
            PendingIntent pendingIntentPrevious = PendingIntent.GetService(this, 0, intentPrevious, PendingIntentFlags.UpdateCurrent);
            _notification.ContentView.SetOnClickPendingIntent(Resource.Id.notificationPlayer_btnPrevious, pendingIntentPrevious);
            _notification.BigContentView.SetOnClickPendingIntent(Resource.Id.bigNotificationPlayer_btnPrevious, pendingIntentPrevious);

            Intent intentNext = new Intent(this, typeof(WidgetService));
            intentNext.SetAction(SessionsWidgetActions.SessionsWidgetNext.ToString());
            PendingIntent pendingIntentNext = PendingIntent.GetService(this, 0, intentNext, PendingIntentFlags.UpdateCurrent);
            _notification.ContentView.SetOnClickPendingIntent(Resource.Id.notificationPlayer_btnNext, pendingIntentNext);
            _notification.BigContentView.SetOnClickPendingIntent(Resource.Id.bigNotificationPlayer_btnNext, pendingIntentNext);

            Intent intentClose = new Intent(this, typeof(WidgetService));
            intentClose.SetAction(SessionsWidgetActions.SessionsWidgetClose.ToString());
            PendingIntent pendingIntentClose = PendingIntent.GetService(this, 0, intentClose, PendingIntentFlags.UpdateCurrent);
            _notification.ContentView.SetOnClickPendingIntent(Resource.Id.notificationPlayer_btnClose, pendingIntentClose);
            _notification.BigContentView.SetOnClickPendingIntent(Resource.Id.bigNotificationPlayer_btnClose, pendingIntentClose);

            Intent notificationIntent = new Intent(this, typeof(PlayerActivity));
            //Intent notificationIntent = new Intent(this, typeof(MainActivity));
            PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, 0);
            _notification.ContentIntent = pendingIntent;            
            StartForeground(1, _notification);

            //UpdateNotificationView();

            return notification;
        }

        private void UpdateNotificationView()
        {
            if (_isShutDowning)
                return;

            Console.WriteLine("WidgetService - UpdateNotificationView");
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

            Console.WriteLine("WidgetService - UpdateNotificationView - Updating notification...");
            var notificationManager = (NotificationManager)ApplicationContext.GetSystemService(NotificationService);
            notificationManager.Notify(1, _notification);

            try
            {
                // Update widget
                Console.WriteLine("WidgetService - UpdateNotificationView - Getting widgets (0)...");
                RemoteViews viewWidget = new RemoteViews(PackageName, Resource.Layout.WidgetPlayer);
                Console.WriteLine("WidgetService - UpdateNotificationView - Getting widgets (1)...");
                AppWidgetManager manager = AppWidgetManager.GetInstance(this);
                Console.WriteLine("WidgetService - UpdateNotificationView - Getting widgets (2) - appWidgetIds count: {0}", _widgetIds.Length);

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

                Console.WriteLine("WidgetService - UpdateNotificationView - Getting widgets (3) - appWidgetIds count: {0}", _widgetIds.Length);

                foreach (int id in _widgetIds)
                {
                    Console.WriteLine("WidgetService - UpdateNotificationView - Updating widgets - id: {0}", id);
                    manager.UpdateAppWidget(id, viewWidget);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("WidgetService - UpdateNotificationView - Widget exception: {0}", ex);
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
                        UpdateNotificationView();
                    }
                    else
                    {
                        //((MainActivity)Activity).BitmapCache.LoadBitmapFromByteArray(bytesImage, key, _imageAlbum);
                        //_bitmapCache.LoadBitmapFromByteArray(bytesImage, key, _imageAlbum);
                        //Console.WriteLine("WidgetService - GetAlbumArt - Getting album art in another thread...");
                        _bitmapCache.LoadBitmapFromByteArray(bytesImage, key, bitmap => {
                            Console.WriteLine("WidgetService - GetAlbumArt - RECEIVED ALBUM ART! SETTING ALBUM ART");
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
            Console.WriteLine("WidgetService - OnBind");
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