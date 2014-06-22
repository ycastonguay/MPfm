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
using Android.Runtime;
using Android.Support.V13.App;
using Android.Support.V4.View;
using Sessions.MVP.Navigation;

namespace Sessions.Android.Classes.Adapters
{
    public class MainTabStatePagerAdapter : FragmentStatePagerAdapter, ActionBar.ITabListener, ViewPager.IOnPageChangeListener
    {
        readonly List<Tuple<MobileNavigationTabType, Fragment>> _fragments;
        readonly ViewPager _viewPager;
        readonly FragmentManager _fragmentManager;

        public MainTabStatePagerAdapter(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public MainTabStatePagerAdapter(FragmentManager fm, ViewPager viewPager)
            : base(fm)
        {
            _fragmentManager = fm;
            _fragments = new List<Tuple<MobileNavigationTabType, Fragment>>();
            _viewPager = viewPager;
        }

        public void SetFragment(MobileNavigationTabType tabType, Fragment fragment)
        {
            //var transaction = _fragmentManager.BeginTransaction();
            int index = _fragments.FindIndex(x => x.Item1 == tabType);
            if (index == -1)
            {
                // New fragment 
                _fragments.Add(new Tuple<MobileNavigationTabType, Fragment>(tabType, fragment));
                //transaction.Add(_viewPager.Id, fragment);
            }
            else
            {
                // Replace fragment
                //transaction.Remove(_fragments[index].Item2);
                _fragments[index] = new Tuple<MobileNavigationTabType, Fragment>(tabType, fragment);
                //transaction.Add(_viewPager.Id, fragment);
            }

            NotifyDataSetChanged();
        }

        public MobileNavigationTabType GetCurrentTab()
        {
            return _fragments[_viewPager.CurrentItem].Item1;
        }

        public void OnTabReselected(ActionBar.Tab tab, FragmentTransaction ft)
        {
        }

        public void OnTabSelected(ActionBar.Tab tab, FragmentTransaction ft)
        {
            //Console.WriteLine("ViewPagerAdapter - OnTabSelected tab: {0}", tab.Text);
            _viewPager.SetCurrentItem(tab.Position, true);
        }

        public void OnTabUnselected(ActionBar.Tab tab, FragmentTransaction ft)
        {
        }

        public override Fragment GetItem(int index)
        {
            return _fragments[index].Item2;
        }

        public override int GetItemPosition(Java.Lang.Object obj)
        {
            bool foundItem = false;
            foreach (var fragmentList in _fragments)
            {
                bool foundItemInList = fragmentList.Item2 == (Fragment)obj;
                if (foundItemInList)
                {
                    foundItem = true;
                    break;
                }
            }
            return foundItem ? PositionUnchanged : PositionNone;
        }

        public override int Count
        {
            get
            {
                return _fragments.Count;
            }
        }

        //public override Java.Lang.Object InstantiateItem(global::Android.Views.View view, int position)
        //{
        //    return base.InstantiateItem(view, position);
        //}

        //public override Java.Lang.Object InstantiateItem(global::Android.Views.ViewGroup container, int position)
        //{
        //    return base.InstantiateItem(container, position);
        //}

        //public override void DestroyItem(global::Android.Views.View view, int position, Java.Lang.Object obj)
        //{
        //    base.DestroyItem(view, position, obj);
        //}

        //public override void DestroyItem(global::Android.Views.ViewGroup container, int position, Java.Lang.Object obj)
        //{
        //    base.DestroyItem(container, position, obj);
        //}

        public void OnPageScrollStateChanged(int p0)
        {
        }

        public void OnPageScrolled(int p0, float p1, int p2)
        {
        }

        public void OnPageSelected(int position)
        {
            //Console.WriteLine("MainTabPagerAdapter - OnPageSelected position: {0}", position);
            //_actionBar.SetSelectedNavigationItem(position);
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(_fragments[position].Item1.ToString());
        }
    }
}
