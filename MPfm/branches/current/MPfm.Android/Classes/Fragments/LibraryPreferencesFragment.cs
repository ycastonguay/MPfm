using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using MPfm.Android.Classes.Fragments.Base;
using MPfm.MVP.Views;

namespace MPfm.Android.Classes.Fragments
{
    public class LibraryPreferencesFragment : BaseFragment, ILibraryPreferencesView, View.IOnClickListener
    {        
        private View _view;
        private TextView _lblTitle;

        public LibraryPreferencesFragment(Action<IBaseView> onViewReady) 
            : base(onViewReady)
        {
            
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.Fragment_LibraryPreferences, container, false);
            _lblTitle = _view.FindViewById<TextView>(Resource.Id.fragment_librarySettings_lblTitle);
            return _view;
        }

        public void OnClick(View v)
        {
            
        }
    }
}