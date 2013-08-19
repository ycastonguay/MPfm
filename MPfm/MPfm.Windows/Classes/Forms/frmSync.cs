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
using MPfm.MVP.Views;

namespace MPfm.Windows.Classes.Forms
{
    public partial class frmSync : BaseForm, ISyncView
    {
        bool _isDiscovering;

        public frmSync(Action<IBaseView> onViewReady)
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
            if (listView.SelectedItems.Count == 0)
                return;

            OnCancelDiscovery();
            OnConnectDevice((SyncDevice) listView.SelectedItems[0].Tag);
        }

        private void btnConnectManual_Click(object sender, EventArgs e)
        {

        }

        private void btnRefreshDevices_Click(object sender, EventArgs e)
        {
            if (_isDiscovering)
                OnCancelDiscovery();
            else
                OnStartDiscovery();
        }

        #region ISyncView implementation

        public Action<SyncDevice> OnConnectDevice { get; set; }
        public Action<string> OnConnectDeviceManually { get; set; }
        public Action OnStartDiscovery { get; set; }
        public Action OnCancelDiscovery { get; set; }

        public void SyncError(Exception ex)
        {
            MethodInvoker methodUIUpdate = delegate {
                MessageBox.Show(string.Format("An error occured in Sync: {0}", ex), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        public void RefreshIPAddress(string address)
        {
            MethodInvoker methodUIUpdate = delegate {
                lblSubtitle.Text = address;
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        public void RefreshDiscoveryProgress(float percentageDone, string status)
        {
            MethodInvoker methodUIUpdate = delegate {                
                if (!_isDiscovering)
                {
                    _isDiscovering = true;
                    progressBar.Visible = true;
                    RefreshDeviceListButton();
                }
                progressBar.Value = (int)percentageDone;
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        public void RefreshDevices(IEnumerable<SyncDevice> devices)
        {
            MethodInvoker methodUIUpdate = delegate {
                listView.BeginUpdate();
                listView.Items.Clear();
                foreach (var device in devices)
                    listView.Items.Add(new ListViewItem(device.Name, (int)device.DeviceType) {
                        Tag = device
                    });
                listView.EndUpdate();
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        public void RefreshDevicesEnded()
        {
            MethodInvoker methodUIUpdate = delegate {
                _isDiscovering = false;
                progressBar.Visible = false;
                RefreshDeviceListButton();
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        public void SyncDevice(SyncDevice device)
        {
        }

        #endregion

    }
}
