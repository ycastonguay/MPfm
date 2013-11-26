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
using MPfm.Android.Classes.Navigation;
using MPfm.Library.Objects;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Models;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.Player.Objects;

namespace MPfm.Android
{
    [Activity(Label = "Resume Playback", ScreenOrientation = ScreenOrientation.Sensor, Theme = "@style/MyAppTheme", ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class ResumePlaybackActivity : BaseActivity, IResumePlaybackView
    {
        private MobileNavigationManager _navigationManager;
        private ListView _listView;
        private ResumePlaybackListAdapter _listAdapter;
        private List<ResumePlaybackEntity> _devices;

        protected override void OnCreate(Bundle bundle)
        {
            Console.WriteLine("ResumePlaybackActivity - OnCreate");
            base.OnCreate(bundle);

            _navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            SetContentView(Resource.Layout.ResumePlayback);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetHomeButtonEnabled(true);

            _listView = FindViewById<ListView>(Resource.Id.resumePlayback_listView);
            _listAdapter = new ResumePlaybackListAdapter(this, new List<ResumePlaybackEntity>());
            _listView.SetAdapter(_listAdapter);
            _listView.ItemClick += ListViewOnItemClick;

            // Since the onViewReady action could not be added to an intent, tell the NavMgr the view is ready
            //((AndroidNavigationManager)_navigationManager).SetResumePlaybackActivityInstance(this);
            _navigationManager.BindResumePlaybackView(this);
        }

        private void ListViewOnItemClick(object sender, AdapterView.ItemClickEventArgs itemClickEventArgs)
        {
            OnResumePlayback(_devices[itemClickEventArgs.Position]);
        }

        protected override void OnStart()
        {
            Console.WriteLine("ResumePlaybackActivity - OnStart");
            base.OnStart();
        }

        protected override void OnRestart()
        {
            Console.WriteLine("ResumePlaybackActivity - OnRestart");
            base.OnRestart();
        }

        protected override void OnPause()
        {
            Console.WriteLine("ResumePlaybackActivity - OnPause");
            base.OnPause();
            OnViewHidden();
        }

        protected override void OnResume()
        {
            Console.WriteLine("ResumePlaybackActivity - OnResume");
            base.OnResume();
            OnViewAppeared();
        }

        protected override void OnStop()
        {
            Console.WriteLine("ResumePlaybackActivity - OnStop");
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            Console.WriteLine("ResumePlaybackActivity - OnDestroy");
            base.OnDestroy();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case global::Android.Resource.Id.Home:
                    var intent = new Intent(this, typeof(MainActivity));
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

        #region IResumePlaybackView implementation

        public Action<ResumePlaybackEntity> OnResumePlayback { get; set; }
        public Action OnOpenPreferencesView { get; set; }
        public Action OnCheckCloudLoginStatus { get; set; }
        public Action OnViewAppeared { get; set; }
        public Action OnViewHidden { get; set; }

        public void ResumePlaybackError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void AudioFilesNotFoundError(string title, string message)
        {
        }

        public void RefreshAppLinkedStatus(bool isAppLinked)
        {
        }

        public void RefreshDevices(IEnumerable<ResumePlaybackEntity> devices)
        {
            RunOnUiThread(() =>
            {
                _devices = devices.ToList();
                _listAdapter.SetData(_devices);
            });
        }
        
        #endregion

    }
}
