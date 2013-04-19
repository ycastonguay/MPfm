// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace MPfm.iOS
{
	[Register ("EqualizerPresetDetailsViewController")]
	partial class EqualizerPresetDetailsViewController
	{
		[Outlet]
		MonoTouch.UIKit.UILabel lblPresetName { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField txtPresetName { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView scrollView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIToolbar toolbar { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIBarButtonItem btnReset { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewOptions { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (lblPresetName != null) {
				lblPresetName.Dispose ();
				lblPresetName = null;
			}

			if (txtPresetName != null) {
				txtPresetName.Dispose ();
				txtPresetName = null;
			}

			if (scrollView != null) {
				scrollView.Dispose ();
				scrollView = null;
			}

			if (toolbar != null) {
				toolbar.Dispose ();
				toolbar = null;
			}

			if (btnReset != null) {
				btnReset.Dispose ();
				btnReset = null;
			}

			if (viewOptions != null) {
				viewOptions.Dispose ();
				viewOptions = null;
			}
		}
	}
}
