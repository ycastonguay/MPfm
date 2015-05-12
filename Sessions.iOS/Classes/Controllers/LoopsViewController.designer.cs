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
	[Register ("LoopsViewController")]
	partial class LoopsViewController
	{
		[Outlet]
        Sessions.iOS.Classes.Controls.Buttons.SessionsSemiTransparentRoundButton btnAddLoop { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblTitle { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView tableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewBackground { get; set; }

		[Action ("actionAddLoop:")]
		partial void actionAddLoop (MonoTouch.Foundation.NSObject sender);
		
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

			if (viewBackground != null) {
				viewBackground.Dispose ();
				viewBackground = null;
			}

			if (btnAddLoop != null) {
				btnAddLoop.Dispose ();
				btnAddLoop = null;
			}
		}
	}
}
