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
    public class RegistrationListener : Java.Lang.Object, NsdManager.IRegistrationListener
    {
        public delegate void ServiceRegisteredDelegate(NsdServiceInfo serviceInfo);
        public event ServiceRegisteredDelegate ServiceRegistered;

        public void OnServiceRegistered(NsdServiceInfo serviceInfo)
        {
            Console.WriteLine("RegistrationListener - OnServiceRegistered - service: {0}", serviceInfo.ServiceName);
            if (ServiceRegistered != null)
                ServiceRegistered(serviceInfo);
        }

        public void OnServiceUnregistered(NsdServiceInfo serviceInfo)
        {
            Console.WriteLine("RegistrationListener - OnServiceUnregistered - service: {0}", serviceInfo.ServiceName);
        }

        public void OnRegistrationFailed(NsdServiceInfo serviceInfo, NsdFailure errorCode)
        {
            Console.WriteLine("RegistrationListener - OnRegistrationFailed - service: {0} error: {1}", serviceInfo.ServiceName, errorCode.ToString());
        }

        public void OnUnregistrationFailed(NsdServiceInfo serviceInfo, NsdFailure errorCode)
        {
            Console.WriteLine("RegistrationListener - OnUnregistrationFailed - service: {0} error: {1}", serviceInfo.ServiceName, errorCode.ToString());
        }
    }
}