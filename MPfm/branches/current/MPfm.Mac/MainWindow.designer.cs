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

		[Action ("actionAddFilesToLibrary:")]
		partial void actionAddFilesToLibrary (MonoMac.Foundation.NSObject sender);

		[Action ("actionAddFolderLibrary:")]
		partial void actionAddFolderLibrary (MonoMac.Foundation.NSObject sender);

		[Action ("actionOpenAudioFiles:")]
		partial void actionOpenAudioFiles (MonoMac.Foundation.NSObject sender);

		[Action ("actionUpdateLibrary:")]
		partial void actionUpdateLibrary (MonoMac.Foundation.NSObject sender);

		[Action ("actionPlay:")]
		partial void actionPlay (MonoMac.Foundation.NSObject sender);

		[Action ("actionPause:")]
		partial void actionPause (MonoMac.Foundation.NSObject sender);

		[Action ("actionPrevious:")]
		partial void actionPrevious (MonoMac.Foundation.NSObject sender);

		[Action ("actionNext:")]
		partial void actionNext (MonoMac.Foundation.NSObject sender);

		[Action ("actionStop:")]
		partial void actionStop (MonoMac.Foundation.NSObject sender);

		[Action ("actionRepeatType:")]
		partial void actionRepeatType (MonoMac.Foundation.NSObject sender);

		[Action ("actionOpenPlaylistWindow:")]
		partial void actionOpenPlaylistWindow (MonoMac.Foundation.NSObject sender);

		[Action ("actionOpenEffectsWindow:")]
		partial void actionOpenEffectsWindow (MonoMac.Foundation.NSObject sender);

		[Action ("actionOpenPreferencesWindow:")]
		partial void actionOpenPreferencesWindow (MonoMac.Foundation.NSObject sender);
		
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
