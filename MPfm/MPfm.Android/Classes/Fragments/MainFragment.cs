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
using Android.OS;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using MPfm.Android.Classes.Adapters;
using MPfm.Android.Classes.Fragments.Base;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using MPfm.Player.Objects;

namespace MPfm.Android.Classes.Fragments
{
    public class MainFragment : Fragment
    {        
        private View _view;
        private ViewPager _viewPager;
        private List<KeyValuePair<MobileNavigationTabType, Fragment>> _fragments;
        private TabPagerAdapter _tabPagerAdapter;

        // Leave an empty constructor or the application will crash at runtime
        public MainFragment() : base()
        {
            Console.WriteLine("MainFragment - Empty constructor");
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {           
            _view = inflater.Inflate(Resource.Layout.Main, container, false);
            _fragments = new List<KeyValuePair<MobileNavigationTabType, Fragment>>();
            _viewPager = _view.FindViewById<ViewPager>(Resource.Id.main_pager);
            _tabPagerAdapter = new TabPagerAdapter(FragmentManager, _fragments, _viewPager, Activity.ActionBar);
            _viewPager.Adapter = _tabPagerAdapter;
            _viewPager.SetOnPageChangeListener(_tabPagerAdapter);

            if (savedInstanceState != null)
            {
                string state = savedInstanceState.GetString("key", "value");
                Console.WriteLine("MainFragment - OnCreateView - State is {0}", state);
            }
            else
            {
                Console.WriteLine("MainFragment - OnCreateView - State is null");

                // Android is in control of fragment recreation when back is pressed?
            }

            return _view;
        }

        public void AddTab(MobileNavigationTabType type, string title, Fragment fragment)
        {
            _fragments.Add(new KeyValuePair<MobileNavigationTabType, Fragment>(type, fragment));
            _tabPagerAdapter.NotifyDataSetChanged();
        }

        public override void OnResume()
        {
            Console.WriteLine("MainFragment - OnResume");
            base.OnResume();
        }

        public override void OnStart()
        {
            Console.WriteLine("MainFragment - OnStart");
            base.OnStart();
        }

        public override void OnStop()
        {
            Console.WriteLine("MainFragment - OnStop");
            base.OnStop();
        }

        public override void OnDestroyView()
        {
            Console.WriteLine("MainFragment - OnDestroyView");
            base.OnDestroyView();
        }

        public override void OnPause()
        {
            Console.WriteLine("MainFragment - OnPause");
            base.OnPause();
        }

        public override void OnDestroy()
        {
            Console.WriteLine("MainFragment - OnDestroy");
            base.OnDestroy();
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            Console.WriteLine("MainFragment - OnSaveInstanceState - Saving state...");
            outState.PutString("key", DateTime.Now.ToLongTimeString());
        }
    }
}
