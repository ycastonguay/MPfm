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
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MPfm.Core;
using MPfm.Library.Objects;
using MPfm.Library.Services.Interfaces;

namespace MPfm.Library.Services
{
    public class SyncDiscoveryService : ISyncDiscoveryService
    {
        bool _isCancelling = false;
        CancellationTokenSource _cancellationTokenSource = null;
        HttpClient _httpClient;

        public bool IsRunning { get; private set; }
        public int Port { get; private set; }

        public event DiscoveryProgress OnDiscoveryProgress;
        public event DeviceFound OnDeviceFound;
        public event DiscoveryEnded OnDiscoveryEnded;

        public SyncDiscoveryService()
        {
            Port = 53551;
            _httpClient = new HttpClient();
            _httpClient.Timeout = new TimeSpan(0, 0, 0, 0, 800);
        }

        /// <summary>
        /// Searches for the 50 most common IP addresses in a subnet (i.e. 192.168.1.0 to 192.168.1.25, 192.168.1.100 to 192.168.1.125).
        /// Searches for uncommon IP addresses in a subnet (i.e. 192.168.1.26 to 192.168.1.99, 192.168.1.126 to 192.168.1.255).
        /// </summary>
        /// <param name="baseIP">Base IP address (i.e. 192.168.1)</param>
        public async void SearchForDevices(string baseIP)
        {
            Tracing.Log("SyncDiscoveryService - SearchForDevices - Searching for common ips in {0}.*", baseIP);
            var commonIPs = new List<string>();
            for(int a = 0; a < 25; a++)
                commonIPs.Add(string.Format("{0}.{1}", baseIP, a));
            for (int a = 100; a < 125; a++)
                commonIPs.Add(string.Format("{0}.{1}", baseIP, a));
            var commonDevices = await SearchForDevicesAsync(commonIPs);

            Tracing.Log("SyncDiscoveryService - SearchForDevices - Searching for uncommon ips in {0}.*", baseIP);
            var uncommonIPs = new List<string>();
            for (int a = 26; a < 99; a++)
                uncommonIPs.Add(string.Format("{0}.{1}", baseIP, a));
            for (int a = 126; a < 255; a++)
                uncommonIPs.Add(string.Format("{0}.{1}", baseIP, a));
            var uncommonDevices = await SearchForDevicesAsync(uncommonIPs);

            Tracing.Log("SyncDiscoveryService - SearchForDevices - Discovery ended!");
            _isCancelling = false;
            var allDevices = new List<SyncDevice>();
            allDevices.AddRange(commonDevices);
            allDevices.AddRange(uncommonDevices);
            if (OnDiscoveryEnded != null)
                OnDiscoveryEnded(allDevices);
        }

        /// <summary>
        /// Searches for devices in the IP addresses passed in parameter.
        /// </summary>
        /// <param name="ips">IP addresses to search</param>
        public async void SearchForDevices(List<string> ips)
        {
            var devices = await SearchForDevicesAsync(ips);
            if (OnDiscoveryEnded != null) OnDiscoveryEnded(devices);
        }

        /// <summary>
        /// Searches for devices in the IP addresses passed in parameter.
        /// </summary>
        /// <param name="ips">IP addresses to search</param>
        private async Task<List<SyncDevice>> SearchForDevicesAsync(List<string> ips)
        {
            Tracing.Log("SyncDiscoveryService - SearchForDevices - processorCount: {0}", System.Environment.ProcessorCount);
            IsRunning = true;            

            List<SyncDevice> devices = new List<SyncDevice>();
            List<Task<SyncDevice>> tasks = new List<Task<SyncDevice>>();
            for (int a = 0; a < ips.Count; a++)
            {
                try
                {
                    //Tracing.Log("SyncDiscoveryService - Pinging {0}...", ips[a]);
                    string url = string.Format("http://{0}:{1}/sessionsapp.version", ips[a], Port);
                    tasks.Add(ProcessDevice(url, ips[a]));

                    //string content = await _httpClient.GetStringAsync(url);

                    //Tracing.Log("SyncDiscoveryService - Got version from {0}: {1}", ips[a], content);
                    //var device = XmlSerialization.Deserialize<SyncDevice>(content);
                    //if (device.SyncVersionId.ToUpper() == SyncListenerService.SyncVersionId.ToUpper())
                    //{
                    //    device.Url = url;
                    //    devices.Add(device);
                    //    if (OnDeviceFound != null) OnDeviceFound(device);
                    //    Tracing.Log("SyncDiscoveryService - The following host is available: {0}", ips[a]);
                    //}
                }
                catch (Exception ex)
                {
                    // Ignore IP address
                    Tracing.Log("SyncDiscoveryService - SearchForDevices - Exception: {0}", ex);
                }
                finally
                {
                    //float percentageDone = ((float)a / (float)ips.Count) * 100;
                    //if (OnDiscoveryProgress != null)
                    //    OnDiscoveryProgress(percentageDone, String.Format("Finding devices on local network ({0:0}%)", percentageDone));
                }
            }

            int b = 0;
            while (true)
            {                
                if (b > tasks.Count - 1)
                    break;

                float percentageDone = ((float)b / (float)tasks.Count) * 100;
                if (OnDiscoveryProgress != null)
                    OnDiscoveryProgress(percentageDone, String.Format("Finding devices on local network ({0:0}%)", percentageDone));

                Tracing.Log("SyncDiscoveryService - SearchForDevicesAsync - while loop index {0} - done {1}", b, percentageDone);

                var device = await tasks[b];
                if(device != null)
                    devices.Add(device);
                b++;
            }

            IsRunning = false;

            return devices;
        }

        private async Task<SyncDevice> ProcessDevice(string url, string ip)
        {
            try
            {
                Tracing.Log("SyncDiscoveryService - ProcessDevice - Getting content... url: {0} ip:{1}", url, ip);
                string content = await _httpClient.GetStringAsync(url);
                Tracing.Log("SyncDiscoveryService - ProcessDevice - Getting content... done! url: {0} ip:{1}", url, ip);
                Tracing.Log("SyncDiscoveryService - Got version from {0}: {1}", ip, content);
                var device = XmlSerialization.Deserialize<SyncDevice>(content);
                if (device.SyncVersionId.ToUpper() == SyncListenerService.SyncVersionId.ToUpper())
                {
                    device.Url = url;
                    if (OnDeviceFound != null) OnDeviceFound(device);
                    Tracing.Log("SyncDiscoveryService - The following host is available: {0}", ip);
                }
                return device;
            }
            catch (Exception ex)
            {
                Tracing.Log("SyncDiscoveryService - ProcessDevice - Exception: {0}", ex);
            }

            return null;
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
