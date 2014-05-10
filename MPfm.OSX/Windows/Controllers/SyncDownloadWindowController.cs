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
using MPfm.MVP.Views;
using MPfm.Library.Objects;
using MonoMac.AppKit;
using MPfm.OSX.Classes.Objects;
using System.Linq;
using MPfm.OSX.Classes.Helpers;
using MPfm.Core;
using MonoMac.Foundation;

namespace MPfm.OSX
{
    public partial class SyncDownloadWindowController : BaseWindowController, ISyncDownloadView
    {
        public SyncDownloadWindowController(IntPtr handle) 
            : base (handle)
        {
            Initialize();
        }

        public SyncDownloadWindowController(Action<IBaseView> onViewReady)
            : base ("SyncDownloadWindow", onViewReady)
        {
            Initialize();
        }

        private void Initialize()
        {
            ShowWindowCentered();
        }

        public override void WindowDidLoad()
        {
            base.WindowDidLoad();
            LoadFontsAndImages();
            OnViewReady.Invoke(this);
        }

        private void LoadFontsAndImages()
        {
            lblTitle.Font = NSFont.FromFontName("Roboto", 18);
            lblStatus.Font = NSFont.FromFontName("Roboto", 12);
            lblCurrentFile.Font = NSFont.FromFontName("Roboto", 12);
            lblCurrentFileValue.Font = NSFont.FromFontName("Roboto", 12);
            lblDownloadSpeed.Font = NSFont.FromFontName("Roboto", 12);
            lblDownloadSpeedValue.Font = NSFont.FromFontName("Roboto", 16);
            lblErrors.Font = NSFont.FromFontName("Roboto", 12);
            lblErrorsValue.Font = NSFont.FromFontName("Roboto", 16);
            lblFilesDownloaded.Font = NSFont.FromFontName("Roboto", 12);
            lblFilesDownloadedValue.Font = NSFont.FromFontName("Roboto", 16);

            btnCancel.Image = ImageResources.Images.FirstOrDefault(x => x.Name == "icon_button_cancel");
        }

        partial void actionCancel(NSObject sender)
        {
            OnCancelDownload();
        }

        #region ISyncDownloadView implementation

        public Action OnCancelDownload { get; set; }

        public void SyncDownloadError(Exception ex)
        {
            InvokeOnMainThread(delegate {
                CocoaHelper.ShowAlert("Error", string.Format("An error occured in SyncMenu: {0}", ex), NSAlertStyle.Critical);
            });
        }

        public void RefreshDevice(SyncDevice device)
        {
            InvokeOnMainThread(delegate {
                lblTitle.StringValue = "Syncing Library With " + device.Name;
                Window.Title = "Syncing Library With " + device.Name;
            });
        }

        public void RefreshStatus(SyncClientDownloadAudioFileProgressEntity entity)
        {
            InvokeOnMainThread(delegate {
                lblStatus.StringValue = entity.Status;
                lblDownloadSpeedValue.StringValue = entity.DownloadSpeed;
                lblErrorsValue.StringValue = entity.Errors.ToString();
                lblFilesDownloadedValue.StringValue = string.Format("{0}/{1}", entity.FilesDownloaded, entity.TotalFiles);
                lblCurrentFileValue.StringValue = entity.DownloadFileName;
                progressIndicator.DoubleValue = entity.PercentageDone;
                progressIndicatorCurrentFile.DoubleValue = entity.DownloadPercentageDone;
            });
        }

        public void SyncCompleted()
        {
            InvokeOnMainThread(delegate {
                lblStatus.StringValue = "Sync completed";
                using(NSAlert alert = new NSAlert())
                {
                    alert.MessageText = "Sync completed";
                    alert.InformativeText = "The sync has completed successfully.";
                    alert.AlertStyle = NSAlertStyle.Informational;
                    alert.BeginSheet(this.Window, () => {
                        this.Window.Close();
                    });
                }
            });
        }

        #endregion
    }
}
