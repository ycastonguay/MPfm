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
using Android.Content.Res;
using Android.Graphics;
using Android.Net.Nsd;

namespace MPfm.Android.Classes.Listeners
{
    public class DiscoveryListener : Java.Lang.Object, NsdManager.IDiscoveryListener
    {
        private readonly NsdManager _nsdManager;

        public DiscoveryListener(NsdManager nsdManager)
        {
            _nsdManager = nsdManager;
        }

        public void OnDiscoveryStarted(string serviceType)
        {
            Console.WriteLine("DiscoveryListener - OnDiscoveryStarted - serviceType: {0}", serviceType);
        }

        public void OnDiscoveryStopped(string serviceType)
        {
            Console.WriteLine("DiscoveryListener - OnDiscoveryStopped - serviceType: {0}", serviceType);
        }

        public void OnServiceFound(NsdServiceInfo serviceInfo)
        {
            Console.WriteLine("DiscoveryListener - OnServiceFound - serviceName: {0} serviceType: {1} port: {2} hostAddress: {3}", serviceInfo.ServiceName, serviceInfo.ServiceType, serviceInfo.Port, serviceInfo.Host == null ? "null" : serviceInfo.Host.HostAddress);
            //if(serviceInfo.ServiceType.)
        }

        public void OnServiceLost(NsdServiceInfo serviceInfo)
        {
            Console.WriteLine("DiscoveryListener - OnServiceLost - serviceName: {0} serviceType: {1} port: {2} hostAddress: {3}", serviceInfo.ServiceName, serviceInfo.ServiceType, serviceInfo.Port, serviceInfo.Host == null ? "null" : serviceInfo.Host.HostAddress);
        }

        public void OnStartDiscoveryFailed(string serviceType, NsdFailure errorCode)
        {
            Console.WriteLine("DiscoveryListener - OnStartDiscoveryFailed - serviceType: {0} errorCode: {1}", serviceType, errorCode.ToString());
            _nsdManager.StopServiceDiscovery(this);
        }

        public void OnStopDiscoveryFailed(string serviceType, NsdFailure errorCode)
        {
            Console.WriteLine("DiscoveryListener - OnStopDiscoveryFailed - serviceType: {0} errorCode: {1}", serviceType, errorCode.ToString());
            _nsdManager.StopServiceDiscovery(this);
        }
    }
}