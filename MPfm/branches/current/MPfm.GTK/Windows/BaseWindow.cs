//  
//  BaseWindow.cs
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
using System.Collections.Generic;
using Pango;
using MPfm.MVP;

namespace MPfm.GTK
{
	/// <summary>
	/// Base window for GTK.
	/// </summary>
	public class BaseWindow : Gtk.Window, IBaseView
	{
		protected Action<IBaseView> OnViewReady;
		public Action OnViewDestroy { get; set; }
		
		public BaseWindow(Gtk.WindowType windowType, Action<IBaseView> onViewReady)
			: base(windowType)
		{
			this.OnViewReady = onViewReady;
		}
		
		protected override bool OnDeleteEvent(Gdk.Event evnt)
		{
			if(OnViewDestroy != null)
				OnViewDestroy.Invoke();
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

