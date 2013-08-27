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
using Android.App;
using Android.Content.PM;
using Android.Views;
using Android.OS;
using Android.Widget;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Messages;
using MPfm.MVP.Navigation;
using TinyMessenger;

namespace MPfm.Android
{
    [Activity(Label = "Sessions Lock Screen", ScreenOrientation = ScreenOrientation.Sensor, Theme = "@style/MyAppTheme", ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class LockScreenActivity : BaseActivity
    {
        private ITinyMessengerHub _messengerHub;
        private TextView _lblArtistName;
        private TextView _lblAlbumTitle;
        private TextView _lblSongTitle;

        protected override void OnCreate(Bundle bundle)
        {
            Console.WriteLine("LockScreenActivity - OnCreate");
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.LockScreen);
            _lblArtistName = FindViewById<TextView>(Resource.Id.lockScreen_lblArtistName);
            _lblAlbumTitle = FindViewById<TextView>(Resource.Id.lockScreen_lblAlbumTitle);
            _lblSongTitle = FindViewById<TextView>(Resource.Id.lockScreen_lblSongTitle);

            _messengerHub = Bootstrapper.GetContainer().Resolve<ITinyMessengerHub>();
            _messengerHub.Subscribe<PlayerPlaylistIndexChangedMessage>((message) =>
            {
                Console.WriteLine("LockScreenActivity - PlayerPlaylistIndexChangedMessage");
                if (message.Data.AudioFileStarted != null)
                {
                    if (_lblArtistName != null)
                    {
                        _lblArtistName.Text = message.Data.AudioFileStarted.ArtistName;
                        _lblAlbumTitle.Text = message.Data.AudioFileStarted.AlbumTitle;
                        _lblSongTitle.Text = message.Data.AudioFileStarted.Title;
                    }
                }
            });
            _messengerHub.Subscribe<PlayerStatusMessage>((message) =>
            {
                Console.WriteLine("LockScreenActivity - PlayerStatusMessage - Status=" + message.Status.ToString());
            });
        }

        public override void OnAttachedToWindow()
        {
            Console.WriteLine("LockScreenActivity - OnAttachedToWindow");
            var window = this.Window;
            window.AddFlags(WindowManagerFlags.ShowWhenLocked);
            //window.AddFlags(WindowManagerFlags.TurnScreenOn | WindowManagerFlags.ShowWhenLocked |
            //                WindowManagerFlags.KeepScreenOn | WindowManagerFlags.DismissKeyguard);
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
    }
}
