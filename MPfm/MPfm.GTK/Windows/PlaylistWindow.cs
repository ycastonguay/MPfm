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

using System;
using MPfm.MVP;
using MPfm.MVP.Views;

namespace MPfm.GTK
{
	/// <summary>
	/// Playlist window.
	/// </summary>
	public partial class PlaylistWindow : BaseWindow, IPlaylistView
	{
		/// <summary>
		/// Reference to the main window.
		/// </summary>
		private MainWindow main = null;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MPfm.GTK.PlaylistWindow"/> class.
		/// </summary>
		/// <param name='main'>Reference to the main window.</param>
		public PlaylistWindow(MainWindow main, Action<IBaseView> onViewReady) : 
				base(Gtk.WindowType.Toplevel, onViewReady)
		{
			this.Build();
			
			// Set reference to main window
			this.main = main;
			
			// Set focus to something else than the toolbar
			treePlaylistBrowser.GrabFocus();
		}
		
		/// <summary>
		/// Raises the delete event (when the form is closing).
		/// Prevents the form from closing by hiding it instead.
		/// </summary>
		/// <param name='o'>Object</param>
		/// <param name='args'>Event arguments</param>
		protected void OnDeleteEvent(object o, Gtk.DeleteEventArgs args)
		{
			// Prevent window from closing
			args.RetVal = true;
			
			// Hide window instead
			this.HideAll();
		}

		protected void OnActionNewPlaylistActivated (object sender, System.EventArgs e)
		{
		}

		protected void OnActionOpenPlaylistActivated (object sender, System.EventArgs e)
		{
		}

		protected void OnActionSavePlaylistActivated (object sender, System.EventArgs e)
		{
		}

		protected void OnActionSavePlaylistAsActivated (object sender, System.EventArgs e)
		{
		}

	}
}

