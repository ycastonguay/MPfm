using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace MPfm.Mac
{
    public partial class SyncWindow : MonoMac.AppKit.NSWindow
    {
        #region Constructors
        
        // Called when created from unmanaged code
        public SyncWindow(IntPtr handle) : base (handle)
        {
            Initialize();
        }
        // Called when created directly from a XIB file
        [Export ("initWithCoder:")]
        public SyncWindow(NSCoder coder) : base (coder)
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

