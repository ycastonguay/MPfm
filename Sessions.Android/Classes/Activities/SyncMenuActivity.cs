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
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Views;
using Android.OS;
using Android.Widget;
using Sessions.Android.Classes.Adapters;
using Newtonsoft.Json;
using Sessions.Library.Objects;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Models;
using Sessions.MVP.Navigation;
using Sessions.MVP.Views;
using Sessions.Sound.AudioFiles;

namespace Sessions.Android
{
    [Activity(Label = "Sync Menu", ScreenOrientation = ScreenOrientation.Sensor, Theme = "@style/MyAppTheme", ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class SyncMenuActivity : BaseActivity, ISyncMenuView
    {
        private SyncDevice _device;
        private LinearLayout _loadingLayout;
        private LinearLayout _mainLayout;
        private TextView _lblStatus;
        private TextView _lblTotal;
        private TextView _lblFreeSpace;
        private Button _btnSelectAll;
        private ListView _listView;
        private SyncMenuListAdapter _listAdapter;
        private List<SyncMenuItemEntity> _items;

        protected override void OnCreate(Bundle bundle)
        {
            Console.WriteLine("SyncMenuActivity - OnCreate");
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.SyncMenu);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetHomeButtonEnabled(true);

            _loadingLayout = FindViewById<LinearLayout>(Resource.Id.syncMenu_loadingLayout);
            _mainLayout = FindViewById<LinearLayout>(Resource.Id.syncMenu_mainLayout);
            _lblStatus = FindViewById<TextView>(Resource.Id.syncMenu_lblStatus);
            _lblTotal = FindViewById<TextView>(Resource.Id.syncMenu_lblTotal);
            _lblFreeSpace = FindViewById<TextView>(Resource.Id.syncMenu_lblFreeSpace);
            _btnSelectAll = FindViewById<Button>(Resource.Id.syncMenu_btnSelectAll);
            _btnSelectAll.Click += (sender, args) => OnSelectButtonClick();

            _listView = FindViewById<ListView>(Resource.Id.syncMenu_listView);
            _listAdapter = new SyncMenuListAdapter(this, new List<SyncMenuItemEntity>());
            _listView.SetAdapter(_listAdapter);
            _listView.ItemClick += ListViewOnItemClick;
            _listView.ItemLongClick += ListViewOnItemLongClick;

            string json = Intent.GetStringExtra("device");
            _device = JsonConvert.DeserializeObject<SyncDevice>(json);

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindSyncMenuView(this, _device);
        }

        private void ListViewOnItemClick(object sender, AdapterView.ItemClickEventArgs itemClickEventArgs)
        {
            OnExpandItem(_items[itemClickEventArgs.Position], null);
        }

        private void ListViewOnItemLongClick(object sender, AdapterView.ItemLongClickEventArgs itemLongClickEventArgs)
        {
            OnSelectItems(new List<SyncMenuItemEntity>(){ _items[itemLongClickEventArgs.Position] });
        }

        protected override void OnStart()
        {
            Console.WriteLine("SyncMenuActivity - OnStart");
            base.OnStart();
        }

        protected override void OnRestart()
        {
            Console.WriteLine("SyncMenuActivity - OnRestart");
            base.OnRestart();
        }

        protected override void OnPause()
        {
            Console.WriteLine("SyncMenuActivity - OnPause");
            base.OnPause();
        }

        protected override void OnResume()
        {
            Console.WriteLine("SyncMenuActivity - OnResume");
            base.OnResume();
        }

        protected override void OnStop()
        {
            Console.WriteLine("SyncMenuActivity - OnStop");
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            Console.WriteLine("SyncMenuActivity - OnDestroy");
            base.OnDestroy();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.syncmenu_menu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case global::Android.Resource.Id.Home:
                    var intent = new Intent(this, typeof (SyncActivity));
                    intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
                    this.StartActivity(intent);
                    this.Finish();
                    return true;
                    break;
                case Resource.Id.syncMenuMenu_item_sync:
                    Console.WriteLine("SyncMenu - Menu item click - Syncing library...");
                    OnSync();
                    return true;
                    break;
                default:
                    return base.OnOptionsItemSelected(item);
                    break;
            }
        }

        #region ISyncMenuView implementation

        public Action<SyncMenuItemEntity, object> OnExpandItem { get; set; }
        public Action<List<SyncMenuItemEntity>> OnSelectItems { get; set; }
        public Action<List<AudioFile>> OnRemoveItems { get; set; }
        public Action OnSync { get; set; }
        public Action OnSelectButtonClick { get; set; }
        public Action OnSelectAll { get; set; }
        public Action OnRemoveAll { get; set; }

        public void SyncMenuError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void SyncEmptyError(Exception ex)
        {
            RunOnUiThread(() => {
                AlertDialog ad = new AlertDialog.Builder(this).Create();
                ad.SetCancelable(false);
                ad.SetTitle("Error");
                ad.SetMessage(ex.Message);
                ad.SetButton("OK", (sender, args) => ad.Dismiss());
                ad.Show();
            });
        }

        public void RefreshDevice(SyncDevice device)
        {
            RunOnUiThread(() => {
                ActionBar.Title = device.Name;
            });
        }

        public void RefreshLoading(bool isLoading, int progressPercentage)
        {
            RunOnUiThread(() => {
                if (isLoading)
                {
                    _loadingLayout.Visibility = ViewStates.Visible;
                    _mainLayout.Visibility = ViewStates.Gone;
                    _lblStatus.Text = progressPercentage < 100 ? String.Format("Loading index ({0}%)...", progressPercentage) : "Processing index...";
                }
                else
                {
                    _loadingLayout.Visibility = ViewStates.Gone;
                    _mainLayout.Visibility = ViewStates.Visible;                    
                }
            });
        }

        public void RefreshSelectButton(string text)
        {
            RunOnUiThread(() => {
                _btnSelectAll.Text = text;
            });
        }

        public void RefreshItems(List<SyncMenuItemEntity> items)
        {
            RunOnUiThread(() => {
                Console.WriteLine("SyncMenuActivity - RefreshItems - count: {0}", items.Count);
                _items = items;
                _listAdapter.SetData(items);
            });
        }

        public void RefreshSelection(List<AudioFile> audioFiles)
        {
        }

        public void RefreshSyncTotal(string title, string subtitle, bool enoughFreeSpace)
        {
            RunOnUiThread(() => {
                _lblTotal.Text = title;
                _lblFreeSpace.Text = subtitle;
                _lblFreeSpace.SetTextColor(enoughFreeSpace ? Color.White : Color.Red);
            });
        }

        public void InsertItems(int index, SyncMenuItemEntity parentItem, List<SyncMenuItemEntity> items, object userData)
        {
            RunOnUiThread(() =>
            {
                _items.InsertRange(index, items);
                _listAdapter.SetData(_items);
            });
        }

        public void RemoveItems(int index, int count, object userData)
        {
            RunOnUiThread(() =>
            {
                _items.RemoveRange(index, count);
                _listAdapter.SetData(_items);
            });
        }

        #endregion
    }
}
