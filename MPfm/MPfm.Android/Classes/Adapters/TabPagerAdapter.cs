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
using Android.Runtime;
using Android.Support.V13.App;
using Android.Support.V4.View;
using MPfm.MVP.Navigation;

namespace MPfm.Android.Classes.Adapters
{
    public class TabPagerAdapter : FragmentPagerAdapter, ActionBar.ITabListener, ViewPager.IOnPageChangeListener
    {
        private readonly List<KeyValuePair<MobileNavigationTabType, Fragment>> _fragments;
        private readonly ViewPager _viewPager;
        private readonly ActionBar _actionBar;

        public TabPagerAdapter(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public TabPagerAdapter(FragmentManager fm, List<KeyValuePair<MobileNavigationTabType, Fragment>> fragments, ViewPager viewPager, ActionBar actionBar)
            : base(fm)
        {
            _fragments = fragments;
            _viewPager = viewPager;
            _actionBar = actionBar;
        }

        public void OnTabReselected(ActionBar.Tab tab, FragmentTransaction ft)
        {
        }

        public void OnTabSelected(ActionBar.Tab tab, FragmentTransaction ft)
        {
            Console.WriteLine("TabPagerAdapter - OnTabSelected tab: {0}", tab.Text);
            _viewPager.SetCurrentItem(tab.Position, true);
        }

        public void OnTabUnselected(ActionBar.Tab tab, FragmentTransaction ft)
        {
        }

        public override Fragment GetItem(int index)
        {
            return _fragments[index].Value;
        }

        public override int Count
        {
            get { return _fragments.Count; }
        }

        public void OnPageScrollStateChanged(int p0)
        {
        }

        public void OnPageScrolled(int p0, float p1, int p2)
        {
        }

        public void OnPageSelected(int position)
        {
            Console.WriteLine("TabPagerAdapter - OnPageSelected position: {0}", position);
            //_actionBar.SetSelectedNavigationItem(position);
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(_fragments[position].Key.ToString());
        }
    }
}
