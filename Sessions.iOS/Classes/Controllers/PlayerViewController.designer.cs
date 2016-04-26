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
	[Register ("PlayerViewController")]
	partial class PlayerViewController
	{
		[Outlet]
		Sessions.iOS.Classes.Controls.Buttons.SessionsPlayerButton btnNext { get; set; }

		[Outlet]
        Sessions.iOS.Classes.Controls.Buttons.SessionsPlayerButton btnPlayPause { get; set; }

		[Outlet]
        Sessions.iOS.Classes.Controls.Buttons.SessionsPlayerButton btnPrevious { get; set; }

		[Outlet]
        Sessions.iOS.Classes.Controls.Buttons.SessionsPlayerButton btnRepeat { get; set; }

		[Outlet]
        Sessions.iOS.Classes.Controls.Buttons.SessionsPlayerButton btnShuffle { get; set; }

		[Outlet]
		Sessions.iOS.Classes.Controls.SessionsEqualizerPresetGraphView graphView { get; set; }

		[Outlet]
		UIKit.UIImageView imageViewAlbumArt { get; set; }

		[Outlet]
		UIKit.UIImageView imageViewVolumeHigh { get; set; }

		[Outlet]
		UIKit.UIImageView imageViewVolumeLow { get; set; }

		[Outlet]
		UIKit.UILabel lblLength { get; set; }

		[Outlet]
		UIKit.UILabel lblPosition { get; set; }

		[Outlet]
		UIKit.UILabel lblPresetName { get; set; }

		[Outlet]
		UIKit.UILabel lblScrubbingType { get; set; }

		[Outlet]
		UIKit.UILabel lblSlideMessage { get; set; }

		[Outlet]
		Sessions.iOS.Classes.Controls.SessionsOutputMeterView outputMeter { get; set; }

		[Outlet]
		UIKit.UIPageControl pageControl { get; set; }

		[Outlet]
		UIKit.UIScrollView scrollView { get; set; }

		[Outlet]
		UIKit.UIScrollView scrollViewPlayer { get; set; }

		[Outlet]
		Sessions.iOS.Classes.Controls.SessionsWaveFormScrollView scrollViewWaveForm { get; set; }

		[Outlet]
		Sessions.iOS.Classes.Controls.SessionsSlider sliderPosition { get; set; }

		[Outlet]
		Sessions.iOS.Classes.Controls.SessionsAlbumArtPopupView viewAlbumArt { get; set; }

		[Outlet]
		Sessions.iOS.Classes.Controls.SessionsScaleView viewEffects { get; set; }

		[Outlet]
		UIKit.UIView viewMain { get; set; }

		[Outlet]
		UIKit.UIView viewPageControls { get; set; }

		[Outlet]
		UIKit.UIView viewPlayerButtons { get; set; }

		[Outlet]
		UIKit.UIView viewPlayerButtonsGroup { get; set; }

		[Outlet]
		UIKit.UIView viewPosition { get; set; }

		[Outlet]
		UIKit.UIView viewVolume { get; set; }

		[Action ("actionNext:")]
		partial void actionNext (Foundation.NSObject sender);

		[Action ("actionPause:")]
		partial void actionPause (Foundation.NSObject sender);

		[Action ("actionPrevious:")]
		partial void actionPrevious (Foundation.NSObject sender);

		[Action ("actionRepeat:")]
		partial void actionRepeat (Foundation.NSObject sender);

		[Action ("actionShuffle:")]
		partial void actionShuffle (Foundation.NSObject sender);
		
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

			if (graphView != null) {
				graphView.Dispose ();
				graphView = null;
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

			if (lblPresetName != null) {
				lblPresetName.Dispose ();
				lblPresetName = null;
			}

			if (lblScrubbingType != null) {
				lblScrubbingType.Dispose ();
				lblScrubbingType = null;
			}

			if (lblSlideMessage != null) {
				lblSlideMessage.Dispose ();
				lblSlideMessage = null;
			}

			if (outputMeter != null) {
				outputMeter.Dispose ();
				outputMeter = null;
			}

			if (pageControl != null) {
				pageControl.Dispose ();
				pageControl = null;
			}

			if (scrollView != null) {
				scrollView.Dispose ();
				scrollView = null;
			}

			if (scrollViewPlayer != null) {
				scrollViewPlayer.Dispose ();
				scrollViewPlayer = null;
			}

			if (scrollViewWaveForm != null) {
				scrollViewWaveForm.Dispose ();
				scrollViewWaveForm = null;
			}

			if (sliderPosition != null) {
				sliderPosition.Dispose ();
				sliderPosition = null;
			}

			if (viewAlbumArt != null) {
				viewAlbumArt.Dispose ();
				viewAlbumArt = null;
			}

			if (viewEffects != null) {
				viewEffects.Dispose ();
				viewEffects = null;
			}

			if (viewPlayerButtonsGroup != null) {
				viewPlayerButtonsGroup.Dispose ();
				viewPlayerButtonsGroup = null;
			}

			if (viewMain != null) {
				viewMain.Dispose ();
				viewMain = null;
			}

			if (viewPageControls != null) {
				viewPageControls.Dispose ();
				viewPageControls = null;
			}

			if (viewPlayerButtons != null) {
				viewPlayerButtons.Dispose ();
				viewPlayerButtons = null;
			}

			if (viewPosition != null) {
				viewPosition.Dispose ();
				viewPosition = null;
			}

			if (viewVolume != null) {
				viewVolume.Dispose ();
				viewVolume = null;
			}
		}
	}
}
