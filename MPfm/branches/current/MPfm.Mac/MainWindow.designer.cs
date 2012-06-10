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
		MonoMac.AppKit.NSTextField lblFileType { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblBitsPerSample { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSampleRate { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblBitrate { get; set; }

		[Outlet]
		MonoMac.AppKit.NSImageView imageAlbumCover { get; set; }

		[Outlet]
		MonoMac.AppKit.NSSplitView splitMain { get; set; }

		[Outlet]
		MPfm.Mac.SongPositionSlider sliderPosition { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblLength { get; set; }

		[Outlet]
		MonoMac.AppKit.NSOutlineView viewLibraryBrowser { get; set; }

		[Outlet]
		MonoMac.AppKit.NSPopUpButton cboSoundFormat { get; set; }

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

		[Action ("actionSoundFormatChanged:")]
		partial void actionSoundFormatChanged (MonoMac.Foundation.NSObject sender);

		[Action ("actionPlay:")]
		partial void actionPlay (MonoMac.Foundation.NSObject sender);

		[Action ("actionPause:")]
		partial void actionPause (MonoMac.Foundation.NSObject sender);

		[Action ("actionStop:")]
		partial void actionStop (MonoMac.Foundation.NSObject sender);

		[Action ("actionPrevious:")]
		partial void actionPrevious (MonoMac.Foundation.NSObject sender);

		[Action ("actionNext:")]
		partial void actionNext (MonoMac.Foundation.NSObject sender);

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
			if (lblFileType != null) {
				lblFileType.Dispose ();
				lblFileType = null;
			}

			if (lblBitsPerSample != null) {
				lblBitsPerSample.Dispose ();
				lblBitsPerSample = null;
			}

			if (lblSampleRate != null) {
				lblSampleRate.Dispose ();
				lblSampleRate = null;
			}

			if (lblBitrate != null) {
				lblBitrate.Dispose ();
				lblBitrate = null;
			}

			if (imageAlbumCover != null) {
				imageAlbumCover.Dispose ();
				imageAlbumCover = null;
			}

			if (splitMain != null) {
				splitMain.Dispose ();
				splitMain = null;
			}

			if (sliderPosition != null) {
				sliderPosition.Dispose ();
				sliderPosition = null;
			}

			if (lblLength != null) {
				lblLength.Dispose ();
				lblLength = null;
			}

			if (viewLibraryBrowser != null) {
				viewLibraryBrowser.Dispose ();
				viewLibraryBrowser = null;
			}

			if (cboSoundFormat != null) {
				cboSoundFormat.Dispose ();
				cboSoundFormat = null;
			}

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
