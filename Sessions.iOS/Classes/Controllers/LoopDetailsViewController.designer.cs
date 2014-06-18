// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace MPfm.iOS
{
	[Register ("LoopDetailsViewController")]
	partial class LoopDetailsViewController
	{
		[Outlet]
		MonoTouch.UIKit.UILabel lblLoopDetails { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnDeleteLoop { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnClose { get; set; }

		[Action ("actionClose:")]
		partial void actionClose (MonoTouch.Foundation.NSObject sender);

		[Action ("actionDeleteLoop:")]
		partial void actionDeleteLoop (MonoTouch.Foundation.NSObject sender);
		
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
