// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace Sessions.iOS
{
	[Register ("QueueViewController")]
	partial class QueueViewController
	{
		[Outlet]
		Sessions.iOS.Classes.Controls.SessionsPlayerButton btnPlay { get; set; }

		[Outlet]
		Sessions.iOS.Classes.Controls.SessionsPlayerButton btnRemoveAll { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblSubtitle { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblTitle { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}

			if (lblSubtitle != null) {
				lblSubtitle.Dispose ();
				lblSubtitle = null;
			}

			if (btnRemoveAll != null) {
				btnRemoveAll.Dispose ();
				btnRemoveAll = null;
			}

			if (btnPlay != null) {
				btnPlay.Dispose ();
				btnPlay = null;
			}
		}
	}
}
