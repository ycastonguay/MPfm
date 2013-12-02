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
	[Register ("PlayerViewController")]
	partial class PlayerViewController
	{
		[Outlet]
		MPfm.iOS.Classes.Controls.MPfmPlayerButton btnNext { get; set; }

		[Outlet]
		MPfm.iOS.Classes.Controls.MPfmPlayerButton btnPlayPause { get; set; }

		[Outlet]
		MPfm.iOS.Classes.Controls.MPfmPlayerButton btnPrevious { get; set; }

		[Outlet]
		MPfm.iOS.Classes.Controls.MPfmPlayerButton btnRepeat { get; set; }

		[Outlet]
		MPfm.iOS.Classes.Controls.MPfmPlayerButton btnShuffle { get; set; }

		[Outlet]
		MPfm.iOS.Classes.Controls.MPfmEqualizerPresetGraphView graphView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView imageViewAlbumArt { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView imageViewVolumeHigh { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIImageView imageViewVolumeLow { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblLength { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblPosition { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblPresetName { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblScrubbingType { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblSlideMessage { get; set; }

		[Outlet]
		MPfm.iOS.Classes.Controls.MPfmOutputMeterView outputMeter { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIPageControl pageControl { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIScrollView scrollView { get; set; }

		[Outlet]
		MPfm.iOS.Classes.Controls.MPfmWaveFormScrollView scrollViewWaveForm { get; set; }

		[Outlet]
		MPfm.iOS.Classes.Controls.MPfmSlider sliderPosition { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewEffects { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewMain { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewPageControls { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewPosition { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewVolume { get; set; }

		[Action ("actionNext:")]
		partial void actionNext (MonoTouch.Foundation.NSObject sender);

		[Action ("actionPause:")]
		partial void actionPause (MonoTouch.Foundation.NSObject sender);

		[Action ("actionPrevious:")]
		partial void actionPrevious (MonoTouch.Foundation.NSObject sender);

		[Action ("actionRepeat:")]
		partial void actionRepeat (MonoTouch.Foundation.NSObject sender);

		[Action ("actionShuffle:")]
		partial void actionShuffle (MonoTouch.Foundation.NSObject sender);
		
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

			if (btnShuffle != null) {
				btnShuffle.Dispose ();
				btnShuffle = null;
			}

			if (imageViewAlbumArt != null) {
				imageViewAlbumArt.Dispose ();
				imageViewAlbumArt = null;
			}

			if (imageViewVolumeHigh != null) {
				imageViewVolumeHigh.Dispose ();
				imageViewVolumeHigh = null;
			}

			if (imageViewVolumeLow != null) {
				imageViewVolumeLow.Dispose ();
				imageViewVolumeLow = null;
			}

			if (lblLength != null) {
				lblLength.Dispose ();
				lblLength = null;
			}

			if (lblPosition != null) {
				lblPosition.Dispose ();
				lblPosition = null;
			}

			if (lblScrubbingType != null) {
				lblScrubbingType.Dispose ();
				lblScrubbingType = null;
			}

			if (lblSlideMessage != null) {
				lblSlideMessage.Dispose ();
				lblSlideMessage = null;
			}

			if (pageControl != null) {
				pageControl.Dispose ();
				pageControl = null;
			}

			if (scrollView != null) {
				scrollView.Dispose ();
				scrollView = null;
			}

			if (scrollViewWaveForm != null) {
				scrollViewWaveForm.Dispose ();
				scrollViewWaveForm = null;
			}

			if (sliderPosition != null) {
				sliderPosition.Dispose ();
				sliderPosition = null;
			}

			if (viewEffects != null) {
				viewEffects.Dispose ();
				viewEffects = null;
			}

			if (viewMain != null) {
				viewMain.Dispose ();
				viewMain = null;
			}

			if (viewPageControls != null) {
				viewPageControls.Dispose ();
				viewPageControls = null;
			}

			if (viewPosition != null) {
				viewPosition.Dispose ();
				viewPosition = null;
			}

			if (viewVolume != null) {
				viewVolume.Dispose ();
				viewVolume = null;
			}

			if (lblPresetName != null) {
				lblPresetName.Dispose ();
				lblPresetName = null;
			}

			if (graphView != null) {
				graphView.Dispose ();
				graphView = null;
			}

			if (outputMeter != null) {
				outputMeter.Dispose ();
				outputMeter = null;
			}
		}
	}
}
