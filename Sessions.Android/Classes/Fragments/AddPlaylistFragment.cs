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
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;
using Sessions.MVP.Views;

namespace Sessions.Android.Classes.Fragments
{
    public class AddPlaylistFragment : BaseDialogFragment, IAddPlaylistView
    {
        private View _view;
        private Button _btnCancel;
        private Button _btnCreate;
        private EditText _txtName;

        public AddPlaylistFragment() : base() {}

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.SetTitle("Add new playlist");
            _view = inflater.Inflate(Resource.Layout.AddPlaylist, container, false);

            _txtName = _view.FindViewById<EditText>(Resource.Id.addPlaylist_txtName);
            _btnCancel = _view.FindViewById<Button>(Resource.Id.addPlaylist_btnCancel);
            _btnCreate = _view.FindViewById<Button>(Resource.Id.addPlaylist_btnCreate);
            _btnCancel.Click += (sender, args) => Dismiss();
            _btnCreate.Click += (sender, args) =>
            {
                OnSavePlaylist(_txtName.Text);
                Dismiss();
            };
            _btnCreate.Enabled = false;

            _txtName.TextChanged += (sender, args) => _btnCreate.Enabled = _txtName.Text.Length > 0;

            return _view;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetStyle((int)DialogFragmentStyle.Normal, (int)Resource.Style.DialogTheme);

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindAddPlaylistView(this);
        }

        #region IAddPlaylistView implementation

        public Action<string> OnSavePlaylist { get; set; }

        public void AddPlaylistError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        #endregion

    }
}
