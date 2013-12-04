// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace MPfm.iOS.Classes.Controllers
{
	[Register ("SplashViewController")]
	partial class SplashViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIActivityIndicatorView activityIndicator { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView imageView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView imageViewLogo { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView imageViewOverlay { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblStatus { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (imageView != null) {
				imageView.Dispose ();
				imageView = null;
			}

			if (lblStatus != null) {
				lblStatus.Dispose ();
				lblStatus = null;
			}

			if (imageViewLogo != null) {
				imageViewLogo.Dispose ();
				imageViewLogo = null;
			}

			if (activityIndicator != null) {
				activityIndicator.Dispose ();
				activityIndicator = null;
			}

			if (imageViewOverlay != null) {
				imageViewOverlay.Dispose ();
				imageViewOverlay = null;
			}
		}
	}
}
