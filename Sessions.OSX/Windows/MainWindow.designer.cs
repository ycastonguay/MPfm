// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;
using System.CodeDom.Compiler;

namespace Sessions.OSX
{
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsButton btnAddLoop { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsButton btnAddMarker { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsButton btnAddSegment { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsButton btnBackLoop { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsButton btnBackLoopPlayback { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsButton btnBackMarker { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsButton btnBackSegment { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsButton btnChangeKey { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsRoundButton btnDecrementPitchShifting { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsRoundButton btnDecrementTimeShifting { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsRoundButton btnDeleteQueue { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnDetectTempo { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsButton btnEditLoop { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsButton btnEditMarker { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsButton btnEditSegment { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnEditSongMetadata { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsButton btnGoToMarker { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsRoundButton btnIncrementPitchShifting { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsRoundButton btnIncrementTimeShifting { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsRoundButton btnNextLoop { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsRoundButton btnNextSegment { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsButton btnPlayLoop { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsRoundButton btnPlayQueue { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsRoundButton btnPreviousLoop { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsRoundButton btnPreviousSegment { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsButton btnPunchInMarker { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsButton btnPunchInSegment { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsButton btnRemoveLoop { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsButton btnRemoveMarker { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsButton btnRemoveSegment { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsRoundButton btnResetPitchShifting { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsRoundButton btnResetTimeShifting { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsTabButton btnTabActions { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsTabButton btnTabInfo { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsTabButton btnTabPitchShifting { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsTabButton btnTabTimeShifting { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsRoundButton btnToolbarEffects { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsRoundButton btnToolbarNext { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsRoundButton btnToolbarPlaylist { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsRoundButton btnToolbarPlayPause { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsRoundButton btnToolbarPrevious { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsRoundButton btnToolbarRepeat { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsRoundButton btnToolbarSettings { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsRoundButton btnToolbarShuffle { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsRoundButton btnToolbarSync { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsButton btnUseTempo { get; set; }

		[Outlet]
		MonoMac.AppKit.NSPopUpButton cboSoundFormat { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsCheckBoxView chkSegmentLinkToMarker { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsPopUpButton comboSegmentMarker { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsFaderView faderVolume { get; set; }

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
		MonoMac.AppKit.NSTextField lblCurrentLoop { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblCurrentSegment { get; set; }

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
		MonoMac.AppKit.NSTextField lblInterval { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblLastPlayed { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblLength { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblLoopName { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblLoopPosition { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblMarkerName { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblMarkerPosition { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblMarkerPositionValue { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblMonoStereo { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblNewKey { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblNewKeyValue { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblPlayCount { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblPosition { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblQueueDetails { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblReferenceKey { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblReferenceKeyValue { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblReferenceTempo { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblReferenceTempoValue { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSampleRate { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSearchWeb { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsLabel lblSegmentLinkToMarker { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSegmentPosition { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSegmentPositionValue { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSongPath { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSongTitle { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSubtitleSongPosition { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSubtitleVolume { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitleCurrentSong { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitleLibraryBrowser { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitleLoopDetails { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitleLoopPlayback { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitleLoops { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitleMarkerDetails { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitleMarkers { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitleQueue { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitleSegmentDetails { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitleSegments { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitleSongBrowser { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitleUpdateLibrary { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblUpdateLibraryStatus { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblVolume { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblYear { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem menuAddToPlaylist { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem menuAddToQueue { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem menuDeleteFromHardDisk { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem menuPlay { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem menuRemoveFromLibrary { get; set; }

		[Outlet]
		MonoMac.AppKit.NSOutlineView outlineLibraryBrowser { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsOutputMeterView outputMeter { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsProgressBarView progressBarUpdateLibrary { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsScrollView scrollViewLibraryBrowser { get; set; }

		[Outlet]
		MonoMac.AppKit.NSSearchField searchSongBrowser { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsSongGridView songGridView { get; set; }

		[Outlet]
		MonoMac.AppKit.NSSplitView splitMain { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTableView tableLoops { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTableView tableMarkers { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTableView tableSegments { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsTrackBarView trackBarMarkerPosition { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsTrackBarView trackBarPitchShifting { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsTrackBarView trackBarPosition { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsTrackBarView trackBarSegmentPosition { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsTrackBarView trackBarTimeShifting { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtCurrentTempoValue { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtIntervalValue { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtLoopName { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtMarkerName { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewActions { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewInformation { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewLeft { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewLeftHeader { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewLibraryBrowser { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewLoopDetails { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewLoopDetailsHeader { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewLoopPlayback { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewLoopPlaybackHeader { get; set; }

		[Outlet]
		MonoMac.AppKit.NSView viewLoops { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewLoopsHeader { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewMarkerDetails { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewMarkerDetailsHeader { get; set; }

		[Outlet]
		MonoMac.AppKit.NSView viewMarkers { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewMarkersHeader { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewNowPlaying { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewPitchShifting { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewQueue { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewQueueHeader { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewRight { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewRightHeader { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewSegmentDetails { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewSegmentDetailsHeader { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewSegmentsHeader { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewSongBrowserHeader { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewSongPosition { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewTimeShifting { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewToolbar { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewUpdateLibrary { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewUpdateLibraryHeader { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsView viewVolume { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsWaveFormScrollView waveFormScrollView { get; set; }

		[Action ("actionAddFilesToLibrary:")]
		partial void actionAddFilesToLibrary (MonoMac.Foundation.NSObject sender);

		[Action ("actionAddFolderLibrary:")]
		partial void actionAddFolderLibrary (MonoMac.Foundation.NSObject sender);

		[Action ("actionAddLoop:")]
		partial void actionAddLoop (MonoMac.Foundation.NSObject sender);

		[Action ("actionAddMarker:")]
		partial void actionAddMarker (MonoMac.Foundation.NSObject sender);

		[Action ("actionAddSegment:")]
		partial void actionAddSegment (MonoMac.Foundation.NSObject sender);

		[Action ("actionAddToPlaylist:")]
		partial void actionAddToPlaylist (MonoMac.Foundation.NSObject sender);

		[Action ("actionAddToQueue:")]
		partial void actionAddToQueue (MonoMac.Foundation.NSObject sender);

		[Action ("actionBackLoopDetails:")]
		partial void actionBackLoopDetails (MonoMac.Foundation.NSObject sender);

		[Action ("actionBackLoopPlayback:")]
		partial void actionBackLoopPlayback (MonoMac.Foundation.NSObject sender);

		[Action ("actionBackMarkerDetails:")]
		partial void actionBackMarkerDetails (MonoMac.Foundation.NSObject sender);

		[Action ("actionBackSegmentDetails:")]
		partial void actionBackSegmentDetails (MonoMac.Foundation.NSObject sender);

		[Action ("actionChangeKey:")]
		partial void actionChangeKey (MonoMac.Foundation.NSObject sender);

		[Action ("actionContextualMenuPlay:")]
		partial void actionContextualMenuPlay (MonoMac.Foundation.NSObject sender);

		[Action ("actionDecrementPitchShifting:")]
		partial void actionDecrementPitchShifting (MonoMac.Foundation.NSObject sender);

		[Action ("actionDecrementTimeShifting:")]
		partial void actionDecrementTimeShifting (MonoMac.Foundation.NSObject sender);

		[Action ("actionDeleteFromHardDisk:")]
		partial void actionDeleteFromHardDisk (MonoMac.Foundation.NSObject sender);

		[Action ("actionDeleteQueue:")]
		partial void actionDeleteQueue (MonoMac.Foundation.NSObject sender);

		[Action ("actionEditLoop:")]
		partial void actionEditLoop (MonoMac.Foundation.NSObject sender);

		[Action ("actionEditMarker:")]
		partial void actionEditMarker (MonoMac.Foundation.NSObject sender);

		[Action ("actionEditSegment:")]
		partial void actionEditSegment (MonoMac.Foundation.NSObject sender);

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

		[Action ("actionNextLoop:")]
		partial void actionNextLoop (MonoMac.Foundation.NSObject sender);

		[Action ("actionNextSegment:")]
		partial void actionNextSegment (MonoMac.Foundation.NSObject sender);

		[Action ("actionOpenAudioFiles:")]
		partial void actionOpenAudioFiles (MonoMac.Foundation.NSObject sender);

		[Action ("actionOpenEffectsWindow:")]
		partial void actionOpenEffectsWindow (MonoMac.Foundation.NSObject sender);

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

		[Action ("actionPlayQueue:")]
		partial void actionPlayQueue (MonoMac.Foundation.NSObject sender);

		[Action ("actionPrevious:")]
		partial void actionPrevious (MonoMac.Foundation.NSObject sender);

		[Action ("actionPreviousLoop:")]
		partial void actionPreviousLoop (MonoMac.Foundation.NSObject sender);

		[Action ("actionPreviousSegment:")]
		partial void actionPreviousSegment (MonoMac.Foundation.NSObject sender);

		[Action ("actionPunchInMarker:")]
		partial void actionPunchInMarker (MonoMac.Foundation.NSObject sender);

		[Action ("actionPunchInSegment:")]
		partial void actionPunchInSegment (MonoMac.Foundation.NSObject sender);

		[Action ("actionRemoveFromLibrary:")]
		partial void actionRemoveFromLibrary (MonoMac.Foundation.NSObject sender);

		[Action ("actionRemoveLoop:")]
		partial void actionRemoveLoop (MonoMac.Foundation.NSObject sender);

		[Action ("actionRemoveMarker:")]
		partial void actionRemoveMarker (MonoMac.Foundation.NSObject sender);

		[Action ("actionRemoveSegment:")]
		partial void actionRemoveSegment (MonoMac.Foundation.NSObject sender);

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

		[Action ("actionSegmentMarker:")]
		partial void actionSegmentMarker (MonoMac.Foundation.NSObject sender);

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

		[Action ("actionUseTempo:")]
		partial void actionUseTempo (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnAddLoop != null) {
				btnAddLoop.Dispose ();
				btnAddLoop = null;
			}

			if (btnAddMarker != null) {
				btnAddMarker.Dispose ();
				btnAddMarker = null;
			}

			if (btnAddSegment != null) {
				btnAddSegment.Dispose ();
				btnAddSegment = null;
			}

			if (btnBackLoop != null) {
				btnBackLoop.Dispose ();
				btnBackLoop = null;
			}

			if (btnBackLoopPlayback != null) {
				btnBackLoopPlayback.Dispose ();
				btnBackLoopPlayback = null;
			}

			if (btnBackMarker != null) {
				btnBackMarker.Dispose ();
				btnBackMarker = null;
			}

			if (btnBackSegment != null) {
				btnBackSegment.Dispose ();
				btnBackSegment = null;
			}

			if (btnChangeKey != null) {
				btnChangeKey.Dispose ();
				btnChangeKey = null;
			}

			if (btnDecrementPitchShifting != null) {
				btnDecrementPitchShifting.Dispose ();
				btnDecrementPitchShifting = null;
			}

			if (btnDecrementTimeShifting != null) {
				btnDecrementTimeShifting.Dispose ();
				btnDecrementTimeShifting = null;
			}

			if (btnDeleteQueue != null) {
				btnDeleteQueue.Dispose ();
				btnDeleteQueue = null;
			}

			if (btnDetectTempo != null) {
				btnDetectTempo.Dispose ();
				btnDetectTempo = null;
			}

			if (btnEditLoop != null) {
				btnEditLoop.Dispose ();
				btnEditLoop = null;
			}

			if (btnEditMarker != null) {
				btnEditMarker.Dispose ();
				btnEditMarker = null;
			}

			if (btnEditSegment != null) {
				btnEditSegment.Dispose ();
				btnEditSegment = null;
			}

			if (btnEditSongMetadata != null) {
				btnEditSongMetadata.Dispose ();
				btnEditSongMetadata = null;
			}

			if (btnGoToMarker != null) {
				btnGoToMarker.Dispose ();
				btnGoToMarker = null;
			}

			if (btnIncrementPitchShifting != null) {
				btnIncrementPitchShifting.Dispose ();
				btnIncrementPitchShifting = null;
			}

			if (btnIncrementTimeShifting != null) {
				btnIncrementTimeShifting.Dispose ();
				btnIncrementTimeShifting = null;
			}

			if (btnNextLoop != null) {
				btnNextLoop.Dispose ();
				btnNextLoop = null;
			}

			if (btnNextSegment != null) {
				btnNextSegment.Dispose ();
				btnNextSegment = null;
			}

			if (btnPlayLoop != null) {
				btnPlayLoop.Dispose ();
				btnPlayLoop = null;
			}

			if (btnPlayQueue != null) {
				btnPlayQueue.Dispose ();
				btnPlayQueue = null;
			}

			if (btnPreviousLoop != null) {
				btnPreviousLoop.Dispose ();
				btnPreviousLoop = null;
			}

			if (btnPreviousSegment != null) {
				btnPreviousSegment.Dispose ();
				btnPreviousSegment = null;
			}

			if (btnPunchInMarker != null) {
				btnPunchInMarker.Dispose ();
				btnPunchInMarker = null;
			}

			if (btnPunchInSegment != null) {
				btnPunchInSegment.Dispose ();
				btnPunchInSegment = null;
			}

			if (btnRemoveLoop != null) {
				btnRemoveLoop.Dispose ();
				btnRemoveLoop = null;
			}

			if (btnRemoveMarker != null) {
				btnRemoveMarker.Dispose ();
				btnRemoveMarker = null;
			}

			if (btnRemoveSegment != null) {
				btnRemoveSegment.Dispose ();
				btnRemoveSegment = null;
			}

			if (btnResetPitchShifting != null) {
				btnResetPitchShifting.Dispose ();
				btnResetPitchShifting = null;
			}

			if (btnResetTimeShifting != null) {
				btnResetTimeShifting.Dispose ();
				btnResetTimeShifting = null;
			}

			if (btnTabActions != null) {
				btnTabActions.Dispose ();
				btnTabActions = null;
			}

			if (btnTabInfo != null) {
				btnTabInfo.Dispose ();
				btnTabInfo = null;
			}

			if (btnTabPitchShifting != null) {
				btnTabPitchShifting.Dispose ();
				btnTabPitchShifting = null;
			}

			if (btnTabTimeShifting != null) {
				btnTabTimeShifting.Dispose ();
				btnTabTimeShifting = null;
			}

			if (btnToolbarEffects != null) {
				btnToolbarEffects.Dispose ();
				btnToolbarEffects = null;
			}

			if (btnToolbarNext != null) {
				btnToolbarNext.Dispose ();
				btnToolbarNext = null;
			}

			if (btnToolbarPlaylist != null) {
				btnToolbarPlaylist.Dispose ();
				btnToolbarPlaylist = null;
			}

			if (btnToolbarPlayPause != null) {
				btnToolbarPlayPause.Dispose ();
				btnToolbarPlayPause = null;
			}

			if (btnToolbarPrevious != null) {
				btnToolbarPrevious.Dispose ();
				btnToolbarPrevious = null;
			}

			if (btnToolbarRepeat != null) {
				btnToolbarRepeat.Dispose ();
				btnToolbarRepeat = null;
			}

			if (btnToolbarSettings != null) {
				btnToolbarSettings.Dispose ();
				btnToolbarSettings = null;
			}

			if (btnToolbarShuffle != null) {
				btnToolbarShuffle.Dispose ();
				btnToolbarShuffle = null;
			}

			if (btnToolbarSync != null) {
				btnToolbarSync.Dispose ();
				btnToolbarSync = null;
			}

			if (btnUseTempo != null) {
				btnUseTempo.Dispose ();
				btnUseTempo = null;
			}

			if (cboSoundFormat != null) {
				cboSoundFormat.Dispose ();
				cboSoundFormat = null;
			}

			if (chkSegmentLinkToMarker != null) {
				chkSegmentLinkToMarker.Dispose ();
				chkSegmentLinkToMarker = null;
			}

			if (comboSegmentMarker != null) {
				comboSegmentMarker.Dispose ();
				comboSegmentMarker = null;
			}

			if (faderVolume != null) {
				faderVolume.Dispose ();
				faderVolume = null;
			}

			if (imageAlbumCover != null) {
				imageAlbumCover.Dispose ();
				imageAlbumCover = null;
			}

			if (lblAlbumTitle != null) {
				lblAlbumTitle.Dispose ();
				lblAlbumTitle = null;
			}

			if (lblArtistName != null) {
				lblArtistName.Dispose ();
				lblArtistName = null;
			}

			if (lblBitrate != null) {
				lblBitrate.Dispose ();
				lblBitrate = null;
			}

			if (lblBitsPerSample != null) {
				lblBitsPerSample.Dispose ();
				lblBitsPerSample = null;
			}

			if (lblCurrentLoop != null) {
				lblCurrentLoop.Dispose ();
				lblCurrentLoop = null;
			}

			if (lblCurrentSegment != null) {
				lblCurrentSegment.Dispose ();
				lblCurrentSegment = null;
			}

			if (lblCurrentTempo != null) {
				lblCurrentTempo.Dispose ();
				lblCurrentTempo = null;
			}

			if (lblDetectedTempo != null) {
				lblDetectedTempo.Dispose ();
				lblDetectedTempo = null;
			}

			if (lblDetectedTempoValue != null) {
				lblDetectedTempoValue.Dispose ();
				lblDetectedTempoValue = null;
			}

			if (lblFileSize != null) {
				lblFileSize.Dispose ();
				lblFileSize = null;
			}

			if (lblFileType != null) {
				lblFileType.Dispose ();
				lblFileType = null;
			}

			if (lblFilterBySoundFormat != null) {
				lblFilterBySoundFormat.Dispose ();
				lblFilterBySoundFormat = null;
			}

			if (lblGenre != null) {
				lblGenre.Dispose ();
				lblGenre = null;
			}

			if (lblInterval != null) {
				lblInterval.Dispose ();
				lblInterval = null;
			}

			if (lblLastPlayed != null) {
				lblLastPlayed.Dispose ();
				lblLastPlayed = null;
			}

			if (lblLength != null) {
				lblLength.Dispose ();
				lblLength = null;
			}

			if (lblLoopName != null) {
				lblLoopName.Dispose ();
				lblLoopName = null;
			}

			if (lblLoopPosition != null) {
				lblLoopPosition.Dispose ();
				lblLoopPosition = null;
			}

			if (lblMarkerName != null) {
				lblMarkerName.Dispose ();
				lblMarkerName = null;
			}

			if (lblMarkerPosition != null) {
				lblMarkerPosition.Dispose ();
				lblMarkerPosition = null;
			}

			if (lblMarkerPositionValue != null) {
				lblMarkerPositionValue.Dispose ();
				lblMarkerPositionValue = null;
			}

			if (lblMonoStereo != null) {
				lblMonoStereo.Dispose ();
				lblMonoStereo = null;
			}

			if (lblNewKey != null) {
				lblNewKey.Dispose ();
				lblNewKey = null;
			}

			if (lblNewKeyValue != null) {
				lblNewKeyValue.Dispose ();
				lblNewKeyValue = null;
			}

			if (lblPlayCount != null) {
				lblPlayCount.Dispose ();
				lblPlayCount = null;
			}

			if (lblPosition != null) {
				lblPosition.Dispose ();
				lblPosition = null;
			}

			if (lblQueueDetails != null) {
				lblQueueDetails.Dispose ();
				lblQueueDetails = null;
			}

			if (lblReferenceKey != null) {
				lblReferenceKey.Dispose ();
				lblReferenceKey = null;
			}

			if (lblReferenceKeyValue != null) {
				lblReferenceKeyValue.Dispose ();
				lblReferenceKeyValue = null;
			}

			if (lblReferenceTempo != null) {
				lblReferenceTempo.Dispose ();
				lblReferenceTempo = null;
			}

			if (lblReferenceTempoValue != null) {
				lblReferenceTempoValue.Dispose ();
				lblReferenceTempoValue = null;
			}

			if (lblSampleRate != null) {
				lblSampleRate.Dispose ();
				lblSampleRate = null;
			}

			if (lblSearchWeb != null) {
				lblSearchWeb.Dispose ();
				lblSearchWeb = null;
			}

			if (lblSegmentLinkToMarker != null) {
				lblSegmentLinkToMarker.Dispose ();
				lblSegmentLinkToMarker = null;
			}

			if (lblSegmentPosition != null) {
				lblSegmentPosition.Dispose ();
				lblSegmentPosition = null;
			}

			if (lblSegmentPositionValue != null) {
				lblSegmentPositionValue.Dispose ();
				lblSegmentPositionValue = null;
			}

			if (lblSongPath != null) {
				lblSongPath.Dispose ();
				lblSongPath = null;
			}

			if (lblSongTitle != null) {
				lblSongTitle.Dispose ();
				lblSongTitle = null;
			}

			if (lblSubtitleSongPosition != null) {
				lblSubtitleSongPosition.Dispose ();
				lblSubtitleSongPosition = null;
			}

			if (lblSubtitleVolume != null) {
				lblSubtitleVolume.Dispose ();
				lblSubtitleVolume = null;
			}

			if (lblTitleCurrentSong != null) {
				lblTitleCurrentSong.Dispose ();
				lblTitleCurrentSong = null;
			}

			if (lblTitleLibraryBrowser != null) {
				lblTitleLibraryBrowser.Dispose ();
				lblTitleLibraryBrowser = null;
			}

			if (lblTitleLoopDetails != null) {
				lblTitleLoopDetails.Dispose ();
				lblTitleLoopDetails = null;
			}

			if (lblTitleLoopPlayback != null) {
				lblTitleLoopPlayback.Dispose ();
				lblTitleLoopPlayback = null;
			}

			if (lblTitleLoops != null) {
				lblTitleLoops.Dispose ();
				lblTitleLoops = null;
			}

			if (lblTitleMarkerDetails != null) {
				lblTitleMarkerDetails.Dispose ();
				lblTitleMarkerDetails = null;
			}

			if (lblTitleMarkers != null) {
				lblTitleMarkers.Dispose ();
				lblTitleMarkers = null;
			}

			if (lblTitleQueue != null) {
				lblTitleQueue.Dispose ();
				lblTitleQueue = null;
			}

			if (lblTitleSegmentDetails != null) {
				lblTitleSegmentDetails.Dispose ();
				lblTitleSegmentDetails = null;
			}

			if (lblTitleSegments != null) {
				lblTitleSegments.Dispose ();
				lblTitleSegments = null;
			}

			if (lblTitleSongBrowser != null) {
				lblTitleSongBrowser.Dispose ();
				lblTitleSongBrowser = null;
			}

			if (lblTitleUpdateLibrary != null) {
				lblTitleUpdateLibrary.Dispose ();
				lblTitleUpdateLibrary = null;
			}

			if (lblUpdateLibraryStatus != null) {
				lblUpdateLibraryStatus.Dispose ();
				lblUpdateLibraryStatus = null;
			}

			if (lblVolume != null) {
				lblVolume.Dispose ();
				lblVolume = null;
			}

			if (lblYear != null) {
				lblYear.Dispose ();
				lblYear = null;
			}

			if (menuAddToPlaylist != null) {
				menuAddToPlaylist.Dispose ();
				menuAddToPlaylist = null;
			}

			if (menuAddToQueue != null) {
				menuAddToQueue.Dispose ();
				menuAddToQueue = null;
			}

			if (menuDeleteFromHardDisk != null) {
				menuDeleteFromHardDisk.Dispose ();
				menuDeleteFromHardDisk = null;
			}

			if (menuPlay != null) {
				menuPlay.Dispose ();
				menuPlay = null;
			}

			if (menuRemoveFromLibrary != null) {
				menuRemoveFromLibrary.Dispose ();
				menuRemoveFromLibrary = null;
			}

			if (outlineLibraryBrowser != null) {
				outlineLibraryBrowser.Dispose ();
				outlineLibraryBrowser = null;
			}

			if (outputMeter != null) {
				outputMeter.Dispose ();
				outputMeter = null;
			}

			if (progressBarUpdateLibrary != null) {
				progressBarUpdateLibrary.Dispose ();
				progressBarUpdateLibrary = null;
			}

			if (scrollViewLibraryBrowser != null) {
				scrollViewLibraryBrowser.Dispose ();
				scrollViewLibraryBrowser = null;
			}

			if (searchSongBrowser != null) {
				searchSongBrowser.Dispose ();
				searchSongBrowser = null;
			}

			if (songGridView != null) {
				songGridView.Dispose ();
				songGridView = null;
			}

			if (splitMain != null) {
				splitMain.Dispose ();
				splitMain = null;
			}

			if (tableLoops != null) {
				tableLoops.Dispose ();
				tableLoops = null;
			}

			if (tableMarkers != null) {
				tableMarkers.Dispose ();
				tableMarkers = null;
			}

			if (tableSegments != null) {
				tableSegments.Dispose ();
				tableSegments = null;
			}

			if (trackBarMarkerPosition != null) {
				trackBarMarkerPosition.Dispose ();
				trackBarMarkerPosition = null;
			}

			if (trackBarPitchShifting != null) {
				trackBarPitchShifting.Dispose ();
				trackBarPitchShifting = null;
			}

			if (trackBarPosition != null) {
				trackBarPosition.Dispose ();
				trackBarPosition = null;
			}

			if (trackBarSegmentPosition != null) {
				trackBarSegmentPosition.Dispose ();
				trackBarSegmentPosition = null;
			}

			if (trackBarTimeShifting != null) {
				trackBarTimeShifting.Dispose ();
				trackBarTimeShifting = null;
			}

			if (txtCurrentTempoValue != null) {
				txtCurrentTempoValue.Dispose ();
				txtCurrentTempoValue = null;
			}

			if (txtIntervalValue != null) {
				txtIntervalValue.Dispose ();
				txtIntervalValue = null;
			}

			if (txtLoopName != null) {
				txtLoopName.Dispose ();
				txtLoopName = null;
			}

			if (txtMarkerName != null) {
				txtMarkerName.Dispose ();
				txtMarkerName = null;
			}

			if (viewActions != null) {
				viewActions.Dispose ();
				viewActions = null;
			}

			if (viewInformation != null) {
				viewInformation.Dispose ();
				viewInformation = null;
			}

			if (viewLeft != null) {
				viewLeft.Dispose ();
				viewLeft = null;
			}

			if (viewLeftHeader != null) {
				viewLeftHeader.Dispose ();
				viewLeftHeader = null;
			}

			if (viewLibraryBrowser != null) {
				viewLibraryBrowser.Dispose ();
				viewLibraryBrowser = null;
			}

			if (viewLoopDetails != null) {
				viewLoopDetails.Dispose ();
				viewLoopDetails = null;
			}

			if (viewLoopDetailsHeader != null) {
				viewLoopDetailsHeader.Dispose ();
				viewLoopDetailsHeader = null;
			}

			if (viewLoopPlayback != null) {
				viewLoopPlayback.Dispose ();
				viewLoopPlayback = null;
			}

			if (viewLoopPlaybackHeader != null) {
				viewLoopPlaybackHeader.Dispose ();
				viewLoopPlaybackHeader = null;
			}

			if (viewLoops != null) {
				viewLoops.Dispose ();
				viewLoops = null;
			}

			if (viewLoopsHeader != null) {
				viewLoopsHeader.Dispose ();
				viewLoopsHeader = null;
			}

			if (viewMarkerDetails != null) {
				viewMarkerDetails.Dispose ();
				viewMarkerDetails = null;
			}

			if (viewMarkerDetailsHeader != null) {
				viewMarkerDetailsHeader.Dispose ();
				viewMarkerDetailsHeader = null;
			}

			if (viewMarkers != null) {
				viewMarkers.Dispose ();
				viewMarkers = null;
			}

			if (viewMarkersHeader != null) {
				viewMarkersHeader.Dispose ();
				viewMarkersHeader = null;
			}

			if (viewNowPlaying != null) {
				viewNowPlaying.Dispose ();
				viewNowPlaying = null;
			}

			if (viewPitchShifting != null) {
				viewPitchShifting.Dispose ();
				viewPitchShifting = null;
			}

			if (viewQueue != null) {
				viewQueue.Dispose ();
				viewQueue = null;
			}

			if (viewQueueHeader != null) {
				viewQueueHeader.Dispose ();
				viewQueueHeader = null;
			}

			if (viewRight != null) {
				viewRight.Dispose ();
				viewRight = null;
			}

			if (viewRightHeader != null) {
				viewRightHeader.Dispose ();
				viewRightHeader = null;
			}

			if (viewSegmentDetails != null) {
				viewSegmentDetails.Dispose ();
				viewSegmentDetails = null;
			}

			if (viewSegmentDetailsHeader != null) {
				viewSegmentDetailsHeader.Dispose ();
				viewSegmentDetailsHeader = null;
			}

			if (viewSegmentsHeader != null) {
				viewSegmentsHeader.Dispose ();
				viewSegmentsHeader = null;
			}

			if (viewSongBrowserHeader != null) {
				viewSongBrowserHeader.Dispose ();
				viewSongBrowserHeader = null;
			}

			if (viewSongPosition != null) {
				viewSongPosition.Dispose ();
				viewSongPosition = null;
			}

			if (viewTimeShifting != null) {
				viewTimeShifting.Dispose ();
				viewTimeShifting = null;
			}

			if (viewToolbar != null) {
				viewToolbar.Dispose ();
				viewToolbar = null;
			}

			if (viewUpdateLibrary != null) {
				viewUpdateLibrary.Dispose ();
				viewUpdateLibrary = null;
			}

			if (viewUpdateLibraryHeader != null) {
				viewUpdateLibraryHeader.Dispose ();
				viewUpdateLibraryHeader = null;
			}

			if (viewVolume != null) {
				viewVolume.Dispose ();
				viewVolume = null;
			}

			if (waveFormScrollView != null) {
				waveFormScrollView.Dispose ();
				waveFormScrollView = null;
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
