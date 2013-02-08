// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace MPfm.iOS
{
	[Register ("PlayerViewController")]
	partial class PlayerViewController
	{
		[Outlet]
		MonoTouch.UIKit.UILabel lblPosition { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblArtistName { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblAlbumTitle { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblTitle { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView imageViewAlbumArt { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISlider sliderPosition { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblLength { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnPrevious { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnPlayPause { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnNext { get; set; }

		[Action ("actionPause:")]
		partial void actionPause (MonoTouch.Foundation.NSObject sender);

		[Action ("actionPrevious:")]
		partial void actionPrevious (MonoTouch.Foundation.NSObject sender);

		[Action ("actionNext:")]
		partial void actionNext (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (lblPosition != null) {
				lblPosition.Dispose ();
				lblPosition = null;
			}

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

			if (imageViewAlbumArt != null) {
				imageViewAlbumArt.Dispose ();
				imageViewAlbumArt = null;
			}

			if (sliderPosition != null) {
				sliderPosition.Dispose ();
				sliderPosition = null;
			}

			if (lblLength != null) {
				lblLength.Dispose ();
				lblLength = null;
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
		}
	}
}
