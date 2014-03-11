// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;
using System.CodeDom.Compiler;

namespace MPfm.Mac
{
	[Register ("SplashWindowController")]
	partial class SplashWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSImageView imageView { get; set; }

		[Outlet]
		MonoMac.AppKit.NSImageView imageViewLogo { get; set; }

		[Outlet]
		MonoMac.AppKit.NSImageView imageViewLogoFull { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblMessage { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (imageView != null) {
				imageView.Dispose ();
				imageView = null;
			}

			if (imageViewLogo != null) {
				imageViewLogo.Dispose ();
				imageViewLogo = null;
			}

			if (lblMessage != null) {
				lblMessage.Dispose ();
				lblMessage = null;
			}

			if (imageViewLogoFull != null) {
				imageViewLogoFull.Dispose ();
				imageViewLogoFull = null;
			}
		}
	}

	[Register ("SplashWindow")]
	partial class SplashWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
