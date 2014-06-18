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
using Android.OS;
using Android.Views;
using Android.Widget;
using MPfm.Android.Classes.Fragments.Base;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;
using Sessions.Library.Objects;
using Sessions.Sound.AudioFiles;

namespace MPfm.Android.Classes.Fragments
{
    public class StartResumePlaybackFragment : BaseDialogFragment, IStartResumePlaybackView
    {
        private readonly CloudDeviceInfo _device;
        private View _view;
        private TextView _lblDeviceName;
        private TextView _lblPlaylistName;
        private TextView _lblArtistName;
        private TextView _lblAlbumTitle;
        private TextView _lblSongTitle;
        private TextView _lblTimestamp;
        private Button _btnCancel;
        private Button _btnResume;

        public StartResumePlaybackFragment() : base()
        {
        }

        public StartResumePlaybackFragment(CloudDeviceInfo device) : base()
        {
            _device = device;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            Dialog.SetTitle("Resume playback");
            _view = inflater.Inflate(Resource.Layout.StartResumePlayback, container, false);

            _lblDeviceName = _view.FindViewById<TextView>(Resource.Id.startResumePlayback_lblDeviceName);
            _lblPlaylistName = _view.FindViewById<TextView>(Resource.Id.startResumePlayback_lblPlaylistName);
            _lblArtistName = _view.FindViewById<TextView>(Resource.Id.startResumePlayback_lblArtistName);
            _lblAlbumTitle = _view.FindViewById<TextView>(Resource.Id.startResumePlayback_lblAlbumTitle);
            _lblSongTitle = _view.FindViewById<TextView>(Resource.Id.startResumePlayback_lblSongTitle);
            _lblTimestamp = _view.FindViewById<TextView>(Resource.Id.startResumePlayback_lblTimestamp);
            _btnCancel = _view.FindViewById<Button>(Resource.Id.startResumePlayback_btnCancel);
            _btnResume = _view.FindViewById<Button>(Resource.Id.startResumePlayback_btnResume);
            _btnCancel.Click += (sender, args) => Dismiss();
            _btnResume.Click += (sender, args) =>
            {
                OnResumePlayback();
                Dismiss();
            };

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindStartResumePlaybackView(this);

            return _view;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetStyle((int)DialogFragmentStyle.Normal, (int)Resource.Style.DialogTheme);
        }

        #region IStartResumePlaybackView implementation

        public Action OnResumePlayback { get; set; }

        public void StartResumePlaybackError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void RefreshCloudDeviceInfo(CloudDeviceInfo info, AudioFile audioFile)
        {
            Activity.RunOnUiThread(() =>
            {
                _lblDeviceName.Text = info.DeviceName;
                _lblPlaylistName.Text = "On-the-fly playlist";
                _lblArtistName.Text = info.ArtistName;
                _lblAlbumTitle.Text = info.AlbumTitle;
                _lblSongTitle.Text = info.SongTitle;
                _lblTimestamp.Text = string.Format("Last updated: {0} {1}", info.Timestamp.ToShortDateString(), info.Timestamp.ToLongTimeString());
            });
        }

        #endregion

    }
}
