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
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using MPfm.Core.Network;
using MPfm.Library.Services.Interfaces;
using System.Collections.Concurrent;
using System.Linq;

namespace MPfm.Library.Services
{
    public class SyncDiscoveryService : ISyncDiscoveryService
    {
        public int Port { get; private set; }

        public SyncDiscoveryService()
        {
            Port = 8080;
        }

        public void SearchForDevices()
        {
            var ips = IPAddressRangeFinder.GetIPRange(IPAddress.Parse("192.168.1.100"), IPAddress.Parse("192.168.1.150")).ToList();
            var validIps = new List<string>();

            var ping2 = new Ping();
            var reply2 = ping2.Send("192.168.1.102", 1000);

            ConcurrentBag<double> values = new ConcurrentBag<double>();
            Parallel.For(1, ips.Count, (index, state) => {
                Console.WriteLine("Task {0}: Pinging {1}...", index, ips[index]);
                var ping = new Ping();
                var reply = ping.Send(ips[index], 1000);
                Console.WriteLine("Task {0}: Pinging {1} - received status: {2}", index, ips[index], reply.Status.ToString());
                if (reply.Status == IPStatus.Success)
                    validIps.Add(ips[index]);
            });
        }
    }
}
