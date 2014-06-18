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
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MPfm.Core;
using MPfm.Core.Helpers;
using MPfm.Core.Network;
using MPfm.Player.Objects;
using Newtonsoft.Json;
using MPfm.Library.Objects;
using MPfm.Library.Services.Interfaces;

namespace MPfm.Library.Services
{
    public class SyncDeviceManagerService : ISyncDeviceManagerService
    {
        private readonly object _locker = new object();
        private readonly ISyncDiscoveryService _discoveryService;
        private readonly ISyncDeviceSpecifications _deviceSpecifications;
        private readonly WebClientTimeout _webClient;
        private readonly WebClientTimeout _webClientRemote;
        private bool _shouldStopLooper = false;
        private List<SyncDevice> _devices;

        public event StatusUpdated OnStatusUpdated;
        public event DeviceUpdated OnDeviceUpdated;
        public event DevicesUpdated OnDevicesUpdated;
        public event DeviceUpdated OnDeviceAdded;
        public event DeviceUpdated OnDeviceRemoved;

        public SyncDeviceManagerService(ISyncDeviceSpecifications deviceSpecifications, ISyncDiscoveryService discoveryService)
        {
            Console.WriteLine("SyncDeviceManagerService - Starting...");
            OnStatusUpdated += (status) => {};
            OnDeviceUpdated += (device) => {};
            OnDevicesUpdated += (devices) => {};
            OnDeviceAdded += (device) => {};
            OnDeviceRemoved += (device) => {};

            _devices = new List<SyncDevice>();
            _deviceSpecifications = deviceSpecifications;
            _discoveryService = discoveryService;
            _discoveryService.OnDeviceFound += HandleOnDeviceFound;
            _discoveryService.OnDiscoveryProgress += HandleOnDiscoveryProgress;
            _webClient = new WebClientTimeout(3000);
            _webClientRemote = new WebClientTimeout(3000);
        }

        public IEnumerable<SyncDevice> GetDeviceList()
        {
            return _devices.ToList();
        }

        private void LoadDeviceStoreFromDisk()
        {
            lock (_locker)
            {
                try
                {
                    string json = string.Empty;
                    using (var reader = File.OpenText(PathHelper.DeviceStoreFilePath))
                    {
                        json = reader.ReadToEnd();
                    }
                    var store = JsonConvert.DeserializeObject<SyncDeviceStore>(json);
                    _devices = store.Devices;
                }
                catch(Exception ex)
                {
                    _devices = new List<SyncDevice>();
                    Tracing.Log("SyncDeviceManagerService - Failed to load device store from disk: {0}", ex);
                }
            }
        }

        private void SaveDeviceStoreToDisk()
        {
            lock (_locker)
            {
                try
                {
                    var store = new SyncDeviceStore(_devices);
                    string json = JsonConvert.SerializeObject(store);
                    using (var writer = new StreamWriter(PathHelper.DeviceStoreFilePath))
                    {
                        writer.Write(json);
                    }
                }
                catch(Exception ex)
                {
                    _devices = new List<SyncDevice>();
                    Tracing.Log("SyncDeviceManagerService - Failed to save device store to disk: {0}", ex);
                }
            }
        }

