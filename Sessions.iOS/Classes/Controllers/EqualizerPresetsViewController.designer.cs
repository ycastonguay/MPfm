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
	[Register ("EqualizerPresetsViewController")]
	partial class EqualizerPresetsViewController
	{
		[Outlet]
		UIKit.UILabel lblBypass { get; set; }

		[Outlet]
		UIKit.UILabel lblMasterVolume { get; set; }

		[Outlet]
		Sessions.iOS.Classes.Controls.SessionsOutputMeterView outputMeter { get; set; }

		[Outlet]
		UIKit.UISwitch switchBypass { get; set; }

		[Outlet]
		UIKit.UITableView tableView { get; set; }

		[Outlet]
		UIKit.UIView viewOptions { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
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
