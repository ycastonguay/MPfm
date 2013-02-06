using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using MPfm.Android.Classes.Fragments.Base;
using MPfm.MVP.Views;

namespace MPfm.Android.Classes.Fragments
{
    public class SplashFragment : BaseDialogFragment, ISplashView, View.IOnClickListener
    {        
        private View _view;

        public SplashFragment(Action<IBaseView> onViewReady) 
            : base(onViewReady)
        {
            
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.Splash, container, false);
            return _view;
        }

        public void OnClick(View v)
        {
            
        }

        #region ISplashView implementation

        public void RefreshStatus(string message)
        {
        }

        public void InitDone()
        {
        }

        #endregion

    }
}