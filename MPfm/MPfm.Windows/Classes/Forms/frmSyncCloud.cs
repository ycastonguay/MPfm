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
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Windows.Threading;
using MPfm.Library.Objects;
using MPfm.Library.Services;
using MPfm.Library.Services.Interfaces;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Views;

namespace MPfm.Windows.Classes.Forms
{
    public partial class frmSyncCloud : BaseForm, ISyncCloudView
    {
        bool _isDiscovering;

        public frmSyncCloud(Action<IBaseView> onViewReady)
            : base(onViewReady)
        {
            InitializeComponent();
            ViewIsReady();
        }

        private void RefreshDeviceListButton()
        {
            if (_isDiscovering)
            {
                btnRefreshDevices.Image = new Bitmap(MPfm.Windows.Properties.Resources.icon_button_cancel_16);
                btnRefreshDevices.Text = "Cancel refresh";
            }
            else
            {
                btnRefreshDevices.Image = new Bitmap(MPfm.Windows.Properties.Resources.icon_button_refresh_16);
                btnRefreshDevices.Text = "Refresh devices";
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
        }

        private void btnConnectManual_Click(object sender, EventArgs e)
        {
            var dropbox = Bootstrapper.GetContainer().Resolve<IDropboxService>();
            dropbox.LinkApp(this);
        }

        private void btnRefreshDevices_Click(object sender, EventArgs e)
        {
        }

        #region ISyncCloudView implementation

        public void SyncCloudError(Exception ex)
        {
            MethodInvoker methodUIUpdate = delegate
            {
                MessageBox.Show(string.Format("An error occured in SyncCloud: {0}", ex), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        #endregion

    }
}
