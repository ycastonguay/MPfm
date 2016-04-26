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
	[Register ("SyncCloudViewController")]
	partial class SyncCloudViewController
	{
		[Outlet]
		UIKit.UILabel lblDataChanged { get; set; }

		[Outlet]
		UIKit.UILabel lblIsLinked { get; set; }

		[Outlet]
		UIKit.UILabel lblValue { get; set; }

		[Action ("actionDelete:")]
		partial void actionDelete (Foundation.NSObject sender);

		[Action ("actionDisableSync:")]
		partial void actionDisableSync (Foundation.NSObject sender);

		[Action ("actionEnableSync:")]
		partial void actionEnableSync (Foundation.NSObject sender);

		[Action ("actionPull:")]
		partial void actionPull (Foundation.NSObject sender);

		[Action ("actionPush:")]
		partial void actionPush (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (lblIsLinked != null) {
				lblIsLinked.Dispose ();
				lblIsLinked = null;
			}

			if (lblDataChanged != null) {
				lblDataChanged.Dispose ();
				lblDataChanged = null;
			}

			if (lblValue != null) {
				lblValue.Dispose ();
				lblValue = null;
			}
		}
	}
}
