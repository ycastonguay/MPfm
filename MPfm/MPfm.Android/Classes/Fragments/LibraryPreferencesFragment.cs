// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of MPfm.
//
// MPfm is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// MPfm is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with MPfm. If not, see <http://www.gnu.org/licenses/>.

using System;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using MPfm.Android.Classes.Fragments.Base;
using MPfm.MVP.Config.Models;
using MPfm.MVP.Views;

namespace MPfm.Android.Classes.Fragments
{
    public class LibraryPreferencesFragment : BaseFragment, ILibraryPreferencesView
    {        
        private View _view;
        private Button _btnResetLibrary;
        private Button _btnUpdateLibrary;
        private Button _btnSelectFolders;

        // Leave an empty constructor or the application will crash at runtime
        public LibraryPreferencesFragment() : base() { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.LibraryPreferences, container, false);
            _btnResetLibrary = _view.FindViewById<Button>(Resource.Id.libraryPreferences_btnResetLibrary);
            _btnUpdateLibrary = _view.FindViewById<Button>(Resource.Id.libraryPreferences_btnUpdateLibrary);
            _btnSelectFolders = _view.FindViewById<Button>(Resource.Id.libraryPreferences_btnSelectFolders);
            _btnResetLibrary.Click += BtnResetLibraryOnClick;
            _btnUpdateLibrary.Click += (sender, args) => OnUpdateLibrary();
            _btnSelectFolders.Click += (sender, args) => OnSelectFolders();
            return _view;
        }

        private void BtnResetLibraryOnClick(object sender, EventArgs eventArgs)
        {
            AlertDialog ad = new AlertDialog.Builder(Activity)
                .SetIconAttribute(global::Android.Resource.Attribute.AlertDialogIcon)
                .SetTitle("Reset Library")
                .SetMessage("Are you sure you wish to reset your library?")
                .SetCancelable(true)
                .SetPositiveButton("OK", (sender2, args) => OnResetLibrary())
                .SetNegativeButton("Cancel", (sender2, args) => { })
                .Create();
            ad.Show();
        }

        #region ILibraryPreferencesView implementation

        public Action OnResetLibrary { get; set; }
        public Action OnUpdateLibrary { get; set; }
        public Action<LibraryAppConfig> OnSetLibraryPreferences { get; set; }
        public Action OnSelectFolders { get; set; }
        public Action OnEnableSyncListener { get; set; }
        public Action<int> OnSetSyncListenerPort { get; set; }

        public void LibraryPreferencesError(Exception ex)
        {
            Activity.RunOnUiThread(() => {
                AlertDialog ad = new AlertDialog.Builder(Activity).Create();
                ad.SetCancelable(false);
                ad.SetMessage(string.Format("An error has occured in LibraryPreferences: {0}", ex));
                ad.SetButton("OK", (sender, args) => ad.Dismiss());
                ad.Show();
            }); 
        }

        public void RefreshLibraryPreferences(LibraryAppConfig config)
        {
        }

        #endregion
    }
}
