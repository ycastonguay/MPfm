// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using Android.App;
using Android.Content.PM;
using Android.Views;
using Android.OS;
using Android.Widget;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;
using Sessions.MVP.Views;

namespace Sessions.Android
{
    [Activity(Label = "Sessions", ScreenOrientation = ScreenOrientation.Sensor, Theme = "@style/MyAppTheme", ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation | ConfigChanges.ScreenSize, WindowSoftInputMode = SoftInput.StateHidden, NoHistory = true)]
    public class SplashActivity : BaseActivity, ISplashView
    {
        private TextView _textView;

        protected override void OnCreate(Bundle bundle)
        {
            Console.WriteLine("SplashActivity - OnCreate");
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Splash);
            _textView = FindViewById<TextView>(Resource.Id.splash_text);

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindSplashView(this);
        }

        protected override void OnStart()
        {
            Console.WriteLine("SplashActivity - OnStart");
            base.OnStart();
        }

        protected override void OnRestart()
        {
            Console.WriteLine("SplashActivity - OnRestart");
            base.OnRestart();
        }

        protected override void OnPause()
        {
            Console.WriteLine("SplashActivity - OnPause");
            base.OnPause();
        }

        protected override void OnResume()
        {
            Console.WriteLine("SplashActivity - OnResume");
            base.OnResume();
        }

        protected override void OnStop()
        {
            Console.WriteLine("SplashActivity - OnStop");
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            Console.WriteLine("SplashActivity - OnDestroy");
            base.OnDestroy();
        }

        #region ISplashView implementation

        public void RefreshStatus(string message)
        {
            RunOnUiThread(() =>
            {
                _textView.Text = message;
            });
        }

        public void InitDone(bool isFirstAppStart)
        {
        }

        public void DestroyView()
        {
            Finish();
        }

        #endregion

    }
}
