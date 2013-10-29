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
	[Register ("StartResumePlaybackViewController")]
	partial class StartResumePlaybackViewController
	{
		[Outlet]
		MPfm.iOS.Classes.Controls.MPfmButton btnCancel { get; set; }

		[Outlet]
		MPfm.iOS.Classes.Controls.MPfmButton btnResume { get; set; }

		[Action ("actionCancel:")]
		partial void actionCancel (MonoTouch.Foundation.NSObject sender);

		[Action ("actionResume:")]
		partial void actionResume (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnResume != null) {
				btnResume.Dispose ();
				btnResume = null;
			}

			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}
		}
	}
}
