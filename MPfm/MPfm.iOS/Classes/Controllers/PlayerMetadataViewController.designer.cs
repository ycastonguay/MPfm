// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace MPfm.iOS
{
	[Register ("PlayerMetadataViewController")]
	partial class PlayerMetadataViewController
	{
		[Outlet]
		MonoTouch.UIKit.UILabel lblArtistName { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblAlbumTitle { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblTitle { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewBackground { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnPlaylist { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnShuffle { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnRepeat { get; set; }

		[Action ("actionRepeat:")]
		partial void actionRepeat (MonoTouch.Foundation.NSObject sender);

		[Action ("actionShuffle:")]
		partial void actionShuffle (MonoTouch.Foundation.NSObject sender);

		[Action ("actionPlaylist:")]
		partial void actionPlaylist (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (lblArtistName != null) {
				lblArtistName.Dispose ();
				lblArtistName = null;
			}

			if (lblAlbumTitle != null) {
				lblAlbumTitle.Dispose ();
				lblAlbumTitle = null;
			}

			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}

			if (viewBackground != null) {
				viewBackground.Dispose ();
				viewBackground = null;
			}

			if (btnPlaylist != null) {
				btnPlaylist.Dispose ();
				btnPlaylist = null;
			}

			if (btnShuffle != null) {
				btnShuffle.Dispose ();
				btnShuffle = null;
			}

			if (btnRepeat != null) {
				btnRepeat.Dispose ();
				btnRepeat = null;
			}
		}
	}
}
