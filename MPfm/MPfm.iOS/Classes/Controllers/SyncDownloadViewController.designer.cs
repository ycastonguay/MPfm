// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace MPfm.iOS
{
	[Register ("SyncDownloadViewController")]
	partial class SyncDownloadViewController
	{
		[Outlet]
		MonoTouch.UIKit.UILabel lblTitle { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIProgressView progressView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblPercentageDoneValue { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblPercentageDone { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblFilesDownloadedValue { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblFilesDownloaded { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblTotalFiles { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblTotalFilesValue { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblCurrentFileValue { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblCurrentFile { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblFileNameValue { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblErrorsValue { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblErrors { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnCancel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextView textViewLog { get; set; }

		[Action ("actionCancel:")]
		partial void actionCancel (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}

			if (progressView != null) {
				progressView.Dispose ();
				progressView = null;
			}

			if (lblPercentageDoneValue != null) {
				lblPercentageDoneValue.Dispose ();
				lblPercentageDoneValue = null;
			}

			if (lblPercentageDone != null) {
				lblPercentageDone.Dispose ();
				lblPercentageDone = null;
			}

			if (lblFilesDownloadedValue != null) {
				lblFilesDownloadedValue.Dispose ();
				lblFilesDownloadedValue = null;
			}

			if (lblFilesDownloaded != null) {
				lblFilesDownloaded.Dispose ();
				lblFilesDownloaded = null;
			}

			if (lblTotalFiles != null) {
				lblTotalFiles.Dispose ();
				lblTotalFiles = null;
			}

			if (lblTotalFilesValue != null) {
				lblTotalFilesValue.Dispose ();
				lblTotalFilesValue = null;
			}

			if (lblCurrentFileValue != null) {
				lblCurrentFileValue.Dispose ();
				lblCurrentFileValue = null;
			}

			if (lblCurrentFile != null) {
				lblCurrentFile.Dispose ();
				lblCurrentFile = null;
			}

			if (lblFileNameValue != null) {
				lblFileNameValue.Dispose ();
				lblFileNameValue = null;
			}

			if (lblErrorsValue != null) {
				lblErrorsValue.Dispose ();
				lblErrorsValue = null;
			}

			if (lblErrors != null) {
				lblErrors.Dispose ();
				lblErrors = null;
			}

			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}

			if (textViewLog != null) {
				textViewLog.Dispose ();
				textViewLog = null;
			}
		}
	}
}
