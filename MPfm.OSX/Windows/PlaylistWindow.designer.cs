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
	[Register ("PlaylistWindowController")]
	partial class PlaylistWindowController
	{
		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmRoundButton btnToolbarNew { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmRoundButton btnToolbarOpen { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmRoundButton btnToolbarSave { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmRoundButton btnToolbarSaveAs { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitle { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmSongGridView songGridView { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmView viewTitle { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmView viewToolbar { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (viewTitle != null) {
				viewTitle.Dispose ();
				viewTitle = null;
			}

			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}

			if (viewToolbar != null) {
				viewToolbar.Dispose ();
				viewToolbar = null;
			}

			if (songGridView != null) {
				songGridView.Dispose ();
				songGridView = null;
			}

			if (btnToolbarNew != null) {
				btnToolbarNew.Dispose ();
				btnToolbarNew = null;
			}

			if (btnToolbarOpen != null) {
				btnToolbarOpen.Dispose ();
				btnToolbarOpen = null;
			}

			if (btnToolbarSave != null) {
				btnToolbarSave.Dispose ();
				btnToolbarSave = null;
			}

			if (btnToolbarSaveAs != null) {
				btnToolbarSaveAs.Dispose ();
				btnToolbarSaveAs = null;
			}
		}
	}

	[Register ("PlaylistWindow")]
	partial class PlaylistWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
