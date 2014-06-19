// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace Sessions.iOS
{
	[Register ("StartResumePlaybackViewController")]
	partial class StartResumePlaybackViewController
	{
		[Outlet]
		Sessions.iOS.Classes.Controls.SessionsButton btnCancel { get; set; }

		[Outlet]
		Sessions.iOS.Classes.Controls.SessionsButton btnResume { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView imageAlbum { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView imageIcon { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblAlbumTitle { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblArtistName { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblDeviceName { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblPlaylistName { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblSongTitle { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblTimestamp { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewPanel { get; set; }

		[Action ("actionCancel:")]
		partial void actionCancel (MonoTouch.Foundation.NSObject sender);

		[Action ("actionResume:")]
		partial void actionResume (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}

			if (btnResume != null) {
				btnResume.Dispose ();
				btnResume = null;
			}

			if (imageAlbum != null) {
				imageAlbum.Dispose ();
				imageAlbum = null;
			}

			if (imageIcon != null) {
				imageIcon.Dispose ();
				imageIcon = null;
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

			if (lblPlaylistName != null) {
				lblPlaylistName.Dispose ();
				lblPlaylistName = null;
			}

			if (lblSongTitle != null) {
				lblSongTitle.Dispose ();
				lblSongTitle = null;
			}

			if (lblTimestamp != null) {
				lblTimestamp.Dispose ();
				lblTimestamp = null;
			}

			if (viewPanel != null) {
				viewPanel.Dispose ();
				viewPanel = null;
			}
		}
	}
}
