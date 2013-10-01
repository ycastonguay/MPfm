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
using MPfm.Core;
using MPfm.Library;
using MPfm.Library.Objects;
using MPfm.Library.Services.Interfaces;
using MPfm.MVP.Presenters.Interfaces;
using MPfm.MVP.Views;
using MPfm.Library.Services;
using MPfm.MVP.Navigation;
using MPfm.MVP.Bootstrap;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Sync view presenter.
	/// </summary>
	public class SyncPresenter : BasePresenter<ISyncView>, ISyncPresenter
	{
        readonly ISyncDiscoveryService _syncDiscoveryService;
        readonly MobileNavigationManager _mobileNavigationManager;
        readonly NavigationManager _navigationManager;
	    readonly ISyncDeviceSpecifications _deviceSpecifications;
        List<SyncDevice> _devices = new List<SyncDevice>();

        public SyncPresenter(ISyncDiscoveryService syncDiscoveryService, ISyncDeviceSpecifications deviceSpecifications)
		{
            _deviceSpecifications = deviceSpecifications;
            _syncDiscoveryService = syncDiscoveryService;
            _syncDiscoveryService.OnDeviceFound += HandleOnDeviceFound;
            _syncDiscoveryService.OnDiscoveryProgress += HandleOnDiscoveryProgress;
            _syncDiscoveryService.OnDiscoveryEnded += HandleOnDiscoveryEnded;

#if IOS || ANDROID || WINDOWS_PHONE || WINDOWSSTORE
            _mobileNavigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
#else
            _navigationManager = Bootstrapper.GetContainer().Resolve<NavigationManager>();
#endif
		}

        public override void BindView(ISyncView view)
        {
            view.OnConnectDevice = ConnectDevice;
            view.OnConnectDeviceManually = ConnectDeviceManually;
            view.OnCancelDiscovery = CancelDiscovery;
            view.OnStartDiscovery = RefreshDevices;
            base.BindView(view);

            Initialize();
        }       

        private void Initialize()
        {
            RefreshDevices();
        }

        private void HandleOnDeviceFound(SyncDevice deviceFound)
        {
            //Tracing.Log("SyncPresenter - HandleOnDeviceFound - deviceName: {0} url: {1}", deviceFound.Name, deviceFound.Url);
            var device = _devices.FirstOrDefault(x => x.Url == deviceFound.Url);
            if(device == null)
                _devices.Add(deviceFound);

            View.RefreshDevices(_devices);
        }

        private void HandleOnDiscoveryProgress(float percentageDone, string status)
        {
            //Tracing.Log("SyncPresenter - HandleOnDiscoveryProgress - percentageDone: {0} status: {1}", percentageDone, status);
            View.RefreshDiscoveryProgress(percentageDone, status);
        }

        private void HandleOnDiscoveryEnded(IEnumerable<SyncDevice> devices)
        {
            //Tracing.Log("SyncPresenter - HandleOnDiscoveryEnded devices.Count: {0}", devices.Count());
            View.RefreshDevicesEnded();
        }

        private void ConnectDevice(SyncDevice device)
        {
#if IOS || ANDROID || WINDOWS_PHONE || WINDOWSSTORE
            _mobileNavigationManager.CreateSyncMenuView(device);
#else
            _navigationManager.CreateSyncMenuView(device);
#endif
        }

        private void ConnectDeviceManually(string url)
        {
        }

        private void CancelDiscovery()
        {
            Tracing.Log("SyncPresenter - CancelDiscovery");
            _syncDiscoveryService.Cancel();
        }

        private void RefreshDevices()
        {
            try
            {
                if(_syncDiscoveryService.IsRunning)
                    return;

                string ip = _deviceSpecifications.GetIPAddress();
                View.RefreshIPAddress(String.Format("My IP address is {0}", ip));

                // Search for devices in subnet
                var split = ip.Split('.');
                string baseIP = split[0] + "." + split[1] + "." + split[2];
                Tracing.Log("SyncPresenter - RefreshDevices with baseIP {0}", baseIP);
                _syncDiscoveryService.SearchForDevices(baseIP);
            }
            catch(Exception ex)
            {
                Tracing.Log("SyncPresenter - RefreshDevices - Failed to refresh devices: {0}", ex);
                View.SyncError(ex);
            }
        }
    }
}

