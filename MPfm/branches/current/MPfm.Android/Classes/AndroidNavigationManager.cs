using MPfm.Android.Classes.Fragments;
using MPfm.MVP;
using MPfm.MVP.Views;

namespace MPfm.Android.Classes
{
    public sealed class AndroidNavigationManager : NavigationManager
    {
        private static readonly AndroidNavigationManager _instance = new AndroidNavigationManager();
        public static AndroidNavigationManager Instance
        {
            get { return _instance; }
        }

        public MainActivity MainActivity { get; set; }

        public override void PushView(IBaseView context, IBaseView newView)
        {
            // If there's no context, this means this is a top-level view
            if (context == null)
            {
                if (newView is IMobileLibraryBrowserView)
                {
                    MobileLibraryBrowserFragment fragment = (MobileLibraryBrowserFragment)newView;
                    //MainActivity.AddTabItem(newView);
                    // iOS: AppDelegate.AddTabItem(newView);
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