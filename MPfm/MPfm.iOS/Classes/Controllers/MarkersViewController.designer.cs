// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace MPfm.iOS
{
	[Register ("MarkersViewController")]
	partial class MarkersViewController
	{
		[Outlet]
		MPfm.iOS.Classes.Controls.MPfmSemiTransparentRoundButton btnAddMarker { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblTitle { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView tableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewBackground { get; set; }

		[Action ("actionAddMarker:")]
		partial void actionAddMarker (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (tableView != null) {
				tableView.Dispose ();
				tableView = null;
			}

			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}

			if (btnAddMarker != null) {
				btnAddMarker.Dispose ();
				btnAddMarker = null;
			}

			if (viewBackground != null) {
				viewBackground.Dispose ();
				viewBackground = null;
			}
		}
	}
}
