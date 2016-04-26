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
	[Register ("PitchShiftingViewController")]
	partial class PitchShiftingViewController
	{
		[Outlet]
        Sessions.iOS.Classes.Controls.Buttons.SessionsSemiTransparentButton btnChangeKey { get; set; }

		[Outlet]
        Sessions.iOS.Classes.Controls.Buttons.SessionsSemiTransparentButton btnDecrementInterval { get; set; }

		[Outlet]
        Sessions.iOS.Classes.Controls.Buttons.SessionsSemiTransparentButton btnIncrementInterval { get; set; }

		[Outlet]
		UIKit.UIButton btnReset { get; set; }

		[Outlet]
		UIKit.UILabel lblInterval { get; set; }

		[Outlet]
		UIKit.UILabel lblKey { get; set; }

		[Outlet]
		UIKit.UILabel lblNewKey { get; set; }

		[Outlet]
		UIKit.UILabel lblTitle { get; set; }

		[Outlet]
		UIKit.UISlider slider { get; set; }

		[Outlet]
		UIKit.UIView viewBackground { get; set; }

		[Outlet]
		UIKit.UIView viewButtons { get; set; }

		[Action ("actionChangeKey:")]
		partial void actionChangeKey (Foundation.NSObject sender);

		[Action ("actionDecrementInterval:")]
		partial void actionDecrementInterval (Foundation.NSObject sender);

		[Action ("actionIncrementInterval:")]
		partial void actionIncrementInterval (Foundation.NSObject sender);

		[Action ("actionReset:")]
		partial void actionReset (Foundation.NSObject sender);
		
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
