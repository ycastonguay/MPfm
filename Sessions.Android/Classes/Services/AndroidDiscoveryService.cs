// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using Android.Content;
using Android.Net.Nsd;
using Android.Net.Wifi.P2p;
using Sessions.Android.Classes.Listeners;
using Sessions.Android.Classes.Receivers;

namespace Sessions.Android.Classes.Services
{
    /// <summary>
    /// This service requires API 16+ (Android 4.1+)
    /// </summary>
    public class AndroidDiscoveryService
    {
        private NsdManager _nsdManager;
        private RegistrationListener _registrationListener;
        private DiscoveryListener _discoveryListener;
        private ResolveListener _resolveListener;
        private ActionListener _actionListener;
        private PeerListListener _peerListListener;
        private IntentFilter _intentFilter;
        private WifiP2pManager _wifiManager;
        private WifiP2pManager.Channel _wifiChannel;
        private WifiDirectReceiver _wifiDirectReceiver;

        public AndroidDiscoveryService()
        {
            Initialize();
        }

        public void Initialize()
        {
            SetupNsd();
            SetupWifiDirect();
        }

        private void SetupNsd()
        {
            string serviceName = string.Empty;
            NsdServiceInfo serviceInfo = new NsdServiceInfo();
            serviceInfo.ServiceName = "SessionsDiscoveryService";
            serviceInfo.ServiceType = "_http._tcp.";
            serviceInfo.Port = 53552;
            _nsdManager = (NsdManager)SessionsApplication.Context.GetSystemService(Context.NsdService);
            _discoveryListener = new DiscoveryListener(_nsdManager);
            _registrationListener = new RegistrationListener();
            _registrationListener.ServiceRegistered += delegate(NsdServiceInfo info)
            {
                serviceName = info.ServiceName;
            };
            _nsdManager.RegisterService(serviceInfo, NsdProtocol.DnsSd, _registrationListener);            
        }

        private void SetupWifiDirect()
        {
            _intentFilter = new IntentFilter();
            _intentFilter.AddAction(WifiP2pManager.WifiP2pStateChangedAction);
            _intentFilter.AddAction(WifiP2pManager.WifiP2pPeersChangedAction);
            _intentFilter.AddAction(WifiP2pManager.WifiP2pConnectionChangedAction);
            _intentFilter.AddAction(WifiP2pManager.WifiP2pThisDeviceChangedAction);

            _actionListener = new ActionListener();
            _peerListListener = new PeerListListener();
            _wifiManager = (WifiP2pManager)SessionsApplication.Context.GetSystemService(Context.WifiP2pService);
            _wifiChannel = _wifiManager.Initialize(SessionsApplication.Context, SessionsApplication.Context.MainLooper, null);
            _wifiDirectReceiver = new WifiDirectReceiver();
            _wifiDirectReceiver.SetManager(_wifiManager, _wifiChannel, _peerListListener);
            SessionsApplication.Context.RegisterReceiver(_wifiDirectReceiver, _intentFilter);
        }

        public void StartDiscovery()
        {
            _nsdManager.DiscoverServices("_http._tcp.", NsdProtocol.DnsSd, _discoveryListener);
        }

        public void DiscoverPeers()
        {
            _wifiManager.DiscoverPeers(_wifiChannel, _actionListener);
        }

        public void ResolveService(NsdServiceInfo serviceInfo)
        {
            _resolveListener = new ResolveListener();
            _nsdManager.ResolveService(serviceInfo, _resolveListener);
        }

        public void Dispose()
        {
            _nsdManager.UnregisterService(_registrationListener);
            _nsdManager.StopServiceDiscovery(_discoveryListener);
        }
    }
}