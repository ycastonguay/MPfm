// Copyright � 2011-2013 Yanick Castonguay
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
using Android.App;
using Android.Content;
using Android.OS;
using MPfm.Android.Classes.Fragments;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using TinyIoC;

namespace MPfm.Android.Classes.Navigation
{
    public sealed class AndroidNavigationManager : MobileNavigationManager
    {
        private Action<IBaseView> _onPreferencesViewReady;

        public MainActivity MainActivity { get; set; }

        public override void ShowSplash(ISplashView view)
        {
            MainActivity.ShowSplash((SplashFragment) view);
        }

        public override void HideSplash()
        {
            MainActivity.HideSplash();
        }

        public override void AddTab(MobileNavigationTabType type, string title, IBaseView view)
        {
            MainActivity.AddTab(type, title, (Fragment) view);
        }

        public override void PushTabView(MobileNavigationTabType type, IBaseView view)
        {
            MainActivity.PushTabView(type, (Fragment) view);
        }

        public override void PushDialogView(string viewTitle, IBaseView view)
        {
            MainActivity.PushDialogView((Fragment) view);
        }

        public override void PushDialogSubview(string parentViewTitle, IBaseView view)
        {
            MainActivity.PushDialogSubview(parentViewTitle, view);
        }

        public override void PushPlayerSubview(IPlayerView playerView, IBaseView view)
        {
            MainActivity.PushPlayerSubview(playerView, view);
        }

        public override void PushPreferencesSubview(IPreferencesView preferencesView, IBaseView view)
        {
            MainActivity.PushPreferencesSubview(preferencesView, view);
        }

        protected override void CreatePreferencesViewInternal(Action<IBaseView> onViewReady)
        {
            // Why is this method necessary on Android? No way to get the activity instance when starting a new activity.
            // No way to create an activity instance other than using intents. No way to pass an object (other than serializable) in intent (i.e. Action onViewReady).
            _onPreferencesViewReady = onViewReady;
            var intent = new Intent(MainActivity, typeof (PreferencesActivity));
            MainActivity.StartActivity(intent);                           
        }

        public void SetPreferencesActivityInstance(PreferencesActivity activity)
        {
            if (_onPreferencesViewReady != null)
                _onPreferencesViewReady(activity);
        }
    }
}
