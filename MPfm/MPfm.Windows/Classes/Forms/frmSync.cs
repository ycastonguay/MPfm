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
    public partial class frmSync : BaseForm, ISyncView
    {
        public frmSync(Action<IBaseView> onViewReady)
            : base(onViewReady)
        {
            InitializeComponent();
            ViewIsReady();
        }

        #region ISyncView implementation

        public Action<SyncDevice> OnConnectDevice { get; set; }
        public Action<string> OnConnectDeviceManually { get; set; }
        public Action OnStartDiscovery { get; set; }
        public Action OnCancelDiscovery { get; set; }

        public void SyncError(Exception ex)
        {
        }

        public void RefreshIPAddress(string address)
        {
        }

        public void RefreshDiscoveryProgress(float percentageDone, string status)
        {
        }

        public void RefreshDevices(IEnumerable<SyncDevice> devices)
        {
        }

        public void RefreshDevicesEnded()
        {
        }

        public void SyncDevice(SyncDevice device)
        {
        }

        #endregion

    }
}
