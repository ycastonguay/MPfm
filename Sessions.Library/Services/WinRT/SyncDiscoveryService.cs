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

#if WINDOWSSTORE || WINDOWS_PHONE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MPfm.Core;
using MPfm.Library.Objects;
using MPfm.Library.Services.Interfaces;

#if WINDOWS_PHONE
using PortableTPL;
#else
using CancellationTokenSource = System.Threading.CancellationTokenSource;
using Task = System.Threading.Tasks.Task;
#endif

namespace MPfm.Library.Services
{
    public class SyncDiscoveryService : ISyncDiscoveryService
    {
        private readonly object _lock;
        private bool _isCancelling = false;
        private CancellationTokenSource _cancellationTokenSource = null;
        private HttpClient _httpClient;
        private System.Threading.Tasks.Task _currentTask;
        private List<SyncDevice> _devices;

        public bool IsRunning { get; private set; }
        public int Port { get; private set; }

        public event DiscoveryProgress OnDiscoveryProgress;
        public event DeviceFound OnDeviceFound;
        public event DiscoveryEnded OnDiscoveryEnded;

        public SyncDiscoveryService()
        {
            _lock = new object();
            Port = 53551;
            _devices = new List<SyncDevice>();
            _httpClient = new HttpClient();
            _httpClient.Timeout = new TimeSpan(0, 0, 0, 0, 800);
        }

        public List<SyncDevice> GetDeviceList()
        {
            return _devices.ToList();
        }

        /// <summary>
        /// Searches for the 50 most common IP addresses in a subnet (i.e. 192.168.1.0 to 192.168.1.25, 192.168.1.100 to 192.168.1.125).
        /// Searches for uncommon IP addresses in a subnet (i.e. 192.168.1.26 to 192.168.1.99, 192.168.1.126 to 192.168.1.255).
        /// </summary>
        /// <param name="baseIP">Base IP address (i.e. 192.168.1)</param>
        public void SearchForDevices(string baseIP)
        {
            Tracing.Log("SyncDiscoveryService - SearchForDevices - Searching for common ips in {0}.*", baseIP);
            var commonIPs = new List<string>();
            for(int a = 0; a < 25; a++)
                commonIPs.Add(string.Format("{0}.{1}", baseIP, a));
            for (int a = 100; a < 120; a++)
                commonIPs.Add(string.Format("{0}.{1}", baseIP, a));
            SearchForDevices(commonIPs, (commonDevices) =>
            {
                Tracing.Log("SyncDiscoveryService - SearchForDevices - Searching for uncommon ips in {0}.*", baseIP);
                var uncommonIPs = new List<string>();
                for (int a = 26; a < 99; a++)
                    uncommonIPs.Add(string.Format("{0}.{1}", baseIP, a));
                for (int a = 126; a < 255; a++)
                    uncommonIPs.Add(string.Format("{0}.{1}", baseIP, a));
                SearchForDevices(uncommonIPs, (uncommonDevices) =>
                {
                    Tracing.Log("SyncDiscoveryService - SearchForDevices - Discovery ended!");
                    _isCancelling = false;
                    var allDevices = new List<SyncDevice>();
                    allDevices.AddRange(commonDevices);
                    allDevices.AddRange(uncommonDevices);
                    if (OnDiscoveryEnded != null)
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
            SearchForDevices(ips, (devices) =>
            {
                if (OnDiscoveryEnded != null)
                    OnDiscoveryEnded(devices);
            });
        }

        private void SearchForDevices(List<string> ips, Action<List<SyncDevice>> actionDiscoveryEnded)
        {            
            int ipCount = 0;
            _cancellationTokenSource = new CancellationTokenSource();
            ParallelOptions parallelOptions = new ParallelOptions();
            parallelOptions.CancellationToken = _cancellationTokenSource.Token;

#if !WINDOWS_PHONE
            parallelOptions.MaxDegreeOfParallelism = 2; //System.Environment.ProcessorCount; // Not available on WP8
#endif
            _currentTask = System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                var devices = new List<SyncDevice>();
                try
                {
                    Tracing.Log("SyncDiscoveryService - SearchForDevices - processorCount: {0}", System.Environment.ProcessorCount);
                    IsRunning = true;
                    Parallel.For(1, ips.Count, parallelOptions, (index) =>
                    {
                        try
                        {
                            Tracing.Log("SyncDiscoveryService - Pinging {0}...", ips[index]);
                            string url = string.Format("http://{0}:{1}/sessionsapp.version", ips[index], Port);

                            //string content = await _httpClient.GetStringAsync(url); // This seems to pile up tasks until they hit await...
                            var task = _httpClient.GetStringAsync(url);
                            string content = task.Result; // This is the good way to use tasks and async properly in Parallel.For

                            Tracing.Log("SyncDiscoveryService - Got version from {0}: {1}", ips[index], content);
                            var device = XmlSerialization.Deserialize<SyncDevice>(content);
                            if (device.SyncVersionId.ToUpper() == SyncListenerService.SyncVersionId.ToUpper())
                            {
                                device.Url = String.Format("http://{0}:{1}/", ips[index], Port);
                                devices.Add(device);

                                lock (_lock)
                                {
                                    _devices.Add(device);
                                }

                                Tracing.Log("SyncDiscoveryService - Raising OnDeviceFound event...");
                                if (OnDeviceFound != null)
                                    OnDeviceFound(device);
                                Tracing.Log("SyncDiscoveryService - The following host is available: {0}", ips[index]);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Ignore IP address
                            //Tracing.Log("SyncDiscoveryService - SearchForDevices - Parallel.For - Exception: {0}", ex);
                        }
                        finally
                        {
                            ipCount++;
                            float percentageDone = ((float)ipCount / (float)ips.Count) * 100;
                            if (OnDiscoveryProgress != null)
                                OnDiscoveryProgress(percentageDone, String.Format("Finding devices on local network ({0:0}%)", percentageDone));
                        }
                    });

                    IsRunning = false;

                    Tracing.Log("SyncDiscoveryService - Discovery done!");
                    if (actionDiscoveryEnded != null)
                        actionDiscoveryEnded(devices);
                }
                catch (System.OperationCanceledException ex)
                {
                    Tracing.Log("SyncDiscoveryService - SearchForDevices - OperationCanceledException: {0}", ex);
                    IsRunning = false;

                    if (actionDiscoveryEnded != null)
                        actionDiscoveryEnded(new List<SyncDevice>());
                }
                catch (Exception ex)
                {
                    Tracing.Log("SyncDiscoveryService - SearchForDevices - Exception: {0}", ex);
                    IsRunning = false;
                }

                _devices = devices.ToList();
            });
        }

        /// <summary>
        /// Cancels the discovery process.
        /// </summary>
        public void Cancel()
        {
            Tracing.Log("SyncDiscoveryService - Cancelling process...");
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
#endif