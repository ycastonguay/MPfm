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
using Android.Content;
using Android.Content.PM;
using Android.Views;
using Android.OS;
using Android.Widget;
using Sessions.Android.Classes.Navigation;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Config;
using Sessions.MVP.Navigation;
using Sessions.MVP.Views;

namespace Sessions.Android
{
    [Activity(Label = "Welcome to Sessions", ScreenOrientation = ScreenOrientation.Sensor, Theme = "@style/MyAppTheme", ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation | ConfigChanges.ScreenSize, WindowSoftInputMode = SoftInput.StateHidden)]
    public class FirstRunActivity : BaseActivity, IFirstRunView
    {
        private AndroidNavigationManager _navigationManager;
        private Button _btnClose;

        protected override void OnCreate(Bundle bundle)
        {
            Console.WriteLine("FirstRunActivity - OnCreate");
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.FirstRun);

            _btnClose = FindViewById<Button>(Resource.Id.firstRun_btnClose);
            _btnClose.Click += (sender, args) =>
            {
                AppConfigManager.Instance.Root.IsFirstRun = false;
                AppConfigManager.Instance.Save();

                var intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
                Finish();
            };

            // Since the onViewReady action could not be added to an intent, tell the NavMgr the view is ready
            _navigationManager = (AndroidNavigationManager)Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            //((AndroidNavigationManager)_navigationManager).SetFirstRunActivityInstance(this);
            _navigationManager.BindFirstRunView(this);
        }

        protected override void OnStart()
        {
            Console.WriteLine("FirstRunActivity - OnStart");
            base.OnStart();
        }

        protected override void OnRestart()
        {
            Console.WriteLine("FirstRunActivity - OnRestart");
            base.OnRestart();
        }

        protected override void OnPause()
        {
            Console.WriteLine("FirstRunActivity - OnPause");
            base.OnPause();
        }

        protected override void OnResume()
        {
            Console.WriteLine("FirstRunActivity - OnResume");
            base.OnResume();
        }

        protected override void OnStop()
        {
            Console.WriteLine("FirstRunActivity - OnStop");
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            Console.WriteLine("FirstRunActivity - OnDestroy");
            base.OnDestroy();
        }
            
        #region IFirstRunView implementation

        public Action OnCloseView { get; set; }

        public void FirstRunError(Exception ex)
        {
            base.ShowErrorDialog(ex);
        }

        #endregion
    }
}
