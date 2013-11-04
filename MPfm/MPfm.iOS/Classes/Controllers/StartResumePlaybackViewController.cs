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
using System.Drawing;
using MPfm.Library.Objects;
using MPfm.MVP.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.MVP.Navigation;
using MPfm.MVP.Bootstrap;

namespace MPfm.iOS
{
    public partial class StartResumePlaybackViewController : BaseViewController, IStartResumePlaybackView
    {
        public StartResumePlaybackViewController()
			: base (UserInterfaceIdiomIsPhone ? "StartResumePlaybackViewController_iPhone" : "StartResumePlaybackViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
            btnResume.SetImage(UIImage.FromBundle("Images/Buttons/select"));
            btnCancel.SetImage(UIImage.FromBundle("Images/Buttons/cancel"));

            base.ViewDidLoad();

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindStartResumePlaybackView(this);

        }

        partial void actionResume(NSObject sender)
        {
            OnResumePlayback();
            Close();
        }

        partial void actionCancel(NSObject sender)
        {
            Close();
        }

        private void Close()
        {
            WillMoveToParentViewController(null);
            UIView.Animate(0.2f, () => {
                this.View.Alpha = 0;
            }, () => {
                View.RemoveFromSuperview();
                RemoveFromParentViewController();
            });
        }

        #region IStartResumePlaybackView implementation

        public Action OnResumePlayback { get; set; }

        public void StartResumePlaybackError(Exception ex)
        {
            InvokeOnMainThread(() => {
                var alertView = new UIAlertView("StartResumePlayback Error", ex.Message, null, "OK", null);
                alertView.Show();
            });
        }

        public void RefreshCloudDeviceInfo(CloudDeviceInfo device)
        {
            InvokeOnMainThread(() => {
                lblDeviceName.Text = device.DeviceName;
                lblPlaylistName.Text = "On-the-fly Playlist";
                lblArtistName.Text = device.ArtistName;
                lblAlbumTitle.Text = device.AlbumTitle;
                lblSongTitle.Text = device.SongTitle;
                lblTimestamp.Text = string.Format("Last updated: {0} {1}", device.Timestamp.ToShortDateString(), device.Timestamp.ToLongTimeString());
            });
        }

        #endregion
    }
}
