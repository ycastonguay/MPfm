// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace MPfm.iOS
{
	[Register ("PitchShiftingViewController")]
	partial class PitchShiftingViewController
	{
		[Outlet]
		MonoTouch.UIKit.UISegmentedControl segmentedControl { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblTitle { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISlider slider { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblInterval { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnReset { get; set; }

		[Action ("actionReset:")]
		partial void actionReset (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (segmentedControl != null) {
				segmentedControl.Dispose ();
				segmentedControl = null;
			}

			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}

			if (slider != null) {
				slider.Dispose ();
				slider = null;
			}

			if (lblInterval != null) {
				lblInterval.Dispose ();
				lblInterval = null;
			}

			if (btnReset != null) {
				btnReset.Dispose ();
				btnReset = null;
			}
		}
	}
}
