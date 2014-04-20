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
using MPfm.MVP.Views;
using MPfm.MVP.Models;

namespace MPfm.Mac
{
    public partial class CloudConnectWindowController : BaseWindowController, ICloudConnectView
    {
        public CloudConnectWindowController(IntPtr handle) 
            : base (handle)
        {
            Initialize();
        }

        public CloudConnectWindowController(Action<IBaseView> onViewReady)
            : base ("CloudConnectWindow", onViewReady)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.Window.Center();
            this.Window.MakeKeyAndOrderFront(this);

            LoadFontsAndImages();
        }

        public override void WindowDidLoad()
        {
            base.WindowDidLoad();
            OnViewReady(this);
        }

        private void LoadFontsAndImages()
        {    
        }

        #region ICloudConnectView implementation

        public Action OnCheckIfAccountIsLinked { get; set; }

        public void CloudConnectError(Exception ex)
        {
        }

        public void RefreshStatus(CloudConnectEntity entity)
        {
        }

        #endregion

    }
}

