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
	[Register ("UpdateLibraryViewController")]
	partial class UpdateLibraryViewController
	{
		[Outlet]
		UIKit.UIActivityIndicatorView activityIndicator { get; set; }

		[Outlet]
        Sessions.iOS.Classes.Controls.Buttons.SessionsSemiTransparentButton btnClose { get; set; }

		[Outlet]
		UIKit.UILabel lblSubtitle { get; set; }

		[Outlet]
		UIKit.UILabel lblTitle { get; set; }

		[Action ("actionClose:")]
		partial void actionClose (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (activityIndicator != null) {
				activityIndicator.Dispose ();
				activityIndicator = null;
			}

			if (btnClose != null) {
				btnClose.Dispose ();
				btnClose = null;
			}

			if (lblSubtitle != null) {
				lblSubtitle.Dispose ();
				lblSubtitle = null;
			}

			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}
		}
	}
}
