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
using System.Reflection.Emit;
using System.Text;
using System.Windows.Forms;
using System.Windows.Threading;
using MPfm.Core;
using MPfm.Library.Objects;
using MPfm.Library.Services;
using MPfm.Library.Services.Interfaces;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Views;

namespace MPfm.Windows.Classes.Forms
{
    public partial class frmSyncCloud : BaseForm, ISyncCloudView
    {
        private ICloudLibraryService _dropbox;
        private bool _isDiscovering;

        public frmSyncCloud(Action<IBaseView> onViewReady)
            : base(onViewReady)
        {
            InitializeComponent();

            _dropbox = Bootstrapper.GetContainer().Resolve<ICloudLibraryService>();

            ViewIsReady();
        }

        protected override void OnActivated(EventArgs e)
        {
            Tracing.Log("frmSyncCloud - OnActivated");
            base.OnActivated(e);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                _dropbox.LinkApp(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("An error occured in SyncCloud: {0}", ex), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {

        }

        private void btnPull_Click(object sender, EventArgs e)
        {

        }

        private void btnPush_Click(object sender, EventArgs e)
        {

        }

        private void btnDelete_Click(object sender, EventArgs e)
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
