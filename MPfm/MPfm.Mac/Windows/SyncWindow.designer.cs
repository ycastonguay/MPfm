// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace MPfm.Mac
{
	[Register ("SyncWindowController")]
	partial class SyncWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSTableView tableViewDevices { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblIPAddress { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnRefreshDevices { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnSyncLibraryWithDevice { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblStatus { get; set; }

		[Outlet]
		MonoMac.AppKit.NSProgressIndicator progressIndicator { get; set; }

		[Action ("actionSyncLibraryWithDevice:")]
		partial void actionSyncLibraryWithDevice (MonoMac.Foundation.NSObject sender);

		[Action ("actionRefreshDevices:")]
		partial void actionRefreshDevices (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (tableViewDevices != null) {
				tableViewDevices.Dispose ();
				tableViewDevices = null;
			}

			if (lblIPAddress != null) {
				lblIPAddress.Dispose ();
				lblIPAddress = null;
			}

			if (btnRefreshDevices != null) {
				btnRefreshDevices.Dispose ();
				btnRefreshDevices = null;
			}

			if (btnSyncLibraryWithDevice != null) {
				btnSyncLibraryWithDevice.Dispose ();
				btnSyncLibraryWithDevice = null;
			}

			if (lblStatus != null) {
				lblStatus.Dispose ();
				lblStatus = null;
			}

			if (progressIndicator != null) {
				progressIndicator.Dispose ();
				progressIndicator = null;
			}
		}
	}

	[Register ("SyncWindow")]
	partial class SyncWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
