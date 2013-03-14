// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace MPfm.iOS
{
	[Register ("MarkerDetailsViewController")]
	partial class MarkerDetailsViewController
	{
		[Outlet]
		MonoTouch.UIKit.UILabel lblMarkerDetails { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnDeleteMarker { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnClose { get; set; }

		[Action ("actionDeleteMarker:")]
		partial void actionDeleteMarker (MonoTouch.Foundation.NSObject sender);

		[Action ("actionClose:")]
		partial void actionClose (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (lblMarkerDetails != null) {
				lblMarkerDetails.Dispose ();
				lblMarkerDetails = null;
			}

			if (btnDeleteMarker != null) {
				btnDeleteMarker.Dispose ();
				btnDeleteMarker = null;
			}

			if (btnClose != null) {
				btnClose.Dispose ();
				btnClose = null;
			}
		}
	}
}
