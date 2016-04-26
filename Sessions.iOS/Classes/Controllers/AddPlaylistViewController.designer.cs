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
	[Register ("AddPlaylistViewController")]
	partial class AddPlaylistViewController
	{
		[Outlet]
        Sessions.iOS.Classes.Controls.Buttons.SessionsButton btnCancel { get; set; }

		[Outlet]
        Sessions.iOS.Classes.Controls.Buttons.SessionsButton btnCreate { get; set; }

		[Outlet]
		UIKit.UITextField txtPlaylistName { get; set; }

		[Outlet]
		UIKit.UIView viewPanel { get; set; }

		[Action ("actionCancel:")]
		partial void actionCancel (Foundation.NSObject sender);

		[Action ("actionCreate:")]
		partial void actionCreate (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}

			if (btnCreate != null) {
				btnCreate.Dispose ();
				btnCreate = null;
			}

			if (txtPlaylistName != null) {
				txtPlaylistName.Dispose ();
				txtPlaylistName = null;
			}

			if (viewPanel != null) {
				viewPanel.Dispose ();
				viewPanel = null;
			}
		}
	}
}
