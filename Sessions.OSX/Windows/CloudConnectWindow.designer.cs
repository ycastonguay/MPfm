// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;
using System.CodeDom.Compiler;

namespace MPfm.OSX
{
	[Register ("CloudConnectWindowController")]
	partial class CloudConnectWindowController
	{
		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmButton btnCancel { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmButton btnOK { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblStep1 { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblStep2 { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblStep2B { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblStep3 { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblStep4 { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitle { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmProgressBarView progressBar { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}

			if (btnOK != null) {
				btnOK.Dispose ();
				btnOK = null;
			}

			if (lblStep1 != null) {
				lblStep1.Dispose ();
				lblStep1 = null;
			}

			if (lblStep2 != null) {
				lblStep2.Dispose ();
				lblStep2 = null;
			}

			if (lblStep2B != null) {
				lblStep2B.Dispose ();
				lblStep2B = null;
			}

			if (lblStep3 != null) {
				lblStep3.Dispose ();
				lblStep3 = null;
			}

			if (lblStep4 != null) {
				lblStep4.Dispose ();
				lblStep4 = null;
			}

			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}

			if (progressBar != null) {
				progressBar.Dispose ();
				progressBar = null;
			}
		}
	}

	[Register ("CloudConnectWindow")]
	partial class CloudConnectWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
