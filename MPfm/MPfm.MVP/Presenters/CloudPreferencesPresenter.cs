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
using System.ComponentModel;
using MPfm.Library.Services.Interfaces;
using MPfm.MVP.Bootstrap;
using MPfm.MVP.Messages;
using MPfm.MVP.Models;
using MPfm.MVP.Navigation;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using TinyMessenger;

namespace MPfm.MVP.Presenters
{
	/// <summary>
    /// Cloud preferences presenter.
	/// </summary>
    public class CloudPreferencesPresenter : BasePresenter<ICloudPreferencesView>, ICloudPreferencesPresenter
	{
        private readonly MobileNavigationManager _mobileNavigationManager;
        private readonly NavigationManager _navigationManager;
	    private readonly ITinyMessengerHub _messengerHub;
	    private readonly ICloudLibraryService _cloudLibraryService;
	    private bool _isApplicationLinked;

	    public CloudPreferencesPresenter(ITinyMessengerHub messengerHub, ICloudLibraryService cloudLibraryService)
        {
	        _messengerHub = messengerHub;
	        _cloudLibraryService = cloudLibraryService;

	        _messengerHub.Subscribe<CloudConnectStatusChangedMessage>(CloudConnectStatusChanged);

#if IOS || ANDROID || WINDOWS_PHONE || WINDOWSSTORE
            _mobileNavigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
#else
            _navigationManager = Bootstrapper.GetContainer().Resolve<NavigationManager>();
#endif
        }

	    public override void BindView(ICloudPreferencesView view)
        {
            view.OnDropboxLoginLogout = LoginLogoutDropbox;
            view.OnSetCloudPreferences = SetCloudPreferences;
            base.BindView(view);

            Initialize();
        }

	    private void Initialize()
	    {
	        _isApplicationLinked = _cloudLibraryService.HasLinkedAccount;
            RefreshPreferences();
            RefreshState();
        }

        private void CloudConnectStatusChanged(CloudConnectStatusChangedMessage cloudConnectStatusChangedMessage)
        {
            _isApplicationLinked = cloudConnectStatusChangedMessage.IsApplicationLinked;
            RefreshState();
        }

        private void SetCloudPreferences(CloudPreferencesEntity entity)
        {
        }
    
        private void LoginLogoutDropbox()
        {
            //if (_cloudLibraryService.HasLinkedAccount)
            //    _cloudLibraryService.UnlinkApp();
            //else
            //    _cloudLibraryService.LinkApp(View);

            //RefreshState();

#if IOS || ANDROID || WINDOWS_PHONE || WINDOWSSTORE
            _mobileNavigationManager.CreateCloudConnectView();
#else
            _navigationManager.CreateCloudConnectView();
#endif
        }

	    private void RefreshState()
	    {
	        var state = new CloudPreferencesStateEntity()
	        {
	            IsDropboxLinkedToApp = _isApplicationLinked
	        };
	        View.RefreshCloudPreferencesState(state);
	    }

        private void RefreshPreferences()
        {
            var entity = new CloudPreferencesEntity()
            {
                IsDropboxResumePlaybackEnabled = false
            };
            View.RefreshCloudPreferences(entity);
        }
    }
}
