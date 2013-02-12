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
using Android.Content.PM;
using Android.Support.V4.View;
using Android.Views;
using Android.OS;
using MPfm.Android.Classes;
using MPfm.Android.Classes.Adapters;
using MPfm.Android.Classes.Fragments;
using MPfm.Android.Classes.Navigation;
using MPfm.Android.Classes.Objects;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using TagLib.Riff;

namespace MPfm.Android
{
    [Activity(Label = "MPfm: Preferences")]
    public class PreferencesActivity : BaseActivity, View.IOnClickListener
    {
        private ViewPager _viewPager;
        private TabPagerAdapter _tabPagerAdapter;
        private List<KeyValuePair<MobileNavigationTabType, Fragment>> _fragments;
        private MobileNavigationManager _navigationManager;

        protected override void OnCreate(Bundle bundle)
        {
            Console.WriteLine("PreferencesActivity - OnCreate");
            base.OnCreate(bundle);

            // Load navigation manager and other important stuff before showing splash screen
            RequestWindowFeature(WindowFeatures.ActionBar);
            SetContentView(Resource.Layout.Settings);
            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;

            // Bind view to presenter
            //AndroidNavigationManager.Instance.BindPreferencesView(this);
        }

        protected override void OnStart()
        {
            Console.WriteLine("PreferencesActivity - OnStart");
            base.OnStart();

            // Create fragments
            _navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            _fragments = new List<KeyValuePair<MobileNavigationTabType, Fragment>>();
            _fragments.Add(new KeyValuePair<MobileNavigationTabType, Fragment>(MobileNavigationTabType.PreferencesGeneral, (Fragment)_navigationManager.CreateGeneralPreferencesView()));
            _fragments.Add(new KeyValuePair<MobileNavigationTabType, Fragment>(MobileNavigationTabType.PreferencesAudio, (Fragment)_navigationManager.CreateAudioPreferencesView()));
            _fragments.Add(new KeyValuePair<MobileNavigationTabType, Fragment>(MobileNavigationTabType.PreferencesLibrary, (Fragment)_navigationManager.CreateLibraryPreferencesView()));

            // Create view pager (for lateral navigation)
            _viewPager = FindViewById<ViewPager>(Resource.Id.settings_pager);
            _tabPagerAdapter = new TabPagerAdapter(FragmentManager, _fragments, _viewPager, ActionBar);
            _viewPager.Adapter = _tabPagerAdapter;
            _viewPager.SetOnPageChangeListener(_tabPagerAdapter);

            // Create tabs
            var generalTab = ActionBar.NewTab();
            generalTab.SetTabListener(_tabPagerAdapter);
            generalTab.SetText("General");
            ActionBar.AddTab(generalTab);
            var audioTab = ActionBar.NewTab();
            audioTab.SetTabListener(_tabPagerAdapter);
            audioTab.SetText("Audio");
            ActionBar.AddTab(audioTab);
            var libraryTab = ActionBar.NewTab();
            libraryTab.SetTabListener(_tabPagerAdapter);
            libraryTab.SetText("Library");
            ActionBar.AddTab(libraryTab);
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

        public void OnClick(View v)
        {
        }

        //public override void OnBackPressed()
        //{
        //    // Close activity
        //    Finish();
        //}
    }
}
