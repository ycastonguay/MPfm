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
using System.IO;
using System.Text;
using System.Windows.Forms;
using MPfm.Library.Objects;
using MPfm.Library.UpdateLibrary;
using MPfm.MVP.Views;

namespace MPfm.Windows.Classes.Forms
{
    /// <summary>
    /// Update Library window. This window is displayed when the application 
    /// is updating the library.
    /// </summary>
    public partial class frmUpdateLibrary : BaseForm, IUpdateLibraryView
    {
        public frmUpdateLibrary(Action<IBaseView> onViewReady) 
            : base(onViewReady)
        {
            InitializeComponent();
            ViewIsReady();
        }

        private void frmUpdateLibraryStatus_Shown(object sender, EventArgs e)
        {
            btnCancel.Enabled = true;
            btnOK.Enabled = false;
            btnSaveLog.Enabled = false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            OnCancelUpdateLibrary();
        }

        private void btnSaveLog_Click(object sender, EventArgs e)
        {
            if (saveLogDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                OnSaveLog(saveLogDialog.FileName);
        }

        #region IUpdateLibraryView implementation

        public Action<UpdateLibraryMode, List<string>, string> OnStartUpdateLibrary { get; set; }
        public Action OnCancelUpdateLibrary { get; set; }
        public Action<string> OnSaveLog { get; set; }

        public void RefreshStatus(UpdateLibraryEntity entity)
        {
            MethodInvoker methodUIUpdate = delegate 
            {
                lblTitle.Text = entity.Title;
                lblMessage.Text = entity.Subtitle;
                lblProgress.Text = string.Format("{0:0.0} %", entity.PercentageDone * 100);
                progressBar.Value = (int)entity.PercentageDone * 100;
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        public void AddToLog(string entry)
        {
            MethodInvoker methodUIUpdate = delegate
            {
                lbLog.Items.Insert(0, entry);
                if (lbLog.Items.Count > 1000)
                    lbLog.Items.RemoveAt(lbLog.Items.Count - 1);                
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        public void ProcessEnded(bool canceled)
        {
            MethodInvoker methodUIUpdate = delegate {
                lblTitle.Text = "Update library completed successfully";
                lblMessage.Text = string.Empty;
                btnCancel.Enabled = false;
                btnOK.Enabled = true;
                btnSaveLog.Enabled = true;
            };

            if (InvokeRequired)
                BeginInvoke(methodUIUpdate);
            else
                methodUIUpdate.Invoke();
        }

        #endregion

    }
}
