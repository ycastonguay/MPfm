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
	[Register ("CloudConnectViewController")]
	partial class CloudConnectViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIActivityIndicatorView activityIndicator { get; set; }

		[Outlet]
		MPfm.iOS.Classes.Controls.MPfmButton btnOK { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblStatus { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblStatusCenter { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewPanel { get; set; }

		[Action ("actionOK:")]
		partial void actionOK (MonoTouch.Foundation.NSObject sender);
		
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
