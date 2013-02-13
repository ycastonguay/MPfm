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
using System.Collections.Generic;
using Pango;
using MPfm.MVP;
using MPfm.MVP.Views;

namespace MPfm.GTK.Windows
{
	/// <summary>
	/// Effects window.
	/// </summary>
	public partial class EffectsWindow : BaseWindow, IEffectsView
	{
		/// <summary>
		/// Reference to the main window.
		/// </summary>
		private MainWindow main = null;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MPfm.GTK.EffectsWindow"/> class.
		/// </summary>
		/// <param name='main'>Reference to the main window.</param>
		public EffectsWindow(MainWindow main, Action<IBaseView> onViewReady) : 
				base(Gtk.WindowType.Toplevel, onViewReady)
		{
			this.Build();
			
			// Set reference to main window
			this.main = main;
			
			// Set font properties
			SetFontProperties();
		}
		
		private void SetFontProperties()
		{				
			// Get default font name
			string defaultFontName = this.label1.Style.FontDescription.Family;
			string label1FontName = defaultFontName + " 8";
			this.label1.ModifyFont(FontDescription.FromString(label1FontName));
			this.label2.ModifyFont(FontDescription.FromString(label1FontName));
			this.label3.ModifyFont(FontDescription.FromString(label1FontName));
			this.label4.ModifyFont(FontDescription.FromString(label1FontName));
			this.label5.ModifyFont(FontDescription.FromString(label1FontName));
			this.label6.ModifyFont(FontDescription.FromString(label1FontName));
			this.label7.ModifyFont(FontDescription.FromString(label1FontName));
			this.label8.ModifyFont(FontDescription.FromString(label1FontName));
			this.label9.ModifyFont(FontDescription.FromString(label1FontName));
			this.label10.ModifyFont(FontDescription.FromString(label1FontName));
			this.label11.ModifyFont(FontDescription.FromString(label1FontName));
			this.label12.ModifyFont(FontDescription.FromString(label1FontName));
			this.label13.ModifyFont(FontDescription.FromString(label1FontName));
			this.label14.ModifyFont(FontDescription.FromString(label1FontName));
			this.label15.ModifyFont(FontDescription.FromString(label1FontName));
			this.label16.ModifyFont(FontDescription.FromString(label1FontName));
			this.label17.ModifyFont(FontDescription.FromString(label1FontName));
			this.label18.ModifyFont(FontDescription.FromString(label1FontName));			
			this.label1Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label2Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label3Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label4Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label5Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label6Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label7Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label8Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label9Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label10Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label11Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label12Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label13Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label14Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label15Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label16Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label17Value.ModifyFont(FontDescription.FromString(label1FontName));
			this.label18Value.ModifyFont(FontDescription.FromString(label1FontName));			
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

		#region IEffectsView implementation
		
		public void UpdateFader(int index, float value)
		{
			
		}

		public void UpdatePresetList(IEnumerable<string> presets)
		{
			
		}
		
		#endregion
		
	}
}

