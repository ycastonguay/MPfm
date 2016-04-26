// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Sessions.iOS.Classes.Controllers
{
	[Register ("ResumePlaybackViewController")]
	partial class ResumePlaybackViewController
	{
		[Outlet]
		UIKit.UIActivityIndicatorView activityIndicator { get; set; }

		[Outlet]
        Sessions.iOS.Classes.Controls.Buttons.SessionsButton btnOpenCloudPreferences { get; set; }

		[Outlet]
		UIKit.UILabel lblTitle { get; set; }

		[Outlet]
		UIKit.UITableView tableView { get; set; }

		[Outlet]
		UIKit.UIView viewAppNotLinked { get; set; }

		[Outlet]
		UIKit.UIView viewLoading { get; set; }

		[Outlet]
		UIKit.UIView viewTable { get; set; }

		[Action ("actionOpenCloudPreferences:")]
		partial void actionOpenCloudPreferences (Foundation.NSObject sender);
		
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
