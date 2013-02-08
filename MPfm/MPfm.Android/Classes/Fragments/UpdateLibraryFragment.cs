using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using MPfm.Android.Classes.Fragments.Base;
using MPfm.Library.UpdateLibrary;
using MPfm.MVP.Models;
using MPfm.MVP.Views;
using Environment = Android.OS.Environment;

namespace MPfm.Android.Classes.Fragments
{
    public class UpdateLibraryFragment : BaseDialogFragment, IUpdateLibraryView
    {        
        private View _view;
        private Button _button;
        private TextView _lblTitle;
        private TextView _lblSubtitle;
        private ProgressBar _progressBar;      

        public UpdateLibraryFragment(System.Action<IBaseView> onViewReady) 
            : base(onViewReady)
        {
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.SetTitle("Update Library");
            _view = inflater.Inflate(Resource.Layout.Fragment_UpdateLibrary, container, false);
            _button = _view.FindViewById<Button>(Resource.Id.fragment_updateLibrary_button);
            _lblTitle = _view.FindViewById<TextView>(Resource.Id.fragment_updateLibrary_lblTitle);
            _lblSubtitle = _view.FindViewById<TextView>(Resource.Id.fragment_updateLibrary_lblSubtitle);
            _progressBar = _view.FindViewById<ProgressBar>(Resource.Id.fragment_updateLibrary_progressBar);
            
            _button.Click += ButtonOnClick;

            return _view;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetStyle(DialogFragmentStyle.Normal, Resource.Style.UpdateLibraryTheme);
        }

        public override void OnStart()
        {
            base.OnStart();

            // Start update library process
            string musicPath = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryMusic).ToString();
            OnStartUpdateLibrary(UpdateLibraryMode.SpecificFolder, null, musicPath);
            //OnStartUpdateLibrary(UpdateLibraryMode.WholeLibrary, null, null);
        }

        private void ButtonOnClick(object sender, EventArgs eventArgs)
        {
            Dismiss();            
            //if (_updateLibraryDialog_button.Text == "Cancel")
            //{
            //    // TODO: Cancel update
            //}

            //RemoveUpdateLibrary();
        }

        #region IUpdateLibraryView implementation

        public Action<UpdateLibraryMode, List<string>, string> OnStartUpdateLibrary { get; set; }

        public void RefreshStatus(UpdateLibraryEntity entity)
        {
            Activity.RunOnUiThread(() =>
            {
                _lblTitle.Text = entity.Title;
                _lblSubtitle.Text = entity.Subtitle;
            });
        }

        public void AddToLog(string entry)
        {
        }

        public void ProcessEnded(bool canceled)
        {
            Activity.RunOnUiThread(() =>
            {
                _lblTitle.Text = "Update successful.";
                _lblSubtitle.Text = string.Empty;
                _button.Text = "OK";
                _progressBar.Visibility = ViewStates.Gone;
            });
        }

        #endregion
    }
}