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
	[Register ("StartResumePlaybackWindowController")]
	partial class StartResumePlaybackWindowController
	{
		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmButton btnCancel { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmButton btnOK { get; set; }

		[Outlet]
		MonoMac.AppKit.NSImageView imageViewAlbum { get; set; }

		[Outlet]
		MonoMac.AppKit.NSImageView imageViewDevice { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblAlbumTitle { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblArtistName { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblDeviceName { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblLastUpdated { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblNote { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblPlaylistName { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblSongTitle { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitle { get; set; }

		[Outlet]
		MPfm.OSX.Classes.Controls.MPfmView viewHeader { get; set; }
		
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

			if (imageViewAlbum != null) {
				imageViewAlbum.Dispose ();
				imageViewAlbum = null;
			}

			if (imageViewDevice != null) {
				imageViewDevice.Dispose ();
				imageViewDevice = null;
			}

			if (lblAlbumTitle != null) {
				lblAlbumTitle.Dispose ();
				lblAlbumTitle = null;
			}

			if (lblArtistName != null) {
				lblArtistName.Dispose ();
				lblArtistName = null;
			}

			if (lblDeviceName != null) {
				lblDeviceName.Dispose ();
				lblDeviceName = null;
			}

			if (lblLastUpdated != null) {
				lblLastUpdated.Dispose ();
				lblLastUpdated = null;
			}

			if (lblNote != null) {
				lblNote.Dispose ();
				lblNote = null;
			}

			if (lblPlaylistName != null) {
				lblPlaylistName.Dispose ();
				lblPlaylistName = null;
			}

			if (lblSongTitle != null) {
				lblSongTitle.Dispose ();
				lblSongTitle = null;
			}

			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}

			if (viewHeader != null) {
				viewHeader.Dispose ();
				viewHeader = null;
			}
		}
	}

	[Register ("StartResumePlaybackWindow")]
	partial class StartResumePlaybackWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
