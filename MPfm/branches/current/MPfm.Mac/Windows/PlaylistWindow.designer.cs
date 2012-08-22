// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace MPfm.Mac
{
	[Register ("PlaylistWindowController")]
	partial class PlaylistWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSToolbar toolbar { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTableView tableView { get; set; }

		[Action ("actionNewPlaylist:")]
		partial void actionNewPlaylist (MonoMac.Foundation.NSObject sender);

		[Action ("actionLoadPlaylist:")]
		partial void actionLoadPlaylist (MonoMac.Foundation.NSObject sender);

		[Action ("actionSavePlaylist:")]
		partial void actionSavePlaylist (MonoMac.Foundation.NSObject sender);

		[Action ("actionSaveAsPlaylist:")]
		partial void actionSaveAsPlaylist (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (toolbar != null) {
				toolbar.Dispose ();
				toolbar = null;
			}

			if (tableView != null) {
				tableView.Dispose ();
				tableView = null;
			}
		}
	}

	[Register ("PlaylistWindow")]
	partial class PlaylistWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
