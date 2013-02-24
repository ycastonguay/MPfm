// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace MPfm.iOS.Classes.Controllers
{
	[Register ("MobileLibraryBrowserViewController")]
	partial class MobileLibraryBrowserViewController
	{
		[Outlet]
		MonoTouch.UIKit.UITableView tableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewAlbumCover { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView imageViewAlbumCover { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblArtistName { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblAlbumTitle { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblSubtitle1 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblSubtitle2 { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (tableView != null) {
				tableView.Dispose ();
				tableView = null;
			}

			if (viewAlbumCover != null) {
				viewAlbumCover.Dispose ();
				viewAlbumCover = null;
			}

			if (imageViewAlbumCover != null) {
				imageViewAlbumCover.Dispose ();
				imageViewAlbumCover = null;
			}

			if (lblArtistName != null) {
				lblArtistName.Dispose ();
				lblArtistName = null;
			}

			if (lblAlbumTitle != null) {
				lblAlbumTitle.Dispose ();
				lblAlbumTitle = null;
			}

			if (lblSubtitle1 != null) {
				lblSubtitle1.Dispose ();
				lblSubtitle1 = null;
			}

			if (lblSubtitle2 != null) {
				lblSubtitle2.Dispose ();
				lblSubtitle2 = null;
			}
		}
	}
}
