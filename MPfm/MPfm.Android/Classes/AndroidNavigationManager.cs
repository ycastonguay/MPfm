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

using MPfm.Android.Classes.Fragments;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;

namespace MPfm.Android.Classes
{
    public sealed class AndroidNavigationManager : MobileNavigationManager
    {
        private static readonly AndroidNavigationManager _instance = new AndroidNavigationManager();
        public static AndroidNavigationManager Instance
        {
            get { return _instance; }
        }

        public MainActivity MainActivity { get; set; }

        public override void ShowSplash(ISplashView view)
        {
            MainActivity.ShowSplashScreen((SplashFragment)view);
        }

        public override void HideSplash()
        {
            MainActivity.RemoveSplashScreen();
        }

        public override void AddTab(string title, IBaseView view)
        {
            MobileLibraryBrowserFragment fragment = (MobileLibraryBrowserFragment)view;
            MainActivity.AddTab(title, fragment);
            // iOS: AppDelegate.AddTabItem(newView);
        }

        public override void PushView(IBaseView context, IBaseView newView)
        {
            // If there's no context, this means this is a top-level view
            if (context == null)
            {
                if (newView is IMobileLibraryBrowserView)
                {
                }
                else if (newView is IAudioPreferencesView)
                {
                    
                }


                // Here would be the top views.
                // Mobile = 4x Mobile Library Browser
                // Desktop = 1x Main Window

                // Need to have Activity ref here.
                // The MainActivity is created before initializing the NavigationManager.
                //MainActivity.PushView(context); // inside main activity, add
            }

            if (context is IMobileLibraryBrowserView)
            {
                MobileLibraryBrowserFragment fragment = (MobileLibraryBrowserFragment)context;
                //fragment.PushView(newView);
            }



        }
    }
}
