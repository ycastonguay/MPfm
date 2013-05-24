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
using MPfm.Core.Network;
using MPfm.Library.Objects;
using MPfm.Library.Services.Interfaces;

namespace MPfm.Library.Services
{
    public class SyncDiscoveryService : ISyncDiscoveryService
    {
        public int Port { get; private set; }

        public delegate void DeviceFound(SyncDevice device);
        /// <summary>
        /// The OnBPMDetected event is triggered when the current BPM has been deteted or has changed.
        /// </summary>
        public event DeviceFound OnDeviceFound;

        public SyncDiscoveryService()
        {
            Port = 53551;
        }

        public void SearchForDevices()
        {
            Task.Factory.StartNew(() => {
                var ips = IPAddressRangeFinder.GetIPRange(IPAddress.Parse("192.168.1.100"), IPAddress.Parse("192.168.1.255")).ToList();
                ConcurrentBag<string> validIps = new ConcurrentBag<string>();
                Parallel.For(1, ips.Count, (index, state) => {
                    try
                    {
                        Console.WriteLine("Pinging {0}...", ips[index]);
                        WebClientTimeout client = new WebClientTimeout(500); // maybe 100 is too short!
                        string content = client.DownloadString(string.Format("http://{0}:{1}/sessionsappversion", ips[index], Port));
                        Console.WriteLine("Got version from {0}: {1}", ips[index], content);
                        if(content.ToUpper() == SyncListenerService.SyncVersionId.ToUpper())
                        {
                            validIps.Add(ips[index]);
                            if(OnDeviceFound != null)
                                OnDeviceFound(new SyncDevice(){
                                });
                            Console.WriteLine("The following host is available: {0}", ips[index]);
                        }
                    }
                    catch(Exception ex)
                    {
                        // Ignore IP address
                    }
                });
            });
        }
    }
}
