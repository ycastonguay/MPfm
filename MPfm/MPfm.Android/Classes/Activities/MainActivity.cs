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
using MPfm.Android.Classes.Fragments;
using MPfm.Android.Classes.Navigation;
using MPfm.Android.Classes.Objects;

namespace MPfm.Android
{
    [Activity(MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/MyAppTheme")]
    public class MainActivity : BaseActivity
    {
        private ViewPager _viewPager;
        private TabPagerAdapter _tabPagerAdapter;
        private List<Fragment> _fragments;
        private SplashFragment _splashFragment;
        private UpdateLibraryFragment _updateLibraryFragment;

        protected override void OnCreate(Bundle bundle)
        {
            Console.WriteLine("MainActivity - OnCreate");
            base.OnCreate(bundle);

            // Get application state
            ApplicationState state = (ApplicationState)LastNonConfigurationInstance;
            if (state != null)
            {
                // Restore state here
            }

            // Request features
            RequestWindowFeature(WindowFeatures.ActionBar);
            SetContentView(Resource.Layout.Main);
            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;

            // Get controls
            _viewPager = FindViewById<ViewPager>(Resource.Id.main_pager);

            // Create view pager adapter
            _fragments = new List<Fragment>();
            _tabPagerAdapter = new TabPagerAdapter(FragmentManager, _fragments, _viewPager, ActionBar);
            _viewPager.Adapter = _tabPagerAdapter;
            _viewPager.SetOnPageChangeListener(_tabPagerAdapter);

            // Bind this activity to splash and update library views
            AndroidNavigationManager.Instance.MainActivity = this;
            AndroidNavigationManager.Instance.Start();
        }

        public void AddTab(string title, Fragment fragment)
        {
            _fragments.Add(fragment);
            var tab = ActionBar.NewTab();
            tab.SetTabListener(_tabPagerAdapter);
            tab.SetText(title);
            ActionBar.AddTab(tab);
        }

        protected override void OnStart()
        {
            Console.WriteLine("MainActivity - OnStart");
            base.OnStart();
        }

        protected override void OnRestart()
        {
            Console.WriteLine("MainActivity - OnRestart");
            base.OnRestart();
        }

        protected override void OnPause()
        {
            Console.WriteLine("MainActivity - OnPause");
            base.OnPause();
        }

        protected override void OnResume()
        {
            Console.WriteLine("MainActivity - OnResume");
            base.OnResume();
        }

        protected override void OnStop()
        {
            Console.WriteLine("MainActivity - OnStop");
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            Console.WriteLine("MainActivity - OnDestroy");
            base.OnDestroy();
        }

        public override Java.Lang.Object OnRetainNonConfigurationInstance()
        {
            // Save stuff here
            ApplicationState state = new ApplicationState();
            return state;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.main_menu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem menuItem)
        {
            // TODO: Determine if the menu should call the NavMgr directly, or the presenter... something like a MainMenuPresenter?
            string text = menuItem.TitleFormatted.ToString();
            if (text.ToUpper() == "EFFECTS")
            {
                
            }
            else if (text.ToUpper() == "UPDATE LIBRARY")
            {
                ShowUpdateLibrary((UpdateLibraryFragment)AndroidNavigationManager.Instance.CreateUpdateLibraryView());
            }
            else if (text.ToUpper() == "PREFERENCES")
            {
                Intent intent = new Intent(this, typeof(PreferencesActivity));
                StartActivity(intent);
            }
            else if (text.ToUpper() == "ABOUT MPFM")
            {
                //ShowSplashScreen();
                //var dialog = new DialogTest();
                //dialog.Show(FragmentManager, "tagnumber");
            }
            return base.OnOptionsItemSelected(menuItem);
        }

        public void ShowSplashScreen(SplashFragment fragment)
        {
            // Display fragment in a dialog
            _splashFragment = fragment;
            _splashFragment.Show(FragmentManager, "");
        }

        public void RemoveSplashScreen()
        {
            _splashFragment.Dialog.Dismiss();
        }

        private void ShowUpdateLibrary(UpdateLibraryFragment fragment)
        {
            _updateLibraryFragment = fragment;
            _updateLibraryFragment.Show(FragmentManager, "");
        }
    }
}
