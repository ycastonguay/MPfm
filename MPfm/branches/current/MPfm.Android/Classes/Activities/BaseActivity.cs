using System;
using Android.App;
using MPfm.MVP.Views;

namespace MPfm.Android
{
    [Activity(Icon = "@drawable/icon")]
    public class BaseActivity : Activity, IBaseView
    {
        public Action OnViewDestroy { get; set; }

        public void ShowView(bool shown)
        {
            // Ignore on Android
        }
    }
}

