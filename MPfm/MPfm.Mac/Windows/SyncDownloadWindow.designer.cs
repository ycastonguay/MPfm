// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace MPfm.Mac
{
	[Register ("SyncDownloadWindowController")]
	partial class SyncDownloadWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSTextField lblTitle { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSubtitle { get; set; }

		[Outlet]
		MonoMac.AppKit.NSProgressIndicator progressIndicator { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmButton btnCancel { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblStatus { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblDownloadSpeedValue { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblDownloadSpeed { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblCurrentFile { get; set; }

		[Outlet]
		MonoMac.AppKit.NSProgressIndicator progressIndicatorCurrentFile { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblFilesDownloadedValue { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblFilesDownloaded { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblErrorsValue { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblErrors { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblCurrentFileValue { get; set; }

		[Action ("actionCancel:")]
		partial void actionCancel (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}

			if (lblSubtitle != null) {
				lblSubtitle.Dispose ();
				lblSubtitle = null;
			}

			if (progressIndicator != null) {
				progressIndicator.Dispose ();
				progressIndicator = null;
			}

			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}

			if (lblStatus != null) {
				lblStatus.Dispose ();
				lblStatus = null;
			}

			if (lblDownloadSpeedValue != null) {
				lblDownloadSpeedValue.Dispose ();
				lblDownloadSpeedValue = null;
			}

			if (lblDownloadSpeed != null) {
				lblDownloadSpeed.Dispose ();
				lblDownloadSpeed = null;
			}

			if (lblCurrentFile != null) {
				lblCurrentFile.Dispose ();
				lblCurrentFile = null;
			}

			if (progressIndicatorCurrentFile != null) {
				progressIndicatorCurrentFile.Dispose ();
				progressIndicatorCurrentFile = null;
			}

			if (lblFilesDownloadedValue != null) {
				lblFilesDownloadedValue.Dispose ();
				lblFilesDownloadedValue = null;
			}

			if (lblFilesDownloaded != null) {
				lblFilesDownloaded.Dispose ();
				lblFilesDownloaded = null;
			}

			if (lblErrorsValue != null) {
				lblErrorsValue.Dispose ();
				lblErrorsValue = null;
			}

			if (lblErrors != null) {
				lblErrors.Dispose ();
				lblErrors = null;
			}

			if (lblCurrentFileValue != null) {
				lblCurrentFileValue.Dispose ();
				lblCurrentFileValue = null;
			}
		}
	}

	[Register ("SyncDownloadWindow")]
	partial class SyncDownloadWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
