using System;
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
        private ViewPager _viewPager;
        private ActionBar _actionBar;

        public TabPagerAdapter(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public TabPagerAdapter(FragmentManager fm, ViewPager viewPager, ActionBar actionBar)
            : base(fm)
        {
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

        public override Fragment GetItem(int p0)
        {
            return new GenericListFragment();
        }

        public override int Count
        {
            get { return 5; }
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