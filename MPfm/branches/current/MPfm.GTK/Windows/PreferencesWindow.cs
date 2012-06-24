//  
//  SettingsWindow.cs
//  
//  Author:
//       Yanick Castonguay <ycastonguay@mp4m.org>
// 
//  Copyright (c) 2012 2011 - 2012 Yanick Castonguay
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;

namespace MPfm.GTK
{
	/// <summary>
	/// Settings window.
	/// </summary>
	public partial class PreferencesWindow : Gtk.Window
	{
		/// <summary>
		/// Reference to the main window.
		/// </summary>
		private MainWindow main = null;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MPfm.GTK.PreferencesWindow"/> class.
		/// </summary>
		/// <param name='main'>Reference to the main window.</param>
		public PreferencesWindow (MainWindow main) : 
				base(Gtk.WindowType.Toplevel)
		{
			this.Build ();
			
			// Set reference to main window
			this.main = main;
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
	}
}

