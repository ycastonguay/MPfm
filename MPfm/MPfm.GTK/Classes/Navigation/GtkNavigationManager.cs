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
using MPfm.MVP;
using MPfm.MVP.Navigation;
using MPfm.MVP.Views;

namespace MPfm.GTK.Navigation
{
	/// <summary>
	/// Navigation manager for Gtk.
	/// </summary>
	public class GtkNavigationManager : NavigationManager
	{
		public override ISplashView CreateSplashView()
		{
			ISplashView view = null;
			Gtk.Application.Invoke(delegate {
				view = base.CreateSplashView();
			});
			return view;
		}
		
		public override IMainView CreateMainView()
		{
			IMainView view = null;
			Gtk.Application.Invoke(delegate {
				view = base.CreateMainView();
			});		
			return view;
		}
		
		public override IPreferencesView CreatePreferencesView()
		{
			IPreferencesView view = null;
			Gtk.Application.Invoke(delegate {
				view = base.CreatePreferencesView();
			});		
			return view;
		}
	}
}

