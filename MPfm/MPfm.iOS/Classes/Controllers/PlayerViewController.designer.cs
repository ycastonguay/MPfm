// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace MPfm.iOS.Classes.Controllers
{
	[Register ("PlayerViewController")]
	partial class PlayerViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIView viewPageControls { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewMain { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblSlideMessage { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblScrubbingType { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewPosition { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblPosition { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView imageViewAlbumArt { get; set; }

		[Outlet]
		MPfm.iOS.Classes.Controls.MPfmSlider sliderPosition { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblLength { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnPrevious { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnPlayPause { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnNext { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView scrollView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIPageControl pageControl { get; set; }

		[Outlet]
		MPfm.iOS.Classes.Controls.MPfmWaveFormScrollView scrollViewWaveForm { get; set; }

		[Action ("actionPause:")]
		partial void actionPause (MonoTouch.Foundation.NSObject sender);

		[Action ("actionPrevious:")]
		partial void actionPrevious (MonoTouch.Foundation.NSObject sender);

		[Action ("actionNext:")]
		partial void actionNext (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (viewPageControls != null) {
				viewPageControls.Dispose ();
				viewPageControls = null;
			}

			if (viewMain != null) {
				viewMain.Dispose ();
				viewMain = null;
			}

			if (lblSlideMessage != null) {
				lblSlideMessage.Dispose ();
				lblSlideMessage = null;
			}

			if (lblScrubbingType != null) {
				lblScrubbingType.Dispose ();
				lblScrubbingType = null;
			}

			if (viewPosition != null) {
				viewPosition.Dispose ();
				viewPosition = null;
			}

			if (lblPosition != null) {
				lblPosition.Dispose ();
				lblPosition = null;
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

			if (scrollView != null) {
				scrollView.Dispose ();
				scrollView = null;
			}

			if (pageControl != null) {
				pageControl.Dispose ();
				pageControl = null;
			}

			if (scrollViewWaveForm != null) {
				scrollViewWaveForm.Dispose ();
				scrollViewWaveForm = null;
			}
		}
	}
}
