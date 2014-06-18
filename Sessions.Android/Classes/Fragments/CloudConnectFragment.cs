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
using MPfm.Android.Classes.Fragments.Base;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Models;
using Sessions.MVP.Navigation;
using Sessions.MVP.Views;

namespace MPfm.Android.Classes.Fragments
{
    public class CloudConnectFragment : BaseDialogFragment, ICloudConnectView
    {
        private View _view;
        private TextView _lblStatus;
        private Button _btnCancel;
        private Button _btnOK;
        private ProgressBar _progressBar;

        // Leave an empty constructor or the application will crash at runtime
        public CloudConnectFragment() : base() { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.SetTitle("Connecting to Dropbox");
            _view = inflater.Inflate(Resource.Layout.CloudConnect, container, false);

            _progressBar = _view.FindViewById<ProgressBar>(Resource.Id.cloudConnect_progressBar);
            _lblStatus = _view.FindViewById<TextView>(Resource.Id.cloudConnect_lblStatus);
            _btnCancel = _view.FindViewById<Button>(Resource.Id.cloudConnect_btnCancel);
            _btnCancel.Click += (sender, args) => Dismiss();
            _btnOK = _view.FindViewById<Button>(Resource.Id.cloudConnect_btnOK);
            _btnOK.Click += (sender, args) => Dismiss();

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindCloudConnectView(this);

            return _view;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetStyle((int)DialogFragmentStyle.Normal, (int)Resource.Style.DialogTheme);
        }

        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            // The Dropbox dialog will return its result here
            if (resultCode == Result.Canceled)
            {
                // The user has cancelled linking the app
                Dismiss();
            }
            else if (resultCode == Result.Ok)
            {
                // The app has been linked successfully
                OnCheckIfAccountIsLinked();
            }

            base.OnActivityResult(requestCode, resultCode, data);
        }

        #region ICloudConnectView implementation

        public Action OnCheckIfAccountIsLinked { get; set; }

        public void CloudConnectError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void RefreshStatus(CloudConnectEntity entity)
        {
            Activity.RunOnUiThread(() =>
            {
                Dialog.SetTitle(string.Format("Connect to {0}", entity.CloudServiceName));
                _btnOK.Enabled = entity.IsAuthenticated;
                _progressBar.Visibility = entity.IsAuthenticated ? ViewStates.Gone : ViewStates.Visible;
                _lblStatus.Text = string.Format(entity.IsAuthenticated ? "Connected to {0} successfully!" : "Connecting to {0}...", entity.CloudServiceName);
            });
        }

        #endregion

    }
}
