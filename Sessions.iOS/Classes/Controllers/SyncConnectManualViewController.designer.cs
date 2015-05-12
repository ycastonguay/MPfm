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
	[Register ("SyncConnectManualViewController")]
	partial class SyncConnectManualViewController
	{
		[Outlet]
        Sessions.iOS.Classes.Controls.Buttons.SessionsButton btnCancel { get; set; }

		[Outlet]
        Sessions.iOS.Classes.Controls.Buttons.SessionsButton btnConnect { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField txtUrl { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewPanel { get; set; }

		[Action ("actionCancel:")]
		partial void actionCancel (MonoTouch.Foundation.NSObject sender);

		[Action ("actionConnect:")]
		partial void actionConnect (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}

			if (btnConnect != null) {
				btnConnect.Dispose ();
				btnConnect = null;
			}

			if (viewPanel != null) {
				viewPanel.Dispose ();
				viewPanel = null;
			}

			if (txtUrl != null) {
				txtUrl.Dispose ();
				txtUrl = null;
			}
		}
	}
}
