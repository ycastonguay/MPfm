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
using MPfm.MVP.Navigation;
using MPfm.MVP.Bootstrap;
using Sessions.Library.Objects;
using Sessions.Library.Services.Interfaces;

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
            _syncDeviceManagerService.OnStatusUpdated += HandleOnStatusUpdated;
            _syncDeviceManagerService.OnDeviceAdded += HandleOnDeviceAdded;
            _syncDeviceManagerService.OnDeviceRemoved += HandleOnDeviceRemoved;
            _syncDeviceManagerService.OnDeviceUpdated += HandleOnDeviceUpdated;
            _syncDeviceManagerService.OnDevicesUpdated += HandleOnDevicesUpdated;

#if IOS || ANDROID || WINDOWS_PHONE || WINDOWSSTORE
            _mobileNavigationManager = Bootstrapper.GetContainer().Resolve<MobileNavigationManager>();
#else
            _navigationManager = Bootstrapper.GetContainer().Resolve<NavigationManager>();
#endif
		}

        public override void BindView(ISyncView view)
        {
            view.OnAddDeviceFromUrl = AddDeviceFromUrl;
            view.OnRemoveDevice = RemoveDevice;
            view.OnSyncLibrary = SyncLibrary;
            view.OnResumePlayback = ResumePlayback;
            view.OnOpenAddDeviceDialog = OpenAddDeviceDialog;

            view.OnRemotePlayPause = RemotePlayPause;
            view.OnRemotePrevious = RemotePrevious;
            view.OnRemoteNext = RemoteNext;
            view.OnRemoteShuffle = RemoteShuffle;
            view.OnRemoteRepeat = RemoteRepeat;
            base.BindView(view);

            Initialize();
        }       

        private void Initialize()
        {
            _syncDeviceManagerService.Start();
        }

        private void HandleOnDeviceAdded(SyncDevice device)
        {
            //Console.WriteLine("SyncPresenter - HandleOnDeviceAdded");
            View.NotifyAddedDevice(device);
        }

        private void HandleOnDeviceRemoved(SyncDevice device)
        {
            //Console.WriteLine("SyncPresenter - HandleOnDeviceRemoved");
            View.NotifyRemovedDevice(device);
        }

        private void HandleOnDeviceUpdated(SyncDevice device)
        {
            //Console.WriteLine("SyncPresenter - HandleOnDeviceUpdated");
            View.NotifyUpdatedDevice(device);
        }

        private void HandleOnDevicesUpdated(IEnumerable<SyncDevice> devices)
        {
            View.NotifyUpdatedDevices(devices);
        }

        private void HandleOnStatusUpdated(string status)
        {
            if (View == null) return;
            View.RefreshStatus(status);
        }

        private void SyncLibrary(SyncDevice device)
        {
#if IOS || ANDROID || WINDOWS_PHONE || WINDOWSSTORE
            _mobileNavigationManager.CreateSyncMenuView(device);
#else
            _navigationManager.CreateSyncMenuView(device);
#endif
        }

        private void ResumePlayback(SyncDevice device)
        {

        }

        private void AddDeviceFromUrl(string url)
        {
            try
            {
                _syncDeviceManagerService.AddDeviceFromUrl(url);
            }
            catch(Exception ex)
            {
                View.SyncError(ex);
            }
        }

        private void OpenAddDeviceDialog()
        {
            #if IOS || ANDROID || WINDOWS_PHONE || WINDOWSSTORE
            var view = _mobileNavigationManager.CreateSyncConnectManualView();
            _mobileNavigationManager.PushDialogView(MobileDialogPresentationType.Overlay, "Connect to device manually", View, view);
            #else
            //_navigationManager.CreateSyncConnectManualView();
            #endif
        }

        private void RemoveDevice(SyncDevice device)
        {
            try
            {
                _syncDeviceManagerService.RemoveDevice(device);
            }
            catch(Exception ex)
            {
                View.SyncError(ex);
            }
        }

        private void RemotePlayPause(SyncDevice device)
        {
            try
            {
                _syncDeviceManagerService.RemotePause(device);
            }
            catch(Exception ex)
            {
                View.SyncError(ex);
            }
        }

        private void RemotePrevious(SyncDevice device)
        {
            try
            {
                _syncDeviceManagerService.RemotePrevious(device);
            }
            catch(Exception ex)
            {
                View.SyncError(ex);
            }
        }

        private void RemoteNext(SyncDevice device)
        {
            try
            {
                _syncDeviceManagerService.RemoteNext(device);
            }
            catch(Exception ex)
            {
                View.SyncError(ex);
            }
        }

        private void RemoteShuffle(SyncDevice device)
        {
            try
            {
                _syncDeviceManagerService.RemoteShuffle(device);
            }
            catch(Exception ex)
            {
                View.SyncError(ex);
            }
        }

        private void RemoteRepeat(SyncDevice device)
        {
            try
            {
                _syncDeviceManagerService.RemoteRepeat(device);
            }
            catch(Exception ex)
            {
                View.SyncError(ex);
            }
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

