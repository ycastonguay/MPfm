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
	[Register ("PlaylistWindowController")]
	partial class PlaylistWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSToolbar toolbar { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTableView tableView { get; set; }

		[Action ("actionNewPlaylist:")]
		partial void actionNewPlaylist (MonoMac.Foundation.NSObject sender);

		[Action ("actionLoadPlaylist:")]
		partial void actionLoadPlaylist (MonoMac.Foundation.NSObject sender);

		[Action ("actionSavePlaylist:")]
		partial void actionSavePlaylist (MonoMac.Foundation.NSObject sender);

		[Action ("actionSaveAsPlaylist:")]
		partial void actionSaveAsPlaylist (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (toolbar != null) {
				toolbar.Dispose ();
				toolbar = null;
			}

			if (tableView != null) {
				tableView.Dispose ();
				tableView = null;
			}
		}
	}

	[Register ("PlaylistWindow")]
	partial class PlaylistWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
