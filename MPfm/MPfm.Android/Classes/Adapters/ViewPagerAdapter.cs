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
using MPfm.Android.Classes.Fragments;
using MPfm.MVP.Navigation;

namespace MPfm.Android.Classes.Adapters
{
    public class ViewPagerAdapter : FragmentPagerAdapter, ViewPager.IOnPageChangeListener
    {
        private readonly List<Fragment> _fragments;
        private readonly ViewPager _viewPager;

        public delegate void PageChangedDelegate(int position);
        public event PageChangedDelegate OnPageChanged;

        public ViewPagerAdapter(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public ViewPagerAdapter(FragmentManager fm, List<Fragment> fragments, ViewPager viewPager)
            : base(fm)
        {
            _fragments = fragments;
            _viewPager = viewPager;
        }

        public override Fragment GetItem(int index)
        {
            return _fragments[index];
        }

        public override int Count
        {
            get
            {
                return _fragments.Count;
            }
        }

        public void OnPageScrollStateChanged(int p0)
        {
        }

        public void OnPageScrolled(int p0, float p1, int p2)
        {
        }

        public void OnPageSelected(int position)
        {
            //Console.WriteLine("ViewPagerAdapter - OnPageSelected position: {0}", position);
            //_actionBar.SetSelectedNavigationItem(position);

            if (OnPageChanged != null)
                OnPageChanged(position);
        }
    }
}
