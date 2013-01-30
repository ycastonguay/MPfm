using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace MPfm.Android.Classes.Fragments
{
    public class GeneralPreferencesFragment : Fragment, View.IOnClickListener
    {        
        private View _view;
        private TextView _lblTitle;

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