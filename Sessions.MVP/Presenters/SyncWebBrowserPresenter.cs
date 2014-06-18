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
using System.Linq;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using Sessions.Library;
using Sessions.Library.Services;
using Sessions.Library.Services.Interfaces;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Sync web browser view presenter.
	/// </summary>
	public class SyncWebBrowserPresenter : BasePresenter<ISyncWebBrowserView>, ISyncWebBrowserPresenter
	{
        private readonly ISyncDeviceSpecifications _deviceSpecifications;
        private readonly ISyncListenerService _syncListenerService;

        public SyncWebBrowserPresenter(ISyncDeviceSpecifications deviceSpecifications, ISyncListenerService syncListenerService)
        {
            _deviceSpecifications = deviceSpecifications;
            _syncListenerService = syncListenerService;
        }

        public override void BindView(ISyncWebBrowserView view)
        {
            view.OnViewAppeared = ViewAppeared;
            base.BindView(view);
        }       

        private void ViewAppeared()
        {
            try
            {
                //string ip = SyncListenerService.GetLocalIPAddress().ToString();
                string ip = _deviceSpecifications.GetIPAddress();
                string url = String.Format("http://{0}:{1}", ip, _syncListenerService.Port);
                View.RefreshContent(url, SyncListenerService.AuthenticationCode);
            }
            catch(Exception ex)
            {
                View.SyncWebBrowserError(ex);
            }
        }
    }
}
