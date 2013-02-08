
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MPfm.MVP;
using MPfm.MVP.Views;

namespace MPfm.Mac
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

