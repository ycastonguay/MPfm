// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace MPfm.iOS.Classes.Controllers
{
	[Register ("MobileLibraryBrowserViewController")]
	partial class MobileLibraryBrowserViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIActivityIndicatorView activityIndicator { get; set; }

		[Outlet]
		MonoTouch.UIKit.UICollectionView collectionView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView imageViewAlbumCover { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblAlbumTitle { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblArtistName { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblSubtitle1 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblSubtitle2 { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView tableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewAlbumCover { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewLoading { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (collectionView != null) {
				collectionView.Dispose ();
				collectionView = null;
			}

			if (imageViewAlbumCover != null) {
				imageViewAlbumCover.Dispose ();
				imageViewAlbumCover = null;
			}

			if (lblAlbumTitle != null) {
				lblAlbumTitle.Dispose ();
				lblAlbumTitle = null;
			}

			if (lblArtistName != null) {
				lblArtistName.Dispose ();
				lblArtistName = null;
			}

			if (lblSubtitle1 != null) {
				lblSubtitle1.Dispose ();
				lblSubtitle1 = null;
			}

			if (lblSubtitle2 != null) {
				lblSubtitle2.Dispose ();
				lblSubtitle2 = null;
			}

			if (tableView != null) {
				tableView.Dispose ();
				tableView = null;
			}

			if (viewAlbumCover != null) {
				viewAlbumCover.Dispose ();
				viewAlbumCover = null;
			}

			if (viewLoading != null) {
				viewLoading.Dispose ();
				viewLoading = null;
			}

			if (activityIndicator != null) {
				activityIndicator.Dispose ();
				activityIndicator = null;
			}
		}
	}
}
