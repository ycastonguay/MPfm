
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace MPfm.Mac
{
    public partial class EditMarkerWindow : MonoMac.AppKit.NSWindow
    {
        #region Constructors
        
        // Called when created from unmanaged code
        public EditMarkerWindow(IntPtr handle) : base (handle)
        {
            Initialize();
        }
        
        // Called when created directly from a XIB file
        [Export ("initWithCoder:")]
        public EditMarkerWindow(NSCoder coder) : base (coder)
        {
            Initialize();
        }
        
        // Shared initialization code
        void Initialize()
        {
        }
        
        #endregion
    }
}

