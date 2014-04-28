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
		MonoMac.AppKit.NSTextField lblLibraryUrl { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnRefreshDevices { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnConnect { get; set; }

		[Outlet]
		MonoMac.AppKit.NSProgressIndicator progressIndicator { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnConnectManual { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitle { get; set; }

		[Action ("actionConnect:")]
		partial void actionConnect (MonoMac.Foundation.NSObject sender);

		[Action ("actionRefreshDevices:")]
		partial void actionRefreshDevices (MonoMac.Foundation.NSObject sender);

		[Action ("actionConnectManual:")]
		partial void actionConnectManual (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (tableViewDevices != null) {
				tableViewDevices.Dispose ();
				tableViewDevices = null;
			}

			if (lblLibraryUrl != null) {
				lblLibraryUrl.Dispose ();
				lblLibraryUrl = null;
			}

			if (btnRefreshDevices != null) {
				btnRefreshDevices.Dispose ();
				btnRefreshDevices = null;
			}

			if (btnConnect != null) {
				btnConnect.Dispose ();
				btnConnect = null;
			}

			if (progressIndicator != null) {
				progressIndicator.Dispose ();
				progressIndicator = null;
			}

			if (btnConnectManual != null) {
				btnConnectManual.Dispose ();
				btnConnectManual = null;
			}

			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
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
