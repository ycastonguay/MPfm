// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace Sessions.iOS.Classes.Controllers
{
	[Register ("SyncMenuViewController")]
	partial class SyncMenuViewController
	{
		[Outlet]
		UIKit.UIView viewLoading { get; set; }

		[Outlet]
		UIKit.UILabel lblTotal { get; set; }

		[Outlet]
		UIKit.UILabel lblFreeSpace { get; set; }

		[Outlet]
		UIKit.UIView viewSync { get; set; }

		[Outlet]
		UIKit.UITableView tableView { get; set; }

		[Outlet]
		UIKit.UILabel lblLoading { get; set; }

		[Outlet]
		UIKit.UIActivityIndicatorView activityIndicator { get; set; }

		[Outlet]
		UIKit.UIButton btnSelect { get; set; }

		[Action ("actionSelect:")]
		partial void actionSelect (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (viewLoading != null) {
				viewLoading.Dispose ();
				viewLoading = null;
			}

			if (lblTotal != null) {
				lblTotal.Dispose ();
				lblTotal = null;
			}

			if (lblFreeSpace != null) {
				lblFreeSpace.Dispose ();
				lblFreeSpace = null;
			}

			if (viewSync != null) {
				viewSync.Dispose ();
				viewSync = null;
			}

			if (tableView != null) {
				tableView.Dispose ();
				tableView = null;
			}

			if (lblLoading != null) {
				lblLoading.Dispose ();
				lblLoading = null;
			}

			if (activityIndicator != null) {
				activityIndicator.Dispose ();
				activityIndicator = null;
			}

			if (btnSelect != null) {
				btnSelect.Dispose ();
				btnSelect = null;
			}
		}
	}
}
