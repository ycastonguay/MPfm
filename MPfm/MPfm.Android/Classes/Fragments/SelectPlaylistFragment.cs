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
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MPfm.Android.Classes.Adapters;
using MPfm.Android.Classes.Fragments.Base;
using MPfm.MVP.Presenters;
using MPfm.MVP.Views;

namespace MPfm.Android.Classes.Fragments
{
    public class SelectPlaylistFragment : BaseDialogFragment
    {
        private readonly int _position = 0;
        private readonly MobileLibraryBrowserFragment _parentFragment;
        private PlaylistListAdapter _listAdapter;
        private View _view;
        private ListView _listView;
        private Button _btnAddNewPlaylist;
        private Button _btnCancel;
        private Button _btnSelect;

        public SelectPlaylistFragment(MobileLibraryBrowserFragment parentFragment, int position) : base(null)
        {
            _parentFragment = parentFragment;
            _position = position;
        }

        public SelectPlaylistFragment(Action<IBaseView> onViewReady) 
            : base(onViewReady)
        {
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
            _btnSelect.Click += (sender, args) => _parentFragment.OnAddItemToPlaylist(_position);
            _btnAddNewPlaylist.Click += (sender, args) =>
            {
                var fragment = new AddNewPlaylistFragment();
                fragment.Show(FragmentManager, "AddNewPlaylistFragment");
            };

            _listAdapter = new PlaylistListAdapter(Activity, _listView,
                new List<string>()
                {
                    "Hello",
                    "World",
                    "How",
                    "Are",
                    "You",
                    "Today",
                    "I",
                    "Hope",
                    "You",
                    "Are",
                    "Having",
                    "A",
                    "Fantastic",
                    "Day"
                });
            _listView.SetAdapter(_listAdapter);
            _listView.ItemClick += ListViewOnItemClick;

            return _view;
        }

        private void ListViewOnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _btnSelect.Enabled = true;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetStyle((int)DialogFragmentStyle.Normal, (int)Resource.Style.DialogTheme);            
        }
    }
}
