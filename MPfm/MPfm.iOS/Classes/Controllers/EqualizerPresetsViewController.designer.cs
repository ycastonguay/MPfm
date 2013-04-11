// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace MPfm.iOS
{
	[Register ("EffectsViewController")]
	partial class EqualizerPresetsViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIToolbar toolBar { get; set; }

		[Outlet]
		MonoTouch.UIKit.UINavigationBar navigationBar { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView tableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem btnBarDone { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem btnBarAdd { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (toolBar != null) {
				toolBar.Dispose ();
				toolBar = null;
			}

			if (navigationBar != null) {
				navigationBar.Dispose ();
				navigationBar = null;
			}

			if (tableView != null) {
				tableView.Dispose ();
				tableView = null;
			}

			if (btnBarDone != null) {
				btnBarDone.Dispose ();
				btnBarDone = null;
			}

			if (btnBarAdd != null) {
				btnBarAdd.Dispose ();
				btnBarAdd = null;
			}
		}
	}
}
