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
		MonoTouch.UIKit.UISlider slider { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblDetectedTempoValue { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblDetectedTempo { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblReferenceTempo { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblReferenceTempoValue { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblCurrentTempo { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblCurrentTempoValue { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnReset { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewBackground { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnUseTempo { get; set; }

		[Action ("actionReset:")]
		partial void actionReset (MonoTouch.Foundation.NSObject sender);

		[Action ("actionUseTempo:")]
		partial void actionUseTempo (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}

			if (slider != null) {
				slider.Dispose ();
				slider = null;
			}

			if (lblDetectedTempoValue != null) {
				lblDetectedTempoValue.Dispose ();
				lblDetectedTempoValue = null;
			}

			if (lblDetectedTempo != null) {
				lblDetectedTempo.Dispose ();
				lblDetectedTempo = null;
			}

			if (lblReferenceTempo != null) {
				lblReferenceTempo.Dispose ();
				lblReferenceTempo = null;
			}

			if (lblReferenceTempoValue != null) {
				lblReferenceTempoValue.Dispose ();
				lblReferenceTempoValue = null;
			}

			if (lblCurrentTempo != null) {
				lblCurrentTempo.Dispose ();
				lblCurrentTempo = null;
			}

			if (lblCurrentTempoValue != null) {
				lblCurrentTempoValue.Dispose ();
				lblCurrentTempoValue = null;
			}

			if (btnReset != null) {
				btnReset.Dispose ();
				btnReset = null;
			}

			if (viewBackground != null) {
				viewBackground.Dispose ();
				viewBackground = null;
			}

			if (btnUseTempo != null) {
				btnUseTempo.Dispose ();
				btnUseTempo = null;
			}
		}
	}
}
