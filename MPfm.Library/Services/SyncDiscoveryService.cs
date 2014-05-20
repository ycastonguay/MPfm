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
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using MPfm.Core;
using MPfm.Core.Network;
using MPfm.Library.Objects;
using MPfm.Library.Services.Interfaces;

namespace MPfm.Library.Services
{
    public class SyncDiscoveryService : ISyncDiscoveryService
    {
        public const int MaximumNumberOfTasks = 2;
        private readonly object _lock = new object();
        private readonly object _lockerIps = new object();
        private bool _isCancelling = false;
        private Task _currentTask;
        private CancellationTokenSource _cancellationTokenSource = null;
        private List<SyncDevice> _devices;
        private List<string> _ipsToSearch;
        private int _numberOfTasksRunning;

        public bool IsRunning { get; private set; }
        public int Port { get; private set; }

        public event DiscoveryProgress OnDiscoveryProgress;
        public event DeviceFound OnDeviceFound;

        public SyncDiscoveryService()
        {
            _devices = new List<SyncDevice>();
            _ipsToSearch = new List<string>();
            Port = 53551;
        }

        public void AddDeviceToSearchList(string ip)
        {
            lock (_lock)
            {
                _ipsToSearch.Add(ip);
            }
        }

        /// <summary>
        /// Searches for devices in the IP addresses passed in parameter.
        /// </summary>
        /// <param name="ips">IP addresses to search</param>
        public void AddDevicesToSearchList(List<string> ips)
        {
            lock (_lock)
            {
                _ipsToSearch.AddRange(ips);
            }
        }

        /// <summary>
        /// Searches for the 50 most common IP addresses in a subnet (i.e. 192.168.1.0 to 192.168.1.25, 192.168.1.100 to 192.168.1.125).
        /// Searches for uncommon IP addresses in a subnet (i.e. 192.168.1.26 to 192.168.1.99, 192.168.1.126 to 192.168.1.255).
        /// </summary>
        /// <param name="baseIP">Base IP address (i.e. 192.168.1)</param>
        public void AddToSearchList(string baseIP)
        {
            //Console.WriteLine("SyncDiscoveryService - SearchForDevices - Searching for common ips in {0}.*", baseIP);
            var commonIPs = new List<string>();
            commonIPs.AddRange(IPAddressRangeFinder.GetIPRange(IPAddress.Parse(baseIP + ".0"), IPAddress.Parse(baseIP + ".25")).ToList());
            commonIPs.AddRange(IPAddressRangeFinder.GetIPRange(IPAddress.Parse(baseIP + ".100"), IPAddress.Parse(baseIP + ".125")).ToList());

            lock (_lock)
            {
                _ipsToSearch.AddRange(commonIPs);
            }

//                //Console.WriteLine("SyncDiscoveryService - SearchForDevices - Searching for uncommon ips in {0}.*", baseIP);
////                var uncommonIPs = new List<string>();
////                uncommonIPs.AddRange(IPAddressRangeFinder.GetIPRange(IPAddress.Parse(baseIP + ".26"), IPAddress.Parse(baseIP + ".99")).ToList());
////                uncommonIPs.AddRange(IPAddressRangeFinder.GetIPRange(IPAddress.Parse(baseIP + ".126"), IPAddress.Parse(baseIP + ".255")).ToList());
////                SearchForDevices(uncommonIPs, (uncommonDevices) => {
////                    Console.WriteLine("SyncDiscoveryService - SearchForDevices - Discovery ended!");
////                    _isCancelling = false;
////                    var allDevices = new List<SyncDevice>();
////                    allDevices.AddRange(commonDevices);
////                    allDevices.AddRange(uncommonDevices);
////                    if(OnDiscoveryEnded != null)
////                        OnDiscoveryEnded(allDevices);
////                });
        }

        public void Start()
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    if (_isCancelling)
                    {
                        _isCancelling = false;
                        break;
                    }

                    //Console.WriteLine("WaveFormCacheService - BitmapRequestProcessLoop - Loop - requests.Count: {0} numberOfBitmapTasksRunning: {1}", _requests.Count, _numberOfBitmapTasksRunning);
                    var ipsToProcess = new List<string>();
                    lock (_lockerIps)
                    {
                        while (_ipsToSearch.Count > 0 && _numberOfTasksRunning < MaximumNumberOfTasks)
                        {
                            //int index = 0; // FIFO
                            int index = _ipsToSearch.Count - 1; // LIFO
                            _numberOfTasksRunning++;
                            var request = _ipsToSearch[index];
                            ipsToProcess.Add(request);
                            _ipsToSearch.RemoveAt(index);
                        }
                    }

                    foreach (var ip in ipsToProcess)
                    {
                        try
                        {
                            if (OnDiscoveryProgress != null)
                                OnDiscoveryProgress(0, string.Format("Pinging ip {0}...", ip));

                            Console.WriteLine("SyncDiscoveryService - Start - Pinging ip {0}...", ip);
                            PingDevice(ip);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("SyncDiscoveryService - Start - Failed to ping ip {0}: {1}", ip, ex);
                        }
                        //Console.WriteLine("WaveFormCacheService - BitmapRequestProcessLoop - Processing bitmap request - boundsBitmap: {0} boundsWaveForm: {1} zoom: {2} numberOfBitmapTasksRunning: {3}", request.BoundsBitmap, request.BoundsWaveForm, request.Zoom, _numberOfBitmapTasksRunning);
                        //_waveFormRenderingService.RequestBitmap(request); // ThreadQueueWorkItem will manage a thread pool
                    }

                    Thread.Sleep(50);
                }
            }));
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void PingDevice(string ip)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(PingDeviceInternal), ip);
        }

        private void PingDeviceInternal(Object stateInfo)
        {
            string ip = (string) stateInfo;
            Console.WriteLine("SyncDiscoveryService - Pinging {0}...", ip);
            WebClientTimeout client = new WebClientTimeout(800);
            string content = client.DownloadString(string.Format("http://{0}:{1}/sessionsapp.version", ip, Port));

            //Console.WriteLine("SyncDiscoveryService - Got version from {0}: {1}", ips[index], content);
            var device = XmlSerialization.Deserialize<SyncDevice>(content);
            if (device.SyncVersionId.ToUpper() == SyncListenerService.SyncVersionId.ToUpper())
            {
                device.Url = String.Format("http://{0}:{1}/", ip, Port);

                lock (_lock)
                {
                    _devices.Add(device);
                }

                //Console.WriteLine("SyncDiscoveryService - The following host is available: {0}", ips[index]);
                if (OnDeviceFound != null)
                    OnDeviceFound(device);
            }
        }

        /// <summary>
        /// Cancels the discovery process.
        /// </summary>
        public void Cancel()
        {
            Console.WriteLine("SyncDiscoveryService - Cancelling process...");
            if (IsRunning)
            {
                if (_cancellationTokenSource != null)
                {
                    _isCancelling = true;
                    _cancellationTokenSource.Cancel();
                }
            }
        }
    }
}
