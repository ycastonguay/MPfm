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
using Android.Content;
using Android.Content.PM;
using Android.Preferences;
using Android.Views;
using Android.OS;
using MPfm.Core;
using MPfm.MVP.Config;

namespace MPfm.Android
{
    [Activity(Label = "Sessions Beta", ScreenOrientation = ScreenOrientation.Sensor, Theme = "@style/MyAppTheme", ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation | ConfigChanges.ScreenSize, WindowSoftInputMode = SoftInput.StateHidden, NoHistory = true, MainLauncher = true)]
    public class LaunchActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            Console.WriteLine("LaunchActivity - OnCreate");
            base.OnCreate(bundle);

            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.Launch);

            AppConfigManager.Instance.Load();
            Console.WriteLine("LaunchActivity - OnCreate - isFirstRun: {0} resumePlayback.currentAudioFileId: {1} resumePlayback.currentPlaylistId: {2}", AppConfigManager.Instance.Root.IsFirstRun, AppConfigManager.Instance.Root.ResumePlayback.CurrentAudioFileId, AppConfigManager.Instance.Root.ResumePlayback.CurrentPlaylistId);
            if (AppConfigManager.Instance.Root.IsFirstRun)
            {
                Tracing.Log("LaunchActivity - First run of the application; launching FirstRun activity...");
                var intent = new Intent(this, typeof(FirstRunActivity));
                StartActivity(intent);
                Finish();
            }
            else if (!string.IsNullOrEmpty(AppConfigManager.Instance.Root.ResumePlayback.CurrentAudioFileId))
            {
                Tracing.Log("LaunchActivity - Resume playback is available; launching Player activity... - audioFileId: {0} playlistId: {1}", AppConfigManager.Instance.Root.ResumePlayback.CurrentAudioFileId, AppConfigManager.Instance.Root.ResumePlayback.CurrentPlaylistId);
                var intent = new Intent(this, typeof(PlayerActivity));
                StartActivity(intent);
                Finish();   
            }
            else
            {
                Tracing.Log("LaunchActivity - Resume playback is not available; launching Main activity...");
                var intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
                Finish();
            }
        }

        protected override void OnStart()
        {
            Console.WriteLine("LaunchActivity - OnStart");
            base.OnStart();
        }

        protected override void OnRestart()
        {
            Console.WriteLine("LaunchActivity - OnRestart");
            base.OnRestart();
        }

        protected override void OnPause()
        {
            Console.WriteLine("LaunchActivity - OnPause");
            base.OnPause();
        }

        protected override void OnResume()
        {
            Console.WriteLine("LaunchActivity - OnResume");
            base.OnResume();
        }

        protected override void OnStop()
        {
            Console.WriteLine("LaunchActivity - OnStop");
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            Console.WriteLine("LaunchActivity - OnDestroy");
            base.OnDestroy();
        }
    }
}
