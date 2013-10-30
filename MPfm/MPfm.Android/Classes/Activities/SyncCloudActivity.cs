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
using Android.Views;
using Android.OS;
using Android.Widget;
using MPfm.Android.Classes.Navigation;
using MPfm.Library.Services.Interfaces;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.Sound.AudioFiles;

namespace MPfm.Android
{
    [Activity(Label = "Sync (Cloud)", ScreenOrientation = ScreenOrientation.Sensor, Theme = "@style/MyAppTheme", ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class SyncCloudActivity : BaseActivity, ISyncCloudView
    {
        private MobileNavigationManager _navigationManager;
        private ICloudLibraryService _cloudLibrary;
        private TextView _lblConnected;
        private TextView _lblDataChanged;
        private TextView _lblValue;
        private Button _btnLogin;
        private Button _btnLogout;
        private Button _btnPull;
        private Button _btnPush;
        private Button _btnDelete;

        protected override void OnCreate(Bundle bundle)
        {
            Console.WriteLine("SyncCloudActivity - OnCreate");
            base.OnCreate(bundle);

            _cloudLibrary = Bootstrapper.GetContainer().Resolve<ICloudLibraryService>();
            _cloudLibrary.OnCloudDataChanged += OnCloudLibraryDataChanged;

            _navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            SetContentView(Resource.Layout.SyncCloud);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetHomeButtonEnabled(true);

            _lblConnected = FindViewById<TextView>(Resource.Id.syncCloud_lblConnected);
            _lblDataChanged = FindViewById<TextView>(Resource.Id.syncCloud_lblDataChanged);
            _lblValue = FindViewById<TextView>(Resource.Id.syncCloud_lblValue);
            _btnLogin = FindViewById<Button>(Resource.Id.syncCloud_btnLogin);
            _btnLogout = FindViewById<Button>(Resource.Id.syncCloud_btnLogout);
            _btnPull = FindViewById<Button>(Resource.Id.syncCloud_btnPull);
            _btnPush = FindViewById<Button>(Resource.Id.syncCloud_btnPush);
            _btnDelete = FindViewById<Button>(Resource.Id.syncCloud_btnDelete);
            _btnLogin.Click += BtnLoginOnClick;
            _btnLogout.Click += BtnLogoutOnClick;
            _btnPull.Click += BtnPullOnClick;
            _btnPush.Click += BtnPushOnClick;
            _btnDelete.Click += BtnDeleteOnClick;
            
            //_lblConnected.Text = string.Format("Is Linked: {0} {1}", _accountManager.HasLinkedAccount, DateTime.Now.ToLongTimeString());
            _lblConnected.Text = string.Format("Is Linked: {0} {1}", _cloudLibrary.HasLinkedAccount, DateTime.Now.ToLongTimeString());

            // Since the onViewReady action could not be added to an intent, tell the NavMgr the view is ready
            //((AndroidNavigationManager)_navigationManager).SetSyncCloudActivityInstance(this);
            _navigationManager.BindSyncCloudView(this);
        }

        private void BtnLoginOnClick(object sender, EventArgs e)
        {
            try
            {
                _cloudLibrary.LinkApp(this);
                _lblConnected.Text = string.Format("Is Linked: {0} {1}", _cloudLibrary.HasLinkedAccount, DateTime.Now.ToLongTimeString());
            }
            catch (Exception ex)
            {
                AlertDialog ad = new AlertDialog.Builder(this).Create();
                ad.SetCancelable(false);
                ad.SetMessage(string.Format("An error has occured in SyncCloud: {0}", ex));
                ad.SetButton("OK", (sender2, args2) => ad.Dismiss());
                ad.Show();
            }
        }

        private void BtnLogoutOnClick(object sender, EventArgs e)
        {
            try
            {
                _cloudLibrary.UnlinkApp();
                _lblConnected.Text = string.Format("Is Linked: {0} {1}", _cloudLibrary.HasLinkedAccount, DateTime.Now.ToLongTimeString());
            }
            catch (Exception ex)
            {
                AlertDialog ad = new AlertDialog.Builder(this).Create();
                ad.SetCancelable(false);
                ad.SetMessage(string.Format("An error has occured in SyncCloud: {0}", ex));
                ad.SetButton("OK", (sender2, args2) => ad.Dismiss());
                ad.Show();
            }
        }

        private void BtnPullOnClick(object sender, EventArgs e)
        {            
            try
            {
                string nowPlaying = _cloudLibrary.PullStuff();
                _lblValue.Text = nowPlaying;
            }
            catch (Exception ex)
            {
                AlertDialog ad = new AlertDialog.Builder(this).Create();
                ad.SetCancelable(false);
                ad.SetMessage(string.Format("An error has occured in SyncCloud: {0}", ex));
                ad.SetButton("OK", (sender2, args2) => ad.Dismiss());
                ad.Show();
            }
        }

        private void BtnPushOnClick(object sender, EventArgs e)
        {
            try
            {
                _cloudLibrary.PushStuff();
                _cloudLibrary.PushPlaylist(null);
            }
            catch (Exception ex)
            {
                AlertDialog ad = new AlertDialog.Builder(this).Create();
                ad.SetCancelable(false);
                ad.SetMessage(string.Format("An error has occured in SyncCloud: {0}", ex));
                ad.SetButton("OK", (sender2, args2) => ad.Dismiss());
                ad.Show();
            }
        }

        private void BtnDeleteOnClick(object sender, EventArgs e)
        {
            try
            {
                _cloudLibrary.DeleteStuff();
                _cloudLibrary.DeleteNowPlaying();
            }
            catch (Exception ex)
            {
                AlertDialog ad = new AlertDialog.Builder(this).Create();
                ad.SetCancelable(false);
                ad.SetMessage(string.Format("An error has occured in SyncCloud: {0}", ex));
                ad.SetButton("OK", (sender2, args2) => ad.Dismiss());
                ad.Show();
            }
        }

        private void OnCloudLibraryDataChanged(string data)
        {
            RunOnUiThread(() =>
            {
                _lblValue.Text = data;
            });
        }

        protected override void OnStart()
        {
            Console.WriteLine("SyncCloudActivity - OnStart");
            base.OnStart();
        }

        protected override void OnRestart()
        {
            Console.WriteLine("SyncCloudActivity - OnRestart");
            base.OnRestart();
        }

        protected override void OnPause()
        {
            Console.WriteLine("SyncCloudActivity - OnPause");
            base.OnPause();
        }

        protected override void OnResume()
        {
            Console.WriteLine("SyncCloudActivity - OnResume");
            base.OnResume();
        }

        protected override void OnStop()
        {
            Console.WriteLine("SyncCloudActivity - OnStop");
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            Console.WriteLine("SyncCloudActivity - OnDestroy");
            base.OnDestroy();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case global::Android.Resource.Id.Home:
                    var intent = new Intent(this, typeof (MainActivity));
                    intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
                    this.StartActivity(intent);
                    this.Finish();
                    return true;
                    break;
                default:
                    return base.OnOptionsItemSelected(item);
                    break;
            }
        }

        #region ISyncCloudView implementation

        public void SyncCloudError(Exception ex)
        {
            RunOnUiThread(() =>
            {
                AlertDialog ad = new AlertDialog.Builder(this).Create();
                ad.SetCancelable(false);
                ad.SetMessage(string.Format("An error has occured in Sync: {0}", ex));
                ad.SetButton("OK", (sender, args) => ad.Dismiss());
                ad.Show();
            });
        }

        #endregion

    }
}
