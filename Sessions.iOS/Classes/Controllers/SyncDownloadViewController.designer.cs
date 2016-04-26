// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace Sessions.iOS.Classes.Controllers
{
	[Register ("SyncDownloadViewController")]
	partial class SyncDownloadViewController
	{
		[Outlet]
		UIKit.UILabel lblTitle { get; set; }

		[Outlet]
		UIKit.UIProgressView progressView { get; set; }

		[Outlet]
		UIKit.UILabel lblPercentageDoneValue { get; set; }

		[Outlet]
		UIKit.UILabel lblPercentageDone { get; set; }

		[Outlet]
		UIKit.UILabel lblFilesDownloadedValue { get; set; }

		[Outlet]
		UIKit.UILabel lblFilesDownloaded { get; set; }

		[Outlet]
		UIKit.UILabel lblTotalFiles { get; set; }

		[Outlet]
		UIKit.UILabel lblDownloadSpeedValue { get; set; }

		[Outlet]
		UIKit.UILabel lblDownloadSpeed { get; set; }

		[Outlet]
		UIKit.UILabel lblTotalFilesValue { get; set; }

		[Outlet]
		UIKit.UILabel lblCurrentFileValue { get; set; }

		[Outlet]
		UIKit.UILabel lblCurrentFile { get; set; }

		[Outlet]
		UIKit.UILabel lblFileNameValue { get; set; }

		[Outlet]
		UIKit.UILabel lblErrorsValue { get; set; }

		[Outlet]
		UIKit.UILabel lblErrors { get; set; }

		[Outlet]
		UIKit.UITextView textViewLog { get; set; }
		
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

			if (lblDownloadSpeedValue != null) {
				lblDownloadSpeedValue.Dispose ();
				lblDownloadSpeedValue = null;
			}

			if (lblDownloadSpeed != null) {
				lblDownloadSpeed.Dispose ();
				lblDownloadSpeed = null;
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

			if (textViewLog != null) {
				textViewLog.Dispose ();
				textViewLog = null;
			}
		}
	}
}
