// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Sessions.iOS.Classes.Controllers
{
	[Register ("PlayerMetadataViewController")]
	partial class PlayerMetadataViewController
	{
		[Outlet]
		UIKit.UILabel lblAlbumTitle { get; set; }

		[Outlet]
		UIKit.UILabel lblArtistName { get; set; }

		[Outlet]
		UIKit.UILabel lblSongCount { get; set; }

		[Outlet]
		UIKit.UILabel lblTitle { get; set; }

		[Outlet]
		UIKit.UIView viewBackground { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (lblAlbumTitle != null) {
				lblAlbumTitle.Dispose ();
				lblAlbumTitle = null;
			}

			if (lblArtistName != null) {
				lblArtistName.Dispose ();
				lblArtistName = null;
			}

			if (lblSongCount != null) {
				lblSongCount.Dispose ();
				lblSongCount = null;
			}

			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}

			if (viewBackground != null) {
				viewBackground.Dispose ();
				viewBackground = null;
			}
		}
	}
}
