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
		MPfm.Mac.MPfmScrollView scrollViewLibraryBrowser { get; set; }

		[Outlet]
		MPfm.Mac.MPfmScrollView scrollViewSongBrowser { get; set; }

		[Outlet]
		MPfm.Mac.MPfmView viewTimeShifting { get; set; }

		[Outlet]
		MPfm.Mac.MPfmView viewSongPosition { get; set; }

		[Outlet]
		MPfm.Mac.MPfmView viewInformation { get; set; }

		[Outlet]
		MPfm.Mac.MPfmView viewVolume { get; set; }

		[Outlet]
		MonoMac.AppKit.NSOutlineView outlineLibraryBrowser { get; set; }

		[Outlet]
		MPfm.Mac.MPfmView viewLeftHeader { get; set; }

		[Outlet]
		MPfm.Mac.MPfmView viewRightHeader { get; set; }

		[Outlet]
		MPfm.Mac.MPfmView viewNowPlaying { get; set; }

		[Outlet]
		MPfm.Mac.MPfmView viewLibraryBrowser { get; set; }

		[Outlet]
		MPfm.Mac.MPfmView viewLeft { get; set; }

		[Outlet]
		MPfm.Mac.MPfmView viewRight { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnPlayLoop { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnStopLoop { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnAddLoop { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnEditLoop { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnRemoveLoop { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnGoToMarker { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnAddMarker { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnEditMarker { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnRemoveMarker { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTableView tableLoops { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTableView tableMarkers { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnPlaySelectedSong { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnAddSongToPlaylist { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnEditSongMetadata { get; set; }

		[Outlet]
		MonoMac.AppKit.NSSlider sliderTimeShifting { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTableView tableSongBrowser { get; set; }

		[Outlet]
		MonoMac.AppKit.NSSearchField searchSongBrowser { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTimeShifting { get; set; }

		[Outlet]
		MonoMac.AppKit.NSSlider sliderVolume { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblVolume { get; set; }

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

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitleLoops { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitleMarkers { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitleSongBrowser { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitleCurrentSong { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitleLibraryBrowser { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSubtitleSongPosition { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSubtitleInformation { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSubtitleVolume { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSubtitleTimeShifting { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblFilterBySoundFormat { get; set; }

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

		[Action ("actionOpenMainWindow:")]
		partial void actionOpenMainWindow (MonoMac.Foundation.NSObject sender);

		[Action ("actionOpenPlaylistWindow:")]
		partial void actionOpenPlaylistWindow (MonoMac.Foundation.NSObject sender);

		[Action ("actionOpenEffectsWindow:")]
		partial void actionOpenEffectsWindow (MonoMac.Foundation.NSObject sender);

		[Action ("actionOpenPreferencesWindow:")]
		partial void actionOpenPreferencesWindow (MonoMac.Foundation.NSObject sender);

		[Action ("actionAddSongToPlaylist:")]
		partial void actionAddSongToPlaylist (MonoMac.Foundation.NSObject sender);

		[Action ("actionEditSongMetadata:")]
		partial void actionEditSongMetadata (MonoMac.Foundation.NSObject sender);

		[Action ("actionPlaySelectedSong:")]
		partial void actionPlaySelectedSong (MonoMac.Foundation.NSObject sender);

		[Action ("actionChangeTimeShifting:")]
		partial void actionChangeTimeShifting (MonoMac.Foundation.NSObject sender);

		[Action ("actionChangeSongPosition:")]
		partial void actionChangeSongPosition (MonoMac.Foundation.NSObject sender);

		[Action ("actionChangeVolume:")]
		partial void actionChangeVolume (MonoMac.Foundation.NSObject sender);

		[Action ("actionPlayLoop:")]
		partial void actionPlayLoop (MonoMac.Foundation.NSObject sender);

		[Action ("actionStopLoop:")]
		partial void actionStopLoop (MonoMac.Foundation.NSObject sender);

		[Action ("actionAddLoop:")]
		partial void actionAddLoop (MonoMac.Foundation.NSObject sender);

		[Action ("actionEditLoop:")]
		partial void actionEditLoop (MonoMac.Foundation.NSObject sender);

		[Action ("actionRemoveLoop:")]
		partial void actionRemoveLoop (MonoMac.Foundation.NSObject sender);

		[Action ("actionGoToMarker:")]
		partial void actionGoToMarker (MonoMac.Foundation.NSObject sender);

		[Action ("actionAddMarker:")]
		partial void actionAddMarker (MonoMac.Foundation.NSObject sender);

		[Action ("actionEditMarker:")]
		partial void actionEditMarker (MonoMac.Foundation.NSObject sender);

		[Action ("actionRemoveMarker:")]
		partial void actionRemoveMarker (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (scrollViewLibraryBrowser != null) {
				scrollViewLibraryBrowser.Dispose ();
				scrollViewLibraryBrowser = null;
			}

			if (scrollViewSongBrowser != null) {
				scrollViewSongBrowser.Dispose ();
				scrollViewSongBrowser = null;
			}

			if (viewTimeShifting != null) {
				viewTimeShifting.Dispose ();
				viewTimeShifting = null;
			}

			if (viewSongPosition != null) {
				viewSongPosition.Dispose ();
				viewSongPosition = null;
			}

			if (viewInformation != null) {
				viewInformation.Dispose ();
				viewInformation = null;
			}

			if (viewVolume != null) {
				viewVolume.Dispose ();
				viewVolume = null;
			}

			if (outlineLibraryBrowser != null) {
				outlineLibraryBrowser.Dispose ();
				outlineLibraryBrowser = null;
			}

			if (viewLeftHeader != null) {
				viewLeftHeader.Dispose ();
				viewLeftHeader = null;
			}

			if (viewRightHeader != null) {
				viewRightHeader.Dispose ();
				viewRightHeader = null;
			}

			if (viewNowPlaying != null) {
				viewNowPlaying.Dispose ();
				viewNowPlaying = null;
			}

			if (viewLibraryBrowser != null) {
				viewLibraryBrowser.Dispose ();
				viewLibraryBrowser = null;
			}

			if (viewLeft != null) {
				viewLeft.Dispose ();
				viewLeft = null;
			}

			if (viewRight != null) {
				viewRight.Dispose ();
				viewRight = null;
			}

			if (btnPlayLoop != null) {
				btnPlayLoop.Dispose ();
				btnPlayLoop = null;
			}

			if (btnStopLoop != null) {
				btnStopLoop.Dispose ();
				btnStopLoop = null;
			}

			if (btnAddLoop != null) {
				btnAddLoop.Dispose ();
				btnAddLoop = null;
			}

			if (btnEditLoop != null) {
				btnEditLoop.Dispose ();
				btnEditLoop = null;
			}

			if (btnRemoveLoop != null) {
				btnRemoveLoop.Dispose ();
				btnRemoveLoop = null;
			}

			if (btnGoToMarker != null) {
				btnGoToMarker.Dispose ();
				btnGoToMarker = null;
			}

			if (btnAddMarker != null) {
				btnAddMarker.Dispose ();
				btnAddMarker = null;
			}

			if (btnEditMarker != null) {
				btnEditMarker.Dispose ();
				btnEditMarker = null;
			}

			if (btnRemoveMarker != null) {
				btnRemoveMarker.Dispose ();
				btnRemoveMarker = null;
			}

			if (tableLoops != null) {
				tableLoops.Dispose ();
				tableLoops = null;
			}

			if (tableMarkers != null) {
				tableMarkers.Dispose ();
				tableMarkers = null;
			}

			if (btnPlaySelectedSong != null) {
				btnPlaySelectedSong.Dispose ();
				btnPlaySelectedSong = null;
			}

			if (btnAddSongToPlaylist != null) {
				btnAddSongToPlaylist.Dispose ();
				btnAddSongToPlaylist = null;
			}

			if (btnEditSongMetadata != null) {
				btnEditSongMetadata.Dispose ();
				btnEditSongMetadata = null;
			}

			if (sliderTimeShifting != null) {
				sliderTimeShifting.Dispose ();
				sliderTimeShifting = null;
			}

			if (tableSongBrowser != null) {
				tableSongBrowser.Dispose ();
				tableSongBrowser = null;
			}

			if (searchSongBrowser != null) {
				searchSongBrowser.Dispose ();
				searchSongBrowser = null;
			}

			if (lblTimeShifting != null) {
				lblTimeShifting.Dispose ();
				lblTimeShifting = null;
			}

			if (sliderVolume != null) {
				sliderVolume.Dispose ();
				sliderVolume = null;
			}

			if (lblVolume != null) {
				lblVolume.Dispose ();
				lblVolume = null;
			}

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

			if (lblTitleLoops != null) {
				lblTitleLoops.Dispose ();
				lblTitleLoops = null;
			}

			if (lblTitleMarkers != null) {
				lblTitleMarkers.Dispose ();
				lblTitleMarkers = null;
			}

			if (lblTitleSongBrowser != null) {
				lblTitleSongBrowser.Dispose ();
				lblTitleSongBrowser = null;
			}

			if (lblTitleCurrentSong != null) {
				lblTitleCurrentSong.Dispose ();
				lblTitleCurrentSong = null;
			}

			if (lblTitleLibraryBrowser != null) {
				lblTitleLibraryBrowser.Dispose ();
				lblTitleLibraryBrowser = null;
			}

			if (lblSubtitleSongPosition != null) {
				lblSubtitleSongPosition.Dispose ();
				lblSubtitleSongPosition = null;
			}

			if (lblSubtitleInformation != null) {
				lblSubtitleInformation.Dispose ();
				lblSubtitleInformation = null;
			}

			if (lblSubtitleVolume != null) {
				lblSubtitleVolume.Dispose ();
				lblSubtitleVolume = null;
			}

			if (lblSubtitleTimeShifting != null) {
				lblSubtitleTimeShifting.Dispose ();
				lblSubtitleTimeShifting = null;
			}

			if (lblFilterBySoundFormat != null) {
				lblFilterBySoundFormat.Dispose ();
				lblFilterBySoundFormat = null;
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
