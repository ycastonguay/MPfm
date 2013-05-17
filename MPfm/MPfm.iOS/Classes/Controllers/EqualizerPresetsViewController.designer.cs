// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace MPfm.iOS
{
	[Register ("EqualizerPresetsViewController")]
	partial class EqualizerPresetsViewController
	{
		[Outlet]
		MonoTouch.UIKit.UISlider sliderMasterVolume { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblMasterVolume { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITableView tableView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblBypass { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISwitch switchBypass { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView viewOptions { get; set; }

		[Outlet]
		MPfm.iOS.Classes.Controls.MPfmOutputMeterView outputMeter { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (sliderMasterVolume != null) {
				sliderMasterVolume.Dispose ();
				sliderMasterVolume = null;
			}

			if (lblMasterVolume != null) {
				lblMasterVolume.Dispose ();
				lblMasterVolume = null;
			}

			if (tableView != null) {
				tableView.Dispose ();
				tableView = null;
			}

			if (lblBypass != null) {
				lblBypass.Dispose ();
				lblBypass = null;
			}

			if (switchBypass != null) {
				switchBypass.Dispose ();
				switchBypass = null;
			}

			if (viewOptions != null) {
				viewOptions.Dispose ();
				viewOptions = null;
			}

			if (outputMeter != null) {
				outputMeter.Dispose ();
				outputMeter = null;
			}
		}
	}
}
