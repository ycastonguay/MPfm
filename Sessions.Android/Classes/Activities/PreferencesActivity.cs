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
using Android.App;
using Android.Content.PM;
using Android.Views;
using Android.OS;
using Sessions.MVP.Views;

namespace Sessions.Android
{
    [Activity(Label = "Preferences", ScreenOrientation = ScreenOrientation.Sensor, Theme = "@style/PreferencesTheme", ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class PreferencesActivity : BasePreferenceActivity, IPreferencesView
    {
        protected override void OnCreate(Bundle bundle)
        {
            Console.WriteLine("PreferencesActivity - OnCreate");
            base.OnCreate(bundle);

            ActionBar.SetDisplayHomeAsUpEnabled(true);

            // The PreferencesActivity is reused as a container for PreferenceFragment. So there is actually multiple instances of this activity.
            // Thus is it not possible to bind to a presenter (or useful!)
            //var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            //navigationManager.BindPreferencesView(this);

            ListView.SetBackgroundColor(Resources.GetColor(Resource.Color.background));
        }

        public override void OnBuildHeaders(IList<Header> target)
        {
            LoadHeadersFromResource(Resource.Xml.preferences_headers, target);
        }

        protected override void OnStart()
        {
            Console.WriteLine("PreferencesActivity - OnStart");
            base.OnStart();
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

        public void PushSubView(IBaseView view)
        {
            //Console.WriteLine("PreferencesActivity - PushSubView view: {0}", view.GetType().FullName);
            //_fragments.Add(new Tuple<MobileNavigationTabType, Fragment>(MobileNavigationTabType.More, (Fragment)view));
            //_fragments.Add((Fragment)view);

            //if (_viewPagerAdapter != null)
            //    _viewPagerAdapter.NotifyDataSetChanged();
        }        

        public void RefreshItems(List<string> items)
        {
        }

        #endregion

    }
}
