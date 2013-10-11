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
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmButton btnAddLoop { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmButton btnAddMarker { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnAddSongToPlaylist { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmButton btnChangeKey { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnDetectTempo { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmButton btnEditLoop { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmButton btnEditMarker { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnEditSongMetadata { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmButton btnGoToMarker { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmButton btnPlayLoop { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnPlaySelectedSong { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmButton btnRemoveLoop { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmButton btnRemoveMarker { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmTabButton btnTabActions { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmTabButton btnTabInfo { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmTabButton btnTabPitchShifting { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmTabButton btnTabTimeShifting { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmButton btnUseTempo { get; set; }

		[Outlet]
		MonoMac.AppKit.NSPopUpButton cboSoundFormat { get; set; }

		[Outlet]
		MonoMac.AppKit.NSImageView imageAlbumCover { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblAlbumTitle { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblArtistName { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblBitrate { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblBitsPerSample { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblCurrentTempo { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblDetectedTempo { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblDetectedTempoValue { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblFileSize { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblFileType { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblFilterBySoundFormat { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblGenre { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblLastPlayed { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblLength { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblMonoStereo { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblNewKey { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblPlayCount { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblPosition { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblReferenceKey { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblReferenceTempo { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblReferenceTempoValue { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSampleRate { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSongPath { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSongTitle { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSubtitleInformation { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSubtitleSongPosition { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSubtitleVolume { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitleCurrentSong { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitleLibraryBrowser { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitleLoops { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitleMarkers { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitleSongBrowser { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblVolume { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblYear { get; set; }

		[Outlet]
		MonoMac.AppKit.NSOutlineView outlineLibraryBrowser { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmScrollView scrollViewAlbumCovers { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmScrollView scrollViewLibraryBrowser { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmScrollView scrollViewSongBrowser { get; set; }

		[Outlet]
		MonoMac.AppKit.NSSearchField searchSongBrowser { get; set; }

		[Outlet]
		MonoMac.AppKit.NSSlider sliderPitchShifting { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmSongPositionSlider sliderPosition { get; set; }

		[Outlet]
		MonoMac.AppKit.NSSlider sliderTimeShifting { get; set; }

		[Outlet]
		MonoMac.AppKit.NSSlider sliderVolume { get; set; }

		[Outlet]
		MonoMac.AppKit.NSSplitView splitMain { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTableView tableAlbumCovers { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTableView tableLoops { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTableView tableMarkers { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTableView tableSongBrowser { get; set; }

		[Outlet]
		MonoMac.AppKit.NSToolbar toolbarMain { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtCurrentTempoValue { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtInterval { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewActions { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewInformation { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewLeft { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewLeftHeader { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewLibraryBrowser { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewLoopsHeader { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewMarkersHeader { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewNowPlaying { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewPitchShifting { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewRight { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewRightHeader { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewSongBrowserHeader { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewSongPosition { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewTimeShifting { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewVolume { get; set; }

		[Action ("actionAddFilesToLibrary:")]
		partial void actionAddFilesToLibrary (MonoMac.Foundation.NSObject sender);

		[Action ("actionAddFolderLibrary:")]
		partial void actionAddFolderLibrary (MonoMac.Foundation.NSObject sender);

		[Action ("actionAddLoop:")]
		partial void actionAddLoop (MonoMac.Foundation.NSObject sender);

		[Action ("actionAddMarker:")]
		partial void actionAddMarker (MonoMac.Foundation.NSObject sender);

		[Action ("actionAddSongToPlaylist:")]
		partial void actionAddSongToPlaylist (MonoMac.Foundation.NSObject sender);

		[Action ("actionChangeKey:")]
		partial void actionChangeKey (MonoMac.Foundation.NSObject sender);

		[Action ("actionChangeSongPosition:")]
		partial void actionChangeSongPosition (MonoMac.Foundation.NSObject sender);

		[Action ("actionChangeTimeShifting:")]
		partial void actionChangeTimeShifting (MonoMac.Foundation.NSObject sender);

		[Action ("actionChangeVolume:")]
		partial void actionChangeVolume (MonoMac.Foundation.NSObject sender);

		[Action ("actionContextualMenuPlay:")]
		partial void actionContextualMenuPlay (MonoMac.Foundation.NSObject sender);

		[Action ("actionDecrementPitchShifting:")]
		partial void actionDecrementPitchShifting (MonoMac.Foundation.NSObject sender);

		[Action ("actionDecrementTimeShifting:")]
		partial void actionDecrementTimeShifting (MonoMac.Foundation.NSObject sender);

		[Action ("actionEditLoop:")]
		partial void actionEditLoop (MonoMac.Foundation.NSObject sender);

		[Action ("actionEditMarker:")]
		partial void actionEditMarker (MonoMac.Foundation.NSObject sender);

		[Action ("actionEditSongMetadata:")]
		partial void actionEditSongMetadata (MonoMac.Foundation.NSObject sender);

		[Action ("actionGoToMarker:")]
		partial void actionGoToMarker (MonoMac.Foundation.NSObject sender);

		[Action ("actionIncrementPitchShfiting:")]
		partial void actionIncrementPitchShfiting (MonoMac.Foundation.NSObject sender);

		[Action ("actionIncrementTimeShifting:")]
		partial void actionIncrementTimeShifting (MonoMac.Foundation.NSObject sender);

		[Action ("actionNext:")]
		partial void actionNext (MonoMac.Foundation.NSObject sender);

		[Action ("actionOpenAudioFiles:")]
		partial void actionOpenAudioFiles (MonoMac.Foundation.NSObject sender);

		[Action ("actionOpenEffectsWindow:")]
		partial void actionOpenEffectsWindow (MonoMac.Foundation.NSObject sender);

		[Action ("actionOpenMainWindow:")]
		partial void actionOpenMainWindow (MonoMac.Foundation.NSObject sender);

		[Action ("actionOpenPlaylistWindow:")]
		partial void actionOpenPlaylistWindow (MonoMac.Foundation.NSObject sender);

		[Action ("actionOpenPreferencesWindow:")]
		partial void actionOpenPreferencesWindow (MonoMac.Foundation.NSObject sender);

		[Action ("actionOpenSyncWindow:")]
		partial void actionOpenSyncWindow (MonoMac.Foundation.NSObject sender);

		[Action ("actionPlay:")]
		partial void actionPlay (MonoMac.Foundation.NSObject sender);

		[Action ("actionPlayLoop:")]
		partial void actionPlayLoop (MonoMac.Foundation.NSObject sender);

		[Action ("actionPlaySelectedSong:")]
		partial void actionPlaySelectedSong (MonoMac.Foundation.NSObject sender);

		[Action ("actionPrevious:")]
		partial void actionPrevious (MonoMac.Foundation.NSObject sender);

		[Action ("actionRemoveLoop:")]
		partial void actionRemoveLoop (MonoMac.Foundation.NSObject sender);

		[Action ("actionRemoveMarker:")]
		partial void actionRemoveMarker (MonoMac.Foundation.NSObject sender);

		[Action ("actionRepeatType:")]
		partial void actionRepeatType (MonoMac.Foundation.NSObject sender);

		[Action ("actionResetPitchShifting:")]
		partial void actionResetPitchShifting (MonoMac.Foundation.NSObject sender);

		[Action ("actionResetTimeShifting:")]
		partial void actionResetTimeShifting (MonoMac.Foundation.NSObject sender);

		[Action ("actionSearchBassTabs:")]
		partial void actionSearchBassTabs (MonoMac.Foundation.NSObject sender);

		[Action ("actionSearchGuitarTabs:")]
		partial void actionSearchGuitarTabs (MonoMac.Foundation.NSObject sender);

		[Action ("actionSearchLyrics:")]
		partial void actionSearchLyrics (MonoMac.Foundation.NSObject sender);

		[Action ("actionShuffle:")]
		partial void actionShuffle (MonoMac.Foundation.NSObject sender);

		[Action ("actionSoundFormatChanged:")]
		partial void actionSoundFormatChanged (MonoMac.Foundation.NSObject sender);

		[Action ("actionTabActions:")]
		partial void actionTabActions (MonoMac.Foundation.NSObject sender);

		[Action ("actionTabInfo:")]
		partial void actionTabInfo (MonoMac.Foundation.NSObject sender);

		[Action ("actionTabPitchShifting:")]
		partial void actionTabPitchShifting (MonoMac.Foundation.NSObject sender);

		[Action ("actionTabTimeShifting:")]
		partial void actionTabTimeShifting (MonoMac.Foundation.NSObject sender);

		[Action ("actionUpdateLibrary:")]
		partial void actionUpdateLibrary (MonoMac.Foundation.NSObject sender);

		[Action ("actionUseTempo:")]
		partial void actionUseTempo (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (tableAlbumCovers != null) {
				tableAlbumCovers.Dispose ();
				tableAlbumCovers = null;
			}

			if (scrollViewAlbumCovers != null) {
				scrollViewAlbumCovers.Dispose ();
				scrollViewAlbumCovers = null;
			}

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

			if (viewPitchShifting != null) {
				viewPitchShifting.Dispose ();
				viewPitchShifting = null;
			}

			if (sliderPitchShifting != null) {
				sliderPitchShifting.Dispose ();
				sliderPitchShifting = null;
			}

			if (btnPlayLoop != null) {
				btnPlayLoop.Dispose ();
				btnPlayLoop = null;
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

			if (btnDetectTempo != null) {
				btnDetectTempo.Dispose ();
				btnDetectTempo = null;
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

			if (lblFilterBySoundFormat != null) {
				lblFilterBySoundFormat.Dispose ();
				lblFilterBySoundFormat = null;
			}

			if (viewLoopsHeader != null) {
				viewLoopsHeader.Dispose ();
				viewLoopsHeader = null;
			}

			if (viewMarkersHeader != null) {
				viewMarkersHeader.Dispose ();
				viewMarkersHeader = null;
			}

			if (viewSongBrowserHeader != null) {
				viewSongBrowserHeader.Dispose ();
				viewSongBrowserHeader = null;
			}

			if (lblDetectedTempo != null) {
				lblDetectedTempo.Dispose ();
				lblDetectedTempo = null;
			}

			if (lblDetectedTempoValue != null) {
				lblDetectedTempoValue.Dispose ();
				lblDetectedTempoValue = null;
			}

			if (lblCurrentTempo != null) {
				lblCurrentTempo.Dispose ();
				lblCurrentTempo = null;
			}

			if (txtCurrentTempoValue != null) {
				txtCurrentTempoValue.Dispose ();
				txtCurrentTempoValue = null;
			}

			if (lblReferenceTempo != null) {
				lblReferenceTempo.Dispose ();
				lblReferenceTempo = null;
			}

			if (lblReferenceTempoValue != null) {
				lblReferenceTempoValue.Dispose ();
				lblReferenceTempoValue = null;
			}

			if (btnTabTimeShifting != null) {
				btnTabTimeShifting.Dispose ();
				btnTabTimeShifting = null;
			}

			if (btnTabPitchShifting != null) {
				btnTabPitchShifting.Dispose ();
				btnTabPitchShifting = null;
			}

			if (btnTabInfo != null) {
				btnTabInfo.Dispose ();
				btnTabInfo = null;
			}

			if (btnTabActions != null) {
				btnTabActions.Dispose ();
				btnTabActions = null;
			}

			if (viewActions != null) {
				viewActions.Dispose ();
				viewActions = null;
			}

			if (lblYear != null) {
				lblYear.Dispose ();
				lblYear = null;
			}

			if (lblMonoStereo != null) {
				lblMonoStereo.Dispose ();
				lblMonoStereo = null;
			}

			if (lblGenre != null) {
				lblGenre.Dispose ();
				lblGenre = null;
			}

			if (lblFileSize != null) {
				lblFileSize.Dispose ();
				lblFileSize = null;
			}

			if (lblPlayCount != null) {
				lblPlayCount.Dispose ();
				lblPlayCount = null;
			}

			if (lblLastPlayed != null) {
				lblLastPlayed.Dispose ();
				lblLastPlayed = null;
			}

			if (btnUseTempo != null) {
				btnUseTempo.Dispose ();
				btnUseTempo = null;
			}

			if (btnChangeKey != null) {
				btnChangeKey.Dispose ();
				btnChangeKey = null;
			}

			if (lblReferenceKey != null) {
				lblReferenceKey.Dispose ();
				lblReferenceKey = null;
			}

			if (txtInterval != null) {
				txtInterval.Dispose ();
				txtInterval = null;
			}

			if (lblNewKey != null) {
				lblNewKey.Dispose ();
				lblNewKey = null;
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
