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
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MPfm.MVP.Views;
using MPfm.iOS.Classes.Controllers.Base;
using MPfm.iOS.Classes.Objects;
using MPfm.iOS.Classes.Controls;
using MPfm.Library.Objects;

namespace MPfm.iOS
{
    public partial class SyncDownloadViewController : BaseViewController, ISyncDownloadView
    {
        public SyncDownloadViewController(Action<IBaseView> onViewReady)
            : base (onViewReady, UserInterfaceIdiomIsPhone ? "SyncDownloadViewController_iPhone" : "SyncDownloadViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
            this.View.BackgroundColor = GlobalTheme.BackgroundColor;
            btnCancel.BackgroundColor = GlobalTheme.SecondaryColor;
            btnCancel.Layer.CornerRadius = 8;

            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            MPfmNavigationController navCtrl = (MPfmNavigationController)this.NavigationController;
            navCtrl.SetTitle("Sync Library", "Downloading audio files");
        }

        partial void actionCancel(NSObject sender)
        {
        }

        #region ISyncDownloadView implementation

        public Action OnButtonPressed { get; set; }

        public void SyncDownloadError(Exception ex)
        {
            InvokeOnMainThread(() => {
                var alertView = new UIAlertView("Sync Download Error", ex.Message, null, "OK", null);
                alertView.Show();
            });
        }

        public void RefreshStatus(SyncClientDownloadAudioFileProgressEntity entity)
        {
            // Maybe add download speed? total download size?
            InvokeOnMainThread(() => {
                progressView.Progress = entity.PercentageDone / 100f;
                lblPercentageDoneValue.Text = string.Format("{0:0.0}%", entity.PercentageDone);
                lblCurrentFileValue.Text = string.Format("{0:0.0}%", entity.DownloadPercentageDone);
                lblFilesDownloadedValue.Text = string.Format("{0}", entity.FilesDownloaded);
                lblTotalFilesValue.Text = string.Format("{0}", entity.TotalFiles);
                lblErrorsValue.Text = string.Format("{0}", entity.Errors);
                lblFileNameValue.Text = entity.DownloadFileName;
                textViewLog.Text = entity.Log;
            });
        }

        #endregion
    }
}
