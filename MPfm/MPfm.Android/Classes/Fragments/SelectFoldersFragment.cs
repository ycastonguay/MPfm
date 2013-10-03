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
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MPfm.Android.Classes.Adapters;
using MPfm.Android.Classes.Fragments.Base;
using MPfm.Library.Objects;
using MPfm.MVP.Models;
using MPfm.MVP.Presenters;
using MPfm.MVP.Views;
using MPfm.Sound.Playlists;

namespace MPfm.Android.Classes.Fragments
{
    public class SelectFoldersFragment : BaseDialogFragment, ISelectFoldersView
    {
        private List<Folder> _folders;
        private FolderListAdapter _listAdapter;
        private View _view;
        private ListView _listView;
        private Button _btnCancel;
        private Button _btnOK;
        private int _selectedIndex;

        public SelectFoldersFragment() : base(null)
        {
        }

        public SelectFoldersFragment(Action<IBaseView> onViewReady) 
            : base(onViewReady)
        {
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.SetTitle("Select folders to scan");
            _view = inflater.Inflate(Resource.Layout.SelectFolders, container, false);

            _listView = _view.FindViewById<ListView>(Resource.Id.selectFolders_listView);
            _btnCancel = _view.FindViewById<Button>(Resource.Id.selectFolders_btnCancel);
            _btnOK = _view.FindViewById<Button>(Resource.Id.selectFolders_btnOK);
            _btnOK.Enabled = false;
            _btnCancel.Click += (sender, args) => Dismiss();
            _btnOK.Click += (sender, args) =>
            {
                OnSaveFolders();
                Dismiss();
            };

            _folders = new List<Folder>();
            _listAdapter = new FolderListAdapter(Activity, _listView, _folders);
            _listView.SetAdapter(_listAdapter);
            _listView.ItemClick += ListViewOnItemClick;

            return _view;
        }

        private void ListViewOnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _btnOK.Enabled = true;
            _selectedIndex = e.Position;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetStyle((int)DialogFragmentStyle.Normal, (int)Resource.Style.DialogTheme);            
        }

        #region ISelectFoldersView implementation

        public Action OnSaveFolders { get; set; }
        public Action<Folder> OnSelectFolder { get; set; }

        public void SelectFoldersError(Exception ex)
        {
            Activity.RunOnUiThread(() =>
            {
                AlertDialog ad = new AlertDialog.Builder(Activity).Create();
                ad.SetCancelable(false);
                ad.SetMessage(string.Format("An error has occured in SelectFolders: {0}", ex));
                ad.SetButton("OK", (sender, args) => ad.Dismiss());
                ad.Show();
            });

        }

        public void RefreshFolders(List<Folder> folders)
        {
            Activity.RunOnUiThread(() =>
            {
                _folders = folders.ToList();
                _listAdapter.SetData(_folders);
            });
        }

        #endregion
    }
}
