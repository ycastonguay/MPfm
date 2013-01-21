
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MPfm.MVP;
using MPfm.MVP.Views;

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
            windowDelegate = new MPfmWindowDelegate(() => { 
                if(OnViewDestroy != null)
                    OnViewDestroy.Invoke(); 
            });
            this.Window.Delegate = windowDelegate;
        }
        
        #endregion

        protected override void Dispose(bool disposing)
        {
            Console.WriteLine("BaseWindowController - Dispose(" + disposing.ToString() + ")");
            base.Dispose(disposing);
        }

        #region IBaseView implementation
        
        public Action OnViewDestroy { get; set; }

        public void ShowView(bool shown)
        {
            this.Window.ContentView.Hidden = !shown;

            if(shown)
                this.Window.MakeKeyAndOrderFront(this);
        }

        #endregion
    }
}
