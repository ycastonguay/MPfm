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
using MPfm.Library;
using MPfm.Library.Objects;
using MPfm.Library.Services.Interfaces;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using MPfm.Library.Services;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Sync cloud view presenter.
	/// </summary>
	public class SyncCloudPresenter : BasePresenter<ISyncCloudView>, ISyncCloudPresenter
	{
        readonly ISyncDeviceSpecifications _deviceSpecifications;

        public SyncCloudPresenter(ISyncDeviceSpecifications deviceSpecifications)
        {
            _deviceSpecifications = deviceSpecifications;
        }

        public override void BindView(ISyncCloudView view)
        {
            base.BindView(view);

            Initialize();
        }       

        private void Initialize()
        {
            try
            {
            }
            catch(Exception ex)
            {
                View.SyncCloudError(ex);
            }
        }
    }
}
