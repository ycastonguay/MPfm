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
	[Register ("CloudConnectViewController")]
	partial class CloudConnectViewController
	{
		[Outlet]
		UIKit.UIActivityIndicatorView activityIndicator { get; set; }

		[Outlet]
        Sessions.iOS.Classes.Controls.Buttons.SessionsButton btnOK { get; set; }

		[Outlet]
		UIKit.UILabel lblStatus { get; set; }

		[Outlet]
		UIKit.UILabel lblStatusCenter { get; set; }

		[Outlet]
		UIKit.UIView viewPanel { get; set; }

		[Action ("actionOK:")]
		partial void actionOK (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (activityIndicator != null) {
				activityIndicator.Dispose ();
				activityIndicator = null;
			}

			if (btnOK != null) {
				btnOK.Dispose ();
				btnOK = null;
			}

			if (lblStatus != null) {
				lblStatus.Dispose ();
				lblStatus = null;
			}

			if (viewPanel != null) {
				viewPanel.Dispose ();
				viewPanel = null;
			}

			if (lblStatusCenter != null) {
				lblStatusCenter.Dispose ();
				lblStatusCenter = null;
			}
		}
	}
}
