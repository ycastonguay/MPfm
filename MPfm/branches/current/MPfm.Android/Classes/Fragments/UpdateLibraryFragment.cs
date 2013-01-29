using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace MPfm.Android.Classes.Fragments
{
    public class UpdateLibraryFragment : Fragment, View.IOnClickListener
    {        
        private View _view;
        private ProgressBar _progressBar;
        private TextView _lblSubtitle;
        private TextView _lblTitle;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.Fragment_UpdateLibrary, container, false);
            _progressBar = _view.FindViewById<ProgressBar>(Resource.Id.fragment_updateLibrary_progressBar);
            _lblTitle = _view.FindViewById<TextView>(Resource.Id.fragment_updateLibrary_lblTitle);
            _lblSubtitle = _view.FindViewById<TextView>(Resource.Id.fragment_updateLibrary_lblSubtitle);
            return _view;
        }

        public void OnClick(View v)
        {
            
        }
    }
}