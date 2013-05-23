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
using MPfm.MVP.Views;
using System.Collections.Generic;
using MPfm.MVP.Models;
using MPfm.GTK.Windows;

namespace MPfm.GTK
{
	public partial class SyncWindow : BaseWindow, ISyncView
	{
		public SyncWindow(Action<IBaseView> onViewReady) : 
			base(Gtk.WindowType.Toplevel, onViewReady)
		{
			this.Build();
            onViewReady(this);
            this.Show();
		}

		/// <summary>
		/// Raises the delete event (when the form is closing).
		/// Prevents the form from closing by hiding it instead.
		/// </summary>
		/// <param name='o'>Object</param>
		/// <param name='args'>Event arguments</param>
		protected void OnDeleteEvent(object o, Gtk.DeleteEventArgs args)
		{
			// Prevent window from closing
			args.RetVal = true;
			
			// Hide window instead
			this.HideAll();
		}

		protected void OnSyncRefreshDeviceList(object sender, EventArgs e)
        {
            OnRefreshDevices();
        }

        #region ISyncView implementation

        public System.Action OnRefreshDevices { get; set; }

        public void RefreshDevices(IEnumerable<MPfm.MVP.Models.SyncDeviceEntity> devices)
        {
        }

        public void SyncDevice(SyncDeviceEntity device)
        {
        }

        #endregion

	}
}
