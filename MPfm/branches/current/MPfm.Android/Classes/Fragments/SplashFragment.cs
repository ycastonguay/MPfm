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
        private TextView _textView;

        // Leave an empty constructor or the application will crash at runtime
        public SplashFragment() : base(null) {}

        public SplashFragment(Action<IBaseView> onViewReady) 
            : base(onViewReady)
        {
            
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.Splash, container, false);
            _textView = _view.FindViewById<TextView>(Resource.Id.splash_text);
            return _view;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetStyle(DialogFragmentStyle.NoFrame, Resource.Style.SplashTheme);
        }

        public void OnClick(View v)
        {
            
        }

        #region ISplashView implementation

        public void RefreshStatus(string message)
        {
            Activity.RunOnUiThread(() =>
            {
                _textView.Text = message;
            });
        }

        public void InitDone()
        {
            //this.Dismiss();
        }

        #endregion

    }
}