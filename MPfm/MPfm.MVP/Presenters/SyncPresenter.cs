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
using MPfm.MVP.Navigation;

namespace MPfm.MVP.Presenters
{
	/// <summary>
	/// Sync view presenter.
	/// </summary>
	public class SyncPresenter : BasePresenter<ISyncView>, ISyncPresenter
	{
        readonly ISyncDiscoveryService _syncDiscoveryService;
        readonly MobileNavigationManager _navigationManager;
        List<SyncDevice> _devices = new List<SyncDevice>();

        public SyncPresenter(MobileNavigationManager navigationManager, ISyncDiscoveryService syncDiscoveryService)
		{
            _navigationManager = navigationManager;
            _syncDiscoveryService = syncDiscoveryService;
            _syncDiscoveryService.OnDeviceFound += HandleOnDeviceFound;
            _syncDiscoveryService.OnDiscoveryProgress += HandleOnDiscoveryProgress;
            _syncDiscoveryService.OnDiscoveryEnded += HandleOnDiscoveryEnded;
		}

        public override void BindView(ISyncView view)
        {
            view.OnConnectDevice = ConnectDevice;
            view.OnConnectDeviceManually = ConnectDeviceManually;
            base.BindView(view);

            Initialize();
        }       

        private void Initialize()
        {
            RefreshDevices();
            //View.RefreshIPAddress(String.Format("Authentication code: {0} ", SyncListenerService.AuthenticationCode));
        }

        private void HandleOnDeviceFound(SyncDevice deviceFound)
        {
            //Console.WriteLine("SyncPresenter - HandleOnDeviceFound - deviceName: {0} url: {1}", deviceFound.Name, deviceFound.Url);
            var device = _devices.FirstOrDefault(x => x.Url == deviceFound.Url);
            if(device == null)
                _devices.Add(deviceFound);

            View.RefreshDevices(_devices);
        }

        private void HandleOnDiscoveryProgress(float percentageDone, string status)
        {
            //Console.WriteLine("SyncPresenter - HandleOnDiscoveryProgress - percentageDone: {0} status: {1}", percentageDone, status);
            View.RefreshDiscoveryProgress(percentageDone, status);
        }

        private void HandleOnDiscoveryEnded(IEnumerable<SyncDevice> devices)
        {
            //Console.WriteLine("SyncPresenter - HandleOnDiscoveryEnded devices.Count: {0}", devices.Count());
            View.RefreshDevicesEnded();
        }

        private void ConnectDevice(string url)
        {
            var view = _navigationManager.CreateSyncMenuView(url);
            _navigationManager.PushTabView(MobileNavigationTabType.More, view);
        }

        private void ConnectDeviceManually(string url)
        {
        }

        private void RefreshDevices()
        {
            try
            {
                string ip = SyncListenerService.GetLocalIPAddress().ToString();
                View.RefreshIPAddress(String.Format("My IP address is {0}", ip));

                // Search for devices in subnet
                var split = ip.Split('.');
                string baseIP = split[0] + "." + split[1] + "." + split[2];
                Console.WriteLine("SyncPresenter - RefreshDevices with baseIP {0}", baseIP);
                _syncDiscoveryService.SearchForDevices(baseIP);
            }
            catch(Exception ex)
            {
                Console.WriteLine("SyncPresenter - RefreshDevices - Failed to refresh devices: {0}", ex);
                View.SyncError(ex);
            }
        }
    }
}

