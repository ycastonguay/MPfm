// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace MPfm.iOS
{
	[Register ("PitchShiftingViewController")]
	partial class PitchShiftingViewController
	{
		[Outlet]
		MPfm.iOS.Classes.Controls.MPfmSemiTransparentButton btnChangeKey { get; set; }

		[Outlet]
		MPfm.iOS.Classes.Controls.MPfmSemiTransparentButton btnDecrementInterval { get; set; }

		[Outlet]
		MPfm.iOS.Classes.Controls.MPfmSemiTransparentButton btnIncrementInterval { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnReset { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblInterval { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblKey { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblNewKey { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblTitle { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISlider slider { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewBackground { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewButtons { get; set; }

		[Action ("actionChangeKey:")]
		partial void actionChangeKey (MonoTouch.Foundation.NSObject sender);

		[Action ("actionDecrementInterval:")]
		partial void actionDecrementInterval (MonoTouch.Foundation.NSObject sender);

		[Action ("actionIncrementInterval:")]
		partial void actionIncrementInterval (MonoTouch.Foundation.NSObject sender);

		[Action ("actionReset:")]
		partial void actionReset (MonoTouch.Foundation.NSObject sender);
		
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

			if (lblInterval != null) {
				lblInterval.Dispose ();
				lblInterval = null;
			}

			if (lblNewKey != null) {
				lblNewKey.Dispose ();
				lblNewKey = null;
			}

			if (lblKey != null) {
				lblKey.Dispose ();
				lblKey = null;
			}

			if (btnReset != null) {
				btnReset.Dispose ();
				btnReset = null;
			}

			if (btnDecrementInterval != null) {
				btnDecrementInterval.Dispose ();
				btnDecrementInterval = null;
			}

			if (btnIncrementInterval != null) {
				btnIncrementInterval.Dispose ();
				btnIncrementInterval = null;
			}

			if (btnChangeKey != null) {
				btnChangeKey.Dispose ();
				btnChangeKey = null;
			}

			if (viewBackground != null) {
				viewBackground.Dispose ();
				viewBackground = null;
			}

			if (viewButtons != null) {
				viewButtons.Dispose ();
				viewButtons = null;
			}
		}
	}
}
