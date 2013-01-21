
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MPfm.MVP;
using MPfm.MVP.Views;

namespace MPfm.Mac
{
    public partial class EditMarkerWindowController : BaseWindowController
    {
        #region Constructors
        
        // Called when created from unmanaged code
        public EditMarkerWindowController(IntPtr handle) 
            : base (handle)
        {
            Initialize();
        }

        // Call to load from the XIB/NIB file
        public EditMarkerWindowController(Action<IBaseView> onViewReady) 
            : base ("EditMarkerWindow", onViewReady)
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

