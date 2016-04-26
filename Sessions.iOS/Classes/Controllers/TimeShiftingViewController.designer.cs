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
	[Register ("TimeShiftingViewController")]
	partial class TimeShiftingViewController
	{
		[Outlet]
        Sessions.iOS.Classes.Controls.Buttons.SessionsSemiTransparentButton btnDecrementTempo { get; set; }

		[Outlet]
        Sessions.iOS.Classes.Controls.Buttons.SessionsSemiTransparentButton btnIncrementTempo { get; set; }

		[Outlet]
		UIKit.UIButton btnReset { get; set; }

		[Outlet]
		UIKit.UIButton btnUseTempo { get; set; }

		[Outlet]
		UIKit.UILabel lblCurrentTempo { get; set; }

		[Outlet]
		UIKit.UILabel lblCurrentTempoValue { get; set; }

		[Outlet]
		UIKit.UILabel lblDetectedTempo { get; set; }

		[Outlet]
		UIKit.UILabel lblDetectedTempoValue { get; set; }

		[Outlet]
		UIKit.UILabel lblReferenceTempo { get; set; }

		[Outlet]
		UIKit.UILabel lblReferenceTempoValue { get; set; }

		[Outlet]
		UIKit.UILabel lblTitle { get; set; }

		[Outlet]
		UIKit.UISlider slider { get; set; }

		[Outlet]
		UIKit.UIView viewBackground { get; set; }

		[Outlet]
		UIKit.UIView viewButtons { get; set; }

		[Action ("actionDecrementTempo:")]
		partial void actionDecrementTempo (Foundation.NSObject sender);

		[Action ("actionIncrementTempo:")]
		partial void actionIncrementTempo (Foundation.NSObject sender);

		[Action ("actionReset:")]
		partial void actionReset (Foundation.NSObject sender);

		[Action ("actionUseTempo:")]
		partial void actionUseTempo (Foundation.NSObject sender);
		
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

			if (btnDecrementTempo != null) {
				btnDecrementTempo.Dispose ();
				btnDecrementTempo = null;
			}

			if (btnIncrementTempo != null) {
				btnIncrementTempo.Dispose ();
				btnIncrementTempo = null;
			}

			if (btnUseTempo != null) {
				btnUseTempo.Dispose ();
				btnUseTempo = null;
			}

			if (viewButtons != null) {
				viewButtons.Dispose ();
				viewButtons = null;
			}
		}
	}
}
