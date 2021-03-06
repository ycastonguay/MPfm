// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Sessions.Android.Classes.Fragments.Base;
using Sessions.Library.Objects;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;
using Sessions.MVP.Views;

namespace Sessions.Android.Classes.Fragments
{
    public class UpdateLibraryFragment : BaseDialogFragment, IUpdateLibraryView
    {        
        private View _view;
        private Button _button;
        private TextView _lblTitle;
        private TextView _lblSubtitle;
        private ProgressBar _progressBar;      

        // Leave an empty constructor or the application will crash at runtime
        public UpdateLibraryFragment() : base() { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (Dialog != null)
                Dialog.SetTitle("Update Library");

            _view = inflater.Inflate(Resource.Layout.UpdateLibrary, container, false);
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
            Cancelable = false;
            SetStyle((int)DialogFragmentStyle.Normal, (int)Resource.Style.DialogTheme);

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindUpdateLibraryView(this);
        }

        public override void OnStart()
        {
            base.OnStart();            
            OnStartUpdateLibrary();
        }

        private void ButtonOnClick(object sender, EventArgs eventArgs)
        {
            if (_button.Text == "Cancel")
            {
                OnCancelUpdateLibrary();
            }
            else
            {
                Dismiss();                
            }
        }

        #region IUpdateLibraryView implementation

        public Action<List<string>> OnAddFilesToLibrary { get; set; }
        public Action<string> OnAddFolderToLibrary { get; set; }
        public Action OnStartUpdateLibrary { get; set; }
        public Action OnCancelUpdateLibrary { get; set; }
        public Action<string> OnSaveLog { get; set; }

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

        public void ProcessStarted()
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
