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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using MPfm.Core;
using MPfm.Core.Network;
using MPfm.Library.Objects;
using MPfm.Library.Services.Interfaces;

namespace MPfm.Library.Services
{
    public class SyncDiscoveryService : ISyncDiscoveryService
    {
        public int Port { get; private set; }

        public delegate void DeviceFound(SyncDevice device);
        public event DeviceFound OnDeviceFound;

        public delegate void DiscoveryEnded(IEnumerable<SyncDevice> devices);
        public event DiscoveryEnded OnDiscoveryEnded;

        public SyncDiscoveryService()
        {
            Port = 53551;
        }

        /// <summary>
        /// Searches for the 50 most common IP addresses in a subnet (i.e. 192.168.1.0 to 192.168.1.25, 192.168.1.100 to 192.168.1.125).
        /// Searches for uncommon IP addresses in a subnet (i.e. 192.168.1.26 to 192.168.1.99, 192.168.1.126 to 192.168.1.255).
        /// </summary>
        /// <param name="baseIP">Base IP address (i.e. 192.168.1)</param>
        public void SearchForDevices(string baseIP)
        {
            Console.WriteLine("SyncDiscoveryService - SearchForDevices - Searching for common ips in {0}.*", baseIP);
            var commonIPs = new List<string>();
            commonIPs.AddRange(IPAddressRangeFinder.GetIPRange(IPAddress.Parse(baseIP + ".0"), IPAddress.Parse(baseIP + ".25")).ToList());
            commonIPs.AddRange(IPAddressRangeFinder.GetIPRange(IPAddress.Parse(baseIP + ".100"), IPAddress.Parse(baseIP + ".125")).ToList());
            SearchForDevices(commonIPs, (commonDevices) => {
                Console.WriteLine("SyncDiscoveryService - SearchForDevices - Searching for uncommon ips in {0}.*", baseIP);
                var uncommonIPs = new List<string>();
                uncommonIPs.AddRange(IPAddressRangeFinder.GetIPRange(IPAddress.Parse(baseIP + ".26"), IPAddress.Parse(baseIP + ".99")).ToList());
                uncommonIPs.AddRange(IPAddressRangeFinder.GetIPRange(IPAddress.Parse(baseIP + ".126"), IPAddress.Parse(baseIP + ".255")).ToList());
                SearchForDevices(uncommonIPs, (uncommonDevices) => {
                    Console.WriteLine("SyncDiscoveryService - SearchForDevices - Discovery ended!");
                    var allDevices = new List<SyncDevice>();
                    allDevices.AddRange(commonDevices);
                    allDevices.AddRange(uncommonDevices);
                    if(OnDiscoveryEnded != null)
                        OnDiscoveryEnded(allDevices);
                });
            });
        }

        /// <summary>
        /// Searches for devices in the IP addresses passed in parameter.
        /// </summary>
        /// <param name="ips">IP addresses to search</param>
        public void SearchForDevices(List<string> ips)
        {
            SearchForDevices(ips, (devices) => {
                if(OnDiscoveryEnded != null)
                    OnDiscoveryEnded(devices);
            });
        }

        /// <summary>
        /// Searches for devices in the IP addresses passed in parameter.
        /// </summary>
        /// <param name="ips">IP addresses to search</param>
        /// <param name="actionDiscoveryEnded">This action will be triggered when the discovery has ended</param> 
        private void SearchForDevices(List<string> ips, Action<ConcurrentBag<SyncDevice>> actionDiscoveryEnded)
        {
            Task.Factory.StartNew(() => {
                ConcurrentBag<SyncDevice> devices = new ConcurrentBag<SyncDevice>();
                Parallel.For(1, ips.Count, (index, state) => {
                    try
                    {
                        Console.WriteLine("SyncDiscoveryService - Pinging {0}...", ips[index]);
                        WebClientTimeout client = new WebClientTimeout(800);
                        string content = client.DownloadString(string.Format("http://{0}:{1}/sessionsapp.version", ips[index], Port));
                        Console.WriteLine("SyncDiscoveryService - Got version from {0}: {1}", ips[index], content);
                        var device = XmlSerialization.Deserialize<SyncDevice>(content);
                        if(device.SyncVersionId.ToUpper() == SyncListenerService.SyncVersionId.ToUpper())
                        {
                            device.Url = String.Format("http://{0}:{1}/", ips[index], Port);
                            devices.Add(device);
                            Console.WriteLine("SyncDiscoveryService - Raising OnDeviceFound event...");
                            if(OnDeviceFound != null)
                                OnDeviceFound(device);
                            Console.WriteLine("SyncDiscoveryService - The following host is available: {0}", ips[index]);
                        }
                    }
                    catch(Exception ex)
                    {
                        // Ignore IP address
                    }
                });

                Console.WriteLine("SyncDiscoveryService - Discovery done!");
                if(actionDiscoveryEnded != null)
                    actionDiscoveryEnded(devices);
            });
        }
    }
}
