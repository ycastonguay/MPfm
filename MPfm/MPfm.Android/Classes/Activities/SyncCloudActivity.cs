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
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Text.Format;
using Android.Views;
using Android.OS;
using Android.Widget;
using Com.Dropbox.Sync.Android;
using MPfm.Android.Classes.Adapters;
using MPfm.Android.Classes.Navigation;
using MPfm.Library;
using MPfm.Library.Objects;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.Player.Objects;

namespace MPfm.Android
{
    [Activity(Label = "Sync (Cloud)", ScreenOrientation = ScreenOrientation.Sensor, Theme = "@style/MyAppTheme", ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class SyncCloudActivity : BaseActivity, ISyncCloudView, DbxDatastore.ISyncStatusListener
    {
        private MobileNavigationManager _navigationManager;
        private ISyncDeviceSpecifications _syncDeviceSpecifications;
        private DbxAccountManager _accountManager;
        private DbxAccount _account;
        private DbxDatastore _store;
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

            // These keys will be replaced when the app is pushed to Google Play :-)
            string appKey = "6tc6565743i743n";
            string appSecret = "fbkt3neevjjl0l2";
            _accountManager = DbxAccountManager.GetInstance(ApplicationContext, appKey, appSecret);

            if (_accountManager.HasLinkedAccount)
                _account = _accountManager.LinkedAccount;

            _syncDeviceSpecifications = Bootstrapper.GetContainer().Resolve<ISyncDeviceSpecifications>();
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
            
            _lblConnected.Text = string.Format("Is Linked: {0} {1}", _accountManager.HasLinkedAccount, DateTime.Now.ToLongTimeString());

            // Since the onViewReady action could not be added to an intent, tell the NavMgr the view is ready
            ((AndroidNavigationManager)_navigationManager).SetSyncCloudActivityInstance(this);
        }        

        private void BtnLoginOnClick(object sender, EventArgs e)
        {
            try
            {
                if (_accountManager.HasLinkedAccount)
                {
                    _account = _accountManager.LinkedAccount;
                }
                else
                {
                    _accountManager.StartLink(this, 0);
                }
                _lblConnected.Text = string.Format("Is Linked: {0} {1}", _accountManager.HasLinkedAccount, DateTime.Now.ToLongTimeString());
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
                if (_accountManager.HasLinkedAccount)
                {
                    _accountManager.Unlink();
                }
                _lblConnected.Text = string.Format("Is Linked: {0} {1}", _accountManager.HasLinkedAccount, DateTime.Now.ToLongTimeString());
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
                DbxTable tableStuff = _store.GetTable("stuff");
                DbxFields queryParams = new DbxFields();
                queryParams.Set("test", true);
                queryParams.Set("hello", "world");
                DbxTable.QueryResult results = tableStuff.Query(queryParams);
                var list = results.AsList();
                if (list.Count == 0)
                {
                    _lblValue.Text = "No value!";
                    return;
                }

                DbxRecord firstResult = list[0];
                string timestamp = firstResult.GetString("timestamp");
                string deviceType = firstResult.GetString("deviceType");
                string deviceName = firstResult.GetString("deviceName");
                _lblValue.Text = string.Format("{0} {1} {2}", deviceType, deviceName, timestamp);
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
                DbxTable tableStuff = _store.GetTable("stuff");
                DbxRecord stuff = tableStuff.Insert();                
                stuff.Set("hello", "world");
                stuff.Set("deviceType", _syncDeviceSpecifications.GetDeviceType().ToString());
                stuff.Set("deviceName", _syncDeviceSpecifications.GetDeviceName());
                stuff.Set("ip", _syncDeviceSpecifications.GetIPAddress());
                stuff.Set("test", true);                
                stuff.Set("timestamp", DateTime.Now.ToLongTimeString());
                _store.Sync();
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
                DbxTable tableStuff = _store.GetTable("stuff");
                DbxTable.QueryResult results = tableStuff.Query();
                var list = results.AsList();
                foreach (var record in list)
                {
                    record.DeleteRecord();
                }
                _store.Sync();
            }
            catch (Exception ex)
            {
                AlertDialog ad = new AlertDialog.Builder(this).Create();
                ad.SetCancelable(false);
                ad.SetMessage(string.Format("An error has occured in SyncCloud: {0}", ex));
                ad.SetButton("OK", (sender2, args2) => ad.Dismiss());
                ad.Show();
            }

            //DbxDatastore store = null;
            //try
            //{
            //    store = DbxDatastore.OpenDefault(_account);                
            //}
            //catch (Exception ex)
            //{
            //}
            //finally
            //{
            //    if (store != null) store.Close();
            //}
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

            _store = DbxDatastore.OpenDefault(_account);
            _store.AddSyncStatusListener(this);
        }

        protected override void OnStop()
        {
            Console.WriteLine("SyncCloudActivity - OnStop");
            base.OnStop();

            _store.RemoveSyncStatusListener(this);
            _store.Close();
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

        public void OnDatastoreStatusChange(DbxDatastore store)
        {
            Console.WriteLine("SyncCloudActivity - OnDatastoreStatusChange - hasIncoming: {0}", store.SyncStatus.HasIncoming);
            _lblDataChanged.Text = string.Format("Data changed: {0} incoming: {1}", DateTime.Now.ToLongTimeString(), store.SyncStatus.HasIncoming);
            if (store.SyncStatus.HasIncoming)
            {
                try
                {
                    var changes = store.Sync();
                    if (!changes.ContainsKey("stuff"))
                        return;

                    var records = changes["stuff"];
                    foreach (var record in records)
                    {
                        string timestamp = record.GetString("timestamp");
                        string deviceType = record.GetString("deviceType");
                        string deviceName = record.GetString("deviceName");
                        _lblValue.Text = string.Format("{0} {1} {2}", deviceType, deviceName, timestamp);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SyncCloudActivity - OnDatastoreStatusChange exception: {0}", ex);
                    _lblValue.Text = string.Format("Error: {0}", ex);
                }
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
