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
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using MPfm.Android.Classes.Fragments.Base;
using MPfm.Library.Objects;
using MPfm.MVP.Views;
using MPfm.Player.Objects;

namespace MPfm.Android.Classes.Fragments
{
    public class SyncFragment : BaseFragment, ISyncView, View.IOnClickListener
    {        
        private View _view;

        // Leave an empty constructor or the application will crash at runtime
        public SyncFragment() : base(null) { }

        public SyncFragment(Action<IBaseView> onViewReady) 
            : base(onViewReady)
        {
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _view = inflater.Inflate(Resource.Layout.Sync, container, false);
            return _view;
        }

        public void OnClick(View v)
        {
            
        }

        public override void OnResume()
        {
            base.OnResume();
            Activity.ActionBar.Title = "Sync Other Devices";
        }

        #region ISyncView implementation

        public Action<string> OnConnectDevice { get; set; }
        public Action<string> OnConnectDeviceManually { get; set; }

        public void SyncError(Exception ex)
        {
        }

        public void RefreshIPAddress(string address)
        {
        }

        public void RefreshDiscoveryProgress(float percentageDone, string status)
        {
        }

        public void RefreshDevices(IEnumerable<SyncDevice> devices)
        {
        }

        public void RefreshDevicesEnded()
        {
        }

        public void SyncDevice(SyncDevice device)
        {
        }

        #endregion
    }
}
