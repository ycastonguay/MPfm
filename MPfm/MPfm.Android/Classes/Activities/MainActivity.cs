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
        private List<KeyValuePair<MobileOptionsMenuType, string>> _options;

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
                _navigationManager.BindOptionsMenuView(this);
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
            Console.WriteLine("MainActivity - PushTabView type: {0} fragment: {1} fragmentCount: {2}", type.ToString(), fragment.GetType().FullName, FragmentManager.BackStackEntryCount);
            var transaction = FragmentManager.BeginTransaction();            
            transaction.SetCustomAnimations(Resource.Animator.fade_in, Resource.Animator.fade_out, Resource.Animator.fade_in, Resource.Animator.fade_out);
            var currentFragment = FragmentManager.FindFragmentById(Resource.Id.main_fragment_container);
            if (currentFragment is MainFragment)
            {
                Console.WriteLine("MainActivity - PushTabView - Hiding main fragment...");
                transaction.Hide(currentFragment);
                transaction.Add(Resource.Id.main_fragment_container, fragment);
            }
            else
            {
                Console.WriteLine("MainActivity - PushTabView - Replacing fragment...");
                transaction.Replace(Resource.Id.main_fragment_container, fragment);
            }

            transaction.AddToBackStack(null);
            transaction.Commit();

            if (fragment is PlayerFragment)
            {
                Console.WriteLine("MainActivity - PushTabView - Showing mini player...");
                _miniPlayer.Visibility = ViewStates.Visible;
                Animation anim = AnimationUtils.LoadAnimation(this, Resource.Animation.slide_in_left);
                _miniPlayer.StartAnimation(anim);
            }
        }

        public void PushDialogView(Fragment fragment)
        {
            Console.WriteLine("MainActivity - PushDialogView fragment: {0} fragmentCount: {1}", fragment.GetType().FullName, FragmentManager.BackStackEntryCount);
            var transaction = FragmentManager.BeginTransaction();
            transaction.SetCustomAnimations(Resource.Animator.fade_in, Resource.Animator.fade_out, Resource.Animator.fade_in, Resource.Animator.fade_out);
            var currentFragment = FragmentManager.FindFragmentById(Resource.Id.main_fragment_container);            
            transaction.Hide(currentFragment);
            transaction.Add(Resource.Id.main_fragment_container, fragment);
            transaction.AddToBackStack(null);
            transaction.Commit();
        }

        public void PushDialogSubview(string parentViewTitle, IBaseView view)
        {
            Console.WriteLine("MainActivity - PushDialogSubview parentViewTitle: {0} view: {1}", parentViewTitle, view.GetType().FullName);
        }

        public void PushPlayerSubview(IPlayerView playerView, IBaseView view)
        {
            Console.WriteLine("MainActivity - PushPlayerSubview - view: {0}", view.GetType().FullName);
            var fragment = (PlayerFragment)playerView;
            fragment.AddSubview(view);
        }

        public void PushPreferencesSubview(IPreferencesView preferencesView, IBaseView view)
        {
            Console.WriteLine("MainActivity - PushPreferencesSubview - view: {0}", view.GetType().FullName);
            var activity = (PreferencesActivity)preferencesView;
            activity.AddSubview(view);
        }

        public override void OnBackPressed()
        {
            var currentFragment = FragmentManager.FindFragmentById(Resource.Id.main_fragment_container);
            Console.WriteLine("MainActivity - OnBackPressed - fragmentCount: {0} fragment: {1}", FragmentManager.BackStackEntryCount, currentFragment.GetType().FullName);

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
            Console.WriteLine("MainActivity - OnCreateOptionsMenu");

            foreach (var option in _options)
            {
                var menuItem = menu.Add(new Java.Lang.String(option.Value));
                switch (option.Key)
                {
                    case MobileOptionsMenuType.About:
                        menuItem.SetIcon(Resource.Drawable.icon_info);
                        break;
                    case MobileOptionsMenuType.EqualizerPresets:
                        menuItem.SetShowAsAction(ShowAsAction.IfRoom);
                        menuItem.SetIcon(Resource.Drawable.icon_equalizer);
                        break;
                    case MobileOptionsMenuType.Preferences:
                        menuItem.SetShowAsAction(ShowAsAction.IfRoom);
                        menuItem.SetIcon(Resource.Drawable.icon_settings);
                        break;
                    case MobileOptionsMenuType.SyncLibrary:
                        menuItem.SetShowAsAction(ShowAsAction.IfRoom);
                        menuItem.SetIcon(Resource.Drawable.icon_mobile);
                        break;
                    case MobileOptionsMenuType.SyncLibraryCloud:
                        menuItem.SetIcon(Resource.Drawable.icon_cloud);
                        break;
                    case MobileOptionsMenuType.SyncLibraryFileSharing:
                        menuItem.SetIcon(Resource.Drawable.icon_share);
                        break;
                    case MobileOptionsMenuType.SyncLibraryWebBrowser:
                        menuItem.SetIcon(Resource.Drawable.icon_earth);
                        break;
                }
            }

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem menuItem)
        {
            var option = _options.FirstOrDefault(x => x.Value == menuItem.TitleFormatted.ToString());
            OnItemClick(option.Key);
            //switch (option.Key)
            //{
            //    case MobileOptionsMenuType.About:                    
            //        break;
            //    case MobileOptionsMenuType.EqualizerPresets:
            //        break;
            //    case MobileOptionsMenuType.Preferences:
            //        Intent intent = new Intent(this, typeof(PreferencesActivity));
            //        StartActivity(intent);
            //        break;
            //    case MobileOptionsMenuType.SyncLibrary:
            //        break;
            //    case MobileOptionsMenuType.SyncLibraryCloud:
            //        break;
            //    case MobileOptionsMenuType.SyncLibraryFileSharing:
            //        ShowUpdateLibrary((UpdateLibraryFragment)_navigationManager.CreateUpdateLibraryView());
            //        break;
            //    case MobileOptionsMenuType.SyncLibraryWebBrowser:
            //        break;
            //}
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
            Console.WriteLine("MainActivity - RefreshMenu");
            _options = options;
        }

        #endregion
    }
}
