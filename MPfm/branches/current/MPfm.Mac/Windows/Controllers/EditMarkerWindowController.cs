
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace MPfm.Mac
{
    public partial class EditMarkerWindowController : MonoMac.AppKit.NSWindowController
    {
        #region Constructors
        
        // Called when created from unmanaged code
        public EditMarkerWindowController(IntPtr handle) : base (handle)
        {
            Initialize();
        }
        
        // Called when created directly from a XIB file
        [Export ("initWithCoder:")]
        public EditMarkerWindowController(NSCoder coder) : base (coder)
        {
            Initialize();
        }
        
        // Call to load from the XIB/NIB file
        public EditMarkerWindowController() : base ("EditMarkerWindow")
        {
            Initialize();
        }
        
        // Shared initialization code
        void Initialize()
        {
        }
        
        #endregion
        
        //strongly typed window accessor
        public new EditMarkerWindow Window
        {
            get
            {
                return (EditMarkerWindow)base.Window;
            }
        }
    }
}

