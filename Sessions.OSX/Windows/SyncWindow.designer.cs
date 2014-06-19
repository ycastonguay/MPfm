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
	[Register ("SyncWindowController")]
	partial class SyncWindowController
	{
		[Outlet]
        Sessions.OSX.Classes.Controls.SessionsButton btnAddDevice { get; set; }

		[Outlet]
        Sessions.OSX.Classes.Controls.SessionsRoundButton btnNext { get; set; }

		[Outlet]
        Sessions.OSX.Classes.Controls.SessionsRoundButton btnPlayPause { get; set; }

		[Outlet]
        Sessions.OSX.Classes.Controls.SessionsRoundButton btnPrevious { get; set; }

		[Outlet]
        Sessions.OSX.Classes.Controls.SessionsRoundButton btnRepeat { get; set; }

		[Outlet]
        Sessions.OSX.Classes.Controls.SessionsButton btnResumePlayback { get; set; }

		[Outlet]
        Sessions.OSX.Classes.Controls.SessionsRoundButton btnShuffle { get; set; }

		[Outlet]
        Sessions.OSX.Classes.Controls.SessionsButton btnSyncLibrary { get; set; }

		[Outlet]
		MonoMac.AppKit.NSImageView imageViewAlbum { get; set; }

		[Outlet]
		MonoMac.AppKit.NSImageView imageViewDeviceType { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblAddDevice { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblAddDevicePort { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblAddDeviceUrl { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblAlbumTitle { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblArtistName { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblDeviceDetails { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblDeviceName { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblDeviceUrl { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblLastUpdated { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblLibraryUrl { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblPlayerStatus { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblPlaylist { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblPosition { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblRemotePlayer { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSongTitle { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblStatus { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSubtitle { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitle { get; set; }

		[Outlet]
		MonoMac.AppKit.NSMenuItem menuItemRemoveDeviceFromList { get; set; }

		[Outlet]
		MonoMac.AppKit.NSProgressIndicator progressIndicator { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTableView tableViewDevices { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtAddDevicePort { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField txtAddDeviceUrl { get; set; }

		[Outlet]
        Sessions.OSX.Classes.Controls.SessionsView viewConnectManualHeader { get; set; }

		[Outlet]
        Sessions.OSX.Classes.Controls.SessionsView viewDetails { get; set; }

		[Outlet]
        Sessions.OSX.Classes.Controls.SessionsView viewDeviceDetailsHeader { get; set; }

		[Outlet]
        Sessions.OSX.Classes.Controls.SessionsView viewRemotePlayerHeader { get; set; }

		[Outlet]
        Sessions.OSX.Classes.Controls.SessionsView viewSubtitleHeader { get; set; }

		[Outlet]
        Sessions.OSX.Classes.Controls.SessionsView viewTitleHeader { get; set; }

		[Action ("actionRemoveDeviceFromList:")]
		partial void actionRemoveDeviceFromList (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnNext != null) {
				btnNext.Dispose ();
				btnNext = null;
			}

			if (btnPlayPause != null) {
				btnPlayPause.Dispose ();
				btnPlayPause = null;
			}

			if (btnPrevious != null) {
				btnPrevious.Dispose ();
				btnPrevious = null;
			}

			if (btnRepeat != null) {
				btnRepeat.Dispose ();
				btnRepeat = null;
			}

			if (btnResumePlayback != null) {
				btnResumePlayback.Dispose ();
				btnResumePlayback = null;
			}

			if (btnShuffle != null) {
				btnShuffle.Dispose ();
				btnShuffle = null;
			}

			if (btnSyncLibrary != null) {
				btnSyncLibrary.Dispose ();
				btnSyncLibrary = null;
			}

			if (imageViewAlbum != null) {
				imageViewAlbum.Dispose ();
				imageViewAlbum = null;
			}

			if (imageViewDeviceType != null) {
				imageViewDeviceType.Dispose ();
				imageViewDeviceType = null;
			}

			if (lblAlbumTitle != null) {
				lblAlbumTitle.Dispose ();
				lblAlbumTitle = null;
			}

			if (lblArtistName != null) {
				lblArtistName.Dispose ();
				lblArtistName = null;
			}

			if (lblAddDevice != null) {
				lblAddDevice.Dispose ();
				lblAddDevice = null;
			}

			if (lblAddDevicePort != null) {
				lblAddDevicePort.Dispose ();
				lblAddDevicePort = null;
			}

			if (lblAddDeviceUrl != null) {
				lblAddDeviceUrl.Dispose ();
				lblAddDeviceUrl = null;
			}

			if (lblDeviceDetails != null) {
				lblDeviceDetails.Dispose ();
				lblDeviceDetails = null;
			}

			if (lblDeviceName != null) {
				lblDeviceName.Dispose ();
				lblDeviceName = null;
			}

			if (lblDeviceUrl != null) {
				lblDeviceUrl.Dispose ();
				lblDeviceUrl = null;
			}

			if (lblLastUpdated != null) {
				lblLastUpdated.Dispose ();
				lblLastUpdated = null;
			}

			if (lblLibraryUrl != null) {
				lblLibraryUrl.Dispose ();
				lblLibraryUrl = null;
			}

			if (lblPlayerStatus != null) {
				lblPlayerStatus.Dispose ();
				lblPlayerStatus = null;
			}

			if (lblPlaylist != null) {
				lblPlaylist.Dispose ();
				lblPlaylist = null;
			}

			if (lblPosition != null) {
				lblPosition.Dispose ();
				lblPosition = null;
			}

			if (lblRemotePlayer != null) {
				lblRemotePlayer.Dispose ();
				lblRemotePlayer = null;
			}

			if (lblSongTitle != null) {
				lblSongTitle.Dispose ();
				lblSongTitle = null;
			}

			if (lblStatus != null) {
				lblStatus.Dispose ();
				lblStatus = null;
			}

			if (lblSubtitle != null) {
				lblSubtitle.Dispose ();
				lblSubtitle = null;
			}

			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}

			if (menuItemRemoveDeviceFromList != null) {
				menuItemRemoveDeviceFromList.Dispose ();
				menuItemRemoveDeviceFromList = null;
			}

			if (progressIndicator != null) {
				progressIndicator.Dispose ();
				progressIndicator = null;
			}

			if (tableViewDevices != null) {
				tableViewDevices.Dispose ();
				tableViewDevices = null;
			}

			if (txtAddDevicePort != null) {
				txtAddDevicePort.Dispose ();
				txtAddDevicePort = null;
			}

			if (txtAddDeviceUrl != null) {
				txtAddDeviceUrl.Dispose ();
				txtAddDeviceUrl = null;
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

			if (viewRemotePlayerHeader != null) {
				viewRemotePlayerHeader.Dispose ();
				viewRemotePlayerHeader = null;
			}

			if (viewSubtitleHeader != null) {
				viewSubtitleHeader.Dispose ();
				viewSubtitleHeader = null;
			}

			if (viewTitleHeader != null) {
				viewTitleHeader.Dispose ();
				viewTitleHeader = null;
			}

			if (btnAddDevice != null) {
				btnAddDevice.Dispose ();
				btnAddDevice = null;
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
