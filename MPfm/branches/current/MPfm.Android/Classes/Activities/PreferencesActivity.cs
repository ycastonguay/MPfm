using System;
using System.Collections.Generic;
using Android.App;
using Android.Content.PM;
using Android.Support.V4.View;
using Android.Views;
using Android.OS;
using MPfm.Android.Classes.Adapters;
using MPfm.Android.Classes.Fragments;
using MPfm.Android.Classes.Objects;
using TagLib.Riff;

namespace MPfm.Android
{
    [Activity(Label = "MPfm: Preferences")]
    public class PreferencesActivity : Activity, View.IOnClickListener
    {
        private ViewPager _viewPager;
        private TabPagerAdapter _tabPagerAdapter;
        private List<Fragment> _fragments;

        protected override void OnCreate(Bundle bundle)
        {
            Console.WriteLine("PreferencesActivity - OnCreate");
            base.OnCreate(bundle);

            // Load navigation manager and other important stuff before showing splash screen
            RequestWindowFeature(WindowFeatures.ActionBar);
            SetContentView(Resource.Layout.Settings);
            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
        }

        protected override void OnStart()
        {
            Console.WriteLine("PreferencesActivity - OnStart");
            base.OnStart();

            // Create fragments
            _fragments = new List<Fragment>();
            _fragments.Add(new GeneralPreferencesFragment());
            _fragments.Add(new AudioPreferencesFragment());
            _fragments.Add(new LibraryPreferencesFragment());

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
