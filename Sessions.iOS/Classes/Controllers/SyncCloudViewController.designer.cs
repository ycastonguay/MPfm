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
	[Register ("SyncCloudViewController")]
	partial class SyncCloudViewController
	{
		[Outlet]
		MonoTouch.UIKit.UILabel lblDataChanged { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblIsLinked { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblValue { get; set; }

		[Action ("actionDelete:")]
		partial void actionDelete (MonoTouch.Foundation.NSObject sender);

		[Action ("actionDisableSync:")]
		partial void actionDisableSync (MonoTouch.Foundation.NSObject sender);

		[Action ("actionEnableSync:")]
		partial void actionEnableSync (MonoTouch.Foundation.NSObject sender);

		[Action ("actionPull:")]
		partial void actionPull (MonoTouch.Foundation.NSObject sender);

		[Action ("actionPush:")]
		partial void actionPush (MonoTouch.Foundation.NSObject sender);
		
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
