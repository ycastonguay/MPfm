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
        readonly ISyncDeviceManagerService _syncDeviceManagerService;
        readonly MobileNavigationManager _mobileNavigationManager;
        readonly NavigationManager _navigationManager;

        public SyncPresenter(ISyncDeviceManagerService syncDeviceManagerService)
		{
            _syncDeviceManagerService = syncDeviceManagerService;
            _syncDeviceManagerService.OnDeviceAdded += HandleOnDeviceAdded;
            _syncDeviceManagerService.OnDeviceRemoved += HandleOnDeviceRemoved;
            _syncDeviceManagerService.OnDeviceUpdated += HandleOnDeviceUpdated;

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
            view.OnOpenConnectDevice = OpenConnectDevice;
            view.OnCancelDiscovery = CancelDiscovery;
            view.OnStartDiscovery = RefreshDevices;
            base.BindView(view);

            Initialize();
        }       

        private void Initialize()
        {
            _syncDeviceManagerService.Start();
            //RefreshDevices();
        }

        private void HandleOnDeviceAdded(SyncDevice device)
        {
            Console.WriteLine("SyncPresenter - HandleOnDeviceAdded");
            View.NotifyAddedDevice(device);
        }

        private void HandleOnDeviceRemoved(SyncDevice device)
        {
            Console.WriteLine("SyncPresenter - HandleOnDeviceRemoved");
            View.NotifyRemovedDevice(device);
        }

        private void HandleOnDeviceUpdated(SyncDevice device)
        {
            Console.WriteLine("SyncPresenter - HandleOnDeviceUpdated");
            View.NotifyUpdatedDevice(device);
        }

//        private void HandleOnDeviceFound(SyncDevice deviceFound)
//        {
//            //Tracing.Log("SyncPresenter - HandleOnDeviceFound - deviceName: {0} url: {1}", deviceFound.Name, deviceFound.Url);
//            var device = _devices.FirstOrDefault(x => x.Url == deviceFound.Url);
//            if(device == null)
//                _devices.Add(deviceFound);
//
//            View.RefreshDevices(_devices);
//        }
//
//        private void HandleOnDiscoveryProgress(float percentageDone, string status)
//        {
//            //Tracing.Log("SyncPresenter - HandleOnDiscoveryProgress - percentageDone: {0} status: {1}", percentageDone, status);
//            View.RefreshDiscoveryProgress(percentageDone, status);
//        }
//
//        private void HandleOnDiscoveryEnded(IEnumerable<SyncDevice> devices)
//        {
//            //Tracing.Log("SyncPresenter - HandleOnDiscoveryEnded devices.Count: {0}", devices.Count());
//            View.RefreshDevicesEnded();
//        }

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

        private void OpenConnectDevice()
        {
            #if IOS || ANDROID || WINDOWS_PHONE || WINDOWSSTORE
            var view = _mobileNavigationManager.CreateSyncConnectManualView();
            _mobileNavigationManager.PushDialogView(MobileDialogPresentationType.Overlay, "Connect to device manually", View, view);
            #else
            //_navigationManager.CreateSyncConnectManualView();
            #endif
        }

        private void CancelDiscovery()
        {
            //Tracing.Log("SyncPresenter - CancelDiscovery");
            //_syncDiscoveryService.Cancel();
        }

        private void RefreshDevices()
        {
//            try
//            {
//                string ip = _deviceSpecifications.GetIPAddress();
//                View.RefreshIPAddress(String.Format("My IP address is {0}", ip));
//
//                //var devices = _syncDiscoveryService.GetDeviceList();
//                var devices = GetTestDeviceList();
//                View.RefreshDevices(devices);
//
//                if(_syncDiscoveryService.IsRunning)
//                    return;
//
//                // Search for devices in subnet
//                var split = ip.Split('.');
//                string baseIP = split[0] + "." + split[1] + "." + split[2];
//                Tracing.Log("SyncPresenter - RefreshDevices with baseIP {0}", baseIP);
//                _syncDiscoveryService.SearchForDevices(baseIP);
//            }
//            catch(Exception ex)
//            {
//                Tracing.Log("SyncPresenter - RefreshDevices - Failed to refresh devices: {0}", ex);
//                View.SyncError(ex);
//            }
        }

        private List<SyncDevice> GetTestDeviceList()
        {
            var list = new List<SyncDevice>();
            list.Add(new SyncDevice() { Name = "Nexus 5", DeviceType = SyncDeviceType.AndroidPhone });
            list.Add(new SyncDevice() { Name = "Nexus 10", DeviceType = SyncDeviceType.AndroidTablet });
            list.Add(new SyncDevice() { Name = "iPhone 5", DeviceType = SyncDeviceType.iPhone });
            list.Add(new SyncDevice() { Name = "iPad Air", DeviceType = SyncDeviceType.iPad }); 
            list.Add(new SyncDevice() { Name = "Nokia Lumia 920", DeviceType = SyncDeviceType.WindowsPhone }); 
            list.Add(new SyncDevice() { Name = "Surface 2", DeviceType = SyncDeviceType.WindowsStore }); 
            list.Add(new SyncDevice() { Name = "Windows 8.1", DeviceType = SyncDeviceType.Windows }); 
            list.Add(new SyncDevice() { Name = "Ubuntu 13.08", DeviceType = SyncDeviceType.Linux }); 
            list.Add(new SyncDevice() { Name = "MacBook Pro", DeviceType = SyncDeviceType.OSX }); 
            return list;
        }
    }
}

