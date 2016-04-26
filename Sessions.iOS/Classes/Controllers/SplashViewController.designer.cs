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
	[Register ("SplashViewController")]
	partial class SplashViewController
	{
		[Outlet]
		UIKit.UIActivityIndicatorView activityIndicator { get; set; }

		[Outlet]
		UIKit.UIImageView imageView { get; set; }

		[Outlet]
		UIKit.UIImageView imageViewOverlay { get; set; }

		[Outlet]
		UIKit.UILabel lblStatus { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (activityIndicator != null) {
				activityIndicator.Dispose ();
				activityIndicator = null;
			}

			if (imageView != null) {
				imageView.Dispose ();
				imageView = null;
			}

			if (imageViewOverlay != null) {
				imageViewOverlay.Dispose ();
				imageViewOverlay = null;
			}

			if (lblStatus != null) {
				lblStatus.Dispose ();
				lblStatus = null;
			}
		}
	}
}
