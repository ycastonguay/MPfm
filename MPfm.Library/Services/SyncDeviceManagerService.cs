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
using System.Net;
using System.Threading;
using MPfm.Core.Network;
using MPfm.Player.Objects;
using MPfm.Library.Objects;
using MPfm.Library.Services.Interfaces;
using Newtonsoft.Json;

namespace MPfm.Library.Services
{
    public class SyncDeviceManagerService : ISyncDeviceManagerService
    {
        private readonly object _locker = new object();
        private readonly ISyncDiscoveryService _discoveryService;
        private readonly ISyncDeviceSpecifications _deviceSpecifications;
        private readonly WebClientTimeout _webClient;
        private List<SyncDevice> _devices;

        public event DeviceUpdated OnDeviceUpdated;
        public event DeviceUpdated OnDeviceAdded;
        public event DeviceUpdated OnDeviceRemoved;

        public SyncDeviceManagerService(ISyncDeviceSpecifications deviceSpecifications, ISyncDiscoveryService discoveryService)
        {
            OnDeviceUpdated += (device) => {};
            OnDeviceAdded += (device) => {};
            OnDeviceRemoved += (device) => {};

            _devices = new List<SyncDevice>();
            _deviceSpecifications = deviceSpecifications;
            _discoveryService = discoveryService;
            _discoveryService.OnDeviceFound += HandleOnDeviceFound;
            _discoveryService.OnDiscoveryProgress += HandleOnDiscoveryProgress;
            _discoveryService.OnDiscoveryEnded += HandleOnDiscoveryEnded;

            _webClient = new WebClientTimeout(3000);
            _webClient.DownloadStringCompleted += HandleDownloadStringCompleted;

            InitializeLooper();
        }

        private void HandleDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Cancelled)
                return;

            if (e.Error != null)
                return;
        }

        private void InitializeLooper()
        {
            var thread = new Thread(new ThreadStart(() =>
                {
                    // Try not to iterate through a collection that can be modified
                    int a = 0;
                    while (true)
                    {
                        SyncDevice item = null;
                        lock(_locker)
                        {
                            //Console.WriteLine("SyncDeviceManagerService - a: {0} devices.Count: {1}", a, _devices.Count);
                            item = _devices.Count - 1 >= a ? _devices[a] : null;
                        }

                        if(item != null)
                        {
                            // Check timestamp; do not update status more often than 5 seconds
                            if(DateTime.Now - item.LastUpdated > TimeSpan.FromSeconds(5))
                            {
                                // Update time stamp even though the call might fail
                                item.LastUpdated = DateTime.Now;

                                string url = string.Format("{0}api/player", item.Url);
                                Console.WriteLine("SyncDeviceManagerService - Downloading player status - url: {0}", url);
                                try
                                {
                                    string json = _webClient.DownloadString(url);
                                    var metadata = JsonConvert.DeserializeObject<PlayerMetadata>(json);
                                    item.PlayerMetadata = metadata;
                                    Console.WriteLine("SyncDeviceManagerService - Downloaded player status successfully! json: {0}", json);
                                    OnDeviceUpdated(item);
                                }
                                catch(Exception ex)
                                {
                                    Console.WriteLine("SyncDeviceManagerService - Error downloading player status: {0}", ex);
                                }
                            }
                            else
                            {
                                //Console.WriteLine("SyncDeviceManagerService - No need to update item {0}", a);
                            }

                            // Move to next device
                            a++;
                        }
                        else
                        {
                            // Move back to start
                            a = 0;
                        }

                        // only for async
//                        if(!_webClient.IsBusy)

                        Thread.Sleep(250);
                    }
                }));
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void Start()
        {
            // 1) Get list of device from persistence store (this service doesn't have access to config though)
            // 2) Start looper to check device status from every device in the list
            // 3) Start discovery service in parallel

            // Search for devices in subnet
            string ip = _deviceSpecifications.GetIPAddress();
            var split = ip.Split('.');
            string baseIP = split[0] + "." + split[1] + "." + split[2];

            _discoveryService.SearchForDevices(baseIP);
        }

        public void Start(List<SyncDevice> devices)
        {
            lock (_locker)
            {
                _devices = devices;
            }
            Start();
        }

        public void Stop()
        {
            if (_discoveryService.IsRunning)
                _discoveryService.Cancel();
        }

        public void AddDevice(SyncDevice device)
        {
            lock (_locker)
            {
                var deviceFound = _devices.FirstOrDefault(x => x.Url == device.Url);
                if (deviceFound == null)
                    _devices.Add(device);
            }
            OnDeviceAdded(device);
        }

        public void RemoveDevice(SyncDevice device)
        {
            lock (_locker)
            {
                _devices.Remove(device);
            }
            OnDeviceRemoved(device);
        }

        private void HandleOnDeviceFound(SyncDevice deviceFound)
        {
            Console.WriteLine("SyncDeviceManagerService - HandleOnDeviceFound - deviceName: {0} url: {1}", deviceFound.Name, deviceFound.Url);
            AddDevice(deviceFound);
        }

        private void HandleOnDiscoveryProgress(float percentageDone, string status)
        {
            //Console.WriteLine("SyncDeviceManagerService - HandleOnDiscoveryProgress - percentageDone: {0} status: {1}", percentageDone, status);
        }

        private void HandleOnDiscoveryEnded(IEnumerable<SyncDevice> devices)
        {
            Console.WriteLine("SyncDeviceManagerService - HandleOnDiscoveryEnded devices.Count: {0}", devices.Count());
            // TODO: Start again?
        }

    }
}
