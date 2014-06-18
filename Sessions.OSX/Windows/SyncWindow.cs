using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MPfm.OSX.Classes.Objects;

namespace MPfm.OSX
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

        public override void AwakeFromNib()
        {
            BackgroundColor = GlobalTheme.SecondaryWindowColor;
        }       
    }
}
