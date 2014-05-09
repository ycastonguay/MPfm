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
using MPfm.MVP;
using MPfm.MVP.Views;

namespace MPfm.OSX
{
    public partial class EditLoopWindowController : BaseWindowController
    {
        #region Constructors
        
        // Called when created from unmanaged code
        public EditLoopWindowController(IntPtr handle) 
            : base (handle)
        {
            Initialize();
        }
        
        // Call to load from the XIB/NIB file
        public EditLoopWindowController(Action<IBaseView> onViewReady) 
            : base ("EditLoopWindow", onViewReady)
        {
            Initialize();
        }
        
        // Shared initialization code
        void Initialize()
        {
        }
        
        #endregion
        
        //strongly typed window accessor
        public new EditLoopWindow Window
        {
            get
            {
                return (EditLoopWindow)base.Window;
            }
        }
    }
}

