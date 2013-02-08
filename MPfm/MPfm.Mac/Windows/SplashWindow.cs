
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace MPfm.Mac
{
    public partial class SplashWindow : MonoMac.AppKit.NSWindow
    {
        #region Constructors
        
        // Called when created from unmanaged code
        public SplashWindow(IntPtr handle) : base (handle)
        {
            Initialize();
        }
        
        // Called when created directly from a XIB file
        [Export ("initWithCoder:")]
        public SplashWindow(NSCoder coder) : base (coder)
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

