
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace MPfm.Mac
{
    public partial class PlaylistWindowController : MonoMac.AppKit.NSWindowController
    {
        #region Constructors
        
        // Called when created from unmanaged code
        public PlaylistWindowController(IntPtr handle) : base (handle)
        {
            Initialize();
        }
        
        // Called when created directly from a XIB file
        [Export ("initWithCoder:")]
        public PlaylistWindowController(NSCoder coder) : base (coder)
        {
            Initialize();
        }
        
        // Call to load from the XIB/NIB file
        public PlaylistWindowController() : base ("PlaylistWindow")
        {
            Initialize();
        }
        
        // Shared initialization code
        void Initialize()
        {
        }
        
        #endregion
        
        //strongly typed window accessor
        public new PlaylistWindow Window
        {
            get
            {
                return (PlaylistWindow)base.Window;
            }
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            LoadImages();
        }

        private void LoadImages()
        {
            // Load images in toolbar
            toolbar.Items.FirstOrDefault(x => x.Identifier == "toolbarNewPlaylist").Image = ImageResources.images32x32[11];
            toolbar.Items.FirstOrDefault(x => x.Identifier == "toolbarLoadPlaylist").Image = ImageResources.images32x32[0];
            toolbar.Items.FirstOrDefault(x => x.Identifier == "toolbarSavePlaylist").Image = ImageResources.images32x32[12];
            toolbar.Items.FirstOrDefault(x => x.Identifier == "toolbarSaveAsPlaylist").Image = ImageResources.images32x32[13];
        }

        partial void actionNewPlaylist(NSObject sender)
        {
        }

        partial void actionLoadPlaylist(NSObject sender)
        {
        }

        partial void actionSavePlaylist(NSObject sender)
        {
        }

        partial void actionSaveAsPlaylist(NSObject sender)
        {
        }
    }
}
