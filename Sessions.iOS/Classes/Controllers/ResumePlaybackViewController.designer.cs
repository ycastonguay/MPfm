// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace Sessions.iOS
{
	[Register ("ResumePlaybackViewController")]
	partial class ResumePlaybackViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIActivityIndicatorView activityIndicator { get; set; }

		[Outlet]
        Sessions.iOS.Classes.Controls.Buttons.SessionsButton btnOpenCloudPreferences { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblTitle { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView tableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewAppNotLinked { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewLoading { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewTable { get; set; }

		[Action ("actionOpenCloudPreferences:")]
		partial void actionOpenCloudPreferences (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnOpenCloudPreferences != null) {
				btnOpenCloudPreferences.Dispose ();
				btnOpenCloudPreferences = null;
			}

			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}

			if (tableView != null) {
				tableView.Dispose ();
				tableView = null;
			}

			if (viewAppNotLinked != null) {
				viewAppNotLinked.Dispose ();
				viewAppNotLinked = null;
			}

			if (viewTable != null) {
				viewTable.Dispose ();
				viewTable = null;
			}

			if (viewLoading != null) {
				viewLoading.Dispose ();
				viewLoading = null;
			}

			if (activityIndicator != null) {
				activityIndicator.Dispose ();
				activityIndicator = null;
			}
		}
	}
}
