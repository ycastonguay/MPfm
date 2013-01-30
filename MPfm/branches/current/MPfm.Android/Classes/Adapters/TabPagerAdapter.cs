using System;
using System.Collections.Generic;
using Android.App;
using Android.Runtime;
using Android.Support.V13.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using MPfm.Android.Classes.Fragments;
using MPfm.Android.Classes.Objects;

namespace MPfm.Android.Classes.Adapters
{
    public class TabPagerAdapter : FragmentPagerAdapter, ActionBar.ITabListener, ViewPager.IOnPageChangeListener
    {
        private readonly List<Fragment> _fragments;
        private ViewPager _viewPager;
        private ActionBar _actionBar;

        public TabPagerAdapter(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public TabPagerAdapter(FragmentManager fm, List<Fragment> fragments, ViewPager viewPager, ActionBar actionBar)
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
            _viewPager.SetCurrentItem(tab.Position, true);
        }

        public void OnTabUnselected(ActionBar.Tab tab, FragmentTransaction ft)
        {
        }

        public override int GetItemPosition(Java.Lang.Object p0)
        {
            return base.GetItemPosition(p0);
        }

        public override Fragment GetItem(int index)
        {
            return _fragments[index];
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
            _actionBar.SetSelectedNavigationItem(position);
        }
    }
}