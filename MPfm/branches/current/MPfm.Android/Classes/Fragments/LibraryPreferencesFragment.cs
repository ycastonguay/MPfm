using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace MPfm.Android.Classes.Fragments
{
    public class LibraryPreferencesFragment : Fragment, View.IOnClickListener
    {        
        private View _view;
        private TextView _lblTitle;

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