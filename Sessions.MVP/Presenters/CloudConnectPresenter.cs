// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Threading.Tasks;
using Sessions.MVP.Messages;
using Sessions.MVP.Models;
using Sessions.MVP.Presenters.Interfaces;
using Sessions.MVP.Views;
using Sessions.Library.Services.Interfaces;
using TinyMessenger;

namespace Sessions.MVP.Presenters
{
	/// <summary>
    /// Cloud Connect preferences presenter.
	/// </summary>
    public class CloudConnectPresenter : BasePresenter<ICloudConnectView>, ICloudConnectPresenter
	{
	    private readonly ITinyMessengerHub _messengerHub;
	    private readonly ICloudService _cloudService;

        public CloudConnectPresenter(ITinyMessengerHub messengerHub, ICloudService cloudService)
        {
            _messengerHub = messengerHub;
            _cloudService = cloudService;
            _cloudService.OnCloudAuthenticationStatusChanged += CloudLibraryServiceOnCloudAuthenticationStatusChanged;
            _cloudService.OnCloudAuthenticationFailed += CloudLibraryServiceOnCloudAuthenticationFailed;
        }

	    public override void BindView(ICloudConnectView view)
        {
            view.OnCheckIfAccountIsLinked = CheckIfAccountIsLinked;
            base.BindView(view);

            Initialize();
        }

	    public override void ViewDestroyed()
	    {
            _cloudService.OnCloudAuthenticationStatusChanged -= CloudLibraryServiceOnCloudAuthenticationStatusChanged;
            _cloudService.OnCloudAuthenticationFailed -= CloudLibraryServiceOnCloudAuthenticationFailed;
	    }

	    private void Initialize()
	    {
            View.RefreshStatus(new CloudConnectEntity()
            {
                CloudServiceName = "Dropbox",
                CurrentStep = 1,
                IsAuthenticated = false
            });

	        Task.Factory.StartNew(() =>
	        {
	            try
	            {
                    _cloudService.LinkApp(View);
	            }
	            catch (Exception ex)
	            {
                    View.CloudConnectError(ex);
	            }
	        });
	    }

        private void CloudLibraryServiceOnCloudAuthenticationFailed()
        {
            View.RefreshStatus(new CloudConnectEntity()
                               {
                CloudServiceName = "Dropbox",
                CurrentStep = 1,
                HasAuthenticationFailed = true
            });
        }

        private void CloudLibraryServiceOnCloudAuthenticationStatusChanged(CloudAuthenticationStatusType statusType)
        {
            Console.WriteLine("CloudConnectPresenter - CloudLibraryServiceOnCloudAuthenticationStatusChanged - statusType: {0}", statusType.ToString());
            View.RefreshStatus(new CloudConnectEntity()
            {
                CloudServiceName = "Dropbox",
                CurrentStep = ((int)statusType)+2,
                IsAuthenticated = statusType == CloudAuthenticationStatusType.ConnectedToDropbox
            });
            _messengerHub.PublishAsync<CloudConnectStatusChangedMessage>(new CloudConnectStatusChangedMessage(this)
            {
                CloudServiceName = "Dropbox",
                StatusType = statusType,
                IsApplicationLinked = statusType == CloudAuthenticationStatusType.ConnectedToDropbox
            });
        }

        private void CheckIfAccountIsLinked()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    _cloudService.ContinueLinkApp();
                }
                catch (Exception ex)
                {
                    // Ignore error since the user may not be logged in yet!
                    Console.WriteLine("CloudConnectPresenter - CheckIfAccountIsLinked - Exception: {0}", ex);
                }
            });
        }
	}
}
