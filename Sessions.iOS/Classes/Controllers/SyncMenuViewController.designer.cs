// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace MPfm.iOS
{
	[Register ("SyncMenuViewController")]
	partial class SyncMenuViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIView viewLoading { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblTotal { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblFreeSpace { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewSync { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView tableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblLoading { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIActivityIndicatorView activityIndicator { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnSelect { get; set; }

		[Action ("actionSelect:")]
		partial void actionSelect (MonoTouch.Foundation.NSObject sender);
		
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
