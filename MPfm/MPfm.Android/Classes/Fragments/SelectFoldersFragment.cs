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
using MPfm.Core;
using MPfm.Library.Objects;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Models;
using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters;
using MPfm.MVP.Views;
using MPfm.Sound.Playlists;

namespace MPfm.Android.Classes.Fragments
{
    public class SelectFoldersFragment : BaseDialogFragment, ISelectFoldersView
    {
        private List<FolderEntity> _folders;
        private FolderListAdapter _listAdapter;
        private View _view;
        private LinearLayout _layoutLoading;
        private ListView _listView;
        private Button _btnCancel;
        private Button _btnOK;
        private int _selectedIndex;

        public SelectFoldersFragment() : base()
        {
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.SetTitle("Select folders to scan");
            _view = inflater.Inflate(Resource.Layout.SelectFolders, container, false);

            _listView = _view.FindViewById<ListView>(Resource.Id.selectFolders_listView);
            _layoutLoading = _view.FindViewById<LinearLayout>(Resource.Id.selectFolders_layoutLoading);
            _btnCancel = _view.FindViewById<Button>(Resource.Id.selectFolders_btnCancel);
            _btnOK = _view.FindViewById<Button>(Resource.Id.selectFolders_btnOK);
            _btnOK.Enabled = false;
            _btnCancel.Click += (sender, args) => Dismiss();
            _btnOK.Click += (sender, args) =>
            {
                OnSaveFolders();
                Dismiss();
            };

            _folders = new List<FolderEntity>();
            _listAdapter = new FolderListAdapter(Activity, _listView, _folders);
            _listView.SetAdapter(_listAdapter);
            _listView.ItemClick += ListViewOnItemClick;

            return _view;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetStyle((int)DialogFragmentStyle.Normal, (int)Resource.Style.DialogTheme);

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindSelectFoldersView(this);
        }

        private void ListViewOnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _btnOK.Enabled = true;
            _selectedIndex = e.Position;
        }

        #region ISelectFoldersView implementation

        public Action OnSaveFolders { get; set; }
        public Action<FolderEntity> OnSelectFolder { get; set; }

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

        public void RefreshFolders(List<FolderEntity> folders)
        {
            Tracing.Log("SelectFoldersFragment - RefreshFolders - folders.Count: {0}", folders.Count);
            Activity.RunOnUiThread(() =>
            {
                _folders = folders.ToList();
                _listAdapter.SetData(_folders);
            });
        }

        public void RefreshLoading(bool isLoading)
        {
            Activity.RunOnUiThread(() =>
            {
                _layoutLoading.Visibility = isLoading
                    ? ViewStates.Visible
                    : ViewStates.Gone;
            });
        }

        #endregion
    }
}
