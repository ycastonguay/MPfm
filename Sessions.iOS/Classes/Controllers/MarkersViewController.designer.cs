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
	[Register ("MarkersViewController")]
	partial class MarkersViewController
	{
		[Outlet]
        Sessions.iOS.Classes.Controls.Buttons.SessionsSemiTransparentRoundButton btnAddMarker { get; set; }

		[Outlet]
		UIKit.UILabel lblTitle { get; set; }

		[Outlet]
		Sessions.iOS.Classes.Controls.SessionsTableView tableView { get; set; }

		[Outlet]
		UIKit.UIView viewBackground { get; set; }

		[Action ("actionAddMarker:")]
		partial void actionAddMarker (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnAddMarker != null) {
				btnAddMarker.Dispose ();
				btnAddMarker = null;
			}

			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}

			if (tableView != null) {
				tableView.Dispose ();
				tableView = null;
			}

			if (viewBackground != null) {
				viewBackground.Dispose ();
				viewBackground = null;
			}
		}
	}
}
