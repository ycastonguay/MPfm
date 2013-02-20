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
using Android.Views.Animations;
using Android.Widget;
using MPfm.Android.Classes.Adapters;
using MPfm.Android.Classes.Fragments;
using MPfm.Android.Classes.Navigation;
using MPfm.Android.Classes.Objects;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;

namespace MPfm.Android
{
    [Activity(MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/MyAppTheme")]
    public class MainActivity : BaseActivity, IMobileOptionsMenuView
    {
        private ViewPager _viewPager;
        private TabPagerAdapter _tabPagerAdapter;
        private List<KeyValuePair<MobileNavigationTabType, Fragment>> _fragments;
        private SplashFragment _splashFragment;
        private AndroidNavigationManager _navigationManager;
        private LinearLayout _miniPlayer;

        protected override void OnCreate(Bundle bundle)
        {
            Console.WriteLine("MainActivity - OnCreate");
            base.OnCreate(bundle);

            // Request features
            RequestWindowFeature(WindowFeatures.ActionBar);
            SetContentView(Resource.Layout.Main);
            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;

            // Get controls
            _viewPager = FindViewById<ViewPager>(Resource.Id.main_pager);
            _miniPlayer = FindViewById<LinearLayout>(Resource.Id.main_miniplayer);
            _miniPlayer.Visibility = ViewStates.Gone;

            // Create view pager adapter
            _fragments = new List<KeyValuePair<MobileNavigationTabType, Fragment>>();
            _tabPagerAdapter = new TabPagerAdapter(FragmentManager, _fragments, _viewPager, ActionBar);
            _viewPager.Adapter = _tabPagerAdapter;
            _viewPager.SetOnPageChangeListener(_tabPagerAdapter);

            // Bind this activity to splash and update library views
            _navigationManager = (AndroidNavigationManager)Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            _navigationManager.MainActivity = this;
            _navigationManager.Start();
        }

        public void AddTab(MobileNavigationTabType type, string title, Fragment fragment)
        {
            _fragments.Add(new KeyValuePair<MobileNavigationTabType, Fragment>(type, fragment));
            var tab = ActionBar.NewTab();
            tab.SetTabListener(_tabPagerAdapter);
            tab.SetText(title);
            ActionBar.AddTab(tab);
        }

        public void PushTabView(MobileNavigationTabType type, Fragment fragment)
        {
            // Check fragment type
            if (fragment is PlayerFragment)
            {
                // This fragment should completely hide the view pager
                //_miniPlayer.Alpha = 1;
                _miniPlayer.Visibility = ViewStates.Visible;
                Animation anim = AnimationUtils.LoadAnimation(this, Resource.Animation.slide_in_left);
                _miniPlayer.StartAnimation(anim);
                ActionBar.NavigationMode = ActionBarNavigationMode.Standard;
                _viewPager.Visibility = ViewStates.Gone;
                var transaction = FragmentManager.BeginTransaction();
                transaction.Add(Resource.Id.main_fragment_container, fragment);
                transaction.AddToBackStack(null);
                transaction.Commit();
            }
        }

        public void PushDialogView(Fragment fragment)
        {
            
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            _viewPager.Visibility = ViewStates.Visible;
            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
            Animation anim = AnimationUtils.LoadAnimation(this, Resource.Animation.slide_out_right);
            anim.AnimationEnd += (sender, args) =>
                {
                    _miniPlayer.Visibility = ViewStates.Gone;
                };
            _miniPlayer.StartAnimation(anim);
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

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.main_menu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem menuItem)
        {
            string text = menuItem.TitleFormatted.ToString();
            if (text.ToUpper() == "EFFECTS")
            {
                
            }
            else if (text.ToUpper() == "UPDATE LIBRARY")
            {
                ShowUpdateLibrary((UpdateLibraryFragment)_navigationManager.CreateUpdateLibraryView());
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

        public void ShowSplash(SplashFragment fragment)
        {
            // Display fragment in a dialog
            _splashFragment = fragment;
            _splashFragment.Show(FragmentManager, "Splash");
        }

        public void HideSplash()
        {
            _splashFragment.Dialog.Dismiss();
        }

        private void ShowUpdateLibrary(UpdateLibraryFragment fragment)
        {
            fragment.Show(FragmentManager, "UpdateLibrary");
        }

        #region IMobileOptionsMenuView implementation

        public Action<MobileOptionsMenuType> OnItemClick { get; set; }
        public void RefreshMenu(List<KeyValuePair<MobileOptionsMenuType, string>> options)
        {
            
        }

        #endregion
    }
}
