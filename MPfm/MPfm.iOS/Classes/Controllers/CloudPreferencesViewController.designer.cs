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
	[Register ("CloudPreferencesViewController")]
	partial class CloudPreferencesViewController
	{
		[Outlet]
		MPfm.iOS.Classes.Controls.MPfmButton btnLoginDropbox { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISwitch switchEnableResumePlayback { get; set; }

		[Action ("actionLoginDropbox:")]
		partial void actionLoginDropbox (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (switchEnableResumePlayback != null) {
				switchEnableResumePlayback.Dispose ();
				switchEnableResumePlayback = null;
			}

			if (btnLoginDropbox != null) {
				btnLoginDropbox.Dispose ();
				btnLoginDropbox = null;
			}
		}
	}
}
