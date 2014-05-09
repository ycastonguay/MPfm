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
using MonoMac.Foundation;
using MonoMac.AppKit;
using MPfm.MVP.Views;
using MPfm.Mac.Classes.Delegates;
using MPfm.Mac.Classes.Helpers;

namespace MPfm.Mac
{
    public class BaseWindowController : MonoMac.AppKit.NSWindowController, IBaseView
    {
        MPfmWindowDelegate windowDelegate;
        protected Action<IBaseView> OnViewReady { get; set; }

        #region Constructors
        
        // Called when created from unmanaged code
        public BaseWindowController(IntPtr handle) 
            : base (handle)
        {
            Initialize();
        }
        
        // Called when created directly from a XIB file
        [Export ("initWithCoder:")]
        public BaseWindowController(NSCoder coder) 
            : base (coder)
        {
            Initialize();
        }
        
        // Call to load from the XIB/NIB file
        public BaseWindowController(string windowNibName, Action<IBaseView> onViewReady) 
            : base (windowNibName)
        {
            this.OnViewReady = onViewReady;
            Initialize();
        }
        
        // Shared initialization code
        void Initialize()
        {
            Window.WillClose += (sender, e) => {
                if(OnViewDestroy != null) OnViewDestroy(this); 
            };
        }
        
        #endregion

        public void ShowWindowCentered()
        {
            this.Window.Center();
            var screenRect = NSScreen.MainScreen.Frame;
            var rect = Window.Frame;
            rect.X = (screenRect.Width - Window.Frame.Width) / 2f;
            rect.Y = (screenRect.Height - Window.Frame.Height) / 2f;
            this.Window.SetFrame(rect, true);
            this.Window.MakeKeyAndOrderFront(this);
        }

        protected override void Dispose(bool disposing)
        {
            Console.WriteLine("BaseWindowController - Dispose(" + disposing.ToString() + ")");
            base.Dispose(disposing);
        }

        #region IBaseView implementation
        
        public Action<IBaseView> OnViewDestroy { get; set; }

        public void ShowView(bool shown)
        {
            this.Window.ContentView.Hidden = !shown;

            if(shown)
                this.Window.MakeKeyAndOrderFront(this);
        }

        #endregion

        protected void ShowError(Exception ex)
        {
            InvokeOnMainThread(delegate
            {
                CocoaHelper.ShowAlert("Error", string.Format("An error occured: {0}", ex), NSAlertStyle.Critical);
            });
        }
    }
}
