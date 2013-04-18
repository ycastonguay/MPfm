// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace MPfm.iOS
{
	[Register ("TimeShiftingViewController")]
	partial class TimeShiftingViewController
	{
		[Outlet]
		MonoTouch.UIKit.UILabel lblTitle { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISegmentedControl segmentedControl { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISlider slider { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblTempo { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnReset { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnDetectTempo { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblOriginalTempo { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewBackground { get; set; }

		[Action ("actionDetectTempo:")]
		partial void actionDetectTempo (MonoTouch.Foundation.NSObject sender);

		[Action ("actionReset:")]
		partial void actionReset (MonoTouch.Foundation.NSObject sender);

		[Action ("actionSegmentChanged:")]
		partial void actionSegmentChanged (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}

			if (segmentedControl != null) {
				segmentedControl.Dispose ();
				segmentedControl = null;
			}

			if (slider != null) {
				slider.Dispose ();
				slider = null;
			}

			if (lblTempo != null) {
				lblTempo.Dispose ();
				lblTempo = null;
			}

			if (btnReset != null) {
				btnReset.Dispose ();
				btnReset = null;
			}

			if (btnDetectTempo != null) {
				btnDetectTempo.Dispose ();
				btnDetectTempo = null;
			}

			if (lblOriginalTempo != null) {
				lblOriginalTempo.Dispose ();
				lblOriginalTempo = null;
			}

			if (viewBackground != null) {
				viewBackground.Dispose ();
				viewBackground = null;
			}
		}
	}
}
