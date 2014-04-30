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
using System.Threading.Tasks;

namespace MPfm.Library.Services
{
    public class SyncDeviceManagerService : ISyncDeviceManagerService
    {
        private readonly object _locker = new object();
        private readonly ISyncDiscoveryService _discoveryService;
        private readonly ISyncDeviceSpecifications _deviceSpecifications;
        private readonly WebClientTimeout _webClient;
        private List<SyncDevice> _devices;

        public event StatusUpdated OnStatusUpdated;
        public event DeviceUpdated OnDeviceUpdated;
        public event DeviceUpdated OnDeviceAdded;
        public event DeviceUpdated OnDeviceRemoved;

        public SyncDeviceManagerService(ISyncDeviceSpecifications deviceSpecifications, ISyncDiscoveryService discoveryService)
        {
            OnStatusUpdated += (status) => {};
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

            InitializeLooper();
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
                            if(DateTime.Now - item.LastUpdated > TimeSpan.FromSeconds(15))
                            {
                                string url = string.Format("{0}api/player", item.Url);
                                OnStatusUpdated(string.Format("Updating status from {0}...", item.Name));
                                Console.WriteLine("SyncDeviceManagerService - Downloading player status - url: {0}", url);
                                try
                                {
                                    string json = _webClient.DownloadString(url);
                                    var metadata = JsonConvert.DeserializeObject<PlayerMetadata>(json);
                                    item.PlayerMetadata = metadata;
                                    item.LastUpdated = DateTime.Now;

                                    //Console.WriteLine("SyncDeviceManagerService - Downloaded player status successfully! json: {0}", json);
                                    OnStatusUpdated(string.Format("Updating status from {0}... finished!", item.Name));
                                    OnDeviceUpdated(item);
                                }
                                catch(Exception ex)
                                {
                                    item.LastUpdated = DateTime.Now;
                                    Console.WriteLine("SyncDeviceManagerService - Error downloading player status: {0}", ex);
                                }

                                string albumArtUrl = string.Format("{0}api/albumart/{1}", item.Url, item.PlayerMetadata.CurrentAudioFile.Id);
                                OnStatusUpdated(string.Format("Downloading album art from {0}...", item.Name));
                                Console.WriteLine("SyncDeviceManagerService - Downloading album art - url: {0}", albumArtUrl);
                                try
                                {
                                    byte[] data = _webClient.DownloadData(albumArtUrl);
                                    item.PlayerMetadata.AlbumArt = data;

                                    //Console.WriteLine("SyncDeviceManagerService - Downloaded player status successfully! json: {0}", json);
                                    OnStatusUpdated(string.Format("Downloading album art from {0}... finished!", item.Name));
                                    OnDeviceUpdated(item);
                                }
                                catch(Exception ex)
                                {
                                    item.LastUpdated = DateTime.Now;
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

                        Thread.Sleep(250);
                    }
                }));
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        // Start? this is probably the wrong term. Start would start the looper, and stop the looper.
        // the discovery service start/stop should not be controller by the user
        public void Start()
        {
            // 1) Get list of device from persistence store (this service doesn't have access to config though)
            // 2) Start looper to check device status from every device in the list
            // 3) Start discovery service in parallel
            StartDiscovery();
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
            bool isDeviceAdded = false;
            lock (_locker)
            {
                var deviceFound = _devices.FirstOrDefault(x => x.Url == device.Url);
                if (deviceFound == null)
                {
                    isDeviceAdded = true;
                    _devices.Add(device);
                }
            }

            if(isDeviceAdded)
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

        private void StartDiscovery()
        {
            // Search for devices in subnet
            string ip = _deviceSpecifications.GetIPAddress();
            var split = ip.Split('.');
            string baseIP = split[0] + "." + split[1] + "." + split[2];

            OnStatusUpdated("Finding devices on your local network...");
            _discoveryService.SearchForDevices(baseIP);
        }

        private void HandleOnDeviceFound(SyncDevice deviceFound)
        {
            //Console.WriteLine("SyncDeviceManagerService - HandleOnDeviceFound - deviceName: {0} url: {1}", deviceFound.Name, deviceFound.Url);
            OnStatusUpdated(string.Format("New device found: {0}", deviceFound.Name));
            AddDevice(deviceFound);
        }

        private void HandleOnDiscoveryProgress(float percentageDone, string status)
        {
            //OnStatusUpdated(string.Format("Discovery status: {0} {1}", percentageDone, status));
            //Console.WriteLine("SyncDeviceManagerService - HandleOnDiscoveryProgress - percentageDone: {0} status: {1}", percentageDone, status);
        }

        private void HandleOnDiscoveryEnded(IEnumerable<SyncDevice> devices)
        {
            //Console.WriteLine("SyncDeviceManagerService - HandleOnDiscoveryEnded devices.Count: {0}", devices.Count());
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(2500);
                StartDiscovery();
            });
        }
    }
}
