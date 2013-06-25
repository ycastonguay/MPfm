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
using MPfm.MVP.Views;

namespace MPfm.Android.Classes.Fragments
{
    public class PreferencesFragment : BaseFragment, IPreferencesView, View.IOnClickListener
    {        
        private View _view;
        private List<Fragment> _fragments;
        private ViewPager _viewPager;
        private TabPagerAdapter _tabPagerAdapter;

        // Leave an empty constructor or the application will crash at runtime
        public PreferencesFragment() : base(null)
        {
            _fragments = new List<Fragment>();
        }

        public PreferencesFragment(Action<IBaseView> onViewReady) 
            : base(onViewReady)
        {
            _fragments = new List<Fragment>();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.Preferences, container, false);
            _viewPager = _view.FindViewById<ViewPager>(Resource.Id.preferences_pager);
            _viewPager.OffscreenPageLimit = 2;
            _tabPagerAdapter = new TabPagerAdapter(FragmentManager, _fragments, _viewPager, Activity.ActionBar);
            _viewPager.Adapter = _tabPagerAdapter;
            _viewPager.SetOnPageChangeListener(_tabPagerAdapter);

            return _view;
        }

        public void AddSubview(IBaseView view)
        {
            Console.WriteLine("PreferencesFragment - AddSubview view: {0}", view.GetType().FullName);
            _fragments.Add((Fragment)view);

            if (_tabPagerAdapter != null)
                _tabPagerAdapter.NotifyDataSetChanged();
        }

        public void OnClick(View v)
        {

        }

        #region IPreferencesView implementation

        public Action<string> OnSelectItem { get; set; }

        public void RefreshItems(List<string> items)
        {
        }

        #endregion
    }
}
