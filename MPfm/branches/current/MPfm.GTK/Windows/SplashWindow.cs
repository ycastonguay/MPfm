//  
//  SplashWindow.cs
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
using MPfm.MVP;

namespace MPfm.GTK
{
	public partial class SplashWindow : Gtk.Window, ISplashView
	{
		public SplashWindow() : 
		base(Gtk.WindowType.Toplevel)
		{
				this.Build();
		}

		#region ISplashView implementation

		public void RefreshStatus(string message)
		{
		}

		public void InitDone()
		{
		}

		#endregion
	}
}