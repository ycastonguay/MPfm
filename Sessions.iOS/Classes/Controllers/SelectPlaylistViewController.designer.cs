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
	[Register ("SelectPlaylistViewController")]
	partial class SelectPlaylistViewController
	{
		[Outlet]
        Sessions.iOS.Classes.Controls.Buttons.SessionsButton btnAddNewPlaylist { get; set; }

		[Outlet]
        Sessions.iOS.Classes.Controls.Buttons.SessionsButton btnCancel { get; set; }

		[Outlet]
        Sessions.iOS.Classes.Controls.Buttons.SessionsButton btnSelect { get; set; }

		[Outlet]
		UIKit.UITableView tableView { get; set; }

		[Outlet]
		UIKit.UIView viewPanel { get; set; }

		[Action ("actionAddNewPlaylist:")]
		partial void actionAddNewPlaylist (Foundation.NSObject sender);

		[Action ("actionCancel:")]
		partial void actionCancel (Foundation.NSObject sender);

		[Action ("actionSelect:")]
		partial void actionSelect (Foundation.NSObject sender);
		
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
