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
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Sessions.Android.Classes.Adapters;
using Sessions.Android.Classes.Fragments.Base;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Models;
using Sessions.MVP.Navigation;
using Sessions.MVP.Views;

namespace Sessions.Android.Classes.Fragments
{
    public class SelectPlaylistFragment : BaseDialogFragment, ISelectPlaylistView
    {
        private readonly LibraryBrowserEntity _item;
        private List<PlaylistEntity> _playlists;
        private PlaylistListAdapter _listAdapter;
        private View _view;
        private ListView _listView;
        private Button _btnAddNewPlaylist;
        private Button _btnCancel;
        private Button _btnSelect;
        private int _selectedIndex;

        public SelectPlaylistFragment() : base()
        {
        }

        public SelectPlaylistFragment(LibraryBrowserEntity item) : base()
        {
            _item = item;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.SetTitle("Select a playlist");
            _view = inflater.Inflate(Resource.Layout.SelectPlaylist, container, false);

            _listView = _view.FindViewById<ListView>(Resource.Id.selectPlaylist_listView);
            _btnAddNewPlaylist = _view.FindViewById<Button>(Resource.Id.selectPlaylist_btnAddNewPlaylist);
            _btnCancel = _view.FindViewById<Button>(Resource.Id.selectPlaylist_btnCancel);
            _btnSelect = _view.FindViewById<Button>(Resource.Id.selectPlaylist_btnSelect);
            _btnSelect.Enabled = false;
            _btnCancel.Click += (sender, args) => Dismiss();
            _btnSelect.Click += (sender, args) =>
            {
                OnSelectPlaylist(_playlists[_selectedIndex]);
                Dismiss();
            };
            _btnAddNewPlaylist.Click += (sender, args) => OnAddNewPlaylist();

            _playlists = new List<PlaylistEntity>();
            _listAdapter = new PlaylistListAdapter(Activity, _listView, _playlists);
            _listView.SetAdapter(_listAdapter);
            _listView.ItemClick += ListViewOnItemClick;

            return _view;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetStyle((int)DialogFragmentStyle.Normal, (int)Resource.Style.DialogTheme);

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindSelectPlaylistView(this, _item);
        }

        private void ListViewOnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _btnSelect.Enabled = true;
            _selectedIndex = e.Position;
        }

        #region ISelectPlaylistView implementation

        public Action OnAddNewPlaylist { get; set; }
        public Action<PlaylistEntity> OnSelectPlaylist { get; set; }
        
        public void SelectPlaylistError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void RefreshPlaylists(List<PlaylistEntity> playlists)
        {
            Activity.RunOnUiThread(() =>
            {
                _playlists = playlists.ToList();
                _listAdapter.SetData(_playlists);
            });
        }

        #endregion

    }
}
