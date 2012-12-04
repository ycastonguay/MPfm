//
// PreferencesWindowController.cs: Preferences window controller.
//
// Copyright © 2011-2012 Yanick Castonguay
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

namespace MPfm.Mac
{
    public partial class PreferencesWindowController : BaseWindowController, IPreferencesView
    {
        readonly IPreferencesPresenter preferencesPresenter;

        #region Constructors
        
        // Called when created from unmanaged code
        public PreferencesWindowController(IntPtr handle) 
            : base (handle)
        {
            Initialize();
        }
        
        // Called when created directly from a XIB file
        [Export ("initWithCoder:")]
        public PreferencesWindowController(NSCoder coder) 
            : base (coder)
        {
            Initialize();
        }
        
        // Call to load from the XIB/NIB file
        public PreferencesWindowController(IPreferencesPresenter preferencesPresenter, Action<IBaseView> onViewReady)
            : base ("PreferencesWindow", onViewReady)
        {
            this.preferencesPresenter = preferencesPresenter;
            Initialize();
            this.preferencesPresenter.BindView(this);
        }
        
        // Shared initialization code
        void Initialize()
        {
        }
        
        #endregion
        
        //strongly typed window accessor
        public new PreferencesWindow Window
        {
            get
            {
                return (PreferencesWindow)base.Window;
            }
        }
    }
}

