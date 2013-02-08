using System;
using Android.App;
using Android.OS;
using Android.Views;
using MPfm.MVP.Views;

namespace MPfm.Android.Classes.Fragments
{
    public class BaseListFragment : ListFragment, IBaseView
    {
        protected Action<IBaseView> OnViewReady { get; set; }
        public Action<IBaseView> OnViewDestroy { get; set; }
        public void ShowView(bool shown)
        {
            // Ignore on Android
        }

        public BaseListFragment(Action<IBaseView> onViewReady)
        {
            this.OnViewReady = onViewReady;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {            
            base.OnCreate(savedInstanceState);
            OnViewReady(this);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            OnViewDestroy(this);
        }
    }
}