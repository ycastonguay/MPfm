using System;
using Android.App;
using Android.OS;
using MPfm.MVP.Views;

namespace MPfm.Android.Classes.Fragments.Base
{
    public class BaseFragment : Fragment, IBaseView
    {
        protected Action<IBaseView> OnViewReady { get; set; }
        public Action<IBaseView> OnViewDestroy { get; set; }
        public void ShowView(bool shown)
        {
            // Ignore on Android
        }

        public BaseFragment(Action<IBaseView> onViewReady)
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