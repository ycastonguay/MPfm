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
	[Register ("AddPlaylistViewController")]
	partial class AddPlaylistViewController
	{
		[Outlet]
		MPfm.iOS.Classes.Controls.MPfmButton btnCancel { get; set; }

		[Outlet]
		MPfm.iOS.Classes.Controls.MPfmButton btnCreate { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField txtPlaylistName { get; set; }

		[Action ("actionCancel:")]
		partial void actionCancel (MonoTouch.Foundation.NSObject sender);

		[Action ("actionCreate:")]
		partial void actionCreate (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (txtPlaylistName != null) {
				txtPlaylistName.Dispose ();
				txtPlaylistName = null;
			}

			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}

			if (btnCreate != null) {
				btnCreate.Dispose ();
				btnCreate = null;
			}
		}
	}
}
