// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace MPfm.iOS
{
	[Register ("UpdateLibraryViewController")]
	partial class UpdateLibraryViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton button { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblTitle { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblSubtitle { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIActivityIndicatorView activityIndicator { get; set; }

		[Action ("actionButtonClicked:")]
		partial void actionButtonClicked (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (button != null) {
				button.Dispose ();
				button = null;
			}

			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}

			if (lblSubtitle != null) {
				lblSubtitle.Dispose ();
				lblSubtitle = null;
			}

			if (activityIndicator != null) {
				activityIndicator.Dispose ();
				activityIndicator = null;
			}
		}
	}
}
