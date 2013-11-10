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
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Models;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;

namespace MPfm.Android.Classes.Fragments
{
    public class CloudConnectFragment : BaseDialogFragment, ICloudConnectView, View.IOnClickListener
    {        
        private View _view;

        // Leave an empty constructor or the application will crash at runtime
        public CloudConnectFragment() : base() { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.SetTitle("Connecting to Dropbox");
            _view = inflater.Inflate(Resource.Layout.CloudConnect, container, false);

            //_listView = _view.FindViewById<ListView>(Resource.Id.selectFolders_listView);
            //_layoutLoading = _view.FindViewById<LinearLayout>(Resource.Id.selectFolders_layoutLoading);
            //_btnCancel = _view.FindViewById<Button>(Resource.Id.selectFolders_btnCancel);
            //_btnOK = _view.FindViewById<Button>(Resource.Id.selectFolders_btnOK);
            //_btnOK.Enabled = false;
            //_btnCancel.Click += (sender, args) => Dismiss();
            //_btnOK.Click += (sender, args) =>
            //{
            //    OnSaveFolders();
            //    Dismiss();
            //};

            //_folders = new List<FolderEntity>();
            //_listAdapter = new FolderListAdapter(Activity, _listView, _folders);
            //_listView.SetAdapter(_listAdapter);
            //_listView.ItemClick += ListViewOnItemClick;

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindCloudConnectView(this);

            return _view;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetStyle((int)DialogFragmentStyle.Normal, (int)Resource.Style.DialogTheme);
        }

        private void NegativeButtonHandler(object sender, DialogClickEventArgs dialogClickEventArgs)
        {
            
        }

        private void PositiveButtonHandler(object sender, DialogClickEventArgs dialogClickEventArgs)
        {

        }

        public void OnClick(View v)
        {
            
        }

        #region ICloudConnectView implementation

        public Action OnCheckIfAccountIsLinked { get; set; }

        public void CloudConnectError(Exception ex)
        {
        }

        public void RefreshStatus(CloudConnectEntity entity)
        {
        }

        #endregion

    }
}
