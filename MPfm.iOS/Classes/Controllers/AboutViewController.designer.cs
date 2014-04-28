// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace MPfm.iOS
{
	[Register ("AboutViewController")]
	partial class AboutViewController
	{
		[Outlet]
		MonoTouch.UIKit.UITextView textView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (textView != null) {
				textView.Dispose ();
				textView = null;
			}
		}
	}
}
