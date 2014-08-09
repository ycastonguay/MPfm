// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;
using System.CodeDom.Compiler;

namespace Sessions.OSX
{
	[Register ("SelectAlbumArtWindowController")]
	partial class SelectAlbumArtWindowController
	{
		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsButton btnApplyToAllSongs { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsButton btnApplyToThisSongOnly { get; set; }

		[Outlet]
		Sessions.OSX.Classes.Controls.SessionsButton btnCancel { get; set; }

		[Outlet]
		MonoMac.AppKit.NSCollectionView collectionView { get; set; }

		[Action ("actionApplyToAllSongs:")]
		partial void actionApplyToAllSongs (MonoMac.Foundation.NSObject sender);

		[Action ("actionApplyToThisSongOnly:")]
		partial void actionApplyToThisSongOnly (MonoMac.Foundation.NSObject sender);

		[Action ("actionCancel:")]
		partial void actionCancel (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (collectionView != null) {
				collectionView.Dispose ();
				collectionView = null;
			}

			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}

			if (btnApplyToAllSongs != null) {
				btnApplyToAllSongs.Dispose ();
				btnApplyToAllSongs = null;
			}

			if (btnApplyToThisSongOnly != null) {
				btnApplyToThisSongOnly.Dispose ();
				btnApplyToThisSongOnly = null;
			}
		}
	}

	[Register ("SelectAlbumArtWindow")]
	partial class SelectAlbumArtWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