        private void StartLooper()
        {
            var thread = new Thread(new ThreadStart(() =>
                {
                    // Try not to iterate through a collection that can be modified
                    int a = 0;
                    while (true)
                    {
                        // Check for cancelling
                        if (_shouldStopLooper)
                        {
                            _shouldStopLooper = false;
                            break;
                        }

                        SyncDevice item = null;
                        lock(_locker)
                        {
                            //Console.WriteLine("SyncDeviceManagerService - a: {0} devices.Count: {1}", a, _devices.Count);
                            item = _devices.Count - 1 >= a ? _devices[a] : null;
                        }

                        if(item != null)
                        {
                            // Check timestamp; do not update status more often than 5 seconds
                            if(DateTime.Now - item.LastTentativeUpdate > TimeSpan.FromSeconds(5))
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
                                    item.LastTentativeUpdate = DateTime.Now;
                                    item.IsOnline = true;

                                    //Console.WriteLine("SyncDeviceManagerService - Downloaded player status successfully! json: {0}", json);
                                    OnStatusUpdated(string.Format("Updating status from {0}... finished!", item.Name));
                                    OnDeviceUpdated(item);
                                }
                                catch(Exception ex)
                                {
                                    item.LastTentativeUpdate = DateTime.Now;
                                    item.IsOnline = false;
                                    Console.WriteLine("SyncDeviceManagerService - Error downloading player status: {0}", ex);
                                    OnDeviceUpdated(item);
                                }

                                if(item.PlayerMetadata != null)
                                {
                                    string albumArtKey = string.Format("{0}_{1}", item.PlayerMetadata.CurrentAudioFile.ArtistName, item.PlayerMetadata.CurrentAudioFile.AlbumTitle);
                                    if(!string.Equals(albumArtKey, item.AlbumArtKey, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        string albumArtUrl = string.Format("{0}api/albumart/{1}", item.Url, item.PlayerMetadata.CurrentAudioFile.Id);
                                        OnStatusUpdated(string.Format("Downloading album art from {0}...", item.Name));
                                        Console.WriteLine("SyncDeviceManagerService - Downloading album art - url: {0}", albumArtUrl);
                                        try
                                        {
                                            byte[] data = _webClient.DownloadData(albumArtUrl);
                                            item.AlbumArt = data;
                                            item.AlbumArtKey = albumArtKey;

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

                        SaveDeviceStoreToDisk();
                        Thread.Sleep(500);
                    }
                }));
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void Start()
        {
            LoadDeviceStoreFromDisk();
            OnDevicesUpdated(_devices);
            StartLooper();
            StartDiscovery();
        }

        public void Stop()
        {
            _shouldStopLooper = true;
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
                    OnStatusUpdated(string.Format("New device found: {0}", device.Name));
                }
            }

            if(isDeviceAdded)
                OnDeviceAdded(device);
        }

        public void AddDeviceFromUrl(string url)
        {
            _discoveryService.AddDeviceToSearchList(url);
        }

        public void RemoveDevice(SyncDevice device)
        {
            lock (_locker)
            {
                _devices.Remove(device);
            }
            OnDeviceRemoved(device);
        }

        private void RemoteCommand(SyncDevice device, string command)
        {
            if (device == null || string.IsNullOrEmpty(device.Url))
                return;

            string remoteUrl = string.Format("{0}api/remote/{1}", device.Url, command);
            _webClientRemote.DownloadStringAsync(new Uri(remoteUrl));
        }

        public void RemotePlay(SyncDevice device)
        {
            RemoteCommand(device, "play");
        }

        public void RemotePause(SyncDevice device)
        {
            RemoteCommand(device, "pause");
        }

        public void RemoteStop(SyncDevice device)
        {
            RemoteCommand(device, "stop");
        }

        public void RemotePrevious(SyncDevice device)
        {
            RemoteCommand(device, "previous");
        }

        public void RemoteNext(SyncDevice device)
        {
            RemoteCommand(device, "next");
        }

        public void RemoteRepeat(SyncDevice device)
        {
            RemoteCommand(device, "repeat");
        }

        public void RemoteShuffle(SyncDevice device)
        {
            RemoteCommand(device, "shuffle");
        }

        private void StartDiscovery()
        {
            // Search for devices in subnet
            string ip = _deviceSpecifications.GetIPAddress();
            var split = ip.Split('.');
            string baseIP = split[0] + "." + split[1] + "." + split[2];

            OnStatusUpdated("Finding devices on your local network...");
            _discoveryService.Start();
            _discoveryService.AddToSearchList(baseIP);
        }

        private void HandleOnDeviceFound(SyncDevice deviceFound)
        {
            // Create another task because this event is inside a task from the discovery service and can affect timeout
            Task.Factory.StartNew(() =>
            {
                //Console.WriteLine("SyncDeviceManagerService - HandleOnDeviceFound - deviceName: {0} url: {1}", deviceFound.Name, deviceFound.Url);
                AddDevice(deviceFound);
            });
        }

        private void HandleOnDiscoveryProgress(float percentageDone, string status)
        {
            //OnStatusUpdated(string.Format("Discovery status: {0} {1}", percentageDone, status));
            //Console.WriteLine("SyncDeviceManagerService - HandleOnDiscoveryProgress - percentageDone: {0} status: {1}", percentageDone, status);
        }

        private class SyncDeviceStore
        {
            public List<SyncDevice> Devices { get; set; }

            public SyncDeviceStore(List<SyncDevice> devices)
            {
                Devices = devices;
            }
        }
    }
}
