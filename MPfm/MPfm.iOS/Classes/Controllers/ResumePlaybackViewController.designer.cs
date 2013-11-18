// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace MPfm.iOS
{
	[Register ("ResumePlaybackViewController")]
	partial class ResumePlaybackViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIActivityIndicatorView activityIndicator { get; set; }

		[Outlet]
		MPfm.iOS.Classes.Controls.MPfmButton btnOpenCloudPreferences { get; set; }

		[Outlet]
		MPfm.iOS.Classes.Controls.MPfmButton btnResumePlayback { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblLoading { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblTitle { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView tableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewAppNotLinked { get; set; }

		[Action ("actionOpenCloudPreferences:")]
		partial void actionOpenCloudPreferences (MonoTouch.Foundation.NSObject sender);

		[Action ("actionResumePlayback:")]
		partial void actionResumePlayback (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnResumePlayback != null) {
				btnResumePlayback.Dispose ();
				btnResumePlayback = null;
			}

			if (tableView != null) {
				tableView.Dispose ();
				tableView = null;
			}

			if (viewAppNotLinked != null) {
				viewAppNotLinked.Dispose ();
				viewAppNotLinked = null;
			}

			if (btnOpenCloudPreferences != null) {
				btnOpenCloudPreferences.Dispose ();
				btnOpenCloudPreferences = null;
			}

			if (activityIndicator != null) {
				activityIndicator.Dispose ();
				activityIndicator = null;
			}

			if (lblLoading != null) {
				lblLoading.Dispose ();
				lblLoading = null;
			}

			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}
		}
	}
}
