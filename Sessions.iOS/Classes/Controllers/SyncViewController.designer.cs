// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace Sessions.iOS.Classes.Controllers
{
	[Register ("SyncViewController")]
	partial class SyncViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIActivityIndicatorView activityIndicator { get; set; }

		[Outlet]
        Sessions.iOS.Classes.Controls.Buttons.SessionsButton btnConnectDeviceManually { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblIPAddress { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblStatus { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView tableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewRefresh { get; set; }

		[Action ("actionConnectDeviceManually:")]
		partial void actionConnectDeviceManually (MonoTouch.Foundation.NSObject sender);
		
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

			if (btnConnectDeviceManually != null) {
				btnConnectDeviceManually.Dispose ();
				btnConnectDeviceManually = null;
			}

			if (lblStatus != null) {
				lblStatus.Dispose ();
				lblStatus = null;
			}

			if (activityIndicator != null) {
				activityIndicator.Dispose ();
				activityIndicator = null;
			}

			if (viewRefresh != null) {
				viewRefresh.Dispose ();
				viewRefresh = null;
			}
		}
	}
}
