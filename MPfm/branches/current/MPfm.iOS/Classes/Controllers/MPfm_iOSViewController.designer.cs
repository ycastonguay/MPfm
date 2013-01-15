// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace MPfm.iOS
{
	[Register ("MPfm_iOSViewController")]
	partial class MPfm_iOSViewController
	{
		[Outlet]
		MonoTouch.UIKit.UILabel lblPosition { get; set; }

		[Action ("actionPlay:")]
		partial void actionPlay (MonoTouch.Foundation.NSObject sender);

		[Action ("actionPause:")]
		partial void actionPause (MonoTouch.Foundation.NSObject sender);

		[Action ("actionStop:")]
		partial void actionStop (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (lblPosition != null) {
				lblPosition.Dispose ();
				lblPosition = null;
			}
		}
	}
}
