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
using MPfm.Library.Objects;
using MPfm.Library.Services.Interfaces;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using MPfm.Library.Services;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Sync web browser view presenter.
	/// </summary>
	public class SyncWebBrowserPresenter : BasePresenter<ISyncWebBrowserView>, ISyncWebBrowserPresenter
	{
        public SyncWebBrowserPresenter()
		{
		}

        public override void BindView(ISyncWebBrowserView view)
        {
            base.BindView(view);

            Initialize();
        }       

        private void Initialize()
        {
            try
            {
                string ip = SyncListenerService.GetLocalIPAddress().ToString();
                string url = String.Format("http://{0}:53551", ip);
                View.RefreshContent(url, SyncListenerService.AuthenticationCode);
            }
            catch(Exception ex)
            {
                View.SyncWebBrowserError(ex);
            }
        }
    }
}
