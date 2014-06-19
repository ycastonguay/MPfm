// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace Sessions.iOS
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

		[Outlet]
		MonoTouch.UIKit.UILabel lblTitleName { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblTitlePosition { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblTitleComments { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblPosition { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblLength { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField txtName { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextView textViewComments { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISlider sliderPosition { get; set; }

		[Action ("actionClose:")]
		partial void actionClose (MonoTouch.Foundation.NSObject sender);

		[Action ("actionDeleteMarker:")]
		partial void actionDeleteMarker (MonoTouch.Foundation.NSObject sender);
		
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

			if (lblTitleName != null) {
				lblTitleName.Dispose ();
				lblTitleName = null;
			}

			if (lblTitlePosition != null) {
				lblTitlePosition.Dispose ();
				lblTitlePosition = null;
			}

			if (lblTitleComments != null) {
				lblTitleComments.Dispose ();
				lblTitleComments = null;
			}

			if (lblPosition != null) {
				lblPosition.Dispose ();
				lblPosition = null;
			}

			if (lblLength != null) {
				lblLength.Dispose ();
				lblLength = null;
			}

			if (txtName != null) {
				txtName.Dispose ();
				txtName = null;
			}

			if (textViewComments != null) {
				textViewComments.Dispose ();
				textViewComments = null;
			}

			if (sliderPosition != null) {
				sliderPosition.Dispose ();
				sliderPosition = null;
			}
		}
	}
}
