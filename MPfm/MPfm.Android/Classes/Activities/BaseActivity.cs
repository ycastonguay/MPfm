using System;
using Android.App;
using MPfm.MVP.Views;

namespace MPfm.Android
{
    [Activity(Icon = "@drawable/icon")]
    public class BaseActivity : Activity, IBaseView
    {
        // Do not call this on activities, it is useless since NavMgr isn't used for activities.
        public Action<IBaseView> OnViewDestroy { get; set; }

        public void ShowView(bool shown)
        {
            // Ignore on Android
        }
    }
}

