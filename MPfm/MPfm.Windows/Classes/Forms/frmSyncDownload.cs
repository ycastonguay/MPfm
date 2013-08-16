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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MPfm.Library.Objects;
using MPfm.MVP.Views;

namespace MPfm.Windows.Classes.Forms
{
    public partial class frmSyncDownload : BaseForm, ISyncDownloadView
    {
        public frmSyncDownload(Action<IBaseView> onViewReady)
            : base(onViewReady)
        {
            InitializeComponent();
            ViewIsReady();
            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }

        #region ISyncDownloadView implementation

        public Action OnCancelDownload { get; set; }

        public void SyncDownloadError(Exception ex)
        {
            MethodInvoker methodUIUpdate = delegate
            {
                MessageBox.Show(string.Format("An error occured in Syncdownload: {0}", ex), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        public void RefreshDevice(SyncDevice device)
        {
            MethodInvoker methodUIUpdate = delegate
            {
                lblTitle.Text = "Syncing Library With " + device.Name;
                this.Text = "Syncing Library With " + device.Name;
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        public void RefreshStatus(SyncClientDownloadAudioFileProgressEntity entity)
        {
            MethodInvoker methodUIUpdate = delegate {
                progressBar.Value = (int)entity.PercentageDone;
                progressBarCurrentFile.Value = (int)entity.DownloadPercentageDone;
                lblStatus.Text = entity.Status;
                lblDownloadSpeedValue.Text = entity.DownloadSpeed;
                lblErrorsValue.Text = entity.Errors.ToString();
                lblFilesDownloadedValue.Text = string.Format("{0}/{1}", entity.FilesDownloaded, entity.TotalFiles);
                lblCurrentFile.Text = entity.DownloadFileName;
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        public void SyncCompleted()
        {
            MethodInvoker methodUIUpdate = delegate {
                MessageBox.Show(string.Format("The sync has completed successfully."), "Sync completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        #endregion

    }
}
