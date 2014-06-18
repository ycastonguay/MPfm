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
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using Sessions.MVP.Views;
using MPfm.OSX.Classes.Helpers;

namespace MPfm.OSX
{
    public partial class SyncWebBrowserWindowController : BaseWindowController, ISyncWebBrowserView
    {
        public SyncWebBrowserWindowController(IntPtr handle) 
            : base (handle)
        {
            Initialize();
        }

        public SyncWebBrowserWindowController(Action<IBaseView> onViewReady)
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
            //LoadFontsAndImages();
            OnViewReady.Invoke(this);
        }

        private void LoadFontsAndImages()
        {   
        }

        #region ISyncWebBrowserView implementation

        public Action OnViewAppeared { get; set; }

        public void SyncWebBrowserError(Exception ex)
        {
            InvokeOnMainThread(delegate {
                CocoaHelper.ShowAlert("Error", string.Format("An error occured in SyncWebBrowser: {0}", ex), NSAlertStyle.Critical);
            });
        }

        public void RefreshContent(string url, string authenticationCode)
        {
        }

        #endregion
    }
}
