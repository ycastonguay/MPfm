using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using MPfm.Android.Classes.Fragments.Base;
using MPfm.MVP.Views;

namespace MPfm.Android.Classes.Fragments
{
    public class GeneralPreferencesFragment : BaseFragment, IGeneralPreferencesView, View.IOnClickListener
    {        
        private View _view;
        private TextView _lblTitle;

        public GeneralPreferencesFragment(Action<IBaseView> onViewReady) 
            : base(onViewReady)
        {
            
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.Fragment_GeneralPreferences, container, false);
            _lblTitle = _view.FindViewById<TextView>(Resource.Id.fragment_generalSettings_lblTitle);
            return _view;
        }

        public void OnClick(View v)
        {
            
        }
    }
}