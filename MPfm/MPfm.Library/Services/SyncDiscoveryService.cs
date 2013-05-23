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
            var ips = IPAddressRangeFinder.GetIPRange(IPAddress.Parse("192.168.1.100"), IPAddress.Parse("192.168.1.255"));
            var validIps = new List<string>();



            //Parallel.For(

//            Task.Factory.StartNew(() => {
//                foreach(var ip in ips)
//                {
//                    bool successful = false;
//                    //lblStatus.StringValue = String.Format("Querying {0}...", ip);
//                    Console.WriteLine("Querying {0}...", ip);
//                    try
//                    {
//                        var ping = new Ping();
//                        ping.PingCompleted += HandlePingCompleted;
//
//                        //ping.SendAsync(ip, 100, ip);
//                        var reply = ping.Send(IPAddress.Parse(ip), 1000);
//                        Console.WriteLine("Ping result - reply_address: {0} reply_roundtripTime: {1} reply_status: {2}", reply.Address.ToString(), reply.RoundtripTime, reply.Status);
////                        var client = new WebClientTimeout(3000);
////                        string data = client.DownloadString("http://" + ip + ":8080/hello");
////                        if(!String.IsNullOrEmpty(data))
////                        {
////                            successful = true;
////                            Console.WriteLine("Successfully connected to {0}", ip);
////                        }
//                    }
//                    catch(Exception ex)
//                    {
//                        Console.WriteLine("Failed to connect to {0}: {1}", ip, ex);
//                    }
//
//                    if(successful)
//                        validIps.Add(ip);
//                }
//            });
        }

        private void HandlePingCompleted(object sender, PingCompletedEventArgs e)
        {
            Console.WriteLine("Ping result - ip: {0} reply_address: {1} reply_roundtripTime: {2} reply_status: {3} cancelled: {4}", e.UserState.ToString(), e.Reply.Address.ToString(), e.Reply.RoundtripTime, e.Reply.Status, e.Cancelled);
        }
    }
}
