// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace MPfm.Mac
{
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSToolbar toolbarMain { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblArtistName { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblAlbumTitle { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSongTitle { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSongPath { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblPosition { get; set; }

		[Action ("_ButtonClick:")]
		partial void _ButtonClick (MonoMac.Foundation.NSObject sender);

		[Action ("toolbarOpenAudioFiles_Click:")]
		partial void toolbarOpenAudioFiles_Click (MonoMac.Foundation.NSObject sender);

		[Action ("toolbarPlay:")]
		partial void toolbarPlay (MonoMac.Foundation.NSObject sender);

		[Action ("toolbarStop:")]
		partial void toolbarStop (MonoMac.Foundation.NSObject sender);

		[Action ("toolbarNext:")]
		partial void toolbarNext (MonoMac.Foundation.NSObject sender);

		[Action ("toolbarPrevious:")]
		partial void toolbarPrevious (MonoMac.Foundation.NSObject sender);

		[Action ("toolbarUpdateLibrary:")]
		partial void toolbarUpdateLibrary (MonoMac.Foundation.NSObject sender);

		[Action ("toolbarEffects:")]
		partial void toolbarEffects (MonoMac.Foundation.NSObject sender);

		[Action ("toolbarPlaylist:")]
		partial void toolbarPlaylist (MonoMac.Foundation.NSObject sender);

		[Action ("toolbarPreferences:")]
		partial void toolbarPreferences (MonoMac.Foundation.NSObject sender);

		[Action ("toolbarRepeatType:")]
		partial void toolbarRepeatType (MonoMac.Foundation.NSObject sender);

		[Action ("toolbarPause:")]
		partial void toolbarPause (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (toolbarMain != null) {
				toolbarMain.Dispose ();
				toolbarMain = null;
			}

			if (lblArtistName != null) {
				lblArtistName.Dispose ();
				lblArtistName = null;
			}

			if (lblAlbumTitle != null) {
				lblAlbumTitle.Dispose ();
				lblAlbumTitle = null;
			}

			if (lblSongTitle != null) {
				lblSongTitle.Dispose ();
				lblSongTitle = null;
			}

			if (lblSongPath != null) {
				lblSongPath.Dispose ();
				lblSongPath = null;
			}

			if (lblPosition != null) {
				lblPosition.Dispose ();
				lblPosition = null;
			}
		}
	}

	[Register ("MainWindow")]
	partial class MainWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
