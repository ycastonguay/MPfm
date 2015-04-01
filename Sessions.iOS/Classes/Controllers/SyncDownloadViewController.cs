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
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Sessions.MVP.Views;
using Sessions.iOS.Classes.Controllers.Base;
using Sessions.iOS.Classes.Objects;
using Sessions.iOS.Classes.Controls;
using Sessions.Library.Objects;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Navigation;

namespace Sessions.iOS
{
    public partial class SyncDownloadViewController : BaseViewController, ISyncDownloadView
    {
        public SyncDownloadViewController()
            : base (UserInterfaceIdiomIsPhone ? "SyncDownloadViewController_iPhone" : "SyncDownloadViewController_iPad", null)
        {
        }

        public override void ViewDidLoad()
        {
            this.View.BackgroundColor = GlobalTheme.BackgroundColor;
            ConfirmBackButton = true;
            ConfirmBackButtonTitle = "Sync download will be canceled";
            ConfirmBackButtonMessage = "Are you sure you wish to exit this screen and cancel the download?";

            // TODO: Detect back button press when the process is currently running. Ask the user if he wants to cancel the operation.
            Console.WriteLine("SyncDownloadViewController - ViewDidLoad");

            base.ViewDidLoad();

            var navigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
            navigationManager.BindSyncDownloadView(this);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            Console.WriteLine("SyncDownloadViewController - ViewWillAppear");

            SessionsNavigationController navCtrl = (SessionsNavigationController)this.NavigationController;
            navCtrl.SetTitle("Sync Library");
        }

        public override void ConfirmedBackButton()
        {
            Console.WriteLine("SyncDownloadViewController - ConfirmedBackButton");
            OnCancelDownload();
        }

        #region ISyncDownloadView implementation

        public Action OnCancelDownload { get; set; }

        public void SyncDownloadError(Exception ex)
        {
            ShowErrorDialog(ex);
        }

        public void RefreshDevice(SyncDevice device)
        {
        }

        public void RefreshStatus(SyncClientDownloadAudioFileProgressEntity entity)
        {
            InvokeOnMainThread(() => {
                progressView.Progress = entity.PercentageDone / 100f;
                lblTitle.Text = entity.Status;
                lblPercentageDoneValue.Text = string.Format("{0:0.0}%", entity.PercentageDone);
                lblCurrentFileValue.Text = string.Format("{0:0.0}%", entity.DownloadPercentageDone);
                lblFilesDownloadedValue.Text = string.Format("{0}", entity.FilesDownloaded);
                lblTotalFilesValue.Text = string.Format("{0}", entity.TotalFiles);
                lblDownloadSpeedValue.Text = entity.DownloadSpeed;
                lblErrorsValue.Text = string.Format("{0}", entity.Errors);
                lblFileNameValue.Text = entity.DownloadFileName;
                textViewLog.Text = entity.Log;
            });
        }

        public void SyncCompleted()
        {
            InvokeOnMainThread(() => {
                lblTitle.Text = "Sync completed";
                var alertView = new UIAlertView("Sync", "Sync completed successfully.", null, "OK", null);
                alertView.Clicked += (sender, e) => { 
                    Console.WriteLine("SyncDownloadViewController - Sync completed; dismissing views");
                    NavigationController.PopViewControllerAnimated(true);
                };
                alertView.Show();
            });
        }

        #endregion
    }
}
