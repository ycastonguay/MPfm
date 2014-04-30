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
	[Register ("SyncWindowController")]
	partial class SyncWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSButton btnConnectManual { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmRoundButton btnNext { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmRoundButton btnPlayPause { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmRoundButton btnPrevious { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnRefreshDevices { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmRoundButton btnRepeat { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmButton btnResumePlayback { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmRoundButton btnShuffle { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmButton btnSyncLibrary { get; set; }

		[Outlet]
		MonoMac.AppKit.NSImageView imageViewAlbum { get; set; }

		[Outlet]
		MonoMac.AppKit.NSImageView imageViewDeviceType { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblAlbumTitle { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblArtistName { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblConnectManual { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblConnectManualPort { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblConnectManualUrl { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblDeviceDetails { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblDeviceName { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblDeviceUrl { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblLibraryUrl { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblPlayerStatus { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblPosition { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblRemotePlayer { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSongTitle { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSubtitle { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitle { get; set; }

		[Outlet]
		MonoMac.AppKit.NSProgressIndicator progressIndicator { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTableView tableViewDevices { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtConnectManualPort { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtConnectManualUrl { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewConnectManualHeader { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewDetails { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewDeviceDetailsHeader { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewRemotePlayerHeader { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewSubtitleHeader { get; set; }

		[Outlet]
		MPfm.Mac.Classes.Controls.MPfmView viewTitleHeader { get; set; }

		[Action ("actionConnectManual:")]
		partial void actionConnectManual (MonoMac.Foundation.NSObject sender);

		[Action ("actionRefreshDevices:")]
		partial void actionRefreshDevices (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnConnectManual != null) {
				btnConnectManual.Dispose ();
				btnConnectManual = null;
			}

			if (btnRefreshDevices != null) {
				btnRefreshDevices.Dispose ();
				btnRefreshDevices = null;
			}

			if (lblConnectManual != null) {
				lblConnectManual.Dispose ();
				lblConnectManual = null;
			}

			if (lblConnectManualPort != null) {
				lblConnectManualPort.Dispose ();
				lblConnectManualPort = null;
			}

			if (lblConnectManualUrl != null) {
				lblConnectManualUrl.Dispose ();
				lblConnectManualUrl = null;
			}

			if (lblDeviceDetails != null) {
				lblDeviceDetails.Dispose ();
				lblDeviceDetails = null;
			}

			if (lblLibraryUrl != null) {
				lblLibraryUrl.Dispose ();
				lblLibraryUrl = null;
			}

			if (lblSubtitle != null) {
				lblSubtitle.Dispose ();
				lblSubtitle = null;
			}

			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}

			if (progressIndicator != null) {
				progressIndicator.Dispose ();
				progressIndicator = null;
			}

			if (tableViewDevices != null) {
				tableViewDevices.Dispose ();
				tableViewDevices = null;
			}

			if (txtConnectManualPort != null) {
				txtConnectManualPort.Dispose ();
				txtConnectManualPort = null;
			}

			if (txtConnectManualUrl != null) {
				txtConnectManualUrl.Dispose ();
				txtConnectManualUrl = null;
			}

			if (viewConnectManualHeader != null) {
				viewConnectManualHeader.Dispose ();
				viewConnectManualHeader = null;
			}

			if (viewDetails != null) {
				viewDetails.Dispose ();
				viewDetails = null;
			}

			if (viewDeviceDetailsHeader != null) {
				viewDeviceDetailsHeader.Dispose ();
				viewDeviceDetailsHeader = null;
			}

			if (viewSubtitleHeader != null) {
				viewSubtitleHeader.Dispose ();
				viewSubtitleHeader = null;
			}

			if (viewTitleHeader != null) {
				viewTitleHeader.Dispose ();
				viewTitleHeader = null;
			}

			if (lblDeviceName != null) {
				lblDeviceName.Dispose ();
				lblDeviceName = null;
			}

			if (lblDeviceUrl != null) {
				lblDeviceUrl.Dispose ();
				lblDeviceUrl = null;
			}

			if (imageViewDeviceType != null) {
				imageViewDeviceType.Dispose ();
				imageViewDeviceType = null;
			}

			if (btnPrevious != null) {
				btnPrevious.Dispose ();
				btnPrevious = null;
			}

			if (btnPlayPause != null) {
				btnPlayPause.Dispose ();
				btnPlayPause = null;
			}

			if (btnNext != null) {
				btnNext.Dispose ();
				btnNext = null;
			}

			if (btnRepeat != null) {
				btnRepeat.Dispose ();
				btnRepeat = null;
			}

			if (btnShuffle != null) {
				btnShuffle.Dispose ();
				btnShuffle = null;
			}

			if (lblPlayerStatus != null) {
				lblPlayerStatus.Dispose ();
				lblPlayerStatus = null;
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

			if (imageViewAlbum != null) {
				imageViewAlbum.Dispose ();
				imageViewAlbum = null;
			}

			if (lblPosition != null) {
				lblPosition.Dispose ();
				lblPosition = null;
			}

			if (lblRemotePlayer != null) {
				lblRemotePlayer.Dispose ();
				lblRemotePlayer = null;
			}

			if (viewRemotePlayerHeader != null) {
				viewRemotePlayerHeader.Dispose ();
				viewRemotePlayerHeader = null;
			}

			if (btnSyncLibrary != null) {
				btnSyncLibrary.Dispose ();
				btnSyncLibrary = null;
			}

			if (btnResumePlayback != null) {
				btnResumePlayback.Dispose ();
				btnResumePlayback = null;
			}
		}
	}

	[Register ("SyncWindow")]
	partial class SyncWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
