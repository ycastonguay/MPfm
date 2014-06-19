// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace Sessions.iOS
{
	[Register ("SyncWebBrowserViewController")]
	partial class SyncWebBrowserViewController
	{
		[Outlet]
		MonoTouch.UIKit.UILabel lblStep1 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblStep1Text { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblStep2 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblStep2Text { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField lblUrl { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField lblAuthenticationCode { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (lblStep1 != null) {
				lblStep1.Dispose ();
				lblStep1 = null;
			}

			if (lblStep1Text != null) {
				lblStep1Text.Dispose ();
				lblStep1Text = null;
			}

			if (lblStep2 != null) {
				lblStep2.Dispose ();
				lblStep2 = null;
			}

			if (lblStep2Text != null) {
				lblStep2Text.Dispose ();
				lblStep2Text = null;
			}

			if (lblUrl != null) {
				lblUrl.Dispose ();
				lblUrl = null;
			}

			if (lblAuthenticationCode != null) {
				lblAuthenticationCode.Dispose ();
				lblAuthenticationCode = null;
			}
		}
	}
}
