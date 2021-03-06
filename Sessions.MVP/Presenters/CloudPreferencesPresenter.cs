﻿// Copyright © 2011-2013 Yanick Castonguay
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
using System.ComponentModel;
using System.Threading.Tasks;
using Sessions.MVP.Bootstrap;
using Sessions.MVP.Config;
using Sessions.MVP.Config.Models;
using Sessions.MVP.Messages;
using Sessions.MVP.Models;
using Sessions.MVP.Navigation;
using Sessions.MVP.Presenters.Interfaces;
using Sessions.MVP.Views;
using Sessions.Library.Services.Interfaces;
using TinyMessenger;

namespace Sessions.MVP.Presenters
{
	/// <summary>
    /// Cloud preferences presenter.
	/// </summary>
    public class CloudPreferencesPresenter : BasePresenter<ICloudPreferencesView>, ICloudPreferencesPresenter
	{
        private readonly MobileNavigationManager _mobileNavigationManager;
        private readonly NavigationManager _navigationManager;
	    private readonly ITinyMessengerHub _messengerHub;
	    private readonly ICloudService _cloudService;

	    public CloudPreferencesPresenter(ITinyMessengerHub messengerHub, ICloudService cloudService)
        {
	        _messengerHub = messengerHub;
            _cloudService = cloudService;

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
            RefreshPreferences();
            RefreshState();
        }

        private void CloudConnectStatusChanged(CloudConnectStatusChangedMessage cloudConnectStatusChangedMessage)
        {
            RefreshState();
        }

        private void SetCloudPreferences(CloudAppConfig config)
        {
            AppConfigManager.Instance.Root.Cloud = config;
            AppConfigManager.Instance.Save();
            _messengerHub.PublishAsync<CloudAppConfigChangedMessage>(new CloudAppConfigChangedMessage(this, config));
        }
    
        private void LoginLogoutDropbox()
        {
            if (_cloudService.HasLinkedAccount)
            {
                Task.Factory.StartNew(() =>
                {
                    _cloudService.UnlinkApp();
                    RefreshState();
                });
            }
            else
            {
                #if IOS || ANDROID || WINDOWS_PHONE || WINDOWSSTORE
                var view = _mobileNavigationManager.CreateCloudConnectView();
                _mobileNavigationManager.PushDialogView(MobileDialogPresentationType.Overlay, "Connect to Dropbox", View, view);
                #else
                _navigationManager.CreateCloudConnectView();
                #endif

                RefreshState();
            }
        }

	    private void RefreshState()
	    {
	        var state = new CloudPreferencesStateEntity()
	        {
                IsDropboxLinkedToApp = _cloudService.HasLinkedAccount
	        };
	        View.RefreshCloudPreferencesState(state);
	    }

        private void RefreshPreferences()
        {
            View.RefreshCloudPreferences(AppConfigManager.Instance.Root.Cloud);
        }
    }
}
