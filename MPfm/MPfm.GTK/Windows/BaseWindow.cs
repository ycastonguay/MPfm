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
	/// Base window for GTK.
	/// </summary>
	public class BaseWindow : Gtk.Window, IBaseView
	{
		protected Action<IBaseView> OnViewReady;
		public Action<IBaseView> OnViewDestroy { get; set; }
		
		public BaseWindow(Gtk.WindowType windowType, Action<IBaseView> onViewReady)
			: base(windowType)
		{
			this.OnViewReady = onViewReady;
		}

        protected void Center()
        {
            Move((Screen.Width - DefaultSize.Width) / 2, (Screen.Height - DefaultSize.Height) / 2);            
        }
		
		protected override bool OnDeleteEvent(Gdk.Event evnt)
		{
			if(OnViewDestroy != null)
				OnViewDestroy.Invoke(this);
			return base.OnDeleteEvent(evnt);
		}
		
		public void ShowView(bool shown)
		{
			if(shown)
			{
				this.ShowAll();
			}
			else
			{
				this.HideAll();
			}
		}
	}
}

