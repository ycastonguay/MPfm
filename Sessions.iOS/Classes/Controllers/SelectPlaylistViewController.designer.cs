// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace Sessions.iOS
{
	[Register ("SelectPlaylistViewController")]
	partial class SelectPlaylistViewController
	{
		[Outlet]
		Sessions.iOS.Classes.Controls.SessionsButton btnAddNewPlaylist { get; set; }

		[Outlet]
		Sessions.iOS.Classes.Controls.SessionsButton btnCancel { get; set; }

		[Outlet]
		Sessions.iOS.Classes.Controls.SessionsButton btnSelect { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView tableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewPanel { get; set; }

		[Action ("actionAddNewPlaylist:")]
		partial void actionAddNewPlaylist (MonoTouch.Foundation.NSObject sender);

		[Action ("actionCancel:")]
		partial void actionCancel (MonoTouch.Foundation.NSObject sender);

		[Action ("actionSelect:")]
		partial void actionSelect (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnAddNewPlaylist != null) {
				btnAddNewPlaylist.Dispose ();
				btnAddNewPlaylist = null;
			}

			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}

			if (btnSelect != null) {
				btnSelect.Dispose ();
				btnSelect = null;
			}

			if (tableView != null) {
				tableView.Dispose ();
				tableView = null;
			}

			if (viewPanel != null) {
				viewPanel.Dispose ();
				viewPanel = null;
			}
		}
	}
}
