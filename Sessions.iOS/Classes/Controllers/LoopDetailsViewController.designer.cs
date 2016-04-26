// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace Sessions.iOS.Classes.Controllers
{
	[Register ("LoopDetailsViewController")]
	partial class LoopDetailsViewController
	{
		[Outlet]
		UIKit.UILabel lblLoopDetails { get; set; }

		[Outlet]
		UIKit.UIButton btnDeleteLoop { get; set; }

		[Outlet]
		UIKit.UIButton btnClose { get; set; }

		[Action ("actionClose:")]
		partial void actionClose (Foundation.NSObject sender);

		[Action ("actionDeleteLoop:")]
		partial void actionDeleteLoop (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (lblLoopDetails != null) {
				lblLoopDetails.Dispose ();
				lblLoopDetails = null;
			}

			if (btnDeleteLoop != null) {
				btnDeleteLoop.Dispose ();
				btnDeleteLoop = null;
			}

			if (btnClose != null) {
				btnClose.Dispose ();
				btnClose = null;
			}
		}
	}
}
