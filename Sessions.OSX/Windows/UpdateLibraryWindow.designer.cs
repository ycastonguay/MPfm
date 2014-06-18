// Copyright © 2011-2013 Yanick Castonguay
//
// This file is part of MPfm.
//
// MPfm is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// MPfm is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with MPfm. If not, see <http://www.gnu.org/licenses/>.

using MonoMac.Foundation;

namespace MPfm.OSX
{
	[Register ("UpdateLibraryWindowController")]
	partial class UpdateLibraryWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSProgressIndicator progressBar { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblPercentageDone { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnCancel { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnSaveLog { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton btnOK { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextView textViewErrorLog { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField lblTitle { get; set; }

		[Outlet]
        MonoMac.AppKit.NSTextField lblSubtitle { get; set; }

		[Outlet]
		MonoMac.AppKit.NSScrollView scrollViewErrorLog { get; set; }

		[Action ("btnOK_Click:")]
		partial void btnOK_Click (MonoMac.Foundation.NSObject sender);

		[Action ("btnCancel_Click:")]
		partial void btnCancel_Click (MonoMac.Foundation.NSObject sender);

		[Action ("btnSaveLog_Click:")]
		partial void btnSaveLog_Click (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (progressBar != null) {
				progressBar.Dispose ();
				progressBar = null;
			}

			if (lblPercentageDone != null) {
				lblPercentageDone.Dispose ();
				lblPercentageDone = null;
			}

			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}

			if (btnSaveLog != null) {
				btnSaveLog.Dispose ();
				btnSaveLog = null;
			}

			if (btnOK != null) {
				btnOK.Dispose ();
				btnOK = null;
			}

			if (textViewErrorLog != null) {
				textViewErrorLog.Dispose ();
				textViewErrorLog = null;
			}

			if (lblTitle != null) {
				lblTitle.Dispose ();
				lblTitle = null;
			}

			if (lblSubtitle != null) {
				lblSubtitle.Dispose ();
				lblSubtitle = null;
			}

			if (scrollViewErrorLog != null) {
				scrollViewErrorLog.Dispose ();
				scrollViewErrorLog = null;
			}
		}
	}

	[Register ("UpdateLibraryWindow")]
	partial class UpdateLibraryWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
