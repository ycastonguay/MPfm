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
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Support.V4.View;
using Android.Views;
using Android.OS;
using MPfm.Android.Classes.Adapters;
using MPfm.Android.Classes.Navigation;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;

namespace MPfm.Android
{
    [Activity(Label = "Preferences", ScreenOrientation = ScreenOrientation.Sensor, Theme = "@style/MyAppTheme", ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class PreferencesActivity : BaseActivity, IPreferencesView
    {
        private MobileNavigationManager _navigationManager; 
        private ViewPager _viewPager;
        private MainTabPagerAdapter _tabPagerAdapter;
        private List<KeyValuePair<MobileNavigationTabType, Fragment>> _fragments;        

        protected override void OnCreate(Bundle bundle)
        {
            Console.WriteLine("PreferencesActivity - OnCreate");
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Preferences);
            ActionBar.SetDisplayHomeAsUpEnabled(true);
            ActionBar.SetHomeButtonEnabled(true);

            _navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            _fragments = new List<KeyValuePair<MobileNavigationTabType, Fragment>>();
            _viewPager = FindViewById<ViewPager>(Resource.Id.preferences_pager);
            _tabPagerAdapter = new MainTabPagerAdapter(FragmentManager, _fragments, _viewPager, ActionBar);
            _viewPager.Adapter = _tabPagerAdapter;
            _viewPager.SetOnPageChangeListener(_tabPagerAdapter);

            // Since the onViewReady action could not be added to an intent, tell the NavMgr the view is ready
            ((AndroidNavigationManager) _navigationManager).SetPreferencesActivityInstance(this);
        }

        protected override void OnStart()
        {
            Console.WriteLine("PreferencesActivity - OnStart");
            base.OnStart();
        }

        public void AddSubview(IBaseView view)
        {
            Console.WriteLine("PreferencesActivity - AddSubview view: {0}", view.GetType().FullName);
            _fragments.Add(new KeyValuePair<MobileNavigationTabType, Fragment>(MobileNavigationTabType.More, (Fragment)view));

            if (_tabPagerAdapter != null)
                _tabPagerAdapter.NotifyDataSetChanged();
        }

        protected override void OnRestart()
        {
            Console.WriteLine("PreferencesActivity - OnRestart");
            base.OnRestart();
        }

        protected override void OnPause()
        {
            Console.WriteLine("PreferencesActivity - OnPause");
            base.OnPause();
        }

        protected override void OnResume()
        {
            Console.WriteLine("PreferencesActivity - OnResume");
            base.OnResume();
        }

        protected override void OnStop()
        {
            Console.WriteLine("PreferencesActivity - OnStop");
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            Console.WriteLine("PreferencesActivity - OnDestroy");
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

        #region IPreferencesView implementation

        public Action<string> OnSelectItem { get; set; }
        public void RefreshItems(List<string> items)
        {
        }

        #endregion

    }
}
