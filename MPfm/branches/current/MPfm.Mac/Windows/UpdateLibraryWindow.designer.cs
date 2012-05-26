// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace MPfm.Mac
{
	[Register ("UpdateLibraryWindowController")]
	partial class UpdateLibraryWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSProgressIndicator progressBar { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblPercentageDone { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnCancel { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnSaveLog { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnOK { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextView textViewErrorLog { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitle { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSubtitle { get; set; }

		[Outlet]
		MonoMac.AppKit.NSScrollView scrollViewErrorLog { get; set; }

		[Action ("btnOK_Click:")]
		partial void btnOK_Click (MonoMac.Foundation.NSObject sender);

		[Action ("btnCancel_Click:")]
		partial void btnCancel_Click (MonoMac.Foundation.NSObject sender);

		[Action ("btnSaveLog_Click:")]
		partial void btnSaveLog_Click (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (progressBar != null) {
				progressBar.Dispose ();
				progressBar = null;
			}

			if (lblPercentageDone != null) {
				lblPercentageDone.Dispose ();
				lblPercentageDone = null;
			}

			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}

			if (btnSaveLog != null) {
				btnSaveLog.Dispose ();
				btnSaveLog = null;
			}

			if (btnOK != null) {
				btnOK.Dispose ();
				btnOK = null;
			}

			if (textViewErrorLog != null) {
				textViewErrorLog.Dispose ();
				textViewErrorLog = null;
			}

			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}

			if (lblSubtitle != null) {
				lblSubtitle.Dispose ();
				lblSubtitle = null;
			}

			if (scrollViewErrorLog != null) {
				scrollViewErrorLog.Dispose ();
				scrollViewErrorLog = null;
			}
		}
	}

	[Register ("UpdateLibraryWindow")]
	partial class UpdateLibraryWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
