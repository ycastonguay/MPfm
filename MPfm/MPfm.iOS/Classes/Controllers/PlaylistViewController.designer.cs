// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace MPfm.iOS
{
	[Register ("PlaylistViewController")]
	partial class PlaylistViewController
	{
		[Outlet]
		MonoTouch.UIKit.UITableView tableView { get; set; }

		[Outlet]
		MPfm.iOS.Classes.Controls.MPfmSemiTransparentButton btnRepeat { get; set; }

		[Outlet]
		MPfm.iOS.Classes.Controls.MPfmSemiTransparentButton btnShuffle { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIToolbar toolbar { get; set; }

		[Action ("actionRepeat:")]
		partial void actionRepeat (MonoTouch.Foundation.NSObject sender);

		[Action ("actionShuffle:")]
		partial void actionShuffle (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (tableView != null) {
				tableView.Dispose ();
				tableView = null;
			}

			if (btnRepeat != null) {
				btnRepeat.Dispose ();
				btnRepeat = null;
			}

			if (btnShuffle != null) {
				btnShuffle.Dispose ();
				btnShuffle = null;
			}

			if (toolbar != null) {
				toolbar.Dispose ();
				toolbar = null;
			}
		}
	}
}
