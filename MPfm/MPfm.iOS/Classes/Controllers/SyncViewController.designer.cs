// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace MPfm.iOS
{
	[Register ("SyncViewController")]
	partial class SyncViewController
	{
		[Outlet]
		MonoTouch.UIKit.UITableView tableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblIPAddress { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnAddDevice { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblStatus { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIActivityIndicatorView activityIndicator { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnRefreshDevices { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewRefresh { get; set; }

		[Action ("actionAddDevice:")]
		partial void actionAddDevice (MonoTouch.Foundation.NSObject sender);

		[Action ("actionRefreshDevices:")]
		partial void actionRefreshDevices (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (tableView != null) {
				tableView.Dispose ();
				tableView = null;
			}

			if (lblIPAddress != null) {
				lblIPAddress.Dispose ();
				lblIPAddress = null;
			}

			if (btnAddDevice != null) {
				btnAddDevice.Dispose ();
				btnAddDevice = null;
			}

			if (lblStatus != null) {
				lblStatus.Dispose ();
				lblStatus = null;
			}

			if (activityIndicator != null) {
				activityIndicator.Dispose ();
				activityIndicator = null;
			}

			if (btnRefreshDevices != null) {
				btnRefreshDevices.Dispose ();
				btnRefreshDevices = null;
			}

			if (viewRefresh != null) {
				viewRefresh.Dispose ();
				viewRefresh = null;
			}
		}
	}
}
