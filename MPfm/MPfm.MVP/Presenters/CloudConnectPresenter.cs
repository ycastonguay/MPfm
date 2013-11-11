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
using System.Threading.Tasks;
//using Mono.Posix;
using MPfm.Library.Services.Interfaces;
using MPfm.MVP.Messages;
using MPfm.MVP.Models;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using TinyMessenger;

namespace MPfm.MVP.Presenters
{
	/// <summary>
    /// Cloud Connect preferences presenter.
	/// </summary>
    public class CloudConnectPresenter : BasePresenter<ICloudConnectView>, ICloudConnectPresenter
	{
	    private readonly ITinyMessengerHub _messengerHub;
	    private readonly ICloudLibraryService _cloudLibraryService;

        public CloudConnectPresenter(ITinyMessengerHub messengerHub, ICloudLibraryService cloudLibraryService)
        {
            _messengerHub = messengerHub;
            _cloudLibraryService = cloudLibraryService;
            _cloudLibraryService.OnCloudAuthenticationStatusChanged += CloudLibraryServiceOnCloudAuthenticationStatusChanged;
        }

	    public override void BindView(ICloudConnectView view)
        {
            view.OnCheckIfAccountIsLinked = CheckIfAccountIsLinked;
            base.BindView(view);

            Initialize();
        }

	    public override void ViewDestroyed()
	    {
	        _cloudLibraryService.OnCloudAuthenticationStatusChanged -= CloudLibraryServiceOnCloudAuthenticationStatusChanged;
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
                    _cloudLibraryService.LinkApp(View);
	            }
	            catch (Exception ex)
	            {
                    View.CloudConnectError(ex);
	            }
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
                    _cloudLibraryService.ContinueLinkApp();
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
