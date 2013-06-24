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
    [Activity(MainLauncher = true, ScreenOrientation = ScreenOrientation.Sensor, Theme = "@style/MyAppTheme", ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class MainActivity : BaseActivity, IMobileOptionsMenuView
    {
        private AndroidNavigationManager _navigationManager;
        private SplashFragment _splashFragment;
        private MainFragment _mainFragment;
        private LinearLayout _miniPlayer;

        protected override void OnCreate(Bundle bundle)
        {
            Console.WriteLine("MainActivity - OnCreate");
            base.OnCreate(bundle);

            //string externalDir = global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            //string internalDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

            RequestWindowFeature(WindowFeatures.ActionBar);
            SetContentView(Resource.Layout.MainActivity);

            _miniPlayer = FindViewById<LinearLayout>(Resource.Id.main_miniplayer);
            _miniPlayer.Visibility = ViewStates.Gone;

            if (bundle == null || !bundle.GetBoolean("applicationStarted"))
            {
                Console.WriteLine("MainActivity - OnCreate - Creating main fragment...");
                _mainFragment = new MainFragment();
                var transaction = FragmentManager.BeginTransaction();
                transaction.Add(Resource.Id.main_fragment_container, _mainFragment);
                transaction.Commit();

                Console.WriteLine("MainActivity - OnCreate - Starting navigation manager...");
                _navigationManager = (AndroidNavigationManager) Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
                _navigationManager.MainActivity = this;
                _navigationManager.Start();
            }
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            Console.WriteLine("MainActivity - OnSaveInstanceState - Saving state...");
            outState.PutBoolean("applicationStarted", true);
        }

        public override void OnConfigurationChanged(global::Android.Content.Res.Configuration newConfig)
        {
            Console.WriteLine("MainActivity - OnConfigurationChanged - newConfig: {0}", newConfig.Orientation.ToString());
            base.OnConfigurationChanged(newConfig);
        }

        public void AddTab(MobileNavigationTabType type, string title, Fragment fragment)
        {
            Console.WriteLine("MainActivity - OnCreate - Adding tab {0}", title);
            _mainFragment.AddTab(type, title, fragment);
        }

        public void PushTabView(MobileNavigationTabType type, Fragment fragment)
        {
            Console.WriteLine("MainActivity - PushTabView type: {0}", type.ToString());
            if (fragment is PlayerFragment)
            {
                _miniPlayer.Visibility = ViewStates.Visible;
                Animation anim = AnimationUtils.LoadAnimation(this, Resource.Animation.slide_in_left);
                _miniPlayer.StartAnimation(anim);

                var transaction = FragmentManager.BeginTransaction();
                var currentFragment = FragmentManager.FindFragmentById(Resource.Id.main_fragment_container);
                if (currentFragment is MainFragment)
                {
                    transaction.Hide(currentFragment);
                    transaction.Add(Resource.Id.main_fragment_container, fragment);
                }
                else
                {
                    transaction.Replace(Resource.Id.main_fragment_container, fragment);
                }

                transaction.AddToBackStack(null);
                transaction.Commit();
            }
        }

        public void PushDialogView(Fragment fragment)
        {
            
        }

        public void PushDialogSubview(string parentViewTitle, IBaseView view)
        {

        }

        public void PushPlayerSubview(IPlayerView playerView, IBaseView view)
        {
            Console.WriteLine("MainActivity - PushPlayerSubview - view: {0}", view.GetType().FullName);
            PlayerFragment fragment = (PlayerFragment)playerView;
            fragment.AddSubview(view);
        }

        public override void OnBackPressed()
        {
            Console.WriteLine("MainActivity - OnBackPressed");
            var currentFragment = FragmentManager.FindFragmentById(Resource.Id.main_fragment_container);
            Console.WriteLine("OnBackPressed - fragment: {0}", currentFragment.GetType().FullName);

            if (FragmentManager.BackStackEntryCount > 0)
            {
                Animation anim = AnimationUtils.LoadAnimation(this, Resource.Animation.slide_out_right);
                anim.AnimationEnd += (sender, args) => {
                    _miniPlayer.Visibility = ViewStates.Gone;
                };
                _miniPlayer.StartAnimation(anim);

                FragmentManager.PopBackStack();
            }
            else
            {
                // Go back to springboard
                base.OnBackPressed();
            }
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
            Console.WriteLine("MainActivity - ShowSplash");
            _splashFragment = fragment;
            _splashFragment.Show(FragmentManager, "Splash");
        }

        public void HideSplash()
        {
            Console.WriteLine("MainActivity - HideSplash");
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
