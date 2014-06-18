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
using Android.Views;
using Android.OS;
using Android.Widget;
using MPfm.Android.Classes.Adapters;
using MPfm.Android.Classes.Fragments;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using Sessions.Library.Objects;

namespace MPfm.Android
{
    [Activity(Label = "Sync Library With Other Devices", ScreenOrientation = ScreenOrientation.Sensor, Theme = "@style/MyAppTheme", ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class SyncActivity : BaseActivity, ISyncView
    {
        private MobileNavigationManager _navigationManager;
        private TextView _lblIPAddress;
        private TextView _lblStatus;
        private Button _btnConnectManually;
        private ListView _listView;
        private SyncListAdapter _listAdapter;
        private List<SyncDevice> _devices;

        protected override void OnCreate(Bundle bundle)
        {
            Console.WriteLine("SyncActivity - OnCreate");
            base.OnCreate(bundle);

            _navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            SetContentView(Resource.Layout.Sync);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetHomeButtonEnabled(true);

            _lblIPAddress = FindViewById<TextView>(Resource.Id.sync_lblIPAddress);
            _lblStatus = FindViewById<TextView>(Resource.Id.sync_lblStatus);
            _btnConnectManually = FindViewById<Button>(Resource.Id.sync_btnConnectManually);
            _btnConnectManually.Click += BtnConnectManuallyOnClick;
            
            _listView = FindViewById<ListView>(Resource.Id.sync_listView);
            _listAdapter = new SyncListAdapter(this, new List<SyncDevice>());
            _listView.SetAdapter(_listAdapter);
            _listView.ItemClick += ListViewOnItemClick;

            // Since the onViewReady action could not be added to an intent, tell the NavMgr the view is ready
            //((AndroidNavigationManager)_navigationManager).SetSyncActivityInstance(this);
            _navigationManager.BindSyncView(this);
        }

        private void BtnConnectManuallyOnClick(object sender, EventArgs eventArgs)
        {
            SyncConnectManualFragment fragment = new SyncConnectManualFragment();
            fragment.Show(FragmentManager, "SyncManualConnect");
        }

        private void ListViewOnItemClick(object sender, AdapterView.ItemClickEventArgs itemClickEventArgs)
        {
            //OnCancelDiscovery();
            //OnConnectDevice(_devices[itemClickEventArgs.Position]);
        }

        protected override void OnStart()
        {
            Console.WriteLine("SyncActivity - OnStart");
            base.OnStart();
        }

        protected override void OnRestart()
        {
            Console.WriteLine("SyncActivity - OnRestart");
            base.OnRestart();
        }

        protected override void OnPause()
        {
            Console.WriteLine("SyncActivity - OnPause");
            base.OnPause();
        }

        protected override void OnResume()
        {
            Console.WriteLine("SyncActivity - OnResume");
            base.OnResume();
        }

        protected override void OnStop()
        {
            Console.WriteLine("SyncActivity - OnStop");
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            Console.WriteLine("SyncActivity - OnDestroy");
            base.OnDestroy();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case global::Android.Resource.Id.Home:
                    var intent = new Intent(this, typeof (MainActivity));
                    intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
                    //OnCancelDiscovery();
                    this.StartActivity(intent);
                    this.Finish();
                    return true;
                    break;
                default:
                    return base.OnOptionsItemSelected(item);
                    break;
            }
        }

        public override void OnBackPressed()
        {
            //OnCancelDiscovery();
            base.OnBackPressed();
        }

        #region ISyncView implementation

        public Action<string> OnAddDeviceFromUrl { get; set; }
        public Action<SyncDevice> OnRemoveDevice { get; set; }
        public Action<SyncDevice> OnSyncLibrary { get; set; }
        public Action<SyncDevice> OnResumePlayback { get; set; }
        public Action OnOpenAddDeviceDialog { get; set; }

        public Action<SyncDevice> OnRemotePlayPause { get; set; }
        public Action<SyncDevice> OnRemotePrevious { get; set; }
        public Action<SyncDevice> OnRemoteNext { get; set; }
        public Action<SyncDevice> OnRemoteRepeat { get; set; }
        public Action<SyncDevice> OnRemoteShuffle { get; set; }

        public void SyncError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void RefreshIPAddress(string address)
        {
            RunOnUiThread(() => {
                _lblIPAddress.Text = address;
            });
        }

        //public void RefreshDiscoveryProgress(float percentageDone, string status)
        //{
        //    RunOnUiThread(() => {
        //        _lblStatus.Text = status;
        //    });
        //}

        //public void RefreshDevices(IEnumerable<SyncDevice> devices)
        //{
        //    RunOnUiThread(() => {
        //        _devices = devices.ToList();
        //        _listAdapter.SetData(_devices);
        //    });
        //}

        public void RefreshStatus(string status)
        {
        }

        public void NotifyAddedDevice(SyncDevice device)
        {
        }

        public void NotifyRemovedDevice(SyncDevice device)
        {
        }

        public void NotifyUpdatedDevice(SyncDevice device)
        {
        }

        public void NotifyUpdatedDevices(IEnumerable<SyncDevice> devices)
        {
        }

        public void SyncDevice(SyncDevice device)
        {
        }

        #endregion

    }
}
