// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;
using System.CodeDom.Compiler;

namespace MPfm.Mac
{
	[Register ("SyncWindowController")]
	partial class SyncWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSButton btnConnect { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnConnectManual { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnRefreshDevices { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblConnectManual { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblConnectManualPort { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblConnectManualUrl { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblDeviceDetails { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblLibraryUrl { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSubtitle { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitle { get; set; }

		[Outlet]
		MonoMac.AppKit.NSProgressIndicator progressIndicator { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTableView tableViewDevices { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtConnectManualPort { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtConnectManualUrl { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewConnectManualHeader { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewDetails { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewDeviceDetailsHeader { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewSubtitleHeader { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewTitleHeader { get; set; }

		[Action ("actionConnect:")]
		partial void actionConnect (MonoMac.Foundation.NSObject sender);

		[Action ("actionConnectManual:")]
		partial void actionConnectManual (MonoMac.Foundation.NSObject sender);

		[Action ("actionRefreshDevices:")]
		partial void actionRefreshDevices (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnConnect != null) {
				btnConnect.Dispose ();
				btnConnect = null;
			}

			if (btnConnectManual != null) {
				btnConnectManual.Dispose ();
				btnConnectManual = null;
			}

			if (btnRefreshDevices != null) {
				btnRefreshDevices.Dispose ();
				btnRefreshDevices = null;
			}

			if (lblConnectManual != null) {
				lblConnectManual.Dispose ();
				lblConnectManual = null;
			}

			if (lblConnectManualPort != null) {
				lblConnectManualPort.Dispose ();
				lblConnectManualPort = null;
			}

			if (lblConnectManualUrl != null) {
				lblConnectManualUrl.Dispose ();
				lblConnectManualUrl = null;
			}

			if (lblLibraryUrl != null) {
				lblLibraryUrl.Dispose ();
				lblLibraryUrl = null;
			}

			if (lblSubtitle != null) {
				lblSubtitle.Dispose ();
				lblSubtitle = null;
			}

			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}

			if (progressIndicator != null) {
				progressIndicator.Dispose ();
				progressIndicator = null;
			}

			if (tableViewDevices != null) {
				tableViewDevices.Dispose ();
				tableViewDevices = null;
			}

			if (txtConnectManualPort != null) {
				txtConnectManualPort.Dispose ();
				txtConnectManualPort = null;
			}

			if (txtConnectManualUrl != null) {
				txtConnectManualUrl.Dispose ();
				txtConnectManualUrl = null;
			}

			if (viewConnectManualHeader != null) {
				viewConnectManualHeader.Dispose ();
				viewConnectManualHeader = null;
			}

			if (viewDetails != null) {
				viewDetails.Dispose ();
				viewDetails = null;
			}

			if (viewSubtitleHeader != null) {
				viewSubtitleHeader.Dispose ();
				viewSubtitleHeader = null;
			}

			if (viewTitleHeader != null) {
				viewTitleHeader.Dispose ();
				viewTitleHeader = null;
			}

			if (lblDeviceDetails != null) {
				lblDeviceDetails.Dispose ();
				lblDeviceDetails = null;
			}

			if (viewDeviceDetailsHeader != null) {
				viewDeviceDetailsHeader.Dispose ();
				viewDeviceDetailsHeader = null;
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
