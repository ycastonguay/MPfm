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
using MPfm.GTK.Windows;
using MPfm.MVP.Views;
using MPfm.Library.Objects;
using System.Text;
using Gtk;

namespace MPfm.GTK
{
    public partial class SyncWebBrowserWindow : BaseWindow, ISyncWebBrowserView
    {
        public SyncWebBrowserWindow(Action<IBaseView> onViewReady) : 
            base(Gtk.WindowType.Toplevel, onViewReady)
        {
            this.Build();
            onViewReady(this);
            //btnRefreshDeviceList.GrabFocus(); // the list view changes color when focused by default. it annoys me!
            this.Center();
            this.Show();
        }

        protected void OnClickCancel(object sender, EventArgs e)
        {
        }

        #region ISyncWebBrowserView implementation

        public System.Action OnViewAppeared { get; set; }

        public void SyncWebBrowserError(Exception ex)
        {
            Gtk.Application.Invoke(delegate
            {            
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("An error occured in the SyncCloud component:");
                sb.AppendLine(ex.Message);
                sb.AppendLine();
                sb.AppendLine(ex.StackTrace);                                                               
                MessageDialog md = new MessageDialog(
                    this,
                    DialogFlags.Modal,
                    MessageType.Error,
                    ButtonsType.Ok,
                    sb.ToString()
                );
                md.Run();
                md.Destroy();
            });
        }

        public void RefreshContent(string url, string authenticationCode)
        {
            Gtk.Application.Invoke(delegate
            {
                lblUrl.Text = url;
                lblAuthenticationCode.Text = authenticationCode;
            });
        }

        #endregion
    }
}
