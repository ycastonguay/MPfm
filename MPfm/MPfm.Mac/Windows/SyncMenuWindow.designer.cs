// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace MPfm.Mac
{
	[Register ("SyncMenuWindowController")]
	partial class SyncMenuWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSView viewLoading { get; set; }

		[Outlet]
		MonoMac.AppKit.NSView viewTable { get; set; }

		[Outlet]
		MonoMac.AppKit.NSProgressIndicator progressIndicator { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmButton btnSync { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmButton btnSelectAll { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTotal { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblFreeSpace { get; set; }

		[Outlet]
		MonoMac.AppKit.NSOutlineView outlineView { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSubtitle { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitle { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblLoading { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (viewLoading != null) {
				viewLoading.Dispose ();
				viewLoading = null;
			}

			if (viewTable != null) {
				viewTable.Dispose ();
				viewTable = null;
			}

			if (progressIndicator != null) {
				progressIndicator.Dispose ();
				progressIndicator = null;
			}

			if (btnSync != null) {
				btnSync.Dispose ();
				btnSync = null;
			}

			if (btnSelectAll != null) {
				btnSelectAll.Dispose ();
				btnSelectAll = null;
			}

			if (lblTotal != null) {
				lblTotal.Dispose ();
				lblTotal = null;
			}

			if (lblFreeSpace != null) {
				lblFreeSpace.Dispose ();
				lblFreeSpace = null;
			}

			if (outlineView != null) {
				outlineView.Dispose ();
				outlineView = null;
			}

			if (lblSubtitle != null) {
				lblSubtitle.Dispose ();
				lblSubtitle = null;
			}

			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}

			if (lblLoading != null) {
				lblLoading.Dispose ();
				lblLoading = null;
			}
		}
	}

	[Register ("SyncMenuWindow")]
	partial class SyncMenuWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
