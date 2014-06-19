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
using System.Collections.Generic;
using System.Linq;
using Sessions.MVP.Views;
using MonoMac.AppKit;
using MonoMac.Foundation;
using Sessions.OSX.Classes.Helpers;

namespace Sessions.OSX
{
    public partial class SyncCloudWindowController : BaseWindowController, ISyncCloudView
    {
        public SyncCloudWindowController(IntPtr handle) 
            : base (handle)
        {
            Initialize();
        }

        public SyncCloudWindowController(Action<IBaseView> onViewReady)
            : base ("SyncWebBrowserWindow", onViewReady)
        {
            Initialize();
        }

        private void Initialize()
        {
            ShowWindowCentered();
        }

        public override void WindowDidLoad()
        {
            base.WindowDidLoad();
            LoadFontsAndImages();
            OnViewReady.Invoke(this);
        }

        private void LoadFontsAndImages()
        {
        }

        #region ISyncCloudView implementation

        public void SyncCloudError(Exception ex)
        {
            InvokeOnMainThread(delegate {
                CocoaHelper.ShowAlert("Error", string.Format("An error occured in SyncCloud: {0}", ex), NSAlertStyle.Critical);
            });
        }

        #endregion

    }
}
