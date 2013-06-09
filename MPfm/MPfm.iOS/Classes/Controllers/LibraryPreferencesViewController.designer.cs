// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace MPfm.iOS
{
	[Register ("LibraryPreferencesViewController")]
	partial class LibraryPreferencesViewController
	{
		[Outlet]
		MonoTouch.UIKit.UILabel lblTitle { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblEnableSyncListener { get; set; }

		[Outlet]
		MonoTouch.UIKit.UISwitch switchEnableSyncListener { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextField txtSyncListenerPort { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel lblSyncListenerPort { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnResetLibrary { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton btnUpdateLibrary { get; set; }

		[Action ("actionResetLibrary:")]
		partial void actionResetLibrary (MonoTouch.Foundation.NSObject sender);

		[Action ("actionUpdateLibrary:")]
		partial void actionUpdateLibrary (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}

			if (lblEnableSyncListener != null) {
				lblEnableSyncListener.Dispose ();
				lblEnableSyncListener = null;
			}

			if (switchEnableSyncListener != null) {
				switchEnableSyncListener.Dispose ();
				switchEnableSyncListener = null;
			}

			if (txtSyncListenerPort != null) {
				txtSyncListenerPort.Dispose ();
				txtSyncListenerPort = null;
			}

			if (lblSyncListenerPort != null) {
				lblSyncListenerPort.Dispose ();
				lblSyncListenerPort = null;
			}

			if (btnResetLibrary != null) {
				btnResetLibrary.Dispose ();
				btnResetLibrary = null;
			}

			if (btnUpdateLibrary != null) {
				btnUpdateLibrary.Dispose ();
				btnUpdateLibrary = null;
			}
		}
	}
}
