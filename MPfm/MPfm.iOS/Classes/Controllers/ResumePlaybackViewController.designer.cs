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
	[Register ("ResumePlaybackViewController")]
	partial class ResumePlaybackViewController
	{
		[Outlet]
		MPfm.iOS.Classes.Controls.MPfmButton btnResumePlayback { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView tableView { get; set; }

		[Action ("actionResumePlayback:")]
		partial void actionResumePlayback (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (tableView != null) {
				tableView.Dispose ();
				tableView = null;
			}

			if (btnResumePlayback != null) {
				btnResumePlayback.Dispose ();
				btnResumePlayback = null;
			}
		}
	}
}
