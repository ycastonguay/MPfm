
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace MPfm.Mac
{
    public partial class EditLoopWindowController : MonoMac.AppKit.NSWindowController
    {
        #region Constructors
        
        // Called when created from unmanaged code
        public EditLoopWindowController(IntPtr handle) : base (handle)
        {
            Initialize();
        }
        
        // Called when created directly from a XIB file
        [Export ("initWithCoder:")]
        public EditLoopWindowController(NSCoder coder) : base (coder)
        {
            Initialize();
        }
        
        // Call to load from the XIB/NIB file
        public EditLoopWindowController() : base ("EditLoopWindow")
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

