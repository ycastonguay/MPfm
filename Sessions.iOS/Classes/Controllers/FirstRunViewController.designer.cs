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
	[Register ("FirstRunViewController")]
	partial class FirstRunViewController
	{
		[Outlet]
        Sessions.iOS.Classes.Controls.Buttons.SessionsButton btnClose { get; set; }

		[Action ("actionClose:")]
		partial void actionClose (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnClose != null) {
				btnClose.Dispose ();
				btnClose = null;
			}
		}
	}
}
